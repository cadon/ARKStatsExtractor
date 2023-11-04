using System;
using System.IO;
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

            try
            {
                var jsonText = ReadExportFile.ReadFile(filePath, "DinoExportGunSave_C", out resultText);
                if (jsonText == null)
                {
                    resultText = $"Error when importing file {filePath}: {resultText}";
                    return null;
                }

                var exportedCreature = JsonConvert.DeserializeObject<ExportGunCreatureFile>(jsonText);
                if (exportedCreature == null) return null;

                serverMultipliersHash = exportedCreature.ServerMultipliersHash;

                return ConvertExportGunToCreature(exportedCreature, out resultText);
            }
            catch (Exception ex)
            {
                resultText = $"Error when importing file {filePath}: {ex.Message}";
            }

            return null;
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

            var arkId = Utils.ConvertArkIdsToLongArkId(ec.DinoID1, ec.DinoID2);

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
                colors = ec.ColorSetIndices,
                Maturation = ec.BabyAge,
                mutationsMaternal = ec.RandomMutationsFemale,
                mutationsPaternal = ec.RandomMutationsMale
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
                if (ec.Ancestry.FemaleDinoId1 != 0 || ec.Ancestry.FemaleDinoId2 != 0)
                    c.motherGuid =
                        Utils.ConvertArkIdToGuid(Utils.ConvertArkIdsToLongArkId(ec.Ancestry.FemaleDinoId1,
                            ec.Ancestry.FemaleDinoId2));
                if (ec.Ancestry.MaleDinoId1 != 0 || ec.Ancestry.MaleDinoId2 != 0)
                    c.fatherGuid =
                        Utils.ConvertArkIdToGuid(Utils.ConvertArkIdsToLongArkId(ec.Ancestry.MaleDinoId1,
                            ec.Ancestry.MaleDinoId2));
            }

            return c;
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

        internal static ExportGunServerFile ReadServerMultipliers(string filePath, out string resultText)
        {
            resultText = null;
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return null;

            try
            {
                var jsonText = ReadExportFile.ReadFile(filePath, "DinoExportGunServerSave_C", out resultText);
                if (jsonText == null)
                {
                    resultText = $"Error when importing file {filePath}: {resultText}";
                    return null;
                }

                var exportedServerMultipliers = JsonConvert.DeserializeObject<ExportGunServerFile>(jsonText);
                if (exportedServerMultipliers == null)
                {
                    resultText = $"Unknown error when importing file {filePath}";
                    return null;
                }

                resultText = $"Server multipliers imported from {filePath}";
                return exportedServerMultipliers;
            }
            catch (Exception ex)
            {
                resultText = $"Error when importing file {filePath}: {ex.Message}";
            }

            return null;
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
            cc.serverMultipliers.MatingSpeedMultiplier = Math.Round(esm.MatingSpeedMultiplier, roundToDigits);
            cc.serverMultipliers.MatingIntervalMultiplier = Math.Round(esm.MatingIntervalMultiplier, roundToDigits);
            cc.serverMultipliers.EggHatchSpeedMultiplier = Math.Round(esm.EggHatchSpeedMultiplier, roundToDigits);
            cc.serverMultipliers.BabyMatureSpeedMultiplier = Math.Round(esm.BabyMatureSpeedMultiplier, roundToDigits);
            cc.serverMultipliers.BabyCuddleIntervalMultiplier = Math.Round(esm.BabyCuddleIntervalMultiplier, roundToDigits);
            cc.serverMultipliers.BabyImprintAmountMultiplier = Math.Round(esm.BabyImprintAmountMultiplier, roundToDigits);
            cc.serverMultipliers.BabyImprintingStatScaleMultiplier = Math.Round(esm.BabyImprintingStatScaleMultiplier, roundToDigits);
            cc.serverMultipliers.BabyFoodConsumptionSpeedMultiplier = Math.Round(esm.BabyFoodConsumptionSpeedMultiplier, roundToDigits);
            cc.serverMultipliers.TamedDinoCharacterFoodDrainMultiplier = Math.Round(esm.TamedDinoCharacterFoodDrainMultiplier, roundToDigits);
            cc.serverMultipliers.AllowFlyerSpeedLeveling = esm.AllowFlyerSpeedLeveling;
            cc.singlePlayerSettings = esm.UseSingleplayerSettings;

            cc.ServerMultipliersHash = newServerMultipliersHash;

            return true;
        }
    }
}
