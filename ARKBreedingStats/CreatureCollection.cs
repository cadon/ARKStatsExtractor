using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace ARKBreedingStats
{
    [Serializable()]
    public class CreatureCollection // simple placeholder for XML serialization
    {
        [XmlArray]
        public List<Creature> creatures = new List<Creature>();
        [XmlArray]
        public List<species.CreatureValues> creaturesValues = new List<species.CreatureValues>();
        [XmlArray]
        public double[][] multipliers; // multipliers[stat][m], m: 0:tamingadd, 1:tamingmult, 2:levelupdom, 3:levelupwild
        [XmlArray]
        public List<TimerListEntry> timerListEntries = new List<TimerListEntry>();
        [XmlArray]
        public List<IncubationTimerEntry> incubationListEntries = new List<IncubationTimerEntry>();
        [XmlArray]
        public List<string> hiddenOwners = new List<string>(); // which owners are not selected to be shown
        [XmlArray]
        internal List<string> hiddenServers = new List<string>();
        public bool showDeads = true;
        public bool showUnavailable = true;
        public bool showNeutered = true;
        public bool showMutated = true;
        public bool showObelisk = true;
        public bool useFiltersInTopStatCalculation = false;
        public int maxDomLevel = 71;
        public int maxWildLevel = 150;
        public int maxChartLevel = 50;
        public int maxBreedingSuggestions = 10;
        public bool considerWildLevelSteps = false;
        public int wildLevelStep = 5;

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
        public List<string> tagsExclude = new List<string>(); // which tags are checked for excluding in the breedingplan

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
                    if (old.species != creature.species) continue;

                    if (old.name != creature.name)
                    {
                        old.name = creature.name;
                        creaturesWereAdded = true;
                    }

                    bool recalculate = false;
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
    }
}
