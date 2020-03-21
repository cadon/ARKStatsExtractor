using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats.library
{
    public static class CreatureExtensions
    {
        /// <summary>
        /// Creates an image with infos about the creature.
        /// </summary>
        /// <param name="creature"></param>
        /// <returns></returns>
        public static Bitmap InfoGraphic(this Creature creature)
        {
            if (creature == null) return null;

            const int width = 300;
            const int height = 150;

            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            using (var font = new Font("Arial", 10))
            using (var fontSmall = new Font("Arial", 8))
            using (var fontHeader = new Font("Arial", 12, FontStyle.Bold))
            using (var fontBrush = new SolidBrush(Color.Black))
            using (var stringFormatRight = new StringFormat() { Alignment = StringAlignment.Far })
            {
                using (var backgroundBrush = new SolidBrush(Color.AntiqueWhite))
                    g.FillRectangle(backgroundBrush, 0, 0, width, height);
                g.DrawString($"{creature.Species.DescriptiveNameAndMod} (Lvl {creature.LevelHatched})", fontHeader, fontBrush, 3, 3);

                const int yBelowHeader = 20;
                // levels
                const int xStatName = 8;
                int xLevelValue = xStatName + 30 + (creature.levelsWild[2].ToString().Length) * 7;
                g.DrawString("Levels", font, fontBrush, xStatName, yBelowHeader);
                int statDisplayIndex = 0;
                for (int si = 0; si < Values.STATS_COUNT; si++)
                {
                    int statIndex = Values.statsDisplayOrder[si];
                    if (creature.Species.UsesStat(statIndex))
                    {
                        int y = yBelowHeader + 20 + (statDisplayIndex++) * 13;
                        g.DrawString($"{Utils.statName(statIndex, true, creature.Species.IsGlowSpecies)}",
                            font, fontBrush, xStatName, y);
                        g.DrawString($"{creature.levelsWild[statIndex]}",
                            font, fontBrush, xLevelValue, y, stringFormatRight);
                    }
                }

                // colors
                int xColor = xLevelValue + 20;
                g.DrawString("Colors", font, fontBrush, xColor, yBelowHeader);
                int colorIndex = 0;
                for (int ci = 0; ci < Species.COLOR_REGION_COUNT; ci++)
                {
                    if (!string.IsNullOrEmpty(creature.Species.colors[ci]?.name))
                    {
                        const int circleDiameter = 16;
                        const int rowHeight = circleDiameter + 2;
                        int y = yBelowHeader + 20 + (colorIndex++) * rowHeight;

                        Color c = CreatureColors.creatureColor(creature.colors[ci]);
                        Color fc = Utils.ForeColor(c);

                        using (var b = new SolidBrush(c))
                            g.FillEllipse(b, xColor, y, circleDiameter, circleDiameter);
                        using (var p = new Pen(Color.Black, 1))
                            g.DrawEllipse(p, xColor, y, circleDiameter, circleDiameter);

                        g.DrawString($"[{ci}] {creature.Species.colors[ci].name}: [{creature.colors[ci]}] {CreatureColors.creatureColorName(creature.colors[ci])}",
                            fontSmall, fontBrush, xColor + circleDiameter + 4, y);
                    }
                }
            }

            return bmp;
        }
    }
}
