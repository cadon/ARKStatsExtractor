using ARKBreedingStats.species;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class StatPotential : UserControl
    {
        private readonly int _statIndex;
        public int maxDomLevel;
        public int levelGraphMax;
        private readonly bool _percent;
        private readonly ToolTip _tt = new ToolTip();

        public StatPotential()
        {
            InitializeComponent();
            Disposed += (s, e) => _tt.RemoveAll();
        }
        public StatPotential(int stat, bool percent)
        {
            InitializeComponent();
            _statIndex = stat;
            _percent = percent;
            label1.Text = Utils.StatName(_statIndex, true);
        }

        public void SetLevel(Species species, int wildLevel, int mutationLevels)
        {
            if (levelGraphMax > 0)
            {
                SuspendLayout();
                labelWildLevels.Width = 60 + 60 * (wildLevel > levelGraphMax ? levelGraphMax : wildLevel) / levelGraphMax;
                labelImprinting.Width = 60;
                labelDomLevels.Width = 60;
                labelImprinting.Location = new Point(33 + labelWildLevels.Width, 0);
                labelDomLevels.Location = new Point(35 + labelWildLevels.Width + labelImprinting.Width, 0);
                labelWildLevels.Text = StatValueCalculation.CalculateValue(species, _statIndex, wildLevel, mutationLevels, 0, true, 1, 0) * (_percent ? 100 : 1) + (_percent ? "%" : "");
                labelImprinting.Text = StatValueCalculation.CalculateValue(species, _statIndex, wildLevel, mutationLevels, 0, true, 1, 1) * (_percent ? 100 : 1) + (_percent ? "%" : "");
                labelDomLevels.Text = StatValueCalculation.CalculateValue(species, _statIndex, wildLevel, mutationLevels, maxDomLevel, true, 1, 1) * (_percent ? 100 : 1) + (_percent ? "%" : "");
                _tt.SetToolTip(labelWildLevels, labelWildLevels.Text);
                _tt.SetToolTip(labelImprinting, labelImprinting.Text);
                _tt.SetToolTip(labelDomLevels, labelDomLevels.Text);
                ResumeLayout();
            }
        }

        public void SetLocalization()
        {
            label1.Text = Utils.StatName(_statIndex, true);
        }
    }
}
