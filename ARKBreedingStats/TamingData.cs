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
        public double affinityNeeded0, affinityIncreasePL, torpor1, torporIncrease, foodConsumptionBase, foodConsumptionMult, torporDepletionPS0, wakeAffinityMult, wakeFoodDeplMult;

        [DataMember]
        private double[] tamingValues
        {
            get { return new double[] { affinityNeeded0, affinityIncreasePL, torpor1, torporIncrease, torporDepletionPS0, foodConsumptionBase, foodConsumptionMult, wakeAffinityMult, wakeFoodDeplMult }; }
            set
            {
                if (value.Length == 9)
                {
                    affinityNeeded0 = value[0];
                    affinityIncreasePL = value[1];
                    torpor1 = value[2];
                    torporIncrease = value[3];
                    torporDepletionPS0 = value[4];
                    foodConsumptionBase = value[5];
                    foodConsumptionMult = value[6];
                    wakeAffinityMult = value[7];
                    wakeFoodDeplMult = value[8];
                }
            }
        }
    }
}
