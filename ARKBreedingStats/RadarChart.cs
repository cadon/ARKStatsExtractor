using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ARKBreedingStats.species;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace ARKBreedingStats
{
    internal class RadarChart : PictureBox
    {
        /// <summary>
        /// outer border of graph
        /// </summary>
        private int _maxLevel;
        /// <summary>
        /// Coords of outer points
        /// </summary>
        private readonly List<Point> _maxPs = new List<Point>();
        /// <summary>
        /// Coords of wild levels
        /// </summary>
        private readonly List<Point> _ps = new List<Point>();
        /// <summary>
        /// Coords of mutated levels
        /// </summary>
        private readonly List<Point> _psm = new List<Point>();
        private int _maxR, _xm, _ym; // max-radius, centerX, centerY
        private readonly int[] _currentWildLevels = new int[Stats.StatsCount];
        private readonly int[] _currentMutatedLevels = new int[Stats.StatsCount];
        /// <summary>
        /// Displayed stats as bit flag
        /// </summary>
        private int _displayedStats;

        private readonly List<int> _displayedStatIndices = new List<int>();
        private double _anglePerStat;
        private PathGradientBrush _grBrushBg, _grBrushFg, _grBrushMutations;
        private int _step;
        private const double AngleOffset = Math.PI / 2;

        public RadarChart()
        {
            SizeMode = PictureBoxSizeMode.Zoom;
            Disposed += RadarChart_Disposed;

            InitializeVariables(50);
        }

        private void RadarChart_Disposed(object sender, EventArgs e)
        {
            _grBrushBg.Dispose();
            _grBrushFg.Dispose();
        }

        private bool SetSize()
        {
            var maxRadius = Math.Min(Width, Height) / 2;
            if (_maxR == maxRadius)
                return false; // already set

            _maxR = maxRadius;
            _xm = _maxR + 1;
            _ym = _maxR + 1;
            _maxR -= 15;

            InitializePoints();

            // color gradients
            _grBrushBg?.Dispose();
            _grBrushFg?.Dispose();
            _grBrushMutations?.Dispose();

            float[] relativePositions = { 0, 0.45f, 1 };
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(_xm - _maxR, _ym - _maxR, 2 * _maxR + 1, 2 * _maxR + 1);
            _grBrushBg = new PathGradientBrush(path)
            {
                InterpolationColors = new ColorBlend
                {
                    Colors = new[] {
                        Color.FromArgb(0, 90, 0),
                        Color.FromArgb(90, 90, 0),
                        Color.FromArgb(90, 0, 0)
                    },
                    Positions = relativePositions
                }
            };
            _grBrushFg = new PathGradientBrush(path)
            {
                InterpolationColors = new ColorBlend
                {
                    Colors = new[] {
                        Color.FromArgb(0, 180, 0),
                        Color.FromArgb(180, 180, 0),
                        Color.FromArgb(180, 0, 0)
                    },
                    Positions = relativePositions
                }
            };
            _grBrushMutations = new PathGradientBrush(path)
            {
                InterpolationColors = new ColorBlend
                {
                    Colors = new[] {
                        Color.FromArgb(0, 180, 180),
                        Color.FromArgb(90, 90, 180),
                        Color.FromArgb(180, 0, 180)
                    },
                    Positions = relativePositions
                }
            };
            return true;
        }

        private bool SetMaxLevel(int maxLevel)
        {
            if (_maxLevel == maxLevel)
                return false;

            _maxLevel = maxLevel;

            _step = (int)Math.Round(_maxLevel / 25d);
            if (_step < 1) _step = 1;
            _step *= 5;

            return true;
        }

        /// <summary>
        /// Initialize with a new max chart level.
        /// </summary>
        /// <param name="maxLevel"></param>
        public void InitializeVariables(int maxLevel)
        {
            if (SetSize() | SetMaxLevel(maxLevel))
                SetLevels(); // update graph
        }

        private void InitializeStats(int displayedStats)
        {
            if (displayedStats == _displayedStats) return;
            _displayedStats = displayedStats;
            _displayedStatIndices.Clear();
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _currentMutatedLevels[s] = 0;
                _currentWildLevels[s] = 0;
                if (s == Stats.Torpidity || (_displayedStats & (1 << s)) == 0) continue;
                _displayedStatIndices.Add(s);
            }

            _anglePerStat = Math.PI * 2 / (_displayedStatIndices.Count > 0 ? _displayedStatIndices.Count : 1);

            InitializePoints();
        }

        /// <summary>
        /// Resets points, call after size or stat count was changed.
        /// </summary>
        private void InitializePoints()
        {
            _maxPs.Clear();
            _ps.Clear();
            _psm.Clear();
            for (var s = 0; s < _displayedStatIndices.Count; s++)
            {
                _ps.Add(new Point(_xm, _ym));
                _psm.Add(new Point(_xm, _ym));
                double angle = _anglePerStat * s - AngleOffset;
                _maxPs.Add(Coords(_maxR, angle));
            }
        }

        private Point Coords(int radius, double angle)
        {
            if (radius < 0) radius = 0;
            if (radius > _maxR) radius = _maxR;
            return new Point(_xm + (int)(radius * Math.Cos(angle)), _ym + (int)(radius * Math.Sin(angle)));
        }

        /// <summary>
        /// Draws a chart with the given levels.
        /// </summary>
        /// <param name="levelsWild">If null, the previous values are redrawn.</param>
        public void SetLevels(int[] levelsWild = null, int[] levelMutations = null, Species species = null)
        {
            if (species != null)
                InitializeStats(species.DisplayedStats);

            if (_maxR <= 5 || _ps.Count == 0) return; // image too small

            Bitmap bmp = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(bmp))
            using (Pen penLine = new Pen(Color.FromArgb(128, 255, 255, 255)))
            using (var font = new Font("Microsoft Sans Serif", 8f))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                if (levelsWild != null || levelMutations != null)
                {
                    var displayedStatIndex = 0;
                    foreach (var s in _displayedStatIndices)
                    {
                        double angle = _anglePerStat * displayedStatIndex - AngleOffset;
                        var wildLevelChanged = levelsWild != null && levelsWild[s] != _currentWildLevels[s];
                        var mutatedLevelChanged = levelMutations != null && levelMutations[s] != _currentMutatedLevels[s];


                        if (wildLevelChanged || mutatedLevelChanged)
                        {
                            if (wildLevelChanged)
                            {
                                _currentWildLevels[s] = levelsWild[s];
                                _ps[displayedStatIndex] = Coords(_currentWildLevels[s] * _maxR / _maxLevel, angle);
                            }

                            if (mutatedLevelChanged)
                                _currentMutatedLevels[s] = levelMutations[s];
                            _psm[displayedStatIndex] = Coords((_currentWildLevels[s] + _currentMutatedLevels[s]) * _maxR / _maxLevel, angle);
                        }

                        displayedStatIndex++;
                    }
                }

                g.FillEllipse(_grBrushBg, _xm - _maxR, _ym - _maxR, 2 * _maxR + 1, 2 * _maxR + 1);

                switch (_psm.Count)
                {
                    case 0: break;
                    case 1:
                        var margin = _psm[0].Y;
                        var width = 2 * (_ym - margin);
                        var rectMutationLevel = new Rectangle(margin, margin, width, width);
                        margin = _ps[0].Y;
                        width = 2 * (_ym - margin);
                        var rectWildLevel = new Rectangle(margin, margin, width, width);
                        g.FillEllipse(_grBrushMutations, rectMutationLevel);
                        g.DrawEllipse(penLine, rectMutationLevel);
                        g.FillEllipse(_grBrushFg, rectWildLevel);
                        g.DrawEllipse(penLine, rectWildLevel);
                        break;
                    default:
                        g.FillPolygon(_grBrushMutations, _psm.ToArray());
                        g.DrawPolygon(penLine, _psm.ToArray());
                        g.FillPolygon(_grBrushFg, _ps.ToArray());
                        g.DrawPolygon(penLine, _ps.ToArray());
                        break;
                }

                // grid circles
                double stepFactor = (double)_step / _maxLevel;
                for (int r = 0; r < 5; r++)
                {
                    using (var pen = new Pen(Utils.GetColorFromPercent((int)(100 * r * stepFactor), -0.4)))
                        g.DrawEllipse(pen,
                            (int)(_xm - _maxR * r * stepFactor), (int)(_ym - _maxR * r * stepFactor),
                            (int)(2 * _maxR * r * stepFactor + 1), (int)(2 * _maxR * r * stepFactor + 1));
                }
                // outline
                using (var pen = new Pen(Utils.GetColorFromPercent(100, -0.4)))
                    g.DrawEllipse(pen, _xm - _maxR, _ym - _maxR, 2 * _maxR + 1, 2 * _maxR + 1);

                // stat lines and bullet points
                using (var pen = new Pen(Color.Gray))
                    for (var sdi = 0; sdi < _displayedStatIndices.Count; sdi++)
                    {
                        var s = _displayedStatIndices[sdi];
                        DrawRadialGridLine(_maxPs[sdi]);
                        DrawLineAndBullet(_currentWildLevels[s] + _currentMutatedLevels[s], _psm[sdi]);
                        DrawLineAndBullet(_currentWildLevels[s], _ps[sdi]);

                        void DrawRadialGridLine(Point maxCoords)
                        {
                            pen.Width = 1;
                            pen.Color = Color.Gray;
                            g.DrawLine(pen, _xm, _ym, maxCoords.X, maxCoords.Y);
                        }

                        void DrawLineAndBullet(int level, Point coords)
                        {
                            Color cl = Utils.GetColorFromPercent(100 * level / _maxLevel);
                            pen.Color = cl;
                            pen.Width = 3;
                            g.DrawLine(pen, _xm, _ym, coords.X, coords.Y);
                            const int bulletRadius = 4;
                            using (var b = new SolidBrush(cl))
                            {
                                g.FillEllipse(b, coords.X - bulletRadius, coords.Y - bulletRadius, 2 * bulletRadius,
                                    2 * bulletRadius);
                            }
                            g.DrawEllipse(penLine, coords.X - bulletRadius, coords.Y - bulletRadius, 2 * bulletRadius,
                                2 * bulletRadius);
                        }
                    }

                using (var brush = new SolidBrush(Color.FromArgb(190, 255, 255, 255)))
                {
                    for (int r = 1; r < 5; r++)
                    {
                        g.DrawString((_step * r).ToString("N0"), font,
                            brush, _xm - 8, _ym - 6 + r * _maxR / 5);
                    }

                    g.DrawString((_maxLevel).ToString("N0"), font,
                        brush, _xm - 8, _ym - 11 + _maxR);
                }

                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                using (var brushBlack = new SolidBrush(Color.Black))
                {
                    for (var sdi = 0; sdi < _displayedStatIndices.Count; sdi++)
                    {
                        var s = _displayedStatIndices[sdi];
                        double angle = _anglePerStat * sdi - AngleOffset;
                        g.DrawString(Utils.StatName(s, true), font,
                            brushBlack, _xm - 9 + (int)((_maxR + 10) * Math.Cos(angle)),
                            _ym - 5 + (int)((_maxR + 10) * Math.Sin(angle)));
                    }
                }
            }

            Image?.Dispose();
            Image = bmp;
        }
    }
}
