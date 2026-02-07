namespace ARKBreedingStats.SpeciesOptions
{
    /// <summary>
    /// Stats options for specific species.
    /// </summary>
    /// <typeparam name="T">Specific type of species stats options.</typeparam>
    public class StatsOptions<T> : SpeciesOptionsBase<T> where T : SpeciesOptionBase
    {
        public StatsOptions() : base(Stats.StatsCount) { }
    }
}
