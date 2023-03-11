using ARKBreedingStats.Library;
using System;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class StatsDisplay : UserControl
    {
        private readonly StatDisplay[] stats;
        private readonly ToolTip tt = new ToolTip();
        private readonly int[] displayedStats;
        private readonly int displayedStatsCount;

        public StatsDisplay()
        {
            InitializeComponent();
            displayedStats = new int[] {Stats.Health,
                                        Stats.Stamina,
                                        Stats.Oxygen,
                                        Stats.Food,
                                        Stats.Weight,
                                        Stats.MeleeDamageMultiplier,
                                        Stats.SpeedMultiplier,
                                        Stats.CraftingSpeedMultiplier
                                        };

            displayedStatsCount = displayedStats.Length;
            stats = new StatDisplay[displayedStatsCount];

            for (int s = 0; s < displayedStatsCount; s++)
            {
                int si = displayedStats[s];
                StatDisplay sd = new StatDisplay(si, Utils.Precision(si) == 3);
                stats[s] = sd;

                sd.Location = new System.Drawing.Point(3, 19 + s * 22);
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

            for (int s = 0; s < displayedStatsCount; s++)
            {
                int si = displayedStats[s];
                stats[s].SetCustomStatNames(creature.Species?.statNames);
                stats[s].SetNumbers(creature.levelsWild[si], creature.levelsDom[si], creature.valuesBreeding[si], creature.valuesDom[si]);
            }

            labelSex.Text = Utils.SexSymbol(creature.sex);

            ResumeLayout();
        }

        public int BarMaxLevel
        {
            set
            {
                for (int s = 0; s < displayedStatsCount; s++)
                {
                    stats[s].barMaxLevel = value;
                }
            }
        }

        public void Clear()
        {
            for (int s = 0; s < displayedStatsCount; s++)
            {
                stats[s].SetNumbers(0, 0, 0, 0);
            }
            labelSex.Text = "";
        }

        private void StatsDisplay_Disposed(object sender, EventArgs e)
        {
            tt.RemoveAll();
        }
    }
}
