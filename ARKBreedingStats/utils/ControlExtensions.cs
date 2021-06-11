using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ARKBreedingStats.utils
{
    public static class ControlExtensions
    {
        /// <summary>
        /// Prevents flickering of ListViews.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="enable"></param>
        public static void DoubleBuffered(this Control control, bool enable)
        {
            var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo?.SetValue(control, enable, null);
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(this Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(this Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
            parent.Refresh();
        }

        /// <summary>
        /// Sets the passed color as BackColor for the Control, then sets either black or white as the ForeColor, depending on the lightness of the backColor.
        /// </summary>
        public static void SetBackColorAndAccordingForeColor(this Control control, System.Drawing.Color backColor)
        {
            control.BackColor = backColor;
            control.ForeColor = Utils.ForeColor(backColor);
        }
    }
}