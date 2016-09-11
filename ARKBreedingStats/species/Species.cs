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
        public BreedingData breeding;

        /// <summary>
        /// creates properties that are not created during deserialization. They are set later with the raw-values with the multipliers applied.
        /// </summary>
        public void initialize()
        {
            stats = new List<CreatureStat>();
            for (int s = 0; s < 8; s++)
            {
                stats.Add(new CreatureStat((StatName)s));
            }
        }
    }
}
