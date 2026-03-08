using System.Collections.Generic;

namespace ARKBreedingStats.ocr.PatternMatching
{
    public class RecognizedCharData
    {
        public RecognizedCharData(int x, int y, byte yOffset)
        {
            Coords = new Coords(x, y);
            YOffset = yOffset;
        }

        /// <summary>
        /// Coordinates of the pattern in the containing bitmap.
        /// </summary>
        public Coords Coords { get; }

        /// <summary>
        /// Y offset if the pattern doesn't start at the top.
        /// </summary>
        public byte YOffset;

        public bool[,] Pattern { get; set; }

        public string Text;

        public override string ToString() => Text;
        //public override string ToString() => OcrUtils.BoolArrayToString(Pattern);
    }
}