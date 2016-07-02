using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public double affinityNeeded0, affinityIncreasePL, torporDepletionPS0, foodConsumptionBase, foodConsumptionMult, wakeAffinityMult, wakeFoodDeplMult;

        [DataMember]
        private double[] tamingValues
        {
            get { return new double[] { affinityNeeded0, affinityIncreasePL, torporDepletionPS0, foodConsumptionBase, foodConsumptionMult, wakeAffinityMult, wakeFoodDeplMult }; }
            set
            {
                if (value.Length == 7)
                {
                    affinityNeeded0 = value[0];
                    affinityIncreasePL = value[1];
                    torporDepletionPS0 = value[2];
                    foodConsumptionBase = value[3];
                    foodConsumptionMult = value[4];
                    wakeAffinityMult = value[5];
                    wakeFoodDeplMult = value[6];
                }
            }
        }
    }
}
