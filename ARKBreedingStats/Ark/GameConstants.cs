using ARKBreedingStats.species;

namespace ARKBreedingStats.Ark
{
    /// <summary>
    /// Constants of the game Ark.
    /// </summary>
    public static class GameConstants
    {
        #region Breeding

        public const double ProbabilityHigherLevel = 0.55; // probability of inheriting the higher level-stat
        public const double ProbabilityLowerLevel = 1 - ProbabilityHigherLevel; // probability of inheriting the lower level-stat
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
        public static int[] StatIndicesAffectedByMutagen => new[]
            { (int)StatNames.Health, (int)StatNames.Stamina, (int)StatNames.Weight, (int)StatNames.MeleeDamageMultiplier };

        #endregion
    }
}
