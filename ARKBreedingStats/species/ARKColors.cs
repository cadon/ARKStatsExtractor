using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats.species
{
    public class ARKColors
    {
        public Dictionary<int, ARKColor> colorsByHash;
        public Dictionary<string, ARKColor> colorsByName;
        public List<ARKColor> colorsList;

        public ARKColors(List<List<object>> colorDefinitions)
        {
            colorsByHash = new Dictionary<int, ARKColor>();
            colorsByName = new Dictionary<string, ARKColor>();
            colorsList = new List<ARKColor>() {
                new ARKColor()
            };

            if (colorDefinitions == null) return;

            int id = 1;

            foreach (List<object> cd in colorDefinitions)
            {
                if (cd.Count < 2
                    || cd[0].GetType() != typeof(string)
                    || cd[1].GetType() != typeof(object[])) continue;

                ARKColor ac = new ARKColor(cd[0] as string, cd[1] as object[]) { id = id++ };
                if (!colorsByHash.ContainsKey(ac.hash))
                    colorsByHash.Add(ac.hash, ac);
                if (!colorsByName.ContainsKey(ac.name))
                    colorsByName.Add(ac.name, ac);
                colorsList.Add(ac);
            }
        }

        public ARKColor ByID(int id)
        {
            if (id > 0 && id < colorsList.Count)
                return colorsList[id];
            return new ARKColor();
        }

        public ARKColor ByName(string name)
        {
            if (colorsByName.ContainsKey(name))
                return colorsByName[name];
            return new ARKColor();
        }

        public ARKColor ByHash(int hash)
        {
            if (colorsByHash.ContainsKey(hash))
                return colorsByHash[hash];
            return new ARKColor();
        }

        public int ClosestColorID(double r, double g, double b)
        {
            return colorsList.IndexOf(ClosestColor(r, g, b));
        }

        public ARKColor ClosestColor(double r, double g, double b)
        {
            int hash = ARKColor.ColorHashCode(r, g, b);
            ARKColor ac = ByHash(hash);
            if (ac.name != "unknown") return ac;

            return ClosestColorFromRGB(r, g, b);
        }

        private ARKColor ClosestColorFromRGB(double r, double g, double b)
        {
            return colorsList.OrderBy(n => ColorDifference(n.arkRgb, r, g, b)).First();
        }

        /// <summary>
        /// Distance in RGB space
        /// </summary>
        /// <returns></returns>
        private static double ColorDifference(double[] rgb, double r, double g, double b)
        {
            return Math.Sqrt((rgb[0] - r) * (rgb[0] - r)
                                + (rgb[1] - g) * (rgb[1] - g)
                                + (rgb[2] - b) * (rgb[2] - b));
        }
    }
}
