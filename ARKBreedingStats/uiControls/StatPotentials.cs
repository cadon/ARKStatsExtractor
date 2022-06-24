using ARKBreedingStats.species;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class StatPotentials : UserControl
    {
        private readonly StatPotential[] stats;
        private Species selectedSpecies;
        private readonly int[] oldLevels;

        public StatPotentials()
        {
            InitializeComponent();

            stats = new StatPotential[Stats.StatsCount];
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                StatPotential stat = new StatPotential(s, Utils.Precision(s) == 3);
                stats[s] = stat;
            }
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                int si = Stats.DisplayOrder[s];
                flpStats.Controls.Add(stats[si]);
                flpStats.SetFlowBreak(stats[si], true);
            }
            oldLevels = new int[Stats.StatsCount];
        }

        public Species Species
        {
            set
            {
                if (value == null || value == selectedSpecies) return;
                selectedSpecies = value;
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    stats[s].Visible = selectedSpecies.UsesStat(s);
                }
            }
        }

        public void SetLevels(int[] levelsWild, bool forceUpdate)
        {
            SuspendLayout();
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (forceUpdate || oldLevels[s] != levelsWild[s])
                {
                    oldLevels[s] = levelsWild[s];
                    stats[s].SetLevel(selectedSpecies, levelsWild[s]);
                }
            }
            ResumeLayout();
        }

        public int levelDomMax
        {
            set
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                    stats[s].maxDomLevel = value;
            }
        }

        public int levelGraphMax
        {
            set
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                    stats[s].levelGraphMax = value;
            }
        }
    }
}
