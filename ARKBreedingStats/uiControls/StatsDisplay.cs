using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class StatsDisplay : UserControl
    {
        private readonly StatDisplay[] stats;
        private readonly ToolTip tt = new ToolTip();
        private static int[] displayedStats;
        private static int displayedStatsCount;

        public StatsDisplay()
        {
            InitializeComponent();
            displayedStats = new int[] {(int)StatNames.Health,
                                        (int)StatNames.Stamina,
                                        (int)StatNames.Oxygen,
                                        (int)StatNames.Food,
                                        (int)StatNames.Weight,
                                        (int)StatNames.MeleeDamageMultiplier,
                                        (int)StatNames.SpeedMultiplier,
                                        (int)StatNames.CraftingSpeedMultiplier
                                        };

            displayedStatsCount = displayedStats.Length;
            stats = new StatDisplay[displayedStatsCount];

            for (int s = 0; s < displayedStatsCount; s++)
            {
                int si = displayedStats[s];
                StatDisplay sd = new StatDisplay
                {
                    statIndex = si,
                    Percent = Utils.precision(si) == 3
                };
                stats[s] = sd;

                sd.Location = new System.Drawing.Point(3, 19 + s * 23);
                Controls.Add(sd);
            }

            // tooltips
            tt.SetToolTip(labelSex, "Sex of the Creature");
            tt.SetToolTip(labelStatHeader, "Wild-levels, Domesticated-levels, Value that is inherited, Current Value of the Creature");

            Disposed += StatsDisplay_Disposed;
        }

        public void SetCreatureValues(Creature creature)
        {
            SuspendLayout();

            bool glowSpecies = creature.Species.IsGlowSpecies;
            for (int s = 0; s < displayedStats.Length; s++)
            {
                int si = displayedStats[s];
                stats[s].GlowSpecies = glowSpecies;
                stats[s].setNumbers(creature.levelsWild[si], creature.levelsDom[si], creature.valuesBreeding[si], creature.valuesDom[si]);
            }

            labelSex.Text = Utils.sexSymbol(creature.sex);

            ResumeLayout();
        }

        public int BarMaxLevel
        {
            set
            {
                for (int s = 0; s < 8; s++)
                {
                    stats[s].barMaxLevel = value;
                }
            }
        }

        public void Clear()
        {
            for (int s = 0; s < 8; s++)
            {
                stats[s].setNumbers(0, 0, 0, 0);
            }
            labelSex.Text = "";
        }

        private void StatsDisplay_Disposed(object sender, EventArgs e)
        {
            tt.RemoveAll();
        }
    }
}
