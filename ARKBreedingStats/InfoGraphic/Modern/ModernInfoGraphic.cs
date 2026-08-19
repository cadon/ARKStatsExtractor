using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Threading.Tasks;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.SpeciesImages;
using ARKBreedingStats.utils;
using static ARKBreedingStats.InfoGraphic.Modern.ModernInfoGraphicSettings;

namespace ARKBreedingStats.InfoGraphic.Modern
{
    /// <summary>
    /// Renders a creature as a portrait card styled after the in game creature panel:
    /// a sex corner badge, the species artwork as a large unframed hero image, icon led stat bars,
    /// color region chips and a mutation footer.
    ///
    /// A drop in alternative to <see cref="CreatureInfoGraphic"/>: it returns a <see cref="Bitmap"/>
    /// the caller owns, so clipboard export, folder export, stitching and the settings preview all
    /// work with it unchanged.
    /// </summary>
    public static class ModernInfoGraphic
    {
        #region design constants

        // All values are in design pixels for a card of ModernInfoGraphicSettings.DesignWidth and
        // are scaled by the configured output width before use.
        private const float CardPadding = 9;
        private const float BorderWidth = 1.5f;
        private const float CornerRadius = 11;
        private const float SexBadgeLeg = 38;

        private const float HeaderGap = 7;

        /// <summary>
        /// Size of the creature artwork, in the lower left. Drawn unframed, the way the classic
        /// graphic does it: a boxed thumbnail made the creature far too small to read.
        /// </summary>
        private const float ArtSize = 160;
        private const float ColumnGap = 8;

        /// <summary>
        /// Thickness of the rim drawn around the creature's silhouette. It overflows into the card
        /// padding and the column gap, both of which are wider than this, so the artwork itself
        /// keeps its full size.
        /// </summary>
        private const float HaloWidth = 4;
        private const int HaloBlurRadius = 1;
        /// <summary>0 is a sharp outline, 1 is fully blurry. Soft enough to read as light.</summary>
        private const float HaloBlurriness = 0.8f;
        private const int HaloAlpha = 175;

        private const float StatRowHeight = 22;
        private const float StatRowGap = 4;
        private const float IconColumnWidth = 18;
        private const float IconGap = 6;
        private const float TrackRadius = 4;
        private const float TrackTextInset = 6;

        private const float ChipsGap = 8;
        private const float ChipHeight = 20;
        private const float ChipGap = 6;
        private const float ChipRowGap = 4;
        private const float ChipRadius = 4;
        private const float MaxChipWidth = 66;
        private const int NamedRegionColumns = 2;
        /// <summary>Shaded part of a chip that carries the color region index.</summary>
        private const float RegionIndexWidth = 20;
        /// <summary>Width of the color chip when a region name is shown next to it.</summary>
        private const float NamedChipWidth = 46;
        private const float ChipNameGap = 5;

        private const float FooterGap = 6;
        private const float FooterHeight = 14;

        // The classic graphic builds its fonts in points, so its "10" is about 13 real pixels.
        // These are pixel sizes, and have to be comparable to that to read as clearly.
        private const float FontSizeName = 15;
        /// <summary>
        /// The title is truncated rather than shrunk below this. An unreadably small name tells the
        /// reader less than the first part of it followed by an ellipsis.
        /// </summary>
        private const float FontSizeNameMin = 9;
        private const float FontSizeDetail = 11;
        private const float FontSizeMeta = 10;
        private const float FontSizeStat = 11.5f;
        private const float FontSizeChip = 10.5f;
        private const float FontSizeFooter = 10;
        private const float FontSizeCaption = 9.5f;
        private const float FontSizeBadge = 13;

        #endregion

