using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{

    public partial class PedigreeCreature : UserControl
    {
        private Creature creature;
        public event Pedigree.CreatureChangedEventHandler CreatureChanged;

        public PedigreeCreature()
        {
            InitC();
        }
        private void InitC()
        {
            InitializeComponent();
            ToolTip tt = new ToolTip();
            tt.SetToolTip(labelHP, "Health");
            tt.SetToolTip(labelSt, "Stamina");
            tt.SetToolTip(labelOx, "Oxygen");
            tt.SetToolTip(labelFo, "Food");
            tt.SetToolTip(labelWe, "Weight");
            tt.SetToolTip(labelDm, "Melee Damage");
            tt.SetToolTip(labelSp, "Speed");
        }

        public PedigreeCreature(Creature creature)
        {
            InitC();
            setCreature(creature);
        }
        public void setCreature(Creature creature)
        {
            this.creature = creature;
            groupBox1.Text = creature.name;
            labelHP.Text = creature.levelsWild[0].ToString();
            labelSt.Text = creature.levelsWild[1].ToString();
            labelOx.Text = creature.levelsWild[2].ToString();
            labelFo.Text = creature.levelsWild[3].ToString();
            labelWe.Text = creature.levelsWild[4].ToString();
            labelDm.Text = creature.levelsWild[5].ToString();
            labelSp.Text = creature.levelsWild[6].ToString();
            labelHP.BackColor = Utils.getColorFromPercent((int)(creature.levelsWild[0] * 2.5), (creature.topBreedingStats[0] ? 0.2 : 0.7));
            labelSt.BackColor = Utils.getColorFromPercent((int)(creature.levelsWild[1] * 2.5), (creature.topBreedingStats[1] ? 0.2 : 0.7));
            labelOx.BackColor = Utils.getColorFromPercent((int)(creature.levelsWild[2] * 2.5), (creature.topBreedingStats[2] ? 0.2 : 0.7));
            labelFo.BackColor = Utils.getColorFromPercent((int)(creature.levelsWild[3] * 2.5), (creature.topBreedingStats[3] ? 0.2 : 0.7));
            labelWe.BackColor = Utils.getColorFromPercent((int)(creature.levelsWild[4] * 2.5), (creature.topBreedingStats[4] ? 0.2 : 0.7));
            labelDm.BackColor = Utils.getColorFromPercent((int)(creature.levelsWild[5] * 2.5), (creature.topBreedingStats[5] ? 0.2 : 0.7));
            labelSp.BackColor = Utils.getColorFromPercent((int)(creature.levelsWild[6] * 2.5), (creature.topBreedingStats[6] ? 0.2 : 0.7));
            switch (creature.gender)
            {
                case Gender.Female:
                    labelGender.Text = "♀";
                    labelGender.BackColor = Color.FromArgb(255, 230, 255);
                    break;
                case Gender.Male:
                    labelGender.Text = "♂";
                    labelGender.BackColor = Color.FromArgb(220, 235, 255);
                    break;
                default:
                    labelGender.Text = "?";
                    labelGender.BackColor = SystemColors.Control;
                    break;
            }
        }
        public bool highlight { set { panel1.Visible = value; } }

        private void PedigreeCreature_Click(object sender, EventArgs e)
        {
            CreatureChanged(this.creature);
        }

        private void element_Click(object sender, EventArgs e)
        {
            PedigreeCreature_Click(sender, e);
        }
    }
}
