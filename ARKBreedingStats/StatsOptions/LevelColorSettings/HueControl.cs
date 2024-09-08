using System;
using System.Drawing;
using System.Windows.Forms;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.utils;
using Newtonsoft.Json;

namespace ARKBreedingStats.StatsOptions.LevelColorSettings
{
    public partial class HueControl : UserControl
    {
        public LevelGraphRepresentation LevelGraphRepresentation;

        public HueControl()
        {
            InitializeComponent();
        }

        public HueControl(ToolTip tt) : this() => UpdateTooltips(tt);

        public void UpdateTooltips(ToolTip tt) => tt?.SetToolTip(CbReverseGradient, "reverse hue direction");

        private void NudLevelValueChanged(object sender, EventArgs e)
        {
            var isHigh = sender == NudLevelHigh;
            if (isHigh)
            {
                var newValue = (int)NudLevelHigh.Value;
                if (LevelGraphRepresentation.UpperBound == newValue) return;
                LevelGraphRepresentation.UpperBound = newValue;
            }
            else
            {
                var newValue = (int)NudLevelLow.Value;
                if (LevelGraphRepresentation.LowerBound == newValue) return;
                LevelGraphRepresentation.LowerBound = newValue;
            }
            if (NudLevelHigh.Value >= NudLevelLow.Value)
            {
                UpdateGradient();
                return;
            }
            if (isHigh)
                NudLevelLow.ValueSave = NudLevelHigh.Value;
            else
                NudLevelHigh.ValueSave = NudLevelLow.Value;
        }

        private void BtColorClick(object sender, EventArgs e) => SelectColor((Button)sender);

        private void SelectColor(Button bt)
        {
            colorDialog1.Color = bt.BackColor;
            if (colorDialog1.ShowDialog() != DialogResult.OK) return;
            var cl = colorDialog1.Color;
            SetColor(cl, bt == BtColorHigh, bt);

        }

        private void SetColor(Color cl, bool higherColor, Button bt)
        {
            if (bt.BackColor == cl) return;
            bt.BackColor = cl;
            if (higherColor)
                LevelGraphRepresentation.UpperColor = cl;
            else
                LevelGraphRepresentation.LowerColor = cl;
            UpdateGradient();
        }

        private void CbReverseGradient_CheckedChanged(object sender, EventArgs e)
        {
            LevelGraphRepresentation.ColorGradientReversed = CbReverseGradient.Checked;
            UpdateGradient();
        }

        private void UpdateGradient()
        {
            if (!(PbColorGradient.Image is Bitmap bmp))
            {
                bmp = new Bitmap(PbColorGradient.Width, PbColorGradient.Height);
                PbColorGradient.Image = bmp;
            }

            var l = bmp.Width;
            var levelLow = LevelGraphRepresentation.LowerBound;
            var levelHigh = LevelGraphRepresentation.UpperBound;
            var levelRange = levelHigh - levelLow + 1;
            if (levelRange <= 0)
            {
                using (var g = Graphics.FromImage(bmp))
                    g.FillRectangle(new SolidBrush(LevelGraphRepresentation.GetLevelColor((int)NudLevelHigh.Value)),
                        0, 0, bmp.Width, bmp.Height);
                PbColorGradient.Invalidate();
                return;
            }
            var barWidth = Math.Max(1, (float)l / levelRange);
            var xBarStart = 0f;
            var h = bmp.Height;
            using (var g = Graphics.FromImage(bmp))
            using (var b = new SolidBrush(Color.Black))
            {
                for (var level = levelLow; level <= levelHigh;)
                {
                    var xBarEnd = xBarStart + barWidth;
                    b.Color = LevelGraphRepresentation.GetLevelColor(level);
                    g.FillRectangle(b, xBarStart, 0, barWidth, h);
                    xBarStart = xBarEnd;
                    level = (int)(levelLow + levelRange * xBarEnd / l);
                }
            }
            PbColorGradient.Invalidate();
        }

        #region Color gradient scrolling

        private (float h, float s, float v) _colorLowHsv;
        private (float h, float s, float v) _colorHighHsv;

