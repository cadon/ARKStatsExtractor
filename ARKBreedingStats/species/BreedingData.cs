using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ARKBreedingStats
{
    [DataContract]
    public class BreedingData
    {
        [DataMember]
        public double pregnancyTime;
        [DataMember]
        public double incubationTime;
        [DataMember]
        public double maturationTime;
        [DataMember]
        public double matingCooldownMin;
        [DataMember]
        public double matingCooldownMax;
        [DataMember]
        public double eggTempMin;
        [DataMember]
        public double eggTempMax;
        public double pregnancyTimeAdjusted;
        public double incubationTimeAdjusted;
        public double maturationTimeAdjusted;
        public double matingCooldownMinAdjusted;
        public double matingCooldownMaxAdjusted;
    }
}
