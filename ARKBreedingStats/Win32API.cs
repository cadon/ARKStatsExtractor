using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace ARKBreedingStats
{
    public static class Win32API
    {
        public const int LVM_FIRST = 0x1000;
        public const int LVM_GETHEADER = LVM_FIRST + 31;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, out Point lpPoint);

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);

        public const int SW_RESTORE = 9;

        [DllImport("user32.dll")]
        public static extern IntPtr ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public int Width => right - left;
            public int Height => bottom - top;
        }

        public struct Point
        {
            public int x;
            public int y;
        }

        public static Bitmap GetScreenshotOfProcess(string processName, int waitMs, bool hideOverlay = false)
        {
            Process[] p = Process.GetProcessesByName(processName);
            if (!p.Any()) return null;

            Bitmap grab = null;

            //Shadow Box users have multiple processes named "Shadow" but only one has an actual window.
            //Iterate through processes until window screen grab is NOT null.
            foreach (Process i in p)
            {
                IntPtr proc = i.MainWindowHandle;
                //return PrintWindow(proc); <-- doesn't work, althought it should

                SetForegroundWindow(proc);
                ShowWindow(proc, SW_RESTORE);

                // You need some amount of delay, but 1 second may be overkill
                Thread.Sleep(waitMs);

                grab = GrabCurrentScreen(proc, hideOverlay);

                if (grab != null)
                    break;
            }
            return grab;
        }

        public static Rect GetWindowRect(string processName)
        {
            Process[] p = Process.GetProcessesByName(processName);
            Rect r;
            r.left = 0; r.right = 0; r.top = 0; r.bottom = 0;

            if (p.Length == 0)
                return r;

            IntPtr proc = p[0].MainWindowHandle;


            GetClientRect(proc, out r);

            return r;
        }

        public static Bitmap GrabCurrentScreen(IntPtr hwnd, bool hideOverlay = false)
        {
            Rect rc;
            Rect client;
            Point p;

            p.x = 0; p.y = 0;

            GetWindowRect(hwnd, out rc);
            GetClientRect(hwnd, out client);
            ClientToScreen(hwnd, out p);


            if (rc.Width == 0 || rc.Height == 0)
                return null;

            rc.left = p.x;
            rc.top = p.y;
            rc.right = rc.left + client.Width;
            rc.bottom = rc.top + client.Height;

            if (rc.Width == 0 || rc.Height == 0)
                return null;

            Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);

            // hide overlay to not capture it
            bool showOverlay = false;
            if (hideOverlay && ARKOverlay.theOverlay != null)
            {
                showOverlay = ARKOverlay.theOverlay.Visible;
                ARKOverlay.theOverlay.Visible = false;
            }

            using (Graphics gfxBmp = Graphics.FromImage(bmp))
            {
                gfxBmp.CopyFromScreen(rc.left, rc.top, 0, 0, new Size(rc.Width, rc.Height), CopyPixelOperation.SourceCopy);
                //gfxBmp.CopyFromScreen(client.left, client.top, 0, 0, new Size(client.Width, client.Height), CopyPixelOperation.SourceCopy);
            }

            // show overlay again if it was visible before and hidden for the screencapture
            if (hideOverlay && showOverlay)
                ARKOverlay.theOverlay.Visible = showOverlay;

            return bmp;
        }

        public static Bitmap PrintWindow(IntPtr hwnd)
        {
            Rect rc;
            GetWindowRect(hwnd, out rc);

            Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);

            IntPtr hdcBitmap = gfxBmp.GetHdc();
            //PrintWindow(hwnd, hdcBitmap, 0);
            gfxBmp.ReleaseHdc(hdcBitmap);

            gfxBmp.CopyFromScreen(rc.left, rc.top, 0, 0, new Size(rc.Width, rc.Height), CopyPixelOperation.SourceCopy);

            gfxBmp.Dispose();

            return bmp;
        }

        /// <summary>
        /// Returns true if the mouse is on the header of a listView regarding the y-axis.
        /// </summary>
        /// <param name="listView"></param>
        /// <returns></returns>
        public static bool IsMouseOnListViewHeader(IntPtr listViewHandle, int mouseY)
        {
            IntPtr headerHandle = Win32API.SendMessage(listViewHandle, Win32API.LVM_GETHEADER, 0, 0);
            return Win32API.GetWindowRect(headerHandle, out Rect rc)
                && (mouseY >= rc.top) && (mouseY < rc.bottom);
        }
    }
}
