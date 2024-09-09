using Newtonsoft.Json;

namespace ARKBreedingStats.StatsOptions
{
    /// <summary>
    /// Options for stats of species, e.g. breeding stat weights, graph representation and top stat considerations.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class StatsOptions<T> where T : StatOptionsBase
    {
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

        public StatsOptions<T> ParentOptions;

        /// <summary>
        /// One option per stat.
        /// </summary>
        [JsonProperty]
        public T[] StatOptions;

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
