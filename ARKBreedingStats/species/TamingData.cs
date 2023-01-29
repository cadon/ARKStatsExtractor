using Newtonsoft.Json;
using System.Collections.Generic;

namespace ARKBreedingStats.species
{
    /// <summary>
    /// Info about taming a creature
    /// </summary>
    [JsonObject]
    public class TamingData
    {
        /// <summary>
        /// If true, a creature of this species can be knocked out to tame it.
        /// </summary>
        public bool violent;
        /// <summary>
        /// If true, a creature of this species can be tamed while awake.
        /// </summary>
        public bool nonViolent;
        public double tamingIneffectiveness;
        /// <summary>
        /// Names of food the species eats during taming.
        /// </summary>
        public string[] eats;
        /// <summary>
        /// If a food has non default values for this species, it's defined here.
        /// </summary>
        public Dictionary<string, TamingFood> specialFoodValues;
        /// <summary>
        /// Food a species eats after being tamed, additionally to the taming food in eats.
        /// </summary>
        public string[] eatsAlsoPostTame;
        /// <summary>
        /// Base value of needed affinity.
        /// </summary>
        public double affinityNeeded0;
        /// <summary>
        /// Increase of needed affinity per level.
        /// </summary>
        public double affinityIncreasePL;
        public double torporDepletionPS0;
        public double foodConsumptionBase;
        /// <summary>
        /// Multiplier during taming
        /// </summary>
        public double foodConsumptionMult;
        /// <summary>
        /// Multiplier when tamed.
        /// Multiply with foodConsumptionBase when maturation is at 0 %, when at 100 % maturation multiply with const 0.000155, in between interpolate linearly.
        /// This value is the product of the ue properties BabyDinoConsumingFoodRateMultiplier and ExtraBabyDinoConsumingFoodRateMultiplier.
        /// </summary>
        public double babyFoodConsumptionMult;
        /// <summary>
        /// Extra multiplier for food consumption once a creature is mature. Only few species use this, e.g. Giganotosaurus, Carcharodontosaurus, Titanosaur and Titans.
        /// </summary>
        public double adultFoodConsumptionMult = 1;
        /// <summary>
        /// Factor for affinity if tamed awake.
        /// </summary>
        public double wakeAffinityMult;
        /// <summary>
        /// Factor of food depletion if tamed awake.
        /// </summary>
        public double wakeFoodDeplMult;
    }
}
