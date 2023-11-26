using System;
using System.Collections.Generic;
using System.Linq;
using ARKBreedingStats.BreedingPlanning;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace ARKBreedingStats.library
{
    /// <summary>
    /// Creates dummy creatures and simulates breeding to populate a library.
    /// </summary>
    public static class DummyCreatures
    {
        public static DummyCreatureCreationSettings LastSettings;

        /// <summary>
        /// Creates a list of random creatures.
        /// </summary>
        /// <param name="count">Number of creatures</param>
        /// <param name="species">If null, species will be selected randomly (domesticable preferred)</param>
        /// <param name="numberSpecies">If species are randomly selected, this is the number of different species</param>
        /// <param name="breedGenerations">If &gt; 0, the creatures will be bred according to the breeding planner, the offspring will also be returned.</param>
        /// <param name="usePairsPerGeneration">If bred, this indicates how many of the top breeding pairs will be used to breed</param>
        /// <param name="probabilityHigherStat"></param>
        /// <param name="randomMutationChance"></param>
        /// <param name="maxWildLevel"></param>
        /// <returns></returns>
        public static List<Creature> CreateCreatures(int count, Species species = null, int numberSpecies = 1,
            int breedGenerations = 0, int usePairsPerGeneration = 2, double probabilityHigherStat = 0.55, double randomMutationChance = 0.025, int maxWildLevel = 150,
            bool setOwner = true, bool setTribe = true, bool setServer = true)
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
                MaxWildLevel = maxWildLevel,
                SetOwner = setOwner,
                SetTribe = setTribe,
                SetServer = setServer
            };

            var creatures = new List<Creature>(count);

            var rand = new Random();

            var randomSpecies = species == null;
            Species[] speciesSelection = null;
            var speciesCount = 0;

            if (randomSpecies)
            {
                if (numberSpecies < 1) numberSpecies = 1;
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

            var nameCounter = new Dictionary<string, int>();

            for (int i = 0; i < count; i++)
            {
                if (randomSpecies)
                    species = speciesSelection[rand.Next(speciesCount)];

                creatures.Add(CreateCreature(species, difficulty, true, rand, setOwner, setTribe, setServer, nameCounter));
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
        /// Creates a creature for testing.
        /// </summary>
        public static Creature CreateCreature(Species species, double difficulty = 5, bool doTame = true, Random rand = null, bool setOwner = true, bool setTribe = true, bool setServer = true, Dictionary<string, int> nameCounter = null)
        {
            if (rand == null) rand = new Random();

            // rather "tame" higher creatures. Base levels are 1-30, scaled by difficulty
            var creatureLevel = (rand.Next(5) == 0 ? rand.Next(21) + 1 : 21 + rand.Next(10)) * difficulty;
            var tamingEffectiveness = -3d; // indicating wild
            if (doTame)
            {
                tamingEffectiveness = 0.5 + rand.NextDouble() / 2; // assume at least 50 % te
                creatureLevel *= 1 + 0.5 * tamingEffectiveness;
            }

            var levelFactor = creatureLevel / _totalLevels;
            var levelsWild = new int[Stats.StatsCount];
            //var levelsMut = new int[Stats.StatsCount];
            var levelsDom = new int[Stats.StatsCount];
            var torpidityLevel = 0;
            for (int si = 0; si < Stats.StatsCount; si++)
            {
                if (!species.UsesStat(si) || !species.CanLevelUpWildOrHaveMutations(si) || si == Stats.Torpidity) continue;
                var level = (int)(levelFactor * GetBinomialLevel(rand));
                torpidityLevel += level;
                levelsWild[si] = level;
            }
            levelsWild[Stats.Torpidity] = torpidityLevel;

            var sex = species.noGender ? Sex.Unknown : rand.Next(2) == 0 ? Sex.Female : Sex.Male;
            string name = null;
            if (doTame)
            {
                var names = sex == Sex.Female ? _namesFemale : _namesMale;
                name = names[rand.Next(names.Length)];
                if (nameCounter != null)
                {
                    if (nameCounter.TryGetValue(name, out var nameCount))
                    {
                        nameCounter[name]++;
                        name += $" {nameCount + 1}";
                    }
                    else
                    {
                        nameCounter.Add(name, 1);
                    }
                }
            }

            var creature = new Creature(species, name, sex: sex, levelsWild: levelsWild,
                levelsDom: levelsDom, tamingEff: tamingEffectiveness)
            {
                guid = Guid.NewGuid(),
                ArkId = Utils.ConvertArkIdsToLongArkId(rand.Next(), rand.Next())
            };
            creature.RecalculateCreatureValues((int)difficulty);

            creature.colors = species.RandomSpeciesColors(rand);
            if (setOwner)
                creature.owner = $"Player {rand.Next(5) + 1}";
            if (setTribe)
                creature.tribe = $"Tribe {rand.Next(5) + 1}";
            if (setServer)
                creature.server = $"Server {rand.Next(5) + 1}";

            creature.InitializeFlags();

            return creature;
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
            var bestLevels = new int[Stats.StatsCount];
            var statWeights = new double[Stats.StatsCount];
            for (int si = 0; si < Stats.StatsCount; si++) statWeights[si] = 1;

            // these variables are not used but needed for the method
            var filteredOutByMutationLimit = false;
            var bestPossibleLevels = new short[Stats.StatsCount];
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
                    var mother = pairs[i].Mother;
                    var father = pairs[i].Father;

                    var mutationsMaternal = mother.Mutations;
                    var mutationsPaternal = father.Mutations;
                    var mutationPossible = mutationsMaternal < Ark.MutationPossibleWithLessThan || mutationsPaternal < Ark.MutationPossibleWithLessThan;

                    var name = $"F{gen + 1}.{i + 1}";
                    var sex = noGender ? Sex.Unknown : rand.Next(2) == 0 ? Sex.Female : Sex.Male;

                    // stats
                    var levelsWild = new int[Stats.StatsCount];
                    var torpidityLevel = 0;
                    var statIndicesForPossibleMutation = mutationPossible ? new List<int>(Stats.StatsCount) : null;
                    for (int si = 0; si < Stats.StatsCount; si++)
                    {
                        if (!species.UsesStat(si) || !species.CanLevelUpWildOrHaveMutations(si) || si == Stats.Torpidity) continue;

                        var level = rand.NextDouble() < probabilityHigherStat
                            ? Math.Max(mother.levelsWild[si], father.levelsWild[si])
                            : Math.Min(mother.levelsWild[si], father.levelsWild[si]);
                        torpidityLevel += level;
                        levelsWild[si] = level;
                        if (mutationPossible && species.stats[si].AddWhenTamed != 0)
                            statIndicesForPossibleMutation.Add(si);
                    }

                    levelsWild[Stats.Torpidity] = torpidityLevel;

                    // colors
                    var colorRegionsForPossibleMutation = mutationPossible ? new List<int>() : null;
                    var colors = new byte[Ark.ColorRegionCount];
                    for (int ci = 0; ci < Ark.ColorRegionCount; ci++)
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
                        for (int m = 0; m < Ark.MutationRolls; m++)
                        {
                            // first select a stat
                            var statIndexForMutation = statIndicesForPossibleMutation[rand.Next(statForPossibleMutationCount)];

                            // mutation is tied to parent, the one with the higher level has a higher chance
                            var mutationFromParentWithHigherStat = rand.NextDouble() < probabilityHigherStat;
                            var mutationFromMother = mutationFromParentWithHigherStat == (mother.levelsWild[statIndexForMutation] >
                                                    father.levelsWild[statIndexForMutation]);

                            if ((mutationFromMother && mother.Mutations >= Ark.MutationPossibleWithLessThan)
                                || (!mutationFromMother && father.Mutations >= Ark.MutationPossibleWithLessThan)
                            ) continue;

                            // check if mutation occurs
                            if (rand.NextDouble() >= randomMutationChance) continue;

                            var newLevel = levelsWild[statIndexForMutation] + Ark.LevelsAddedPerMutation;
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
                        colors = colors,
                        owner = mother.owner ?? father.owner,
                        tribe = mother.tribe ?? father.tribe,
                        server = mother.server ?? father.server
                    };
                    creature.RecalculateCreatureValues(levelStep);

                    if (mutationHappened)
                        creature.RecalculateNewMutations();

                    creature.RecalculateAncestorGenerations();
                    creature.InitializeFlags();

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

        /// <summary>
        /// Used to get binomial distributed levels.
        /// </summary>
        private static int GetBinomialLevel(Random rand)
        {
            if (_levelInverseCumulativeFunction == null)
                InitializeLevelFunction();
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
            ProbabilityHigherStat = Ark.ProbabilityInheritHigherLevel;
            RandomMutationChance = Ark.ProbabilityOfMutation;
            MaxWildLevel = CreatureCollection.CurrentCreatureCollection?.maxWildLevel ?? 150;
            SetOwner = true;
            SetTribe = true;
            SetServer = true;
        }
        public int CreatureCount;
        public bool OnlySelectedSpecies;
        public int SpeciesCount;
        public int Generations;
        public int PairsPerGeneration;
        public double ProbabilityHigherStat;
        public double RandomMutationChance;
        public int MaxWildLevel;
        public bool SetOwner;
        public bool SetTribe;
        public bool SetServer;
    }
}
