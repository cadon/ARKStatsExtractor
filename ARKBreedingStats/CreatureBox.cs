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
        private NumericUpDown[] numUDLevelsDom;
        public delegate void ChangedEventHandler(object sender, int listViewIndex, Creature creature);
        public event ChangedEventHandler Changed;
        public int indexInListView;

        public CreatureBox()
        {
            initializeVars();
        }

        public CreatureBox(Creature creature)
        {
            initializeVars();
            setCreature(creature);
        }

        private void initializeVars()
        {
            InitializeComponent();
            this.creature = null;
            stats = new StatDisplay[] { statDisplayHP, statDisplaySt, statDisplayOx, statDisplayFo, statDisplayWe, statDisplayDm, statDisplaySp, statDisplayTo };
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.labelHeaderDomLevelSet, "Set the spend domesticated Levels here");
            tt.SetToolTip(labelGender, "Gender of the Cretaure");
            tt.SetToolTip(labelStatHeader, "Wild-levels, Domesticated-levels, Value that is inherited, Current Value of the Creature");
            tt.SetToolTip(buttonEdit, "Edit");
        }

        public void setCreature(Creature creature)
        {
            this.creature = creature;
            numUDLevelsDom = new NumericUpDown[] { numericUpDown1, numericUpDown2, numericUpDown3, numericUpDown4, numericUpDown5, numericUpDown6, numericUpDown7 };
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
            statDisplayTo.ShowBar = false;
            textBoxName.Text = creature.name;
            for (int s = 0; s < 8; s++) { updateStat(s); }
            updateLabel();
        }

        public void updateStat(int stat)
        {
            if (stat >= 0 && stat < 8)
            {
                stats[stat].setNumbers(creature.levelsWild[stat], creature.levelsDom[stat], creature.valuesBreeding[stat], creature.valuesDom[stat]);
            }
        }

        public void buttonEdit_Click(object sender, EventArgs e)
        {
            if (creature != null)
            {
                if (panel1.Visible)
                {
                    closeSettings(false);
                }
                else
                {
                    textBoxName.Text = creature.name;
                    textBoxOwner.Text = creature.owner;
                    switch (creature.gender)
                    {
                        case Gender.Female:
                            checkBoxFemale.Checked = true;
                            break;
                        case Gender.Male:
                            checkBoxMale.Checked = true;
                            break;
                        default:
                            checkBoxFemale.Checked = false;
                            checkBoxMale.Checked = false;
                            break;
                    }
                    textBoxName.SelectAll();
                    textBoxName.Focus();
                    panel1.Visible = true;
                    for (int s = 0; s < 7; s++)
                    {
                        numUDLevelsDom[s].Value = creature.levelsDom[s];
                    }
                }
            }
        }

        private void updateLabel()
        {
            switch (creature.gender)
            {
                case Gender.Male:
                    labelGender.Text = "♂";
                    break;
                case Gender.Female:
                    labelGender.Text = "♀";
                    break;
                default:
                    labelGender.Text = "?";
                    break;
            }
            groupBox1.Text = creature.name + " (" + creature.species + ", Lvl " + creature.level + ")";
        }

        private void closeSettings(bool save)
        {
            panel1.Visible = false;
            if (save)
            {
                creature.name = textBoxName.Text;
                creature.gender = (checkBoxMale.Checked ? Gender.Male : (checkBoxFemale.Checked ? Gender.Female : Gender.Neutral));
                creature.owner = textBoxOwner.Text;
                for (int s = 0; s < 7; s++)
                {
                    creature.levelsDom[s] = (int)numUDLevelsDom[s].Value;
                }
                updateLabel();
                Changed(this, indexInListView, creature);
            }
        }

        // call this function to cleara all contents of this element
        public void Clear()
        {
            closeSettings(false);
            labelGender.Text = "";
            groupBox1.Text = "";
            creature = null;
            for (int s = 0; s < 8; s++)
            {
                stats[s].setNumbers(0, 0, 0, 0);
            }
        }

        private void checkBoxFemale_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxFemale.Checked)
                checkBoxMale.Checked = false;
        }

        private void checkBoxMale_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMale.Checked)
                checkBoxFemale.Checked = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            closeSettings(true);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            closeSettings(false);
        }

    }
}
