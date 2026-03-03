using Newtonsoft.Json;

namespace ARKBreedingStats.Models
{
    /// <summary>
    /// Static breeding data for a species (from values JSON).
    /// Does not include adjusted times with server multipliers - those are calculated at runtime.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BreedingData
    {
        [JsonProperty]
        public double gestationTime { get; set; }
        
        /// <summary>
        /// GestationTime with the according multipliers applied.
        /// </summary>
        public double gestationTimeAdjusted { get; set; }
        
        [JsonProperty]
        public double incubationTime { get; set; }
        
        public double incubationTimeAdjusted { get; set; }
        
        [JsonProperty]
        public double maturationTime { get; set; }
        
        public double maturationTimeAdjusted { get; set; }
        
        [JsonProperty]
        public double matingTime { get; set; }
        
        public double matingTimeAdjusted { get; set; }
        
        [JsonProperty]
        public double matingCooldownMin { get; set; }
        
        public double matingCooldownMinAdjusted { get; set; }
        
        [JsonProperty]
        public double matingCooldownMax { get; set; }
        
        public double matingCooldownMaxAdjusted { get; set; }
        
        [JsonProperty]
        public double eggTempMin { get; set; }
        
        [JsonProperty]
        public double eggTempMax { get; set; }
    }
}
