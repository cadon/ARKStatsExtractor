using ARKBreedingStats.Library;

namespace ARKBreedingStats.species
{
    public class BreedingPair
    {
        public readonly Creature Female;
        public readonly Creature Male;
        public double BreedingScore;
        /// <summary>
        /// Probability of at least one mutation for the offspring.
        /// </summary>
        public double MutationProbability;
        /// <summary>
        /// The highest possible offspring is over the level limit if all possible dom levels are applied (server setting).
        /// </summary>
        public bool HighestOffspringOverLevelLimit;

        public BreedingPair(Creature female, Creature male, double breedingScore, double mutationProbability, bool highestOffspringOverLevelLimit)
        {
            Female = female;
            Male = male;
            BreedingScore = breedingScore;
            MutationProbability = mutationProbability;
            HighestOffspringOverLevelLimit = highestOffspringOverLevelLimit;
        }
    }
}
