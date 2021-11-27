using System;
using System.Collections.Generic;
using System.Linq;
using ARKBreedingStats.Ark;
using ARKBreedingStats.BreedingPlanning;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;

namespace ARKBreedingStats.library
{
    /// <summary>
    /// Creates dummy creatures to populate a library.
    /// </summary>
    public static class DummyCreatures
    {
        public static DummyCreatureCreationSettings LastSettings;

        public static List<Creature> CreateCreatures(int count, Species species, int numberSpecies, int breedGenerations, int usePairsPerGeneration, double probabilityHigherStat = 0.55, double randomMutationChance = 0.025, int maxWildLevel = 150)
        {
            if (count < 1) return null;

            LastSettings = new DummyCreatureCreationSettings
            {
                CreatureCount = count,
                OnlySelectedSpecies = species != null,
                SpeciesCount = numberSpecies,
                Generations = breedGenerations,
                PairsPerGeneration = usePairsPerGeneration,
                ProbabilityHigherStat = probabilityHigherStat,
                RandomMutationChance = randomMutationChance,
                MaxWildLevel = maxWildLevel
            };

            if (_levelInverseCumulativeFunction == null)
                InitializeLevelFunction();

            var creatures = new List<Creature>(count);

            var rand = new Random();

            var randomSpecies = species == null;
            Species[] speciesSelection = null;
            var speciesCount = 0;

            if (randomSpecies)
            {
                speciesSelection = Values.V.species.Where(s => s.IsDomesticable && !s.name.Contains("Tek") && !s.name.Contains("Alpha") && (s.variants?.Length ?? 0) < 2).ToArray();
                speciesCount = speciesSelection.Length;
                if (speciesCount > numberSpecies)
                {
                    var speciesLeft = numberSpecies;
                    var speciesIndices = new List<int>(numberSpecies);

                    while (speciesLeft > 0)
                    {
                        var i = rand.Next(speciesCount);
                        if (speciesIndices.Contains(i)) continue;
                        speciesIndices.Add(i);
                        speciesLeft--;
                    }

                    speciesSelection = speciesIndices.Select(i => speciesSelection[i]).ToArray();
                    speciesCount = speciesSelection.Length;
                }
            }

            if (maxWildLevel < 1)
                maxWildLevel = CreatureCollection.CurrentCreatureCollection?.maxWildLevel ?? 150;
            var difficulty = maxWildLevel / 30d;
            var levelStep = (int)difficulty;

            var nameCounter = new Dictionary<string, int>();

            for (int i = 0; i < count; i++)
            {
                if (randomSpecies)
                    species = speciesSelection[rand.Next(speciesCount)];

                // rather "tame" higher creatures
                var creatureLevel = (rand.Next(5) == 0 ? rand.Next(21) + 1 : 21 + rand.Next(10)) * difficulty;
                var tamingEffectiveness = 0.5 + rand.NextDouble() / 2; // assume at least 50 % te
                creatureLevel *= 1 + 0.5 * tamingEffectiveness;

                var levelFactor = creatureLevel / _totalLevels;
                var levelsWild = new int[Values.STATS_COUNT];
                var levelsDom = new int[Values.STATS_COUNT];
                var torpidityLevel = 0;
                for (int si = 0; si < Values.STATS_COUNT; si++)
                {
                    if (!species.UsesStat(si) || si == (int)StatNames.Torpidity) continue;
                    var level = (int)(levelFactor * GetBinomialLevel(rand));
                    torpidityLevel += level;
                    levelsWild[si] = level;
                }
                levelsWild[(int)StatNames.Torpidity] = torpidityLevel;

                var sex = species.noGender ? Sex.Unknown : rand.Next(2) == 0 ? Sex.Female : Sex.Male;
                var names = sex == Sex.Female ? _namesFemale : _namesMale;
                var name = names[rand.Next(names.Length)];
                if (nameCounter.TryGetValue(name, out var nameCount))
                {
                    nameCounter[name]++;
                    name += $" {nameCount + 1}";
                }
                else
                {
                    nameCounter.Add(name, 1);
                }

                var creature = new Creature(species, name, sex: sex, levelsWild: levelsWild,
                    levelsDom: levelsDom, tamingEff: tamingEffectiveness)
                {
                    guid = Guid.NewGuid()
                };
                creature.RecalculateCreatureValues(levelStep);

                creature.colors = species.RandomSpeciesColors(rand);

                creatures.Add(creature);
            }

            if (breedGenerations > 0)
            {
                var creaturesBySpecies = creatures.GroupBy(c => c.Species).ToDictionary(g => g.Key, g => g.ToArray());

                foreach (var s in creaturesBySpecies)
                {
                    var newCreatures = BreedCreatures(s.Value, s.Key, breedGenerations,
                        usePairsPerGeneration, probabilityHigherStat, randomMutationChance);
                    if (newCreatures != null)
                    {
                        creatures.AddRange(newCreatures);
                    }
                }
            }

            return creatures;
        }

