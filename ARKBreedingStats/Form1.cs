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
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ARKBreedingStats.utils;
using static ARKBreedingStats.settings.Settings;

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
        private readonly List<StatIO> _statIOs = new List<StatIO>();
        private readonly List<StatIO> _testingIOs = new List<StatIO>();
        private int _activeStatIndex = -1;
        private readonly bool[] _activeStats = { true, true, true, true, true, true, true, true, true, true, true, true }; // stats used by the creature (some don't use oxygen)
        private bool _libraryNeedsUpdate;

        public delegate void CollectionChangedEventHandler(bool changed = true, Species species = null); // if null is passed for species, breeding-related controls are not updated
        public delegate void SetMessageLabelTextEventHandler(string text = null, MessageBoxIcon icon = MessageBoxIcon.None);

        private bool _updateTorporInTester;
        private bool _filterListAllowed;

        /// <summary>
        /// The stat indices that are considered for color highlighting and topness calculation.
        /// </summary>
        private readonly bool[] _considerStatHighlight = new bool[Values.STATS_COUNT];
        private bool _autoSave;
        private DateTime _lastAutoSaveBackup = DateTime.Now.AddDays(-1);
        private int _autoSaveMinutes;
        private Creature _creatureTesterEdit;
        private int _hiddenLevelsCreatureTester;
        private FileSync _fileSync;
        private FileWatcherExports _filewatcherExports;
        private readonly Extraction _extractor = new Extraction();
        private SpeechRecognition _speechRecognition;
        private readonly System.Windows.Forms.Timer _timerGlobal = new System.Windows.Forms.Timer();
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
            var args = Environment.GetCommandLineArgs();
            if (args.Contains("cleanupUpdater"))
                FileService.TryDeleteFile(Path.Combine(Path.GetTempPath(), Updater.UpdaterExe));

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
            initLocalization();
            InitializeComponent();

            columnHeaderTo.DisplayIndex = 16; // workaround for designer issue with displayIndices.

            // Create an instance of a ListView column sorter and assign it
            // to the ListView controls
            listViewLibrary.ListViewItemSorter = new ListViewColumnSorter();
            listViewPossibilities.ListViewItemSorter = new ListViewColumnSorter();
            timerList1.ColumnSorter = new ListViewColumnSorter();

            listViewLibrary.DoubleBuffered(true);

            toolStripStatusLabel.Text = Application.ProductVersion;

            // delegates
            pedigree1.EditCreature += EditCreatureInTester;
            pedigree1.BestBreedingPartners += ShowBestBreedingPartner;
            pedigree1.ExportToClipboard += ExportAsTextToClipboard;
            breedingPlan1.EditCreature += EditCreatureInTester;
            breedingPlan1.DisplayInPedigree += DisplayCreatureInPedigree;
            breedingPlan1.CreateIncubationTimer += CreateIncubationTimer;
            breedingPlan1.BestBreedingPartners += ShowBestBreedingPartner;
            breedingPlan1.ExportToClipboard += ExportAsTextToClipboard;
            breedingPlan1.SetMessageLabelText += SetMessageLabelText;
            breedingPlan1.SetGlobalSpecies += SetSpecies;
            timerList1.OnTimerChange += SetCollectionChanged;
            breedingPlan1.BindChildrenControlEvents();
            raisingControl1.onChange += SetCollectionChanged;
            tamingControl1.CreateTimer += CreateTimer;
            raisingControl1.extractBaby += ExtractBaby;
            raisingControl1.SetGlobalSpecies += SetSpecies;
            raisingControl1.timerControl = timerList1;
            notesControl1.changed += SetCollectionChanged;
            creatureInfoInputExtractor.CreatureDataRequested += CreatureInfoInput_CreatureDataRequested;
            creatureInfoInputTester.CreatureDataRequested += CreatureInfoInput_CreatureDataRequested;
            creatureInfoInputExtractor.ColorsChanged += CreatureInfoInputExtractor_ColorsChanged;
            creatureInfoInputTester.ColorsChanged += CreatureInfoInputExtractor_ColorsChanged;
            speciesSelector1.OnSpeciesSelected += SpeciesSelector1OnSpeciesSelected;
            statsMultiplierTesting1.OnApplyMultipliers += StatsMultiplierTesting1_OnApplyMultipliers;
            raisingControl1.AdjustTimers += timerList1.AdjustAllTimersByOffset;

            speciesSelector1.SetTextBox(tbSpeciesGlobal);

            ArkOCR.OCR.setOCRControl(ocrControl1);
            ocrControl1.UpdateWhiteThreshold += OcrUpdateWhiteThreshold;
            ocrControl1.DoOcr += DoOcr;

            openSettingsToolStripMenuItem.ShortcutKeyDisplayString = new KeysConverter().ConvertTo(Keys.Control, typeof(string))?.ToString().Replace("None", ",");

            _timerGlobal.Interval = 1000;
            _timerGlobal.Tick += TimerGlobal_Tick;

            ReloadNamePatternCustomReplacings();

            _reactOnCreatureSelectionChange = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            setLocalizations(false);

            // load window-position and size
            Utils.SetWindowRectangle(this, Properties.Settings.Default.MainWindowRect, Properties.Settings.Default.MainWindowMaximized);

            // Load column-widths, display-indices and sort-order of the TimerControlListView
            ListView lv = (ListView)timerList1.Controls["tableLayoutPanel1"].Controls["listViewTimer"];
            LoadListViewSettings(lv, "TCLVColumnWidths", "TCLVColumnDisplayIndices", "TCLVSortCol", "TCLVSortAsc");

            // Load column-widths, display-indices and sort-order  of the listViewLibrary
            LoadListViewSettings(listViewLibrary, "columnWidths", "libraryColumnDisplayIndices", "listViewSortCol", "listViewSortAsc");

            // load stat weights
            double[][] custWd = Properties.Settings.Default.customStatWeights;
            string[] custWs = Properties.Settings.Default.customStatWeightNames;
            Dictionary<string, double[]> custW = new Dictionary<string, double[]>();
            if (custWs != null && custWd != null)
            {
                for (int i = 0; i < custWs.Length && i < custWd.Length; i++)
                {
                    custW.Add(custWs[i], custWd[i]);
                }
            }
            breedingPlan1.statWeighting.CustomWeightings = custW;
            // last set values are saved at the end of the custom weightings
            if (custWs != null && custWd != null && custWd.Length > custWs.Length)
                breedingPlan1.statWeighting.WeightValues = custWd[custWs.Length];

            _autoSave = Properties.Settings.Default.autosave;
            _autoSaveMinutes = Properties.Settings.Default.autosaveMinutes;

            // load weapon damages
            tamingControl1.WeaponDamages = Properties.Settings.Default.weaponDamages;
            tamingControl1.WeaponDamagesEnabled = Properties.Settings.Default.weaponDamagesEnabled;

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                var statIO = new StatIO
                {
                    InputType = StatIOInputType.FinalValueInputType,
                    Title = Utils.StatName(s),
                    statIndex = s
                };
                var statIOTesting = new StatIO
                {
                    InputType = StatIOInputType.LevelsInputType,
                    Title = Utils.StatName(s),
                    statIndex = s
                };

                if (Utils.Precision(s) == 3)
                {
                    statIO.Percent = true;
                    statIOTesting.Percent = true;
                }

                statIOTesting.LevelChanged += testingStatIOValueUpdate;
                statIO.InputValueChanged += StatIOQuickWildLevelCheck;
                statIO.Click += new System.EventHandler(this.StatIO_Click);
                _considerStatHighlight[s] = (Properties.Settings.Default.consideredStats & (1 << s)) != 0;

                _statIOs.Add(statIO);
                _testingIOs.Add(statIOTesting);
            }
            // add controls in the order they are shown ingame
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                flowLayoutPanelStatIOsExtractor.Controls.Add(_statIOs[Values.statsDisplayOrder[s]]);
                flowLayoutPanelStatIOsExtractor.SetFlowBreak(_statIOs[Values.statsDisplayOrder[s]], true);
                flowLayoutPanelStatIOsTester.Controls.Add(_testingIOs[Values.statsDisplayOrder[s]]);
                flowLayoutPanelStatIOsTester.SetFlowBreak(_testingIOs[Values.statsDisplayOrder[s]], true);
                checkedListBoxConsiderStatTop.Items.Add(Utils.StatName(Values.statsDisplayOrder[s]), _considerStatHighlight[Values.statsDisplayOrder[s]]);
            }

            // torpor should not show bar, it get's too wide and is not interesting for breeding
            _statIOs[(int)StatNames.Torpidity].ShowBarAndLock = false;
            _testingIOs[(int)StatNames.Torpidity].ShowBarAndLock = false;
            // move sums and footnote to bottom
            flowLayoutPanelStatIOsExtractor.Controls.Add(panelSums);
            flowLayoutPanelStatIOsExtractor.Controls.Add(labelFootnote);
            flowLayoutPanelStatIOsTester.Controls.Add(panelStatTesterFootnote);

            // some stats are not used for any species, hide them permanently (until needed in a later release)
            _statIOs[(int)StatNames.Water].Hide();
            _statIOs[(int)StatNames.Temperature].Hide();
            _statIOs[(int)StatNames.TemperatureFortitude].Hide();
            _testingIOs[(int)StatNames.Water].Hide();
            _testingIOs[(int)StatNames.Temperature].Hide();
            _testingIOs[(int)StatNames.TemperatureFortitude].Hide();

            breedingPlan1.MutationLimit = Properties.Settings.Default.MutationLimitBreedingPlanner;

            // enable 0-lock for dom-levels of oxygen, food (most often they are not leveled up)
            _statIOs[(int)StatNames.Oxygen].DomLevelLockedZero = true;
            _statIOs[(int)StatNames.Food].DomLevelLockedZero = true;

            InitializeCollection();

            // Set up the file watcher
            _fileSync = new FileSync(_currentFileName, CollectionChanged);
            // exports file watcher
            bool enableExportWatcher = Utils.GetFirstImportExportFolder(out string exportFolderDefault)
                && Properties.Settings.Default.AutoImportExportedCreatures;
            _filewatcherExports = new FileWatcherExports(exportFolderDefault, ImportExportedAddIfPossible_WatcherThread, enableExportWatcher);

            if (!LoadStatAndKibbleValues(applySettings: false).statValuesLoaded || !Values.V.species.Any())
            {
                MessageBox.Show(Loc.S("valuesFileLoadingError"),
                        $"{Loc.S("error")}: Values-file not found - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            for (int s = 0; s < Values.STATS_COUNT; s++)
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

            // initialize speech recognition if enabled
            InitializeSpeechRecognition();

            // default owner and tribe
            creatureInfoInputExtractor.CreatureOwner = Properties.Settings.Default.DefaultOwnerName;
            creatureInfoInputExtractor.CreatureTribe = Properties.Settings.Default.DefaultTribeName;
            creatureInfoInputExtractor.CreatureServer = Properties.Settings.Default.DefaultServerName;
            creatureInfoInputExtractor.OwnerLock = Properties.Settings.Default.OwnerNameLocked;
            creatureInfoInputExtractor.TribeLock = Properties.Settings.Default.TribeNameLocked;

            // UI loaded

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
            }
            else
            {
                extractionTestControl1.LoadExtractionTestCases(Properties.Settings.Default.LastSaveFileTestCases);
            }

            // set TLS-protocol (github needs at least TLS 1.2) for update-check
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            // check for updates
            if (DateTime.Now.AddHours(-20) > Properties.Settings.Default.lastUpdateCheck)
                CheckForUpdates(true);

            if (!Properties.Settings.Default.AlreadyAskedToDownloadImageFilesTropeognathus)
            {
                Properties.Settings.Default.AlreadyAskedToDownloadImageFilesTropeognathus = true;
                if (!File.Exists(FileService.GetPath(FileService.ImageFolderName, "Tropeognathus.png"))
                    && MessageBox.Show("Download new species images to display the creature colors?\n\nThe file to be downloaded has a size of ~17 MB.\nYou can later download these images in the menu ? - Download Species Images",
                    "Download species images?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    DownloadSpeciesImagesAsync();
            }

            _filterListAllowed = true;
            // load last loaded file
            bool createNewCollection = string.IsNullOrEmpty(Properties.Settings.Default.LastSaveFile);
            if (!createNewCollection)
            {
                // if the last loaded file was already converted by someone else (e.g. if the library-file is shared),
                // ask if the converted version should be loaded instead.
                if (Path.GetExtension(Properties.Settings.Default.LastSaveFile).ToLower() == ".xml")
                {
                    string possibleConvertedCollectionPath = Path.Combine(Path.GetDirectoryName(Properties.Settings.Default.LastSaveFile), Path.GetFileNameWithoutExtension(Properties.Settings.Default.LastSaveFile) + CollectionFileExtension);
                    if (File.Exists(possibleConvertedCollectionPath)
                        && MessageBox.Show("The creature collection file seems to be already converted to the new file format.\n"
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
                && ExportFolderLocation.GetListOfExportFolders(out (string path, string steamPlayerName)[] arkInstallFolders, out _))
            {
                Properties.Settings.Default.ExportCreatureFolders = arkInstallFolders
                    .Select(f => $"default ({f.steamPlayerName})||{f.path}").ToArray();
            }

            if (createNewCollection)
                NewCollection();

            _updateExtractorVisualData = true;
            _timerGlobal.Start();
        }

        /// <summary>
        /// If the according property is set, the speechRecognition is initialized. Else it's disposed.
        /// </summary>
        private void InitializeSpeechRecognition()
        {
            bool speechRecognitionInitialized = false;
            if (Properties.Settings.Default.SpeechRecognition)
            {
                // var speechRecognitionAvailable = (AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.Substring(0, 13) == "System.Speech")); // TODO doesn't work as intended. Should only require System.Speech if available to allow running it on MONO

                _speechRecognition = new SpeechRecognition(_creatureCollection.maxWildLevel, _creatureCollection.considerWildLevelSteps ? _creatureCollection.wildLevelStep : 1, Values.V.speciesWithAliasesList, lbListening);
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
            if (speciesSelector1.SelectedSpecies != null && speciesSelector1.SelectedSpecies.taming != null &&
                    speciesSelector1.SelectedSpecies.taming.eats != null &&
                    speciesSelector1.SelectedSpecies.taming.eats.Any())
            {
                tamingControl1.SetLevel(level, false);
                tamingControl1.SetSpecies(speciesSelector1.SelectedSpecies);
                _overlay?.SetInfoText($"{speciesName} ({Loc.S("Level")} {level}):\n{tamingControl1.quickTamingInfos}");
            }
        }

        private void SpeechCommand(SpeechRecognition.Commands command)
        {
            // currently this command is not existing, accidental execution occured too often
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
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                speciesSelector1.SetSpeciesByName(tbSpeciesGlobal.Text);
            }
        }

        // global species changed / globalspecieschanged
        private void SpeciesSelector1OnSpeciesSelected(bool speciesChanged)
        {
            Species species = speciesSelector1.SelectedSpecies;
            ToggleViewSpeciesSelector(false);
            tbSpeciesGlobal.Text = species.name;
            if (!speciesChanged) return;
            _clearExtractionCreatureData = true; // as soon as the user changes the species, it's assumed it's not an exported creature anymore
            pbSpecies.Image = speciesSelector1.SpeciesImage();

            creatureInfoInputExtractor.SelectedSpecies = species;
            creatureInfoInputTester.SelectedSpecies = species;
            var statNames = species.statNames;

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                _activeStats[s] = Properties.Settings.Default.DisplayHiddenStats
                    ? species.UsesStat(s)
                    : species.DisplaysStat(s);


                _statIOs[s].IsActive = _activeStats[s];
                _testingIOs[s].IsActive = species.UsesStat(s);
                if (!_activeStats[s]) _statIOs[s].Input = 0;
                _statIOs[s].Title = Utils.StatName(s, false, statNames);
                _testingIOs[s].Title = Utils.StatName(s, false, statNames);
                // don't lock special stats of glowspecies
                if ((statNames != null &&
                     (s == (int)StatNames.Stamina
                      || s == (int)StatNames.Oxygen
                      || s == (int)StatNames.MeleeDamageMultiplier)
                  )
                  || (species.name.Contains("Daeodon")
                         && s == (int)StatNames.Food
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
                if (Values.V.loadedModsHash != Values.NoModsHash
                    && species.Mod == null
                    && Values.V.TryGetSpeciesByName(species.name, out var modSpecies)
                    && modSpecies.Mod != null
                    )
                {
                    SetMessageLabelText($"The selected species \"{species}\" is not from a mod, but there is a variant of that species that appears in the loaded mod \"{modSpecies.Mod.title}\". Probably you want to select the mod variant", MessageBoxIcon.Warning);
                }
            }
            else if (tabControlMain.SelectedTab == tabPageStatTesting)
            {
                UpdateAllTesterValues();
                statPotentials1.Species = species;
                statPotentials1.SetLevels(_testingIOs.Select(s => s.LevelWild).ToArray(), true);
                SetTesterInfoInputCreature();
            }
            else if (tabControlMain.SelectedTab == tabPageLibrary)
            {
                if (Properties.Settings.Default.ApplyGlobalSpeciesToLibrary)
                    listBoxSpeciesLib.SelectedItem = species;
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
            tamingControl1.SetTamingMultipliers(Values.V.currentServerMultipliers.TamingSpeedMultiplier,
                                                Values.V.currentServerMultipliers.DinoCharacterFoodDrainMultiplier);

            ColorModeColors.SetColors((ColorModeColors.AsbColorMode)Properties.Settings.Default.ColorMode);
            RecalculateAllCreaturesValues();

            breedingPlan1.UpdateBreedingData();
            raisingControl1.UpdateRaisingData();

            // apply level settings
            creatureBoxListView.CreatureCollection = _creatureCollection;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                _statIOs[s].barMaxLevel = _creatureCollection.maxChartLevel;
                _testingIOs[s].barMaxLevel = _creatureCollection.maxChartLevel;
            }
            breedingPlan1.MaxWildLevels = _creatureCollection.maxWildLevel;
            radarChart1.InitializeVariables(_creatureCollection.maxChartLevel);
            radarChartExtractor.InitializeVariables(_creatureCollection.maxChartLevel);
            radarChartLibrary.InitializeVariables(_creatureCollection.maxChartLevel);
            statPotentials1.levelDomMax = _creatureCollection.maxDomLevel;
            statPotentials1.levelGraphMax = _creatureCollection.maxChartLevel;

            _speechRecognition?.SetMaxLevelAndSpecies(_creatureCollection.maxWildLevel, _creatureCollection.considerWildLevelSteps ? _creatureCollection.wildLevelStep : 1, Values.V.speciesWithAliasesList);
            if (_overlay != null)
            {
                _overlay.InfoDuration = Properties.Settings.Default.OverlayInfoDuration;
                _overlay.checkInventoryStats = Properties.Settings.Default.inventoryCheckTimer;
            }

            ArkOCR.OCR.screenCaptureApplicationName = Properties.Settings.Default.OCRApp;

            if (Properties.Settings.Default.showOCRButton)
            {
                Loc.ControlText(btReadValuesFromArk, _tt);
            }
            else
            {
                btReadValuesFromArk.Text = "Import Exported Data";
                _tt.SetToolTip(btReadValuesFromArk, "Displays all exported creatures in the default-folder (needs to be set in the settings).");
            }
            ArkOCR.OCR.waitBeforeScreenCapture = Properties.Settings.Default.waitBeforeScreenCapture;
            ocrControl1.SetWhiteThreshold(Properties.Settings.Default.OCRWhiteThreshold);

            int maxImprintingPercentage = _creatureCollection.allowMoreThanHundredImprinting ? 100000 : 100;
            numericUpDownImprintingBonusExtractor.Maximum = maxImprintingPercentage;
            numericUpDownImprintingBonusTester.Maximum = maxImprintingPercentage;

            // sound-files
            timerList1.sounds = new[]
            {
                    File.Exists(Properties.Settings.Default.soundStarving) ? new System.Media.SoundPlayer(Properties.Settings.Default.soundStarving) : null,
                    File.Exists(Properties.Settings.Default.soundWakeup) ? new System.Media.SoundPlayer(Properties.Settings.Default.soundWakeup) : null,
                    File.Exists(Properties.Settings.Default.soundBirth) ? new System.Media.SoundPlayer(Properties.Settings.Default.soundBirth) : null,
                    File.Exists(Properties.Settings.Default.soundCustom) ? new System.Media.SoundPlayer(Properties.Settings.Default.soundCustom) : null
            };

            timerList1.TimerAlertsCSV = Properties.Settings.Default.playAlarmTimes;

            ClearAll();
            // update enabled stats
            for (int s = 0; s < Values.STATS_COUNT; s++)
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

            // import exported menu
            importExportedCreaturesToolStripMenuItem.DropDownItems.Clear();
            if (!(Properties.Settings.Default.ExportCreatureFolders?.Any() != true))
            {
                foreach (string f in Properties.Settings.Default.ExportCreatureFolders)
                {
                    ATImportExportedFolderLocation aTImportExportedFolderLocation = ATImportExportedFolderLocation.CreateFromString(f);
                    string menuItemHeader = string.IsNullOrEmpty(aTImportExportedFolderLocation.ConvenientName) ? "<unnamed>" : aTImportExportedFolderLocation.ConvenientName;
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(menuItemHeader
                        + (string.IsNullOrEmpty(aTImportExportedFolderLocation.OwnerSuffix) ? string.Empty : " - " + aTImportExportedFolderLocation.OwnerSuffix))
                    {
                        Tag = aTImportExportedFolderLocation
                    };
                    tsmi.Click += OpenImportExportForm;
                    importExportedCreaturesToolStripMenuItem.DropDownItems.Add(tsmi);
                }
                importExportedCreaturesToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            }
            // open folder for importExport
            ToolStripMenuItem tsmif = new ToolStripMenuItem("Open folder for importing exported files");
            tsmif.Click += ImportAllCreaturesInSelectedFolder;
            importExportedCreaturesToolStripMenuItem.DropDownItems.Add(tsmif);

            // savegame importer menu
            importingFromSavegameToolStripMenuItem.DropDownItems.Clear();
            if (Properties.Settings.Default.arkSavegamePaths?.Any() != true)
            {
                importingFromSavegameToolStripMenuItem.DropDownItems.Add(importingFromSavegameEmptyToolStripMenuItem);
            }
            else
            {
                foreach (string f in Properties.Settings.Default.arkSavegamePaths)
                {
                    ATImportFileLocation atImportFileLocation = ATImportFileLocation.CreateFromString(f);
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(atImportFileLocation.ConvenientName)
                    {
                        Tag = atImportFileLocation
                    };
                    tsmi.Click += SavegameImportClick;
                    importingFromSavegameToolStripMenuItem.DropDownItems.Add(tsmi);
                }
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
            breedingPlan1.breedingPlanNeedsUpdate = true;
            pedigree1.UpdateListView();
            raisingControl1.RecreateList();
        }

        /// <summary>
        /// This function should be called if the creatureCollection is changed, i.e. after loading a file or adding/removing a creature.
        /// It updates the listed species in the treeList and in the speciesSelector.
        /// </summary>
        private void UpdateSpeciesLists(List<Creature> creatures, bool keepCurrentlySelectedSpecies = true)
        {
            Species selectedSpeciesLibrary = keepCurrentlySelectedSpecies && listBoxSpeciesLib.SelectedItem is Species sp ? sp : null;

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
            availableSpecies = Values.V.species.Where(sn => availableSpecies.Contains(sn)).ToList();

            // add node to show all
            listBoxSpeciesLib.BeginUpdate();
            listBoxSpeciesLib.Items.Add(Loc.S("All"));
            foreach (Species s in availableSpecies)
                listBoxSpeciesLib.Items.Add(s);
            listBoxSpeciesLib.EndUpdate();

            if (selectedSpeciesLibrary != null)
                listBoxSpeciesLib.SelectedItem = selectedSpeciesLibrary;

            breedingPlan1.SetSpeciesList(availableSpecies, creatures);
            speciesSelector1.SetLibrarySpecies(availableSpecies);
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
            var ownerList = new List<string>();
            var tribesList = new List<string>();
            var serverList = new List<string>();

            //// check all creature for info
            var creaturesToCheck = _creatureCollection.creatures.Where(c => !c.flags.HasFlag(CreatureFlags.Placeholder)).ToArray();
            foreach (Creature c in creaturesToCheck)
            {
                AddIfNotContains(ownerList, c.owner);
                AddIfNotContains(tribesList, c.tribe);
                AddIfNotContains(serverList, c.server);

                void AddIfNotContains(List<string> list, string name)
                {
                    if (!string.IsNullOrEmpty(name) && !list.Contains(name))
                        list.Add(name);
                }
            }

            ownerList.Sort();
            tribesList.Sort();
            serverList.Sort();

            // owners
            foreach (var owner in ownerList)
            {
                if (!string.IsNullOrEmpty(owner) && !tribesControl1.PlayerExists(owner))
                    tribesControl1.AddPlayer(owner);
            }

            // tribes
            foreach (var tribe in tribesList)
            {
                if (!string.IsNullOrEmpty(tribe) && !tribesControl1.TribeExists(tribe))
                    tribesControl1.AddTribe(tribe);
            }

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

        private async void CheckForUpdates(bool silentCheck = false)
        {
            bool? updaterRunning = await Updater.CheckForPortableUpdate(silentCheck, _collectionDirty);
            if (!updaterRunning.HasValue) return; // error
            if (updaterRunning.Value)
            {
                // new version is available, user wants to update and the updater has just been started
                Close();
                return;
            }

            // download mod-manifest file to check for value updates
            if (!await LoadModsManifestAsync(Values.V, forceUpdate: true))
                return;

            // check if values-files can be updated
            //Values.V.CheckAndUpdateModFiles(Values.V.modsManifest.modsByFiles.Select(mikv => mikv.Value).Where(mi => mi.downloaded).Select(mi => mi.mod.FileName).ToList()); // mod-files are already checked when loaded
            bool valuesUpdated = CheckAvailabilityAndUpdateModFiles(new List<string> { FileService.ValuesJson }, Values.V);

            // update last successful update check
            Properties.Settings.Default.lastUpdateCheck = DateTime.Now;

            if (valuesUpdated)
            {
                var statsLoaded = LoadStatAndKibbleValues();
                if (statsLoaded.statValuesLoaded)
                {
                    MessageBox.Show(Loc.S("downloadingValuesSuccess"),
                            Loc.S("updateSuccessTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ApplySpeciesObjectsToCollection(_creatureCollection);
                }
                else
                {
                    MessageBox.Show("Download of new stat successful, but files couldn't be loaded.\nTry again later, or redownload the tool.",
                            $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (!silentCheck)
            {
                MessageBox.Show("You already have the newest version of both the program and values file.\n\n" +
                        "If your stats are outdated and no new version is available, we probably don\'t have the new ones either.",
                        "No new Version available", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Load stat- and kibble-values.
        /// </summary>
        private (bool statValuesLoaded, bool kibbleValuesLoaded) LoadStatAndKibbleValues(bool applySettings = true)
        {
            (bool statValuesLoaded, bool kibbleValuesLoaded) success = (false, false);
            if (LoadStatValues(Values.V))
            {
                if (applySettings)
                    ApplySettingsToValues();
                speciesSelector1.SetSpeciesLists(Values.V.species, Values.V.aliases);
                UpdateStatusBar();
                success.statValuesLoaded = true;
            }

            success.kibbleValuesLoaded = Kibbles.K.LoadValues();
            if (!success.kibbleValuesLoaded)
            {
                MessageBox.Show("The kibbles-file couldn't be loaded, the kibble-recipes will not be available. " +
                        "You can redownload this application to get this file.",
                        $"{Loc.S("error")}: Kibble-recipe-file not loaded - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            Creature creature = new Creature(speciesSelector1.SelectedSpecies, "", "", "", 0, GetCurrentWildLevels(fromExtractor), levelStep: _creatureCollection.getWildLevelStep())
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
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewCollection();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_collectionDirty && MessageBox.Show("Your Creature Collection has been modified since it was last saved, " +
                    "are you sure you want to discard your changes and quit without saving?",
                    "Discard Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                e.Cancel = true;
        }

        /// <summary>
        /// Save the properties of a listview: column width, column order and sorting.
        /// </summary>
        private void SaveListViewSettings(ListView lv, string widthName, string indicesName, string sortColName, string sortAscName)
        {
            if (lv == null) return;

            int[] cw = new int[lv.Columns.Count];
            int[] colIndices = new int[lv.Columns.Count];
            for (int c = 0; c < lv.Columns.Count; c++)
            {
                cw[c] = lv.Columns[c].Width;
                colIndices[c] = lv.Columns[c].DisplayIndex;
            }

            Properties.Settings.Default[widthName] = cw;
            Properties.Settings.Default[indicesName] = colIndices;

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
        /// <param name="lv"></param>
        /// <param name="widthName"></param>
        /// <param name="indicesName"></param>
        /// <param name="sortColName"></param>
        /// <param name="sortAscName"></param>
        private void LoadListViewSettings(ListView lv, string widthName, string indicesName, string sortColName, string sortAscName)
        {
            if (lv == null) return;

            // load column-widths
            if (Properties.Settings.Default[widthName] is int[] cw)
            {
                for (int c = 0; c < cw.Length && c < lv.Columns.Count; c++)
                    lv.Columns[c].Width = cw[c];
            }

            // load column display indices
            if (Properties.Settings.Default[indicesName] is int[] colIndices)
            {
                // indices have to be set increasingly, or they will "push" other values up
                var colIndicesOrdered = colIndices.Select((i, c) => (columnIndex: c, displayIndex: i))
                    .OrderBy(c => c.displayIndex).ToArray();
                for (int c = 0; c < colIndicesOrdered.Length && c < lv.Columns.Count; c++)
                    lv.Columns[colIndicesOrdered[c].columnIndex].DisplayIndex = colIndicesOrdered[c].displayIndex;
            }

            // load listviewLibSorting
            if (lv.ListViewItemSorter is ListViewColumnSorter lvcs)
            {
                lvcs.SortColumn = (int)Properties.Settings.Default[sortColName];
                lvcs.Order = (bool)Properties.Settings.Default[sortAscName] ? SortOrder.Ascending : SortOrder.Descending;
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // savesettings save settings

            // save window-position and size
            if (WindowState != FormWindowState.Minimized)
            {
                (Properties.Settings.Default.MainWindowRect, Properties.Settings.Default.MainWindowMaximized) = Utils.GetWindowRectangle(this);
            }

            // Save column-widths, display-indices and sort-order of the TimerControlListView
            ListView lv = (ListView)timerList1.Controls["tableLayoutPanel1"].Controls["listViewTimer"];
            SaveListViewSettings(lv, "TCLVColumnWidths", "TCLVColumnDisplayIndices", "TCLVSortCol", "TCLVSortAsc");

            // Save column-widths, display-indices and sort-order of the listViewLibrary
            SaveListViewSettings(listViewLibrary, "columnWidths", "libraryColumnDisplayIndices", "listViewSortCol", "listViewSortAsc");

            // save custom statweights
            List<string> custWs = new List<string>();
            List<double[]> custWd = new List<double[]>();
            foreach (KeyValuePair<string, double[]> w in breedingPlan1.statWeighting.CustomWeightings)
            {
                custWs.Add(w.Key);
                custWd.Add(w.Value);
            }
            custWd.Add(breedingPlan1.statWeighting.WeightValues); // add current values
            Properties.Settings.Default.customStatWeights = custWd.ToArray();
            Properties.Settings.Default.customStatWeightNames = custWs.ToArray();

            // save weapondamages for ko-calculation
            Properties.Settings.Default.weaponDamages = tamingControl1.WeaponDamages;
            Properties.Settings.Default.weaponDamagesEnabled = tamingControl1.WeaponDamagesEnabled;

            // save last selected species in combobox
            Properties.Settings.Default.lastSpecies = speciesSelector1.LastSpecies;

            // save onlyNonMutatedInBreedingPlanner
            Properties.Settings.Default.MutationLimitBreedingPlanner = breedingPlan1.MutationLimit;

            // save locked state of owner and tribe name
            Properties.Settings.Default.OwnerNameLocked = creatureInfoInputExtractor.OwnerLock;
            Properties.Settings.Default.TribeNameLocked = creatureInfoInputExtractor.TribeLock;

            // save splitter distance of speciesSelector
            Properties.Settings.Default.SpeciesSelectorVerticalSplitterDistance = speciesSelector1.SplitterDistance;
            Properties.Settings.Default.DisabledVariants = speciesSelector1.VariantSelector.DisabledVariants.ToArray();

            /////// save settings for next session
            Properties.Settings.Default.Save();

            // remove old cache-files
            CreatureColored.CleanupCache();

            _tt.Dispose();
            _timerGlobal.Dispose();
        }

        /// <summary>
        /// Sets the text at the top to display infos.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="icon"></param>
        private void SetMessageLabelText(string text = null, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            lbLibrarySelectionInfo.Text = text;
            switch (icon)
            {
                case MessageBoxIcon.Information:
                    lbLibrarySelectionInfo.BackColor = Color.LightGreen;
                    break;
                case MessageBoxIcon.Warning:
                    lbLibrarySelectionInfo.BackColor = Color.LightSalmon;
                    break;
                default:
                    lbLibrarySelectionInfo.BackColor = SystemColors.Control;
                    break;
            }
        }

        private void listBoxSpeciesLib_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetSpecies(listBoxSpeciesLib.SelectedItem as Species);
            FilterLibRecalculate();
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
            List<Creature>[] parents = FindPossibleParents(creature);
            creatureBoxListView.parentListSimilarity = FindParentSimilarities(parents, creature);
            creatureBoxListView.parentList = parents;
        }

        /// <summary>
        /// Returns lists of possible parents.
        /// </summary>
        private List<Creature>[] FindPossibleParents(Creature creature)
        {
            var fatherList = _creatureCollection.creatures
                    .Where(cr => cr.Species == creature.Species && cr.sex == Sex.Male && cr.guid != creature.guid && !cr.flags.HasFlag(CreatureFlags.Placeholder))
                    .OrderBy(cr => cr.name).ToList();
            var motherList = _creatureCollection.creatures
                    .Where(cr => cr.Species == creature.Species && cr.sex == Sex.Female && cr.guid != creature.guid && !cr.flags.HasFlag(CreatureFlags.Placeholder))
                    .OrderBy(cr => cr.name).ToList();

            // display new results
            return new[] { motherList, fatherList };
        }

        private List<int>[] FindParentSimilarities(List<Creature>[] parents, Creature creature)
        {
            // similarities (number of equal wildLevels as creature, to find parents easier)
            int e; // number of equal wildLevels
            List<int> motherListSimilarities = new List<int>();
            List<int> fatherListSimilarities = new List<int>();
            List<int>[] parentListSimilarities = new List<int>[2] { motherListSimilarities, fatherListSimilarities };

            if (parents.Length == 2 && parents[0] != null && parents[1] != null)
            {
                for (int ps = 0; ps < 2; ps++)
                {
                    foreach (Creature c in parents[ps])
                    {
                        e = 0;
                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            if (s != (int)StatNames.Torpidity && creature.levelsWild[s] >= 0 && creature.levelsWild[s] == c.levelsWild[s])
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
            }
            return parentListSimilarities;
        }

        //tabcontrolmainchange, maintabchange
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            timerList1.updateTimer = tabControlMain.SelectedTab == tabPageTimer;
            toolStripButtonCopy2Extractor.Visible = tabControlMain.SelectedTab == tabPageStatTesting;

            bool extrTab = tabControlMain.SelectedTab == tabPageExtractor;
            toolStripButtonCopy2Tester.Visible = extrTab;
            toolStripButtonDeleteTempCreature.Visible = extrTab;
            toolStripButtonSaveCreatureValuesTemp.Visible = extrTab;
            toolStripCBTempCreatures.Visible = extrTab;

            toolStripButtonAddPlayer.Visible = tabControlMain.SelectedTab == tabPagePlayerTribes;
            toolStripButtonAddTribe.Visible = tabControlMain.SelectedTab == tabPagePlayerTribes;
            toolStripButtonClear.Visible = tabControlMain.SelectedTab == tabPageExtractor || tabControlMain.SelectedTab == tabPageStatTesting;
            var libraryShown = tabControlMain.SelectedTab == tabPageLibrary;
            ToolStripLabelFilter.Visible = libraryShown;
            ToolStripTextBoxLibraryFilter.Visible = libraryShown;
            ToolStripButtonLibraryFilterClear.Visible = libraryShown;
            SetMessageLabelText();
            copyCreatureToolStripMenuItem.Visible = tabControlMain.SelectedTab == tabPageLibrary;
            toolStripButtonAddNote.Visible = tabControlMain.SelectedTab == tabPageNotes;
            toolStripButtonRemoveNote.Visible = tabControlMain.SelectedTab == tabPageNotes;
            raisingControl1.updateListView = tabControlMain.SelectedTab == tabPageRaising;
            toolStripButtonDeleteExpiredIncubationTimers.Visible = tabControlMain.SelectedTab == tabPageRaising || tabControlMain.SelectedTab == tabPageTimer;
            tsBtAddAsExtractionTest.Visible = Properties.Settings.Default.DevTools && tabControlMain.SelectedTab == tabPageStatTesting;
            copyToMultiplierTesterToolStripButton.Visible = Properties.Settings.Default.DevTools && (extrTab || tabControlMain.SelectedTab == tabPageStatTesting);

            if (tabControlMain.SelectedTab == tabPageStatTesting)
            {
                UpdateAllTesterValues();
                statPotentials1.Species = speciesSelector1.SelectedSpecies;
                statPotentials1.SetLevels(_testingIOs.Select(s => s.LevelWild).ToArray(), true);
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
            else if (tabControlMain.SelectedTab == tabPagePedigree)
            {
                if (pedigree1.PedigreeNeedsUpdate)
                {
                    Creature c = null;
                    if (listViewLibrary.SelectedItems.Count > 0)
                        c = (Creature)listViewLibrary.SelectedItems[0].Tag;
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

        private void NumericUpDownTestingTE_ValueChanged(object sender, EventArgs e)
        {
            UpdateAllTesterValues();
            lbWildLevelTester.Text =
                $"{Loc.S("preTameLevel")}: {Math.Ceiling(Math.Round((_testingIOs[(int)StatNames.Torpidity].LevelWild + 1) / (1 + NumericUpDownTestingTE.Value / 200), 6))}";
        }

        private void numericUpDownImprintingBonusTester_ValueChanged(object sender, EventArgs e)
        {
            UpdateAllTesterValues();
            // calculate number of imprintings
            if (speciesSelector1.SelectedSpecies.breeding != null && speciesSelector1.SelectedSpecies.breeding.maturationTimeAdjusted > 0)
                lbImprintedCount.Text = "(" + Math.Round((double)numericUpDownImprintingBonusTester.Value / (100 * Utils.ImprintingGainPerCuddle(speciesSelector1.SelectedSpecies.breeding.maturationTimeAdjusted, Values.V.currentServerMultipliers.BabyCuddleIntervalMultiplier)), 2) + "×)";
            else lbImprintedCount.Text = string.Empty;
        }

        private void numericUpDownImprintingBonusExtractor_ValueChanged(object sender, EventArgs e)
        {
            // calculate number of imprintings
            if (speciesSelector1.SelectedSpecies.breeding != null && speciesSelector1.SelectedSpecies.breeding.maturationTimeAdjusted > 0)
                lbImprintingCuddleCountExtractor.Text = "(" + Math.Round((double)numericUpDownImprintingBonusExtractor.Value / (100 * Utils.ImprintingGainPerCuddle(speciesSelector1.SelectedSpecies.breeding.maturationTimeAdjusted, Values.V.currentServerMultipliers.BabyCuddleIntervalMultiplier))) + "×)";
            else lbImprintingCuddleCountExtractor.Text = string.Empty;
        }

        private void checkBoxQuickWildCheck_CheckedChanged(object sender, EventArgs e)
        {
            UpdateQuickTamingInfo();
        }

        private void onlinehelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/cadon/ARKStatsExtractor/wiki/Manual");
        }

        private void breedingPlanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/cadon/ARKStatsExtractor/wiki/Breeding-Plan");
        }

        private void extractionIssuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/cadon/ARKStatsExtractor/wiki/Extraction-issues");
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
                        te = _extractor.UniqueTE();
                        imprinting = _extractor.ImprintingBonus;
                    }
                    else
                    {
                        input = creatureInfoInputTester;
                        bred = rbBredTester.Checked;
                        te = (double)NumericUpDownTestingTE.Value / 100;
                        imprinting = (double)numericUpDownImprintingBonusTester.Value / 100;
                    }

                    var levelStep = _creatureCollection.getWildLevelStep();
                    Creature creature = new Creature(species, input.CreatureName, input.CreatureOwner, input.CreatureTribe, input.CreatureSex, GetCurrentWildLevels(fromExtractor), GetCurrentDomLevels(fromExtractor), te, bred, imprinting, levelStep)
                    {
                        colors = input.RegionColors,
                        ArkId = input.ArkId
                    };
                    creature.RecalculateCreatureValues(levelStep);
                    ExportAsTextToClipboard(creature, breeding, ARKml);
                }
                else
                    MessageBox.Show(Loc.S("noValidExtractedCreatureToExport"), Loc.S("NoValidData"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (listViewLibrary.SelectedItems.Count > 0)
                ExportAsTextToClipboard((Creature)listViewLibrary.SelectedItems[0].Tag, breedingValues, ARKml);
            else
                MessageBox.Show(Loc.S("noCreatureSelectedInLibrary"), Loc.S("error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void importValuesFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteCreatureValuesFromClipboard();
        }

        private void pasteCreatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteCreatureFromClipboardToTester();
        }

        /// <summary>
        /// Export creature data to clipboard serialized in json-format.
        /// </summary>
        /// <param name="c"></param>
        private void CopyCreatureToClipboard(Creature c)
        {
            if (c != null)
            {
                string clpb = Newtonsoft.Json.JsonConvert.SerializeObject(c);
                if (!string.IsNullOrEmpty(clpb))
                    Clipboard.SetText(clpb);
            }
        }

        /// <summary>
        /// Import creature-data from the clipboard.
        /// </summary>
        private void PasteCreatureFromClipboardToTester()
        {
            string clpb = Clipboard.GetText();
            if (!string.IsNullOrEmpty(clpb))
            {
                Creature c;
                try
                {
                    c = Newtonsoft.Json.JsonConvert.DeserializeObject<Creature>(clpb);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Invalid Data in clipboard. Couldn\'t paste creature-data\nErrormessage:\n\n{e.Message}", $"{Loc.S("error")} - {Utils.ApplicationNameVersion}",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                UpdateParents(new List<Creature> { c });
                EditCreatureInTester(c, true);
            }
        }

        /// <summary>
        /// import creature values from plain text
        /// </summary>
        private void PasteCreatureValuesFromClipboard()
        {
            string clpb = Clipboard.GetText();
            if (clpb.Length > 0)
            {
                Regex r = new Regex(@"(.*?) \(([^,]+), Lvl (\d+), (?:wild|TE: ([\d.]+)%|Impr: ([\d.]+)%)?(?:, (Female|Male))?\): \w\w: ([\d.]+) \((\d+)(?:, (\d+))?\); \w\w: ([\d.]+) \((\d+)(?:, (\d+))?\); \w\w: ([\d.]+) \((\d+)(?:, (\d+))?\); \w\w: ([\d.]+) \((\d+)(?:, (\d+))?\); \w\w: ([\d.]+) \((\d+)(?:, (\d+))?\); \w\w: ([\d.]+)% \((\d+)(?:, (\d+))?\); \w\w: ([\d.]+)% \((\d+)(?:, (\d+))?\); \w\w: ([\d.]+) \((\d+)\);");
                Match m = r.Match(clpb);
                if (m.Success)
                {
                    Sex sex = Sex.Unknown;
                    switch (m.Groups[6].Value)
                    {
                        case "Female":
                            sex = Sex.Female;
                            break;
                        case "Male":
                            sex = Sex.Male;
                            break;
                    }
                    double[] sv = new double[Values.STATS_COUNT];
                    int[] wl = new int[Values.STATS_COUNT];
                    int[] dl = new int[Values.STATS_COUNT];
                    for (int s = 0; s < Values.STATS_COUNT; s++)
                    {
                        double.TryParse(m.Groups[7 + 3 * s].Value, out sv[s]); // TODO adjust to new stat-indices
                        int.TryParse(m.Groups[8 + 3 * s].Value, out wl[s]);
                        if (s != (int)StatNames.Torpidity)
                            int.TryParse(m.Groups[9 + 3 * s].Value, out dl[s]);
                        if (Utils.Precision(s) == 3)// percentage values
                            sv[s] *= 0.01;
                    }

                    int.TryParse(m.Groups[3].Value, out int totalLevel);
                    double.TryParse(m.Groups[4].Value, out double te);
                    te *= .01;
                    double.TryParse(m.Groups[5].Value, out double ib);
                    ib *= .01;

                    if (Values.V.TryGetSpeciesByName(m.Groups[2].Value, out Species species))
                    {
                        var cv = new CreatureValues(species, m.Groups[1].Value, string.Empty, string.Empty, sex, sv, totalLevel, te, te, te > 0 || ib > 0, ib > 0, ib, CreatureFlags.None, null, null)
                        {
                            levelsWild = wl,
                            levelsDom = dl
                        };
                        if (tabControlMain.SelectedTab == tabPageStatTesting)
                            SetCreatureValuesToTester(cv);
                        else
                            SetCreatureValuesToExtractor(cv);
                    }
                    else MessageBox.Show($"{Loc.S("unknownSpecies")}:\n" + m.Groups[2].Value, $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonRecalculateTops_Click(object sender, EventArgs e)
        {
            int consideredStats = 0;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                _considerStatHighlight[Values.statsDisplayOrder[s]] = checkedListBoxConsiderStatTop.GetItemChecked(s);

                // save consideredStats
                if (_considerStatHighlight[s])
                    consideredStats += 1 << s;
            }
            Properties.Settings.Default.consideredStats = consideredStats;

            // recalculate topstats
            CalculateTopStats(_creatureCollection.creatures);
            FilterLibRecalculate();
        }

        private void SetMatureBreedingStateOfSelectedCreatures(bool setMature = false, bool clearMatingCooldown = false, bool justMated = false)
        {
            listViewLibrary.BeginUpdate();
            foreach (ListViewItem i in listViewLibrary.SelectedItems)
            {
                Creature c = (Creature)i.Tag;
                if (setMature && c.growingUntil > DateTime.Now)
                    c.growingUntil = null;

                if (clearMatingCooldown && c.cooldownUntil > DateTime.Now)
                    c.cooldownUntil = null;

                if (justMated)
                    c.cooldownUntil = DateTime.Now.AddSeconds(c.Species.breeding?.matingCooldownMinAdjusted ?? 0);

                i.SubItems[11].Text =
                    DisplayedCreatureCountdown(c.cooldownUntil, c.growingUntil)?.ToString() ?? "-";

                CooldownColors(c, out Color foreColor, out Color backColor);
                i.SubItems[11].ForeColor = foreColor;
                i.SubItems[11].BackColor = backColor;
            }
            breedingPlan1.breedingPlanNeedsUpdate = true;
            listViewLibrary.EndUpdate();
        }

        private void setToMatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMatureBreedingStateOfSelectedCreatures(setMature: true);
        }

        private void clearMatingCooldownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMatureBreedingStateOfSelectedCreatures(clearMatingCooldown: true);
        }

        private void justMatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMatureBreedingStateOfSelectedCreatures(justMated: true);
        }

        private void aliveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStatusOfSelected(CreatureStatus.Available);
        }

        private void deadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStatusOfSelected(CreatureStatus.Dead);
        }

        private void unavailableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStatusOfSelected(CreatureStatus.Unavailable);
        }

        private void obeliskToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetStatusOfSelected(CreatureStatus.Obelisk);
        }

        private void SetStatusOfSelected(CreatureStatus s)
        {
            List<Creature> cs = new List<Creature>();
            foreach (ListViewItem i in listViewLibrary.SelectedItems)
                cs.Add((Creature)i.Tag);
            if (cs.Any())
                SetStatus(cs, s);
        }

        private void SetStatus(IEnumerable<Creature> cs, CreatureStatus s)
        {
            bool changed = false;
            List<string> speciesBlueprints = new List<string>();
            foreach (Creature c in cs)
            {
                if (c.Status != s)
                {
                    changed = true;
                    c.Status = s;
                    if (!speciesBlueprints.Contains(c.speciesBlueprint))
                        speciesBlueprints.Add(c.speciesBlueprint);
                }
            }
            if (changed)
            {
                // update list / recalculate topStats
                CalculateTopStats(_creatureCollection.creatures.Where(c => speciesBlueprints.Contains(c.speciesBlueprint)).ToList());
                FilterLibRecalculate();
                UpdateStatusBar();
                SetCollectionChanged(true, speciesBlueprints.Count == 1 ? Values.V.SpeciesByBlueprint(speciesBlueprints[0]) : null);
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
                    && MessageBox.Show("Selected Creature is currently not marked as \"Available\" and probably cannot be used for breeding right now. " +
                            "Do you want to change its status to \"Available\"?",
                            "Selected Creature not Available",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SetStatus(new List<Creature> { c }, CreatureStatus.Available);
                breedingPlan1.breedingPlanNeedsUpdate = false;
            }
            else
            {
                breedingPlan1.breedingPlanNeedsUpdate = true;
            }
            speciesSelector1.SetSpecies(c.Species);
            breedingPlan1.DetermineBestBreeding(c);
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
            using (Settings settingsfrm = new Settings(_creatureCollection, page))
            {
                bool libraryTopCreatureColorHighlight = Properties.Settings.Default.LibraryHighlightTopCreatures;
                if (settingsfrm.ShowDialog() == DialogResult.OK)
                {
                    ApplySettingsToValues();
                    if (settingsfrm.LanguageChanged) setLocalizations();
                    _autoSave = Properties.Settings.Default.autosave;
                    _autoSaveMinutes = Properties.Settings.Default.autosaveMinutes;
                    creatureBoxListView.CreatureCollection = _creatureCollection;
                    _fileSync.ChangeFile(_currentFileName); // only to trigger the update, filename is not changed

                    bool enableExportWatcher = Utils.GetFirstImportExportFolder(out string exportFolderDefault)
                        && Properties.Settings.Default.AutoImportExportedCreatures;
                    _filewatcherExports.SetWatchFolder(exportFolderDefault, enableExportWatcher);

                    InitializeSpeechRecognition();
                    _overlay?.SetInfoPositions();
                    if (Properties.Settings.Default.DevTools)
                        statsMultiplierTesting1.CheckIfMultipliersAreEqualToSettings();

                    if (libraryTopCreatureColorHighlight != Properties.Settings.Default.LibraryHighlightTopCreatures)
                        FilterLibRecalculate();

                    SetOverlayLocation();

                    SetCollectionChanged(true);
                }
                _settingsLastTabPage = settingsfrm.LastTabPageIndex;
            }
        }

        /// <summary>
        /// Display the wild-levels, assuming it's a wild creature. Used for quick checking
        /// </summary>
        /// <param name="sIo"></param>
        private void StatIOQuickWildLevelCheck(StatIO sIo)
        {
            _clearExtractionCreatureData = true; // as soon as the user changes stat-values, it's assumed it's not an exported creature anymore
            if (cbQuickWildCheck.Checked)
            {
                int lvlWild = (int)Math.Round((sIo.Input - speciesSelector1.SelectedSpecies.stats[sIo.statIndex].BaseValue) / (speciesSelector1.SelectedSpecies.stats[sIo.statIndex].BaseValue * speciesSelector1.SelectedSpecies.stats[sIo.statIndex].IncPerWildLevel));
                sIo.LevelWild = lvlWild < 0 ? 0 : lvlWild;
                sIo.LevelDom = 0;
                if (sIo.statIndex == (int)StatNames.Torpidity)
                {
                    SetQuickTamingInfo(_statIOs[(int)StatNames.Torpidity].LevelWild + 1);
                }
            }
        }

        // context-menu for library
        private void toolStripMenuItemEdit_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count > 0)
                EditCreatureInTester((Creature)listViewLibrary.Items[listViewLibrary.SelectedIndices[0]].Tag);
        }

        private void toolStripMenuItemRemove_Click(object sender, EventArgs e)
        {
            DeleteSelectedCreatures();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SetStatusOfSelected(CreatureStatus.Available);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SetStatusOfSelected(CreatureStatus.Unavailable);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            SetStatusOfSelected(CreatureStatus.Dead);
        }

        private void obeliskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStatusOfSelected(CreatureStatus.Obelisk);
        }

        private void cryopodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStatusOfSelected(CreatureStatus.Cryopod);
        }

        private void currentValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count > 0)
                SetCreatureValuesToExtractor((Creature)listViewLibrary.Items[listViewLibrary.SelectedIndices[0]].Tag);
        }

        private void wildValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count > 0)
                SetCreatureValuesToExtractor((Creature)listViewLibrary.Items[listViewLibrary.SelectedIndices[0]].Tag, true);
        }

        private void CreateTimer(string name, DateTime time, Creature c, string group)
        {
            timerList1.AddTimer(name, time, c, group);
        }

        /// <summary>
        /// Performs an optical character recognition on either the specified image or a screenshot of the game and extracts the stat-values.
        /// </summary>
        /// <param name="imageFilePath">If specified, this image is taken instead of a screenshot.</param>
        /// <param name="manuallyTriggered">If false, the method is called by a timer based event when the user looks at a creature-inventory.</param>
        public void DoOcr(string imageFilePath = "", bool manuallyTriggered = true)
        {
            cbQuickWildCheck.Checked = false;

            double[] OCRvalues = ArkOCR.OCR.DoOcr(out string debugText, out string dinoName, out string speciesName, out string ownerName, out string tribeName, out Sex sex, imageFilePath, manuallyTriggered);

            ocrControl1.output.Text = debugText;
            if (OCRvalues.Length <= 1)
            {
                if (manuallyTriggered) MessageBox.Show(debugText, $"OCR {Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            numericUpDownLevel.ValueSave = (decimal)OCRvalues[9];

            creatureInfoInputExtractor.CreatureName = dinoName;
            if (!creatureInfoInputExtractor.OwnerLock)
                creatureInfoInputExtractor.CreatureOwner = ownerName;
            if (!creatureInfoInputExtractor.TribeLock)
                creatureInfoInputExtractor.CreatureTribe = tribeName;
            creatureInfoInputExtractor.CreatureSex = sex;
            creatureInfoInputExtractor.RegionColors = new int[6];
            creatureInfoInputTester.SetArkId(0, false);

            int[] displayedStatIndices = new[]{
                (int)StatNames.Health,
                (int)StatNames.Stamina,
                (int)StatNames.Oxygen,
                (int)StatNames.Food,
                (int)StatNames.Weight,
                (int)StatNames.MeleeDamageMultiplier,
                (int)StatNames.SpeedMultiplier,
                (int)StatNames.Torpidity
            };

            for (int i = 0; i < displayedStatIndices.Length; i++)
            {
                _statIOs[displayedStatIndices[i]].Input = _statIOs[displayedStatIndices[i]].percent
                    ? OCRvalues[i] / 100.0
                    : OCRvalues[i];
            }

            // use imprinting if existing
            if (OCRvalues.Length > 8 && OCRvalues[8] >= 0 && (OCRvalues[8] <= 100 || _creatureCollection.allowMoreThanHundredImprinting))
            {
                rbBredExtractor.Checked = true;
                if (!Properties.Settings.Default.OCRIgnoresImprintValue)
                    numericUpDownImprintingBonusExtractor.ValueSave = (decimal)OCRvalues[8];
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
                double[] statValues = new double[Values.STATS_COUNT];
                for (int s = 0; s < displayedStatIndices.Length; s++)
                {
                    statValues[displayedStatIndices[s]] = OCRvalues[s];
                }

                List<Species> possibleSpecies = DetermineSpeciesFromStats(statValues, speciesName);

                if (possibleSpecies.Count == 1)
                {
                    if (possibleSpecies[0] != null)
                        speciesSelector1.SetSpecies(possibleSpecies[0]);
                    ExtractLevels(true, showLevelsInOverlay: !manuallyTriggered); // only one possible dino, use that one
                }
                else
                {
                    bool sameValues = true;

                    if (_lastOcrValues != null)
                        for (int i = 0; i < 10; i++)
                            if (OCRvalues[i] != _lastOcrValues[i])
                            {
                                sameValues = false;
                                break;
                            }

                    // if there's more than one option, on manual we cycle through the options if we're trying multiple times
                    // on automated, we take the first one that yields an error-free level extraction
                    if (manuallyTriggered && sameValues)
                    {
                        int newindex = (possibleSpecies.IndexOf(_lastOcrSpecies) + 1) % possibleSpecies.Count;
                        speciesSelector1.SetSpecies(possibleSpecies[newindex]);
                        _lastOcrSpecies = possibleSpecies[newindex];
                        _lastOcrValues = OCRvalues;
                        ExtractLevels(true);
                    }
                    else
                    {
                        // automated, or first manual attempt at new values
                        bool foundPossiblyGood = false;
                        for (int dinooption = 0; dinooption < possibleSpecies.Count() && foundPossiblyGood == false; dinooption++)
                        {
                            // if the last OCR'ed values are the same as this one, the user may not be happy with the dino species selection and want another one
                            // so we'll cycle to the next one, but only if the OCR is manually triggered, on autotrigger (ie, overlay), don't change
                            speciesSelector1.SetSpecies(possibleSpecies[dinooption]);
                            _lastOcrSpecies = possibleSpecies[dinooption];
                            _lastOcrValues = OCRvalues;
                            foundPossiblyGood = ExtractLevels(showLevelsInOverlay: !manuallyTriggered);
                        }
                    }
                }
            }
            else
            {
                if (speciesByName != null
                    && (speciesSelector1.SelectedSpecies == null
                       || speciesByName.name != speciesSelector1.SelectedSpecies.name)) // don't change already selected variant of a species
                {
                    speciesSelector1.SetSpecies(speciesByName);
                }
                ExtractLevels();
            }

            _lastOcrValues = OCRvalues;
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

            // if dice-coefficient is promising, just take that
            var scores = Values.V.species.Where(sp => sp.IsDomesticable).Select(sp => new { Score = DiceCoefficient.diceCoefficient(sp.name.Replace(" ", ""), speciesName.Replace(" ", "")), Species = sp }).OrderByDescending(o => o.Score).ToArray();
            const double minimumScore = 0.5;
            if (scores.First().Score > minimumScore)
            {
                possibleSpecies.AddRange(scores.Where(s => s.Score > minimumScore).Select(s => s.Species)
                    .Where(sp => !(stats[(int)StatNames.Oxygen] != 0 ^ sp.DisplaysStat((int)StatNames.Oxygen)))
                    );
                return possibleSpecies;
            }

            if (stats.Length > Values.STATS_COUNT && stats[Values.STATS_COUNT] > 0)
            {
                // creature is imprinted, the following algorithm cannot handle this yet. use current selected species
                possibleSpecies.Add(speciesSelector1.SelectedSpecies);
                return possibleSpecies;
            }

            foreach (var species in Values.V.species.Where(sp => sp.IsDomesticable))
            {
                if (species == speciesSelector1.SelectedSpecies) continue; // the currently selected species is ignored here and set as top priority at the end

                // if value for oxygen is given but current species doesn't display it, skip
                if (stats[(int)StatNames.Oxygen] != 0 ^ species.DisplaysStat((int)StatNames.Oxygen))
                    continue;

                bool possible = true;
                // check that all stats are possible (no negative levels)
                double baseValue;
                double incWild;
                double possibleLevel;
                for (int s = Values.STATS_COUNT - 1; s >= 0; s--)
                {
                    baseValue = species.stats[s].BaseValue;
                    incWild = species.stats[s].IncPerWildLevel;
                    if (incWild > 0)
                    {
                        //possibleLevel = ((statIOs[s].Input - species.stats[s].AddWhenTamed) - baseValue) / (baseValue * incWild); // this fails if creature is wild
                        possibleLevel = (_statIOs[s].Input - baseValue) / (baseValue * incWild);

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
                baseValue = species.stats[(int)StatNames.Torpidity].BaseValue;
                incWild = species.stats[(int)StatNames.Torpidity].IncPerWildLevel;

                possibleLevel = (_statIOs[(int)StatNames.Torpidity].Input - species.stats[(int)StatNames.Torpidity].AddWhenTamed - baseValue) / (baseValue * incWild);
                double possibleLevelWild = (_statIOs[(int)StatNames.Torpidity].Input - baseValue) / (baseValue * incWild);

                if (possibleLevelWild < 0 || Math.Round(possibleLevel, 3) > (double)numericUpDownLevel.Value - 1 || Math.Round(possibleLevel, 3) % 1 > 0.001 && Math.Round(possibleLevelWild, 3) % 1 > 0.001)
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
                baseValue = species.stats[(int)StatNames.Oxygen].BaseValue;
                incWild = species.stats[(int)StatNames.Oxygen].IncPerWildLevel;
                possibleLevel = (_statIOs[(int)StatNames.Oxygen].Input - species.stats[(int)StatNames.Oxygen].AddWhenTamed - baseValue) / (baseValue * incWild);

                if (possibleLevel < 0 || possibleLevel > (double)numericUpDownLevel.Value - 1)
                    continue;

                if (Math.Round(possibleLevel, 3) != (int)possibleLevel || possibleLevel > (double)numericUpDownLevel.Value / 2)
                    likely = false;

                if (_statIOs[(int)StatNames.Oxygen].Input != 0 && baseValue == 0)
                    likely = false; // having an oxygen value for non-oxygen dino is a disqualifier

                if (likely)
                    possibleSpecies.Insert(0, species); // insert species at top
                else
                    possibleSpecies.Add(species);
            }

            if (speciesSelector1.SelectedSpecies != null)
                possibleSpecies.Insert(0, speciesSelector1.SelectedSpecies); // adding the currently selected creature in the combobox as first priority. the user might already have that selected
            return possibleSpecies;
        }

        private void chkbToggleOverlay_CheckedChanged(object sender, EventArgs e)
        {
            if (_overlay == null)
            {
                _overlay = new ARKOverlay
                {
                    ExtractorForm = this,
                    InfoDuration = Properties.Settings.Default.OverlayInfoDuration,
                    checkInventoryStats = Properties.Settings.Default.inventoryCheckTimer
                };
                _overlay.InitLabelPositions();
            }

            if (!SetOverlayLocation()) return;

            _overlay.Visible = cbToggleOverlay.Checked;
            _overlay.EnableOverlayTimer = cbToggleOverlay.Checked;

            // disable speechRecognition if overlay is disabled. (no use if no data can be displayed)
            if (_speechRecognition != null && !cbToggleOverlay.Checked)
                _speechRecognition.Listen = false;
        }

        /// <summary>
        /// Sets the overlay location to the game location or the custom location.
        /// If the automatic location could not be found it disables the overlay and returns false.
        /// </summary>
        /// <returns></returns>
        private bool SetOverlayLocation()
        {
            if (cbToggleOverlay.Checked)
            {
                if (Properties.Settings.Default.UseCustomOverlayLocation)
                {
                    _overlay.Location = Properties.Settings.Default.CustomOverlayLocation;
                }
                else
                {
                    Process[] p = Process.GetProcessesByName(Properties.Settings.Default.OCRApp);

                    if (!p.Any())
                    {
                        MessageBox.Show("Process for capturing screenshots and for overlay (e.g. the game, or a stream of the game) not found.\n" +
                                "Start the game or change the process in the settings.", $"Game started? - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cbToggleOverlay.Checked = false;
                        return false;
                    }
                    IntPtr mwhd = p[0].MainWindowHandle;
                    Screen scr = Screen.FromHandle(mwhd);
                    _overlay.Location = scr.WorkingArea.Location;
                }
            }
            return true;
        }

        private void toolStripButtonCopy2Tester_Click(object sender, EventArgs e)
        {
            double te = _extractor.UniqueTE();
            NumericUpDownTestingTE.ValueSave = (decimal)(te >= 0 ? te * 100 : 80);
            numericUpDownImprintingBonusTester.Value = numericUpDownImprintingBonusExtractor.Value;
            if (rbBredExtractor.Checked)
                rbBredTester.Checked = true;
            else if (rbTamedExtractor.Checked)
                rbTamedTester.Checked = true;
            else
                rbWildTester.Checked = true;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                _testingIOs[s].LevelWild = _statIOs[s].LevelWild;
                _testingIOs[s].LevelDom = _statIOs[s].LevelDom;
                testingStatIOValueUpdate(_testingIOs[s]);
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
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    _testingIOs[s].LevelDom = 0;
                    _testingIOs[s].LevelWild = 0;
                }
                creatureInfoInputTester.Clear();
            }
        }

        private void toolStripButtonCopy2Extractor_Click(object sender, EventArgs e)
        {
            ClearAll();
            // copy values from tester over to extractor
            for (int s = 0; s < Values.STATS_COUNT; s++)
                _statIOs[s].Input = _testingIOs[s].Input;
            if (rbBredTester.Checked)
                rbBredExtractor.Checked = true;
            else if (rbTamedTester.Checked)
                rbTamedExtractor.Checked = true;
            else
                rbWildExtractor.Checked = true;
            numericUpDownImprintingBonusExtractor.Value = numericUpDownImprintingBonusTester.Value;
            // set total level
            numericUpDownLevel.Value = _testingIOs[(int)StatNames.Torpidity].LevelWild + GetCurrentDomLevels(false).Sum() + 1;

            creatureInfoInputExtractor.CreatureSex = creatureInfoInputTester.CreatureSex;
            creatureInfoInputExtractor.RegionColors = creatureInfoInputTester.RegionColors;

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
            if (_overlay != null && _overlay.checkInventoryStats)
            {
                var wildLevels = GetCurrentWildLevels();
                var tamedLevels = GetCurrentDomLevels();
                Color[] colors = new Color[Values.STATS_COUNT];


                for (int i = 0; i < Values.STATS_COUNT; i++)
                {
                    wildLevels[i] = wildLevels[i] > 0 ? wildLevels[i] : 0;
                    tamedLevels[i] = tamedLevels[i] > 0 ? tamedLevels[i] : 0;
                    colors[i] = _statIOs[i].BackColor;
                }
                int levelWild = wildLevels[(int)StatNames.Torpidity] + 1;
                int levelDom = tamedLevels.Sum();

                string extraText = speciesSelector1.SelectedSpecies.name;
                if (!_extractor.PostTamed)
                {
                    string foodName = speciesSelector1.SelectedSpecies.taming.eats[0];
                    int foodNeeded = Taming.FoodAmountNeeded(speciesSelector1.SelectedSpecies, levelWild, Values.V.currentServerMultipliers.TamingSpeedMultiplier, foodName, speciesSelector1.SelectedSpecies.taming.nonViolent);
                    Taming.TamingTimes(speciesSelector1.SelectedSpecies, levelWild, Values.V.currentServerMultipliers.TamingSpeedMultiplier, Values.V.currentServerMultipliers.DinoCharacterFoodDrainMultiplier, foodName, foodNeeded, out _, out TimeSpan duration, out int narcoBerries, out int ascerbicMushrooms, out int narcotics, out int bioToxines, out double te, out _, out int bonusLevel, out _);
                    string foodNameDisplay = foodName == "Kibble" ? speciesSelector1.SelectedSpecies.taming.favoriteKibble + " Egg Kibble" : foodName;
                    extraText += "\nTaming takes " + duration.ToString(@"hh\:mm\:ss") + " with " + foodNeeded + "×" + foodNameDisplay
                            + "\n" + narcoBerries + " Narcoberries or " + ascerbicMushrooms + " Ascerbic Mushrooms or " + narcotics + " Narcotics or " + bioToxines + " Bio Toxines are needed"
                            + "\nTaming Effectiveness: " + Math.Round(100 * te, 1) + " % (+" + bonusLevel + " lvl)";
                }

                _overlay.SetStatLevels(wildLevels, tamedLevels, levelWild, levelDom, colors);
                _overlay.SetInfoText(extraText);
            }
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
                DoOcr();
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
                tamingControl1.SetLevel(_statIOs[(int)StatNames.Torpidity].LevelWild + 1);
            else
                tamingControl1.SetLevel((int)numericUpDownLevel.Value);
            tabControlMain.SelectedTab = tabPageTaming;
        }

        private void labelImprintedCount_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // set imprinting-count to closes integer
                if (speciesSelector1.SelectedSpecies.breeding != null && speciesSelector1.SelectedSpecies.breeding.maturationTimeAdjusted > 0)
                {
                    double imprintingGainPerCuddle = Utils.ImprintingGainPerCuddle(speciesSelector1.SelectedSpecies.breeding.maturationTimeAdjusted, Values.V.currentServerMultipliers.BabyCuddleIntervalMultiplier);
                    int cuddleCount = (int)Math.Round((double)numericUpDownImprintingBonusTester.Value / (100 * imprintingGainPerCuddle));
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
                double imprintingFactorTorpor = speciesSelector1.SelectedSpecies.StatImprintMultipliers[(int)StatNames.Torpidity] * _creatureCollection.serverMultipliers.BabyImprintingStatScaleMultiplier;
                // set imprinting value so the set levels in the tester yield the value in the extractor
                double imprintingBonus = imprintingFactorTorpor != 0
                                         ? (_statIOs[(int)StatNames.Torpidity].Input / StatValueCalculation.CalculateValue(speciesSelector1.SelectedSpecies, (int)StatNames.Torpidity, _testingIOs[(int)StatNames.Torpidity].LevelWild, 0, true, 1, 0) - 1) / imprintingFactorTorpor
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
            if (cc.modIDs == null) cc.modIDs = new List<string>();
            cc.modIDs = cc.modIDs.Distinct().ToList();

            List<string> filePaths = new List<string>();

            var unknownModIDs = new List<string>();

            // determine file-names of mod-value files
            foreach (var modId in cc.modIDs)
            {
                if (Values.V.modsManifest.modsByID.ContainsKey(modId)
                    && !string.IsNullOrEmpty(Values.V.modsManifest.modsByID[modId].mod?.FileName))
                    filePaths.Add(Values.V.modsManifest.modsByID[modId].mod.FileName);
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
            return result;
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

            // if the mods for the library changed,
            // first check if all mod value files are available and load missing files if possible,
            // then reload all values and modvalues
            if (_creatureCollection.ModValueReloadNeeded
                && LoadModValuesOfCollection(_creatureCollection, true, true))
                SetCollectionChanged(true);
        }

        private void toolStripButtonAddPlayer_Click(object sender, EventArgs e)
        {
            tribesControl1.AddPlayer();
        }

        private void UpdateStatusBar()
        {
            var creatureCount = _creatureCollection.creatures.Where(c => !c.flags.HasFlag(CreatureFlags.Placeholder)).ToArray();
            int total = creatureCount.Length;
            int obelisk = creatureCount.Count(c => c.Status == CreatureStatus.Obelisk);
            int cryopod = creatureCount.Count(c => c.Status == CreatureStatus.Cryopod);

            bool modsLoaded = _creatureCollection.ModList?.Any() ?? false;

            toolStripStatusLabel.Text = total + " creatures in Library"
                + (total > 0 ? " ("
                + "available: " + creatureCount.Count(c => c.Status == CreatureStatus.Available)
                + ", unavailable: " + creatureCount.Count(c => c.Status == CreatureStatus.Unavailable)
                + ", dead: " + creatureCount.Count(c => c.Status == CreatureStatus.Dead)
                + (obelisk > 0 ? ", obelisk: " + obelisk : string.Empty)
                + (cryopod > 0 ? ", cryopod: " + cryopod : string.Empty)
                + ")" : string.Empty)
                + ". v" + Application.ProductVersion
                //+ "-BETA" // TODO BETA indicator
                + " / values: " + Values.V.Version +
                    (modsLoaded ? ", additional values from " + _creatureCollection.ModList.Count + " mods (" + string.Join(", ", _creatureCollection.ModList.Select(m => m.title).ToArray()) + ")" : string.Empty);
        }

        private void toolStripButtonAddNote_Click(object sender, EventArgs e)
        {
            notesControl1.AddNote();
        }

        private void toolStripButtonRemoveNote_Click(object sender, EventArgs e)
        {
            notesControl1.RemoveSelectedNote();
        }

        private void labelListening_Click(object sender, EventArgs e)
        {
            _speechRecognition?.ToggleListening();
        }

        private void CreateIncubationTimer(Creature mother, Creature father, TimeSpan incubationDuration, bool incubationStarted)
        {
            raisingControl1.AddIncubationTimer(mother, father, incubationDuration, incubationStarted);
            _libraryNeedsUpdate = true; // because mating-cooldown of mother was set
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

            tamingControl1.SetTamingMultipliers(Values.V.currentServerMultipliers.TamingSpeedMultiplier,
                                                Values.V.currentServerMultipliers.DinoCharacterFoodDrainMultiplier);
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

        private static void OcrUpdateWhiteThreshold(int value)
        {
            Properties.Settings.Default.OCRWhiteThreshold = value;
            ArkOCR.OCR.whiteThreshold = value;
        }

        private void toolStripCBTempCreatures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripCBTempCreatures.SelectedIndex >= 0 && toolStripCBTempCreatures.SelectedIndex < _creatureCollection.creaturesValues.Count)
            {
                SetCreatureValuesToExtractor(_creatureCollection.creaturesValues[toolStripCBTempCreatures.SelectedIndex]);
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
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                _testingIOs[s].LevelWild = cv.levelsWild[s];
                _testingIOs[s].LevelDom = cv.levelsDom[s];
            }
            SetCreatureValuesToInfoInput(cv, creatureInfoInputTester);

            NumericUpDownTestingTE.ValueSave = (decimal)cv.tamingEffMin * 100;

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
            input.CreatureServer = cv.server;
            input.CreatureSex = cv.sex;
            input.CreatureGuid = cv.guid;
            input.CreatureFlags = cv.flags;
            input.Mother = cv.Mother;
            input.Father = cv.Father;
            input.RegionColors = cv.colorIDs;
            input.SetArkId(cv.ARKID, cv.guid == Utils.ConvertArkIdToGuid(cv.ARKID));
            input.MutationCounterMother = cv.mutationCounterMother;
            input.MutationCounterFather = cv.mutationCounterFather;
            input.GrowingUntil = cv.growingUntil;
            input.CooldownUntil = cv.cooldownUntil;
            input.MotherArkId = cv.motherArkId;
            input.FatherArkId = cv.fatherArkId;
            input.CreatureNote = string.Empty;
            input.CreatureStatus = CreatureStatus.Available;
            input.SetTimersToChanged();
        }

        private void toolStripButtonSaveCreatureValuesTemp_Click(object sender, EventArgs e)
        {
            _creatureCollection.creaturesValues.Add(GetCreatureValuesFromExtractor());
            SetCollectionChanged(true);
            UpdateTempCreatureDropDown();
        }

        /// <summary>
        /// Returns the entered creature values from the extractor.
        /// </summary>
        private CreatureValues GetCreatureValuesFromExtractor()
        {
            CreatureValues cv = new CreatureValues();
            for (int s = 0; s < Values.STATS_COUNT; s++)
                cv.statValues[s] = _statIOs[s].Input;
            cv.speciesName = speciesSelector1.SelectedSpecies.name;
            cv.speciesBlueprint = speciesSelector1.SelectedSpecies.blueprintPath;
            cv.name = creatureInfoInputExtractor.CreatureName;
            cv.owner = creatureInfoInputExtractor.CreatureOwner;
            cv.tribe = creatureInfoInputExtractor.CreatureTribe;
            cv.server = creatureInfoInputExtractor.CreatureServer;
            cv.sex = creatureInfoInputExtractor.CreatureSex;
            cv.flags = creatureInfoInputExtractor.CreatureFlags;
            cv.Mother = creatureInfoInputExtractor.Mother;
            cv.Father = creatureInfoInputExtractor.Father;
            cv.colorIDs = creatureInfoInputExtractor.RegionColors;

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
                    && MessageBox.Show("Remove the data of this cached creature?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
                toolStripCBTempCreatures.Items.Add($"{cv.name} ({cv.Species?.name ?? "unknown species"})");
        }

        /// <summary>
        /// Collects the data needed for the name pattern editor.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="openPatternEditor"></param>
        private void CreatureInfoInput_CreatureDataRequested(CreatureInfoInput input, bool openPatternEditor, bool updateInheritance, bool showDuplicateNameWarning, int namingPatternIndex)
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
                cr.imprintingBonus = _extractor.ImprintingBonus;
                cr.tamingEff = _extractor.UniqueTE();
                cr.isBred = rbBredExtractor.Checked;
                cr.topBreedingStats = _statIOs.Select(s => s.TopLevel == StatIOStatus.TopLevel || s.TopLevel == StatIOStatus.NewTopLevel).ToArray();
            }
            else
            {
                cr.levelsWild = _testingIOs.Select(s => s.LevelWild).ToArray();
                cr.imprintingBonus = (double)numericUpDownImprintingBonusTester.Value / 100;
                cr.tamingEff = (double)NumericUpDownTestingTE.Value / 100;
                cr.isBred = rbBredTester.Checked;
            }
            Species species = speciesSelector1.SelectedSpecies;
            cr.Species = species;
            cr.RecalculateCreatureValues(_creatureCollection.getWildLevelStep());

            if (openPatternEditor)
            {
                input.OpenNamePatternEditor(cr, _topLevels.ContainsKey(cr.Species) ? _topLevels[species] : null, _lowestLevels.ContainsKey(cr.Species) ? _lowestLevels[species] : null,
                       _customReplacingNamingPattern, namingPatternIndex, ReloadNamePatternCustomReplacings);
            }
            else if (updateInheritance)
            {
                if (_extractor.ValidResults && _updateExtractorVisualData)
                    input.UpdateParentInheritances(cr);
            }
            else
            {
                input.GenerateCreatureName(cr, _topLevels.ContainsKey(cr.Species) ? _topLevels[species] : null, _lowestLevels.ContainsKey(cr.Species) ? _lowestLevels[species] : null,
                    _customReplacingNamingPattern, showDuplicateNameWarning, namingPatternIndex);
            }
        }

        private void ExtractionTestControl1_CopyToTester(string speciesBP, int[] wildLevels, int[] domLevels, bool postTamed, bool bred, double te, double imprintingBonus, bool gotoTester, testCases.TestCaseControl tcc)
        {
            NewCollection();
            LoadMultipliersFromTestCase(tcc.TestCase);
            Species species = Values.V.SpeciesByBlueprint(speciesBP);
            if (species != null)
            {
                EditCreatureInTester(new Creature(species, "", "", "", Sex.Unknown, wildLevels, domLevels, te, bred, imprintingBonus), true);
                if (gotoTester) tabControlMain.SelectedTab = tabPageStatTesting;
            }
        }

        private void ExtractionTestControl1_CopyToExtractor(string speciesBlueprint, int level, double[] statValues, bool postTamed, bool bred, double imprintingBonus, bool gotoExtractor, testCases.TestCaseControl tcc)
        {
            // test if the testcase can be extracted
            NewCollection();
            ClearAll();
            for (int s = 0; s < Values.STATS_COUNT; s++)
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

            if (tcc != null)
            {
                bool success = _extractor.ValidResults;
                if (!success)
                    tcc.SetTestResult(false, (int)watch.ElapsedMilliseconds, 0, "extraction failed");
                else
                {
                    string testText = null;
                    // test if the expected levels are possible
                    int resultCount = -Values.STATS_COUNT; // one result per stat is allowed, only count the additional ones. // TODO only consider possible stats
                    for (int s = 0; s < Values.STATS_COUNT; s++)
                    {
                        resultCount += _extractor.Results[s].Count;
                        bool statValid = false;
                        for (int r = 0; r < _extractor.Results[s].Count; r++)
                        {
                            if (_extractor.Results[s][r].levelWild == -1 || s == (int)StatNames.SpeedMultiplier && _extractor.Results[s][r].levelWild == 0 || _extractor.Results[s][r].levelWild == tcc.TestCase.levelsWild[s]
                                    && _extractor.Results[s][r].levelDom == tcc.TestCase.levelsDom[s]
                                    && (_extractor.Results[s][r].TE.Max == -1 || _extractor.Results[s][r].TE.Includes(tcc.TestCase.tamingEff))
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
            }
            if (gotoExtractor) tabControlMain.SelectedTab = tabPageExtractor;
        }

        private void LoadMultipliersFromTestCase(testCases.ExtractionTestCase etc)
        {
            // set all stat-multipliers from testcase
            _creatureCollection.serverMultipliers = etc.serverMultipliers.Copy(true);
            _creatureCollection.singlePlayerSettings = etc.singleplayerSettings;
            _creatureCollection.allowMoreThanHundredImprinting = etc.allowMoreThanHundredPercentImprinting;
            _creatureCollection.maxWildLevel = etc.maxWildLevel;

            if (Values.V.loadedModsHash == 0 || Values.V.loadedModsHash != etc.modListHash)
                LoadStatAndKibbleValues(false); // load original multipliers if they were changed

            if (etc.ModIDs.Any())
                LoadModValueFiles(Values.V.modsManifest.modsByFiles.Where(mi => etc.ModIDs.Contains(mi.Value.mod.id)).Select(mi => mi.Value.mod.FileName).ToList(),
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
                etc.tamingEff = etc.bred ? 1 : etc.postTamed ? (double)NumericUpDownTestingTE.Value / 100 : 0;
                etc.imprintingBonus = etc.bred ? (double)numericUpDownImprintingBonusTester.Value / 100 : 0;
                etc.levelsDom = GetCurrentDomLevels(false);
                etc.levelsWild = GetCurrentWildLevels(false);
                etc.ModIDs = _creatureCollection.modIDs?.ToList();
                etc.serverMultipliers = _creatureCollection.serverMultipliers;
                etc.Species = speciesSelector1.SelectedSpecies;
                etc.singleplayerSettings = _creatureCollection.singlePlayerSettings;
                etc.allowMoreThanHundredPercentImprinting = _creatureCollection.allowMoreThanHundredImprinting;
                etc.maxWildLevel = _creatureCollection.maxWildLevel;

                double[] statValues = new double[Values.STATS_COUNT];
                for (int s = 0; s < Values.STATS_COUNT; s++)
                    statValues[s] = _statIOs[s].Input;
                etc.statValues = statValues;

                extractionTestControl1.AddTestCase(etc);
                tabControlMain.SelectedTab = tabPageExtractionTests;
            }
        }

        private void copyToMultiplierTesterToolStripButton_Click(object sender, EventArgs e)
        {
            double[] statValues = new double[Values.STATS_COUNT];
            for (int s = 0; s < Values.STATS_COUNT; s++)
                statValues[s] = _statIOs[s].Input;

            bool fromExtractor = tabControlMain.SelectedTab == tabPageExtractor;

            var wildLevels = GetCurrentWildLevels(false);
            // the torpor level of the tester is only the sum of the recognized stats. Use the level of the extractor, if that value was recognized.
            if (_statIOs[(int)StatNames.Torpidity].LevelWild > 0)
                wildLevels[(int)StatNames.Torpidity] = _statIOs[(int)StatNames.Torpidity].LevelWild;

            statsMultiplierTesting1.SetCreatureValues(statValues,
               wildLevels,
                GetCurrentDomLevels(false),
                (int)numericUpDownLevel.Value,
                (double)NumericUpDownTestingTE.Value / 100,
                (double)(fromExtractor ? numericUpDownImprintingBonusExtractor.Value : numericUpDownImprintingBonusTester.Value) / 100,
                fromExtractor ? rbTamedExtractor.Checked : rbTamedTester.Checked,
                fromExtractor ? rbBredExtractor.Checked : rbBredTester.Checked);
            tabControlMain.SelectedTab = tabPageMultiplierTesting;
        }

        private void StatsMultiplierTesting1_OnApplyMultipliers()
        {
            Values.V.ApplyMultipliers(_creatureCollection);
            SetCollectionChanged(true);
        }

        private void openFolderOfCurrentFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFileName)) return;
            string path = Path.GetDirectoryName(_currentFileName);
            if (string.IsNullOrEmpty(path)) return;
            Process.Start(path);
        }

        private void customStatOverridesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var frm = new mods.CustomStatOverridesEditor(Values.V.species, _creatureCollection))
            {
                Utils.SetWindowRectangle(frm, Properties.Settings.Default.CustomStatOverrideFormRectangle);
                frm.ShowDialog();
                if (frm.StatOverridesChanged)
                {
                    Values.V.ApplyMultipliers(_creatureCollection, eventMultipliers: cbEventMultipliers.Checked, applyStatMultipliers: true);
                    SetCollectionChanged(true);
                }
                (Properties.Settings.Default.CustomStatOverrideFormRectangle, _) = Utils.GetWindowRectangle(frm);
            }
        }

        private void adminCommandToSetColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdminCommandToSetColors();
        }

        private void AdminCommandToSetColors()
        {
            if (listViewLibrary.SelectedItems.Count > 0
                && listViewLibrary.SelectedItems[0].Tag is Creature cr)
            {
                int[] cl = cr.colors;
                if (cl == null) return;
                var colorCommands = new List<string>(6);
                var enabledColorRegions = cr.Species.EnabledColorRegions;
                for (int ci = 0; ci < 6; ci++)
                {
                    if (enabledColorRegions[ci])
                        colorCommands.Add($"setTargetDinoColor {ci} {cl[ci]}");
                }
                if (colorCommands.Any())
                {
                    Clipboard.SetText((Properties.Settings.Default.AdminConsoleCommandWithCheat ? "cheat " : string.Empty) + string.Join(" | ", colorCommands));
                }
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

            string filePath = files[0];
            string ext = Path.GetExtension(filePath).ToLower();
            if (File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
            {
                ShowExportedCreatureListControl();
                _exportedCreatureList.LoadFilesInFolder(filePath);
            }
            else if (ext == ".ini")
            {
                ExtractExportedFileInExtractor(filePath);
            }
            else if (ext == ".asb")
            {
                if (!_collectionDirty
                    || MessageBox.Show("Your Creature Collection has been modified since it was last saved, " +
                            "are you sure you want to discard your changes and load the file without saving first?",
                            "Discard Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    LoadCollectionFile(filePath);
                }
            }
            else if (ext == ".zip")
            {
                if (!_collectionDirty
                    || MessageBox.Show("Your Creature Collection has been modified since it was last saved, " +
                            "are you sure you want to discard your changes and load the file without saving first?",
                            "Discard Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    OpenZippedLibrary(filePath);
                }
            }
            else if (ext == ".ark")
            {
                if (MessageBox.Show($"Import all of the creatures in the following ARK save file to the currently opened library?\n{filePath}",
                    "Import savefile?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    RunSavegameImport(new ATImportFileLocation(null, null, filePath));
            }
            else
                DoOcr(files[0]);
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
            if (listViewLibrary.SelectedItems.Count > 0)
            {
                string name = ((Creature)listViewLibrary.SelectedItems[0].Tag).name;
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
            if (listViewLibrary.SelectedItems.Count == 0) return;

            Creature[] sameSpecies = null;

            listViewLibrary.BeginUpdate();
            for (int s = 0; s < listViewLibrary.SelectedItems.Count; s++)
            {
                Creature cr = ((Creature)listViewLibrary.SelectedItems[s].Tag);

                if (sameSpecies == null || sameSpecies[0].Species != cr.Species)
                    sameSpecies = _creatureCollection.creatures.Where(c => c.Species == cr.Species).ToArray();

                // set new name
                cr.name = NamePatterns.GenerateCreatureName(cr, sameSpecies,
                    _topLevels.ContainsKey(cr.Species) ? _topLevels[cr.Species] : null,
                    _lowestLevels.ContainsKey(cr.Species) ? _lowestLevels[cr.Species] : null, _customReplacingNamingPattern, false, 0);

                UpdateDisplayedCreatureValues(cr, false, false);
            }
            listViewLibrary.EndUpdate();
        }

        private void fixColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedItems.Count == 0
            || MessageBox.Show("This color fix will only result in the correct values if no mods are used that add colors to the game.\nA backup of the library file is recommended before this fix is applied.\n\nApply color fix?",
            "Create a backup first", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            listViewLibrary.BeginUpdate();
            for (int s = 0; s < listViewLibrary.SelectedItems.Count; s++)
            {
                Creature cr = ((Creature)listViewLibrary.SelectedItems[s].Tag);

                for (int c = 0; c < 6; c++)
                    if (cr.colors[c] < 201)
                        cr.colors[c] = (cr.colors[c] - 1) % 56 + 1;
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
                MessageBox.Show($"Folder not found\n{FileService.GetJsonPath()}\n\nException: {ex.Message}", $"No data folder - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Reloads the file for the custom replacings in the naming patterns.
        /// </summary>
        private void ReloadNamePatternCustomReplacings(PatternEditor pe = null)
        {
            string filePath = FileService.GetJsonPath(FileService.CustomReplacingsNamePattern);
            string errorMessage = null;
            if (!File.Exists(filePath) || !FileService.LoadJsonFile(filePath, out _customReplacingNamingPattern, out errorMessage))
            {
                if (!string.IsNullOrEmpty(errorMessage))
                    MessageBox.Show(errorMessage, $"Custom replacing file loading error - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (pe != null) pe.SetCustomReplacings(_customReplacingNamingPattern);
        }

        private void toolStripMenuItemResetLibraryColumnWidths_Click(object sender, EventArgs e)
        {
            for (int ci = 0; ci < listViewLibrary.Columns.Count; ci++)
                listViewLibrary.Columns[ci].Width = (ci > 11 && ci < 30) ? 30 : 60;
        }

        private void copyInfographicToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedItems.Count == 0) return;

            (listViewLibrary.SelectedItems[0].Tag as Creature).ExportInfoGraphicToClipboard(_creatureCollection);
        }

        private void ToolStripMenuItemOpenWiki_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedItems.Count > 0)
            {
                string speciesName = ((Creature)listViewLibrary.SelectedItems[0].Tag).Species.name;
                if (!string.IsNullOrEmpty(speciesName))
                    Process.Start("https://ark.gamepedia.com/" + speciesName);
            }
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

        private void downloadSpeciesImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadSpeciesImagesAsync();
        }

        private async void DownloadSpeciesImagesAsync()
        {
            bool overwrite = !Directory.Exists(FileService.GetPath(FileService.ImageFolderName));
            if (!overwrite)
            {
                var msgBoxResult = MessageBox.Show(
                    "Some species color region image files seem to already exist.\nDo you want to overwrite them with possible new versions?",
                    $"Overwrite existing species images? - {Utils.ApplicationNameVersion}", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                if (msgBoxResult == DialogResult.Yes)
                    overwrite = true;
                else if (msgBoxResult != DialogResult.No)
                    return;
            }

            var (success, result) = await Updater.DownloadSpeciesImages(overwrite).ConfigureAwait(true);

            MessageBox.Show(result, $"Species images download - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }

        private void copyLibrarydumpToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDebugFile();
        }

        private void BtCopyIssueDumpToClipboard_Click(object sender, EventArgs e)
        {
            SaveDebugFile();
        }
    }
}
