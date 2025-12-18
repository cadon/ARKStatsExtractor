using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ARKBreedingStats.SpeciesImages
{
    /// <summary>
    /// Creates an image of a species with given colors for the regions.
    /// </summary>
    internal static class CreatureColored
    {
        private static readonly ConcurrentDictionary<string, Lazy<bool>> CurrentCacheCreations =
            new ConcurrentDictionary<string, Lazy<bool>>();

        /// <summary>
        /// Size of the cached images.
        /// </summary>
        private const int TemplateSize = 256;

        /// <summary>
        /// Stores current creature image callbacks to avoid race conditions where a previously requested image overwrites a later requested image.
        /// Only the latest call will have its callback called.
        /// </summary>
        private static readonly ConcurrentDictionary<Action<Bitmap>, int> ColoredCreatureCallbacks =
            new ConcurrentDictionary<Action<Bitmap>, int>();

        /// <summary>
        /// Retrieves a bitmap image that represents the given colors. If a species color file is available, that is used, else a pie-chart like representation.
        /// This method runs the retrieval in a separate thread and calls a callback once finished on the thread of the passed control.
        /// </summary>
        /// <param name="callBack">Method to call when the image is ready.</param>
        /// <param name="uiElement">UI element whose thread is used to invoke the callback on the UI thread.</param>
        /// <param name="colorIds"></param>
        /// <param name="species"></param>
        /// <param name="enabledColorRegions"></param>
        /// <param name="size"></param>
        /// <param name="pieSize"></param>
        /// <param name="onlyColors">Only return a pie-chart like color representation.</param>
        /// <param name="onlyImage">Only return an image of the colored creature. If that's not possible, return null.</param>
        /// <param name="creatureSex">If given, it's tried for find a sex-specific image.</param>
        /// <param name="game">Name of the game if there is a specific image for that, e.g. ASA or ASE</param>
        /// <returns>Image representing the colors.</returns>
        public static void GetColoredCreatureWithCallback(Action<Bitmap> callBack, Control uiElement, byte[] colorIds, Species species,
            bool[] enabledColorRegions, int size, int pieSize = 64, bool onlyColors = false, bool onlyImage = false,
            Sex creatureSex = Sex.Unknown, string game = null)
        {
            Task.Run(async () =>
            {
                var currentTaskId = Task.CurrentId ?? 0;
                ColoredCreatureCallbacks[callBack] = currentTaskId;
                var bmp = await GetColoredCreatureAsync(colorIds, species,
                    enabledColorRegions, size, pieSize, creatureSex: creatureSex, game: game);
                uiElement.Invoke(
                    new Action(() =>
                    {
                        if (ColoredCreatureCallbacks.TryGetValue(callBack, out var taskId)
                            && taskId == currentTaskId)
                        {
                            callBack(bmp);
                            ColoredCreatureCallbacks.TryRemove(callBack, out _);
                        }
                        else bmp.Dispose(); // callback is outdated, dispose bitmap
                    }));
            });
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
        /// <param name="game">Name of the game if there is a specific image for that, e.g. ASA or ASE</param>
        /// <returns>Image representing the colors.</returns>
        public static async Task<Bitmap> GetColoredCreatureAsync(byte[] colorIds, Species species, bool[] enabledColorRegions,
            int size, int pieSize = 64, bool onlyColors = false, bool onlyImage = false, Sex creatureSex = Sex.Unknown, string game = null)
        {
            if (colorIds == null) colorIds = new byte[Ark.ColorRegionCount];

            if (string.IsNullOrEmpty(species?.name))
                return DrawPieChart(colorIds, enabledColorRegions, size, pieSize);

            var patternId = -1;
            if (species.patterns != null)
            {
                patternId = colorIds[species.patterns.selectRegion] % species.patterns.count;
                // the pattern id is >0
                if (patternId <= 0) patternId += species.patterns.count;
            }

            var pose = Poses.GetPose(species);

            var speciesBaseImageFilePath = await CreatureImageFile.SpeciesImageFilePath(species, game, creatureSex, patternId, useComposition: true, pose: pose).ConfigureAwait(false);

            if (string.IsNullOrEmpty(speciesBaseImageFilePath))
                return onlyImage ? null : DrawPieChart(colorIds, enabledColorRegions, size, pieSize); // no available images

            var speciesColorMaskFilePath = CreatureImageFile.MaskFilePath(speciesBaseImageFilePath);

            string cacheFilePath = CreatureImageFile.ColoredCreatureCacheFilePath(Path.GetFileNameWithoutExtension(speciesBaseImageFilePath), colorIds);
            bool cacheFileExists = File.Exists(cacheFilePath);
            var compositionParts = ImageCompositions.GetComposition(species)?.Parts;
            if (!cacheFileExists)
            {
                cacheFileExists = CreateAndSaveCacheSpeciesFile(colorIds, enabledColorRegions, speciesBaseImageFilePath, speciesColorMaskFilePath, cacheFilePath, compositionParts);
            }

            if (onlyImage && !cacheFileExists) return null; // creating the species file failed

            if (cacheFileExists && size == TemplateSize)
            {
                try
                {
                    // use temp bitmap to avoid persistent file locking
                    using (var bmpTemp = new Bitmap(cacheFilePath))
                        return new Bitmap(bmpTemp);
                }
                catch
                {
                    // cached file corrupted, recreate
                    if (CreateAndSaveCacheSpeciesFile(colorIds, enabledColorRegions, speciesBaseImageFilePath,
                        speciesColorMaskFilePath, cacheFilePath, compositionParts))
                    {
                        try
                        {
                            // use temp bitmap to avoid persistent file locking
                            using (var bmpTemp = new Bitmap(cacheFilePath))
                                return new Bitmap(bmpTemp);
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

            // cache file exists, resize to requested size
            Bitmap bm = new Bitmap(size, size);
            using (Graphics graph = Graphics.FromImage(bm))
            {
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
                        speciesColorMaskFilePath, cacheFilePath, compositionParts))
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

        internal static Bitmap DrawPieChart(byte[] colorIds, bool[] enabledColorRegions, int size, int pieSize)
        {
            if (colorIds == null)
                colorIds = Enumerable.Repeat((byte)0, Ark.ColorRegionCount).ToArray();
            if (enabledColorRegions == null)
                enabledColorRegions = Enumerable.Repeat(true, Ark.ColorRegionCount).ToArray();
            var pieAngle = enabledColorRegions.Count(c => c);
            pieAngle = 360 / (pieAngle > 0 ? pieAngle : 1);
            var pieNr = 0;

            var bm = new Bitmap(size, size);
            using (var graph = Graphics.FromImage(bm))
            {
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                for (var c = 0; c < Ark.ColorRegionCount; c++)
                {
                    if (!enabledColorRegions[c]) continue;
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

                using (var pen = new Pen(Color.Gray))
                    graph.DrawEllipse(pen, (size - pieSize) / 2, (size - pieSize) / 2, pieSize, pieSize);
            }

            return bm;
        }

        /// <summary>
        /// Creates a colored species image and saves it as cache file.
        /// </summary>
        /// <returns>True if image was created successfully.</returns>
        internal static bool CreateAndSaveCacheSpeciesFile(byte[] colorIds, bool[] enabledColorRegions,
            string speciesBaseImageFilePath, string speciesColorMaskFilePath, string cacheFilePath,
            ImageCompositionPart[] compositionParts = null, int outputSize = 256)
            => CurrentCacheCreations.GetOrAdd(cacheFilePath, new Lazy<bool>(() => CreateAndSaveCacheSpeciesFileOnce(colorIds, enabledColorRegions, speciesBaseImageFilePath,
                speciesColorMaskFilePath, cacheFilePath, compositionParts, outputSize))).Value;

        /// <summary>
        /// Creates a colored species image and saves it as cache file.
        /// </summary>
        /// <returns>True if image was created successfully.</returns>
        internal static bool CreateAndSaveCacheSpeciesFileOnce(byte[] colorIds, bool[] enabledColorRegions,
            string speciesBaseImageFilePath, string speciesColorMaskFilePath, string cacheFilePath, ImageCompositionPart[] compositionParts = null, int outputSize = 256)
        {
            try
            {
                if (string.IsNullOrEmpty(cacheFilePath)
                    || !File.Exists(speciesBaseImageFilePath))
                    return false;

                using (Bitmap bmpBaseImage = new Bitmap(speciesBaseImageFilePath))
                using (Bitmap bmpColoredCreature =
                       new Bitmap(bmpBaseImage.Width, bmpBaseImage.Height, PixelFormat.Format32bppArgb))
                using (Graphics graph = Graphics.FromImage(bmpColoredCreature))
                {
                    bool imageFine = true;
                    graph.CompositingQuality = CompositingQuality.HighQuality;
                    graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graph.SmoothingMode = SmoothingMode.HighQuality;
                    graph.PixelOffsetMode = PixelOffsetMode.HighQuality;

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

                    if (!imageFine) return false;

                    // if image is a composition, optional shadows are already included in the base image
                    if (compositionParts?.Any() != true)
                    {
                        // use default shadow
                        var shadowRectangle = new RectangleF(0, bmpBaseImage.Height * 0.626f, bmpBaseImage.Width,
                            bmpBaseImage.Height * 0.3f);
                        DrawShadowEllipse(graph, shadowRectangle, Color.Black);
                    }

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
            finally
            {
                if (!string.IsNullOrEmpty(cacheFilePath))
                    CurrentCacheCreations.TryRemove(cacheFilePath, out _);
            }
        }

        public static void DrawShadowEllipse(Graphics graph, RectangleF rectangleShadow, Color shadowColor, float shadowRotation = 0, float shadowIntensity = 1)
        {
            var scx = (int)(rectangleShadow.Left + rectangleShadow.Width / 2);
            var scy = (int)(rectangleShadow.Top + rectangleShadow.Height / 2);
            var colorBlend = new ColorBlend
            {
                Colors = new[] {
                    Color.FromArgb(0),
                    Color.FromArgb(Math.Min(255,(int)shadowIntensity * 6), shadowColor),
                    Color.FromArgb(Math.Min(255,(int)shadowIntensity * 40), shadowColor),
                    Color.FromArgb(Math.Min(255,(int)shadowIntensity * 60), shadowColor)
                },
                Positions = new[] { 0, .1f, .6f, 1 }
            };

            if (shadowRotation != 0)
            {
                graph.TranslateTransform(scx, scy);
                graph.RotateTransform(shadowRotation);
                graph.TranslateTransform(-scx, -scy);
            }

            using (var pathShadow = new GraphicsPath())
            {
                pathShadow.AddEllipse(rectangleShadow);
                using (var pthGrBrush = new PathGradientBrush(pathShadow))
                {
                    pthGrBrush.InterpolationColors = colorBlend;
                    graph.FillEllipse(pthGrBrush, rectangleShadow);
                }
            }

            if (shadowRotation != 0)
            {
                graph.ResetTransform();
            }
        }

        private static bool SaveBitmapToFile(Bitmap bmp, string filePath)
        {
            try
            {
                bmp.Save(filePath);
                return true;
            }
            catch (Exception ex)
            {
                // something went wrong when trying to save the cached creature image.
                // could be related to have no write access if the portable version is placed in a protected folder like program files.
                MessageBoxes.ExceptionMessageBox(ex,
                    $"Error when trying to save species image file to\n{filePath}\n\nMaybe the folder is write protected, or the file is used by a different application.");
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
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawImage(bmpMaskOriginal, 0, 0, bmpBaseImage.Width, bmpBaseImage.Height);
                }

                BitmapData bmpDataBaseImage = bmpBaseImage.LockBits(
                    new Rectangle(0, 0, bmpBaseImage.Width, bmpBaseImage.Height), ImageLockMode.ReadOnly,
                    bmpBaseImage.PixelFormat);
                BitmapData bmpDataMask = bmpMask.LockBits(
                    new Rectangle(0, 0, bmpMask.Width, bmpMask.Height), ImageLockMode.ReadOnly,
                    bmpMask.PixelFormat);

                var bgBytes = bmpBaseImage.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;
                var msBytes = bmpDataMask.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;
                var bgHasTransparency = bgBytes > 3;

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
                                if (bgHasTransparency && dBg[3] == 0)
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

                                    float o;
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
                                        default: continue;
                                    }

                                    if (o == 0) continue;

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
    }
}
