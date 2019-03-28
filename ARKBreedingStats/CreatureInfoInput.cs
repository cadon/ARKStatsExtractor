﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class CreatureInfoInput : UserControl
    {
        public delegate void Add2LibraryClickedEventHandler(CreatureInfoInput sender);
        public event Add2LibraryClickedEventHandler Add2Library_Clicked;
        public delegate void Save2LibraryClickedEventHandler(CreatureInfoInput sender);
        public event Save2LibraryClickedEventHandler Save2Library_Clicked;
        public delegate void RequestParentListEventHandler(CreatureInfoInput sender);
        public event RequestParentListEventHandler ParentListRequested;
        public delegate void RequestCreatureDataEventHandler(CreatureInfoInput sender, bool patternEditor);
        public event RequestCreatureDataEventHandler CreatureDataRequested;
        public bool extractor;
        private Sex sex;
        public Guid CreatureGuid;
        public bool ArkIdImported;
        private CreatureStatus status;
        public bool parentListValid; // TODO change to parameter, if set to false, show n/a in the comboboxes
        private int speciesIndex;
        private ToolTip tt = new ToolTip();
        private bool updateMaturation;
        private List<Creature> _females;
        private List<Creature> _males;
        public List<string> NamesOfAllCreatures;
        private string[] _ownersTribes;
        private int[] regionColorIDs;
        private bool _tribeLock, _ownerLock;
        public long MotherArkId, FatherArkId; // is only used when importing creatures with set parents. these ids are set externally after the creature data is set in the infoinput

        public CreatureInfoInput()
        {
            InitializeComponent();
            speciesIndex = -1;
            textBoxName.Text = "";
            parentComboBoxMother.naLabel = " - " + Loc.s("Mother") + " n/a";
            parentComboBoxMother.Items.Add(" - " + Loc.s("Mother") + " n/a");
            parentComboBoxFather.naLabel = " - " + Loc.s("Father") + " n/a";
            parentComboBoxFather.Items.Add(" - " + Loc.s("Father") + " n/a");
            parentComboBoxMother.SelectedIndex = 0;
            parentComboBoxFather.SelectedIndex = 0;
            updateMaturation = true;
            regionColorIDs = new int[6];
            Cooldown = new DateTime(2000, 1, 1);
            Grown = new DateTime(2000, 1, 1);
            NamesOfAllCreatures = new List<string>();
        }

        private void buttonAdd2Library_Click(object sender, EventArgs e)
        {
            Add2Library_Clicked(this);
        }

        private void buttonSaveChanges_Click(object sender, EventArgs e)
        {
            Save2Library_Clicked(this);
        }

        public string CreatureName
        {
            get => textBoxName.Text;
            set
            {
                textBoxName.Text = value;
                textBoxName.BackColor = SystemColors.Window;
            }
        }
        public string CreatureOwner
        {
            get => textBoxOwner.Text;
            set => textBoxOwner.Text = value;
        }
        public string CreatureTribe
        {
            get => textBoxTribe.Text;
            set => textBoxTribe.Text = value;
        }
        public Sex CreatureSex
        {
            get => sex;
            set
            {
                sex = value;
                buttonSex.Text = Utils.sexSymbol(sex);
                buttonSex.BackColor = Utils.sexColor(sex);
                tt.SetToolTip(buttonSex, Loc.s("Sex") + ": " + Loc.s(sex.ToString()));
                cbNeutered.Text = Loc.s(sex == Sex.Female ? "Spayed" : "Neutered");
            }
        }
        public CreatureStatus CreatureStatus
        {
            get => status;
            set
            {
                status = value;
                buttonStatus.Text = Utils.statusSymbol(status);
                tt.SetToolTip(buttonStatus, Loc.s("Status") + ": " + Loc.s(status.ToString()));
            }
        }
        public string CreatureServer
        {
            get => cbServer.Text;
            set => cbServer.Text = value;
        }
        public Creature mother
        {
            get => parentComboBoxMother.SelectedParent;
            set
            {
                parentComboBoxMother.preselectedCreatureGuid = value?.guid ?? Guid.Empty;
                MotherArkId = 0;
            }
        }
        public Creature father
        {
            get => parentComboBoxFather.SelectedParent;
            set
            {
                parentComboBoxFather.preselectedCreatureGuid = value?.guid ?? Guid.Empty;
                FatherArkId = 0;
            }
        }
        public string CreatureNote
        {
            get => textBoxNote.Text;
            set => textBoxNote.Text = value;
        }

        private void buttonSex_Click(object sender, EventArgs e)
        {
            CreatureSex = Utils.nextSex(sex);
        }

        private void buttonStatus_Click(object sender, EventArgs e)
        {
            CreatureStatus = Utils.nextStatus(status);
        }

        public List<Creature>[] Parents
        {
            set
            {
                if (value != null)
                {
                    _females = parentComboBoxMother.ParentList = value[0];
                    _males = parentComboBoxFather.ParentList = value[1];
                }
            }
        }
        public List<int>[] ParentsSimilarities
        {
            set
            {
                if (value != null)
                {
                    parentComboBoxMother.parentsSimilarity = value[0];
                    parentComboBoxFather.parentsSimilarity = value[1];
                }
            }
        }

        public bool ButtonEnabled { set { btAdd2Library.Enabled = value; } }

        public bool ShowSaveButton
        {
            set
            {
                btSaveChanges.Visible = value;
                btAdd2Library.Location = new Point((value ? 154 : 88), btAdd2Library.Location.Y);
                btAdd2Library.Size = new Size((value ? 68 : 134), 37);
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            if (!parentListValid)
                ParentListRequested?.Invoke(this);
        }

        private void dhmsInputGrown_ValueChanged(object sender, TimeSpan ts)
        {
            if (updateMaturation && speciesIndex >= 0 && Values.V.species != null && Values.V.species[speciesIndex] != null)
            {
                updateMaturation = false;
                double maturation = 0;
                if (Values.V.species[speciesIndex].breeding != null && Values.V.species[speciesIndex].breeding.maturationTimeAdjusted > 0)
                {
                    maturation = 1 - dhmsInputGrown.Timespan.TotalSeconds / Values.V.species[speciesIndex].breeding.maturationTimeAdjusted;
                    if (maturation < 0) maturation = 0;
                    if (maturation > 1) maturation = 1;
                }
                nudMaturation.Value = (decimal)maturation * 100;

                updateMaturation = true;
            }
        }

        private void nudMaturation_ValueChanged(object sender, EventArgs e)
        {
            if (updateMaturation)
            {
                updateMaturation = false;
                if (Values.V.species[speciesIndex].breeding != null)
                {
                    dhmsInputGrown.Timespan = new TimeSpan(0, 0, (int)(Values.V.species[speciesIndex].breeding.maturationTimeAdjusted *
                            (1 - (double)nudMaturation.Value / 100)));
                    dhmsInputGrown.changed = true;
                }
                else dhmsInputGrown.Timespan = TimeSpan.Zero;
                updateMaturation = true;
            }
        }

        public DateTime Cooldown
        {
            get => dhmsInputCooldown.changed ? DateTime.Now.Add(dhmsInputCooldown.Timespan) : DateTime.Now;
            set
            {
                dhmsInputCooldown.Timespan = value - DateTime.Now;
                dhmsInputGrown_ValueChanged(dhmsInputGrown, dhmsInputGrown.Timespan);
            }
        }

        public DateTime Grown
        {
            get => dhmsInputGrown.changed ? DateTime.Now.Add(dhmsInputGrown.Timespan) : DateTime.Now;
            set => dhmsInputGrown.Timespan = value - DateTime.Now;
        }

        public void SetTimersToChanged()
        {
            dhmsInputCooldown.changed = true;
            dhmsInputGrown.changed = true;
        }

        public string[] AutocompleteOwnerList
        {
            set
            {
                var l = new AutoCompleteStringCollection();
                l.AddRange(value);
                textBoxOwner.AutoCompleteCustomSource = l;
            }
        }

        public string[] AutocompleteTribeList
        {
            set
            {
                var l = new AutoCompleteStringCollection();
                l.AddRange(value);
                textBoxTribe.AutoCompleteCustomSource = l;
            }
        }

        public string[] OwnersTribes
        {
            set => _ownersTribes = value;
        }

        public string[] ServersList
        {
            set
            {
                var l = new AutoCompleteStringCollection();
                l.AddRange(value);
                cbServer.AutoCompleteCustomSource = l;
                cbServer.Items.Clear();
                foreach (string s in value)
                    cbServer.Items.Add(s);
            }
        }

        public DateTime domesticatedAt
        {
            get => dateTimePickerAdded.Value;
            set => dateTimePickerAdded.Value = value < dateTimePickerAdded.MinDate ? dateTimePickerAdded.MinDate : value;
        }

        public bool Neutered
        {
            get => cbNeutered.Checked;
            set => cbNeutered.Checked = value;
        }

        public int MutationCounterMother
        {
            get => (int)nudMutationsMother.Value;
            set => nudMutationsMother.ValueSave = value;
        }

        public int MutationCounterFather
        {
            get => (int)nudMutationsFather.Value;
            set => nudMutationsFather.ValueSave = value;
        }

        public void SetArkId(long arkId, bool arkIdImported)
        {
            tbARKID.Text = arkId.ToString();
            ArkIdImported = arkIdImported;

            if (arkIdImported)
            {
                tbArkIdIngame.Text = Utils.ConvertImportedArkIdToIngameVisualization(arkId);
            }
            lbArkIdIngame.Visible = arkIdImported;
            tbArkIdIngame.Visible = arkIdImported;
        }

        public long ArkId
        {
            get
            {
                long.TryParse(tbARKID.Text, out long result);
                return result;
            }
        }

        public int[] RegionColors
        {
            get => regionColorChooser1.colorIDs;
            set
            {
                if (speciesIndex >= 0)
                {
                    regionColorIDs = (int[])value.Clone();
                    regionColorChooser1.setCreature(Values.V.speciesNames[speciesIndex], regionColorIDs);
                }
            }
        }

        public int SpeciesIndex
        {
            set
            {
                speciesIndex = value;
                bool breedingPossible = Values.V.species.Count > value && Values.V.species[speciesIndex].breeding != null;

                dhmsInputCooldown.Visible = breedingPossible;
                dhmsInputGrown.Visible = breedingPossible;
                nudMaturation.Visible = breedingPossible;
                lbGrownIn.Visible = breedingPossible;
                lbCooldown.Visible = breedingPossible;
                lbMaturationPerc.Visible = breedingPossible;
                nudMutationsMother.Visible = breedingPossible;
                nudMutationsFather.Visible = breedingPossible;
                lbMutations.Visible = breedingPossible;
                label11.Visible = breedingPossible;
                label12.Visible = breedingPossible;
                if (!breedingPossible)
                {
                    nudMaturation.Value = 0;
                    dhmsInputGrown.Timespan = TimeSpan.Zero;
                    dhmsInputCooldown.Timespan = TimeSpan.Zero;
                }
                RegionColors = new int[6];
            }
        }

        private void parentComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateMutations();
        }

        private void updateMutations()
        {
            // it's assumed that if a parent has a higher mutation-count than the current set one, the set one is not valid and will be updated
            int mutationsMo = (parentComboBoxMother.SelectedParent != null ? parentComboBoxMother.SelectedParent.Mutations : 0);
            int mutationsFa = (parentComboBoxFather.SelectedParent != null ? parentComboBoxFather.SelectedParent.Mutations : 0);

            if (mutationsMo > nudMutationsMother.Value)
            {
                nudMutationsMother.Value = mutationsMo;
            }
            if (mutationsFa > nudMutationsFather.Value)
            {
                nudMutationsFather.Value = mutationsFa;
            }
        }

        private void btnGenerateUniqueName_Click(object sender, EventArgs e)
        {
            if (speciesIndex >= 0 && speciesIndex < Values.V.species.Count)
            {
                CreatureDataRequested?.Invoke(this, false);
            }
        }

        private void btnGenerateUniqueName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                CreatureDataRequested?.Invoke(this, true); // TODO doesn't get called when right-clicking. Why?
            }
        }

        /// <summary>
        /// Generates a creature name with a given pattern
        /// </summary>
        public void generateCreatureName(Creature creature)
        {
            setCreatureData(creature);
            CreatureName = uiControls.NamePatterns.generateCreatureName(creature, _females, _males);
        }

        public void openNamePatternEditor(Creature creature)
        {
            setCreatureData(creature);
            var pe = new uiControls.PatternEditor(creature, _females, _males);
            if (pe.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.sequentialUniqueNamePattern = pe.NamePattern;
            }
        }

        private void setCreatureData(Creature cr)
        {
            cr.Mother = mother;
            cr.Father = father;
            cr.species = Values.V.species[speciesIndex].name;
            cr.sex = sex;
            cr.mutationsMaternal = MutationCounterMother;
            cr.mutationsPaternal = MutationCounterFather;
        }

        private void textBoxOwner_Leave(object sender, EventArgs e)
        {
            // if tribe is not yet given and player has a given tribe, set tribe
            if (textBoxTribe.Text.Length == 0)
            {
                int i = textBoxOwner.AutoCompleteCustomSource.IndexOf(textBoxOwner.Text);
                if (i >= 0 && i < _ownersTribes.Length)
                {
                    textBoxTribe.Text = _ownersTribes[i];
                }
            }
        }

        // if true the OCR will not change these fields
        public bool OwnerLock
        {
            get => _ownerLock;
            set
            {
                _ownerLock = value;
                textBoxOwner.BackColor = value ? Color.LightGray : SystemColors.Window;
            }
        }
        // if true the OCR will not change these fields
        public bool TribeLock
        {
            get => _tribeLock;
            set
            {
                _tribeLock = value;
                textBoxTribe.BackColor = value ? Color.LightGray : SystemColors.Window;
            }
        }

        private void lblOwner_Click(object sender, EventArgs e)
        {
            OwnerLock = !OwnerLock;
        }

        private void lblName_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text.Length > 0)
                Clipboard.SetText(textBoxName.Text);
        }

        private void btClearColors_Click(object sender, EventArgs e)
        {
            clearColors();
        }

        private void clearColors()
        {
            regionColorChooser1.Clear();
        }

        private void lblTribe_Click(object sender, EventArgs e)
        {
            TribeLock = !TribeLock;
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            // feedback if name already exists
            if (NamesOfAllCreatures != null && NamesOfAllCreatures.Contains(textBoxName.Text))
            {
                textBoxName.BackColor = Color.Khaki;
            }
            else
            {
                textBoxName.BackColor = SystemColors.Window;
            }
        }

        internal void Clear()
        {
            textBoxName.Clear();
            textBoxOwner.Clear();
            mother = null;
            father = null;
            MotherArkId = 0;
            FatherArkId = 0;
            parentComboBoxMother.Clear();
            parentComboBoxFather.Clear();
            textBoxNote.Clear();
            Cooldown = DateTime.Now;
            Grown = DateTime.Now;
            MutationCounterMother = 0;
            MutationCounterFather = 0;
            CreatureSex = Sex.Unknown;
            Neutered = false;
            clearColors();
            CreatureStatus = CreatureStatus.Available;
        }

        public void SetLocalizations()
        {
            Loc.ControlText(gbCreatureInfo);
            Loc.ControlText(lbName, "Name", tt);
            Loc.ControlText(lbOwner, "Owner", tt);
            Loc.ControlText(lbTribe, "Tribe", tt);
            Loc.ControlText(lbServer, "Server");
            Loc.ControlText(lbMother, "Mother");
            Loc.ControlText(lbFather, "Father");
            Loc.ControlText(lbNote, "Note");
            Loc.ControlText(lbCooldown, "cooldown");
            Loc.ControlText(lbGrownIn, "grownIn");
            lbMaturationPerc.Text = Loc.s("Maturation") + " [%]";
            Loc.ControlText(lbMutations, "Mutations");
            Loc.ControlText(lbSex, "Sex");
            Loc.ControlText(lbStatus, "Status");
            Loc.ControlText(btClearColors, "Colors", tt);
            Loc.ControlText(btSaveChanges);
            Loc.ControlText(btAdd2Library);
            //tooltips
            Loc.setToolTip(buttonSex, "Sex", tt);
            Loc.setToolTip(buttonStatus, "Status", tt);
            Loc.setToolTip(dateTimePickerAdded, "domesticatedAt", tt);
            Loc.setToolTip(nudMutationsMother, "mutationCounter", tt);
            Loc.setToolTip(btnGenerateUniqueName, tt);
        }
    }
}
