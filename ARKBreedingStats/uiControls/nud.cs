using System;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    class Nud : NumericUpDown
    {
        private bool brightForeColor;
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
            if (brightForeColor)
            {
                ForeColor = Value == NeutralNumber ? Color.LightGray : Color.White;
            }
            else
            {
                ForeColor = Value == NeutralNumber ? SystemColors.GrayText : SystemColors.WindowText;
            }
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                brightForeColor = Utils.ForeColor(BackColor).GetBrightness() == 1;
                updateColors();
            }
        }

        public decimal NeutralNumber
        {
            get => _NeutralNumber;
            set
            {
                _NeutralNumber = value;
                updateColors();
            }
        }

        private void updateColors()
        {
            if (brightForeColor)
            {
                ForeColor = Value == NeutralNumber ? Color.LightGray : Color.White;
            }
            else
            {
                ForeColor = Value == NeutralNumber ? SystemColors.GrayText : SystemColors.WindowText;
            }
        }
    }
}
