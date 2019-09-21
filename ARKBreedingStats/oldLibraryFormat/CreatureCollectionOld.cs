using ARKBreedingStats.Library;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ARKBreedingStats.oldLibraryFormat
{
    /// <summary>
    /// This is the old format and should only be used for loading, then converting
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "CreatureCollection")]
    public class CreatureCollectionOld
    {
        public string FormatVersion; // currently set to 1.12 to represent the supported 12 stats
        [XmlArray]
        public List<CreatureOld> creatures = new List<CreatureOld>();
        [XmlArray]
        public List<CreatureValuesOld> creaturesValues = new List<CreatureValuesOld>();
        // TODO these values are now saved in the object serverMultipliers. This variable is kept to load old library-files.
        [XmlArray]
        public double[][] multipliers; // multipliers[stat][m], m: 0:tamingadd, 1:tamingmult, 2:levelupdom, 3:levelupwild
        [XmlArray]
        public List<TimerListEntryOld> timerListEntries = new List<TimerListEntryOld>();
        [XmlArray]
        public List<IncubationTimerEntryOld> incubationListEntries = new List<IncubationTimerEntryOld>();
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

        public int? getWildLevelStep()
        {
            return considerWildLevelSteps ? wildLevelStep : default(int?);
        }
    }
}
