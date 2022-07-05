using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats.oldLibraryFormat
{
    /// <summary>
    /// This class provides methods to convert old file-formats to new formats, e.g. the 8-stat-format to the 12-stat-format.
    /// </summary>
    static class FormatConverter
    {
        /// <summary>
        /// Convert old libraries
        /// </summary>
        /// <param name="ccOld">CreatureCollection to be converted</param>
        public static CreatureCollection ConvertXml2Asb(CreatureCollectionOld ccOld, string libraryFilePath)
        {
            MessageBox.Show($"The library will be converted to the new format that supports all possible ARK-stats (e.g. the crafting speed for the Gacha).\n\nThe old library file is still available at \n{libraryFilePath}\nyou can keep it as a backup.",
                        "Library will be converted", MessageBoxButtons.OK, MessageBoxIcon.Information);

            CreatureCollection ccNew = new CreatureCollection();

            UpgradeFormatTo12Stats(ccOld, ccNew);
            TransferParameters(ccOld, ccNew);

            return ccNew;
        }

        /// <summary>
        /// Tries to converts the library from the 8-stats format to the 12-stats format and the species identification by the blueprintpath.
        /// </summary>
        public static void UpgradeFormatTo12Stats(CreatureCollectionOld ccOld, CreatureCollection ccNew)
        {
            if (ccOld == null) return;

            // if library has the old statMultiplier-indices, fix the order
            var newToOldIndices = new int[] { 0, 1, 7, 2, 3, -1, -1, 4, 5, 6, -1, -1 };
            if (ccOld.multipliers != null && ccOld.multipliers.Length == 8)
            {
                // old order was
                // HP, Stam, Ox, Fo, We, Dm, Sp, To
                // new order is
                // 0: Health
                // 1: Stamina / Charge Capacity
                // 2: Torpidity
                // 3: Oxygen / Charge Regeneration
                // 4: Food
                // 5: Water
                // 6: Temperature
                // 7: Weight
                // 8: MeleeDamageMultiplier / Charge Emission Range
                // 9: SpeedMultiplier
                // 10: TemperatureFortitude
                // 11: CraftingSpeedMultiplier

                // imprinting bonus factor default 0.2, 0, 0.2, 0, 0.2, 0.2, 0, 0.2, 0.2, 0.2, 0, 0
                // i.e. stats without imprinting are by default: St, Ox, Te, TF, Cr

                // create new multiplierArray
                var newMultipliers = new double[Stats.StatsCount][];
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    newMultipliers[s] = new double[4];
                    if (newToOldIndices[s] >= 0)
                    {
                        for (int si = 0; si < 4; si++)
                            newMultipliers[s][si] = ccOld.multipliers[newToOldIndices[s]][si];
                    }
                    else
                    {
                        for (int si = 0; si < 4; si++)
                            newMultipliers[s][si] = 1;
                    }
                }
                ccOld.multipliers = newMultipliers;
            }

            ccNew.creatures = new List<Creature>();

            foreach (CreatureOld c in ccOld.creatures)
            {
                Creature newC = new Creature
                {
                    addedToLibrary = c.addedToLibrary.Year < 2000 ? default(DateTime?) : c.addedToLibrary,
                    ArkId = c.ArkId,
                    ArkIdImported = c.ArkIdImported,
                    colors = c.colors.Select(ci => (byte)ci).ToArray(),
                    cooldownUntil = c.cooldownUntil.Year < 2000 ? default(DateTime?) : c.cooldownUntil,
                    domesticatedAt = c.domesticatedAt.Year < 2000 ? default(DateTime?) : c.domesticatedAt,
                    fatherGuid = c.fatherGuid,
                    flags = c.flags,
                    generation = c.generation,
                    growingLeft = c.growingLeft,
                    growingPaused = c.growingPaused,
                    growingUntil = c.growingUntil.Year < 2000 ? default(DateTime?) : c.growingUntil,
                    guid = c.guid,
                    imprinterName = c.imprinterName,
                    imprintingBonus = c.imprintingBonus,
                    isBred = c.isBred,
                    motherGuid = c.motherGuid,
                    mutationsMaternal = c.mutationsMaternal,
                    mutationsPaternal = c.mutationsPaternal,
                    name = c.name,
                    note = c.note,
                    owner = c.owner,
                    server = c.server,
                    sex = c.sex,
                    Status = c.status,
                    tags = c.tags,
                    tamingEff = c.tamingEff,
                    tribe = c.tribe
                };
                newC.InitializeArkInGame();
                ccNew.creatures.Add(newC);

                if (c.IsPlaceholder) newC.flags |= CreatureFlags.Placeholder;
                if (c.neutered) newC.flags |= CreatureFlags.Neutered;

                // set new species-id
                if (c.Species == null
                    && !string.IsNullOrEmpty(c.speciesBlueprint))
                    c.Species = Values.V.SpeciesByBlueprint(c.speciesBlueprint);
                if (c.Species == null
                    && Values.V.TryGetSpeciesByName(c.species, out Species speciesObject))
                    c.Species = speciesObject;

                newC.Species = c.Species;

                // fix statlevel-indices
                newC.levelsWild = Convert8To12(c.levelsWild);
                newC.levelsDom = Convert8To12(c.levelsDom);
            }

            ccNew.creaturesValues = new List<CreatureValues>();

            foreach (var cvOld in ccOld.creaturesValues)
            {
                var cv = new CreatureValues
                {
                    ARKID = cvOld.ARKID,
                    colorIDs = cvOld.colorIDs.Select(ci => (byte)ci).ToArray(),
                    cooldownUntil = cvOld.cooldownUntil.Year < 2000 ? default(DateTime?) : cvOld.cooldownUntil,
                    domesticatedAt = cvOld.domesticatedAt.Year < 2000 ? default(DateTime?) : cvOld.domesticatedAt,
                    fatherArkId = cvOld.fatherArkId,
                    fatherGuid = cvOld.fatherGuid,
                    growingUntil = cvOld.growingUntil.Year < 2000 ? default(DateTime?) : cvOld.growingUntil,
                    guid = cvOld.guid,
                    imprinterName = cvOld.imprinterName,
                    imprintingBonus = cvOld.imprintingBonus,
                    isBred = cvOld.isBred,
                    isTamed = cvOld.isTamed,
                    level = cvOld.level,
                    levelsDom = cvOld.levelsDom,
                    levelsWild = cvOld.levelsWild,
                    motherArkId = cvOld.motherArkId,
                    motherGuid = cvOld.motherGuid,
                    mutationCounterFather = cvOld.mutationCounterFather,
                    mutationCounterMother = cvOld.mutationCounterMother,
                    name = cvOld.name,
                    owner = cvOld.owner,
                    server = cvOld.server,
                    sex = cvOld.sex,
                    statValues = cvOld.statValues,
                    tamingEffMax = cvOld.tamingEffMax,
                    tamingEffMin = cvOld.tamingEffMin,
                    tribe = cvOld.tribe
                };

                if (cvOld.neutered) cv.flags |= CreatureFlags.Neutered;

                if (Values.V.TryGetSpeciesByName(cvOld.species, out Species species))
                    cv.Species = species;

                ccNew.creaturesValues.Add(cv);

                // fix statlevel-indices
                cv.levelsWild = Convert8To12(cvOld.levelsWild);
                cv.levelsDom = Convert8To12(cvOld.levelsDom);
                cv.statValues = Convert8To12(cvOld.statValues);
            }
        }

        private static int[] Convert8To12(int[] a)
        {
            var newToOldIndices = new int[] { 0, 1, 7, 2, 3, -1, -1, 4, 5, 6, -1, -1 };
            var newA = new int[Stats.StatsCount];
            if (a.Length == 12)
            {
                return a;
            }
            else
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (newToOldIndices[s] >= 0)
                        newA[s] = a[newToOldIndices[s]];
                }
            }
            return newA;
        }

        private static double[] Convert8To12(double[] a)
        {
            var newToOldIndices = new int[] { 0, 1, 7, 2, 3, -1, -1, 4, 5, 6, -1, -1 };
            var newA = new double[Stats.StatsCount];
            if (a.Length == 12)
            {
                return a;
            }
            else
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (newToOldIndices[s] >= 0)
                        newA[s] = a[newToOldIndices[s]];
                }
            }
            return newA;
        }

        public static void TransferParameters(CreatureCollectionOld ccOld, CreatureCollection ccNew)
        {
            ccNew.allowMoreThanHundredImprinting = ccOld.allowMoreThanHundredImprinting;
            ccNew.changeCreatureStatusOnSavegameImport = ccOld.changeCreatureStatusOnSavegameImport;
            ccNew.considerWildLevelSteps = ccOld.considerWildLevelSteps;
            ccNew.incubationListEntries = ccOld.incubationListEntries.Select(ile => new IncubationTimerEntry
            {
                fatherGuid = ile.fatherGuid,
                incubationDuration = ile.incubationDuration,
                incubationEnd = ile.incubationEnd,
                motherGuid = ile.motherGuid,
                timerIsRunning = ile.timerIsRunning,
            }).ToList();
            ccNew.maxBreedingSuggestions = ccOld.maxBreedingSuggestions;
            ccNew.maxChartLevel = ccOld.maxChartLevel;
            ccNew.maxDomLevel = ccOld.maxDomLevel;
            ccNew.maxServerLevel = ccOld.maxServerLevel;
            ccNew.noteList = ccOld.noteList;
            ccNew.ownerList = ccOld.ownerList;
            ccNew.players = ccOld.players;
            ccNew.serverList = ccOld.serverList;
            ccNew.singlePlayerSettings = ccOld.singlePlayerSettings;
            ccNew.tags = ccOld.tags;
            ccNew.tagsExclude = ccOld.tagsExclude;
            ccNew.tagsInclude = ccOld.tagsInclude;
            ccNew.timerListEntries = ccOld.timerListEntries.Select(tle => new TimerListEntry
            {
                creatureGuid = tle.creatureGuid,
                group = tle.group,
                name = tle.name,
                sound = tle.sound,
                time = tle.time
            }).ToList();
            ccNew.tribes = ccOld.tribes;
            ccNew.wildLevelStep = ccOld.wildLevelStep;

            // check if multiplier-conversion is possible
            if (ccOld?.multipliers == null) return;

            ccNew.serverMultipliers = new ServerMultipliers
            {
                BabyImprintingStatScaleMultiplier = ccOld.imprintingMultiplier,
                BabyCuddleIntervalMultiplier = ccOld.babyCuddleIntervalMultiplier,
                TamingSpeedMultiplier = ccOld.tamingSpeedMultiplier,
                DinoCharacterFoodDrainMultiplier = ccOld.tamingFoodRateMultiplier,
                MatingIntervalMultiplier = ccOld.MatingIntervalMultiplier,
                EggHatchSpeedMultiplier = ccOld.EggHatchSpeedMultiplier,
                BabyMatureSpeedMultiplier = ccOld.BabyMatureSpeedMultiplier,
                BabyFoodConsumptionSpeedMultiplier = ccOld.BabyFoodConsumptionSpeedMultiplier,
                statMultipliers = ccOld.multipliers // was converted to 12-stats before
            };

            ccNew.serverMultipliersEvents = new ServerMultipliers
            {
                BabyImprintingStatScaleMultiplier = ccOld.imprintingMultiplier, // cannot be changed in events
                BabyCuddleIntervalMultiplier = ccOld.babyCuddleIntervalMultiplierEvent,
                TamingSpeedMultiplier = ccOld.tamingSpeedMultiplierEvent,
                DinoCharacterFoodDrainMultiplier = ccOld.tamingFoodRateMultiplierEvent,
                MatingIntervalMultiplier = ccOld.MatingIntervalMultiplierEvent,
                EggHatchSpeedMultiplier = ccOld.EggHatchSpeedMultiplierEvent,
                BabyMatureSpeedMultiplier = ccOld.BabyMatureSpeedMultiplierEvent,
                BabyFoodConsumptionSpeedMultiplier = ccOld.BabyFoodConsumptionSpeedMultiplierEvent
            };
        }
    }
}
