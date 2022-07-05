using System;
using System.Xml.Serialization;

namespace ARKBreedingStats.oldLibraryFormat
{
    /// <summary>
    /// Outdated, only used to convert.
    /// This class is used to store creature-values of creatures that couldn't be extracted, to store their values temporarily until the issue is solved.
    /// </summary>
    [Serializable]
    [XmlType(TypeName = "CreatureValues")]
    public class CreatureValuesOld
    {
        public string species; // the speciesName
        public Guid guid;
        public long ARKID;
        public string name;
        public Library.Sex sex;
        public double[] statValues = new double[Stats.StatsCount];
        public int[] levelsWild = new int[Stats.StatsCount];
        public int[] levelsDom = new int[Stats.StatsCount];
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
        public DateTime growingUntil = new DateTime(0);
        public DateTime cooldownUntil = new DateTime(0);
        public DateTime domesticatedAt = new DateTime(0);
        public bool neutered = false;
        public int mutationCounter, mutationCounterMother, mutationCounterFather;
        public int[] colorIDs = new int[6];

        public CreatureValuesOld() { }
    }
}
