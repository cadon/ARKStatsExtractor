using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ARKBreedingStats.ocr.PatternMatching
{
    public static class PatternOcr
    {
        /// <summary>
        /// X offsets for all 8 neighboring pixels.
        /// </summary>
        private static readonly int[] OffsetX = { -1, 0, 1, 0, -1, 1, 1, -1 };
        /// <summary>
        /// Y offsets for all 8 neighboring pixels.
        /// </summary>
        private static readonly int[] OffsetY = { 0, -1, 0, 1, -1, -1, 1, 1 };

        /// <summary>
        /// Performs OCR on a bitmap and returns the recognized characters.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="onlyNumbers"></param>
        /// <param name="whiteThreshold">Value from 0 to 255, affects which pixels are considered white or black.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ocrControl">If given the read letters are added for further editing and comparing.</param>
        /// <returns></returns>
        public static string ReadImageOcr(Bitmap source, bool onlyNumbers, byte whiteThreshold, int x = 0, int y = 0, OCRControl ocrControl = null)
        {
            var ret = string.Empty;
            var maxDistanceForNonSpace = Math.Max(2, source.Height / 4);

            var imageArray = ImageUtils.GetBooleanArrayOfImage(source, whiteThreshold);

            //var debugArray = new bool[source.Width, source.Height];
            //for (int xx = 0; xx < source.Width; xx++)
            //{
            //    for (int yy = 0; yy < source.Height; yy++)
            //    {
            //        debugArray[xx, yy] = db.GetPixel(xx, yy).R == 0;
            //    }
            //}
            //Boolean2DimArrayConverter.ToDebugLog(debugArray);

            // some images are just lines, ignore these. the lines can be at any height, probably not worth checking these
            //if (source.Width > 3 * source.Height)
            //{
            //    bool isJustLine = true;
            //    var minNotSetPixel = (int)(source.Width * 0.1);
            //    for (int sx = 0; sx < source.Width; sx++)
            //    {
            //        if (db.GetPixel(sx, 0).R == byte.MaxValue)
            //        {
            //            minNotSetPixel--;
            //            if (minNotSetPixel < 0)
            //            {
            //                isJustLine = false;
            //                break;
            //            }
            //        }
            //    }
            //    if (isJustLine) return string.Empty;
            //}

            var charSymbols = SplitBySymbol(imageArray, onlyNumbers).ToArray();

            int xPos = charSymbols.FirstOrDefault()?.Coords.X ?? 0;

            foreach (var sym in charSymbols)
            {
                // read spaces
                if (!onlyNumbers && sym.Coords.X - xPos > maxDistanceForNonSpace)
                    ret += " ";

                xPos = sym.Coords.X + sym.Pattern.GetLength(0);

                var c = ArkOcr.Ocr.ocrConfig.RecognitionPatterns.FindMatchingChar(sym, imageArray, onlyNumbers: onlyNumbers);
                // if c==string.Empty: character was not recognized and skipped in the manual recognition
                if (string.IsNullOrEmpty(c))
                {
                    if (c == null)
                        return CleanUpOcr(ret); // recognition was cancelled
                    continue;
                }

                ret += c;
                //ocrControl?.AddLetterToRecognized(c, sym.Pattern);
                ocrControl?.AddLetterToRecognized(new RecognizedCharData(x + sym.Coords.X, y + sym.Coords.Y, sym.YOffset) { Text = c, Pattern = sym.Pattern });
            }

            return CleanUpOcr(ret);
        }

        private static string CleanUpOcr(string ret)
        {
            return ret.Replace("..", ".").Replace("  ", " ").Replace("♂`", "♂");
        }

        private static IEnumerable<RecognizedCharData> SplitBySymbol(bool[,] image, bool onlyNumbers)
        {
            var ret = new List<RecognizedCharData>();

            var w = image.GetLength(0);
            var h = image.GetLength(1);

            var visited = new bool[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    var canBeVisited = !visited[i, j];
                    visited[i, j] = true;
                    if (!canBeVisited || !image[i, j]) continue;

                    var coordsData = VisitChar(i, j, visited, image);
                    VisitVertical(coordsData, visited, image);

                    // TODO debug
                    //coordsData.ToDebugConsole();

                    var charData = coordsData.ToRecognizedCharData();
                    var p = charData.Pattern;
                    var xSize = p.GetLength(0);
                    var ySize = p.GetLength(1);
                    if (onlyNumbers && xSize > 7 && xSize > ySize + 1)
                    {
                        Debug.WriteLine($"Splitting too wide character (width: {xSize}, height: {ySize}):");
                        Boolean2DimArrayConverter.ToDebugLog(charData.Pattern);
                        SplitIn2Chars(ret, charData);
                    }
                    else
                    {
                        ret.Add(charData);
                    }

                    i += xSize - 1;
                    break;
                }
            }

            return ret;
        }

        private static void SaveDebugImage(DirectBitmap db, CoordsData coordsData, int nr)
        {
            using (var bmp = db.Bitmap.Clone(new Rectangle(0, 0, db.Width, db.Height), db.Bitmap.PixelFormat))
            {
                var setCoordsCount = coordsData.Coords.Count;
                var step = 255 / setCoordsCount;
                using (var g = Graphics.FromImage(bmp))
                {
                    using (var b = new SolidBrush(Color.Black))
                        for (int c = 0; c < setCoordsCount; c++)
                        {
                            b.Color = Color.FromArgb(255 - (c * step), c * step, 50);
                            g.FillRectangle(b, new Rectangle(coordsData.Coords[c].X, coordsData.Coords[c].Y, 1, 1));
                        }

                    const int factor = 5;
                    using (var resizedBmp = new Bitmap(bmp.Width * factor, bmp.Height * factor))
                    using (var gr = Graphics.FromImage(resizedBmp))
                    {
                        gr.DrawImage(bmp, new Rectangle(0, 0, bmp.Width * factor, bmp.Height * factor));
                        resizedBmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                            "ASBFontTesting", "RegionGrowing", $"RG_{DateTime.Now:yyyy-MM-dd-HH-mm-ss-ffff}_{nr}.png"));
                    }
                }
            }
        }

        /// <summary>
        /// Adds set pixels of all columns of the pattern that are not part of another pattern.
        /// </summary>
        private static void VisitVertical(CoordsData coordsData, bool[,] visited, bool[,] image)
        {
            var h = image.GetLength(1);

            for (int x = coordsData.MinX; x <= coordsData.MaxX; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (visited[x, y]) continue;

                    if (image[x, y])
                    {
                        VisitChar(x, y, visited, image, coordsData);
                    }

                    visited[x, y] = true;
                }
            }
        }

        ///// <summary>
        ///// Collect all neighboring pixels of a character (region growing with recursive calls).
        ///// </summary>
        //private static CoordsData VisitCharRecursively(int x, int y, bool[,] visited, bool[,] image, CoordsData data = null)
        //{
        //    if (data == null)
        //        data = new CoordsData(x, x, y, y);

        //    var w = image.GetLength(0);
        //    var h = image.GetLength(1);
        //    data.Add(x, y);

        //    for (int i = 0; i < OffsetX.Length; i++)
        //    {
        //        var nextX = OffsetX[i] + x;
        //        var nextY = OffsetY[i] + y;

        //        var isSafe = nextX >= 0 && nextX < w && nextY >= 0 && nextY < h && !visited[nextX, nextY];
        //        if (!isSafe)
        //        {
        //            continue;
        //        }

        //        visited[nextX, nextY] = true;

        //        if (image[nextX, nextY])
        //        {
        //            VisitCharRecursively(nextX, nextY, visited, image, data);
        //        }
        //    }

        //    // debug
        //    //data.ToDebugConsole();

        //    return data;
        //}

        /// <summary>
        /// Collect all neighboring pixels of a character (region growing with a stack).
        /// </summary>
        private static CoordsData VisitChar(int x, int y, bool[,] visited, bool[,] image, CoordsData data = null)
        {
            if (data == null)
                data = new CoordsData(x, x, y, y);

            var w = image.GetLength(0);
            var h = image.GetLength(1);
            var stackToCheck = new Stack<(int x, int y)>(image.GetLength(1) * 2);
            stackToCheck.Push((-1, -1));
            stackToCheck.Push((x, y));

            do
            {
                var (cx, cy) = stackToCheck.Pop();
                data.Add(cx, cy);
                visited[cx, cy] = true;

                for (int i = 0; i < OffsetX.Length; i++)
                {
                    var nextX = OffsetX[i] + cx;
                    var nextY = OffsetY[i] + cy;

                    // add neighbors that are set
                    if (nextX >= 0 && nextX < w && nextY >= 0 && nextY < h
                        && !visited[nextX, nextY]
                        && image[nextX, nextY])
                    {
                        visited[nextX, nextY] = true;
                        stackToCheck.Push((nextX, nextY));
                    }
                }
            }
            while (stackToCheck.Peek().x != -1);

            return data;
        }

        private static void SplitIn2Chars(List<RecognizedCharData> ret, RecognizedCharData charData)
        {
            var xSize = charData.Pattern.GetLength(0);
            var maxX = xSize / 2;
            var maxY = charData.Pattern.GetLength(1);

            var c1 = new bool[maxX, maxY];
            var c2 = new bool[maxX, maxY];
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    c1[x, y] = charData.Pattern[x, y];
                }
            }

            var start = xSize - maxX;
            for (int x = xSize - maxX; x < xSize; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    c2[x - start, y] = charData.Pattern[x, y];
                }
            }

            ret.Add(new RecognizedCharData(charData.Coords.X, charData.Coords.Y, (byte)charData.Coords.Y) { Pattern = c1 });
            ret.Add(new RecognizedCharData(start, charData.Coords.Y, (byte)charData.Coords.Y) { Pattern = c2 });
        }

        public static string RemoveNonNumeric(string ocrValue)
        {
            return new string(ocrValue.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());
        }
    }
}
