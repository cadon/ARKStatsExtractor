using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Runtime.Serialization;

namespace ARKBreedingStats.Library
{
    /// <summary>
    /// This class is used to store creature-values of creatures that couldn't be extracted, to store their values temporarily until the issue is solved
    /// </summary>
    [DataContract]
    public class CreatureValues
    {
        /// <summary>
        /// Used to identify the species
        /// </summary>
        [DataMember]
        internal string speciesBlueprint;
        /// <summary>
        /// Used for displaying the speciesName
        /// </summary>
        [DataMember]
        internal string speciesName;
        [IgnoreDataMember]
        private Species _species;
        [DataMember]
        public Guid guid;
        [DataMember]
        public long ARKID;
        [DataMember]
        public string name;
        [DataMember]
        public Sex sex;
        // order of the stats is Health, Stamina, Oxygen, Food, Weight, MeleeDamage, Speed, Torpor
        [DataMember]
        public double[] statValues = new double[Values.STATS_COUNT];
        [DataMember]
        public int[] levelsWild = new int[Values.STATS_COUNT];
        [DataMember]
        public int[] levelsDom = new int[Values.STATS_COUNT];
        [DataMember]
        public int level = 0;
        [DataMember]
        public double tamingEffMin, tamingEffMax;
        [DataMember]
        public double imprintingBonus;
        [DataMember]
        public bool isTamed, isBred;
        [DataMember]
        public string owner = "";
        [DataMember]
        public string imprinterName = "";
        [DataMember]
        public string tribe = "";
        [DataMember]
        public string server = "";
        [DataMember]
        public long fatherArkId; // used when importing creatures, parents are indicated by this id
        [DataMember]
        public long motherArkId;
        [DataMember]
        public Guid motherGuid;
        [DataMember]
        public Guid fatherGuid;
        [IgnoreDataMember]
        private Creature mother;
        [IgnoreDataMember]
        private Creature father;
        [DataMember]
        public DateTime? growingUntil;
        [DataMember]
        public DateTime? cooldownUntil;
        [DataMember]
        public DateTime? domesticatedAt;
        [DataMember]
        public bool neutered;
        [DataMember]
        public int mutationCounter, mutationCounterMother, mutationCounterFather;
        [DataMember]
        public int[] colorIDs = new int[6];

        public CreatureValues() { }

        public CreatureValues(Species species, string name, string owner, string tribe, Sex sex,
                double[] statValues, int level, double tamingEffMin, double tamingEffMax, bool isTamed, bool isBred, double imprintingBonus, bool neutered,
                Creature mother, Creature father)
        {
            this.Species = species;
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

        [IgnoreDataMember]
        public Creature Mother
        {
            get => mother;
            set
            {
                mother = value;
                motherArkId = mother?.ArkId ?? 0;
                motherGuid = mother?.guid ?? Guid.Empty;
            }
        }

        [IgnoreDataMember]
        public Creature Father
        {
            get => father;
            set
            {
                father = value;
                fatherArkId = father?.ArkId ?? 0;
                fatherGuid = father?.guid ?? Guid.Empty;
            }
        }

        [IgnoreDataMember]
        public Species Species
        {
            set
            {
                _species = value;
                speciesBlueprint = value?.blueprintPath ?? string.Empty;
                speciesName = value?.name ?? string.Empty;
            }
            get
            {
                if (_species == null)
                {
                    _species = Values.V.speciesByBlueprint(speciesBlueprint);
                }
                return _species;
            }
        }
    }
}
