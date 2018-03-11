namespace ARKBreedingStats
{
    class StatResult
    {
        public int levelWild, levelDom;
        public double TE, TEMin, TEMax, statValue;
        public bool currentlyNotValid; // set to true if result violates other choosen result

        public StatResult(int levelWild, int levelDom, double statValue = 0, double TE = -1, double TEMin = -1, double TEMax = -1)
        {
            this.levelWild = levelWild;
            this.levelDom = levelDom;
            this.TE = TE;
            this.TEMin = TEMin;
            this.TEMax = TEMax;
            currentlyNotValid = false;
            this.statValue = statValue;
        }
    }
}
