using ARKBreedingStats.Settings;
using System;

namespace ARKBreedingStats.Models
{
    public static class StatValueCalculation
    {
        /// <summary>
        /// Calculate the stat value.
        /// </summary>
        public static double CalculateValue(Species species, int statIndex, int levelWild, int levelMut, int levelDom,
            bool dom, double tamingEff = 0, double imprintingBonus = 0, bool roundToIngamePrecision = true,
            Troodonism.AffectedStats useTroodonismStats = Troodonism.AffectedStats.None,
            ServerMultipliers multipliers = null)
        {
            if (species?.stats == null) return 0;

            var speciesStat = useTroodonismStats == Troodonism.AffectedStats.None
                ? species.stats[statIndex]
                : Troodonism.SelectStats(species.stats[statIndex], species.altStats[statIndex], useTroodonismStats);

            if (speciesStat == null) return 0;

            // if stat is generally available but level is set to -1 (== unknown), return -1 (== unknown)
            if (levelWild < 0 && speciesStat.IncPerWildLevel != 0)
                return -1;

            double add = 0, domMult = 1, imprintingM = 1, tamedBaseHP = 1;
            if (dom)
            {
                add = speciesStat.AddWhenTamed;
                double domMultAffinity = speciesStat.MultAffinity;
                // the multiplicative bonus is only multiplied with the TE if it is positive (i.e. negative boni won't get less bad if the TE is low)
                if (domMultAffinity >= 0)
                    domMultAffinity *= tamingEff;
                domMult = tamingEff >= 0 ? 1 + domMultAffinity : 1;
                if (imprintingBonus > 0
                    && species.StatImprintMultipliers[statIndex] != 0)
                    imprintingM = 1 + species.StatImprintMultipliers[statIndex] * imprintingBonus * (multipliers?.BabyImprintingStatScaleMultiplier ?? 1);
                if (statIndex == Stats.Health)
                    tamedBaseHP = species.TamedBaseHealthMultiplier ?? 1;
            }
            else
            {
                levelDom = 0;
            }

            var wildLevelIncrease = levelWild * speciesStat.IncPerWildLevel +
                                    levelMut * speciesStat.IncPerMutatedLevel;
            var domLevelIncrease = levelDom * speciesStat.IncPerTamedLevel;

            var result = speciesStat.IncreaseStatAsPercentage
                ? (speciesStat.BaseValue * (1 + wildLevelIncrease) * tamedBaseHP * imprintingM + add) * domMult * (1 + domLevelIncrease)
                : ((speciesStat.BaseValue + wildLevelIncrease) * tamedBaseHP * imprintingM + add) * domMult + domLevelIncrease;

            if (result <= 0) return 0;
            result = speciesStat.ApplyCap(result);

            if (roundToIngamePrecision)
                return Math.Round(result, Stats.Precision(statIndex), MidpointRounding.AwayFromZero);

            return result;
        }

        /// <summary>
        /// ARK uses float-types for the stats which have precision errors. This method returns the possible aberration of that value.
        /// </summary>
        public static float DisplayedAberration(double displayedStatValue, int displayedDecimals = 1, bool highPrecisionInput = false)
        {
            const float arkDisplayValueError = 0.06f;
            const float minValueError = 0.001f;
            const float calculationErrorFactor = 20;

            return highPrecisionInput || displayedStatValue * (displayedDecimals == 3 ? 100 : 1) > 1e6
                    ? Math.Max(minValueError, ((float)displayedStatValue).FloatPrecision() * calculationErrorFactor)
                    : arkDisplayValueError * (displayedDecimals == 3 ? .01f : 1);
        }
    }
}
