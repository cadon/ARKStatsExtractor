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
    public partial class CreatureBox : UserControl
    {
        Creature creature;
        private StatDisplay[] stats;

        public CreatureBox(Creature creature)
        {
            this.creature = creature;
            InitializeComponent();
            stats = new StatDisplay[] { statDisplayHP, statDisplaySt, statDisplayOx, statDisplayFo, statDisplayWe, statDisplayDm, statDisplaySp, statDisplayTo };
            stats[0].Title = "HP";
            stats[1].Title = "St";
            stats[2].Title = "Ox";
            stats[3].Title = "Fo";
            stats[4].Title = "We";
            stats[5].Title = "Dm";
            stats[6].Title = "Sp";
            stats[7].Title = "To";
            stats[5].Percent = true;
            stats[6].Percent = true;
            textBoxName.Text = creature.name;
            for (int s = 0; s < 8; s++) { updateStat(s); }
            updateTitle();
            updateGenderButton();
        }

        public void updateStat(int stat)
        {
            if (stat >= 0 && stat < 8)
            {
                stats[stat].setNumbers(creature.levelsWild[stat], creature.levelsDom[stat], creature.valuesBreeding[stat], creature.valuesDom[stat]);
            }
        }

        private void setName(bool save)
        {
            textBoxName.Visible = false;
            if (save) 
            {
                creature.name = textBoxName.Text;
                updateTitle();
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            textBoxName.SelectAll();
            textBoxName.Visible = true;
            textBoxName.Focus();
        }

        private void textBoxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                // on escape, revert the change.
                textBoxName.Text = creature.name;
                e.Handled = true;
                textBoxName.Visible = false;
            }
        }

        private void buttonSex_Click(object sender, EventArgs e)
        {
            creature.gender = (creature.gender + 1) % 3;
        }

        private void updateGenderButton()
        {
            switch (creature.gender)
            {
                case 1:
                    buttonGender.Text = "♂";
                    break;
                case 2:
                    buttonGender.Text = "♀";
                    break;
                default:
                    buttonGender.Text = "?";
                    break;
            }
        }

        private void updateTitle()
        {
            groupBox1.Text = creature.name + " (" + creature.species + ", Lvl " + creature.level + ")";
        }

        private void textBoxName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Escape)
            {
                // on escape, revert the change.
                textBoxName.Text = creature.name;
                e.Handled = true;
                textBoxName.Visible = false;
            }
        }

        private void textBoxName_Leave(object sender, EventArgs e)
        {
            setName(true);
            textBoxName.Visible = false;
        }
    }
}
