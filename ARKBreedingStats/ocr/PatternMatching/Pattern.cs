using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ARKBreedingStats.ocr.PatternMatching
{
    /// <summary>
    /// Pattern of a character (or more), represented as a binary pixel array.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Pattern
    {
        public Pattern(bool[,] arr, byte yOffset = 0)
        {
            Data = arr;
            YOffset = yOffset;
            if (Data != null)
                UpdateProperties();
        }

        [OnDeserialized]
        private void UpdateProperties(StreamingContext _) => UpdateProperties();
        internal void UpdateProperties()
        {
            Width = (byte)Data.GetLength(0);
            Height = (byte)Data.GetLength(1);
            HeightWithOffset = (byte)(Height + YOffset);
            Length = (short)Data.Length;
            SetPixels = (short)Data.Cast<bool>().Count(b => b);
            Apertures = GetApertures();
        }

        [JsonProperty, JsonConverter(typeof(Boolean2DimArrayConverter))]
        public bool[,] Data { get; set; }

        ///// <summary>
        ///// Used to debugging
        ///// </summary>
        //public (int x, int y)[] AperturePixels;

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
        /// Height of character and the y offset.
        /// </summary>
        public byte HeightWithOffset;
        /// <summary>
        /// Count of character pixels.
        /// </summary>
        public short SetPixels;
        /// <summary>
        /// Apertures
        /// </summary>
        public byte Apertures;

        /// <summary>
        /// Y coordinate where the pattern begins. E.g. a dot has a high offset being at the baseline.
        /// </summary>
        [JsonProperty("y")]
        public byte YOffset;

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

        public Pattern Clone() => new Pattern((bool[,])Data.Clone()) { YOffset = YOffset };

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
            var yStart = -1;
            var yEnd = -1;
            bool inChar = false;

            unsafe
            {
                byte* scan0Bg = (byte*)bmpData.Scan0.ToPointer();

                for (int x = 0; x < bmpData.Width; x++)
                {
                    if (!inChar)
                    {
                        // check if char starts in this column
                        for (int y = 0; y < bmpData.Height; y++)
                        {
                            byte* dBg = scan0Bg + y * bmpData.Stride + x * bmpBytes;
                            // check if pixel is white by checking the red channel
                            if (dBg[2] > 200)
                            {
                                inChar = true;
                                xStart = x;
                                break;
                            }
                        }
                    }

                    if (!inChar) continue;

                    for (int y = 0; y < bmpData.Height; y++)
                    {
                        byte* dBg = scan0Bg + y * bmpData.Stride + x * bmpBytes;
                        if (dBg[2] > 200)
                        {
                            allBits[x, y] = true;
                            if (yStart == -1 || y < yStart)
                                yStart = y;
                            if (xEnd < x)
                                xEnd = x;
                            if (yEnd < y)
                                yEnd = y;
                        }
                    }
                }
            }
            bmp.UnlockBits(bmpData);
            //Boolean2DimArrayConverter.ToDebugLog(allBits);

            if (xStart < 0 || xStart > xEnd || yStart < 0 || yStart > yEnd) return null;
            // create cropped pattern

            var width = xEnd - xStart + 1;
            var height = yEnd - yStart + 1;
            var croppedBits = new bool[width, height];

            for (int x = 0; x < width; x++)
            {
                var xOriginal = x + xStart;
                for (int y = 0; y < height; y++)
                {
                    if (allBits[xOriginal, y + yStart])
                        croppedBits[x, y] = true;
                }
            }

            return new Pattern(croppedBits, (byte)yStart);
        }

        /// <summary>
        /// Checks if the pattern is equal to the passed one.
        /// </summary>
        public bool Equals(Pattern otherPattern)
        {
            if (otherPattern == null
                || Apertures != otherPattern.Apertures
                || YOffset != otherPattern.YOffset
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

        /// <summary>
        /// Determines the apertures (i.e. holes) of the pattern and returns a byte that represents these.
        /// If an aperture is present, the bit is 1, else 0.
        /// </summary>
        private byte GetApertures()
        {
            // bit position of the apertures
            //
            // ..4..
            // 0.5.1
            // .....
            // 2.6.3
            // ..7..

            if (Data == null
                || Data.GetLength(0) < 5
                || Data.GetLength(1) + YOffset < 5
            ) return 0;

            // divide each side into two parts, then check if in the middle of a part there is an aperture at least 1/4 of the width or height.
            var widthHalf = Width / 2;
            var widthQuarter = Width / 4;
            var heightHalf = HeightWithOffset / 2;
            var heightQuarter = HeightWithOffset / 4;

            var fixedLocations = new[]{
                heightQuarter,
                heightQuarter,
                3 * heightHalf / 2,
                3 * heightHalf / 2,
                widthHalf,
                widthHalf,
                widthHalf,
                widthHalf
            };


            // TODO debug
            //Boolean2DimArrayConverter.ToDebugLog(Data);
            //var aperturePixelsList = new List<(int x, int y)>();

            var variableStart = new[] { 0, 3 * Width / 4, 0, 3 * Width / 4,
                0, heightQuarter, HeightWithOffset - heightHalf/2 -2,  HeightWithOffset -heightQuarter-1
                };
            var variableEnd = new[] {
                widthQuarter + 1, Width,widthQuarter + 1, Width,
                heightQuarter + 1, heightQuarter+ 2, HeightWithOffset - heightQuarter, HeightWithOffset
                };

            byte apertures = 0;
            for (int i = 0; i < 8; i++)
            {
                bool aperture = true;
                bool horizontal = (i >> 2) == 0;

                for (int j = variableStart[i]; j != variableEnd[i]; j++)
                {
                    if (horizontal)
                    {
                        //aperturePixelsList.Add((j, fixedLocations[i]));
                        if (fixedLocations[i] >= YOffset && Data[j, fixedLocations[i] - YOffset])
                        {
                            aperture = false;
                            break; // TODO
                        }
                    }
                    else
                    {
                        //aperturePixelsList.Add((fixedLocations[i], j));
                        if (j >= YOffset && Data[fixedLocations[i], j - YOffset])
                        {
                            aperture = false;
                            break; // TODO
                        }
                    }
                }
                if (aperture)
                    apertures += (byte)(1 << i);
            }

            //AperturePixels = aperturePixelsList.ToArray(); // TODO debug remove

            return apertures;
        }

        /// <summary>
        /// Creates a TextData with this pattern as the only pattern.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public TextData CreateTextData(string text)
        {
            return new TextData
            {
                Text = text,
                Patterns = new List<Pattern> { this }
            };
        }
    }
}
