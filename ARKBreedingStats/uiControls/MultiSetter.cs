using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class MultiSetter : Form
    {

        private Creature c;
        private List<bool> appliedSettings; // {owner, status, gender, bred, mother, father, note, color1,...,color6}
        private MyColorPicker cp = new MyColorPicker();
        private bool uniqueSpecies;
        private ToolTip tt = new ToolTip();

        public MultiSetter()
        {
            InitializeComponent();
        }
        public MultiSetter(Creature creatureSettings, List<bool> appliedSettings, List<Creature>[] parents)
        {
            InitializeComponent();
            if (appliedSettings.Count != 13)
            {
                DialogResult = DialogResult.Cancel; // invalid parameters
            }
            this.c = creatureSettings;
            this.appliedSettings = appliedSettings;
            parentComboBoxMother.naLabel = " - Mother n/a";
            parentComboBoxFather.naLabel = " - Father n/a";
            if (parents == null)
            {
                // disable parents, probably multiple species selected
                checkBoxMother.Enabled = false;
                checkBoxFather.Enabled = false;
                parentComboBoxMother.Enabled = false;
                parentComboBoxFather.Enabled = false;
            }
            else
            {
                parentComboBoxMother.ParentList = parents[0];
                parentComboBoxFather.ParentList = parents[1];
                uniqueSpecies = true;
            }
            checkBoxMother.Checked = false;
            checkBoxFather.Checked = false;

            pictureBox1.Image = CreatureColored.getColoredCreature(c.colors, (uniqueSpecies ? c.species : ""), new bool[] { true, true, true, true, true, true });
        }

        private void buttonStatus_Click(object sender, EventArgs e)
        {
            c.status = Utils.nextStatus(c.status);
            buttonStatus.Text = Utils.statusSymbol(c.status);
            checkBoxStatus.Checked = true;
            tt.SetToolTip(buttonStatus, "Status: " + c.status.ToString());
        }

        private void buttonGender_Click(object sender, EventArgs e)
        {
            c.gender = Utils.nextSex(c.gender);
            buttonSex.Text = Utils.sexSymbol(c.gender);
            checkBoxSex.Checked = true;
            tt.SetToolTip(buttonSex, "Sex: " + c.gender.ToString());
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            // set all variables
            appliedSettings[0] = checkBoxOwner.Checked;
            appliedSettings[1] = checkBoxStatus.Checked;
            appliedSettings[2] = checkBoxSex.Checked;
            appliedSettings[3] = checkBoxBred.Checked;
            appliedSettings[4] = checkBoxMother.Checked;
            appliedSettings[5] = checkBoxFather.Checked;
            appliedSettings[6] = checkBoxNote.Checked;
            appliedSettings[7] = checkBoxColor1.Checked;
            appliedSettings[8] = checkBoxColor2.Checked;
            appliedSettings[9] = checkBoxColor3.Checked;
            appliedSettings[10] = checkBoxColor4.Checked;
            appliedSettings[11] = checkBoxColor5.Checked;
            appliedSettings[12] = checkBoxColor6.Checked;

            c.owner = textBoxOwner.Text;
            c.isBred = checkBoxIsBred.Checked;
            if (checkBoxMother.Enabled && checkBoxMother.Checked)
                c.motherGuid = (parentComboBoxMother.SelectedParent == null ? Guid.Empty : parentComboBoxMother.SelectedParent.guid);
            if (checkBoxFather.Enabled && checkBoxFather.Checked)
                c.fatherGuid = (parentComboBoxFather.SelectedParent == null ? Guid.Empty : parentComboBoxFather.SelectedParent.guid);
        }

        private void textBoxOwner_TextChanged(object sender, EventArgs e)
        {
            checkBoxOwner.Checked = true;
        }

        private void checkBoxIsBred_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxBred.Checked = true;
        }

        private void parentComboBoxMother_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkBoxMother.Checked = true;
        }

        private void parentComboBoxFather_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkBoxFather.Checked = true;
        }

        private void textBoxNote_TextChanged(object sender, EventArgs e)
        {
            checkBoxNote.Checked = true;
        }

        private void buttonColor1_Click(object sender, EventArgs e)
        {
            chooseColor(0, buttonColor1);
            checkBoxColor1.Checked = true;
        }

        private void buttonColor2_Click(object sender, EventArgs e)
        {
            chooseColor(1, buttonColor2);
            checkBoxColor2.Checked = true;
        }

        private void buttonColor3_Click(object sender, EventArgs e)
        {
            chooseColor(2, buttonColor3);
            checkBoxColor3.Checked = true;
        }

        private void buttonColor4_Click(object sender, EventArgs e)
        {
            chooseColor(3, buttonColor4);
            checkBoxColor4.Checked = true;
        }

        private void buttonColor5_Click(object sender, EventArgs e)
        {
            chooseColor(4, buttonColor5);
            checkBoxColor5.Checked = true;
        }

        private void buttonColor6_Click(object sender, EventArgs e)
        {
            chooseColor(5, buttonColor6);
            checkBoxColor6.Checked = true;
        }
        private void chooseColor(int region, Button sender)
        {
            if (c != null && !cp.isShown)
            {
                cp.SetColors(c.colors, region, "Region " + region.ToString());
                if (cp.ShowDialog() == DialogResult.OK)
                {
                    // color was chosen
                    setColorButton(sender, Utils.creatureColor(c.colors[region]));
                    pictureBox1.Image = CreatureColored.getColoredCreature(c.colors, (uniqueSpecies ? c.species : ""), new bool[] { true, true, true, true, true, true });
                }
            }
        }

        private void setColorButton(Button bt, Color cl)
        {
            bt.BackColor = cl;
            bt.ForeColor = ((cl.R * .3f + cl.G * .59f + cl.B * .11f) < 100 ? Color.White : SystemColors.ControlText);
        }

        public void DisposeToolTips()
        {
            tt.RemoveAll();
        }
    }
}
