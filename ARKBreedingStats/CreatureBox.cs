using ARKBreedingStats.library;
using ARKBreedingStats.Library;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;

namespace ARKBreedingStats
{
    public partial class CreatureBox : UserControl
    {
        private Creature _creature;
        public event Action<Creature, bool, bool> Changed;
        public event Action<Creature> GiveParents;
        /// <summary>
        /// Selects the creature in the library.
        /// </summary>
        public event Action<Creature> SelectCreature;
        private Sex sex;
        private CreatureStatus _creatureStatus;
        public List<Creature>[] parentList; // all creatures that could be parents (i.e. same species, separated by sex)
        public List<int>[] parentListSimilarity; // for all possible parents the number of equal stats (to find the parents easier)
        private bool[] _colorRegionUseds;
        private CreatureCollection _cc;
        private readonly ToolTip _tt;

        public CreatureBox()
        {
            InitializeComponent();

            _tt = new ToolTip
            {
                AutoPopDelay = 10000
            };
            Disposed += (s, e) =>
            {
                _tt.RemoveAll();
                _tt.Dispose();
            };

            _creature = null;
            regionColorChooser1.RegionColorChosen += RegionColorChooser1_RegionColorChosen;
        }

        public void SetCreature(Creature creature)
        {
            Clear();
            _creature = creature;
            regionColorChooser1.SetSpecies(creature.Species, creature.colors);
            regionColorChooser1.ColorIdsAlsoPossible = creature.ColorIdsAlsoPossible;
            _colorRegionUseds = regionColorChooser1.ColorRegionsUseds;

            UpdateLabel();
        }

        public CreatureCollection CreatureCollection
        {
            set
            {
                _cc = value;
                statsDisplay1.BarMaxLevel = _cc?.maxChartLevel ?? 50;
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
                    _creatureStatus = _creature.Status;
                    SetStatusButton(_creatureStatus);
                    textBoxName.SelectAll();
                    textBoxName.Focus();
                    panel1.Visible = true;
                }
            }
        }

        private void SetStatusButton(CreatureStatus status)
        {
            buttonStatus.Text = Utils.StatusSymbol(status);
            _tt.SetToolTip(buttonStatus, $"Status: {Utils.StatusText(status)}");
        }

        private void PopulateParentsList()
        {
            if (parentList[0] == null || parentList[1] == null)
            {
                GiveParents?.Invoke(_creature);

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
            LbMotherAndWildInfo.Text = string.Empty;
            if (_creature != null)
            {
                groupBox1.Text = $"{_creature.name} (Lvl {_creature.Level}/{_creature.LevelHatched + _cc.maxDomLevel})";

                void SetParentLabel(Label l, string lbText = null, bool clickable = false)
                {
                    l.Text = lbText;
                    l.Cursor = clickable ? Cursors.Hand : null;
                    _tt.SetToolTip(l, clickable ? lbText : null);
                }

                SetParentLabel(LbFather);

                if (_creature.Mother != null || _creature.Father != null)
                {
                    SetParentLabel(LbMotherAndWildInfo, _creature.Mother != null ? $"{Loc.S("Mother")}: {_creature.Mother.name}" : null, _creature.Mother != null);
                    SetParentLabel(LbFather, _creature.Father != null ? $"{Loc.S("Father")}: {_creature.Father.name}" : null, _creature.Father != null);
                }
                else if (_creature.isBred)
                {
                    SetParentLabel(LbMotherAndWildInfo, "bred, click 'edit' to add parents");
                }
                else if (_creature.isDomesticated)
                {
                    SetParentLabel(LbMotherAndWildInfo, "was level " + _creature.levelFound + " when wild" + (_creature.tamingEff >= 0 ? ", tamed with TE: " + (_creature.tamingEff * 100).ToString("N1") + "%" : ", TE unknown."));
                }
                else
                {
                    SetParentLabel(LbMotherAndWildInfo, "is wild level " + _creature.levelFound);
                }
                statsDisplay1.SetCreatureValues(_creature);
                labelNotes.Text = _creature.note;
                _tt.SetToolTip(labelNotes, _creature.note);
                labelSpecies.Text = _creature.Species.name;
                pictureBox1.SetImageAndDisposeOld(CreatureColored.GetColoredCreature(_creature.colors, _creature.Species, _colorRegionUseds, creatureSex: _creature.sex));
                _tt.SetToolTip(pictureBox1, CreatureColored.RegionColorInfo(_creature.Species, _creature.colors)
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
                bool creatureStatusChanged = (_creature.Status != _creatureStatus);
                _creature.Status = _creatureStatus;

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
            LbMotherAndWildInfo.Text = string.Empty;
            LbFather.Text = string.Empty;
            statsDisplay1.Clear();
            pictureBox1.Visible = false;
            labelSpecies.Text = string.Empty;
            labelNotes.Text = string.Empty;
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
            _creatureStatus = Utils.NextStatus(_creatureStatus);
            SetStatusButton(_creatureStatus);
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

            _creature.colors = regionColorChooser1.ColorIds;
            pictureBox1.SetImageAndDisposeOld(CreatureColored.GetColoredCreature(_creature.colors, _creature.Species, _colorRegionUseds, creatureSex: _creature.sex));
            Changed?.Invoke(_creature, false, false);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            _creature?.ExportInfoGraphicToClipboard(_cc);
        }

        public void SetLocalizations()
        {
            parentComboBoxMother.naLabel = $"- {Loc.S("Mother")} {Loc.S("na")}";
            parentComboBoxFather.naLabel = $"- {Loc.S("Father")} {Loc.S("na")}";
            // tooltips
            _tt.SetToolTip(buttonEdit, "Edit");
            _tt.SetToolTip(labelM, Loc.S("Mother"));
            _tt.SetToolTip(labelF, Loc.S("Father"));
            _tt.SetToolTip(textBoxNote, "Note");
            _tt.SetToolTip(buttonSex, Loc.S("Sex"));
        }

        private void LbMotherClick(object sender, EventArgs e)
        {
            if (_creature?.Mother == null) return;
            SelectCreature?.Invoke(_creature.Mother);
        }

        private void LbFatherClick(object sender, EventArgs e)
        {
            if (_creature?.Father == null) return;
            SelectCreature?.Invoke(_creature.Father);
        }
    }
}