using ARKBreedingStats.BreedingPlanning;
using ARKBreedingStats.Library;

namespace ARKBreedingStats.species
{
    public class BreedingPair
    {
        public readonly Creature Mother;
        public readonly Creature Father;
        public Score BreedingScore;
        /// <summary>
        /// Probability of at least one mutation for the offspring.
        /// </summary>
        public double MutationProbability;
        /// <summary>
        /// The highest possible offspring is over the level limit if all possible dom levels are applied (server setting).
        /// </summary>
        public bool HighestOffspringOverLevelLimit;

        public BreedingPair(Creature mother, Creature father, Score breedingScore, double mutationProbability, bool highestOffspringOverLevelLimit)
        {
            Mother = mother;
            Father = father;
            BreedingScore = breedingScore;
            MutationProbability = mutationProbability;
            HighestOffspringOverLevelLimit = highestOffspringOverLevelLimit;
        }
    }
}
