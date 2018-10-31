using System;
using System.Linq;
using System.Xml.Serialization;

namespace ARKBreedingStats.species
{
    /// <summary>
    /// This class is used to store creature-values of creatures that couldn't be extracted, to store their values temporarily until the issue is solved
    /// </summary>
    [Serializable()]
    public class CreatureValues
    {
        public string species;
        public Guid guid;
        public long ARKID;
        public string name;
        public Sex sex;
        // order of the stats is Health, Stamina, Oxygen, Food, Weight, MeleeDamage, Speed, Torpor
        public double[] statValues = new double[8];
        public int[] levelsWild = new int[8];
        public int[] levelsDom = new int[8];
        public int level = 0;
        public double tamingEffMin, tamingEffMax;
        public double imprintingBonus;
        public bool isTamed, isBred;
        public string owner = "";
        public string imprinterName = "";
        public string tribe = "";
        public string server = "";
        public long fatherArkId; // used when importing creatures, parent's are indicated by this id
        public long motherArkId;
        public Guid motherGuid;
        public Guid fatherGuid;
        public bool ArkIdImported; // if true, the ArkId is used to create the Guid
        [XmlIgnore]
        private Creature mother;
        [XmlIgnore]
        private Creature father;
        public DateTime growingUntil = new DateTime(0);
        public DateTime cooldownUntil = new DateTime(0);
        public DateTime domesticatedAt = new DateTime(0);
        public bool neutered = false;
        public int mutationCounter, mutationCounterMother, mutationCounterFather;
        public int[] colorIDs = new int[6];

        public CreatureValues()
        {
        }

        public CreatureValues(string species, string name, string owner, string tribe, Sex sex,
            double[] statValues, int level, double tamingEffMin, double tamingEffMax, bool isTamed, bool isBred, double imprintingBonus, bool neutered,
            Creature mother, Creature father)
        {
            this.species = species;
            this.name = name;
            this.owner = owner;
            this.tribe = tribe;
            this.sex = sex;
            this.statValues = statValues;
            this.level = level;
            this.tamingEffMin = tamingEffMin;
            this.tamingEffMax = tamingEffMax;
            this.isTamed = isTamed;
            this.isBred = isBred;
            this.imprintingBonus = imprintingBonus;
            this.neutered = neutered;
            Mother = mother;
            Father = father;
        }

        [XmlIgnore]
        public Creature Mother
        {
            set
            {
                mother = value;
                motherArkId = (mother != null ? mother.ArkId : 0);
                motherGuid = (mother != null ? mother.guid : Guid.Empty);
            }
            get { return mother; }
        }
        [XmlIgnore]
        public Creature Father
        {
            set
            {
                father = value;
                fatherArkId = (father != null ? father.ArkId : 0);
                fatherGuid = (father != null ? father.guid : Guid.Empty);
            }
            get { return father; }
        }
    }
}