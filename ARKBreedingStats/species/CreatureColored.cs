using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using ARKBreedingStats.Library;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.species
{
    /// <summary>
    /// Creates an image of a species with given colors for the regions.
    /// </summary>
    internal static class CreatureColored
    {
        private const string Extension = ".png";
        internal static string ImageFolder;
        private static string _imgCacheFolderPath;
        /// <summary>
        /// Size of the cached images.
        /// </summary>
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
        private static string ColoredCreatureCacheFilePath(string speciesName, byte[] colorIds, bool listView = false)
            => Path.Combine(_imgCacheFolderPath, speciesName.Substring(0, Math.Min(speciesName.Length, 5)) + "_" + Convert32.ToBase32String(colorIds.Select(ci => ci).Concat(Encoding.UTF8.GetBytes(speciesName)).ToArray()).Replace('/', '-') + (listView ? "_lv" : string.Empty) + Extension);

        /// <summary>
        /// Checks if an according species image exists in the cache folder, if not it tries to creates one. Returns false if there's no image.
        /// </summary>
        internal static (bool imageExists, string imagePath, string speciesListName) SpeciesImageExists(Species species, byte[] colorIds)
        {
            string speciesImageName = SpeciesImageName(species?.name);
            string speciesNameForList = SpeciesImageName(species?.name, false);
            string cacheFileName = ColoredCreatureCacheFilePath(speciesImageName, colorIds, true);
            if (File.Exists(cacheFileName))
                return (true, cacheFileName, speciesNameForList);

            string speciesBaseImageFilePath = Path.Combine(ImageFolder, speciesImageName + Extension);
            string speciesColorMaskFilePath = Path.Combine(ImageFolder, speciesImageName + "_m" + Extension);

            if (CreateAndSaveCacheSpeciesFile(colorIds,
                    species?.EnabledColorRegions,
                    speciesBaseImageFilePath, speciesColorMaskFilePath, cacheFileName, 64))
                return (true, cacheFileName, speciesNameForList);

            return (false, null, null);
        }

        /// <summary>
        /// Returns a bitmap image that represents the given colors. If a species color file is available, that is used, else a pie-chart like representation.
        /// </summary>
        /// <param name="colorIds"></param>
        /// <param name="species"></param>
        /// <param name="enabledColorRegions"></param>
        /// <param name="size"></param>
        /// <param name="pieSize"></param>
        /// <param name="onlyColors">Only return a pie-chart like color representation.</param>
        /// <param name="onlyImage">Only return an image of the colored creature. If that's not possible, return null.</param>
        /// <param name="creatureSex">If given, it's tried for find a sex-specific image.</param>
        /// <returns>Image representing the colors.</returns>
        public static Bitmap GetColoredCreature(byte[] colorIds, Species species, bool[] enabledColorRegions, int size = 128, int pieSize = 64, bool onlyColors = false, bool onlyImage = false, Sex creatureSex = Sex.Unknown)
        {
            if (colorIds == null) colorIds = new byte[Ark.ColorRegionCount];

            string speciesName = null;
            if (string.IsNullOrEmpty(species?.name))
                onlyColors = true;
            else
                speciesName = SpeciesImageName(species.name);

            if (onlyColors || string.IsNullOrEmpty(ImageFolder))
                return DrawPieChart(colorIds, enabledColorRegions, size, pieSize);

            // check if there are sex specific images
            if (creatureSex != Sex.Unknown)
            {
                string speciesNameWithSex;
                switch (creatureSex)
                {
                    case Sex.Female:
                        speciesNameWithSex = speciesName + "F";
                        if (File.Exists(Path.Combine(ImageFolder, speciesNameWithSex + Extension)))
                            speciesName = speciesNameWithSex;
                        break;
                    case Sex.Male:
                        speciesNameWithSex = speciesName + "M";
                        if (File.Exists(Path.Combine(ImageFolder, speciesNameWithSex + Extension)))
                            speciesName = speciesNameWithSex;
                        break;
                }
            }

            // if species image not found, check if sex specific files are available
            if (!File.Exists(Path.Combine(ImageFolder, speciesName + Extension)))
            {
                string speciesNameWithSex = speciesName + "M";
                if (File.Exists(Path.Combine(ImageFolder, speciesNameWithSex + Extension)))
                {
                    speciesName = speciesNameWithSex;
                }
                else
                {
                    speciesNameWithSex = speciesName + "F";
                    if (File.Exists(Path.Combine(ImageFolder, speciesNameWithSex + Extension)))
                    {
                        speciesName = speciesNameWithSex;
                    }
                }
            }

            string speciesBaseImageFilePath = Path.Combine(ImageFolder, speciesName + Extension);
            string speciesColorMaskFilePath = Path.Combine(ImageFolder, speciesName + "_m" + Extension);
            string cacheFilePath = ColoredCreatureCacheFilePath(speciesName, colorIds);
            bool cacheFileExists = File.Exists(cacheFilePath);
            if (!cacheFileExists)
            {
                cacheFileExists = CreateAndSaveCacheSpeciesFile(colorIds, enabledColorRegions, speciesBaseImageFilePath, speciesColorMaskFilePath, cacheFilePath);
            }

            if (onlyImage && !cacheFileExists) return null; // creating the species file failed

            if (cacheFileExists && size == TemplateSize)
            {
                try
                {
                    return new Bitmap(cacheFilePath);
                }
                catch
                {
                    // cached file corrupted, recreate
                    if (CreateAndSaveCacheSpeciesFile(colorIds, enabledColorRegions, speciesBaseImageFilePath,
                        speciesColorMaskFilePath, cacheFilePath))
                    {
                        try
                        {
                            return new Bitmap(cacheFilePath);
                        }
                        catch
                        {
                            // file is still invalid after recreation, ignore file
                        }
                    }
                }
                return null;
            }

            if (!cacheFileExists)
            {
                // cache file doesn't exist and couldn't be created. Return pie chart
                return DrawPieChart(colorIds, enabledColorRegions, size, pieSize);
            }

            Bitmap bm = new Bitmap(size, size);
            using (Graphics graph = Graphics.FromImage(bm))
            {
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                graph.CompositingMode = CompositingMode.SourceCopy;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graph.SmoothingMode = SmoothingMode.HighQuality;
                graph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                try
                {
                    using (var cachedImgBmp = new Bitmap(cacheFilePath))
                        graph.DrawImage(cachedImgBmp, 0, 0, size, size);
                }
                catch
                {
                    // cached file invalid, recreate
                    if (CreateAndSaveCacheSpeciesFile(colorIds, enabledColorRegions, speciesBaseImageFilePath,
                        speciesColorMaskFilePath, cacheFilePath))
                    {
                        try
                        {
                            using (var cachedImgBmp = new Bitmap(cacheFilePath))
                                graph.DrawImage(cachedImgBmp, 0, 0, size, size);
                        }
                        catch
                        {
                            // file is still invalid after recreation, ignore file
                            bm.Dispose();
                            return null;
                        }
                    }
                }
            }

            return bm;
        }

        private static Bitmap DrawPieChart(byte[] colorIds, bool[] enabledColorRegions, int size, int pieSize)
        {
            int pieAngle = enabledColorRegions?.Count(c => c) ?? Ark.ColorRegionCount;
            pieAngle = 360 / (pieAngle > 0 ? pieAngle : 1);
            int pieNr = 0;

            Bitmap bm = new Bitmap(size, size);
            using (Graphics graph = Graphics.FromImage(bm))
            {
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                for (int c = 0; c < Ark.ColorRegionCount; c++)
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

            return bm;
        }

        /// <summary>
        /// Creates a colored species image and saves it as cache file.
        /// </summary>
        /// <returns>True if image was created successfully.</returns>
        private static bool CreateAndSaveCacheSpeciesFile(byte[] colorIds, bool[] enabledColorRegions,
            string speciesBaseImageFilePath, string speciesColorMaskFilePath, string cacheFilePath, int outputSize = 256)
        {
            if (string.IsNullOrEmpty(cacheFilePath)
                || !File.Exists(speciesBaseImageFilePath))
                return false;

            using (Bitmap bmpBaseImage = new Bitmap(speciesBaseImageFilePath))
            using (Bitmap bmpColoredCreature = new Bitmap(bmpBaseImage.Width, bmpBaseImage.Height, PixelFormat.Format32bppArgb))
            using (Graphics graph = Graphics.FromImage(bmpColoredCreature))
            {
                bool imageFine = true;
                graph.SmoothingMode = SmoothingMode.AntiAlias;

                //// ellipse background shadow
                int scx = bmpBaseImage.Width / 2;
                int scy = (int)(scx * 1.6);
                const double perspectiveFactor = 0.3;
                int yStart = scy - (int)(perspectiveFactor * .7 * scx);
                int yEnd = (int)(2 * perspectiveFactor * scx);
                GraphicsPath pathShadow = new GraphicsPath();
                pathShadow.AddEllipse(0, yStart, bmpBaseImage.Width, yEnd);
                var colorBlend = new ColorBlend
                {
                    Colors = new[] { Color.FromArgb(0), Color.FromArgb(40, 0, 0, 0), Color.FromArgb(80, 0, 0, 0) },
                    Positions = new[] { 0, 0.6f, 1 }
                };

                using (var pthGrBrush = new PathGradientBrush(pathShadow)
                {
                    InterpolationColors = colorBlend
                })
                    graph.FillEllipse(pthGrBrush, 0, yStart, bmpBaseImage.Width, yEnd);
                // background shadow done

                // if species has color regions, apply colors
                if (File.Exists(speciesColorMaskFilePath))
                {
                    var rgb = new byte[Ark.ColorRegionCount][];
                    var useColorRegions = new bool[Ark.ColorRegionCount];
                    for (int c = 0; c < Ark.ColorRegionCount; c++)
                    {
                        useColorRegions[c] = enabledColorRegions[c] && colorIds[c] != 0;
                        if (useColorRegions[c])
                        {
                            Color cl = CreatureColors.CreatureColor(colorIds[c]);
                            rgb[c] = new[] { cl.R, cl.G, cl.B };
                        }
                    }
                    imageFine = ApplyColorsUnsafe(rgb, useColorRegions, speciesColorMaskFilePath, bmpBaseImage);
                }

                if (imageFine)
                {
                    // draw species image on background
                    graph.DrawImage(bmpBaseImage, 0, 0, bmpColoredCreature.Width, bmpColoredCreature.Height);

                    // save image in cache for later use
                    string cacheFolder = Path.GetDirectoryName(cacheFilePath);
                    if (string.IsNullOrEmpty(cacheFolder)) return false;
                    if (!Directory.Exists(cacheFolder))
                        Directory.CreateDirectory(cacheFolder);
                    if (outputSize == bmpColoredCreature.Width)
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
        private static bool ApplyColorsUnsafe(byte[][] rgb, bool[] enabledColorRegions, string speciesColorMaskFilePath, Bitmap bmpBaseImage)
        {
            var imageFine = false;
            using (Bitmap bmpMask = new Bitmap(bmpBaseImage.Width, bmpBaseImage.Height))
            {
                // get mask in correct size
                using (var g = Graphics.FromImage(bmpMask))
                using (var bmpMaskOriginal = new Bitmap(speciesColorMaskFilePath))
                {
                    g.InterpolationMode = InterpolationMode.Bicubic;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawImage(bmpMaskOriginal, 0, 0, bmpBaseImage.Width, bmpBaseImage.Height);
                }

                BitmapData bmpDataBaseImage = bmpBaseImage.LockBits(
                    new Rectangle(0, 0, bmpBaseImage.Width, bmpBaseImage.Height), ImageLockMode.ReadOnly,
                    bmpBaseImage.PixelFormat);
                BitmapData bmpDataMask = bmpMask.LockBits(
                    new Rectangle(0, 0, bmpMask.Width, bmpMask.Height), ImageLockMode.ReadOnly,
                    bmpMask.PixelFormat);

                int bgBytes = bmpBaseImage.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;
                int msBytes = bmpDataMask.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;

                float o = 0;
                try
                {
                    unsafe
                    {
                        byte* scan0Bg = (byte*)bmpDataBaseImage.Scan0.ToPointer();
                        byte* scan0Ms = (byte*)bmpDataMask.Scan0.ToPointer();

                        var width = bmpDataBaseImage.Width;
                        var height = bmpDataBaseImage.Height;
                        var strideBaseImage = bmpDataBaseImage.Stride;
                        var strideMask = bmpDataMask.Stride;

                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                byte* dBg = scan0Bg + j * strideBaseImage + i * bgBytes;
                                // continue if the pixel is transparent
                                if (dBg[3] == 0)
                                    continue;

                                byte* dMs = scan0Ms + j * strideMask + i * msBytes;

                                int r = dMs[2];
                                int g = dMs[1];
                                int b = dMs[0];
                                byte finalR = dBg[2];
                                byte finalG = dBg[1];
                                byte finalB = dBg[0];

                                for (int m = 0; m < Ark.ColorRegionCount; m++)
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
                                dBg[0] = finalB;
                                dBg[1] = finalG;
                                dBg[2] = finalR;
                            }
                        }
                        imageFine = true;
                    }
                }
                catch
                {
                    // error during drawing, maybe mask is smaller than image
                }

                bmpBaseImage.UnlockBits(bmpDataBaseImage);
                bmpMask.UnlockBits(bmpDataMask);
            }

            return imageFine;
        }

        public static string RegionColorInfo(Species species, byte[] colorIds)
        {
            if (species?.colors == null || colorIds == null) return null;

            var creatureRegionColors = new StringBuilder("Colors:");
            var cs = species.colors;
            for (int r = 0; r < Ark.ColorRegionCount; r++)
            {
                if (species.EnabledColorRegions[r])
                {
                    creatureRegionColors.Append($"\n{cs[r]?.name} ({r}): {CreatureColors.CreatureColorName(colorIds[r])} ({colorIds[r]})");
                }
            }
            return creatureRegionColors.ToString();
        }

        /// <summary>
        /// Deletes all cached species color images with a specific pattern that weren't used for some time.
        /// </summary>
        internal static void CleanupCache(bool clearAllCacheFiles = false)
        {
            if (!Directory.Exists(_imgCacheFolderPath)) return;

            DirectoryInfo directory = new DirectoryInfo(_imgCacheFolderPath);
            var oldCacheFiles = clearAllCacheFiles ? directory.GetFiles() : directory.GetFiles().Where(f => f.LastAccessTime < DateTime.Now.AddDays(-7)).ToArray();
            foreach (FileInfo f in oldCacheFiles)
            {
                FileService.TryDeleteFile(f);
            }
        }

        /// <summary>
        /// If the setting ImgCacheUseLocalAppData is true, the image cache files are saved in the %localAppData% folder instead of the app folder.
        /// This is always true if the app is installed.
        /// Call this method after that setting or the speciesImageFolder where the images are stored are changed.
        /// The reason to use the appData folder is that this folder is used to save files, the portable version can be shared and be write protected.
        /// </summary>
        internal static void InitializeSpeciesImageLocation()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.SpeciesImagesFolder))
            {
                ImageFolder = null;
                _imgCacheFolderPath = null;
                return;
            }

            ImageFolder = FileService.GetPath(Properties.Settings.Default.SpeciesImagesFolder);
            _imgCacheFolderPath = GetImgCacheFolderPath();
        }

        private static string GetImgCacheFolderPath() => FileService.GetPath(Properties.Settings.Default.SpeciesImagesFolder, FileService.CacheFolderName,
            useAppData: Updater.Updater.IsProgramInstalled || Properties.Settings.Default.ImgCacheUseLocalAppData);
    }
}
