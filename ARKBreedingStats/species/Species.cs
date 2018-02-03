using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ARKBreedingStats
{
    [DataContract]
    public class Species
    {
        [DataMember]
        public string name;
        [DataMember]
        public double?[][] statsRaw; // without multipliers
        public List<CreatureStat> stats;
        [DataMember]
        public float? TamedBaseHealthMultiplier;
        [DataMember]
        public bool? NoImprintingForSpeed;
        [DataMember]
        public List<ColorRegion> colors; // each creature has up to 6 colorregions
        [DataMember]
        public TamingData taming;
        [DataMember]
        public BreedingData breeding;
        [DataMember]
        public bool doesNotUseOxygen;
        [DataMember]
        public Dictionary<double, string> boneDamageAdjusters;
        [DataMember]
        public List<string> immobilizedBy;

        /// <summary>
        /// creates properties that are not created during deserialization. They are set later with the raw-values with the multipliers applied.
        /// </summary>
        public void initialize()
        {
            stats = new List<CreatureStat>();
            double?[][] completeRaws = new double?[8][];
            for (int s = 0; s < 8; s++)
            {
                stats.Add(new CreatureStat((StatName)s));
                completeRaws[s] = new double?[] { 0, 0, 0, 0, 0 };
                if (statsRaw.Length > s && statsRaw[s] != null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (statsRaw[s].Length > i)
                            completeRaws[s][i] = statsRaw[s][i] != null ? statsRaw[s][i] : 0;
                    }
                }
            }
            statsRaw = completeRaws;
            if (TamedBaseHealthMultiplier == null)
                TamedBaseHealthMultiplier = 1;
            if (NoImprintingForSpeed == null)
                NoImprintingForSpeed = false;

            if (colors == null)
                colors = new List<ColorRegion>();
            for (int c = 0; c < 6; c++)
            {
                if (colors.Count <= c)
                {
                    colors.Add(new ColorRegion());
                    colors[c].colorIds = new List<int>();
                }
            }
        }
    }
}
