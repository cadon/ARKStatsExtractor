using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Globalization;

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
            int r, g, b;
            getRGBFromPercent(out r, out g, out b, percent, light);
            return Color.FromArgb(r, g, b);
        }

        public static string getARKmlFromPercent(string text, int percent, double light = 0)
        {
            int r, g, b;
            getRGBFromPercent(out r, out g, out b, percent, light);
            return getARKml(text, r, g, b);
        }

        public static string getARKml(string text, int r, int g, int b)
        {
            return "<RichColor Color=\"" + Math.Round(r / 255d, 2).ToString(CultureInfo.InvariantCulture) + "," + Math.Round(g / 255d, 2).ToString(CultureInfo.InvariantCulture) + "," + Math.Round(b / 255d, 2).ToString(CultureInfo.InvariantCulture) + ",1\">" + text + "</>";
        }

        private static void getRGBFromPercent(out int r, out int g, out int b, int percent, double light = 0)
        {
            if (light > 1) { light = 1; }
            if (light < -1) { light = -1; }
            r = 511 - percent * 255 / 50;
            g = percent * 255 / 50;
            b = 0;
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
                //b = (int)(b * light); // b == 0 always
            }
        }

        public static string sexSymbol(Sex g)
        {
            switch (g)
            {
                case Sex.Male:
                    return "♂";
                case Sex.Female:
                    return "♀";
                default:
                    return "?";
            }
        }

        public static Color sexColor(Sex g)
        {
            switch (g)
            {
                case Sex.Male:
                    return Color.FromArgb(220, 235, 255);
                case Sex.Female:
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

        public static Sex nextSex(Sex s)
        {
            switch (s)
            {
                case Sex.Female:
                    return Sex.Male;
                case Sex.Male:
                    return Sex.Unknown;
                default:
                    return Sex.Female;
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

        public static string statName(int s, bool abr = false)
        {
            if (s >= 0 && s < 8)
            {
                string[] statNames;
                if (abr) statNames = new string[] { "HP", "St", "Ox", "Fo", "We", "Dm", "Sp", "To" };
                else statNames = new string[] { "Health", "Stamina", "Oxygen", "Food", "Weight", "Damage", "Speed", "Torpor" };
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
            if (colorId > 0 && colorId < 57)
            {
                string[] cn = new string[] { "Red", "Blue", "Green", "Yellow", "Cyan", "Magenta", "Light Green", "Light Grey", "Light Brown", "Light Orange", "Light Yellow", "Light Red", "Dark Grey", "Black", "Brown", "Dark Green", "Dark Red", "White", "Dino Light Red", "Dino Dark Red", "Dino Light Orange", "Dino Dark Orange", "Dino Light Yellow", "Dino Dark Yellow", "Dino Light Green", "Dino Medium Green", "Dino Dark Green", "Dino Light Blue", "Dino Dark Blue", "Dino Light Purple", "Dino Dark Purple", "Dino Light Brown", "Dino Medium Brown", "Dino Dark Brown", "Dino Darker Grey", "Dino Albino", "BigFoot0", "BigFoot4", "BigFoot5", "WolfFur", "DarkWolfFur", "DragonBase0", "DragonBase1", "DragonFire", "DragonGreen0", "DragonGreen1", "DragonGreen2", "DragonGreen3", "WyvernPurple0", "WyvernPurple1", "WyvernBlue0", "WyvernBlue1", "Dino Medium Blue", "Dino Deep Blue", "NearWhite", "NearBlack" };
                return cn[colorId - 1];
            }
            return "unknown";
        }

        public static string duration(TimeSpan ts)
        {
            return ts.ToString("d':'hh':'mm':'ss");
        }

        public static string duration(int seconds)
        {
            return duration(new TimeSpan(0, 0, seconds));
        }

        public static string durationUntil(TimeSpan ts)
        {
            return ts.ToString("d':'hh':'mm':'ss") + " (until: " + shortTimeDate(DateTime.Now.Add(ts)) + ")";
        }

        public static string shortTimeDate(DateTime dt)
        {
            return dt.ToShortTimeString() + (DateTime.Today == dt.Date ? "" : " - " + dt.ToShortDateString());
        }

        public static string timeLeft(DateTime dt)
        {
            if (dt < DateTime.Now)
                return "-";
            return duration(dt.Subtract(DateTime.Now));
        }

        public static Color creatureColor(int colorId)
        {
            Color color = Color.FromArgb(0, 0, 0);

            if (colorId >= 0 && colorId < 57)
            {
                Color[] creatureColors = new Color[] {
Color.Gray, // 0: unknown
Color.FromArgb(255,0,0),
Color.FromArgb(0,0,255),
Color.FromArgb(0,255,0),
Color.FromArgb(255,255,0),
Color.FromArgb(0,255,255),
Color.FromArgb(255,0,255),
Color.FromArgb(192,255,186),
Color.FromArgb(200,202,202),
Color.FromArgb(120,103,89),
Color.FromArgb(255,180,108),
Color.FromArgb(255,250,138),
Color.FromArgb(255,117,108),
Color.FromArgb(123,123,123),
Color.FromArgb(59,59,59),
Color.FromArgb(89,58,42),
Color.FromArgb(34,73,0),
Color.FromArgb(129,33,24),
Color.FromArgb(255,255,255),
Color.FromArgb(255,168,168),
Color.FromArgb(89,43,43),
Color.FromArgb(255,182,148),
Color.FromArgb(136,83,47),
Color.FromArgb(202,202,160),
Color.FromArgb(148,148,108),
Color.FromArgb(224,255,224),
Color.FromArgb(121,148,121),
Color.FromArgb(34,65,34),
Color.FromArgb(217,224,255),
Color.FromArgb(57,66,99),
Color.FromArgb(228,217,255),
Color.FromArgb(64,52,89),
Color.FromArgb(255,224,186),
Color.FromArgb(148,133,117),
Color.FromArgb(89,78,65),
Color.FromArgb(89,89,89),
Color.FromArgb(255,255,255),
Color.FromArgb(183,150,131),
Color.FromArgb(234,218,213),
Color.FromArgb(208,167,148),
Color.FromArgb(195,179,159),
Color.FromArgb(136,118,102),
Color.FromArgb(160,102,75),
Color.FromArgb(203,121,86),
Color.FromArgb(188,79,0),
Color.FromArgb(121,132,108),
Color.FromArgb(144,156,121),
Color.FromArgb(165,164,139),
Color.FromArgb(116,147,156),
Color.FromArgb(120,116,150),
Color.FromArgb(176,162,192),
Color.FromArgb(98,129,167),
Color.FromArgb(72,92,117),
Color.FromArgb(95,164,234),
Color.FromArgb(69,104,212),
Color.FromArgb(237,237,237),
Color.FromArgb(81,81,81)
                };
                color = creatureColors[colorId];
            }
            return color;
        }
    }
}
