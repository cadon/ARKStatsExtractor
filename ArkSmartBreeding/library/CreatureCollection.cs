using ARKBreedingStats.BreedingPlanning;
using ARKBreedingStats.Models;
using ARKBreedingStats.Mods;
using ARKBreedingStats.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ARKBreedingStats.Library;

[JsonObject(MemberSerialization.OptIn)]
public class CreatureCollection
{
    public const string CurrentLibraryFormatVersion = "1.13";

    public const int MaxDomLevelDefault = 88;
    public const int MaxDomLevelSinglePlayerDefault = 88;

    /// <summary>
    /// The currently loaded creature collection.
    /// </summary>
    [JsonIgnore]
    public static CreatureCollection CurrentCreatureCollection { get; set; }
    [JsonProperty]
    public string FormatVersion { get; set; } = CurrentLibraryFormatVersion;
    [JsonProperty]
    public List<Creature> creatures { get; set; } = new List<Creature>();
    [JsonProperty]
    public List<CreatureValues> creaturesValues { get; set; } = new List<CreatureValues>();
    [JsonProperty]
    public List<TimerListEntry> timerListEntries { get; set; } = new List<TimerListEntry>();
    [JsonProperty]
    public List<IncubationTimerEntry> incubationListEntries { get; set; } = new List<IncubationTimerEntry>();
    [JsonProperty]
    public int maxDomLevel { get; set; } = MaxDomLevelDefault;
    [JsonProperty]
    public int maxWildLevel { get; set; } = Ark.MaxWildLevelDefault;
    [JsonProperty]
    public int minChartLevel { get; set; }
    [JsonProperty]
    public int maxChartLevel { get; set; } = Ark.MaxWildLevelDefault / 3;
    [JsonProperty]
    public int maxBreedingSuggestions { get; set; } = 10;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool considerWildLevelSteps { get; set; }
    [JsonProperty]
    public int wildLevelStep { get; set; } = Ark.WildLevelStepDefault;
    /// <summary>
    /// On official servers a creature with more than 450 total levels will be deleted
    /// </summary>
    [JsonProperty]
    public int maxServerLevel { get; set; } = 450;
    /// <summary>
    /// Contains a list of creature's guids that are deleted. This is needed for synced libraries.
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<Guid> DeletedCreatureGuids { get; set; }

    [JsonProperty]
    public ServerMultipliers serverMultipliers { get; set; }

    /// <summary>
    /// Only the taming and breeding multipliers of this are used.
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public ServerMultipliers serverMultipliersEvents { get; set; }

    /// <summary>
    /// Deprecated setting, remove on 2025-01-01
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool singlePlayerSettings { get; set; }

    /// <summary>
    /// Indicates the game the library is used for. Possible values are "ASE" (default) for ARK: Survival Evolved or "ASA" for ARK: Survival Ascended.
    /// </summary>
    [JsonProperty("Game")]
    private string _game = Ark.Ase;

    /// <summary>
    /// Used for the exportGun mod.
    /// This hash is used to determine if an imported creature file is using the current server multipliers.
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string ServerMultipliersHash { get; set; }

    /// <summary>
    /// Allow more than 100% imprinting, can happen with mods, e.g. S+ Nanny
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool allowMoreThanHundredImprinting { get; set; }

    [JsonProperty]
    public bool changeCreatureStatusOnSavegameImport { get; set; } = true;

    [JsonProperty]
    public List<string> modIDs { get; set; }

    private List<Mod> _modList = new List<Mod>();

    /// <summary>
    /// Hash-Code that represents the loaded mod-values and their order
    /// </summary>
    public int modListHash { get; set; }

    [JsonProperty]
    public List<Player> players { get; set; } = new List<Player>();
    [JsonProperty]
    public List<Tribe> tribes { get; set; } = new List<Tribe>();
    [JsonProperty]
    public List<Note> noteList { get; set; } = new List<Note>();
    public List<string> tags { get; set; } = new List<string>();
    /// <summary>
    /// Which tags are checked for including in the breeding plan
    /// </summary>
    [JsonProperty]
    public List<string> tagsInclude { get; set; } = new List<string>();
    /// <summary>
    /// Which tags are checked for excluding in the breeding plan
    /// </summary>
    [JsonProperty]
    public List<string> tagsExclude { get; set; } = new List<string>();

