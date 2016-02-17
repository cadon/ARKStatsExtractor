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
        public string species { get; set; }
        //[DataMember]
        public string name { get; set; }
        //[DataMember]
        public int sex { get; set; } // 0: unknown, 1: male, 2: female
        // order of the stats is Health, Stamina, Oxygen, Food, Weight, MeleeDamage, Speed, Torpor
        //[DataMember]
        public int[] levelsWild { get; set; }
        //[DataMember]
        public int[] levelsDom { get; set; }
        //[DataMember]
        public double tamingEff { get; set; }
        public double[] valuesBreeding { get; set; }
        public double[] valuesDom { get; set; }

        public Creature()
        {
            ;
        }

        public Creature(string species, string name, int sex, int[] levelsWild, int[] levelsDom, double tamingEff, double[] valuesBreeding, double[] valuesDom)
        {
            this.species = species;
            this.name = name;
            this.sex = sex;
            this.levelsWild = levelsWild;
            this.levelsDom = levelsDom;
            this.valuesBreeding = valuesBreeding;
            this.valuesDom = valuesDom;
            this.tamingEff = tamingEff;
        }

        public int level { get { return 1 + levelsWild.Sum() - levelsWild[7] + levelsDom.Sum() - levelsDom[7]; } }
    }
}