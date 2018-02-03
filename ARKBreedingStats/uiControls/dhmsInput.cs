using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ARKBreedingStats.uiControls
{
    public partial class dhmsInput : UserControl
    {
        private TimeSpan ts;
        public delegate void ValueChangedEventHandler(dhmsInput sender, TimeSpan timespan);
        public event ValueChangedEventHandler ValueChanged;
        public bool changed;
        private bool change;

        public dhmsInput()
        {
            InitializeComponent();
            ts = TimeSpan.Zero;
            changed = false;
            change = true;
        }

        private void mTB_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox input = (TextBox)sender;
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                int i = 0;
                if (input == mTBH) { i = 1; }
                else if (input == mTBM) { i = 2; }
                else if (input == mTBS) { i = 3; }

                List<TextBox> inputs = new List<TextBox> { mTBD, mTBH, mTBM, mTBS };

                if (e.KeyCode == Keys.Left) i--;
                else i++;

                if (i < 0) i = 3;
                else if (i > 3) i = 0;

                inputs[i].Focus();
            }
            else if (e.KeyCode == Keys.Up)
            {
                int i;
                int.TryParse(input.Text, out i);
                input.Text = (++i).ToString("D2");
                input.SelectAll();
            }
            else if (e.KeyCode == Keys.Down)
            {
                int i;
                int.TryParse(input.Text, out i);
                i--;
                if (i < 0) i = 0;
                input.Text = i.ToString("D2");
                input.SelectAll();
            }
            else
            {
                input.Text = Regex.Replace(input.Text, @"\D", "");
            }
        }

        private void mTB_TextChanged(object sender, EventArgs e)
        {
            if (change)
            {
                int d, h, m, s;
                int.TryParse(mTBD.Text, out d);
                int.TryParse(mTBH.Text, out h);
                int.TryParse(mTBM.Text, out m);
                int.TryParse(mTBS.Text, out s);
                ts = new TimeSpan(d, h, m, s);
                changed = true;
                ValueChanged?.Invoke(this, ts);
            }
        }

        public TimeSpan Timespan
        {
            get { return ts; }
            set
            {
                change = false;
                ts = value.TotalSeconds >= 0 ? value : TimeSpan.Zero;
                mTBD.Text = ((int)Math.Floor(ts.TotalDays)).ToString("D2");
                mTBH.Text = ts.Hours.ToString("D2");
                mTBM.Text = ts.Minutes.ToString("D2");
                mTBS.Text = ts.Seconds.ToString("D2");
                changed = false;
                change = true;
            }
        }

        private void mTB_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }
    }
}