    /// <summary>
    /// Temporary list of all owners (used in autocomplete / dropdowns)
    /// </summary>
    public string[] ownerList { get; set; }
    /// <summary>
    /// Temporary list of all servers (used in autocomplete / dropdowns)
    /// </summary>
    public string[] serverList { get; set; }
    /// <summary>
    /// Count of creatures that have a specific color in a specific region, dictionary key is species blueprint path.
    /// The value is an int[][]. First index is the color region, second index is the color id, the value is the count of the creature with that color in that region.
    /// The index 6 is all color regions combined, i.e. counts color ids in all regions (i.e. a[6][i] = a[0][i] + ... + a[5][i])
    /// </summary>
    public Dictionary<string, int[][]> _existingColors { get; set; } = new Dictionary<string, int[][]>();

    /// <summary>
    /// Some mods allow to change stat values of species in an extra ini file. These overrides are stored here.
    /// The last item (i.e. index StatNames.StatsCount) is an array of possible imprintingMultiplier overrides.
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Dictionary<string, double?[][]> CustomSpeciesStats { get; set; }

    private Dictionary<string, int> _creatureCountBySpecies;
    private int _totalCreatureCount;

    /// <summary>
    /// ServerMultipliers uri on the server to pull the settings.
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string ServerSettingsUriSource { get; set; }

    /// <summary>
    /// List of pairs currently breeding.
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public CurrentBreedingPair[] CurrentBreedingPairs { get; set; }

    /// <summary>
    /// List of all top stats per species.
    /// </summary>
    public readonly Dictionary<Species, TopLevels> TopLevels = new Dictionary<Species, TopLevels>();

