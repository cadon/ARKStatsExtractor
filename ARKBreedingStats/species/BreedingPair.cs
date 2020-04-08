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

        public BreedingPair(Creature female, Creature male, double breedingScore, double mutationProbability)
        {
            Female = female;
            Male = male;
            BreedingScore = breedingScore;
            MutationProbability = mutationProbability;
        }
    }
}
