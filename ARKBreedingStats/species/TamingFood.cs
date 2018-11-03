using System.Runtime.Serialization;

namespace ARKBreedingStats.species
{
    [DataContract]
    public class TamingFood
    {
        public double affinity, foodValue;
        public int quantity;
        [DataMember]
        public double[] d
        {
            get => new[] { foodValue, affinity };
            set
            {
                if (value.Length > 1)
                {
                    foodValue = value[0];
                    affinity = value[1];
                }
                quantity = value.Length > 2 ? (int)value[2] : 1;
            }
        }
    }
}