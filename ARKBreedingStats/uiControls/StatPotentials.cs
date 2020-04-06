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

            stats = new StatPotential[values.Values.STATS_COUNT];
            for (int s = 0; s < values.Values.STATS_COUNT; s++)
            {
                StatPotential stat = new StatPotential(s, Utils.Precision(s) == 3);
                stats[s] = stat;
            }
            for (int s = 0; s < values.Values.STATS_COUNT; s++)
            {
                int si = values.Values.statsDisplayOrder[s];
                flpStats.Controls.Add(stats[si]);
                flpStats.SetFlowBreak(stats[si], true);
            }
            oldLevels = new int[values.Values.STATS_COUNT];
        }

        public Species Species
        {
            set
            {
                if (value == null || value == selectedSpecies) return;
                selectedSpecies = value;
                for (int s = 0; s < values.Values.STATS_COUNT; s++)
                {
                    stats[s].Visible = selectedSpecies.UsesStat(s);
                }
            }
        }

        public void SetLevels(int[] levelsWild, bool forceUpdate)
        {
            SuspendLayout();
            for (int s = 0; s < values.Values.STATS_COUNT; s++)
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
                for (int s = 0; s < values.Values.STATS_COUNT; s++)
                    stats[s].maxDomLevel = value;
            }
        }

        public int levelGraphMax
        {
            set
            {
                for (int s = 0; s < values.Values.STATS_COUNT; s++)
                    stats[s].levelGraphMax = value;
            }
        }
    }
}
