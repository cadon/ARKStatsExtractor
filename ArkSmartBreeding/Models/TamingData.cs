using Newtonsoft.Json;
using System.Collections.Generic;

namespace ARKBreedingStats.Models
{
    /// <summary>
    /// Static taming data for a species (from values JSON).
    /// Does not include server multipliers - those are applied at runtime.
    /// </summary>
    [JsonObject]
    public class TamingData
    {
        /// <summary>
        /// If true, a creature of this species can be knocked out to tame it.
        /// </summary>
        public bool violent { get; set; }
        
        /// <summary>
        /// If true, a creature of this species can be tamed while awake.
        /// </summary>
        public bool nonViolent { get; set; }
        
        public double tamingIneffectiveness { get; set; }
        
        /// <summary>
        /// Names of food the species eats during taming.
        /// </summary>
        public string[] eats { get; set; }
        
        /// <summary>
        /// If a food has non default values for this species, it's defined here.
        /// </summary>
        public Dictionary<string, TamingFood> specialFoodValues { get; set; }
        
        /// <summary>
        /// Food a species eats after being tamed, additionally to the taming food in eats.
        /// </summary>
        public string[] eatsAlsoPostTame { get; set; }
        
        /// <summary>
        /// Base value of needed affinity.
        /// </summary>
        public double affinityNeeded0 { get; set; }
        
        /// <summary>
        /// Increase of needed affinity per level.
        /// </summary>
        public double affinityIncreasePL { get; set; }
        
        public double torporDepletionPS0 { get; set; }
        public double foodConsumptionBase { get; set; }
        
        /// <summary>
        /// Multiplier during taming
        /// </summary>
        public double foodConsumptionMult { get; set; }
        
        /// <summary>
        /// Multiplier when tamed.
        /// Multiply with foodConsumptionBase when maturation is at 0 %, when at 100 % maturation multiply with const 0.000155, in between interpolate linearly.
        /// This value is the product of the ue properties BabyDinoConsumingFoodRateMultiplier and ExtraBabyDinoConsumingFoodRateMultiplier.
        /// </summary>
        public double babyFoodConsumptionMult { get; set; }
        
        /// <summary>
        /// Extra multiplier for food consumption once a creature is mature. Only few species use this, e.g. Giganotosaurus, Carcharodontosaurus, Titanosaur and Titans.
        /// </summary>
        public double adultFoodConsumptionMult { get; set; } = 1;
        
        /// <summary>
        /// Factor for affinity if tamed awake.
        /// </summary>
        public double wakeAffinityMult { get; set; }
        
        /// <summary>
        /// Factor of food depletion if tamed awake.
        /// </summary>
        public double wakeFoodDeplMult { get; set; }
    }
}
