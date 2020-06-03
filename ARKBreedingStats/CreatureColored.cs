using ARKBreedingStats.species;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using ARKBreedingStats.Library;

namespace ARKBreedingStats
{
    static class CreatureColored
    {
        private const string extension = ".png";

        /// <summary>
        /// Returns a bitmap image that represents the given colors. If a species color file is available, that is used, else a pic-chart like representation.
        /// </summary>
        /// <param name="colorIds"></param>
        /// <param name="species"></param>
        /// <param name="enabledColorRegions"></param>
        /// <param name="size"></param>
        /// <param name="pieSize"></param>
        /// <param name="onlyColors">Only return a pie-chart like color representation.</param>
        /// <param name="onlyImage">Only return an image of the colored creature. If that's not possible, return null.</param>
        /// <returns></returns>
        public static Bitmap GetColoredCreature(int[] colorIds, Species species, bool[] enabledColorRegions, int size = 128, int pieSize = 64, bool onlyColors = false, bool onlyImage = false, Library.Sex creatureSex = Sex.Unknown)
        {
            if (colorIds == null) return null;
            //float[][] hsl = new float[Species.ColorRegionCount][];
            int[][] rgb = new int[Species.ColorRegionCount][];
            for (int c = 0; c < Species.ColorRegionCount; c++)
            {
                Color cl = CreatureColors.CreatureColor(colorIds[c]);
                rgb[c] = new int[] { cl.R, cl.G, cl.B };
            }

            string imgFolder = FileService.GetPath(FileService.ImageFolderName);
            string cacheFolder = FileService.GetPath(FileService.ImageFolderName, FileService.CacheFolderName);
            string speciesName = species?.name ?? string.Empty;
            // check if there are sex specific images
            if (creatureSex != Sex.Unknown)
            {
                string speciesNameWithSex = null;
                switch (creatureSex)
                {
                    case Sex.Female:
                        speciesNameWithSex = speciesName + "F";
                        if (File.Exists(Path.Combine(imgFolder, speciesNameWithSex + extension)))
                            speciesName = speciesNameWithSex;
                        break;
                    case Sex.Male:
                        speciesNameWithSex = speciesName + "M";
                        if (File.Exists(Path.Combine(imgFolder, speciesNameWithSex + extension)))
                            speciesName = speciesNameWithSex;
                        break;
                }
            }

            string speciesBackgroundFilePath = Path.Combine(imgFolder, speciesName + extension);
            string cacheFileName = Path.Combine(cacheFolder, speciesName.Substring(0, Math.Min(speciesName.Length, 5)) + "_" + (speciesName + string.Join(".", colorIds.Select(i => i.ToString()))).GetHashCode().ToString("X8") + extension);
            string speciesColorMaskFilePath = Path.Combine(imgFolder, speciesName + "_m" + extension);
            if (!onlyColors && File.Exists(speciesBackgroundFilePath) && File.Exists(speciesColorMaskFilePath) && !File.Exists(cacheFileName))
            {
                using (Bitmap bmpBackground = new Bitmap(speciesBackgroundFilePath))
                using (Bitmap bmpCreature = new Bitmap(bmpBackground.Width, bmpBackground.Height, PixelFormat.Format32bppArgb))
                using (Graphics graph = Graphics.FromImage(bmpCreature))
                {
                    bool imageFine = false;
                    graph.SmoothingMode = SmoothingMode.AntiAlias;
                    const int defaultSizeOfTemplates = 256;

                    using (Bitmap bmpMask = new Bitmap(defaultSizeOfTemplates, defaultSizeOfTemplates))
                    {
                        using (var g = Graphics.FromImage(bmpMask))
                        using (var bmpMaskOriginal = new Bitmap(speciesColorMaskFilePath))
                            g.DrawImage(bmpMaskOriginal, 0, 0,
                        defaultSizeOfTemplates, defaultSizeOfTemplates);
                        float o = 0;
                        try
                        {
                            // shadow
                            using (var b = new SolidBrush(Color.FromArgb(12, 0, 0, 0)))
                            {
                                int scx = defaultSizeOfTemplates / 2;
                                int scy = (int)(scx * 1.6);
                                int factor = 25;
                                int sr = scx - 2 * factor;
                                double heightFactor = 0.3;

                                for (int i = 2; i >= 0; i--)
                                {
                                    int radius = sr + i * factor;
                                    graph.FillEllipse(b, scx - radius, scy - (int)(heightFactor * .7 * radius), 2 * radius,
                                        (int)(2 * heightFactor * radius));
                                }
                            }

                            graph.DrawImage(bmpBackground, 0, 0, defaultSizeOfTemplates, defaultSizeOfTemplates);

                            for (int i = 0; i < bmpBackground.Width; i++)
                            {
                                for (int j = 0; j < bmpBackground.Height; j++)
                                {
                                    Color bc = bmpBackground.GetPixel(i, j);
                                    if (bc.A > 0)
                                    {
                                        var cl = bmpMask.GetPixel(i, j);
                                        int r = cl.R;
                                        int g = cl.G;
                                        int b = cl.B;
                                        for (int m = 0; m < Species.ColorRegionCount; m++)
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
                                            // using "grain merge", e.g. see https://docs.gimp.org/en/gimp-concepts-layer-modes.html
                                            int rMix = bc.R + rgb[m][0] - 128;
                                            if (rMix < 0) rMix = 0;
                                            else if (rMix > 255) rMix = 255;
                                            int gMix = bc.G + rgb[m][1] - 128;
                                            if (gMix < 0) gMix = 0;
                                            else if (gMix > 255) gMix = 255;
                                            int bMix = bc.B + rgb[m][2] - 128;
                                            if (bMix < 0) bMix = 0;
                                            else if (bMix > 255) bMix = 255;
                                            Color c = Color.FromArgb(rMix, gMix, bMix);
                                            bc = Color.FromArgb(bc.A, (int)(o * c.R + (1 - o) * bc.R),
                                                (int)(o * c.G + (1 - o) * bc.G), (int)(o * c.B + (1 - o) * bc.B));
                                        }

                                        bmpCreature.SetPixel(i, j, bc);
                                    }
                                }
                            }

                            imageFine = true;
                        }
                        catch
                        {
                            // error during drawing, maybe mask is smaller than image
                        }
                    }
                    if (imageFine)
                    {
                        if (!Directory.Exists(cacheFolder))
                            Directory.CreateDirectory(cacheFolder);
                        bmpCreature.Save(cacheFileName); // safe in cache}
                    }
                }
            }

