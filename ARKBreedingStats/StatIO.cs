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
        public bool postTame; // if false (aka creature untamed) display note that stat can be higher after taming
        private StatIOStatus status;
        private bool percent, showBar;
        private string statName;
        private double breedingValue;
        private StatIOInputType inputType;
        public event Form1.LevelChangedEventHandler LevelChanged;
        public int statIndex;

        public StatIO()
        {
            InitializeComponent();
            this.numLvW.Value = 0;
            this.numLvD.Value = 0;
            this.labelBValue.Text = "";
            postTame = true;
            percent = false;
            breedingValue = 0;
            showBar = true;
            this.groupBox1.Click += new System.EventHandler(this.groupBox1_Click);
            InputType = inputType;
        }

        public double Input
        {
            get { return (double)this.numericUpDownInput.Value; }
            set
            {
                this.numericUpDownInput.Value = (decimal)value * (percent ? 100 : 1);
                this.labelFinalValue.Text = (value * (percent ? 100 : 1)).ToString();
            }
        }

        public string Title
        {
            set
            {
                statName = value;
                groupBox1.Text = value + (Percent ? " [%]" : "");
            }
        }

        public Int32 LevelWild
        {
            set
            {
                this.labelWildLevel.Text = value.ToString();
                this.numLvW.Value = value;
            }
            get { return (Int16)this.numLvW.Value; }
        }

        public Int32 LevelDom
        {
            set
            {
                this.labelDomLevel.Text = value.ToString();
                this.numLvD.Value = value;
            }
            get { return (Int16)this.numLvD.Value; }
        }

        public double BreedingValue
        {
            set
            {
                if (value >= 0)
                {
                    this.labelBValue.Text = Math.Round((percent ? 100 : 1) * value, 1).ToString() + (postTame ? "" : " +*");
                    breedingValue = value;
                }
                else { this.labelBValue.Text = "error"; }
            }
            get { return breedingValue; }
        }

        public double TamingEfficiency;

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

        public StatIOStatus Status
        {
            // represents how successful this stat was extracted: 1: unique, 0: neutral, -1: not unique, -2: error
            set
            {
                status = value;
                this.ForeColor = SystemColors.ControlText;
                this.numericUpDownInput.BackColor = System.Drawing.SystemColors.Window;
                switch (status)
                {
                    case StatIOStatus.Unique:
                        this.BackColor = Color.FromArgb(180, 255, 128);
                        break;
                    case StatIOStatus.Neutral:
                        this.BackColor = SystemColors.Control;
                        break;
                    case StatIOStatus.Nonunique:
                        this.BackColor = Color.FromArgb(255, 255, 127);
                        break;
                    case StatIOStatus.Error:
                        this.numericUpDownInput.BackColor = Color.FromArgb(255, 200, 200);
                        this.BackColor = Color.LightCoral;
                        break;
                }
            }
            get { return status; }
        }

        public bool ShowBar
        {
            set
            {
                showBar = value;
                panelBar.Visible = value;
            }
        }

        public StatIOInputType InputType
        {
            set
            {
                panelFinalValue.Visible = (value == StatIOInputType.FinalValueInputType);
                inputPanel.Visible = (value != StatIOInputType.FinalValueInputType);

                inputType = value;
            }

            get
            {
                return inputType;
            }

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
            Status = StatIOStatus.Neutral;
            numLvW.Value = 0;
            numLvD.Value = 0;
            labelDomLevel.Text = "0";
            labelWildLevel.Text = "0";
            labelFinalValue.Text = "0";
            labelBValue.Text = "";
        }

        private void numLvW_ValueChanged(object sender, EventArgs e)
        {
            int length = (int)((int)numLvW.Value * (100.0f / Properties.Settings.Default.BarMaximum)); // in percentage of the max-barwidth

            if (length > 100) { length = 100; }
            if (length < 0) { length = 0; }
            this.panelBar.Width = length * 283 / 100;
            int r = 511 - length * 255 / 50;
            int g = length * 255 / 50;
            if (r < 0) { r = 0; }
            if (g < 0) { g = 0; }
            if (r > 255) { r = 255; }
            if (g > 255) { g = 255; }
            this.panelBar.BackColor = Color.FromArgb(r, g, 0);
            LevelChanged(this);
        }

        private void numLvD_ValueChanged(object sender, EventArgs e)
        {
            LevelChanged(this);
        }

        private void inputPanel_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }
        private void groupBox1_Click(object sender, System.EventArgs e)
        {
            this.OnClick(e);
        }

        private void labelBValue_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        private void labelWildLevel_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        private void labelDomLevel_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

    }

    public enum StatIOStatus
    {
        Neutral, Unique, Nonunique, Error
    }

    public enum StatIOInputType
    {
        FinalValueInputType,
        LevelsInputType
    };
}