        /// <summary>
        /// Combine pairs according to their breeding score and create probable offspring. Only the new creatures are returned.
        /// </summary>
        private static List<Creature> BreedCreatures(Creature[] creatures, Species species, int generations, int usePairsPerGeneration, double probabilityHigherStat = 0.55, double randomMutationChance = 0.025)
        {
            var noGender = species.noGender;

            var femalesMales = creatures.GroupBy(c => c.sex).ToDictionary(g => g.Key, g => g.ToList());
            if ((noGender && creatures.Length < 2)
                || (!noGender && (
                    !femalesMales.ContainsKey(Sex.Female)
                    || !femalesMales.ContainsKey(Sex.Male))))
            {
                return null;
            }

            var newCreatures = new List<Creature>();
            var rand = new Random();
            var levelStep = CreatureCollection.CurrentCreatureCollection?.wildLevelStep ?? 5;
            var bestLevels = new int[Values.STATS_COUNT];
            var statWeights = new double[Values.STATS_COUNT];
            for (int si = 0; si < Values.STATS_COUNT; si++) statWeights[si] = 1;

            // these variables are not used but needed for the method
            var filteredOutByMutationLimit = false;
            var bestPossibleLevels = new short[Values.STATS_COUNT];
            List<Creature> allCreatures = null;
            for (int gen = 0; gen < generations; gen++)
            {
                if (noGender)
                {
                    if (allCreatures == null)
                        allCreatures = creatures.ToList();
                }
                else
                {
                    allCreatures = femalesMales[Sex.Female].ToList();
                    allCreatures.AddRange(femalesMales[Sex.Male]);
                }

                BreedingScore.SetBestLevels(allCreatures, bestLevels, statWeights);

                var allCreaturesArray = noGender ? allCreatures.ToArray() : null;
                var pairs = BreedingScore.CalculateBreedingScores(noGender ? allCreaturesArray : femalesMales[Sex.Female].ToArray(),
                    noGender ? allCreaturesArray : femalesMales[Sex.Male].ToArray(), species, bestPossibleLevels, statWeights, bestLevels,
                    BreedingPlan.BreedingMode.TopStatsConservative, false, false, 0, ref filteredOutByMutationLimit);

                var pairsCount = Math.Min(usePairsPerGeneration, pairs.Count);
                for (int i = 0; i < pairsCount; i++)
                {
                    var mother = pairs[i].Female;
                    var father = pairs[i].Male;

                    var mutationsMaternal = mother.Mutations;
                    var mutationsPaternal = father.Mutations;
                    var mutationPossible = mutationsMaternal < GameConstants.MutationPossibleWithLessThan || mutationsPaternal < GameConstants.MutationPossibleWithLessThan;

                    var name = $"F{gen + 1}.{i + 1}";
                    var sex = noGender ? Sex.Unknown : rand.Next(2) == 0 ? Sex.Female : Sex.Male;

                    // stats
                    var levelsWild = new int[Values.STATS_COUNT];
                    var torpidityLevel = 0;
                    var statIndicesForPossibleMutation = mutationPossible ? new List<int>(Values.STATS_COUNT) : null;
                    for (int si = 0; si < Values.STATS_COUNT; si++)
                    {
                        if (!species.UsesStat(si) || si == (int)StatNames.Torpidity) continue;

                        var level = rand.NextDouble() < probabilityHigherStat
                            ? Math.Max(mother.levelsWild[si], father.levelsWild[si])
                            : Math.Min(mother.levelsWild[si], father.levelsWild[si]);
                        torpidityLevel += level;
                        levelsWild[si] = level;
                        if (mutationPossible && species.stats[si].AddWhenTamed != 0)
                            statIndicesForPossibleMutation.Add(si);
                    }

                    levelsWild[(int)StatNames.Torpidity] = torpidityLevel;

                    // colors
                    var colorRegionsForPossibleMutation = mutationPossible ? new List<int>() : null;
                    var colors = new byte[Species.ColorRegionCount];
                    for (int ci = 0; ci < Species.ColorRegionCount; ci++)
                    {
                        if (!species.EnabledColorRegions[ci]) continue;
                        colors[ci] = rand.Next(2) == 0 ? mother.colors[ci] : father.colors[ci];
                        if (mutationPossible)
                            colorRegionsForPossibleMutation.Add(ci);
                    }

                    // mutations
                    var mutationHappened = false;
                    var statForPossibleMutationCount = mutationPossible ? statIndicesForPossibleMutation.Count : 0;
                    if (statForPossibleMutationCount != 0)
                    {
                        for (int m = 0; m < GameConstants.MutationRolls; m++)
                        {
                            // first select a stat
                            var statIndexForMutation = statIndicesForPossibleMutation[rand.Next(statForPossibleMutationCount)];

                            // mutation is tied to parent, the one with the higher level has a higher chance
                            var mutationFromParentWithHigherStat = rand.NextDouble() < probabilityHigherStat;
                            var mutationFromMother = mutationFromParentWithHigherStat == (mother.levelsWild[statIndexForMutation] >
                                                    father.levelsWild[statIndexForMutation]);

                            if ((mutationFromMother && mother.Mutations >= GameConstants.MutationPossibleWithLessThan)
                                || (!mutationFromMother && father.Mutations >= GameConstants.MutationPossibleWithLessThan)
                            ) continue;

                            // check if mutation occurs
                            if (rand.NextDouble() >= randomMutationChance) continue;

                            var newLevel = levelsWild[statIndexForMutation] + GameConstants.LevelsAddedPerMutation;
                            if (newLevel > 255) continue;

                            mutationHappened = true;
                            levelsWild[statIndexForMutation] = newLevel;
                            if (mutationFromMother) mutationsMaternal++;
                            else mutationsPaternal++;

                            var colorRegionsForMutationsCount = colorRegionsForPossibleMutation.Count;
                            if (colorRegionsForMutationsCount != 0)
                            {
                                var mutatedRegion = colorRegionsForPossibleMutation[rand.Next(colorRegionsForMutationsCount)];
                                colors[mutatedRegion] = (byte)rand.Next(100); // for now considering all color ids up to 99
                            }

                        }
                    }

                    var creature = new Creature(species, name, sex: sex, levelsWild: levelsWild, isBred: true)
                    {
                        guid = Guid.NewGuid(),
                        mutationsMaternal = mutationsMaternal,
                        mutationsPaternal = mutationsPaternal,
                        Mother = mother,
                        Father = father,
                        colors = colors
                    };
                    creature.RecalculateCreatureValues(levelStep);

                    if (mutationHappened)
                        creature.RecalculateNewMutations();

                    creature.RecalculateAncestorGenerations();

                    if (noGender)
                        allCreatures.Add(creature);
                    else
                    {
                        if (creature.sex == Sex.Female)
                            femalesMales[Sex.Female].Add(creature);
                        else
                            femalesMales[Sex.Male].Add(creature);
                    }

                    newCreatures.Add(creature);
                }
            }

            return newCreatures;

        }

