using System;
using System.Collections.Generic;
using System.Linq;
using ARKBreedingStats.Library;
using ARKBreedingStats.Properties;
using ARKBreedingStats.species;
using ARKBreedingStats.values;

namespace ARKBreedingStats.BreedingPlanning
{
    /// <summary>
    /// Calculation of a breeding score, which will help to decide which pairings will result in a desired offspring.
    /// </summary>
    public static class BreedingScore
    {
        public static List<BreedingPair> CalculateBreedingScores(Creature[] females, Creature[] males, Species species,
            short[] bestPossLevels, double[] statWeights, int[] bestLevels, BreedingPlan.BreedingMode breedingMode,
            bool considerChosenCreature, bool considerMutationLimit, int mutationLimit,
            ref bool creaturesMutationsFilteredOut)
        {
            var breedingPairs = new List<BreedingPair>();
            for (int fi = 0; fi < females.Length; fi++)
            {
                var female = females[fi];
                for (int mi = 0; mi < males.Length; mi++)
                {
                    var male = males[mi];
                    // if Properties.Settings.Default.IgnoreSexInBreedingPlan (useful when using S+ mutator), skip pair if
                    // creatures are the same, or pair has already been added
                    if (Settings.Default.IgnoreSexInBreedingPlan)
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
                    int nrTS = 0;
                    double eTS = 0;

                    int topFemale = 0;
                    int topMale = 0;

                    for (int s = 0; s < Values.STATS_COUNT; s++)
                    {
                        if (s == (int)StatNames.Torpidity || !species.UsesStat(s)) continue;
                        bestPossLevels[s] = 0;
                        int higherLevel = Math.Max(female.levelsWild[s], male.levelsWild[s]);
                        int lowerLevel = Math.Min(female.levelsWild[s], male.levelsWild[s]);
                        if (higherLevel < 0) higherLevel = 0;
                        if (lowerLevel < 0) lowerLevel = 0;

                        bool ignoreTopStats = Settings.Default.BreedingPlannerConsiderOnlyEvenForHighStats
                                              && higherLevel % 2 != 0
                                              && statWeights[s] > 0;

                        bool higherIsBetter = statWeights[s] >= 0;

                        double tt = statWeights[s] * (BreedingPlan.ProbabilityHigherLevel * higherLevel + BreedingPlan.ProbabilityLowerLevel * lowerLevel) / 40;
                        if (tt != 0)
                        {
                            if (breedingMode == BreedingPlan.BreedingMode.TopStatsLucky)
                            {
                                if (!ignoreTopStats && (female.levelsWild[s] == bestLevels[s] || male.levelsWild[s] == bestLevels[s]))
                                {
                                    if (female.levelsWild[s] == bestLevels[s] && male.levelsWild[s] == bestLevels[s])
                                        tt *= 1.142;
                                }
                                else if (bestLevels[s] > 0)
                                    tt *= .01;
                            }
                            else if (breedingMode == BreedingPlan.BreedingMode.TopStatsConservative && bestLevels[s] > 0)
                            {
                                bestPossLevels[s] = (short)(higherIsBetter ? Math.Max(female.levelsWild[s], male.levelsWild[s]) : Math.Min(female.levelsWild[s], male.levelsWild[s]));
                                tt *= .01;
                                if (!ignoreTopStats && (female.levelsWild[s] == bestLevels[s] || male.levelsWild[s] == bestLevels[s]))
                                {
                                    nrTS++;
                                    eTS += female.levelsWild[s] == bestLevels[s] && male.levelsWild[s] == bestLevels[s] ? 1 : BreedingPlan.ProbabilityHigherLevel;
                                    if (female.levelsWild[s] == bestLevels[s])
                                        topFemale++;
                                    if (male.levelsWild[s] == bestLevels[s])
                                        topMale++;
                                }
                            }
                        }
                        t += tt;
                    }

                    if (breedingMode == BreedingPlan.BreedingMode.TopStatsConservative)
                    {
                        if (topFemale < nrTS && topMale < nrTS)
                            t += eTS;
                        else
                            t += .1 * eTS;
                        // check if the best possible stat outcome regarding topLevels already exists in a male
                        bool maleExists = false;

                        foreach (Creature cr in males)
                        {
                            maleExists = true;
                            for (int s = 0; s < Values.STATS_COUNT; s++)
                            {
                                if (s == (int)StatNames.Torpidity
                                    || !cr.Species.UsesStat(s)
                                    || cr.levelsWild[s] == bestPossLevels[s]
                                    || bestPossLevels[s] != bestLevels[s])
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
                                for (int s = 0; s < Values.STATS_COUNT; s++)
                                {
                                    if (s == (int)StatNames.Torpidity
                                        || !cr.Species.UsesStat(s)
                                        || cr.levelsWild[s] == bestPossLevels[s]
                                        || bestPossLevels[s] != bestLevels[s])
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


                    int mutationPossibleFrom = female.Mutations < BreedingPlan.MutationPossibleWithLessThan && male.Mutations < BreedingPlan.MutationPossibleWithLessThan ? 2
                        : female.Mutations < BreedingPlan.MutationPossibleWithLessThan || male.Mutations < BreedingPlan.MutationPossibleWithLessThan ? 1 : 0;

                    breedingPairs.Add(new BreedingPair(female, male, t * 1.25, (mutationPossibleFrom == 2 ? BreedingPlan.ProbabilityOfOneMutation : mutationPossibleFrom == 1 ? BreedingPlan.ProbabilityOfOneMutationFromOneParent : 0)));
                }
            }

            return breedingPairs.OrderByDescending(p => p.BreedingScore).ToList();
        }

        public static void SetBestLevels(IEnumerable<Creature> creatures, int[] bestLevels, double[] statWeights)
        {
            for (int s = 0; s < Values.STATS_COUNT; s++)
                bestLevels[s] = -1;

            foreach (Creature c in creatures)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if ((s == (int)StatNames.Torpidity || statWeights[s] >= 0) && c.levelsWild[s] > bestLevels[s])
                        bestLevels[s] = c.levelsWild[s];
                    else if (s != (int)StatNames.Torpidity && statWeights[s] < 0 && c.levelsWild[s] >= 0 && (c.levelsWild[s] < bestLevels[s] || bestLevels[s] < 0))
                        bestLevels[s] = c.levelsWild[s];
                }
            }
        }
    }
}
