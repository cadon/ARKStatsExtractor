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
        public bool[] topBreedingStats; // indexes of stats that are top for that species in the creaturecollection
        [XmlIgnore]
        public bool topBreedingCreature; // true if it has some topBreedingStats and if it's male, no other male has more topBreedingStats
        public string owner;
        public string note; // user defined note about that creature
        public Guid guid;
        public bool isBred;
        public Guid fatherGuid;
        public Guid motherGuid;
        [XmlIgnore]
        public Creature father;
        [XmlIgnore]
        public Creature mother;
        public int generation; // number of generations from the oldest wild creature

        public Creature()
        {
            initVars();
        }

        public Creature(string species, string name, Gender gender, int[] levelsWild, int[] levelsDom, double tamingEff, bool isBred)
        {
            this.species = species;
            this.name = name;
            this.gender = (Gender)gender;
            this.levelsWild = levelsWild;
            this.levelsDom = levelsDom;
            this.tamingEff = tamingEff;
            this.isBred = isBred;
            initVars();
        }

        private void initVars()
        {
            topBreedingStats = new bool[8];
            valuesBreeding = new double[8];
            valuesDom = new double[8];
        }

        public int level { get { return 1 + levelsWild.Sum() - levelsWild[7] + levelsDom.Sum() - levelsDom[7]; } }

        public Int32 topStatsCount { get { return topBreedingStats.Count(s => s); } }

        public void recalculateAncestorGenerations()
        {
            generation = ancestorGenerations(this, 0);
        }

        /// <summary>
        /// Returns the number of generations to the oldest known ancestor
        /// </summary>
        /// <param name="c">Creature to check</param>
        /// <param name="g">Generations so far</param>
        /// <returns></returns>
        private int ancestorGenerations(Creature c, int g = 0)
        {
            int mgen = 0, fgen = 0;
            if (c.mother != null)
                mgen = ancestorGenerations(c.mother, g) + 1;
            if (c.father != null)
                fgen = ancestorGenerations(c.father, g) + 1;
            if (mgen > fgen)
                return mgen + g;
            else
                return fgen + g;
        }
    }

    public enum Gender
    {
        Unknown = 0,
        Male = 1,
        Female = 2,
    };
}