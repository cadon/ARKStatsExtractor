using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.Library;
using ARKBreedingStats.NamePatterns;
using ARKBreedingStats.Properties;
using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.utils;

namespace ARKBreedingStats
{
    public partial class CreatureInfoInput : UserControl
    {
        public event Action<CreatureInfoInput> Add2LibraryClicked;
        public event Action<CreatureInfoInput> Save2LibraryClicked;
        public event Action<CreatureInfoInput> ParentListRequested;
        public event Form1.SetMessageLabelTextEventHandler SetMessageLabelText;

        /// <summary>
        /// Check for existing color id of the given region is requested. if the region is -1, all regions are requested.
        /// </summary>
        public event Action<CreatureInfoInput> ColorsChanged;
        public delegate void RequestCreatureDataEventHandler(CreatureInfoInput sender, bool openPatternEditor, bool updateInheritance, bool showDuplicateNameWarning, int namingPatternIndex, Creature alreadyExistingCreature);
        public event RequestCreatureDataEventHandler CreatureDataRequested;
        private Sex _sex;
        private CreatureFlags _creatureFlags;
        public Guid CreatureGuid;
        public bool ArkIdImported;
        private CreatureStatus _creatureStatus;
        public bool parentListValid; // TODO change to parameter, if set to false, show n/a in the comboBoxes
        private Species _selectedSpecies;
        private readonly ToolTip _tt;
        private bool _updateMaturation;
        private Creature[] _sameSpecies;
        public int LibraryCreatureCount;
        public List<string> NamesOfAllCreatures;
        private string[] _ownersTribes;
        private byte[] _regionColorIDs;
        private byte[] _colorIdsAlsoPossible;
        private bool _tribeLock, _ownerLock;
        public long MotherArkId, FatherArkId; // is only used when importing creatures with set parents. these ids are set externally after the creature data is set in the info input
        /// <summary>
        /// Creature if it's already existing in the library.
        /// </summary>
        private Creature _alreadyExistingCreature;

        private readonly Debouncer _parentsChangedDebouncer = new Debouncer();
        private readonly Debouncer _nameChangedDebouncer = new Debouncer();

        /// <summary>
        /// The pictureBox that displays the colored species dependent on the selected region colors.
        /// </summary>
        public PictureBox PbColorRegion;

        /// <summary>
        /// If false, the visualization of the colors and the image are not updated.
        /// </summary>
        public bool DontUpdateVisuals;

        /// <summary>
        /// Displays the parents and inherited stats.
        /// </summary>
        public ParentInheritance ParentInheritance;

        internal CreatureCollection.ColorExisting[] ColorAlreadyExistingInformation;

        private Button[] ButtonsNamingPattern => new[] { btnGenerateUniqueName, btNamingPattern2, btNamingPattern3, btNamingPattern4, btNamingPattern5, btNamingPattern6 };

        public CreatureInfoInput()
        {
            InitializeComponent();
            _selectedSpecies = null;
            textBoxName.Text = string.Empty;
            parentComboBoxMother.naLabel = " - " + Loc.S("Mother") + " n/a";
            parentComboBoxMother.Items.Add(" - " + Loc.S("Mother") + " n/a");
            parentComboBoxFather.naLabel = " - " + Loc.S("Father") + " n/a";
            parentComboBoxFather.Items.Add(" - " + Loc.S("Father") + " n/a");
            parentComboBoxMother.SelectedIndex = 0;
            parentComboBoxFather.SelectedIndex = 0;
            _updateMaturation = true;
            _regionColorIDs = new byte[Ark.ColorRegionCount];
            NamesOfAllCreatures = new List<string>();

            var namingPatternButtons = ButtonsNamingPattern;
            for (int bi = 0; bi < namingPatternButtons.Length; bi++)
            {
                int localIndex = bi;
                // apply naming pattern
                namingPatternButtons[bi].Click += (s, e) =>
                {
                    if (_selectedSpecies != null)
                    {
                        CreatureDataRequested?.Invoke(this, false, false, true, localIndex, _alreadyExistingCreature);
                    }
                };
                // open naming pattern editor
                namingPatternButtons[bi].MouseDown += (s, e) =>
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        CreatureDataRequested?.Invoke(this, true, false, false, localIndex, _alreadyExistingCreature);
                    }
                };
            }

