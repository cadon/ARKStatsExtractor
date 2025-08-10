using System.Collections.Generic;
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

        public override string ToString()
        {
            return $"{TraitDefinition?.Name ?? "<unknown>"} (T{Tier + 1})";
        }

        [OnDeserialized]
        private void Initializing(StreamingContext _)
        {
            TraitDefinition = TraitDefinition.GetTraitDefinition(Id);
            InheritHigherProbability = TraitDefinition?.InheritHigherProbability?[Tier] ?? 0;
            MutationProbability = TraitDefinition?.MutationProbability?[Tier] ?? 0;
        }

        public CreatureTrait() { }

        public CreatureTrait(TraitDefinition traitDefinition, int tier = 0, string traitId = null)
        {
            TraitDefinition = traitDefinition;
            Id = traitId ?? traitDefinition?.Id;
            Tier = (byte)tier;
            InheritHigherProbability = traitDefinition?.InheritHigherProbability?[tier] ?? 0;
            MutationProbability = traitDefinition?.MutationProbability?[tier] ?? 0;
        }

        public static CreatureTrait TryParse(string traitDefinitionString)
        {
            if (string.IsNullOrEmpty(traitDefinitionString)) return null;
            var bracketIndex = traitDefinitionString.IndexOf("[");
            var id = bracketIndex == -1
               ? traitDefinitionString
               : traitDefinitionString.Substring(0, traitDefinitionString.IndexOf("["));
            var tier = (byte)(bracketIndex == -1
               ? 0
               : int.TryParse(traitDefinitionString.Substring(bracketIndex, 1), out var tierParsed) ? tierParsed : 0);

            return new CreatureTrait(TraitDefinition.GetTraitDefinition(id), tier, id);
        }

        /// <summary>
        /// Returns a humanly readable list of traits.
        /// </summary>
        public static string StringList(IEnumerable<CreatureTrait> traits, string separator = ", ") => traits == null ? string.Empty : string.Join(separator, traits);

        /// <summary>
        /// Returns the definition string, e.g. used by the export gun.
        /// </summary>
        public string ToDefinitionString() => $"{Id}[{Tier}]";
    }
}
