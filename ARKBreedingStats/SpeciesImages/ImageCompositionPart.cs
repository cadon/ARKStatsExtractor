using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ARKBreedingStats.SpeciesImages
{
    /// <summary>
    /// Describes how an image is composed.
    /// </summary>
    internal class ImageCompositionPart
    {
        /// <summary>
        /// Name of the image pack this part is taken.
        /// </summary>
        [JsonProperty("pack")]
        public string ImagePackName;

        /// <summary>
        /// Name of the image this part is taken.
        /// </summary>
        [JsonProperty("image")]
        public string ImageName;

        /// <summary>
        /// Part of the image taken from the source.
        /// </summary>
        [JsonProperty("rectSource")]
        public Rectangle RectSource;

        /// <summary>
        /// Area in the final image this image part is drawn.
        /// </summary>
        [JsonProperty("rectDest")]
        public Rectangle RectDestination;

        /// <summary>
        /// Using points of PolygonDestination
        /// </summary>
        [JsonProperty("ellipseDest")]
        public Rectangle EllipseDestination;

        /// <summary>
        /// Polygon in the final image where the image part is drawn.
        /// If this exists, RectDestination is ignored.
        /// </summary>
        [JsonProperty("polygonDest")]
        public PointF[] PolygonDestination;

        /// <summary>
        /// Path of image part in result image.
        /// </summary>
        public GraphicsPath PathDestination;

        [JsonProperty("borderColor")]
        public Color BorderColor;

        [JsonProperty("borderWidth")]
        public int BorderWidth;

        [JsonProperty("backgroundColor")]
        public Color BackgroundColor;

        /// <summary>
        /// If true, the part of the image is deleted before the part is drawn.
        /// </summary>
        [JsonProperty("clear")]
        public bool Clear;

        [OnDeserialized]
        void Initializing(StreamingContext _)
        {
            PathDestination = new GraphicsPath();
            if (PolygonDestination?.Any() == true)
                PathDestination.AddPolygon(PolygonDestination);
            else if (!EllipseDestination.IsEmpty)
                PathDestination.AddEllipse(EllipseDestination);
            PathDestination.AddRectangle(RectDestination);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + (ImagePackName?.GetHashCode() ?? 0);
                hash = hash * 23 + (ImageName?.GetHashCode() ?? 0);
                hash = hash * 23 + RectSource.GetHashCode();
                hash = hash * 23 + PathDestination.GetHashCode();
                hash = hash * 23 + BorderColor.GetHashCode();
                hash = hash * 23 + BorderWidth;

                return hash;
            }
        }
    }
}
