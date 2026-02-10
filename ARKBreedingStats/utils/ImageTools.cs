using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ARKBreedingStats.utils
{
    internal class ImageTools
    {
        /// <summary>
        /// Blurs an image by the specified radius (using box blur), the result bitmap is expanded by 2 times the radius.
        /// </summary>
        internal static Bitmap BlurImageAlpha(Bitmap bmp, int radius)
        {
            if (bmp == null || radius <= 0) return bmp;

            var expandedWidth = bmp.Width + 2 * radius;
            var expandedHeight = bmp.Height + 2 * radius;

            var expandedBitmap = new Bitmap(expandedWidth, expandedHeight, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(expandedBitmap))
                g.DrawImage(bmp, radius, radius);

            var blurredBitmap = new Bitmap(expandedWidth, expandedHeight, PixelFormat.Format32bppArgb);

            var rect = new Rectangle(0, 0, expandedWidth, expandedHeight);
            var sourceData = expandedBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var destData = blurredBitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                var bytesPerPixel = Image.GetPixelFormatSize(PixelFormat.Format32bppArgb) / 8;
                var stride = destData.Stride;
                var buffer = new byte[stride * expandedHeight];
                System.Runtime.InteropServices.Marshal.Copy(sourceData.Scan0, buffer, 0, buffer.Length);

                var kernelSize = 2 * radius + 1;
                var kernelSum = 0;
                var kernel = new int[kernelSize];
                for (var i = 0; i < kernelSize; i++)
                {
                    kernel[i] = 1;
                    kernelSum++;
                }

                var tempBuffer = new byte[buffer.Length];

                for (var y = 0; y < expandedHeight; y++)
                    for (var x = 0; x < expandedWidth; x++)
                    {
                        //int r = 0, g = 0, b = 0;
                        int a = 0;

                        for (var k = -radius; k <= radius; k++)
                        {
                            var offsetX = x + k;
                            if (offsetX < 0 || offsetX >= expandedWidth) continue;

                            var pixelIndex = y * stride + offsetX * bytesPerPixel;
                            //b += buffer[pixelIndex] * kernel[k + radius];
                            //g += buffer[pixelIndex + 1] * kernel[k + radius];
                            //r += buffer[pixelIndex + 2] * kernel[k + radius];
                            a += buffer[pixelIndex + 3] * kernel[k + radius];
                        }

                        var destIndex = y * stride + x * bytesPerPixel;
                        tempBuffer[destIndex] = buffer[destIndex];
                        tempBuffer[destIndex + 1] = buffer[destIndex + 1];
                        tempBuffer[destIndex + 2] = buffer[destIndex + 2];
                        tempBuffer[destIndex + 3] = (byte)(a / kernelSum);
                    }

                buffer = tempBuffer;
                tempBuffer = new byte[buffer.Length];

                for (var x = 0; x < expandedWidth; x++)
                    for (var y = 0; y < expandedHeight; y++)
                    {
                        //int r = 0, g = 0, b = 0;
                        int a = 0;

                        for (var k = -radius; k <= radius; k++)
                        {
                            var offsetY = y + k;
                            if (offsetY < 0 || offsetY >= expandedHeight) continue;

                            var pixelIndex = offsetY * stride + x * bytesPerPixel;
                            //b += buffer[pixelIndex] * kernel[k + radius];
                            //g += buffer[pixelIndex + 1] * kernel[k + radius];
                            //r += buffer[pixelIndex + 2] * kernel[k + radius];
                            a += buffer[pixelIndex + 3] * kernel[k + radius];
                        }

                        var destIndex = y * stride + x * bytesPerPixel;
                        tempBuffer[destIndex] = buffer[destIndex];
                        tempBuffer[destIndex + 1] = buffer[destIndex + 1];
                        tempBuffer[destIndex + 2] = buffer[destIndex + 2];
                        tempBuffer[destIndex + 3] = (byte)(a / kernelSum);
                    }

                System.Runtime.InteropServices.Marshal.Copy(tempBuffer, 0, destData.Scan0, tempBuffer.Length);
            }
            finally
            {
                expandedBitmap.UnlockBits(sourceData);
                blurredBitmap.UnlockBits(destData);
            }

            return blurredBitmap;
        }

        /// <summary>
        /// Returns a new bitmap consisting of an outline drawn around the non-transparent parts of a base image (without the base image). The outline has a given color with a given radius, the outline can be blurry.
        /// Only pixels that have an alpha value above alphaThreshold are considered opaque and will get an outline in the result.
        /// </summary>
        /// <param name="outlineBlurriness">0 to 1, 0 rather sharp, 1 blurry</param>
        internal static unsafe Bitmap OutlineOpacities(Bitmap bmpBase, Color outlineColor, int outlineSize,
            float outlineBlurriness = 1, byte alphaThreshold = 60)
        {
            if (bmpBase == null || outlineSize <= 0) return null;
            if (bmpBase.PixelFormat != PixelFormat.Format32bppArgb) return null;
            outlineSize = Math.Min(100, outlineSize); // limit to 100

            var width = bmpBase.Width;
            var height = bmpBase.Height;
            var widthResult = width + 2 * outlineSize;
            var heightResult = height + 2 * outlineSize;

            // Create a result bitmap with expanded size to accommodate the outline
            var resultBitmap = new Bitmap(widthResult, heightResult, PixelFormat.Format32bppArgb);

            // Lock the bitmaps for direct memory access
            var baseData = bmpBase.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bmpBase.PixelFormat);
            var resultData = resultBitmap.LockBits(new Rectangle(0, 0, widthResult, heightResult),
                ImageLockMode.WriteOnly, resultBitmap.PixelFormat);

            try
            {
                // brush for outline
                var outlinePlus1 = outlineSize + 1;
                var alphaBrush = new byte[outlinePlus1 * outlinePlus1];
                var brushBorderFactor = (outlineBlurriness - 0.5) * -4;
                for (var x = 0; x < outlinePlus1; x++)
                    for (var y = 0; y < outlinePlus1; y++)
                    {
                        var relativeDistance = Math.Min(1, Math.Sqrt(x * x + y * y) / outlineSize);

                        var expTimesLinear = Math.Min(1, Math.Exp(brushBorderFactor * relativeDistance) * (1 - relativeDistance));
                        alphaBrush[y * outlinePlus1 + x] = (byte)(outlineColor.A * expTimesLinear);
                    }

                var bytesPerPixel = Image.GetPixelFormatSize(PixelFormat.Format32bppArgb) / 8;
                var baseStride = baseData.Stride;
                var resultStride = resultData.Stride;

                var basePtr = (byte*)baseData.Scan0;
                var resultPtr = (byte*)resultData.Scan0;

                // Loop through each pixel in the base image
                for (var y = 0; y < heightResult; y++)
                    for (var x = 0; x < widthResult; x++)
                    {
                        // set color
                        var resultPixel = resultPtr + y * resultStride + x * bytesPerPixel;
                        resultPixel[0] = outlineColor.B;
                        resultPixel[1] = outlineColor.G;
                        resultPixel[2] = outlineColor.R;

                        // check if pixel is considered transparent and one but not all neighbour pixels are opaque
                        var xBase = x - outlineSize;
                        var yBase = y - outlineSize;
                        if (xBase < 0 || xBase >= width || yBase < 0 || yBase >= height || basePtr[yBase * baseStride + xBase * bytesPerPixel + 3] <= alphaThreshold) continue;

                        var rightSolid = xBase + 1 < width && basePtr[yBase * baseStride + (xBase + 1) * bytesPerPixel + 3] > alphaThreshold;
                        var leftSolid = xBase - 1 >= 0 && basePtr[yBase * baseStride + (xBase - 1) * bytesPerPixel + 3] > alphaThreshold;
                        var downSolid = yBase + 1 < height && basePtr[(yBase + 1) * baseStride + xBase * bytesPerPixel + 3] > alphaThreshold;
                        var upSolid = yBase - 1 >= 0 && basePtr[(yBase - 1) * baseStride + xBase * bytesPerPixel + 3] > alphaThreshold;

                        if (!(rightSolid || leftSolid || downSolid || upSolid)) continue;
                        if (rightSolid && leftSolid && downSolid && upSolid) continue;

                        // draw outline
                        for (var oy = -outlineSize; oy <= outlineSize; oy++)
                            for (var ox = -outlineSize; ox <= outlineSize; ox++)
                            {
                                var nx = x + ox;
                                var ny = y + oy;
                                if (nx < 0 || nx >= widthResult || ny < 0 || ny >= heightResult) continue;

                                var alpha = alphaBrush[Math.Abs(oy) * outlinePlus1 + Math.Abs(ox)];
                                if (alpha == 0) continue;
                                // set alpha

                                if (alpha > resultPtr[ny * resultStride + nx * bytesPerPixel + 3])
                                    resultPtr[ny * resultStride + nx * bytesPerPixel + 3] = alpha;
                            }
                    }
            }
            finally
            {
                // Unlock the bitmaps
                bmpBase.UnlockBits(baseData);
                resultBitmap.UnlockBits(resultData);
            }

            return resultBitmap;
        }
    }
}
