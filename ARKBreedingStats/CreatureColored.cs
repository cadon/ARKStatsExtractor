using ARKBreedingStats.species;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using ARKBreedingStats.Library;

namespace ARKBreedingStats
{
    static class CreatureColored
    {
        private const string Extension = ".png";
        private static readonly string ImgFolder = FileService.GetPath(FileService.ImageFolderName);
        private static readonly string CacheFolder = FileService.GetPath(FileService.ImageFolderName, FileService.CacheFolderName);
        private const int TemplateSize = 256;

        /// <summary>
        /// Returns the name of the image file used for the species. E.g. parts like Aberrant or Brute are removed, they share the same graphics.
        /// </summary>
        internal static string SpeciesImageName(string speciesName, bool replacePolar = true)
        {
            if (string.IsNullOrEmpty(speciesName)) return string.Empty;
            return replacePolar
                ? speciesName.Replace("Aberrant ", string.Empty).Replace("Brute ", string.Empty)
                    .Replace("Polar Bear", "Dire Bear").Replace("Polar ", string.Empty)
                : speciesName.Replace("Aberrant ", string.Empty).Replace("Brute ", string.Empty);
        }

        /// <summary>
        /// Returns the image file path to the image with the according colorization.
        /// </summary>
        private static string ColoredCreatureCacheFilePath(string speciesName, int[] colorIds, bool listView = false)
            => Path.Combine(CacheFolder, speciesName.Substring(0, Math.Min(speciesName.Length, 5)) + "_" + (speciesName + string.Join(".", colorIds.Select(i => i.ToString()))).GetHashCode().ToString("X8") + (listView ? "_lv" : string.Empty) + Extension);

