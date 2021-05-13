using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.NamePatterns
{
    /// <summary>
    /// A keyword or function for using in NamePatterns.
    /// </summary>
    internal class NamePatternEntry : Panel
    {
        internal string FilterString;

        internal NamePatternEntry()
        {
            //BorderStyle = BorderStyle.FixedSingle;
            Height = 30;
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        }
    }
}