        private static readonly string[] _namesFemale = { "Aurora", "Bess", "Bones", "Breeze", "Casey", "Casia", "Catlin", "Chromy", "Chuckles", "Cosmo", "Cupcake", "Danele", "Daphne", "Durva", "Electra", "Ellie", "Elora", "Flare", "Ginger", "Hope", "Indigo", "Jackie", "Layka", "Myst", "Nectar", "Oracle", "Pandora", "Peachy", "Peanuts", "Princess", "Raye", "Sabre", "Shellbie", "Shine", "Tia", "Vanity", "Wilde", "Zara" };
        private static readonly string[] _namesMale = { "Austin", "Bran", "Cosmo", "Dearborn", "Eclipse", "Fuzz", "Gazoo", "Hercules", "Indy", "Jiggles", "Lightning", "Marble", "Noah", "Pepper", "Rancher", "Sparkler", "Tweeter", "Whiskers", "Zion" };

        #region Binomial distributed levels

        private static int GetBinomialLevel(Random rand)
        {
            return _levelInverseCumulativeFunction[rand.Next(MaxSteps)];
        }

        private static int[] _levelInverseCumulativeFunction;

        /// <summary>
        /// Steps of the binomial distribution. The higher the value, the more granular the results.
        /// </summary>
        private const int MaxSteps = 10000;
        private const int _totalLevels = 150;

