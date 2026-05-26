using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public class Nud : NumericUpDown
    {
        private bool _brightForeColor;

        public Nud()
        {
            _brightForeColor = Utils.ForeColor(BackColor).GetBrightness() > 0.5;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            Select(0, Text.Length);
        }

        /// <summary>
        /// Sets the value after checking it is &lt; Maximum and &gt; Minimum. If it's out of range, the closest valid value is set
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public decimal ValueSave
        {
            set
            {
                if (value > Maximum) value = Maximum;
                if (value < Minimum) value = Minimum;
                Value = value;
            }
        }

        /// <summary>
        /// Sets the value after checking it is &lt; Maximum and &gt; Minimum. If it is out of range, the closest valid value is set.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double ValueSaveDouble
        {
            set => ValueSave = (decimal)value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double ValueDouble => (double)Value;

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
                _brightForeColor = Utils.ForeColor(BackColor).GetBrightness() > 0.5;
                UpdateForeColor();
            }
        }

        /// <summary>
        /// If the control displays this number, it will be dimmed. That way controls with changed numbers will be more visible.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public decimal NeutralNumber
        {
            get;
            set
            {
                field = value;
                UpdateForeColor();
            }
        }

        private void UpdateForeColor()
        {
            if (Value == NeutralNumber)
            {
                ForeColor = _brightForeColor ? Color.LightGray : Color.FromArgb(44, 44, 44);
            }
            else
            {
                ForeColor = _brightForeColor ? Color.White : Color.Black;
            }
        }

        /// <summary>
        /// Highlights the control if the value is not the neutral value.
        /// </summary>
        /// <param name="highlight"></param>
        public void SetExtraHighlightNonDefault(bool highlight)
        {
            BackColor = highlight && Value != NeutralNumber ? Color.FromArgb(220, 120, 20) : SystemColors.Window;
            UpdateForeColor();
        }
    }
}
