using System.Collections.Generic;
using ARKBreedingStats.ocr.PatternMatching;

namespace ARKBreedingStats.ocr
{
    internal class CoordsData
    {
        public CoordsData(int minX, int maxX, int minY, int maxY)
        {
            this.MinY = minY;
            this.MaxY = maxY;
            this.MinX = minX;
            this.MaxX = maxX;
        }

        public List<Coords> Coords { get; } = new List<Coords>();

        public void Add(int x, int y)
        {
            if (x > this.MaxX)
            {
                this.MaxX = x;
            }
            else if (x < this.MinX)
            {
                this.MinX = x;
            }

            if (y > this.MaxY)
            {
                this.MaxY = y;
            }
            else if (y < this.MinY)
            {
                this.MinY = y;
            }

            this.Coords.Add(new Coords(x, y));
        }

        public int MaxX { get; private set; }
        public int MinX { get; private set; }
        public int MaxY { get; private set; }
        public int MinY { get; private set; }

        public RecognizedCharData ToRecognizedCharData()
        {
            var xSize = this.MaxX - this.MinX + 1;
            var ySize = this.MaxY - this.MinY + 1;
            var boolArr = new bool[xSize, ySize];

            foreach (var c in this.Coords)
            {
                var x = c.X - this.MinX;
                var y = c.Y - this.MinY;
                boolArr[x, y] = true;
            }
            
            return new RecognizedCharData(this.MinX, this.MinY)
            {
                Pattern = boolArr
            };
        }
    }
}