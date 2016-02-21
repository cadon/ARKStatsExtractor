using System;
using System.Linq;
using System.Xml.Serialization;

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
        [XmlIgnore]
        public bool[] topBreedingStats; // indexes of stats that are top for that species in the creaturecollection. TODO: is it possible to exclude this from saving? It is and should recalculated when loaded or new creatures are added anyway
        public string owner;

        public Creature()
        {
            topBreedingStats = new bool[8];
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
            topBreedingStats = new bool[8];
        }

        public int level { get { return 1 + levelsWild.Sum() - levelsWild[7] + levelsDom.Sum() - levelsDom[7]; } }

        public Int32 topStatsCount { get { return topBreedingStats.Count(s => s); } }
    }

    public enum Gender
    {
        Neutral = 0, // or unknown
        Male = 1,
        Female = 2,
    };
}