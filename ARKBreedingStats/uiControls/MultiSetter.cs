using ARKBreedingStats.uiControls;
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

        private List<Creature> creatureList;
        private MyColorPicker cp = new MyColorPicker();
        private bool uniqueSpecies;
        private ToolTip tt = new ToolTip();
        public bool ParentsChanged, TagsChanged;
        private CreatureStatus creatureStatus;
        private Sex creatureSex;
        private int[] colors;
        private List<MultiSetterTag> tagControls;

        public MultiSetter()
        {
            InitializeComponent();
        }

        public MultiSetter(List<Creature> creatureList, List<bool> appliedSettings, List<Creature>[] parents, List<string> tagList)
        {
            InitializeComponent();

            colors = new int[6];
            tagControls = new List<MultiSetterTag>();

            this.creatureList = creatureList;
            parentComboBoxMother.naLabel = " - Mother n/a";
            parentComboBoxFather.naLabel = " - Father n/a";
            if (parents == null)
            {
                // disable parents, probably multiple species selected
                checkBoxMother.Enabled = false;
                checkBoxFather.Enabled = false;
                parentComboBoxMother.Enabled = false;
                parentComboBoxFather.Enabled = false;
                uniqueSpecies = false;
            }
            else
            {
                parentComboBoxMother.ParentList = parents[0];
                parentComboBoxFather.ParentList = parents[1];
                uniqueSpecies = true;
            }
            checkBoxMother.Checked = false;
            checkBoxFather.Checked = false;
            creatureStatus = CreatureStatus.Alive;
            creatureSex = Sex.Unknown;

            ParentsChanged = false;
            TagsChanged = false;

            pictureBox1.Image = CreatureColored.getColoredCreature(colors, (uniqueSpecies ? creatureList[0].species : ""), new bool[] { true, true, true, true, true, true });

            // tags
            MultiSetterTag mst;
            int i = 0;
            foreach (string t in tagList)
            {
                mst = new MultiSetterTag(t);
                mst.Location = new Point(3, 3 + i * 29 - panelTags.VerticalScroll.Value);
                panelTags.Controls.Add(mst);
                tagControls.Add(mst);
                i++;
            }
        }

        private void buttonStatus_Click(object sender, EventArgs e)
        {
            creatureStatus = Utils.nextStatus(creatureStatus);
            buttonStatus.Text = Utils.statusSymbol(creatureStatus);
            checkBoxStatus.Checked = true;
            tt.SetToolTip(buttonStatus, "Status: " + creatureStatus.ToString());
        }

        private void buttonGender_Click(object sender, EventArgs e)
        {
            creatureSex = Utils.nextSex(creatureSex);
            buttonSex.Text = Utils.sexSymbol(creatureSex);
            checkBoxSex.Checked = true;
            tt.SetToolTip(buttonSex, "Sex: " + creatureSex.ToString());
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            ParentsChanged = checkBoxMother.Checked || checkBoxFather.Checked;

            // set all variables
            foreach (Creature c in creatureList)
            {
                if (checkBoxOwner.Checked) c.owner = textBoxOwner.Text;
                if (checkBoxStatus.Checked) c.status = creatureStatus;
                if (checkBoxSex.Checked) c.gender = creatureSex;
                if (checkBoxBred.Checked) c.isBred = checkBoxIsBred.Checked;
                if (checkBoxMother.Enabled && checkBoxMother.Checked)
                    c.motherGuid = (parentComboBoxMother.SelectedParent == null ? Guid.Empty : parentComboBoxMother.SelectedParent.guid);
                if (checkBoxFather.Enabled && checkBoxFather.Checked)
                    c.fatherGuid = (parentComboBoxFather.SelectedParent == null ? Guid.Empty : parentComboBoxFather.SelectedParent.guid);
                if (checkBoxNote.Checked) c.note = textBoxNote.Text;

                if (checkBoxColor1.Checked) c.colors[0] = colors[0];
                if (checkBoxColor2.Checked) c.colors[1] = colors[1];
                if (checkBoxColor3.Checked) c.colors[2] = colors[2];
                if (checkBoxColor4.Checked) c.colors[3] = colors[3];
                if (checkBoxColor5.Checked) c.colors[4] = colors[4];
                if (checkBoxColor6.Checked) c.colors[5] = colors[5];

                // tags
                foreach (MultiSetterTag mst in tagControls)
                {
                    if (mst.Considered)
                    {
                        if (mst.TagChecked && c.tags.IndexOf(mst.TagName) == -1)
                            c.tags.Add(mst.TagName);
                        else if (!mst.TagChecked && c.tags.IndexOf(mst.TagName) != -1)
                            while (c.tags.Remove(mst.TagName)) ;
                        TagsChanged = true;
                    }
                }
            }
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
            if (creatureList[0] != null && !cp.isShown)
            {
                cp.SetColors(colors, region, "Region " + region.ToString());
                if (cp.ShowDialog() == DialogResult.OK)
                {
                    // color was chosen
                    setColorButton(sender, Utils.creatureColor(colors[region]));
                    pictureBox1.Image = CreatureColored.getColoredCreature(colors, (uniqueSpecies ? creatureList[0].species : ""), new bool[] { true, true, true, true, true, true });
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

        private void bAddTag_Click(object sender, EventArgs e)
        {
            MultiSetterTag mst = new MultiSetterTag(tbNewTag.Text);
            mst.Location = new Point(3, 3 + panelTags.Controls.Count * 29 - panelTags.VerticalScroll.Value);
            panelTags.Controls.Add(mst);
            tagControls.Add(mst);
            mst.TagChecked = true;
        }
    }
}