        private void PbColorGradient_MouseDown(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Shift)
            {
                // copy or paste hue setting
                if (e.Button == MouseButtons.Right)
                    CopyHueSetting();
                else if (e.Button == MouseButtons.Left)
                    PasteHueSetting();
                return;
            }

            if (ModifierKeys == Keys.Control)
            {
                ResetColors();
                return;
            }

            // create invisible scroll form to scroll the gradient
            var sf = new ScrollForm();
            var startupLocation = Cursor.Position;
            startupLocation.Offset(-sf.Width / 2, -sf.Height / 2);
            sf.SetLocation(startupLocation);
            sf.Moved += UpdateLevelRepresentation;
            _colorLowHsv = LevelGraphRepresentation.LowerColor.GetHsv();
            _colorHighHsv = LevelGraphRepresentation.UpperColor.GetHsv();
            if (_colorLowHsv.h > _colorHighHsv.h)
                _colorLowHsv.h -= 360;
            sf.Show(this);
        }

        /// <summary>
        /// Copy hue settings to clipboard.
        /// </summary>
        private void CopyHueSetting()
        {
            Clipboard.SetText(JsonConvert.SerializeObject(LevelGraphRepresentation));
        }

        /// <summary>
        /// Paste hue settings from clipboard.
        /// </summary>
        private void PasteHueSetting()
        {
            var clipText = Clipboard.GetText();
            if (string.IsNullOrEmpty(clipText)) return;
            var levelGraphSettings = JsonConvert.DeserializeObject<LevelGraphRepresentation>(clipText);
            if (levelGraphSettings == null) return;
            SetControlValues(levelGraphSettings);
        }

        public void SetControlValues(LevelGraphRepresentation levelGraphSettings)
        {
            if (levelGraphSettings == null) return;
            NudLevelLow.ValueSave = levelGraphSettings.LowerBound;
            NudLevelHigh.ValueSave = levelGraphSettings.UpperBound;
            SetColor(levelGraphSettings.LowerColor, false, BtColorLow);
            SetColor(levelGraphSettings.UpperColor, true, BtColorHigh);
            CbReverseGradient.Checked = levelGraphSettings.ColorGradientReversed;
        }

        public void SetValues(LevelGraphRepresentation levelGraphSettings)
        {
            var visible = levelGraphSettings != null;
            Visible = visible;
            if (!visible)
                return;

            LevelGraphRepresentation = levelGraphSettings;
            SetControlValues(levelGraphSettings);
            UpdateGradient();
        }

        private void UpdateLevelRepresentation(int dx, int dy)
        {
            var centerHue = (_colorHighHsv.h + _colorLowHsv.h) / 2;
            var hueHalfDistance = Math.Abs(centerHue - _colorLowHsv.h) + dy * 0.3;
            if (hueHalfDistance >= 179) hueHalfDistance = 179;
            if (hueHalfDistance < 0) hueHalfDistance = 0;
            if (LevelGraphRepresentation.ColorGradientReversed)
                dx = -dx;
            var rangeFactor = 1 + dy * 0.005;
            dx = (int)(dx * rangeFactor);
            centerHue -= dx;

            LevelGraphRepresentation.LowerColor = Utils.ColorFromHsv(centerHue - hueHalfDistance, _colorLowHsv.s, _colorLowHsv.v);
            LevelGraphRepresentation.UpperColor = Utils.ColorFromHsv(centerHue + hueHalfDistance, _colorHighHsv.s, _colorHighHsv.v);

            BtColorLow.BackColor = LevelGraphRepresentation.LowerColor;
            BtColorHigh.BackColor = LevelGraphRepresentation.UpperColor;
            UpdateGradient();
        }

        #endregion

        private void ResetColors()
        {
            var newSettings = LevelGraphRepresentation.GetDefaultValue;
            if (newSettings == LevelGraphRepresentation)
                newSettings = LevelGraphRepresentation.GetDefaultMutationLevelValue;

            SetColor(newSettings.LowerColor, false, BtColorLow);
            SetColor(newSettings.UpperColor, true, BtColorHigh);
        }
    }
}
