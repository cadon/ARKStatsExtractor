using System;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    class Nud : NumericUpDown
    {
        private bool _brightForeColor;
        private decimal _NeutralNumber;

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            Select(0, Text.Length);
        }

        /// <summary>
        /// Sets the value after checking it's &lt; Maximum and > Minimum. If it's out of range, the closes possible value is set
        /// </summary>
        public decimal ValueSave
        {
            set
            {
                if (value > Maximum) value = Maximum;
                if (value < Minimum) value = Minimum;
                Value = value;
            }
        }

        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);

            UpdateForeColor();
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                _brightForeColor = Utils.ForeColor(BackColor).GetBrightness() == 1;
                UpdateForeColor();
            }
        }

        public decimal NeutralNumber
        {
            get => _NeutralNumber;
            set
            {
                _NeutralNumber = value;
                UpdateForeColor();
            }
        }

        private void UpdateForeColor()
        {
            if (Value == NeutralNumber)
            {
                ForeColor = _brightForeColor ? Color.LightGray : SystemColors.GrayText;
            }
            else
            {
                ForeColor = _brightForeColor ? Color.White : SystemColors.WindowText;
            }
        }

        /// <summary>
        /// Highlights the control if the value is not the neutral value.
        /// </summary>
        /// <param name="highlight"></param>
        public void SetExtraHighlightNonDefault(bool highlight)
        {
            BackColor = highlight && Value != NeutralNumber ? Color.FromArgb(190, 40, 20) : SystemColors.Window;
            UpdateForeColor();
        }
    }
}
