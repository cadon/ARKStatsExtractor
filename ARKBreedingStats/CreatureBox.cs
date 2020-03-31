using ARKBreedingStats.library;
using ARKBreedingStats.Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class CreatureBox : UserControl
    {
        private Creature creature;
        public event Action<Creature, bool, bool> Changed;
        public event Action<Creature> GiveParents;
        private Sex sex;
        private CreatureStatus creatureStatus;
        public List<Creature>[] parentList; // all creatures that could be parents (i.e. same species, separated by sex)
        public List<int>[] parentListSimilarity; // for all possible parents the number of equal stats (to find the parents easier)
        private bool[] colorRegionUseds;
        private CreatureCollection cc;
        ToolTip tt = new ToolTip();

        public CreatureBox()
        {
            InitializeVars();
        }

        public CreatureBox(Creature creature)
        {
            InitializeVars();
            SetCreature(creature);
        }

        private void InitializeVars()
        {
            InitializeComponent();
            creature = null;
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
            tt.SetToolTip(buttonStatus, "Status: Available, Unavailable, Dead");
        }

        public void SetCreature(Creature creature)
        {
            Clear();
            this.creature = creature;
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
            SuspendLayout();
            if (creature != null)
            {
                if (panel1.Visible)
                {
                    CloseSettings(false);
                }
                else
                {
                    checkBoxIsBred.Checked = creature.isBred;
                    panelParents.Visible = creature.isBred;
                    if (creature.isBred)
                        PopulateParentsList();
                    textBoxName.Text = creature.name;
                    textBoxOwner.Text = creature.owner;
                    textBoxNote.Text = creature.note;
                    sex = creature.sex;
                    buttonSex.Text = Utils.sexSymbol(sex);
                    creatureStatus = creature.status;
                    buttonStatus.Text = Utils.statusSymbol(creatureStatus);
                    textBoxName.SelectAll();
                    textBoxName.Focus();
                    panel1.Visible = true;
                }
            }
            ResumeLayout();
        }

        private void PopulateParentsList()
        {
            if (parentList[0] == null || parentList[1] == null)
            {
                GiveParents(creature);

                parentComboBoxMother.parentsSimilarity = parentListSimilarity[0];
                parentComboBoxMother.ParentList = parentList[0];
                parentComboBoxMother.PreselectedCreatureGuid = creature.motherGuid;
                parentComboBoxFather.parentsSimilarity = parentListSimilarity[1];
                parentComboBoxFather.ParentList = parentList[1];
                parentComboBoxFather.PreselectedCreatureGuid = creature.fatherGuid;
            }
        }

        public void UpdateLabel()
        {
            labelParents.Text = "";
            if (creature != null)
            {
                groupBox1.Text = $"{creature.name} (Lvl {creature.Level}/{creature.LevelHatched + cc.maxDomLevel})";
                if (creature.Mother != null || creature.Father != null)
                {
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
                    labelParents.Text = "found wild " + creature.levelFound + (creature.tamingEff >= 0 ? ", tamed with TE: " + (creature.tamingEff * 100).ToString("N1") + "%" : ", TE unknown.");
                }
                statsDisplay1.SetCreatureValues(creature);
                labelNotes.Text = creature.note;
                labelSpecies.Text = creature.Species.name;
                pictureBox1.Image = CreatureColored.getColoredCreature(creature.colors, creature.Species, colorRegionUseds);
                tt.SetToolTip(pictureBox1, CreatureColored.RegionColorInfo(creature.Species, creature.colors)
                    + "\n\nClick to copy creature infos as image to the clipboard");
                pictureBox1.Visible = true;
            }
        }

        private void CloseSettings(bool save)
        {
            panel1.Visible = false;
            if (save)
            {
                SuspendLayout();
                creature.name = textBoxName.Text;
                creature.sex = sex;
                creature.owner = textBoxOwner.Text;
                Creature parent = null;
                if (checkBoxIsBred.Checked)
                    parent = parentComboBoxMother.SelectedParent;
                creature.motherGuid = parent?.guid ?? Guid.Empty;
                bool parentsChanged = false;
                if (creature.Mother != parent)
                {
                    creature.Mother = parent;
                    parentsChanged = true;
                }
                parent = null;
                if (checkBoxIsBred.Checked)
                    parent = parentComboBoxFather.SelectedParent;
                creature.fatherGuid = parent?.guid ?? Guid.Empty;
                if (creature.Father != parent)
                {
                    creature.Father = parent;
                    parentsChanged = true;
                }
                if (parentsChanged)
                    creature.RecalculateAncestorGenerations();

                creature.isBred = checkBoxIsBred.Checked;

                creature.note = textBoxNote.Text;
                bool creatureStatusChanged = (creature.status != creatureStatus);
                creature.status = creatureStatus;

                Changed(creature, creatureStatusChanged, true);
                UpdateLabel();
                ResumeLayout();
            }
        }

        // call this function to clear all contents of this element
        public void Clear()
        {
            parentComboBoxMother.Items.Clear();
            parentComboBoxFather.Items.Clear();
            parentList = new List<Creature>[2];
            CloseSettings(false);
            groupBox1.Text = "";
            creature = null;
            labelParents.Text = "";
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
            sex = Utils.nextSex(sex);
            buttonSex.Text = Utils.sexSymbol(sex);
        }

        private void buttonStatus_Click(object sender, EventArgs e)
        {
            creatureStatus = Utils.nextStatus(creatureStatus);
            buttonStatus.Text = Utils.statusSymbol(creatureStatus);
        }

        private void checkBoxIsBred_CheckedChanged(object sender, EventArgs e)
        {
            panelParents.Visible = checkBoxIsBred.Checked;
            if (checkBoxIsBred.Checked)
                PopulateParentsList();
        }

        private void RegionColorChooser1_RegionColorChosen()
        {
            creature.colors = regionColorChooser1.ColorIDs;
            pictureBox1.Image = CreatureColored.getColoredCreature(creature.colors, creature.Species, colorRegionUseds);
            Changed(creature, false, false);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            creature?.ExportInfoGraphicToClipboard(cc);
        }
    }
}