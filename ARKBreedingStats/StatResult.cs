using ARKBreedingStats.valueClasses;

namespace ARKBreedingStats
{
    public class StatResult
    {
        public int levelWild, levelDom;
        public double statValue;
        public MinMaxDouble TE;
        public bool currentlyNotValid; // set to true if result violates other choosen result

        public StatResult(int levelWild, int levelDom, double statValue = 0, MinMaxDouble TE = null)
        {
            this.levelWild = levelWild;
            this.levelDom = levelDom;
            if (TE == null)
                this.TE = new MinMaxDouble(-1);
            else
                this.TE = TE;
            currentlyNotValid = false;
            this.statValue = statValue;
        }
    }
}
