using System;
using System.Collections.Generic;
using System.Linq;
using ARKBreedingStats.Library;
using ARKBreedingStats.Models;

namespace ARKBreedingStats.BreedingPlanning
{
    /// <summary>
    /// Pure logic for filtering creatures for breeding plan purposes.
    /// </summary>
    public static class CreatureFiltering
    {
        /// <summary>
        /// Filters creatures by tags: excludes creatures with any excluding tag, then re-includes creatures with any including tag.
        /// </summary>
        /// <param name="creatures">Creatures to filter.</param>
        /// <param name="excludingTags">Tags that cause exclusion.</param>
        /// <param name="includingTags">Tags that override exclusion.</param>
        /// <param name="excludeByDefault">If true, all creatures are excluded by default unless they have an including tag.</param>
        /// <returns>Filtered creatures, or the original enumerable if no filtering is needed.</returns>
        public static IEnumerable<Creature> FilterByTags(IEnumerable<Creature> creatures,
            List<string> excludingTags, List<string> includingTags, bool excludeByDefault)
        {
            if (creatures == null)
                return null;

            if (!excludeByDefault && (excludingTags == null || !excludingTags.Any()))
                return creatures;

            var filteredList = new List<Creature>();
            foreach (var c in creatures)
            {
                bool exclude = excludeByDefault;
                if (!exclude && excludingTags != null && excludingTags.Any())
                {
                    foreach (string t in c.tags)
                    {
                        if (excludingTags.Contains(t))
                        {
                            exclude = true;
                            break;
                        }
                    }
                }
                if (exclude && includingTags != null && includingTags.Any())
                {
                    foreach (string t in c.tags)
                    {
                        if (includingTags.Contains(t))
                        {
                            exclude = false;
                            break;
                        }
                    }
                }
                if (!exclude)
                {
                    filteredList.Add(c);
                }
            }
            return filteredList;
        }

        /// <summary>
        /// Filters creatures qualifying for the breeding plan from a creature collection.
        /// </summary>
        /// <param name="allCreatures">All creatures in the collection.</param>
        /// <param name="speciesBlueprints">Blueprint paths of valid species (includes matesWith).</param>
        /// <param name="includeWithCooldown">If true, creatures on cooldown are included.</param>
        /// <param name="includeCryopodded">If true, cryopodded creatures are included.</param>
        /// <param name="ignoreBreedingCooldown">If true, breeding cooldown is ignored (for hermaphrodites).</param>
        /// <returns>List of qualifying creatures.</returns>
        public static List<Creature> GetQualifyingCreatures(IEnumerable<Creature> allCreatures,
            HashSet<string> speciesBlueprints, bool includeWithCooldown, bool includeCryopodded,
            bool ignoreBreedingCooldown)
        {
            var now = DateTime.Now;
            return allCreatures
                .Where(c => speciesBlueprints.Contains(c.speciesBlueprint)
                            && !c.flags.HasFlag(CreatureFlags.Neutered)
                            && !c.flags.HasFlag(CreatureFlags.Placeholder)
                            && (c.Status == CreatureStatus.Available
                                || (c.Status == CreatureStatus.Cryopod && includeCryopodded))
                            && (includeWithCooldown
                                || !(c.growingUntil > now
                                     || (!ignoreBreedingCooldown && c.cooldownUntil > now))
                            )
                )
                .ToList();
        }

        /// <summary>
        /// Filters creatures from a manually selected subset.
        /// </summary>
        public static List<Creature> GetQualifyingCreaturesFromSubset(IEnumerable<Creature> selectedCreatures,
            HashSet<string> speciesBlueprints)
        {
            return selectedCreatures
                .Where(c => speciesBlueprints.Contains(c.speciesBlueprint)
                            && !c.flags.HasFlag(CreatureFlags.Neutered)
                            && !c.flags.HasFlag(CreatureFlags.Placeholder)
                )
                .ToList();
        }

        /// <summary>
        /// Splits creatures into females and males (or all into females for no-gender species).
        /// </summary>
        public static (Creature[] females, Creature[] males) SplitBySex(List<Creature> creatures, bool noGender)
        {
            if (noGender)
            {
                return (creatures.ToArray(), null);
            }
            return (creatures.Where(c => c.sex == Sex.Female).ToArray(),
                    creatures.Where(c => c.sex == Sex.Male).ToArray());
        }

        /// <summary>
        /// Filters creatures by server, owner, and tribe.
        /// </summary>
        public static IEnumerable<Creature> FilterByServerOwnerTribe(IEnumerable<Creature> creatures,
            ICollection<string> hideServers, ICollection<string> hideOwners, ICollection<string> hideTribes)
        {
            if (creatures == null) return null;

            if (hideServers?.Any() == true)
                creatures = creatures.Where(c => !hideServers.Contains(c.server));
            if (hideOwners?.Any() == true)
                creatures = creatures.Where(c => !hideOwners.Contains(c.owner));
            if (hideTribes?.Any() == true)
                creatures = creatures.Where(c => !hideTribes.Contains(c.tribe));

            return creatures;
        }

        /// <summary>
        /// Determines whether a species can breed given the available creatures.
        /// </summary>
        /// <param name="availableCreatures">Creatures of this species that are available or cryopodded.</param>
        /// <param name="ignoreSex">If true, sex is ignored (e.g. S+ mutator).</param>
        /// <param name="noGender">If true, species has no gender.</param>
        /// <returns>True if breeding is possible.</returns>
        public static bool CanSpeciesBreed(Creature[] availableCreatures, bool ignoreSex, bool noGender)
        {
            if (availableCreatures == null || availableCreatures.Length == 0)
                return false;

            var ignoreSexInSpecies = ignoreSex || noGender;
            return (ignoreSexInSpecies && availableCreatures.Length > 1)
                   || (availableCreatures.Any(c => c.sex == Sex.Female) && availableCreatures.Any(c => c.sex == Sex.Male));
        }
    }
}
