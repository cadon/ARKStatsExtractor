using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats.library
{
    public static class CreatureInfoGraphic
    {
        /// <summary>
        /// Creates an image with infos about the creature, using the user settings.
        /// </summary>
        /// <param name="cc">CreatureCollection for server settings.</param>
        public static Bitmap InfoGraphic(this Creature creature, CreatureCollection cc)
        {
            var fontName = Properties.Settings.Default.InfoGraphicFontName;
            if (string.IsNullOrWhiteSpace(fontName))
            {
                fontName = "Arial";
                Properties.Settings.Default.InfoGraphicFontName = fontName;
            }

            return InfoGraphic(creature, cc,
                Properties.Settings.Default.InfoGraphicHeight,
                fontName,
                Properties.Settings.Default.InfoGraphicForeColor,
                Properties.Settings.Default.InfoGraphicBackColor,
                Properties.Settings.Default.InfoGraphicBorderColor,
                Properties.Settings.Default.InfoGraphicDisplayName,
                Properties.Settings.Default.InfoGraphicWithDomLevels,
                Properties.Settings.Default.InfoGraphicDisplayMutations,
                Properties.Settings.Default.InfoGraphicDisplayGeneration,
                Properties.Settings.Default.InfoGraphicShowStatValues,
                Properties.Settings.Default.InfoGraphicShowMaxWildLevel,
                Properties.Settings.Default.InfoGraphicExtraRegionNames,
                Properties.Settings.Default.InfoGraphicShowRegionNamesIfNoImage);
        }


        /// <summary>
        /// Creates an image with infos about the creature.
        /// </summary>
        /// <param name="cc">CreatureCollection for server settings.</param>
        public static Bitmap InfoGraphic(this Creature creature, CreatureCollection cc,
            int infoGraphicHeight, string fontName, Color foreColor, Color backColor, Color borderColor,
            bool displayCreatureName, bool displayWithDomLevels, bool displayMutations, bool displayGenerations, bool displayStatValues, bool displayMaxWildLevel,
            bool displayExtraRegionNames, bool displayRegionNamesIfNoImage)
        {
            if (creature?.Species == null) return null;
            var secondaryCulture = Loc.UseSecondaryCulture;
            int maxGraphLevel = cc?.maxChartLevel ?? 0;
            if (maxGraphLevel < 1) maxGraphLevel = 50;

            int height = infoGraphicHeight < 1 ? 180 : infoGraphicHeight; // 180
            int width = height * 11 / 6; // 330
            if (displayExtraRegionNames)
                width += height / 2;

            int fontSize = Math.Max(5, height / 18); // 10
            int fontSizeSmall = Math.Max(5, height * 2 / 45); // 8
            int fontSizeHeader = Math.Max(5, height / 15); // 12
            int frameThickness = Math.Max(1, height / 180);

            int statLineHeight = height * 5 / 59; // 15

            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            using (var font = new Font(fontName, fontSize))
            using (var fontSmall = new Font(fontName, fontSizeSmall))
            using (var fontHeader = new Font(fontName, fontSizeHeader, FontStyle.Bold))
            using (var fontBrush = new SolidBrush(foreColor))
            using (var borderAroundColors = new Pen(Utils.ForeColor(backColor), 1))
            using (var stringFormatRight = new StringFormat { Alignment = StringAlignment.Far })
            using (var stringFormatRightUp = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Far })
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                int currentYPosition = frameThickness * 3;

                using (var backgroundBrush = new SolidBrush(backColor))
                    g.FillRectangle(backgroundBrush, 0, 0, width, height);

                var headerText = creature.Species.DescriptiveNameAndMod + (displayCreatureName ? $" - {creature.name}" : string.Empty);

                var fontSizeHeaderCalculated = CalculateFontSize(g, headerText, fontHeader, width);
                if (fontSizeHeaderCalculated < fontSizeHeader)
                {
                    using (var fontHeaderScaled = new Font(fontName, (int)fontSizeHeaderCalculated, FontStyle.Bold))
                        g.DrawString(headerText, fontHeaderScaled, fontBrush, 3 * frameThickness, currentYPosition);

                }
                else
                    g.DrawString(headerText, fontHeader, fontBrush, 3 * frameThickness, currentYPosition);

                currentYPosition += height * 19 / 180; //19
                string creatureLevel;
                if (displayWithDomLevels)
                    creatureLevel = $"{creature.Level}/{creature.LevelHatched + cc?.maxDomLevel ?? 0}";
                else
                    creatureLevel = creature.LevelHatched.ToString();

                string creatureInfos = $"{Loc.S("Level", secondaryCulture: secondaryCulture)} {creatureLevel} | {Utils.SexSymbol(creature.sex) + (creature.flags.HasFlag(CreatureFlags.Neutered) ? $" ({Loc.S(creature.sex == Sex.Female ? "Spayed" : "Neutered", secondaryCulture: secondaryCulture)})" : string.Empty)}";
                if (displayMutations)
                    creatureInfos += $" | {creature.Mutations} {Loc.S("Mutations", secondaryCulture: secondaryCulture)}";
                if (displayGenerations)
                    creatureInfos += $" | {Loc.S("generation", secondaryCulture: secondaryCulture)} {creature.generation}";

                var availableWidth = width - 9 * frameThickness;
                var textWidth = g.MeasureString(creatureInfos, font).Width;
                Font resizedFont = null;
                if (textWidth > availableWidth)
                {
                    var adjustedSize = Math.Max(5, fontSize * availableWidth / textWidth);
                    resizedFont = new Font(font.FontFamily, adjustedSize);
                }

                g.DrawString(creatureInfos, resizedFont ?? font, fontBrush, 5 * frameThickness, currentYPosition);
                resizedFont?.Dispose();
                currentYPosition += height * 17 / 180; //17

                using (var p = new Pen(Color.FromArgb(50, foreColor), 1))
                    g.DrawLine(p, 0, currentYPosition, width, currentYPosition);
                currentYPosition += 2;

                // levels
                double meanLetterWidth = fontSize * 7d / 10;
                int xStatName = (int)meanLetterWidth;
                // x position of level number. torpor is the largest level number.
                int xRightLevelValue = (int)(xStatName + ((displayWithDomLevels ? 6 : 5) + creature.levelsWild[2].ToString().Length) * meanLetterWidth);
                int xRightLevelDomValue = xRightLevelValue;
                if (displayWithDomLevels)
                    xRightLevelDomValue += (int)((creature.levelsDom.Max().ToString().Length) * meanLetterWidth);
                int xRightBrValue = (int)(xRightLevelDomValue + (2 + MaxCharLength(creature.valuesBreeding)) * meanLetterWidth);
                int maxBoxLength = xRightBrValue - xStatName;
                int statBoxHeight = Math.Max(2, height / 90);
                g.DrawString(Loc.S("Levels", secondaryCulture: secondaryCulture), font, fontBrush, xRightLevelDomValue, currentYPosition, stringFormatRight);
                if (displayStatValues)
                    g.DrawString(Loc.S("Values", secondaryCulture: secondaryCulture), font, fontBrush, xRightBrValue, currentYPosition, stringFormatRight);
                int statDisplayIndex = 0;
                for (int si = 0; si < Stats.StatsCount; si++)
                {
                    int statIndex = Stats.DisplayOrder[si];
                    if (statIndex == Stats.Torpidity || !creature.Species.UsesStat(statIndex))
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
                    g.DrawString($"{Utils.StatName(statIndex, true, creature.Species.statNames, secondaryCulture)}",
                        font, fontBrush, xStatName, y);
                    // stat level number
                    g.DrawString($"{(creature.levelsWild[statIndex] < 0 ? "?" : creature.levelsWild[statIndex].ToString())}{(displayWithDomLevels ? " +" : null)}",
                        font, fontBrush, xRightLevelValue, y, stringFormatRight);
                    // dom level number
                    if (displayWithDomLevels)
                        g.DrawString($"{creature.levelsDom[statIndex]}",
                            font, fontBrush, xRightLevelDomValue, y, stringFormatRight);
                    // stat breeding value
                    if (displayStatValues && creature.valuesBreeding != null)
                    {
                        double displayedValue =
                            displayWithDomLevels ? creature.valuesDom[statIndex] : creature.valuesBreeding[statIndex];
                        string statValueRepresentation;
                        if (displayedValue < 0)
                        {
                            statValueRepresentation = "?";
                        }
                        else
                        {
                            if (Utils.Precision(statIndex) == 3)
                            {
                                statValueRepresentation = (100 * displayedValue).ToString("0.0");
                                g.DrawString("%", font, fontBrush, xRightBrValue, y);
                            }
                            else
                                statValueRepresentation = displayedValue.ToString("0.0");
                        }

                        g.DrawString(statValueRepresentation, font, fontBrush, xRightBrValue, y, stringFormatRight);
                    }
                }

                // colors
                int xColor = (int)(xRightBrValue + meanLetterWidth * 3.5);
                int circleDiameter = height * 4 / 45;
                int colorRowHeight = circleDiameter + 2;

                bool creatureImageShown = false;
                int extraMarginBottom = displayMaxWildLevel ? fontSizeSmall : 0;
                int imageSize = (int)Math.Min(width - xColor - circleDiameter - 8 * meanLetterWidth - frameThickness * 4,
                                              height - currentYPosition - frameThickness * 4 - extraMarginBottom);
                if (imageSize > 5)
                {
                    using (var crBmp =
                        CreatureColored.GetColoredCreature(creature.colors, creature.Species, creature.Species.EnabledColorRegions,
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

                int maxColorNameLength = (int)((width - xColor - circleDiameter - (creatureImageShown ? imageSize : 0)) * 1.5 / meanLetterWidth); // max char length for the color region name
                if (maxColorNameLength < 0) maxColorNameLength = 0;

                if (creature.colors != null)
                {
                    g.DrawString(Loc.S("Colors", secondaryCulture: secondaryCulture), font, fontBrush, xColor, currentYPosition);
                    int colorRow = 0;
                    for (int ci = 0; ci < Ark.ColorRegionCount; ci++)
                    {
                        if (!creature.Species.EnabledColorRegions[ci])
                            continue;

                        int y = currentYPosition + (height / 9) + (colorRow++) * colorRowHeight;

                        Color c = CreatureColors.CreatureColor(creature.colors[ci]);
                        //Color fc = Utils.ForeColor(c);

                        using (var b = new SolidBrush(c))
                            g.FillEllipse(b, xColor, y, circleDiameter, circleDiameter);
                        g.DrawEllipse(borderAroundColors, xColor, y, circleDiameter, circleDiameter);

                        string colorRegionName = null;
                        //string colorName = CreatureColors.CreatureColorName(creature.colors[ci]);

                        if (displayExtraRegionNames || (!creatureImageShown && displayRegionNamesIfNoImage))
                        {
                            colorRegionName = creature.Species.colors?[ci]?.name;
                            if (colorRegionName != null)
                            {
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
                        }

                        g.DrawString($"{creature.colors[ci]} - [{ci}]{colorRegionName}",
                            fontSmall, fontBrush, xColor + circleDiameter + 4, y);
                    }
                }

                // mutagen
                if (creature.flags.HasFlag(CreatureFlags.MutagenApplied))
                    g.DrawString("Mutagen applied",
                        fontSmall, fontBrush, xColor, height - fontSizeSmall - 5 * frameThickness);

                // imprinting
                if (displayWithDomLevels)
                {
                    if (creature.isBred || creature.imprintingBonus > 0)
                        g.DrawString($"Imp: {creature.imprintingBonus * 100:0.0} %", font, fontBrush, xColor + (int)((Loc.S("Colors", secondaryCulture: secondaryCulture).Length + 3) * meanLetterWidth), currentYPosition);
                    else if (creature.tamingEff >= 0)
                        g.DrawString($"TE: {creature.tamingEff * 100:0.0} %", font, fontBrush, xColor + (int)((Loc.S("Colors", secondaryCulture: secondaryCulture).Length + 3) * meanLetterWidth), currentYPosition);
                }

                // max wild level on server
                if (cc != null && displayMaxWildLevel)
                {
                    g.DrawString($"{Loc.S("max wild level", secondaryCulture: secondaryCulture)}: {cc.maxWildLevel}",
                        fontSmall, fontBrush, width - 2 * frameThickness, height - frameThickness, stringFormatRightUp);
                }

                // frame
                using (var p = new Pen(borderColor, frameThickness))
                    g.DrawRectangle(p, 0, 0, width - frameThickness, height - frameThickness);
            }

            return bmp;
        }

        /// <summary>
        /// If the text is too long, the smaller font size is returned to fit the available width.
        /// </summary>
        private static float CalculateFontSize(Graphics g, string text, Font font, int availableWidth)
        {
            var size = g.MeasureString(text, font);
            if (availableWidth < size.Width)
                return font.Size * availableWidth / size.Width;
            return font.Size;
        }

        /// <summary>
        /// Maximal character length of numbers, also considering percentage signs.
        /// </summary>
        private static int MaxCharLength(double[] values)
        {
            int max = 0;
            for (int si = 0; si < Stats.StatsCount; si++)
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
