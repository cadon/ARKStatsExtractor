using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ARKBreedingStats.Library
{
    [DataContract]
    public class Creature : IEquatable<Creature>
    {
        [DataMember]
        public string speciesBlueprint;
        [IgnoreDataMember]
        private Species _species;
        [DataMember]
        public string name = string.Empty;
        [DataMember]
        public Sex sex;
        [DataMember]
        public CreatureStatus status;
        [DataMember]
        public CreatureFlags flags;
        [DataMember]
        public int[] levelsWild = new int[Values.STATS_COUNT];
        [DataMember]
        public int[] levelsDom = new int[Values.STATS_COUNT];
        [DataMember]
        public double tamingEff;
        [DataMember]
        public double imprintingBonus;
        [IgnoreDataMember]
        public double[] valuesBreeding = new double[Values.STATS_COUNT];
        [IgnoreDataMember]
        public double[] valuesDom = new double[Values.STATS_COUNT];
        /// <summary>
        /// Indices of stats that are top for that species in the creaturecollection
        /// </summary>
        [IgnoreDataMember]
        public bool[] topBreedingStats = new bool[Values.STATS_COUNT];
        [IgnoreDataMember]
        public short topStatsCount;
        /// <summary>
        /// topstatcount with all stats (regardless of considerStatHighlight[]) and without torpor (for breedingplanner)
        /// </summary>
        [IgnoreDataMember]
        public short topStatsCountBP;
        [IgnoreDataMember]
        public bool topBreedingCreature; // true if it has some topBreedingStats and if it's male, no other male has more topBreedingStats
        [IgnoreDataMember]
        public short topness; // permille of mean of wildlevels compared to toplevels
        [DataMember]
        public string owner = "";
        [DataMember]
        public string imprinterName = ""; // todo implement in creatureInfoInbox
        [DataMember]
        public string tribe = "";
        [DataMember]
        public string server = "";
        [DataMember]
        public string note; // user defined note about that creature
        [DataMember]
        public Guid guid; // the id used in ASB for parent-linking. The user cannot change it
        [DataMember]
        public long ArkId; // the creature's id in ARK. This id is shown to the user ingame, but it's not always unique. (It's build from two int, which are concatenated as strings).
        [DataMember]
        public bool ArkIdImported; // if true it's assumed the ArkId is correct (ingame visualization can be wrong). This field should only be true if the ArkId was imported.
        [DataMember]
        public bool isBred;
        [DataMember]
        public Guid fatherGuid;
        [DataMember]
        public Guid motherGuid;
        [IgnoreDataMember]
        public long motherArkId; // only set if the id is imported
        [IgnoreDataMember]
        public long fatherArkId; // only set if the id is imported
        [IgnoreDataMember]
        public string fatherName; // only used during import to create placeholder ancestors
        [IgnoreDataMember]
        public string motherName; // only used during import to create placeholder ancestors
        [IgnoreDataMember]
        private Creature father; // only the parent-guid is saved in the file, not the parent-object
        [IgnoreDataMember]
        private Creature mother; // only the parent-guid is saved in the file, not the parent-object
        [IgnoreDataMember]
        public int levelFound;
        [DataMember]
        public int generation; // number of generations from the oldest wild creature
        [DataMember]
        public int[] colors = new int[6]; // id of colors
        [DataMember]
        public DateTime? growingUntil;
        [IgnoreDataMember]
        public TimeSpan growingLeft;
        [DataMember]
        public bool growingPaused;
        [DataMember]
        public DateTime? cooldownUntil;
        [DataMember]
        public DateTime? domesticatedAt;
        [DataMember]
        public DateTime? addedToLibrary;
        [DataMember]
        public int mutationsMaternal;
        [DataMember]
        public int mutationsPaternal;
        /// <summary>
        /// Number of new occured maternal mutations
        /// </summary>
        [DataMember(Name = "mutMatNew")]
        public int mutationsMaternalNew;
        /// <summary>
        /// Number of new occured paternal mutations
        /// </summary>
        [DataMember(Name = "mutPatNew")]
        public int mutationsPaternalNew;
        [DataMember]
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
            status = CreatureStatus.Available;
            CalculateLevelFound(levelStep);
        }

        [IgnoreDataMember]
        public Species Species
        {
            set
            {
                _species = value;
                speciesBlueprint = value?.blueprintPath ?? string.Empty;
            }
            get { return _species; }
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

        public bool Equals(Creature other)
        {
            return other.guid == guid;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return obj is Creature creatureObj && Equals(creatureObj);
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

        [IgnoreDataMember]
        public int LevelHatched => levelsWild[(int)StatNames.Torpidity] + 1;

        [IgnoreDataMember]
        public int Level => LevelHatched + levelsDom.Sum();

        public void RecalculateAncestorGenerations()
        {
            generation = AncestorGenerations();
        }

        /// <summary>
        /// Returns the number of generations to the oldest known ancestor
        /// </summary>
        /// <returns></returns>
        private int AncestorGenerations(int g = 0)
        {
            // to detect loop (if a creature is falsely listed as its own ancestor)
            if (g > 99)
                return 0;

            int mgen = 0, fgen = 0;
            if (mother != null)
                mgen = mother.AncestorGenerations(g + 1) + 1;
            if (father != null)
                fgen = father.AncestorGenerations(g + 1) + 1;
            if (isBred && mgen == 0 && fgen == 0)
                return 1;
            return mgen > fgen ? mgen : fgen;
        }

        [IgnoreDataMember]
        public Creature Mother
        {
            get => mother;
            set
            {
                mother = value;
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
                fatherGuid = father?.guid ?? Guid.Empty;
            }
        }

        public void SetTopStatCount(bool[] considerStatHighlight)
        {
            if (topBreedingStats == null) return;

            short c = 0, cBP = 0;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (topBreedingStats[s])
                {
                    if (s != (int)StatNames.Torpidity)
                        cBP++;
                    if (considerStatHighlight[s])
                        c++;
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

        [IgnoreDataMember]
        public int Mutations => mutationsMaternal + mutationsPaternal;

        public override string ToString()
        {
            return name + " (" + _species.name + ")";
        }

        private void StartTimer()
        {
            if (growingPaused)
            {
                growingPaused = false;
                growingUntil = DateTime.Now.Add(growingLeft);
            }
        }

        private void PauseTimer()
        {
            if (!growingPaused)
            {
                growingPaused = true;
                growingLeft = growingUntil?.Subtract(DateTime.Now) ?? new TimeSpan();
                if (growingLeft.TotalHours < 0) growingLeft = new TimeSpan();
            }
        }

        public void StartStopMatureTimer(bool start)
        {
            if (start)
                StartTimer();
            else PauseTimer();
        }

        // XmlSerializer does not support TimeSpan, so use this property for serialization instead.
        [System.ComponentModel.Browsable(false)]
        [DataMember(Name = "growingLeft")]
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
        Unavailable = 2,
        Dead = 4,
        Obelisk = 8,
        Cryopod = 16,
        // Deleted = 32, // not used anymore
        Mutated = 64,
        Neutered = 128,
        /// <summary>
        /// If a creature has unknown parents, they are placeholders until they are imported. placeholders are not shown in the library
        /// </summary>
        Placeholder = 256
    }
}