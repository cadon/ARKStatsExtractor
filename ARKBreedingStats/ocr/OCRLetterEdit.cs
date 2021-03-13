using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.ocr.PatternMatching;

namespace ARKBreedingStats.ocr
{
    internal class OCRLetterEdit : PictureBox
    {
        private Pattern _patternDisplay;
        private Pattern _patternRecognized;
        public bool drawingEnabled = false;
        private bool _overlay;
        private int _pxSize = 5;
        private int _offset;
        public event Action PatternChanged;
        private const int ImageWidth = 100;
        private const int ImageHeight = 100;

        public OCRLetterEdit()
        {
            BorderStyle = BorderStyle.FixedSingle;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //int testOffset = _offset > 0 ? _offset : 0;
            //int templateOffset = _offset < 0 ? -_offset : 0;
            e.Graphics.FillRectangle(Brushes.Black, 0, 0, ImageWidth, ImageHeight);

            // draw grid
            using (var gridPen = (Pen)Pens.Gray.Clone())
            using (var gridPenBright = (Pen)Pens.DarkGray.Clone())
            {
                int gi = 0;
                for (var x = 0; x < ImageWidth; x += _pxSize)
                    e.Graphics.DrawLine(gi++ % 5 == 0 ? gridPenBright : gridPen, x, 0, x, ImageHeight);
                gi = 0;
                for (var y = 0; y < ImageHeight; y += _pxSize)
                    e.Graphics.DrawLine(gi++ % 5 == 0 ? gridPenBright : gridPen, 0, y, ImageWidth, y);
            }

            if (_patternDisplay == null) return;


            for (int y = 0; y < _patternDisplay.Height; y++)
            {
                var yTemplate = y + _patternDisplay.YOffset;
                for (int x = 0; x < _patternDisplay.Width; x++)
                {
                    //var xTemplate = x + templateOffset;
                    //var bitTemplate = xTemplate >= 0 && xTemplate < _patternDisplay.Width && _patternDisplay[xTemplate, yTemplate];
                    var bitTemplate = _patternDisplay[x, yTemplate];

                    if (!_overlay)
                    {
                        if (bitTemplate)
                            e.Graphics.FillRectangle(Brushes.LightCoral, x * _pxSize, yTemplate * _pxSize, _pxSize, _pxSize);
                    }
                    else
                    {
                        //var xRecognized = x + testOffset;
                        //var bitRecognized = xRecognized >= 0 && xRecognized < _patternRecognized.Width && y < _patternRecognized.Height && _patternRecognized[x, y];
                        var bitRecognized = _patternRecognized[x, y];

                        if (bitTemplate && bitRecognized)
                            e.Graphics.FillRectangle(Brushes.White, x * _pxSize, yTemplate * _pxSize, _pxSize, _pxSize);
                        else if (bitTemplate)
                            e.Graphics.FillRectangle(Brushes.LightGreen, x * _pxSize, yTemplate * _pxSize, _pxSize, _pxSize);
                        else if (bitRecognized)
                            e.Graphics.FillRectangle(Brushes.DarkRed, x * _pxSize, yTemplate * _pxSize, _pxSize, _pxSize);
                    }
                }
            }

            // draw aperturePixels for debugging
            //using (var aperturePxBrush = new SolidBrush(Color.FromArgb(180, Color.LightBlue)))
            //{
            //    if (_patternDisplay.AperturePixels?.Any() ?? false)
            //        e.Graphics.FillRectangles(aperturePxBrush, _patternDisplay.AperturePixels.Select(c => new Rectangle(c.x * _pxSize + _pxSize / 4, c.y * _pxSize + _pxSize / 4, _pxSize / 2, _pxSize / 2)).ToArray());
            //}

            // draw apertures
            // ..4..
            // 0.5.1
            // .....
            // 2.6.3
            // ..7..
            using (var brush = new SolidBrush(Color.FromArgb(180, Color.LightGreen)))
            {
                var pieWidth = (int)(ImageHeight * 0.15);
                var pxHeight = _patternDisplay.HeightWithOffset * _pxSize;
                var pxWidth = _patternDisplay.Width * _pxSize;

                if ((_patternDisplay.Apertures & 1) == 1)
                    e.Graphics.FillPie(brush, -pieWidth / 2, pieWidth, pieWidth, pieWidth, 270, 180);
                if (((_patternDisplay.Apertures >> 1) & 1) == 1)
                    e.Graphics.FillPie(brush, pxWidth - pieWidth / 2, pieWidth, pieWidth, pieWidth, 90, 180);
                if (((_patternDisplay.Apertures >> 2) & 1) == 1)
                    e.Graphics.FillPie(brush, -pieWidth / 2, pxHeight - 2 * pieWidth, pieWidth, pieWidth, 270, 180);
                if (((_patternDisplay.Apertures >> 3) & 1) == 1)
                    e.Graphics.FillPie(brush, pxWidth - pieWidth / 2, pxHeight - 2 * pieWidth, pieWidth, pieWidth, 90, 180);
                if (((_patternDisplay.Apertures >> 4) & 1) == 1)
                    e.Graphics.FillPie(brush, pxWidth / 2 - pieWidth / 2, -pieWidth / 2, pieWidth, pieWidth, 180, 360);
                if (((_patternDisplay.Apertures >> 5) & 1) == 1)
                    e.Graphics.FillPie(brush, pxWidth / 2 - pieWidth / 2, pxHeight / 3 - pieWidth / 2, pieWidth, pieWidth, 0, 360);
                if (((_patternDisplay.Apertures >> 6) & 1) == 1)
                    e.Graphics.FillPie(brush, pxWidth / 2 - pieWidth / 2, 2 * pxHeight / 3 - pieWidth / 2, pieWidth, pieWidth, 0, 360);
                if (((_patternDisplay.Apertures >> 7) & 1) == 1)
                    e.Graphics.FillPie(brush, pxWidth / 2 - pieWidth / 2, pxHeight - pieWidth / 2, pieWidth, pieWidth, 180, 180);
            }
        }

