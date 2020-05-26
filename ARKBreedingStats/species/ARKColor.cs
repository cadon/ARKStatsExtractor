using System;
using System.Drawing;

namespace ARKBreedingStats.species
{
    /// <summary>
    /// Class that represents a color in ARK.
    /// It contains the ingame name, a Color object and the sRGBA values in the array arkRgba.
    /// </summary>
    public class ARKColor
    {
        public string name;
        public Color color;
        /// <summary>
        /// Depends on the unreal rgb-values
        /// </summary>
        public int hash;
        public double[] arkRgba;
        /// <summary>
        /// The id currently used, it can change with mods.
        /// </summary>
        public int id;

        public ARKColor()
        {
            id = 0;
            name = Loc.S("Unknown");
            color = Color.LightGray;
            arkRgba = null;
            hash = 0;
        }

        public ARKColor(string name, double[] colorValues)
        {
            this.name = name;
            if (colorValues.Length > 3)
            {
                color = Color.FromArgb(LinearColorComponentToColorComponentClamped(colorValues[0]),
                                       LinearColorComponentToColorComponentClamped(colorValues[1]),
                                       LinearColorComponentToColorComponentClamped(colorValues[2]));

                hash = ColorHashCode(
                    colorValues[0],
                    colorValues[1],
                    colorValues[2],
                    colorValues[3]
                    );

                arkRgba = new double[] {
                    colorValues[0],
                    colorValues[1],
                    colorValues[2],
                    colorValues[3]
                };
            }
            else
            {
                // color is invalid and will be ignored.
                arkRgba = null;
            }
        }

        /// <summary>
        /// Convert the color definition of the unreal engine to default RGB-values
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        private static int LinearColorComponentToColorComponentClamped(double lc)
        {
            int v = (int)(255.999f * (lc <= 0.0031308f ? lc * 12.92f : Math.Pow(lc, 1.0f / 2.4f) * 1.055f - 0.055f));
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
        {
            return (
                    Math.Round(r, 5).ToString() + "," +
                    Math.Round(g, 5).ToString() + "," +
                    Math.Round(b, 5).ToString() + "," +
                    Math.Round(a, 5).ToString()
                    ).GetHashCode();
        }
    }
}
