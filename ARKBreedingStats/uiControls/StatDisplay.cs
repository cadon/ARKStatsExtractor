using System;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class StatDisplay : UserControl
    {
        public bool Percent = false;
        private ToolTip tt = new ToolTip();
        public int barMaxLevel = 45;
        public int statIndex;
        private bool? isGlowSpeciesStat;

        public StatDisplay()
        {
            InitializeComponent();
            tt.InitialDelay = 300;
        }

        public bool GlowSpecies
        {
            set
            {
                if (isGlowSpeciesStat != value)
                {
                    isGlowSpeciesStat = value;
                    labelName.Text = Utils.StatName(statIndex, true, isGlowSpeciesStat.Value);
                    tt.SetToolTip(labelName, Utils.StatName(statIndex, false, isGlowSpeciesStat.Value));
                }
            }
        }

        public void setNumbers(int levelWild, int levelDom, double valueBreeding, double valueDom)
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

            // if stat is not used, e.g. craftingspeed
            if (levelWild == 0 && levelDom == 0 && valueBreeding == 0 && valueDom == 0)
            {
                labelWildLevel.Text = "n/a";
                labelWildLevel.ForeColor = Color.LightGray;
                labelLevelDom.Text = string.Empty;
                labelBreedingValue.Text = string.Empty;
                labelDomValue.Text = string.Empty;
            }
            else
            {
                if (levelWild < 0)
                {
                    labelWildLevel.Text = "n/a";
                    labelWildLevel.ForeColor = Color.LightGray;
                }
                else
                {
                    labelWildLevel.Text = levelWild.ToString();
                    labelWildLevel.ForeColor = SystemColors.ControlText;
                }
                labelLevelDom.Text = levelDom.ToString();
                labelBreedingValue.Text = valueBreeding > 0 ? (Percent ? Math.Round(100 * valueBreeding, 1).ToString("N1") + " %" : valueBreeding.ToString("N1")) : "?";
                labelDomValue.Text = valueDom > 0 ? (Percent ? Math.Round(100 * valueDom, 1).ToString("N1") + " %" : valueDom.ToString("N1")) : "?";
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
