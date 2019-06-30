using System;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class StatsDisplay : UserControl
    {
        private readonly StatDisplay[] stats;
        private readonly ToolTip tt = new ToolTip();

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
                stats[s].Title = Utils.statName(s, true, glowSpecies);
                stats[s].setNumbers(creature.levelsWild[s], creature.levelsDom[s], creature.valuesBreeding[s], creature.valuesDom[s]);
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
