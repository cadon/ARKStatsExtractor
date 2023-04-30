using System;
using System.Collections.Generic;
using System.Linq;
using ARKBreedingStats.Library;
using ARKBreedingStats.Properties;
using ARKBreedingStats.species;

namespace ARKBreedingStats.BreedingPlanning
{
    /// <summary>
    /// Calculation of a breeding score, which will help to decide which pairings will result in a desired offspring.
    /// </summary>
    public static class BreedingScore
    {
        /// <summary>
        /// Calculates the breeding score of all possible pairs.
        /// </summary>
        /// <param name="females"></param>
        /// <param name="males"></param>
        /// <param name="species"></param>
        /// <param name="bestPossLevels"></param>
        /// <param name="statWeights"></param>
        /// <param name="bestLevelsOfSpecies">If the according stat weight is negative, the lowest level is contained.</param>
        /// <param name="breedingMode"></param>
        /// <param name="considerChosenCreature"></param>
        /// <param name="considerMutationLimit"></param>
        /// <param name="mutationLimit"></param>
        /// <param name="creaturesMutationsFilteredOut"></param>
        /// <param name="offspringLevelLimit">If &gt; 0, pairs that can result in a creature with a level higher than that, are highlighted. This can be used if there's a level cap.</param>
        /// <param name="downGradeOffspringWithLevelHigherThanLimit">Downgrade score if level is higher than limit.</param>
        /// <param name="onlyBestSuggestionForFemale">Only the pairing with the highest score is kept for each female. Is not used if species has no sex or sex is ignored in breeding planner.</param>
        /// <param name="anyOddEven">Array for each stat if the higher level should be considered for score: 0: consider any level, 1: consider only if odd, 2: consider only if even.</param>
        /// <returns></returns>
        public static List<BreedingPair> CalculateBreedingScores(Creature[] females, Creature[] males, Species species,
            short[] bestPossLevels, double[] statWeights, int[] bestLevelsOfSpecies, BreedingPlan.BreedingMode breedingMode,
            bool considerChosenCreature, bool considerMutationLimit, int mutationLimit,
            ref bool creaturesMutationsFilteredOut, int offspringLevelLimit = 0, bool downGradeOffspringWithLevelHigherThanLimit = false,
            bool onlyBestSuggestionForFemale = false, byte[] anyOddEven = null)
        {
            var breedingPairs = new List<BreedingPair>();
            var ignoreSex = Properties.Settings.Default.IgnoreSexInBreedingPlan || species.noGender;
            if (anyOddEven != null && anyOddEven.Length != Stats.StatsCount)
                anyOddEven = null;

            var customIgnoreTopStatsEvenOdd = new bool[Stats.StatsCount];
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                customIgnoreTopStatsEvenOdd[s] = anyOddEven != null && statWeights[s] > 0;
            }

