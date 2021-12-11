using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace ARKBreedingStats.species
{
    [JsonObject]
    public class TamingFood
    {
        /// <summary>
        /// Amount of affinity raise if one piece of this food is eaten.
        /// </summary>
        [JsonProperty("a")]
        public double affinity;
        /// <summary>
        /// Amount of food one of this food gives.
        /// </summary>
        [JsonProperty("f")]
        public double foodValue;
        /// <summary>
        /// When taming, some foods can only be feed in higher quantities, this indicates that amount.
        /// </summary>
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