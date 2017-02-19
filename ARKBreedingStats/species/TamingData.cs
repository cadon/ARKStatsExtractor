using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ARKBreedingStats
{
    [DataContract]
    public class TamingData
    {
        [DataMember]
        public bool violent;
        [DataMember]
        public bool nonViolent;
        [DataMember]
        public double tamingIneffectiveness;
        [DataMember]
        public List<string> eats = new List<string>();
        [DataMember]
        public string favoriteKibble;
        [DataMember]
        public Dictionary<string, TamingFood> specialFoodValues;
        [DataMember]
        public double affinityNeeded0, affinityIncreasePL, torporDepletionPS0, foodConsumptionBase, foodConsumptionMult, wakeAffinityMult, wakeFoodDeplMult;
    }
}
