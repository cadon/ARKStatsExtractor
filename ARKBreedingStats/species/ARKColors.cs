using System;
using System.Collections.Generic;
using System.Linq;

namespace ARKBreedingStats.species
{
    public class ARKColors
    {
        private readonly Dictionary<int, ARKColor> colorsByHash;
        private readonly Dictionary<string, ARKColor> colorsByName;
        public readonly List<ARKColor> colorsList;
        private readonly Dictionary<int, ARKColor> colorsById;

        public ARKColors(List<List<object>> colorDefinitions, List<List<object>> colorDefinitions2 = null)
        {
            colorsByHash = new Dictionary<int, ARKColor>();
            colorsByName = new Dictionary<string, ARKColor>();
            colorsList = new List<ARKColor>() {
                new ARKColor()
            };

            if (colorDefinitions == null) return;

            ParseColors(colorDefinitions, 1);
            if (colorDefinitions2 != null)
                ParseColors(colorDefinitions2, 201); // dye colors can appear as color mutation, they start with id 201

            void ParseColors(List<List<object>> colorDefs, int idStart)
            {
                foreach (List<object> cd in colorDefs)
                {
                    var t = cd[0].GetType();
                    var tt = cd[1].GetType();

                    if (cd.Count == 2
                        && cd[0] is string colorName
                        && cd[1] is Newtonsoft.Json.Linq.JArray colorValues)
                    {
                        ARKColor ac = new ARKColor(colorName,
                            new double[] {
                            (double)colorValues[0],
                            (double)colorValues[1],
                            (double)colorValues[2],
                            (double)colorValues[3],
                            })
                        { id = idStart };
                        if (!colorsByHash.ContainsKey(ac.hash))
                            colorsByHash.Add(ac.hash, ac);
                        if (!colorsByName.ContainsKey(ac.name))
                            colorsByName.Add(ac.name, ac);
                        colorsList.Add(ac);
                    }
                    idStart++;
                }
            }
            colorsById = colorsList.ToDictionary(c => c.id, c => c);
        }

        public ARKColor ByID(int id)
        {
            if (colorsById.ContainsKey(id))
                return colorsById[id];
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

        /// <summary>
        /// Returns the ARK-id of the color that is closest to the rgb values.
        /// </summary>
        public int ClosestColorID(double r, double g, double b)
        {
            return ClosestColor(r, g, b).id;
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
