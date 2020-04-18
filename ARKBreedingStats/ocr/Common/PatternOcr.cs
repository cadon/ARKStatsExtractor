using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ARKBreedingStats.ocr.Common
{
    public static class PatternOcr
    {
        private static readonly int[] offsetX = { -1, 0, 1, 0, -1, 1, 1, -1 };
        private static readonly int[] offsetY = { 0, -1, 0, 1, -1, -1, 1, 1 };
        // ReSharper disable once InconsistentNaming
        private const byte FF = 0xFF;

        public static string ReadImageOcr(Bitmap source, bool onlyNumbers, float brightAdj = 1f)
        {
            var ret = "";

            using (var db = ImageUtils.GetAdjustedDirectBitmapOfImage(source, brightAdj))
            {
                var adjPic = db.ToBitmap();

                var charSymbols = SplitBySymbol(db, onlyNumbers);
                foreach (var sym in charSymbols)
                {
                    var c = RecognitionPatterns.Settings.FindMatchingChar(sym, adjPic);
                    ret += c ?? throw new OperationCanceledException();
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
                        if (onlyNumbers && xSize > 7 && xSize > ySize)
                        {
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
            data = data ?? new CoordsData(x, x, y, y);
            data.Add(x, y);

            for (int i = 0; i < offsetX.Length; i++)
            {
                var nextX = offsetX[i] + x;
                var nextY = offsetY[i] + y;

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

        private static bool IsNotBlank(DirectBitmap db, int x, int y)
        {
            return db.GetPixel(x, y).R != FF;
        }

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