    /// <summary>
    /// Calculates a hashcode for a list of mods and their order. Can be used to check for changes.
    /// </summary>
    public static int CalculateModListHash(IEnumerable<Mod> modList)
    {
        if (modList == null) { return 0; }

        return CalculateModListHash(modList.Select(m => m.Id));
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
        modIDs = ModList?.Select(m => m.Id).ToList() ?? new List<string>();
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
    public bool IsModValueReloadNeeded(int loadedModsHash) => modListHash == 0 || modListHash != loadedModsHash;

    private Dictionary<string, Creature[]> _creaturesByBlueprint;

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

        if (removeCreatures != null)
        {
            creaturesWereAddedOrUpdated = creatures.RemoveAll(c => removeCreatures.Contains(c.guid)) > 0;
        }

        var guidDict = creatures.ToDictionary(c => c.guid);

        foreach (Creature creatureNew in creaturesToMerge)
        {
            if (!addPreviouslyDeletedCreatures && DeletedCreatureGuids != null && DeletedCreatureGuids.Contains(creatureNew.guid))
            {
                continue;
            }

            if (onlyOneSpeciesAdded)
            {
                if (onlyThisSpeciesBlueprintAdded == null)
                {
                    onlyThisSpeciesBlueprintAdded = creatureNew.speciesBlueprint;
                }
                else if (onlyThisSpeciesBlueprintAdded != creatureNew.speciesBlueprint)
                {
                    onlyOneSpeciesAdded = false;
                }
            }

            if (!guidDict.TryGetValue(creatureNew.guid, out var creatureExisting))
            {
                if (creatureNew.addedToLibrary == null)
                {
                    creatureNew.addedToLibrary = DateTime.Now;
                }

                creatures.Add(creatureNew);
                creaturesWereAddedOrUpdated = true;
                continue;
            }
            // creature already exists, a placeholder doesn't add more info
            if (creatureNew.flags.HasFlag(CreatureFlags.Placeholder))
            {
                continue;
            }

            // creature is already in the library. Update its properties.
            if (creatureExisting.Species == null
                || creatureExisting.speciesBlueprint != creatureNew.speciesBlueprint)
            {
                creatureExisting.Species = creatureNew.Species;
                creaturesWereAddedOrUpdated = true;
            }

            if (creatureNew.Mother != null)
            {
                creatureExisting.Mother = creatureNew.Mother;
            }
            else if (creatureNew.motherGuid != Guid.Empty)
            {
                creatureExisting.motherGuid = creatureNew.motherGuid;
            }

            if (creatureNew.Father != null)
            {
                creatureExisting.Father = creatureNew.Father;
            }
            else if (creatureNew.fatherGuid != Guid.Empty)
            {
                creatureExisting.fatherGuid = creatureNew.fatherGuid;
            }

            if (!string.IsNullOrEmpty(creatureNew.motherName))
            {
                creatureExisting.motherName = creatureNew.motherName;
            }

            if (!string.IsNullOrEmpty(creatureNew.fatherName))
            {
                creatureExisting.fatherName = creatureNew.fatherName;
            }

            // if the new ArkId is imported, use that
            if (creatureExisting.ArkId != creatureNew.ArkId && ArkIdConverter.IsArkIdImported(creatureNew.ArkId, creatureNew.guid))
            {
                creatureExisting.ArkId = creatureNew.ArkId;
                creatureExisting.ArkIdImported = true;
                creatureExisting.ArkIdInGame = ArkIdConverter.ConvertImportedArkIdToIngameVisualization(creatureNew.ArkId);
            }

            creatureExisting.colors = creatureNew.colors;
            creatureExisting.Status = creatureNew.Status;
            creatureExisting.sex = creatureNew.sex;
            creatureExisting.cooldownUntil = creatureNew.cooldownUntil;
            if (!creatureExisting.domesticatedAt.HasValue || creatureExisting.domesticatedAt.Value.Year < 2000
                || (creatureNew.domesticatedAt.HasValue && creatureNew.domesticatedAt.Value.Year > 2000 && creatureExisting.domesticatedAt > creatureNew.domesticatedAt))
            {
                creatureExisting.domesticatedAt = creatureNew.domesticatedAt;
            }

            creatureExisting.generation = creatureNew.generation;
            creatureExisting.growingUntil = creatureNew.growingUntil;
            creatureExisting.imprintingBonus = creatureNew.imprintingBonus;
            creatureExisting.isBred = creatureNew.isBred;
            if (!string.IsNullOrEmpty(creatureNew.note))
            {
                creatureExisting.note = creatureNew.note;
            }

            creatureExisting.Traits = creatureNew.Traits;

            UpdateString(creatureNew.name, v => creatureExisting.name = v);
            UpdateString(creatureNew.owner, v => creatureExisting.owner = v);
            UpdateString(creatureNew.tribe, v => creatureExisting.tribe = v);
            UpdateString(creatureNew.server, v => creatureExisting.server = v);
            UpdateString(creatureNew.imprinterName, v => creatureExisting.imprinterName = v);

            void UpdateString(string newValue, Action<string> setter)
            {
                if (newValue != null)
                {
                    setter(newValue);
                    creaturesWereAddedOrUpdated = true;
                }
            }

            bool recalculate = false;
            if (creatureExisting.flags.HasFlag(CreatureFlags.Placeholder) ||
                (creatureExisting.Status == CreatureStatus.Unavailable && creatureNew.Status == CreatureStatus.Available))
            {
                creatureExisting.levelFound = creatureNew.levelFound;
                creatureExisting.levelsWild = creatureNew.levelsWild;
                creatureExisting.levelsMutated = creatureNew.levelsMutated;
                creatureExisting.levelsDom = creatureNew.levelsDom;
                creatureExisting.mutationsMaternal = creatureNew.mutationsMaternal;
                creatureExisting.mutationsPaternal = creatureNew.mutationsPaternal;
                creatureExisting.tamingEff = creatureNew.tamingEff;
                creatureExisting.Traits = creatureNew.Traits;
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
            {
                creatureExisting.RecalculateCreatureValues(getWildLevelStep());
            }
        }

        if (creaturesWereAddedOrUpdated)
        {
            ResetExistingColors(onlyOneSpeciesAdded ? onlyThisSpeciesBlueprintAdded : null);
            _creatureCountBySpecies = null;
            _totalCreatureCount = -1;
            _creaturesByBlueprint = null;
        }

        return creaturesWereAddedOrUpdated;
    }

    /// <summary>
    /// Removes creature from library and adds its guid to the deleted creatures.
    /// </summary>
    public void DeleteCreature(Creature c)
    {
        if (!creatures.Remove(c))
        {
            return;
        }

        if (DeletedCreatureGuids == null)
        {
            DeletedCreatureGuids = new List<Guid>();
        }

        DeletedCreatureGuids.Add(c.guid);
        ResetExistingColors(c.Species.blueprintPath);
        _creatureCountBySpecies = null;
        _totalCreatureCount = -1;
        _creaturesByBlueprint = null;
    }

    public int? getWildLevelStep()
    {
        return considerWildLevelSteps ? wildLevelStep : default(int?);
    }

    /// <summary>
    /// Checks if an existing creature has the given ARK-ID
    /// </summary>
    /// <param name="arkId">ARK-ID to check</param>
    /// <param name="concerningCreature">the creature with that id (if already in the collection it will be ignored)</param>
    /// <param name="creatureWithSameId">null if the Ark-Id is not yet in the collection, else the creature with the same Ark-Id</param>
    /// <returns>True if there is a creature with the given Ark-Id</returns>
    public bool ArkIdAlreadyExist(long arkId, Creature concerningCreature, out Creature creatureWithSameId)
    {
        // ArkId is not always unique. ARK uses ArkId = id1.ToString() + id2.ToString(); internally. If id2 has less decimal digits than int.MaxValue, the ids will differ. TODO handle this correctly
        creatureWithSameId = null;
        bool exists = false;
        foreach (var c in creatures)
        {
            if (c.ArkId == arkId && c != concerningCreature)
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
        if (guid == Guid.Empty && arkId == 0)
        {
            return false;
        }

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
            if (c.flags.HasFlag(CreatureFlags.Placeholder))
            {
                continue;
            }

            var usedPlaceholder = unusedPlaceHolders.FirstOrDefault(p => p.guid == c.motherGuid || p.guid == c.fatherGuid);
            if (usedPlaceholder != null)
            {
                unusedPlaceHolders.Remove(usedPlaceholder);
            }

            if (unusedPlaceHolders.Count == 0)
            {
                break;
            }
        }

        foreach (var p in unusedPlaceHolders)
        {
            creatures.Remove(p);
        }
    }

    [OnDeserialized]
    private void InitializeProperties(StreamingContext ct)
    {
        if (tags == null)
        {
            tags = new List<string>();
        }

        // backwards compatibility, remove 10 lines below in 2025-01-01
        if (singlePlayerSettings && serverMultipliers != null)
        {
            serverMultipliers.SinglePlayerSettings = singlePlayerSettings;
            singlePlayerSettings = false;
        }

        // convert DateTimes to local times
        foreach (var tle in timerListEntries)
        {
            tle.time = tle.time.ToLocalTime();
        }

        foreach (var ile in incubationListEntries)
        {
            ile.incubationEnd = ile.incubationEnd.ToLocalTime();
        }

        foreach (var c in creatures)
        {
            c.cooldownUntil = c.cooldownUntil?.ToLocalTime();
            c.growingUntil = c.growingUntil?.ToLocalTime();
            c.domesticatedAt = c.domesticatedAt?.ToLocalTime();
            c.addedToLibrary = c.addedToLibrary?.ToLocalTime();
        }

        if (CurrentBreedingPairs != null)
        {
            var guids = creatures.ToDictionary(c => c.guid);
            foreach (var pair in CurrentBreedingPairs)
            {
                if (guids.TryGetValue(pair.GuidMother, out var m))
                {
                    pair.Mother = m;
                }

                if (guids.TryGetValue(pair.GuidFather, out var f))
                {
                    pair.Father = f;
                }
            }
        }
    }

    /// <summary>
    /// Reset the lists of available color ids. Call this method after a creature was added or removed from the collection.
    /// <param name="speciesBlueprintPath">If null, the color info of all species is cleared, else only the matching one.</param>
    /// </summary>
    public void ResetExistingColors(string speciesBlueprintPath = null)
    {
        if (speciesBlueprintPath == null)
        {
            _existingColors.Clear();
        }
        else if (!string.IsNullOrEmpty(speciesBlueprintPath))
        {
            _existingColors.Remove(speciesBlueprintPath);
        }
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
                    if (modIDs == null)
                    {
                        modIDs = new List<string>();
                    }

                    if (!modIDs.Contains(Ark.Asa))
                    {
                        modIDs.Insert(0, Ark.Asa);
                        modListHash = 0; // making sure the mod values are reloaded when checked
                    }
                    break;
                default:
                    // non ASA
                    if (modIDs == null)
                    {
                        return;
                    }

                    ModList.RemoveAll(m => m.Id == Ark.Asa);
                    if (modIDs.Remove(Ark.Asa))
                    {
                        modListHash = 0;
                    }

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
        {
            _totalCreatureCount = creatures.Count(c => !c.flags.HasFlag(CreatureFlags.Placeholder));
        }

        return _totalCreatureCount;
    }

    /// <summary>
    /// Returns all creatures of a species and if available all creatures of mating compatible species. Ignores placeholder creatures.
    /// </summary>
    public List<Creature> GetSpeciesCompatibleCreatures(Species species)
    {
        if (species == null)
        {
            return null;
        }

        if (_creaturesByBlueprint == null)
        {
            ReGroupCreaturesByBp();
        }

        var creaturesResult = new List<Creature>();
        var bpList = new List<string> { species.blueprintPath };

        if (species.matesWith?.Any() == true)
        {
            bpList.AddRange(species.matesWith);
        }

        foreach (var bp in bpList)
        {
            _creaturesByBlueprint.TryGetValue(bp, out var creatures);
            if (creatures != null)
            {
                creaturesResult.AddRange(creatures);
            }
        }

        return creaturesResult;
    }

    private void ReGroupCreaturesByBp()
    {
        _creaturesByBlueprint = creatures
             .Where(c => !c.flags.HasFlag(CreatureFlags.Placeholder))
             .GroupBy(c => c.speciesBlueprint)
             .ToDictionary(g => g.Key, g => g.ToArray());
    }
}
