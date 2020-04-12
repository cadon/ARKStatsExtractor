using Newtonsoft.Json;

namespace ARKBreedingStats.species
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BreedingData
    {
        [JsonProperty]
        public double gestationTime;
        /// <summary>
        /// GestationTime with the according multipliers applied.
        /// </summary>
        public double gestationTimeAdjusted;
        [JsonProperty]
        public double incubationTime;
        public double incubationTimeAdjusted;
        [JsonProperty]
        public double maturationTime;
        public double maturationTimeAdjusted;
        [JsonProperty]
        public double matingTime;
        public double matingTimeAdjusted;
        [JsonProperty]
        public double matingCooldownMin;
        public double matingCooldownMinAdjusted;
        [JsonProperty]
        public double matingCooldownMax;
        public double matingCooldownMaxAdjusted;
        [JsonProperty]
        public double eggTempMin;
        [JsonProperty]
        public double eggTempMax;

    }
}
