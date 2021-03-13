using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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

            int width = Properties.Settings.Default.InfoGraphicWidth; // 330
            int height = width * 6 / 11; //180

            int fontSize = Math.Max(1, height / 18); // 10
            int fontSizeSmall = Math.Max(1, height * 2 / 45); // 8
            int fontSizeHeader = Math.Max(1, height / 15); // 12
            int frameThickness = Math.Max(1, height / 180);

            int statLineHeight = height * 5 / 59; // 15

            var fontName = Properties.Settings.Default.InfoGraphicFontName;
            if (string.IsNullOrWhiteSpace(fontName))
            {
                fontName = "Arial";
                Properties.Settings.Default.InfoGraphicFontName = fontName;
            }

            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            using (var font = new Font(fontName, fontSize))
            using (var fontSmall = new Font(fontName, fontSizeSmall))
            using (var fontHeader = new Font(fontName, fontSizeHeader, FontStyle.Bold))
            using (var fontBrush = new SolidBrush(Properties.Settings.Default.InfoGraphicForeColor))
            using (var borderAroundColors = new Pen(Utils.ForeColor(Properties.Settings.Default.InfoGraphicBackColor), 1))
            using (var stringFormatRight = new StringFormat { Alignment = StringAlignment.Far })
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                int currentYPosition = frameThickness * 3;

                using (var backgroundBrush = new SolidBrush(Properties.Settings.Default.InfoGraphicBackColor))
                    g.FillRectangle(backgroundBrush, 0, 0, width, height);

                g.DrawString(creature.Species.DescriptiveNameAndMod, fontHeader, fontBrush, 3, currentYPosition);
                currentYPosition += height * 19 / 180; //19
                string creatureInfos = $"{Loc.S("Level")} {creature.LevelHatched} | {Utils.SexSymbol(creature.sex) + (creature.flags.HasFlag(CreatureFlags.Neutered) ? $" ({Loc.S(creature.sex == Sex.Female ? "Spayed" : "Neutered")})" : string.Empty)}";
                if (Properties.Settings.Default.InfoGraphicDisplayMutations)
                    creatureInfos += $" | {creature.Mutations} {Loc.S("Mutations")}";
                if (Properties.Settings.Default.InfoGraphicDisplayGeneration)
                    creatureInfos += $" | {Loc.S("generation")} {creature.generation}";
                g.DrawString(creatureInfos, font, fontBrush, width * 4 / 165, currentYPosition);
                currentYPosition += height * 17 / 180; //17

                using (var p = new Pen(Color.FromArgb(50, Properties.Settings.Default.InfoGraphicForeColor), 1))
                    g.DrawLine(p, 0, currentYPosition, width, currentYPosition);
                currentYPosition += 2;

                // levels
                double meanLetterWidth = fontSize * 7d / 10;
                int xStatName = (int)meanLetterWidth;
                // x position of level number. torpor is the largest level number.
                bool showDomLevel = Properties.Settings.Default.InfoGraphicWithDomLevels;
                int xRightLevelValue = (int)(xStatName + ((showDomLevel ? 6 : 5) + creature.levelsWild[2].ToString().Length) * meanLetterWidth);
                int xRightLevelDomValue = xRightLevelValue;
                if (showDomLevel)
                    xRightLevelDomValue += (int)((creature.levelsDom.Max().ToString().Length) * meanLetterWidth);
                int xRightBrValue = (int)(xRightLevelDomValue + (2 + MaxCharLength(creature.valuesBreeding)) * meanLetterWidth);
                int maxBoxLength = xRightBrValue - xStatName;
                int statBoxHeight = Math.Max(2, height / 90);
                g.DrawString(Loc.S("Levels"), font, fontBrush, xRightLevelDomValue, currentYPosition, stringFormatRight);
                g.DrawString(Loc.S("Values"), font, fontBrush, xRightBrValue, currentYPosition, stringFormatRight);
                int statDisplayIndex = 0;
                for (int si = 0; si < Values.STATS_COUNT; si++)
                {
                    int statIndex = Values.statsDisplayOrder[si];
                    if (statIndex == (int)StatNames.Torpidity || !creature.Species.UsesStat(statIndex))
                        continue;

                    int y = currentYPosition + (height / 9) + (statDisplayIndex++) * statLineHeight;

                    // box
                    // empty box to show the max possible length
                    using (var b = new SolidBrush(Color.DarkGray))
                        g.FillRectangle(b, xStatName, y + statLineHeight - 1, maxBoxLength, statBoxHeight);
                    double levelFractionOfMax = Math.Min(1, (double)creature.levelsWild[statIndex] / maxGraphLevel);
                    if (levelFractionOfMax < 0) levelFractionOfMax = 0;
                    int levelPercentageOfMax = (int)(100 * levelFractionOfMax);
                    int statBoxLength = Math.Max((int)(maxBoxLength * levelFractionOfMax), 1);
                    var statColor = Utils.GetColorFromPercent(levelPercentageOfMax);
                    using (var b = new SolidBrush(statColor))
                        g.FillRectangle(b, xStatName, y + statLineHeight - 1, statBoxLength, statBoxHeight);
                    using (var b = new SolidBrush(Color.FromArgb(10, statColor)))
                    {
                        for (int r = 4; r > 0; r--)
                            g.FillRectangle(b, xStatName - r, y + statLineHeight - 2 - r, statBoxLength + 2 * r, statBoxHeight + 2 * r);
                    }
                    using (var p = new Pen(Utils.GetColorFromPercent(levelPercentageOfMax, -0.5), 1))
                        g.DrawRectangle(p, xStatName, y + statLineHeight - 1, statBoxLength, statBoxHeight);

                    // stat name
                    g.DrawString($"{Utils.StatName(statIndex, true, creature.Species.statNames)}",
                        font, fontBrush, xStatName, y);
                    // stat level number
                    g.DrawString($"{creature.levelsWild[statIndex]}{(showDomLevel ? " +" : string.Empty)}",
                        font, fontBrush, xRightLevelValue, y, stringFormatRight);
                    // dom level number
                    if (showDomLevel)
                        g.DrawString($"{creature.levelsDom[statIndex]}",
                            font, fontBrush, xRightLevelDomValue, y, stringFormatRight);
                    // stat breeding value
                    double displayedValue = showDomLevel ? creature.valuesDom[statIndex] : creature.valuesBreeding[statIndex];
                    string statValueRepresentation;
                    if (Utils.Precision(statIndex) == 3)
                    {
                        statValueRepresentation = (100 * displayedValue).ToString("0.0");
                        g.DrawString("%", font, fontBrush, xRightBrValue, y);
                    }
                    else
                        statValueRepresentation = displayedValue.ToString("0.0");
                    g.DrawString(statValueRepresentation, font, fontBrush, xRightBrValue, y, stringFormatRight);
                }

                // colors
                var enabledColorRegions = creature.Species.EnabledColorRegions;

                int xColor = (int)(xRightBrValue + meanLetterWidth * 3.5);
                int circleDiameter = height * 4 / 45;
                int colorRowHeight = circleDiameter + 2;
                int maxColorNameLength = (int)((width - xColor - circleDiameter) / meanLetterWidth); // max char length for the color region name
                if (maxColorNameLength < 0) maxColorNameLength = 0;

                bool creatureImageShown = false;
                bool displayMaxWild = Properties.Settings.Default.InfoGraphicShowMaxWildLevel;
                int extraMarginBottom = displayMaxWild ? fontSizeSmall : 0;
                int imageSize = (int)Math.Min(width - xColor - circleDiameter - 8 * meanLetterWidth - frameThickness * 4,
                                              height - currentYPosition - frameThickness * 4 - extraMarginBottom);
                if (imageSize > 5)
                {
                    using (var crBmp =
                        CreatureColored.GetColoredCreature(creature.colors, creature.Species, enabledColorRegions,
                            imageSize, onlyImage: true, creatureSex: creature.sex))
                    {
                        if (crBmp != null)
                        {
                            g.DrawImage(crBmp, width - imageSize - frameThickness * 4,
                                height - imageSize - frameThickness * 4 - extraMarginBottom, imageSize, imageSize);
                            creatureImageShown = true;
                        }
                    }
                }

                if (creature.colors != null)
                {
                    g.DrawString(Loc.S("Colors"), font, fontBrush, xColor, currentYPosition);
                    int colorRow = 0;
                    for (int ci = 0; ci < Species.ColorRegionCount; ci++)
                    {
                        if (!enabledColorRegions[ci])
                            continue;

                        int y = currentYPosition + (height / 9) + (colorRow++) * colorRowHeight;

                        Color c = CreatureColors.CreatureColor(creature.colors[ci]);
                        //Color fc = Utils.ForeColor(c);

                        using (var b = new SolidBrush(c))
                            g.FillEllipse(b, xColor, y, circleDiameter, circleDiameter);
                        g.DrawEllipse(borderAroundColors, xColor, y, circleDiameter, circleDiameter);

                        string colorRegionName = null;
                        //string colorName = CreatureColors.CreatureColorName(creature.colors[ci]);

                        if (!creatureImageShown)
                        {
                            colorRegionName = creature.Species.colors[ci].name;
                            int totalColorLength = colorRegionName.Length + 11;
                            if (totalColorLength > maxColorNameLength)
                            {
                                // shorten color region name
                                int lengthForRegionName =
                                    colorRegionName.Length - (totalColorLength - maxColorNameLength);
                                colorRegionName = lengthForRegionName < 2
                                    ? string.Empty
                                    : colorRegionName.Substring(0, lengthForRegionName - 1) + "…";
                            }

                            if (!string.IsNullOrEmpty(colorRegionName))
                                colorRegionName = " (" + colorRegionName + ")";
                        }

                        g.DrawString($"{creature.colors[ci]} - [{ci}]{colorRegionName}",
                            fontSmall, fontBrush, xColor + circleDiameter + 4, y);
                    }
                }

                // imprinting
                if (showDomLevel)
                {
                    g.DrawString($"Imp: {creature.imprintingBonus * 100:0.0} %", font, fontBrush, xColor + (int)((Loc.S("Colors").Length + 3) * meanLetterWidth), currentYPosition);
                }

                // max wild level on server
                if (cc != null && displayMaxWild)
                {
                    g.DrawString($"{Loc.S("max wild level")}: {cc.maxWildLevel}",
                        fontSmall, fontBrush, width - 2 * frameThickness, height - fontSizeSmall - 4 * frameThickness, stringFormatRight);
                }

                // frame
                using (var p = new Pen(Properties.Settings.Default.InfoGraphicBorderColor, frameThickness))
                    g.DrawRectangle(p, 0, 0, width - frameThickness, height - frameThickness);
            }

            return bmp;
        }

        /// <summary>
        /// Maximal character length of numbers, also considering percentage signs.
        /// </summary>
        private static int MaxCharLength(double[] values)
        {
            int max = 0;
            for (int si = 0; si < Values.STATS_COUNT; si++)
            {
                int l = values[si].ToString("0").Length + Utils.Precision(si);
                if (l > max) max = l;
            }
            return max;
        }

        /// <summary>
        /// Creates infoGraphic and copies it to the clipboard.
        /// </summary>
        /// <param name="creature"></param>
        /// <param name="cc">CreatureCollection for server settings.</param>
        public static void ExportInfoGraphicToClipboard(this Creature creature, CreatureCollection cc = null)
        {
            if (creature == null) return;

            using (var bmp = creature.InfoGraphic(cc))
            {
                if (bmp != null)
                    Clipboard.SetImage(bmp);
            }
        }
    }
}
