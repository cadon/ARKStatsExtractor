using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.uiControls
{
    public partial class MultiSetter : Form
    {

        private readonly List<Creature> _creatureList;
        private readonly MyColorPicker _cp = new MyColorPicker();
        private readonly bool _uniqueSpecies;
        private readonly ToolTip _tt = new ToolTip();
        public bool ParentsChanged, TagsChanged, SpeciesChanged;
        private CreatureStatus _creatureStatus;
        private Sex _creatureSex;
        private readonly byte[] _colors;
        private readonly List<MultiSetterTag> _tagControls;

        public MultiSetter()
        {
            InitializeComponent();
        }

        public MultiSetter(List<Creature> creatureList, List<Creature>[] parents, List<string> tagList, List<Species> speciesList, string[] ownerList, string[] tribeList, string[] serverList)
        {
            InitializeComponent();
            Disposed += MultiSetter_Disposed;

            SuspendLayout();
            _colors = new byte[Ark.ColorRegionCount];
            _tagControls = new List<MultiSetterTag>();

            _creatureList = creatureList;
            parentComboBoxMother.naLabel = " - Mother n/a";
            parentComboBoxFather.naLabel = " - Father n/a";
            if (parents == null)
            {
                // disable parents, probably multiple species selected
                checkBoxMother.Enabled = false;
                checkBoxFather.Enabled = false;
                parentComboBoxMother.Enabled = false;
                parentComboBoxFather.Enabled = false;
                _uniqueSpecies = false;
            }
            else
            {
                parentComboBoxMother.ParentList = parents[0];
                parentComboBoxFather.ParentList = parents[1] ?? parents[0];
                _uniqueSpecies = true;
            }
            checkBoxMother.Checked = false;
            checkBoxFather.Checked = false;
            _creatureStatus = CreatureStatus.Available;
            _creatureSex = Sex.Unknown;

            ParentsChanged = false;
            TagsChanged = false;
            SpeciesChanged = false;

            pictureBox1.SetImageAndDisposeOld(CreatureColored.GetColoredCreature(_colors, _uniqueSpecies ? creatureList[0].Species : null,
                    new[] { true, true, true, true, true, true }));

            // tags
            foreach (string t in tagList)
            {
                MultiSetterTag mst = new MultiSetterTag(t);
                flowLayoutPanelTags.SetFlowBreak(mst, true);
                flowLayoutPanelTags.Controls.Add(mst);
                _tagControls.Add(mst);
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
            }

            cbbSpecies.Items.AddRange(speciesList.ToArray());

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

            SetLocalizations();
            ResumeLayout();
        }

        private void buttonStatus_Click(object sender, EventArgs e)
        {
            _creatureStatus = Utils.NextStatus(_creatureStatus);
            buttonStatus.Text = Utils.StatusSymbol(_creatureStatus);
            checkBoxStatus.Checked = true;
            _tt.SetToolTip(buttonStatus, "Status: " + _creatureStatus);
        }

        private void buttonSex_Click(object sender, EventArgs e)
        {
            _creatureSex = Utils.NextSex(_creatureSex);
            buttonSex.Text = Utils.SexSymbol(_creatureSex);
            checkBoxSex.Checked = true;
            _tt.SetToolTip(buttonSex, "Sex: " + _creatureSex);
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            ParentsChanged = checkBoxMother.Checked || checkBoxFather.Checked;
            SpeciesChanged = checkBoxSpecies.Checked;

            var tagsToUpdate = _tagControls.Where(t => t.TagCheckState != CheckState.Indeterminate).ToArray();

            // set all variables
            foreach (Creature c in _creatureList)
            {
                if (checkBoxOwner.Checked) c.owner = cbbOwner.Text;
                if (cbTribe.Checked) c.tribe = cbbTribe.Text;
                if (cbServer.Checked) c.server = cbbServer.Text;
                if (checkBoxStatus.Checked) c.Status = _creatureStatus;
                if (checkBoxSex.Checked) c.sex = _creatureSex;
                if (checkBoxBred.Checked) c.isBred = checkBoxIsBred.Checked;
                if (checkBoxMother.Enabled && checkBoxMother.Checked)
                    c.motherGuid = parentComboBoxMother.SelectedParent?.guid ?? Guid.Empty;
                if (checkBoxFather.Enabled && checkBoxFather.Checked)
                    c.fatherGuid = parentComboBoxFather.SelectedParent?.guid ?? Guid.Empty;
                if (checkBoxNote.Checked) c.note = textBoxNote.Text;
                if (checkBoxSpecies.Checked) c.Species = (Species)cbbSpecies.SelectedItem;

                if (checkBoxColor1.Checked) c.colors[0] = _colors[0];
                if (checkBoxColor2.Checked) c.colors[1] = _colors[1];
                if (checkBoxColor3.Checked) c.colors[2] = _colors[2];
                if (checkBoxColor4.Checked) c.colors[3] = _colors[3];
                if (checkBoxColor5.Checked) c.colors[4] = _colors[4];
                if (checkBoxColor6.Checked) c.colors[5] = _colors[5];

                // tags
                foreach (MultiSetterTag mst in tagsToUpdate)
                {
                    if (mst.TagCheckState == CheckState.Checked && c.tags.IndexOf(mst.TagName) == -1)
                        c.tags.Add(mst.TagName);
                    else if (mst.TagCheckState == CheckState.Unchecked && c.tags.IndexOf(mst.TagName) != -1)
                        while (c.tags.Remove(mst.TagName)) { }

                    TagsChanged = true;
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
            ChooseColor(0, buttonColor1);
            checkBoxColor1.Checked = true;
        }

        private void buttonColor2_Click(object sender, EventArgs e)
        {
            ChooseColor(1, buttonColor2);
            checkBoxColor2.Checked = true;
        }

        private void buttonColor3_Click(object sender, EventArgs e)
        {
            ChooseColor(2, buttonColor3);
            checkBoxColor3.Checked = true;
        }

        private void buttonColor4_Click(object sender, EventArgs e)
        {
            ChooseColor(3, buttonColor4);
            checkBoxColor4.Checked = true;
        }

        private void buttonColor5_Click(object sender, EventArgs e)
        {
            ChooseColor(4, buttonColor5);
            checkBoxColor5.Checked = true;
        }

        private void buttonColor6_Click(object sender, EventArgs e)
        {
            ChooseColor(5, buttonColor6);
            checkBoxColor6.Checked = true;
        }
        private void ChooseColor(int region, Button sender)
        {
            if (_creatureList[0] != null && !_cp.isShown)
            {
                _cp.PickColor(_colors[region], "Region " + region);
                if (_cp.ShowDialog() == DialogResult.OK)
                {
                    // color was chosen
                    _colors[region] = _cp.SelectedColorId;
                    sender.SetBackColorAndAccordingForeColor(CreatureColors.CreatureColor(_colors[region]));
                    pictureBox1.SetImageAndDisposeOld(CreatureColored.GetColoredCreature(_colors, _uniqueSpecies ? _creatureList[0].Species : null,
                            new[] { true, true, true, true, true, true }));
                }
            }
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
            var tagName = tbNewTag.Text.Trim();
            if (string.IsNullOrEmpty(tagName)) return;

            if (_tagControls.Any(t => t.TagName == tagName))
                return;

            MultiSetterTag mst = new MultiSetterTag(tagName);
            flowLayoutPanelTags.SetFlowBreak(mst, true);
            flowLayoutPanelTags.Controls.Add(mst);
            _tagControls.Add(mst);
            mst.TagCheckState = CheckState.Checked;
        }

        private void MultiSetter_Disposed(object sender, EventArgs e)
        {
            _tt.RemoveAll();
            _tt.Dispose();
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
