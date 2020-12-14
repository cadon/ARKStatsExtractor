using System.Collections.Generic;

namespace ARKBreedingStats.ocr.PatternMatching
{
    public class RecognizedCharData
    {
        public RecognizedCharData(int x, int y)
        {
            Coords = new Coords(x, y);
        }

        public Coords Coords { get; }

        public bool[,] Pattern { get; set; }

        public TextData ToCharData(string s)
        {
            return new TextData
            {
                Text = s,
                Patterns = new List<Pattern> { Pattern }
            };
        }

        public override string ToString()
        {
            return OcrUtils.BoolArrayToString(Pattern);
        }
    }
}