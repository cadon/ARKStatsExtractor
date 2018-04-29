using System;
using System.Drawing;
using System.Linq;

namespace ARKBreedingStats.species
{
    static class CreatureColors
    {
        public static string creatureColorName(int colorId)
        {
            if (colorId > 0 && colorId < colorNames.Length)
            {
                string[] cn = new string[] { "Red", "Blue", "Green", "Yellow", "Cyan", "Magenta", "Light Green", "Light Grey", "Light Brown", "Light Orange", "Light Yellow", "Light Red", "Dark Grey", "Black", "Brown", "Dark Green", "Dark Red", "White", "Dino Light Red", "Dino Dark Red", "Dino Light Orange", "Dino Dark Orange", "Dino Light Yellow", "Dino Dark Yellow", "Dino Light Green", "Dino Medium Green", "Dino Dark Green", "Dino Light Blue", "Dino Dark Blue", "Dino Light Purple", "Dino Dark Purple", "Dino Light Brown", "Dino Medium Brown", "Dino Dark Brown", "Dino Darker Grey", "Dino Albino", "BigFoot0", "BigFoot4", "BigFoot5", "WolfFur", "DarkWolfFur", "DragonBase0", "DragonBase1", "DragonFire", "DragonGreen0", "DragonGreen1", "DragonGreen2", "DragonGreen3", "WyvernPurple0", "WyvernPurple1", "WyvernBlue0", "WyvernBlue1", "Dino Medium Blue", "Dino Deep Blue", "NearWhite", "NearBlack" };
                return cn[colorId - 1];
            }
            return "unknown";
        }
        public static Color creatureColor(int colorId)
        {
            Color color = Color.FromArgb(0, 0, 0);

            if (colorId >= 0 && colorId < creatureColors.Length)
            {
                color = creatureColors[colorId];
            }
            return color;
        }

        public static int closestColorIDFromRGB(int r, int g, int b)
        {
            Color closestColor = creatureColors.OrderBy(n => ColorDiff(n, r, g, b)).First();
            return Array.IndexOf(creatureColors, closestColor);
        }

        // distance in RGB space
        private static int ColorDiff(Color c1, int r, int g, int b)
        {
            return (int)Math.Sqrt((c1.R - r) * (c1.R - r)
                                   + (c1.G - g) * (c1.G - g)
                                   + (c1.B - b) * (c1.B - b));
        }

        static private string[] colorNames = new string[] { "Red", "Blue", "Green", "Yellow", "Cyan", "Magenta", "Light Green", "Light Grey", "Light Brown", "Light Orange", "Light Yellow", "Light Red", "Dark Grey", "Black", "Brown", "Dark Green", "Dark Red", "White", "Dino Light Red", "Dino Dark Red", "Dino Light Orange", "Dino Dark Orange", "Dino Light Yellow", "Dino Dark Yellow", "Dino Light Green", "Dino Medium Green", "Dino Dark Green", "Dino Light Blue", "Dino Dark Blue", "Dino Light Purple", "Dino Dark Purple", "Dino Light Brown", "Dino Medium Brown", "Dino Dark Brown", "Dino Darker Grey", "Dino Albino", "BigFoot0", "BigFoot4", "BigFoot5", "WolfFur", "DarkWolfFur", "DragonBase0", "DragonBase1", "DragonFire", "DragonGreen0", "DragonGreen1", "DragonGreen2", "DragonGreen3", "WyvernPurple0", "WyvernPurple1", "WyvernBlue0", "WyvernBlue1", "Dino Medium Blue", "Dino Deep Blue", "NearWhite", "NearBlack" };
        static private Color[] creatureColors = new Color[] {
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
    }
}
