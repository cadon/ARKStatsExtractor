using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ARKBreedingStats.Traits
{
    /// <summary>
    /// Can give bonus or malus on inheritance or mutations.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class CreatureTrait
    {
        [JsonProperty("id")]
        public string Id;
        public TraitDefinition TraitDefinition;
        /// <summary>
        /// Tier of the trait, 0-based.
        /// </summary>
        [JsonProperty("tier")]
        public byte Tier;
        /// <summary>
        /// Additive probability to inherit the according stat.
        /// </summary>
        public double InheritHigherProbability;
        /// <summary>
        /// Additive probability to mutate the according stat.
        /// </summary>
        public double MutationProbability;

        [OnDeserialized]
        private void Initializing()
        {
            TraitDefinition = TraitDefinition.GetTraitDefinition(Id);
            InheritHigherProbability = TraitDefinition?.InheritHigherProbability?[Tier] ?? 0;
            MutationProbability = TraitDefinition?.MutationProbability?[Tier] ?? 0;
        }
    }
}
