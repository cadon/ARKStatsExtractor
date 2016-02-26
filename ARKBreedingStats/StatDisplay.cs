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
        public StatDisplay()
        {
            InitializeComponent();
        }

        public string Title
        {
            set { this.labelName.Text = value; }
        }

        public void setNumbers(int levelWild, int levelDom, double valueBreeding, double valueDom)
        {
            // visualization of wild level
            int barLengthPercentage = (int)Math.Max(1, (levelWild * (100.0f / Properties.Settings.Default.BarMaximum)));
            this.panelBar.Width = (int)(164 * barLengthPercentage / 100.0f);
            panelBar.BackColor = Utils.getColorFromPercent(barLengthPercentage);
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
            labelBreedingValue.Text = (Percent ? Math.Round(100 * valueBreeding, 1).ToString() + " %" : valueBreeding.ToString());
            labelDomValue.Text = (Percent ? Math.Round(100 * valueDom, 1).ToString() + " %" : valueDom.ToString());
        }

        public bool ShowBar
        {
            set
            {
                panelBar.Visible = value;
            }
        }

    }
}
