using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats.ocr
{
    class OCRLetterEdit : PictureBox
    {
        private uint[] letterArray;
        private uint[] letterArrayComparing;
        public bool drawingEnabled;
        private bool overlay;
        private int pxSize;
        private int offset;

        public OCRLetterEdit()
        {
            letterArray = new uint[32];
            letterArrayComparing = new uint[32];
            drawingEnabled = false;
            BorderStyle = BorderStyle.FixedSingle;
            overlay = false;
            pxSize = 5;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int testOffset = offset > 0 ? offset : 0;
            int templateOffset = offset < 0 ? -offset : 0;
            base.OnPaint(e);
            e.Graphics.FillRectangle(Brushes.Black, 0, 0, 100, 100);
            for (int y = 0; y < letterArray.Length - 1; y++)
            {
                uint row = letterArray[y + 1] << templateOffset;
                uint rowC = letterArrayComparing[y + 1] << testOffset;
                int x = 0;
                while (row > 0 || rowC > 0)
                {
                    if (!overlay)
                    {
                        if ((row & 1) == 1)
                            e.Graphics.FillRectangle(Brushes.LightCoral, x * pxSize, y * pxSize, pxSize, pxSize);
                    }
                    else
                    {
                        if ((row & 1) == 1 && (rowC & 1) == 1)
                            e.Graphics.FillRectangle(Brushes.White, x * pxSize, y * pxSize, pxSize, pxSize);
                        else if ((row & 1) == 1 && (rowC & 1) == 0)
                            e.Graphics.FillRectangle(Brushes.LightGreen, x * pxSize, y * pxSize, pxSize, pxSize);
                        else if ((row & 1) == 0 && (rowC & 1) == 1)
                            e.Graphics.FillRectangle(Brushes.DarkRed, x * pxSize, y * pxSize, pxSize, pxSize);
                    }
                    row = row >> 1;
                    rowC = rowC >> 1;
                    x++;
                }
            }
        }

        public uint[] LetterArray
        {
            set
            {
                if (value != null)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        if (y < value.Length)
                            letterArray[y] = value[y];
                        else letterArray[y] = 0;
                    }
                    int size = (int)Math.Max(letterArray[0], value.Length);
                    if (size > 0)
                        pxSize = 100 / size;
                    else pxSize = 3;
                    Invalidate();
                }
            }
            get
            {
                int l = 0;
                for (int y = 0; y < 32; y++)
                    if (letterArray[y] > 0)
                        l = y;
                l++;
                uint[] lArray = new uint[l];
                for (int y = 0; y < l; y++)
                    lArray[y] = letterArray[y];
                return lArray;
            }
        }

        public uint[] LetterArrayComparing
        {
            set
            {
                if (value != null)
                {
                    overlay = false;
                    for (int y = 0; y < 32; y++)
                    {
                        if (y < value.Length)
                        {
                            letterArrayComparing[y] = value[y];
                            if (value[y] > 0)
                                overlay = true;
                        }
                        else letterArrayComparing[y] = 0;
                    }
                    Invalidate();
                }
            }
        }

        public int recognizedOffset { set { offset = value; Invalidate(); } }

        internal void Clear()
        {
            LetterArrayComparing = new uint[0];
            LetterArray = new uint[0];
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (drawingEnabled)
            {
                Point p = e.Location;
                int x = p.X / pxSize + (offset < 0 ? offset : 0);
                // if toggled pixel is outside the borders due to a shifted display, ignore click
                if (x >= 0)
                {
                    int y = p.Y / pxSize + 1; // first row is array-length
                    while (letterArray.Length < y) letterArray[letterArray.Length] = 0;
                    // toggle pixel
                    letterArray[y] ^= (uint)(1 << x);
                    Invalidate();
                }
            }
        }
    }
}
