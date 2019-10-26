using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ARKBreedingStats.Library
{
    [DataContract]
    public class CreatureCollection
    {
        [IgnoreDataMember]
        public const string CURRENT_FORMAT_VERSION = "1.12";

        [DataMember]
        public string FormatVersion; // currently set to 1.12 to represent the supported 12 stats
        [DataMember]
        public List<Creature> creatures = new List<Creature>();
        [DataMember]
        public List<CreatureValues> creaturesValues = new List<CreatureValues>();
        [DataMember]
        public List<TimerListEntry> timerListEntries = new List<TimerListEntry>();
        [DataMember]
        public List<IncubationTimerEntry> incubationListEntries = new List<IncubationTimerEntry>();
        [DataMember]
        public List<string> hiddenOwners = new List<string>(); // which owners are not selected to be shown
        [DataMember]
        public List<string> hiddenServers = new List<string>();
        [DataMember]
        public List<string> dontShowTags = new List<string>(); // which tags are selected to be not shown
        [DataMember]
        internal CreatureFlags showFlags = CreatureFlags.Available | CreatureFlags.Cryopod | CreatureFlags.Dead | CreatureFlags.Mutated | CreatureFlags.Neutered | CreatureFlags.Obelisk | CreatureFlags.Unavailable;
        [DataMember]
        public bool useFiltersInTopStatCalculation = false;
        [DataMember]
        public int maxDomLevel = 73;
        [DataMember]
        public int maxWildLevel = 150;
        [DataMember]
        public int maxChartLevel = 50;
        [DataMember]
        public int maxBreedingSuggestions = 10;
        [DataMember]
        public bool considerWildLevelSteps = false;
        [DataMember]
        public int wildLevelStep = 5;
        /// <summary>
        /// On official servers a creature with more than 450 total levels will be deleted
        /// </summary>
        [DataMember]
        public int maxServerLevel = 450;
        /// <summary>
        /// Contains a list of creature's guids that are deleted. This is needed for synced libraries.
        /// </summary>
        [DataMember]
        public List<Guid> DeletedCreatureGuids;

        [DataMember]
        public ServerMultipliers serverMultipliers;
        [DataMember]
        public ServerMultipliers serverMultipliersEvents; // this object's statMultipliers are not used

        [DataMember]
        public bool singlePlayerSettings = false;
        /// <summary>
        /// Allow more than 100% imprinting, can happen with mods, e.g. S+ Nanny
        /// </summary>
        [DataMember]
        public bool allowMoreThanHundredImprinting = false;

        [DataMember]
        public bool changeCreatureStatusOnSavegameImport = true;

        [DataMember]
        public List<string> modIDs;

        [IgnoreDataMember]
        private List<Mod> _modList = new List<Mod>();

        /// <summary>
        /// Hash-Code that represents the loaded mod-values and their order
        /// </summary>
        [DataMember]
        public int modListHash;

        [DataMember]
        public List<Player> players = new List<Player>();
        [DataMember]
        public List<Tribe> tribes = new List<Tribe>();
        [DataMember]
        public List<Note> noteList = new List<Note>();
        [IgnoreDataMember]
        public List<string> tags = new List<string>();
        [DataMember]
        public List<string> tagsInclude = new List<string>(); // which tags are checked for including in the breedingplan
        [DataMember]
        public List<string> tagsExclude = new List<string>(); // which tags are checked for excluding in the breedingplan

        [IgnoreDataMember]
        public string[] ownerList; // temporary list of all owners (used in autocomplete / dropdowns)
        [IgnoreDataMember]
        public string[] serverList; // temporary list of all servers (used in autocomplete / dropdowns)

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
        public static int CalculateModListHash(List<string> modIDList)
        {
            if (modIDList == null) { return 0; }
            return string.Join(",", modIDList).GetHashCode();
        }

        /// <summary>
        /// Recalculates the modListHash for comparison and sets the mod-IDs of the modvalues for the library.
        /// Should be called after the mods are changed.
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
        [IgnoreDataMember]
        public bool ModValueReloadNeeded { get { return modListHash == 0 || modListHash != Values.V.loadedModsHash; } }

        /// <summary>
        /// Adds creatures to the current library.
        /// </summary>
        /// <param name="creaturesToMerge"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public bool MergeCreatureList(List<Creature> creaturesToMerge, bool update = false)
        {
            bool creaturesWereAdded = false;
            foreach (Creature creature in creaturesToMerge)
            {
                if (DeletedCreatureGuids != null && DeletedCreatureGuids.Contains(creature.guid)) continue;

                if (!creatures.Contains(creature))
                {
                    creatures.Add(creature);
                    creaturesWereAdded = true;
                }
                else if (update)
                {
                    // Merge in some specific parts: imprinting level, dom stats, name
                    var old = creatures.Single(c => c.guid == creature.guid);
                    if (old.Species == null)
                        old.Species = creature.Species;
                    else if (old.Species != creature.Species) continue;

                    bool recalculate = false;
                    if (old.flags.HasFlag(CreatureFlags.Placeholder) ||
                        (old.status == CreatureStatus.Unavailable && creature.status == CreatureStatus.Available))
                    {
                        old.colors = creature.colors;
                        old.cooldownUntil = creature.cooldownUntil;
                        old.domesticatedAt = creature.domesticatedAt;
                        old.sex = creature.sex;
                        old.generation = creature.generation;
                        old.growingUntil = creature.growingUntil;
                        old.imprinterName = creature.imprinterName;
                        old.imprintingBonus = creature.imprintingBonus;
                        old.isBred = creature.isBred;
                        old.levelFound = creature.levelFound;
                        old.levelsDom = creature.levelsDom;
                        old.levelsWild = creature.levelsWild;
                        if (creature.Mother != null)
                            old.Mother = creature.Mother;
                        else
                            old.motherGuid = creature.motherGuid;
                        old.motherName = creature.motherName;
                        if (creature.Father != null)
                            old.Father = creature.Father;
                        else
                            old.fatherGuid = creature.fatherGuid;
                        old.fatherName = creature.fatherName;
                        old.mutationsMaternal = creature.mutationsMaternal;
                        old.mutationsPaternal = creature.mutationsPaternal;
                        old.name = creature.name;
                        old.note = creature.note;
                        old.owner = creature.owner;
                        old.server = creature.server;
                        old.flags = creature.flags;
                        old.status = creature.status;
                        old.tamingEff = creature.tamingEff;
                        old.topBreedingCreature = creature.topBreedingCreature;
                        old.topBreedingStats = creature.topBreedingStats;
                        old.topStatsCount = creature.topStatsCount;
                        old.topStatsCountBP = creature.topStatsCountBP;
                        old.topness = creature.topness;
                        old.tribe = creature.tribe;
                        old.valuesBreeding = creature.valuesBreeding;
                        old.valuesDom = creature.valuesDom;
                        old.ArkId = creature.ArkId;
                        old.ArkIdImported = creature.ArkIdImported;
                        creaturesWereAdded = true;
                        recalculate = true;
                    }
                    else
                    {
                        if (old.name != creature.name)
                        {
                            old.name = creature.name;
                            creaturesWereAdded = true;
                        }

                        if (old.server != creature.server)
                        {
                            old.server = creature.server;
                            creaturesWereAdded = true;
                        }

                        if (!old.levelsWild.SequenceEqual(creature.levelsWild))
                        {
                            old.levelsWild = creature.levelsWild;
                            recalculate = true;
                            creaturesWereAdded = true;
                        }

                        if (!old.levelsDom.SequenceEqual(creature.levelsDom))
                        {
                            old.levelsDom = creature.levelsDom;
                            recalculate = true;
                            creaturesWereAdded = true;
                        }

                        if (old.imprintingBonus != creature.imprintingBonus)
                        {
                            old.imprintingBonus = creature.imprintingBonus;
                            recalculate = true;
                            creaturesWereAdded = true;
                        }

                        if (old.tamingEff != creature.tamingEff)
                        {
                            old.tamingEff = creature.tamingEff;
                            recalculate = true;
                            creaturesWereAdded = true;
                        }
                        // usually not necessary, mutations will not change, but if in ARK before exporting the ancestors screen was not opened, 0 will be assumed by ARK.
                        if (creature.mutationsMaternal != 0 || creature.mutationsPaternal != 0)
                        {
                            old.mutationsMaternal = creature.mutationsMaternal;
                            old.mutationsPaternal = creature.mutationsPaternal;
                        }
                        if (old.motherGuid == Guid.Empty || old.fatherGuid == Guid.Empty)
                        {
                            old.motherGuid = creature.motherGuid;
                            old.motherName = creature.motherName;
                            old.fatherGuid = creature.fatherGuid;
                            old.fatherName = creature.fatherName;
                        }
                    }

                    if (recalculate)
                        old.RecalculateCreatureValues(getWildLevelStep());
                }
            }
            return creaturesWereAdded;
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
    }
}
