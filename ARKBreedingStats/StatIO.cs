using System;
using System.Drawing;
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
        public event Action<StatIO> LevelChanged;
        public event Action<StatIO> InputValueChanged;
        public int statIndex;
        private bool domZeroFixed;
        private readonly ToolTip _tt;
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
            groupBox1.Click += groupBox1_Click;
            InputType = inputType;
            // ToolTips
            _tt = new ToolTip { InitialDelay = 300 };
            _tt.SetToolTip(checkBoxFixDomZero, "Check to lock to zero (if you never leveled up this stat)");
        }

        public double Input
        {
            get => (double)numericUpDownInput.Value * (percent ? 0.01 : 1);
            set
            {
                if (value < 0)
                {
                    numericUpDownInput.Value = 0;
                    labelFinalValue.Text = Loc.S("Unknown");
                }
                else
                {
                    value = value * (percent ? 100 : 1);
                    numericUpDownInput.ValueSave = (decimal)value;
                    labelFinalValue.Text = value.ToString("N1");
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

        public int LevelWild
        {
            get => (short)numLvW.Value;
            set
            {
                int v = value;
                if (v < 0)
                    numLvW.Value = -1; // value can be unknown if multiple stats are not shown (e.g. wild speed and oxygen)
                else
                {
                    if (v > numLvW.Maximum)
                        v = (int)numLvW.Maximum;
                    numLvW.Value = v;
                }
                labelWildLevel.Text = (value < 0 ? "?" : v.ToString());
            }
        }

        public int LevelDom
        {
            get => (short)numLvD.Value;
            set
            {
                labelDomLevel.Text = value.ToString();
                numLvD.Value = value;
            }
        }

        public double BreedingValue
        {
            get => breedingValue;
            set
            {
                if (value >= 0)
                {
                    labelBValue.Text = Math.Round((percent ? 100 : 1) * value, 1).ToString("N1") + (postTame ? "" : " +*");
                    breedingValue = value;
                }
                else
                {
                    labelBValue.Text = Loc.S("Unknown");
                }
            }
        }

        public bool Percent
        {
            get => percent;
            set
            {
                percent = value;
                Title = statName;
            }
        }

        public bool Selected
        {
            set
            {
                if (value)
                {
                    BackColor = SystemColors.Highlight;
                    ForeColor = SystemColors.HighlightText;
                }
                else
                {
                    Status = status;
                }
            }
        }

        public StatIOStatus Status
        {
            get => status;
            set
            {
                status = value;
                ForeColor = SystemColors.ControlText;
                numericUpDownInput.BackColor = SystemColors.Window;
                switch (status)
                {
                    case StatIOStatus.Unique:
                        BackColor = Color.FromArgb(180, 255, 128);
                        break;
                    case StatIOStatus.Neutral:
                        BackColor = Color.Transparent; //SystemColors.Control;
                        break;
                    case StatIOStatus.NonUnique:
                        BackColor = Color.FromArgb(255, 255, 127);
                        break;
                    case StatIOStatus.Error:
                        numericUpDownInput.BackColor = Color.FromArgb(255, 200, 200);
                        BackColor = Color.LightCoral;
                        break;
                }
            }
        }

        private StatIOStatus _topLevel;
        public StatIOStatus TopLevel
        {
            get => _topLevel;
            set
            {
                _topLevel = value;
                switch (_topLevel)
                {
                    case StatIOStatus.TopLevel:
                        labelWildLevel.BackColor = Color.LightGreen;
                        _tt.SetToolTip(labelWildLevel, Loc.S("topLevel"));
                        break;
                    case StatIOStatus.NewTopLevel:
                        labelWildLevel.BackColor = Color.Gold;
                        _tt.SetToolTip(labelWildLevel, Loc.S("newTopLevel"));
                        break;
                    default:
                        labelWildLevel.BackColor = Color.Transparent;
                        _tt.SetToolTip(labelWildLevel, null);
                        _topLevel = StatIOStatus.Neutral;
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
            get => inputType;
            set
            {
                panelFinalValue.Visible = (value == StatIOInputType.FinalValueInputType);
                inputPanel.Visible = (value != StatIOInputType.FinalValueInputType);

                inputType = value;
            }
        }

        public bool IsActive
        {
            set
            {
                Height = value ? 50 : 16;
                Enabled = value;
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

            if (lengthPercentage > 100)
            {
                lengthPercentage = 100;
            }
            if (lengthPercentage < 0)
            {
                lengthPercentage = 0;
            }
            panelBarWildLevels.Width = lengthPercentage * 283 / 100;
            panelBarWildLevels.BackColor = Utils.GetColorFromPercent(lengthPercentage);
            _tt.SetToolTip(panelBarWildLevels, Utils.LevelPercentile((int)numLvW.Value));

            if (inputType != StatIOInputType.FinalValueInputType)
                LevelChanged(this);
        }

        private void numLvD_ValueChanged(object sender, EventArgs e)
        {
            int lengthPercentage = 100 * (int)numLvD.Value / barMaxLevel; // in percentage of the max-barwidth

            if (lengthPercentage > 100)
            {
                lengthPercentage = 100;
            }
            if (lengthPercentage < 0)
            {
                lengthPercentage = 0;
            }
            panelBarDomLevels.Width = lengthPercentage * 283 / 100;
            panelBarDomLevels.BackColor = Utils.GetColorFromPercent(lengthPercentage);

            if (inputType != StatIOInputType.FinalValueInputType)
                LevelChanged(this);
        }

        private void numericUpDownInput_ValueChanged(object sender, EventArgs e)
        {
            if (InputType == StatIOInputType.FinalValueInputType)
                InputValueChanged?.Invoke(this);
        }

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            n?.Select(0, n.Text.Length);
        }

        private void groupBox1_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        private void labelBValue_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        private void labelWildLevel_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        private void labelDomLevel_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        private void checkBoxFixDomZero_CheckedChanged(object sender, EventArgs e)
        {
            domZeroFixed = checkBoxFixDomZero.Checked;
            checkBoxFixDomZero.Image = (domZeroFixed ? Properties.Resources.locked : Properties.Resources.unlocked);
        }

        private void panelFinalValue_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        private void panelBar_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        public bool DomLevelLockedZero
        {
            get => domZeroFixed;
            set => checkBoxFixDomZero.Checked = value;
        }
    }

    public enum StatIOStatus
    {
        Neutral,
        Unique,
        NonUnique,
        Error,
        /// <summary>
        /// wild level is equal to the current top-level
        /// </summary>
        TopLevel,
        /// <summary>
        /// wild level is higher than the current top-level
        /// </summary>
        NewTopLevel
    }

    public enum StatIOInputType
    {
        FinalValueInputType,
        LevelsInputType
    };
}
