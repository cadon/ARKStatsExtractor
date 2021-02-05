using System;
using System.Drawing;
using System.Windows.Forms;
using ARKBreedingStats.ocr.PatternMatching;

namespace ARKBreedingStats.ocr
{
    class OCRLetterEdit : PictureBox
    {
        private readonly uint[] _letterArray = new uint[32];
        private readonly uint[] _letterArrayComparing = new uint[32];
        private Pattern _patternDisplay;
        private Pattern _patternRecognized;
        public bool drawingEnabled = false;
        private bool _overlay;
        private int _pxSize = 5;
        private int _offset;
        public event Action PatternChanged;

        public OCRLetterEdit()
        {
            BorderStyle = BorderStyle.FixedSingle;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int testOffset = _offset > 0 ? _offset : 0;
            int templateOffset = _offset < 0 ? -_offset : 0;
            base.OnPaint(e);
            e.Graphics.FillRectangle(Brushes.Black, 0, 0, 100, 100);

            if (_patternDisplay == null) return;

            for (int y = 0; y < _patternDisplay.Height; y++)
            {
                for (int x = 0; x < _patternDisplay.Width; x++)
                {
                    var xTemplate = x + templateOffset;

                    var bitTemplate = xTemplate >= 0 && xTemplate < _patternDisplay.Width && _patternDisplay[x, y];

                    if (!_overlay)
                    {
                        if (bitTemplate)
                            e.Graphics.FillRectangle(Brushes.LightCoral, x * _pxSize, y * _pxSize, _pxSize, _pxSize);
                    }
                    else
                    {
                        var xRecognized = x + testOffset;
                        var bitRecognized = xRecognized >= 0 && xRecognized < _patternRecognized.Width && y < _patternRecognized.Height && _patternRecognized[x, y];

                        if (bitTemplate && bitRecognized)
                            e.Graphics.FillRectangle(Brushes.White, x * _pxSize, y * _pxSize, _pxSize, _pxSize);
                        else if (bitTemplate)
                            e.Graphics.FillRectangle(Brushes.LightGreen, x * _pxSize, y * _pxSize, _pxSize, _pxSize);
                        else if (bitRecognized)
                            e.Graphics.FillRectangle(Brushes.DarkRed, x * _pxSize, y * _pxSize, _pxSize, _pxSize);
                    }
                }
            }
            
            //for (int y = 0; y < _letterArray.Length - 1; y++)
            //{
            //    uint row = _letterArray[y + 1] << templateOffset;
            //    uint rowC = _letterArrayComparing[y + 1] << testOffset;
            //    int x = 0;
            //    while (row > 0 || rowC > 0)
            //    {
            //        if (!_overlay)
            //        {
            //            if ((row & 1) == 1)
            //                e.Graphics.FillRectangle(Brushes.LightCoral, x * _pxSize, y * _pxSize, _pxSize, _pxSize);
            //        }
            //        else
            //        {
            //            if ((row & 1) == 1 && (rowC & 1) == 1)
            //                e.Graphics.FillRectangle(Brushes.White, x * _pxSize, y * _pxSize, _pxSize, _pxSize);
            //            else if ((row & 1) == 1 && (rowC & 1) == 0)
            //                e.Graphics.FillRectangle(Brushes.LightGreen, x * _pxSize, y * _pxSize, _pxSize, _pxSize);
            //            else if ((row & 1) == 0 && (rowC & 1) == 1)
            //                e.Graphics.FillRectangle(Brushes.DarkRed, x * _pxSize, y * _pxSize, _pxSize, _pxSize);
            //        }
            //        row = row >> 1;
            //        rowC = rowC >> 1;
            //        x++;
            //    }
            //}
        }

        ///// <summary>
        ///// Displayed pattern.
        ///// </summary>
        //public uint[] LetterArray
        //{
        //    set
        //    {
        //        if (value != null)
        //        {
        //            for (int y = 0; y < 32; y++)
        //            {
        //                if (y < value.Length)
        //                    _letterArray[y] = value[y];
        //                else _letterArray[y] = 0;
        //            }
        //            int size = (int)Math.Max(_letterArray[0], value.Length);
        //            if (size > 0)
        //                _pxSize = 100 / size;
        //            else _pxSize = 3;
        //            Invalidate();
        //        }
        //    }
        //    get
        //    {
        //        int l = 0;
        //        for (int y = 0; y < 32; y++)
        //            if (_letterArray[y] > 0)
        //                l = y;
        //        l++;
        //        uint[] lArray = new uint[l];
        //        for (int y = 0; y < l; y++)
        //            lArray[y] = _letterArray[y];
        //        return lArray;
        //    }
        //}

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
                        _pxSize = Math.Max(2, 100 / size);
                    else _pxSize = 3;
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

        /// <summary>
        /// The differences to this pattern will be highlighted.
        /// </summary>
        public Pattern PatternComparing
        {
            set => _patternRecognized = value.Clone();
        }

        public int RecognizedOffset
        {
            set
            {
                _offset = value;
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (!drawingEnabled || _patternDisplay == null || _pxSize == 0) return;

            Point p = e.Location;
            int x = p.X / _pxSize + (_offset < 0 ? _offset : 0);
            int y = p.Y / _pxSize;
            // if toggled pixel is outside the borders due to a shifted display, ignore click
            if (x < 0 || x >= _patternDisplay.Width || y >= _patternDisplay.Height) return;

            // toggle pixel
            _patternDisplay[x, y] = !_patternDisplay[x, y];
            Invalidate();
            PatternChanged?.Invoke();

            //int y = p.Y / pxSize + 1; // first row is array-length
            //while (letterArray.Length < y) letterArray[letterArray.Length] = 0;
            //// toggle pixel
            //letterArray[y] ^= (uint)(1 << x);
            //Invalidate();
        }
    }
}
