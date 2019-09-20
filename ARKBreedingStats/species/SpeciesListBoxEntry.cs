namespace ARKBreedingStats.species
{
    /// <summary>
    /// Currently not used.
    /// </summary>
    class SpeciesListBoxEntry
    {
        public Species species;
        public override string ToString()
        {
            return species.name;
        }

        public SpeciesListBoxEntry(Species species)
        {
            this.species = species;
        }
    }
}
