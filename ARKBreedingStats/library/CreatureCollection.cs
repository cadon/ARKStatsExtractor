using ARKBreedingStats.species;
using ARKBreedingStats.values;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ARKBreedingStats.Library
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CreatureCollection
    {
        public const string CURRENT_FORMAT_VERSION = "1.13";

        public const int MaxDomLevelDefault = 73;
        public const int MaxDomLevelSinglePlayerDefault = 88;

        [JsonProperty]
        public string FormatVersion;
        [JsonProperty]
        public List<Creature> creatures = new List<Creature>();
        [JsonProperty]
        public List<CreatureValues> creaturesValues = new List<CreatureValues>();
        [JsonProperty]
        public List<TimerListEntry> timerListEntries = new List<TimerListEntry>();
        [JsonProperty]
        public List<IncubationTimerEntry> incubationListEntries = new List<IncubationTimerEntry>();
        [JsonProperty]
        public int maxDomLevel = MaxDomLevelDefault;
        [JsonProperty]
        public int maxWildLevel = 150;
        [JsonProperty]
        public int maxChartLevel = 50;
        [JsonProperty]
        public int maxBreedingSuggestions = 10;
        [JsonProperty]
        public bool considerWildLevelSteps = false;
        [JsonProperty]
        public int wildLevelStep = 5;
        /// <summary>
        /// On official servers a creature with more than 450 total levels will be deleted
        /// </summary>
        [JsonProperty]
        public int maxServerLevel = 450;
        /// <summary>
        /// Contains a list of creature's guids that are deleted. This is needed for synced libraries.
        /// </summary>
        [JsonProperty]
        public List<Guid> DeletedCreatureGuids;

        [JsonProperty]
        public ServerMultipliers serverMultipliers;
        [JsonProperty]
        public ServerMultipliers serverMultipliersEvents; // this object's statMultipliers are not used

        [JsonProperty]
        public bool singlePlayerSettings = false;
        /// <summary>
        /// Allow more than 100% imprinting, can happen with mods, e.g. S+ Nanny
        /// </summary>
        [JsonProperty]
        public bool allowMoreThanHundredImprinting = false;

        [JsonProperty]
        public bool changeCreatureStatusOnSavegameImport = true;

        [JsonProperty]
        public List<string> modIDs;

        private List<Mod> _modList = new List<Mod>();

        /// <summary>
        /// Hash-Code that represents the loaded mod-values and their order
        /// </summary>
        [JsonProperty]
        public int modListHash;

        [JsonProperty]
        public List<Player> players = new List<Player>();
        [JsonProperty]
        public List<Tribe> tribes = new List<Tribe>();
        [JsonProperty]
        public List<Note> noteList = new List<Note>();
        public List<string> tags = new List<string>();
        /// <summary>
        /// Which tags are checked for including in the breeding plan
        /// </summary>
        [JsonProperty]
        public List<string> tagsInclude = new List<string>();
        /// <summary>
        /// Which tags are checked for excluding in the breeding plan
        /// </summary>
        [JsonProperty]
        public List<string> tagsExclude = new List<string>();

        /// <summary>
        /// Temporary list of all owners (used in autocomplete / dropdowns)
        /// </summary>
        public string[] ownerList;
        /// <summary>
        /// Temporary list of all servers (used in autocomplete / dropdowns)
        /// </summary>
        public string[] serverList;
        /// <summary>
        /// All existing color ids for each species (by blueprint path). Each species has an array of 7 int[].
        /// Index 0-5 is an array of the colors of the according region, index 6 is an array of all colors in all regions.
        /// </summary>
        private readonly Dictionary<string, List<int>[]> _existingColors = new Dictionary<string, List<int>[]>();

        /// <summary>
        /// Some mods allow to change stat values of species in an extra ini file. These overrides are stored here.
        /// The last item (i.e. index Values.STATS_COUNT) is an array of possible imprintingMultiplier overrides.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, double?[][]> CustomSpeciesStats;

        /// <summary>
        /// Calculates a hashcode for a list of mods and their order. Can be used to check for changes.
        /// </summary>
        public static int CalculateModListHash(List<Mod> modList)
        {
            if (modList == null) { return 0; }

            return CalculateModListHash(modList.Select(m => m.id).ToList());
        }

        /// <summary>
        /// Calculates a hashcode for a list of mods and their order. Can be used to check for changes.
        /// </summary>
        public static int CalculateModListHash(List<string> modIdList)
        {
            if (modIdList == null) { return 0; }
            return string.Join(",", modIdList).GetHashCode();
        }

        /// <summary>
        /// Recalculates the modListHash for comparison and sets the mod-IDs of the modValues for the library.
        /// Should be called after the loaded mods are changed.
        /// </summary>
        public void UpdateModList()
        {
            modIDs = ModList?.Select(m => m.id).ToList() ?? new List<string>();
            modListHash = CalculateModListHash(ModList);
        }

        public List<Mod> ModList
        {
            set
            {
                _modList = value;
                UpdateModList();
            }
            get => _modList;
        }

        /// <summary>
        /// Returns true if the currently loaded modValues differ from the listed modValues of the library-file.
        /// </summary>
        public bool ModValueReloadNeeded => modListHash == 0 || modListHash != Values.V.loadedModsHash;

        /// <summary>
        /// Adds creatures to the current library.
        /// </summary>
        /// <param name="creaturesToMerge">List of creatures to add</param>
        /// <param name="addPreviouslylDeletedCreatures">If true creatures will be added even if they were just deleted.</param>
        /// <returns></returns>
        public bool MergeCreatureList(List<Creature> creaturesToMerge, bool addPreviouslylDeletedCreatures = false)
        {
            bool creaturesWereAddedOrUpdated = false;
            Species onlyThisSpeciesAdded = null;
            bool onlyOneSpeciesAdded = true;

            foreach (Creature creatureNew in creaturesToMerge)
            {
                if (!addPreviouslylDeletedCreatures && DeletedCreatureGuids != null && DeletedCreatureGuids.Contains(creatureNew.guid)) continue;

                if (onlyOneSpeciesAdded)
                {
                    if (onlyThisSpeciesAdded == null || onlyThisSpeciesAdded == creatureNew.Species)
                        onlyThisSpeciesAdded = creatureNew.Species;
                    else
                        onlyOneSpeciesAdded = false;
                }

                if (!creatures.Contains(creatureNew))
                {
                    creatures.Add(creatureNew);
                    creaturesWereAddedOrUpdated = true;
                    continue;
                }

                // creature is already in the library. Update it's properties.
                var creatureExisting = creatures.Single(c => c.guid == creatureNew.guid);
                if (creatureExisting.Species == null)
                    creatureExisting.Species = creatureNew.Species;
                else if (creatureExisting.speciesBlueprint != creatureNew.speciesBlueprint) continue;

                if (creatureNew.Mother != null)
                    creatureExisting.Mother = creatureNew.Mother;
                else if (creatureNew.motherGuid != Guid.Empty)
                    creatureExisting.motherGuid = creatureNew.motherGuid;
                if (creatureNew.Father != null)
                    creatureExisting.Father = creatureNew.Father;
                else if (creatureNew.fatherGuid != Guid.Empty)
                    creatureExisting.fatherGuid = creatureNew.fatherGuid;

                if (!string.IsNullOrEmpty(creatureNew.motherName))
                    creatureExisting.motherName = creatureNew.motherName;
                if (!string.IsNullOrEmpty(creatureNew.fatherName))
                    creatureExisting.fatherName = creatureNew.fatherName;

                // if the new ArkId is imported, use that
                if (creatureExisting.ArkId != creatureNew.ArkId && Utils.IsArkIdImported(creatureNew.ArkId, creatureNew.guid))
                {
                    creatureExisting.ArkId = creatureNew.ArkId;
                    creatureExisting.ArkIdImported = true;
                }

                creatureExisting.colors = creatureNew.colors;
                creatureExisting.Status = creatureNew.Status;
                creatureExisting.sex = creatureNew.sex;
                creatureExisting.cooldownUntil = creatureNew.cooldownUntil;
                if (!creatureExisting.domesticatedAt.HasValue || creatureExisting.domesticatedAt.Value.Year < 2000
                    || (creatureNew.domesticatedAt.HasValue && creatureNew.domesticatedAt.Value.Year > 2000 && creatureExisting.domesticatedAt > creatureNew.domesticatedAt))
                    creatureExisting.domesticatedAt = creatureNew.domesticatedAt;
                creatureExisting.generation = creatureNew.generation;
                creatureExisting.growingUntil = creatureNew.growingUntil;
                creatureExisting.imprintingBonus = creatureNew.imprintingBonus;
                creatureExisting.isBred = creatureNew.isBred;
                if (!string.IsNullOrEmpty(creatureNew.note))
                    creatureExisting.note = creatureNew.note;

                UpdateString(ref creatureExisting.name, ref creatureNew.name);
                UpdateString(ref creatureExisting.owner, ref creatureNew.owner);
                UpdateString(ref creatureExisting.tribe, ref creatureNew.tribe);
                UpdateString(ref creatureExisting.server, ref creatureNew.server);
                UpdateString(ref creatureExisting.imprinterName, ref creatureNew.imprinterName);

                void UpdateString(ref string oldCreatureValue, ref string newCreatureValue)
                {
                    if (oldCreatureValue != newCreatureValue)
                    {
                        oldCreatureValue = newCreatureValue;
                        creaturesWereAddedOrUpdated = true;
                    }
                }

                bool recalculate = false;
                if (creatureExisting.flags.HasFlag(CreatureFlags.Placeholder) ||
                    (creatureExisting.Status == CreatureStatus.Unavailable && creatureNew.Status == CreatureStatus.Available))
                {
                    creatureExisting.levelFound = creatureNew.levelFound;
                    creatureExisting.levelsDom = creatureNew.levelsDom;
                    creatureExisting.levelsWild = creatureNew.levelsWild;
                    creatureExisting.mutationsMaternal = creatureNew.mutationsMaternal;
                    creatureExisting.mutationsPaternal = creatureNew.mutationsPaternal;
                    creatureExisting.tamingEff = creatureNew.tamingEff;
                    creaturesWereAddedOrUpdated = true;
                    recalculate = true;
                }
                else
                {
                    if (!creatureExisting.levelsWild.SequenceEqual(creatureNew.levelsWild))
                    {
                        creatureExisting.levelsWild = creatureNew.levelsWild;
                        recalculate = true;
                        creaturesWereAddedOrUpdated = true;
                    }

                    if (!creatureExisting.levelsDom.SequenceEqual(creatureNew.levelsDom))
                    {
                        creatureExisting.levelsDom = creatureNew.levelsDom;
                        recalculate = true;
                        creaturesWereAddedOrUpdated = true;
                    }

                    if (creatureExisting.imprintingBonus != creatureNew.imprintingBonus)
                    {
                        creatureExisting.imprintingBonus = creatureNew.imprintingBonus;
                        recalculate = true;
                        creaturesWereAddedOrUpdated = true;
                    }

                    if (creatureExisting.tamingEff != creatureNew.tamingEff)
                    {
                        creatureExisting.tamingEff = creatureNew.tamingEff;
                        recalculate = true;
                        creaturesWereAddedOrUpdated = true;
                    }
                    // usually not necessary, mutations will not change, but if in ARK before exporting the ancestors screen was not opened, 0 will be assumed by ARK.
                    if (creatureNew.mutationsMaternal != 0 || creatureNew.mutationsPaternal != 0)
                    {
                        creatureExisting.mutationsMaternal = creatureNew.mutationsMaternal;
                        creatureExisting.mutationsPaternal = creatureNew.mutationsPaternal;
                    }
                }
                creatureExisting.flags = creatureNew.flags;

                if (recalculate)
                    creatureExisting.RecalculateCreatureValues(getWildLevelStep());
            }

            if (creaturesWereAddedOrUpdated)
                ResetExistingColors(onlyOneSpeciesAdded ? onlyThisSpeciesAdded : null);

            return creaturesWereAddedOrUpdated;
        }

        /// <summary>
        /// Removes creature from library and adds its guid to the deleted creatures.
        /// </summary>
        /// <param name="c"></param>
        internal void DeleteCreature(Creature c)
        {
            if (creatures.Remove(c))
            {
                if (DeletedCreatureGuids == null)
                    DeletedCreatureGuids = new List<Guid>();
                DeletedCreatureGuids.Add(c.guid);
            }
        }

        public int? getWildLevelStep()
        {
            return considerWildLevelSteps ? wildLevelStep : default(int?);
        }

        /// <summary>
        /// Checks if an existing creature has the given ARK-ID
        /// </summary>
        /// <param name="arkID">ARK-ID to check</param>
        /// <param name="concerningCreature">the creature with that id (if already in the collection it will be ignored)</param>
        /// <param name="creatureWithSameId">null if the Ark-Id is not yet in the collection, else the creature with the same Ark-Id</param>
        /// <returns>True if there is a creature with the given Ark-Id</returns>
        public bool ArkIdAlreadyExist(long arkID, Creature concerningCreature, out Creature creatureWithSameId)
        {
            // ArkId is not always unique. ARK uses ArkId = id1.ToString() + id2.ToString(); internally. If id2 has less decimal digits than int.MaxValue, the ids will differ. TODO handle this correctly
            creatureWithSameId = null;
            bool exists = false;
            foreach (var c in creatures)
            {
                if (c.ArkId == arkID && c != concerningCreature)
                {
                    creatureWithSameId = c;
                    exists = true;
                    break;
                }
            }
            return exists;
        }

        public bool CreatureById(Guid guid, long arkId, Species species, Sex sex, out Creature foundCreature)
        {
            foundCreature = null;
            var creaturesToCheck = creatures.Where(c => c.Species == species && c.sex == sex).ToList();

            if (guid != Guid.Empty)
            {
                foreach (var c in creaturesToCheck)
                {
                    if (c.guid == guid)
                    {
                        foundCreature = c;
                        return true;
                    }
                }
            }

            // TODO. not always unique, prompt message?
            if (arkId != 0)
            {
                foreach (var c in creaturesToCheck)
                {
                    if (c.ArkId == arkId)
                    {
                        foundCreature = c;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all placeholder creatures that have no other creature linked to them.
        /// Call this method after creatures were deleted
        /// </summary>
        public void RemoveUnlinkedPlaceholders()
        {
            var unusedPlaceHolders = creatures.Where(c => c.flags.HasFlag(CreatureFlags.Placeholder)).ToList();

            foreach (Creature c in creatures)
            {
                if (c.flags.HasFlag(CreatureFlags.Placeholder)) continue;

                var usedPlaceholder = unusedPlaceHolders.FirstOrDefault(p => p.guid == c.motherGuid || p.guid == c.fatherGuid);
                if (usedPlaceholder != null) unusedPlaceHolders.Remove(usedPlaceholder);

                if (unusedPlaceHolders.Count == 0) break;
            }

            foreach (var p in unusedPlaceHolders)
                creatures.Remove(p);
        }

        [OnDeserialized]
        private void InitializeProperties(StreamingContext ct)
        {
            if (tags == null) tags = new List<string>();

            // convert DateTimes to local times
            foreach (var tle in timerListEntries)
                tle.time = tle.time.ToLocalTime();

            foreach (var ile in incubationListEntries)
                ile.incubationEnd = ile.incubationEnd.ToLocalTime();

            foreach (var c in creatures)
            {
                c.cooldownUntil = c.cooldownUntil?.ToLocalTime();
                c.growingUntil = c.growingUntil?.ToLocalTime();
                c.domesticatedAt = c.domesticatedAt?.ToLocalTime();
                c.addedToLibrary = c.addedToLibrary?.ToLocalTime();
            }
        }

        /// <summary>
        /// Reset the lists of available color ids. Call this method after a creature was added or removed from the collection.
        /// </summary>
        /// <param name="species"></param>
        internal void ResetExistingColors(Species species = null)
        {
            if (species == null)
                _existingColors.Clear();
            else if (!string.IsNullOrEmpty(species.blueprintPath))
                _existingColors.Remove(species.blueprintPath);
        }

        /// <summary>
        /// Returns a tuple that indicates if a color id is already available in that species
        /// (inTheRegion, inAnyRegion).
        /// </summary>
        /// <returns></returns>
        internal ColorExisting[] ColorAlreadyAvailable(Species species, int[] colorIds)
        {
            if (string.IsNullOrEmpty(species?.blueprintPath)) return null;

            if (!_existingColors.TryGetValue(species.blueprintPath, out var speciesExistingColors))
            {
                speciesExistingColors = new List<int>[Species.ColorRegionCount + 1];
                for (int i = 0; i < Species.ColorRegionCount + 1; i++) speciesExistingColors[i] = new List<int>();
                foreach (Creature c in creatures)
                {
                    if (c.flags.HasFlag(CreatureFlags.Placeholder) || c.Species == null || c.speciesBlueprint != species.blueprintPath)
                        continue;

                    for (int i = 0; i < Species.ColorRegionCount; i++)
                    {
                        int cColorId = c.colors[i];
                        if (!speciesExistingColors[i].Contains(cColorId))
                            speciesExistingColors[i].Add(cColorId);
                        if (!speciesExistingColors[Species.ColorRegionCount].Contains(cColorId))
                            speciesExistingColors[Species.ColorRegionCount].Add(cColorId);
                    }
                }

                _existingColors.Add(species.blueprintPath, speciesExistingColors);
            }

            var results = new ColorExisting[Species.ColorRegionCount];
            for (int ci = 0; ci < Species.ColorRegionCount; ci++)
                results[ci] = speciesExistingColors[ci].Contains(colorIds[ci]) ? ColorExisting.ColorExistingInRegion
                    : speciesExistingColors[Species.ColorRegionCount].Contains(colorIds[ci]) ? ColorExisting.ColorExistingInOtherRegion
                    : ColorExisting.ColorIsNew;
            return results;
        }

        internal enum ColorExisting
        {
            Unknown,
            /// <summary>
            /// The color is already available in that region on a creature of that species.
            /// </summary>
            ColorExistingInRegion,
            /// <summary>
            /// The color is already available in a different region on a creature of that species.
            /// </summary>
            ColorExistingInOtherRegion,
            /// <summary>
            /// The color is not existing on any region on any creature of that species.
            /// </summary>
            ColorIsNew
        }
    }
}
