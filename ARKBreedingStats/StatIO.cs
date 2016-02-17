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
        private StatIOInputType inputType;

        public StatIO()
        {
            InitializeComponent();
            this.numLvW.Value = 0;
            this.numLvD.Value = 0;
            this.labelBValue.Text = "";
            postTame = true;
            percent = false;
            //InputType = StatIOInputType.FinalValueInputType;
            this.groupBox1.Click += new System.EventHandler(this.groupBox1_Click);
            InputType = inputType;
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

        public Int32 LevelWild
        {
            set { this.numLvW.Value= value; }
            get { return (Int16)this.numLvW.Value; }
        }

        public Int32 LevelDom
        {
            set { this.numLvD.Value = value; }
            get { return (Int16)this.numLvD.Value; }
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
                inputPanel.Visible = !value;
            }
        }

        public StatIOInputType InputType
        {
            set
            {
                if (value == StatIOInputType.FinalValueInputType)
                {
                    numLvW.ReadOnly = true;
                    numLvW.TabStop = false;
                    numLvD.ReadOnly = true;
                    numLvD.TabStop = false;
                    numericUpDownInput.ReadOnly = false;
                    numericUpDownInput.TabStop = true;
                }
                else
                {
                    numLvW.ReadOnly = false;
                    numLvW.TabStop = true;
                    numLvD.ReadOnly = false;
                    numLvD.TabStop = true;
                    numericUpDownInput.ReadOnly = true;
                    numericUpDownInput.TabStop = false;
                }
                inputType = value;
            }

            get
            {
                return inputType;
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
            numLvW.Value = 0;
            numLvD.Value = 0;
            labelBValue.Text = "";
        }

        private void panelSettings_Paint(object sender, PaintEventArgs e)
        {

        }

        private void numLvW_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numLvD_ValueChanged(object sender, EventArgs e)
        {

        }
        
        public void computeStatValueFromLevelsWithTamingEfficiency( CreatureStat animalData, double tamingEfficiency )
        {
            // Stat value according to wiki is:
            // V = (B * ( 1 + Lw * Iw) + Ta * TaM) * (1 + TE * Tm * TmM) * (1 + Ld * Id * IdM)

            double v = (animalData.BaseValue * (1 + LevelWild * animalData.IncPerWildLevel ) + MultAdd * animalData.AddWhenTamed  ); // already inaccurate. Pterano with 30 wild hp levels yields 1470.01125 instead of 1470.1 visible in-game
            v *= ( 1 + (tamingEfficiency/100.0f) * animalData.MultAffinity /* * MultAff */ ); // damage is always wrong at this stage. MultAff is already included.
            v *= (1 + LevelDom * animalData.IncPerTamedLevel * MultLevel);

            numericUpDownInput.Value = (decimal)v;

        }

    }

    public enum StatIOInputType
    {
        FinalValueInputType,
        LevelsInputType
    };
}
