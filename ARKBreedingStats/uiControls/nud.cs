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

        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            if (brightForeColor)
            {
                if (Value == NeutralNumber)
                    ForeColor = Color.LightGray;
                else ForeColor = Color.White;
            }
            else
            {
                if (Value == NeutralNumber)
                    ForeColor = SystemColors.GrayText;
                else ForeColor = SystemColors.WindowText;
            }
        }

        public override Color BackColor
        {
            set
            {
                base.BackColor = value;
                brightForeColor = Utils.ForeColor(BackColor).GetBrightness() == 1;
                updateColors();
            }
            get
            {
                return base.BackColor;
            }
        }

        public decimal NeutralNumber
        {
            get { return _NeutralNumber; }
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
                if (Value == NeutralNumber)
                    ForeColor = Color.LightGray;
                else ForeColor = Color.White;
            }
            else
            {
                if (Value == NeutralNumber)
                    ForeColor = SystemColors.GrayText;
                else ForeColor = SystemColors.WindowText;
            }
        }
    }
}
