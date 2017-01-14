using System;
using System.Linq;
using System.Xml.Serialization;

namespace ARKBreedingStats
{
    [Serializable()]
    public class Creature : IEquatable<Creature>
    {
        public string species;
        public string name;
        public Sex gender;
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
        public string owner;
        public string note; // user defined note about that creature
        public Guid guid;
        public bool isBred;
        public Guid fatherGuid;
        public Guid motherGuid;
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
        public bool neutered = false;

        public Creature()
        {
        }

        public Creature(string species, string name, string owner, Sex sex, int[] levelsWild, int[] levelsDom = null, double tamingEff = 0, bool isBred = false, double imprinting = 0)
        {
            this.species = species;
            this.name = name;
            this.owner = owner;
            this.gender = sex;
            this.levelsWild = levelsWild;
            this.levelsDom = (levelsDom == null ? new int[] { 0, 0, 0, 0, 0, 0, 0, 0 } : levelsDom);
            if (isBred)
                this.tamingEff = 1;
            else
                this.tamingEff = tamingEff;
            this.isBred = isBred;
            imprintingBonus = imprinting;
            this.status = CreatureStatus.Available;
            calculateLevelFound();
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

        public void calculateLevelFound()
        {
            levelFound = 0;
            if (!isBred && tamingEff >= 0)
                levelFound = (int)Math.Ceiling(levelHatched / (1 + tamingEff / 2)); // TODO due to rounding of ingame TE, it can differ. Round to next multiple of 4?
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
        Alive = Available // backwards-compatibility
    };
}