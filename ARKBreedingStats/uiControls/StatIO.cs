using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.utils;

namespace ARKBreedingStats
{
    public partial class StatIO : UserControl
    {
        public bool postTame; // if false (aka creature untamed) display note that stat can be higher after taming
        private StatIOStatus _status;
        public bool percent; // indicates whether this stat is expressed as a percentile
        private string _statName;
        private double _breedingValue;
        private StatIOInputType _inputType;
        public event Action<StatIO> LevelChanged;
        public event Action<StatIO> InputValueChanged;
        public int statIndex;
        private bool _domZeroFixed;
        private readonly ToolTip _tt;
        public int barMaxLevel = 45;
        private const int MaxBarLength = 335;

        public StatIO()
        {
            InitializeComponent();
            numLvW.Value = 0;
            numLvD.Value = 0;
            labelBValue.Text = string.Empty;
            postTame = true;
            percent = false;
            _breedingValue = 0;
            groupBox1.Click += groupBox1_Click;
            InputType = _inputType;
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
                _statName = value;
                groupBox1.Text = value + (Percent ? " [%]" : string.Empty);
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

        public int LevelMut
        {
            get => (short)nudLvM.Value;
            set
            {
                labelMutatedLevel.Text = value.ToString();
                nudLvM.Value = value;
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
            get => _breedingValue;
            set
            {
                if (value >= 0)
                {
                    labelBValue.Text = Math.Round((percent ? 100 : 1) * value, 1).ToString("N1") + (postTame ? string.Empty : " +*");
                    _breedingValue = value;
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
                Title = _statName;
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
                    Status = _status;
                }
            }
        }

        public StatIOStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                ForeColor = SystemColors.ControlText;
                numericUpDownInput.BackColor = SystemColors.Window;
                Cursor = null;
                switch (_status)
                {
                    case StatIOStatus.Unique:
                        BackColor = ColorModeColors.Success;
                        break;
                    case StatIOStatus.Neutral:
                        BackColor = ColorModeColors.Neutral;
                        break;
                    case StatIOStatus.NonUnique:
                        BackColor = ColorModeColors.NonUnique;
                        Cursor = Cursors.Hand;
                        break;
                    case StatIOStatus.Error:
                        numericUpDownInput.BackColor = Color.FromArgb(255, 200, 200);
                        BackColor = ColorModeColors.Error;
                        break;
                }
            }
        }

