using ARKBreedingStats.importExported;
using ARKBreedingStats.library;
using ARKBreedingStats.Library;
using ARKBreedingStats.ocr;
using ARKBreedingStats.settings;
using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ARKBreedingStats.mods;
using ARKBreedingStats.NamePatterns;
using ARKBreedingStats.utils;
using static ARKBreedingStats.settings.Settings;
using Color = System.Drawing.Color;

namespace ARKBreedingStats
{
    public partial class Form1 : Form
    {
        private CreatureCollection _creatureCollection = new CreatureCollection();
        private string _currentFileName;
        private bool _collectionDirty;

        /// <summary>
        /// List of all top stats per species
        /// </summary>
        private readonly Dictionary<Species, int[]> _topLevels = new Dictionary<Species, int[]>();

        private readonly Dictionary<Species, int[]> _lowestLevels = new Dictionary<Species, int[]>();
        private readonly StatIO[] _statIOs = new StatIO[Stats.StatsCount];
        private readonly StatIO[] _testingIOs = new StatIO[Stats.StatsCount];
        private int _activeStatIndex = -1;

        private readonly bool[]
            _activeStats =
            {
                true, true, true, true, true, true, true, true, true, true, true, true
            }; // stats used by the creature (some don't use oxygen)

        private bool _libraryNeedsUpdate;

        public delegate void
            CollectionChangedEventHandler(bool changed = true,
                Species species = null, // if null is passed for species, breeding-related controls are not updated
                bool triggeredByFileWatcher = false);

        public delegate void SetMessageLabelTextEventHandler(string text = null,
            MessageBoxIcon icon = MessageBoxIcon.None, string path = null, string clipboardContent = null);

        private bool _updateTorporInTester;
        private bool _filterListAllowed;

        /// <summary>
        /// The stat indices that are considered for color highlighting and topness calculation.
        /// </summary>
        private readonly bool[] _considerStatHighlight = new bool[Stats.StatsCount];

        private DateTime _lastAutoSaveBackup;
        private Creature _creatureTesterEdit;
        private int _hiddenLevelsCreatureTester;
        private FileSync _fileSync;
        private FileWatcherExports _fileWatcherExports;
        private readonly Extraction _extractor = new Extraction();
        private SpeechRecognition _speechRecognition;
        private readonly Timer _timerGlobal = new Timer();
        private ExportedCreatureList _exportedCreatureList;
        private ExportedCreatureControl _exportedCreatureControl;
        private readonly ToolTip _tt;
        private bool _reactOnCreatureSelectionChange;
        private bool _clearExtractionCreatureData;

        /// <summary>
        /// The last tab-page opened in the settings.
        /// </summary>
        private SettingsTabPages _settingsLastTabPage;

        /// <summary>
        /// Custom replacings for species names used in naming patterns.
        /// </summary>
        private Dictionary<string, string> _customReplacingNamingPattern;

        // 0: Health
        // 1: Stamina / Charge Capacity
        // 2: Torpidity
        // 3: Oxygen / Charge Regeneration
        // 4: Food
        // 5: Water
        // 6: Temperature
        // 7: Weight
        // 8: MeleeDamageMultiplier / Charge Emission Range
        // 9: SpeedMultiplier
        // 10: TemperatureFortitude
        // 11: CraftingSpeedMultiplier

        // OCR stuff
        private ARKOverlay _overlay;
        private static double[] _lastOcrValues;
        private Species _lastOcrSpecies;

        public Form1()
        {
            // load settings of older version if possible after an upgrade
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            // the eol is changed during the loading of the settings, the \r is removed. re-add it.
            var namingPatterns = Properties.Settings.Default.NamingPatterns;
            if (namingPatterns != null)
            {
                for (int i = 0; i < namingPatterns.Length; i++)
                {
                    if (!string.IsNullOrEmpty(namingPatterns[i]))
                        namingPatterns[i] = namingPatterns[i].Replace("\r", string.Empty).Replace("\n", "\r\n");
                }
            }

            _tt = new ToolTip();
            InitLocalization();
            InitializeComponent();

            // Create an instance of a ListView column sorter and assign it
            // to the ListView controls
            listViewPossibilities.ListViewItemSorter = new ListViewColumnSorter();
            timerList1.ColumnSorter = new ListViewColumnSorter();

            listViewLibrary.DoubleBuffered(true);

            toolStripStatusLabel.Text = Application.ProductVersion;

            // delegates
            pedigree1.EditCreature += EditCreatureInTester;
            pedigree1.BestBreedingPartners += ShowBestBreedingPartner;
            breedingPlan1.EditCreature += EditCreatureInTester;
            breedingPlan1.DisplayInPedigree += DisplayCreatureInPedigree;
            breedingPlan1.CreateIncubationTimer += CreateIncubationTimer;
            breedingPlan1.BestBreedingPartners += ShowBestBreedingPartner;
            breedingPlan1.SetGlobalSpecies += SetSpecies;
            breedingPlan1.SetMessageLabelText += SetMessageLabelText;
            creatureInfoInputExtractor.SetMessageLabelText += SetMessageLabelText;
            creatureInfoInputTester.SetMessageLabelText += SetMessageLabelText;
            timerList1.OnTimerChange += SetCollectionChanged;
            timerList1.TimerAddedRemoved += EnableGlobalTimerIfNeeded;
            breedingPlan1.BindChildrenControlEvents();
            raisingControl1.onChange += SetCollectionChanged;
            tamingControl1.CreateTimer += CreateTimer;
            raisingControl1.ExtractBaby += ExtractBaby;
            raisingControl1.SetGlobalSpecies += SetSpecies;
            raisingControl1.timerControl = timerList1;
            raisingControl1.TimerAddedRemoved += EnableGlobalTimerIfNeeded;
            notesControl1.changed += SetCollectionChanged;
            creatureInfoInputExtractor.CreatureDataRequested += CreatureInfoInput_CreatureDataRequested;
            creatureInfoInputTester.CreatureDataRequested += CreatureInfoInput_CreatureDataRequested;
            creatureInfoInputExtractor.ColorsChanged += CreatureInfoInputColorsChanged;
            creatureInfoInputTester.ColorsChanged += CreatureInfoInputColorsChanged;
            speciesSelector1.OnSpeciesSelected += SpeciesSelector1OnSpeciesSelected;
            speciesSelector1.ToggleVisibility += ToggleViewSpeciesSelector;
            statsMultiplierTesting1.OnApplyMultipliers += StatsMultiplierTesting1_OnApplyMultipliers;
            raisingControl1.AdjustTimersByOffset += timerList1.AdjustAllTimersByOffset;

            listViewLibrary.VirtualMode = true;
            listViewLibrary.RetrieveVirtualItem += ListViewLibrary_RetrieveVirtualItem;
            listViewLibrary.CacheVirtualItems += ListViewLibrary_CacheVirtualItems;
            listViewLibrary.OwnerDraw = true;
            listViewLibrary.DrawItem += ListViewLibrary_DrawItem;
            listViewLibrary.DrawColumnHeader += (sender, args) => args.DrawDefault = true;
            listViewLibrary.DrawSubItem += ListViewLibrary_DrawSubItem;

            speciesSelector1.SetTextBox(tbSpeciesGlobal);

            ArkOcr.Ocr.SetOcrControl(ocrControl1);
            ocrControl1.UpdateWhiteThreshold += OcrUpdateWhiteThreshold;
            ocrControl1.DoOcr += DoOcr;
            ocrControl1.OcrLabelSetsChanged += InitializeOcrLabelSets;
            ocrControl1.OcrLabelSelectedSetChanged += SetCurrentOcrLabelSet;

            openSettingsToolStripMenuItem.ShortcutKeyDisplayString = new KeysConverter()
                .ConvertTo(Keys.Control, typeof(string))?.ToString().Replace("None", ",");

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                var statIo = new StatIO
                {
                    InputType = StatIOInputType.FinalValueInputType,
                    Title = Utils.StatName(s),
                    statIndex = s
                };
                var statIoTesting = new StatIO
                {
                    InputType = StatIOInputType.LevelsInputType,
                    Title = Utils.StatName(s),
                    statIndex = s
                };

                if (Utils.Precision(s) == 3)
                {
                    statIo.Percent = true;
                    statIoTesting.Percent = true;
                }

                statIoTesting.LevelChanged += TestingStatIoValueUpdate;
                statIo.InputValueChanged += StatIOQuickWildLevelCheck;
                statIo.Click += StatIO_Click;
                _considerStatHighlight[s] = (Properties.Settings.Default.consideredStats & (1 << s)) != 0;

                _statIOs[s] = statIo;
                _testingIOs[s] = statIoTesting;
            }

            // add controls in the order they are shown in-game
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                var displayIndex = Stats.DisplayOrder[s];
                flowLayoutPanelStatIOsExtractor.Controls.Add(_statIOs[displayIndex]);
                flowLayoutPanelStatIOsTester.Controls.Add(_testingIOs[displayIndex]);
                checkedListBoxConsiderStatTop.Items.Add(Utils.StatName(displayIndex),
                    _considerStatHighlight[displayIndex]);
            }

            _timerGlobal.Interval = 1000;
            _timerGlobal.Tick += TimerGlobal_Tick;

            ReloadNamePatternCustomReplacings();

            lbTesterWildLevel.ContextMenu = new ContextMenu(new[] { new MenuItem("Set random wild levels", SetRandomWildLevels) });

            _reactOnCreatureSelectionChange = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetLocalizations(false);

            // load window-position and size
            Utils.SetWindowRectangle(this, Properties.Settings.Default.MainWindowRect,
                Properties.Settings.Default.MainWindowMaximized);

            // Load column-widths, display-indices and sort-order of the TimerControlListView

            LoadListViewSettings(timerList1.ListViewTimers, nameof(Properties.Settings.Default.TCLVColumnWidths),
                nameof(Properties.Settings.Default.TCLVColumnDisplayIndices),
                nameof(Properties.Settings.Default.TCLVSortCol), nameof(Properties.Settings.Default.TCLVSortAsc));
            if (Properties.Settings.Default.PedigreeWidthLeftColum > 20)
                pedigree1.LeftColumnWidth = Properties.Settings.Default.PedigreeWidthLeftColum;

            LoadListViewSettings(pedigree1.ListViewCreatures, nameof(Properties.Settings.Default.PedigreeListViewColumnWidths));

            // Load column-widths, display-indices and sort-order  of the listViewLibrary
            // new columns were added, reset widths and order, old settings don't match the new indices
            if ((Properties.Settings.Default.columnWidths?.Length ?? 0) < 40)
            {
                resetColumnOrderToolStripMenuItem_Click(null, null);
                toolStripMenuItemResetLibraryColumnWidths_Click(null, null);
            }
            else
                LoadListViewSettings(listViewLibrary, nameof(Properties.Settings.Default.columnWidths), nameof(Properties.Settings.Default.libraryColumnDisplayIndices));
            _creatureListSorter.SortColumnIndex = Properties.Settings.Default.listViewSortCol;
            _creatureListSorter.Order = Properties.Settings.Default.listViewSortAsc
                ? SortOrder.Ascending
                : SortOrder.Descending;

            LoadListViewSettings(tribesControl1.ListViewPlayers, nameof(Properties.Settings.Default.PlayerListColumnWidths), nameof(Properties.Settings.Default.PlayerListColumnDisplayIndices),
                nameof(Properties.Settings.Default.PlayerListSortColumn), nameof(Properties.Settings.Default.PlayerListSortAsc));

            _creatureListSorter.UseNaturalSort = Properties.Settings.Default.UseNaturalSort;
            _creatureListSorter.IgnoreSpacesBetweenWords = Properties.Settings.Default.NaturalSortIgnoreSpaces;

            CbLibraryInfoUseFilter.Checked = Properties.Settings.Default.LibraryColorInfoUseFilter;

            // load stat weights
            double[][] custWd = Properties.Settings.Default.customStatWeights;
            var customStatWeightOddEven = Properties.Settings.Default.CustomStatWeightOddEven;
            string[] custWs = Properties.Settings.Default.customStatWeightNames;
            var custW = new Dictionary<string, (double[], byte[])>();
            if (custWs != null && custWd != null)
            {
                for (int i = 0; i < custWs.Length && i < custWd.Length && i < customStatWeightOddEven.Length; i++)
                {
                    custW.Add(custWs[i], (custWd[i], customStatWeightOddEven[i]));
                }
            }

            breedingPlan1.StatWeighting.CustomWeightings = custW;
            // last set values are saved at the end of the custom weightings
            if (custWs != null && custWd != null && custWd.Length > custWs.Length)
                breedingPlan1.StatWeighting.WeightValues = custWd[custWs.Length];
            if (custWs != null && customStatWeightOddEven != null && customStatWeightOddEven.Length > custWs.Length)
                breedingPlan1.StatWeighting.AnyOddEven = customStatWeightOddEven[custWs.Length];

            // load weapon damages
            tamingControl1.WeaponDamages = Properties.Settings.Default.weaponDamages;
            tamingControl1.WeaponDamagesEnabled = Properties.Settings.Default.weaponDamagesEnabled;

            // torpor should not show bar, it get's too wide and is not interesting for breeding
            _statIOs[Stats.Torpidity].ShowBarAndLock = false;
            _testingIOs[Stats.Torpidity].ShowBarAndLock = false;
            // move sums and footnote to bottom
            flowLayoutPanelStatIOsExtractor.Controls.Add(panelSums);
            flowLayoutPanelStatIOsExtractor.Controls.Add(labelFootnote);
            flowLayoutPanelStatIOsTester.Controls.Add(panelStatTesterFootnote);

            breedingPlan1.MutationLimit = Properties.Settings.Default.MutationLimitBreedingPlanner;

            // enable 0-lock for dom-levels of oxygen, food (most often they are not leveled up)
            _statIOs[Stats.Oxygen].DomLevelLockedZero = true;
            _statIOs[Stats.Food].DomLevelLockedZero = true;

            LbWarningLevel255.Visible = false;

            InitializeCollection();

            CreatureColored.InitializeSpeciesImageLocation();

            if (!LoadStatAndKibbleValues(false).statValuesLoaded || !Values.V.species.Any())
            {
                MessageBoxes.ShowMessageBox(Loc.S("valuesFileLoadingError"),
                    $"{Loc.S("error")}: Values-file not found");
                Environment.Exit(1);
            }

