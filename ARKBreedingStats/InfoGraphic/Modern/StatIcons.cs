using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ARKBreedingStats.InfoGraphic.Modern
{
    /// <summary>
    /// Vector icons for the stat rows of the modern info graphic.
    /// Authored as <see cref="GraphicsPath"/> shapes rather than bitmaps so they stay crisp at any
    /// supersample factor, can be tinted to the theme, and add no binary assets to the repository.
    /// Every shape is drawn in a 100x100 design box and then transformed into the target rectangle.
    /// </summary>
    internal static class StatIcons
    {
        private const float DesignSize = 100f;

        /// <summary>
        /// Draws the icon of the given stat, centered and aspect preserved inside <paramref name="bounds"/>.
        /// </summary>
        internal static void Draw(Graphics g, int statIndex, RectangleF bounds, Color color)
        {
            if (color.A == 0 || bounds.Width <= 0 || bounds.Height <= 0) return;

            using var path = CreatePath(statIndex);
            if (path == null) return;

            var size = Math.Min(bounds.Width, bounds.Height);
            var scale = size / DesignSize;
            using var matrix = new Matrix();
            matrix.Translate(bounds.X + (bounds.Width - size) / 2, bounds.Y + (bounds.Height - size) / 2);
            matrix.Scale(scale, scale);
            path.Transform(matrix);

            using var brush = new SolidBrush(color);
            g.FillPath(brush, path);
        }

        /// <summary>
        /// The icon shape in design coordinates. Dispose after use. Null if the stat has no icon.
        /// </summary>
        private static GraphicsPath CreatePath(int statIndex)
        {
            switch (statIndex)
            {
                case Stats.Health: return Health();
                case Stats.Stamina: return Stamina();
                case Stats.Torpidity: return Torpidity();
                case Stats.Oxygen: return Oxygen();
                case Stats.Food: return Food();
                case Stats.Water: return Water();
                case Stats.Temperature: return Temperature();
                case Stats.Weight: return Weight();
                case Stats.MeleeDamageMultiplier: return Melee();
                case Stats.SpeedMultiplier: return Speed();
                case Stats.TemperatureFortitude: return Fortitude();
                case Stats.CraftingSpeedMultiplier: return CraftingSpeed();
                default: return Fallback();
            }
        }

        /// <summary>A medical cross.</summary>
        private static GraphicsPath Health()
        {
            var p = new GraphicsPath();
            p.AddPolygon(new[]
            {
                new PointF(36, 8), new PointF(64, 8), new PointF(64, 36), new PointF(92, 36),
                new PointF(92, 64), new PointF(64, 64), new PointF(64, 92), new PointF(36, 92),
                new PointF(36, 64), new PointF(8, 64), new PointF(8, 36), new PointF(36, 36)
            });
            return p;
        }

        /// <summary>A lightning bolt.</summary>
        private static GraphicsPath Stamina()
        {
            var p = new GraphicsPath();
            p.AddPolygon(new[]
            {
                new PointF(62, 4), new PointF(24, 58), new PointF(46, 58), new PointF(36, 96),
                new PointF(78, 40), new PointF(54, 40)
            });
            return p;
        }

        /// <summary>
        /// Two sleeping Zs. Built from rectangles rather than from a stroked spiral like the in game
        /// symbol, because widening a spiral into a fillable outline self intersects and fills solid
        /// at icon sizes.
        /// </summary>
        private static GraphicsPath Torpidity()
        {
            var p = new GraphicsPath { FillMode = FillMode.Winding };
            AddZ(p, 36, 4, 56, 50, 13);
            AddZ(p, 6, 56, 38, 38, 10);
            return p;

            // top bar, diagonal, bottom bar
            void AddZ(GraphicsPath path, float x, float y, float w, float h, float t)
            {
                path.AddRectangle(new RectangleF(x, y, w, t));
                path.AddRectangle(new RectangleF(x, y + h - t, w, t));
                path.AddPolygon(new[]
                {
                    new PointF(x + w - t, y + t), new PointF(x + w, y + t),
                    new PointF(x + t, y + h - t), new PointF(x, y + h - t)
                });
            }
        }

        /// <summary>
        /// Three rising bubbles of decreasing size. Solid discs rather than outlines, thin rings
        /// wash out at icon size.
        /// </summary>
        private static GraphicsPath Oxygen()
        {
            // sizes decrease going up, so they read as bubbles rising rather than as three dots
            var p = new GraphicsPath { FillMode = FillMode.Winding };
            p.AddEllipse(4, 46, 50, 50);
            p.AddEllipse(52, 30, 32, 32);
            p.AddEllipse(62, 4, 20, 20);
            return p;
        }

        /// <summary>
        /// A drumstick. The meat has to stay much wider than the bone end, or the silhouette reads
        /// as a tool rather than as food.
        /// </summary>
        private static GraphicsPath Food()
        {
            var p = new GraphicsPath { FillMode = FillMode.Winding };
            p.AddEllipse(34, 4, 62, 62);

            // the shaft starts inside the meat so the two shapes merge into one silhouette
            using (var bone = new GraphicsPath())
            {
                bone.AddLine(52, 52, 24, 82);
                using var pen = new Pen(Color.Black, 13) { StartCap = LineCap.Round, EndCap = LineCap.Round };
                bone.Widen(pen);
                p.AddPath(bone, false);
            }

            // the knuckle, two lobes perpendicular to the shaft and only a little wider than it
            p.AddEllipse(7, 66, 20, 20);
            p.AddEllipse(21, 78, 20, 20);
            return p;
        }

        /// <summary>A droplet.</summary>
        private static GraphicsPath Water()
        {
            var p = new GraphicsPath();
            p.AddArc(20, 34, 60, 60, 20, 140);
            p.AddBezier(new PointF(21.8f, 74.3f), new PointF(22, 44), new PointF(38, 24), new PointF(50, 8));
            p.AddBezier(new PointF(50, 8), new PointF(62, 24), new PointF(78, 44), new PointF(78.2f, 74.3f));
            p.CloseFigure();
            return p;
        }

        /// <summary>A thermometer.</summary>
        private static GraphicsPath Temperature()
        {
            var p = new GraphicsPath { FillMode = FillMode.Winding };
            p.AddEllipse(32, 62, 36, 36);
            using var stem = DrawingPrimitives.RoundedRect(new RectangleF(42, 6, 16, 66), 8);
            p.AddPath(stem, false);
            return p;
        }

        /// <summary>A weight with a handle.</summary>
        private static GraphicsPath Weight()
        {
            var p = new GraphicsPath { FillMode = FillMode.Winding };
            p.AddPolygon(new[]
            {
                new PointF(28, 40), new PointF(72, 40), new PointF(88, 94), new PointF(12, 94)
            });
            // handle, a stroked half ring
            using var handle = new GraphicsPath();
            handle.AddArc(32, 14, 36, 36, 180, 180);
            using var pen = new Pen(Color.Black, 10) { StartCap = LineCap.Flat, EndCap = LineCap.Flat };
            handle.Widen(pen);
            p.AddPath(handle, false);
            return p;
        }

        /// <summary>A sword.</summary>
        private static GraphicsPath Melee()
        {
            var p = new GraphicsPath { FillMode = FillMode.Winding };
            p.AddPolygon(new[]
            {
                new PointF(50, 0), new PointF(67, 24), new PointF(67, 56), new PointF(33, 56),
                new PointF(33, 24)
            });
            p.AddRectangle(new RectangleF(18, 56, 64, 13));
            p.AddRectangle(new RectangleF(42, 69, 16, 18));
            p.AddEllipse(37, 82, 26, 18);
            return p;
        }

        /// <summary>Three chevrons.</summary>
        private static GraphicsPath Speed()
        {
            var p = new GraphicsPath { FillMode = FillMode.Winding };
            foreach (var x in new float[] { 4, 30, 56 })
            {
                p.AddPolygon(new[]
                {
                    new PointF(x, 20), new PointF(x + 18, 50), new PointF(x, 80),
                    new PointF(x + 12, 80), new PointF(x + 30, 50), new PointF(x + 12, 20)
                });
            }
            return p;
        }

        /// <summary>A shield.</summary>
        private static GraphicsPath Fortitude()
        {
            var p = new GraphicsPath();
            p.AddLine(50, 4, 88, 18);
            p.AddLine(88, 18, 88, 50);
            p.AddBezier(new PointF(88, 50), new PointF(88, 76), new PointF(66, 90), new PointF(50, 97));
            p.AddBezier(new PointF(50, 97), new PointF(34, 90), new PointF(12, 76), new PointF(12, 50));
            p.AddLine(12, 50, 12, 18);
            p.CloseFigure();
            return p;
        }

        /// <summary>A cog.</summary>
        private static GraphicsPath CraftingSpeed()
        {
            const int teeth = 8;
            const float outerRadius = 48;
            const float innerRadius = 36;
            const float center = 50;

            var points = new PointF[teeth * 4];
            var i = 0;
            for (var t = 0; t < teeth; t++)
            {
                // each tooth is a tip pair followed by a root pair
                var baseAngle = t * 2 * Math.PI / teeth;
                var toothHalf = Math.PI / teeth * 0.36;
                var rootHalf = Math.PI / teeth * 0.64;

                points[i++] = Polar(center, baseAngle - toothHalf, outerRadius);
                points[i++] = Polar(center, baseAngle + toothHalf, outerRadius);
                points[i++] = Polar(center, baseAngle + rootHalf, innerRadius);
                points[i++] = Polar(center, baseAngle + 2 * Math.PI / teeth - rootHalf, innerRadius);
            }

            var p = new GraphicsPath { FillMode = FillMode.Alternate };
            p.AddPolygon(points);
            p.AddEllipse(center - 15, center - 15, 30, 30);
            return p;

            PointF Polar(float c, double angle, float radius) =>
                new PointF(c + (float)(radius * Math.Cos(angle)), c + (float)(radius * Math.Sin(angle)));
        }

        /// <summary>Used for stat indices without a dedicated icon.</summary>
        private static GraphicsPath Fallback()
        {
            var p = new GraphicsPath { FillMode = FillMode.Alternate };
            p.AddEllipse(14, 14, 72, 72);
            p.AddEllipse(30, 30, 40, 40);
            return p;
        }
    }
}
