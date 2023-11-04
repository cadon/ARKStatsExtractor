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
        public string speciesName;
        public string speciesBlueprintPath;
        public double[] statValues;
        public int[] levelsWild;
        public int[] levelsDom;
        public int[] levelsMut;
        public bool postTamed;
        public bool bred;
        public double tamingEff;
        public double imprintingBonus;
        public ServerMultipliers serverMultipliers;
        public bool singleplayerSettings;
        public bool AtlasSettings;
        public int maxWildLevel;
        public bool allowMoreThanHundredPercentImprinting;
        [XmlArray]
        private List<string> modIDs;
        public int modListHash;
        [XmlIgnore]
        public int totalLevel => levelsWild[Stats.Torpidity] + 1 + levelsDom.Sum();

        public Species Species
        {
            set
            {
                speciesName = value?.name ?? string.Empty;
                speciesBlueprintPath = value?.blueprintPath ?? string.Empty;
            }
        }

        public List<string> ModIDs
        {
            set
            {
                modIDs = value ?? new List<string>();
                modListHash = Library.CreatureCollection.CalculateModListHash(modIDs);
            }
            get { return modIDs; }
        }
    }
}
