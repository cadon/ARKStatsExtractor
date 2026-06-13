using System;
using System.Drawing;
using System.Windows.Forms;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.uiControls
{
    /// <summary>
    /// GroupBox control that draws custom border using SystemColor to adapt to a light or dark theme.
    /// </summary>
    public class GroupBoxC : GroupBox
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var textSize = TextRenderer.MeasureText(Text, Font);

            var rect = ClientRectangle;
            var top = (int)Math.Round(textSize.Height / 2f);
            var bottom = rect.Height - 2;
            var right = rect.Width - 1;

            using var pen = new Pen(UiColors.Current?.ControlBorder ?? SystemColors.ControlDark);
            e.Graphics.DrawLine(pen, 0, top, 6, top);
            e.Graphics.DrawLine(pen, textSize.Width + 4, top, right, top);

            e.Graphics.DrawLine(pen, 0, top, 0, bottom);
            e.Graphics.DrawLine(pen, right, top, right, bottom);
            e.Graphics.DrawLine(pen, 0, bottom, right, bottom);
        }
    }
}
