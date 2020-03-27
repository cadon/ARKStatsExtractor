using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Drawing;

namespace ARKBreedingStats.library
{
    public static class CreatureExtensions
    {
        /// <summary>
        /// Creates an image with infos about the creature.
        /// </summary>
        /// <param name="creature"></param>
        /// <param name="cc">CreatureCollection for server settings.</param>
        /// <returns></returns>
        public static Bitmap InfoGraphic(this Creature creature, CreatureCollection cc)
        {
            if (creature == null) return null;
            int maxGraphLevel = cc?.maxChartLevel ?? 0;
            if (maxGraphLevel < 1) maxGraphLevel = 50;

            const int width = 300;
            const int height = 180;

            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            using (var font = new Font("Arial", 10))
            using (var fontSmall = new Font("Arial", 8))
            using (var fontHeader = new Font("Arial", 12, FontStyle.Bold))
            using (var fontBrush = new SolidBrush(Color.Black))
            using (var penBlack = new Pen(Color.Black, 1))
            using (var stringFormatRight = new StringFormat() { Alignment = StringAlignment.Far })
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                int currentYPosition = 3;

                using (var backgroundBrush = new SolidBrush(Color.AntiqueWhite))
                    g.FillRectangle(backgroundBrush, 0, 0, width, height);

                g.DrawString(creature.Species.DescriptiveNameAndMod, fontHeader, fontBrush, 3, currentYPosition);
                currentYPosition += 19;
                g.DrawString($"Lvl {creature.LevelHatched} | {Utils.sexSymbol(creature.sex) + (creature.flags.HasFlag(CreatureFlags.Neutered) ? $" ({Loc.s(creature.sex == Sex.Female ? "Spayed" : "Neutered")})" : string.Empty)} | {creature.Mutations} mutations", font, fontBrush, 8, currentYPosition);
                currentYPosition += 17;

                using (var p = new Pen(Color.LightGray, 1))
                    g.DrawLine(p, 0, currentYPosition, width, currentYPosition);
                currentYPosition += 2;

                // levels
                const int xStatName = 8;
                int xLevelValue = xStatName + 30 + (creature.levelsWild[2].ToString().Length) * 7;
                int maxBoxLenght = xLevelValue - xStatName;
                g.DrawString("Levels", font, fontBrush, xStatName, currentYPosition);
                int statDisplayIndex = 0;
                for (int si = 0; si < Values.STATS_COUNT; si++)
                {
                    int statIndex = Values.statsDisplayOrder[si];
                    if (statIndex == (int)StatNames.Torpidity || !creature.Species.UsesStat(statIndex))
                        continue;

                    int y = currentYPosition + 20 + (statDisplayIndex++) * 15;
                    // box
                    double levelFractionOfMax = Math.Min(1, (double)creature.levelsWild[statIndex] / maxGraphLevel);
                    if (levelFractionOfMax < 0) levelFractionOfMax = 0;
                    int levelPercentageOfMax = (int)(100 * levelFractionOfMax);
                    int statBoxLength = Math.Max((int)(maxBoxLenght * levelFractionOfMax), 1);
                    const int statBoxHeight = 2;
                    var statColor = Utils.getColorFromPercent(levelPercentageOfMax);
                    using (var b = new SolidBrush(statColor))
                        g.FillRectangle(b, xStatName, y + 14, statBoxLength, statBoxHeight);
                    using (var b = new SolidBrush(Color.FromArgb(10, statColor)))
                    {
                        for (int r = 4; r > 0; r--)
                            g.FillRectangle(b, xStatName - r, y + 13 - r, statBoxLength + 2 * r, statBoxHeight + 2 * r);
                    }
                    using (var p = new Pen(Utils.getColorFromPercent(levelPercentageOfMax, -0.5), 1))
                        g.DrawRectangle(p, xStatName, y + 14, statBoxLength, statBoxHeight);

                    // stat name
                    g.DrawString($"{Utils.statName(statIndex, true, creature.Species.IsGlowSpecies)}",
                        font, fontBrush, xStatName, y);
                    // stat level number
                    g.DrawString($"{creature.levelsWild[statIndex]}",
                        font, fontBrush, xLevelValue, y, stringFormatRight);
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
                        g.DrawEllipse(penBlack, xColor, y, circleDiameter, circleDiameter);

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

                // max wild level on server
                if (cc != null)
                {
                    g.DrawString($"max wild level: {cc.maxWildLevel}",
                        fontSmall, fontBrush, width - 4, height - 14, stringFormatRight);
                }

                // frame
                using (var p = new Pen(Color.DarkRed, 1))
                    g.DrawRectangle(p, 0, 0, width - 1, height - 1);
            }

            return bmp;
        }

        /// <summary>
        /// Creates infographic and copies it to the clipboard.
        /// </summary>
        /// <param name="creature"></param>
        /// <param name="cc">CreatureCollection for server settings.</param>
        public static void ExportInfoGraphicToClipboard(this Creature creature, CreatureCollection cc)
        {
            if (creature == null) return;

            using (var bmp = creature.InfoGraphic(cc))
            {
                if (bmp != null)
                    System.Windows.Forms.Clipboard.SetImage(bmp);
            }
        }
    }
}
