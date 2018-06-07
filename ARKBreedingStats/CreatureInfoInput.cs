using System;
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
        private CreatureStatus status;
        public bool parentListValid;
        private int speciesIndex;
        private ToolTip tt = new ToolTip();
        private bool mutationManuallyChanged;
        private bool updateMaturation;
        private List<Creature> _females;
        private List<Creature> _males;
        private string[] _ownersTribes;
        private int[] regionColorIDs;
        private bool _tribeLock, _ownerLock;

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
            get { return textBoxName.Text; }
            set { textBoxName.Text = value; }
        }
        public string CreatureOwner
        {
            get { return textBoxOwner.Text; }
            set { textBoxOwner.Text = value; }
        }
        public string CreatureTribe
        {
            get { return textBoxTribe.Text; }
            set { textBoxTribe.Text = value; }
        }
        public Sex CreatureSex
        {
            get { return sex; }
            set
            {
                sex = value;
                buttonSex.Text = Utils.sexSymbol(sex);
                buttonSex.BackColor = Utils.sexColor(sex);
                tt.SetToolTip(buttonSex, Loc.s("Sex") + ": " + Loc.s(sex.ToString()));
                if (sex == Sex.Female)
                    cbNeutered.Text = Loc.s("Spayed");
                else
                    cbNeutered.Text = Loc.s("Neutered");
            }
        }
        public CreatureStatus CreatureStatus
        {
            get { return status; }
            set
            {
                status = value;
                buttonStatus.Text = Utils.statusSymbol(status);
                tt.SetToolTip(buttonStatus, Loc.s("Status") + ": " + Loc.s(status.ToString()));
            }
        }
        public string CreatureServer
        {
            get { return cbServer.Text; }
            set { cbServer.Text = value; }
        }
        public Creature mother
        {
            get
            {
                return parentComboBoxMother.SelectedParent;
            }
            set
            {
                parentComboBoxMother.preselectedCreatureGuid = (value == null ? Guid.Empty : value.guid);
            }
        }
        public Creature father
        {
            get
            {
                return parentComboBoxFather.SelectedParent;
            }
            set
            {
                parentComboBoxFather.preselectedCreatureGuid = (value == null ? Guid.Empty : value.guid);
            }
        }
        public Guid motherId
        {
            get { return parentComboBoxMother.preselectedCreatureGuid; }
            set { parentComboBoxMother.preselectedCreatureGuid = value; }
        }
        public Guid fatherId
        {
            get { return parentComboBoxFather.preselectedCreatureGuid; }
            set { parentComboBoxFather.preselectedCreatureGuid = value; }
        }
        public string CreatureNote
        {
            get { return textBoxNote.Text; }
            set { textBoxNote.Text = value; }
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
                    dhmsInputGrown.Timespan = new TimeSpan(0, 0, (int)(Values.V.species[speciesIndex].breeding.maturationTimeAdjusted * (1 - (double)nudMaturation.Value / 100)));
                    dhmsInputGrown.changed = true;
                }
                else dhmsInputGrown.Timespan = TimeSpan.Zero;
                updateMaturation = true;
            }
        }

        public DateTime Cooldown
        {
            set
            {
                dhmsInputCooldown.Timespan = value - DateTime.Now;
                dhmsInputGrown_ValueChanged(dhmsInputGrown, dhmsInputGrown.Timespan);
            }
            get { return dhmsInputCooldown.changed ? DateTime.Now.Add(dhmsInputCooldown.Timespan) : DateTime.Now; }
        }

        public DateTime Grown
        {
            set { dhmsInputGrown.Timespan = value - DateTime.Now; }
            get { return dhmsInputGrown.changed ? DateTime.Now.Add(dhmsInputGrown.Timespan) : DateTime.Now; }
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
            set
            {
                _ownersTribes = value;
            }
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
            set
            {
                if (value < dateTimePickerAdded.MinDate)
                    dateTimePickerAdded.Value = dateTimePickerAdded.MinDate;
                else
                    dateTimePickerAdded.Value = value;
            }
            get { return dateTimePickerAdded.Value; }
        }

        public bool Neutered
        {
            set { cbNeutered.Checked = value; }
            get { return cbNeutered.Checked; }
        }

        public int MutationCounterMother
        {
            set
            {
                nudMutationsMother.ValueSave = value;
                mutationManuallyChanged = false;
            }
            get { return (int)nudMutationsMother.Value; }
        }

        public int MutationCounterFather
        {
            set
            {
                nudMutationsFather.ValueSave = value;
                mutationManuallyChanged = false;
            }
            get { return (int)nudMutationsFather.Value; }
        }

        public int[] RegionColors
        {
            set
            {
                if (speciesIndex >= 0)
                {
                    regionColorIDs = (int[])value.Clone();
                    regionColorChooser1.setCreature(Values.V.speciesNames[speciesIndex], regionColorIDs);
                }
            }
            get { return regionColorChooser1.colorIDs; }
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
            if (!mutationManuallyChanged)
            {
                nudMutationsMother.Value = (parentComboBoxMother.SelectedParent != null ? parentComboBoxMother.SelectedParent.Mutations : 0);
                nudMutationsFather.Value = (parentComboBoxFather.SelectedParent != null ? parentComboBoxFather.SelectedParent.Mutations : 0);
                mutationManuallyChanged = false;
            }
        }

        private void numericUpDownMutations_ValueChanged(object sender, EventArgs e)
        {
            mutationManuallyChanged = true;
        }

        private void btnGenerateUniqueName_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Left)
            {
                if (speciesIndex >= 0 && speciesIndex < Values.V.species.Count)
                {
                    CreatureDataRequested?.Invoke(this, false);
                }
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
            cr.mutationsMaternal = (int)nudMutationsMother.Value;
            cr.mutationsPaternal = (int)nudMutationsFather.Value;
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
            get { return _ownerLock; }
            set
            {
                _ownerLock = value;
                textBoxOwner.BackColor = value ? Color.LightGray : SystemColors.Window;
            }
        }
        // if true the OCR will not change these fields
        public bool TribeLock
        {
            get { return _tribeLock; }
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

        public void clearColors()
        {
            regionColorChooser1.Clear();
        }

        private void lblTribe_Click(object sender, EventArgs e)
        {
            TribeLock = !TribeLock;
        }

        internal void Clear()
        {
            textBoxName.Clear();
            textBoxOwner.Clear();
            mother = null;
            father = null;
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