        /// <summary>
        /// Checks if an according species image exists in the cache folder, if not it tries to creates one. Returns false if there's no image.
        /// </summary>
        internal static (bool imageExists, string imagePath, string speciesListName) SpeciesImageExists(Species species, int[] colorIds)
        {
            string speciesImageName = SpeciesImageName(species?.name);
            string speciesNameForList = SpeciesImageName(species?.name, false);
            string cacheFileName = ColoredCreatureCacheFilePath(speciesImageName, colorIds, true);
            if (File.Exists(cacheFileName))
                return (true, cacheFileName, speciesNameForList);

            string speciesBackgroundFilePath = Path.Combine(ImgFolder, speciesImageName + Extension);
            string speciesColorMaskFilePath = Path.Combine(ImgFolder, speciesImageName + "_m" + Extension);

            if (CreateAndSaveCacheSpeciesFile(colorIds,
                    species?.EnabledColorRegions,
                    speciesBackgroundFilePath, speciesColorMaskFilePath, cacheFileName, 64))
                return (true, cacheFileName, speciesNameForList);

            return (false, null, null);
        }

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
        /// <param name="creatureSex">If given, it's tried for find a sex-specific image.</param>
        /// <returns></returns>
        public static Bitmap GetColoredCreature(int[] colorIds, Species species, bool[] enabledColorRegions, int size = 128, int pieSize = 64, bool onlyColors = false, bool onlyImage = false, Sex creatureSex = Sex.Unknown)
        {
            if (colorIds == null) return null;

            string speciesName = SpeciesImageName(species?.name);

            // check if there are sex specific images
            if (creatureSex != Sex.Unknown)
            {
                string speciesNameWithSex;
                switch (creatureSex)
                {
                    case Sex.Female:
                        speciesNameWithSex = speciesName + "F";
                        if (File.Exists(Path.Combine(ImgFolder, speciesNameWithSex + Extension)))
                            speciesName = speciesNameWithSex;
                        break;
                    case Sex.Male:
                        speciesNameWithSex = speciesName + "M";
                        if (File.Exists(Path.Combine(ImgFolder, speciesNameWithSex + Extension)))
                            speciesName = speciesNameWithSex;
                        break;
                }
            }

            string speciesBackgroundFilePath = Path.Combine(ImgFolder, speciesName + Extension);
            string speciesColorMaskFilePath = Path.Combine(ImgFolder, speciesName + "_m" + Extension);
            string cacheFilePath = ColoredCreatureCacheFilePath(speciesName, colorIds);
            bool cacheFileExists = File.Exists(cacheFilePath);
            if (!onlyColors && !cacheFileExists)
            {
                cacheFileExists = CreateAndSaveCacheSpeciesFile(colorIds, enabledColorRegions, speciesBackgroundFilePath, speciesColorMaskFilePath, cacheFilePath);
            }

            if (onlyImage && !cacheFileExists) return null;

            if (cacheFileExists && size == TemplateSize)
                return new Bitmap(cacheFilePath);

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
                    graph.DrawImage(new Bitmap(cacheFilePath), 0, 0, size, size);
                }
                else
                {
                    // draw piechart
                    int pieAngle = enabledColorRegions?.Count(c => c) ?? Species.ColorRegionCount;
                    pieAngle = 360 / (pieAngle > 0 ? pieAngle : 1);
                    int pieNr = 0;
                    for (int c = 0; c < Species.ColorRegionCount; c++)
                    {
                        if (enabledColorRegions?[c] ?? true)
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

        /// <summary>
        /// Creates a colored species image and saves it as cache file. Returns true when created successful.
        /// </summary>
        /// <returns></returns>
        private static bool CreateAndSaveCacheSpeciesFile(int[] colorIds, bool[] enabledColorRegions,
            string speciesBackgroundFilePath, string speciesColorMaskFilePath, string cacheFilePath, int outputSize = 256)
        {
            if (!File.Exists(speciesBackgroundFilePath)) return false;

            using (Bitmap bmpBackground = new Bitmap(speciesBackgroundFilePath))
            using (Bitmap bmpColoredCreature = new Bitmap(bmpBackground.Width, bmpBackground.Height, PixelFormat.Format32bppArgb))
            using (Graphics graph = Graphics.FromImage(bmpColoredCreature))
            {
                bool imageFine = true;
                graph.SmoothingMode = SmoothingMode.AntiAlias;

                // shadow
                using (var b = new SolidBrush(Color.FromArgb(12, 0, 0, 0)))
                {
                    int scx = TemplateSize / 2;
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

                // shaded base image
                graph.DrawImage(bmpBackground, 0, 0, TemplateSize, TemplateSize);

                // if species has color regions, apply colors
                if (File.Exists(speciesColorMaskFilePath))
                {
                    var rgb = new byte[Species.ColorRegionCount][];
                    var useColorRegions = new bool[Species.ColorRegionCount];
                    for (int c = 0; c < Species.ColorRegionCount; c++)
                    {
                        useColorRegions[c] = enabledColorRegions[c] && colorIds[c] != 0;
                        if (useColorRegions[c])
                        {
                            Color cl = CreatureColors.CreatureColor(colorIds[c]);
                            rgb[c] = new byte[] { cl.R, cl.G, cl.B };
                        }
                    }
                    imageFine = ApplyColorsUnsafe(rgb, useColorRegions, speciesColorMaskFilePath, TemplateSize, bmpBackground, bmpColoredCreature);
                }

                if (imageFine)
                {
                    string cacheFolder = Path.GetDirectoryName(cacheFilePath);
                    if (!Directory.Exists(cacheFolder))
                        Directory.CreateDirectory(cacheFolder);
                    if (outputSize == TemplateSize)
                        return SaveBitmapToFile(bmpColoredCreature, cacheFilePath);

                    using (var resized = new Bitmap(outputSize, outputSize))
                    using (var g = Graphics.FromImage(resized))
                    {
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.DrawImage(bmpColoredCreature, 0, 0, outputSize, outputSize);
                        return SaveBitmapToFile(resized, cacheFilePath);
                    }
                }
            }

            return false;
        }

        private static bool SaveBitmapToFile(Bitmap bmp, string filePath)
        {
            try
            {
                bmp.Save(filePath);
                return true;
            }
            catch
            {
                // something went wrong when trying to save the cached creature image.
                // could be related to have no write access if the portable version is placed in a protected folder like program files.
                return false;
            }
        }

        /// <summary>
        /// Applies the colors to the base image.
        /// </summary>
        private static bool ApplyColorsUnsafe(byte[][] rgb, bool[] enabledColorRegions, string speciesColorMaskFilePath,
            int templateSize, Bitmap bmpBackground, Bitmap bmpColoredCreature)
        {
            var imageFine = false;
            using (Bitmap bmpMask = new Bitmap(templateSize, templateSize))
            {
                // get mask in correct size
                using (var g = Graphics.FromImage(bmpMask))
                using (var bmpMaskOriginal = new Bitmap(speciesColorMaskFilePath))
                {
                    g.InterpolationMode = InterpolationMode.Bicubic;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawImage(bmpMaskOriginal, 0, 0,
                        templateSize, templateSize);
                }

                BitmapData bmpDataBackground = bmpBackground.LockBits(
                    new Rectangle(0, 0, bmpBackground.Width, bmpBackground.Height), ImageLockMode.ReadOnly,
                    bmpBackground.PixelFormat);
                BitmapData bmpDataMask = bmpMask.LockBits(
                    new Rectangle(0, 0, bmpMask.Width, bmpMask.Height), ImageLockMode.ReadOnly,
                    bmpMask.PixelFormat);
                BitmapData bmpDataColoredCreature = bmpColoredCreature.LockBits(
                    new Rectangle(0, 0, bmpColoredCreature.Width, bmpColoredCreature.Height),
                    ImageLockMode.WriteOnly,
                    bmpColoredCreature.PixelFormat);

                int bgBytes = bmpBackground.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;
                int msBytes = bmpDataMask.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;
                int ccBytes = bmpColoredCreature.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;

                float o = 0;
                try
                {
                    unsafe
                    {
                        byte* scan0Bg = (byte*)bmpDataBackground.Scan0.ToPointer();
                        byte* scan0Ms = (byte*)bmpDataMask.Scan0.ToPointer();
                        byte* scan0Cc = (byte*)bmpDataColoredCreature.Scan0.ToPointer();

                        for (int i = 0; i < bmpDataBackground.Width; i++)
                        {
                            for (int j = 0; j < bmpDataBackground.Height; j++)
                            {
                                byte* dBg = scan0Bg + j * bmpDataBackground.Stride + i * bgBytes;
                                // continue if the pixel is transparent
                                if (dBg[3] == 0)
                                    continue;

                                byte* dMs = scan0Ms + j * bmpDataMask.Stride + i * msBytes;
                                byte* dCc = scan0Cc + j * bmpDataColoredCreature.Stride + i * ccBytes;

                                int r = dMs[2];
                                int g = dMs[1];
                                int b = dMs[0];
                                byte finalR = dBg[2];
                                byte finalG = dBg[1];
                                byte finalB = dBg[0];

                                for (int m = 0; m < Species.ColorRegionCount; m++)
                                {
                                    if (!enabledColorRegions[m])
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
                                    int rMix = finalR + rgb[m][0] - 128;
                                    if (rMix < 0) rMix = 0;
                                    else if (rMix > 255) rMix = 255;
                                    int gMix = finalG + rgb[m][1] - 128;
                                    if (gMix < 0) gMix = 0;
                                    else if (gMix > 255) gMix = 255;
                                    int bMix = finalB + rgb[m][2] - 128;
                                    if (bMix < 0) bMix = 0;
                                    else if (bMix > 255) bMix = 255;

                                    finalR = (byte)(o * rMix + (1 - o) * finalR);
                                    finalG = (byte)(o * gMix + (1 - o) * finalG);
                                    finalB = (byte)(o * bMix + (1 - o) * finalB);
                                }

                                // set final color
                                dCc[0] = finalB;
                                dCc[1] = finalG;
                                dCc[2] = finalR;
                                dCc[3] = dBg[3]; // same alpha as base image
                            }
                        }
                        imageFine = true;
                    }
                }
                catch
                {
                    // error during drawing, maybe mask is smaller than image
                }

                bmpBackground.UnlockBits(bmpDataBackground);
                bmpMask.UnlockBits(bmpDataMask);
                bmpColoredCreature.UnlockBits(bmpDataColoredCreature);
            }

            return imageFine;
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

        /// <summary>
        /// Deletes all cached species color images with a specific pattern that weren't used for some time.
        /// </summary>
        internal static void CleanupCache()
        {
            string imgCachePath = FileService.GetPath(FileService.ImageFolderName, FileService.CacheFolderName);
            if (!Directory.Exists(imgCachePath)) return;

            DirectoryInfo directory = new DirectoryInfo(imgCachePath);
            var oldCacheFiles = directory.GetFiles().Where(f => f.LastAccessTime < DateTime.Now.AddDays(-5)).ToArray();
            foreach (FileInfo f in oldCacheFiles)
            {
                FileService.TryDeleteFile(f);
            }
        }
    }
}
