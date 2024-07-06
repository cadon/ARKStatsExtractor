namespace ARKBreedingStats.StatsOptions
{
    public abstract class StatOptionsBase
    {
        public abstract void Initialize();

        public abstract void PrepareForSaving();

        /// <summary>
        /// If true don't use values of parent but overrides of this object.
        /// </summary>
        public bool OverrideParent;

        /// <summary>
        /// Contains data and doesn't depend on parent data.
        /// </summary>
        public abstract bool DefinesData();
    }
}
