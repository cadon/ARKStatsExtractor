using System;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    class Nud : NumericUpDown
    {
        private bool brightForeColor;
        public decimal NeutralNumber;

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
            get
            {
                return base.BackColor;
            }
        }
    }
}
