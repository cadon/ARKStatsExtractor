using System;
using System.Drawing;

namespace ARKBreedingStats.species
{
    /// <summary>
    /// Class that represents a color in ARK.
    /// It contains the ingame name, a Color object and the linear color values.
    /// </summary>
    public class ArkColor
    {
        public readonly string Name;
        public Color Color;
        /// <summary>
        /// Depends on the linear rgb-values
        /// </summary>
        public readonly int Hash;
        /// <summary>
        /// Linear color values.
        /// </summary>
        public readonly double[] LinearRgba;
        /// <summary>
        /// Color Id in Ark.
        /// </summary>
        public int Id;

        public ArkColor()
        {
            Id = 0;
            Name = Loc.S("noColor");
            Color = Color.LightGray;
            LinearRgba = null;
            Hash = 0;
        }

        public ArkColor(string name, double[] linearColorValues)
        {
            Name = name;
            if (linearColorValues.Length > 3)
            {
                Color = Color.FromArgb(LinearColorComponentToColorComponentClamped(linearColorValues[0]),
                                       LinearColorComponentToColorComponentClamped(linearColorValues[1]),
                                       LinearColorComponentToColorComponentClamped(linearColorValues[2]));

                Hash = ColorHashCode(
                    linearColorValues[0],
                    linearColorValues[1],
                    linearColorValues[2],
                    linearColorValues[3]
                    );

                LinearRgba = new double[] {
                    linearColorValues[0],
                    linearColorValues[1],
                    linearColorValues[2],
                    linearColorValues[3]
                };
            }
            else
            {
                // color is invalid and will be ignored.
                LinearRgba = null;
            }
        }

        /// <summary>
        /// Convert the color definition of the unreal engine to default RGB-values
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        private static int LinearColorComponentToColorComponentClamped(double lc)
        {
            //int v = (int)(255.999f * (lc <= 0.0031308f ? lc * 12.92f : Math.Pow(lc, 1.0f / 2.4f) * 1.055f - 0.055f)); // this formula is only used since UE4.15
            // ARK uses this simplified formula
            int v = (int)(255.999f * Math.Pow(lc, 1f / 2.2f));
            if (v > 255) return 255;
            if (v < 0) return 0;
            return v;
        }

        /// <summary>
        /// Returns a hashcode for the given sRGBA values.
        /// The creature export files contain the color definitions with 6 decimal places;
        /// round to 5 decimal places to get consistent matches.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static int ColorHashCode(double r, double g, double b, double a)
            => $"{Math.Round(r, 5)},{Math.Round(g, 5)},{Math.Round(b, 5)},{Math.Round(a, 5)}".GetHashCode();
    }
}
