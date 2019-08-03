using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ARKColor(string name, object[] colorObjectValues)
        {
            this.name = name;
            if (colorObjectValues?.Length > 3)
            {
                decimal[] decimalValues = new decimal[colorObjectValues.Length];
                colorObjectValues.CopyTo(decimalValues, 0);
                var doubleValues = Array.ConvertAll<decimal, double>(decimalValues, d => (double)d);

                color = Color.FromArgb(LinearColorComponentToColorComponentClamped(doubleValues[0]),
                                       LinearColorComponentToColorComponentClamped(doubleValues[1]),
                                       LinearColorComponentToColorComponentClamped(doubleValues[2]));
                // TODO support HDR color part

                hash = ColorHashCode(
                    doubleValues[0],
                    doubleValues[1],
                    doubleValues[2]
                    );

                arkRgb = new double[] {doubleValues[0],
                                       doubleValues[1],
                                       doubleValues[2]};
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
