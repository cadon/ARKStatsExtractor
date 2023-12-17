using ARKBreedingStats.species;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ARKBreedingStats.Library
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Creature : IEquatable<Creature>
    {
        [JsonProperty]
        public string speciesBlueprint;
        private Species _species;
        [JsonProperty]
        public string name;
        [JsonProperty]
        public Sex sex;
        [JsonProperty("status")]
        private CreatureStatus _status;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public CreatureFlags flags;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int[] levelsWild;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int[] levelsDom;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int[] levelsMutated;

        /// <summary>
        /// The taming effectiveness (0: 0, 1: 100 %).
        /// Special values are:
        /// -1: TE is unknown (e.g. cannot be determined exactly for the giganotosaurus)
        /// -2: invalid TE (used in the extraction if different stats rely on a different TE).
        /// -3: creature is not yet domesticated, i.e. wild.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double tamingEff;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double imprintingBonus;

        public double[] valuesBreeding;
        public double[] valuesDom;
        /// <summary>
        /// Indices of stats that are top for that species in the creatureCollection
        /// </summary>
        public bool[] topBreedingStats;
        public short topStatsCount;
        /// <summary>
        /// topStatCount with all stats (regardless of considerStatHighlight[]) and without torpor (for breedingPlanner)
        /// </summary>
        public short topStatsCountBP;
        /// <summary>
        /// True if it has some topBreedingStats and if it's male, no other male has more topBreedingStats.
        /// </summary>
        public bool topBreedingCreature;
        /// <summary>
        /// True if the creature has only top stats of the stats that its species levels and that are considered.
        /// </summary>
        public bool onlyTopConsideredStats;
        /// <summary>
        /// Permille of mean of wildLevels compared to topLevels.
        /// </summary>
        public short topness;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string owner;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string imprinterName; // todo implement in creatureInfoInbox
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string tribe;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string server;
        /// <summary>
        /// User defined note about that creature.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string note;
        /// <summary>
        /// The guid used in ASB for parent-linking. The user cannot change it.
        /// </summary>
        [JsonProperty]
        public Guid guid;
        /// <summary>
        /// This field contains either the real Ark id or a user input value, depending on ArkIdImported.
        /// The real, unique creature's id in ARK is created by id1 &lt;&lt; 32 | id2. This is not the one that is shown to the user in game (see ArkIdInGame for that).
        /// This property is only set if the creature was imported.
        /// If ArkIdImported is false, this field can contain any user input value, intended is the creature's id in ARK like it is shown to the user in game.
        /// The shown id is not always unique. It's build from two 32 bit integers which are converted to strings and then concatenated.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long ArkId;
        /// <summary>
        /// If true it's assumed the ArkId is correct (in game visualization can be wrong). This field should only be true if the ArkId was imported.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool ArkIdImported;
        /// <summary>
        /// Ark id how it is shown in game.
        /// </summary>
        [JsonIgnore]
        public string ArkIdInGame;

        /// <summary>
        /// True if the creature is tamed or bred, false if it's wild.
        /// That property depends on the taming effectiveness.
        /// </summary>
        public bool isDomesticated => tamingEff > -3;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool isBred;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Guid fatherGuid;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Guid motherGuid;
        /// <summary>
        /// Only used during import to create placeholder ancestors.
        /// </summary>
        public string fatherName;
        /// <summary>
        /// Only used during import to create placeholder ancestors.
        /// </summary>
        public string motherName;
        /// <summary>
        /// Only the parent-guid is saved in the file, not the parent-object.
        /// </summary>
        private Creature father;
        /// <summary>
        /// Only the parent-guid is saved in the file, not the parent-object.
        /// </summary>
        private Creature mother;
        public int levelFound;
        /// <summary>
        /// Number of generations from the oldest wild creature.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int generation;

        /// <summary>
        /// Color ids.
        /// </summary>
        [JsonIgnore]
        public byte[] colors;
        [JsonProperty("colors", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int[] colorsSerialization
        {
            set => colors = value?.Select(i => (byte)i).ToArray();
            get => colors?.Select(i => (int)i).ToArray();
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

        private DateTime? _growingUntil;

        [JsonProperty]
        public DateTime? growingUntil
        {
            set
            {
                if (growingPaused && value != null)
                    growingLeft = value.Value.Subtract(DateTime.Now);
                else
                    _growingUntil = value == null || value <= DateTime.Now ? null : value;
            }
            get => !growingPaused ? _growingUntil : growingLeft.Ticks > 0 ? DateTime.Now.Add(growingLeft) : default(DateTime?);
        }

        public bool ShowInOverlay;

        public TimeSpan growingLeft;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool growingPaused;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? cooldownUntil;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? domesticatedAt;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? addedToLibrary;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int mutationsMaternal;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int mutationsPaternal;
        /// <summary>
        /// Number of new occurred maternal mutations
        /// </summary>
        [JsonProperty("mutMatNew", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int mutationsMaternalNew;
        /// <summary>
        /// Number of new occurred paternal mutations
        /// </summary>
        [JsonProperty("mutPatNew", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int mutationsPaternalNew;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> tags = new List<string>();

        /// <summary>
        /// Used to display the creature's position in a list.
        /// </summary>
        public int ListIndex;

        public Creature() { }

        public Creature(Species species, string name, string owner = null, string tribe = null, Sex sex = Sex.Unknown,
            int[] levelsWild = null, int[] levelsDom = null, int[] levelsMutated = null, double tamingEff = 0, bool isBred = false, double imprinting = 0, int? levelStep = null)
        {
            Species = species;
            this.name = name ?? string.Empty;
            this.owner = owner;
            this.tribe = tribe;
            this.sex = sex;
            this.levelsWild = levelsWild;
            this.levelsDom = levelsDom ?? new int[Stats.StatsCount];
            this.levelsMutated = levelsMutated;
            this.isBred = isBred;
            if (isBred)
            {
                this.tamingEff = 1;
                imprintingBonus = imprinting;
            }
            else
            {
                this.tamingEff = tamingEff;
                imprintingBonus = 0;
            }
            Status = CreatureStatus.Available;
            if (levelsWild == null) return;

            InitializeArrays();
            CalculateLevelFound(levelStep);
        }

        public Species Species
        {
            set
            {
                _species = value;
                if (value != null)
                    speciesBlueprint = value.blueprintPath;
            }
            get => _species;
        }

        /// <summary>
        /// Creates a placeholder creature with the given ArkId, which have to be imported
        /// </summary>
        /// <param name="arkId">ArkId from an imported source (no user input)</param>
        public Creature(long arkId)
        {
            ArkId = arkId;
            ArkIdImported = true;
            guid = Utils.ConvertArkIdToGuid(arkId);
            flags = CreatureFlags.Placeholder;
        }

        /// <summary>
        /// Creates a placeholder creature with a species and no other info.
        /// </summary>
        public Creature(Species species)
        {
            _species = species;
            flags = CreatureFlags.Placeholder;
        }

        public bool Equals(Creature other) => other != null && other.guid == guid;

        public override bool Equals(object obj) => obj is Creature creatureObj && creatureObj.guid == guid;

        public CreatureStatus Status
        {
            get => _status;
            set
            {
                // remove other status while keeping the other flags
                flags = (flags & CreatureFlags.StatusMask) | (CreatureFlags)(1 << (int)value);

                if (_status == value) return;

                if (Maturation < 1)
                {
                    if (value == CreatureStatus.Dead)
                        PauseMaturationTimer();
                    else if ((_status == CreatureStatus.Cryopod || _status == CreatureStatus.Obelisk)
                             && (value == CreatureStatus.Available || value == CreatureStatus.Unavailable))
                        StartMaturationTimer();
                    else if ((_status == CreatureStatus.Available || _status == CreatureStatus.Unavailable)
                             && (value == CreatureStatus.Cryopod || value == CreatureStatus.Obelisk))
                        PauseMaturationTimer();
                }

                _status = value;
            }
        }

        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }

        public void CalculateLevelFound(int? levelStep)
        {
            levelFound = 0;

            if (!isDomesticated)
            {
                levelFound = LevelHatched;
                return;
            }

            if (isBred || tamingEff < 0) return;

            if (levelStep.HasValue)
                levelFound = (int)Math.Round(LevelHatched / (1 + tamingEff / 2) / levelStep.Value) * levelStep.Value;
            else
                levelFound = (int)Math.Ceiling(Math.Round(LevelHatched / (1 + tamingEff / 2), 6));
        }

        /// <summary>
        /// The total level without domesticate levels, i.e. the torpidity level + 1.
        /// </summary>
        public int LevelHatched => (levelsWild?[Stats.Torpidity] ?? 0) + 1;

        /// <summary>
        /// The total current level inclusive domesticate levels.
        /// </summary>
        public int Level => LevelHatched + levelsDom.Sum();

        /// <summary>
        /// Force ancestor recalculation.
        /// </summary>
        public void RecalculateAncestorGenerations()
        {
            generation = -1;
            generation = AncestorGenerations();
            if (generation < 0) generation = 0;
        }

        /// <summary>
        /// Returns the number of generations to the oldest known ancestor
        /// </summary>
        /// <returns></returns>
        private int AncestorGenerations(int g = 0)
        {
            if (generation != -1)
            {
                // assume the generation is already calculated
                return generation;
            }

            // to detect loop (if a creature is falsely listed as its own ancestor)
            if (g > 299)
            {
                return -1;
            }

            int mgen = 0, fgen = 0;
            if (mother != null)
            {
                mgen = mother.AncestorGenerations(g + 1) + 1;
                if (mgen == 0)
                    return -1;
            }
            if (father != null)
            {
                fgen = father.AncestorGenerations(g + 1) + 1;
                if (fgen == 0)
                    return -1;
            }
            if (isBred && mgen == 0 && fgen == 0)
                generation = 1;
            generation = mgen > fgen ? mgen : fgen;
            return generation;
        }

        public Creature Mother
        {
            get => mother;
            set
            {
                mother = value;
                motherGuid = mother?.guid ?? Guid.Empty;
            }
        }
        public Creature Father
        {
            get => father;
            set
            {
                father = value;
                fatherGuid = father?.guid ?? Guid.Empty;
            }
        }

        /// <summary>
        /// Sets the count of top stats according to the considered stat indices.
        /// </summary>
        /// <param name="considerStatHighlight"></param>
        /// <param name="considerWastedStats">If false, stats that don't increase its wild value with levels don't make a creature non-top.</param>
        public void SetTopStatCount(bool[] considerStatHighlight, bool considerWastedStats)
        {
            if (Species == null
                || flags.HasFlag(CreatureFlags.Placeholder))
                return;

            if (topBreedingStats == null) topBreedingStats = new bool[Stats.StatsCount];

            short c = 0, cBP = 0;
            onlyTopConsideredStats = true;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (topBreedingStats[s])
                {
                    if (s != Stats.Torpidity)
                        cBP++;
                    if (considerStatHighlight[s])
                        c++;
                }
                else if (onlyTopConsideredStats && considerStatHighlight[s] && Species.UsesStat(s) && (considerWastedStats || Species.stats[s].IncPerWildLevel > 0))
                {
                    onlyTopConsideredStats = false;
                }
            }
            topStatsCount = c;
            topStatsCountBP = cBP;
        }

        /// <summary>
        /// call this function to recalculate all stat-values of Creature c according to its levels
        /// </summary>
        public void RecalculateCreatureValues(int? levelStep)
        {
            CalculateLevelFound(levelStep);
            if (Species == null || levelsWild == null) return;

            InitializeArrays();
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                valuesBreeding[s] = StatValueCalculation.CalculateValue(Species, s, levelsWild[s], levelsMutated?[s] ?? 0, 0, true, 1, 0);
                valuesDom[s] = StatValueCalculation.CalculateValue(Species, s, levelsWild[s], levelsMutated?[s] ?? 0, levelsDom[s], true, tamingEff, imprintingBonus);
            }
        }

        /// <summary>
        /// Recalculates the new occurred mutations.
        /// </summary>
        public void RecalculateNewMutations()
        {
            if (mother != null && mutationsMaternal > mother.Mutations)
            {
                mutationsMaternalNew = mutationsMaternal - mother.Mutations;
            }
            else mutationsMaternalNew = 0;
            if (father != null && mutationsPaternal > father.Mutations)
            {
                mutationsPaternalNew = mutationsPaternal - father.Mutations;
            }
            else mutationsPaternalNew = 0;
        }

        public int Mutations => mutationsMaternal + mutationsPaternal;

        public override string ToString() => $"{name} ({_species.name})";

        /// <summary>
        /// Starts the timer for maturation.
        /// </summary>
        private void StartMaturationTimer()
        {
            if (growingPaused)
            {
                growingPaused = false;
                if (growingLeft.Ticks <= 0)
                    growingUntil = null;
                else
                    growingUntil = DateTime.Now.Add(growingLeft);
            }
        }

        /// <summary>
        /// Pauses the timer for maturation.
        /// </summary>
        private void PauseMaturationTimer()
        {
            if (!growingPaused)
            {
                growingLeft = growingUntil?.Subtract(DateTime.Now) ?? TimeSpan.Zero;
                if (growingLeft.Ticks > 0)
                {
                    growingPaused = true;
                    return;
                }
                growingLeft = TimeSpan.Zero;
                growingUntil = null;
            }
        }

        /// <summary>
        /// Starts or stops the timer for maturation.
        /// </summary>
        public void StartStopMatureTimer(bool start)
        {
            if (start)
                StartMaturationTimer();
            else PauseMaturationTimer();
        }

        /// <summary>
        /// XmlSerializer does not support TimeSpan, so use this property for serialization instead.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [JsonProperty("growingLeft", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string GrowingLeftString
        {
            get => System.Xml.XmlConvert.ToString(growingLeft);
            set => growingLeft = string.IsNullOrEmpty(value) ?
                    TimeSpan.Zero : System.Xml.XmlConvert.ToTimeSpan(value);
        }

        /// <summary>
        /// Maturation of this creature, 0: baby, 1: adult.
        /// </summary>
        public double Maturation
        {
            get => Species?.breeding == null || growingUntil == null
                    ? 1
                    : 1 - growingUntil.Value.Subtract(DateTime.Now).TotalSeconds /
                    Species.breeding.maturationTimeAdjusted;
            set => growingUntil = Species?.breeding == null
                ? default(DateTime?)
                : DateTime.Now.AddSeconds(Species.breeding.maturationTimeAdjusted * (1 - value));
        }

        [OnDeserialized]
        private void Initialize(StreamingContext ct)
        {
            InitializeArkInGame();
            if (flags.HasFlag(CreatureFlags.Placeholder)) return;
            InitializeArrays();
        }

        /// <summary>
        /// Set the string of ArkIdInGame depending on the real ArkId or the user input number.
        /// </summary>
        internal void InitializeArkInGame() => ArkIdInGame = ArkIdImported ? Utils.ConvertImportedArkIdToIngameVisualization(ArkId) : ArkId.ToString();

        private void InitializeArrays()
        {
            if (levelsDom == null) levelsDom = new int[Stats.StatsCount];
            if (valuesBreeding == null) valuesBreeding = new double[Stats.StatsCount];
            if (valuesDom == null) valuesDom = new double[Stats.StatsCount];
            if (topBreedingStats == null) topBreedingStats = new bool[Stats.StatsCount];
        }

        /// <summary>
        /// Sets flags of properties that are stored in their own field.
        /// Should be called until the flags are used globally and if no backwards compatibility is needed anymore.
        /// </summary>
        public void InitializeFlags()
        {
            // status
            flags = (flags & CreatureFlags.StatusMask) | (CreatureFlags)(1 << (int)_status);
            // sex
            flags = (flags & ~(CreatureFlags.Female | CreatureFlags.Male)) | (sex == Sex.Female ? CreatureFlags.Female : sex == Sex.Male ? CreatureFlags.Male : CreatureFlags.None);
            // mutated
            flags = (flags & ~CreatureFlags.Mutated) | (Mutations > 0 ? CreatureFlags.Mutated : CreatureFlags.None);
        }

        /// <summary>
        /// Calculates the pretame wild level. This value can be off due to wrong inputs due to ingame rounding.
        /// </summary>
        /// <param name="postTameLevel"></param>
        /// <param name="tamingEffectiveness"></param>
        /// <returns></returns>
        internal static int CalculatePreTameWildLevel(int postTameLevel, double tamingEffectiveness) => (int)Math.Ceiling(Math.Round(postTameLevel / (1 + tamingEffectiveness / 2), 6));
    }

    public enum Sex
    {
        Unknown = 0,
        Male = 1,
        Female = 2
    };

    public enum CreatureStatus
    {
        Available,
        Dead,
        Unavailable,
        Obelisk,
        Cryopod
    };

    [Flags]
    public enum CreatureFlags
    {
        None = 0,
        Available = 1,
        Dead = 2,
        Unavailable = 4,
        Obelisk = 8,
        Cryopod = 16,
        // Deleted = 32, // not used anymore
        Mutated = 64,
        Neutered = 128,
        /// <summary>
        /// If a creature has unknown parents, they are placeholders until they are imported. placeholders are not shown in the library
        /// </summary>
        Placeholder = 256,
        Female = 512,
        Male = 1024,
        MutagenApplied = 2048,
        /// <summary>
        /// Indicates a dummy creature used as a species separator in the library listView.
        /// </summary>
        Divider = 4096,
        /// <summary>
        /// If applied to the flags with &, the status is removed.
        /// </summary>
        StatusMask = Mutated | Neutered | Placeholder | Female | Male | MutagenApplied | Divider
    }
}