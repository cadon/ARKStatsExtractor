using System;
using System.Windows.Forms;
using ARKBreedingStats.library;

namespace ARKBreedingStats.uiControls
{
    public partial class StatOptionsControl : UserControl
    {
        private readonly ToolTip _tt;
        private StatOptions _statOptions;
        private int _statIndex;
        private StatsOptions _parent;

        public StatOptionsControl()
        {
            InitializeComponent();
        }

        public StatOptionsControl(string name, int statIndex, ToolTip tt) : this()
        {
            LbStatName.Text = name;
            _statIndex = statIndex;
            _tt = tt;
            hueControl.UpdateTooltips(_tt);
            hueControlOdd.UpdateTooltips(_tt);
            _tt.SetToolTip(CbUseDifferentColorsForOddLevels, "Use different colors for odd levels");
        }

        public void SetStatOptions(StatOptions so, bool isNotRoot, StatsOptions parent)
        {
            _statOptions = so;
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
            _statOptions.OverrideParent = overrideStat;
            if (overrideStat && _statOptions.LevelGraphRepresentation == null)
            {
                _statOptions.LevelGraphRepresentation = _parent?.StatOptions[_statIndex].LevelGraphRepresentation.Copy() ?? LevelGraphRepresentation.GetDefaultValue;
                hueControl.SetValues(_statOptions.LevelGraphRepresentation);
            }
        }

        private void CbUseDifferentColorsForOddLevels_CheckedChanged(object sender, EventArgs e)
        {
            _statOptions.UseDifferentColorsForOddLevels = CbUseDifferentColorsForOddLevels.Checked;
            hueControlOdd.Visible = _statOptions.UseDifferentColorsForOddLevels;
            if (_statOptions.UseDifferentColorsForOddLevels && _statOptions.LevelGraphRepresentationOdd == null)
            {
                _statOptions.LevelGraphRepresentationOdd = _parent?.StatOptions[_statIndex].LevelGraphRepresentationOdd?.Copy()
                                                           ?? LevelGraphRepresentation.GetDefaultValue;
                hueControlOdd.SetValues(_statOptions.LevelGraphRepresentationOdd);
            }
        }
    }
}