        /// <summary>
        /// Creates the info graphic. Returns null if the creature has no species.
        /// The returned bitmap has to be disposed by the caller.
        /// </summary>
        /// <param name="cc">CreatureCollection for server settings. May be null.</param>
        public static async Task<Bitmap> RenderAsync(Creature creature, CreatureCollection cc,
            ModernInfoGraphicSettings settings)
        {
            if (!CreatureInfoGraphic.CanDrawInfoGraphic(creature)) return null;
            settings ??= new ModernInfoGraphicSettings();

            var secondaryCulture = Loc.UseSecondaryCulture;
            var theme = ModernTheme.Create(settings, creature);

            // Everything is drawn at the final resolution. Rendering larger and downscaling costs
            // text quality, because the glyph hinting is lost in the resample.
            var width = Math.Max(120, settings.Width);
            // the design constants describe a card of DesignWidth, scale them proportionally
            var scale = width / (float)DesignWidth;

            var statIndices = UsedStats(creature.Species);
            var showColors = settings.ShowColors && creature.colors != null;
            var colorRegions = showColors
                ? EnabledColorRegions(creature.Species, creature.colors)
                : new List<int>();
            showColors &= colorRegions.Count > 0;
            var showMutations = settings.ShowMutations;

            // the artwork is fetched before any drawing starts, it is the only awaited step.
            // Whether real species art exists decides if the color regions need to be named.
            var (artwork, hasSpeciesArtwork) =
                await GetArtworkAsync(creature, cc, (int)Math.Round(ArtSize * scale)).ConfigureAwait(false);

            var halo = CreateArtworkHalo(artwork, theme, settings, scale, out var haloPadding);

            var namedRegions = showColors && RegionNamesWanted(settings, hasSpeciesArtwork, creature.Species);
            // named regions are laid out in a grid instead of a single strip of chips
            var chipRows = !showColors ? 0
                : namedRegions ? (colorRegions.Count + NamedRegionColumns - 1) / NamedRegionColumns
                : 1;

            using var fonts = new FontCache(settings.FontName);
            var contentWidth = (DesignWidth - 2 * CardPadding) * scale;

            // The header text is measured before the card is sized, because a long species name
            // breaks onto a second line and the card has to grow to fit it.
            List<HeaderLine> headerLines;
            using (var probe = new Bitmap(1, 1))
            using (var probeGraphics = Graphics.FromImage(probe))
            {
                probeGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                headerLines = BuildHeaderLines(probeGraphics, fonts, theme, settings, creature, cc, contentWidth,
                    scale, secondaryCulture);
            }

            var headerTextHeight = 0f;
            foreach (var line in headerLines) headerTextHeight += line.Height;

            // artwork and stat rows sit side by side, the taller one sets the height of the body
            var statsHeight = statIndices.Count * StatRowHeight
                              + Math.Max(0, statIndices.Count - 1) * StatRowGap;
            var bodyHeight = Math.Max(ArtSize, statsHeight);

            var designHeight = 2 * CardPadding
                               + headerTextHeight / scale
                               + HeaderGap
                               + bodyHeight
                               + (showColors ? ChipsGap + chipRows * ChipHeight + (chipRows - 1) * ChipRowGap : 0)
                               + (showMutations ? FooterGap + FooterHeight : 0);

            var height = (int)Math.Round(designHeight * scale);

            var bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    var cardRect = new RectangleF(BorderWidth * scale / 2, BorderWidth * scale / 2,
                        width - BorderWidth * scale, height - BorderWidth * scale);
                    var radius = CornerRadius * scale;

                    if (!settings.TransparentBackground)
                        DrawingPrimitives.FillRoundedRectGradient(g, theme.BackgroundTop, theme.BackgroundBottom,
                            cardRect, radius);

                    // everything inside the card is clipped to the rounded shape, so the corner
                    // badge follows the border instead of poking out of it
                    var clipState = g.Save();
                    using (var cardPath = DrawingPrimitives.RoundedRect(cardRect, radius))
                        g.SetClip(cardPath, CombineMode.Intersect);

                    DrawSexBadge(g, fonts, theme, creature.sex, scale);

                    var content = new RectangleF(CardPadding * scale, CardPadding * scale,
                        width - 2 * CardPadding * scale, height - 2 * CardPadding * scale);
                    var cursor = new LayoutCursor(content);

                    DrawHeaderText(g, fonts, headerLines, cursor.Row(headerTextHeight));
                    DrawDivider(g, theme, cursor.Row(HeaderGap * scale), scale);

                    var body = cursor.Row(bodyHeight * scale);
                    // the artwork is centered vertically in the body, the stat rows start at the top
                    var artColumn = new RectangleF(body.X, body.Y, ArtSize * scale, body.Height);
                    var statColumn = new RectangleF(artColumn.Right + ColumnGap * scale, body.Y,
                        body.Right - artColumn.Right - ColumnGap * scale, body.Height);

                    // a caption under the artwork says which values the stat rows are showing
                    var valuesCaption = ValuesCaptionText(settings);
                    if (valuesCaption != null)
                    {
                        var captionHeight = FontSizeCaption * scale * 1.5f;
                        artColumn = new RectangleF(artColumn.X, artColumn.Y, artColumn.Width,
                            artColumn.Height - captionHeight);
                        DrawingPrimitives.TextFitted(g, valuesCaption, fonts, FontSizeCaption * scale,
                            FontStyle.Bold,
                            new RectangleF(artColumn.X, artColumn.Bottom, artColumn.Width, captionHeight),
                            theme.TextMuted, StringAlignment.Center, StringAlignment.Center);
                    }

                    DrawHeroArtwork(g, artColumn, artwork, halo, haloPadding);
                    DrawStatRows(g, fonts, theme, settings, creature, cc, statIndices, statColumn, scale);

                    if (showColors)
                    {
                        cursor.Skip(ChipsGap * scale);
                        var chipsHeight = chipRows * ChipHeight + (chipRows - 1) * ChipRowGap;
                        DrawColorChips(g, fonts, theme, creature, colorRegions, cursor.Row(chipsHeight * scale),
                            scale, namedRegions);
                    }

                    if (showMutations)
                    {
                        cursor.Skip(FooterGap * scale);
                        DrawFooter(g, fonts, theme, creature, cursor.Row(FooterHeight * scale), scale, secondaryCulture);
                    }

                    g.Restore(clipState);

                    DrawingPrimitives.DrawRoundedRect(g, theme.Border, BorderWidth * scale, cardRect, radius);
                }
            }
            catch (Exception)
            {
                bmp.Dispose();
                throw;
            }
            finally
            {
                artwork?.Dispose();
                halo?.Dispose();
            }

