using System;
using System.Drawing;

namespace ARKBreedingStats.species
{
    public class ARKColor
    {
        public string name;
        public Color color;
        /// <summary>
        /// Depends on the unreal rgb-values
        /// </summary>
        public int hash;
        public double[] arkRgb;
        /// <summary>
        /// The id currently used, it can change with mods.
        /// </summary>
        public int id;

        public ARKColor()
        {
            name = "unknown";
            color = Color.LightGray;
            arkRgb = new double[] { 0, 0, 0 };
        }

        public ARKColor(string name, double[] colorValues)
        {
            this.name = name;
            if (colorValues.Length > 3)
            {
                color = Color.FromArgb(LinearColorComponentToColorComponentClamped(colorValues[0]),
                                       LinearColorComponentToColorComponentClamped(colorValues[1]),
                                       LinearColorComponentToColorComponentClamped(colorValues[2]));
                // TODO support HDR color part

                hash = ColorHashCode(
                    colorValues[0],
                    colorValues[1],
                    colorValues[2]
                    );

                arkRgb = new double[] {colorValues[0],
                                       colorValues[1],
                                       colorValues[2]};
            }
            else
            {
                color = Color.LightGray;
                hash = 0;
                arkRgb = new double[] { 0, 0, 0 };
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

        public static int ColorHashCode(double a, double r, double g, double b)
        {
            // the creature export files contain the color definitions with 6 decimal places
            // round to 5 decimal places to get consistent matches
            return (
                    Math.Round(r, 5).ToString() + "," +
                    Math.Round(g, 5).ToString() + "," +
                    Math.Round(b, 5).ToString() + "," +
                    Math.Round(a, 5).ToString()
                    ).GetHashCode();
        }

        public static int ColorHashCode(double r, double g, double b)
        {
            return ColorHashCode(0, r, g, b);
        }
    }
}
