using System.Linq;
using ARKBreedingStats.species;

namespace ARKBreedingStats
{
    /// <summary>
    /// Constants of the game Ark.
    /// </summary>
    public static class Ark
    {
        #region Breeding

        /// <summary>
        /// Probability of an offspring to inherit the higher level-stat
        /// </summary>
        public const double ProbabilityInheritHigherLevel = 0.55;

        /// <summary>
        /// Probability of an offspring to inherit the lower level-stat
        /// </summary>
        public const double ProbabilityInheritLowerLevel = 1 - ProbabilityInheritHigherLevel;

        /// <summary>
        /// Probability of a mutation in an offspring
        /// </summary>
        public const double ProbabilityOfMutation = 0.025;

        /// <summary>
        /// The max possible new mutations for a bred creature.
        /// </summary>
        public const int MutationRolls = 3;

        /// <summary>
        /// Number of levels that are added to a stat if a mutation occurred.
        /// </summary>
        public const int LevelsAddedPerMutation = 2;

        /// <summary>
        /// A mutation is possible if the Mutations are less than this number.
        /// </summary>
        public const int MutationPossibleWithLessThan = 20;

        /// <summary>
        /// The probability that at least one mutation happens if both parents have a mutation counter of less than 20.
        /// </summary>
        public const double ProbabilityOfOneMutation = 1 - (1 - ProbabilityOfMutation) * (1 - ProbabilityOfMutation) * (1 - ProbabilityOfMutation);

        /// <summary>
        /// The approximate probability of at least one mutation if one parent has less and one parent has larger or equal 20 mutation.
        /// It's assumed that the stats of the mutated stat are the same for the parents.
        /// If they differ, the probability for a mutation from the parent with the higher stat is probabilityHigherLevel * probabilityOfMutation etc.
        /// </summary>
        public const double ProbabilityOfOneMutationFromOneParent = 1 - (1 - ProbabilityOfMutation / 2) * (1 - ProbabilityOfMutation / 2) * (1 - ProbabilityOfMutation / 2);

        #endregion

        #region Mutagen

        /// <summary>
        /// Level ups per stat when applying mutagen to a non bred creature.
        /// </summary>
        public const int MutagenLevelUpsNonBred = 5;
        /// <summary>
        /// Level ups per stat when applying mutagen to a bred creature.
        /// </summary>
        public const int MutagenLevelUpsBred = 1;
        /// <summary>
        /// Indices of the stats that are affected by a mutagen application (HP, St, We, Dm).
        /// </summary>
        public static readonly int[] StatIndicesAffectedByMutagen =
        {
            Stats.Health,
            Stats.Stamina,
            Stats.Weight,
            Stats.MeleeDamageMultiplier
        };

        private const int StatCountAffectedByMutagen = 4;

        /// <summary>
        /// Total level ups for bred creatures when mutagen is applied.
        /// </summary>
        public const int MutagenTotalLevelUpsBred = MutagenLevelUpsBred * StatCountAffectedByMutagen;

        /// <summary>
        /// Total level ups for non bred creatures when mutagen is applied.
        /// </summary>
        public const int MutagenTotalLevelUpsNonBred = MutagenLevelUpsNonBred * StatCountAffectedByMutagen;

        #endregion

        #region Colors

        public const byte ColorFirstId = 1;
        public const byte ColorMaxId = 200;
        public const byte DyeFirstId = 201;
        public const byte DyeMaxId = 255;
        /// <summary>
        /// When choosing a random color for a mutation, ARK can erroneously select an undefined color. Usually this is color id 227 (one too high to be defined).
        /// </summary>
        public const byte UndefinedColorId = 227;

        /// <summary>
        /// Number of possible color regions for all species.
        /// </summary>
        public const int ColorRegionCount = 6;

        #endregion
    }

    /// <summary>
    /// Stat indices and count.
    /// </summary>
    public static class Stats
    {
        /// <summary>
        /// Total count of all stats.
        /// </summary>
        public const int StatsCount = 12;

        public const int Health = 0;
        public const int Stamina = 1;
        public const int Torpidity = 2;
        public const int Oxygen = 3;
        public const int Food = 4;
        public const int Water = 5;
        public const int Temperature = 6;
        public const int Weight = 7;
        public const int MeleeDamageMultiplier = 8;
        public const int SpeedMultiplier = 9;
        public const int TemperatureFortitude = 10;
        public const int CraftingSpeedMultiplier = 11;

        /// <summary>
        /// Returns the stat-index for the given order index (like it is ordered in game).
        /// </summary>
        public static readonly int[] DisplayOrder = {
            Health,
            Stamina,
            Oxygen,
            Food,
            Water,
            Temperature,
            Weight,
            MeleeDamageMultiplier,
            SpeedMultiplier,
            TemperatureFortitude,
            CraftingSpeedMultiplier,
            Torpidity
        };
    }
}
