using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.SpeciesImages;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ARKBreedingStats.species
{
    /// <summary>
    /// Creates an image of a species with given colors for the regions.
    /// </summary>
    internal static class CreatureColored
    {
        /// <summary>
        /// Size of the cached images.
        /// </summary>
        private const int TemplateSize = 256;

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
                var bmp = await GetColoredCreatureAsync(colorIds, species,
                    enabledColorRegions, size, pieSize, creatureSex: creatureSex, game: game);
                uiElement.Invoke(new Action(() => callBack(bmp)));
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

            var speciesBaseImageFilePath = await CreatureImageFile.SpeciesImageFilePath(species, game, creatureSex, patternId, useComposition: true).ConfigureAwait(false);

            if (string.IsNullOrEmpty(speciesBaseImageFilePath))
                return onlyImage ? null : DrawPieChart(colorIds, enabledColorRegions, size, pieSize); // no available images

            var speciesColorMaskFilePath = CreatureImageFile.MaskFilePath(speciesBaseImageFilePath);

            string cacheFilePath = CreatureImageFile.ColoredCreatureCacheFilePath(Path.GetFileNameWithoutExtension(speciesBaseImageFilePath), colorIds);
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
                    // use temp bitmap to avoid persistent file locking
                    using (var bmpTemp = new Bitmap(cacheFilePath))
                        return new Bitmap(bmpTemp);
                }
                catch
                {
                    // cached file corrupted, recreate
                    if (CreateAndSaveCacheSpeciesFile(colorIds, enabledColorRegions, speciesBaseImageFilePath,
                        speciesColorMaskFilePath, cacheFilePath))
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

        internal static Bitmap DrawPieChart(byte[] colorIds, bool[] enabledColorRegions, int size, int pieSize)
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
        internal static bool CreateAndSaveCacheSpeciesFile(byte[] colorIds, bool[] enabledColorRegions,
            string speciesBaseImageFilePath, string speciesColorMaskFilePath, string cacheFilePath, int outputSize = 256)
        {
            if (string.IsNullOrEmpty(cacheFilePath)
                || !File.Exists(speciesBaseImageFilePath))
                return false;

            using (Bitmap bmpBaseImage = new Bitmap(speciesBaseImageFilePath))
            using (Bitmap bmpColoredCreature = new Bitmap(bmpBaseImage.Width, bmpBaseImage.Height, PixelFormat.Format32bppArgb))
            using (Bitmap bmpShadow = new Bitmap(bmpBaseImage.Width, bmpBaseImage.Height, PixelFormat.Format32bppArgb))
            using (Graphics graph = Graphics.FromImage(bmpColoredCreature))
            {
                bool imageFine = true;
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                var rectangleShadow = Rectangle.Empty;

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
                    imageFine = ApplyColorsUnsafe(rgb, useColorRegions, speciesColorMaskFilePath, bmpBaseImage, bmpShadow, out rectangleShadow);
                }

                if (imageFine)
                {
                    DrawShadowSimpleEllipse(graph, bmpShadow);
                    //DrawShadow(graph, bmpShadow, rectangleShadow);
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

        private static void DrawShadow(Graphics graph, Bitmap bmpShadow, Rectangle rectangleShadow)
        {
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graph.SmoothingMode = SmoothingMode.HighQuality;
            graph.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // destination points of: upper left, upper right, lower left
            var upperLeft = new PointF(rectangleShadow.X + rectangleShadow.Width / 3f, rectangleShadow.Y + rectangleShadow.Height / 2f);
            var desPoints = new[]
            {
                upperLeft,
                new PointF(upperLeft.X+rectangleShadow.Width, upperLeft.Y),
                new PointF(rectangleShadow.Left,rectangleShadow.Bottom)
            };

            // set opacity
            var att = new ImageAttributes();
            att.SetColorMatrix(new ColorMatrix { Matrix33 = 0.2f });

            graph.DrawImage(bmpShadow, desPoints, rectangleShadow, GraphicsUnit.Pixel, att);
        }

        private static void DrawShadowSimpleEllipse(Graphics graph, Bitmap bmpBaseImage)
        {
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
        private static bool ApplyColorsUnsafe(byte[][] rgb, bool[] enabledColorRegions, string speciesColorMaskFilePath, Bitmap bmpBaseImage, Bitmap bmpShadow, out Rectangle shadowRect)
        {
            var imageFine = false;
            var shadowLeft = int.MaxValue;
            var shadowRight = -1;
            var shadowTop = int.MaxValue;
            var shadowBottom = -1;
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
                BitmapData bmpDataShadow = bmpShadow.LockBits(
                    new Rectangle(0, 0, bmpShadow.Width, bmpShadow.Height), ImageLockMode.ReadOnly,
                    bmpShadow.PixelFormat);

                int bgBytes = bmpBaseImage.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;
                int msBytes = bmpDataMask.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;
                int shdBytes = bmpShadow.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;

                try
                {
                    unsafe
                    {
                        byte* scan0Bg = (byte*)bmpDataBaseImage.Scan0.ToPointer();
                        byte* scan0Ms = (byte*)bmpDataMask.Scan0.ToPointer();
                        byte* scan0Sh = (byte*)bmpDataShadow.Scan0.ToPointer();

                        var width = bmpDataBaseImage.Width;
                        var height = bmpDataBaseImage.Height;
                        var strideBaseImage = bmpDataBaseImage.Stride;
                        var strideMask = bmpDataMask.Stride;
                        var strideShadow = bmpDataShadow.Stride;

                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                byte* dBg = scan0Bg + j * strideBaseImage + i * bgBytes;
                                // continue if the pixel is transparent
                                if (dBg[3] == 0)
                                    continue;

                                // set shadow alpha
                                var shadowPixelPt = scan0Sh + j * strideShadow + i * shdBytes;
                                shadowPixelPt[3] = dBg[3];
                                if (i < shadowLeft) shadowLeft = i;
                                if (i > shadowRight) shadowRight = i;
                                if (j < shadowTop) shadowTop = j;
                                if (j > shadowBottom) shadowBottom = j;

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
                bmpShadow.UnlockBits(bmpDataShadow);
            }
            shadowRect = shadowLeft != -1 ? new Rectangle(shadowLeft, shadowTop, shadowRight - shadowLeft, shadowBottom - shadowTop) : Rectangle.Empty;

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
