using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats
{
    public class ColorRegions
    {
        public string[] names;
        public int[][] colorIds;
        public bool[] regionUsed;

        public ColorRegions()
        {
            names = new string[6];
            colorIds = new int[6][];
            regionUsed = new bool[6];
        }
    }
}
