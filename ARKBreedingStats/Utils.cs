using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ARKBreedingStats
{
    static class Utils
    {
        /// <summary>
        /// This method returns a shade from red over yellow to green, corresponding to the value p (0-100). If light is given, the brightness is adjusted (-1 to 1)
        /// </summary>
        /// <param name="percent">percent that determines the shade between red and green (0 to 100)</param>
        /// <param name="light">double from -1 to 1. Values greater zero make the color brighter, lower than zero make the color darker.</param>
        /// <returns>the calculated color.</returns>
        public static Color getColorFromPercent(int percent, double light = 0)
        {
            if (light > 1) { light = 1; }
            if (light < -1) { light = -1; }
            int r = 511 - percent * 255 / 50;
            int g = percent * 255 / 50;
            int b = 0;
            if (r < 0) { r = 0; }
            if (g < 0) { g = 0; }
            if (r > 255) { r = 255; }
            if (g > 255) { g = 255; }
            if (light > 0)
            {
                r = (int)((255 - r) * light + r);
                g = (int)((255 - g) * light + g);
                b = (int)((255 - b) * light + b);
            }
            else
            {
                light += 1;
                r = (int)(r * light);
                g = (int)(g * light);
                //b = (int)(b * light); // b == 0 anyway
            }
            return Color.FromArgb(r, g, b);
        }

        public static string genderSymbol(Gender g)
        {
            switch (g)
            {
                case Gender.Male:
                    return "♂";
                case Gender.Female:
                    return "♀";
                default:
                    return "?";
            }
        }

        public static Color genderColor(Gender g)
        {
            switch (g)
            {
                case Gender.Male:
                    return Color.FromArgb(220, 235, 255);
                case Gender.Female:
                    return Color.FromArgb(255, 230, 255);
                default:
                    return SystemColors.Control;
            }
        }

        public static string statusSymbol(CreatureStatus s)
        {
            switch (s)
            {
                case CreatureStatus.Dead:
                    return "†";
                case CreatureStatus.Unavailable:
                    return "✗";
                default:
                    return "✓";
            }
        }

        public static Gender nextGender(Gender g)
        {
            switch (g)
            {
                case Gender.Female:
                    return Gender.Male;
                case Gender.Male:
                    return Gender.Unknown;
                default:
                    return Gender.Female;
            }
        }

        public static CreatureStatus nextStatus(CreatureStatus s)
        {
            switch (s)
            {
                case CreatureStatus.Available:
                    return CreatureStatus.Unavailable;
                case CreatureStatus.Unavailable:
                    return CreatureStatus.Dead;
                default:
                    return CreatureStatus.Available;
            }
        }

        public static string levelPercentile(int l)
        {
            // percentiles assuming a normal distribution of 180 levels on 7 stats
            double[] prb = new double[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 99.99, 99.98, 99.95, 99.88, 99.72, 99.40, 98.83, 97.85, 96.28, 93.94, 90.62, 86.20, 80.61, 73.93, 66.33, 58.10, 49.59, 41.19, 33.26, 26.08, 19.85, 14.66, 10.50, 7.30, 4.92, 3.21, 2.04, 1.25, 0.75, 0.43, 0.24, 0.13 };
            if (l < 0) l = 0;
            if (l >= prb.Length) l = prb.Length - 1;
            return "This level is in the top " + prb[l].ToString("N2") + "% of what you can find.";
        }

        public static string statName(int s)
        {
            if (s >= 0 && s < 8)
            {
                string[] statNames = new string[] { "Health", "Stamina", "Oxygen", "Food", "Weight", "Damage", "Speed", "Torpor" };
                return statNames[s];
            }
            return "";
        }

        public static int precision(int s)
        {
            int p = 1;
            if (s == 5 || s == 6)
            {
                p = 3; // damage and speed are percentagevalues, need more precision
            }
            return p;
        }

        public static string creatureColorName(int colorId)
        {
            if (colorId > 0 && colorId < 42)
            {
                string[] cn = new string[] { "Red", "Blue", "Green", "Yellow", "Cyan", "Magenta", "Light Green", "Light Grey", "Light Brown", "Light Orange", "Light Yellow", "Light Red", "Dark Grey", "Black", "Brown", "Dark Green", "Dark Red", "White", "Dino Light Red", "Dino Dark Red", "Dino Light Orange", "Dino Dark Orange", "Dino Light Yellow", "Dino Dark Yellow", "Dino Light Green", "Dino Medium Green", "Dino Dark Green", "Dino Light Blue", "Dino Dark Blue", "Dino Light Purple", "Dino Dark Purple", "Dino Light Brown", "Dino Medium Brown", "Dino Dark Brown", "Dino Darker Grey", "Dino Albino", "BigFoot0", "BigFoot4", "BigFoot5", "Wolf Fur", "Dark Wolf Fur" };
                return cn[colorId - 1];
            }
            return "unknown";
        }

        public static Color creatureColor(int colorId)
        {
            Color color = Color.FromArgb(0, 0, 0);

            if (colorId >= 0 && colorId < 42)
            {
                Color[] creatureColors = new Color[] {
                    Color.Gray, // 0: unknown
                    Color.FromArgb(255,0,0), // 1: Red
                    Color.FromArgb(0, 0, 255),
                    Color.FromArgb(0, 255, 0),
                    Color.FromArgb(255, 255, 0),
                    Color.FromArgb(0, 255, 255),
                    Color.FromArgb(255, 0, 255),
                    Color.FromArgb(192, 255, 186), // Light Green
                    Color.FromArgb(200, 202, 202), // Light Grey
                    Color.FromArgb(119, 102, 89), // Light Brown
                    Color.FromArgb(255, 180, 107), // Light Orange
                    Color.FromArgb(255, 250, 138),
                    Color.FromArgb(255, 117, 107),
                    Color.FromArgb(123, 123, 123),
                    Color.FromArgb(59, 59, 59),
                    Color.FromArgb(89, 56, 39),
                    Color.FromArgb(34, 73, 0),
                    Color.FromArgb(129, 28, 20), // Dark Red
                    Color.FromArgb(255, 255, 255),
                    Color.FromArgb(255, 168, 168),
                    Color.FromArgb(89, 43, 43), // Dino Dark Red
                    Color.FromArgb(255, 182, 148),
                    Color.FromArgb(136, 82, 46),
                    Color.FromArgb(202, 202, 159),
                    Color.FromArgb(148, 148, 107),
                    Color.FromArgb(224, 255, 224), // Dino Light Green
                    Color.FromArgb(120, 148, 120),
                    Color.FromArgb(34, 64, 34),
                    Color.FromArgb(217, 224, 255),
                    Color.FromArgb(56, 66, 98),
                    Color.FromArgb(228, 217, 255),
                    Color.FromArgb(64, 50, 89), // 31: Dino Dark Purple
                    Color.FromArgb(255, 224, 186),
                    Color.FromArgb(148, 133, 117),
                    Color.FromArgb(89, 77, 64),
                    Color.FromArgb(89, 89, 89),
                    Color.FromArgb(255, 255, 255), // 36: Dino Albino
                    Color.FromArgb(183, 150, 131),
                    Color.FromArgb(234, 218, 213),
                    Color.FromArgb(208, 166, 148),
                    Color.FromArgb(195, 179, 159),
                    Color.FromArgb(136, 117, 102)};
                color = creatureColors[colorId];
            }
            return color;
        }
    }
}
