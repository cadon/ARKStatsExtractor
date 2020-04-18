using System.Collections.Generic;

namespace ARKBreedingStats.ocr.Common
{
    public class CharData
    {
        public char Char { get; set; }

        public List<bool[,]> Patterns { get; set; } = new List<bool[,]>();
    }
}