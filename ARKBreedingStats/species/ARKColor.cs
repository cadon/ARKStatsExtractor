using System;
using System.Drawing;
using SavegameToolkit.Types;

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
        /// Linear color values.
        /// </summary>
        public readonly double[] LinearRgba;
        /// <summary>
        /// Color Id in Ark.
        /// </summary>
        public byte Id;

        public bool IsDye;

        public ArkColor()
        {
            Id = 0;
            Name = Loc.S("noColor");
            Color = Color.LightGray;
            LinearRgba = null;
        }

        public ArkColor(string name, double[] linearColorValues, bool isDye)
        {
            Name = name;
            IsDye = isDye;
            if (linearColorValues.Length > 3)
            {
                Color = Color.FromArgb(LinearColorComponentToColorComponentClamped(linearColorValues[0]),
                                       LinearColorComponentToColorComponentClamped(linearColorValues[1]),
                                       LinearColorComponentToColorComponentClamped(linearColorValues[2]));

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

        public override string ToString() => $"{Name}{(IsDye ? " (Dye)" : string.Empty)} ({Color})";
    }
}
