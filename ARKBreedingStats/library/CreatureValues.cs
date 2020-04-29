using ARKBreedingStats.species;
using ARKBreedingStats.values;
using Newtonsoft.Json;
using System;

namespace ARKBreedingStats.Library
{
    /// <summary>
    /// This class is used to store creature-values of creatures that couldn't be extracted, to store their values temporarily until the issue is solved
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class CreatureValues
    {
        /// <summary>
        /// Used to identify the species
        /// </summary>
        [JsonProperty]
        internal string speciesBlueprint;
        /// <summary>
        /// Used for displaying the speciesName
        /// </summary>
        [JsonProperty]
        internal string speciesName;
        private Species _species;
        [JsonProperty]
        public Guid guid;
        [JsonProperty]
        public long ARKID;
        [JsonProperty]
        public string name;
        [JsonProperty]
        public Sex sex;
        [JsonProperty]
        public double[] statValues = new double[Values.STATS_COUNT];
        [JsonProperty]
        public int[] levelsWild = new int[Values.STATS_COUNT];
        [JsonProperty]
        public int[] levelsDom = new int[Values.STATS_COUNT];
        [JsonProperty]
        public int level = 0;
        [JsonProperty]
        public double tamingEffMin, tamingEffMax;
        [JsonProperty]
        public double imprintingBonus;
        [JsonProperty]
        public bool isTamed, isBred;
        [JsonProperty]
        public string owner = "";
        [JsonProperty]
        public string imprinterName = "";
        [JsonProperty]
        public string tribe = "";
        [JsonProperty]
        public string server = "";
        [JsonProperty]
        public long fatherArkId; // used when importing creatures, parents are indicated by this id
        [JsonProperty]
        public long motherArkId;
        [JsonProperty]
        public Guid motherGuid;
        [JsonProperty]
        public Guid fatherGuid;
        private Creature mother;
        private Creature father;
        [JsonProperty]
        public DateTime? growingUntil;
        [JsonProperty]
        public DateTime? cooldownUntil;
        [JsonProperty]
        public DateTime? domesticatedAt;
        [JsonProperty]
        public CreatureFlags flags;
        [JsonProperty]
        public int mutationCounter, mutationCounterMother, mutationCounterFather;
        [JsonProperty]
        public int[] colorIDs = new int[6];

        public CreatureValues() { }

        public CreatureValues(Species species, string name, string owner, string tribe, Sex sex,
                double[] statValues, int level, double tamingEffMin, double tamingEffMax, bool isTamed, bool isBred, double imprintingBonus, CreatureFlags flags,
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
            this.flags = flags;
            Mother = mother;
            Father = father;
        }

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

        public Species Species
        {
            set
            {
                _species = value;
                if (value != null)
                {
                    speciesBlueprint = value.blueprintPath;
                    speciesName = value.name;
                }
            }
            get
            {
                if (_species == null)
                {
                    _species = Values.V.SpeciesByBlueprint(speciesBlueprint);
                }
                return _species;
            }
        }
    }
}
