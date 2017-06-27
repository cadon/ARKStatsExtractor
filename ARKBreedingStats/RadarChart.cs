using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    class RadarChart : PictureBox
    {
        public int maxLevel; // outer border of graph
        private List<Point> maxPs, ps; // coords of outer points
        private int maxR, xm, ym; // max-radius,centerx,centery
        private int[] oldLevels;
        private PathGradientBrush grBrushBG, grBrushFG;
        private int step;

        public RadarChart()
        {
            initializeVariables(50);
            SizeMode = PictureBoxSizeMode.Zoom;
        }

        public void initializeVariables(int maxLevel)
        {
            this.maxLevel = maxLevel;
            maxR = Math.Min(Width, Height) / 2;
            xm = maxR + 1;
            ym = maxR + 1;
            maxR -= 15;
            double angleSeven = Math.PI / 3.5;
            double offset = Math.PI / 2;

            step = (int)Math.Round(maxLevel / 25d);
            if (step < 1) step = 1;
            step *= 5;

            maxPs = new List<Point>();
            ps = new List<Point>();
            oldLevels = new int[7];
            int r = 0;
            for (int s = 0; s < 7; s++)
            {
                double angle = angleSeven * s - offset;
                ps.Add(new Point(xm + (int)(r * Math.Cos(angle)), ym + (int)(r * Math.Sin(angle))));

                maxPs.Add(new Point(xm + (int)(maxR * Math.Cos(angle)), ym + (int)(maxR * Math.Sin(angle))));
            }

            // colorgradient
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(xm - maxR, ym - maxR, 2 * maxR + 1, 2 * maxR + 1);
            grBrushBG = new PathGradientBrush(path);
            grBrushFG = new PathGradientBrush(path);

            Color[] colorsBG = {
            Color.FromArgb(0, 90, 0),
            Color.FromArgb(90, 90, 0),
            Color.FromArgb(90, 0, 0)};

            Color[] colorsFG = {
            Color.FromArgb(0, 180, 0),
            Color.FromArgb(180, 180, 0),
            Color.FromArgb(180, 0, 0)};

            float[] relativePositions = { 0, 0.45f, 1 };

            ColorBlend colorBlendBG = new ColorBlend();
            colorBlendBG.Colors = colorsBG;
            colorBlendBG.Positions = relativePositions;
            grBrushBG.InterpolationColors = colorBlendBG;

            ColorBlend colorBlendFG = new ColorBlend();
            colorBlendFG.Colors = colorsFG;
            colorBlendFG.Positions = relativePositions;
            grBrushFG.InterpolationColors = colorBlendFG;

            setLevels(new int[7]);
        }

        public void setLevels(int[] levels)
        {
            if (levels != null && levels.Length > 6 && maxR > 5)
            {
                Bitmap bmp = new Bitmap(Width, Height);
                Graphics g = Graphics.FromImage(bmp);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                double angleSeven = Math.PI / 3.5;
                double offset = Math.PI / 2;

                for (int s = 0; s < 7; s++)
                {
                    if (levels[s] != oldLevels[s])
                    {
                        oldLevels[s] = levels[s];
                        int r = levels[s] * maxR / maxLevel;
                        if (r < 0) r = 0;
                        if (r > maxR) r = maxR;
                        double angle = angleSeven * s - offset;
                        ps[s] = new Point(xm + (int)(r * Math.Cos(angle)), ym + (int)(r * Math.Sin(angle)));
                    }
                }
                g.FillEllipse(grBrushBG, xm - maxR, ym - maxR, 2 * maxR + 1, 2 * maxR + 1);

                Pen penL = new Pen(Color.FromArgb(128, 255, 255, 255));
                g.FillPolygon(grBrushFG, ps.ToArray());
                g.DrawPolygon(penL, ps.ToArray());

                double stepFactor = (double)step / maxLevel;
                for (int r = 0; r < 5; r++)
                    g.DrawEllipse(new Pen(Utils.getColorFromPercent((int)(100 * r * stepFactor), -0.4)), (int)(xm - maxR * r * stepFactor), (int)(ym - maxR * r * stepFactor), (int)(2 * maxR * r * stepFactor + 1), (int)(2 * maxR * r * stepFactor + 1));
                g.DrawEllipse(new Pen(Utils.getColorFromPercent(100, -0.4)), xm - maxR, ym - maxR, 2 * maxR + 1, 2 * maxR + 1);

                Pen pen = new Pen(Color.Black);
                for (int s = 0; s < 7; s++)
                {
                    pen.Width = 1;
                    pen.Color = Color.Gray;
                    g.DrawLine(pen, xm, ym, maxPs[s].X, maxPs[s].Y);
                    Color cl = Utils.getColorFromPercent(100 * levels[s] / maxLevel);
                    pen.Color = cl;
                    pen.Width = 3;
                    g.DrawLine(pen, xm, ym, ps[s].X, ps[s].Y);
                    Brush b = new SolidBrush(cl);
                    g.FillEllipse(b, ps[s].X - 4, ps[s].Y - 4, 8, 8);
                    g.DrawEllipse(penL, ps[s].X - 4, ps[s].Y - 4, 8, 8);
                    b.Dispose();
                }
                for (int r = 1; r < 5; r++)
                    g.DrawString((step * r).ToString("N0"), new System.Drawing.Font("Microsoft Sans Serif", 8f), new System.Drawing.SolidBrush(Color.FromArgb(190, 255, 255, 255)), xm - 8, ym - 6 + r * maxR / 5);
                g.DrawString((maxLevel).ToString("N0"), new System.Drawing.Font("Microsoft Sans Serif", 8f), new System.Drawing.SolidBrush(Color.FromArgb(190, 255, 255, 255)), xm - 8, ym - 11 + maxR);

                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                for (int s = 0; s < 7; s++)
                {
                    double angle = angleSeven * s - offset;
                    g.DrawString(Utils.statName(s, true), new System.Drawing.Font("Microsoft Sans Serif", 8f), new System.Drawing.SolidBrush(Color.Black), xm - 9 + (int)((maxR + 10) * Math.Cos(angle)), ym - 5 + (int)((maxR + 10) * Math.Sin(angle)));
                }

                g.Dispose();

                Image = bmp;
            }
        }
    }
}
