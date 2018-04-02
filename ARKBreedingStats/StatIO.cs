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
        public bool percent; // indicates whether this stat is expressed as a percentile
        private string statName;
        private double breedingValue;
        private StatIOInputType inputType;
        public event Form1.LevelChangedEventHandler LevelChanged;
        public event Form1.InputValueChangedEventHandler InputValueChanged;
        public int statIndex;
        private bool domZeroFixed;
        ToolTip tt = new ToolTip();
        public int barMaxLevel = 45;

        public StatIO()
        {
            InitializeComponent();
            numLvW.Value = 0;
            numLvD.Value = 0;
            labelBValue.Text = "";
            postTame = true;
            percent = false;
            breedingValue = 0;
            this.groupBox1.Click += new System.EventHandler(groupBox1_Click);
            InputType = inputType;
            // ToolTips
            tt.InitialDelay = 300;
            tt.SetToolTip(checkBoxFixDomZero, "Check to lock to zero (if you never leveled up this stat)");
        }

        public double Input
        {
            get { return (double)numericUpDownInput.Value * (percent ? 0.01 : 1); }
            set
            {
                if (value < 0)
                {
                    this.numericUpDownInput.Value = 0;
                    labelFinalValue.Text = "unknown";
                }
                else
                {
                    value = value * (percent ? 100 : 1);
                    if (value > (double)numericUpDownInput.Maximum) value = (double)numericUpDownInput.Maximum;
                    this.numericUpDownInput.Value = (decimal)value;
                    this.labelFinalValue.Text = value.ToString("N1");
                }
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
                int v = value;
                if (v < 0)
                    numLvW.Value = -1; // value can be unknown if multiple stats are not shown (e.g. wild speed and oxygen)
                else
                {
                    if (v > numLvW.Maximum)
                        v = (int)numLvW.Maximum;
                    this.numLvW.Value = v;
                }
                this.labelWildLevel.Text = (value < 0 ? "?" : v.ToString());
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
                    this.labelBValue.Text = Math.Round((percent ? 100 : 1) * value, 1).ToString("N1") + (postTame ? "" : " +*");
                    breedingValue = value;
                }
                else { this.labelBValue.Text = "unknown"; }
            }
            get { return breedingValue; }
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

        public StatIOStatus Status
        {
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
                        this.BackColor = Color.Transparent;//SystemColors.Control;
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

        public StatIOStatus TopLevel
        {
            set
            {
                switch (value)
                {
                    case StatIOStatus.TopLevel:
                        labelWildLevel.BackColor = Color.LightGreen;
                        break;
                    case StatIOStatus.NewTopLevel:
                        labelWildLevel.BackColor = Color.Gold;
                        break;
                    default:
                        labelWildLevel.BackColor = Color.Transparent;
                        break;
                }
            }
        }

        public bool ShowBarAndLock
        {
            set
            {
                panelBarWildLevels.Visible = value;
                panelBarDomLevels.Visible = value;
                checkBoxFixDomZero.Visible = value;
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

        public void Clear()
        {
            Status = StatIOStatus.Neutral;
            TopLevel = StatIOStatus.Neutral;
            numLvW.Value = 0;
            numLvD.Value = 0;
            labelDomLevel.Text = "0";
            labelWildLevel.Text = "0";
            labelFinalValue.Text = "0";
            labelBValue.Text = "";
        }

        private void numLvW_ValueChanged(object sender, EventArgs e)
        {
            int lengthPercentage = 100 * (int)numLvW.Value / barMaxLevel; // in percentage of the max-barwidth

            if (lengthPercentage > 100) { lengthPercentage = 100; }
            if (lengthPercentage < 0) { lengthPercentage = 0; }
            this.panelBarWildLevels.Width = lengthPercentage * 283 / 100;
            this.panelBarWildLevels.BackColor = Utils.getColorFromPercent(lengthPercentage);
            tt.SetToolTip(panelBarWildLevels, Utils.levelPercentile((int)numLvW.Value));

            if (inputType != StatIOInputType.FinalValueInputType)
                LevelChanged(this);
        }

        private void numLvD_ValueChanged(object sender, EventArgs e)
        {
            int lengthPercentage = 100 * (int)numLvD.Value / barMaxLevel; // in percentage of the max-barwidth

            if (lengthPercentage > 100) { lengthPercentage = 100; }
            if (lengthPercentage < 0) { lengthPercentage = 0; }
            this.panelBarDomLevels.Width = lengthPercentage * 283 / 100;
            this.panelBarDomLevels.BackColor = Utils.getColorFromPercent(lengthPercentage);

            if (inputType != StatIOInputType.FinalValueInputType)
                LevelChanged(this);
        }

        private void numericUpDownInput_ValueChanged(object sender, EventArgs e)
        {
            if (InputType == StatIOInputType.FinalValueInputType)
                InputValueChanged(this);
        }

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            if (n != null)
            {
                n.Select(0, n.Text.Length);
            }
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

        private void checkBoxFixDomZero_CheckedChanged(object sender, EventArgs e)
        {
            domZeroFixed = checkBoxFixDomZero.Checked;
            checkBoxFixDomZero.Image = (domZeroFixed ? Properties.Resources.locked : Properties.Resources.unlocked);
        }

        private void panelFinalValue_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        private void panelBar_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        public bool DomLevelLockedZero
        {
            get { return domZeroFixed; }
            set { checkBoxFixDomZero.Checked = value; }
        }

    }

    public enum StatIOStatus
    {
        Neutral, Unique, Nonunique, Error,
        TopLevel, // wild level is equal to the current top-level
        NewTopLevel // wild level is higher than the current top-level
    }

    public enum StatIOInputType
    {
        FinalValueInputType,
        LevelsInputType
    };
}