            // set tooltips
            _tt = new ToolTip();
            _tt.SetToolTip(LbArkId, "The real Ark id of the creature, not directly shown in game.\nEach creature has its id stored in two 32 bit integers (id1, id2), this value is created by (id1 << 32) | id2");
            _tt.SetToolTip(LbArkIdIngame, "The id of the creature like it is shown in game.\nIt is created by the game by two 32 bit integers which are concatenated as strings.");

            regionColorChooser1.RegionColorChosen += UpdateRegionColorImage;
        }

        /// <summary>
        /// Updates the displayed colors of the creature.
        /// </summary>
        private void UpdateRegionColorImage()
        {
            ParentInheritance?.UpdateColors(RegionColors);
            ColorsChanged?.Invoke(this);
            PbColorRegion?.SetImageAndDisposeOld(CreatureColored.GetColoredCreature(RegionColors, _selectedSpecies, regionColorChooser1.ColorRegionsUseds, 256, onlyImage: true, creatureSex: CreatureSex));
        }

        internal void UpdateParentInheritances(Creature creature)
        {
            if (ParentInheritance == null) return;
            SetCreatureData(creature);
            ParentInheritance.SetCreatures(creature, Mother, Father);
        }

        private void buttonAdd2Library_Click(object sender, EventArgs e)
        {
            Add2LibraryClicked?.Invoke(this);
        }

        private void buttonSaveChanges_Click(object sender, EventArgs e)
        {
            Save2LibraryClicked?.Invoke(this);
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
            get => _sex;
            set
            {
                _sex = value;
                buttonSex.Text = Utils.SexSymbol(_sex);
                buttonSex.BackColor = Utils.SexColor(_sex);
                _tt.SetToolTip(buttonSex, $"{Loc.S("Sex")}: {Loc.S(_sex.ToString())}");
                cbNeutered.Text = Loc.S(_sex == Sex.Female ? "Spayed" : "Neutered");
                if (value == Sex.Female)
                {
                    _creatureFlags |= CreatureFlags.Female;
                    _creatureFlags &= ~CreatureFlags.Male;
                }
                else if (value == Sex.Male)
                {
                    _creatureFlags |= CreatureFlags.Male;
                    _creatureFlags &= ~CreatureFlags.Female;
                }
            }
        }

        public CreatureStatus CreatureStatus
        {
            get => _creatureStatus;
            set
            {
                _creatureStatus = value;
                buttonStatus.Text = Utils.StatusSymbol(_creatureStatus);
                _tt.SetToolTip(buttonStatus, $"{Loc.S("Status")}: {Utils.StatusText(_creatureStatus)}");
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
            CreatureSex = Utils.NextSex(_sex);
        }

        private void buttonStatus_Click(object sender, EventArgs e)
        {
            CreatureStatus = Utils.NextStatus(_creatureStatus);
        }

        public Creature[] CreaturesOfSameSpecies
        {
            set => _sameSpecies = value;
        }

        /// <summary>
        /// Possible parents of the current creature. Index 0: possible mothers, index 1: possible fathers. If species has no sex all parents are in index 0.
        /// </summary>
        public List<Creature>[] Parents
        {
            set
            {
                if (value == null) return;
                parentComboBoxMother.ParentList = value[0];
                parentComboBoxFather.ParentList = value[1] ?? value[0];
            }
        }

        public List<int>[] ParentsSimilarities
        {
            set
            {
                if (value == null) return;
                parentComboBoxMother.parentsSimilarity = value[0];
                parentComboBoxFather.parentsSimilarity = value[1] ?? value[0];
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
                btAdd2Library.Size = new Size((value ? Width / 2 : Width) - 10, btAdd2Library.Size.Height);
                btAdd2Library.Location = new Point(value ? Width / 2 + 6 : 6, btAdd2Library.Location.Y);
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            if (!parentListValid)
                ParentListRequested?.Invoke(this);
        }

        private void dhmsInputGrown_ValueChanged(object sender, TimeSpan ts)
        {
            if (!_updateMaturation || _selectedSpecies?.breeding == null) return;
            dhmsInputGrown.changed = true;
            SetMaturationAccordingToGrownUpIn();
        }

        private void SetMaturationAccordingToGrownUpIn()
        {
            double maturation = 1;
            if (_selectedSpecies.breeding != null && _selectedSpecies.breeding.maturationTimeAdjusted > 0)
            {
                maturation = 1 - dhmsInputGrown.Timespan.TotalSeconds / _selectedSpecies.breeding.maturationTimeAdjusted;
                if (maturation < 0) maturation = 0;
                if (maturation > 1) maturation = 1;
            }
            _updateMaturation = false;
            nudMaturation.Value = (decimal)maturation * 100;
            _updateMaturation = true;
        }

        private void nudMaturation_ValueChanged(object sender, EventArgs e)
        {
            if (!_updateMaturation) return;

            _updateMaturation = false;
            if (_selectedSpecies.breeding != null)
            {
                dhmsInputGrown.Timespan = TimeSpan.FromSeconds(_selectedSpecies.breeding.maturationTimeAdjusted * (1 - (double)nudMaturation.Value / 100));
                dhmsInputGrown.changed = true;
            }
            else dhmsInputGrown.Timespan = TimeSpan.Zero;
            _updateMaturation = true;
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
                if (!value.HasValue) return;
                dhmsInputGrown.Timespan = value.Value - DateTime.Now;
                SetMaturationAccordingToGrownUpIn();
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
                if (value == null) return;
                var l = new AutoCompleteStringCollection();
                l.AddRange(value);
                cbServer.AutoCompleteCustomSource = l;
                cbServer.Items.Clear();
                cbServer.Items.AddRange(value);
            }
        }

        /// <summary>
        /// DateTime when the creature was domesticated.
        /// </summary>
        public DateTime? DomesticatedAt
        {
            get => dateTimePickerDomesticatedAt.Value;
            set
            {
                if (value.HasValue)
                    dateTimePickerDomesticatedAt.Value = value.Value < dateTimePickerDomesticatedAt.MinDate ? dateTimePickerDomesticatedAt.MinDate : value.Value;
                else
                    dateTimePickerDomesticatedAt.Value = dateTimePickerDomesticatedAt.MinDate;
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
                if (CbMutagen.Checked)
                    _creatureFlags |= CreatureFlags.MutagenApplied;
                else _creatureFlags &= ~CreatureFlags.MutagenApplied;
                if (MutationCounterMother > 0 || MutationCounterFather > 0)
                    _creatureFlags |= CreatureFlags.Mutated;
                else _creatureFlags &= ~CreatureFlags.Mutated;

                return _creatureFlags;
            }
            set
            {
                _creatureFlags = value;
                cbNeutered.Checked = _creatureFlags.HasFlag(CreatureFlags.Neutered);
                CbMutagen.Checked = _creatureFlags.HasFlag(CreatureFlags.MutagenApplied);
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
            ArkIdImported = arkIdImported;

            if (arkIdImported)
            {
                TbArkIdIngame.Text = Utils.ConvertImportedArkIdToIngameVisualization(arkId);
                TbArkId.Text = arkId.ToString();
            }
            else
            {
                TbArkIdIngame.Text = arkId.ToString();
            }
            // if the creature is imported, the id is considered to be correct and the user should not change it.
            TbArkIdIngame.ReadOnly = arkIdImported;
            LbArkId.Visible = arkIdImported;
            TbArkId.Visible = arkIdImported;
        }

        public long ArkId
        {
            get
            {
                long.TryParse(ArkIdImported ? TbArkId.Text : TbArkIdIngame.Text, out long result);
                return result;
            }
        }

        public byte[] RegionColors
        {
            get => DontUpdateVisuals ? _regionColorIDs : regionColorChooser1.ColorIds;
            set
            {
                if (_selectedSpecies == null) return;
                _regionColorIDs = (byte[])value?.Clone() ?? new byte[Ark.ColorRegionCount];
                if (DontUpdateVisuals) return;
                regionColorChooser1.SetSpecies(_selectedSpecies, _regionColorIDs);
                UpdateRegionColorImage();
            }
        }

        public byte[] ColorIdsAlsoPossible
        {
            get
            {
                var arr = DontUpdateVisuals ? _colorIdsAlsoPossible : regionColorChooser1.ColorIdsAlsoPossible;
                if (arr == null) return null;

                // if array is empty, return null
                var isEmpty = true;
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }

                return isEmpty ? null : arr;
            }
            set
            {
                if (_selectedSpecies == null) return;
                _colorIdsAlsoPossible = (byte[])value?.Clone() ?? new byte[Ark.ColorRegionCount];
                if (DontUpdateVisuals) return;
                regionColorChooser1.ColorIdsAlsoPossible = _colorIdsAlsoPossible;
            }
        }

        public Species SelectedSpecies
        {
            set
            {
                _selectedSpecies = value;
                if (DontUpdateVisuals) return;
                bool breedingPossible = _selectedSpecies.breeding != null;

                dhmsInputCooldown.Visible = breedingPossible;
                dhmsInputGrown.Visible = breedingPossible;
                nudMaturation.Visible = breedingPossible;
                lbGrownIn.Visible = breedingPossible;
                lbCooldown.Visible = breedingPossible;
                lbMaturationPerc.Visible = breedingPossible;
                if (!breedingPossible)
                {
                    nudMaturation.Value = 1;
                    dhmsInputGrown.Timespan = TimeSpan.Zero;
                    dhmsInputCooldown.Timespan = TimeSpan.Zero;
                }
                RegionColors = null;
            }
        }

        private void parentComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMutations();
            CalculateNewMutations();
            if (ParentInheritance != null)
                _parentsChangedDebouncer.Debounce(100, ParentsChanged, Dispatcher.CurrentDispatcher);
        }

        private void ParentsChanged()
            => CreatureDataRequested?.Invoke(this, false, true, false, 0, _alreadyExistingCreature);

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
            CreatureDataRequested?.Invoke(this, true, false, false, 0, _alreadyExistingCreature);
        }

        /// <summary>
        /// Generates a creature name with a given pattern
        /// </summary>
        public void GenerateCreatureName(Creature creature, Creature alreadyExistingCreature, int[] speciesTopLevels, int[] speciesLowestLevels, Dictionary<string, string> customReplacings, bool showDuplicateNameWarning, int namingPatternIndex)
        {
            SetCreatureData(creature);
            CreatureName = NamePattern.GenerateCreatureName(creature, alreadyExistingCreature, _sameSpecies, speciesTopLevels, speciesLowestLevels, customReplacings,
                showDuplicateNameWarning, namingPatternIndex, false, colorsExisting: ColorAlreadyExistingInformation, libraryCreatureCount: LibraryCreatureCount);
            if (CreatureName.Length > Ark.MaxCreatureNameLength)
                SetMessageLabelText?.Invoke($"The generated name is longer than {Ark.MaxCreatureNameLength} characters, the name will look like this in game:\r\n" + CreatureName.Substring(0, Ark.MaxCreatureNameLength), MessageBoxIcon.Error);
        }

        public void OpenNamePatternEditor(Creature creature, int[] speciesTopLevels, int[] speciesLowestLevels, Dictionary<string, string> customReplacings, int namingPatternIndex, Action<PatternEditor> reloadCallback)
        {
            if (!parentListValid)
                ParentListRequested?.Invoke(this);
            using (var pe = new PatternEditor(creature, _sameSpecies, speciesTopLevels, speciesLowestLevels, ColorAlreadyExistingInformation, customReplacings, namingPatternIndex, reloadCallback, LibraryCreatureCount))
            {
                if (pe.ShowDialog() == DialogResult.OK)
                {
                    var namingPatterns = Settings.Default.NamingPatterns ?? new string[6];
                    namingPatterns[namingPatternIndex] = pe.NamePattern;
                    Settings.Default.NamingPatterns = namingPatterns;
                    Settings.Default.PatternNameToClipboardAfterManualApplication = pe.PatternNameToClipboardAfterManualApplication;
                }

                (Settings.Default.PatternEditorFormRectangle, _) = Utils.GetWindowRectangle(pe);
                Settings.Default.PatternEditorSplitterDistance = pe.SplitterDistance;
            }
        }

        /// <summary>
        /// Sets the data of the given creature to the values of the controls.
        /// </summary>
        public void SetCreatureData(Creature cr)
        {
            cr.name = CreatureName;
            cr.sex = _sex;
            cr.owner = CreatureOwner;
            cr.tribe = CreatureTribe;
            cr.server = CreatureServer;
            cr.note = CreatureNote;
            cr.flags = CreatureFlags;
            cr.Status = CreatureStatus;
            cr.Mother = Mother;
            cr.Father = Father;
            cr.mutationsMaternal = MutationCounterMother;
            cr.mutationsPaternal = MutationCounterFather;
            cr.colors = RegionColors;
            cr.ColorIdsAlsoPossible = ColorIdsAlsoPossible;
            cr.cooldownUntil = CooldownUntil;
            if (GrowingUntil != null) // if growing was not changed, don't change that value, growing could be paused
                cr.growingUntil = GrowingUntil;
            cr.domesticatedAt = DomesticatedAt;
            cr.ArkId = ArkId;
            cr.InitializeArkInGame();
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

        private bool _lockServer;
        /// <summary>
        /// If true the importing will not change the server field.
        /// </summary>
        public bool LockServer
        {
            get => _lockServer;
            set
            {
                _lockServer = value;
                cbServer.BackColor = value ? Color.LightGray : SystemColors.Window;
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
        public Creature AlreadyExistingCreature
        {
            set
            {
                btAdd2Library.Text = value != null ?
                                     Loc.S("btUpdateLibraryCreature") :
                                     Loc.S("btAdd2Library");

                _alreadyExistingCreature = value;
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
                : _alreadyExistingCreature != null ? Color.LightGreen
                : Color.LightSkyBlue;
        }

        private void lblOwner_Click(object sender, EventArgs e) => OwnerLock = !OwnerLock;

        private void lbServer_Click(object sender, EventArgs e) => LockServer = !LockServer;

        private void lblName_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxName.Text))
                Clipboard.SetText(textBoxName.Text);
        }

        private void btClearColors_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == (Keys.Control | Keys.Shift))
                regionColorChooser1.RandomColors();
            else if ((ModifierKeys & Keys.Control) != 0)
                regionColorChooser1.RandomNaturalColors(_selectedSpecies);
            else
                ClearColors();
        }

        private void ClearColors()
        {
            regionColorChooser1.Clear();
        }

        private void lblTribe_Click(object sender, EventArgs e)
        {
            TribeLock = !TribeLock;
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            _nameChangedDebouncer.Debounce(500, CheckIfNameAlreadyExists, Dispatcher.CurrentDispatcher);
        }

        private void CheckIfNameAlreadyExists()
        {
            // feedback if name already exists
            if (!string.IsNullOrEmpty(textBoxName.Text) && NamesOfAllCreatures != null && NamesOfAllCreatures.Contains(textBoxName.Text))
            {
                textBoxName.BackColor = Color.Khaki;
                _tt.SetToolTip(textBoxName, Loc.S("nameAlreadyExistsInLibrary"));
            }
            else
            {
                textBoxName.BackColor = SystemColors.Window;
                _tt.SetToolTip(textBoxName, null);
            }
        }

        private void CalculateNewMutations()
        {
            int newMutations = 0;
            if (parentComboBoxMother.SelectedParent != null)
                newMutations += NewMutations(parentComboBoxMother.SelectedParent.Mutations, (int)nudMutationsMother.Value);
            if (parentComboBoxFather.SelectedParent != null)
                newMutations += NewMutations(parentComboBoxFather.SelectedParent.Mutations, (int)nudMutationsFather.Value);

            int NewMutations(int mutationCountParent, int mutationCountChild)
            {
                var newMutationsFromParent = mutationCountChild - mutationCountParent;
                if (newMutationsFromParent > 0 && newMutationsFromParent <= Ark.MutationRolls)
                    return mutationCountChild - mutationCountParent;
                return 0;
            }

            lbNewMutations.Text = $"+{newMutations} mut";
            lbNewMutations.BackColor = newMutations != 0 ? Utils.MutationColor : SystemColors.Control;
        }

        private void NudMutations_ValueChanged(object sender, EventArgs e)
        {
            CalculateNewMutations();
        }

        private void BtSaveOTSPreset_Click(object sender, EventArgs e)
        {
            Settings.Default.DefaultOwnerName = CreatureOwner;
            Settings.Default.DefaultTribeName = CreatureTribe;
            Settings.Default.DefaultServerName = CreatureServer;
        }

        private void BtApplyOTSPreset_Click(object sender, EventArgs e)
        {
            CreatureOwner = Settings.Default.DefaultOwnerName;
            CreatureTribe = Settings.Default.DefaultTribeName;
            CreatureServer = Settings.Default.DefaultServerName;
        }

        /// <summary>
        /// Sets the background of the naming pattern buttons to indicate if they contain a pattern or are empty.
        /// </summary>
        internal void SetNamePatternButtons(string[] patterns)
        {
            if (patterns == null) return;
            var namingPatternButtons = ButtonsNamingPattern;
            var l = Math.Min(namingPatternButtons.Length, patterns.Length);
            for (int i = 0; i < namingPatternButtons.Length; i++)
            {
                namingPatternButtons[i].BackColor = (patterns?.Length ?? 0) > i && !string.IsNullOrWhiteSpace(patterns[i])
                    ? Color.FromArgb(150, 110, 255, 104)
                    : Color.Transparent;
            }
        }

        internal void Clear(bool keepGeneralInfo = false)
        {
            textBoxName.Clear();
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
            ClearColors();
            CreatureStatus = CreatureStatus.Available;
            ParentInheritance?.SetCreatures();
            SetRegionColorsExisting();
            if (!keepGeneralInfo)
            {
                textBoxOwner.Clear();
            }
        }

        public void SetLocalizations()
        {
            Loc.ControlText(gbCreatureInfo);
            Loc.ControlText(lbName, "Name", _tt);
            Loc.ControlText(lbOwner, "Owner", _tt);
            Loc.ControlText(lbTribe, "Tribe", _tt);
            Loc.ControlText(lbServer, "Server", _tt);
            Loc.ControlText(lbMother, "Mother");
            Loc.ControlText(lbFather, "Father");
            Loc.ControlText(lbNote, "Note");
            Loc.ControlText(lbCooldown, "cooldown");
            Loc.ControlText(lbGrownIn, "grownIn");
            lbMaturationPerc.Text = $"{Loc.S("Maturation")} [%]";
            Loc.ControlText(lbMutations, "Mutations");
            Loc.ControlText(lbSex, "Sex");
            Loc.ControlText(cbNeutered, _sex == Sex.Female ? "Spayed" : "Neutered");
            Loc.ControlText(lbStatus, "Status");
            Loc.ControlText(btClearColors, "clearColors");
            _tt.SetToolTip(btClearColors, Loc.S("clearColors") + "\n" + Loc.S("holdCtrlForRandomColors"));
            Loc.ControlText(btSaveChanges);
            Loc.ControlText(btAdd2Library);
            //tooltips
            Loc.SetToolTip(buttonSex, "Sex", _tt);
            Loc.SetToolTip(buttonStatus, "Status", _tt);
            Loc.SetToolTip(dateTimePickerDomesticatedAt, "domesticatedAt", _tt);
            Loc.SetToolTip(nudMutationsMother, "mutationCounter", _tt);
            Loc.SetToolTip(nudMutationsFather, "mutationCounter", _tt);
            Loc.ControlText(BtApplyOTSPreset, _tt);
            Loc.ControlText(BtSaveOTSPreset, _tt);

            var namingPatternButtons = new List<Button> { btnGenerateUniqueName, btNamingPattern2, btNamingPattern3, btNamingPattern4, btNamingPattern5, btNamingPattern6 };
            for (int bi = 0; bi < namingPatternButtons.Count; bi++)
                _tt.SetToolTip(namingPatternButtons[bi], Loc.S("btnGenerateUniqueNameTT", false));
        }

        internal (bool newInRegion, bool newInSpecies) SetRegionColorsExisting(CreatureCollection.ColorExisting[] colorAlreadyAvailable = null)
        {
            regionColorChooser1.SetRegionColorsExisting(colorAlreadyAvailable);
            LbColorNewInRegion.Visible = regionColorChooser1.ColorNewInRegion;
            LbColorNewInSpecies.Visible = regionColorChooser1.ColorNewInSpecies;
            return (regionColorChooser1.ColorNewInRegion, regionColorChooser1.ColorNewInSpecies);
        }
    }
}
