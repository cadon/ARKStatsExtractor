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
    public partial class StatDisplay : UserControl
    {
        public bool Percent = false;
        private ToolTip tt = new ToolTip();
        public int barMaxLevel = 45;

        public StatDisplay()
        {
            InitializeComponent();
            tt.InitialDelay = 300;
        }

        public string Title
        {
            set { this.labelName.Text = value; }
        }

        public void setNumbers(int levelWild, int levelDom, double valueBreeding, double valueDom)
        {
            // visualization of wild level
            int barLengthPercentage = levelWild > 0 ? (int)Math.Min(100, Math.Round(100d * levelWild / barMaxLevel)) : 0;
            this.panelBarWildLevels.Width = (int)(164 * barLengthPercentage / 100.0f);
            panelBarWildLevels.BackColor = Utils.getColorFromPercent(barLengthPercentage);
            tt.SetToolTip(panelBarWildLevels, Utils.levelPercentile(levelWild));
            // visualization of dom level
            barLengthPercentage = (int)Math.Min(100, Math.Round(100d * levelDom / barMaxLevel));
            this.panelBarDomLevels.Width = (int)(164 * barLengthPercentage / 100.0f);
            panelBarDomLevels.BackColor = Utils.getColorFromPercent(barLengthPercentage);
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
