using System.Drawing;
using ARKBreedingStats.Library;
using static ARKBreedingStats.InfoGraphic.Modern.ModernInfoGraphicSettings;

namespace ARKBreedingStats.InfoGraphic.Modern
{
    /// <summary>
    /// Resolved color roles for one render. Everything the renderer paints refers to a role here
    /// rather than to a literal color, so a new theme is a change in one place.
    /// </summary>
    internal class ModernTheme
    {
        internal Color BackgroundTop;
        internal Color BackgroundBottom;
        internal Color Accent;
        /// <summary>
        /// The accent as text. Pushed away from the background's brightness when the two are too
        /// close, so a pale accent stays readable on a pale background.
        /// </summary>
        internal Color AccentText;
        internal Color Border;
        /// <summary>Empty part of a stat bar.</summary>
        internal Color Track;
        internal Color TextPrimary;
        internal Color TextMuted;
        /// <summary>Text drawn on top of a filled stat bar.</summary>
        internal Color TextOnBar;
        internal Color Divider;
        internal Color SexFemale;
        internal Color SexMale;
        internal Color SexUnknown;

        /// <summary>
        /// Builds the palette for a creature. When
        /// <see cref="ModernInfoGraphicSettings.AccentFromCreature"/> is set, the accent takes the hue
        /// of the creature's main color region, reusing the same approach as the classic renderer.
        /// </summary>
        internal static ModernTheme Create(ModernInfoGraphicSettings settings, Creature creature)
        {
            var accent = settings.AccentColor;
            if (settings.AccentFromCreature)
            {
                var mainColor = CreatureInfoGraphic.GetMainColor(creature);
                // keep the hue of the creature but force a saturation and brightness that stays legible
                accent = Utils.ColorFromHsv(mainColor.GetHue(), 0.78, settings.Theme == ModernThemes.Dark ? 0.93 : 0.62);
            }

            var theme = settings.Theme == ModernThemes.Light
                ? new ModernTheme
                {
                    BackgroundTop = Color.FromArgb(0xF6, 0xF9, 0xFA),
                    BackgroundBottom = Color.FromArgb(0xE2, 0xEA, 0xED),
                    Track = Color.FromArgb(0xCB, 0xD8, 0xDD),
                    TextPrimary = Color.FromArgb(0x10, 0x22, 0x2A),
                    TextMuted = Color.FromArgb(0x52, 0x70, 0x7B),
                    TextOnBar = Color.FromArgb(0x10, 0x22, 0x2A),
                    Divider = Color.FromArgb(0x40, 0x10, 0x22, 0x2A)
                }
                : new ModernTheme
                {
                    BackgroundTop = Color.FromArgb(0x0B, 0x1A, 0x20),
                    BackgroundBottom = Color.FromArgb(0x12, 0x26, 0x2E),
                    Track = Color.FromArgb(0x24, 0x3B, 0x44),
                    TextPrimary = Color.White,
                    TextMuted = Color.FromArgb(0xA8, 0xC4, 0xCC),
                    TextOnBar = Color.White,
                    Divider = Color.FromArgb(0x50, 0xA8, 0xC4, 0xCC)
                };

            ApplyCustomBackground(theme, settings.BackgroundColor);

            theme.Accent = accent;
            theme.AccentText = ReadableOn(accent, theme.BackgroundTop);
            theme.Border = accent;
            theme.SexFemale = Color.FromArgb(0xF0, 0x50, 0xA0);
            theme.SexMale = Color.FromArgb(0x3F, 0xA9, 0xF5);
            theme.SexUnknown = Color.FromArgb(0x8A, 0x9B, 0xA3);
            return theme;
        }

        /// <summary>
        /// Replaces the theme background with a user picked one. The text and track colors are
        /// derived from it rather than kept, otherwise picking a light background on the dark theme
        /// leaves white text on white.
        /// </summary>
        private static void ApplyCustomBackground(ModernTheme theme, Color background)
        {
            if (background.IsEmpty || background.A == 0) return;

            theme.BackgroundTop = background;
            theme.BackgroundBottom = Utils.AdjustColorLight(background, -0.18);

            var foreColor = DrawingPrimitives.ContrastingText(background);
            var onDarkBackground = foreColor.R > 127;

            theme.TextPrimary = foreColor;
            theme.TextOnBar = foreColor;
            theme.TextMuted = Utils.AdjustColorLight(foreColor, onDarkBackground ? -0.28 : 0.38);
            theme.Track = Utils.AdjustColorLight(background, onDarkBackground ? 0.16 : -0.14);
            theme.Divider = Color.FromArgb(0x50, foreColor);
        }

        /// <summary>
        /// Pushes a color away from the background when both sit in the same brightness band.
        /// ContrastingText answers "does this need light text", so two colors that give the same
        /// answer are too close to read one on the other.
        /// </summary>
        private static Color ReadableOn(Color color, Color background)
        {
            var backgroundNeedsLightText = DrawingPrimitives.ContrastingText(background) == Color.White;
            if (backgroundNeedsLightText != (DrawingPrimitives.ContrastingText(color) == Color.White)) return color;

            return Utils.AdjustColorLight(color, backgroundNeedsLightText ? 0.5 : -0.5);
        }

        internal Color SexColor(Sex sex)
        {
            switch (sex)
            {
                case Sex.Female: return SexFemale;
                case Sex.Male: return SexMale;
                default: return SexUnknown;
            }
        }
    }
}
