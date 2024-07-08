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
        /// Name of the stats options, usually a species name.
        /// </summary>
        [JsonProperty]
        public string Name;

        public override string ToString() => string.IsNullOrEmpty(Name) ? $"<{Loc.S("default")}>" : Name;

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
    }
}