            for (int fi = 0; fi < females.Length; fi++)
            {
                var female = females[fi];
                for (int mi = 0; mi < males.Length; mi++)
                {
                    var male = males[mi];
                    // if ignoreSex (useful when using S+ mutator), skip pair if
                    // creatures are the same, or pair has already been added
                    if (ignoreSex)
                    {
                        if (considerChosenCreature)
                        {
                            if (male == female)
                                continue;
                        }
                        else if (fi == mi)
                            break;
                    }
                    // if mutation limit is set, only skip pairs where both parents exceed that limit. One parent is enough to trigger a mutation.
                    if (considerMutationLimit && female.Mutations > mutationLimit && male.Mutations > mutationLimit)
                    {
                        creaturesMutationsFilteredOut = true;
                        continue;
                    }

                    double t = 0;
                    int offspringPotentialTopStatCount = 0;
                    double offspringExpectedTopStatCount = 0; // a guaranteed top stat counts 1, otherwise the inheritance probability of the top stat is counted

                    int topStatsMother = 0;
                    int topStatsFather = 0;

                    int maxPossibleOffspringLevel = 1;

                    for (int s = 0; s < Stats.StatsCount; s++)
                    {
                        if (s == Stats.Torpidity || !species.UsesStat(s)) continue;
                        bestPossLevels[s] = 0;
                        int higherLevel = Math.Max(female.levelsWild[s], male.levelsWild[s]);
                        int lowerLevel = Math.Min(female.levelsWild[s], male.levelsWild[s]);
                        if (higherLevel < 0) higherLevel = 0;
                        if (lowerLevel < 0) lowerLevel = 0;
                        maxPossibleOffspringLevel += higherLevel;

                        bool ignoreTopStats = false;

                        if (customIgnoreTopStatsEvenOdd[s])
                        {
                            // if there is a custom setting for this species, consider that for higher levels
                            // 0: consider all levels, 1: consider only odd levels, 2: consider only even levels
                            switch (anyOddEven[s])
                            {
                                case 1:
                                    ignoreTopStats = higherLevel % 2 == 0;
                                    break;
                                case 2:
                                    ignoreTopStats = higherLevel % 2 != 0;
                                    break;
                            }
                        }

                        double weightedExpectedStatLevel = statWeights[s] * (Ark.ProbabilityInheritHigherLevel * higherLevel + Ark.ProbabilityInheritLowerLevel * lowerLevel) / 40;
                        if (weightedExpectedStatLevel != 0)
                        {
                            if (breedingMode == BreedingPlan.BreedingMode.TopStatsLucky)
                            {
                                if (!ignoreTopStats && (female.levelsWild[s] == bestLevelsOfSpecies[s] || male.levelsWild[s] == bestLevelsOfSpecies[s]))
                                {
                                    if (female.levelsWild[s] == bestLevelsOfSpecies[s] && male.levelsWild[s] == bestLevelsOfSpecies[s])
                                        weightedExpectedStatLevel *= 1.142;
                                }
                                else if (bestLevelsOfSpecies[s] > 0)
                                    weightedExpectedStatLevel *= .01;
                            }
                            else if (breedingMode == BreedingPlan.BreedingMode.TopStatsConservative && bestLevelsOfSpecies[s] > 0)
                            {
                                bool higherIsBetter = statWeights[s] >= 0;
                                bestPossLevels[s] = (short)(higherIsBetter ? Math.Max(female.levelsWild[s], male.levelsWild[s]) : Math.Min(female.levelsWild[s], male.levelsWild[s]));
                                weightedExpectedStatLevel *= .01;
                                if (!ignoreTopStats && (female.levelsWild[s] == bestLevelsOfSpecies[s] || male.levelsWild[s] == bestLevelsOfSpecies[s]))
                                {
                                    offspringPotentialTopStatCount++;
                                    offspringExpectedTopStatCount += female.levelsWild[s] == bestLevelsOfSpecies[s] && male.levelsWild[s] == bestLevelsOfSpecies[s] ? 1 : Ark.ProbabilityInheritHigherLevel;
                                    if (female.levelsWild[s] == bestLevelsOfSpecies[s])
                                        topStatsMother++;
                                    if (male.levelsWild[s] == bestLevelsOfSpecies[s])
                                        topStatsFather++;
                                }
                            }
                            t += weightedExpectedStatLevel;
                        }
                    }

                    if (breedingMode == BreedingPlan.BreedingMode.TopStatsConservative)
                    {
                        if (topStatsMother < offspringPotentialTopStatCount && topStatsFather < offspringPotentialTopStatCount)
                            t += offspringExpectedTopStatCount;
                        else
                            t += .1 * offspringExpectedTopStatCount;
                        // check if the best possible stat outcome regarding topLevels already exists in a male
                        bool maleExists = false;

                        foreach (Creature cr in males)
                        {
                            maleExists = true;
                            for (int s = 0; s < Stats.StatsCount; s++)
                            {
                                if (s == Stats.Torpidity
                                    || !cr.Species.UsesStat(s)
                                    || cr.levelsWild[s] == bestPossLevels[s]
                                    || bestPossLevels[s] != bestLevelsOfSpecies[s])
                                    continue;

                                maleExists = false;
                                break;
                            }
                            if (maleExists)
                                break;
                        }
                        if (maleExists)
                            t *= .4; // another male with the same stats is not worth much, the mating-cooldown of males is short.
                        else
                        {
                            // check if the best possible stat outcome already exists in a female
                            bool femaleExists = false;
                            foreach (Creature cr in females)
                            {
                                femaleExists = true;
                                for (int s = 0; s < Stats.StatsCount; s++)
                                {
                                    if (s == Stats.Torpidity
                                        || !cr.Species.UsesStat(s)
                                        || cr.levelsWild[s] == bestPossLevels[s]
                                        || bestPossLevels[s] != bestLevelsOfSpecies[s])
                                        continue;

                                    femaleExists = false;
                                    break;
                                }
                                if (femaleExists)
                                    break;
                            }
                            if (femaleExists)
                                t *= .8; // another female with the same stats may be useful, but not so much in conservative breeding
                        }
                        //t *= 2; // scale conservative mode as it rather displays improvement, but only scarcely
                    }

                    var highestOffspringOverLevelLimit =
                        offspringLevelLimit > 0 && offspringLevelLimit < maxPossibleOffspringLevel;
                    if (highestOffspringOverLevelLimit && downGradeOffspringWithLevelHigherThanLimit)
                        t *= 0.01;

                    int mutationPossibleFrom = female.Mutations < Ark.MutationPossibleWithLessThan && male.Mutations < Ark.MutationPossibleWithLessThan ? 2
                        : female.Mutations < Ark.MutationPossibleWithLessThan || male.Mutations < Ark.MutationPossibleWithLessThan ? 1 : 0;

                    breedingPairs.Add(new BreedingPair(female, male,
                        new Score(t * 1.25),
                        (mutationPossibleFrom == 2 ? Ark.ProbabilityOfOneMutation : mutationPossibleFrom == 1 ? Ark.ProbabilityOfOneMutationFromOneParent : 0),
                        highestOffspringOverLevelLimit));
                }
            }

