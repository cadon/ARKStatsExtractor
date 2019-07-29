using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats
{
    /// <summary>
    /// This class provides methods to convert old file-formats to new formats, e.g. the 8-stat-format to the 12-stat-format.
    /// </summary>
    static class FormatConverter
    {
        /// <summary>
        /// Tries to converts the library from the 8-stats format to the 12-stats format and the species identification by the blueprintpath.
        /// </summary>
        public static void UpgradeFormatTo12Stats(CreatureCollection cc)
        {
            if (cc == null) return;

            // if library has the old statMultiplier-indices, fix the order
            var newToOldIndices = new int[] { 0, 1, 7, 2, 3, -1, -1, 4, 5, 6, -1, -1 };
            if (cc.multipliers.Length == 8)
            {
                /// old order was
                /// HP, Stam, Ox, Fo, We, Dm, Sp, To
                /// new order is
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
                var newMultipliers = new double[Values.STATS_COUNT][];
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    newMultipliers[s] = new double[4];
                    if (newToOldIndices[s] >= 0)
                    {
                        for (int si = 0; si < 4; si++)
                            newMultipliers[s][si] = cc.multipliers[newToOldIndices[s]][si];
                    }
                    else
                    {
                        for (int si = 0; si < 4; si++)
                            newMultipliers[s][si] = 1;
                    }
                }
                cc.multipliers = newMultipliers;
            }

            foreach (Creature c in cc.creatures)
            {
                // set new species-id
                if (c.Species == null && Values.V.TryGetSpeciesByName(c.species, out Species speciesObject))
                    c.Species = speciesObject;

                // fix statlevel-indices
                if (c.levelsWild.Length == 8)
                {
                    var newLevels = new int[Values.STATS_COUNT];
                    for (int s = 0; s < Values.STATS_COUNT; s++)
                    {
                        if (newToOldIndices[s] >= 0)
                            newLevels[s] = c.levelsWild[newToOldIndices[s]];
                    }
                    c.levelsWild = newLevels;
                }
                if (c.levelsDom.Length == 8)
                {
                    var newLevels = new int[Values.STATS_COUNT];
                    for (int s = 0; s < Values.STATS_COUNT; s++)
                    {
                        if (newToOldIndices[s] >= 0)
                            newLevels[s] = c.levelsDom[newToOldIndices[s]];
                    }
                    c.levelsDom = newLevels;
                }
            }

            // Mark it as the new format
            cc.FormatVersion = CreatureCollection.CURRENT_FORMAT_VERSION;
        }

        public static void ConvertMultipliers(CreatureCollection cc)
        {
            // check if conversion is needed
            if (cc.multipliers == null) return;

            cc.serverMultipliers = new ServerMultipliers
            {
                BabyImprintingStatScaleMultiplier = cc.imprintingMultiplier,
                BabyCuddleIntervalMultiplier = cc.babyCuddleIntervalMultiplier,
                TamingSpeedMultiplier = cc.tamingSpeedMultiplier,
                DinoCharacterFoodDrainMultiplier = cc.tamingFoodRateMultiplier,
                MatingIntervalMultiplier = cc.MatingIntervalMultiplier,
                EggHatchSpeedMultiplier = cc.EggHatchSpeedMultiplier,
                BabyMatureSpeedMultiplier = cc.BabyMatureSpeedMultiplier,
                BabyFoodConsumptionSpeedMultiplier = cc.BabyFoodConsumptionSpeedMultiplier
            };

            cc.serverMultipliers.statMultipliers = cc.multipliers;
            cc.multipliers = null;

            cc.serverMultipliersEvents = new ServerMultipliers
            {
                BabyImprintingStatScaleMultiplier = cc.imprintingMultiplier, // cannot be changed in events
                BabyCuddleIntervalMultiplier = cc.babyCuddleIntervalMultiplierEvent,
                TamingSpeedMultiplier = cc.tamingSpeedMultiplierEvent,
                DinoCharacterFoodDrainMultiplier = cc.tamingFoodRateMultiplierEvent,
                MatingIntervalMultiplier = cc.MatingIntervalMultiplierEvent,
                EggHatchSpeedMultiplier = cc.EggHatchSpeedMultiplierEvent,
                BabyMatureSpeedMultiplier = cc.BabyMatureSpeedMultiplierEvent,
                BabyFoodConsumptionSpeedMultiplier = cc.BabyFoodConsumptionSpeedMultiplierEvent
            };
        }
    }
}
