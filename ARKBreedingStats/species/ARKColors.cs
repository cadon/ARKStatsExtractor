using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ARKBreedingStats.species
{
    public class ArkColors
    {
        private readonly Dictionary<int, ArkColor> colorsByHash;
        private readonly Dictionary<string, ArkColor> colorsByName;
        public readonly List<ArkColor> colorsList;
        private readonly Dictionary<byte, ArkColor> colorsById;

        public ArkColors(List<List<object>> colorDefinitions, List<List<object>> colorDefinitions2 = null)
        {
            colorsByHash = new Dictionary<int, ArkColor>();
            colorsByName = new Dictionary<string, ArkColor>();
            colorsList = new List<ArkColor> { new ArkColor() };

            if (colorDefinitions == null) return;

            ParseColors(colorDefinitions, 1);
            if (colorDefinitions2 != null)
                ParseColors(colorDefinitions2, 201); // dye colors can appear as color mutation, they start with id 201

            void ParseColors(List<List<object>> colorDefs, byte idStart)
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

        public ArkColor ById(byte id) => colorsById.TryGetValue(id, out var color) ? color : new ArkColor();

        public ArkColor ByName(string name) => colorsByName.TryGetValue(name, out var color) ? color : new ArkColor();

        public ArkColor ByHash(int hash) => colorsByHash.TryGetValue(hash, out var color) ? color : new ArkColor();

        /// <summary>
        /// Returns the ARK-id of the color that is closest to the sRGB values.
        /// </summary>
        public byte ClosestColorId(double r, double g, double b, double a)
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

        private static byte[][] _equalColorIds;

        /// <summary>
        /// If the color ids contain ids that represent colors with multiple ids, returns an array with the alternative ids.
        /// </summary>
        public static byte[] GetAlternativeColorIds(byte[] colorIds)
        {
            if (colorIds == null) return null;

            if (_equalColorIds == null)
            {
                var filePath = FileService.GetJsonPath(FileService.EqualColorIdsFile);
                if (!File.Exists(filePath)) return null;
                if (!FileService.LoadJsonFile(filePath, out _equalColorIds, out _))
                    return null;
            }

            var altColorIds = new byte[colorIds.Length];

            var altColorIdExists = false;

            byte GetAlternativeId(byte id)
            {
                for (int j = 0; j < _equalColorIds.Length; j++)
                {
                    for (int k = 0; k < _equalColorIds[j].Length; k++)
                    {
                        if (_equalColorIds[j][k] == id)
                            return k == 0 ? _equalColorIds[j][1] : _equalColorIds[j][0];
                    }
                }

                return 0;
            }

            for (int i = 0; i < colorIds.Length; i++)
            {
                var altId = GetAlternativeId(colorIds[i]);
                if (altId == 0) continue;

                altColorIds[i] = altId;
                altColorIdExists = true;
            }

            return altColorIdExists ? altColorIds : null;
        }
    }
}
