using ARKBreedingStats.species;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class StatPotentials : UserControl
    {
        private readonly StatPotential[] _stats;
        private Species _selectedSpecies;
        private readonly int[] _currentLevelsWild = new int[Stats.StatsCount];
        private readonly int[] _currentLevelsMutations = new int[Stats.StatsCount];

        public StatPotentials()
        {
            InitializeComponent();

            _stats = new StatPotential[Stats.StatsCount];
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                StatPotential stat = new StatPotential(s, Stats.IsPercentage(s));
                _stats[s] = stat;
            }
            foreach (var si in Stats.DisplayOrder)
            {
                flpStats.Controls.Add(_stats[si]);
                flpStats.SetFlowBreak(_stats[si], true);
            }
        }

        public Species Species
        {
            set
            {
                if (value == null || value == _selectedSpecies) return;
                _selectedSpecies = value;
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    _stats[s].Visible = _selectedSpecies.UsesStat(s);
                }
            }
        }

        public void SetLevels(int[] levelsWild, int[] levelsMutations, bool forceUpdate)
        {
            SuspendLayout();
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (forceUpdate || _currentLevelsWild[s] != levelsWild[s] || _currentLevelsMutations[s] != levelsMutations[s])
                {
                    _currentLevelsWild[s] = levelsWild[s];
                    _currentLevelsMutations[s] = levelsMutations[s];
                    _stats[s].SetLevel(_selectedSpecies, levelsWild[s], levelsMutations[s]);
                }
            }
            ResumeLayout();
        }

        public int LevelDomMax
        {
            set
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                    _stats[s].maxDomLevel = value;
            }
        }

        public int LevelGraphMax
        {
            set
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                    _stats[s].levelGraphMax = value;
            }
        }

        public void SetLocalization()
        {
            if (_stats == null) return;
            foreach (var s in _stats)
                s.SetLocalization();
        }
    }
}
