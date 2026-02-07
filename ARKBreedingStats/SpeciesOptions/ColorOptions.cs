namespace ARKBreedingStats.SpeciesOptions
{
    /// <summary>
    /// Color options for specific species.
    /// </summary>
    /// <typeparam name="T">Specific type of species color options</typeparam>
    public class ColorOptions<T> : SpeciesOptionsBase<T> where T : SpeciesOptionBase
    {
        public ColorOptions() : base(Ark.ColorRegionCount) { }
    }
}