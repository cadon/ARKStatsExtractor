using System.Collections.Generic;

namespace ARKBreedingStats.ocr.PatternMatching
{
    internal class CoordsData
    {
        public CoordsData(int minX, int maxX, int minY, int maxY)
        {
            MinY = minY;
            MaxY = maxY;
            MinX = minX;
            MaxX = maxX;
        }

        public List<Coords> Coords { get; } = new List<Coords>();

        public void Add(int x, int y)
        {
            if (x > MaxX)
            {
                MaxX = x;
            }
            else if (x < MinX)
            {
                MinX = x;
            }

            if (y > MaxY)
            {
                MaxY = y;
            }
            else if (y < MinY)
            {
                MinY = y;
            }

            Coords.Add(new Coords(x, y));
        }

        public int MinX { get; private set; }
        public int MaxX { get; private set; }
        private int MinY { get; set; }
        private int MaxY { get; set; }

        public RecognizedCharData ToRecognizedCharData()
        {
            var xSize = MaxX - MinX + 1;
            var ySize = MaxY - MinY + 1;
            var boolArr = new bool[xSize, ySize];

            foreach (var c in Coords)
            {
                var x = c.X - MinX;
                var y = c.Y - MinY;
                boolArr[x, y] = true;
            }

            return new RecognizedCharData(MinX, MinY, (byte)MinY)
            {
                Pattern = boolArr
            };
        }

        public void ToDebugConsole()
        {
            var xSize = MaxX - MinX + 1;
            var ySize = MaxY - MinY + 1;
            var boolArr = new bool[xSize, ySize];

            foreach (var c in Coords)
            {
                var x = c.X - MinX;
                var y = c.Y - MinY;
                boolArr[x, y] = true;
            }

            Boolean2DimArrayConverter.ToDebugLog(boolArr);
        }
    }
}