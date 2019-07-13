using ARKBreedingStats.species;
using System;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class StatsDisplay : UserControl
    {
        private readonly StatDisplay[] stats;
        private readonly ToolTip tt = new ToolTip();
        private static int[] displayedStats = new int[] {
                                                        (int)StatNames.Health,
                                                        (int)StatNames.Stamina,
                                                        (int)StatNames.Oxygen,
                                                        (int)StatNames.Food,
                                                        (int)StatNames.Weight,
                                                        (int)StatNames.MeleeDamageMultiplier,
                                                        (int)StatNames.SpeedMultiplier,
                                                        (int)StatNames.CraftingSpeedMultiplier
                                                        };

        public StatsDisplay()
        {
            InitializeComponent();
            stats = new[] { statDisplayHP, statDisplaySt, statDisplayOx, statDisplayFo, statDisplayWe, statDisplayDm, statDisplaySp, statDisplayTo };
            for (int s = 0; s < 8; s++)
                stats[s].Title = Utils.statName(s, true);
            statDisplayDm.Percent = true;
            statDisplaySp.Percent = true;
            statDisplayTo.ShowBars = false;

            // tooltips
            tt.SetToolTip(labelSex, "Sex of the Creature");
            tt.SetToolTip(labelStatHeader, "Wild-levels, Domesticated-levels, Value that is inherited, Current Value of the Creature");

            Disposed += StatsDisplay_Disposed;
        }

        public void SetCreatureValues(Creature creature)
        {
            SuspendLayout();

            bool glowSpecies = Values.V.IsGlowSpecies(creature.Species.name);
            for (int s = 0; s < 8; s++)
            {
                int si = displayedStats[s];
                stats[s].Title = Utils.statName(si, true, glowSpecies);
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
