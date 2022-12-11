using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class dhmsInput : UserControl
    {
        private TimeSpan ts;

        public delegate void ValueChangedEventHandler(dhmsInput sender, TimeSpan timespan);

        public event ValueChangedEventHandler ValueChanged;
        public bool changed;
        private bool _change;

        public dhmsInput()
        {
            InitializeComponent();
            ts = TimeSpan.Zero;
            changed = false;
            _change = true;

            mTBD.MouseWheel += (s, e) => ChangeValue((TextBox)s, Math.Sign(e.Delta));
            mTBH.MouseWheel += (s, e) => ChangeValue((TextBox)s, Math.Sign(e.Delta));
            mTBM.MouseWheel += (s, e) => ChangeValue((TextBox)s, Math.Sign(e.Delta) * 5);
            mTBS.MouseWheel += (s, e) => ChangeValue((TextBox)s, Math.Sign(e.Delta) * 5);
        }

        private void mTB_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox input = (TextBox)sender;
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                    {
                        int i = 0;
                        if (input == mTBH)
                        {
                            i = 1;
                        }
                        else if (input == mTBM)
                        {
                            i = 2;
                        }
                        else if (input == mTBS)
                        {
                            i = 3;
                        }

                        var inputs = new TextBox[] { mTBD, mTBH, mTBM, mTBS };

                        if (e.KeyCode == Keys.Left) i--;
                        else i++;

                        if (i < 0) i = 3;
                        else if (i > 3) i = 0;

                        inputs[i].Focus();
                        break;
                    }
                case Keys.Up:
                    {
                        ChangeValue(input, 1);
                        break;
                    }
                case Keys.Down:
                    {
                        ChangeValue(input, -1);
                        break;
                    }
                default:
                    input.Text = RegexNonNumbers.Replace(input.Text, string.Empty);
                    break;
            }
        }

        private static readonly Regex RegexNonNumbers = new Regex(@"\D", RegexOptions.Compiled);

        private void ChangeValue(TextBox input, int valueChange, bool selectAfterChange = true)
        {
            int.TryParse(input.Text, out var v);
            v += valueChange;
            if (v < 0) v = 0;
            input.Text = v.ToString("D2");
            if (selectAfterChange)
                input.SelectAll();
        }

        private void mTB_TextChanged(object sender, EventArgs e)
        {
            if (!_change) return;
            int.TryParse(mTBD.Text, out int d);
            int.TryParse(mTBH.Text, out int h);
            int.TryParse(mTBM.Text, out int m);
            int.TryParse(mTBS.Text, out int s);
            ts = new TimeSpan(d, h, m, s);
            changed = true;
            ValueChanged?.Invoke(this, ts);
        }

        public TimeSpan Timespan
        {
            get => ts;
            set
            {
                _change = false;
                ts = value.TotalSeconds >= 0 ? value : TimeSpan.Zero;
                mTBD.Text = ((int)Math.Floor(ts.TotalDays)).ToString("D2");
                mTBH.Text = ts.Hours.ToString("D2");
                mTBM.Text = ts.Minutes.ToString("D2");
                mTBS.Text = ts.Seconds.ToString("D2");
                changed = false;
                _change = true;
            }
        }

        private void mTB_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }
    }
}
