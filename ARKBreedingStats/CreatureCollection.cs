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
        public double[] breedingMultipliers = new double[] { 1, 1 }; // multipliers[m], m: 0:EggHatchSpeedMultiplier, 1:BabyMatureSpeedMultiplier
        [XmlArray]
        public List<TimerListEntry> timerListEntries = new List<TimerListEntry>();
        [XmlArray]
        public List<string> hiddenOwners = new List<string>(); // which owners are not selected to be shown
        [XmlArray]
        public bool[] shownStatus = new bool[2]; // which status are selected to be shown. 0: dead, 1: unavailable
        public int maxDomLevel = 61;
        public int maxWildLevel = 150;
        public int maxBreedingSuggestions = 10;
        public double imprintingMultiplier = 1;
        [XmlArray]
        public List<Player> players = new List<Player>();
        [XmlArray]
        public List<Tribe> tribes = new List<Tribe>();
    }
}
