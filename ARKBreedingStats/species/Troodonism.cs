using System;
using System.Windows.Forms;

namespace ARKBreedingStats.species
{
    /// <summary>
    /// Handling the troodonism bug in ARK.
    /// </summary>
    public static class Troodonism
    {
        /// <summary>
        /// Flags which part of a stat calculation are affected by troodonism values.
        /// </summary>
        [Flags]
        public enum AffectedStats
        {
            /// <summary>
            /// All stat parts use the non troodonism values.
            /// </summary>
            None = 0,
            /// <summary>
            /// The base value uses the troodonism value.
            /// </summary>
            Base = 1,
            /// <summary>
            /// The increase per wild level value uses the troodonism value.
            /// </summary>
            IncreaseWild = 2,
            /// <summary>
            /// Combination for a creature when wild.
            /// </summary>
            WildCombination = Base,
            /// <summary>
            /// Combination for a creature after releasing from a cryopod.
            /// </summary>
            UncryoCombination = Base | IncreaseWild,
            /// <summary>
            /// Combination for a creature after a server restart.
            /// </summary>
            ServerRestartCombination = None
        }

        /// <summary>
        /// Returns the stats considering the troodonism stats stated in troodonismStats.
        /// </summary>
        public static SpeciesStat[] SelectStats(SpeciesStat[] speciesStats, SpeciesStat[] speciesAltStats, AffectedStats troodonismStats)
        {
            if (speciesAltStats == null) return speciesStats;
            var stats = new SpeciesStat[Stats.StatsCount];
            for (int s = 0; s < Stats.StatsCount; s++)
                stats[s] = SelectStats(speciesStats[s], speciesAltStats[s], troodonismStats);
            return stats;
        }

        /// <summary>
        /// Returns the stats considering the troodonism stats stated in troodonismStats.
        /// </summary>
        public static SpeciesStat SelectStats(SpeciesStat speciesStats, SpeciesStat speciesAltStats, AffectedStats troodonismStats)
        {
            if (speciesAltStats == null) return speciesStats;
            return new SpeciesStat
            {
                BaseValue = (troodonismStats.HasFlag(Troodonism.AffectedStats.Base) ? speciesAltStats : speciesStats).BaseValue,
                IncPerWildLevel = (troodonismStats.HasFlag(Troodonism.AffectedStats.IncreaseWild) ? speciesAltStats : speciesStats).IncPerWildLevel,
                AddWhenTamed = speciesStats.AddWhenTamed,
                MultAffinity = speciesStats.MultAffinity,
                IncPerTamedLevel = speciesStats.IncPerTamedLevel
            };
        }
    }
}