            statsMultiplierTesting1.SetGameDefaultMultiplier();

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _statIOs[s].Input = 0;
            }

            creatureInfoInputTester.PbColorRegion = pictureBoxColorRegionsTester;
            creatureInfoInputExtractor.PbColorRegion = PbCreatureColorsExtractor;
            creatureInfoInputExtractor.ParentInheritance = parentInheritanceExtractor;
            parentInheritanceExtractor.Visible = false;

            // set last species
            speciesSelector1.LastSpecies = Properties.Settings.Default.lastSpecies;

            if (Properties.Settings.Default.lastSpecies?.Any() == true)
            {
                speciesSelector1.SetSpecies(Values.V.SpeciesByBlueprint(Properties.Settings.Default.lastSpecies[0]));
            }

            if (speciesSelector1.SelectedSpecies == null && Values.V.species.Any())
                speciesSelector1.SetSpecies(Values.V.species[0]);
            tamingControl1.SetSpecies(speciesSelector1.SelectedSpecies);

            // OCR
            ocrControl1.Initialize();
            cbGuessSpecies.Checked = Properties.Settings.Default.OcrGuessSpecies;
            InitializeOcrLabelSets();

            // initialize speech recognition if enabled
            InitializeSpeechRecognition();

            // default owner and tribe
            creatureInfoInputExtractor.CreatureOwner = Properties.Settings.Default.DefaultOwnerName;
            creatureInfoInputExtractor.CreatureTribe = Properties.Settings.Default.DefaultTribeName;
            creatureInfoInputExtractor.CreatureServer = Properties.Settings.Default.DefaultServerName;
            creatureInfoInputExtractor.OwnerLock = Properties.Settings.Default.OwnerNameLocked;
            creatureInfoInputExtractor.TribeLock = Properties.Settings.Default.TribeNameLocked;
            creatureInfoInputExtractor.LockServer = Properties.Settings.Default.ServerNameLocked;

            // UI loaded

            // set theme colors
            //this.InitializeTabControls();
            //this.SetColors(Color.FromArgb(20, 20, 20), Color.LightGray);


            //// initialize controls
            radarChart1.InitializeVariables(_creatureCollection.maxChartLevel);
            radarChartExtractor.InitializeVariables(_creatureCollection.maxChartLevel);
            radarChartLibrary.InitializeVariables(_creatureCollection.maxChartLevel);
            extractionTestControl1.CopyToExtractor += ExtractionTestControl1_CopyToExtractor;
            extractionTestControl1.CopyToTester += ExtractionTestControl1_CopyToTester;

            // dev tabs
            if (!Properties.Settings.Default.DevTools)
            {
                tabControlMain.TabPages.Remove(tabPageExtractionTests);
                tabControlMain.TabPages.Remove(tabPageMultiplierTesting);
                devToolStripMenuItem.Visible = false;
            }
            else
            {
                extractionTestControl1.LoadExtractionTestCases(Properties.Settings.Default.LastSaveFileTestCases);
            }

            // set TLS-protocol (github needs at least TLS 1.2) for update-check
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            // check for updates
            MoveSpeciesImagesToNewFolder();
            if (DateTime.Now.AddHours(-20) > Properties.Settings.Default.lastUpdateCheck)
            {
                bool selectDefaultImagesIfNotYet = false;
                bool initializeImages = false;
                if (!Properties.Settings.Default.AlreadyAskedToDownloadSpeciesImageFiles)
                {
                    Properties.Settings.Default.AlreadyAskedToDownloadSpeciesImageFiles = true;

                    if (Updater.Updater.IsProgramInstalled)
                        initializeImages = true;
                    else
                        selectDefaultImagesIfNotYet = true;
                }
                CheckForUpdates(true, selectDefaultImagesIfNotYet, initializeImages);
            }

            RemoveNonExistingFilesInRecentlyUsedFiles();

            _filterListAllowed = true;
            // load last loaded file
            bool createNewCollection = string.IsNullOrEmpty(Properties.Settings.Default.LastSaveFile);
            if (!createNewCollection)
            {
                // if the last loaded file was already converted by someone else (e.g. if the library-file is shared),
                // ask if the converted version should be loaded instead.
                if (Path.GetExtension(Properties.Settings.Default.LastSaveFile).ToLower() == ".xml")
                {
                    string possibleConvertedCollectionPath = Path.Combine(
                        Path.GetDirectoryName(Properties.Settings.Default.LastSaveFile),
                        Path.GetFileNameWithoutExtension(Properties.Settings.Default.LastSaveFile) +
                        CollectionFileExtension);
                    if (File.Exists(possibleConvertedCollectionPath)
                        && MessageBox.Show(
                            "The creature collection file seems to be already converted to the new file format.\n"
                            + "Path of the collection file:\n" + Properties.Settings.Default.LastSaveFile
                            + "\n\nIf you click No, the old file-version will be loaded and then automatically converted."
                            + "\nIt is recommended to load the already converted version to avoid synchronisation-issues."
                            + "\nDo you want to load the converted version?", "Library seems to be already converted",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question
                        ) == DialogResult.Yes)
                    {
                        Properties.Settings.Default.LastSaveFile = possibleConvertedCollectionPath;
                    }
                }

                // load last save file:
                if (!LoadCollectionFile(Properties.Settings.Default.LastSaveFile))
                    createNewCollection = true;
            }

            // if no export folder is set, try to detect it
            if ((Properties.Settings.Default.ExportCreatureFolders == null
                 || Properties.Settings.Default.ExportCreatureFolders.Length == 0)
                && ArkInstallationPath.GetListOfExportFolders(
                    out (string path, string steamPlayerName)[] arkInstallFolders, out _))
            {
                var orderedList = ArkInstallationPath.OrderByNewestFileInFolders(arkInstallFolders.Select(l => (l.path, l)));

                Properties.Settings.Default.ExportCreatureFolders = orderedList
                    .Select(f => $"{f.steamPlayerName}||{f.path}").ToArray();
            }

            if (createNewCollection)
            {
                NewCollection();
                UpdateRecentlyUsedFileMenu();
            }

            var filterPresets = Properties.Settings.Default.LibraryFilterPresets;
            if (filterPresets != null)
                ToolStripTextBoxLibraryFilter.AutoCompleteCustomSource.AddRange(filterPresets);

            UpdateAsaIndicator();
            UpdatePatternButtons();

            SetupAutoLoadFileWatcher();
            SetupExportFileWatcher();

            timerList1.SetTimerPresets(Properties.Settings.Default.TimerPresets);
        }

        /// <summary>
        /// If the according property is set, the speechRecognition is initialized. Else it's disposed.
        /// </summary>
        private void InitializeSpeechRecognition()
        {
            bool speechRecognitionInitialized = false;
            if (Properties.Settings.Default.SpeechRecognition)
            {
                try
                {
                    _speechRecognition = new SpeechRecognition(_creatureCollection.maxWildLevel,
                        _creatureCollection.considerWildLevelSteps ? _creatureCollection.wildLevelStep : 1,
                        Values.V.speciesWithAliasesList, lbListening);
                    if (_speechRecognition.Initialized)
                    {
                        speechRecognitionInitialized = true;
                        _speechRecognition.speechRecognized += TellTamingData;
                        _speechRecognition.speechCommandRecognized += SpeechCommand;
                        lbListening.Visible = true;
                    }
                    else
                    {
                        Properties.Settings.Default.SpeechRecognition = false;
                    }
                }
                catch (PlatformNotSupportedException ex)
                {
                    MessageBoxes.ExceptionMessageBox(ex,
                        "The speech recognition could not be initialized on this system.");
                }
            }

            if (!speechRecognitionInitialized)
            {
                _speechRecognition?.Dispose();
                _speechRecognition = null;
                lbListening.Visible = false;
            }
        }

        private void SetSpecies(Species species)
        {
            speciesSelector1.SetSpecies(species);
        }

        private void TellTamingData(string speciesName, int level)
        {
            speciesSelector1.SetSpeciesByName(speciesName);
            if (speciesSelector1.SelectedSpecies?.taming?.eats?.Any() == true)
            {
                tamingControl1.SetLevel(level, false);
                tamingControl1.SetSpecies(speciesSelector1.SelectedSpecies);
                _overlay?.SetInfoText($"{speciesName} ({Loc.S("Level")} {level}):\n{tamingControl1.quickTamingInfos}");
            }
        }

        private void SpeechCommand(SpeechRecognition.Commands command)
        {
            // currently this command does not exist, accidental execution occurred too often
            if (command == SpeechRecognition.Commands.Extract)
                DoOcr();
        }

        private void radioButtonWild_CheckedChanged(object sender, EventArgs e)
        {
            if (rbWildExtractor.Checked)
                UpdateExtractorDetails();
        }

        private void radioButtonTamed_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTamedExtractor.Checked)
                UpdateExtractorDetails();
        }

        private void radioButtonBred_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBredExtractor.Checked)
                UpdateExtractorDetails();
        }

        private void radioButtonTesterWild_CheckedChanged(object sender, EventArgs e)
        {
            if (rbWildTester.Checked)
                UpdateTesterDetails();
        }

        private void radioButtonTesterTamed_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTamedTester.Checked)
                UpdateTesterDetails();
            lbWildLevelTester.Visible = rbTamedTester.Checked;
        }

        private void radioButtonTesterBred_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBredTester.Checked)
                UpdateTesterDetails();
        }

        private void StatIO_Click(object sender, EventArgs e)
        {
            StatIO se = (StatIO)sender;
            if (se != null)
            {
                SetActiveStat(se.statIndex);
            }
        }

        private void tbSpeciesGlobal_Click(object sender, EventArgs e)
        {
            ToggleViewSpeciesSelector(true);
        }

        private void tbSpeciesGlobal_Enter(object sender, EventArgs e)
        {
            ToggleViewSpeciesSelector(true);
        }

        private void pbSpecies_Click(object sender, EventArgs e)
        {
            if (tabControlMain.Visible)
            {
                if (tbSpeciesGlobal.Focused)
                    pbSpecies.Focus();
                tbSpeciesGlobal.Focus();
            }
            else
            {
                ToggleViewSpeciesSelector(false);
            }
        }

        private void ToggleViewSpeciesSelector(bool showSpeciesSelector)
        {
            tabControlMain.Visible = !showSpeciesSelector;
            speciesSelector1.Visible = showSpeciesSelector;
        }

        private void TbSpeciesGlobal_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter && e.KeyCode != Keys.Tab) return;
            if (speciesSelector1.SetSpeciesByName(tbSpeciesGlobal.Text))
                ToggleViewSpeciesSelector(false);
        }

        // global species changed / globalspecieschanged
        private void SpeciesSelector1OnSpeciesSelected(bool speciesChanged)
        {
            Species species = speciesSelector1.SelectedSpecies;
            ToggleViewSpeciesSelector(false);
            tbSpeciesGlobal.Text = species.name;
            LbBlueprintPath.Text = species.blueprintPath;
            if (!speciesChanged) return;
            _clearExtractionCreatureData =
                true; // as soon as the user changes the species, it's assumed it's not an exported creature anymore
            pbSpecies.Image = speciesSelector1.SpeciesImage();

            creatureInfoInputExtractor.SelectedSpecies = species;
            creatureInfoInputTester.SelectedSpecies = species;
            var statNames = species.statNames;

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _activeStats[s] = Properties.Settings.Default.DisplayHiddenStats
                    ? species.UsesStat(s)
                    : species.DisplaysStat(s);

                _statIOs[s].IsActive = _activeStats[s];
                _statIOs[s].Visible = species.UsesStat(s);
                _testingIOs[s].Visible = species.UsesStat(s);
                if (!_activeStats[s]) _statIOs[s].Input = 0;
                _statIOs[s].Title = Utils.StatName(s, false, statNames);
                _testingIOs[s].Title = Utils.StatName(s, false, statNames);
                // don't lock special stats of glow species
                if ((statNames != null &&
                     (s == Stats.Stamina
                      || s == Stats.Oxygen
                      || s == Stats.MeleeDamageMultiplier)
                    )
                    || (species.name.Contains("Daeodon")
                        && s == Stats.Food
                    )
                )
                {
                    _statIOs[s].DomLevelLockedZero = false;
                }
            }

            if (tabControlMain.SelectedTab == tabPageExtractor)
            {
                ClearAll();
                // warn if a species selected that has a possible mod variant
                if ((species.Mod == null || species.Mod.expansion)
                    && Values.V.TryGetSpeciesByName(species.name, out var modSpecies)
                    && modSpecies.Mod?.expansion == false
                )
                {
                    SetMessageLabelText(
                        $"The selected species \"{species}\" is not from a mod, but there is a variant of that species that appears in the loaded mod \"{modSpecies.Mod.title}\". Probably you want to select the mod variant",
                        MessageBoxIcon.Warning);
                }
            }
            else if (tabControlMain.SelectedTab == tabPageStatTesting)
            {
                UpdateAllTesterValues();
                statPotentials1.Species = species;
                statPotentials1.SetLevels(_testingIOs.Select(s => s.LevelWild).ToArray(), _testingIOs.Select(s => s.LevelMut).ToArray(), true);
                SetTesterInfoInputCreature();
            }
            else if (tabControlMain.SelectedTab == tabPageLibrary)
            {
                if (Properties.Settings.Default.ApplyGlobalSpeciesToLibrary)
                    listBoxSpeciesLib.SelectedItem = species;
            }
            else if (tabControlMain.SelectedTab == tabPageLibraryInfo)
            {
                LibraryInfo.SetColorInfo(speciesSelector1.SelectedSpecies, CbLibraryInfoUseFilter.Checked ? (IList<Creature>)ApplyLibraryFilterSettings(_creatureCollection.creatures).ToArray() : _creatureCollection.creatures, CbLibraryInfoUseFilter.Checked, tlpLibraryInfo);
            }
            else if (tabControlMain.SelectedTab == tabPagePedigree)
            {
                pedigree1.SetSpecies(species);
            }
            else if (tabControlMain.SelectedTab == tabPageTaming)
            {
                tamingControl1.SetSpecies(species);
            }
            else if (tabControlMain.SelectedTab == tabPageRaising)
            {
                raisingControl1.UpdateRaisingData(species);
            }
            else if (tabControlMain.SelectedTab == tabPageMultiplierTesting)
            {
                statsMultiplierTesting1.SetSpecies(species);
            }
            else if (tabControlMain.SelectedTab == tabPageBreedingPlan)
            {
                if (breedingPlan1.CurrentSpecies == species)
                    breedingPlan1.UpdateIfNeeded();
                else
                {
                    breedingPlan1.SetSpecies(species);
                }
            }
            hatching1.SetSpecies(species, _topLevels.TryGetValue(species, out var bl) ? bl : null, _lowestLevels.TryGetValue(species, out var ll) ? ll : null);

            _hiddenLevelsCreatureTester = 0;

            _tt.SetToolTip(tbSpeciesGlobal, species.DescriptiveNameAndMod + "\n" + species.blueprintPath);
        }

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            n?.Select(0, n.Text.Length);
        }

        /// <summary>
        /// Recalculate cached values affected by global stat-multipliers.
        /// </summary>
        private void ApplySettingsToValues()
        {
            // apply multipliers
            Values.V.ApplyMultipliers(_creatureCollection, cbEventMultipliers.Checked);
            tamingControl1.SetServerMultipliers(Values.V.currentServerMultipliers);

            ColorModeColors.SetColors((ColorModeColors.AsbColorMode)Properties.Settings.Default.ColorMode);
            RecalculateAllCreaturesValues();

            breedingPlan1.UpdateBreedingData();
            raisingControl1.UpdateRaisingData();

            // apply level settings
            creatureBoxListView.CreatureCollection = _creatureCollection;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _statIOs[s].barMaxLevel = _creatureCollection.maxChartLevel;
                _testingIOs[s].barMaxLevel = _creatureCollection.maxChartLevel;
            }

            breedingPlan1.MaxWildLevels = _creatureCollection.maxWildLevel;
            radarChart1.InitializeVariables(_creatureCollection.maxChartLevel);
            radarChartExtractor.InitializeVariables(_creatureCollection.maxChartLevel);
            radarChartLibrary.InitializeVariables(_creatureCollection.maxChartLevel);
            statPotentials1.LevelDomMax = _creatureCollection.maxDomLevel;
            statPotentials1.LevelGraphMax = _creatureCollection.maxChartLevel;

            _speechRecognition?.SetMaxLevelAndSpecies(_creatureCollection.maxWildLevel,
                _creatureCollection.considerWildLevelSteps ? _creatureCollection.wildLevelStep : 1,
                Values.V.speciesWithAliasesList);
            if (_overlay != null)
            {
                _overlay.InfoDuration = Properties.Settings.Default.OverlayInfoDuration;
                _overlay.checkInventoryStats = Properties.Settings.Default.inventoryCheckTimer;
            }

            ArkOcr.Ocr.screenCaptureApplicationName = Properties.Settings.Default.OCRApp;

            if (Properties.Settings.Default.showOCRButton)
            {
                Loc.ControlText(btReadValuesFromArk, _tt);
            }
            else
            {
                btReadValuesFromArk.Text = "Import Exported Data";
                _tt.SetToolTip(btReadValuesFromArk,
                    "Displays all exported creatures in the default-folder (needs to be set in the settings).");
            }

            ArkOcr.Ocr.waitBeforeScreenCapture = Properties.Settings.Default.waitBeforeScreenCapture;
            ocrControl1.SetWhiteThreshold(Properties.Settings.Default.OCRWhiteThreshold);

            int maxImprintingPercentage = _creatureCollection.allowMoreThanHundredImprinting ? 100000 : 100;
            numericUpDownImprintingBonusExtractor.Maximum = maxImprintingPercentage;
            numericUpDownImprintingBonusTester.Maximum = maxImprintingPercentage;

            // sound-files
            timerList1.sounds = new[]
            {
                File.Exists(Properties.Settings.Default.soundStarving)
                    ? new System.Media.SoundPlayer(Properties.Settings.Default.soundStarving)
                    : null,
                File.Exists(Properties.Settings.Default.soundWakeup)
                    ? new System.Media.SoundPlayer(Properties.Settings.Default.soundWakeup)
                    : null,
                File.Exists(Properties.Settings.Default.soundBirth)
                    ? new System.Media.SoundPlayer(Properties.Settings.Default.soundBirth)
                    : null,
                File.Exists(Properties.Settings.Default.soundCustom)
                    ? new System.Media.SoundPlayer(Properties.Settings.Default.soundCustom)
                    : null
            };

            timerList1.TimerAlertsCSV = Properties.Settings.Default.playAlarmTimes;

            ClearAll();
            // update enabled stats
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _activeStats[s] = speciesSelector1.SelectedSpecies == null
                    ? (Species.displayedStatsDefault & 1 << s) != 0
                    : Properties.Settings.Default.DisplayHiddenStats
                        ? speciesSelector1.SelectedSpecies.UsesStat(s)
                        : speciesSelector1.SelectedSpecies.DisplaysStat(s);
                _statIOs[s].IsActive = _activeStats[s];
                if (!_activeStats[s]) _statIOs[s].Input = 0;
            }

            if (tabControlMain.SelectedTab == tabPageStatTesting)
            {
                UpdateAllTesterValues();
            }

            CreateImportExportedMenu();

            // save game importer menu
            importingFromSavegameToolStripMenuItem.DropDownItems.Clear();
            if (Properties.Settings.Default.arkSavegamePaths?.Any() != true)
            {
                importingFromSavegameToolStripMenuItem.DropDownItems.Add(importingFromSavegameEmptyToolStripMenuItem);
                TsbQuickSaveGameImport.ToolTipText = "No quick import save files configured,\nyou can do this in the settings.";
            }
            else
            {
                var quickImportInfo = new List<string>();
                foreach (string f in Properties.Settings.Default.arkSavegamePaths)
                {
                    ATImportFileLocation atImportFileLocation = ATImportFileLocation.CreateFromString(f);

                    string menuItemHeader = string.IsNullOrEmpty(atImportFileLocation.ConvenientName)
                        ? "<unnamed>"
                        : atImportFileLocation.ConvenientName;
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(menuItemHeader)
                    {
                        Tag = atImportFileLocation,
                        ToolTipText = atImportFileLocation.FileLocation
                    };
                    tsmi.Click += SavegameImportClick;
                    importingFromSavegameToolStripMenuItem.DropDownItems.Add(tsmi);
                    if (atImportFileLocation.ImportWithQuickImport)
                        quickImportInfo.Add($"{atImportFileLocation.ConvenientName} ({atImportFileLocation.FileLocation})");
                }

                TsbQuickSaveGameImport.ToolTipText = quickImportInfo.Any()
                    ? "Quick save game import. The following save files will be imported:\n\n" + string.Join("\n", quickImportInfo)
                    : "No quick import save files configured,\nyou can do this in the settings.";
            }
        }

        private void importingFromSavegameEmptyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSettingsDialog(SettingsTabPages.SaveImport);
        }

        /// <summary>
        /// Checks each creature for tags and saves them in a list.
        /// </summary>
        private void CreateCreatureTagList()
        {
            _creatureCollection.tags.Clear();
            foreach (Creature c in _creatureCollection.creatures)
            {
                foreach (string t in c.tags)
                {
                    if (!_creatureCollection.tags.Contains(t))
                        _creatureCollection.tags.Add(t);
                }
            }

            _creatureCollection.tags.Sort();

            breedingPlan1.CreateTagList();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutBox1 aboutBox = new AboutBox1())
            {
                aboutBox.ShowDialog();
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadCollection();
        }

        private void loadAndAddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadCollection(true);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCollection();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveNewCollection();
        }

        /// <summary>
        /// This function should be called if the creatureCollection was changed, i.e. after loading a file or adding/removing a creature
        /// </summary>
        /// <param name="species">If not null, only the creatures of the species are updated</param>
        /// <param name="keepCurrentlySelectedSpecies"></param>
        private void UpdateCreatureListings(Species species = null, bool keepCurrentlySelectedSpecies = true)
        {
            // if speciesIndex == null consider all creatures, else recalculate only the indicated species if applicable
            List<Creature> creatures = _creatureCollection.creatures;
            if (species != null)
            {
                creatures = creatures.Where(c => c.Species == species).ToList();
            }

            UpdateOwnerServerTagLists();
            CalculateTopStats(creatures);
            UpdateSpeciesLists(_creatureCollection.creatures, keepCurrentlySelectedSpecies);
            FilterLibRecalculate();
            UpdateStatusBar();
            breedingPlan1.BreedingPlanNeedsUpdate = true;
            pedigree1.SetSpecies(forceUpdate: true);
            raisingControl1.RecreateList();
            LibraryInfo.ClearInfo();
        }

        /// <summary>
        /// This function should be called if the creatureCollection is changed, i.e. after loading a file or adding/removing a creature.
        /// It updates the listed species in the treeList and in the speciesSelector.
        /// Also call after the sort order of the species was changed.
        /// </summary>
        private void UpdateSpeciesLists(List<Creature> creatures, bool keepCurrentlySelectedSpecies = true)
        {
            Species selectedSpeciesLibrary =
                keepCurrentlySelectedSpecies && listBoxSpeciesLib.SelectedItem is Species sp ? sp : null;

            // clear speciesList
            listBoxSpeciesLib.Items.Clear();
            List<Species> availableSpecies = new List<Species>();

            foreach (Creature cr in creatures)
            {
                // add new item for species if not existent
                if (!availableSpecies.Contains(cr.Species))
                {
                    availableSpecies.Add(cr.Species);
                }
            }

            // sort species according to selected order (can be modified by json/sortNames.txt)
            _speciesInLibraryOrdered = Values.V.species.Where(sn => availableSpecies.Contains(sn)).ToArray();

            // add node to show all
            listBoxSpeciesLib.BeginUpdate();
            listBoxSpeciesLib.Items.Add(Loc.S("All"));
            listBoxSpeciesLib.Items.AddRange(_speciesInLibraryOrdered);
            listBoxSpeciesLib.EndUpdate();

            if (selectedSpeciesLibrary != null)
                listBoxSpeciesLib.SelectedItem = selectedSpeciesLibrary;

            breedingPlan1.SetSpeciesList(_speciesInLibraryOrdered, creatures);
            speciesSelector1.SetLibrarySpecies(_speciesInLibraryOrdered);
        }

        /// <summary>
        /// Updates the list of set owners, servers, tribes and tags of all creatures.
        /// </summary>
        private void UpdateOwnerServerTagLists()
        {
            bool filterListAllowedKeeper = _filterListAllowed;
            _filterListAllowed = false;

            //// clear lists
            // owner
            var ownerList = new HashSet<string>();
            var tribesList = new HashSet<string>();
            var serverList = new HashSet<string>();

            //// check all creature for info
            var creaturesToCheck = _creatureCollection.creatures.Where(c => !c.flags.HasFlag(CreatureFlags.Placeholder))
                .ToArray();
            foreach (Creature c in creaturesToCheck)
            {
                AddIfNotEmpty(ownerList, c.owner);
                AddIfNotEmpty(tribesList, c.tribe);
                AddIfNotEmpty(serverList, c.server);

                void AddIfNotEmpty(HashSet<string> list, string name)
                {
                    if (!string.IsNullOrEmpty(name))
                        list.Add(name);
                }
            }

            // owners
            tribesControl1.AddPlayers(ownerList);

            // tribes
            tribesControl1.AddTribes(tribesList);

            ///// Apply autocomplete lists
            // owners
            string[] owners = tribesControl1.PlayerNames;
            creatureInfoInputExtractor.AutocompleteOwnerList = owners;
            creatureInfoInputTester.AutocompleteOwnerList = owners;

            // tribes
            string[] tribes = tribesControl1.TribeNames;
            creatureInfoInputExtractor.AutocompleteTribeList = tribes;
            creatureInfoInputTester.AutocompleteTribeList = tribes;

            // tribes of the owners (same index as owners)
            string[] ownersTribes = tribesControl1.OwnersTribes;
            creatureInfoInputExtractor.OwnersTribes = ownersTribes;
            creatureInfoInputTester.OwnersTribes = ownersTribes;

            // server
            var serverArray = serverList.ToArray();
            creatureInfoInputExtractor.ServersList = serverArray;
            creatureInfoInputTester.ServersList = serverArray;

            _creatureCollection.ownerList = owners;
            _creatureCollection.serverList = serverArray;

            _filterListAllowed = filterListAllowedKeeper;
        }

        #region check for update

        private void checkForUpdatedStatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        private async void CheckForUpdates(bool silentCheck = false, bool selectDefaultImagesIfNotYet = false,
            bool initializeImages = false)
        {
            bool? updaterRunning = await Updater.Updater.CheckForPortableUpdate(silentCheck, UnsavedChanges());
            if (!updaterRunning.HasValue) return; // error
            if (updaterRunning.Value)
            {
                // new version is available, user wants to update and the updater has just been started
                Close();
                return;
            }

            // download mod-manifest file to check for value updates
            if (!await LoadModsManifestAsync(Values.V, true))
                return;

            // check if values-files can be updated
            var downloadedModFiles = Values.V.modsManifest.modsByFiles.Select(mikv => mikv.Value)
                .Where(mi => mi.LocallyAvailable).Select(mi => mi.mod.FileName).ToList();
            downloadedModFiles.Add(FileService.ValuesJson); // check also base values file

            bool valuesUpdated = CheckAvailabilityAndUpdateModFiles(downloadedModFiles, Values.V);

            // update last successful update check
            Properties.Settings.Default.lastUpdateCheck = DateTime.Now;

            if (valuesUpdated)
            {
                var statsLoaded = LoadStatAndKibbleValues(forceReload: true);
                if (statsLoaded.statValuesLoaded)
                {
                    MessageBox.Show(Loc.S("downloadingValuesSuccess"),
                        Loc.S("updateSuccessTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ApplySpeciesObjectsToCollection(_creatureCollection);
                }
                else
                {
                    MessageBoxes.ShowMessageBox(
                        "Download of new stat successful, but files couldn't be loaded.\nTry again later, or redownload the tool.");
                }
            }
            else if (!silentCheck)
            {
                MessageBox.Show(
                    $"You already have the newest version of both the program ({Application.ProductVersion}) and values file ({Values.V.Version}).\n\n" +
                    "If your stats are outdated and no new version is available, we probably don\'t have the new ones either.",
                    "No new Version available", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (!silentCheck || selectDefaultImagesIfNotYet || initializeImages)
                DisplayUpdateModules(!silentCheck, selectDefaultImagesIfNotYet, initializeImages);
        }

        /// <summary>
        /// Load stat- and kibble-values.
        /// </summary>
        private (bool statValuesLoaded, bool kibbleValuesLoaded) LoadStatAndKibbleValues(bool applySettings = true, bool forceReload = false)
        {
            (bool statValuesLoaded, bool kibbleValuesLoaded) success = (false, false);
            if (LoadStatValues(Values.V, forceReload))
            {
                speciesSelector1.SetSpeciesLists(Values.V.species, Values.V.aliases);
                if (applySettings)
                    ApplySettingsToValues();
                UpdateStatusBar();
                success.statValuesLoaded = true;
            }

            if (Kibbles.K.kibble == null
                && !Kibbles.K.LoadValues(out var errorMessageKibbleLoading))
            {
                if (MessageBox.Show(errorMessageKibbleLoading +
                                    "\n\nDo you want to visit the homepage of the tool to redownload it?",
                        $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    Process.Start(Updater.Updater.ReleasesUrl);
            }
            else
            {
                success.kibbleValuesLoaded = true;
            }

            return success;
        }

        #endregion

        private void CreatureInfoInput_ParentListRequested(CreatureInfoInput sender)
        {
            UpdateParentListInput(sender);
        }

        private void UpdateParentListInput(CreatureInfoInput input)
        {
            // set possible parents
            bool fromExtractor = input == creatureInfoInputExtractor;
            Creature creature = new Creature(speciesSelector1.SelectedSpecies, null, levelsWild: GetCurrentWildLevels(fromExtractor), levelStep: _creatureCollection.getWildLevelStep())
            {
                guid = input.CreatureGuid
            };
            List<Creature>[] parents = FindPossibleParents(creature);
            input.ParentsSimilarities = FindParentSimilarities(parents, creature);
            input.Parents = parents;
            input.CreaturesOfSameSpecies = _creatureCollection.creatures
                .Where(c => c.Species == speciesSelector1.SelectedSpecies).ToArray();
            input.parentListValid = true;
            input.NamesOfAllCreatures = _creatureCollection.creatures.Select(c => c.name).ToList();
            input.LibraryCreatureCount = _creatureCollection.creatures.Count;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewCollection();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (UnsavedChanges())
            {
                switch (CustomMessageBox.Show(Loc.S("Collection changed discard and quit?"),
                Loc.S("Discard changes?"), buttonYes: Loc.S("Save and quit"), buttonNo: Loc.S("Discard changes and quit"), buttonCancel: Loc.S("Cancel quitting"),
                icon: MessageBoxIcon.Warning))
                {
                    case DialogResult.Yes:
                        SaveCollection();
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                    default:
                        e.Cancel = true;
                        break;
                }
            }

        }

        /// <summary>
        /// Save the properties of a listView: column width, column order and sorting.
        /// </summary>
        private static void SaveListViewSettings(ListView lv, string widthName, string indicesName = null, string sortColName = null, string sortAscName = null)
        {
            if (lv == null || string.IsNullOrEmpty(widthName)) return;

            int[] cw = new int[lv.Columns.Count];
            int[] colIndices = new int[lv.Columns.Count];
            for (int c = 0; c < lv.Columns.Count; c++)
            {
                cw[c] = lv.Columns[c].Width;
                colIndices[c] = lv.Columns[c].DisplayIndex;
            }

            Properties.Settings.Default[widthName] = cw;
            if (!string.IsNullOrEmpty(indicesName))
                Properties.Settings.Default[indicesName] = colIndices;

            if (string.IsNullOrEmpty(sortColName) || string.IsNullOrEmpty(sortAscName)) return;

            // save listViewSorting of the listViewLibrary
            ListViewColumnSorter lvcs = (ListViewColumnSorter)lv.ListViewItemSorter;
            if (lvcs != null)
            {
                Properties.Settings.Default[sortColName] = lvcs.SortColumn;
                Properties.Settings.Default[sortAscName] = lvcs.Order == SortOrder.Ascending;
            }

        }

        /// <summary>
        /// Loads settings for a listView: column widths, column order and sorting.
        /// </summary>
        private static void LoadListViewSettings(ListView lv, string widthName, string indicesName = null, string sortColName = null, string sortAscName = null)
        {
            if (lv == null) return;

            // load column-widths
            if (Properties.Settings.Default[widthName] is int[] cw)
            {
                for (int c = 0; c < cw.Length && c < lv.Columns.Count; c++)
                    lv.Columns[c].Width = cw[c];
            }

            // load column display indices
            if (!string.IsNullOrEmpty(indicesName) && Properties.Settings.Default[indicesName] is int[] colIndices)
            {
                // indices have to be set increasingly, or they will "push" other values up
                var colIndicesOrdered = colIndices.Select((i, c) => (columnIndex: c, displayIndex: i))
                    .OrderBy(c => c.displayIndex).ToArray();
                for (int c = 0; c < colIndicesOrdered.Length && c < lv.Columns.Count; c++)
                    lv.Columns[colIndicesOrdered[c].columnIndex].DisplayIndex = colIndicesOrdered[c].displayIndex;
            }

            // load listViewSorting
            if (!string.IsNullOrEmpty(sortColName) && !string.IsNullOrEmpty(sortAscName) && lv.ListViewItemSorter is ListViewColumnSorter lvcs)
            {
                lvcs.SortColumn = (int)Properties.Settings.Default[sortColName];
                lvcs.Order = (bool)Properties.Settings.Default[sortAscName]
                    ? SortOrder.Ascending
                    : SortOrder.Descending;
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // savesettings save settings

            // save window-position and size
            if (WindowState != FormWindowState.Minimized)
            {
                (Properties.Settings.Default.MainWindowRect, Properties.Settings.Default.MainWindowMaximized) =
                    Utils.GetWindowRectangle(this);
            }

            // Save column-widths, display-indices and sort-order of the TimerControlListView
            SaveListViewSettings(timerList1.ListViewTimers, "TCLVColumnWidths", "TCLVColumnDisplayIndices", "TCLVSortCol", "TCLVSortAsc");
            SaveListViewSettings(pedigree1.ListViewCreatures, "PedigreeListViewColumnWidths");
            Properties.Settings.Default.PedigreeWidthLeftColum = pedigree1.LeftColumnWidth;
            SaveListViewSettings(tribesControl1.ListViewPlayers, "PlayerListColumnWidths", "PlayerListColumnDisplayIndices", "PlayerListSortColumn", "PlayerListSortAsc");

            // Save column-widths, display-indices and sort-order of the listViewLibrary
            SaveListViewSettings(listViewLibrary, "columnWidths", "libraryColumnDisplayIndices");
            Properties.Settings.Default.listViewSortCol = _creatureListSorter.SortColumnIndex;
            Properties.Settings.Default.listViewSortAsc = _creatureListSorter.Order == SortOrder.Ascending;

            if (_libraryFilterTemplates != null)
                Properties.Settings.Default.LibraryFilterPresets = _libraryFilterTemplates.Presets;

            Properties.Settings.Default.OcrGuessSpecies = cbGuessSpecies.Checked;

            Properties.Settings.Default.TimerPresets = timerList1.GetTimerPresets();

            // save custom statWeights
            var custWs = new List<string>();
            var custWd = new List<double[]>();
            var custOddEven = new List<byte[]>();
            foreach (KeyValuePair<string, (double[], byte[])> w in breedingPlan1.StatWeighting.CustomWeightings)
            {
                custWs.Add(w.Key);
                custWd.Add(w.Value.Item1);
                custOddEven.Add(w.Value.Item2);
            }

            // add current values without name
            custWd.Add(breedingPlan1.StatWeighting.WeightValues);
            custOddEven.Add(breedingPlan1.StatWeighting.AnyOddEven);

            Properties.Settings.Default.customStatWeightNames = custWs.ToArray();
            Properties.Settings.Default.customStatWeights = custWd.ToArray();
            Properties.Settings.Default.CustomStatWeightOddEven = custOddEven.ToArray();

            // save weaponDamages for KO calculation
            Properties.Settings.Default.weaponDamages = tamingControl1.WeaponDamages;
            Properties.Settings.Default.weaponDamagesEnabled = tamingControl1.WeaponDamagesEnabled;

            // save last selected species in combobox
            Properties.Settings.Default.lastSpecies = speciesSelector1.LastSpecies;

            // save onlyNonMutatedInBreedingPlanner
            Properties.Settings.Default.MutationLimitBreedingPlanner = breedingPlan1.MutationLimit;

            // save locked state of owner, tribe and server
            Properties.Settings.Default.OwnerNameLocked = creatureInfoInputExtractor.OwnerLock;
            Properties.Settings.Default.TribeNameLocked = creatureInfoInputExtractor.TribeLock;
            Properties.Settings.Default.ServerNameLocked = creatureInfoInputExtractor.LockServer;

            // save splitter distance of speciesSelector
            Properties.Settings.Default.SpeciesSelectorVerticalSplitterDistance = speciesSelector1.SplitterDistance;
            Properties.Settings.Default.DisabledVariants = speciesSelector1.VariantSelector?.DisabledVariants?.ToArray();

            Properties.Settings.Default.RaisingFoodLastSelected = raisingControl1.LastSelectedFood;
            Properties.Settings.Default.LibraryColorInfoUseFilter = CbLibraryInfoUseFilter.Checked;

            /////// save settings for next session
            Properties.Settings.Default.Save();

            // remove old cache-files
            CreatureColored.CleanupCache();

            _tt?.Dispose();
            _timerGlobal?.Dispose();
        }

        /// <summary>
        /// Sets the text at the top to display infos.
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="icon">Back color of the message</param>
        /// <param name="path">If valid path to file or folder, the user can click on the message to display the path in the explorer</param>
        /// <param name="clipboardText">If not null, user can copy this text to the clipboard by clicking on the label</param>
        private void SetMessageLabelText(string text = null, MessageBoxIcon icon = MessageBoxIcon.None,
            string path = null, string clipboardText = null)
        {
            if (_ignoreNextMessageLabel)
            {
                _ignoreNextMessageLabel = false;
                return;
            }
            // a TextBox needs \r\n for a new line, only \n will not result in a line break.
            TbMessageLabel.Text = text;
            SetMessageLabelLink(path, clipboardText);

            switch (icon)
            {
                case MessageBoxIcon.Information:
                    TbMessageLabel.BackColor = Color.LightGreen;
                    break;
                case MessageBoxIcon.Warning:
                    TbMessageLabel.BackColor = Color.Yellow;
                    break;
                case MessageBoxIcon.Error:
                    TbMessageLabel.BackColor = Color.LightSalmon;
                    break;
                default:
                    TbMessageLabel.BackColor = SystemColors.Control;
                    break;
            }
        }

        /// <summary>
        /// If valid path to file or folder, the user can click on the message to display the path in the explorer
        /// </summary>
        private void SetMessageLabelLink(string path = null, string clipboardText = null)
        {
            _messageLabelPath = path;
            _messageLabelClipboardContent = clipboardText;

            if (string.IsNullOrEmpty(path)
                && string.IsNullOrEmpty(clipboardText))
            {
                TbMessageLabel.Cursor = null;
                _tt.SetToolTip(TbMessageLabel, null);
            }
            else
            {
                TbMessageLabel.Cursor = Cursors.Hand;
                _tt.SetToolTip(TbMessageLabel,
                    (string.IsNullOrEmpty(path) ? string.Empty : Loc.S("ClickDisplayFile"))
                    + (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(clipboardText) ? Environment.NewLine : string.Empty)
                    + (string.IsNullOrEmpty(clipboardText) ? string.Empty : Loc.S("ClickCopyToClipboard")));
            }
        }

        /// <summary>
        /// Contains the path to open if the library selection info label is clicked, used to open the path in the explorer.
        /// </summary>
        private string _messageLabelPath;

        /// <summary>
        /// Contains the content to copy to the clipboard if the message label is clicked.
        /// </summary>
        private string _messageLabelClipboardContent;

        /// <summary>
        /// If true, the next message is ignored to preserve the previous one. This is used to avoid that the library selection info overwrites the results of the save game import.
        /// </summary>
        private bool _ignoreNextMessageLabel;

        private void TbMessageLabel_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_messageLabelClipboardContent))
                Clipboard.SetText(_messageLabelClipboardContent);
            OpenFolderInExplorer(_messageLabelPath);
        }

        private void listBoxSpeciesLib_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetSpecies(listBoxSpeciesLib.SelectedItem as Species);
            FilterLibRecalculate(true);
        }

        /// <summary>
        /// Recalculate topStats if filters are used in topStat-calculation
        /// </summary>
        private void RecalculateTopStatsIfNeeded()
        {
            if (Properties.Settings.Default.useFiltersInTopStatCalculation)
                CalculateTopStats(_creatureCollection.creatures);
        }

        private void deleteSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedCreatures();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// this function is called if the user enters the settings of a creature. Finds the possible parents and saves them in the creatureBox
        /// </summary>
        /// <param name="creature"></param>
        private void CreatureBoxListView_FindParents(Creature creature)
        {
            var parents = FindPossibleParents(creature);
            creatureBoxListView.parentListSimilarity = FindParentSimilarities(parents, creature);
            creatureBoxListView.parentList = parents;
        }

        /// <summary>
        /// Returns lists of possible parents. Index 0: possible mothers, index 1: possible fathers.
        /// </summary>
        private List<Creature>[] FindPossibleParents(Creature creature)
        {
            var parentList = _creatureCollection.creatures
                .Where(cr =>
                    cr.Species == creature.Species && cr.guid != creature.guid && !cr.flags.HasFlag(CreatureFlags.Placeholder))
                .OrderBy(cr => cr.name).ToList();

            if (creature.Species?.noGender == true)
                return new[] { parentList, null };

            var motherList = parentList.Where(cr => cr.sex == Sex.Female).ToList();
            var fatherList = parentList.Where(cr => cr.sex == Sex.Male).ToList();

            // display new results
            return new[] { motherList, fatherList };
        }

        /// <summary>
        /// Determines the similar stats (number of equal wildLevels as creature), to find parents easier.
        /// </summary>
        private List<int>[] FindParentSimilarities(List<Creature>[] parents, Creature creature)
        {
            if (parents.Length != 2 || (parents[0] == null && parents[1] == null)) return new List<int>[] { null, null };

            var parentListCount = parents[1] == null ? 1 : 2;
            List<int> motherListSimilarities = new List<int>();
            List<int> fatherListSimilarities = parentListCount == 2 ? new List<int>() : null;
            List<int>[] parentListSimilarities = { motherListSimilarities, fatherListSimilarities };
            int e; // number of equal wildLevels
            for (int ps = 0; ps < parentListCount; ps++)
            {
                foreach (Creature c in parents[ps])
                {
                    e = 0;
                    for (int s = 0; s < Stats.StatsCount; s++)
                    {
                        if (s != Stats.Torpidity && creature.levelsWild[s] >= 0 &&
                            creature.levelsWild[s] == c.levelsWild[s])
                            e++;
                    }

                    parentListSimilarities[ps].Add(e);
                }

                // sort parents: put all creatures not available to the end, then the ones with 0 common stats to the end
                int moved = 0;
                for (int p = 0; p < parents[ps].Count - moved; p++)
                {
                    if (parents[ps][p].Status != CreatureStatus.Available)
                    {
                        parentListSimilarities[ps].Add(parentListSimilarities[ps][p]);
                        parentListSimilarities[ps].RemoveAt(p);
                        parents[ps].Add(parents[ps][p]);
                        parents[ps].RemoveAt(p);
                        moved++;
                        p--;
                    }
                }

                moved = 0;
                for (int p = 0; p < parents[ps].Count - moved; p++)
                {
                    if (parentListSimilarities[ps][p] == 0)
                    {
                        parentListSimilarities[ps].Add(parentListSimilarities[ps][p]);
                        parentListSimilarities[ps].RemoveAt(p);
                        parents[ps].Add(parents[ps][p]);
                        parents[ps].RemoveAt(p);
                        moved++;
                        p--;
                    }
                }
            }

            return parentListSimilarities;
        }

        //tabcontrolmainchange, maintabchange
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            timerList1.updateTimer = tabControlMain.SelectedTab == tabPageTimer;
            toolStripButtonCopy2Extractor.Visible = tabControlMain.SelectedTab == tabPageStatTesting;

            bool extractorTab = tabControlMain.SelectedTab == tabPageExtractor;
            bool extractorOrTesterTab = extractorTab || tabControlMain.SelectedTab == tabPageStatTesting;
            toolStripButtonCopy2Tester.Visible = extractorTab;
            toolStripButtonDeleteTempCreature.Visible = extractorTab;
            toolStripButtonSaveCreatureValuesTemp.Visible = extractorTab;
            toolStripCBTempCreatures.Visible = extractorTab;

            toolStripButtonAddPlayer.Visible = tabControlMain.SelectedTab == tabPagePlayerTribes;
            toolStripButtonAddTribe.Visible = tabControlMain.SelectedTab == tabPagePlayerTribes;
            toolStripButtonClear.Visible = extractorOrTesterTab;
            var libraryShown = tabControlMain.SelectedTab == tabPageLibrary;
            ToolStripLabelFilter.Visible = libraryShown;
            ToolStripTextBoxLibraryFilter.Visible = libraryShown;
            ToolStripButtonLibraryFilterClear.Visible = libraryShown;
            ToolStripButtonSaveFilterPreset.Visible = libraryShown;
            SetMessageLabelText();
            copyCreatureToolStripMenuItem.Visible = tabControlMain.SelectedTab == tabPageLibrary;
            raisingControl1.updateListView = tabControlMain.SelectedTab == tabPageRaising;
            toolStripButtonDeleteExpiredIncubationTimers.Visible = tabControlMain.SelectedTab == tabPageRaising ||
                                                                   tabControlMain.SelectedTab == tabPageTimer;
            tsBtAddAsExtractionTest.Visible = Properties.Settings.Default.DevTools &&
                                              tabControlMain.SelectedTab == tabPageStatTesting;
            copyToMultiplierTesterToolStripButton.Visible = Properties.Settings.Default.DevTools && extractorOrTesterTab;
            exactSpawnCommandToolStripMenuItem.Visible = extractorOrTesterTab;
            exactSpawnCommandDS2ToolStripMenuItem.Visible = extractorOrTesterTab;
            toolStripSeparator25.Visible = extractorOrTesterTab;

            if (tabControlMain.SelectedTab == tabPageStatTesting)
            {
                UpdateAllTesterValues();
                statPotentials1.Species = speciesSelector1.SelectedSpecies;
                statPotentials1.SetLevels(_testingIOs.Select(s => s.LevelWild).ToArray(), _testingIOs.Select(s => s.LevelMut).ToArray(), true);
            }
            else if (tabControlMain.SelectedTab == tabPageLibrary)
            {
                if (Properties.Settings.Default.ApplyGlobalSpeciesToLibrary
                    && speciesSelector1.SelectedSpecies != null
                    && (listBoxSpeciesLib.SelectedItem as Species) != speciesSelector1.SelectedSpecies)
                {
                    listBoxSpeciesLib.SelectedItem = speciesSelector1.SelectedSpecies;
                }
                else if (_libraryNeedsUpdate)
                    FilterLibRecalculate();
            }
            else if (tabControlMain.SelectedTab == tabPageLibraryInfo)
            {
                LibraryInfo.SetColorInfo(speciesSelector1.SelectedSpecies, CbLibraryInfoUseFilter.Checked ? (IList<Creature>)ApplyLibraryFilterSettings(_creatureCollection.creatures).ToArray() : _creatureCollection.creatures, CbLibraryInfoUseFilter.Checked, tlpLibraryInfo);
            }
            else if (tabControlMain.SelectedTab == tabPagePedigree)
            {
                pedigree1.SetSpeciesIfNotSet(speciesSelector1.SelectedSpecies);
                if (pedigree1.PedigreeNeedsUpdate)
                {
                    Creature c = null;
                    if (listViewLibrary.SelectedIndices.Count > 0)
                        c = _creaturesDisplayed[listViewLibrary.SelectedIndices[0]];
                    pedigree1.SetCreature(c, true);
                }
            }
            else if (tabControlMain.SelectedTab == tabPageTaming)
            {
                tamingControl1.SetSpecies(speciesSelector1.SelectedSpecies);
            }
            else if (tabControlMain.SelectedTab == tabPageBreedingPlan)
            {
                if (breedingPlan1.CurrentSpecies == speciesSelector1.SelectedSpecies)
                    breedingPlan1.UpdateIfNeeded();
                else
                {
                    breedingPlan1.SetSpecies(speciesSelector1.SelectedSpecies);
                }
            }
            else if (tabControlMain.SelectedTab == tabPageRaising)
            {
                raisingControl1.UpdateRaisingData(speciesSelector1.SelectedSpecies);
            }
            else if (tabControlMain.SelectedTab == tabPageMultiplierTesting)
            {
                statsMultiplierTesting1.SetSpecies(speciesSelector1.SelectedSpecies);
            }
        }

        private void DisplayCreatureInPedigree(Creature creature)
        {
            pedigree1.SetCreature(creature, true);
            tabControlMain.SelectedTab = tabPagePedigree;
        }

        private void ExtractBaby(Creature mother, Creature father)
        {
            if (mother == null || father == null)
                return;

            speciesSelector1.SetSpecies(mother.Species);
            rbBredExtractor.Checked = true;
            numericUpDownImprintingBonusTester.Value = 0;

            creatureInfoInputExtractor.Mother = mother;
            creatureInfoInputExtractor.Father = father;
            creatureInfoInputExtractor.CreatureOwner = mother.owner;
            creatureInfoInputExtractor.CreatureTribe = mother.tribe;
            creatureInfoInputExtractor.CreatureServer = mother.server;
            UpdateParentListInput(creatureInfoInputExtractor);
            tabControlMain.SelectedTab = tabPageExtractor;
        }

        private void numericUpDownImprintingBonusTester_ValueChanged(object sender, EventArgs e)
        {
            UpdateAllTesterValues();
            // calculate number of imprintings
            if (speciesSelector1.SelectedSpecies.breeding != null &&
                speciesSelector1.SelectedSpecies.breeding.maturationTimeAdjusted > 0)
                lbImprintedCount.Text =
                    "(" + Math.Round(
                        (double)numericUpDownImprintingBonusTester.Value / (100 *
                                                                             Utils.ImprintingGainPerCuddle(
                                                                                 speciesSelector1.SelectedSpecies
                                                                                     .breeding.maturationTimeAdjusted)),
                        2) + "×)";
            else lbImprintedCount.Text = string.Empty;
        }

        private void numericUpDownImprintingBonusExtractor_ValueChanged(object sender, EventArgs e)
        {
            // calculate number of imprintings
            if (speciesSelector1.SelectedSpecies.breeding != null &&
                speciesSelector1.SelectedSpecies.breeding.maturationTimeAdjusted > 0)
                lbImprintingCuddleCountExtractor.Text = "(" +
                                                        Math.Round(
                                                            (double)numericUpDownImprintingBonusExtractor.Value /
                                                            (100 * Utils.ImprintingGainPerCuddle(speciesSelector1
                                                                .SelectedSpecies.breeding.maturationTimeAdjusted))) +
                                                        "×)";
            else lbImprintingCuddleCountExtractor.Text = string.Empty;
        }

        private void checkBoxQuickWildCheck_CheckedChanged(object sender, EventArgs e)
        {
            UpdateQuickTamingInfo();
            var quickCheckMode = cbQuickWildCheck.Checked;
            if (quickCheckMode)
                ExtractionFailed();
            btExtractLevels.Enabled = !quickCheckMode;
            cbQuickWildCheck.BackColor = quickCheckMode ? Color.Orange : Color.Transparent;
        }

        private void onlinehelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RepositoryInfo.OpenWikiPage("Manual");
        }

        private void breedingPlanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RepositoryInfo.OpenWikiPage("Breeding-Plan");
        }

        private void extractionIssuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RepositoryInfo.OpenWikiPage("Extraction-issues");
        }

        private void ExportSelectedCreatureToClipboard(bool breeding = true, bool ARKml = false)
        {
            if (tabControlMain.SelectedTab == tabPageStatTesting || tabControlMain.SelectedTab == tabPageExtractor)
            {
                bool fromExtractor = tabControlMain.SelectedTab == tabPageExtractor;
                if (!fromExtractor || _extractor.ValidResults)
                {
                    CreatureInfoInput input;
                    bool bred;
                    double te, imprinting;
                    Species species = speciesSelector1.SelectedSpecies;
                    if (fromExtractor)
                    {
                        input = creatureInfoInputExtractor;
                        bred = rbBredExtractor.Checked;
                        te = rbWildExtractor.Checked ? -3 : _extractor.UniqueTamingEffectiveness();
                        imprinting = _extractor.ImprintingBonus;
                    }
                    else
                    {
                        input = creatureInfoInputTester;
                        bred = rbBredTester.Checked;
                        te = TamingEffectivenessTester;
                        imprinting = (double)numericUpDownImprintingBonusTester.Value / 100;
                    }

                    var levelStep = _creatureCollection.getWildLevelStep();
                    Creature creature = new Creature(species, input.CreatureName, input.CreatureOwner,
                        input.CreatureTribe, input.CreatureSex, GetCurrentWildLevels(fromExtractor),
                        GetCurrentDomLevels(fromExtractor), GetCurrentMutLevels(fromExtractor), te, bred, imprinting, levelStep)
                    {
                        colors = input.RegionColors,
                        ArkId = input.ArkId
                    };
                    creature.RecalculateCreatureValues(levelStep);
                    ExportImportCreatures.ExportToClipboard(creature, breeding, ARKml);
                }
                else
                    MessageBox.Show(Loc.S("noValidExtractedCreatureToExport"), Loc.S("NoValidData"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                CopySelectedCreatureFromLibraryToClipboard(breeding, ARKml);
            }
        }

        private void forSpreadsheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportForSpreadsheet();
        }

        private void plainTextbreedingValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportSelectedCreatureToClipboard(true, false);
        }

        private void plainTextbreedingValuesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExportSelectedCreatureToClipboard(true, false);
        }

        private void plainTextcurrentValuesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExportSelectedCreatureToClipboard(false, false);
        }

        private void plainTextcurrentValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportSelectedCreatureToClipboard(false, false);
        }

        private void copyCreatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedCreatureFromLibraryToClipboard();
        }

        /// <summary>
        /// Copies the values of the first selected creature in the library to the clipboard.
        /// </summary>
        private void CopySelectedCreatureFromLibraryToClipboard(bool breedingValues = true, bool ARKml = false)
        {
            if (listViewLibrary.SelectedIndices.Count > 0)
                ExportImportCreatures.ExportToClipboard(_creaturesDisplayed[listViewLibrary.SelectedIndices[0]], breedingValues, ARKml);
            else
                MessageBoxes.ShowMessageBox(Loc.S("noCreatureSelectedInLibrary"));
        }

        private void pasteCreatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteCreatureFromClipboard();
        }

        /// <summary>
        /// Import creature-data from the clipboard.
        /// </summary>
        private void PasteCreatureFromClipboard()
        {
            var importedCreature = ExportImportCreatures.ImportFromClipboard();
            if (importedCreature == null) return;

            importedCreature.Species = Values.V.SpeciesByBlueprint(importedCreature.speciesBlueprint);
            importedCreature.RecalculateCreatureValues(_creatureCollection?.getWildLevelStep());
            importedCreature.RecalculateNewMutations();
            UpdateParents(new List<Creature> { importedCreature });

            if (tabControlMain.SelectedTab == tabPageExtractor)
                SetCreatureValuesToExtractor(importedCreature);
            else
                EditCreatureInTester(importedCreature, true);
        }

        private void buttonRecalculateTops_Click(object sender, EventArgs e)
        {
            int consideredStats = 0;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _considerStatHighlight[Stats.DisplayOrder[s]] = checkedListBoxConsiderStatTop.GetItemChecked(s);

                // save consideredStats
                if (_considerStatHighlight[s])
                    consideredStats += 1 << s;
            }

            Properties.Settings.Default.consideredStats = consideredStats;

            // recalculate topstats
            CalculateTopStats(_creatureCollection.creatures);
            FilterLibRecalculate();
        }

        private void aliveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStatusOfSelectedCreatures(CreatureStatus.Available);
        }

        private void deadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStatusOfSelectedCreatures(CreatureStatus.Dead);
        }

        private void unavailableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStatusOfSelectedCreatures(CreatureStatus.Unavailable);
        }

        private void obeliskToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetStatusOfSelectedCreatures(CreatureStatus.Obelisk);
        }

        private void SetStatusOfSelectedCreatures(CreatureStatus s)
        {
            List<Creature> cs = new List<Creature>();
            foreach (int i in listViewLibrary.SelectedIndices)
                cs.Add(_creaturesDisplayed[i]);
            if (cs.Any())
                SetCreatureStatus(cs, s);
        }

        private void SetCreatureStatus(IEnumerable<Creature> cs, CreatureStatus s)
        {
            var changed = false;
            var deadStatusWasSet = false;
            var changedSpecies = new List<Species>();
            foreach (Creature c in cs)
            {
                if (c.Status != s)
                {
                    changed = true;
                    deadStatusWasSet = deadStatusWasSet || c.Status.HasFlag(CreatureStatus.Dead);
                    c.Status = s;
                    if (!changedSpecies.Contains(c.Species))
                        changedSpecies.Add(c.Species);
                }
            }

            if (changed)
            {
                // update list / recalculate topStats
                CalculateTopStats(_creatureCollection.creatures
                    .Where(c => changedSpecies.Contains(c.Species)).ToList());
                Species speciesIfOnlyOne = changedSpecies.Count == 1 ? changedSpecies[0] : null;
                if (s.HasFlag(CreatureStatus.Dead) ^ deadStatusWasSet)
                {
                    LibraryInfo.ClearInfo();
                    _creatureCollection.ResetExistingColors(speciesIfOnlyOne?.blueprintPath);
                }
                FilterLibRecalculate();
                UpdateStatusBar();
                SetCollectionChanged(true, speciesIfOnlyOne);
            }
        }

        private void editAllSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMultiSetter();
        }

        private void multiSetterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMultiSetter();
        }

        private void bestBreedingPartnersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count > 0)
            {
                Creature sc = (Creature)listViewLibrary.Items[listViewLibrary.SelectedIndices[0]].Tag;
                ShowBestBreedingPartner(sc);
            }
        }

        /// <summary>
        /// Displays the breeding planner with pairings exclusively for the given creature.
        /// </summary>
        /// <param name="c"></param>
        private void ShowBestBreedingPartner(Creature c)
        {
            if (c.Status != CreatureStatus.Available
                && MessageBox.Show(
                    "Selected Creature is currently not marked as \"Available\" and probably cannot be used for breeding right now. " +
                    "Do you want to change its status to \"Available\"?",
                    "Selected Creature not Available",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SetCreatureStatus(new List<Creature> { c }, CreatureStatus.Available);
                breedingPlan1.BreedingPlanNeedsUpdate = false;
            }
            else
            {
                breedingPlan1.BreedingPlanNeedsUpdate = true;
            }

            speciesSelector1.SetSpecies(c.Species);
            breedingPlan1.DetermineBestBreeding(c);
            tabControlMain.SelectedTab = tabPageBreedingPlan;
        }

        private void breedingPlanForSelectedCreaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count < 2)
            {
                MessageBoxes.ShowMessageBox("For a breeding plan you need to select at least 2 creatures.", "Too few creatures selected", MessageBoxIcon.Error);
                return;
            }

            var creatures = new List<Creature>();
            foreach (int i in listViewLibrary.SelectedIndices)
            {
                creatures.Add(_creaturesDisplayed[i]);
            }

            if (!creatures.Any()) return;

            speciesSelector1.SetSpecies(creatures[0].Species);
            breedingPlan1.DetermineBestBreeding(onlyConsiderTheseCreatures: creatures);
            tabControlMain.SelectedTab = tabPageBreedingPlan;
        }

        private void toolStripButtonSettings_Click(object sender, EventArgs e)
        {
            OpenSettingsDialog();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSettingsDialog();
        }

        /// <summary>
        /// Displays the settings window.
        /// </summary>
        /// <param name="page">The tab-page displayed first</param>
        private void OpenSettingsDialog(SettingsTabPages page = SettingsTabPages.Unknown)
        {
            if (page == SettingsTabPages.Unknown)
                page = _settingsLastTabPage;

            bool libraryTopCreatureColorHighlight = Properties.Settings.Default.LibraryHighlightTopCreatures;
            bool considerWastedStatsForTopCreatures = Properties.Settings.Default.ConsiderWastedStatsForTopCreatures;
            var gameSettingBefore = _creatureCollection.Game;
            var displayLibraryCreatureIndexBefore = Properties.Settings.Default.DisplayLibraryCreatureIndex;

            using (Settings settingsForm = new Settings(_creatureCollection, page))
            {
                var settingsSaved = settingsForm.ShowDialog() == DialogResult.OK;
                _settingsLastTabPage = settingsForm.LastTabPageIndex;

                if (!settingsSaved)
                    return;

                if (settingsForm.LanguageChanged) SetLocalizations();
                if (settingsForm.ColorRegionDisplayChanged)
                {
                    foreach (var sp in Values.V.species)
                        sp.InitializeColorRegions();
                    // update visible color region buttons
                    creatureInfoInputExtractor.RegionColors = creatureInfoInputExtractor.RegionColors;
                    creatureInfoInputTester.RegionColors = creatureInfoInputTester.RegionColors;
                }
            }

            if (_creatureCollection.Game != gameSettingBefore)
            {
                // ASA setting changed
                var loadAsa = gameSettingBefore != Ark.Asa;
                ReloadModValuesOfCollectionIfNeeded(loadAsa, false, false, false);
            }

            ApplySettingsToValues();
            CreatureColored.InitializeSpeciesImageLocation();
            creatureBoxListView.CreatureCollection = _creatureCollection;

            _creatureListSorter.UseNaturalSort = Properties.Settings.Default.UseNaturalSort;
            _creatureListSorter.IgnoreSpacesBetweenWords = Properties.Settings.Default.NaturalSortIgnoreSpaces;

            SetupAutoLoadFileWatcher();
            SetupExportFileWatcher();

            InitializeSpeechRecognition();
            _overlay?.SetInfoPositionsAndFontSize();
            if (Properties.Settings.Default.DevTools)
                statsMultiplierTesting1.CheckIfMultipliersAreEqualToSettings();
            devToolStripMenuItem.Visible = Properties.Settings.Default.DevTools;

            bool recalculateTopStats = considerWastedStatsForTopCreatures != Properties.Settings.Default.ConsiderWastedStatsForTopCreatures;
            if (recalculateTopStats)
                CalculateTopStats(_creatureCollection.creatures);

            breedingPlan1.IgnoreSexInBreedingPlan = Properties.Settings.Default.IgnoreSexInBreedingPlan;

            if (recalculateTopStats
                || libraryTopCreatureColorHighlight != Properties.Settings.Default.LibraryHighlightTopCreatures)
                FilterLibRecalculate();

            SetOverlayLocation();

            if (displayLibraryCreatureIndexBefore != Properties.Settings.Default.DisplayLibraryCreatureIndex)
                FilterLib();

            SetCollectionChanged(true);
        }

        /// <summary>
        /// Initializes or disposes the fileWatcher for the collection file, e.g. used in file syncing.
        /// </summary>
        private void SetupAutoLoadFileWatcher()
        {
            if (Properties.Settings.Default.syncCollection)
            {
                if (_fileSync == null)
                    _fileSync = new FileSync(_currentFileName, CollectionChanged);
            }
            else if (_fileSync != null)
            {
                _fileSync.Dispose();
                _fileSync = null;
            }
        }

        /// <summary>
        /// Initializes or disposes the fileWatcher for the export files, used in auto importing.
        /// </summary>
        private void SetupExportFileWatcher()
        {
            if (Properties.Settings.Default.AutoImportExportedCreatures
                && Utils.GetFirstImportExportFolder(out string exportFolderDefault))
            {
                if (_fileWatcherExports == null)
                {
                    _fileWatcherExports =
                           new FileWatcherExports(exportFolderDefault, ImportExportedAddIfPossible_WatcherThread);
                }
                else
                {
                    _fileWatcherExports.SetWatchFolder(exportFolderDefault);
                }
            }
            else if (_fileWatcherExports != null)
            {
                _fileWatcherExports.Dispose();
                _fileWatcherExports = null;
            }
        }

        /// <summary>
        /// Display the wild-levels, assuming it's a wild creature. Used for quick checking
        /// </summary>
        /// <param name="sIo"></param>
        private void StatIOQuickWildLevelCheck(StatIO sIo)
        {
            _clearExtractionCreatureData =
                true; // as soon as the user changes stat-values, it's assumed it's not an exported creature anymore
            if (!cbQuickWildCheck.Checked) return;
            int lvlWild = (int)Math.Round(
                (sIo.Input - speciesSelector1.SelectedSpecies.stats[sIo.statIndex].BaseValue) /
                (speciesSelector1.SelectedSpecies.stats[sIo.statIndex].BaseValue *
                 speciesSelector1.SelectedSpecies.stats[sIo.statIndex].IncPerWildLevel));
            sIo.LevelWild = lvlWild < 0 ? 0 : lvlWild;
            sIo.LevelMut = 0;
            sIo.LevelDom = 0;
            if (sIo.statIndex == Stats.Torpidity)
            {
                SetQuickTamingInfo(_statIOs[Stats.Torpidity].LevelWild + 1);
            }
        }

        private void CreateTimer(string name, DateTime time, Creature c, string group)
        {
            timerList1.AddTimer(name, time, c, group);
        }

        /// <summary>
        /// Performs an optical character recognition on either the specified image or a screenShot of the game and extracts the stat-values.
        /// </summary>
        /// <param name="imageFilePath">If specified, this image is taken instead of a screenShot.</param>
        /// <param name="manuallyTriggered">If false, the method is called by a timer based event when the user looks at a creature-inventory.</param>
        /// <param name="screenShotFromClipboard">If true, use the image in the clipboard for OCR.</param>
        public void DoOcr(string imageFilePath = null, bool manuallyTriggered = true, bool screenShotFromClipboard = false)
        {
            cbQuickWildCheck.Checked = false;

            double[] OcrValues = ArkOcr.Ocr.DoOcr(out string debugText, out string dinoName, out string speciesName,
                out string ownerName, out string tribeName, out Sex sex, imageFilePath, manuallyTriggered, screenShotFromClipboard);

            ocrControl1.output.Text = debugText;
            if (OcrValues.Length <= 1)
            {
                if (manuallyTriggered) MessageBoxes.ShowMessageBox(debugText, "OCR " + Loc.S("error"));
                return;
            }

            numericUpDownLevel.ValueSave = (decimal)OcrValues[9];

            creatureInfoInputExtractor.CreatureName = dinoName;
            if (!creatureInfoInputExtractor.OwnerLock)
                creatureInfoInputExtractor.CreatureOwner = ownerName;
            if (!creatureInfoInputExtractor.TribeLock)
                creatureInfoInputExtractor.CreatureTribe = tribeName;
            creatureInfoInputExtractor.CreatureSex = sex;
            creatureInfoInputExtractor.RegionColors = new byte[Ark.ColorRegionCount];
            creatureInfoInputTester.SetArkId(0, false);

            int[] displayedStatIndices = new[]
            {
                (int) Stats.Health,
                (int) Stats.Stamina,
                (int) Stats.Oxygen,
                (int) Stats.Food,
                (int) Stats.Weight,
                (int) Stats.MeleeDamageMultiplier,
                (int) Stats.SpeedMultiplier,
                (int) Stats.Torpidity
            };

            for (int i = 0; i < displayedStatIndices.Length; i++)
            {
                _statIOs[displayedStatIndices[i]].Input = _statIOs[displayedStatIndices[i]].percent
                    ? OcrValues[i] / 100
                    : OcrValues[i];
            }

            // use imprinting if existing
            if (OcrValues.Length > 8 && OcrValues[8] >= 0 &&
                (OcrValues[8] <= 100 || _creatureCollection.allowMoreThanHundredImprinting))
            {
                rbBredExtractor.Checked = true;
                if (!Properties.Settings.Default.OCRIgnoresImprintValue)
                    numericUpDownImprintingBonusExtractor.ValueSave = (decimal)OcrValues[8];
            }
            else
            {
                rbTamedExtractor.Checked = true;
            }

            // adjust the selected species if the ocr is triggered automatically or if the species cannot be determined by name
            Species speciesByName = null;
            if (!manuallyTriggered
                || (cbGuessSpecies.Checked
                    && !Values.V.TryGetSpeciesByName(speciesName, out speciesByName))
            )
            {
                double[] statValues = new double[Stats.StatsCount];
                for (int s = 0; s < displayedStatIndices.Length; s++)
                {
                    statValues[displayedStatIndices[s]] = OcrValues[s];
                }

                List<Species> possibleSpecies = DetermineSpeciesFromStats(statValues, speciesName);

                if (possibleSpecies.Count == 1)
                {
                    if (possibleSpecies[0] != null)
                        speciesSelector1.SetSpecies(possibleSpecies[0]);
                    ExtractLevels(true,
                        showLevelsInOverlay: !manuallyTriggered, possiblyMutagenApplied: true); // only one possible dino, use that one
                }
                else
                {
                    bool sameValues = true;

                    if (_lastOcrValues != null)
                        for (int i = 0; i < 10; i++)
                            if (OcrValues[i] != _lastOcrValues[i])
                            {
                                sameValues = false;
                                break;
                            }

                    // if there's more than one option, on manual we cycle through the options if we're trying multiple times
                    // on automated, we take the first one that yields an error-free level extraction
                    if (manuallyTriggered && sameValues)
                    {
                        int newIndex = (possibleSpecies.IndexOf(_lastOcrSpecies) + 1) % possibleSpecies.Count;
                        speciesSelector1.SetSpecies(possibleSpecies[newIndex], ignoreInRecent: true);
                        _lastOcrSpecies = possibleSpecies[newIndex];
                        _lastOcrValues = OcrValues;
                        ExtractLevels(true, possiblyMutagenApplied: true);
                    }
                    else
                    {
                        // automated, or first manual attempt at new values
                        bool foundPossiblyGood = false;
                        for (int speciesOption = 0;
                            !foundPossiblyGood && speciesOption < possibleSpecies.Count;
                            speciesOption++)
                        {
                            // if the last OCR'ed values are the same as this one, the user may not be happy with the dino species selection and want another one
                            // so we'll cycle to the next one, but only if the OCR is manually triggered, on auto trigger (i.e. overlay), don't change
                            speciesSelector1.SetSpecies(possibleSpecies[speciesOption], ignoreInRecent: true);
                            _lastOcrSpecies = possibleSpecies[speciesOption];
                            _lastOcrValues = OcrValues;
                            foundPossiblyGood = ExtractLevels(showLevelsInOverlay: !manuallyTriggered, possiblyMutagenApplied: true);
                        }
                    }
                }
            }
            else
            {
                if (speciesByName != null
                    && (speciesSelector1.SelectedSpecies == null
                        || speciesByName.name !=
                        speciesSelector1.SelectedSpecies.name)) // don't change already selected variant of a species
                {
                    speciesSelector1.SetSpecies(speciesByName);
                }

                ExtractLevels(possiblyMutagenApplied: true);
            }

            _lastOcrValues = OcrValues;
            if (tabControlMain.SelectedTab != TabPageOCR)
                tabControlMain.SelectedTab = tabPageExtractor;
        }

        /// <summary>
        /// Used for OCR. If a species of a creature is not known, try to determine it by its stats.
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="speciesName"></param>
        /// <returns></returns>
        private List<Species> DetermineSpeciesFromStats(double[] stats, string speciesName)
        {
            // todo implement https://en.wikipedia.org/wiki/Levenshtein_distance
            List<Species> possibleSpecies = new List<Species>();

            // for wild dinos, we can get the name directly.
            if (Values.V.TryGetSpeciesByName(speciesName, out Species speciesByName))
            {
                possibleSpecies.Add(speciesByName);
                return possibleSpecies;
            }

            // only consider species that can be domesticated and
            // that only have an oxygen value if they display it
            var speciesToCheck = Values.V.species
                .Where(sp => sp.IsDomesticable && !(stats[Stats.Oxygen] != 0 ^ sp.DisplaysStat(Stats.Oxygen)))
                .ToArray();
            // if dice-coefficient is promising, just take that
            const double minimumScore = 0.5;
            var speciesWithoutSpaces = speciesName.Replace(" ", string.Empty);
            var scores = speciesToCheck.Select(sp => (
                Score: DiceCoefficient.diceCoefficient(sp.name.Replace(" ", string.Empty), speciesWithoutSpaces),
                Species: sp
                ))
                .Where(s => s.Score > minimumScore)
                .OrderByDescending(o => o.Score)
                .ThenBy(o => o.Species.Mod == null) // prefer mod species
                .ThenBy(o => o.Species.variants?.Length ?? 0)
                .ToArray();

            if (scores.Any() && scores.First().Score > minimumScore)
            {
                possibleSpecies.AddRange(scores.Select(s => s.Species));
                return possibleSpecies;
            }

            if (stats.Length > Stats.StatsCount && stats[Stats.StatsCount] > 0)
            {
                // creature is imprinted, the following algorithm cannot handle this yet. use current selected species
                possibleSpecies.Add(speciesSelector1.SelectedSpecies);
                return possibleSpecies;
            }

            // later species are higher in the final order, so put unlikely variants first
            speciesToCheck = speciesToCheck
                .OrderBy(sp => sp.Mod != null) // prefer the mod variant, i.e. put them at the end
                .ThenByDescending(sp => sp.variants?.Length ?? 0)
                .ToArray();

            foreach (var species in speciesToCheck)
            {
                if (species == speciesSelector1.SelectedSpecies)
                    continue; // the currently selected species is ignored here and set as top priority at the end

                bool possible = true;
                // check that all stats are possible (no negative levels)
                double baseValue;
                double incWild;
                double possibleLevel;
                var tamedBaseHealthMultiplier = species.TamedBaseHealthMultiplier ?? 1;
                for (int s = Stats.StatsCount - 1; s >= 0; s--)
                {
                    baseValue = species.stats[s].BaseValue;
                    incWild = species.stats[s].IncPerWildLevel;
                    if (incWild > 0)
                    {
                        //possibleLevel = ((statIOs[s].Input - species.stats[s].AddWhenTamed) - baseValue) / (baseValue * incWild); // this fails if creature is wild
                        possibleLevel = (_statIOs[s].Input * (s == Stats.Health && tamedBaseHealthMultiplier < 1 ? 1 / tamedBaseHealthMultiplier : 1) - baseValue) / (baseValue * incWild);

                        if (possibleLevel < 0)
                        {
                            possible = false;
                            break;
                        }
                    }
                }

                if (!possible)
                    continue;

                // check that torpor is integer
                baseValue = species.stats[Stats.Torpidity].BaseValue;
                incWild = species.stats[Stats.Torpidity].IncPerWildLevel;

                possibleLevel =
                    (stats[Stats.Torpidity] - species.stats[Stats.Torpidity].AddWhenTamed -
                     baseValue) / (baseValue * incWild);
                double possibleLevelWild =
                    (stats[Stats.Torpidity] - baseValue) / (baseValue * incWild);

                if (possibleLevelWild < 0 || Math.Round(possibleLevel, 3) > (double)numericUpDownLevel.Value - 1 ||
                    Math.Round(possibleLevel, 3) % 1 > 0.001 && Math.Round(possibleLevelWild, 3) % 1 > 0.001)
                    continue;

                bool likely = true;

                // food and oxygen are stats that are unlikely to be levelled for most dinos, so let's order the possibilities with those first
                /*
                baseValue = Stats.statValue(i, 3).BaseValue;
                incWild = Stats.statValue(i, 3).IncPerWildLevel;
                possibleLevel = ((statIOs[3].Input - Stats.statValue(i, 3).AddWhenTamed) - baseValue) / (baseValue * incWild);

                if (possibleLevel < 0 || possibleLevel > (double)numericUpDownLevel.Value - 1)
                    continue;

                if (possibleLevel != (int)possibleLevel)
                    likely = false;
                */

                // now oxygen
                if (species.UsesStat(Stats.Oxygen))
                {
                    baseValue = species.stats[Stats.Oxygen].BaseValue;
                    incWild = species.stats[Stats.Oxygen].IncPerWildLevel;
                    possibleLevel =
                        (stats[Stats.Oxygen] - species.stats[Stats.Oxygen].AddWhenTamed -
                         baseValue) / (baseValue * incWild);

                    if (possibleLevel < 0 || possibleLevel > (double)numericUpDownLevel.Value - 1)
                        continue;

                    if (Math.Round(possibleLevel, 3) != (int)possibleLevel ||
                        possibleLevel > (double)numericUpDownLevel.Value / 2)
                        likely = false;
                }

                if (likely)
                    possibleSpecies.Insert(0, species); // insert species at top
                else
                    possibleSpecies.Add(species);
            }

            if (speciesSelector1.SelectedSpecies != null)
                possibleSpecies.Insert(0,
                    speciesSelector1
                        .SelectedSpecies); // adding the currently selected creature in the combobox as first priority. the user might already have that selected
            return possibleSpecies;
        }

        private void chkbToggleOverlay_CheckedChanged(object sender, EventArgs e)
        {
            var enableOverlay = cbToggleOverlay.Checked;

            cbToggleOverlay.BackColor = enableOverlay ? Color.LightGreen : SystemColors.ButtonFace;

            if (enableOverlay && (_overlay == null || _overlay.IsDisposed))
            {
                _overlay = new ARKOverlay
                {
                    ExtractorForm = this,
                    InfoDuration = Properties.Settings.Default.OverlayInfoDuration,
                    checkInventoryStats = Properties.Settings.Default.inventoryCheckTimer
                };
                _overlay.InitLabelPositions();
                _overlay.CreatureTimers = _creatureCollection.creatures.Where(c => c.ShowInOverlay).ToList();
                _overlay.timers = _creatureCollection.timerListEntries.Where(t => t.showInOverlay).OrderBy(t => t.time).ToArray();
            }

            if (enableOverlay && !SetOverlayLocation()) return;

            _overlay.Visible = enableOverlay;
            _overlay.EnableOverlayTimer = enableOverlay;

            // disable speechRecognition if overlay is disabled. (no use if no data can be displayed)
            if (_speechRecognition != null && !enableOverlay)
                _speechRecognition.Listen = false;
        }

        /// <summary>
        /// Sets the overlay location to the game location or the custom location.
        /// If the automatic location could not be found it disables the overlay and returns false.
        /// </summary>
        /// <returns></returns>
        private bool SetOverlayLocation()
        {
            if (!cbToggleOverlay.Checked) return true;

            if (Properties.Settings.Default.UseCustomOverlayLocation)
            {
                _overlay.Location = Properties.Settings.Default.CustomOverlayLocation;
            }
            else
            {
                var p = Process.GetProcessesByName(Properties.Settings.Default.OCRApp).FirstOrDefault();

                if (p == null)
                {
                    MessageBoxes.ShowMessageBox(
                        "Process for capturing screenshots and for overlay (e.g. the game, or a stream of the game) not found.\n" +
                        "Start the game or change the process in the settings.", "Game started?",
                        MessageBoxIcon.Warning);
                    cbToggleOverlay.Checked = false;
                    return false;
                }

                IntPtr mwhd = p.MainWindowHandle;
                Screen scr = Screen.FromHandle(mwhd);
                _overlay.Location = scr.WorkingArea.Location;
            }

            return true;
        }

        private void toolStripButtonCopy2Tester_Click(object sender, EventArgs e)
        {
            double te = _extractor.UniqueTamingEffectiveness();
            TamingEffectivenessTester = te;
            numericUpDownImprintingBonusTester.Value = numericUpDownImprintingBonusExtractor.Value;
            if (rbBredExtractor.Checked)
                rbBredTester.Checked = true;
            else if (rbTamedExtractor.Checked)
                rbTamedTester.Checked = true;
            else
                rbWildTester.Checked = true;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _testingIOs[s].LevelWild = _statIOs[s].LevelWild;
                _testingIOs[s].LevelMut = _statIOs[s].LevelMut;
                _testingIOs[s].LevelDom = _statIOs[s].LevelDom;
                TestingStatIoValueUpdate(_testingIOs[s]);
            }

            // set the data in the creatureInfoInput
            SetTesterInfoInputCreature(); // clear data
            creatureInfoInputTester.CreatureName = creatureInfoInputExtractor.CreatureName;
            creatureInfoInputTester.CreatureOwner = creatureInfoInputExtractor.CreatureOwner;
            creatureInfoInputTester.CreatureTribe = creatureInfoInputExtractor.CreatureTribe;
            creatureInfoInputTester.CreatureServer = creatureInfoInputExtractor.CreatureServer;
            creatureInfoInputTester.Mother = creatureInfoInputExtractor.Mother;
            creatureInfoInputTester.Father = creatureInfoInputExtractor.Father;
            creatureInfoInputTester.CreatureNote = creatureInfoInputExtractor.CreatureNote;
            creatureInfoInputTester.CooldownUntil = creatureInfoInputExtractor.CooldownUntil;
            creatureInfoInputTester.GrowingUntil = creatureInfoInputExtractor.GrowingUntil;
            creatureInfoInputTester.AddedToLibraryAt = creatureInfoInputExtractor.AddedToLibraryAt;
            creatureInfoInputTester.MutationCounterMother = creatureInfoInputExtractor.MutationCounterMother;
            creatureInfoInputTester.MutationCounterFather = creatureInfoInputExtractor.MutationCounterFather;
            creatureInfoInputTester.CreatureSex = creatureInfoInputExtractor.CreatureSex;
            creatureInfoInputTester.CreatureFlags = creatureInfoInputExtractor.CreatureFlags;
            creatureInfoInputTester.CreatureStatus = creatureInfoInputExtractor.CreatureStatus;
            creatureInfoInputTester.RegionColors = creatureInfoInputExtractor.RegionColors;
            creatureInfoInputTester.ColorIdsAlsoPossible = creatureInfoInputExtractor.ColorIdsAlsoPossible;

            tabControlMain.SelectedTab = tabPageStatTesting;
        }

        private void toolStripButtonClear_Click(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageExtractor)
            {
                ClearAll();
                numericUpDownLevel.Value = 1;
                creatureInfoInputExtractor.Clear();
            }
            else if (tabControlMain.SelectedTab == tabPageStatTesting)
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    _testingIOs[s].LevelWild = 0;
                    _testingIOs[s].LevelMut = 0;
                    _testingIOs[s].LevelDom = 0;
                }

                creatureInfoInputTester.Clear();
                SetTesterInfoInputCreature();
            }
        }

        private void toolStripButtonCopy2Extractor_Click(object sender, EventArgs e)
        {
            ClearAll();
            // copy values from tester over to extractor
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _statIOs[s].Input = _testingIOs[s].Input;
                if (_testingIOs[s].LevelDom > 0) _statIOs[s].DomLevelLockedZero = false;
            }

            if (rbBredTester.Checked)
                rbBredExtractor.Checked = true;
            else if (rbTamedTester.Checked)
                rbTamedExtractor.Checked = true;
            else
                rbWildExtractor.Checked = true;
            numericUpDownImprintingBonusExtractor.Value = numericUpDownImprintingBonusTester.Value;
            // set total level
            numericUpDownLevel.Value =
                _testingIOs[Stats.Torpidity].LevelWild + GetCurrentDomLevels(false).Sum() + 1;

            creatureInfoInputExtractor.CreatureSex = creatureInfoInputTester.CreatureSex;
            creatureInfoInputExtractor.RegionColors = creatureInfoInputTester.RegionColors;
            creatureInfoInputExtractor.ColorIdsAlsoPossible = creatureInfoInputTester.ColorIdsAlsoPossible;

            tabControlMain.SelectedTab = tabPageExtractor;
        }

        private void newToolStripButton1_Click(object sender, EventArgs e)
        {
            NewCollection();
        }

        private void openToolStripButton1_Click(object sender, EventArgs e)
        {
            LoadCollection();
        }

        private void saveToolStripButton1_Click(object sender, EventArgs e)
        {
            SaveCollection();
        }

        /// <summary>
        /// Display extracted levels in the overlay.
        /// </summary>
        private void ShowLevelsInOverlay()
        {
            if (_overlay == null || !_overlay.checkInventoryStats) return;

            var wildLevels = GetCurrentWildLevels();
            var tamedLevels = GetCurrentDomLevels();
            Color[] statColors = new Color[Stats.StatsCount];

            for (int i = 0; i < Stats.StatsCount; i++)
            {
                statColors[i] = _statIOs[i].BackColor;
            }

            int levelWild = wildLevels[Stats.Torpidity] + 1;
            int levelDom = tamedLevels.Sum();

            string extraText = speciesSelector1.SelectedSpecies.name;
            if (!_extractor.PostTamed)
            {
                string foodName = speciesSelector1.SelectedSpecies.taming.eats[0];
                int foodNeeded = Taming.FoodAmountNeeded(speciesSelector1.SelectedSpecies, levelWild,
                    Values.V.currentServerMultipliers.TamingSpeedMultiplier, foodName,
                    speciesSelector1.SelectedSpecies.taming.nonViolent);
                Taming.TamingTimes(speciesSelector1.SelectedSpecies, levelWild,
                    Values.V.currentServerMultipliers, foodName, foodNeeded, out _,
                    out TimeSpan duration, out int narcoBerries, out int ascerbicMushrooms, out int narcotics,
                    out int bioToxines, out double te, out _, out int bonusLevel, out _);
                extraText += $"\nTaming takes {(int)duration.TotalHours}:{duration:mm':'ss} with {foodNeeded} × {foodName}"
                             + "\n" + narcoBerries + " Narcoberries or " + ascerbicMushrooms +
                             " Ascerbic Mushrooms or " + narcotics + " Narcotics or " + bioToxines +
                             " Bio Toxines are needed"
                             + "\nTaming Effectiveness: " + Math.Round(100 * te, 1) + " % (+" + bonusLevel + " lvl)";
            }

            _overlay.SetStatLevels(wildLevels, tamedLevels, levelWild, levelDom, statColors);
            _overlay.SetInfoText(extraText);
        }

        private void findDuplicatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowDuplicateMergerAndCheckForDuplicates(_creatureCollection.creatures);
        }

        private void ShowDuplicateMergerAndCheckForDuplicates(List<Creature> creatureList)
        {
            MessageBox.Show("This feature is not yet included.");
            return;
            // TODO

            //if (mergingDuplicatesWindow == null || mergingDuplicatesWindow.IsDisposed)
            //{
            //    mergingDuplicatesWindow = new duplicates.MergingDuplicatesWindow();
            //    mergingDuplicatesWindow.RefreshLibrary += FilterLib;
            //}
            //mergingDuplicatesWindow.Show();
            //mergingDuplicatesWindow.CheckForDuplicates(creatureList);
        }

        private void btnReadValuesFromArk_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showOCRButton)
                DoOcr(screenShotFromClipboard: Properties.Settings.Default.OCRFromClipboard);
            else
                ImportExportedCreaturesDefaultFolder();
        }

        private void toolStripButtonAddTribe_Click(object sender, EventArgs e)
        {
            tribesControl1.AddTribe();
        }

        private void button2TamingCalc_Click(object sender, EventArgs e)
        {
            tamingControl1.SetSpecies(speciesSelector1.SelectedSpecies);
            if (cbQuickWildCheck.Checked)
                tamingControl1.SetLevel(_statIOs[Stats.Torpidity].LevelWild + 1);
            else
                tamingControl1.SetLevel((int)numericUpDownLevel.Value);
            tabControlMain.SelectedTab = tabPageTaming;
        }

        private void labelImprintedCount_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // set imprinting-count to closes integer
                if (speciesSelector1.SelectedSpecies.breeding != null &&
                    speciesSelector1.SelectedSpecies.breeding.maturationTimeAdjusted > 0)
                {
                    double imprintingGainPerCuddle =
                        Utils.ImprintingGainPerCuddle(speciesSelector1.SelectedSpecies.breeding.maturationTimeAdjusted);
                    int cuddleCount = (int)Math.Round((double)numericUpDownImprintingBonusTester.Value /
                                                       (100 * imprintingGainPerCuddle));
                    double imprintingBonus;
                    do
                    {
                        imprintingBonus = Math.Round(100 * cuddleCount * imprintingGainPerCuddle, 5);
                        cuddleCount--;
                    }
                    while (imprintingBonus > 100 && !_creatureCollection.allowMoreThanHundredImprinting);

                    numericUpDownImprintingBonusTester.ValueSave = (decimal)imprintingBonus;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                double imprintingFactorTorpor =
                    speciesSelector1.SelectedSpecies.StatImprintMultipliers[Stats.Torpidity] *
                    _creatureCollection.serverMultipliers.BabyImprintingStatScaleMultiplier;
                // set imprinting value so the set levels in the tester yield the value in the extractor
                double imprintingBonus = imprintingFactorTorpor != 0
                    ? (_statIOs[Stats.Torpidity].Input / StatValueCalculation.CalculateValue(
                        speciesSelector1.SelectedSpecies, Stats.Torpidity,
                        _testingIOs[Stats.Torpidity].LevelWild, 0, 0, true, 1, 0) - 1) / imprintingFactorTorpor
                    : 0;
                if (imprintingBonus < 0)
                    imprintingBonus = 0;
                if (!_creatureCollection.allowMoreThanHundredImprinting && imprintingBonus > 1)
                    imprintingBonus = 1;
                numericUpDownImprintingBonusTester.ValueSave = 100 * (decimal)imprintingBonus;
            }
        }

        /// <summary>
        /// Loads all mod value files of a collection.
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="showResult"></param>
        /// <param name="applySettings"></param>
        /// <returns></returns>
        private bool LoadModValuesOfCollection(CreatureCollection cc, bool showResult, bool applySettings)
        {
            if (cc == null) return false;

            if (!applySettings && (cc.modIDs == null || !cc.modIDs.Any()))
            {
                // nothing to do, and no error, the modHash seems to be wrong.
                cc.UpdateModList();
                UpdateAsaIndicator();
                return true;
            }

            if (cc.modIDs == null) cc.modIDs = new List<string>();
            cc.modIDs = cc.modIDs.Distinct().ToList();

            List<string> filePaths = new List<string>();

            var unknownModIDs = new List<string>();

            // determine file-names of mod-value files
            foreach (var modId in cc.modIDs)
            {
                if (Values.V.modsManifest.modsByID.TryGetValue(modId, out var modInfo)
                    && modInfo.mod?.FileName != null)
                    filePaths.Add(modInfo.mod.FileName);
                else
                    unknownModIDs.Add(modId);
            }

            if (unknownModIDs.Any())
                MessageBox.Show("The library is dependent on some unknown mods with the following IDs:\n\n"
                                + string.Join("\n", unknownModIDs) + "\n\n"
                                + "There are no mod files available for an automatic download.\n"
                                + "The library may not display all creatures.",
                    "Unknown mod IDs", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            bool result = LoadModValueFiles(filePaths, showResult, applySettings, out _);
            UpdateAsaIndicator();
            return result;

        }

        /// <summary>
        /// Displays a small indicator in the UI if the ASA values are loaded.
        /// </summary>
        private void UpdateAsaIndicator()
        {
            LbAsa.Visible = _creatureCollection.Game == Ark.Asa;
        }

        private void loadAdditionalValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var modValuesManager = new ModValuesManager
            {
                CreatureCollection = _creatureCollection
            })
            {
                Utils.SetWindowRectangle(modValuesManager, Properties.Settings.Default.ModManagerWindowRect);
                modValuesManager.ShowDialog();
                (Properties.Settings.Default.ModManagerWindowRect, _) = Utils.GetWindowRectangle(modValuesManager);
            }

            // if Asa values are added or removed manually, adjust Asa setting
            _creatureCollection.Game = _creatureCollection.modIDs?.Contains(Ark.Asa) == true ? Ark.Asa : null;
            ReloadModValuesOfCollectionIfNeeded();
        }

        /// <summary>
        /// Loads mod value files according to the ModList of the library.
        /// </summary>
        /// <param name="onlyAdd">If true the values are not reset to the default first.</param>
        private void ReloadModValuesOfCollectionIfNeeded(bool onlyAdd = false, bool showResult = true, bool applySettings = true, bool setCollectionChanged = true)
        {
            // if the mods for the library changed,
            // first check if all mod value files are available and load missing files if possible,
            // then reload all values and mod values
            if (_creatureCollection.ModValueReloadNeeded)
            {
                var modValuesNeedToBeLoaded = _creatureCollection.modIDs?.Any() == true;
                // first reset values to default if needed
                if (!onlyAdd)
                    LoadStatAndKibbleValues(!modValuesNeedToBeLoaded);
                // then load mod values if any
                if (modValuesNeedToBeLoaded)
                    LoadModValuesOfCollection(_creatureCollection, showResult, applySettings);
                else
                    UpdateAsaIndicator();

                if (setCollectionChanged)
                    SetCollectionChanged(true);
            }
        }

        private void toolStripButtonAddPlayer_Click(object sender, EventArgs e)
        {
            tribesControl1.AddPlayer();
        }

        private void UpdateStatusBar()
        {
            var creatureCount = _creatureCollection.creatures.Where(c => !c.flags.HasFlag(CreatureFlags.Placeholder))
                .ToArray();
            int total = creatureCount.Length;
            int obelisk = creatureCount.Count(c => c.Status == CreatureStatus.Obelisk);
            int cryopod = creatureCount.Count(c => c.Status == CreatureStatus.Cryopod);

            var loadedMods = _creatureCollection.ModList?.Where(m => !m.expansion).ToArray();

            toolStripStatusLabel.Text = total + " creatures in Library"
                                              + (total > 0
                                                  ? " ("
                                                    + "available: " + creatureCount.Count(c =>
                                                        c.Status == CreatureStatus.Available)
                                                    + ", unavailable: " + creatureCount.Count(c =>
                                                        c.Status == CreatureStatus.Unavailable)
                                                    + ", dead: " + creatureCount.Count(c =>
                                                        c.Status == CreatureStatus.Dead)
                                                    + (obelisk > 0 ? ", obelisk: " + obelisk : string.Empty)
                                                    + (cryopod > 0 ? ", cryopod: " + cryopod : string.Empty)
                                                    + ")"
                                                  : string.Empty)
                                              + ". v" + Application.ProductVersion
                                              + "-BETA" // TODO BETA indicator
                                              + " / values: " + Values.V.Version +
                                              (loadedMods?.Any() == true
                                                  ? ", additional values from " + _creatureCollection.ModList.Count +
                                                    " mod" + (loadedMods.Length == 1 ? string.Empty : "s") + " (" + string.Join(", ",
                                                        loadedMods.Select(m => m.title)) +
                                                    ")"
                                                  : string.Empty);
        }

        private void labelListening_Click(object sender, EventArgs e)
        {
            _speechRecognition?.ToggleListening();
        }

        private void CreateIncubationTimer(Creature mother, Creature father, TimeSpan incubationDuration,
            bool incubationStarted)
        {
            raisingControl1.AddIncubationTimer(mother, father, incubationDuration, incubationStarted);
            _libraryNeedsUpdate = true; // because mating-cooldown of mother was set
        }

        private void EnableGlobalTimerIfNeeded()
        {
            _timerGlobal.Enabled = timerList1.TimerIsNeeded
                                   || raisingControl1.TimerIsNeeded;
        }

        private void TimerGlobal_Tick(object sender, EventArgs e)
        {
            timerList1.Tick();
            raisingControl1.Tick();
        }

        private void cbEvolutionEvent_CheckedChanged(object sender, EventArgs e)
        {
            ApplyEvolutionMultipliers();
        }

        private void ApplyEvolutionMultipliers()
        {
            Values.V.ApplyMultipliers(_creatureCollection, cbEventMultipliers.Checked, false);

            tamingControl1.SetServerMultipliers(Values.V.currentServerMultipliers);
            breedingPlan1.UpdateBreedingData();
            raisingControl1.UpdateRaisingData();
        }

        private void toolStripButtonDeleteExpiredIncubationTimers_Click(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageRaising)
                raisingControl1.DeleteAllExpiredIncubationTimers();
            else if (tabControlMain.SelectedTab == tabPageTimer)
                timerList1.DeleteAllExpiredTimers();
        }

        private static void OcrUpdateWhiteThreshold(byte value)
        {
            Properties.Settings.Default.OCRWhiteThreshold = value;
        }

        private void toolStripCBTempCreatures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripCBTempCreatures.SelectedIndex >= 0 &&
                toolStripCBTempCreatures.SelectedIndex < _creatureCollection.creaturesValues.Count)
            {
                ExtractValuesInExtractor(_creatureCollection.creaturesValues[toolStripCBTempCreatures.SelectedIndex],
                    null, false, false, out _);
                toolStripButtonDeleteTempCreature.Visible = true;
            }
            else
            {
                toolStripButtonDeleteTempCreature.Visible = false;
            }
        }

        private void SetCreatureValuesToTester(CreatureValues cv)
        {
            speciesSelector1.SetSpecies(Values.V.SpeciesByBlueprint(cv.speciesBlueprint));
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _testingIOs[s].LevelWild = cv.levelsWild[s];
                _testingIOs[s].LevelMut = cv.levelsMut[s];
                _testingIOs[s].LevelDom = cv.levelsDom[s];
            }

            SetCreatureValuesToInfoInput(cv, creatureInfoInputTester);

            TamingEffectivenessTester = cv.tamingEffMin;

            if (cv.isBred)
                rbBredTester.Checked = true;
            else if (cv.isTamed)
                rbTamedTester.Checked = true;
            else rbWildTester.Checked = true;
            numericUpDownImprintingBonusTester.ValueSave = (decimal)cv.imprintingBonus * 100;
        }

        private void SetCreatureValuesToInfoInput(CreatureValues cv, CreatureInfoInput input)
        {
            input.CreatureName = cv.name;
            if (!creatureInfoInputExtractor.OwnerLock)
                input.CreatureOwner = cv.owner;
            if (!creatureInfoInputExtractor.TribeLock)
                input.CreatureTribe = cv.tribe;
            if (!creatureInfoInputExtractor.LockServer)
                input.CreatureServer = cv.server;
            input.CreatureNote = cv.note;
            input.CreatureSex = cv.sex;
            input.CreatureGuid = cv.guid;
            input.CreatureFlags = cv.flags;
            input.Mother = cv.Mother;
            input.Father = cv.Father;
            input.RegionColors = cv.colorIDs;
            input.ColorIdsAlsoPossible = cv.ColorIdsAlsoPossible;
            input.SetArkId(cv.ARKID, cv.guid == Utils.ConvertArkIdToGuid(cv.ARKID));
            input.MutationCounterMother = cv.mutationCounterMother;
            input.MutationCounterFather = cv.mutationCounterFather;
            input.GrowingUntil = cv.growingUntil;
            input.CooldownUntil = cv.cooldownUntil;
            input.MotherArkId = cv.motherArkId;
            input.FatherArkId = cv.fatherArkId;
            input.CreatureStatus = CreatureStatus.Available;
            input.SetTimersToChanged();
        }

        private void toolStripButtonSaveCreatureValuesTemp_Click(object sender, EventArgs e)
        {
            _creatureCollection.creaturesValues = _creatureCollection.creaturesValues.Append(GetCreatureValuesFromExtractor())
                .OrderBy(c => c.Species?.DescriptiveNameAndMod).ThenBy(c => c.name).ToList();
            SetCollectionChanged(true);
            UpdateTempCreatureDropDown();
        }

        /// <summary>
        /// Returns the entered creature values from the extractor.
        /// </summary>
        private CreatureValues GetCreatureValuesFromExtractor()
        {
            CreatureValues cv = new CreatureValues();
            for (int s = 0; s < Stats.StatsCount; s++)
                cv.statValues[s] = _statIOs[s].Input;
            cv.speciesBlueprint = speciesSelector1.SelectedSpecies.blueprintPath;
            cv.name = creatureInfoInputExtractor.CreatureName;
            cv.owner = creatureInfoInputExtractor.CreatureOwner;
            cv.tribe = creatureInfoInputExtractor.CreatureTribe;
            cv.server = creatureInfoInputExtractor.CreatureServer;
            cv.note = creatureInfoInputExtractor.CreatureNote;
            cv.sex = creatureInfoInputExtractor.CreatureSex;
            cv.flags = creatureInfoInputExtractor.CreatureFlags;
            cv.Mother = creatureInfoInputExtractor.Mother;
            cv.Father = creatureInfoInputExtractor.Father;
            cv.colorIDs = creatureInfoInputExtractor.RegionColors;
            cv.ColorIdsAlsoPossible = creatureInfoInputExtractor.ColorIdsAlsoPossible;

            cv.level = (int)numericUpDownLevel.Value;
            cv.tamingEffMin = (double)numericUpDownLowerTEffBound.Value * 0.01;
            cv.tamingEffMax = (double)numericUpDownUpperTEffBound.Value * 0.01;

            cv.isBred = false;
            cv.isTamed = false;
            if (rbBredExtractor.Checked)
                cv.isBred = true;
            else if (rbTamedExtractor.Checked)
                cv.isTamed = true;
            cv.imprintingBonus = (double)numericUpDownImprintingBonusExtractor.Value * 0.01;

            return cv;
        }

        private void toolStripButtonDeleteTempCreature_Click(object sender, EventArgs e)
        {
            if (toolStripCBTempCreatures.SelectedIndex >= 0
                && MessageBox.Show("Remove the data of this cached creature?", "Delete?", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _creatureCollection.creaturesValues.RemoveAt(toolStripCBTempCreatures.SelectedIndex);
                UpdateTempCreatureDropDown();
                SetCollectionChanged(true, null);
            }
        }

        /// <summary>
        /// Update the combo list with the temporary saved creature-values.
        /// </summary>
        private void UpdateTempCreatureDropDown()
        {
            toolStripCBTempCreatures.Items.Clear();
            foreach (CreatureValues cv in _creatureCollection.creaturesValues)
                toolStripCBTempCreatures.Items.Add($"{cv.name} ({cv.Species?.name ?? "unknown species"}, Lv {cv.level})");
        }

        /// <summary>
        /// Collects the data needed for the name pattern editor.
        /// </summary>
        private void CreatureInfoInput_CreatureDataRequested(CreatureInfoInput input, bool openPatternEditor,
            bool updateInheritance, bool showDuplicateNameWarning, int namingPatternIndex, Creature alreadyExistingCreature)
        {
            var cr = CreateCreatureFromExtractorOrTester(input);

            if (openPatternEditor)
            {
                input.OpenNamePatternEditor(cr, _topLevels.TryGetValue(cr.Species, out var tl) ? tl : null,
                    _lowestLevels.TryGetValue(cr.Species, out var ll) ? ll : null,
                    _customReplacingNamingPattern, namingPatternIndex, ReloadNamePatternCustomReplacings);

                UpdatePatternButtons();
            }
            else if (updateInheritance)
            {
                if (_extractor.ValidResults && !_dontUpdateExtractorVisualData)
                    input.UpdateParentInheritances(cr);
            }
            else
            {
                CreatureCollection.ColorExisting[] colorAlreadyExistingInformation = null;
                if (Properties.Settings.Default.NamingPatterns != null
                    && !string.IsNullOrEmpty(Properties.Settings.Default.NamingPatterns[namingPatternIndex])
                    && Properties.Settings.Default.NamingPatterns[namingPatternIndex].IndexOf("#colorNew:", StringComparison.InvariantCultureIgnoreCase) != -1)
                    colorAlreadyExistingInformation = _creatureCollection.ColorAlreadyAvailable(cr.Species, input.RegionColors, out _);
                input.ColorAlreadyExistingInformation = colorAlreadyExistingInformation;

                input.GenerateCreatureName(cr, alreadyExistingCreature, _topLevels.TryGetValue(cr.Species, out var tl) ? tl : null,
                    _lowestLevels.TryGetValue(cr.Species, out var ll) ? ll : null,
                    _customReplacingNamingPattern, showDuplicateNameWarning, namingPatternIndex);
                if (Properties.Settings.Default.PatternNameToClipboardAfterManualApplication)
                {
                    if (string.IsNullOrEmpty(input.CreatureName))
                        Clipboard.Clear();
                    else Clipboard.SetText(input.CreatureName);
                }
            }
        }

        private Creature CreateCreatureFromExtractorOrTester(CreatureInfoInput input)
        {
            Creature cr = new Creature
            {
                ArkId = input.ArkId,
                ArkIdImported = input.ArkIdImported,
                guid = input.CreatureGuid,
                name = input.CreatureName
            };
            if (input == creatureInfoInputExtractor)
            {
                cr.levelsWild = _statIOs.Select(s => s.LevelWild).ToArray();
                cr.levelsMutated = _statIOs.Select(s => s.LevelMut).ToArray();
                cr.imprintingBonus = _extractor.ImprintingBonus;
                cr.tamingEff = _extractor.UniqueTamingEffectiveness();
                cr.isBred = rbBredExtractor.Checked;
                cr.topBreedingStats = _statIOs.Select(s =>
                    s.TopLevel.HasFlag(LevelStatus.TopLevel) || s.TopLevel.HasFlag(LevelStatus.NewTopLevel)).ToArray();
            }
            else
            {
                cr.levelsWild = _testingIOs.Select(s => s.LevelWild).ToArray();
                cr.levelsMutated = _testingIOs.Select(s => s.LevelMut).ToArray();
                cr.imprintingBonus = (double)numericUpDownImprintingBonusTester.Value / 100;
                cr.tamingEff = TamingEffectivenessTester;
                cr.isBred = rbBredTester.Checked;
            }

            Species species = speciesSelector1.SelectedSpecies;
            cr.Species = species;
            cr.RecalculateCreatureValues(_creatureCollection.getWildLevelStep());
            input.SetCreatureData(cr);
            return cr;
        }

        /// <summary>
        /// Updates the background of the pattern buttons to indicate which are not empty.
        /// </summary>
        private void UpdatePatternButtons()
        {
            creatureInfoInputExtractor.SetNamePatternButtons(Properties.Settings.Default.NamingPatterns);
            creatureInfoInputTester.SetNamePatternButtons(Properties.Settings.Default.NamingPatterns);
        }

        private void ExtractionTestControl1_CopyToTester(string speciesBP, int[] wildLevels, int[] domLevels, int[] mutLevels,
            bool postTamed, bool bred, double te, double imprintingBonus, bool gotoTester,
            testCases.TestCaseControl tcc)
        {
            NewCollection();
            LoadMultipliersFromTestCase(tcc.TestCase);
            Species species = Values.V.SpeciesByBlueprint(speciesBP);
            if (species != null)
            {
                EditCreatureInTester(
                    new Creature(species, null, null, null, Sex.Unknown, wildLevels, domLevels, mutLevels,
                        te, bred, imprintingBonus), true);
                if (gotoTester) tabControlMain.SelectedTab = tabPageStatTesting;
            }
        }

        private void ExtractionTestControl1_CopyToExtractor(string speciesBlueprint, int level, double[] statValues,
            bool postTamed, bool bred, double imprintingBonus, bool gotoExtractor, testCases.TestCaseControl tcc)
        {
            // test if the testCase can be extracted
            NewCollection();
            ClearAll();
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _statIOs[s].DomLevelLockedZero = false;
                _statIOs[s].Input = statValues[s];
            }

            speciesSelector1.SetSpecies(Values.V.SpeciesByBlueprint(speciesBlueprint));

            numericUpDownLevel.ValueSave = level;
            numericUpDownLowerTEffBound.Value = 0;
            numericUpDownUpperTEffBound.Value = 100;

            if (bred)
                rbBredExtractor.Checked = true;
            else if (postTamed)
                rbTamedExtractor.Checked = true;
            else rbWildExtractor.Checked = true;
            numericUpDownImprintingBonusExtractor.ValueSave = (decimal)imprintingBonus * 100;

            LoadMultipliersFromTestCase(tcc.TestCase);

            var watch = Stopwatch.StartNew();
            ExtractLevels(true);
            watch.Stop();

            bool success = _extractor.ValidResults;
            if (!success)
                tcc.SetTestResult(false, (int)watch.ElapsedMilliseconds, 0, "extraction failed");
            else
            {
                string testText = null;
                // test if the expected levels are possible
                int resultCount =
                    -Stats.StatsCount; // one result per stat is allowed, only count the additional ones. // TODO only consider possible stats
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    resultCount += _extractor.Results[s].Count;
                    bool statValid = false;
                    for (int r = 0; r < _extractor.Results[s].Count; r++)
                    {
                        if (_extractor.Results[s][r].levelWild == -1 ||
                            s == Stats.SpeedMultiplier && _extractor.Results[s][r].levelWild == 0 ||
                            _extractor.Results[s][r].levelWild == tcc.TestCase.levelsWild[s]
                            && _extractor.Results[s][r].levelDom == tcc.TestCase.levelsDom[s]
                            && (_extractor.Results[s][r].TE.Max == -1 ||
                                _extractor.Results[s][r].TE.Includes(tcc.TestCase.tamingEff))
                        )
                        {
                            statValid = true;
                            break;
                        }
                    }

                    if (!statValid)
                    {
                        success = false;
                        testText = Utils.StatName(s, true) + " not expected value";
                        break;
                    }
                }

                tcc.SetTestResult(success, (int)watch.ElapsedMilliseconds, resultCount, testText);
            }

            if (gotoExtractor) tabControlMain.SelectedTab = tabPageExtractor;
        }

        private void LoadMultipliersFromTestCase(testCases.ExtractionTestCase etc)
        {
            // set all stat-multipliers from testCase
            _creatureCollection.serverMultipliers = etc.serverMultipliers.Copy(true);
            _creatureCollection.singlePlayerSettings = etc.singleplayerSettings;
            _creatureCollection.AtlasSettings = etc.AtlasSettings;
            _creatureCollection.allowMoreThanHundredImprinting = etc.allowMoreThanHundredPercentImprinting;
            _creatureCollection.maxWildLevel = etc.maxWildLevel;

            if (Values.V.loadedModsHash == 0 || Values.V.loadedModsHash != etc.modListHash)
                LoadStatAndKibbleValues(false); // load original multipliers if they were changed

            if (etc.ModIDs.Any())
                LoadModValueFiles(
                    Values.V.modsManifest.modsByFiles.Where(mi => etc.ModIDs.Contains(mi.Value.mod.id))
                        .Select(mi => mi.Value.mod.FileName).ToList(),
                    false, false, out _);

            Values.V.ApplyMultipliers(_creatureCollection);
        }

        private void tsBtAddAsExtractionTest_Click(object sender, EventArgs e)
        {
            if (Utils.ShowTextInput("Testcase name", out string name, "Name of the testcase"))
            {
                testCases.ExtractionTestCase etc = new testCases.ExtractionTestCase
                {
                    testName = name,
                    bred = rbBredTester.Checked,
                    postTamed = rbTamedTester.Checked || rbBredTester.Checked
                };
                etc.tamingEff = etc.bred ? 1 : etc.postTamed ? TamingEffectivenessTester : -3;
                etc.imprintingBonus = etc.bred ? (double)numericUpDownImprintingBonusTester.Value / 100 : 0;
                etc.levelsDom = GetCurrentDomLevels(false);
                etc.levelsWild = GetCurrentWildLevels(false);
                etc.ModIDs = _creatureCollection.modIDs?.ToList();
                etc.serverMultipliers = _creatureCollection.serverMultipliers;
                etc.Species = speciesSelector1.SelectedSpecies;
                etc.singleplayerSettings = _creatureCollection.singlePlayerSettings;
                etc.AtlasSettings = _creatureCollection.AtlasSettings;
                etc.allowMoreThanHundredPercentImprinting = _creatureCollection.allowMoreThanHundredImprinting;
                etc.maxWildLevel = _creatureCollection.maxWildLevel;

                double[] statValues = new double[Stats.StatsCount];
                for (int s = 0; s < Stats.StatsCount; s++)
                    statValues[s] = _statIOs[s].Input;
                etc.statValues = statValues;

                extractionTestControl1.AddTestCase(etc);
                tabControlMain.SelectedTab = tabPageExtractionTests;
            }
        }

        private void copyToMultiplierTesterToolStripButton_Click(object sender, EventArgs e)
        {
            bool fromExtractor = tabControlMain.SelectedTab == tabPageExtractor;
            var tamed = fromExtractor ? rbTamedExtractor.Checked : rbTamedTester.Checked;
            var bred = fromExtractor ? rbBredExtractor.Checked : rbBredTester.Checked;

            double[] statValues = new double[Stats.StatsCount];
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                statValues[s] = _statIOs[s].IsActive
                    ? _statIOs[s].Input
                    : StatValueCalculation.CalculateValue(speciesSelector1.SelectedSpecies, s, 0, 0, 0, tamed || bred);
            }

            var wildLevels = GetCurrentWildLevels(false);
            // the torpor level of the tester is only the sum of the recognized stats. Use the level of the extractor, if that value was recognized.
            if (_statIOs[Stats.Torpidity].LevelWild > 0)
                wildLevels[Stats.Torpidity] = _statIOs[Stats.Torpidity].LevelWild;

            statsMultiplierTesting1.SetCreatureValues(statValues,
                wildLevels,
                GetCurrentDomLevels(false),
                (int)numericUpDownLevel.Value,
                TamingEffectivenessTester,
                (double)(fromExtractor
                    ? numericUpDownImprintingBonusExtractor.Value
                    : numericUpDownImprintingBonusTester.Value) / 100,
                tamed,
                bred,
                speciesSelector1.SelectedSpecies);
            tabControlMain.SelectedTab = tabPageMultiplierTesting;
        }

        private void StatsMultiplierTesting1_OnApplyMultipliers()
        {
            Values.V.ApplyMultipliers(_creatureCollection);
            SetCollectionChanged(true);
        }

        private void openFolderOfCurrentFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFolderInExplorer(_currentFileName);
        }

        /// <summary>
        /// Opens the folder in the explorer. If it's a file, it will be selected.
        /// </summary>
        private static void OpenFolderInExplorer(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            bool isFile = false;

            if (File.Exists(path))
                isFile = true;
            else if (!Directory.Exists(path))
                return;

            Process.Start("explorer.exe",
                $"{(isFile ? "/select, " : string.Empty)}\"{path}\"");
        }

        private void customStatOverridesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var frm = new mods.CustomStatOverridesEditor(Values.V.species, _creatureCollection))
            {
                Utils.SetWindowRectangle(frm, Properties.Settings.Default.CustomStatOverrideFormRectangle);
                frm.ShowDialog();
                if (frm.StatOverridesChanged)
                {
                    Values.V.ApplyMultipliers(_creatureCollection, eventMultipliers: cbEventMultipliers.Checked,
                        applyStatMultipliers: true);
                    SetCollectionChanged(true);
                    if (tabControlMain.SelectedTab == tabPageStatTesting)
                    {
                        UpdateAllTesterValues();
                    }
                }

                (Properties.Settings.Default.CustomStatOverrideFormRectangle, _) = Utils.GetWindowRectangle(frm);
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        /// <summary>
        /// If a file was dropped onto the extractor, try to extract it as an exported creature, perform an ocr on an image, or if it's a folder, import all included files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (!(e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Any()))
                return;
            ProcessDroppedFiles(files);
        }

        private void ProcessDroppedFiles(string[] files)
        {
            string filePath = files[0];
            // if first item is folder, only consider all files in first folder
            if (File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
            {
                // if folder contains .sav files (mod dino export gun)
                files = Directory.GetFiles(filePath);
                if (!files.Any())
                {
                    MessageBoxes.ShowMessageBox("No files to import in first folder");
                    return;
                }
                filePath = files[0];
            }

            switch (Path.GetExtension(filePath).ToLower())
            {
                case ".gz":
                    OpenCompressedFile(filePath, true);
                    return;
                case ".ini" when files.Length == 1:
                    ExtractExportedFileInExtractor(filePath, out _, out _);
                    break;
                case ".ini":
                    ShowExportedCreatureListControl();
                    _exportedCreatureList.LoadFiles(files);
                    break;
                case ".sav":
                case ".json":
                    ImportExportGunFiles(files, out _, out _, out _);
                    break;
                case ".asb":
                case ".xml":
                    {
                        if (DiscardChangesAndLoadNewLibrary())
                        {
                            LoadCollectionFile(filePath);
                        }

                        break;
                    }
                case ".zip":
                    {
                        if (DiscardChangesAndLoadNewLibrary())
                        {
                            OpenZippedLibrary(filePath);
                        }

                        break;
                    }
                case ".ark":
                    {
                        if (MessageBox.Show(
                                $"Import all of the creatures in the following ARK save file to the currently opened library?\n{filePath}",
                                "Import savefile?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            RunSavegameImport(new ATImportFileLocation(null, null, filePath));
                        break;
                    }
                default:
                    DoOcr(filePath);
                    break;
            }
        }

        private bool OpenCompressedFile(string filePath, bool usegzip)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return false;

            try
            {
                // get temp folder for zipping
                var tempFolder = FileService.GetTempDirectory();
                if (usegzip)
                {
                    var fileName = Path.GetFileName(filePath);
                    var extractedFilePath = Path.Combine(tempFolder, fileName.Substring(0, fileName.Length - Path.GetExtension(fileName).Length));

                    using (FileStream compressedFileStream = File.Open(filePath, FileMode.Open))
                    using (FileStream outputFileStream = File.Create(extractedFilePath))
                    using (var decompressor = new GZipStream(compressedFileStream, CompressionMode.Decompress))
                        decompressor.CopyTo(outputFileStream);
                }
                else
                {
                    // unzip files
                    ZipFile.ExtractToDirectory(filePath, tempFolder);
                }
                var extractedFilePaths = Directory.GetFiles(tempFolder);
                if (!extractedFilePaths.Any())
                {
                    MessageBoxes.ShowMessageBox("No files in archive found: " + filePath, "Error while loading compressed file");
                    return false;
                }
                ProcessDroppedFiles(extractedFilePaths);

                // delete temp extracted file
                foreach (var f in extractedFilePaths)
                    FileService.TryDeleteFile(f);
                FileService.TryDeleteDirectory(tempFolder);
            }
            catch (Exception ex)
            {
                MessageBoxes.ExceptionMessageBox(ex, "Error while loading compressed file " + filePath);
                return false;
            }

            return true;
        }

        private void toolStripMenuItemCopyCreatureName_Click(object sender, EventArgs e)
        {
            CopySelectedCreatureName();
        }

        /// <summary>
        /// Copies the name of the currently selected creature to the clipboard.
        /// </summary>
        private void CopySelectedCreatureName()
        {
            if (listViewLibrary.SelectedIndices.Count > 0)
            {
                string name = _creaturesDisplayed[listViewLibrary.SelectedIndices[0]].name;
                if (string.IsNullOrEmpty(name))
                    Clipboard.Clear();
                else
                    Clipboard.SetText(name);
            }
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            GenerateCreatureNames();
        }

        /// <summary>
        /// Replaces the names of the selected creatures with a pattern generated name.
        /// </summary>
        private void GenerateCreatureNames()
        {
            if (listViewLibrary.SelectedIndices.Count == 0) return;

            var creaturesToUpdate = new List<Creature>();
            Creature[] sameSpecies = null;
            var libraryCreatureCount = _creatureCollection.GetTotalCreatureCount();

            foreach (int i in listViewLibrary.SelectedIndices)
            {
                var cr = _creaturesDisplayed[i];

                if (sameSpecies == null || sameSpecies[0].Species != cr.Species)
                    sameSpecies = _creatureCollection.creatures.Where(c => c.Species == cr.Species).ToArray();

                // set new name
                cr.name = NamePattern.GenerateCreatureName(cr, cr, sameSpecies,
                    _topLevels.ContainsKey(cr.Species) ? _topLevels[cr.Species] : null,
                    _lowestLevels.ContainsKey(cr.Species) ? _lowestLevels[cr.Species] : null,
                    _customReplacingNamingPattern, false, 0, libraryCreatureCount: libraryCreatureCount);

                creaturesToUpdate.Add(cr);
            }

            listViewLibrary.BeginUpdate();
            foreach (var cr in creaturesToUpdate)
                UpdateDisplayedCreatureValues(cr, false, false);

            listViewLibrary.EndUpdate();
        }

        private void fixColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count == 0
                || MessageBox.Show(
                    "This color fix will only result in the correct values if no mods are used that add colors to the game.\nA backup of the library file is recommended before this fix is applied.\n\nApply color fix?",
                    "Create a backup first", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) !=
                DialogResult.Yes) return;

            listViewLibrary.BeginUpdate();
            foreach (int i in listViewLibrary.SelectedIndices)
            {
                var cr = _creaturesDisplayed[i];

                for (int c = 0; c < 6; c++)
                    if (cr.colors[c] < 201)
                        cr.colors[c] = (byte)((cr.colors[c] - 1) % 56 + 1);
                UpdateDisplayedCreatureValues(cr, false, false);
            }

            listViewLibrary.EndUpdate();
        }

        private void openJsonDataFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(FileService.GetJsonPath());
            }
            catch (FileNotFoundException ex)
            {
                MessageBoxes.ExceptionMessageBox(ex, $"Folder not found\n{FileService.GetJsonPath()}",
                    "No data folder");
            }
        }

        /// <summary>
        /// Reloads the file for the custom replacings in the naming patterns.
        /// </summary>
        private void ReloadNamePatternCustomReplacings(PatternEditor pe = null)
        {
            string filePath = Properties.Settings.Default.CustomReplacingFilePath;
            if (string.IsNullOrEmpty(filePath))
                filePath = FileService.GetJsonPath(FileService.CustomReplacingsNamePattern);

            string errorMessage = null;
            if (!File.Exists(filePath) ||
                !FileService.LoadJsonFile(filePath, out _customReplacingNamingPattern, out errorMessage))
            {
                if (!string.IsNullOrEmpty(errorMessage))
                    MessageBoxes.ShowMessageBox(errorMessage, "Custom replacing file loading error");
            }
            else if (pe != null) pe.SetCustomReplacings(_customReplacingNamingPattern);
        }

        private void copyInfographicToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count != 0)
                _creaturesDisplayed[listViewLibrary.SelectedIndices[0]].ExportInfoGraphicToClipboard(_creatureCollection);
        }

        private void ToolStripMenuItemOpenWiki_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count != 0)
                ArkWiki.OpenPage(_creaturesDisplayed[listViewLibrary.SelectedIndices[0]]?.Species?.name);
        }

        private void libraryFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var libraryFilter = new LibraryFilter(_creatureCollection);
            Utils.SetWindowRectangle(libraryFilter, Properties.Settings.Default.LibraryFilterWindowRect);

            bool useFilterInTopStatsOld = Properties.Settings.Default.useFiltersInTopStatCalculation;
            if (libraryFilter.ShowDialog() == DialogResult.OK)
            {
                if (Properties.Settings.Default.useFiltersInTopStatCalculation
                    || Properties.Settings.Default.useFiltersInTopStatCalculation != useFilterInTopStatsOld)
                    CalculateTopStats(_creatureCollection.creatures);

                FilterLibRecalculate();
            }

            (Properties.Settings.Default.LibraryFilterWindowRect, _) = Utils.GetWindowRectangle(libraryFilter);
        }

        /// <summary>
        /// Verify if the right click was into the header of the list view, if so, open a specific contextMenu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStripLibrary_Opening(object sender, CancelEventArgs e)
        {
            if (Win32API.IsMouseOnListViewHeader(listViewLibrary.Handle, System.Windows.Forms.Control.MousePosition.Y))
            {
                e.Cancel = true;
                contextMenuStripLibraryHeader.Show(Control.MousePosition);
            }
        }

        private void extraDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayUpdateModules();
        }

        private async void DisplayUpdateModules(bool onlyShowDialogIfUpdatesAreAvailable = false, bool selectDefaultImagesIfNotYet = false, bool initializeImages = false)
        {
            var manifestFilePath = FileService.GetPath(FileService.ManifestFileName);
            if (!File.Exists(manifestFilePath)
                && !await Updater.Updater.DownloadManifest())
                return;

            using (var modules = new Updater.UpdateModules())
            {
                if (!modules.UpdateAvailable && !selectDefaultImagesIfNotYet && onlyShowDialogIfUpdatesAreAvailable)
                {
                    if (initializeImages) InitializeImages();
                    return;
                }

                if (selectDefaultImagesIfNotYet)
                    modules.SelectDefaultImages();

                modules.ShowDialog();
                if (modules.DialogResult != DialogResult.OK)
                    return;

                var result = await modules.DownloadRequestedModulesAsync();

                if (!string.IsNullOrEmpty(result))
                    MessageBox.Show(result, $"Data downloaded - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                if (modules.ImagesWereChanged)
                {
                    // clear outdated image cache
                    CreatureColored.CleanupCache(true);
                    InitializeImages();
                }

                void InitializeImages()
                {
                    Properties.Settings.Default.SpeciesImagesFolder = modules.GetSpeciesImagesFolder();
                    CreatureColored.InitializeSpeciesImageLocation();

                    if (Properties.Settings.Default.SpeciesImagesFolder != null)
                        speciesSelector1.InitializeSpeciesImages(Values.V.species);
                }
            }
        }

        /// <summary>
        /// If the user has downloaded the species images already but not in the new folder, move them.
        /// This method can probably be removed at 08-2021.
        /// </summary>
        private void MoveSpeciesImagesToNewFolder()
        {
            const string relativeImageFolder = "images/speciesImages";
            var oldImagesFolder = FileService.GetPath("img");
            var newImagesFolder = FileService.GetPath(relativeImageFolder);

            if (Directory.Exists(newImagesFolder))
            {
                // images are already moved
                // check if the images folder is set correctly (currently there's only one option)
                if (Properties.Settings.Default.SpeciesImagesFolder == relativeImageFolder) return;

                Properties.Settings.Default.SpeciesImagesFolder = relativeImageFolder;
                CreatureColored.InitializeSpeciesImageLocation();
                speciesSelector1.InitializeSpeciesImages(Values.V.species);
                return;
            }

            if (!Directory.Exists(oldImagesFolder)) return;

            try
            {
                Directory.Move(oldImagesFolder, newImagesFolder);

                Properties.Settings.Default.SpeciesImagesFolder = relativeImageFolder;
                CreatureColored.InitializeSpeciesImageLocation();
                speciesSelector1.InitializeSpeciesImages(Values.V.species);
            }
            catch
            {
                // ignore
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ControlKey
                || tabControlMain.TabPages[0].Tag != null) return;

            for (int i = 0; i < 10; i++)
            {
                var header = tabControlMain.TabPages[i].Text;
                tabControlMain.TabPages[i].Tag = header;
                tabControlMain.TabPages[i].Text = $"{(i == 9 ? 0 : i + 1)}: {header}";
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                if (tabControlMain.TabPages[0].Tag == null) return;
                for (int i = 0; i < 10; i++)
                {
                    if (tabControlMain.TabPages[i].Tag is string header)
                    {
                        tabControlMain.TabPages[i].Text = header;
                        tabControlMain.TabPages[i].Tag = null;
                    }
                }
                return;
            }

            if (!e.Control || e.Alt) return;

            int index;

            switch (e.KeyCode)
            {
                case Keys.D1: index = 0; break;
                case Keys.D2: index = 1; break;
                case Keys.D3: index = 2; break;
                case Keys.D4: index = 3; break;
                case Keys.D5: index = 4; break;
                case Keys.D6: index = 5; break;
                case Keys.D7: index = 6; break;
                case Keys.D8: index = 7; break;
                case Keys.D9: index = 8; break;
                case Keys.D0: index = 9; break;
                default: return;
            }

            if (index < tabControlMain.TabCount)
                tabControlMain.SelectedIndex = index;

            e.Handled = true;
        }

        private void addRandomCreaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Species selectedSpecies;
            using (var addRandomCreatureDialog = new AddDummyCreaturesSettings())
            {
                if (addRandomCreatureDialog.ShowDialog() != DialogResult.OK) return;

                var s = addRandomCreatureDialog.Settings;
                selectedSpecies = s.OnlySelectedSpecies ? speciesSelector1.SelectedSpecies : null;
                _creatureCollection.MergeCreatureList(DummyCreatures.CreateCreatures(s.CreatureCount,
                   selectedSpecies, s.SpeciesCount,
                    s.Generations, s.PairsPerGeneration, s.ProbabilityHigherStat, s.RandomMutationChance, s.MaxWildLevel,
                   s.SetOwner, s.SetTribe, s.SetServer));
            }

            _filterListAllowed = false;
            UpdateCreatureListings();
            _filterListAllowed = true;
            _libraryNeedsUpdate = true;
            pedigree1.PedigreeNeedsUpdate = true;
            creatureInfoInputExtractor.parentListValid = false;
            creatureInfoInputTester.parentListValid = false;

            SetCollectionChanged(true, selectedSpecies);
            if (tabControlMain.SelectedTab == tabPagePedigree)
                pedigree1.SetSpecies(selectedSpecies, true);
            else
                tabControlMain.SelectedTab = tabPageLibrary;
            listBoxSpeciesLib.SelectedIndex = 0;
        }

        private void resetSortingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Values.V.ResetDefaultSpeciesNameSorting();
            UpdateSpeciesLists(_creatureCollection.creatures);
        }

        private void resetSortingToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Values.V.ResetSpeciesNameSorting();
            UpdateSpeciesLists(_creatureCollection.creatures);
        }

        private void applyChangedSortingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Values.V.ApplySpeciesOrdering();
            UpdateSpeciesLists(_creatureCollection.creatures);
        }

        private void editSortingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Values.V.OpenSpeciesNameSortingFile();
        }

        private void helpAboutSpeciesSortingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RepositoryInfo.OpenWikiPage("Library#order-of-the-species-in-the-library");
        }

        private void colorDefinitionsToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // copy currently loaded color definitions to the clipboard
            Clipboard.SetText(string.Join("\n", Values.V.Colors.ColorsList.Select(c => $"{c.Id,3}: {c}")));
        }

        private void BtCopyLibraryColorToClipboard_Click(object sender, EventArgs e)
        {
            LibraryInfo.SetColorInfo(speciesSelector1.SelectedSpecies, CbLibraryInfoUseFilter.Checked ? (IList<Creature>)ApplyLibraryFilterSettings(_creatureCollection.creatures).ToArray() : _creatureCollection.creatures, CbLibraryInfoUseFilter.Checked);
            var colorInfo = LibraryInfo.GetSpeciesInfo();
            Clipboard.SetText(string.IsNullOrEmpty(colorInfo) ? $"no color info available for species {speciesSelector1.SelectedSpecies}" : colorInfo);
            SetMessageLabelText($"Color information about {speciesSelector1.SelectedSpecies} has been copied to the clipboard, you can paste it in a text editor to view it.", MessageBoxIcon.Information);
        }

        private void CbLibraryInfoUseFilter_CheckedChanged(object sender, EventArgs e)
        {
            LibraryInfo.SetColorInfo(speciesSelector1.SelectedSpecies, CbLibraryInfoUseFilter.Checked ? (IList<Creature>)ApplyLibraryFilterSettings(_creatureCollection.creatures).ToArray() : _creatureCollection.creatures, CbLibraryInfoUseFilter.Checked, tlpLibraryInfo);
        }

        private void discordServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(RepositoryInfo.DiscordServerInviteLink);
        }

        #region Server

        private void listenToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (listenToolStripMenuItem.Checked)
                AsbServerStartListening(false);
            else AsbServer.Connection.StopListening();
        }

        private void listenWithNewTokenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AsbServerStartListening(true);
        }

        private void AsbServerStartListening(bool newToken = false)
        {
            AsbServer.Connection.StopListening();
            var progressDataSent = new Progress<(string jsonText, string serverHash, string message)>(AsbServerDataSent);
            if (newToken || string.IsNullOrEmpty(Properties.Settings.Default.ExportServerToken))
                Properties.Settings.Default.ExportServerToken = AsbServer.Connection.CreateNewToken();
            Task.Factory.StartNew(() => AsbServer.Connection.StartListeningAsync(progressDataSent, Properties.Settings.Default.ExportServerToken));
            MessageServerListening(Properties.Settings.Default.ExportServerToken);
        }

        private void MessageServerListening(string token)
        {
            SetMessageLabelText($"Now listening to the export server using the token (also copied to clipboard){Environment.NewLine}{token}", MessageBoxIcon.Information, clipboardText: token);
            if (!string.IsNullOrEmpty(token))
                Clipboard.SetText(token);
        }

        private void sendExampleCreatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // debug function, sends a test creature to the server
            AsbServer.Connection.SendCreatureData(DummyCreatures.CreateCreature(speciesSelector1.SelectedSpecies), Properties.Settings.Default.ExportServerToken);
        }

        #endregion

    }
}
