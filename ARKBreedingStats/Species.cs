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
        public TamingData taming;
        [DataMember]
        public int[] breedingTimesRaw;
        public int[] breedingTimes;

        /// <summary>
        /// creates properties that are not created during deserialization
        /// </summary>
        public void initialize()
        {
            stats = new List<CreatureStat>();
            breedingTimes = new int[3];
            for (int s = 0; s < 8; s++)
            {
                stats.Add(new CreatureStat((StatName)s));
            }
        }
    }
}
