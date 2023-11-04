using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ARKBreedingStats.species
{
    /// <summary>
    /// Loaded color definitions used by the library.
    /// </summary>
    public class ArkColors
    {
        public ArkColor[] ColorsList;
        private Dictionary<string, ArkColor> _colorsByName;
        private Dictionary<byte, ArkColor> _colorsById;
        /// <summary>
        /// Color used if there's no definition for it.
        /// </summary>
        private static ArkColor _undefinedColor = new ArkColor("undefined", new double[] { 1, 1, 1, 0 }, false);

        /// <summary>
        /// Color definitions of the base game.
        /// </summary>
        private readonly List<ArkColor> _baseColors;

        /// <summary>
        /// If mods are loaded, each mod has its colors (or null if no color definitions are given) in the according order.
        /// </summary>
        private List<(List<ArkColor> colors, int dyeStartIndex)> _modColors;

        public ArkColors(List<ArkColor> baseColorList)
        {
            _baseColors = baseColorList;
        }

        /// <summary>
        /// Adds Ark colors of a mod value file to the base values. Should be called even if the mod has no color definitions (ARK can then add missing colors that where left out before due to mod-overwriting).
        /// </summary>
        internal void AddModArkColors((List<ArkColor> colors, int dyeStartIndex) modColors)
        {
            if (_modColors == null) _modColors = new List<(List<ArkColor> colors, int dyeStartIndex)>();
            _modColors.Add(modColors);
        }

        /// <summary>
        /// Creates the color id table according to the mod order and the lookup tables to find colors by their name or id.
        /// Call this function after the values file is loaded and after mod values are loaded that contain colors.
        /// </summary>
        public void InitializeArkColors()
        {
            if (_baseColors == null) return;

            // if no mods are loaded, use the color definitions of the base game
            // mods can overwrite the color definitions, if no colors are defined in a mod, the base color definitions are used
            // if mods are loaded, the color definitions of the first mod are used first (i.e. base colors if no mod color definitions)
            // mod colors are appended then in the according order if their name is not already used
            // example 1: only 1 mod loaded that defines colors up until id 100: 100 colors are used, if the base game has more colors, these are not used
            // example 2: 2 mods are loaded, the first defines colors up until id 100, the second has no color definitions: 100 mod colors are used, then the base colors not appearing yet are appended (from the second mod that inherits the base colors)

            _colorsByName = new Dictionary<string, ArkColor>();
            _colorsById = new Dictionary<byte, ArkColor> { { 0, new ArkColor() } };
            var nextFreeColorId = Ark.ColorFirstId;
            var nextFreeDyeId = Ark.DyeFirstIdASE;
            var colorIdMax = Ark.DyeFirstIdASE - 1;
            var noMoreAvailableColorId = false;
            var noMoreAvailableDyeId = false;

            var baseColorsAdded = false;
            void AddBaseColors()
            {
                AddColorDefinitions(_baseColors);
                baseColorsAdded = true;
            }

            // no mods are loaded or first mod has no color overrides, use base colors first
            if (_modColors?.Any() != true)
            {
                AddBaseColors();
            }
            else
            {
                // add mod color definitions, these are appended if the color name doesn't exist yet
                foreach (var modColors in _modColors)
                {
                    if (modColors.colors == null)
                    {
                        // if the mod has no color definitions, it uses the base color definitions; add them if not yet added
                        if (!baseColorsAdded)
                            AddBaseColors();

                        continue;
                    }

                    // if the mod only overwrites colors, it needs the base colors loaded
                    if (modColors.dyeStartIndex != 0 && !baseColorsAdded)
                        AddBaseColors();

                    AddColorDefinitions(modColors.colors, (byte)modColors.dyeStartIndex);
                }

                // dye colors are apparently added independently from the colors, even if base colors are not added. This might need more testing, so far no mods are found that add dye colors.
                if (!baseColorsAdded)
                {
                    AddColorDefinitions(_baseColors.Where(c => c.IsDye));
                }
            }

            // if dyeStartIndex != 0 the dye information from the mod colors overwrites the existing definitions from the index/id on
            void AddColorDefinitions(IEnumerable<ArkColor> colorDefinitions, byte dyeStartIndex = 0)
            {
                if (colorDefinitions == null) return;

                if (dyeStartIndex != 0 && dyeStartIndex <= Ark.DyeMaxId)
                {
                    nextFreeDyeId = dyeStartIndex;
                    noMoreAvailableDyeId = false;
                }

                foreach (var c in colorDefinitions)
                {
                    var colorNameExists = _colorsByName.ContainsKey(c.Name);
                    if (colorNameExists && !c.IsDye) continue; // dyes can have duplicate names, e.g. "Purple Coloring" with id 207, 211

                    if (c.IsDye)
                    {
                        if (noMoreAvailableDyeId) continue;

                        c.Id = nextFreeDyeId;
                        if (nextFreeDyeId == Ark.DyeMaxId)
                            noMoreAvailableDyeId = true;
                        else nextFreeDyeId++;
                    }
                    else
                    {
                        if (noMoreAvailableColorId) continue;

                        c.Id = nextFreeColorId;
                        if (nextFreeColorId == colorIdMax)
                            noMoreAvailableColorId = true;
                        else nextFreeColorId++;
                    }
                    if (!colorNameExists)
                        _colorsByName.Add(c.Name, c);
                    _colorsById[c.Id] = c;
                }
            }

            ColorsList = _colorsById.Values.OrderBy(c => c.Id).ToArray();
        }

        public ArkColor ById(byte id) => _colorsById.TryGetValue(id, out var color) ? color : _undefinedColor;

        public ArkColor ByName(string name) => _colorsByName.TryGetValue(name, out var color) ? color : _undefinedColor;

        /// <summary>
        /// Returns the ARK-id of the color that is closest to the sRGB values.
        /// </summary>
        public byte ClosestColorId(double r, double g, double b, double a)
            => ClosestColor(r, g, b, a).Id;

        /// <summary>
        /// Returns the ARKColor that is closest to the given argb (sRGB) values.
        /// </summary>
        private ArkColor ClosestColor(double r, double g, double b, double a)
        {
            var acc = ColorsList.FirstOrDefault(c => c.LinearRgba != null && c.LinearRgba[0] == r && c.LinearRgba[1] == g && c.LinearRgba[2] == b && c.LinearRgba[3] == a);
            if (acc != null && acc.Id != 0) return acc;

            return ClosestColorFromRgb(r, g, b, a);
        }

        /// <summary>
        /// Returns the ARKColor that is closest to the given sRGB-values.
        /// </summary>
        private ArkColor ClosestColorFromRgb(double r, double g, double b, double a)
            => ColorsList.OrderBy(n => ColorDifference(n.LinearRgba, r, g, b, a)).First();

        /// <summary>
        /// Distance in sRGB space
        /// </summary>
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

        public static List<ArkColor> ParseColorDefinitions(object[][] colorDefinitions, List<ArkColor> parsedColors, bool isDye = false)
        {
            if (colorDefinitions == null) return parsedColors;

            if (parsedColors == null) parsedColors = new List<ArkColor>();

            foreach (object[] cd in colorDefinitions)
            {
                if (cd.Length == 2
                    && cd[0] is string colorName
                    && cd[1] is Newtonsoft.Json.Linq.JArray colorValues)
                {
                    ArkColor ac = new ArkColor(colorName,
                        new[] {
                            (double)colorValues[0],
                            (double)colorValues[1],
                            (double)colorValues[2],
                            (double)colorValues[3]
                        },
                        isDye);
                    if (ac.LinearRgba != null)
                        parsedColors.Add(ac);
                }
            }

            return parsedColors.Any() ? parsedColors : null;
        }

        /// <summary>
        /// Returns an array with random color ids.
        /// </summary>
        public byte[] GetRandomColors(Random rand = null)
        {
            if (ColorsList?.Any() != true)
                return new byte[Ark.ColorRegionCount];

            if (rand == null)
                rand = new Random();

            var colors = new byte[Ark.ColorRegionCount];
            var colorCount = ColorsList.Length;
            for (int i = 0; i < Ark.ColorRegionCount; i++)
                colors[i] = ColorsList[rand.Next(colorCount)].Id;
            return colors;
        }
    }
}
