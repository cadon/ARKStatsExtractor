// TraitDefinition data model moved to ARKBreedingStats.Core/TraitDefinition.cs
// This file contains only the app-layer file loader.

using ARKBreedingStats.Core;
using System.Collections.Generic;

namespace ARKBreedingStats.Traits
{
    public static class TraitDefinitionLoader
    {
        public static void LoadTraitDefinitions()
        {
            FileService.LoadJsonFile(FileService.GetJsonPath(FileService.TraitDefinitionsFile), out Dictionary<string, TraitDefinition> defs, out _);
            if (defs == null) return;

            foreach (var t in defs)
            {
                var traitDef = t.Value;
                if (traitDef == null) continue;
                traitDef.Id = t.Key;
                if (!string.IsNullOrEmpty(traitDef.BaseId)
                    && defs.TryGetValue(traitDef.BaseId, out var baseTrait))
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

            TraitDefinition.SetTraitDefinitions(defs);
        }
    }
}
