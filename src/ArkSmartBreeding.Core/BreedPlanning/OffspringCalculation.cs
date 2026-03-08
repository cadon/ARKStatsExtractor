using System;
using System.Linq;
using ARKBreedingStats.Library;
using ARKBreedingStats.Models;

namespace ARKBreedingStats.BreedingPlanning
{
    /// <summary>
    /// Result of calculating offspring stats for a breeding pair.
    /// </summary>
    public class OffspringPotential
    {
        /// <summary>
        /// Best possible offspring creature (virtual).
        /// </summary>
        public Creature Best { get; set; }

        /// <summary>
        /// Worst possible offspring creature (virtual).
        /// </summary>
        public Creature Worst { get; set; }

        /// <summary>
        /// Probability to get the best outcome for all relevant stats.
        /// </summary>
        public double ProbabilityBest { get; set; }

        /// <summary>
        /// True if any stat level is unknown (-1), making total level uncertain.
        /// </summary>
        public bool TotalLevelUnknown { get; set; }
    }

    /// <summary>
    /// Pure logic for calculating potential offspring stats from a breeding pair.
    /// </summary>
    public static class OffspringCalculation
    {
        /// <summary>
        /// Calculates the best and worst possible offspring for a breeding pair.
        /// </summary>
        /// <param name="mother">Mother creature.</param>
        /// <param name="father">Father creature.</param>
        /// <param name="species">Species of the pair.</param>
        /// <param name="statWeights">Stat weights determining which direction is "better".</param>
        /// <param name="statOddEvens">Odd/even preference per stat for higher level selection.</param>
        /// <param name="bestLevelsWild">Best wild levels in the species for top-stat marking.</param>
        /// <param name="bestLevelsMutated">Best mutated levels in the species for top-mutation-stat marking.</param>
        /// <param name="breedingMode">Current breeding mode.</param>
        /// <param name="levelStep">Wild level step for the creature collection.</param>
        public static OffspringPotential CalculateOffspringPotential(
            Creature mother, Creature father, Species species,
            double[] statWeights, StatValueEvenOdd[] statOddEvens,
            int[] bestLevelsWild, int[] bestLevelsMutated,
            BreedingScore.BreedingMode breedingMode, int? levelStep)
        {
            var crB = new Creature(species, string.Empty, levelsWild: new int[Stats.StatsCount], levelsMutated: new int[Stats.StatsCount], isBred: true, levelStep: levelStep);
            var crW = new Creature(species, string.Empty, levelsWild: new int[Stats.StatsCount], levelsMutated: new int[Stats.StatsCount], isBred: true, levelStep: levelStep);
            crB.Mother = mother;
            crB.Father = father;
            crW.Mother = mother;
            crW.Father = father;

            double probabilityBest = 1;
            bool totalLevelUnknown = false;
            bool topStatBreedingMode = breedingMode == BreedingScore.BreedingMode.TopStatsConservative
                                       || breedingMode == BreedingScore.BreedingMode.TopStatsLucky;

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s == Stats.Torpidity || !mother.Species.UsesStat(s))
                    continue;

                var higherLevelPreferred = statWeights[s] >= 0;
                crB.levelsWild[s] = higherLevelPreferred
                    ? BreedingScore.GetHigherBestLevel(mother.levelsWild[s], father.levelsWild[s], statOddEvens[s])
                    : Math.Min(mother.levelsWild[s], father.levelsWild[s]);
                crB.levelsMutated[s] = higherLevelPreferred
                    ? Math.Max(mother.levelsMutated?[s] ?? 0, father.levelsMutated?[s] ?? 0)
                    : Math.Min(mother.levelsMutated?[s] ?? 0, father.levelsMutated?[s] ?? 0);
                crB.valuesBreeding[s] = StatValueCalculation.CalculateValue(species, s, crB.levelsWild[s], crB.levelsMutated[s], 0, true, 1, 0);
                crB.SetTopStat(s, species.stats[s].IncPerTamedLevel != 0 && crB.levelsWild[s] == bestLevelsWild[s]);
                crB.SetTopMutationStat(s, crB.levelsMutated[s] == bestLevelsMutated[s]);

                crW.levelsWild[s] = higherLevelPreferred
                    ? Math.Min(mother.levelsWild[s], father.levelsWild[s])
                    : Math.Max(mother.levelsWild[s], father.levelsWild[s]);
                crW.levelsMutated[s] = higherLevelPreferred
                    ? Math.Min(mother.levelsMutated?[s] ?? 0, father.levelsMutated?[s] ?? 0)
                    : Math.Max(mother.levelsMutated?[s] ?? 0, father.levelsMutated?[s] ?? 0);
                crW.valuesBreeding[s] = StatValueCalculation.CalculateValue(species, s, crW.levelsWild[s], crW.levelsMutated[s], 0, true, 1, 0);
                crW.SetTopStat(s, species.stats[s].IncPerTamedLevel != 0 && crW.levelsWild[s] == bestLevelsWild[s]);
                // note: original code sets crB's top mutation stat from crW's levels (appears intentional for worst-case mutation tracking)
                crB.SetTopMutationStat(s, crW.levelsMutated[s] == bestLevelsMutated[s]);

                if (crB.levelsWild[s] == -1 || crW.levelsWild[s] == -1)
                    totalLevelUnknown = true;

                var probabilityInheritingHigherLevel = Ark.ProbabilityInheritHigherLevel
                    + mother.ProbabilityOffsetInheritingHigherLevel(s)
                    + father.ProbabilityOffsetInheritingHigherLevel(s);

                if (crB.levelsWild[s] > crW.levelsWild[s]
                    && (!topStatBreedingMode || crB.IsTopStat(s) || crB.IsTopMutationStat(s)))
                {
                    probabilityBest *= probabilityInheritingHigherLevel;
                }
                else if (crB.levelsWild[s] < crW.levelsWild[s]
                         && (!topStatBreedingMode || crB.IsTopStat(s) || crB.IsTopMutationStat(s)))
                {
                    probabilityBest *= 1 - probabilityInheritingHigherLevel;
                }
            }

            crB.levelsWild[Stats.Torpidity] = crB.levelsWild.Sum() + crB.levelsMutated.Sum();
            crW.levelsWild[Stats.Torpidity] = crW.levelsWild.Sum() + crW.levelsMutated.Sum();
            crB.RecalculateCreatureValues(levelStep);
            crW.RecalculateCreatureValues(levelStep);

            crB.mutationsMaternal = mother.Mutations;
            crB.mutationsPaternal = father.Mutations;
            crW.mutationsMaternal = mother.Mutations;
            crW.mutationsPaternal = father.Mutations;

            return new OffspringPotential
            {
                Best = crB,
                Worst = crW,
                ProbabilityBest = probabilityBest,
                TotalLevelUnknown = totalLevelUnknown
            };
        }
    }
}
