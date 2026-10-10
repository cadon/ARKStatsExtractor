using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ARKBreedingStats.InfoGraphic.Modern
{
    /// <summary>
    /// Painting helpers for the modern info graphic. Keeps <see cref="ModernInfoGraphic"/> readable
    /// by separating the how-to-paint from the what-goes-where.
    /// All coordinates are in device pixels of the (possibly supersampled) target bitmap.
    /// </summary>
    internal static class DrawingPrimitives
    {
        /// <summary>
        /// Path around a rectangle with rounded corners. Dispose after use.
        /// </summary>
        internal static GraphicsPath RoundedRect(RectangleF r, float radius)
        {
            var path = new GraphicsPath();
            radius = Math.Max(0, Math.Min(radius, Math.Min(r.Width, r.Height) / 2));
            if (radius <= 0.01f)
            {
                path.AddRectangle(r);
                return path;
            }

            var d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        internal static void FillRoundedRect(Graphics g, Color color, RectangleF r, float radius)
        {
            if (r.Width <= 0 || r.Height <= 0 || color.A == 0) return;
            using var path = RoundedRect(r, radius);
            using var b = new SolidBrush(color);
            g.FillPath(b, path);
        }

        internal static void DrawRoundedRect(Graphics g, Color color, float width, RectangleF r, float radius)
        {
            if (r.Width <= 0 || r.Height <= 0 || color.A == 0 || width <= 0) return;
            using var path = RoundedRect(r, radius);
            using var p = new Pen(color, width);
            g.DrawPath(p, path);
        }

        /// <summary>
        /// Fills a rounded rectangle with a vertical two stop gradient.
        /// </summary>
        internal static void FillRoundedRectGradient(Graphics g, Color top, Color bottom, RectangleF r, float radius)
        {
            if (r.Width <= 0 || r.Height <= 0) return;
            using var path = RoundedRect(r, radius);
            // inflate the brush rectangle slightly, a gradient brush that exactly matches the bounds
            // can produce a wrapped band of the opposite color along the edge.
            using var brush = new LinearGradientBrush(
                new RectangleF(r.X - 1, r.Y - 1, r.Width + 2, r.Height + 2), top, bottom, LinearGradientMode.Vertical);
            g.FillPath(brush, path);
        }

        /// <summary>
        /// Black or white, whichever reads better on the given color.
        ///
        /// Utils.ForeColor switches at a plain average luminance of 110, which is low: colors in
        /// the mid range get black text where white is easier to read. This compares the real
        /// contrast ratio of both against the sRGB relative luminance, which puts the crossover at
        /// about 186 on a grey scale.
        /// </summary>
        internal static Color ContrastingText(Color background)
        {
            var luminance = 0.2126 * Linear(background.R)
                            + 0.7152 * Linear(background.G)
                            + 0.0722 * Linear(background.B);

            var contrastWithWhite = 1.05 / (luminance + 0.05);
            var contrastWithBlack = (luminance + 0.05) / 0.05;
            return contrastWithWhite >= contrastWithBlack ? Color.White : Color.Black;

            double Linear(int channel)
            {
                var c = channel / 255.0;
                return c <= 0.03928 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
            }
        }

        /// <summary>
        /// Fills a rectangle, clipped to a rounded rectangle, so the fill picks up its rounded
        /// corners. Used for segments of a rounded chip.
        /// </summary>
        internal static void FillClippedToRoundedRect(Graphics g, Color color, RectangleF clipTo, float radius,
            RectangleF fill)
        {
            if (color.A == 0 || fill.Width <= 0 || fill.Height <= 0) return;

            var currentState = g.Save();
            using var path = RoundedRect(clipTo, radius);
            g.SetClip(path, CombineMode.Intersect);
            using (var brush = new SolidBrush(color))
                g.FillRectangle(brush, fill);
            g.Restore(currentState);
        }

        /// <summary>
        /// Fills a proportion of a rounded track, clipped so the fill keeps the track's rounded shape.
        /// A slight vertical gradient gives the bar some depth.
        /// </summary>
        internal static void FillTrackPortion(Graphics g, RectangleF track, float radius, float fraction, Color color)
        {
            if (color.A == 0 || track.Width <= 0 || track.Height <= 0) return;
            fraction = Math.Max(0, Math.Min(1, fraction));
            if (fraction <= 0) return;

            var currentState = g.Save();
            using var path = RoundedRect(track, radius);
            g.SetClip(path, CombineMode.Intersect);

            var fillRect = new RectangleF(track.X, track.Y, track.Width * fraction, track.Height);
            using (var brush = new LinearGradientBrush(
                       new RectangleF(fillRect.X - 1, fillRect.Y - 1, fillRect.Width + 2, fillRect.Height + 2),
                       Utils.AdjustColorLight(color, 0.14), Utils.AdjustColorLight(color, -0.14),
                       LinearGradientMode.Vertical))
                g.FillRectangle(brush, fillRect);

            g.Restore(currentState);
        }

        /// <summary>
        /// Draws text inside a rectangle, shrinking the font until it fits the available width.
        /// Returns the size actually used. The supplied font is never disposed by this method.
        /// </summary>
        internal static SizeF TextFitted(Graphics g, string text, FontCache fonts, float fontSize, FontStyle style,
            RectangleF bounds, Color color, StringAlignment horizontal = StringAlignment.Near,
            StringAlignment vertical = StringAlignment.Center, float minFontSize = 5f)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || color.A == 0) return SizeF.Empty;

            using var format = new StringFormat(StringFormat.GenericTypographic)
            {
                Alignment = horizontal,
                LineAlignment = vertical,
                FormatFlags = StringFormatFlags.NoWrap,
                Trimming = StringTrimming.EllipsisCharacter
            };

            var font = fonts.Get(FittedFontSize(g, text, fonts, fontSize, style, bounds.Width, minFontSize), style);
            var size = g.MeasureString(text, font, int.MaxValue, format);

            using var brush = new SolidBrush(color);
            g.DrawString(text, font, brush, bounds, format);
            return size;
        }

        /// <summary>
        /// The size <see cref="TextFitted"/> would use for this text, without drawing it.
        /// Lets a caller find one size that suits a whole group of rows, so the numbers do not jump
        /// between a short row and a long one.
        /// </summary>
        internal static float FittedFontSize(Graphics g, string text, FontCache fonts, float fontSize,
            FontStyle style, float availableWidth, float minFontSize = 5f)
        {
            if (string.IsNullOrEmpty(text) || availableWidth <= 0) return fontSize;

            using var format = new StringFormat(StringFormat.GenericTypographic)
            { FormatFlags = StringFormatFlags.NoWrap };
            var width = g.MeasureString(text, fonts.Get(fontSize, style), int.MaxValue, format).Width;

            // Shrink until it fits. A single proportional step undershoots, because glyph advances
            // do not scale linearly with the font size, and the text then gets an ellipsis.
            var attempts = 0;
            while (width > availableWidth && fontSize > minFontSize && attempts++ < 12)
            {
                fontSize = Math.Max(minFontSize, Math.Min(fontSize - 1, fontSize * availableWidth / width));
                width = g.MeasureString(text, fonts.Get(fontSize, style), int.MaxValue, format).Width;
            }
            return fontSize;
        }

        /// <summary>
        /// Measures text without drawing it, using the same format as <see cref="TextFitted"/>.
        /// </summary>
        internal static SizeF MeasureText(Graphics g, string text, FontCache fonts, float fontSize, FontStyle style)
        {
            if (string.IsNullOrEmpty(text)) return SizeF.Empty;
            using var format = new StringFormat(StringFormat.GenericTypographic)
            { FormatFlags = StringFormatFlags.NoWrap };
            return g.MeasureString(text, fonts.Get(fontSize, style), int.MaxValue, format);
        }

    }

    /// <summary>
    /// Creates fonts on demand and disposes them all at the end of a render.
    /// Fonts are created in pixel units so the design constants are true pixels.
    /// </summary>
    internal sealed class FontCache : IDisposable
    {
        private readonly FontFamily _family;
        private readonly bool _ownsFamily;
        private readonly Dictionary<(int, FontStyle), Font> _fonts = new();

        internal FontCache(string fontName)
        {
            try
            {
                _family = new FontFamily(string.IsNullOrWhiteSpace(fontName) ? Asb.DefaultFontName : fontName);
                _ownsFamily = true;
            }
            catch (ArgumentException)
            {
                // the configured font is not installed
                _family = FontFamily.GenericSansSerif;
                _ownsFamily = false;
            }
        }

        /// <summary>
        /// Font at the given pixel size. Falls back to a style the family supports.
        /// </summary>
        internal Font Get(float sizePx, FontStyle style)
        {
            // quantize to whole pixels, otherwise a debounced preview creates a font per keystroke
            var key = ((int)Math.Max(1, Math.Round(sizePx)), style);
            if (_fonts.TryGetValue(key, out var font)) return font;

            if (!_family.IsStyleAvailable(style))
                style = _family.IsStyleAvailable(FontStyle.Regular) ? FontStyle.Regular : FontStyle.Bold;

            font = new Font(_family, key.Item1, style, GraphicsUnit.Pixel);
            _fonts[key] = font;
            return font;
        }

        public void Dispose()
        {
            foreach (var font in _fonts.Values) font.Dispose();
            _fonts.Clear();
            if (_ownsFamily) _family.Dispose();
        }
    }

    /// <summary>
    /// Walks down a rectangle handing out rows. Replaces the hand computed y offsets of the
    /// classic renderer, so the layout code reads in the same order it paints.
    /// </summary>
    internal struct LayoutCursor
    {
        private RectangleF _remaining;

        internal LayoutCursor(RectangleF area)
        {
            _remaining = area;
        }

        internal RectangleF Remaining => _remaining;

        /// <summary>
        /// Takes a row of the given height off the top.
        /// </summary>
        internal RectangleF Row(float height)
        {
            var row = new RectangleF(_remaining.X, _remaining.Y, _remaining.Width, height);
            _remaining = new RectangleF(_remaining.X, _remaining.Y + height, _remaining.Width,
                Math.Max(0, _remaining.Height - height));
            return row;
        }

        /// <summary>
        /// Skips vertical space without returning it.
        /// </summary>
        internal void Skip(float height) => Row(height);
    }
}
