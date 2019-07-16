using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;

namespace ARKBreedingStats
{
    public static class Stats
    {
        private const double roundupDelta = 0.0001;

        public static double calculateValue(Species species, int stat, int levelWild, int levelDom, bool dom, double tamingEff, double imprintingBonus)
        {
            // if stat is generally available but level is set to -1 (== unknown), return -1 (== unknown)
            if (levelWild < 0 && species != null && species.stats[stat].IncPerWildLevel != 0)
            {
                return -1;
            }
            if (species != null)
            {
                double add = 0, domMult = 1, imprintingM = 1, tamedBaseHP = 1;
                if (dom)
                {
                    add = species.stats[stat].AddWhenTamed;
                    double domMultAffinity = species.stats[stat].MultAffinity;
                    // the multiplicative bonus is only multiplied with the TE if it is positive (i.e. negative boni won't get less bad if the TE is low)
                    if (domMultAffinity >= 0)
                        domMultAffinity *= tamingEff;
                    domMult = (tamingEff >= 0 ? (1 + domMultAffinity) : 1) * (1 + levelDom * species.stats[stat].IncPerTamedLevel);
                    if (imprintingBonus > 0
                        && stat != (int)StatNames.Stamina
                        && stat != (int)StatNames.Oxygen
                        && stat != (int)StatNames.Temperature
                        && (stat != (int)StatNames.SpeedMultiplier || species.NoImprintingForSpeed == false)
                        && stat != (int)StatNames.TemperatureFortitude
                        && stat != (int)StatNames.CraftingSpeedMultiplier
                        )
                        imprintingM = 1 + 0.2 * imprintingBonus * Values.V.currentServerMultipliers.BabyImprintingStatScaleMultiplier; // TODO 0.2 is not always true
                    if (stat == 0)
                        tamedBaseHP = (float)species.TamedBaseHealthMultiplier;
                }
                //double result = Math.Round((species.stats[stat].BaseValue * tamedBaseHP * (1 + species.stats[stat].IncPerWildLevel * levelWild) * imprintingM + add) * domMult, Utils.precision(stat), MidpointRounding.AwayFromZero);
                // double is too precise and results in wrong values due to rounding. float results in better values, probably ARK uses float as well.
                // or rounding first to a precision of 7, then use the rounding of the precision
                //double resultt = Math.Round((species.stats[stat].BaseValue * tamedBaseHP * (1 + species.stats[stat].IncPerWildLevel * levelWild) * imprintingM + add) * domMult, 7);
                //resultt = Math.Round(resultt, Utils.precision(stat), MidpointRounding.AwayFromZero);

                // adding an epsilon to handle rounding-errors
                double result = Math.Round((species.stats[stat].BaseValue * tamedBaseHP *
                        (1 + species.stats[stat].IncPerWildLevel * levelWild) * imprintingM + add) *
                        domMult + roundupDelta, Utils.precision(stat), MidpointRounding.AwayFromZero);

                return result >= 0 ? result : 0;
            }
            return 0;
        }
    }
}
