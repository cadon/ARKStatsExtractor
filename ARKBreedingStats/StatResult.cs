using ARKBreedingStats.miscClasses;

namespace ARKBreedingStats
{
    public class StatResult
    {
        public readonly int LevelWild;
        public readonly int LevelMut;
        public readonly int LevelDom;
        public readonly MinMaxDouble Te;
        public bool CurrentlyNotValid = false; // set to true if result violates other chosen result

        public StatResult(int levelWild, int levelDom, MinMaxDouble? te = null, int levelMut = 0)
        {
            LevelWild = levelWild;
            LevelMut = levelMut;
            LevelDom = levelDom;
            Te = te ?? new MinMaxDouble(-1);
        }

        public override string ToString() => $"w: {LevelWild}, m: {LevelMut}, d: {LevelDom}, TE: {Te.Mean:.000}";
    }
}