        /// <summary>
        /// Displayed pattern.
        /// </summary>
        public Pattern PatternDisplay
        {
            set
            {
                if (value == null)
                {
                    _patternDisplay = null;
                    Invalidate();
                    return;
                }

                _patternDisplay = value.Clone();

                // apply yOffset
                var yOffset = _patternDisplay.YOffset;
                if (yOffset > 0)
                {
                    var width = _patternDisplay.Data.GetLength(0);
                    var heightWithoutYOffset = _patternDisplay.Data.GetLength(1);
                    var data = new bool[width, heightWithoutYOffset + yOffset];

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < heightWithoutYOffset; y++)
                        {
                            data[x, y + yOffset] = _patternDisplay.Data[x, y];
                        }
                    }

                    _patternDisplay.Data = data;
                    _patternDisplay.YOffset = 0;
                    _patternDisplay.UpdateProperties();
                }

                int size = Math.Max(value.HeightWithOffset, value.Width);
                if (size > 0)
                    _pxSize = Math.Max(2, 100 / size);
                else _pxSize = 3;
                Invalidate();
            }
            get
            {
                if (_patternDisplay == null) return null;
                // if array can be trimmed, recreate it
                bool recreateArray = false;
                // check for yOffset to reduce array size
                var width = _patternDisplay.Data.GetLength(0);
                var height = _patternDisplay.Data.GetLength(1);
                byte yOffset = 0;
                for (int y = 0; y < height; y++)
                {
                    bool rowContainsData = false;
                    for (int x = 0; x < width; x++)
                    {
                        if (_patternDisplay.Data[x, y])
                        {
                            rowContainsData = true;
                            break;
                        }
                    }

                    if (rowContainsData)
                        break;

                    yOffset++;
                    recreateArray = true;
                }

                // trim empty rows at the end
                for (int y = height - 1; y > yOffset; y--)
                {
                    bool rowContainsData = false;
                    for (int x = 0; x < width; x++)
                    {
                        if (_patternDisplay.Data[x, y])
                        {
                            rowContainsData = true;
                            break;
                        }
                    }

                    if (rowContainsData)
                        break;

                    height = y;
                    recreateArray = true;
                }

                // trim columns at the start
                int xOffset = 0;
                for (int x = 0; x < width; x++)
                {
                    bool columnContainsData = false;
                    for (int y = yOffset; y < height; y++)
                    {
                        if (_patternDisplay.Data[x, y])
                        {
                            columnContainsData = true;
                            break;
                        }
                    }

                    if (columnContainsData)
                        break;

                    xOffset++;
                    recreateArray = true;
                }

                // trim columns at the end
                for (int x = width - 1; x > xOffset; x--)
                {
                    bool columnContainsData = false;
                    for (int y = yOffset; y < height; y++)
                    {
                        if (_patternDisplay.Data[x, y])
                        {
                            columnContainsData = true;
                            break;
                        }
                    }

                    if (columnContainsData)
                        break;

                    width = x;
                    recreateArray = true;
                }

                Pattern trimmedArray = null;

                if (recreateArray)
                {
                    var data = new bool[width - xOffset, height - yOffset];

                    for (int x = xOffset; x < width; x++)
                    {
                        for (int y = yOffset; y < height; y++)
                        {
                            data[x - xOffset, y - yOffset] = _patternDisplay.Data[x, y];
                        }
                    }

                    trimmedArray = new Pattern(data, yOffset);
                }

                return trimmedArray ?? _patternDisplay;
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
        }
    }
}
