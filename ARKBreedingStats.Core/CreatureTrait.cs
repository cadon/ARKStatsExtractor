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
            return $"{TraitDefinition?.Name ?? "unknown trait id: " + Id} (T{Tier + 1})";
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

        public CreatureTrait(string traitId, int tier = 0)
        {
            TraitDefinition = TraitDefinition.GetTraitDefinition(traitId);
            Id = traitId;
            Tier = (byte)tier;
            InheritHigherProbability = TraitDefinition?.InheritHigherProbability?[tier] ?? 0;
            MutationProbability = TraitDefinition?.MutationProbability?[tier] ?? 0;
        }

        public static CreatureTrait TryParse(string traitDefinitionString)
        {
            if (string.IsNullOrEmpty(traitDefinitionString)) return null;
            var bracketIndex = traitDefinitionString.IndexOf("[");
            string id;
            byte tier;
            if (bracketIndex == -1)
            {
                id = traitDefinitionString;
                tier = 0;
            }
            else
            {
                id = traitDefinitionString.Substring(0, bracketIndex);
                tier = (byte)(int.TryParse(traitDefinitionString.Substring(bracketIndex + 1, 1), out var tierParsed)
                    ? tierParsed
                    : 0);
            }

            return new CreatureTrait(id, tier);
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