            return bmp;
        }

        #region sections

        /// <summary>
        /// Corner triangle carrying the sex symbol, clipped to the card by the caller's clip region.
        /// </summary>
        private static void DrawSexBadge(Graphics g, FontCache fonts, ModernTheme theme, Sex sex, float scale)
        {
            var leg = SexBadgeLeg * scale;
            using (var brush = new SolidBrush(theme.SexColor(sex)))
                g.FillPolygon(brush, new[] { new PointF(0, 0), new PointF(leg, 0), new PointF(0, leg) });

            // offset from the very corner, otherwise the rounded card corner clips the symbol
            var symbolBox = new RectangleF(leg * 0.09f, leg * 0.06f, leg * 0.44f, leg * 0.44f);
            // drawn where a vector exists, the font glyphs are too thin to read at this size
            if (!SexSymbols.Draw(g, sex, symbolBox, Color.White))
                DrawingPrimitives.TextFitted(g, Utils.SexSymbol(sex), fonts, FontSizeBadge * scale, FontStyle.Bold,
                    symbolBox, Color.White, StringAlignment.Center, StringAlignment.Center);
        }

        /// <summary>One line of the header text block, already sized to fit.</summary>
        private readonly struct HeaderLine
        {
            internal readonly string Text;
            internal readonly float Size;
            internal readonly FontStyle Style;
            internal readonly Color Color;
            internal readonly float Height;
            /// <summary>
            /// Smallest size this line may shrink to before it is truncated instead. Shrinking a
            /// long name into illegibility is worse than showing the start of it.
            /// </summary>
            internal readonly float MinSize;

            internal HeaderLine(string text, float size, FontStyle style, Color color, float height,
                float minSize = 5f)
            {
                Text = text;
                Size = size;
                Style = style;
                Color = color;
                Height = height;
                MinSize = minSize;
            }
        }

        /// <summary>
        /// Draws the creature unframed and as large as the row allows, behind it the halo that
        /// separates it from the card.
        /// </summary>
        private static void DrawHeroArtwork(Graphics g, RectangleF row, Bitmap artwork, Bitmap halo, float haloPadding)
        {
            if (artwork == null || row.Width <= 0 || row.Height <= 0) return;

            var factor = Math.Min(row.Width / artwork.Width, row.Height / artwork.Height);
            var drawWidth = artwork.Width * factor;
            var drawHeight = artwork.Height * factor;
            var artRect = new RectangleF(
                row.X + (row.Width - drawWidth) / 2,
                row.Y + (row.Height - drawHeight) / 2,
                drawWidth, drawHeight);

            // the halo bitmap is the artwork grown by the rim and the blur on every side
            if (halo != null)
                g.DrawImage(halo, RectangleF.Inflate(artRect, haloPadding, haloPadding));

            g.DrawImage(artwork, artRect);
        }

        /// <summary>
        /// A blurred rim following the creature's alpha silhouette, so a dark creature does not
        /// disappear into a dark card. The color is taken from the background rather than from the
        /// artwork, which is what makes it work whatever the species looks like.
        /// </summary>
        /// <param name="padding">How far the rim extends beyond the artwork on each side.</param>
        private static Bitmap CreateArtworkHalo(Bitmap artwork, ModernTheme theme,
            ModernInfoGraphicSettings settings, float scale, out float padding)
        {
            padding = 0;
            if (artwork == null || !settings.ArtworkHalo) return null;

            var rimWidth = Math.Max(1, (int)Math.Round(HaloWidth * scale));
            try
            {
                var color = Color.FromArgb(HaloAlpha, DrawingPrimitives.ContrastingText(theme.BackgroundTop));
                var halo = ImageTools.BlurImageAlpha(
                    ImageTools.OutlineOpacities(artwork, color, rimWidth, HaloBlurriness), HaloBlurRadius);
                padding = rimWidth + HaloBlurRadius;
                return halo;
            }
            catch (Exception)
            {
                // the rim is decoration only, a broken image pack file must not fail the whole card
                return null;
            }
        }

        private static void DrawHeaderText(Graphics g, FontCache fonts, List<HeaderLine> lines, RectangleF area)
        {
            var y = area.Y;
            foreach (var line in lines)
            {
                DrawingPrimitives.TextFitted(g, line.Text, fonts, line.Size, line.Style,
                    new RectangleF(area.X, y, area.Width, line.Height), line.Color,
                    StringAlignment.Center, StringAlignment.Center, line.MinSize);
                y += line.Height;
            }
        }

        /// <summary>
        /// Adds the title, giving up the least useful part first when it does not fit.
        ///
        /// A creature's own name is capped by the game at <see cref="Ark.MaxCreatureNameLength"/>,
        /// so it always fits. A species name is not capped, and a modded one carries a variant and
        /// a mod suffix that can be longer than the name itself, e.g.
        /// "Fasolasuchus (ScorchedEarth) (ASA)". Those suffixes are dropped before the text is
        /// shrunk, and the full name still appears on the detail line underneath.
        /// </summary>
        /// <param name="levelText">Appended to the title, or null when it belongs on the detail line.</param>
        private static void AddTitleLines(List<HeaderLine> lines, Graphics g, FontCache fonts, ModernTheme theme,
            ModernInfoGraphicSettings settings, Creature creature, string displayName, bool useOwnName,
            string levelText, float availableWidth, float scale)
        {
            var titleSize = FontSizeName * scale;
            var minSize = FontSizeNameMin * scale;

            // Most complete first. A creature's own name has nothing that can be dropped, and
            // neither has a species name whose suffixes are already switched off.
            var candidates = useOwnName || !settings.ShowSpeciesSuffixes
                ? new[] { displayName }
                : new[]
                {
                    displayName,
                    creature.Species.Name(creature.sex, true, false), // without the mod suffix
                    creature.Species.Name(creature.sex, false, false) // without the variant either
                };

            foreach (var candidate in candidates)
            {
                var oneLine = levelText == null ? candidate : $"{candidate} - {levelText}";
                if (DrawingPrimitives.MeasureText(g, oneLine, fonts, titleSize, FontStyle.Bold).Width > availableWidth)
                    continue;

                lines.Add(new HeaderLine(oneLine, titleSize, FontStyle.Bold, theme.TextPrimary, titleSize * 1.32f));
                return;
            }

            // Use the shortest name from here on, it has the best chance of staying readable.
            var shortest = candidates[candidates.Length - 1];

            // the level is already on the detail line, so there is nothing to split off
            if (levelText == null)
            {
                lines.Add(new HeaderLine(shortest, titleSize, FontStyle.Bold, theme.TextPrimary, titleSize * 1.32f,
                    minSize));
                return;
            }

            // nothing fits beside the level, so the level takes a line of its own
            lines.Add(new HeaderLine(shortest, titleSize, FontStyle.Bold, theme.TextPrimary, titleSize * 1.3f,
                minSize));
            lines.Add(new HeaderLine(levelText, titleSize * 0.85f, FontStyle.Bold, theme.TextPrimary,
                titleSize * 1.15f, minSize));
        }

        private static List<HeaderLine> BuildHeaderLines(Graphics g, FontCache fonts, ModernTheme theme,
            ModernInfoGraphicSettings settings, Creature creature, CreatureCollection cc, float availableWidth,
            float scale, bool secondaryCulture)
        {
            // the bracketed variant and mod suffixes are optional, so this is the full name only
            // when the user wants them
            var speciesName = creature.Species.Name(creature.sex, settings.ShowSpeciesSuffixes,
                settings.ShowSpeciesSuffixes);
            var useOwnName = settings.ShowCreatureName && !string.IsNullOrWhiteSpace(creature.name);
            var displayName = useOwnName ? creature.name : speciesName;
            var levelText = $"{Loc.S("Level", secondaryCulture: secondaryCulture)} {creature.Level}";

            var meta = MetaText(creature, cc, settings, secondaryCulture);

            // The detail line always carries something, and the title never repeats it. With a name
            // of its own the creature leads with the name and the species drops to the detail line;
            // without one the species leads and the level drops instead.
            var lines = new List<HeaderLine>();
            AddTitleLines(lines, g, fonts, theme, settings, creature, displayName, useOwnName,
                useOwnName ? levelText : null, availableWidth, scale);

            // Everything on the detail line shares it. On a card this size each line costs artwork
            // height, so the header is kept to two.
            var detailParts = new List<string>();
            if (!useOwnName) detailParts.Add(levelText);
            else if (settings.ShowSpecies) detailParts.Add(speciesName);
            if (!string.IsNullOrEmpty(meta)) detailParts.Add(meta);
            if (detailParts.Count > 0)
                lines.Add(new HeaderLine(string.Join(" · ", detailParts), FontSizeDetail * scale, FontStyle.Bold,
                    theme.TextMuted, FontSizeDetail * scale * 1.32f));

            return lines;
        }

        private static void DrawDivider(Graphics g, ModernTheme theme, RectangleF gapRow, float scale)
        {
            var y = gapRow.Y + gapRow.Height / 2;
            using var pen = new Pen(theme.Divider, Math.Max(1, scale));
            g.DrawLine(pen, gapRow.X, y, gapRow.Right, y);
        }

        private static void DrawStatRows(Graphics g, FontCache fonts, ModernTheme theme,
            ModernInfoGraphicSettings settings, Creature creature, CreatureCollection cc, List<int> statIndices,
            RectangleF area, float scale)
        {
            var maxGraphLevel = cc?.maxChartLevel ?? 0;
            if (maxGraphLevel < 1) maxGraphLevel = 50;
            var showMutatedLevels = !settings.SumWildAndMutatedLevels
                                    && creature.levelsMutated != null && cc?.Game == Ark.Asa;

            // Every row's text is laid out in the same width, so one font size is chosen up front
            // for all of them. Fitting each row on its own shrinks the long values while the short
            // ones stay large, which makes the numbers jump from row to row.
            var trackTextWidth = area.Width - (IconColumnWidth + IconGap) * scale - 2 * TrackTextInset * scale;
            var rowTexts = new string[statIndices.Count];
            var rowFontSize = FontSizeStat * scale;
            for (var i = 0; i < statIndices.Count; i++)
            {
                rowTexts[i] = StatRowText(creature, statIndices[i], settings, showMutatedLevels);
                rowFontSize = Math.Min(rowFontSize, DrawingPrimitives.FittedFontSize(g, rowTexts[i], fonts,
                    FontSizeStat * scale, FontStyle.Bold, trackTextWidth));
            }

            var cursor = new LayoutCursor(area);
            for (var i = 0; i < statIndices.Count; i++)
            {
                if (i > 0) cursor.Skip(StatRowGap * scale);
                var row = cursor.Row(StatRowHeight * scale);
                var si = statIndices[i];

                var iconRect = new RectangleF(row.X, row.Y + row.Height * 0.18f,
                    IconColumnWidth * scale, row.Height * 0.64f);
                StatIcons.Draw(g, si, iconRect, theme.TextMuted);

                var track = new RectangleF(row.X + (IconColumnWidth + IconGap) * scale, row.Y,
                    row.Width - (IconColumnWidth + IconGap) * scale, row.Height);
                if (track.Width <= 0) continue;

                var trackRadius = TrackRadius * scale;
                DrawingPrimitives.FillRoundedRect(g, theme.Track, track, trackRadius);

                // the bar shows the same level the row's text does, so the two cannot disagree
                var wildLevel = DisplayedWildLevel(creature, si, settings);
                var fraction = wildLevel <= 0 ? 0 : Math.Min(1f, wildLevel / (float)maxGraphLevel);
                if (fraction > 0)
                {
                    var barColor = settings.BarsByLevelQuality
                        // darkened a little so white text stays readable on the bright end of the ramp
                        ? Utils.GetColorFromPercent((int)(100 * fraction), settings.Theme == ModernThemes.Dark ? -0.35 : 0.15)
                        : theme.Accent;
                    DrawingPrimitives.FillTrackPortion(g, track, trackRadius, fraction, barColor);
                }

                var textRect = new RectangleF(track.X + TrackTextInset * scale, track.Y,
                    track.Width - 2 * TrackTextInset * scale, track.Height);
                // bold: this text is small and often sits on a colored bar, and a chat client that
                // downscales the card eats thin strokes first
                DrawingPrimitives.TextFitted(g, rowTexts[i], fonts,
                    rowFontSize, FontStyle.Bold, textRect, theme.TextOnBar,
                    StringAlignment.Center, StringAlignment.Center);
            }
        }

        private static void DrawColorChips(Graphics g, FontCache fonts, ModernTheme theme, Creature creature,
            List<int> colorRegions, RectangleF area, float scale, bool withNames)
        {
            if (withNames)
                DrawNamedColorRegions(g, fonts, theme, creature, colorRegions, area, scale);
            else
                DrawColorChipStrip(g, fonts, theme, creature, colorRegions, area, scale);
        }

        /// <summary>
        /// A single row of color chips carrying just the color id.
        /// </summary>
        private static void DrawColorChipStrip(Graphics g, FontCache fonts, ModernTheme theme, Creature creature,
            List<int> colorRegions, RectangleF row, float scale)
        {
            var count = colorRegions.Count;
            var gap = ChipGap * scale;
            // capped, a species with only two or three regions would otherwise stretch each chip
            // across half the card
            var chipWidth = Math.Min((row.Width - (count - 1) * gap) / count, MaxChipWidth * scale);
            if (chipWidth <= 0) return;

            for (var i = 0; i < count; i++)
                DrawChip(g, fonts, theme, colorRegions[i], creature.colors[colorRegions[i]], scale,
                    new RectangleF(row.X + i * (chipWidth + gap), row.Y, chipWidth, row.Height));
        }

        /// <summary>
        /// A grid of chip plus region name. Without artwork the name is the only thing that says
        /// which part of the creature a color belongs to.
        /// </summary>
        private static void DrawNamedColorRegions(Graphics g, FontCache fonts, ModernTheme theme, Creature creature,
            List<int> colorRegions, RectangleF area, float scale)
        {
            const int columns = NamedRegionColumns;
            var columnGap = ChipGap * scale;
            var columnWidth = (area.Width - (columns - 1) * columnGap) / columns;
            var chipWidth = Math.Min(NamedChipWidth * scale, columnWidth);
            if (columnWidth <= 0) return;

            var rowHeight = ChipHeight * scale;
            var rowGap = ChipRowGap * scale;

            for (var i = 0; i < colorRegions.Count; i++)
            {
                var ci = colorRegions[i];
                var cell = new RectangleF(
                    area.X + i % columns * (columnWidth + columnGap),
                    area.Y + i / columns * (rowHeight + rowGap),
                    columnWidth, rowHeight);

                DrawChip(g, fonts, theme, ci, creature.colors[ci], scale,
                    new RectangleF(cell.X, cell.Y, chipWidth, cell.Height));

                var name = creature.Species.colors?[ci]?.name;
                if (string.IsNullOrEmpty(name)) name = $"[{ci}]";
                var nameRect = new RectangleF(cell.X + chipWidth + ChipNameGap * scale, cell.Y,
                    cell.Width - chipWidth - ChipNameGap * scale, cell.Height);
                DrawingPrimitives.TextFitted(g, name, fonts, FontSizeChip * scale, FontStyle.Bold, nameRect,
                    theme.TextMuted, StringAlignment.Near, StringAlignment.Center);
            }
        }

        /// <summary>
        /// A color chip: the region index in a shaded notch, then the color id on the region's
        /// actual color. The index is needed because only the species' enabled regions get a chip,
        /// so the third chip is not necessarily region two.
        /// </summary>
        private static void DrawChip(Graphics g, FontCache fonts, ModernTheme theme, int regionIndex, byte colorId,
            float scale, RectangleF chipRect)
        {
            var chipColor = CreatureColors.CreatureColor(colorId);
            var radius = ChipRadius * scale;
            // both numbers sit on an arbitrary ARK color, so each picks its own contrasting ink
            var chipForeColor = DrawingPrimitives.ContrastingText(chipColor);

            DrawingPrimitives.FillRoundedRect(g, chipColor, chipRect, radius);

            // shading the notch rather than separating the numbers with a character keeps them from
            // being read as one number, and costs no width
            var notchWidth = Math.Min(chipRect.Width * 0.45f, RegionIndexWidth * scale);
            var notchRect = new RectangleF(chipRect.X, chipRect.Y, notchWidth, chipRect.Height);
            var notchColor = Utils.AdjustColorLight(chipColor, chipForeColor == Color.White ? 0.34 : -0.34);
            DrawingPrimitives.FillClippedToRoundedRect(g, notchColor, chipRect, radius, notchRect);

            DrawingPrimitives.DrawRoundedRect(g, Color.FromArgb(0x60, theme.TextMuted), Math.Max(1, scale),
                chipRect, radius);

            DrawingPrimitives.TextFitted(g, regionIndex.ToString(CultureInfo.InvariantCulture), fonts,
                FontSizeChip * scale, FontStyle.Bold, notchRect, DrawingPrimitives.ContrastingText(notchColor),
                StringAlignment.Center, StringAlignment.Center);

            var colorIdRect = new RectangleF(notchRect.Right, chipRect.Y, chipRect.Width - notchWidth,
                chipRect.Height);
            DrawingPrimitives.TextFitted(g, colorId.ToString(CultureInfo.InvariantCulture), fonts,
                FontSizeChip * scale, FontStyle.Bold, colorIdRect, chipForeColor,
                StringAlignment.Center, StringAlignment.Center);
        }

        /// <summary>
        /// Whether the color regions should be named. Falls back to the compact strip when the
        /// species has no region names to show anyway.
        /// </summary>
        private static bool RegionNamesWanted(ModernInfoGraphicSettings settings, bool hasSpeciesArtwork,
            Species species)
        {
            switch (settings.RegionNames)
            {
                case ColorRegionNameDisplays.Always:
                    break;
                case ColorRegionNameDisplays.WithoutArtwork when !hasSpeciesArtwork:
                    break;
                default:
                    return false;
            }

            if (species.colors == null) return false;
            foreach (var region in species.colors)
                if (!string.IsNullOrEmpty(region?.name))
                    return true;
            return false;
        }

        private static void DrawFooter(Graphics g, FontCache fonts, ModernTheme theme, Creature creature,
            RectangleF row, float scale, bool secondaryCulture)
        {
            DrawingPrimitives.TextFitted(g, Loc.S("Mutations", secondaryCulture: secondaryCulture) + ":", fonts,
                FontSizeFooter * scale, FontStyle.Bold, row, theme.TextMuted,
                StringAlignment.Near, StringAlignment.Center);

            var values =
                $"{Utils.SexSymbol(Sex.Male)} {creature.mutationsPaternal:N0}   {Utils.SexSymbol(Sex.Female)} {creature.mutationsMaternal:N0}";
            DrawingPrimitives.TextFitted(g, values, fonts, FontSizeFooter * scale, FontStyle.Bold, row,
                theme.AccentText, StringAlignment.Far, StringAlignment.Center);
        }

        #endregion

        #region text building

        /// <summary>
        /// The wild level as it is presented, which adds the mutated levels on top when they are
        /// not given a column of their own. Negative when the level is unknown.
        /// </summary>
        private static int DisplayedWildLevel(Creature creature, int statIndex, ModernInfoGraphicSettings settings)
        {
            var wild = LevelAt(creature.levelsWild, statIndex, -1);
            if (wild < 0 || !settings.SumWildAndMutatedLevels) return wild;

            var mutated = LevelAt(creature.levelsMutated, statIndex);
            return mutated > 0 ? wild + mutated : wild;
        }

        /// <summary>
        /// A creature's stat arrays are serialized with DefaultValueHandling.Ignore, so a creature
        /// loaded from the library can have them null or shorter than the stat count.
        /// </summary>
        private static int LevelAt(int[] levels, int statIndex, int fallback = 0) =>
            levels != null && statIndex >= 0 && statIndex < levels.Length ? levels[statIndex] : fallback;

        /// <summary>
        /// Stat value, or a negative number when it is not known. See <see cref="LevelAt"/>.
        /// </summary>
        private static double ValueAt(double[] values, int statIndex) =>
            values != null && statIndex >= 0 && statIndex < values.Length ? values[statIndex] : -1;

        /// <summary>
        /// Stat indices this species uses, without torpidity, in the order shown in game.
        /// </summary>
        private static List<int> UsedStats(Species species)
        {
            var list = new List<int>();
            foreach (var si in Stats.DisplayOrder)
            {
                if (si == Stats.Torpidity || !species.UsesStat(si)) continue;
                list.Add(si);
            }
            return list;
        }

        /// <summary>
        /// Regions the species uses, bounded by both arrays. Creature arrays are serialized with
        /// DefaultValueHandling.Ignore, so they can come back from the library shorter than
        /// expected or missing entirely.
        /// </summary>
        private static List<int> EnabledColorRegions(Species species, byte[] creatureColors)
        {
            var list = new List<int>();
            if (species.EnabledColorRegions == null || creatureColors == null) return list;

            var limit = Math.Min(Ark.ColorRegionCount,
                Math.Min(species.EnabledColorRegions.Length, creatureColors.Length));
            for (var ci = 0; ci < limit; ci++)
                if (species.EnabledColorRegions[ci])
                    list.Add(ci);
            return list;
        }

        /// <summary>
        /// The value part and the level triple of a stat row, e.g. "1,204.0 / 1,594.2  (38 | 30 | 0)".
        /// </summary>
        internal static string StatRowText(Creature creature, int si, ModernInfoGraphicSettings settings,
            bool showMutatedLevels)
        {
            var wild = DisplayedWildLevel(creature, si, settings);
            var dom = LevelAt(creature.levelsDom, si);
            var mutated = LevelAt(creature.levelsMutated, si);

            // wild, mutated, domesticated. Same order as in game and as the classic graphic's
            // W | M | D columns.
            var wildText = wild < 0 ? "?" : wild.ToString(CultureInfo.CurrentCulture);
            var levels = showMutatedLevels
                ? $"({wildText} | {mutated} | {dom})"
                : $"({wildText} | {dom})";

            var values = StatValueText(creature, si, settings);
            return string.IsNullOrEmpty(values) ? levels : $"{values}   {levels}";
        }

        private static string StatValueText(Creature creature, int si, ModernInfoGraphicSettings settings)
        {
            if (!settings.ShowStatValues) return null;

            var breeding = ValueAt(creature.valuesBreeding, si);
            var current = ValueAt(creature.valuesCurrent, si);

            switch (settings.ValueDisplay)
            {
                case StatValueDisplays.Breeding:
                    return FormatValue(breeding, si, true);
                case StatValueDisplays.Both:
                    // ASB has no damage state, so an identical pair carries no information
                    if (Math.Abs(breeding - current) < 0.05) return FormatValue(current, si, true);
                    return $"{FormatValue(breeding, si, false)} / {FormatValue(current, si, true)}";
                default:
                    return FormatValue(current, si, true);
            }
        }

        /// <summary>
        /// Says which values the stat rows carry. Without it a lone number is ambiguous: the
        /// breeding value and the current value of an undamaged wild creature look identical.
        /// Null when no values are shown and there is nothing to label.
        /// </summary>
        private static string ValuesCaptionText(ModernInfoGraphicSettings settings)
        {
            if (!settings.ShowStatValues) return null;

            switch (settings.ValueDisplay)
            {
                case StatValueDisplays.Breeding: return "Breeding values";
                case StatValueDisplays.Both: return "Breeding / current values";
                default: return "Current values";
            }
        }

        private static string FormatValue(double value, int si, bool withUnit)
        {
            if (value < 0) return "?";
            if (Stats.IsPercentage(si))
                return (100 * value).ToString("N1", CultureInfo.CurrentCulture) + (withUnit ? " %" : string.Empty);
            return value.ToString("N1", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Secondary facts that have no row of their own: generation, imprinting or taming
        /// effectiveness, neutered state and applied mutagen.
        /// </summary>
        private static string MetaText(Creature creature, CreatureCollection cc, ModernInfoGraphicSettings settings,
            bool secondaryCulture)
        {
            var parts = new List<string>();

            if (settings.ShowGeneration && creature.generation > 0)
                parts.Add($"{Loc.S("generation", secondaryCulture: secondaryCulture)} {creature.generation}");

            // Imprinting and taming effectiveness describe the tamed side of a creature, so like the
            // classic graphic they only appear when the current values are on show.
            // A bred creature with no imprinting at all adds nothing but noise, so it is left out.
            if (settings.ValueDisplay != StatValueDisplays.Breeding)
            {
                if (creature.imprintingBonus > 0)
                    parts.Add($"Imp {creature.imprintingBonus * 100:0.0} %");
                else if (creature.tamingEff >= 0)
                    parts.Add($"TE {creature.tamingEff * 100:0.0} %");
            }

            if (creature.flags.HasFlag(CreatureFlags.Neutered))
                parts.Add(Loc.S(creature.sex == Sex.Female ? "Spayed" : "Neutered", secondaryCulture: secondaryCulture));

            if (creature.flags.HasFlag(CreatureFlags.MutagenApplied))
                parts.Add("Mutagen");

            if (settings.ShowMaxWildLevel && cc != null)
                parts.Add($"{Loc.S("max wild level", secondaryCulture: secondaryCulture)} {cc.maxWildLevel}");

            return parts.Count == 0 ? null : string.Join(" · ", parts);
        }

        #endregion

        /// <summary>
        /// The colored species image. Falls back to the color region pie chart when the image pack
        /// has no artwork for this species, so the card never shows an empty panel.
        /// </summary>
        /// <returns>
        /// The image, and whether it is the real species artwork rather than the pie chart fallback.
        /// </returns>
        private static async Task<(Bitmap Image, bool IsSpeciesArtwork)> GetArtworkAsync(Creature creature,
            CreatureCollection cc, int size)
        {
            if (size <= 0) return (null, false);
            try
            {
                var image = (await CreatureColored.GetColoredCreatureAsync(creature.colors, creature.Species,
                        creature.Species.EnabledColorRegions, size, onlyImage: true, creatureSex: creature.sex,
                        game: cc?.Game)
                    .ConfigureAwait(false)).Bmp;
                if (image != null) return (image, true);

                var pieChart = (await CreatureColored.GetColoredCreatureAsync(creature.colors, creature.Species,
                        creature.Species.EnabledColorRegions, size, pieSize: size, onlyColors: true,
                        creatureSex: creature.sex, game: cc?.Game)
                    .ConfigureAwait(false)).Bmp;
                return (pieChart, false);
            }
            catch (Exception)
            {
                // a missing or broken image pack file must not prevent the graphic from being created
                return (null, false);
            }
        }
    }
}
