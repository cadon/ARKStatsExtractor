using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ARKBreedingStats.SpeciesOptions
{
    /// <summary>
    /// Options array for a species, e.g. per stat like breeding stat weights, graph representation, top stat considerations, or colors.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class SpeciesOptionsBase<T> where T : SpeciesOptionBase
    {
        protected SpeciesOptionsBase(int optionsCount)
        {
            Options = new T[optionsCount];
        }

        /// <summary>
        /// Name of the stats options, usually a species name or a group.
        /// </summary>
        [JsonProperty]
        public string Name;

        public override string ToString() => string.IsNullOrEmpty(Name) ? $"<{Loc.S("default")}>" : new string(' ', HierarchyLevel * 2) + Name;

        /// <summary>
        /// Name of the parent setting.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ParentName;

        public SpeciesOptionsBase<T> ParentOptions;

        /// <summary>
        /// Array of options, using old json node name for backwards compatibility, conversion was 2026-02, may be removed after some months
        /// </summary>
        [JsonProperty("StatOptions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private T[] _speciesOptionElementsBackwardCompatibility;

        /// <summary>
        /// Array of options, e.g. one option for each stat or color.
        /// </summary>
        [JsonProperty("options", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public T[] Options;

        [OnDeserialized]
        private void BackwardCompatibility(StreamingContext _)
        {
            if (_speciesOptionElementsBackwardCompatibility == null) return;
            Options = _speciesOptionElementsBackwardCompatibility;
            _speciesOptionElementsBackwardCompatibility = null;
        }

        /// <summary>
        /// List of species these settings are valid.
        /// Possible values are the blueprint path and shorter defined names, e.g. Species.name, Species.DescriptiveName.
        /// </summary>
        [JsonProperty("sp", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string[] AffectedSpecies;

        /// <summary>
        /// Used for UI layout.
        /// </summary>
        public int HierarchyLevel;
    }
}
