using ARKBreedingStats.Library;

namespace ARKBreedingStats.species
{
    public class BreedingPair
    {
        public readonly Creature Female;
        public readonly Creature Male;
        public double BreedingScore;

        public BreedingPair() { }

        public BreedingPair(Creature female, Creature male, double breedingScore)
        {
            Female = female;
            Male = male;
            BreedingScore = breedingScore;
        }
    }
}
