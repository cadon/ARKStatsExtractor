using Newtonsoft.Json;

namespace ArkSmartBreeding.Models.Species
{
    [JsonObject]
    public class SpeciesStat
    {
        public double BaseValue;
        public double IncPerWildLevel;
        public double IncPerMutatedLevel;
        public double IncPerTamedLevel;
        public double AddWhenTamed;
        public double MultAffinity;
    }
}
