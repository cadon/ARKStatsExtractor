namespace ArkSmartBreeding.Models.Ark
{
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
        /// <summary>
        /// Stamina, or Charge Capacity for glow species
        /// </summary>
        public const int Stamina = 1;
        public const int Torpidity = 2;
        /// <summary>
        /// Oxygen, or Charge Regeneration for glow species
        /// </summary>
        public const int Oxygen = 3;
        public const int Food = 4;
        public const int Water = 5;
        public const int Temperature = 6;
        public const int Weight = 7;
        /// <summary>
        /// MeleeDamageMultiplier, or Charge Emission Range for glow species
        /// </summary>
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

        /// <summary>
        /// Returns the stat indices for the stats usually displayed for species (e.g. no crafting speed Gacha) in game.
        /// </summary>
        public static readonly bool[] UsuallyVisibleStats = {
            true, //Health,
            true, //Stamina,
            true, //Torpidity,
            true, //Oxygen,
            true, //Food,
            false, //Water,
            false, //Temperature,
            true, //Weight,
            true, //MeleeDamageMultiplier,
            true, //SpeedMultiplier,
            false, //TemperatureFortitude,
            false, //CraftingSpeedMultiplier
        };

        /// <summary>
        /// Returns if the stat is a percentage value.
        /// </summary>
        public static bool IsPercentage(int statIndex)
        {
            return statIndex == MeleeDamageMultiplier
                   || statIndex == SpeedMultiplier
                   || statIndex == TemperatureFortitude
                   || statIndex == CraftingSpeedMultiplier;
        }

        /// <summary>
        /// Returns the displayed decimal values of the stat with the given index
        /// </summary>
        public static int Precision(int statIndex)
        {
            // damage and speed are percentage values and thus the displayed values have a higher precision
            return IsPercentage(statIndex) ? 3 : 1;
        }
    }
}
