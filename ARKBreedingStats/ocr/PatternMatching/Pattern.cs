using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ARKBreedingStats.ocr.PatternMatching
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Pattern
    {
        public Pattern(bool[,] arr)
        {
            Data = arr;
            if (Data != null)
                UpdateProperties();
        }

        [OnDeserialized]
        private void UpdateProperties(StreamingContext _) => UpdateProperties();
        private void UpdateProperties()
        {
            Width = (byte)Data.GetLength(0);
            Height = (byte)Data.GetLength(1);
            Length = (short)Data.Length;
            SetPixels = (short)Data.Cast<bool>().Count(b => b);
        }

        [JsonProperty, JsonConverter(typeof(Boolean2DimArrayConverter))]
        public bool[,] Data { get; set; }

        /// <summary>
        /// Total count of pixels.
        /// </summary>
        public short Length;
        /// <summary>
        /// Width of character.
        /// </summary>
        public byte Width;
        /// <summary>
        /// Height of character.
        /// </summary>
        public byte Height;
        /// <summary>
        /// Count of character pixels.
        /// </summary>
        public short SetPixels;

        public bool this[int x, int y]
        {
            get => Data[x, y];
            set
            {
                Data[x, y] = value;
                UpdateProperties();
            }
        }

        public static implicit operator Pattern(bool[,] arr) => new Pattern(arr);
        public static explicit operator bool[,](Pattern arr) => (bool[,])arr.Data.Clone();


        public override string ToString() => OcrUtils.BoolArrayToString(Data);

        public Pattern Clone() => new Pattern((bool[,])Data.Clone());

        /// <summary>
        /// Assuming a black/white bmp of the letter, returns the according Pattern, cropped.
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Pattern FromBmp(Bitmap bmp)
        {
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

            int bmpBytes = bmpData.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;

            var allBits = new bool[bmp.Width, bmp.Height];
            var xStart = -1;
            var xEnd = -1;
            bool inChar = false;

            unsafe
            {
                byte* scan0Bg = (byte*)bmpData.Scan0.ToPointer();

                for (int i = 0; i < bmpData.Width; i++)
                {
                    if (!inChar)
                    {
                        // check if char starts in this column
                        for (int j = 0; j < bmpData.Height; j++)
                        {
                            byte* dBg = scan0Bg + j * bmpData.Stride + i * bmpBytes;
                            // check if pixel is white by checking the red channel
                            if (dBg[2] > 200)
                            {
                                inChar = true;
                                xStart = i;
                                break;
                            }
                        }
                    }

                    if (!inChar) continue;

                    for (int j = 0; j < bmpData.Height; j++)
                    {
                        byte* dBg = scan0Bg + j * bmpData.Stride + i * bmpBytes;
                        if (dBg[2] > 200)
                        {
                            allBits[i, j] = true;
                            xEnd = i;
                        }
                    }
                }
            }
            bmp.UnlockBits(bmpData);

            // create cropped pattern from xStart to xEnd
            if (xStart < 0 || xStart > xEnd) return null;

            var width = xEnd - xStart + 1;
            var croppedBits = new bool[width, bmp.Height];

            for (int x = 0; x < width; x++)
            {
                var xOriginal = x + xStart;
                for (int y = 0; y < bmp.Height; y++)
                {
                    if (allBits[xOriginal, y])
                        croppedBits[x, y] = true;
                }
            }

            return croppedBits;
        }

        /// <summary>
        /// Checks if the pattern is equal to the passed one.
        /// </summary>
        public bool Equals(Pattern otherPattern)
        {
            if (otherPattern == null
                || Width != otherPattern.Width
                || Height != otherPattern.Height
                || SetPixels != otherPattern.SetPixels
                ) return false;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (Data[x, y] != otherPattern.Data[x, y])
                        return false;

            return true;
        }
    }
}
