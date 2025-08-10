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
        /// If true this is a base trait definition which should not be displayed in the user interface and only used for other definitions as base.
        /// </summary>
        [JsonProperty("isBase")]
        public bool IsBase;

        public override string ToString()
        {
            return Name;
        }

        private static Dictionary<string, TraitDefinition> _traitDefinitions;

        public static void LoadTraitDefinitions()
        {
            FileService.LoadJsonFile(FileService.GetJsonPath(FileService.TraitDefinitionsFile), out _traitDefinitions, out _);
            if (_traitDefinitions == null) return;

            foreach (var t in _traitDefinitions)
            {
                var traitDef = t.Value;
                if (traitDef == null) continue;
                traitDef.Id = t.Key;
                if (!string.IsNullOrEmpty(traitDef.BaseId)
                    && _traitDefinitions.TryGetValue(traitDef.BaseId, out var baseTrait))
                {
                    if (string.IsNullOrEmpty(traitDef.Name)) traitDef.Name = baseTrait.Name;
                    if (string.IsNullOrEmpty(traitDef.Description)) traitDef.Description = baseTrait.Description;
                    if (string.IsNullOrEmpty(traitDef.Effect)) traitDef.Effect = baseTrait.Effect;
                    if (traitDef.MutationProbability == null) traitDef.MutationProbability = baseTrait.MutationProbability;
                    if (traitDef.InheritHigherProbability == null) traitDef.InheritHigherProbability = baseTrait.InheritHigherProbability;
                    if (traitDef.MaxCopies == -1) traitDef.MaxCopies = baseTrait.MaxCopies;
                }

                if (traitDef.StatIndex >= 0)
                {
                    var statName = Utils.StatName(traitDef.StatIndex);
                    traitDef.Name = traitDef.Name.Replace("%s", statName);
                    traitDef.Description = traitDef.Description.Replace("%s", statName);
                    traitDef.Effect = traitDef.Effect.Replace("%s", statName);
                }
            }
        }

        public static TraitDefinition GetTraitDefinition(string id)
        {
            if (!string.IsNullOrEmpty(id) && _traitDefinitions.TryGetValue(id, out var traitDefinition))
                return traitDefinition;
            return null;
        }

        public static TraitDefinition[] GetTraitDefinitions() => _traitDefinitions?.Values.ToArray();
    }
}
