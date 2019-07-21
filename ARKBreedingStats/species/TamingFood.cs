using System.Runtime.Serialization;

namespace ARKBreedingStats.species
{
    [DataContract]
    public class TamingFood
    {
        [DataMember(Name = "a")]
        public double affinity;
        [DataMember(Name = "f")]
        public double foodValue;
        [DataMember(Name = "q")]
        public int quantity;

        [OnDeserializing]
        private void SetDefaultValues(StreamingContext context)
        {
            // set default value if not given
            quantity = 1;
        }
    }
}