using ARKBreedingStats.species;
using ARKBreedingStats.values;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ARKBreedingStats.mods;

namespace ARKBreedingStats.Library
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CreatureCollection
    {
        public const string CurrentLibraryFormatVersion = "1.13";

        public CreatureCollection()
        {
            FormatVersion = CurrentLibraryFormatVersion;
        }

        public const int MaxDomLevelDefault = 88;
        public const int MaxDomLevelSinglePlayerDefault = 88;

        /// <summary>
        /// The currently loaded creature collection.
        /// </summary>
        [JsonIgnore]
        public static CreatureCollection CurrentCreatureCollection;
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
        public int minChartLevel;
        [JsonProperty]
        public int maxChartLevel = 50;
        [JsonProperty]
        public int maxBreedingSuggestions = 10;
        [JsonProperty]
        public bool considerWildLevelSteps;
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
        public bool singlePlayerSettings;

        /// <summary>
        /// If true, apply extra multipliers for the game ATLAS.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool AtlasSettings;

        /// <summary>
        /// Indicates the game the library is used for. Possible values are "ASE" (default) for ARK: Survival Evolved or "ASA" for ARK: Survival Ascended.
        /// </summary>
        [JsonProperty("Game")]
        private string _game = "ASE";

        /// <summary>
        /// Used for the exportGun mod.
        /// This hash is used to determine if an imported creature file is using the current server multipliers.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ServerMultipliersHash;

        /// <summary>
        /// Allow more than 100% imprinting, can happen with mods, e.g. S+ Nanny
        /// </summary>
        [JsonProperty]
        public bool allowMoreThanHundredImprinting;

        [JsonProperty]
        public bool changeCreatureStatusOnSavegameImport = true;

        [JsonProperty]
        public List<string> modIDs;

        private List<Mod> _modList = new List<Mod>();

        /// <summary>
        /// Hash-Code that represents the loaded mod-values and their order
        /// </summary>
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
        /// The last item (i.e. index StatNames.StatsCount) is an array of possible imprintingMultiplier overrides.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, double?[][]> CustomSpeciesStats;

        private Dictionary<string, int> _creatureCountBySpecies;
        private int _totalCreatureCount;

        /// <summary>
        /// Calculates a hashcode for a list of mods and their order. Can be used to check for changes.
        /// </summary>
        public static int CalculateModListHash(IEnumerable<Mod> modList)
        {
            if (modList == null) { return 0; }

            return CalculateModListHash(modList.Select(m => m.id));
        }

        /// <summary>
        /// Calculates a hashcode for a list of mods and their order. Can be used to check for changes.
        /// </summary>
        public static int CalculateModListHash(IEnumerable<string> modIdList)
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

        /// <summary>
        /// Mods currently loaded to this collection.
        /// </summary>
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
        /// <param name="addPreviouslyDeletedCreatures">If true creatures will be added even if they were just deleted.</param>
        /// <returns>True if creatures were added or updated</returns>
        public bool MergeCreatureList(IEnumerable<Creature> creaturesToMerge, bool addPreviouslyDeletedCreatures = false, List<Guid> removeCreatures = null)
        {
            bool creaturesWereAddedOrUpdated = false;
            string onlyThisSpeciesBlueprintAdded = null;
            bool onlyOneSpeciesAdded = true;

            var guidDict = creatures.ToDictionary(c => c.guid);

            if (removeCreatures != null)
            {
                creaturesWereAddedOrUpdated = creatures.RemoveAll(c => removeCreatures.Contains(c.guid)) > 0;
            }

            foreach (Creature creatureNew in creaturesToMerge)
            {
                if (!addPreviouslyDeletedCreatures && DeletedCreatureGuids != null && DeletedCreatureGuids.Contains(creatureNew.guid)) continue;

                if (onlyOneSpeciesAdded)
                {
                    if (onlyThisSpeciesBlueprintAdded == null)
                        onlyThisSpeciesBlueprintAdded = creatureNew.speciesBlueprint;
                    else if (onlyThisSpeciesBlueprintAdded != creatureNew.speciesBlueprint)
                        onlyOneSpeciesAdded = false;
                }

                if (!guidDict.TryGetValue(creatureNew.guid, out var creatureExisting))
                {
                    creatures.Add(creatureNew);
                    creaturesWereAddedOrUpdated = true;
                    continue;
                }
                // creature already exists, a placeholder doesn't add more info
                if (creatureNew.flags.HasFlag(CreatureFlags.Placeholder)) continue;

                // creature is already in the library. Update its properties.
                if (creatureExisting.Species == null
                    || creatureExisting.speciesBlueprint != creatureNew.speciesBlueprint)
                {
                    creatureExisting.Species = creatureNew.Species;
                    creaturesWereAddedOrUpdated = true;
                }

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
                    creatureExisting.ArkIdInGame = Utils.ConvertImportedArkIdToIngameVisualization(creatureNew.ArkId);
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

                    if ((creatureExisting.levelsMutated == null && creatureNew.levelsMutated != null)
                        || (creatureExisting.levelsMutated != null && creatureNew.levelsMutated != null && !creatureExisting.levelsMutated.SequenceEqual(creatureNew.levelsMutated)))
                    {
                        creatureExisting.levelsMutated = creatureNew.levelsMutated;
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
            {
                ResetExistingColors(onlyOneSpeciesAdded ? onlyThisSpeciesBlueprintAdded : null);
                _creatureCountBySpecies = null;
                _totalCreatureCount = -1;
            }

            return creaturesWereAddedOrUpdated;
        }

        /// <summary>
        /// Removes creature from library and adds its guid to the deleted creatures.
        /// </summary>
        internal void DeleteCreature(Creature c)
        {
            if (!creatures.Remove(c)) return;

            if (DeletedCreatureGuids == null)
                DeletedCreatureGuids = new List<Guid>();
            DeletedCreatureGuids.Add(c.guid);
            ResetExistingColors(c.Species.blueprintPath);
            _creatureCountBySpecies = null;
            _totalCreatureCount = -1;
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

        /// <summary>
        /// Returns a creature based on the guid or ArkId.
        /// </summary>
        public bool CreatureById(Guid guid, long arkId, out Creature foundCreature)
        {
            foundCreature = null;
            if (guid == Guid.Empty && arkId == 0) return false;

            if (guid != Guid.Empty)
            {
                foreach (var c in creatures)
                {
                    if (c.guid == guid)
                    {
                        foundCreature = c;
                        return true;
                    }
                }
            }

            if (arkId != 0)
            {
                foreach (var c in creatures)
                {
                    if (c.ArkIdImported && c.ArkId == arkId)
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
        /// <param name="speciesBlueprintPath">If null, the color info of all species is cleared, else only the matching one.</param>
        /// </summary>
        internal void ResetExistingColors(string speciesBlueprintPath = null)
        {
            if (speciesBlueprintPath == null)
                _existingColors.Clear();
            else if (!string.IsNullOrEmpty(speciesBlueprintPath))
                _existingColors.Remove(speciesBlueprintPath);
        }

        /// <summary>
        /// Returns a tuple that indicates if a color id is already available in that species
        /// (inTheRegion, inAnyRegion).
        /// </summary>
        /// <returns></returns>
        internal ColorExisting[] ColorAlreadyAvailable(Species species, byte[] colorIds, out string infoText)
        {
            infoText = null;
            if (string.IsNullOrEmpty(species?.blueprintPath) || colorIds == null) return null;

            var usedColorIndices = Enumerable.Range(0, Ark.ColorRegionCount).Where(i => species.EnabledColorRegions[i]).ToArray();
            var usedColorCount = usedColorIndices.Length;

            // create data if not available in the cache
            if (!_existingColors.TryGetValue(species.blueprintPath, out var speciesExistingColors) || speciesExistingColors.Length != usedColorCount + 1)
            {
                // list of color ids in each region. The last index contains the ids of all regions
                speciesExistingColors = new List<int>[usedColorCount + 1];
                for (int i = 0; i < usedColorCount + 1; i++) speciesExistingColors[i] = new List<int>();
                foreach (Creature c in creatures)
                {
                    if (c.flags.HasFlag(CreatureFlags.Placeholder)
                        || c.flags.HasFlag(CreatureFlags.Dead)
                        || c.Species == null
                        || c.speciesBlueprint != species.blueprintPath)
                        continue;

                    for (int i = 0; i < usedColorCount; i++)
                    {
                        var colorRegionId = usedColorIndices[i];
                        var cColorId = c.colors[colorRegionId];
                        if (!speciesExistingColors[i].Contains(cColorId))
                            speciesExistingColors[i].Add(cColorId);
                        if (!speciesExistingColors[usedColorCount].Contains(cColorId))
                            speciesExistingColors[usedColorCount].Add(cColorId);
                    }
                }

                _existingColors[species.blueprintPath] = speciesExistingColors;
            }

            var newSpeciesColors = new List<string>(usedColorCount);
            var newRegionColors = new List<string>(usedColorCount);

            var results = new ColorExisting[Ark.ColorRegionCount];
            for (int i = 0; i < usedColorCount; i++)
            {
                var colorRegionId = usedColorIndices[i];
                var colorStatus = speciesExistingColors[i].Contains(colorIds[colorRegionId]) ? ColorExisting.ColorExistingInRegion
                    : speciesExistingColors[usedColorCount].Contains(colorIds[colorRegionId]) ? ColorExisting.ColorExistingInOtherRegion
                    : ColorExisting.ColorIsNew;
                results[colorRegionId] = colorStatus;
                switch (colorStatus)
                {
                    case ColorExisting.ColorIsNew:
                        var description = ColorDescription(colorIds[colorRegionId]);
                        if (!newSpeciesColors.Contains(description))
                            newSpeciesColors.Add(description);
                        break;
                    case ColorExisting.ColorExistingInOtherRegion:
                        newRegionColors.Add($"{ColorDescription(colorIds[colorRegionId])} in region {colorRegionId}");
                        break;
                }

                string ColorDescription(byte colorId)
                {
                    var color = CreatureColors.CreatureArkColor(colorId);
                    return $"{color.Name} ({color.Id})";
                }
            }

            if (newSpeciesColors.Any())
            {
                infoText = $"These colors are new for the {species.name}: {string.Join(", ", newSpeciesColors)}.";
            }
            if (newRegionColors.Any())
            {
                infoText += $"{(infoText == null ? null : "\n")}These colors are new in their region: {string.Join(", ", newRegionColors)}.";
            }

            infoText = infoText == null ? "No new colors" : infoText;

            return results;
        }

        public enum ColorExisting
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
            /// The color does not exist on any region on any creature of that species.
            /// </summary>
            ColorIsNew
        }

        public string Game
        {
            get => _game;
            set
            {
                _game = value;
                switch (value)
                {
                    case Ark.Asa:
                        if (modIDs == null) modIDs = new List<string>();
                        if (!modIDs.Contains(Ark.Asa))
                        {
                            modIDs.Insert(0, Ark.Asa);
                            modListHash = 0; // making sure the mod values are reloaded when checked
                        }
                        break;
                    default:
                        // non ASA
                        if (modIDs == null) return;
                        ModList.RemoveAll(m => m.id == Ark.Asa);
                        if (modIDs.Remove(Ark.Asa))
                            modListHash = 0;
                        break;
                }
            }
        }

        public Dictionary<string, int> GetCreatureCountBySpecies(bool recalculate = false)
        {
            if (_creatureCountBySpecies == null || recalculate)
            {
                _creatureCountBySpecies = creatures.Where(c => !c.flags.HasFlag(CreatureFlags.Placeholder)).GroupBy(c => c.speciesBlueprint)
                    .ToDictionary(g => g.Key, g => g.Count());
            }

            return _creatureCountBySpecies;
        }

        /// <summary>
        /// Returns total creature count. Ignoring placeholders.
        /// </summary>
        /// <returns></returns>
        public int GetTotalCreatureCount()
        {
            if (_totalCreatureCount == -1)
                _totalCreatureCount = creatures.Count(c => !c.flags.HasFlag(CreatureFlags.Placeholder));
            return _totalCreatureCount;
        }
    }
}
