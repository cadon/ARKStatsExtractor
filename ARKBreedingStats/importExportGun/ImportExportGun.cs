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
        /// Load creature from file created with the export gun (mod).
        /// Supports .sav files (ASE) and .json files (ASA).
        /// The out parameter statValues contains the stat values of the export file.
        /// </summary>
        public static Creature LoadCreature(string filePath, out string resultText, out string serverMultipliersHash,
            out double[] statValues, bool allowUnknownSpecies = false)
        {
            var exportedCreature = LoadCreatureFile(filePath, out resultText, out serverMultipliersHash);

            if (exportedCreature == null)
            {
                statValues = null;
                return null;
            }

            var creature = ConvertExportGunToCreature(exportedCreature, out resultText, out statValues, allowUnknownSpecies);
            if (creature != null)
                creature.domesticatedAt = File.GetLastWriteTime(filePath);
            return creature;
        }

        /// <summary>
        /// Load exportGunCreatureFile from file created with the export gun (mod).
        /// Supports .sav files (ASE) and .json files (ASA).
        /// </summary>
        public static ExportGunCreatureFile LoadCreatureFile(string filePath, out string resultText, out string serverMultipliersHash)
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

                    var creature = LoadExportGunCreatureFromJson(jsonText, resultText, out resultText, out serverMultipliersHash, filePath);

                    return creature;
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

        public static Creature LoadCreatureFromExportGunJson(string jsonText, out string resultText, out string serverMultipliersHash, string filePath = null, bool allowUnknownSpecies = false)
        {
            var exportGunFile = LoadExportGunCreatureFromJson(jsonText, null, out resultText,
                out serverMultipliersHash, filePath);
            if (exportGunFile == null)
            {
                return null;
            }

            return ConvertExportGunToCreature(exportGunFile, out resultText, out double[] statValues, allowUnknownSpecies);
        }

        public static ExportGunCreatureFile LoadExportGunCreatureFromJson(string jsonText, string resultSoFar, out string resultText, out string serverMultipliersHash, string filePath = null)
        {
            resultText = resultSoFar;
            serverMultipliersHash = null;
            if (string.IsNullOrEmpty(jsonText))
            {
                resultText = $"Error when importing file {filePath}: file is empty. {resultText}";
                return null;
            }
            var exportedCreature = JsonConvert.DeserializeObject<ExportGunCreatureFile>(jsonText);
            if (exportedCreature == null)
            {
                resultText = "jsonText couldn't be deserialized";
                return null;
            }

            if (string.IsNullOrEmpty(exportedCreature.BlueprintPath))
            {
                resultText = $"file {filePath} contains no blueprint path, it's probably not a creature file (could be a server multipliers file).";
                return null;
            }

            serverMultipliersHash = exportedCreature.ServerMultipliersHash;
            return exportedCreature;
        }

        private static Creature ConvertExportGunToCreature(ExportGunCreatureFile ec, out string error, out double[] statValues, bool allowUnknownSpecies = false)
        {
            error = null;
            statValues = null;
            if (ec == null) return null;

            var species = Values.V.SpeciesByBlueprint(ec.BlueprintPath, true);
            if (species == null)
            {
                error = $"Unknown species. The blueprint path {ec.BlueprintPath} couldn't be found, maybe you need to load a mod values file.";
                if (!allowUnknownSpecies)
                    return null;
            }

            var wildLevels = new int[Stats.StatsCount];
            var domLevels = new int[Stats.StatsCount];
            var mutLevels = new int[Stats.StatsCount];
            statValues = new double[Stats.StatsCount];
            var si = 0;
            foreach (var s in ec.Stats)
            {
                wildLevels[si] = s.Wild;
                domLevels[si] = s.Tamed;
                mutLevels[si] = s.Mutated;
                statValues[si] = s.Value + (Stats.IsPercentage(si) ? 1 : 0);
                si++;
            }

            var arkId = Utils.ConvertArkIdsToLongArkId(ec.DinoId1Int, ec.DinoId2Int);

            var c = new Creature(species, ec.DinoName, ec.Owner(), ec.TribeName, species?.noGender != false ? Sex.Unknown : ec.IsFemale ? Sex.Female : Sex.Male,
                wildLevels, domLevels, mutLevels, ec.IsWild() ? -3 : ec.TameEffectiveness, ec.IsBred(), ec.DinoImprintingQuality,
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
                    Value = (float)(c.valuesDom[si] - (Stats.IsPercentage(si) ? 1 : 0))
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
            return SetCollectionMultipliers(cc, exportedServerMultipliers, newServerMultipliersHash);
        }

        /// <summary>
        /// Import server multipliers file from the export gun mod.
        /// </summary>
        public static bool ImportServerMultipliersFromJson(CreatureCollection cc, string jsonServerMultipliers, string newServerMultipliersHash, out string resultText)
        {
            var exportedServerMultipliers = ReadServerMultipliersFromJson(jsonServerMultipliers, null, out resultText);
            if (exportedServerMultipliers == null) return false;
            return SetCollectionMultipliers(cc, exportedServerMultipliers, newServerMultipliersHash);
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
            if (string.IsNullOrEmpty(jsonText))
            {
                resultText = $"The file is empty and cannot be imported: {filePath}{Environment.NewLine}{resultText}";
                return null;
            }
            var exportedServerMultipliers = JsonConvert.DeserializeObject<ExportGunServerFile>(jsonText);

            // check if the file is a valid server settings file
            if (exportedServerMultipliers?.WildLevel == null
                || exportedServerMultipliers.TameLevel == null
                || exportedServerMultipliers.TameAdd == null
                || exportedServerMultipliers.TameAff == null
               )
            {
                resultText = $"The file is not a valid server multipliers file and cannot be imported: {filePath}{Environment.NewLine}{resultText}";
                return null;
            }

            if (string.IsNullOrEmpty(exportedServerMultipliers.Game))
                exportedServerMultipliers.Game = game;
            resultText = $"Server multipliers imported from {filePath}";
            return exportedServerMultipliers;
        }

        internal static bool SetCollectionMultipliers(CreatureCollection cc, ExportGunServerFile esm, string newServerMultipliersHash)
        {
            if (cc?.serverMultipliers == null
                || esm?.TameAdd == null
                || esm.TameAff == null
                || esm.WildLevel == null
                || esm.TameLevel == null
                )
                return false; // invalid server multipliers

            SetServerMultipliers(cc.serverMultipliers, esm);

            cc.maxWildLevel = (int)Math.Ceiling(esm.MaxWildLevel);
            cc.maxServerLevel = esm.DestroyTamesOverLevelClamp;
            cc.Game = esm.Game;

            cc.ServerMultipliersHash = newServerMultipliersHash;

            return true;
        }

        /// <summary>
        /// Sets the properties of the exportGunServerFile to the passed ServerMultipliers.
        /// </summary>
        /// <param name="sm">The properties of this object are set</param>
        /// <param name="esm">The properties of this object are used</param>
        internal static bool SetServerMultipliers(ServerMultipliers sm, ExportGunServerFile esm)
        {
            if (sm == null
                || esm?.TameAdd == null
                || esm.TameAff == null
                || esm.WildLevel == null
                || esm.TameLevel == null
                )
                return false; // invalid server multipliers

            const int roundToDigits = 6;

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                sm.statMultipliers[s][ServerMultipliers.IndexTamingAdd] = Math.Round(esm.TameAdd[s], roundToDigits);
                sm.statMultipliers[s][ServerMultipliers.IndexTamingMult] = Math.Round(esm.TameAff[s], roundToDigits);
                sm.statMultipliers[s][ServerMultipliers.IndexLevelWild] = Math.Round(esm.WildLevel[s], roundToDigits);
                sm.statMultipliers[s][ServerMultipliers.IndexLevelDom] = Math.Round(esm.TameLevel[s], roundToDigits);
            }
            sm.TamingSpeedMultiplier = Math.Round(esm.TamingSpeedMultiplier, roundToDigits);
            sm.DinoCharacterFoodDrainMultiplier = Math.Round(esm.DinoCharacterFoodDrainMultiplier, roundToDigits);
            sm.WildDinoCharacterFoodDrainMultiplier = Math.Round(esm.WildDinoCharacterFoodDrainMultiplier, roundToDigits);
            sm.TamedDinoCharacterFoodDrainMultiplier = Math.Round(esm.TamedDinoCharacterFoodDrainMultiplier, roundToDigits);
            sm.WildDinoTorporDrainMultiplier = Math.Round(esm.WildDinoTorporDrainMultiplier, roundToDigits);
            sm.MatingSpeedMultiplier = Math.Round(esm.MatingSpeedMultiplier, roundToDigits);
            sm.MatingIntervalMultiplier = Math.Round(esm.MatingIntervalMultiplier, roundToDigits);
            sm.EggHatchSpeedMultiplier = Math.Round(esm.EggHatchSpeedMultiplier, roundToDigits);
            sm.BabyMatureSpeedMultiplier = Math.Round(esm.BabyMatureSpeedMultiplier, roundToDigits);
            sm.BabyCuddleIntervalMultiplier = Math.Round(esm.BabyCuddleIntervalMultiplier, roundToDigits);
            sm.BabyImprintAmountMultiplier = Math.Round(esm.BabyImprintAmountMultiplier, roundToDigits);
            sm.BabyImprintingStatScaleMultiplier = Math.Round(esm.BabyImprintingStatScaleMultiplier, roundToDigits);
            sm.BabyFoodConsumptionSpeedMultiplier = Math.Round(esm.BabyFoodConsumptionSpeedMultiplier, roundToDigits);
            sm.AllowSpeedLeveling = esm.AllowSpeedLeveling;
            sm.AllowFlyerSpeedLeveling = esm.AllowFlyerSpeedLeveling;
            sm.SinglePlayerSettings = esm.UseSingleplayerSettings;

            return true;
        }
    }
}
