using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{

    public partial class StatIO : UserControl
    {
        private bool postTame; // if false (aka creature untamed) display note that stat can be higher after taming
        private int status; // 0: neutral, 1: good, -1: not unique, -2: error
        private bool percent;
        private string statName;

        public StatIO()
        {
            InitializeComponent();
            this.labelLvW.Text = "";
            this.labelLvD.Text = "";
            this.labelBValue.Text = "";
            postTame = true;
            percent = false;
            this.groupBox1.Click += new System.EventHandler(this.groupBox1_Click);
        }

        private void groupBox1_Click(object sender, System.EventArgs e)
        {
            this.OnClick(e);
        }
        private void labelLvW_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        private void labelLvD_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        private void labelBValue_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        public double Input
        {
            get { return (double)this.numericUpDownInput.Value; }
            set { this.numericUpDownInput.Value = (decimal)value; }
        }

        public string Title
        {
            set
            {
                statName = value;
                groupBox1.Text = value + (Percent ? " [%]" : "");
            }
        }

        public string LevelWild
        {
            set { this.labelLvW.Text = value; }
            get { return this.labelLvW.Text; }
        }

        public string LevelDom
        {
            set { this.labelLvD.Text = value; }
        }

        public double BreedingValue
        {
            set
            {
                if (value >= 0) { this.labelBValue.Text = Math.Round((percent ? 100 : 1) * value, 1).ToString() + (percent ? " %" : "") + (postTame ? "" : " +*"); }
                else { this.labelBValue.Text = "error"; }
            }
        }

        public bool PostTame
        {
            set { postTame = value; }
            get { return postTame; }
        }

        public bool Percent
        {
            set
            {
                percent = value;
                Title = statName;
            }
            get { return percent; }
        }

        public bool Selected
        {
            set
            {
                if (value)
                {
                    this.BackColor = SystemColors.Highlight;
                    this.ForeColor = SystemColors.HighlightText;
                }
                else
                {
                    Status = status;
                }
            }
        }

        public int Status
        {
            set
            {
                status = value;
                this.ForeColor = SystemColors.ControlText;
                this.numericUpDownInput.BackColor = System.Drawing.SystemColors.Window;
                switch (status)
                {
                    case 1:
                        this.BackColor = Color.FromArgb(180, 255, 128);
                        break;
                    case 0:
                        this.BackColor = SystemColors.Control;
                        break;
                    case -1:
                        this.BackColor = Color.FromArgb(255, 255, 127);
                        break;
                    case -2:
                        this.numericUpDownInput.BackColor = Color.FromArgb(255, 200, 200);
                        this.BackColor = Color.LightCoral;
                        break;
                }
            }
            get { return status; }
        }

        // sets the visual representation of the bar in %
        public int BarLength
        {
            set
            {
                if (value > 100) { value = 100; }
                if(value < 0) { value = 0; }
                this.panelBar.Width = value * 283 / 100;
                int r = 511 - value * 255 / 33;
                int g = value * 255 / 33;
                if (r < 0) { r = 0; }
                if (g < 0) { g = 0; }
                if (r > 255) { r = 255; }
                if (g > 255) { g = 255; }
                this.panelBar.BackColor = Color.FromArgb(r, g, 0);
            }
        }

        public bool Settings
        {
            set
            {
                panelSettings.Visible = value;
                numericUpDownInput.Visible = !value;
            }
        }

        public double MultAdd
        {
            set { numericUpDownMultAdd.Value = (decimal)value; }
            get { return (double)numericUpDownMultAdd.Value; }
        }
        public double MultAff
        {
            set { numericUpDownMultAff.Value = (decimal)value; }
            get { return (double)numericUpDownMultAff.Value; }
        }
        public double MultLevel
        {
            set { numericUpDownMultLevel.Value = (decimal)value; }
            get { return (double)numericUpDownMultLevel.Value; }
        }

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            if (n != null)
            {
                n.Select(0, n.Text.Length);
            }
        }

        public void Clear()
        {
            Status = 0;
            labelLvD.Text = "n/a";
            labelLvW.Text = "n/a";
            labelBValue.Text = "";
        }

    }
}