            bool cacheFileExists = File.Exists(cacheFileName);

            if (onlyImage && !cacheFileExists) return null;

            Bitmap bm = new Bitmap(size, size);
            using (Graphics graph = Graphics.FromImage(bm))
            {
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                if (cacheFileExists)
                {
                    graph.CompositingMode = CompositingMode.SourceCopy;
                    graph.CompositingQuality = CompositingQuality.HighQuality;
                    graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graph.SmoothingMode = SmoothingMode.HighQuality;
                    graph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graph.DrawImage(new Bitmap(cacheFileName), 0, 0, size, size);
                }
                else
                {
                    // draw piechart
                    int pieAngle = enabledColorRegions.Count(c => c);
                    pieAngle = 360 / (pieAngle > 0 ? pieAngle : 1);
                    int pieNr = 0;
                    for (int c = 0; c < Species.ColorRegionCount; c++)
                    {
                        if (enabledColorRegions[c])
                        {
                            if (colorIds[c] > 0)
                            {
                                using (var b = new SolidBrush(CreatureColors.CreatureColor(colorIds[c])))
                                {
                                    graph.FillPie(b, (size - pieSize) / 2, (size - pieSize) / 2, pieSize, pieSize,
                                        pieNr * pieAngle + 270, pieAngle);
                                }
                            }

                            pieNr++;
                        }
                    }

                    using (var pen = new Pen(Color.Gray))
                        graph.DrawEllipse(pen, (size - pieSize) / 2, (size - pieSize) / 2, pieSize, pieSize);
                }
            }

            return bm;
        }

        public static string RegionColorInfo(Species species, int[] colorIds)
        {
            if (species == null || colorIds == null) return null;

            var creatureRegionColors = new StringBuilder("Colors:");
            var cs = species.colors;
            for (int r = 0; r < Species.ColorRegionCount; r++)
            {
                if (!string.IsNullOrEmpty(cs[r]?.name))
                {
                    creatureRegionColors.Append($"\n{cs[r].name} ({r}): {CreatureColors.CreatureColorName(colorIds[r])} ({colorIds[r]})");
                }
            }
            return creatureRegionColors.ToString();
        }
    }
}
