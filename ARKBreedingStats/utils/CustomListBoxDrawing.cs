using System.Windows.Forms;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace ARKBreedingStats.utils
{
    internal static class CustomListBoxDrawing
    {
        /// <summary>
        /// Adds support for drawing separator lines if the item.ToString() == "---".
        /// </summary>
        /// <param name="lb"></param>
        public static void SupportSeparatorLines(this ListBox lb)
        {
            lb.DrawMode = DrawMode.OwnerDrawVariable;
            lb.MeasureItem += Lb_MeasureItem;
            lb.DrawItem += Lb_DrawItem;
        }

        public const string SeparatorString = "---";

        private static void Lb_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0 || !(sender is ListBox lb)) return;
            if (lb.Items[e.Index].ToString() == SeparatorString)
                e.ItemHeight = 5;
            else
                e.ItemHeight = TextRenderer.MeasureText(lb.Items[e.Index].ToString(), lb.Font).Height;
        }

        private static void Lb_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || !(sender is ListBox lb)) return;

            e.DrawBackground();
            var itemText = lb.Items[e.Index].ToString();
            Brush textBrush = new SolidBrush(e.ForeColor);

            if (itemText == SeparatorString)
            {
                // Draw separator line
                using (var pen = new Pen(Color.Gray, 1))
                {
                    var midY = e.Bounds.Top + (e.Bounds.Height / 2);
                    e.Graphics.DrawLine(pen, e.Bounds.Left + 5, midY, e.Bounds.Right - 5, midY);
                }
            }
            else
            {
                // Draw normal text
                e.Graphics.DrawString(itemText, e.Font, textBrush, e.Bounds.Left, e.Bounds.Top);
            }

            e.DrawFocusRectangle();
        }
    }
}
