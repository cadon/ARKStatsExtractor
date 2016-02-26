using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                //b = (int)(b * light); // b == 0 anyway
            }
            return System.Drawing.Color.FromArgb(r, g, b);
        }
    }
}
