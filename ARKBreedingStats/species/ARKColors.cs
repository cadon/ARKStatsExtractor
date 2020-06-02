using System;
using System.Collections.Generic;
using System.Linq;

namespace ARKBreedingStats.species
{
    public class ARKColors
    {
        private readonly Dictionary<int, ArkColor> colorsByHash;
        private readonly Dictionary<string, ArkColor> colorsByName;
        public readonly List<ArkColor> colorsList;
        private readonly Dictionary<int, ArkColor> colorsById;

        public ARKColors(List<List<object>> colorDefinitions, List<List<object>> colorDefinitions2 = null)
        {
            colorsByHash = new Dictionary<int, ArkColor>();
            colorsByName = new Dictionary<string, ArkColor>();
            colorsList = new List<ArkColor>() {
                new ArkColor()
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
                        ArkColor ac = new ArkColor(colorName,
                            new double[] {
                            (double)colorValues[0],
                            (double)colorValues[1],
                            (double)colorValues[2],
                            (double)colorValues[3],
                            })
                        { Id = idStart };

                        // add color to lists if it is valid
                        if (ac.LinearRgba != null)
                        {
                            if (!colorsByHash.ContainsKey(ac.Hash))
                                colorsByHash.Add(ac.Hash, ac);
                            if (!colorsByName.ContainsKey(ac.Name))
                                colorsByName.Add(ac.Name, ac);
                            colorsList.Add(ac);
                        }
                    }
                    idStart++;
                }
            }
            colorsById = colorsList.ToDictionary(c => c.Id, c => c);
        }

        public ArkColor ById(int id) => colorsById.ContainsKey(id) ? colorsById[id] : new ArkColor();

        public ArkColor ByName(string name) => colorsByName.ContainsKey(name) ? colorsByName[name] : new ArkColor();

        public ArkColor ByHash(int hash) => colorsByHash.ContainsKey(hash) ? colorsByHash[hash] : new ArkColor();

        /// <summary>
        /// Returns the ARK-id of the color that is closest to the sRGB values.
        /// </summary>
        public int ClosestColorId(double r, double g, double b, double a)
            => ClosestColor(r, g, b, a).Id;

        /// <summary>
        /// Returns the ARKColor that is closest to the given argb (sRGB) values.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        private ArkColor ClosestColor(double r, double g, double b, double a)
        {
            int hash = ArkColor.ColorHashCode(r, g, b, a);
            ArkColor ac = ByHash(hash);
            if (ac.Id != 0) return ac;

            return ClosestColorFromRgb(r, g, b, a);
        }

        /// <summary>
        /// Returns the ARKColor that is closest to the given sRGB-values.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private ArkColor ClosestColorFromRgb(double r, double g, double b, double a)
            => colorsList.OrderBy(n => ColorDifference(n.LinearRgba, r, g, b, a)).First();

        /// <summary>
        /// Distance in sRGB space
        /// </summary>
        /// <returns></returns>
        private static double ColorDifference(double[] srgb, double r, double g, double b, double a)
            => srgb == null ? int.MaxValue
            : Math.Sqrt((srgb[0] - r) * (srgb[0] - r)
                                + (srgb[1] - g) * (srgb[1] - g)
                                + (srgb[2] - b) * (srgb[2] - b)
                                + (srgb[3] - a) * (srgb[3] - a)
                                );
    }
}
