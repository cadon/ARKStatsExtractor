using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    class Win32Stuff
    {
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


        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public int Width
            {
                get { return right - left; }
            }
            public int Height
            {
                get { return bottom - top; }
            }
        }

        public struct Point
        {
            public int x;
            public int y;
        }

        public static Bitmap GetSreenshotOfProcess(String processName)
        {
            Process[] p = Process.GetProcessesByName(processName);

            if (p.Length == 0)
                return null;

            IntPtr proc = p[0].MainWindowHandle;
            //return PrintWindow(proc); <-- doesn't work, althought it should


            SetForegroundWindow(proc);
            ShowWindow(proc, SW_RESTORE);

            // You need some amount of delay, but 1 second may be overkill
            Thread.Sleep(500);

            Bitmap grab = GrabCurrentScreen(proc);

            return grab;
        }

        public static Rect GetWindowRect(String processName)
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

        public static Bitmap GrabCurrentScreen(IntPtr hwnd)
        {
            Rect rc;
            Rect client;
            Point p;

            p.x = 0; p.y = 0;

            GetWindowRect(hwnd, out rc);
            GetClientRect(hwnd, out client);
            ClientToScreen(hwnd, out p);


            rc.left = p.x;
            rc.top = p.y;
            rc.right = rc.left + client.Width;
            rc.bottom = rc.top + client.Height;

            Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);


            gfxBmp.CopyFromScreen(rc.left, rc.top, 0, 0, new Size(rc.Width, rc.Height), CopyPixelOperation.SourceCopy);
            //gfxBmp.CopyFromScreen(client.left, client.top, 0, 0, new Size(client.Width, client.Height), CopyPixelOperation.SourceCopy);

            gfxBmp.Dispose();

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
    }
}
