using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class StatPotential : UserControl
    {
        private int statIndex;
        public int maxDomLevel;
        public int levelGraphMax;
        private bool percent;

        public StatPotential()
        {
            InitializeComponent();
        }
        public StatPotential(int stat, bool percent)
        {
            InitializeComponent();
            statIndex = stat;
            this.percent = percent;
            label1.Text = Utils.statName(stat, true);
        }

        public void setLevel(int speciesIndex, int wildLevel)
        {
            if (levelGraphMax > 0)
            {
                SuspendLayout();
                labelWildLevels.Width = 60 + 68 * (wildLevel > levelGraphMax ? levelGraphMax : wildLevel) / levelGraphMax;
                labelImprinting.Width = 60;
                labelDomLevels.Width = 60;
                labelImprinting.Location = new Point(33 + labelWildLevels.Width, 0);
                labelDomLevels.Location = new Point(35 + labelWildLevels.Width + labelImprinting.Width, 0);
                labelWildLevels.Text = (Stats.calculateValue(speciesIndex, statIndex, wildLevel, 0, true, 1, 0) * (percent ? 100 : 1)).ToString() + (percent ? "%" : "");
                labelImprinting.Text = (Stats.calculateValue(speciesIndex, statIndex, wildLevel, 0, true, 1, 1) * (percent ? 100 : 1)).ToString() + (percent ? "%" : "");
                labelDomLevels.Text = (Stats.calculateValue(speciesIndex, statIndex, wildLevel, maxDomLevel, true, 1, 1) * (percent ? 100 : 1)).ToString() + (percent ? "%" : "");
                ResumeLayout();
            }
        }
    }
}
