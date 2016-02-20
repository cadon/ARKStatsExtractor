using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats
{
    static class Utils
    {
        // calculates value for given levels // TODO: where to save stats?
        //public static double calculateValue(int creature, int stat, int levelWild, int levelDom, bool dom, double tamingEff)
        //{
        //    double add = 0, domMult = 1;
        //    if (dom)
        //    {
        //        add = stats[cC][stat].AddWhenTamed;
        //        domMult = (tamingEff >= 0 ? (1 + stats[creature][stat].MultAffinity) : 1) * (1 + levelDom * stats[creature][stat].IncPerTamedLevel);
        //    }
        //    return Math.Round((stats[creature][stat].BaseValue * (1 + stats[creature][stat].IncPerWildLevel * levelWild) + add) * domMult, precisions[stat], MidpointRounding.AwayFromZero);
        //}

        // returns a shade between red over yellow and green, corresponding to the value p (0-100). If light is given, the brightness is adjusted (-1 to 1)
        public static System.Drawing.Color getColorFromPercent(int percent, double light = 0)
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
                b = (int)(b * light);
            }
            return System.Drawing.Color.FromArgb(r, g, b);
        }
    }
}
