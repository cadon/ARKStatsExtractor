using System.Collections.Generic;
using Newtonsoft.Json;

namespace ARKBreedingStats.Traits
{
    /// <summary>
    /// Definition of creature traits.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class TraitDefinition
    {
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

        private static Dictionary<string, TraitDefinition> _traitDefinitions;

        public static void LoadTraitDefinitions()
        {
            FileService.LoadJsonFile(FileService.GetJsonPath(FileService.TraitDefinitionsFile), out _traitDefinitions, out _);
            if (_traitDefinitions == null) return;

            foreach (var t in _traitDefinitions.Values)
            {
                if (!string.IsNullOrEmpty(t.BaseId)
                    && _traitDefinitions.TryGetValue(t.BaseId, out var baseTrait))
                {
                    if (string.IsNullOrEmpty(t.Name)) t.Name = baseTrait.Name;
                    if (string.IsNullOrEmpty(t.Description)) t.Description = baseTrait.Description;
                    if (string.IsNullOrEmpty(t.Effect)) t.Effect = baseTrait.Effect;
                    if (t.MutationProbability == null) t.MutationProbability = baseTrait.MutationProbability;
                    if (t.InheritHigherProbability == null) t.InheritHigherProbability = baseTrait.InheritHigherProbability;
                    if (t.MaxCopies == -1) t.MaxCopies = baseTrait.MaxCopies;
                }

                if (t.StatIndex >= 0)
                {
                    var statName = Utils.StatName(t.StatIndex);
                    t.Name = t.Name.Replace("%s", statName);
                    t.Description = t.Description.Replace("%s", statName);
                    t.Effect = t.Effect.Replace("%s", statName);
                }
            }
        }

        public static TraitDefinition GetTraitDefinition(string id)
        {
            if (!string.IsNullOrEmpty(id) && _traitDefinitions.TryGetValue(id, out var traitDefinition))
                return traitDefinition;
            return null;
        }
    }
}
