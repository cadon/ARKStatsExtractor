using System;
using System.Drawing;
using System.Windows.Forms;
using ARKBreedingStats.ocr.PatternMatching;

namespace ARKBreedingStats.ocr
{
    class OCRLetterEdit : PictureBox
    {
        private readonly uint[] letterArray = new uint[32];
        private readonly uint[] letterArrayComparing = new uint[32];
        private Pattern _patternDisplay;
        private Pattern _patternRecognized;
        public bool drawingEnabled = false;
        private bool overlay;
        private int pxSize = 5;
        private int offset;

        public OCRLetterEdit()
        {
            BorderStyle = BorderStyle.FixedSingle;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int testOffset = offset > 0 ? offset : 0;
            int templateOffset = offset < 0 ? -offset : 0;
            base.OnPaint(e);
            e.Graphics.FillRectangle(Brushes.Black, 0, 0, 100, 100);

            if (_patternDisplay == null) return;

            for (int y = 0; y < _patternDisplay.Height; y++)
            {
                for (int x = 0; x < _patternDisplay.Width; x++)
                {
                    var xTemplate = x + templateOffset;

                    var bitTemplate = xTemplate >= 0 && xTemplate < _patternDisplay.Width && _patternDisplay[x, y];

                    if (!overlay)
                    {
                        if (bitTemplate)
                            e.Graphics.FillRectangle(Brushes.LightCoral, x * pxSize, y * pxSize, pxSize, pxSize);
                    }
                    else
                    {
                        var xRecognized = x + testOffset;
                        var bitRecognized = xRecognized >= 0 && xRecognized < _patternRecognized.Width && y < _patternRecognized.Height && _patternRecognized[x, y];

                        if (bitTemplate && bitRecognized)
                            e.Graphics.FillRectangle(Brushes.White, x * pxSize, y * pxSize, pxSize, pxSize);
                        else if (bitTemplate)
                            e.Graphics.FillRectangle(Brushes.LightGreen, x * pxSize, y * pxSize, pxSize, pxSize);
                        else if (bitRecognized)
                            e.Graphics.FillRectangle(Brushes.DarkRed, x * pxSize, y * pxSize, pxSize, pxSize);
                    }
                }
            }

            return;

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

        /// <summary>
        /// Displayed pattern.
        /// </summary>
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

        /// <summary>
        /// Displayed pattern.
        /// </summary>
        public Pattern PatternDisplay
        {
            set
            {
                if (value != null)
                {
                    _patternDisplay = value.Clone();

                    int size = (int)Math.Max(value.Height, value.Width);
                    if (size > 0)
                        pxSize = Math.Max(2, 100 / size);
                    else pxSize = 3;
                    Invalidate();
                }
            }
            get
            {
                return _patternDisplay;

                //int l = 0;
                //for (int y = 0; y < 32; y++)
                //    if (letterArray[y] > 0)
                //        l = y;
                //l++;
                //uint[] lArray = new uint[l];
                //for (int y = 0; y < l; y++)
                //    lArray[y] = letterArray[y];
                //return lArray;
            }
        }

        public Pattern PatternComparing
        {
            set => _patternRecognized = value.Clone();
        }

        /// <summary>
        /// The differences to this pattern will be highlighted.
        /// </summary>
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

        public int recognizedOffset
        {
            set
            {
                offset = value;
                Invalidate();
            }
        }

        internal void Clear()
        {
            LetterArrayComparing = new uint[0];
            LetterArray = new uint[0];
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (!drawingEnabled || _patternDisplay == null || pxSize == 0) return;

            Point p = e.Location;
            int x = p.X / pxSize + (offset < 0 ? offset : 0);
            // if toggled pixel is outside the borders due to a shifted display, ignore click
            if (x < 0) return;

            int y = p.Y / pxSize;
            // toggle pixel
            _patternDisplay[x, y] = !_patternDisplay[x, y];
            Invalidate();

            //int y = p.Y / pxSize + 1; // first row is array-length
            //while (letterArray.Length < y) letterArray[letterArray.Length] = 0;
            //// toggle pixel
            //letterArray[y] ^= (uint)(1 << x);
            //Invalidate();
        }
    }
}
