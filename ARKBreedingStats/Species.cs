using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ARKBreedingStats
{
    [DataContract]
    public class Species
    {
        [DataMember]
        public string name;
        [DataMember]
        public List<CreatureStat> statsRaw; // without multipliers
        public List<CreatureStat> stats;
        [DataMember]
        public List<ColorRegion> colors; // each creature has up to 6 colorregions
        [DataMember]
        public TamingData tamingRaw = new TamingData();
        public TamingData taming = new TamingData();
        [DataMember]
        public int[] breedingTimesRaw = new int[3];
        public int[] breedingTimes = new int[3];

        public Species(string name)
        {
            this.name = name;
            stats = new List<CreatureStat>();
            statsRaw = new List<CreatureStat>();
            colors = new List<ColorRegion>();
            for (int s = 0; s < 8; s++)
            {
                stats.Add(new CreatureStat((StatName)s));
                statsRaw.Add(new CreatureStat((StatName)s));
                if (s < 6)
                    colors.Add(new ColorRegion());
            }
        }
    }
}
