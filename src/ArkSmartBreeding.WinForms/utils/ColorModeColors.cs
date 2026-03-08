using System.Drawing;

namespace ARKBreedingStats.utils
{
    /// <summary>
    /// Contains colors dependent on the color mode. Used for color blind mode.
    /// </summary>
    internal static class ColorModeColors
    {
        internal static Color Success;
        internal static Color Neutral;
        internal static Color NonUnique;
        internal static Color Error;

        internal static void SetColors(AsbColorMode colorMode = AsbColorMode.Regular)
        {
            switch (colorMode)
            {
                case AsbColorMode.Deuteranopia:
                    Success = Color.FromArgb(111, 157, 255);
                    Neutral = Color.White;
                    NonUnique = Color.FromArgb(200, 123, 60);
                    Error = Color.FromArgb(255, 200, 50);
                    break;
                case AsbColorMode.Protanopia:
                    Success = Color.FromArgb(111, 157, 255);
                    Neutral = Color.White;
                    NonUnique = Color.FromArgb(200, 123, 60);
                    Error = Color.FromArgb(255, 200, 50);
                    break;
                case AsbColorMode.Tritanopia:
                    Success = Color.FromArgb(140, 241, 255);
                    Neutral = Color.White;
                    NonUnique = Color.FromArgb(255, 199, 214);
                    Error = Color.FromArgb(255, 112, 119);
                    break;
                case AsbColorMode.Monochromacy:
                    Success = Color.FromArgb(218, 218, 218);
                    Neutral = Color.White;
                    NonUnique = Color.FromArgb(176, 176, 176);
                    Error = Color.FromArgb(132, 132, 132);
                    break;
                default:
                    Success = Color.FromArgb(180, 255, 128);
                    Neutral = Color.White;
                    NonUnique = Color.FromArgb(255, 255, 127);
                    Error = Color.LightCoral;
                    break;
            }
        }

        internal enum AsbColorMode
        {
            Regular,
            /// <summary>
            /// No green
            /// </summary>
            Deuteranopia,
            /// <summary>
            /// No red
            /// </summary>
            Protanopia,
            /// <summary>
            /// No blue
            /// </summary>
            Tritanopia,
            /// <summary>
            /// Only lightness
            /// </summary>
            Monochromacy
        }
    }
}
