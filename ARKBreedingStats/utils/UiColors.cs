using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Newtonsoft.Json;

namespace ARKBreedingStats.utils
{
    /// <summary>
    /// Central color palette for the application. Provides the current palette
    /// based on the active theme (Light/Dark) and accessibility color mode.
    /// User customizations are stored in Properties.Settings.Default.CustomPalettes.
    /// </summary>
    internal static class UiColors
    {
        private static readonly JsonSerializerSettings JsonSettings = new()
        {
            Formatting = Formatting.Indented,
        };

        /// <summary>
        /// The active palette. Set once at startup via <see cref="Initialize"/>.
        /// </summary>
        internal static UiPalette Current { get; private set; } = RegularLight;

        /// <summary>
        /// True when the OS / user theme is dark. Set during <see cref="Initialize"/>.
        /// </summary>
        internal static bool IsDark { get; private set; }

        /// <summary>
        /// Light adjustment for the backcolor of controls that display a top stat level.
        /// </summary>
        internal static float DeltaLightnessTopStat { get; private set; }
        /// <summary>
        /// Light adjustment for the backcolor of controls that display a non top stat level of a considered stat.
        /// </summary>
        internal static float DeltaLightnessConsideredStat { get; private set; }
        /// <summary>
        /// Light adjustment for the backcolor of controls that display a stat level of a stat that is not considered.
        /// </summary>
        internal static float DeltaLightnessUnconsideredStat { get; private set; }

        /// <summary>
        /// Initializes the palette for the given color mode and theme.
        /// Loads the user's saved palette from settings if one exists, otherwise uses the built-in default.
        /// </summary>
        internal static void Initialize(ColorMode colorMode, int appTheme)
        {
            IsDark = appTheme == 2
                     || (appTheme == 0 && SystemColors.Window.R * .3f + SystemColors.Window.G * .59f +
                         SystemColors.Window.B * .11f < 110);

            Current = LoadUserPalette(colorMode, IsDark) ?? GetDefaultPalette(colorMode, IsDark);

            DeltaLightnessTopStat = IsDark ? -0.4f : 0.2f;
            DeltaLightnessConsideredStat = IsDark ? -0.75f : 0.75f;
            DeltaLightnessUnconsideredStat = IsDark ? -0.88f : 0.93f;
        }

        /// <summary>
        /// Returns the built-in (default) palette for the given mode and theme.
        /// </summary>
        internal static UiPalette GetDefaultPalette(ColorMode colorMode, bool isDark)
        {
            return colorMode switch
            {
                ColorMode.Deuteranopia => isDark
                    ? DeuteranopiaLight
                    : DeuteranopiaLight, // TODO dark variants
                ColorMode.Protanopia => isDark
                    ? ProtanopiaLight
                    : ProtanopiaLight, // TODO dark variants
                ColorMode.Tritanopia => isDark
                    ? TritanopiaLight
                    : TritanopiaLight, // TODO dark variants
                ColorMode.Monochromacy => isDark
                    ? MonochromacyLight
                    : MonochromacyLight, // TODO dark variants
                _ => isDark ? RegularDark : RegularLight,
            };
        }

        /// <summary>
        /// Saves the full palette to the user's settings.
        /// </summary>
        internal static void SaveUserPalette(ColorMode colorMode, bool isDark, UiPalette palette)
        {
            var key = PaletteKey(colorMode, isDark);
            var allPalettes = LoadAllPalettes();
            allPalettes[key] = PaletteToDict(palette);

            Properties.Settings.Default.CustomPalettes = JsonConvert.SerializeObject(allPalettes, JsonSettings);
        }

        /// <summary>
        /// Resets the current palette to the built-in default and removes it from settings.
        /// </summary>
        internal static void ResetToDefault(ColorMode colorMode, bool isDark)
        {
            var key = PaletteKey(colorMode, isDark);
            var allPalettes = LoadAllPalettes();
            allPalettes.Remove(key);

            Properties.Settings.Default.CustomPalettes = allPalettes.Count == 0
                ? string.Empty
                : JsonConvert.SerializeObject(allPalettes, JsonSettings);

            Current = GetDefaultPalette(colorMode, isDark);
        }

        /// <summary>
        /// Applies a full palette as the current one and saves it.
        /// </summary>
        internal static void ApplyAndSave(ColorMode colorMode, bool isDark, UiPalette palette)
        {
            Current = palette;
            SaveUserPalette(colorMode, isDark, palette);
        }

        /// <summary>
        /// Returns the user's saved palette for the given mode/theme, or the built-in default if none is saved.
        /// </summary>
        internal static UiPalette LoadSavedOrDefault(ColorMode colorMode, bool isDark)
        {
            return LoadUserPalette(colorMode, isDark) ?? GetDefaultPalette(colorMode, isDark);
        }

