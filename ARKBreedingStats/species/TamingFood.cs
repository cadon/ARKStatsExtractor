using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace ARKBreedingStats.species
{
    [JsonObject]
    public class TamingFood
    {
        [JsonProperty("a")]
        public double affinity;
        [JsonProperty("f")]
        public double foodValue;
        [JsonProperty("q")]
        public int quantity;

        [OnDeserializing]
        private void SetDefaultValues(StreamingContext context)
        {
            // set default value if not given
            quantity = 1;
        }
    }
}