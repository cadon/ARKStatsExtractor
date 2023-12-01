using System;
using System.IO;
using System.Threading;
using ARKBreedingStats.Library;
using ARKBreedingStats.values;
using Newtonsoft.Json;

namespace ARKBreedingStats.importExportGun
{
    /// <summary>
    /// Imports creature files created with the export gun (mod).
    /// </summary>
    internal static class ImportExportGun
    {
        /// <summary>
        /// Import file created with the export gun (mod).
        /// </summary>
        public static Creature ImportCreature(string filePath, out string resultText, out string serverMultipliersHash)
        {
            resultText = null;
            serverMultipliersHash = null;
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return null;

            const int tryLoadCount = 3;
            const int waitAfterFailedLoadMs = 200;

            for (int tryIndex = 0; tryIndex < tryLoadCount; tryIndex++)
            {
                try
                {
                    string jsonText = null;
                    switch (Path.GetExtension(filePath))
                    {
                        case ".sav":
                            jsonText = ReadExportFile.ReadFile(filePath, "DinoExportGunSave_C", out resultText);
                            break;
                        case ".json":
                            jsonText = File.ReadAllText(filePath);
                            break;
                    }

                    return ImportCreatureFromJson(jsonText, resultText, out resultText, out serverMultipliersHash);

                }
                catch (IOException) when (tryIndex < tryLoadCount - 1)
                {
                    // file is probably still being written. Try up to 3 times again after some time.
                    Thread.Sleep(waitAfterFailedLoadMs * (1 << tryIndex));
                }
                catch (Exception ex)
                {
                    resultText = $"Error when importing file {filePath}: {ex.Message}";
                    return null;
                }
            }

            return null;
        }

        public static Creature ImportCreatureFromJson(string jsonText, string resultSoFar, out string resultText, out string serverMultipliersHash, string filePath = null)
        {
            resultText = resultSoFar;
            serverMultipliersHash = null;
            if (string.IsNullOrEmpty(jsonText))
            {
                resultText = $"Error when importing file {filePath}: {resultText}";
                return null;
            }
            var exportedCreature = JsonConvert.DeserializeObject<ExportGunCreatureFile>(jsonText);
            if (exportedCreature == null)
            {
                resultText = "jsonText couldn't be deserialized";
                return null;
            }

            serverMultipliersHash = exportedCreature.ServerMultipliersHash;

            return ConvertExportGunToCreature(exportedCreature, out resultText);
        }

        private static Creature ConvertExportGunToCreature(ExportGunCreatureFile ec, out string error)
        {
            error = null;
            if (ec == null) return null;

            var species = Values.V.SpeciesByBlueprint(ec.BlueprintPath, true);
            if (species == null)
            {
                error = $"blueprintpath {ec.BlueprintPath} couldn't be found, maybe you need to load a mod values file.";
                return null;
            }

            var wildLevels = new int[Stats.StatsCount];
            var domLevels = new int[Stats.StatsCount];
            var mutLevels = new int[Stats.StatsCount];
            var si = 0;
            foreach (var s in ec.Stats)
            {
                wildLevels[si] = s.Wild;
                domLevels[si] = s.Tamed;
                mutLevels[si] = s.Mutated;
                si++;
            }

            var arkId = Utils.ConvertArkIdsToLongArkId(ec.DinoId1Int, ec.DinoId2Int);

            var isWild = string.IsNullOrEmpty(ec.DinoName)
                         && string.IsNullOrEmpty(ec.TribeName)
                         && string.IsNullOrEmpty(ec.TamerString)
                         && string.IsNullOrEmpty(ec.OwningPlayerName)
                         && string.IsNullOrEmpty(ec.ImprinterName)
                         && ec.OwningPlayerID == 0
                         ;

            var c = new Creature(species, ec.DinoName, !string.IsNullOrEmpty(ec.OwningPlayerName) ? ec.OwningPlayerName : !string.IsNullOrEmpty(ec.ImprinterName) ? ec.ImprinterName : ec.TamerString,
                ec.TribeName, species.noGender ? Sex.Unknown : ec.IsFemale ? Sex.Female : Sex.Male, wildLevels, domLevels, mutLevels,
                isWild ? -3 : ec.TameEffectiveness, !string.IsNullOrEmpty(ec.ImprinterName), ec.DinoImprintingQuality,
                CreatureCollection.CurrentCreatureCollection?.wildLevelStep)
            {
                ArkId = arkId,
                guid = Utils.ConvertArkIdToGuid(arkId),
                ArkIdImported = true,
                ArkIdInGame = Utils.ConvertImportedArkIdToIngameVisualization(arkId),
                colors = ec.ColorIds,
                Maturation = ec.BabyAge,
                mutationsMaternal = ec.RandomMutationsFemale,
                mutationsPaternal = ec.RandomMutationsMale,
                generation = -1 // indication that it has to be recalculated
            };

            c.RecalculateCreatureValues(CreatureCollection.CurrentCreatureCollection?.wildLevelStep);
            if (ec.NextAllowedMatingTimeDuration > 0)
                c.cooldownUntil = DateTime.Now.AddSeconds(ec.NextAllowedMatingTimeDuration);
            if (ec.MutagenApplied)
                c.flags |= CreatureFlags.MutagenApplied;
            if (ec.Neutered)
                c.flags |= CreatureFlags.Neutered;
            if (ec.Ancestry != null)
            {
                if (ec.Ancestry.FemaleDinoId1Int != 0 || ec.Ancestry.FemaleDinoId2Int != 0)
                    c.motherGuid =
                        Utils.ConvertArkIdToGuid(Utils.ConvertArkIdsToLongArkId(ec.Ancestry.FemaleDinoId1Int,
                            ec.Ancestry.FemaleDinoId2Int));
                if (ec.Ancestry.MaleDinoId1Int != 0 || ec.Ancestry.MaleDinoId2Int != 0)
                    c.fatherGuid =
                        Utils.ConvertArkIdToGuid(Utils.ConvertArkIdsToLongArkId(ec.Ancestry.MaleDinoId1Int,
                            ec.Ancestry.MaleDinoId2Int));
            }

            return c;
        }

