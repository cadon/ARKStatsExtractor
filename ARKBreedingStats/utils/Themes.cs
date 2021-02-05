using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.utils
{
    /// <summary>
    /// Used for color themes.
    /// </summary>
    internal static class Themes
    {
        private static Color _foreColor;
        private static Color _backColor;

        internal static void SetColors(this Control control, Color backColor, Color foreColor)
        {
            _foreColor = foreColor;
            _backColor = backColor;
            control.SetControlColors(backColor, foreColor);
        }

        private static void SetControlColors(this Control control, Color backColor, Color foreColor)
        {
            control.BackColor = backColor;
            control.ForeColor = foreColor;

            foreach (Control c in control.Controls)
                c.SetControlColors(backColor, foreColor);
        }

        /// <summary>
        /// Set DrawMode of all tabControls to OwnerDrawFixed to draw them manually
        /// </summary>
        /// <param name="control"></param>
        internal static void InitializeTabControls(this Control control)
        {
            if (control is TabControl tc)
            {
                tc.DrawMode = TabDrawMode.OwnerDrawFixed;
                tc.DrawItem += TabControl_DrawItem;
            }

            foreach (Control c in control.Controls)
                c.InitializeTabControls();
        }

        private static void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tc = (TabControl)sender;
            using (Brush bb = new SolidBrush(_backColor))
            using (Brush fb = new SolidBrush(_foreColor))
            {
                e.Graphics.FillRectangle(bb, e.Bounds);
                SizeF sz = e.Graphics.MeasureString(tc.TabPages[e.Index].Text, e.Font);
                e.Graphics.DrawString(tc.TabPages[e.Index].Text, e.Font, fb, e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1);

                Rectangle rect = e.Bounds;
                rect.Offset(0, 1);
                rect.Inflate(0, -1);
                e.Graphics.DrawRectangle(Pens.DarkGray, rect);
                e.DrawFocusRectangle();
            }
        }
    }
}