        /// <summary>
        /// Returns the user's saved palette for the given key, or the built-in default if none is saved.
        /// </summary>
        internal static UiPalette LoadSavedOrDefault(string paletteKey)
        {
            if (!TryParsePaletteKey(paletteKey, out var colorMode, out var isDark))
                return GetDefaultPalette(ColorMode.Regular, false);
            return LoadSavedOrDefault(colorMode, isDark);
        }

        #region Persistence helpers

        private static string PaletteKey(ColorMode colorMode, bool isDark)
            => $"{colorMode}_{(isDark ? "Dark" : "Light")}";

        /// <summary>
        /// Returns the palette key for the currently active palette.
        /// </summary>
        internal static string CurrentPaletteKey => PaletteKey(
            (ColorMode)Properties.Settings.Default.ColorMode, IsDark);

        /// <summary>
        /// Returns all possible palette keys (one per colorMode × light/dark).
        /// </summary>
        internal static string[] GetAllPaletteKeys()
        {
            var keys = new List<string>();
            foreach (ColorMode mode in Enum.GetValues(typeof(ColorMode)))
            {
                keys.Add(PaletteKey(mode, false));
                keys.Add(PaletteKey(mode, true));
            }

            return keys.ToArray();
        }

        /// <summary>
        /// Parses a palette key back into its colorMode and isDark components.
        /// </summary>
        internal static bool TryParsePaletteKey(string key, out ColorMode colorMode, out bool isDark)
        {
            colorMode = ColorMode.Regular;
            isDark = false;
            if (string.IsNullOrEmpty(key)) return false;

            var lastUnderscore = key.LastIndexOf('_');
            if (lastUnderscore < 0) return false;

            var modePart = key.Substring(0, lastUnderscore);
            var themePart = key.Substring(lastUnderscore + 1);

            if (!Enum.TryParse(modePart, out colorMode)) return false;
            isDark = string.Equals(themePart, "Dark", StringComparison.OrdinalIgnoreCase);
            return true;
        }