        private static void InitializeLevelFunction()
        {
            const int possibleStatsToDistributeLevels = 7;

            // inverse function
            _levelInverseCumulativeFunction = new int[MaxSteps];

            // cumulative distribution function
            //var cumulativeDF = new double[totalLevels];
            var probability = 1d / possibleStatsToDistributeLevels;
            var sum = 0d;
            var currentStep = 0;
            for (int level = 0; level <= _totalLevels; level++)
            {
                sum += Bernoulli(probability, _totalLevels, level);
                //cumulativeDF[level] = sum;
                //Console.WriteLine(new string('♥', (int)(10 * sum)));

                var upToStep = (int)(sum * MaxSteps) + 1;
                if (upToStep > MaxSteps) upToStep = MaxSteps;
                for (int s = currentStep; s < upToStep; s++)
                    _levelInverseCumulativeFunction[s] = level;

                if (upToStep == MaxSteps)
                    break;
                currentStep = upToStep;
            }
        }

        private static double Bernoulli(double probability, int trials, int successes)
        {
            return Binomial(trials, successes) * Math.Pow(probability, successes) *
                   Math.Pow(1 - probability, trials - successes);
        }

        private static double Binomial(int n, int k)
        {
            double result = 1;
            for (int i = k; i > 0; i--)
            {
                result = result * (n - (k - i)) / i;
            }
            return result;
        }

        #endregion
    }

    /// <summary>
    /// Settings that were used the last time.
    /// </summary>
    public class DummyCreatureCreationSettings
    {
        public DummyCreatureCreationSettings()
        {
            CreatureCount = 20;
            OnlySelectedSpecies = true;
            SpeciesCount = 10;
            Generations = 4;
            PairsPerGeneration = 2;
            ProbabilityHigherStat = GameConstants.ProbabilityHigherLevel;
            RandomMutationChance = GameConstants.ProbabilityOfMutation;
            MaxWildLevel = CreatureCollection.CurrentCreatureCollection?.maxWildLevel ?? 150;
        }
        public int CreatureCount;
        public bool OnlySelectedSpecies;
        public int SpeciesCount;
        public int Generations;
        public int PairsPerGeneration;
        public double ProbabilityHigherStat;
        public double RandomMutationChance;
        public int MaxWildLevel;
    }
}
