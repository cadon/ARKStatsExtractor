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
        [JsonProperty("pack")] public string ImagePackName;

        /// <summary>
        /// Name of the image this part is taken.
        /// </summary>
        [JsonProperty("image")] public string ImageName;

        /// <summary>
        /// Part of the image taken from the source, absolute coordinates.
        /// </summary>
        [JsonProperty("sourceRectangle")] public float[] SourceRectangle;

        /// <summary>
        /// Area in the final image this image part is drawn, relative to the final image dimensions.
        /// </summary>
        [JsonProperty("destRectangle")] public float[] DestinationRectangle;

        /// <summary>
        /// Area in the final image this image part is drawn using an ellipse, relative to the final image dimensions.
        /// </summary>
        [JsonProperty("destEllipse")] public float[] DestinationEllipse;

        /// <summary>
        /// Polygon in the final image where the image part is drawn, relative to the final image dimensions.
        /// If this exists, RectDestination is ignored.
        /// </summary>
        [JsonProperty("destPolygon")] public PointF[] DestinationPolygon;

        /// <summary>
        /// Path of image part in result image.
        /// </summary>
        public GraphicsPath DestinationPath;

        [JsonProperty("borderColor")] public Color BorderColor;

        [JsonProperty("borderWidth")] public int BorderWidth;

        [JsonProperty("backgroundColor")] public Color BackgroundColor;

        /// <summary>
        /// Rectangle of ellipse shadow.
        /// </summary>
        [JsonProperty("shadowRectangle")] public float[] ShadowRectangle;

        [JsonProperty("shadowAngle")] public float ShadowAngle;

        [JsonProperty("shadowColor")] public Color ShadowColor = Color.Black;

        [JsonProperty("shadowIntensity")] public float ShadowIntensity = 1;

        /// <summary>
        /// If true, the part of the image is deleted before the part is drawn.
        /// </summary>
        [JsonProperty("clear")] public bool Clear;

        /// <summary>
        /// Set DestinationPath depending on the rectangleReference, assuming relative values in the dest variables.
        /// </summary>
        public void InitializeDestinationPath(RectangleF rectangleReference)
        {
            DestinationPath = new GraphicsPath();
            if (PolygonRelativeToAbsolute(DestinationPolygon, rectangleReference, out var p))
                DestinationPath.AddPolygon(p);
            else if (ArrayToRectangleF(DestinationEllipse, rectangleReference, out var r))
                DestinationPath.AddEllipse(r);
            else if (ArrayToRectangleF(DestinationRectangle, rectangleReference, out r))
                DestinationPath.AddRectangle(r);
            if (DestinationPath.GetBounds().IsEmpty)
                DestinationPath.AddRectangle(rectangleReference);
        }

        /// <summary>
        /// Converts a relative polygon to absolute coordinates, assuming relative values using rectangleBase.
        /// </summary>
        public static bool PolygonRelativeToAbsolute(PointF[] polygonRelative, RectangleF rectangleBase, out PointF[] p, float offsetX = 0, float offsetY = 0)
        {
            if (polygonRelative?.Any() != true)
            {
                p = null;
                return false;
            }

            p = polygonRelative.Select(v => new PointF(
                rectangleBase.X + rectangleBase.Width * v.X + offsetX,
                rectangleBase.Y + rectangleBase.Height * v.Y + offsetY
                )).ToArray();

            return true;
        }

        /// <summary>
        /// Converts a float[4] in a RectangleF, assuming absolute values.
        /// </summary>
        public static bool ArrayToRectangleF(float[] a, out RectangleF r)
        {
            if (a?.Length != 4)
            {
                r = RectangleF.Empty;
                return false;
            }

            r = new RectangleF(a[0], a[1], a[2], a[3]);
            return true;
        }

        /// <summary>
        /// Converts a float[4] in a RectangleF, assuming relative values using rectangleBase.
        /// </summary>
        public static bool ArrayToRectangleF(float[] a, RectangleF rectangleBase, out RectangleF r) =>
            ArrayToRectangleF(a, out r)
            && GetAbsoluteRectangle(r, rectangleBase, out r);

        /// <summary>
        /// Calculates absoluteRectangle depending on a relativeRectangle in a baseRectangle.
        /// </summary>
        public static bool GetAbsoluteRectangle(RectangleF relativeRectangle, RectangleF baseRectangle, out RectangleF absoluteRectangle)
        {
            if (relativeRectangle.IsEmpty || baseRectangle.IsEmpty)
            {
                absoluteRectangle = RectangleF.Empty;
                return false;
            }

            absoluteRectangle = new RectangleF(
                baseRectangle.X + relativeRectangle.X * baseRectangle.Width,
                baseRectangle.Y + relativeRectangle.Y * baseRectangle.Height,
                relativeRectangle.Width * baseRectangle.Width,
                relativeRectangle.Height * baseRectangle.Height
                );

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + (ImagePackName?.GetHashCode() ?? 0);
                hash = hash * 23 + (ImageName?.GetHashCode() ?? 0);
                hash = hash * 23 + SourceRectangle?.GetHashCode() ?? 0;
                hash = hash * 23 + DestinationRectangle?.GetHashCode() ?? 0;
                hash = hash * 23 + DestinationEllipse?.GetHashCode() ?? 0;
                hash = hash * 23 + DestinationPolygon?.GetHashCode() ?? 0;
                hash = hash * 23 + ShadowRectangle?.GetHashCode() ?? 0;
                hash = hash * 23 + ShadowColor.GetHashCode();
                hash = hash * 23 + ShadowAngle.GetHashCode();
                hash = hash * 23 + ShadowIntensity.GetHashCode();
                hash = hash * 23 + BorderColor.GetHashCode();
                hash = hash * 23 + BorderWidth;

                return hash;
            }
        }
    }
}
