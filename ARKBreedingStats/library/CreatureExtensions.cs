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
        /// Width of the infographic in pixels.
        /// </summary>
        private const int width = 300;
        /// <summary>
        /// Height of the infographic in pixels.
        /// </summary>
        private const int height = 180;

        /// <summary>
        /// Creates an image with infos about the creature.
        /// </summary>
        /// <param name="creature"></param>
        /// <returns></returns>
        public static Bitmap InfoGraphic(this Creature creature)
        {
            if (creature == null) return null;


            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            using (var font = new Font("Arial", 10))
            using (var fontSmall = new Font("Arial", 8))
            using (var fontHeader = new Font("Arial", 12, FontStyle.Bold))
            using (var fontBrush = new SolidBrush(Color.Black))
            using (var stringFormatRight = new StringFormat() { Alignment = StringAlignment.Far })
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                int currentYPosition = 3;

                using (var backgroundBrush = new SolidBrush(Color.AntiqueWhite))
                    g.FillRectangle(backgroundBrush, 0, 0, width, height);
                g.DrawString($"{creature.Species.DescriptiveNameAndMod} (Lvl {creature.LevelHatched})", fontHeader, fontBrush, 3, currentYPosition);
                currentYPosition += 19;
                g.DrawString($"{Utils.sexSymbol(creature.sex) + (creature.flags.HasFlag(CreatureFlags.Neutered) ? $" ({Loc.s(creature.sex == Sex.Female ? "Spayed" : "Neutered")})" : string.Empty)} | {creature.Mutations} mutations", font, fontBrush, 8, currentYPosition);
                currentYPosition += 17;

                using (var p = new Pen(Color.LightGray, 1))
                    g.DrawLine(p, 0, currentYPosition, width, currentYPosition);
                currentYPosition += 2;

                // levels
                const int xStatName = 8;
                int xLevelValue = xStatName + 30 + (creature.levelsWild[2].ToString().Length) * 7;
                g.DrawString("Levels", font, fontBrush, xStatName, currentYPosition);
                int statDisplayIndex = 0;
                for (int si = 0; si < Values.STATS_COUNT; si++)
                {
                    int statIndex = Values.statsDisplayOrder[si];
                    if (statIndex != (int)StatNames.Torpidity && creature.Species.UsesStat(statIndex))
                    {
                        int y = currentYPosition + 20 + (statDisplayIndex++) * 14;
                        g.DrawString($"{Utils.statName(statIndex, true, creature.Species.IsGlowSpecies)}",
                            font, fontBrush, xStatName, y);
                        g.DrawString($"{creature.levelsWild[statIndex]}",
                            font, fontBrush, xLevelValue, y, stringFormatRight);
                    }
                }

                // colors
                const int maxColorNameLength = 38;
                int xColor = xLevelValue + 20;
                g.DrawString("Colors", font, fontBrush, xColor, currentYPosition);
                int colorIndex = 0;
                for (int ci = 0; ci < Species.COLOR_REGION_COUNT; ci++)
                {
                    if (!string.IsNullOrEmpty(creature.Species.colors[ci]?.name))
                    {
                        const int circleDiameter = 16;
                        const int rowHeight = circleDiameter + 2;
                        int y = currentYPosition + 20 + (colorIndex++) * rowHeight;

                        Color c = CreatureColors.creatureColor(creature.colors[ci]);
                        Color fc = Utils.ForeColor(c);

                        using (var b = new SolidBrush(c))
                            g.FillEllipse(b, xColor, y, circleDiameter, circleDiameter);
                        using (var p = new Pen(Color.Black, 1))
                            g.DrawEllipse(p, xColor, y, circleDiameter, circleDiameter);

                        string colorRegionName = creature.Species.colors[ci].name;
                        string colorName = CreatureColors.creatureColorName(creature.colors[ci]);

                        int totalColorLenght = colorRegionName.Length + colorName.Length + 9;
                        if (totalColorLenght > maxColorNameLength)
                        {
                            // shorten color region name
                            int lengthForRegionName = colorRegionName.Length - (totalColorLenght - maxColorNameLength);
                            colorRegionName = lengthForRegionName <= 0
                                ? string.Empty
                                : lengthForRegionName < colorRegionName.Length
                                ? colorRegionName.Substring(0, lengthForRegionName)
                                : colorRegionName;
                        }

                        g.DrawString($"[{ci}] {colorRegionName}: [{creature.colors[ci]}] {colorName}",
                            fontSmall, fontBrush, xColor + circleDiameter + 4, y);
                    }
                }
            }

            return bmp;
        }
    }
}
