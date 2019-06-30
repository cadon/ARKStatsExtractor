using ARKBreedingStats.species;
using System;
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
        [XmlArray]
        public double[][] multipliers; // multipliers[stat][m], m: 0:tamingadd, 1:tamingmult, 2:levelupdom, 3:levelupwild
        public string multiplierModifierFile;
        public double matureSpeedMultiplier;
        public double cuddleIntervalMultiplier;
        public double imprintingStatScaleMultiplier;
        public bool singleplayerSettings;
        public int maxWildLevel;
        public bool allowMoreThanHundredPercentImprinting;
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
    }
}
