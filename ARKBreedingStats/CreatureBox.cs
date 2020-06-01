using ARKBreedingStats.library;
using ARKBreedingStats.Library;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class CreatureBox : UserControl
    {
        private Creature _creature;
        public event Action<Creature, bool, bool> Changed;
        public event Action<Creature> GiveParents;
        private Sex sex;
        private CreatureStatus creatureStatus;
        public List<Creature>[] parentList; // all creatures that could be parents (i.e. same species, separated by sex)
        public List<int>[] parentListSimilarity; // for all possible parents the number of equal stats (to find the parents easier)
        private bool[] colorRegionUseds;
        private CreatureCollection cc;
        private readonly ToolTip tt;

        public CreatureBox()
        {
            InitializeComponent();

            tt = new ToolTip();
            Disposed += (s, e) =>
            {
                tt.RemoveAll();
                tt.Dispose();
            };

            _creature = null;
            parentComboBoxMother.naLabel = "- Mother n/a";
            parentComboBoxFather.naLabel = "- Father n/a";
            regionColorChooser1.RegionColorChosen += RegionColorChooser1_RegionColorChosen;

            // tooltips
            tt.SetToolTip(buttonEdit, "Edit");
            tt.SetToolTip(labelM, "Mother");
            tt.SetToolTip(labelF, "Father");
            tt.SetToolTip(textBoxNote, "Note");
            tt.SetToolTip(labelParents, "Mother and Father (if bred and choosen)");
            tt.SetToolTip(buttonSex, "Sex");
        }

        public void SetCreature(Creature creature)
        {
            Clear();
            this._creature = creature;
            regionColorChooser1.SetSpecies(creature.Species, creature.colors);
            colorRegionUseds = regionColorChooser1.ColorRegionsUseds;

            UpdateLabel();
        }

        public CreatureCollection CreatureCollection
        {
            set
            {
                cc = value;
                statsDisplay1.BarMaxLevel = cc?.maxChartLevel ?? 50;
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (_creature != null)
            {
                if (panel1.Visible)
                {
                    CloseSettings(false);
                }
                else
                {
                    checkBoxIsBred.Checked = _creature.isBred;
                    panelParents.Visible = _creature.isBred;
                    if (_creature.isBred)
                        PopulateParentsList();
                    textBoxName.Text = _creature.name;
                    textBoxOwner.Text = _creature.owner;
                    textBoxNote.Text = _creature.note;
                    sex = _creature.sex;
                    buttonSex.Text = Utils.SexSymbol(sex);
                    creatureStatus = _creature.Status;
                    SetStatusButton(creatureStatus);
                    textBoxName.SelectAll();
                    textBoxName.Focus();
                    panel1.Visible = true;
                }
            }
        }

        private void SetStatusButton(CreatureStatus status)
        {
            buttonStatus.Text = Utils.StatusSymbol(status);
            tt.SetToolTip(buttonStatus, $"Status: {Utils.StatusText(status)}");
        }

        private void PopulateParentsList()
        {
            if (parentList[0] == null || parentList[1] == null)
            {
                GiveParents(_creature);

                parentComboBoxMother.parentsSimilarity = parentListSimilarity[0];
                parentComboBoxMother.ParentList = parentList[0];
                parentComboBoxMother.PreselectedCreatureGuid = _creature.motherGuid;
                parentComboBoxFather.parentsSimilarity = parentListSimilarity[1];
                parentComboBoxFather.ParentList = parentList[1];
                parentComboBoxFather.PreselectedCreatureGuid = _creature.fatherGuid;
            }
        }

        public void UpdateLabel()
        {
            labelParents.Text = "";
            if (_creature != null)
            {
                groupBox1.Text = $"{_creature.name} (Lvl {_creature.Level}/{_creature.LevelHatched + cc.maxDomLevel})";
                if (_creature.Mother != null || _creature.Father != null)
                {
                    if (_creature.Mother != null)
                        labelParents.Text = "Mo: " + _creature.Mother.name;
                    if (_creature.Father != null && _creature.Mother != null)
                        labelParents.Text += "; ";
                    if (_creature.Father != null)
                        labelParents.Text += "Fa: " + _creature.Father.name;
                }
                else if (_creature.isBred)
                {
                    labelParents.Text = "bred, click 'edit' to add parents";
                }
                else
                {
                    labelParents.Text = "found wild " + _creature.levelFound + (_creature.tamingEff >= 0 ? ", tamed with TE: " + (_creature.tamingEff * 100).ToString("N1") + "%" : ", TE unknown.");
                }
                statsDisplay1.SetCreatureValues(_creature);
                labelNotes.Text = _creature.note;
                labelSpecies.Text = _creature.Species.name;
                pictureBox1.Image = CreatureColored.GetColoredCreature(_creature.colors, _creature.Species, colorRegionUseds, creatureSex: _creature.sex);
                tt.SetToolTip(pictureBox1, CreatureColored.RegionColorInfo(_creature.Species, _creature.colors)
                    + "\n\nClick to copy creature infos as image to the clipboard");
                pictureBox1.Visible = true;
            }
        }

        private void CloseSettings(bool save)
        {
            panel1.Visible = false;
            if (save)
            {
                _creature.name = textBoxName.Text;
                _creature.sex = sex;
                _creature.owner = textBoxOwner.Text;
                Creature parent = null;
                if (checkBoxIsBred.Checked)
                    parent = parentComboBoxMother.SelectedParent;
                _creature.motherGuid = parent?.guid ?? Guid.Empty;
                bool parentsChanged = false;
                if (_creature.Mother != parent)
                {
                    _creature.Mother = parent;
                    parentsChanged = true;
                }
                parent = null;
                if (checkBoxIsBred.Checked)
                    parent = parentComboBoxFather.SelectedParent;
                _creature.fatherGuid = parent?.guid ?? Guid.Empty;
                if (_creature.Father != parent)
                {
                    _creature.Father = parent;
                    parentsChanged = true;
                }
                if (parentsChanged)
                    _creature.RecalculateAncestorGenerations();

                _creature.isBred = checkBoxIsBred.Checked;

                _creature.note = textBoxNote.Text;
                bool creatureStatusChanged = (_creature.Status != creatureStatus);
                _creature.Status = creatureStatus;

                Changed?.Invoke(_creature, creatureStatusChanged, true);
                UpdateLabel();
            }
        }

        // call this function to clear all contents of this element
        public void Clear()
        {
            parentComboBoxMother.Items.Clear();
            parentComboBoxFather.Items.Clear();
            parentList = new List<Creature>[2];
            CloseSettings(false);
            groupBox1.Text = string.Empty;
            _creature = null;
            labelParents.Text = string.Empty;
            statsDisplay1.Clear();
            pictureBox1.Visible = false;
            regionColorChooser1.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CloseSettings(true);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            CloseSettings(false);
        }

        private void buttonSex_Click(object sender, EventArgs e)
        {
            sex = Utils.NextSex(sex);
            buttonSex.Text = Utils.SexSymbol(sex);
        }

        private void buttonStatus_Click(object sender, EventArgs e)
        {
            creatureStatus = Utils.NextStatus(creatureStatus);
            SetStatusButton(creatureStatus);
        }

        private void checkBoxIsBred_CheckedChanged(object sender, EventArgs e)
        {
            panelParents.Visible = checkBoxIsBred.Checked;
            if (checkBoxIsBred.Checked)
                PopulateParentsList();
        }

        private void RegionColorChooser1_RegionColorChosen()
        {
            if (_creature == null) return;

            pictureBox1.Image = CreatureColored.GetColoredCreature(_creature.colors, _creature.Species, colorRegionUseds, creatureSex: _creature.sex);
            _creature.colors = regionColorChooser1.ColorIDs;
            Changed?.Invoke(_creature, false, false);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            _creature?.ExportInfoGraphicToClipboard(cc);
        }
    }
}