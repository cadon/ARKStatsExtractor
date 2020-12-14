using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace ARKBreedingStats.ocr.PatternMatching
{
    public static class PatternOcr
    {
        private static readonly int[] OffsetX = { -1, 0, 1, 0, -1, 1, 1, -1 };
        private static readonly int[] OffsetY = { 0, -1, 0, 1, -1, -1, 1, 1 };
        // ReSharper disable once InconsistentNaming
        private const byte FF = 0xFF;

        public static string ReadImageOcr(Bitmap source, bool onlyNumbers, float brightAdj = 1f, OCRControl ocrControl = null)
        {
            var ret = string.Empty;

            using (var db = ImageUtils.GetAdjustedDirectBitmapOfImage(source, brightAdj)) // TODO use whiteThreshold from user
            {
                var adjPic = db.ToBitmap();

                var charSymbols = SplitBySymbol(db, onlyNumbers);

                int xPos = charSymbols.FirstOrDefault()?.Coords.X ?? 0;

                foreach (var sym in charSymbols)
                {
                    // read spaces
                    if (!onlyNumbers && sym.Coords.X - xPos > 7)
                        ret += " ";
                    xPos = sym.Coords.X + sym.Pattern.GetLength(0);

                    var c = ArkOCR.OCR.ocrConfig.RecognitionPatterns.FindMatchingChar(sym, adjPic, onlyNumbers: onlyNumbers);
                    // if c==string.Empty: character was not recognized and skipped in the manual recognition
                    if (string.IsNullOrEmpty(c))
                    {
                        if (c == null) return ret;
                        continue;
                    }

                    ret += c;
                    ocrControl?.AddLetterToRecognized(c, sym.Pattern);
                }
            }

            ret = CleanUpOcr(ret);

            return ret;
        }

        private static string CleanUpOcr(string ret)
        {
            return ret.Replace("..", ".").Replace("  ", " ");
        }

        private static IEnumerable<RecognizedCharData> SplitBySymbol(DirectBitmap db, bool onlyNumbers)
        {
            var ret = new List<RecognizedCharData>();

            var visited = new bool[db.Width, db.Height];
            for (int i = 0; i < db.Width; i++)
            {
                for (int j = 0; j < db.Height; j++)
                {
                    var canBeVisited = !visited[i, j];
                    visited[i, j] = true;
                    if (canBeVisited && IsNotBlank(db, i, j))
                    {
                        var coordsData = VisitChar(i, j, visited, db);
                        VisitVertical(coordsData, visited, db);
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
                    }
                }
            }

            return ret;
        }

        private static void VisitVertical(CoordsData coordsData, bool[,] visited, DirectBitmap db)
        {
            for (int i = coordsData.MinX; i <= coordsData.MaxX; i++)
            {
                for (int j = 0; j < db.Height; j++)
                {
                    if (!visited[i, j] && IsNotBlank(db, i, j))
                    {
                        coordsData.Add(i, j);
                    }

                    visited[i, j] = true;
                }
            }
        }

        private static CoordsData VisitChar(int x, int y, bool[,] visited, DirectBitmap db, CoordsData data = null)
        {
            // TODO better region growing algorithm?
            if (data == null)
                data = new CoordsData(x, x, y, y);
            data.Add(x, y);

            for (int i = 0; i < OffsetX.Length; i++)
            {
                var nextX = OffsetX[i] + x;
                var nextY = OffsetY[i] + y;

                var isSafe = nextX > 0 && nextX < db.Width && nextY > 0 && nextY < db.Height && !visited[nextX, nextY];
                if (!isSafe)
                {
                    continue;
                }

                visited[nextX, nextY] = true;

                if (IsNotBlank(db, nextX, nextY))
                {
                    VisitChar(nextX, nextY, visited, db, data);
                }
            }

            return data;
        }

        private static bool IsNotBlank(DirectBitmap db, int x, int y) => db.GetPixel(x, y).R != FF;

        private static void SplitIn2Chars(List<RecognizedCharData> ret, RecognizedCharData charData)
        {
            var xSize = charData.Pattern.GetLength(0);
            var maxX = xSize / 2;
            var maxY = charData.Pattern.GetLength(1);

            var c1 = new bool[maxX, maxY];
            var c2 = new bool[maxX, maxY];
            for (int i = 0; i < maxX; i++)
            {
                for (int j = 0; j < maxY; j++)
                {
                    c1[i, j] = charData.Pattern[i, j];
                }
            }

            var start = xSize - maxX;
            for (int i = xSize - maxX; i < xSize; i++)
            {
                for (int j = 0; j < maxY; j++)
                {
                    c2[i - start, j] = charData.Pattern[i, j];
                }
            }

            ret.Add(new RecognizedCharData(charData.Coords.X, charData.Coords.Y) { Pattern = c1 });
            ret.Add(new RecognizedCharData(start, charData.Coords.Y) { Pattern = c2 });
        }

        public static string RemoveNonNumeric(string ocrValue)
        {
            return new string(ocrValue.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());
        }
    }
}
