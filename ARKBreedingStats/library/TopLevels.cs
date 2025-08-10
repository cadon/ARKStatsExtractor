using System.Linq;

namespace ARKBreedingStats.library
{
    /// <summary>
    /// Top levels per species.
    /// </summary>
    public class TopLevels
    {
        private readonly int[][] _levels;
        /// <summary>
        /// The minimum total level for a creature to have at least all current top levels.
        /// Offspring with less than that level miss at least one top level.
        /// </summary>
        public int MinLevelForTopCreature = -1;

        public TopLevels()
        {
            _levels = GetUninitialized();
        }

        public TopLevels(bool allZeros)
        {
            _levels = allZeros ? GetZeros() : GetUninitialized();
        }

        public int[] WildLevelsHighest
        {
            get => _levels[0];
            set => _levels[0] = value;
        }
        public int[] WildLevelsLowest
        {
            get => _levels[1];
            set => _levels[1] = value;
        }
        public int[] MutationLevelsHighest
        {
            get => _levels[2];
            set => _levels[2] = value;
        }
        public int[] MutationLevelsLowest
        {
            get => _levels[3];
            set => _levels[3] = value;
        }

        private int[][] GetZeros() => new[]
        {
            Enumerable.Repeat(0, Stats.StatsCount).ToArray(),
            Enumerable.Repeat(0, Stats.StatsCount).ToArray(),
            Enumerable.Repeat(0, Stats.StatsCount).ToArray(),
            Enumerable.Repeat(0, Stats.StatsCount).ToArray()
        };

        private int[][] GetUninitialized() => new[]
        {
            Enumerable.Repeat(0, Stats.StatsCount).ToArray(),
            Enumerable.Repeat(int.MaxValue, Stats.StatsCount).ToArray(),
            Enumerable.Repeat(0, Stats.StatsCount).ToArray(),
            Enumerable.Repeat(int.MaxValue, Stats.StatsCount).ToArray()
        };
    }
}
