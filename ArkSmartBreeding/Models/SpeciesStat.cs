using System;
using Newtonsoft.Json;

namespace ARKBreedingStats.Models
{
    /// <summary>
    /// Raw stat values for a species with all multipliers applied.
    /// These are the calculated values used to determine creature stats.
    /// </summary>
    [JsonObject]
    public class SpeciesStat
    {
        public double BaseValue { get; set; }
        public double IncPerWildLevel { get; set; }
        public double IncPerMutatedLevel { get; set; }
        public double IncPerTamedLevel { get; set; }
        public double AddWhenTamed { get; set; }
        public double MultAffinity { get; set; }
        
        /// <summary>
        /// If true adding a level will increase the stat value as a percentage of the stat value so far.
        /// If false adding a level will increase the stat value by a fixed value.
        /// This is true for most stats.
        /// </summary>
        public bool IncreaseStatAsPercentage { get; set; } = true;
        
        public double ValueCap { get; set; }

        public double ApplyCap(double statValue) => Math.Min(statValue, ValueCap);
    }
}
