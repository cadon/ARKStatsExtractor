using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ARKBreedingStats
{
    [DataContract]
    public class TamingFood
    {
        public double affinity = 0, foodValue = 0;
        [DataMember]
        public double[] d
        {
            get { return new double[] { foodValue, affinity }; }
            set { if (value.Length == 2) { foodValue = value[0]; affinity = value[1]; } }
        }

        public TamingFood(double[] d)
        {
            this.d = d;
        }
    }
}