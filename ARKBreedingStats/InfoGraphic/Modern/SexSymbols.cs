using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using ARKBreedingStats.Library;

namespace ARKBreedingStats.InfoGraphic.Modern
{
    /// <summary>
    /// Vector versions of the male and female symbols for the corner badge.
    /// The font glyphs have thin strokes that wash out at badge size, these are drawn with a
    /// controlled weight instead. Same 100x100 design box as <see cref="StatIcons"/>.
    /// </summary>
    internal static class SexSymbols
    {
        private const float DesignSize = 100f;

        /// <summary>
        /// Draws the symbol centered in the bounds. Returns false when nothing was drawn, so the
        /// caller can fall back to the text glyph.
        /// </summary>
        internal static bool Draw(Graphics g, Sex sex, RectangleF bounds, Color color)
        {
            if (color.A == 0 || bounds.Width <= 0 || bounds.Height <= 0) return false;

            var parts = CreateParts(sex);
            if (parts == null) return false;

            try
            {
                var size = Math.Min(bounds.Width, bounds.Height);
                using var matrix = new Matrix();
                matrix.Translate(bounds.X + (bounds.Width - size) / 2, bounds.Y + (bounds.Height - size) / 2);
                matrix.Scale(size / DesignSize, size / DesignSize);

                using var brush = new SolidBrush(color);
                foreach (var part in parts)
                {
                    part.Transform(matrix);
                    g.FillPath(brush, part);
                }
            }
            finally
            {
                foreach (var part in parts) part.Dispose();
            }

            return true;
        }

        /// <summary>
        /// The symbol as separate paths. The ring needs an alternate fill to leave a hole and the
        /// rest needs a winding fill to union, so they cannot share one path.
        /// </summary>
        private static List<GraphicsPath> CreateParts(Sex sex)
        {
            return sex switch
            {
                Sex.Female => Female(),
                Sex.Male => Male(),
                _ => Unknown()
            };
        }

        private static List<GraphicsPath> Female()
        {
            var ring = new GraphicsPath { FillMode = FillMode.Alternate };
            ring.AddEllipse(24, 2, 52, 52);
            ring.AddEllipse(35, 13, 30, 30);

            var cross = new GraphicsPath { FillMode = FillMode.Winding };
            cross.AddRectangle(new RectangleF(43, 50, 14, 46));
            cross.AddRectangle(new RectangleF(28, 68, 44, 13));

            return [ring, cross];
        }

        /// <summary>
        /// A question mark, for creatures whose sex was never determined. Drawn rather than set in
        /// the font so it carries the same weight as the other two.
        /// </summary>
        private static List<GraphicsPath> Unknown()
        {
            var hook = new GraphicsPath();
            hook.AddArc(28, 8, 44, 44, 180, 250);
            hook.AddLine(57.5f, 50.7f, 50, 64);
            using (var pen = new Pen(Color.Black, 15) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                hook.Widen(pen);
            // after Widen, it resets the fill mode and the widened outline self intersects
            hook.FillMode = FillMode.Winding;

            var dot = new GraphicsPath();
            dot.AddEllipse(41, 76, 18, 18);

            return [hook, dot];
        }

        private static List<GraphicsPath> Male()
        {
            var ring = new GraphicsPath { FillMode = FillMode.Alternate };
            ring.AddEllipse(4, 40, 56, 56);
            ring.AddEllipse(16, 52, 32, 32);

            var arrow = new GraphicsPath { FillMode = FillMode.Alternate };
            // shaft, from the ring up to the arrow head
            arrow.AddPolygon(new[]
            {
                new PointF(48f, 45f), new PointF(55f, 52f),
                new PointF(82f, 25f), new PointF(95f, 40f),
                new PointF(95f, 5f), new PointF(60f, 5f),
                new PointF(78f, 15f)
            });

            return [ring, arrow];
        }
    }
}
