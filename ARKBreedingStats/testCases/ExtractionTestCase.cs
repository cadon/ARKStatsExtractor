using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ARKBreedingStats.testCases
{
    [Serializable]
    public class ExtractionTestCase
    {
        public string testName;
        public string species;
        public string speciesName;
        public string speciesBP;
        public double[] statValues;
        public int[] levelsWild;
        public int[] levelsDom;
        public bool postTamed;
        public bool bred;
        public double tamingEff;
        public double imprintingBonus;
        public ServerMultipliers serverMultipliers;
        public bool singleplayerSettings;
        public int maxWildLevel;
        public bool allowMoreThanHundredPercentImprinting;
        [XmlArray]
        private List<string> modIDs;
        public int modListHash;
        [XmlIgnore]
        public int totalLevel => levelsWild[(int)StatNames.Torpidity] + 1 + levelsDom.Sum();

        public Species Species
        {
            set
            {
                speciesName = value.name;
                speciesBP = value.blueprintPath;
            }
        }

        public List<string> ModIDs
        {
            set
            {
                modIDs = value ?? new List<string>();
                UpdateModHash();
            }
            get { return modIDs; }
        }

        public void UpdateModHash()
        {
            modListHash = CreatureCollection.CalculateModListHash(modIDs);
        }
    }
}
