using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats
{
    class StatResult
    {
        public int levelWild, levelDom;
        public double TE;
        public bool currentlyNotValid; // set to true if result violates other choosen result

        public StatResult(int levelWild, int levelDom, double TE)
        {
            this.levelWild = levelWild;
            this.levelDom = levelDom;
            this.TE = TE;
            currentlyNotValid = false;
        }
    }
}
