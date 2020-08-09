using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class StatDisplay : UserControl
    {
        private readonly bool _isPercent;
        private readonly int _statIndex;
        private ToolTip tt = new ToolTip();
        public int barMaxLevel = 45;
        private bool _statNamesAreDefault;

        public StatDisplay()
        {
            InitializeComponent();
            tt.InitialDelay = 300;
        }

        public StatDisplay(int statIndex, bool isPercent) : this()
        {
            _statIndex = statIndex;
            _isPercent = isPercent;
        }

        public void SetCustomStatNames(Dictionary<string, string> customStatNames)
        {
            if (customStatNames == null && _statNamesAreDefault) return;

            _statNamesAreDefault = customStatNames == null;

            labelName.Text = Utils.StatName(_statIndex, true, customStatNames);
            tt.SetToolTip(labelName, Utils.StatName(_statIndex, false, customStatNames));
        }

        public void SetNumbers(int levelWild, int levelDom, double valueBreeding, double valueDom)
        {
            // visualization of wild level
            int barLengthPercentage = levelWild > 0 ? (int)Math.Min(100, Math.Round(100d * levelWild / barMaxLevel)) : 0;
            panelBarWildLevels.Width = (int)(164 * barLengthPercentage / 100.0f);
            panelBarWildLevels.BackColor = Utils.GetColorFromPercent(barLengthPercentage);
            tt.SetToolTip(panelBarWildLevels, Utils.LevelPercentile(levelWild));
            // visualization of dom level
            barLengthPercentage = (int)Math.Min(100, Math.Round(100d * levelDom / barMaxLevel));
            panelBarDomLevels.Width = (int)(164 * barLengthPercentage / 100.0f);
            panelBarDomLevels.BackColor = Utils.GetColorFromPercent(barLengthPercentage);

            // if stat is not used, e.g. crafting speed
            if (levelWild == 0 && levelDom == 0 && valueBreeding == 0 && valueDom == 0)
            {
                labelWildLevel.Text = Loc.S("na");
                labelWildLevel.ForeColor = Color.LightGray;
                labelLevelDom.Text = string.Empty;
                labelBreedingValue.Text = string.Empty;
                labelDomValue.Text = string.Empty;
            }
            else
            {
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
                labelLevelDom.Text = levelDom.ToString();
                labelBreedingValue.Text = valueBreeding > 0 ? (_isPercent ? Math.Round(100 * valueBreeding, 1).ToString("N1") + " %" : valueBreeding.ToString("N1")) : "?";
                labelDomValue.Text = valueDom > 0 ? (_isPercent ? Math.Round(100 * valueDom, 1).ToString("N1") + " %" : valueDom.ToString("N1")) : "?";
            }
        }

        public bool ShowBars
        {
            set
            {
                panelBarWildLevels.Visible = value;
                panelBarDomLevels.Visible = value;
            }
        }
    }
}
