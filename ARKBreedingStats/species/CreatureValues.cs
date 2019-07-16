using ARKBreedingStats.values;
using System;
using System.Xml.Serialization;

namespace ARKBreedingStats.species
{
    /// <summary>
    /// This class is used to store creature-values of creatures that couldn't be extracted, to store their values temporarily until the issue is solved
    /// </summary>
    [Serializable]
    public class CreatureValues
    {
        /// <summary>
        /// Used to identify the species
        /// </summary>
        internal string speciesBlueprint;
        /// <summary>
        /// Used for displaying the speciesName
        /// </summary>
        internal string speciesName;
        [XmlIgnore]
        private Species speciesObject; // TODO rename to species
        [ObsoleteAttribute("Use speciesName for the species name instead. For identification only use speciesBP.")]
        public string species;
        public Guid guid;
        public long ARKID;
        public string name;
        public Sex sex;
        // order of the stats is Health, Stamina, Oxygen, Food, Weight, MeleeDamage, Speed, Torpor
        public double[] statValues = new double[Values.STATS_COUNT];
        public int[] levelsWild = new int[Values.STATS_COUNT];
        public int[] levelsDom = new int[Values.STATS_COUNT];
        public int level = 0;
        public double tamingEffMin, tamingEffMax;
        public double imprintingBonus;
        public bool isTamed, isBred;
        public string owner = "";
        public string imprinterName = "";
        public string tribe = "";
        public string server = "";
        public long fatherArkId; // used when importing creatures, parents are indicated by this id
        public long motherArkId;
        public Guid motherGuid;
        public Guid fatherGuid;
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

        [XmlIgnore]
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

        [XmlIgnore]
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

        [XmlIgnore]
        public Species Species
        {
            set
            {
                speciesObject = value;
                if (value != null)
                {
                    speciesBlueprint = value.blueprintPath;
                }
            }
            get
            {
                if (speciesObject == null)
                {
                    speciesObject = Values.V.speciesByBlueprint(speciesBlueprint);
                    // TODO this section is only necessary for backwards compatibility
                    if (speciesObject == null)
                        Values.V.TryGetSpeciesByName(string.IsNullOrEmpty(speciesName) ? species : speciesName, out speciesObject);
                    if (speciesObject != null)
                        speciesBlueprint = speciesObject.blueprintPath;
                    // TODO end section
                }
                return speciesObject;
            }
        }
    }
}
