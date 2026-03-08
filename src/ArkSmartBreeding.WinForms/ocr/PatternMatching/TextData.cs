using System.Collections.Generic;

namespace ARKBreedingStats.ocr.PatternMatching
{
    /// <summary>
    /// Represents all the possible patterns for a specific string, usually one character.
    /// </summary>
    public class TextData
    {
        /// <summary>
        /// The text represented by the Patterns.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// All the known patterns for the Text.
        /// </summary>
        public List<Pattern> Patterns { get; set; } = new List<Pattern>();
    }
}