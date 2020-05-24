using ARKBreedingStats.species;
using ARKBreedingStats.values;
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
        [JsonProperty]
        public CreatureFlags flags;
        [JsonProperty]
        public int[] levelsWild = new int[Values.STATS_COUNT];
        [JsonProperty]
        public int[] levelsDom = new int[Values.STATS_COUNT];
        [JsonProperty]
        public double tamingEff;
        [JsonProperty]
        public double imprintingBonus;

        public double[] valuesBreeding = new double[Values.STATS_COUNT];
        public double[] valuesDom = new double[Values.STATS_COUNT];
        /// <summary>
        /// Indices of stats that are top for that species in the creaturecollection
        /// </summary>
        public bool[] topBreedingStats = new bool[Values.STATS_COUNT];
        public short topStatsCount;
        /// <summary>
        /// topstatcount with all stats (regardless of considerStatHighlight[]) and without torpor (for breedingplanner)
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
        /// Permille of mean of wildlevels compared to toplevels.
        /// </summary>
        public short topness;
        [JsonProperty]
        public string owner;
        [JsonProperty]
        public string imprinterName; // todo implement in creatureInfoInbox
        [JsonProperty]
        public string tribe;
        [JsonProperty]
        public string server;
        /// <summary>
        /// User defined note about that creature.
        /// </summary>
        [JsonProperty]
        public string note;
        /// <summary>
        /// The guid used in ASB for parent-linking. The user cannot change it.
        /// </summary>
        [JsonProperty]
        public Guid guid;
        /// <summary>
        /// The creature's id in ARK. This id is shown to the user ingame, but it's not always unique. (It's build from two integers which are concatenated as strings).
        /// </summary>
        [JsonProperty]
        public long ArkId;
        /// <summary>
        /// If true it's assumed the ArkId is correct (ingame visualization can be wrong). This field should only be true if the ArkId was imported.
        /// </summary>
        [JsonProperty]
        public bool ArkIdImported;
        [JsonProperty]
        public bool isBred;
        [JsonProperty]
        public Guid fatherGuid;
        [JsonProperty]
        public Guid motherGuid;
        /// <summary>
        /// Only set if the id is imported.
        /// </summary>
        public long motherArkId;
        /// <summary>
        /// Only set if the id is imported.
        /// </summary>
        public long fatherArkId;
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
        [JsonProperty]
        public int generation;
        /// <summary>
        /// Color ids.
        /// </summary>
        [JsonProperty]
        public int[] colors;
        [JsonProperty]
        public DateTime? growingUntil;
        public TimeSpan growingLeft;
        [JsonProperty]
        public bool growingPaused;
        [JsonProperty]
        public DateTime? cooldownUntil;
        [JsonProperty]
        public DateTime? domesticatedAt;
        [JsonProperty]
        public DateTime? addedToLibrary;
        [JsonProperty]
        public int mutationsMaternal;
        [JsonProperty]
        public int mutationsPaternal;
        /// <summary>
        /// Number of new occured maternal mutations
        /// </summary>
        [JsonProperty("mutMatNew")]
        public int mutationsMaternalNew;
        /// <summary>
        /// Number of new occured paternal mutations
        /// </summary>
        [JsonProperty("mutPatNew")]
        public int mutationsPaternalNew;
        [JsonProperty]
        public List<string> tags = new List<string>();

        public Creature()
        {
            levelsWild = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }; // unknown wild levels
        }

        public Creature(Species species, string name, string owner, string tribe, Sex sex, int[] levelsWild, int[] levelsDom = null, double tamingEff = 0, bool isBred = false, double imprinting = 0, int? levelStep = null)
        {
            Species = species;
            this.name = name ?? string.Empty;
            this.owner = owner;
            this.tribe = tribe;
            this.sex = sex;
            this.levelsWild = levelsWild;
            this.levelsDom = levelsDom ?? new int[Values.STATS_COUNT];
            if (isBred)
                this.tamingEff = 1;
            else
                this.tamingEff = tamingEff;
            this.isBred = isBred;
            imprintingBonus = imprinting;
            Status = CreatureStatus.Available;
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
            levelsWild = new[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }; // unknown wild levels
            flags = CreatureFlags.Placeholder;
        }

        public bool Equals(Creature other) => other != null && other.guid == guid;

        public override bool Equals(object obj) => obj is Creature creatureObj && creatureObj.guid == guid;

        public CreatureStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                // remove other status while keeping the other flags
                flags = (flags & CreatureFlags.StatusMask) | (CreatureFlags)(1 << (int)value);
            }
        }

        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }

        public void CalculateLevelFound(int? levelStep)
        {
            levelFound = 0;
            if (!isBred && tamingEff >= 0)
            {
                if (levelStep.HasValue)
                    levelFound = (int)Math.Round(LevelHatched / (1 + tamingEff / 2) / levelStep.Value) * levelStep.Value;
                else
                    levelFound = (int)Math.Ceiling(Math.Round(LevelHatched / (1 + tamingEff / 2), 6));
            }
        }

        /// <summary>
        /// The total level without domesticate levels.
        /// </summary>
        public int LevelHatched => levelsWild[(int)StatNames.Torpidity] + 1;

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
            if (g > 99)
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

        public void SetTopStatCount(bool[] considerStatHighlight)
        {
            if (Species == null
                || flags.HasFlag(CreatureFlags.Placeholder))
                return;

            if (topBreedingStats == null) topBreedingStats = new bool[Values.STATS_COUNT];

            short c = 0, cBP = 0;
            onlyTopConsideredStats = true;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (topBreedingStats[s])
                {
                    if (s != (int)StatNames.Torpidity)
                        cBP++;
                    if (considerStatHighlight[s])
                        c++;
                }
                else if (onlyTopConsideredStats && considerStatHighlight[s] && Species.UsesStat(s) && Species.stats[s].IncPerWildLevel > 0)
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
            if (Species != null)
            {
                InitializeArrays();
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    valuesBreeding[s] = StatValueCalculation.CalculateValue(Species, s, levelsWild[s], 0, true, 1, 0);
                    valuesDom[s] = StatValueCalculation.CalculateValue(Species, s, levelsWild[s], levelsDom[s], true, tamingEff, imprintingBonus);
                }
            }
            CalculateLevelFound(levelStep);
        }

        /// <summary>
        /// Recalculates the new occured mutations.
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
        private void StartTimer()
        {
            if (growingPaused)
            {
                growingPaused = false;
                growingUntil = DateTime.Now.Add(growingLeft);
            }
        }

        /// <summary>
        /// Pauses the timer for maturation.
        /// </summary>
        private void PauseTimer()
        {
            if (!growingPaused)
            {
                growingPaused = true;
                growingLeft = growingUntil?.Subtract(DateTime.Now) ?? new TimeSpan();
                if (growingLeft.TotalHours < 0) growingLeft = new TimeSpan();
            }
        }

        /// <summary>
        /// Starts or stops the timer for maturation.
        /// </summary>
        public void StartStopMatureTimer(bool start)
        {
            if (start)
                StartTimer();
            else PauseTimer();
        }

        // XmlSerializer does not support TimeSpan, so use this property for serialization instead.
        [System.ComponentModel.Browsable(false)]
        [JsonProperty("growingLeft")]
        public string GrowingLeftString
        {
            get => System.Xml.XmlConvert.ToString(growingLeft);
            set
            {
                growingLeft = string.IsNullOrEmpty(value) ?
                    TimeSpan.Zero : System.Xml.XmlConvert.ToTimeSpan(value);
            }
        }

        [OnDeserialized]
        private void Initialize(StreamingContext ct)
        {
            InitializeArrays();
        }

        private void InitializeArrays()
        {
            if (valuesBreeding == null) valuesBreeding = new double[Values.STATS_COUNT];
            if (valuesDom == null) valuesDom = new double[Values.STATS_COUNT];
            if (topBreedingStats == null) topBreedingStats = new bool[Values.STATS_COUNT];
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
        internal static int CalculatePreTameWildLevel(int postTameLevel, double tamingEffectiveness) => (int)Math.Ceiling(postTameLevel / (1 + tamingEffectiveness / 2));
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
        /// <summary>
        /// If applied to the flags with &, the status is removed.
        /// </summary>
        StatusMask = Mutated | Neutered | Placeholder | Female | Male
    }
}