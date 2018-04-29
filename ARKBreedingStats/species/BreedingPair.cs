namespace ARKBreedingStats.species
{
    class BreedingPair
    {
        public Creature Female, Male;
        public double BreedingScore;

        public BreedingPair()
        {
        }

        public BreedingPair(Creature female, Creature male, double breedingScore)
        {
            Female = female;
            Male = male;
            BreedingScore = breedingScore;
        }
    }
}
