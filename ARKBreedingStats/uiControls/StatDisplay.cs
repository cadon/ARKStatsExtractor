using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ARKBreedingStats.SpeciesOptions.LevelColorSettings;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.uiControls
{
    public partial class StatDisplay : UserControl
    {
        private readonly bool _isPercent;
        private readonly int _statIndex;
        private readonly ToolTip _tt;
        public int BarMaxLevel = 45;
        private bool _statNamesAreDefault;
        public StatLevelColors LevelColors;

        public StatDisplay()
        {
            InitializeComponent();
        }

        public StatDisplay(int statIndex, bool isPercent, ToolTip tt) : this()
        {
            _statIndex = statIndex;
            _isPercent = isPercent;
            _tt = tt;
            _tt.InitialDelay = 300;
        }

        public void SetCustomStatNames(Dictionary<string, string> customStatNames)
        {
            if (customStatNames == null && _statNamesAreDefault) return;

            _statNamesAreDefault = customStatNames == null;

            labelName.Text = Utils.StatName(_statIndex, true, customStatNames);
            _tt.SetToolTip(labelName, Utils.StatName(_statIndex, false, customStatNames));
        }

        public void SetNumbers(int levelWild, int levelMut, int levelDom, double valueBreeding, double valueDom)
        {
            const int barMaxLength = 164;
            LevelColorBar.SetLevelBar(panelBarWildLevels, LevelColors, barMaxLength, levelWild);
            LevelColorBar.SetLevelBar(panelBarMutLevels, LevelColors, barMaxLength, levelMut, mutationLevel: true);

            if (levelWild < 0)
            {
                labelWildLevel.Text = Loc.S("na");
                labelWildLevel.ForeColor = Color.LightGray;
            }
            else
            {
                labelWildLevel.Text = levelWild.ToString();
                labelWildLevel.ForeColor = SystemColors.ControlText;
            }
            labelMutLevel.Text = levelMut == 0 ? string.Empty : levelMut.ToString();
            labelLevelDom.Text = levelDom.ToString();
            labelBreedingValue.Text = valueBreeding > 0 ? (_isPercent ? Math.Round(100 * valueBreeding, 1).ToString("N1") + " %" : valueBreeding.ToString("N1")) : "?";
            labelDomValue.Text = valueDom > 0 ? (_isPercent ? Math.Round(100 * valueDom, 1).ToString("N1") + " %" : valueDom.ToString("N1")) : "?";
        }

        public bool ShowBars
        {
            set
            {
                panelBarWildLevels.Visible = value;
                panelBarMutLevels.Visible = value;
            }
        }
    }
}