        private LevelStatus _topLevel;
        public LevelStatus TopLevel
        {
            get => _topLevel;
            set
            {
                _topLevel = value;

                if (_topLevel == LevelStatus.Neutral)
                {
                    labelWildLevel.BackColor = Color.Transparent;
                    _tt.SetToolTip(labelWildLevel, null);
                    return;
                }

                if (_topLevel.HasFlag(LevelStatus.TopLevel))
                {
                    labelWildLevel.BackColor = Color.LightGreen;
                    _tt.SetToolTip(labelWildLevel, Loc.S("topLevel"));
                }
                else if (_topLevel.HasFlag(LevelStatus.NewTopLevel))
                {
                    labelWildLevel.BackColor = Color.Gold;
                    _tt.SetToolTip(labelWildLevel, Loc.S("newTopLevel"));
                }

                if (_topLevel.HasFlag(LevelStatus.MaxLevelForLevelUp))
                {
                    labelWildLevel.BackColor = Color.DeepSkyBlue;
                    _tt.SetToolTip(labelWildLevel, Loc.S("maxLevelForLevelUp"));
                }
                else if (_topLevel.HasFlag(LevelStatus.MaxLevel))
                {
                    labelWildLevel.BackColor = Color.Orange;
                    _tt.SetToolTip(labelWildLevel, Loc.S("maxLevelSaved"));
                }
                else if (_topLevel.HasFlag(LevelStatus.UltraMaxLevel))
                {
                    labelWildLevel.BackColor = Color.LightCoral;
                    _tt.SetToolTip(labelWildLevel, Loc.S("ultraMaxLevel"));
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
            get => _inputType;
            set
            {
                panelFinalValue.Visible = (value == StatIOInputType.FinalValueInputType);
                inputPanel.Visible = (value != StatIOInputType.FinalValueInputType);

                _inputType = value;
            }
        }

        public bool IsActive
        {
            set
            {
                Height = value ? 50 : 16;
                Enabled = value;
            }
            get => Enabled;
        }

        public void Clear()
        {
            Status = StatIOStatus.Neutral;
            TopLevel = LevelStatus.Neutral;
            numLvW.Value = 0;
            numericUpDownInput.Value = 0;
            numLvD.Value = 0;
            labelWildLevel.Text = "0";
            labelMutatedLevel.Text = "0";
            labelDomLevel.Text = "0";
            labelFinalValue.Text = "0";
            labelBValue.Text = string.Empty;
        }

        private void numLvW_ValueChanged(object sender, EventArgs e)
        {
            int lengthPercentage = 100 * (int)numLvW.Value / barMaxLevel; // in percentage of the max bar width

            if (lengthPercentage > 100)
            {
                lengthPercentage = 100;
            }
            if (lengthPercentage < 0)
            {
                lengthPercentage = 0;
            }
            panelBarWildLevels.Width = lengthPercentage * MaxBarLength / 100;
            panelBarWildLevels.BackColor = Utils.GetColorFromPercent(lengthPercentage);
            _tt.SetToolTip(panelBarWildLevels, Utils.LevelPercentile((int)numLvW.Value));

            if (_inputType != StatIOInputType.FinalValueInputType)
                LevelChangedDebouncer();
        }

        private void nudLvM_ValueChanged(object sender, EventArgs e)
        {
            int lengthPercentage = 100 * (int)nudLvM.Value / barMaxLevel; // in percentage of the max bar width

            if (lengthPercentage > 100)
            {
                lengthPercentage = 100;
            }
            if (lengthPercentage < 0)
            {
                lengthPercentage = 0;
            }
            panelBarMutLevels.Width = lengthPercentage * MaxBarLength / 100;
            panelBarMutLevels.BackColor = Utils.GetColorFromPercent(lengthPercentage);

            if (_inputType != StatIOInputType.FinalValueInputType)
                LevelChangedDebouncer();
        }

        private void numLvD_ValueChanged(object sender, EventArgs e)
        {
            int lengthPercentage = 100 * (int)numLvD.Value / barMaxLevel; // in percentage of the max bar width

            if (lengthPercentage > 100)
            {
                lengthPercentage = 100;
            }
            if (lengthPercentage < 0)
            {
                lengthPercentage = 0;
            }
            panelBarDomLevels.Width = lengthPercentage * MaxBarLength / 100;
            panelBarDomLevels.BackColor = Utils.GetColorFromPercent(lengthPercentage);

            if (_inputType != StatIOInputType.FinalValueInputType)
                LevelChangedDebouncer();
        }

        private readonly Debouncer _levelChangedDebouncer = new Debouncer();

        private void LevelChangedDebouncer() => _levelChangedDebouncer.Debounce(200, FireLevelChanged, Dispatcher.CurrentDispatcher);

        private void FireLevelChanged() => LevelChanged?.Invoke(this);

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
            _domZeroFixed = checkBoxFixDomZero.Checked;
            checkBoxFixDomZero.Image = (_domZeroFixed ? Properties.Resources.locked : Properties.Resources.unlocked);
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
            get => _domZeroFixed;
            set => checkBoxFixDomZero.Checked = value;
        }
    }

    public enum StatIOStatus
    {
        Neutral,
        Unique,
        NonUnique,
        Error
    }

    /// <summary>
    /// Status of wild levels, e.g. top level, max level.
    /// </summary>
    [Flags]
    public enum LevelStatus
    {
        Neutral = 0,
        /// <summary>
        /// wild level is equal to the current top-level
        /// </summary>
        TopLevel = 1,
        /// <summary>
        /// wild level is higher than the current top-level
        /// </summary>
        NewTopLevel = 2,
        /// <summary>
        /// Max level to apply domesticated levels.
        /// </summary>
        MaxLevelForLevelUp = 4,
        /// <summary>
        /// Max level that can be saved.
        /// </summary>
        MaxLevel = 8,
        /// <summary>
        /// Level too high to be saved, rollover will happen.
        /// </summary>
        UltraMaxLevel = 16
    }

    public enum StatIOInputType
    {
        FinalValueInputType,
        LevelsInputType
    };
}
