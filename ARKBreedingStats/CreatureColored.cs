using ARKBreedingStats.species;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace ARKBreedingStats
{
    class CreatureColored
    {
        public static Bitmap getColoredCreature(int[] colorIds, string species, bool[] enabledColorRegions, int size = 128, int pieSize = 64, bool onlyColors = false, bool dontCache = false)
        {
            //float[][] hsl = new float[6][];
            int[][] rgb = new int[6][];
            for (int c = 0; c < 6; c++)
            {
                Color cl = CreatureColors.creatureColor(colorIds[c]);
                rgb[c] = new int[] { cl.R, cl.G, cl.B };
            }
            Bitmap bm = new Bitmap(size, size);
            Graphics graph = Graphics.FromImage(bm);
            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            string imgFolder = "img/";
            // cachefilename
            string cf = imgFolder + "cache/" + species.Substring(0, Math.Min(species.Length, 5)) + "_" + (species + string.Join("", colorIds.Select(i => i.ToString()).ToArray())).GetHashCode().ToString("X8") + ".png";
            if (!onlyColors && System.IO.File.Exists(imgFolder + species + ".png") && System.IO.File.Exists(imgFolder + species + "_m.png"))
            {
                if (!System.IO.File.Exists(cf))
                {
                    int defaultSizeOfTemplates = 256;
                    Bitmap bmC = new Bitmap(imgFolder + species + ".png");
                    graph.DrawImage(new Bitmap(imgFolder + species + ".png"), 0, 0, defaultSizeOfTemplates, defaultSizeOfTemplates);
                    Bitmap mask = new Bitmap(defaultSizeOfTemplates, defaultSizeOfTemplates);
                    Graphics.FromImage(mask).DrawImage(new Bitmap(imgFolder + species + "_m.png"), 0, 0, defaultSizeOfTemplates, defaultSizeOfTemplates);
                    float o = 0, l;
                    Color c = Color.Black, bc = Color.Black;
                    int r, g, b, rMix, gMix, bMix;
                    bool imageFine = false;
                    try
                    {
                        for (int i = 0; i < bmC.Width; i++)
                        {
                            for (int j = 0; j < bmC.Height; j++)
                            {
                                bc = bmC.GetPixel(i, j);
                                if (bc.A > 0)
                                {
                                    r = mask.GetPixel(i, j).R;
                                    g = mask.GetPixel(i, j).G;
                                    b = mask.GetPixel(i, j).B;
                                    l = (Math.Max(bc.R, (Math.Max(bc.G, bc.B))) + Math.Min(bc.R, Math.Min(bc.G, bc.B))) / 510f;
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
                                    bmC.SetPixel(i, j, bc);
                                }
                            }
                        }
                        imageFine = true;
                    }
                    catch
                    {
                        // error during drawing, maybe mask is smaller than image
                    }
                    if (imageFine)
                    {
                        if (!System.IO.Directory.Exists("img/cache"))
                            System.IO.Directory.CreateDirectory("img/cache");
                        bmC.Save(cf); // safe in cache}
                    }
                }
            }
            if (System.IO.File.Exists(cf))
            {
                graph.CompositingMode = CompositingMode.SourceCopy;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graph.SmoothingMode = SmoothingMode.HighQuality;
                graph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graph.DrawImage(new Bitmap(cf), 0, 0, size, size);
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
                            b = new SolidBrush(CreatureColors.creatureColor(colorIds[c]));
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
    }
}