        private static Dictionary<string, Dictionary<string, string>> LoadAllPalettes()
        {
            var json = Properties.Settings.Default.CustomPalettes;
            if (string.IsNullOrEmpty(json))
                return new Dictionary<string, Dictionary<string, string>>();

            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json)
                       ?? new Dictionary<string, Dictionary<string, string>>();
            }
            catch
            {
                return new Dictionary<string, Dictionary<string, string>>();
            }
        }

        private static UiPalette LoadUserPalette(ColorMode colorMode, bool isDark)
        {
            var all = LoadAllPalettes();
            var key = PaletteKey(colorMode, isDark);
            return all.TryGetValue(key, out var dict) && dict.Count > 0
                ? DictToPalette(dict)
                : null;
        }

        private static Dictionary<string, string> PaletteToDict(UiPalette palette)
        {
            var dict = new Dictionary<string, string>();
            foreach (var prop in typeof(UiPalette).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.PropertyType != typeof(Color)) continue;
                var c = (Color)prop.GetValue(palette);
                dict[prop.Name] = $"{c.R},{c.G},{c.B}";
            }

            return dict;
        }

        private static UiPalette DictToPalette(Dictionary<string, string> dict)
        {
            var palette = new UiPalette();
            foreach (var prop in typeof(UiPalette).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.PropertyType != typeof(Color)) continue;
                if (!dict.TryGetValue(prop.Name, out var rgb)) continue;

                var parts = rgb.Split(',');
                if (parts.Length == 3
                    && int.TryParse(parts[0].Trim(), out int r)
                    && int.TryParse(parts[1].Trim(), out int g)
                    && int.TryParse(parts[2].Trim(), out int b))
                {
                    prop.SetValue(palette, Color.FromArgb(
                        Math.Clamp(r, 0, 255),
                        Math.Clamp(g, 0, 255),
                        Math.Clamp(b, 0, 255)));
                }
            }

            return palette;
        }

        /// <summary>
        /// Returns a copy of the palette with one slot changed.
        /// </summary>
        internal static UiPalette SetColor(UiPalette palette, string slotName, Color color)
        {
            var clone = palette with { };
            var prop = typeof(UiPalette).GetProperty(slotName, BindingFlags.Public | BindingFlags.Instance);
            prop?.SetValue(clone, color);
            return clone;
        }

        /// <summary>
        /// Returns all Color property names on <see cref="UiPalette"/> in declaration order.
        /// </summary>
        internal static string[] GetSlotNames()
        {
            var names = new List<string>();
            foreach (var prop in typeof(UiPalette).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.PropertyType == typeof(Color))
                    names.Add(prop.Name);
            }

            return names.ToArray();
        }

        /// <summary>
        /// Gets a color from a palette by slot name.
        /// </summary>
        internal static Color GetColor(UiPalette palette, string slotName)
        {
            var prop = typeof(UiPalette).GetProperty(slotName, BindingFlags.Public | BindingFlags.Instance);
            return prop != null ? (Color)prop.GetValue(palette) : Color.Empty;
        }

        #endregion

        #region fixed colors

        internal static Color LinkLabelText() => IsDark ? Color.CornflowerBlue : Color.Blue;

        #endregion

        // =====================================================================
        // Built-in palettes
        // =====================================================================

        #region Regular

        internal static readonly UiPalette RegularLight = new()
        {
            // UI Semantic
            Success = Color.FromArgb(180, 255, 128),
            Warning = Color.LightSalmon,
            Error = Color.LightCoral,
            Info = Color.LightSkyBlue,
            Caution = Color.Gold,
            Neutral = Color.FromArgb(240, 240, 240),
            NonUnique = Color.FromArgb(255, 255, 127),
            SuccessText = Color.DarkGreen,
            WarningText = Color.FromArgb(150, 142, 0),
            ErrorText = Color.Red,

            // Sex
            SexMale = Color.FromArgb(220, 235, 255),
            SexFemale = Color.FromArgb(255, 230, 255),
            SexNeutered = Color.FromArgb(220, 220, 220),
            SexMaleText = Color.DarkBlue,
            SexFemaleText = Color.DarkRed,

            // Creature Status
            DeadCreature = Color.FloralWhite,
            ObeliskText = Color.DarkBlue,
            OverLevelWarning = Color.OrangeRed,
            TopBreedingAll = Color.Gold,
            TopBreedingSome = Color.LightGreen,
            CreatureExisting = Color.Aquamarine,

            // Mutations
            Mutation = Color.FromArgb(225, 192, 255),
            MutationOverLimit = Color.FromArgb(255, 200, 200),
            MutationMarker = Color.Magenta,
            MutationMarkerPossible = Color.FromArgb(204, 123, 255),

            // Special Levels
            Level254 = Color.FromArgb(0, 196, 255),
            Level255 = Color.FromArgb(255, 0, 159),

            // Countdown: Growing
            GrowingImminent = Color.FromArgb(168, 187, 255),
            GrowingSoon = Color.FromArgb(197, 168, 255),
            GrowingLater = Color.FromArgb(236, 168, 255),

            // Countdown: Cooldown
            CooldownImminent = Color.FromArgb(235, 255, 109),
            CooldownSoon = Color.FromArgb(255, 250, 109),
            CooldownLater = Color.FromArgb(255, 179, 109),

            // Similarity
            SimilarityGood = Color.Green,
            SimilarityOk = Color.DarkOrange,
            SimilarityPoor = Color.DarkRed,

            // Actions
            ActionAdd = Color.FromArgb(210, 255, 240),
            ActionRemove = Color.FromArgb(255, 225, 225),
            LockedInput = Color.LightGray,

            // Filter
            FilterActive = Color.LightGoldenrodYellow,
            FilterEmpty = Color.LightSalmon,
            FilterButton = Color.Orange,

            // Color Indicators
            NewColorInRegion = Color.DarkGreen,
            NewColorInSpecies = Color.Gold,

            // Divider
            DividerLine = Color.FromArgb(80, 120, 200),

            // Pedigree
            PedigreeLineMaternal = Color.Green,
            PedigreeLinePaternal = Color.DarkRed,
            PedigreeSelected = Color.DodgerBlue,
        };

        internal static readonly UiPalette RegularDark = new()
        {
            // UI Semantic
            Success = Color.FromArgb(60, 120, 40),
            Warning = Color.FromArgb(140, 70, 50),
            Error = Color.FromArgb(150, 50, 50),
            Info = Color.FromArgb(50, 80, 130),
            Caution = Color.FromArgb(130, 110, 0),
            Neutral = Color.FromArgb(32, 32, 32),
            NonUnique = Color.FromArgb(120, 110, 30),
            SuccessText = Color.LightGreen,
            WarningText = Color.FromArgb(255, 234, 64),
            ErrorText = Color.FromArgb(255, 100, 100),

            // Sex
            SexMale = Color.FromArgb(40, 50, 80),
            SexFemale = Color.FromArgb(80, 40, 60),
            SexNeutered = Color.FromArgb(80, 80, 80),
            SexMaleText = Color.CornflowerBlue,
            SexFemaleText = Color.FromArgb(220, 100, 100),

            // Creature Status
            DeadCreature = Color.FromArgb(50, 40, 35),
            ObeliskText = Color.CornflowerBlue,
            OverLevelWarning = Color.OrangeRed,
            TopBreedingAll = Color.FromArgb(130, 110, 0),
            TopBreedingSome = Color.FromArgb(0, 100, 0),
            CreatureExisting = Color.FromArgb(0, 80, 70),

            // Mutations
            Mutation = Color.FromArgb(90, 60, 120),
            MutationOverLimit = Color.FromArgb(120, 50, 50),
            MutationMarker = Color.Magenta,
            MutationMarkerPossible = Color.FromArgb(140, 70, 170),

            // Special Levels
            Level254 = Color.FromArgb(0, 120, 170),
            Level255 = Color.FromArgb(170, 0, 100),

            // Countdown: Growing
            GrowingImminent = Color.FromArgb(50, 60, 120),
            GrowingSoon = Color.FromArgb(60, 50, 120),
            GrowingLater = Color.FromArgb(80, 50, 100),

            // Countdown: Cooldown
            CooldownImminent = Color.FromArgb(70, 80, 20),
            CooldownSoon = Color.FromArgb(80, 75, 20),
            CooldownLater = Color.FromArgb(80, 55, 20),

            // Similarity
            SimilarityGood = Color.FromArgb(50, 180, 50),
            SimilarityOk = Color.DarkOrange,
            SimilarityPoor = Color.FromArgb(200, 60, 60),

            // Actions
            ActionAdd = Color.FromArgb(40, 80, 60),
            ActionRemove = Color.FromArgb(100, 40, 40),
            LockedInput = Color.FromArgb(60, 60, 60),

            // Filter
            FilterActive = Color.FromArgb(80, 75, 20),
            FilterEmpty = Color.FromArgb(120, 50, 40),
            FilterButton = Color.FromArgb(140, 90, 0),

            // Color Indicators
            NewColorInRegion = Color.FromArgb(0, 140, 0),
            NewColorInSpecies = Color.FromArgb(180, 150, 0),

            // Divider
            DividerLine = Color.FromArgb(80, 120, 200),

            // Pedigree
            PedigreeLineMaternal = Color.FromArgb(80, 200, 80),
            PedigreeLinePaternal = Color.FromArgb(200, 80, 80),
            PedigreeSelected = Color.DodgerBlue,
        };

        #endregion

        #region Accessibility modes (TODO: needs review by people with these conditions)

        // Deuteranopia — no green perception.
        // Greens are replaced with blues; reds with yellows/oranges.
        internal static readonly UiPalette DeuteranopiaLight = RegularLight with
        {
            Success = Color.FromArgb(111, 157, 255),
            NonUnique = Color.FromArgb(200, 123, 60),
            Error = Color.FromArgb(255, 200, 50),
            TopBreedingSome = Color.FromArgb(140, 180, 255),
            SimilarityGood = Color.FromArgb(50, 100, 200),
        };

        // Protanopia — no red perception.
        internal static readonly UiPalette ProtanopiaLight = RegularLight with
        {
            Success = Color.FromArgb(111, 157, 255),
            NonUnique = Color.FromArgb(200, 123, 60),
            Error = Color.FromArgb(255, 200, 50),
            SimilarityPoor = Color.FromArgb(180, 130, 0),
        };

        // Tritanopia — no blue perception.
        internal static readonly UiPalette TritanopiaLight = RegularLight with
        {
            Success = Color.FromArgb(140, 241, 255),
            NonUnique = Color.FromArgb(255, 199, 214),
            Error = Color.FromArgb(255, 112, 119),
            Info = Color.FromArgb(200, 230, 200),
        };

        // Monochromacy — no color perception, only lightness.
        internal static readonly UiPalette MonochromacyLight = RegularLight with
        {
            Success = Color.FromArgb(218, 218, 218),
            NonUnique = Color.FromArgb(176, 176, 176),
            Error = Color.FromArgb(132, 132, 132),
            Info = Color.FromArgb(200, 200, 200),
            Caution = Color.FromArgb(160, 160, 160),
        };

        #endregion

        internal enum ColorMode
        {
            Regular,

            /// <summary>
            /// Deuteranopia — no green perception.
            /// Greens are replaced with blues; reds with yellows/oranges.
            /// </summary>
            Deuteranopia,

            /// <summary>
            /// Protanopia — no red perception.
            /// </summary>
            Protanopia,

            /// <summary>
            /// Tritanopia — no blue perception.
            /// </summary>
            Tritanopia,

            /// <summary>
            /// Monochromacy — no color perception, only lightness.
            /// </summary>
            Monochromacy
        }
    }
}
