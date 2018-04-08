using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// this class is used to enable double-buffering for listview-controls to prevent the flickering.
public static class ControlExtensions
{
    public static void DoubleBuffered(this Control control, bool enable)
    {
        var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
        doubleBufferPropertyInfo.SetValue(control, enable, null);
    }

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

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

    public static void SetBackColorAndAccordingForeColor(this Control control, System.Drawing.Color backColor)
    {
        control.BackColor = backColor;
        control.ForeColor = ARKBreedingStats.Utils.ForeColor(control.BackColor);
    }
}