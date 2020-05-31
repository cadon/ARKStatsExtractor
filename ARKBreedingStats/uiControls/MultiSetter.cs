using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class MultiSetter : Form
    {

        private readonly List<Creature> creatureList;
        private readonly MyColorPicker cp = new MyColorPicker();
        private readonly bool uniqueSpecies;
        private readonly ToolTip tt = new ToolTip();
        public bool ParentsChanged, TagsChanged, SpeciesChanged;
        private CreatureStatus creatureStatus;
        private Sex creatureSex;
        private readonly int[] colors;
        private readonly List<MultiSetterTag> tagControls;

        public MultiSetter()
        {
            InitializeComponent();
        }

        public MultiSetter(List<Creature> creatureList, List<Creature>[] parents, List<string> tagList, List<Species> speciesList, string[] ownerList, string[] tribeList, string[] serverList)
        {
            InitializeComponent();
            Disposed += MultiSetter_Disposed;

            SuspendLayout();
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
            creatureStatus = CreatureStatus.Available;
            creatureSex = Sex.Unknown;

            ParentsChanged = false;
            TagsChanged = false;
            SpeciesChanged = false;

            pictureBox1.Image = CreatureColored.GetColoredCreature(colors, uniqueSpecies ? creatureList[0].Species : null,
                    new[] { true, true, true, true, true, true });

            // tags
            foreach (string t in tagList)
            {
                MultiSetterTag mst = new MultiSetterTag(t);
                flowLayoutPanelTags.SetFlowBreak(mst, true);
                flowLayoutPanelTags.Controls.Add(mst);
                tagControls.Add(mst);
                mst.TagCheckState = CheckState.Indeterminate;
                foreach (var c in creatureList)
                {
                    if (c.tags.Contains(t))
                    {
                        if (mst.TagCheckState == CheckState.Indeterminate)
                            mst.TagCheckState = CheckState.Checked;
                        else if (mst.TagCheckState == CheckState.Unchecked)
                        {
                            mst.TagCheckState = CheckState.Indeterminate;
                            break;
                        }
                    }
                    else
                    {
                        if (mst.TagCheckState == CheckState.Indeterminate)
                            mst.TagCheckState = CheckState.Unchecked;
                        else if (mst.TagCheckState == CheckState.Checked)
                        {
                            mst.TagCheckState = CheckState.Indeterminate;
                            break;
                        }
                    }
                }
                mst.Considered = false;
            }

            foreach (var s in speciesList)
                cbbSpecies.Items.Add(s);

            // owner combobox
            var l = new AutoCompleteStringCollection();
            l.AddRange(ownerList);
            cbbOwner.AutoCompleteCustomSource = l;
            foreach (string s in ownerList)
                cbbOwner.Items.Add(s);

            // tribe combobox
            l = new AutoCompleteStringCollection();
            l.AddRange(tribeList);
            cbbTribe.AutoCompleteCustomSource = l;
            foreach (string s in tribeList)
                cbbTribe.Items.Add(s);

            // server combobox
            l = new AutoCompleteStringCollection();
            l.AddRange(serverList);
            cbbServer.AutoCompleteCustomSource = l;
            foreach (string s in serverList)
                cbbServer.Items.Add(s);

            tt.SetToolTip(lbTagSettingInfo, "The left checkbox indicates if the setting of that tag is applied, " +
                    "the right checkbox indicates if the tag is added or removed from the selected creatures.");

            SetLocalizations();
            ResumeLayout();
        }

        private void buttonStatus_Click(object sender, EventArgs e)
        {
            creatureStatus = Utils.NextStatus(creatureStatus);
            buttonStatus.Text = Utils.StatusSymbol(creatureStatus);
            checkBoxStatus.Checked = true;
            tt.SetToolTip(buttonStatus, "Status: " + creatureStatus);
        }

        private void buttonSex_Click(object sender, EventArgs e)
        {
            creatureSex = Utils.NextSex(creatureSex);
            buttonSex.Text = Utils.SexSymbol(creatureSex);
            checkBoxSex.Checked = true;
            tt.SetToolTip(buttonSex, "Sex: " + creatureSex);
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            ParentsChanged = checkBoxMother.Checked || checkBoxFather.Checked;
            SpeciesChanged = checkBoxSpecies.Checked;

            // set all variables
            foreach (Creature c in creatureList)
            {
                if (checkBoxOwner.Checked) c.owner = cbbOwner.Text;
                if (cbTribe.Checked) c.tribe = cbbTribe.Text;
                if (cbServer.Checked) c.server = cbbServer.Text;
                if (checkBoxStatus.Checked) c.Status = creatureStatus;
                if (checkBoxSex.Checked) c.sex = creatureSex;
                if (checkBoxBred.Checked) c.isBred = checkBoxIsBred.Checked;
                if (checkBoxMother.Enabled && checkBoxMother.Checked)
                    c.motherGuid = parentComboBoxMother.SelectedParent?.guid ?? Guid.Empty;
                if (checkBoxFather.Enabled && checkBoxFather.Checked)
                    c.fatherGuid = parentComboBoxFather.SelectedParent?.guid ?? Guid.Empty;
                if (checkBoxNote.Checked) c.note = textBoxNote.Text;
                if (checkBoxSpecies.Checked) c.Species = (Species)cbbSpecies.SelectedItem;

                if (checkBoxColor1.Checked) c.colors[0] = colors[0];
                if (checkBoxColor2.Checked) c.colors[1] = colors[1];
                if (checkBoxColor3.Checked) c.colors[2] = colors[2];
                if (checkBoxColor4.Checked) c.colors[3] = colors[3];
                if (checkBoxColor5.Checked) c.colors[4] = colors[4];
                if (checkBoxColor6.Checked) c.colors[5] = colors[5];

                // tags
                foreach (MultiSetterTag mst in tagControls)
                {
                    if (mst.Considered && mst.TagCheckState != CheckState.Indeterminate)
                    {
                        if (mst.TagCheckState == CheckState.Checked && c.tags.IndexOf(mst.TagName) == -1)
                            c.tags.Add(mst.TagName);
                        else if (mst.TagCheckState == CheckState.Unchecked && c.tags.IndexOf(mst.TagName) != -1)
                            while (c.tags.Remove(mst.TagName)) ;
                        TagsChanged = true;
                    }
                }
            }
        }

        private void cbbOwner_TextUpdate(object sender, EventArgs e)
        {
            checkBoxOwner.Checked = true;
        }

        private void cbbOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkBoxOwner.Checked = true;
        }

        private void cbbTribe_TextUpdate(object sender, EventArgs e)
        {
            cbTribe.Checked = true;
        }

        private void cbbTribe_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbTribe.Checked = true;
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

        private void cbbServer_TextUpdate(object sender, EventArgs e)
        {
            cbServer.Checked = true;
        }

        private void cbbServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbServer.Checked = true;
        }

        private void textBoxNote_TextChanged(object sender, EventArgs e)
        {
            checkBoxNote.Checked = true;
        }

        private void cbbSpecies_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkBoxSpecies.Checked = true;
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
                cp.SetColors(colors[region], "Region " + region);
                if (cp.ShowDialog() == DialogResult.OK)
                {
                    // color was chosen
                    colors[region] = cp.SelectedColorId;
                    setColorButton(sender, species.CreatureColors.CreatureColor(colors[region]));
                    pictureBox1.Image = CreatureColored.GetColoredCreature(colors, uniqueSpecies ? creatureList[0].Species : null,
                            new[] { true, true, true, true, true, true });
                }
            }
        }

        private void setColorButton(Button bt, Color cl)
        {
            bt.BackColor = cl;
            bt.ForeColor = Utils.ForeColor(cl);
        }

        private void checkBoxSpecies_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSpecies.Checked && MessageBox.Show("Do you really want to change the species of the selected creatures?\n" +
                    "This should only be done if you are sure the species was not the correct one.",
                "Changing the species?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                checkBoxSpecies.Checked = false;
            }
        }

        private void bAddTag_Click(object sender, EventArgs e)
        {
            MultiSetterTag mst = new MultiSetterTag(tbNewTag.Text);
            flowLayoutPanelTags.SetFlowBreak(mst, true);
            flowLayoutPanelTags.Controls.Add(mst);
            tagControls.Add(mst);
            mst.TagCheckState = CheckState.Checked;
        }

        private void MultiSetter_Disposed(object sender, EventArgs e)
        {
            tt.RemoveAll();
        }

        private void SetLocalizations()
        {
            Loc.ControlText(checkBoxOwner, "Owner");
            Loc.ControlText(cbTribe, "Tribe");
            Loc.ControlText(checkBoxStatus, "Status");
            Loc.ControlText(checkBoxSex, "Sex");
            Loc.ControlText(checkBoxBred, "Bred");
            Loc.ControlText(checkBoxMother, "Mother");
            Loc.ControlText(checkBoxFather, "Father");
            Loc.ControlText(cbServer, "Server");
            Loc.ControlText(checkBoxNote, "Note");
            Loc.ControlText(checkBoxSpecies, "Species");
            Loc.ControlText(buttonApply, "apply");
            Loc.ControlText(buttonCancel, "Cancel");
        }
    }
}
