using Newtonsoft.Json;
using System.Collections.Generic;

namespace ARKBreedingStats.species
{
    [JsonObject]
    public class TamingData
    {
        public bool violent;
        public bool nonViolent;
        public double tamingIneffectiveness;
        public string[] eats;
        public string favoriteKibble;
        public Dictionary<string, TamingFood> specialFoodValues;
        public double affinityNeeded0;
        public double affinityIncreasePL;
        public double torporDepletionPS0;
        public double foodConsumptionBase;
        public double foodConsumptionMult;
        public double wakeAffinityMult;
        public double wakeFoodDeplMult;
    }
}