            breedingPairs = breedingPairs.OrderByDescending(p => p.BreedingScore).ToList();

            if (onlyBestSuggestionForFemale && !ignoreSex)
            {
                var onlyOneSuggestionPerFemale = new List<BreedingPair>();
                foreach (var bp in breedingPairs)
                {
                    if (!onlyOneSuggestionPerFemale.Any(p => p.Mother == bp.Mother))
                        onlyOneSuggestionPerFemale.Add(bp);
                }

                breedingPairs = onlyOneSuggestionPerFemale;
            }

            return breedingPairs;
        }

        /// <summary>
        /// Sets the best levels in the passed bestLevels array, depending on the statWeights and onlyHighEvenLevels.
        /// </summary>
        public static void SetBestLevels(IEnumerable<Creature> creatures, int[] bestLevels, double[] statWeights, byte[] anyOddEven = null)
        {
            for (int s = 0; s < Stats.StatsCount; s++)
                bestLevels[s] = -1;

            foreach (Creature c in creatures)
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if ((s == Stats.Torpidity || statWeights[s] >= 0) && c.levelsWild[s] > bestLevels[s])
                    {
                        if ((anyOddEven?[s] ?? 0) == 0
                            || (anyOddEven[s] == 1 && c.levelsWild[s] % 2 == 1)
                            || (anyOddEven[s] == 2 && c.levelsWild[s] % 2 == 0)
                            )
                            bestLevels[s] = c.levelsWild[s];
                    }
                    else if (s != Stats.Torpidity && statWeights[s] < 0 && c.levelsWild[s] >= 0 && (c.levelsWild[s] < bestLevels[s] || bestLevels[s] < 0))
                        bestLevels[s] = c.levelsWild[s];
                }
            }
        }

        /// <summary>
        /// Returns better of two given levels. If anyOddEven == 0: higher of both, if == 1: higher of odd levels, if == 2: higher of even levels.
        /// If both levels don't match odd/even, -1 is returned.
        /// </summary>
        public static int GetHigherBestLevel(int level1, int level2, byte anyOddEven)
        {
            switch (anyOddEven)
            {
                case 1:
                    if (level1 % 2 == 1 && level2 % 2 == 1) return Math.Max(level1, level2);
                    if (level1 % 2 == 1) return level1;
                    if (level2 % 2 == 1) return level2;
                    return -1;
                case 2:
                    if (level1 % 2 == 0 && level2 % 2 == 0) return Math.Max(level1, level2);
                    if (level1 % 2 == 0) return level1;
                    if (level2 % 2 == 0) return level2;
                    return -1;
                default: return Math.Max(level1, level2);
            }
        }
    }
}
