namespace ARKBreedingStats
{
    class StatResult
    {
        public int levelWild, levelDom;
        public double TE, TEMin, TEMax;
        public bool currentlyNotValid; // set to true if result violates other choosen result

        public StatResult(int levelWild, int levelDom, double TE = -1, double TEMin = -1, double TEMax = -1)
        {
            this.levelWild = levelWild;
            this.levelDom = levelDom;
            this.TE = TE;
            this.TEMin = TEMin;
            this.TEMax = TEMax;
            currentlyNotValid = false;
        }
    }
}
