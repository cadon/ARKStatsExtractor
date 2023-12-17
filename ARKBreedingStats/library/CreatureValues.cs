using ARKBreedingStats.species;
using ARKBreedingStats.values;
using Newtonsoft.Json;
using System;
using System.Linq;

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
        private Species _species;
        [JsonProperty]
        public Guid guid;
        /// <summary>
        /// Real Ark Id, not the one displayed ingame. Can only be set by importing a creature.
        /// </summary>
        [JsonProperty]
        public long ARKID;
        /// <summary>
        /// Ark Id like it is shown in game. Is not unique, because it's built by two 32 bit integers concatenated as strings.
        /// </summary>
        [JsonProperty]
        public string ArkIdInGame;
        [JsonProperty]
        public string name;
        [JsonProperty]
        public Sex sex;
        [JsonProperty]
        public double[] statValues = new double[Stats.StatsCount];
        [JsonProperty]
        public int[] levelsWild = new int[Stats.StatsCount];
        [JsonProperty]
        public int[] levelsMut = new int[Stats.StatsCount];
        [JsonProperty]
        public int[] levelsDom = new int[Stats.StatsCount];
        [JsonProperty]
        public int level;
        [JsonProperty]
        public double tamingEffMin, tamingEffMax;
        [JsonProperty]
        public double imprintingBonus;
        [JsonProperty]
        public bool isTamed, isBred;
        [JsonProperty]
        public string owner;
        [JsonProperty]
        public string imprinterName;
        [JsonProperty]
        public string tribe;
        [JsonProperty]
        public string server;
        [JsonProperty]
        public string note;
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
        [JsonIgnore]
        public byte[] colorIDs = new byte[Ark.ColorRegionCount];
        [JsonProperty("colorIDs", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int[] colorIDsSerialization
        {
            set => colorIDs = value?.Select(i => (byte)i).ToArray();
            get => colorIDs?.Select(i => (int)i).ToArray();
        }
        /// <summary>
        /// Some color ids cannot be determined uniquely because of equal color values.
        /// If this property is set it contains the other possible color ids.
        /// </summary>
        [JsonIgnore]
        public byte[] ColorIdsAlsoPossible;
        [JsonProperty("altCol", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int[] ColorIdsAlsoPossibleSerialization
        {
            set => ColorIdsAlsoPossible = value?.Select(i => (byte)i).ToArray();
            get => ColorIdsAlsoPossible?.Select(i => (int)i).ToArray();
        }

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
