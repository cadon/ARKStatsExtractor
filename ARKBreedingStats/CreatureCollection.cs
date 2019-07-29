using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ARKBreedingStats
{
    [Serializable()]
    public class CreatureCollection // simple placeholder for XML serialization
    {
        [XmlIgnore]
        public const string CURRENT_FORMAT_VERSION = "1.12";

        public string FormatVersion; // currently set to 1.12 to represent the supported 12 stats
        [XmlArray]
        public List<Creature> creatures = new List<Creature>();
        [XmlArray]
        public List<species.CreatureValues> creaturesValues = new List<species.CreatureValues>();
        // TODO these values are now saved in the object serverMultipliers. This variable is kept to load old library-files.
        [XmlArray]
        public double[][] multipliers; // multipliers[stat][m], m: 0:tamingadd, 1:tamingmult, 2:levelupdom, 3:levelupwild
        [XmlArray]
        public List<TimerListEntry> timerListEntries = new List<TimerListEntry>();
        [XmlArray]
        public List<IncubationTimerEntry> incubationListEntries = new List<IncubationTimerEntry>();
        [XmlArray]
        public List<string> hiddenOwners = new List<string>(); // which owners are not selected to be shown
        [XmlArray]
        public List<string> hiddenServers = new List<string>();
        [XmlArray]
        public List<string> dontShowTags = new List<string>(); // which tags are selected to be not shown
        public bool showDeads = true;
        public bool showUnavailable = true;
        public bool showNeutered = true;
        public bool showMutated = true;
        public bool showObelisk = true;
        public bool showCryopod = true;
        public bool useFiltersInTopStatCalculation = false;
        public int maxDomLevel = 73;
        public int maxWildLevel = 150;
        public int maxChartLevel = 50;
        public int maxBreedingSuggestions = 10;
        public bool considerWildLevelSteps = false;
        public int wildLevelStep = 5;
        public int maxServerLevel = 450; // on official servers a creature with more than 450 total levels will be deleted

        [DataMember]
        public ServerMultipliers serverMultipliers;
        [DataMember]
        public ServerMultipliers serverMultipliersEvents; // this object's statMultipliers are not used

        // these values are outdated and are only used to create the new ServerMultiplier-objects in 
        public double imprintingMultiplier = 1;
        public double babyCuddleIntervalMultiplier = 1;
        public double tamingSpeedMultiplier = 1;
        public double tamingFoodRateMultiplier = 1;
        public double MatingIntervalMultiplier = 1;
        public double EggHatchSpeedMultiplier = 1;
        public double BabyMatureSpeedMultiplier = 1;
        public double BabyFoodConsumptionSpeedMultiplier = 1;
        // event multiplier
        public double babyCuddleIntervalMultiplierEvent = 1;
        public double tamingSpeedMultiplierEvent = 1.5;
        public double tamingFoodRateMultiplierEvent = 1;
        public double MatingIntervalMultiplierEvent = 1;
        public double EggHatchSpeedMultiplierEvent = 1;
        public double BabyMatureSpeedMultiplierEvent = 1;
        public double BabyFoodConsumptionSpeedMultiplierEvent = 1;

        public bool singlePlayerSettings = false;
        public bool allowMoreThanHundredImprinting = false; // allow more than 100% imprinting, can happen with mods, e.g. S+ Nanny

        public bool changeCreatureStatusOnSavegameImport = true;

        [XmlArray]
        public List<string> modIDs;

        [XmlIgnore]
        public List<Mod> ModList = new List<Mod>();

        /// <summary>
        /// Hash-Code that represents the loaded mod-values and their order
        /// </summary>
        public int modListHash;

        [XmlArray]
        public List<Player> players = new List<Player>();
        [XmlArray]
        public List<Tribe> tribes = new List<Tribe>();
        public string additionalValues = "";
        [XmlArray]
        public List<Note> noteList = new List<Note>();
        [XmlIgnore]
        public List<string> tags = new List<string>();
        [XmlArray]
        public List<string> tagsInclude = new List<string>(); // which tags are checked for including in the breedingplan
        [XmlArray]
        public List<string> tagsExclude = new List<string>(); // which tags are checked for excluding in the breedingplan

        [XmlIgnore]
        public string[] ownerList; // temporary list of all owners (used in autocomplete / dropdowns)
        [XmlIgnore]
        public string[] serverList; // temporary list of all servers (used in autocomplete / dropdowns)

        /// <summary>
        /// Calculates a hashcode for a list of mods and their order. Can be used to check for changes.
        /// </summary>
        public static int CalculateModListId(List<Mod> modList)
        {
            if (modList == null) { return 0; }

            return string.Join(",", modList.Select(m => m.id).ToArray()).GetHashCode();
        }

        /// <summary>
        /// Recalculates the modListHash for comparison and sets the filenames of the modvalues for the library.
        /// Should be called after the mods are changed.
        /// </summary>
        public void UpdateModList()
        {
            modIDs = ModList?.Select(m => m.id).ToList() ?? new List<string>();
            modListHash = CalculateModListId(ModList);
        }

        /// <summary>
        /// Returns true if the currently loaded modValues differ from the listed modValues of the library-file.
        /// </summary>
        [XmlIgnore]
        public bool ModValueReloadNeeded { get { return modListHash == 0 || modListHash != Values.V.loadedModsHash; } }

        public bool mergeCreatureList(List<Creature> creaturesToMerge, bool update = false)
        {
            bool creaturesWereAdded = false;
            foreach (Creature creature in creaturesToMerge)
            {
                if (!creatures.Contains(creature))
                {
                    creatures.Add(creature);
                    creaturesWereAdded = true;
                }
                else if (update)
                {
                    // Merge in some specific parts: imprinting level, dom stats, name
                    var old = creatures.Single(c => c.guid == creature.guid);
                    if (old.Species != creature.Species) continue;

                    bool recalculate = false;
                    if (old.IsPlaceholder ||
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
                        old.motherGuid = creature.motherGuid;
                        old.motherName = creature.motherName;
                        old.fatherGuid = creature.fatherGuid;
                        old.fatherName = creature.fatherName;
                        old.mutationsMaternal = creature.mutationsMaternal;
                        old.mutationsPaternal = creature.mutationsPaternal;
                        old.name = creature.name;
                        old.neutered = creature.neutered;
                        old.note = creature.note;
                        old.owner = creature.owner;
                        old.server = creature.server;
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
                        old.IsPlaceholder = creature.IsPlaceholder;
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
                        old.recalculateCreatureValues(getWildLevelStep());
                }
            }
            return creaturesWereAdded;
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
            var unusedPlaceHolders = creatures.Where(c => c.IsPlaceholder).ToList();

            foreach (Creature c in creatures)
            {
                if (c.IsPlaceholder || c.flags.HasFlag(CreatureFlags.Deleted)) continue;

                var usedPlaceholder = unusedPlaceHolders.FirstOrDefault(p => p.guid == c.motherGuid || p.guid == c.fatherGuid);
                if (usedPlaceholder != null) unusedPlaceHolders.Remove(usedPlaceholder);

                if (unusedPlaceHolders.Count == 0) break;
            }

            foreach (var p in unusedPlaceHolders)
                creatures.Remove(p);
        }
    }
}
