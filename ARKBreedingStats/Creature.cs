using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

//namespace XmlSerialization
namespace ARKBreedingStats
{
    //[DataContract]
    [Serializable()]
    public class Creature
    {
        //[DataMember]
        public string species;
        //[DataMember]
        public string name;
        //[DataMember]
        public int gender; // 0: unknown, 1: male, 2: female
        // order of the stats is Health, Stamina, Oxygen, Food, Weight, MeleeDamage, Speed, Torpor
        //[DataMember]
        public int[] levelsWild;
        //[DataMember]
        public int[] levelsDom;
        //[DataMember]
        public double tamingEff;
        public double[] valuesBreeding;
        public double[] valuesDom;

        public Creature(string species, string name, int gender, int[] levelsWild, int[] levelsDom, double tamingEff, double[] valuesBreeding, double[] valuesDom)
        {
            this.species = species;
            this.name = name;
            this.gender = gender;
            this.levelsWild = levelsWild;
            this.levelsDom = levelsDom;
            this.valuesBreeding = valuesBreeding;
            this.valuesDom = valuesDom;
            this.tamingEff = tamingEff;
        }

        public int level { get { return 1 + levelsWild.Sum() - levelsWild[7] + levelsDom.Sum() - levelsDom[7]; } }
    }
}