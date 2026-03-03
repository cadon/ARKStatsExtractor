using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ARKBreedingStats.Traits
{
    /// <summary>
    /// Definition of creature traits.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class TraitDefinition
    {
        public string Id;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("description")]
        public string Description;
        /// <summary>
        /// Description of the effect.
        /// </summary>
        [JsonProperty("effect")]
        public string Effect;
        /// <summary>
        /// Amount of this trait a creature can maximally have.
        /// </summary>
        [JsonProperty("maxCopies")]
        public int MaxCopies = -1;
        /// <summary>
        /// Stat the trait has an effect on.
        /// </summary>
        [JsonProperty("statIndex")]
        public int StatIndex = -1;
        /// <summary>
        /// Additive probability to inherit the according stat.
        /// </summary>
        [JsonProperty("inheritHigherProbability")]
        public double[] InheritHigherProbability;
        /// <summary>
        /// Additive probability to mutate the according stat.
        /// </summary>
        [JsonProperty("mutationProbability")]
        public double[] MutationProbability;
        /// <summary>
        /// Id of Trait this trait is based on. This is used to reduce redundant definition.
        /// </summary>
        [JsonProperty("traitBase")]
        public string BaseId;
        /// <summary>
        /// If true this is a base trait definition which should not be displayed in the user interface
        /// and only used for other definitions as base.
        /// </summary>
        [JsonProperty("isBase")]
        public bool IsBase;

        public override string ToString() => Name;

        private static Dictionary<string, TraitDefinition> _traitDefinitions;

        /// <summary>
        /// Sets the loaded trait definitions. Called by the app-layer loader after file loading and
        /// stat-name substitution are complete.
        /// </summary>
        public static void SetTraitDefinitions(Dictionary<string, TraitDefinition> definitions)
        {
            _traitDefinitions = definitions;
        }

        public static TraitDefinition GetTraitDefinition(string id)
        {
            if (!string.IsNullOrEmpty(id) && _traitDefinitions != null
                && _traitDefinitions.TryGetValue(id, out var traitDefinition))
                return traitDefinition;
            return null;
        }

        public static TraitDefinition[] GetTraitDefinitions() => _traitDefinitions?.Values.ToArray();
    }
}
