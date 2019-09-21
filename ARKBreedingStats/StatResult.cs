using ARKBreedingStats.miscClasses;

namespace ARKBreedingStats
{
    public class StatResult
    {
        public readonly int levelWild;
        public readonly int levelDom;
        public double statValue;
        public readonly MinMaxDouble TE;
        public bool currentlyNotValid = false; // set to true if result violates other choosen result

        public StatResult(int levelWild, int levelDom, double statValue = 0, MinMaxDouble? TE = null)
        {
            this.levelWild = levelWild;
            this.levelDom = levelDom;
            this.TE = TE ?? new MinMaxDouble(-1);
            this.statValue = statValue;
        }
    }
}
