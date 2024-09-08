using Newtonsoft.Json;

namespace ARKBreedingStats.library
{
    /// <summary>
    /// Can give bonus or malus on inheritance or mutations.
    /// </summary>
    [JsonObject]
    public class CreatureTrait
    {
        [JsonProperty("n")]
        public string Name;
        [JsonProperty("si")]
        public int StatIndex;
        [JsonProperty("t")]
        public byte Tier;
        /// <summary>
        /// Robust
        /// </summary>
        [JsonProperty("r", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double InheritHigherLevelBonus;
        /// <summary>
        /// Frail
        /// </summary>
        [JsonProperty("f", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double InheritLowerLevelBonus;
        /// <summary>
        /// Mutable
        /// </summary>
        [JsonProperty("m", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double MutationChanceBonus;
    }
}
