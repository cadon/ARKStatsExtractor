namespace ARKBreedingStats.SpeciesOptions
{
    /// <summary>
    /// Base of species options, base of stat and color options.
    /// </summary>
    public abstract class SpeciesOptionBase
    {
        public abstract void Initialize();

        public abstract void PrepareForSaving(bool isRoot);

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
