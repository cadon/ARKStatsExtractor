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
        public double[][] multipliers; // multipliers[stat][m], m: 0:tamingadd, 1:tamingmult, 2:levelupdom, 3:levelupwild
        [XmlArray]
        public List<TimerListEntry> timerListEntries = new List<TimerListEntry>();
        [XmlArray]
        public List<IncubationTimerEntry> incubationListEntries = new List<IncubationTimerEntry>();
        [XmlArray]
        public List<string> hiddenOwners = new List<string>(); // which owners are not selected to be shown
        public bool showDeads = true;
        public bool showUnavailable = true;
        public bool showNeutered = true;
        public bool showMutated = true;
        public bool useFiltersInTopStatCalculation = false;
        public int maxDomLevel = 66;
        public int maxWildLevel = 150;
        public int maxChartLevel = 50;
        public int maxBreedingSuggestions = 10;
        public bool considerWildLevelSteps = true;
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
        [XmlArray]
        public List<Player> players = new List<Player>();
        [XmlArray]
        public List<Tribe> tribes = new List<Tribe>();
        public string additionalValues = "";
        [XmlArray]
        public List<Note> noteList = new List<Note>();

        public bool mergeCreatureList(List<Creature> creaturesToMerge)
        {
            bool creaturesWereAdded = false;
            foreach (Creature creature in creaturesToMerge)
            {
                if (!creatures.Contains(creature))
                {
                    creatures.Add(creature);
                    creaturesWereAdded = true;
                }
            }
            return creaturesWereAdded;
        }
    }
}
