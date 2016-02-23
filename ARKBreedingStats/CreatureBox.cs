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
        public delegate void EventHandler(object sender, Creature creature);
        public event EventHandler EnterSettings;
        public int indexInListView;
        private Gender gender;
        public List<Creature>[] parentList = new List<Creature>[2]; // all creatures that could be parents (i.e. same species, separated by gender)

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
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.labelHeaderDomLevelSet, "Set the spend domesticated Levels here");
            tt.SetToolTip(labelGender, "Gender of the Creature");
            tt.SetToolTip(labelStatHeader, "Wild-levels, Domesticated-levels, Value that is inherited, Current Value of the Creature");
            tt.SetToolTip(buttonEdit, "Edit");
            tt.SetToolTip(labelM, "Mother");
            tt.SetToolTip(labelF, "Father");
        }

        public void setCreature(Creature creature)
        {
            this.creature = creature;
            panel1.Visible = false;
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
                    panelParents.Visible = creature.isBred;
                    if (creature.isBred)
                    {
                        EnterSettings(this, creature);
                        int selectedParentIndex = -1;
                        comboBoxMother.Items.Clear();
                        foreach (Creature c in parentList[0])
                        {
                            comboBoxMother.Items.Add(c.name);
                            if (c.guid == creature.motherGuid)
                                selectedParentIndex = comboBoxMother.Items.Count - 1;
                        }
                        comboBoxMother.SelectedIndex = selectedParentIndex;
                        selectedParentIndex = -1;
                        comboBoxFather.Items.Clear();
                        foreach (Creature c in parentList[1])
                        {
                            comboBoxFather.Items.Add(c.name);
                            if (c.guid == creature.fatherGuid)
                                selectedParentIndex = comboBoxFather.Items.Count - 1;
                        }
                        comboBoxFather.SelectedIndex = selectedParentIndex;
                    }
                    textBoxName.Text = creature.name;
                    textBoxOwner.Text = creature.owner;
                    gender = creature.gender;
                    switch (gender)
                    {
                        case Gender.Female:
                            buttonGender.Text = "♀";
                            break;
                        case Gender.Male:
                            buttonGender.Text = "♂";
                            break;
                        default:
                            buttonGender.Text = "?";
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
                creature.gender = gender;
                creature.owner = textBoxOwner.Text;
                Creature parent = null;
                if (comboBoxMother.SelectedIndex >= 0)
                    parent = parentList[0][comboBoxMother.SelectedIndex];
                creature.motherGuid = (parent != null ? parent.guid : Guid.Empty);
                creature.mother = parent;
                parent = null;
                if (comboBoxFather.SelectedIndex >= 0)
                    parent = parentList[1][comboBoxFather.SelectedIndex];
                creature.fatherGuid = (parent != null ? parent.guid : Guid.Empty);
                creature.father = parent;
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

        private void button1_Click(object sender, EventArgs e)
        {
            closeSettings(true);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            closeSettings(false);
        }

        private void buttonGender_Click(object sender, EventArgs e)
        {
            switch (gender)
            {
                case Gender.Female:
                    gender = Gender.Male;
                    buttonGender.Text = "♂";
                    break;
                case Gender.Male:
                    gender = Gender.Neutral;
                    buttonGender.Text = "?";
                    break;
                default:
                    gender = Gender.Female;
                    buttonGender.Text = "♀";
                    break;
            }
        }
    }
}
