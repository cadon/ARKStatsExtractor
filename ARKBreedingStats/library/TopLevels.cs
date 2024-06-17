namespace ARKBreedingStats.library
{
    /// <summary>
    /// Top levels per species.
    /// </summary>
    public class TopLevels
    {
        private readonly int[][] _levels = new int[4][];

        public int[] WildLevelsHighest
        {
            get => _levels?[0];
            set => _levels[0] = value;
        }
        public int[] WildLevelsLowest
        {
            get => _levels?[1];
            set => _levels[1] = value;
        }
        public int[] MutationLevelsHighest
        {
            get => _levels?[2];
            set => _levels[2] = value;
        }
        public int[] MutationLevelsLowest
        {
            get => _levels?[3];
            set => _levels[3] = value;
        }
    }
}
