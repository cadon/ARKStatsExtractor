using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ARKBreedingStats
{
    class CreatureColored
    {
        public static Bitmap getColoredCreature(int[] colorIds, string species, bool[] enabledColorRegions, int size = 128, int pieSize = 64, bool onlyColors = false)
        {

            float[][] hsl = new float[6][];
            int[][] rgb = new int[6][];
            for (int c = 0; c < 6; c++)
            {
                Color cl = Utils.creatureColor(colorIds[c]);
                rgb[c] = new int[] { cl.R, cl.G, cl.B };
                float mx = Math.Max(cl.R, Math.Max(cl.G, cl.B)) / 255f;
                float mn = Math.Min(cl.R, Math.Min(cl.G, cl.B)) / 255f;
                float s = mx - mn;
                if (s > 0)
                {
                    s /= (mx + mn <= 1) ? (mx + mn) : (2 - mx - mn);
                }
                hsl[c] = new float[] { cl.GetHue(), s, (Math.Max(cl.R, (Math.Max(cl.G, cl.B))) + Math.Min(cl.R, Math.Min(cl.G, cl.B))) / 510f };
            }
            Bitmap bm = new Bitmap(size, size);
            Graphics graph = Graphics.FromImage(bm);
            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            string imgFolder = "img\\";
            if (!onlyColors && System.IO.File.Exists(imgFolder + species + ".png") && System.IO.File.Exists(imgFolder + species + "_m.png"))
            {
                //bm = new Bitmap(imgFolder + species + ".png");
                graph.CompositingMode = CompositingMode.SourceCopy;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graph.SmoothingMode = SmoothingMode.HighQuality;
                graph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graph.DrawImage(new Bitmap(imgFolder + species + ".png"), 0, 0, size, size);
                Bitmap mask = new Bitmap(size, size);
                graph.Dispose();
                graph = Graphics.FromImage(mask);
                graph.DrawImage(new Bitmap(imgFolder + species + "_m.png"), 0, 0, size, size);
                float o = 0, l;
                Color c = Color.Black, bc = Color.Black;
                int r, g, b, rMix, gMix, bMix;
                for (int i = 0; i < bm.Width; i++)
                {
                    for (int j = 0; j < bm.Height; j++)
                    {
                        bc = bm.GetPixel(i, j);
                        if (bc.A > 0)
                        {
                            r = mask.GetPixel(i, j).R;
                            g = mask.GetPixel(i, j).G;
                            b = mask.GetPixel(i, j).B;
                            l = (Math.Max(bc.R, (Math.Max(bc.G, bc.B))) + Math.Min(bc.R, Math.Min(bc.G, bc.B))) / 510f;
                            //w = (l >= .5 && l < .7 ? .7f - l : (l > .3 && l < .5 ? l - .3f : 0));
                            for (int m = 0; m < 6; m++)
                            {
                                if (!enabledColorRegions[m] || colorIds[m] == 0)
                                    continue;
                                switch (m)
                                {
                                    case 0:
                                        o = Math.Max(0, r - g - b) / 255f;
                                        break;
                                    case 1:
                                        o = Math.Max(0, g - r - b) / 255f;
                                        break;
                                    case 2:
                                        o = Math.Max(0, b - r - g) / 255f;
                                        break;
                                    case 3:
                                        o = Math.Min(g, b) / 255f;
                                        break;
                                    case 4:
                                        o = Math.Min(r, g) / 255f;
                                        break;
                                    case 5:
                                        o = Math.Min(r, b) / 255f;
                                        break;
                                }
                                if (o == 0)
                                    continue;
                                //c = HSLtoRGB(hsl[m][0], hsl[m][1], l);
                                rMix = bc.R + rgb[m][0] - 128;
                                if (rMix < 0) rMix = 0;
                                else if (rMix > 255) rMix = 255;
                                gMix = bc.G + rgb[m][1] - 128;
                                if (gMix < 0) gMix = 0;
                                else if (gMix > 255) gMix = 255;
                                bMix = bc.B + rgb[m][2] - 128;
                                if (bMix < 0) bMix = 0;
                                else if (bMix > 255) bMix = 255;
                                c = Color.FromArgb(rMix, gMix, bMix);
                                bc = Color.FromArgb(bc.A, (int)(o * c.R + (1 - o) * bc.R), (int)(o * c.G + (1 - o) * bc.G), (int)(o * c.B + (1 - o) * bc.B));
                            }
                            bm.SetPixel(i, j, bc);
                        }
                    }
                }
            }
            else
            {
                Brush b = new SolidBrush(Color.Black);
                int pieAngle = enabledColorRegions.Count(c => c);
                pieAngle = 360 / (pieAngle > 0 ? pieAngle : 1);
                int pieNr = 0;
                for (int c = 0; c < 6; c++)
                {
                    if (enabledColorRegions[c])
                    {
                        if (colorIds[c] > 0)
                        {
                            b = new SolidBrush(Utils.creatureColor(colorIds[c]));
                            graph.FillPie(b, (size - pieSize) / 2, (size - pieSize) / 2, pieSize, pieSize, pieNr * pieAngle + 270, pieAngle);
                        }
                        pieNr++;
                    }
                }
                graph.DrawEllipse(new Pen(Color.Gray), (size - pieSize) / 2, (size - pieSize) / 2, pieSize, pieSize);
                b.Dispose();
            }
            graph.Dispose();
            return bm;
        }

        private static Color HSLtoRGB(float h, float s, float l)
        {
            double r = 0;
            double g = 0;
            double b = 0;

            if (s == 0)
            {
                // If s == 0 color is grey
                r = l;
                g = l;
                b = l;
            }
            else
            {
                double c;
                double m;
                double x1, x2, sv;

                double sectorFrac, sectorPos;
                int sectorNr;

                // The color wheel has 6 sectors. Set sectorNr and the fraction
                sectorPos = h / 60;
                sectorNr = (int)(Math.Floor(sectorPos));
                sectorFrac = sectorPos - sectorNr;

                // Calculate three helper variables
                c = (l <= 0.5) ? (l * (1.0 + s)) : (l + s - l * s);
                m = l + l - c;
                sv = (c - m) * sectorFrac;
                x1 = m + sv;
                x2 = c - sv;

                // set r,g,b according to the sector
                switch (sectorNr)
                {
                    case 0:
                        r = c;
                        g = x1;
                        b = m;
                        break;
                    case 1:
                        r = x2;
                        g = c;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = c;
                        b = x1;
                        break;
                    case 3:
                        r = m;
                        g = x2;
                        b = c;
                        break;
                    case 4:
                        r = x1;
                        g = m;
                        b = c;
                        break;
                    case 5:
                        r = c;
                        g = m;
                        b = x2;
                        break;
                }
            }
            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }
    }
}
