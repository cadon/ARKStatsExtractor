using System;
using ARKBreedingStats.species;
using System.Drawing;
using System.Windows.Forms;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.uiControls
{
    public partial class StatPotential : UserControl
    {
        private readonly int _statIndex;
        public int MaxDomLevel;
        public int LevelGraphMax;
        private readonly bool _percent;
        private readonly ToolTip _tt = new();

        public StatPotential()
        {
            InitializeComponent();
            Disposed += (s, e) => _tt.RemoveAllAndDispose();
            labelWildLevels.SetBackColorAndAccordingForeColor(UiColors.Current.SimilarityGood);
            labelImprinting.SetBackColorAndAccordingForeColor(UiColors.Current.SimilarityOk);
            labelDomLevels.SetBackColorAndAccordingForeColor(UiColors.Current.SimilarityPoor);
        }
        public StatPotential(int stat, bool percent) : this()
        {
            _statIndex = stat;
            _percent = percent;
            label1.Text = Utils.StatName(_statIndex, true);
        }

        public void SetLevel(Species species, int wildLevel, int mutationLevels)
        {
            if (LevelGraphMax <= 0) return;
            var wildMutSumLevel = Math.Min(Math.Max(0, wildLevel + mutationLevels), LevelGraphMax);
            if (_statIndex == Stats.Torpidity) wildMutSumLevel /= 7;
            var widthForLevelsBase = (Width - label1.Width) / 4;
            labelWildLevels.Width = widthForLevelsBase + widthForLevelsBase * wildMutSumLevel / LevelGraphMax;
            labelImprinting.Width = widthForLevelsBase;
            labelDomLevels.Width = widthForLevelsBase;
            if (wildLevel < 0 || mutationLevels < 0)
            {
                labelWildLevels.Text = "?";
                labelImprinting.Text = "?";
                labelDomLevels.Text = "?";
            }
            else
            {
                var suffix = _percent ? "%" : string.Empty;
                labelWildLevels.Text = Math.Round(StatValueCalculation.CalculateValue(species, _statIndex, wildLevel, mutationLevels, 0, true, 1, 0) * (_percent ? 100 : 1), 2) + suffix;
                labelImprinting.Text = Math.Round(StatValueCalculation.CalculateValue(species, _statIndex, wildLevel, mutationLevels, 0, true, 1, 1) * (_percent ? 100 : 1), 2) + suffix;
                labelDomLevels.Text = Math.Round(StatValueCalculation.CalculateValue(species, _statIndex, wildLevel, mutationLevels, MaxDomLevel, true, 1, 1) * (_percent ? 100 : 1), 2) + suffix;
            }
            _tt.SetToolTip(labelWildLevels, labelWildLevels.Text);
            _tt.SetToolTip(labelImprinting, labelImprinting.Text);
            _tt.SetToolTip(labelDomLevels, labelDomLevels.Text);
        }

        public void SetLocalization()
        {
            label1.Text = Utils.StatName(_statIndex, true);
        }
    }
}
