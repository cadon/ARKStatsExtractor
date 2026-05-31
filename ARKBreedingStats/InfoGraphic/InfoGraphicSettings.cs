using System.Drawing;

namespace ARKBreedingStats.InfoGraphic
{
    public class InfoGraphicSettings
    {
        /// <summary>
        /// Height of the infographic. Other size properties are relative to that.
        /// </summary>
        public int infoGraphicHeight;
        public string fontName;
        public Color foreColor;
        public Color backColor;
        public Color borderColor;
        public int borderWidth;
        public float borderRadius;
        /// <summary>
        /// Padding in the order top right bottom left.
        /// </summary>
        public int[] Padding;
        public Color colorOutlineText;
        public float widthOutlineText;
        public bool displayCreatureName;
        public bool displayWithDomLevels;
        public bool displaySumWildMutLevels;
        public bool displayMutations;
        public bool displayGenerations;
        public bool displayStatValues;
        public bool displayMaxWildLevel;
        public bool displayExtraRegionNames;
        public bool displayRegionNamesIfNoImage;
        public Color colorOutlineCreature;
        public string backgroundImagePath = null;
        public BackgroundImageResizings BackgroundImageResizing = BackgroundImageResizings.Cover;
        public int widthOutlineCreature = 0;
        public float creatureOutlineBlurring = 1;
        public float creatureScaling = 1;
        public bool ColorBasedOnCreature;
        public bool TintBackgroundImage;

        public enum BackgroundImageResizings
        {
            /// <summary>
            /// Centered
            /// </summary>
            Original,
            Stretch,
            /// <summary>
            /// Scaling the image so it is not cropped.
            /// </summary>
            Contain,
            /// <summary>
            /// Scaling the image to cover the background and possibly crop the image.
            /// </summary>
            Cover
        }
    }
}
