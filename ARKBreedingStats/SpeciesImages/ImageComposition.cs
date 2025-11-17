using System.Windows;
using Newtonsoft.Json;

namespace ARKBreedingStats.SpeciesImages
{
    /// <summary>
    /// Describes how an image is composed.
    /// </summary>
    [JsonObject]
    internal class ImageComposition
    {
        /// <summary>
        /// Name of the image pack this part is taken.
        /// </summary>
        public string ImagePackName;
        /// <summary>
        /// Part of the image taken from the source.
        /// </summary>
        public Rect RectSource;

        /// <summary>
        /// Area of the final image this image part is placed.
        /// </summary>
        public Rect RectPosition;
    }
}
