using System;
using System.Windows.Forms;

namespace ARKBreedingStats.StatsOptions.LevelColorSettings
{
    public partial class StatLevelGraphOptionsControl : UserControl
    {
        private StatLevelColors _statLevelColors;
        private readonly int _statIndex;
        private StatsOptions<StatLevelColors> _parent;

        public StatLevelGraphOptionsControl()
        {
            InitializeComponent();
        }

        public StatLevelGraphOptionsControl(string name, int statIndex, ToolTip tt) : this()
        {
            LbStatName.Text = name;
            _statIndex = statIndex;
            hueControl.UpdateTooltips(tt);
            hueControlOdd.UpdateTooltips(tt);
            tt.SetToolTip(CbUseDifferentColorsForOddLevels, "Use different colors for odd wild levels");
            tt.SetToolTip(CbUseDifferentColorsForMutationLevels, "Use different colors for mutation levels");
        }

        public void SetStatOptions(StatLevelColors so, bool isNotRoot, StatsOptions<StatLevelColors> parent)
        {
            _statLevelColors = so;
            _parent = parent;
            CbUseDifferentColorsForOddLevels.Checked = so?.UseDifferentColorsForOddLevels == true;
            CbUseDifferentColorsForMutationLevels.Checked = so?.UseDifferentColorsForMutationLevels == true;
            hueControl.SetValues(so?.LevelGraphRepresentation);
            hueControlOdd.SetValues(so?.LevelGraphRepresentationOdd);
            HueControlMutations.SetValues(so?.LevelGraphRepresentationMutation);
            CbOverrideGraphSettings.Checked = !isNotRoot || so?.OverrideParent == true;
            CbOverrideGraphSettings.Visible = isNotRoot;
        }

        private void CbOverrideGraphSettings_CheckedChanged(object sender, EventArgs e)
        {
            var overrideStat = CbOverrideGraphSettings.Checked;
            hueControl.Enabled = overrideStat;
            hueControlOdd.Enabled = overrideStat;
            CbUseDifferentColorsForOddLevels.Enabled = overrideStat;
            CbUseDifferentColorsForMutationLevels.Enabled = overrideStat;
            _statLevelColors.OverrideParent = overrideStat;
            if (overrideStat && _statLevelColors.LevelGraphRepresentation == null)
            {
                _statLevelColors.LevelGraphRepresentation = _parent?.StatOptions?[_statIndex]?.LevelGraphRepresentation?.Copy()
                                                            ?? LevelGraphRepresentation.GetDefaultValue;
                hueControl.SetValues(_statLevelColors.LevelGraphRepresentation);
            }
        }

        private void CbUseDifferentColorsForOddLevels_CheckedChanged(object sender, EventArgs e)
        {
            _statLevelColors.UseDifferentColorsForOddLevels = CbUseDifferentColorsForOddLevels.Checked;
            hueControlOdd.Visible = _statLevelColors.UseDifferentColorsForOddLevels;
            if (_statLevelColors.UseDifferentColorsForOddLevels && _statLevelColors.LevelGraphRepresentationOdd == null)
            {
                _statLevelColors.LevelGraphRepresentationOdd = _parent?.StatOptions?[_statIndex]?.LevelGraphRepresentationOdd?.Copy()
                                                           ?? LevelGraphRepresentation.GetDefaultValue;
                hueControlOdd.SetValues(_statLevelColors.LevelGraphRepresentationOdd);
            }
        }

        private void CbUseDifferentColorsForMutationLevels_CheckedChanged(object sender, EventArgs e)
        {
            _statLevelColors.UseDifferentColorsForMutationLevels = CbUseDifferentColorsForMutationLevels.Checked;
            HueControlMutations.Visible = _statLevelColors.UseDifferentColorsForMutationLevels;
            if (_statLevelColors.UseDifferentColorsForMutationLevels && _statLevelColors.LevelGraphRepresentationMutation == null)
            {
                _statLevelColors.LevelGraphRepresentationMutation = _parent?.StatOptions?[_statIndex]?.LevelGraphRepresentationMutation?.Copy()
                                                               ?? LevelGraphRepresentation.GetDefaultValue;
                HueControlMutations.SetValues(_statLevelColors.LevelGraphRepresentationMutation);
            }
        }
    }
}
