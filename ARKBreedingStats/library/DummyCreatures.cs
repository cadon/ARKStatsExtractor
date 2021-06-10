using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
        public static Creature[] CreateArray(int count = 10, Species species = null, int numberSpecies = 10, bool breedThem = false, double mutationProbability = 0.05)
        {
            if (count < 1) return null;

            if (_levelInverseCumulativeFunction == null)
                InitializeLevelFunction();

            var creatures = new Creature[count];

            var rand = new Random();

            var randomSpecies = species == null;
            Species[] speciesSelection = null;
            var speciesCount = 0;

            if (randomSpecies)
            {
                speciesSelection = values.Values.V.species.Where(s => s.IsDomesticable && !s.name.Contains("Tek") && !s.name.Contains("Alpha") && (s.variants?.Length ?? 0) < 2).ToArray();
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

            var levelStep = CreatureCollection.CurrentCreatureCollection?.wildLevelStep ?? 5;
            var maxWildLevel = CreatureCollection.CurrentCreatureCollection?.maxWildLevel ?? 150;
            var difficulty = maxWildLevel / 30d;

            var nameCounter = new Dictionary<string, int>();

            for (int i = 0; i < count; i++)
            {
                if (randomSpecies)
                    species = speciesSelection[rand.Next(speciesCount)];

                // only "tame" higher creatures
                //var creatureLevel = (1 + rand.Next(30)) * difficulty;
                var creatureLevel = (21 + rand.Next(10)) * difficulty;
                var tamingEffectiveness = 0.5 + rand.NextDouble() / 2; // assume at least 50 % te
                creatureLevel *= (1 + 0.5 * tamingEffectiveness);

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

                var sex = rand.Next(2) == 0 ? Sex.Female : Sex.Male;
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

                var colors = new int[Species.ColorRegionCount];
                for (int ci = 0; ci < Species.ColorRegionCount; ci++)
                {
                    if (!species.EnabledColorRegions[ci]) continue;
                    var colorCount = species.colors[ci]?.naturalColors?.Count ?? 0;
                    if (colorCount == 0)
                        colors[ci] = rand.Next(50);
                    else colors[ci] = species.colors[ci].naturalColors[rand.Next(colorCount)].Id;
                }

                creature.colors = colors;

                creatures[i] = creature;
            }

            if (breedThem) return BreedCreatures(creatures, mutationProbability);

            return creatures;
        }

        /// <summary>
        /// Combine pairs according to their breeding score and create probable offspring.
        /// </summary>
        /// <param name="creatures"></param>
        /// <param name="mutationProbability"></param>
        /// <returns></returns>
        private static Creature[] BreedCreatures(Creature[] creatures, double mutationProbability)
        {
            // TODO
            return creatures;


            //creature.RecalculateNewMutations();
            //creature.RecalculateAncestorGenerations();
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
}
