using System;
using System.Linq;

namespace ARKBreedingStats
{
    [Serializable()]
    public class Creature
    {
        public string species;
        public string name;
        public Gender gender;
        // order of the stats is Health, Stamina, Oxygen, Food, Weight, MeleeDamage, Speed, Torpor
        public int[] levelsWild;
        public int[] levelsDom;
        public double tamingEff;
        public double[] valuesBreeding;
        public double[] valuesDom;
        public bool[] topBreedingStats; // indexes of stats that are top for that species in the creaturecollection

        public Creature()
        {
            this.topBreedingStats = new bool[] { false, false, false, false, false, false, false, false };
        }

        public Creature(string species, string name, Gender gender, int[] levelsWild, int[] levelsDom, double tamingEff, double[] valuesBreeding, double[] valuesDom)
        {
            this.species = species;
            this.name = name;
            this.gender = (Gender)gender;
            this.levelsWild = levelsWild;
            this.levelsDom = levelsDom;
            this.valuesBreeding = valuesBreeding;
            this.valuesDom = valuesDom;
            this.tamingEff = tamingEff;
            this.topBreedingStats = new bool[] { false, false, false, false, false, false, false, false };
        }

        public int level { get { return 1 + levelsWild.Sum() - levelsWild[7] + levelsDom.Sum() - levelsDom[7]; } }

        public bool isTopCreature
        {
            get { return (topBreedingStats[0] || topBreedingStats[1] || topBreedingStats[2] || topBreedingStats[3] || topBreedingStats[4] || topBreedingStats[5] || topBreedingStats[6]); }
        }
    }

    public enum Gender
    {
        Neutral = 0, // or unknown
        Male = 1,
        Female = 2,
    };
}