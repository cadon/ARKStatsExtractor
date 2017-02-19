using System.Runtime.Serialization;

namespace ARKBreedingStats
{
    [DataContract]
    public class TamingFood
    {
        public double affinity, foodValue;
        public int quantity;
        [DataMember]
        public double[] d
        {
            get
            {
                return new double[] { foodValue, affinity };
            }
            set
            {
                if (value.Length > 1)
                {
                    foodValue = value[0];
                    affinity = value[1];
                }
                if (value.Length > 2)
                    quantity = (int)value[2];
                else quantity = 1;
            }
        }
    }
}