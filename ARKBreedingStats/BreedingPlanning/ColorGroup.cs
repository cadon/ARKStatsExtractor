using System.Collections.Generic;
using System.Drawing;

namespace ARKBreedingStats.BreedingPlanning
{
    /// <summary>
    /// Represents a group of colors, e.g. different shades of green.
    /// </summary>
    public class ColorGroup
    {
        public string Name { get; set; }

        public byte[] ColorIds { get; set; }

        public ColorGroup(string name, byte[] colorIds)
        {
            Name = name;
            ColorIds = colorIds;
        }

        public ColorGroup(string name, Color baseColor, double deltaHue = 0, double deltaLightness = 0,
            double deltaSaturation = 0)
        {
            Name = name;
            var hueMin = (baseColor.GetHue() - deltaHue + 360) % 360;
            var hueMax = (hueMin + 2 * deltaHue + 360) % 360;
            var hueMax2 = -1d;
            if (hueMin > hueMax)
            {
                hueMax2 = hueMax;
                hueMax = 360;
            }

            var lightnessMin = baseColor.GetBrightness() - deltaLightness;
            var lightnessMax = lightnessMin + 2 * deltaLightness;
            var saturationMin = baseColor.GetSaturation() - deltaSaturation;
            var saturationMax = saturationMin + 2 * deltaSaturation;
            var colorIds = new List<byte>();
            foreach (var c in values.Values.V.Colors.ColorsList)
            {
                var h = c.Color.GetHue();
                var l = c.Color.GetBrightness();
                var s = c.Color.GetSaturation();
                if (((h >= hueMin && h <= hueMax) || h <= hueMax2)
                   && l >= lightnessMin && l <= lightnessMax
                   && s >= saturationMin && s <= saturationMax)
                    colorIds.Add(c.Id);
            }

            ColorIds = colorIds.ToArray();
        }
    }
}
