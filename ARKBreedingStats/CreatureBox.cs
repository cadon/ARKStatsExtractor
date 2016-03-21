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
        public delegate void ChangedEventHandler(object sender, int listViewIndex, Creature creature, bool creatureStatusChanged);
        public event ChangedEventHandler Changed;
        public delegate void EventHandler(object sender, Creature creature);
        public event EventHandler NeedParents;
        public int indexInListView;
        private Gender gender;
        public List<Creature>[] parentList; // all creatures that could be parents (i.e. same species, separated by gender)
        public List<int>[] parentListSimilarity; // for all possible parents the number of equal stats (to find the parents easier)

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
            statDisplayTo.ShowBars = false;
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.labelHeaderDomLevelSet, "Set the spend domesticated Levels here");
            tt.SetToolTip(labelGender, "Gender of the Creature");
            tt.SetToolTip(labelStatHeader, "Wild-levels, Domesticated-levels, Value that is inherited, Current Value of the Creature");
            tt.SetToolTip(buttonEdit, "Edit");
            tt.SetToolTip(labelM, "Mother");
            tt.SetToolTip(labelF, "Father");
            tt.SetToolTip(comboBoxMother, "Mother");
            tt.SetToolTip(comboBoxFather, "Father");
            tt.SetToolTip(textBoxNote, "Note");
            tt.SetToolTip(labelParents, "Mother and Father (if bred and choosen)");
            tt.SetToolTip(buttonGender, "Gender");
            tt.SetToolTip(checkBoxDead, "Has the creature passed away?");
        }

        public void setCreature(Creature creature)
        {
            Clear();
            this.creature = creature;
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
            SuspendLayout();
            if (creature != null)
            {
                if (panel1.Visible)
                {
                    closeSettings(false);
                }
                else
                {
                    checkBoxIsBred.Checked = creature.isBred;
                    panelParents.Visible = creature.isBred;
                    if (creature.isBred)
                        populateParentsList();
                    textBoxName.Text = creature.name;
                    textBoxOwner.Text = creature.owner;
                    gender = creature.gender;
                    textBoxNote.Text = creature.note;
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
            ResumeLayout();
        }

        private void populateParentsList()
        {
            if (parentList[0] == null || parentList[0].Count == 0)
            {
                NeedParents(this, creature);
                int selectedParentIndex = 0;
                comboBoxMother.Items.Clear();
                comboBoxMother.Items.Add("- Mother n/a");
                for (int c = 0; c < parentList[0].Count; c++)
                {
                    comboBoxMother.Items.Add(parentList[0][c].name + " (" + parentListSimilarity[0][c] + ")");
                    //comboBoxMother
                    if (parentList[0][c].guid == creature.motherGuid)
                        selectedParentIndex = comboBoxMother.Items.Count - 1;
                }
                comboBoxMother.SelectedIndex = selectedParentIndex;
                selectedParentIndex = 0;
                comboBoxFather.Items.Clear();
                comboBoxFather.Items.Add("- Father n/a");
                for (int c = 0; c < parentList[1].Count; c++)
                {
                    comboBoxFather.Items.Add(parentList[1][c].name + " (" + parentListSimilarity[1][c] + ")");
                    if (parentList[1][c].guid == creature.fatherGuid)
                        selectedParentIndex = comboBoxFather.Items.Count - 1;
                }
                comboBoxFather.SelectedIndex = selectedParentIndex;
            }
        }

        public void updateLabel()
        {
            if (creature != null)
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
                if (creature.Mother != null || creature.Father != null)
                {
                    labelParents.Text = "";
                    if (creature.Mother != null)
                        labelParents.Text = "Mo: " + creature.Mother.name;
                    if (creature.Father != null && creature.Mother != null)
                        labelParents.Text += "; ";
                    if (creature.Father != null)
                        labelParents.Text += "Fa: " + creature.Father.name;
                }
                else if (creature.isBred)
                {
                    labelParents.Text = "bred, click 'edit' to add parents";
                }
                else
                {
                    labelParents.Text = "found wild, tamed with TE: " + (creature.tamingEff * 100) + "%";
                }
                for (int s = 0; s < 8; s++) { updateStat(s); }
                labelNotes.Text = creature.note;
                checkBoxDead.Checked = (creature.status == CreatureStatus.Dead);
            }
        }

        private void closeSettings(bool save)
        {
            panel1.Visible = false;
            if (save)
            {
                SuspendLayout();
                creature.name = textBoxName.Text;
                creature.gender = gender;
                creature.owner = textBoxOwner.Text;
                Creature parent = null;
                if (checkBoxIsBred.Checked && comboBoxMother.SelectedIndex > 0)
                    parent = parentList[0][comboBoxMother.SelectedIndex - 1];
                creature.motherGuid = (parent != null ? parent.guid : Guid.Empty);
                bool parentsChanged = false;
                if (creature.Mother != parent)
                {
                    creature.Mother = parent;
                    parentsChanged = true;
                }
                parent = null;
                if (checkBoxIsBred.Checked && comboBoxFather.SelectedIndex > 0)
                    parent = parentList[1][comboBoxFather.SelectedIndex - 1];
                creature.fatherGuid = (parent != null ? parent.guid : Guid.Empty);
                if (creature.Father != parent)
                {
                    creature.Father = parent;
                    parentsChanged = true;
                }
                if (parentsChanged)
                    creature.recalculateAncestorGenerations();

                creature.isBred = checkBoxIsBred.Checked;

                for (int s = 0; s < 7; s++)
                {
                    creature.levelsDom[s] = (int)numUDLevelsDom[s].Value;
                }
                creature.note = textBoxNote.Text;
                CreatureStatus newStatus = (checkBoxDead.Checked ? CreatureStatus.Dead : CreatureStatus.Alive);
                bool creatureStatusChanged = (creature.status != newStatus);
                creature.status = newStatus;

                Changed(this, indexInListView, creature, creatureStatusChanged);
                updateLabel();
                ResumeLayout();
            }
        }

        // call this function to clear all contents of this element
        public void Clear()
        {
            comboBoxMother.Items.Clear();
            comboBoxFather.Items.Clear();
            parentList = new List<Creature>[2];
            closeSettings(false);
            labelGender.Text = "";
            groupBox1.Text = "";
            checkBoxDead.Checked = false;
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
                    gender = Gender.Unknown;
                    buttonGender.Text = "?";
                    break;
                default:
                    gender = Gender.Female;
                    buttonGender.Text = "♀";
                    break;
            }
        }

        private void checkBoxIsBred_CheckedChanged(object sender, EventArgs e)
        {
            panelParents.Visible = checkBoxIsBred.Checked;
            if (checkBoxIsBred.Checked)
                populateParentsList();
        }

        private void comboBoxParents_DrawItem(object sender, DrawItemEventArgs e)
        {
            // index of item in parentListSimilarity
            int i = e.Index - 1;
            if (i < -1)
            {
                return;
            }

            int p = 1;
            if ((ComboBox)sender == comboBoxMother)
                p = 0;

            // Draw the background of the ComboBox control for each item.
            e.DrawBackground();
            // Define the default color of the brush as black.
            Brush myBrush = Brushes.Black;

            // colors of similarity
            Brush[] brushes = new Brush[] { Brushes.Black, Brushes.DarkRed, Brushes.DarkOrange, Brushes.Green, Brushes.Green, Brushes.Green, Brushes.Green, Brushes.Green };

            if (i == -1)
            {
                myBrush = Brushes.DarkGray; // no parent
            }
            else
            {
                // Determine the color of the brush to draw each item based on the similarity of the wildlevels
                myBrush = brushes[parentListSimilarity[p][i]];
            }

            // Draw the current item text
            e.Graphics.DrawString(((ComboBox)sender).Items[e.Index].ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
        }
    }
}
