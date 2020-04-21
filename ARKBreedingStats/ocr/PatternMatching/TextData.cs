using System.Collections;
using System.Collections.Generic;

namespace ARKBreedingStats.ocr.PatternMatching
{
    public class TextData
    {
        public string Text { get; set; }

        public List<Pattern> Patterns { get; set; } = new List<Pattern>();
    }
}