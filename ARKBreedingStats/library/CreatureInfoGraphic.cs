using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ARKBreedingStats.SpeciesImages;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.library
{
    public static class CreatureInfoGraphic
    {
        private static Bitmap _lastOutlineBitmap;
        /// <summary>
        /// Csv string of species blueprint path, outline color, outline width and outline blur
        /// </summary>
        private static string _lastOutlineBmpId;

        private static string LastOutlineBmpIdString(string speciesBlueprintpath, Color outlineColor, int outlineWidth,
            float outlineBlur) => $"{speciesBlueprintpath},{outlineColor},{outlineWidth},{outlineBlur}";

        /// <summary>
        /// Creates an image with infos about the creature, using the user settings.
        /// </summary>
        /// <param name="cc">CreatureCollection for server settings.</param>
        public static Task<Bitmap> InfoGraphicAsync(this Creature creature, CreatureCollection cc) =>
            InfoGraphicAsync(creature, cc,
                Properties.Settings.Default.InfoGraphicHeight,
                GetUserFont(),
                Properties.Settings.Default.InfoGraphicForeColor,
                Properties.Settings.Default.InfoGraphicBackColor,
                Properties.Settings.Default.InfoGraphicBorderColor,
                Properties.Settings.Default.InfoGraphicBorderWidth,
                Properties.Settings.Default.InfoGraphicTextOutlineColor,
                Properties.Settings.Default.InfoGraphicTextOutlineWidth,
                Properties.Settings.Default.InfoGraphicDisplayName,
                Properties.Settings.Default.InfoGraphicWithDomLevels,
                Properties.Settings.Default.InfoGraphicDisplaySumWildMut,
                Properties.Settings.Default.InfoGraphicDisplayMutations,
                Properties.Settings.Default.InfoGraphicDisplayGeneration,
                Properties.Settings.Default.InfoGraphicShowStatValues,
                Properties.Settings.Default.InfoGraphicShowMaxWildLevel,
                Properties.Settings.Default.InfoGraphicExtraRegionNames,
                Properties.Settings.Default.InfoGraphicShowRegionNamesIfNoImage,
                Properties.Settings.Default.InfoGraphicCreatureOutlineColor,
                Properties.Settings.Default.InfoGraphicBackgroundImagePath,
                Properties.Settings.Default.InfoGraphicCreatureOutlineWidth,
                Properties.Settings.Default.InfoGraphicCreatureOutlineBlurring
                );

        /// <summary>
        /// Gets user set font. If not font is set, Arial is set.
        /// </summary>
        /// <returns></returns>
        private static string GetUserFont()
        {
            var fontName = Properties.Settings.Default.InfoGraphicFontName;
            if (string.IsNullOrWhiteSpace(fontName))
            {
                fontName = "Arial";
                Properties.Settings.Default.InfoGraphicFontName = fontName;
            }
            return fontName;
        }

        /// <summary>
        /// Creates an image with infos about the creature.
        /// </summary>
        /// <param name="cc">CreatureCollection for server settings.</param>
        public static async Task<Bitmap> InfoGraphicAsync(this Creature creature, CreatureCollection cc,
            int infoGraphicHeight, string fontName, Color foreColor, Color backColor, Color borderColor, int borderWidth, Color colorOutlineText, float widthOutlineText,
            bool displayCreatureName, bool displayWithDomLevels, bool displaySumWildMutLevels, bool displayMutations, bool displayGenerations, bool displayStatValues, bool displayMaxWildLevel,
            bool displayExtraRegionNames, bool displayRegionNamesIfNoImage, Color colorOutlineCreature, string backgroundImagePath = null, int widthOutlineCreature = 0, float creatureOutlineBlurring = 1)
        {
            if (creature?.Species == null) return null;
            var secondaryCulture = Loc.UseSecondaryCulture;
            var maxGraphLevel = cc?.maxChartLevel ?? 0;
            if (maxGraphLevel < 1) maxGraphLevel = 50;

            var height = infoGraphicHeight < 5 ? 180 : infoGraphicHeight; // 180
            var contentHeight = height - 2 * borderWidth;
            var contentWidth = contentHeight * 12 / 6; // 330
            var width = contentWidth + 2 * borderWidth;
            if (displayExtraRegionNames)
                width += contentHeight / 2;
            var padding = 3 * Math.Max(1, height / 180);
            var borderAndPadding = borderWidth + padding;

            var fontSize = Math.Max(5, contentHeight / 18); // 10
            var fontSizeSmall = Math.Max(5, contentHeight * 2 / 45); // 8
            var fontSizeHeader = Math.Max(5, contentHeight / 15); // 12

            var statLineHeight = contentHeight * 5 / 59; // 15

            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            using (var font = new Font(fontName, fontSize))
            using (var fontSmall = new Font(fontName, fontSizeSmall))
            using (var fontHeader = new Font(fontName, fontSizeHeader, FontStyle.Bold))
            using (var fontBrush = new SolidBrush(foreColor))
            using (var penOutline = new Pen(colorOutlineText, widthOutlineText * 2 + 1))
            using (var borderAroundColors = new Pen(Utils.ForeColor(backColor), 1))
            using (var stringFormatRight = new StringFormat { Alignment = StringAlignment.Far })
            using (var stringFormatRightUp = new StringFormat
            { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Far })
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                var currentYPosition = borderAndPadding;

                if (backColor.A != 255)
                    DrawBackgroundImage(g, width, height, backgroundImagePath);

                using (var backgroundBrush = new SolidBrush(backColor))
                    g.FillRectangle(backgroundBrush, 0, 0, width, height);

                var headerText = creature.Species.DescriptiveNameAndMod +
                                 (displayCreatureName ? $" - {creature.name}" : string.Empty);

                var fontSizeHeaderCalculated = CalculateFontSize(g, headerText, fontHeader, contentWidth);
                if (fontSizeHeaderCalculated < fontSizeHeader)
                {
                    using (var fontHeaderScaled = new Font(fontName, (int)fontSizeHeaderCalculated, FontStyle.Bold))
                        DrawTextWithOutline(g, headerText, fontHeaderScaled, fontBrush, penOutline, borderAndPadding, currentYPosition);
                }
                else
                    DrawTextWithOutline(g, headerText, fontHeader, fontBrush, penOutline, borderAndPadding, currentYPosition);

                currentYPosition += contentHeight * 19 / 180; //19
                string creatureLevel;
                if (displayWithDomLevels)
                    creatureLevel = $"{creature.Level}/{creature.LevelHatched + cc?.maxDomLevel ?? 0}";
                else
                    creatureLevel = creature.LevelHatched.ToString();

                var creatureInfos =
                    $"{Loc.S("Level", secondaryCulture: secondaryCulture)} {creatureLevel} | {Utils.SexSymbol(creature.sex) + (creature.flags.HasFlag(CreatureFlags.Neutered) ? $" ({Loc.S(creature.sex == Sex.Female ? "Spayed" : "Neutered", secondaryCulture: secondaryCulture)})" : string.Empty)}";
                if (displayMutations)
                    creatureInfos +=
                        $" | {Loc.S("mutation counter", secondaryCulture: secondaryCulture)} {creature.Mutations}";
                if (displayGenerations)
                    creatureInfos +=
                        $" | {Loc.S("generation", secondaryCulture: secondaryCulture)} {creature.generation}";

                var availableWidth = width - 2 * borderAndPadding;
                var textWidth = g.MeasureString(creatureInfos, font).Width;
                Font resizedFont = null;
                if (textWidth > availableWidth)
                {
                    var adjustedSize = Math.Max(5, fontSize * availableWidth / textWidth);
                    resizedFont = new Font(font.FontFamily, adjustedSize);
                }

                DrawTextWithOutline(g, creatureInfos, resizedFont ?? font, fontBrush, penOutline, borderAndPadding, currentYPosition);
                resizedFont?.Dispose();
                currentYPosition += contentHeight * 17 / 180; //17

                using (var p = new Pen(Color.FromArgb(50, foreColor), 1))
                    g.DrawLine(p, borderWidth, currentYPosition, width - borderWidth, currentYPosition);
                currentYPosition += 2;

                // levels
                var meanLetterWidth = fontSize * 7d / 10;
                var xStatName = borderAndPadding;
                var displayMutatedLevels =
                    !displaySumWildMutLevels && creature.levelsMutated != null && cc?.Game == Ark.Asa;
                // x position of level number. torpor is the largest level number.
                var xRightLevelValue = (int)(xStatName +
                                             (6 + creature.levelsWild[Stats.Torpidity].ToString().Length) *
                                             meanLetterWidth);
                var xRightLevelMutValue = xRightLevelValue + (!displayMutatedLevels
                    ? 0
                    : (int)((creature.levelsMutated.Max().ToString().Length + 2) * meanLetterWidth));
                var xRightLevelDomValue = xRightLevelMutValue + (!displayWithDomLevels
                    ? 0
                    : (int)((creature.levelsDom.Max().ToString().Length + 1) * meanLetterWidth));
                var xRightBrValue =
                    (int)(xRightLevelDomValue + (2 + MaxCharLength(creature.valuesBreeding)) * meanLetterWidth);
                var maxBoxLength = xRightBrValue - xStatName;
                var statBoxHeight = Math.Max(2, contentHeight / 90);
                DrawTextWithOutline(g, Loc.S("W", secondaryCulture: secondaryCulture) + (displaySumWildMutLevels
                        ? "+" + Loc.S("M", secondaryCulture: secondaryCulture)
                        : string.Empty)
                    , font, fontBrush, penOutline,
                    xRightLevelValue - (displayMutatedLevels || displayWithDomLevels ? (int)meanLetterWidth : 0),
                    currentYPosition, stringFormatRight);
                if (displayMutatedLevels)
                    DrawTextWithOutline(g, Loc.S("M", secondaryCulture: secondaryCulture), font, fontBrush, penOutline,
                        xRightLevelMutValue - (displayWithDomLevels ? (int)meanLetterWidth : 0), currentYPosition,
                        stringFormatRight);
                if (displayWithDomLevels)
                    DrawTextWithOutline(g, Loc.S("D", secondaryCulture: secondaryCulture), font, fontBrush, penOutline, xRightLevelDomValue,
                        currentYPosition, stringFormatRight);
                if (displayStatValues)
                    DrawTextWithOutline(g, Loc.S("Values", secondaryCulture: secondaryCulture), font, fontBrush, penOutline, xRightBrValue,
                        currentYPosition, stringFormatRight);
                var statDisplayIndex = 0;
                foreach (var si in Stats.DisplayOrder)
                {
                    if (si == Stats.Torpidity || !creature.Species.UsesStat(si))
                        continue;

                    var y = currentYPosition + (contentHeight / 9) + (statDisplayIndex++) * statLineHeight;

                    // box
                    // empty box to show the max possible length
                    using (var b = new SolidBrush(Color.DarkGray))
                        g.FillRectangle(b, xStatName, y + statLineHeight - 1, maxBoxLength, statBoxHeight);
                    var levelFractionOfMax = Math.Min(1, (double)creature.levelsWild[si] / maxGraphLevel);
                    if (levelFractionOfMax < 0) levelFractionOfMax = 0;
                    var levelPercentageOfMax = (int)(100 * levelFractionOfMax);
                    var statBoxLength = Math.Max((int)(maxBoxLength * levelFractionOfMax), 1);
                    var statColor = Utils.GetColorFromPercent(levelPercentageOfMax);
                    using (var b = new SolidBrush(statColor))
                        g.FillRectangle(b, xStatName, y + statLineHeight - 1, statBoxLength, statBoxHeight);
                    using (var b = new SolidBrush(Color.FromArgb(10, statColor)))
                    {
                        for (var r = 4; r > 0; r--)
                            g.FillRectangle(b, xStatName - r, y + statLineHeight - 2 - r, statBoxLength + 2 * r,
                                statBoxHeight + 2 * r);
                    }

                    using (var p = new Pen(Utils.GetColorFromPercent(levelPercentageOfMax, -0.5), 1))
                        g.DrawRectangle(p, xStatName, y + statLineHeight - 1, statBoxLength, statBoxHeight);

                    // stat name
                    DrawTextWithOutline(g, $"{Utils.StatName(si, true, creature.Species.statNames, secondaryCulture)}",
                        font, fontBrush, penOutline, xStatName, y);
                    // stat level number
                    var displayedLevel = creature.levelsWild[si] +
                                         (displaySumWildMutLevels && creature.levelsMutated != null &&
                                          creature.levelsMutated[si] > 0
                                             ? creature.levelsMutated[si]
                                             : 0);
                    DrawTextWithOutline(g,
                        $"{(creature.levelsWild[si] < 0 ? "?" : displayedLevel.ToString())}{(displayMutatedLevels || displayWithDomLevels ? " |" : string.Empty)}",
                        font, fontBrush, penOutline, xRightLevelValue, y, stringFormatRight);
                    if (displayMutatedLevels)
                        DrawTextWithOutline(g,
                            $"{(creature.levelsMutated[si] < 0 ? string.Empty : creature.levelsMutated[si].ToString())}{(displayWithDomLevels ? " |" : string.Empty)}",
                            font, fontBrush, penOutline, xRightLevelMutValue, y, stringFormatRight);
                    // dom level number
                    if (displayWithDomLevels)
                        DrawTextWithOutline(g, $"{creature.levelsDom[si]}",
                            font, fontBrush, penOutline, xRightLevelDomValue, y, stringFormatRight);
                    // stat breeding value
                    if (displayStatValues && creature.valuesBreeding != null)
                    {
                        var displayedValue =
                            displayWithDomLevels ? creature.valuesCurrent[si] : creature.valuesBreeding[si];
                        string statValueRepresentation;
                        if (displayedValue < 0)
                        {
                            statValueRepresentation = "?";
                        }
                        else
                        {
                            if (Stats.IsPercentage(si))
                            {
                                statValueRepresentation = (100 * displayedValue).ToString("0.0");
                                DrawTextWithOutline(g, "%", font, fontBrush, penOutline, xRightBrValue, y);
                            }
                            else
                                statValueRepresentation = displayedValue.ToString("0.0");
                        }

                        DrawTextWithOutline(g, statValueRepresentation, font, fontBrush, penOutline, xRightBrValue, y, stringFormatRight);
                    }
                }

                // colors
                var xColor = (int)(xRightBrValue + meanLetterWidth * 3.5);
                var circleDiameter = contentHeight * 4 / 45;
                var colorRowHeight = circleDiameter + 2;

                var creatureImageShown = false;
                var extraMarginBottom = displayMaxWildLevel ? fontSizeSmall : 0;
                var imageSize = (int)Math.Min(
                    contentWidth - xColor + borderWidth - circleDiameter - 8 * meanLetterWidth,
                    contentHeight - currentYPosition + borderWidth - extraMarginBottom);
                if (imageSize > 5)
                {
                    using (var crBmp = (await
                               CreatureColored.GetColoredCreatureAsync(creature.colors, creature.Species,
                                       creature.Species.EnabledColorRegions,
                                       imageSize, onlyImage: true, creatureSex: creature.sex, game: cc.Game)
                                   .ConfigureAwait(false)).Bmp)
                    {
                        if (crBmp != null)
                        {
                            const int blurRadius = 1; // seems to result in good enough smoothing of the outline brush

                            if (widthOutlineCreature > 0 && colorOutlineCreature.A != 0)
                            {
                                var outlineId = LastOutlineBmpIdString(creature.speciesBlueprint, colorOutlineCreature,
                                    widthOutlineCreature, creatureOutlineBlurring);
                                Bitmap outline;
                                if (outlineId == _lastOutlineBmpId)
                                    outline = _lastOutlineBitmap;
                                else
                                {
                                    outline = ImageTools.BlurImageAlpha(
                                        ImageTools.OutlineOpacities(crBmp, colorOutlineCreature, widthOutlineCreature, creatureOutlineBlurring), blurRadius);
                                    _lastOutlineBitmap?.Dispose();
                                    _lastOutlineBitmap = outline;
                                    _lastOutlineBmpId = outlineId;
                                }

                                var outlinePadding = widthOutlineCreature + blurRadius;
                                g.DrawImage(outline, width - imageSize - borderAndPadding - outlinePadding,
                                    height - imageSize - borderAndPadding - extraMarginBottom - outlinePadding, imageSize + 2 * outlinePadding, imageSize + 2 * outlinePadding);
                            }

                            g.DrawImage(crBmp, width - imageSize - borderAndPadding,
                                height - imageSize - borderAndPadding - extraMarginBottom, imageSize, imageSize);
                            creatureImageShown = true;
                        }
                    }
                }

                var maxColorNameLength =
                    (int)((width - 2 * borderWidth - xColor - circleDiameter - (creatureImageShown ? imageSize : 0)) * 1.5 /
                          meanLetterWidth); // max char length for the color region name
                if (maxColorNameLength < 0) maxColorNameLength = 0;

                if (creature.colors != null)
                {
                    DrawTextWithOutline(g, Loc.S("Colors", secondaryCulture: secondaryCulture), font, fontBrush, penOutline
                        , xColor, currentYPosition);
                    DrawColors(creature.Species, creature.colors, displayExtraRegionNames, displayRegionNamesIfNoImage,
                        currentYPosition, contentHeight, colorRowHeight,
                        g, xColor, circleDiameter, borderAroundColors, creatureImageShown, maxColorNameLength,
                        fontSmall, fontBrush, penOutline);
                }

                // mutagen
                if (creature.flags.HasFlag(CreatureFlags.MutagenApplied))
                    DrawTextWithOutline(g, "Mutagen applied",
                        fontSmall, fontBrush, penOutline, xColor, height - fontSizeSmall - borderAndPadding);

                // imprinting
                if (displayWithDomLevels)
                {
                    if (creature.isBred || creature.imprintingBonus > 0)
                        DrawTextWithOutline(g, $"Imp: {creature.imprintingBonus * 100:0.0} %", font, fontBrush, penOutline,
                            xColor + (int)((Loc.S("Colors", secondaryCulture: secondaryCulture).Length + 3) *
                                           meanLetterWidth), currentYPosition);
                    else if (creature.tamingEff >= 0)
                        DrawTextWithOutline(g, $"TE: {creature.tamingEff * 100:0.0} %", font, fontBrush, penOutline,
                            xColor + (int)((Loc.S("Colors", secondaryCulture: secondaryCulture).Length + 3) *
                                           meanLetterWidth), currentYPosition);
                }

                // max wild level on server
                if (cc != null && displayMaxWildLevel)
                {
                    DrawTextWithOutline(g, $"{Loc.S("max wild level", secondaryCulture: secondaryCulture)}: {cc.maxWildLevel}",
                        fontSmall, fontBrush, penOutline, width - borderAndPadding, height - borderAndPadding, stringFormatRightUp);
                }

                // border
                if (borderWidth > 0)
                {
                    g.SmoothingMode = SmoothingMode.None;
                    using (var p = new Pen(borderColor, borderWidth))
                        g.DrawRectangle(p, borderWidth / 2f, borderWidth / 2f, width - borderWidth, height - borderWidth);
                }
            }

            return bmp;
        }

        private static void DrawTextWithOutline(Graphics g, string text, Font font, Brush fontBrush, Pen outlinePen, int x, int y, StringFormat stringFormat = null)
        {
            if (stringFormat == null)
                stringFormat = StringFormat.GenericDefault;
            if (outlinePen.Width > 1)
                using (var path = new GraphicsPath())
                {
                    path.AddString(text, font.FontFamily, (int)font.Style, g.DpiY * font.Size / 72, new PointF(x - .5f, y - .5f), stringFormat);
                    // outline
                    g.DrawPath(outlinePen, path);
                }
            g.DrawString(text, font, fontBrush, x, y, stringFormat);
        }

        private static void DrawBackgroundImage(Graphics g, int width, int height, string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath)) return;
            try
            {
                using (var bgImg = new Bitmap(imagePath))
                {
                    var widthRatio = (float)width / bgImg.Width;
                    var heightRatio = (float)height / bgImg.Height;
                    var scaleFactor = Math.Max(widthRatio, heightRatio);
                    var widthScaled = (int)Math.Round(bgImg.Width * scaleFactor);
                    var heightScaled = (int)Math.Round(bgImg.Height * scaleFactor);
                    var offsetX = widthScaled > width ? (width - widthScaled) / 2 : 0;
                    var offsetY = heightScaled > height ? (height - heightScaled) / 2 : 0;
                    g.DrawImage(bgImg, offsetX, offsetY, widthScaled, heightScaled);
                }
            }
            catch (Exception ex)
            {
                MessageBoxes.ExceptionMessageBox(ex, "Error when trying to draw the background image of the infographic with the path\n" + imagePath);
            }
        }

        private static void DrawColors(Species species, byte[] creatureColors, bool displayExtraRegionNames, bool displayRegionNamesIfNoImage,
            int currentYPosition, int height, int colorRowHeight, Graphics g, int xColor, int circleDiameter,
            Pen borderAroundColors, bool creatureImageShown, int maxColorNameLength, Font fontSmall, SolidBrush fontBrush, Pen penOutline)
        {
            var colorRow = 0;
            for (var ci = 0; ci < Ark.ColorRegionCount; ci++)
            {
                if (!species.EnabledColorRegions[ci])
                    continue;

                var y = currentYPosition + (height / 9) + (colorRow++) * colorRowHeight;

                var c = CreatureColors.CreatureColor(creatureColors[ci]);
                //Color fc = Utils.ForeColor(c);

                using (var b = new SolidBrush(c))
                    g.FillEllipse(b, xColor, y, circleDiameter, circleDiameter);
                g.DrawEllipse(borderAroundColors, xColor, y, circleDiameter, circleDiameter);

                string colorRegionName = null;
                //string colorName = CreatureColors.CreatureColorName(creature.colors[ci]);

                if (displayExtraRegionNames || (!creatureImageShown && displayRegionNamesIfNoImage))
                {
                    colorRegionName = species.colors?[ci]?.name;
                    if (colorRegionName != null)
                    {
                        var totalColorLength = colorRegionName.Length + 11;
                        if (totalColorLength > maxColorNameLength)
                        {
                            // shorten color region name
                            var lengthForRegionName =
                                colorRegionName.Length - (totalColorLength - maxColorNameLength);
                            colorRegionName = lengthForRegionName < 2
                                ? string.Empty
                                : colorRegionName.Substring(0, lengthForRegionName - 1) + "…";
                        }

                        if (!string.IsNullOrEmpty(colorRegionName))
                            colorRegionName = " (" + colorRegionName + ")";
                    }
                }

                DrawTextWithOutline(g, $"[{ci}] {creatureColors[ci]}{colorRegionName}",
                    fontSmall, fontBrush, penOutline, xColor + circleDiameter + 4, y);
            }
        }

        /// <summary>
        /// If the text is too long, the smaller font size is returned to fit the available width.
        /// </summary>
        private static float CalculateFontSize(Graphics g, string text, Font font, int availableWidth)
        {
            var size = g.MeasureString(text, font);
            if (availableWidth < size.Width)
                return Math.Max(5, font.Size * availableWidth / size.Width);
            return Math.Max(5, font.Size);
        }

        /// <summary>
        /// Maximal character length of numbers, also considering percentage signs.
        /// </summary>
        private static int MaxCharLength(double[] values)
        {
            var max = 0;
            for (var si = 0; si < Stats.StatsCount; si++)
            {
                var l = values[si].ToString("0").Length + Stats.Precision(si);
                if (l > max) max = l;
            }
            return max;
        }

        /// <summary>
        /// Creates infoGraphic and copies it to the clipboard.
        /// </summary>
        /// <param name="creature"></param>
        /// <param name="cc">CreatureCollection for server settings.</param>
        public static async Task ExportInfoGraphicToClipboard(this Creature creature, CreatureCollection cc)
        {
            if (creature == null) return;
            ClipboardHandler.SetImageWithAlphaToClipboard(await creature.InfoGraphicAsync(cc).ConfigureAwait(false));
        }

        /// <summary>
        /// Returns the coloredCreature Image with additional info about the colors.
        /// </summary>
        public static Image GetImageWithColors(Image coloredCreature, byte[] creatureColors, Species species)
        {
            return CreateImageWithColors(coloredCreature, creatureColors, species,
                Properties.Settings.Default.InfoGraphicHeight,
                GetUserFont(),
                Properties.Settings.Default.InfoGraphicForeColor,
                Properties.Settings.Default.InfoGraphicBackColor,
                Properties.Settings.Default.InfoGraphicTextOutlineColor,
                Properties.Settings.Default.InfoGraphicTextOutlineWidth,
                Properties.Settings.Default.InfoGraphicExtraRegionNames,
                Properties.Settings.Default.InfoGraphicShowRegionNamesIfNoImage);
        }

        /// <summary>
        /// Returns the coloredCreature Image with additional info about the colors.
        /// </summary>
        private static Image CreateImageWithColors(Image coloredCreature, byte[] creatureColors, Species species,
            int infoGraphicHeight, string fontName, Color foreColor, Color backColor, Color outlineColor, float outlineWidth,
            bool displayExtraRegionNames, bool displayRegionNamesIfNoImage)
        {
            const int margin = 10;
            var height = coloredCreature.Height;
            var width = (int)(coloredCreature.Width * (displayExtraRegionNames ? 2 : 1.5));
            var widthForColors = width - coloredCreature.Width;
            var circleDiameter = height * 4 / 45;
            var fontSize = circleDiameter * 6 / 10;
            var meanLetterWidth = fontSize * 7d / 10;
            var heightWithoutHeader = height;
            height += fontSize;

            var maxColorNameLength = (int)((widthForColors - circleDiameter) * 1.5 / meanLetterWidth); // max char length for the color region name
            if (maxColorNameLength < 0) maxColorNameLength = 0;

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            using (var font = new Font(fontName, fontSize))
            using (var fontBrush = new SolidBrush(foreColor))
            using (var borderAroundColors = new Pen(Utils.ForeColor(backColor), 1))
            using (var penOutline = new Pen(outlineColor, outlineWidth * 2 + 1))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                DrawTextWithOutline(g, species.DescriptiveNameAndMod, font, fontBrush, penOutline, margin, margin);

                g.DrawImage(coloredCreature, margin, margin + fontSize);
                DrawColors(species, creatureColors, displayExtraRegionNames, displayRegionNamesIfNoImage, fontSize,
                    heightWithoutHeader, (heightWithoutHeader - 4 * margin) / Ark.ColorRegionCount, g, coloredCreature.Width + margin,
                    circleDiameter, borderAroundColors, true, maxColorNameLength, font, fontBrush, penOutline);
                if (infoGraphicHeight != height)
                {
                    var scaleFactor = (float)infoGraphicHeight / height;
                    var scaledHeight = (int)(scaleFactor * height);
                    var scaledWidth = (int)(scaleFactor * width);

                    var bmpScaled = new Bitmap(scaledWidth, scaledHeight);
                    using (var gs = Graphics.FromImage(bmpScaled))
                    {
                        gs.CompositingMode = CompositingMode.SourceCopy;
                        gs.CompositingQuality = CompositingQuality.HighQuality;
                        gs.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gs.SmoothingMode = SmoothingMode.HighQuality;
                        gs.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        gs.DrawImage(bmp, 0, 0, scaledWidth, scaledHeight);
                    }
                    bmp.Dispose();
                    bmp = bmpScaled;
                }
            }

            return bmp;
        }
    }
}
