using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.raising
{
    public partial class ParentStatValues : UserControl
    {
        public ParentStatValues()
        {
            InitializeComponent();
            labelM.TextAlign = ContentAlignment.MiddleRight;
            labelF.TextAlign = ContentAlignment.MiddleRight;
        }

        public string StatName
        {
            set => label1.Text = value;
        }

        internal void SetValues(double motherValue = -1, double fatherValue = -1, int highlight = 0, int bestLevel = -1, int bestLevelPercent = 0)
        {
            labelM.Text = motherValue >= 0 ? motherValue.ToString("N1") : "-";
            labelF.Text = fatherValue >= 0 ? fatherValue.ToString("N1") : "-";
            lbBest.Text = motherValue > fatherValue ? labelM.Text : labelF.Text;
            lbBestLevel.Text = bestLevel >= 0 ? $" [Lv {bestLevel}]" : "";
            lbBestLevel.BackColor = bestLevel >= 0 ? Utils.GetColorFromPercent(bestLevelPercent, 0.5) : SystemColors.Window;
            switch (highlight)
            {
                case 1:
                    labelM.ForeColor = Color.DarkBlue;
                    labelF.ForeColor = Color.DarkRed;
                    break;
                case 2:
                    labelF.ForeColor = Color.DarkBlue;
                    labelM.ForeColor = Color.DarkRed;
                    break;
                default:
                    labelM.ForeColor = Color.Black;
                    labelF.ForeColor = Color.Black;
                    break;
            }
        }
    }
}
