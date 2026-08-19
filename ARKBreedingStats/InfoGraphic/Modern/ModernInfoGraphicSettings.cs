using System.Drawing;

namespace ARKBreedingStats.InfoGraphic.Modern
{
    /// <summary>
    /// Settings of the modern info graphic. Deliberately a small, fresh set rather than a reuse of
    /// the classic <see cref="InfoGraphicSettings"/>, most of whose values describe a layout that
    /// does not exist in this style.
    /// </summary>
    public class ModernInfoGraphicSettings
    {
        /// <summary>
        /// Width of the card in pixels. The height follows from the content, because species use
        /// different numbers of stats.
        /// </summary>
        public int Width = DefaultWidth;

        public ModernThemes Theme = ModernThemes.Dark;

        public string FontName = Asb.DefaultFontName;

        /// <summary>
        /// Accent used for the border, the artwork glow and the highlighted numbers.
        /// </summary>
        public Color AccentColor = DefaultAccent;

        /// <summary>
        /// Derive the accent from the creature's main color region instead of <see cref="AccentColor"/>.
        /// </summary>
        public bool AccentFromCreature;

        /// <summary>
        /// Card background. <see cref="Color.Empty"/> means follow the theme, which is the default.
        /// A custom background also drives the text colors, so the card stays readable whatever is
        /// picked.
        /// </summary>
        public Color BackgroundColor = Color.Empty;

        /// <summary>
        /// Color the stat bars by level quality (the red to green ramp used everywhere else in ASB)
        /// instead of filling them flat with the accent color.
        /// </summary>
        public bool BarsByLevelQuality = true;

        public StatValueDisplays ValueDisplay = StatValueDisplays.Both;

        /// <summary>
        /// Use the creature's own name as the title. Without it the species name is the title.
        /// </summary>
        public bool ShowCreatureName = true;

        public bool ShowSpecies = true;

        /// <summary>
        /// Include the bracketed variant and mod suffixes in the species name, e.g.
        /// "Veilwyn (Companion, LostColony) (ASA)" rather than just "Veilwyn". They matter in a
        /// library mixing mods, and are noise in one that does not.
        /// </summary>
        public bool ShowSpeciesSuffixes = true;
        public bool ShowStatValues = true;
        public bool ShowColors = true;
        public bool ShowMutations = true;
        public bool ShowGeneration = true;

        /// <summary>
        /// Max wild level of the server, for judging how good the wild levels are.
        /// </summary>
        public bool ShowMaxWildLevel;

        /// <summary>
        /// Add the mutated levels onto the wild levels instead of listing them in their own column.
        /// </summary>
        public bool SumWildAndMutatedLevels;

        /// <summary>
        /// When to name the color regions instead of only showing their color id.
        /// </summary>
        public ColorRegionNameDisplays RegionNames = ColorRegionNameDisplays.WithoutArtwork;

        /// <summary>
        /// Draw a soft rim around the creature so it separates from the card. The rim color comes
        /// from the background, not from the artwork, so it works for any species.
        /// </summary>
        public bool ArtworkHalo = true;

        /// <summary>
        /// Draw no card background, so the graphic can be placed on any surface.
        /// </summary>
        public bool TransparentBackground;

        /// <summary>
        /// Width the layout is designed for. Every size in the renderer is expressed relative to
        /// this and scaled by <see cref="Width"/> divided by it.
        /// The card is deliberately wide and short: chat clients cap the height of an inline
        /// preview at roughly 350 px, so a tall card gets scaled down until its text is unreadable
        /// and the reader has to open the image.
        /// </summary>
        public const int DesignWidth = 360;

        /// <summary>
        /// Default output width, deliberately the design width so the card is rendered 1:1.
        /// A chat client shows an image untouched while it fits the preview box and downscales it
        /// otherwise, and a downscaled render is what makes the text look soft. Exporting larger
        /// does not help: the client just divides it down further.
        /// </summary>
        public const int DefaultWidth = DesignWidth;
        public static readonly Color DefaultAccent = Color.FromArgb(0x22, 0xD3, 0xEE);

        public enum ModernThemes
        {
            Dark,
            Light
        }

        public enum ColorRegionNameDisplays
        {
            /// <summary>
            /// Only the color ids, in a compact single row of chips.
            /// </summary>
            Off,
            /// <summary>
            /// Name the regions when the species has no artwork, where nothing else in the graphic
            /// says which region a color belongs to.
            /// </summary>
            WithoutArtwork,
            Always
        }

        public enum StatValueDisplays
        {
            /// <summary>
            /// Value including domesticated levels and imprinting.
            /// </summary>
            Current,
            /// <summary>
            /// Value from wild and mutated levels only, the figure that matters for breeding.
            /// </summary>
            Breeding,
            /// <summary>
            /// Breeding value and current value, separated by a slash.
            /// </summary>
            Both
        }
    }
}