        public static ExportGunCreatureFile ConvertCreatureToExportGunFile(Creature c, out string error)
        {
            error = null;
            if (c == null) return null;

            var stats = new Stat[Stats.StatsCount];

            for (var si = 0; si < Stats.StatsCount; si++)
            {
                stats[si] = new Stat
                {
                    Wild = c.levelsWild?[si] ?? 0,
                    Tamed = c.levelsDom?[si] ?? 0,
                    Mutated = c.levelsMutated?[si] ?? 0,
                    Value = (float)c.valuesDom[si]
                };
            }

            var (id1, id2) = Utils.ConvertArkId64ToArkIds32(c.ArkId);

            Ancestry ancestry = null;
            if (c.motherGuid != Guid.Empty || c.fatherGuid != Guid.Empty)
            {
                ancestry = new Ancestry();
                if (c.motherGuid != Guid.Empty)
                    (ancestry.FemaleDinoId1Int, ancestry.FemaleDinoId2Int) =
                        Utils.ConvertArkId64ToArkIds32(Utils.ConvertCreatureGuidToArkId(c.motherGuid));
                if (c.fatherGuid != Guid.Empty)
                    (ancestry.MaleDinoId1Int, ancestry.MaleDinoId2Int) =
                        Utils.ConvertArkId64ToArkIds32(Utils.ConvertCreatureGuidToArkId(c.fatherGuid));
            }

            var ec = new ExportGunCreatureFile
            {
                BlueprintPath = c.speciesBlueprint,
                Stats = stats,
                DinoId1Int = id1,
                DinoId2Int = id2,
                DinoName = c.name,
                ImprinterName = c.imprinterName,
                Ancestry = ancestry,
                BabyAge = (float)c.Maturation,
                BaseCharacterLevel = c.Level,
                ColorIds = c.colors,
                DinoImprintingQuality = (float)c.imprintingBonus,
                IsFemale = c.sex == Sex.Female,
                MutagenApplied = c.flags.HasFlag(CreatureFlags.MutagenApplied),
                SpeciesName = c.Species?.name,
                Neutered = c.flags.HasFlag(CreatureFlags.Neutered),
                RandomMutationsFemale = c.mutationsMaternal,
                RandomMutationsMale = c.mutationsPaternal,
                TameEffectiveness = (float)c.tamingEff,
                TamerString = c.owner,
                TribeName = c.tribe,
                NextAllowedMatingTimeDuration = c.cooldownUntil == null ? 0 : (c.cooldownUntil.Value - DateTime.Now).Seconds
            };

            return ec;
        }

        /// <summary>
        /// Import server multipliers file from the export gun mod.
        /// </summary>
        public static bool ImportServerMultipliers(CreatureCollection cc, string filePath, string newServerMultipliersHash, out string resultText)
        {
            var exportedServerMultipliers = ReadServerMultipliers(filePath, out resultText);
            if (exportedServerMultipliers == null) return false;
            return SetServerMultipliers(cc, exportedServerMultipliers, newServerMultipliersHash);
        }

        /// <summary>
        /// Import server multipliers file from the export gun mod.
        /// </summary>
        public static bool ImportServerMultipliersFromJson(CreatureCollection cc, string jsonServerMultipliers, string newServerMultipliersHash, out string resultText)
        {
            var exportedServerMultipliers = ReadServerMultipliersFromJson(jsonServerMultipliers, null, out resultText);
            if (exportedServerMultipliers == null) return false;
            return SetServerMultipliers(cc, exportedServerMultipliers, newServerMultipliersHash);
        }

