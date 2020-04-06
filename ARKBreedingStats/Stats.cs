using ARKBreedingStats.species;
using ARKBreedingStats.utils;
using ARKBreedingStats.values;
using System;

namespace ARKBreedingStats
{
    public static class StatValueCalculation
    {
        //private const double ROUND_UP_DELTA = 0.0001; // remove for now. Rounding issues should be handled during extractino with value-ranges.

        public static double CalculateValue(Species species, int stat, int levelWild, int levelDom, bool dom, double tamingEff, double imprintingBonus, bool roundToIngamePrecision = true)
        {
            if (species == null)
                return 0;

            // if stat is generally available but level is set to -1 (== unknown), return -1 (== unknown)
            if (levelWild < 0 && species.stats[stat].IncPerWildLevel != 0)
                return -1;

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
                    && species.StatImprintMultipliers[stat] != 0
                    )
                    imprintingM = 1 + species.StatImprintMultipliers[stat] * imprintingBonus * Values.V.currentServerMultipliers.BabyImprintingStatScaleMultiplier;
                if (stat == 0)
                    tamedBaseHP = (float)species.TamedBaseHealthMultiplier;
            }
            //double result = Math.Round((species.stats[stat].BaseValue * tamedBaseHP * (1 + species.stats[stat].IncPerWildLevel * levelWild) * imprintingM + add) * domMult, Utils.precision(stat), MidpointRounding.AwayFromZero);
            // double is too precise and results in wrong values due to rounding. float results in better values, probably ARK uses float as well.
            // or rounding first to a precision of 7, then use the rounding of the precision
            //double resultt = Math.Round((species.stats[stat].BaseValue * tamedBaseHP * (1 + species.stats[stat].IncPerWildLevel * levelWild) * imprintingM + add) * domMult, 7);
            //resultt = Math.Round(resultt, Utils.precision(stat), MidpointRounding.AwayFromZero);

            // adding an epsilon to handle rounding-errors
            double result = (species.stats[stat].BaseValue * tamedBaseHP *
                    (1 + species.stats[stat].IncPerWildLevel * levelWild) * imprintingM + add) *
                    domMult;// + (Utils.precision(stat) == 3 ? ROUND_UP_DELTA * 0.01 : ROUND_UP_DELTA);

            if (result <= 0) return 0;

            if (roundToIngamePrecision)
                return Math.Round(result, Utils.Precision(stat), MidpointRounding.AwayFromZero);

            return result;
        }


        /// <summary>
        /// ARK uses float-types for the stats which have precision errors. This method returns the possible aberration of that value.
        /// </summary>
        /// <param name="displayedStatValue">Stat value</param>
        /// <param name="displayedDecimals">Percentage values have a higher precision, they display 3 decimal digits</param>
        /// <param name="highPrecisionInput">When obtained from an export file, stat values are given with more decimal digits.</param>
        public static float DisplayedAberration(double displayedStatValue, int displayedDecimals = 1, bool highPrecisionInput = false)
        {
            // ARK displays one decimal digit, so the minimal error of a given number is assumed to be 0.06.
            // the theoretical value of a maximal error of 0.05 is too low.
            const float ARKDISPLAYVALUEERROR = 0.06f;
            // If an export file is used, the full float precision of the stat value is given, the precision is calculated then.
            // For values > 1e6 the float precision error is larger than 0.06

            // always consider at least an error of. When using only the float-precision often the stat-calculations increase the resulting error to be much larger.
            const float MINVALUEERROR = 0.001f;

            // the error can increase due to the stat-calculation. Assume a factor of 10 for now, values lower than 6 were too low.
            const float CALCULATIONERRORFACTOR = 10f;

            return highPrecisionInput || displayedStatValue * (displayedDecimals == 3 ? 100 : 1) > 1e6
                    ? Math.Max(MINVALUEERROR, ((float)displayedStatValue).FloatPrecision() * CALCULATIONERRORFACTOR)
                    : ARKDISPLAYVALUEERROR * (displayedDecimals == 3 ? .01f : 1);
        }
    }
}
