using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using ARKBreedingStats.library;
using ARKBreedingStats.SpeciesOptions.LevelColorSettings;
using ARKBreedingStats.utils;
using Cursors = System.Windows.Forms.Cursors;

namespace ARKBreedingStats.uiControls
{
    public partial class StatIO : UserControl
    {
        /// <summary>
        /// If false (aka creature untamed/wild) display note that stat can be higher after taming.
        /// </summary>
        public bool PostTame;
        private StatIOStatus _status;
        private bool _percent;
        private string _statName;
        private double _breedingValue;
        private StatIOInputType _inputType;
        public event Action<StatIO> LevelChanged;
        public event Action<StatIO> InputValueChanged;
        public int StatIndex;
        private bool _domZeroFixed;
        private readonly ToolTip _tt;
        public int BarMaxLevel = 45;
        private const int MaxBarLength = 335;
        private bool _linkWildMutated;
        private int _wildMutatedSum;
        private readonly Debouncer _levelChangedDebouncer = new Debouncer();
        private StatLevelColors _statLevelColors;
        /// <summary>
        /// True if wild level and mutated level have different effects on the stat value. False for most stats and species.
        /// </summary>
        public bool CustomMutationLevelMultiplier;

        public StatIO()
        {
            InitializeComponent();
            nudLvW.Value = 0;
            nudLvD.Value = 0;
            labelBValue.Text = string.Empty;
            PostTame = true;
            _percent = false;
            _breedingValue = 0;
            groupBox1.Click += groupBox1_Click;
            InputType = _inputType;

            _tt = new ToolTip { InitialDelay = 300 };
            _tt.SetToolTip(checkBoxFixDomZero, "Check to lock to zero (if you never leveled up this stat)");
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double Input
        {
            get => (double)numericUpDownInput.Value * (_percent ? 0.01 : 1);
            set
            {
                if (value < 0)
                {
                    numericUpDownInput.Value = 0;
                    labelFinalValue.Text = Loc.S("Unknown");
                }
                else
                {
                    if (_percent) value *= 100;
                    numericUpDownInput.ValueSave = (decimal)value;
                    labelFinalValue.Text = value.ToString("N1");
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title
        {
            set
            {
                _statName = value;
                groupBox1.Text = value + (Percent ? " [%]" : string.Empty);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int LevelWild
        {
            get => (short)nudLvW.Value;
            set
            {
                int v = value;
                if (v < 0)
                {
                    nudLvW.Value = -1; // value can be unknown if multiple stats are not shown (e.g. wild speed and oxygen)
                    _wildMutatedSum = -1;
                }
                else
                {
                    if (v > nudLvW.Maximum)
                        v = (int)nudLvW.Maximum;
                    _wildMutatedSum = (int)(v + nudLvM.Value);
                    nudLvW.Value = v;
                }
                labelWildLevel.Text = (value < 0 ? "?" : v.ToString());
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int LevelMut
        {
            get => (short)nudLvM.Value;
            set
            {
                labelMutatedLevel.Text = value.ToString();
                if (nudLvW.Value < 0)
                    _wildMutatedSum = -1;
                else
                    _wildMutatedSum = (int)(nudLvW.Value + value);
                nudLvM.Value = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int LevelDom
        {
            get => (short)nudLvD.Value;
            set
            {
                labelDomLevel.Text = value.ToString();
                nudLvD.Value = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double BreedingValue
        {
            get => _breedingValue;
            set
            {
                if (value >= 0)
                {
                    labelBValue.Text = Math.Round((_percent ? 100 : 1) * value, 1).ToString("N1") + (PostTame ? string.Empty : " +*");
                    _breedingValue = value;
                }
                else
                {
                    labelBValue.Text = Loc.S("Unknown");
                }
            }
        }

        /// <summary>
        /// Indicates whether this stat is expressed as a percentage.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Percent
        {
            get => _percent;
            set
            {
                _percent = value;
                Title = _statName;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        private LevelColorStatusFlags.LevelStatus _topLevel;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LevelColorStatusFlags.LevelStatus TopLevel
        {
            get => _topLevel;
            set
            {
                if (_topLevel == value) return;
                _topLevel = value;

                labelWildLevel.BackColor = Color.Transparent;
                labelMutatedLevel.BackColor = Color.Transparent;
                _tt.SetToolTip(labelWildLevel, null);
                _tt.SetToolTip(labelMutatedLevel, null);

                if (_topLevel == LevelColorStatusFlags.LevelStatus.Neutral) return;

                if (_topLevel.HasFlag(LevelColorStatusFlags.LevelStatus.TopLevel))
                {
                    labelWildLevel.BackColor = Color.LightGreen;
                    _tt.SetToolTip(labelWildLevel, Loc.S("topLevel"));
                }
                else if (_topLevel.HasFlag(LevelColorStatusFlags.LevelStatus.NewTopLevel))
                {
                    labelWildLevel.BackColor = Color.Gold;
                    _tt.SetToolTip(labelWildLevel, Loc.S("newTopLevel"));
                }

                if (_topLevel.HasFlag(LevelColorStatusFlags.LevelStatus.MaxLevelForLevelUp))
                {
                    labelWildLevel.BackColor = Color.DeepSkyBlue;
                    _tt.SetToolTip(labelWildLevel, Loc.S("maxLevelForLevelUp"));
                }
                else if (_topLevel.HasFlag(LevelColorStatusFlags.LevelStatus.MaxLevel))
                {
                    labelWildLevel.BackColor = Color.Orange;
                    _tt.SetToolTip(labelWildLevel, Loc.S("maxLevelSaved"));
                }
                else if (_topLevel.HasFlag(LevelColorStatusFlags.LevelStatus.UltraMaxLevel))
                {
                    labelWildLevel.BackColor = Color.LightCoral;
                    _tt.SetToolTip(labelWildLevel, Loc.S("ultraMaxLevel"));
                }

                if (_topLevel.HasFlag(LevelColorStatusFlags.LevelStatus.NewMutation))
                {
                    labelMutatedLevel.BackColor = Color.Gold;
                    _tt.SetToolTip(labelMutatedLevel, Loc.S("new mutation"));
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowBarAndLock
        {
            set
            {
                panelBarWildLevels.Visible = value;
                panelBarDomLevels.Visible = value;
                checkBoxFixDomZero.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
            TopLevel = LevelColorStatusFlags.LevelStatus.Neutral;
            nudLvW.Value = 0;
            nudLvM.Value = 0;
            nudLvD.Value = 0;
            labelWildLevel.Text = "0";
            labelMutatedLevel.Text = "0";
            labelDomLevel.Text = "0";
            labelFinalValue.Text = "0";
            labelBValue.Text = string.Empty;
        }

        private void numLvW_ValueChanged(object sender, EventArgs e)
        {
            SetLevelBar(panelBarWildLevels, (int)nudLvW.Value);
            _tt.SetToolTip(panelBarWildLevels, Utils.LevelPercentile((int)nudLvW.Value));

            if (_linkWildMutated && _wildMutatedSum != -1)
            {
                nudLvM.ValueSave = Math.Max(0, _wildMutatedSum - nudLvW.Value);
            }

            if (_inputType != StatIOInputType.FinalValueInputType)
                LevelChangedDebouncer();
        }

        private void nudLvM_ValueChanged(object sender, EventArgs e)
        {
            SetLevelBar(panelBarMutLevels, (int)nudLvM.Value, false, true);

            if (_linkWildMutated && _wildMutatedSum != -1)
            {
                nudLvW.ValueSave = Math.Max(0, _wildMutatedSum - nudLvM.Value);
            }

            if (_inputType != StatIOInputType.FinalValueInputType)
                LevelChangedDebouncer();
        }

        private void numLvD_ValueChanged(object sender, EventArgs e)
        {
            SetLevelBar(panelBarDomLevels, (int)nudLvD.Value, false);

            if (_inputType != StatIOInputType.FinalValueInputType)
                LevelChangedDebouncer();
        }

        private void SetLevelBar(Panel panel, int level, bool useCustomOdd = true, bool mutationLevel = false) =>
            LevelColorBar.SetLevelBar(panel, _statLevelColors, MaxBarLength, level, useCustomOdd, mutationLevel);

        private void LevelChangedDebouncer() => _levelChangedDebouncer.Debounce(200, () => LevelChanged?.Invoke(this), Dispatcher.CurrentDispatcher);

        private void numericUpDownInput_ValueChanged(object sender, EventArgs e)
        {
            if (InputType == StatIOInputType.FinalValueInputType)
                _levelChangedDebouncer.Debounce(200, FireStatValueChanged, Dispatcher.CurrentDispatcher);
        }

        private void FireStatValueChanged() => InputValueChanged?.Invoke(this);

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            n?.Select(0, n.Text.Length);
        }

        private void groupBox1_Click(object sender, EventArgs e) => OnClick(e);

        private void labelBValue_Click(object sender, EventArgs e) => OnClick(e);

        private void labelDomLevel_Click(object sender, EventArgs e) => OnClick(e);

        private void panelFinalValue_Click(object sender, EventArgs e) => OnClick(e);

        private void panelBar_Click(object sender, EventArgs e) => OnClick(e);

        private void labelWildLevel_Click(object sender, EventArgs e)
        {
            OnClick(e);
            if (CustomMutationLevelMultiplier) return;

            var levelDelta = LevelDeltaMutationShift(LevelMut);
            if (levelDelta <= 0) return;
            LevelWild += levelDelta;
            LevelMut -= levelDelta;
            LevelChangedDebouncer();
        }

        private void labelMutatedLevel_Click(object sender, EventArgs e)
        {
            OnClick(e);
            if (CustomMutationLevelMultiplier) return;

            var levelDelta = LevelDeltaMutationShift(LevelWild);
            if (levelDelta <= 0) return;
            LevelWild -= levelDelta;
            LevelMut += levelDelta;
            LevelChangedDebouncer();
        }

        private int LevelDeltaMutationShift(int remainingLevel)
        {
            var levelDelta = Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Shift) ? 10 : 2;
            if (remainingLevel < levelDelta) levelDelta = (remainingLevel / 2) * 2;
            return levelDelta;
        }

        private void checkBoxFixDomZero_CheckedChanged(object sender, EventArgs e)
        {
            _domZeroFixed = checkBoxFixDomZero.Checked;
            checkBoxFixDomZero.Image = (_domZeroFixed ? Properties.Resources.locked : Properties.Resources.unlocked);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DomLevelLockedZero
        {
            get => _domZeroFixed;
            set => checkBoxFixDomZero.Checked = value;
        }

        /// <summary>
        /// If true, the control tries to keep the sum of the wild and mutated levels equal.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool LinkWildMutated
        {
            set
            {
                _linkWildMutated = value;
                _wildMutatedSum = (int)(nudLvW.Value + nudLvM.Value);
            }
        }

        public void SetStatOptions(StatLevelColors so)
        {
            if (_statLevelColors == so) return;
            _statLevelColors = so;
            if (nudLvW.Value > 0)
                SetLevelBar(panelBarWildLevels, (int)nudLvW.Value);
            if (nudLvD.Value > 0)
                SetLevelBar(panelBarDomLevels, (int)nudLvD.Value, false);
            if (nudLvM.Value > 0)
                SetLevelBar(panelBarMutLevels, (int)nudLvM.Value, false, true);
        }
    }

    public enum StatIOStatus
    {
        Neutral,
        Unique,
        NonUnique,
        Error
    }

    public enum StatIOInputType
    {
        FinalValueInputType,
        LevelsInputType
    };
}
