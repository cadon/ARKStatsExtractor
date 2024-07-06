using System;
using System.Windows.Forms;

namespace ARKBreedingStats.StatsOptions
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
            tt.SetToolTip(CbUseDifferentColorsForOddLevels, "Use different colors for odd levels");
        }

        public void SetStatOptions(StatLevelColors so, bool isNotRoot, StatsOptions<StatLevelColors> parent)
        {
            _statLevelColors = so;
            _parent = parent;
            CbUseDifferentColorsForOddLevels.Checked = so?.UseDifferentColorsForOddLevels == true;
            hueControl.SetValues(so?.LevelGraphRepresentation);
            hueControlOdd.SetValues(so?.LevelGraphRepresentationOdd);
            CbOverrideGraphSettings.Checked = so?.OverrideParent == true;
            CbOverrideGraphSettings.Visible = isNotRoot;
        }

        private void CbOverrideGraphSettings_CheckedChanged(object sender, EventArgs e)
        {
            var overrideStat = CbOverrideGraphSettings.Checked;
            hueControl.Enabled = overrideStat;
            hueControlOdd.Enabled = overrideStat;
            CbUseDifferentColorsForOddLevels.Enabled = overrideStat;
            _statLevelColors.OverrideParent = overrideStat;
            if (overrideStat && _statLevelColors.LevelGraphRepresentation == null)
            {
                _statLevelColors.LevelGraphRepresentation = _parent?.StatOptions[_statIndex].LevelGraphRepresentation.Copy() ?? LevelGraphRepresentation.GetDefaultValue;
                hueControl.SetValues(_statLevelColors.LevelGraphRepresentation);
            }
        }

        private void CbUseDifferentColorsForOddLevels_CheckedChanged(object sender, EventArgs e)
        {
            _statLevelColors.UseDifferentColorsForOddLevels = CbUseDifferentColorsForOddLevels.Checked;
            hueControlOdd.Visible = _statLevelColors.UseDifferentColorsForOddLevels;
            if (_statLevelColors.UseDifferentColorsForOddLevels && _statLevelColors.LevelGraphRepresentationOdd == null)
            {
                _statLevelColors.LevelGraphRepresentationOdd = _parent?.StatOptions[_statIndex].LevelGraphRepresentationOdd?.Copy()
                                                           ?? LevelGraphRepresentation.GetDefaultValue;
                hueControlOdd.SetValues(_statLevelColors.LevelGraphRepresentationOdd);
            }
        }
    }
}
