using System;
using Newtonsoft.Json;

namespace ARKBreedingStats.species
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
        /// <summary>
        /// If true adding a level will increase the stat value as a percentage of the stat value so far.
        /// If false adding a level will increase the stat value by a fixed value.
        /// This is true for most stats.
        /// </summary>
        public bool IncreaseStatAsPercentage = true;
        public double ValueCap;

        public double ApplyCap(double statValue) => Math.Min(statValue, ValueCap);
    }
}