        internal static ExportGunServerFile ReadServerMultipliers(string filePath, out string resultText)
        {
            resultText = null;
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return null;

            const int tryLoadCount = 3;
            const int waitAfterFailedLoadMs = 200;

            for (int tryIndex = 0; tryIndex < tryLoadCount; tryIndex++)
            {
                try
                {
                    string jsonText = null;
                    string game = null;
                    switch (Path.GetExtension(filePath))
                    {
                        case ".sav":
                            jsonText = ReadExportFile.ReadFile(filePath, "DinoExportGunServerSave_C", out resultText);
                            game = "ASE";
                            break;
                        case ".json":
                            jsonText = File.ReadAllText(filePath);
                            game = "ASA";
                            break;
                    }

                    return ReadServerMultipliersFromJson(jsonText, resultText, out resultText, game, filePath);
                }
                catch (IOException) when (tryIndex < tryLoadCount - 1)
                {
                    // file is probably still being written. Try up to 3 times again after some time.
                    Thread.Sleep(waitAfterFailedLoadMs * (1 << tryIndex));
                }
                catch (Exception ex)
                {
                    resultText = $"Error when importing file {filePath}: {ex.Message}";
                    return null;
                }
            }

            return null;
        }

        public static ExportGunServerFile ReadServerMultipliersFromJson(string jsonText, string resultSoFar, out string resultText, string game = null, string filePath = null)
        {
            resultText = resultSoFar;
            var exportedServerMultipliers = JsonConvert.DeserializeObject<ExportGunServerFile>(jsonText);
            if (string.IsNullOrEmpty(jsonText))
            {
                resultText = $"Error when importing file {filePath}: {resultText}";
                return null;
            }

            if (string.IsNullOrEmpty(exportedServerMultipliers.Game))
                exportedServerMultipliers.Game = game;
            resultText = $"Server multipliers imported from {filePath}";
            return exportedServerMultipliers;
        }

        internal static bool SetServerMultipliers(CreatureCollection cc, ExportGunServerFile esm, string newServerMultipliersHash)
        {
            if (cc == null) return false;

            const int roundToDigits = 6;

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                cc.serverMultipliers.statMultipliers[s][Stats.IndexTamingAdd] = Math.Round(esm.TameAdd[s], roundToDigits);
                cc.serverMultipliers.statMultipliers[s][Stats.IndexTamingMult] = Math.Round(esm.TameAff[s], roundToDigits);
                cc.serverMultipliers.statMultipliers[s][Stats.IndexLevelWild] = Math.Round(esm.WildLevel[s], roundToDigits);
                cc.serverMultipliers.statMultipliers[s][Stats.IndexLevelDom] = Math.Round(esm.TameLevel[s], roundToDigits);
            }
            cc.maxWildLevel = esm.MaxWildLevel;
            cc.maxServerLevel = esm.DestroyTamesOverLevelClamp;
            cc.serverMultipliers.TamingSpeedMultiplier = Math.Round(esm.TamingSpeedMultiplier, roundToDigits);
            cc.serverMultipliers.DinoCharacterFoodDrainMultiplier = Math.Round(esm.DinoCharacterFoodDrainMultiplier, roundToDigits);
            cc.serverMultipliers.WildDinoTorporDrainMultiplier = Math.Round(esm.WildDinoTorporDrainMultiplier, roundToDigits);
            cc.serverMultipliers.MatingSpeedMultiplier = Math.Round(esm.MatingSpeedMultiplier, roundToDigits);
            cc.serverMultipliers.MatingIntervalMultiplier = Math.Round(esm.MatingIntervalMultiplier, roundToDigits);
            cc.serverMultipliers.EggHatchSpeedMultiplier = Math.Round(esm.EggHatchSpeedMultiplier, roundToDigits);
            cc.serverMultipliers.BabyMatureSpeedMultiplier = Math.Round(esm.BabyMatureSpeedMultiplier, roundToDigits);
            cc.serverMultipliers.BabyCuddleIntervalMultiplier = Math.Round(esm.BabyCuddleIntervalMultiplier, roundToDigits);
            cc.serverMultipliers.BabyImprintAmountMultiplier = Math.Round(esm.BabyImprintAmountMultiplier, roundToDigits);
            cc.serverMultipliers.BabyImprintingStatScaleMultiplier = Math.Round(esm.BabyImprintingStatScaleMultiplier, roundToDigits);
            cc.serverMultipliers.BabyFoodConsumptionSpeedMultiplier = Math.Round(esm.BabyFoodConsumptionSpeedMultiplier, roundToDigits);
            cc.serverMultipliers.TamedDinoCharacterFoodDrainMultiplier = Math.Round(esm.TamedDinoCharacterFoodDrainMultiplier, roundToDigits);
            cc.serverMultipliers.AllowSpeedLeveling = esm.AllowSpeedLeveling;
            cc.serverMultipliers.AllowFlyerSpeedLeveling = esm.AllowFlyerSpeedLeveling;
            cc.singlePlayerSettings = esm.UseSingleplayerSettings;
            cc.Game = esm.Game;

            cc.ServerMultipliersHash = newServerMultipliersHash;

            return true;
        }
    }
}
