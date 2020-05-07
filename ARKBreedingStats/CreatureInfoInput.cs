using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class CreatureInfoInput : UserControl
    {
        public event Action<CreatureInfoInput> Add2Library_Clicked;
        public event Action<CreatureInfoInput> Save2Library_Clicked;
        public event Action<CreatureInfoInput> ParentListRequested;
        public delegate void RequestCreatureDataEventHandler(CreatureInfoInput sender, bool openPatternEditor, bool showDuplicateNameWarning, int namingPatternIndex);
        public event RequestCreatureDataEventHandler CreatureDataRequested;
        public bool extractor;
        private Sex sex;
        private CreatureFlags _creatureFlags;
        public Guid CreatureGuid;
        public bool ArkIdImported;
        private CreatureStatus creatureStatus;
        public bool parentListValid; // TODO change to parameter, if set to false, show n/a in the comboboxes
        private Species selectedSpecies;
        private ToolTip tt = new ToolTip();
        private bool updateMaturation;
        private List<Creature> _females;
        private List<Creature> _males;
        public List<string> NamesOfAllCreatures;
        private string[] _ownersTribes;
        private int[] regionColorIDs;
        private bool _tribeLock, _ownerLock;
        public long MotherArkId, FatherArkId; // is only used when importing creatures with set parents. these ids are set externally after the creature data is set in the infoinput
        /// <summary>
        /// True if creature is new, false if creature already exists
        /// </summary>
        private bool isNewCreature;

        public CreatureInfoInput()
        {
            InitializeComponent();
            selectedSpecies = null;
            textBoxName.Text = "";
            parentComboBoxMother.naLabel = " - " + Loc.s("Mother") + " n/a";
            parentComboBoxMother.Items.Add(" - " + Loc.s("Mother") + " n/a");
            parentComboBoxFather.naLabel = " - " + Loc.s("Father") + " n/a";
            parentComboBoxFather.Items.Add(" - " + Loc.s("Father") + " n/a");
            parentComboBoxMother.SelectedIndex = 0;
            parentComboBoxFather.SelectedIndex = 0;
            updateMaturation = true;
            regionColorIDs = new int[6];
            CooldownUntil = new DateTime(2000, 1, 1);
            GrowingUntil = new DateTime(2000, 1, 1);
            NamesOfAllCreatures = new List<string>();

            var namingPatternButtons = new List<Button> { btnGenerateUniqueName, btNamingPattern2, btNamingPattern3, btNamingPattern4, btNamingPattern5, btNamingPattern6 };
            for (int bi = 0; bi < namingPatternButtons.Count; bi++)
            {
                int localIndex = bi;
                // apply naming pattern
                namingPatternButtons[bi].Click += (s, e) =>
                {
                    if (selectedSpecies != null)
                    {
                        CreatureDataRequested?.Invoke(this, false, true, localIndex);
                    }
                };
                // open naming pattern editor
                namingPatternButtons[bi].MouseDown += (s, e) =>
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        CreatureDataRequested?.Invoke(this, true, false, localIndex);
                    }
                };
            }
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
                buttonSex.Text = Utils.SexSymbol(sex);
                buttonSex.BackColor = Utils.SexColor(sex);
                tt.SetToolTip(buttonSex, Loc.s("Sex") + ": " + Loc.s(sex.ToString()));
                cbNeutered.Text = Loc.s(sex == Sex.Female ? "Spayed" : "Neutered");
            }
        }

        public CreatureStatus CreatureStatus
        {
            get => creatureStatus;
            set
            {
                creatureStatus = value;
                buttonStatus.Text = Utils.StatusSymbol(creatureStatus);
                tt.SetToolTip(buttonStatus, Loc.s("Status") + ": " + Utils.StatusText(creatureStatus));
            }
        }

        public string CreatureServer
        {
            get => cbServer.Text;
            set => cbServer.Text = value;
        }

        public Creature Mother
        {
            get => parentComboBoxMother.SelectedParent;
            set
            {
                parentComboBoxMother.PreselectedCreatureGuid = value?.guid ?? Guid.Empty;
                MotherArkId = 0;
            }
        }

        public Creature Father
        {
            get => parentComboBoxFather.SelectedParent;
            set
            {
                parentComboBoxFather.PreselectedCreatureGuid = value?.guid ?? Guid.Empty;
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
            CreatureSex = Utils.NextSex(sex);
        }

        private void buttonStatus_Click(object sender, EventArgs e)
        {
            CreatureStatus = Utils.NextStatus(creatureStatus);
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

        public bool ButtonEnabled
        {
            set
            {
                btAdd2Library.Enabled = value;
                SetAdd2LibColor(value);
            }
        }

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
            if (updateMaturation && selectedSpecies != null)
            {
                updateMaturation = false;
                double maturation = 0;
                if (selectedSpecies.breeding != null && selectedSpecies.breeding.maturationTimeAdjusted > 0)
                {
                    maturation = 1 - dhmsInputGrown.Timespan.TotalSeconds / selectedSpecies.breeding.maturationTimeAdjusted;
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
                if (selectedSpecies.breeding != null)
                {
                    dhmsInputGrown.Timespan = new TimeSpan(0, 0, (int)(selectedSpecies.breeding.maturationTimeAdjusted *
                            (1 - (double)nudMaturation.Value / 100)));
                    dhmsInputGrown.changed = true;
                }
                else dhmsInputGrown.Timespan = TimeSpan.Zero;
                updateMaturation = true;
            }
        }

        /// <summary>
        /// DateTime when the cooldown of the creature is finished.
        /// </summary>
        public DateTime? CooldownUntil
        {
            get => dhmsInputCooldown.changed ? DateTime.Now.Add(dhmsInputCooldown.Timespan) : default(DateTime?);
            set
            {
                if (value.HasValue)
                {
                    dhmsInputCooldown.Timespan = value.Value - DateTime.Now;
                    dhmsInputGrown_ValueChanged(dhmsInputGrown, dhmsInputGrown.Timespan);
                }
            }
        }

        /// <summary>
        /// DateTime when the creature is mature.
        /// </summary>
        public DateTime? GrowingUntil
        {
            get => dhmsInputGrown.changed ? DateTime.Now.Add(dhmsInputGrown.Timespan) : default(DateTime?);
            set
            {
                if (value.HasValue)
                    dhmsInputGrown.Timespan = value.Value - DateTime.Now;
            }
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

        /// <summary>
        /// List of tribes of owners.
        /// </summary>
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

        /// <summary>
        /// DateTime when the creature was domesticated.
        /// </summary>
        public DateTime? DomesticatedAt
        {
            get => dateTimePickerAdded.Value;
            set
            {
                if (value.HasValue)
                    dateTimePickerAdded.Value = value.Value < dateTimePickerAdded.MinDate ? dateTimePickerAdded.MinDate : value.Value;
                else
                    dateTimePickerAdded.Value = dateTimePickerAdded.MinDate;
            }
        }

        /// <summary>
        /// Flags of the creature, e.g. if the creature is neutered.
        /// </summary>
        public CreatureFlags CreatureFlags
        {
            get
            {
                if (cbNeutered.Checked)
                    _creatureFlags |= CreatureFlags.Neutered;
                else _creatureFlags &= ~CreatureFlags.Neutered;
                return _creatureFlags;
            }
            set
            {
                _creatureFlags = value;
                cbNeutered.Checked = _creatureFlags.HasFlag(CreatureFlags.Neutered);
            }
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
            get => regionColorChooser1.ColorIDs;
            set
            {
                if (selectedSpecies != null)
                {
                    regionColorIDs = (int[])value?.Clone() ?? new int[6];
                    regionColorChooser1.SetSpecies(selectedSpecies, regionColorIDs);
                }
            }
        }

        public Species SelectedSpecies
        {
            set
            {
                selectedSpecies = value;
                bool breedingPossible = selectedSpecies.breeding != null;

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
            UpdateMutations();
            CalculateNewMutations();
        }

        /// <summary>
        /// It's assumed that if a parent has a higher mutation-count than the current set one, the set one is not valid and will be updated.
        /// </summary>
        private void UpdateMutations()
        {
            int? mutationsMo = parentComboBoxMother.SelectedParent?.Mutations;
            int? mutationsFa = parentComboBoxFather.SelectedParent?.Mutations;

            if (mutationsMo.HasValue && nudMutationsMother.Value > 0 && mutationsMo.Value > nudMutationsMother.Value)
            {
                nudMutationsMother.Value = mutationsMo.Value;
            }
            if (mutationsFa.HasValue && nudMutationsFather.Value > 0 && mutationsFa.Value > nudMutationsFather.Value)
            {
                nudMutationsFather.Value = mutationsFa.Value;
            }
        }

        private void btNamingPatternEditor_Click(object sender, EventArgs e)
        {
            CreatureDataRequested?.Invoke(this, true, false, 0);
        }

        /// <summary>
        /// Generates a creature name with a given pattern
        /// </summary>
        public void GenerateCreatureName(Creature creature, int[] speciesTopLevels, Dictionary<string, string> customReplacings, bool showDuplicateNameWarning, int namingPatternIndex)
        {
            SetCreatureData(creature);
            CreatureName = uiControls.NamePatterns.GenerateCreatureName(creature, _females, _males, speciesTopLevels, customReplacings, showDuplicateNameWarning, namingPatternIndex);
        }

        public void OpenNamePatternEditor(Creature creature, int[] speciesTopLevels, Dictionary<string, string> customReplacings, int namingPatternIndex, Action<uiControls.PatternEditor> reloadCallback)
        {
            SetCreatureData(creature);
            using (var pe = new uiControls.PatternEditor(creature, _females, _males, speciesTopLevels, customReplacings, namingPatternIndex, reloadCallback))
            {
                if (Properties.Settings.Default.PatternEditorLocation.X > -100000)
                    pe.Location = Properties.Settings.Default.PatternEditorLocation;
                if (Properties.Settings.Default.PatternEditorSize.Width > 50)
                    pe.Size = Properties.Settings.Default.PatternEditorSize;
                if (Properties.Settings.Default.PatternEditorSplitterDistance > 0)
                    pe.SplitterDistance = Properties.Settings.Default.PatternEditorSplitterDistance;
                if (pe.ShowDialog() == DialogResult.OK)
                {
                    var namingPatterns = Properties.Settings.Default.NamingPatterns ?? new string[6];
                    namingPatterns[namingPatternIndex] = pe.NamePattern;
                    Properties.Settings.Default.NamingPatterns = namingPatterns;
                }
                Properties.Settings.Default.PatternEditorLocation = pe.Location;
                Properties.Settings.Default.PatternEditorSize = pe.Size;
                Properties.Settings.Default.PatternEditorSplitterDistance = pe.SplitterDistance;
            }
        }

        /// <summary>
        /// Sets the data of the given creature to the values of the controls.
        /// </summary>
        /// <param name="cr"></param>
        private void SetCreatureData(Creature cr)
        {
            cr.Mother = Mother;
            cr.Father = Father;
            cr.sex = sex;
            cr.mutationsMaternal = MutationCounterMother;
            cr.mutationsPaternal = MutationCounterFather;
            cr.owner = CreatureOwner;
            cr.tribe = CreatureTribe;
            cr.server = CreatureServer;
            cr.flags = CreatureFlags;
            cr.colors = RegionColors;
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

        /// <summary>
        /// If true the OCR and import exported methods will not change the owner field.
        /// </summary>
        public bool OwnerLock
        {
            get => _ownerLock;
            set
            {
                _ownerLock = value;
                textBoxOwner.BackColor = value ? Color.LightGray : SystemColors.Window;
            }
        }

        /// <summary>
        /// If true the OCR and import exported methods will not change the tribe field.
        /// </summary>
        public bool TribeLock
        {
            get => _tribeLock;
            set
            {
                _tribeLock = value;
                textBoxTribe.BackColor = value ? Color.LightGray : SystemColors.Window;
            }
        }

        /// <summary>
        /// If set to true, it's assumed the creature is already existing.
        /// </summary>
        public bool UpdateExistingCreature
        {
            set
            {
                btAdd2Library.Text = value ?
                                     Loc.s("btUpdateLibraryCreature") :
                                     Loc.s("btAdd2Library");

                isNewCreature = !value;
                SetAdd2LibColor(btAdd2Library.Enabled);
            }
        }

        /// <summary>
        /// Timestamp when the creature was added to the library. Only relevant when creatures are already have been added and are edited.
        /// </summary>
        public DateTime? AddedToLibraryAt { get; internal set; }

        private void SetAdd2LibColor(bool buttonEnabled)
        {
            btAdd2Library.BackColor = !buttonEnabled
                ? SystemColors.Control
                : isNewCreature ? Color.LightGreen
                : Color.LightSkyBlue;
        }

        private void lblOwner_Click(object sender, EventArgs e)
        {
            OwnerLock = !OwnerLock;
        }

        private void lblName_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxName.Text))
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

        private void CalculateNewMutations()
        {
            int newMutations = 0;
            if (parentComboBoxMother.SelectedParent != null
                && nudMutationsMother.Value > parentComboBoxMother.SelectedParent.Mutations)
            {
                newMutations += (int)nudMutationsMother.Value - parentComboBoxMother.SelectedParent.Mutations;
            }
            if (parentComboBoxFather.SelectedParent != null
                && nudMutationsFather.Value > parentComboBoxFather.SelectedParent.Mutations)
            {
                newMutations += (int)nudMutationsFather.Value - parentComboBoxFather.SelectedParent.Mutations;
            }

            lbNewMutations.Text = "+" + newMutations + " mut";
            lbNewMutations.BackColor = newMutations != 0 ? Utils.MutationColor : SystemColors.Control;
        }

        private void NudMutations_ValueChanged(object sender, EventArgs e)
        {
            CalculateNewMutations();
        }

        internal void Clear()
        {
            textBoxName.Clear();
            textBoxOwner.Clear();
            Mother = null;
            Father = null;
            MotherArkId = 0;
            FatherArkId = 0;
            parentComboBoxMother.Clear();
            parentComboBoxFather.Clear();
            textBoxNote.Clear();
            CooldownUntil = DateTime.Now;
            GrowingUntil = DateTime.Now;
            MutationCounterMother = 0;
            MutationCounterFather = 0;
            CreatureSex = Sex.Unknown;
            CreatureFlags = CreatureFlags.None;
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
            Loc.setToolTip(dateTimePickerAdded, "addedAt", tt);
            Loc.setToolTip(nudMutationsMother, "mutationCounter", tt);
            Loc.setToolTip(nudMutationsFather, "mutationCounter", tt);

            var namingPatternButtons = new List<Button> { btnGenerateUniqueName, btNamingPattern2, btNamingPattern3, btNamingPattern4, btNamingPattern5, btNamingPattern6 };
            for (int bi = 0; bi < namingPatternButtons.Count; bi++)
                tt.SetToolTip(namingPatternButtons[bi], Loc.s("btnGenerateUniqueNameTT", false));
        }
    }
}
