using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ARKBreedingStats
{
    [Serializable()]
    public class Creature : IEquatable<Creature>
    {
        public string species;
        public string name;
        public Sex gender; // remove on 07/2018
        public Sex sex;
        public CreatureStatus status;
        // order of the stats is Health, Stamina, Oxygen, Food, Weight, MeleeDamage, Speed, Torpor
        public int[] levelsWild;
        public int[] levelsDom;
        public double tamingEff;
        public double imprintingBonus;
        [XmlIgnore]
        public double[] valuesBreeding = new double[8];
        [XmlIgnore]
        public double[] valuesDom = new double[8];
        [XmlIgnore]
        public bool[] topBreedingStats = new bool[8]; // indexes of stats that are top for that species in the creaturecollection
        [XmlIgnore]
        public Int16 topStatsCount;
        [XmlIgnore]
        public Int16 topStatsCountBP; // topstatcount with all stats (regardless of considerStatHighlight[]) and without torpor (for breedingplanner)
        [XmlIgnore]
        public bool topBreedingCreature; // true if it has some topBreedingStats and if it's male, no other male has more topBreedingStats
        [XmlIgnore]
        public Int16 topness; // permille of mean of wildlevels compared to toplevels
        public string owner = "";
        public string imprinterName = ""; // todo implement in creatureInfoInbox
        public string tribe = "";
        public string server = "";
        public string note; // user defined note about that creature
        public Guid guid;
        public bool isBred;
        public Guid fatherGuid;
        public Guid motherGuid;
        [XmlIgnore]
        public string fatherName; // only used during import for missing ancestors
        [XmlIgnore]
        public string motherName; // only used during import for missing ancestors
        [XmlIgnore]
        private Creature father;
        [XmlIgnore]
        private Creature mother;
        [XmlIgnore]
        public int levelFound;
        public int generation; // number of generations from the oldest wild creature
        public int[] colors = new int[6] { 0, 0, 0, 0, 0, 0 }; // id of colors
        public DateTime growingUntil = new DateTime(0);
        public DateTime cooldownUntil = new DateTime(0);
        public DateTime domesticatedAt = new DateTime(0);
        public DateTime addedToLibrary = new DateTime(0);
        public bool neutered = false;
        public int mutationCounter; // remove this field on 07-2018
        public int mutationsMaternal;
        public int mutationsPaternal;
        public List<string> tags = new List<string>();

        public Creature()
        {
            levelsWild = new int[] { -1, -1, -1, -1, -1, -1, -1, -1 }; // unknown wild levels
        }

        public Creature(string species, string name, string owner, string tribe, Sex sex, int[] levelsWild, int[] levelsDom = null, double tamingEff = 0, bool isBred = false, double imprinting = 0, int? levelStep = null)
        {
            this.species = species;
            this.name = name;
            this.owner = owner;
            this.tribe = tribe;
            this.sex = sex;
            this.levelsWild = levelsWild;
            this.levelsDom = (levelsDom == null ? new int[] { 0, 0, 0, 0, 0, 0, 0, 0 } : levelsDom);
            if (isBred)
                this.tamingEff = 1;
            else
                this.tamingEff = tamingEff;
            this.isBred = isBred;
            imprintingBonus = imprinting;
            this.status = CreatureStatus.Available;
            calculateLevelFound(levelStep);
        }

        public bool Equals(Creature other)
        {
            if (other.guid == guid)
                return true;
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Creature creatureObj = obj as Creature;
            if (creatureObj == null)
                return false;
            else
                return Equals(creatureObj);
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
        public int levelHatched { get { return levelsWild[7] + 1; } }
        [XmlIgnore]
        public int level { get { return levelHatched + levelsDom.Sum(); } }

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
            if (mgen > fgen)
                return mgen;
            else
                return fgen;
        }

        [XmlIgnore]
        public Creature Mother
        {
            set
            {
                mother = value;
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
                fatherGuid = (father != null ? father.guid : Guid.Empty);
            }
            get { return father; }
        }

        public void setTopStatCount(bool[] considerStatHighlight)
        {
            Int16 c = 0, cBP = 0;
            for (int s = 0; s < 8; s++)
            {
                if (topBreedingStats[s])
                {
                    if (s < 7)
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
            int speciesIndex = Values.V.speciesNames.IndexOf(species);
            if (speciesIndex >= 0)
            {
                for (int s = 0; s < 8; s++)
                {
                    valuesBreeding[s] = Stats.calculateValue(speciesIndex, s, levelsWild[s], 0, true, 1, 0);
                    valuesDom[s] = Stats.calculateValue(speciesIndex, s, levelsWild[s], levelsDom[s], true, tamingEff, imprintingBonus);
                }
            }
            calculateLevelFound(levelStep);
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
        Alive = Available // backwards-compatibility
    };
}