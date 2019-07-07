using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ARKBreedingStats
{
    [Serializable]
    public class Creature : IEquatable<Creature>
    {
        public string speciesBlueprint;
        [XmlIgnore]
        private Species _species;
        //[ObsoleteAttribute("Use speciesName for the display name. Use speciesBP for identifying the species")] // commented out so that it will be still loaded from the xml-file
        public string species;
        public string name = string.Empty;
        public Sex sex;
        public CreatureStatus status;
        // order of the stats is Health, Stamina, Oxygen, Food, Weight, MeleeDamage, Speed, Torpor
        public int[] levelsWild;
        public int[] levelsDom;
        public double tamingEff;
        public double imprintingBonus;
        [XmlIgnore]
        public double[] valuesBreeding = new double[12];
        [XmlIgnore]
        public double[] valuesDom = new double[12];
        [XmlIgnore]
        public bool[] topBreedingStats = new bool[12]; // indexes of stats that are top for that species in the creaturecollection
        [XmlIgnore]
        public short topStatsCount;
        /// <summary>
        /// topstatcount with all stats (regardless of considerStatHighlight[]) and without torpor (for breedingplanner)
        /// </summary>
        [XmlIgnore]
        public short topStatsCountBP;
        [XmlIgnore]
        public bool topBreedingCreature; // true if it has some topBreedingStats and if it's male, no other male has more topBreedingStats
        [XmlIgnore]
        public short topness; // permille of mean of wildlevels compared to toplevels
        public string owner = "";
        public string imprinterName = ""; // todo implement in creatureInfoInbox
        public string tribe = "";
        public string server = "";
        public string note; // user defined note about that creature
        public Guid guid; // the id used in ASB for parent-linking. The user cannot change it
        public long ArkId; // the creature's id in ARK. This id is shown to the user ingame, but it's not always unique. (It's build from two int, which are concatenated as strings).
        public bool ArkIdImported; // if true it's assumed the ArkId is correct (ingame visualization can be wrong). This field should only be true if the ArkId was imported.
        public bool isBred;
        public Guid fatherGuid;
        public Guid motherGuid;
        [XmlIgnore]
        public long motherArkId; // only set if the id is imported
        [XmlIgnore]
        public long fatherArkId; // only set if the id is imported
        [XmlIgnore]
        public string fatherName; // only used during import to create placeholder ancestors
        [XmlIgnore]
        public string motherName; // only used during import to create placeholder ancestors
        [XmlIgnore]
        private Creature father; // only the parent-guid is saved in the xml, not the parent-object
        [XmlIgnore]
        private Creature mother; // only the parent-guid is saved in the xml, not the parent-object
        [XmlIgnore]
        public int levelFound;
        public int generation; // number of generations from the oldest wild creature
        public int[] colors = new int[6] { 0, 0, 0, 0, 0, 0 }; // id of colors
        public DateTime growingUntil = new DateTime(0);
        [XmlIgnore]
        public TimeSpan growingLeft = new TimeSpan();
        public bool growingPaused;
        public DateTime cooldownUntil = new DateTime(0);
        public DateTime domesticatedAt = new DateTime(0);
        public DateTime addedToLibrary = new DateTime(0);
        public bool neutered = false;
        public int mutationsMaternal;
        public int mutationsPaternal;
        public List<string> tags = new List<string>();
        public bool IsPlaceholder; // if a creature has unknown parents, they are placeholders until they are imported. placeholders are not shown in the library
        [XmlIgnore]
        private static int statsCount = 12;

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
            this.levelsDom = levelsDom ?? new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            if (isBred)
                this.tamingEff = 1;
            else
                this.tamingEff = tamingEff;
            this.isBred = isBred;
            imprintingBonus = imprinting;
            status = CreatureStatus.Available;
            calculateLevelFound(levelStep);
        }

        [XmlIgnore]
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
            IsPlaceholder = true;
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

        public void calculateLevelFound(int? levelStep)
        {
            levelFound = 0;
            if (!isBred && tamingEff >= 0)
            {
                if (levelStep.HasValue)
                    levelFound = (int)Math.Round(levelHatched / (1 + tamingEff / 2) / levelStep.Value) * levelStep.Value;
                else
                    levelFound = (int)Math.Ceiling(Math.Round(levelHatched / (1 + tamingEff / 2), 6));
            }
        }

        [XmlIgnore]
        public int levelHatched => levelsWild[(int)StatNames.Torpidity] + 1;

        [XmlIgnore]
        public int level => levelHatched + levelsDom.Sum();

        public void recalculateAncestorGenerations()
        {
            generation = ancestorGenerations();
        }

        /// <summary>
        /// Returns the number of generations to the oldest known ancestor
        /// </summary>
        /// <returns></returns>
        private int ancestorGenerations(int g = 0)
        {
            // to detect loop (if a creature is falsely listed as its own ancestor)
            if (g > 99)
                return 0;

            int mgen = 0, fgen = 0;
            if (mother != null)
                mgen = mother.ancestorGenerations(g + 1) + 1;
            if (father != null)
                fgen = father.ancestorGenerations(g + 1) + 1;
            if (isBred && mgen == 0 && fgen == 0)
                return 1;
            return mgen > fgen ? mgen : fgen;
        }

        [XmlIgnore]
        public Creature Mother
        {
            get => mother;
            set
            {
                mother = value;
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
                fatherGuid = father?.guid ?? Guid.Empty;
            }
        }

        public void setTopStatCount(bool[] considerStatHighlight)
        {
            short c = 0, cBP = 0;
            for (int s = 0; s < statsCount; s++)
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
        public void recalculateCreatureValues(int? levelStep)
        {
            if (Species != null)
            {
                for (int s = 0; s < statsCount; s++)
                {
                    valuesBreeding[s] = Stats.calculateValue(Species, s, levelsWild[s], 0, true, 1, 0);
                    valuesDom[s] = Stats.calculateValue(Species, s, levelsWild[s], levelsDom[s], true, tamingEff, imprintingBonus);
                }
            }
            calculateLevelFound(levelStep);
        }

        [XmlIgnore]
        public int Mutations => mutationsMaternal + mutationsPaternal;

        public override string ToString()
        {
            return name + " (" + _species.name + ")";
        }

        private void startTimer()
        {
            if (growingPaused)
            {
                growingPaused = false;
                growingUntil = DateTime.Now.Add(growingLeft);
            }
        }

        private void pauseTimer()
        {
            if (!growingPaused)
            {
                growingPaused = true;
                growingLeft = growingUntil.Subtract(DateTime.Now);
                if (growingLeft.TotalHours < 0) growingLeft = new TimeSpan();
            }
        }

        public void startStopMatureTimer(bool start)
        {
            if (start)
                startTimer();
            else pauseTimer();
        }

        // XmlSerializer does not support TimeSpan, so use this property for serialization instead.
        [System.ComponentModel.Browsable(false)]
        [XmlElement(DataType = "duration", ElementName = "growingLeft")]
        public string growingLeftString
        {
            get
            {
                return System.Xml.XmlConvert.ToString(growingLeft);
            }
            set
            {
                growingLeft = string.IsNullOrEmpty(value) ?
                    TimeSpan.Zero : System.Xml.XmlConvert.ToTimeSpan(value);
            }
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
}