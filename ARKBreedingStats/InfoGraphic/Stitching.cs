using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using ARKBreedingStats.Library;
using ARKBreedingStats.utils;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ARKBreedingStats.InfoGraphic
{
    internal static class Stitching
    {
        private record Placement(
               string Path,
               int X,
               int Y);

        /// <summary>
        /// Creates a single stitched image from multiple image files.
        /// </summary>
        /// <param name="creatures">Array of creatures for creating the infographics</param>
        /// <param name="outputFilePath">The file path where the stitched image will be saved.</param>
        /// <param name="maxWidth">The maximum width of the stitched image. Images exceeding this width will wrap to the next row.</param>
        /// <param name="backgroundColor">The background color of the stitched image.</param>
        /// <param name="gap">The gap (in pixels) between images in the stitched image. Default is 0.</param>
        /// <returns>
        /// <c>true</c> if the stitched image was created and saved successfully; otherwise, <c>false</c>.
        /// </returns>
        public static async Task<int> CreateStitchedImages(IList<Creature> creatures, CreatureCollection cc, string outputFilePath, int maxWidth, Color backgroundColor, int gap = 0)
        {
            if (creatures == null || string.IsNullOrEmpty(outputFilePath) || maxWidth < 1) return 0;

            var placements = new List<Placement>();
            var currentX = gap;
            var currentY = gap;
            var rowHeight = 0;
            var totalWidth = 0;

            foreach (var c in creatures)
            {
                using var img = await c.InfoGraphicAsync(cc);
                // no image for creatures the selected style cannot draw, e.g. one that was never
                // extracted and so has no stat levels. Skip it rather than failing the whole sheet.
                if (img == null) continue;

                var filePath = Path.GetTempFileName();
                img.Save(filePath);

                var width = img.Width;
                var height = img.Height;

                if (currentX + width + gap > maxWidth)
                {
                    currentY += rowHeight;
                    currentX = gap;
                    rowHeight = 0;
                }

                placements.Add(new Placement(
                    filePath,
                    currentX,
                    currentY));

                currentX += width + gap;
                totalWidth = Math.Max(totalWidth, currentX);
                rowHeight = Math.Max(rowHeight, height + gap);
            }

            if (placements.Count == 0) return 0;

            var totalHeight = currentY + rowHeight + gap;

            try
            {
                using var bmp = new Bitmap(totalWidth, totalHeight, PixelFormat.Format32bppArgb);
                using var g = Graphics.FromImage(bmp);
                g.CompositingMode = CompositingMode.SourceOver;
                var fileExtension = Path.GetExtension(outputFilePath);
                using var backgroundBrush = new SolidBrush(backgroundColor);
                if (fileExtension != ".png" && backgroundColor.A < 255)
                {
                    backgroundBrush.Color = Color.White;
                    g.FillRectangle(backgroundBrush, 0, 0, totalWidth, totalHeight);
                    backgroundBrush.Color = backgroundColor;
                }
                if (backgroundColor.A != 0)
                    g.FillRectangle(backgroundBrush, 0, 0, totalWidth, totalHeight);

                foreach (var imgF in placements)
                {
                    using (var img = new Bitmap(imgF.Path))
                        g.DrawImage(img, imgF.X, imgF.Y);
                    FileService.TryDeleteFile(imgF.Path);
                }

                await using var fs = File.OpenWrite(outputFilePath);
                bmp.Save(fs, fileExtension == ".png" ? ImageFormat.Png : ImageFormat.Jpeg);
                return placements.Count;
            }
            catch (Exception ex)
            {
                MessageBoxes.ExceptionMessageBox(ex, "Error while stitching images");
                return 0;
            }
        }
    }
}
