using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    class dhmInput : MaskedTextBox
    {
        private TimeSpan ts;
        public bool changed;

        public dhmInput()
        {
            ts = new TimeSpan();
            Mask = @"00\:00\:00";
            TextChanged += new EventHandler(textBox_TextChanged);

            Timespan = ts;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            Match m = Regex.Match(Text, @"(\d+):(\d+):(\d+)");
            if (m.Groups.Count > 3)
            {
                ts = new TimeSpan(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), 0);
                if (ts.TotalMinutes < 1)
                    ForeColor = SystemColors.GrayText;
                else ForeColor = SystemColors.ControlText;
                BackColor = SystemColors.Window;
                changed = true;
            }
            else
            {
                BackColor = Color.LightSalmon;
                ForeColor = SystemColors.ControlText;
            }
        }

        public TimeSpan Timespan
        {
            get { return ts; }
            set
            {
                ts = value.TotalMinutes >= 0 ? value : new TimeSpan(0);
                changed = false;
                Text = ((int)Math.Floor(ts.TotalDays)).ToString("D2") + ":" + ts.Hours.ToString("D2") + ":" + ts.Minutes.ToString("D2");
            }
        }
    }
}
