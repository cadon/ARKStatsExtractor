using System.Drawing;
using System.Drawing.Imaging;

namespace ARKBreedingStats.ocr.PatternMatching
{
    public static class ImageUtils
    {
        /// <summary>
        /// Creates a boolean 2 dim array representing the passed image depending on the threshold.
        /// </summary>
        public static bool[,] GetBooleanArrayOfImage(Image img, byte whiteThreshold)
        {
            var w = img.Width;
            var h = img.Height;

            var arr = new bool[w, h];

            var bmpData = ((Bitmap)img).LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            var bBytes = img.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;
            unsafe
            {
                byte* scan0 = (byte*)bmpData.Scan0.ToPointer();

                for (var x = 0; x < w; x++)
                {
                    for (var y = 0; y < h; y++)
                    {
                        byte* b = scan0 + y * bmpData.Stride + x * bBytes;

                        if (ArkOcr.HslLightness(b[0], b[1], b[2]) >= whiteThreshold)
                            arr[x, y] = true;
                    }
                }
            }
            ((Bitmap)img).UnlockBits(bmpData);

            return arr;
        }
    }
}
