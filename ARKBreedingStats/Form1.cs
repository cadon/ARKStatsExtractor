using ARKBreedingStats.duplicates;
using ARKBreedingStats.importExported;
using ARKBreedingStats.Library;
using ARKBreedingStats.ocr;
using ARKBreedingStats.settings;
using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ARKBreedingStats
{
    public partial class Form1 : Form
    {
        private CreatureCollection creatureCollection = new CreatureCollection();
        private string currentFileName = "";
        private bool collectionDirty;
        /// <summary>
        /// List of all top stats per species
        /// </summary>
        private readonly Dictionary<Species, int[]> topLevels = new Dictionary<Species, int[]>();
        private readonly List<StatIO> statIOs = new List<StatIO>();
        private readonly List<StatIO> testingIOs = new List<StatIO>();
        private int activeStatIndex = -1;
        private readonly bool[] activeStats = { true, true, true, true, true, true, true, true, true, true, true, true }; // stats used by the creature (some don't use oxygen)
        private bool pedigreeNeedsUpdate;
        private bool libraryNeedsUpdate;

        public delegate void LevelChangedEventHandler(StatIO s);
        public delegate void InputValueChangedEventHandler(StatIO s);
        public delegate void collectionChangedEventHandler(bool changed = true, Species species = null); // if null is passed for species, breeding-related controls are not updated
        public delegate void SetSpeciesEventHandler(Species species);
        public delegate void SetMessageLabelTextEventHandler(string text, MessageBoxIcon icon);

        private bool updateTorporInTester, filterListAllowed;
        private readonly bool[] considerStatHighlight = new bool[Values.STATS_COUNT]; // consider this stat for color-highlighting, topness etc
        private bool autoSave;
        private DateTime lastAutoSaveBackup = DateTime.Now.AddDays(-1);
        private int autoSaveMinutes;
        private Creature creatureTesterEdit;
        private int hiddenLevelsCreatureTester;
        private FileSync fileSync;
        private FileWatcherExports filewatcherExports;
        private readonly Extraction extractor = new Extraction();
        /// <summary>
        /// Some creatures have hidden stats they level, e.g. oxygen for aquatics
        /// </summary>
        private bool displayHiddenStats;
        private SpeechRecognition speechRecognition;
        private readonly System.Windows.Forms.Timer timerGlobal = new System.Windows.Forms.Timer();
        private readonly Dictionary<string, bool> libraryViews;
        private ExportedCreatureList exportedCreatureList;
        private MergingDuplicatesWindow mergingDuplicatesWindow;
        private ExportedCreatureControl exportedCreatureControl;
        private readonly ToolTip tt = new ToolTip();
        private bool reactOnSelectionChange;
        private CancellationTokenSource cancelTokenLibrarySelection;
        private bool clearExtractionCreatureData;
        /// <summary>
        /// The last tab-page opened in the settings.
        /// </summary>
        private int settingsLastTabPageIndex;
        /// <summary>
        /// Custom replacings for species names used in naming patterns.
        /// </summary>
        private Dictionary<string, string> customReplacingsNamingPattern;

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
        public ARKOverlay overlay;
        private static double[] lastOCRValues;
        private Species lastOCRSpecies;

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

            initLocalization();
            InitializeComponent();

            libraryViews = new Dictionary<string, bool>
            {
                    { "Dead", true },
                    { "Unavailable", true },
                    { "Neutered", true },
                    { "Mutated", true },
                    { "Obelisk", true },
                    { "Cryopod", true },
                    { "Females", true },
                    { "Males", true }
            };

            // Create an instance of a ListView column sorter and assign it
            // to the ListView controls
            listViewLibrary.ListViewItemSorter = new ListViewColumnSorter();
            listViewPossibilities.ListViewItemSorter = new ListViewColumnSorter();
            timerList1.ColumnSorter = new ListViewColumnSorter();

            listViewLibrary.DoubleBuffered(true);

            toolStripStatusLabel.Text = Application.ProductVersion;

            // delegates
            pedigree1.EditCreature += editCreatureInTester;
            pedigree1.BestBreedingPartners += ShowBestBreedingPartner;
            pedigree1.exportToClipboard += ExportAsTextToClipboard;
            breedingPlan1.EditCreature += editCreatureInTester;
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
            speciesSelector1.onSpeciesChanged += SpeciesSelector1_onSpeciesChanged;
            statsMultiplierTesting1.OnApplyMultipliers += StatsMultiplierTesting1_OnApplyMultipliers;
            raisingControl1.AdjustTimers += timerList1.AdjustAllTimersByOffset;

            speciesSelector1.SetTextBox(tbSpeciesGlobal);

            ArkOCR.OCR.setOCRControl(ocrControl1);
            ocrControl1.updateWhiteThreshold += OcrupdateWhiteThreshold;

            openSettingsToolStripMenuItem.ShortcutKeyDisplayString = new KeysConverter().ConvertTo(Keys.Control, typeof(string))?.ToString().Replace("None", ",");

            timerGlobal.Interval = 1000;
            timerGlobal.Tick += TimerGlobal_Tick;

            ReloadNamePatternCustomReplacings();

            reactOnSelectionChange = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            setLocalizations(false);

            // load window-position and size
            Size = Properties.Settings.Default.formSize;
            if (Size.Height < 200)
                Size = new Size(Size.Width, 200);
            if (Size.Width < 400)
                Size = new Size(400, Size.Height);
            Location = Properties.Settings.Default.formLocation;
            // check if form is on screen
            bool isOnScreen = false;
            foreach (Screen screen in Screen.AllScreens)
            {
                Rectangle formRectangle = new Rectangle(Left, Top, Width, Height);

                if (screen.WorkingArea.Contains(formRectangle))
                {
                    isOnScreen = true;
                    break;
                }
            }
            if (!isOnScreen)
                Location = new Point(50, 50);

            // load column-widths
            int[] cw = Properties.Settings.Default.columnWidths;
            if (cw != null)
            {
                for (int c = 0; c < cw.Length && c < listViewLibrary.Columns.Count; c++)
                    listViewLibrary.Columns[c].Width = cw[c];

                // if columns of new and not used stats is opened the first time, set their width to 0
                if (cw.Length + 4 == listViewLibrary.Columns.Count)
                {
                    for (int c = 12; c < cw.Length && c < listViewLibrary.Columns.Count; c++)
                    {
                        if (c == 17 || c == 18 || c == 22)
                            listViewLibrary.Columns[c].Width = 0;
                        else
                            listViewLibrary.Columns[c].Width = 30;
                    }
                }
            }

            // load listviewLibSorting
            ListViewColumnSorter lwvs = (ListViewColumnSorter)listViewLibrary.ListViewItemSorter;
            if (lwvs != null)
            {
                lwvs.SortColumn = Properties.Settings.Default.listViewSortCol;
                lwvs.Order = Properties.Settings.Default.listViewSortAsc ? SortOrder.Ascending : SortOrder.Descending;
            }

            // load statweights
            double[][] custWd = Properties.Settings.Default.customStatWeights;
            string[] custWs = Properties.Settings.Default.customStatWeightNames;
            Dictionary<string, double[]> custW = new Dictionary<string, double[]>();
            if (custWs != null && custWd != null)
            {
                var newToOldIndicesStatWeightings = new int[] { 0, 1, -1, 2, 3, -1, -1, 4, 5, 6, -1, -1 };

                // TODO remove this when new stat-order is established, e.g. in 6 months (2019-11)
                // if statWeights use the old order, convert
                for (int i = 0; i < custWd.Length; i++)
                {
                    if (custWd[i].Length == 7)
                    {
                        double[] newOrder = new double[Values.STATS_COUNT];
                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            if (newToOldIndicesStatWeightings[s] >= 0)
                            {
                                newOrder[s] = custWd[i][newToOldIndicesStatWeightings[s]];
                            }
                            else
                            {
                                newOrder[s] = 1;
                            }
                        }
                        custWd[i] = newOrder;
                    }
                }
                // end of conversion

                for (int i = 0; i < custWs.Length; i++)
                {
                    if (i < custWd.Length)
                    {
                        custW.Add(custWs[i], custWd[i]);
                    }
                }
            }
            breedingPlan1.statWeighting.CustomWeightings = custW;
            // last set values are saved at the end of the customweightings
            if (custWs != null && custWd != null && custWd.Length > custWs.Length)
                breedingPlan1.statWeighting.WeightValues = custWd[custWs.Length];

            autoSave = Properties.Settings.Default.autosave;
            autoSaveMinutes = Properties.Settings.Default.autosaveMinutes;

            // load weapondamages
            tamingControl1.WeaponDamages = Properties.Settings.Default.weaponDamages;
            tamingControl1.WeaponDamagesEnabled = Properties.Settings.Default.weaponDamagesEnabled;

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                var statIO = new StatIO
                {
                    InputType = StatIOInputType.FinalValueInputType,
                    Title = Utils.statName(s),
                    statIndex = s
                };
                var statIOTesting = new StatIO
                {
                    InputType = StatIOInputType.LevelsInputType,
                    Title = Utils.statName(s),
                    statIndex = s
                };

                if (Utils.precision(s) == 3)
                {
                    statIO.Percent = true;
                    statIOTesting.Percent = true;
                }

                statIOTesting.LevelChanged += testingStatIOValueUpdate;
                statIO.InputValueChanged += StatIOQuickWildLevelCheck;
                statIO.Click += new System.EventHandler(this.StatIO_Click);
                considerStatHighlight[s] = (Properties.Settings.Default.consideredStats & (1 << s)) > 0;

                statIOs.Add(statIO);
                testingIOs.Add(statIOTesting);
            }
            // add controls in the order they are shown ingame
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                flowLayoutPanelStatIOsExtractor.Controls.Add(statIOs[Values.statsDisplayOrder[s]]);
                flowLayoutPanelStatIOsExtractor.SetFlowBreak(statIOs[Values.statsDisplayOrder[s]], true);
                flowLayoutPanelStatIOsTester.Controls.Add(testingIOs[Values.statsDisplayOrder[s]]);
                flowLayoutPanelStatIOsTester.SetFlowBreak(testingIOs[Values.statsDisplayOrder[s]], true);
                checkedListBoxConsiderStatTop.Items.Add(Utils.statName(Values.statsDisplayOrder[s]), considerStatHighlight[Values.statsDisplayOrder[s]]);
            }

            // torpor should not show bar, it get's too wide and is not interesting for breeding
            statIOs[(int)StatNames.Torpidity].ShowBarAndLock = false;
            testingIOs[(int)StatNames.Torpidity].ShowBarAndLock = false;
            // move sums and footnote to bottom
            flowLayoutPanelStatIOsExtractor.Controls.Add(panelSums);
            flowLayoutPanelStatIOsExtractor.Controls.Add(labelFootnote);
            flowLayoutPanelStatIOsTester.Controls.Add(panelStatTesterFootnote);

            // some stats are not used for any species, hide them permamently (until needed in a later release)
            statIOs[(int)StatNames.Water].Hide();
            statIOs[(int)StatNames.Temperature].Hide();
            statIOs[(int)StatNames.TemperatureFortitude].Hide();
            testingIOs[(int)StatNames.Water].Hide();
            testingIOs[(int)StatNames.Temperature].Hide();
            testingIOs[(int)StatNames.TemperatureFortitude].Hide();

            breedingPlan1.MutationLimit = Properties.Settings.Default.MutationLimitBreedingPlanner;

            // enable 0-lock for dom-levels of oxygen, food (most often they are not leveld up)
            statIOs[(int)StatNames.Oxygen].DomLevelLockedZero = true;
            statIOs[(int)StatNames.Food].DomLevelLockedZero = true;

            InitializeCollection();
            filterListAllowed = true;

            // Set up the file watcher
            fileSync = new FileSync(currentFileName, CollectionChanged);
            // exports file watcher
            bool enableExportWatcher = Utils.GetFirstImportExportFolder(out string exportFolderDefault)
                && Properties.Settings.Default.AutoImportExportedCreatures;
            filewatcherExports = new FileWatcherExports(exportFolderDefault, ImportExportedAddIfPossible_WatcherThread, enableExportWatcher);

            if (!LoadStatAndKibbleValues(applySettings: false).statValuesLoaded || !Values.V.species.Any())
            {
                MessageBox.Show("The values-file couldn't be loaded, this application does not work without. Try redownloading the tool.",
                        "Error: Values-file not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            bool createNewCollection = string.IsNullOrEmpty(Properties.Settings.Default.LastSaveFile);
            if (!createNewCollection)
            {
                // if the last loaded file was already converted by someone else (e.g. if the library-file is shared),
                // ask if the converted version should be loaded instead.
                if (Path.GetExtension(Properties.Settings.Default.LastSaveFile).ToLower() == ".xml")
                {
                    string possibleConvertedCollectionPath = Path.Combine(Path.GetDirectoryName(Properties.Settings.Default.LastSaveFile), Path.GetFileNameWithoutExtension(Properties.Settings.Default.LastSaveFile) + COLLECTION_FILE_EXTENSION);
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

            if (createNewCollection)
                NewCollection();

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                statIOs[s].Input = 0;
            }

            speciesSelector1.LastSpecies = Properties.Settings.Default.lastSpecies;
            speciesSelector1.lastTabPage = tabPageExtractor;

            if (Properties.Settings.Default.lastSpecies?.Any() == true)
            {
                speciesSelector1.SetSpecies(Values.V.SpeciesByBlueprint(Properties.Settings.Default.lastSpecies[0]));
            }
            if (speciesSelector1.SelectedSpecies == null && Values.V.species.Count > 0)
                speciesSelector1.SetSpecies(Values.V.species[0]);
            tamingControl1.SetSpecies(speciesSelector1.SelectedSpecies);

            // OCR
            ocrControl1.Initialize();

            // initialize speech recognition if enabled
            InitializeSpeechRecognition();

            // default owner and tribe
            creatureInfoInputExtractor.CreatureOwner = Properties.Settings.Default.DefaultOwnerName;
            creatureInfoInputExtractor.CreatureTribe = Properties.Settings.Default.DefaultTribeName;
            creatureInfoInputExtractor.OwnerLock = Properties.Settings.Default.OwnerNameLocked;
            creatureInfoInputExtractor.TribeLock = Properties.Settings.Default.TribeNameLocked;

            // UI loaded

            //// initialize controls
            radarChart1.initializeVariables(creatureCollection.maxChartLevel);
            radarChartExtractor.initializeVariables(creatureCollection.maxChartLevel);
            radarChartLibrary.initializeVariables(creatureCollection.maxChartLevel);
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
            if (DateTime.Now.AddHours(-12) > Properties.Settings.Default.lastUpdateCheck)
                CheckForUpdates(true);

            timerGlobal.Start();
        }

        /// <summary>
        /// If the according property is set, the speechrecognition is initialized. Else it's disposed.
        /// </summary>
        private void InitializeSpeechRecognition()
        {
            if (Properties.Settings.Default.SpeechRecognition)
            {
                // var speechRecognitionAvailable = (AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.Substring(0, 13) == "System.Speech")); // TODO doens't work as intended. Should only require System.Speech if available to allow running it on MONO

                speechRecognition = new SpeechRecognition(creatureCollection.maxWildLevel, creatureCollection.considerWildLevelSteps ? creatureCollection.wildLevelStep : 1, Values.V.speciesWithAliasesList, lbListening);
                speechRecognition.speechRecognized += TellTamingData;
                speechRecognition.speechCommandRecognized += SpeechCommand;
                lbListening.Visible = true;
            }
            else
            {
                speechRecognition?.Dispose();
                speechRecognition = null;
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
                    speciesSelector1.SelectedSpecies.taming.eats.Count > 0)
            {
                tamingControl1.SetLevel(level, false);
                tamingControl1.SetSpecies(speciesSelector1.SelectedSpecies);
                overlay?.SetInfoText($"{speciesName} ({Loc.s("Level")} {level}):\n{tamingControl1.quickTamingInfos}");
            }
        }

        private void SpeechCommand(SpeechRecognition.Commands command)
        {
            // currently this command is not existing, accidental execution occured too often
            if (command == SpeechRecognition.Commands.Extract)
                DoOCR();
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
                updateTesterDetails();
        }

        private void radioButtonTesterTamed_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTamedTester.Checked)
                updateTesterDetails();
            lbWildLevelTester.Visible = rbTamedTester.Checked;
        }

        private void radioButtonTesterBred_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBredTester.Checked)
                updateTesterDetails();
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
        private void SpeciesSelector1_onSpeciesChanged(bool speciesChanged)
        {
            Species species = speciesSelector1.SelectedSpecies;
            ToggleViewSpeciesSelector(false);
            tbSpeciesGlobal.Text = species.name;
            if (!speciesChanged) return;
            clearExtractionCreatureData = true; // as soon as the user changes the species, it's assumed it's not an exported creature anymore
            pbSpecies.Image = speciesSelector1.SpeciesImage();

            creatureInfoInputExtractor.SelectedSpecies = species;
            creatureInfoInputTester.SelectedSpecies = species;
            bool isglowSpecies = species.IsGlowSpecies;

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                activeStats[s] = displayHiddenStats
                    ? species.UsesStat(s)
                    : species.DisplaysStat(s);


                statIOs[s].IsActive = activeStats[s];
                testingIOs[s].IsActive = species.UsesStat(s);
                if (!activeStats[s]) statIOs[s].Input = 0;
                statIOs[s].Title = Utils.statName(s, false, glow: isglowSpecies);
                testingIOs[s].Title = Utils.statName(s, false, isglowSpecies);
                // don't lock special stats of glowspecies
                if ((isglowSpecies &&
                      (s == (int)StatNames.Stamina
                    || s == (int)StatNames.Oxygen
                    || s == (int)StatNames.MeleeDamageMultiplier)
                  )
                  || (species.name.Contains("Daeodon")
                         && s == (int)StatNames.Food
                     )
                  )
                {
                    statIOs[s].DomLevelLockedZero = false;
                }
            }
            if (tabControlMain.SelectedTab == tabPageExtractor)
            {
                ClearAll();
            }
            else if (tabControlMain.SelectedTab == tabPageStatTesting)
            {
                updateAllTesterValues();
                statPotentials1.Species = species;
                statPotentials1.SetLevels(testingIOs.Select(s => s.LevelWild).ToArray(), true);
                setTesterInfoInputCreature();
            }
            else if (tabControlMain.SelectedTab == tabPageLibrary)
            {
                if (Properties.Settings.Default.ApplyGlobalSpeciesToLibrary)
                    listBoxSpeciesLib.SelectedIndex = listBoxSpeciesLib.Items.IndexOf(species);
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

            hiddenLevelsCreatureTester = 0;

            tt.SetToolTip(tbSpeciesGlobal, species != null ? species.DescriptiveNameAndMod + "\n" + species.blueprintPath : "[unknown]");
        }

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            n?.Select(0, n.Text.Length);
        }

        private void creatureInfoInputExtractor_Add2Library_Clicked(CreatureInfoInput sender)
        {
            AddCreatureToCollection();
        }

        private void creatureInfoInputTester_Add2Library_Clicked(CreatureInfoInput sender)
        {
            AddCreatureToCollection(false);
        }

        /// <summary>
        /// Recalculate cached values affected by global stat-multipliers.
        /// </summary>
        private void ApplySettingsToValues()
        {
            // apply multipliers
            Values.V.ApplyMultipliers(creatureCollection, cbEventMultipliers.Checked);
            tamingControl1.SetTamingMultipliers(Values.V.currentServerMultipliers.TamingSpeedMultiplier,
                                                Values.V.currentServerMultipliers.DinoCharacterFoodDrainMultiplier);

            RecalculateAllCreaturesValues();

            breedingPlan1.UpdateBreedingData();
            raisingControl1.UpdateRaisingData();

            // apply level settings
            creatureBoxListView.BarMaxLevel = creatureCollection.maxChartLevel;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                statIOs[s].barMaxLevel = creatureCollection.maxChartLevel;
                testingIOs[s].barMaxLevel = creatureCollection.maxChartLevel;
            }
            breedingPlan1.MaxWildLevels = creatureCollection.maxWildLevel;
            radarChart1.initializeVariables(creatureCollection.maxChartLevel);
            radarChartExtractor.initializeVariables(creatureCollection.maxChartLevel);
            radarChartLibrary.initializeVariables(creatureCollection.maxChartLevel);
            statPotentials1.levelDomMax = creatureCollection.maxDomLevel;
            statPotentials1.levelGraphMax = creatureCollection.maxChartLevel;

            speechRecognition?.SetMaxLevelAndSpecies(creatureCollection.maxWildLevel, creatureCollection.considerWildLevelSteps ? creatureCollection.wildLevelStep : 1, Values.V.speciesWithAliasesList);
            if (overlay != null)
            {
                overlay.InfoDuration = Properties.Settings.Default.OverlayInfoDuration;
                overlay.checkInventoryStats = Properties.Settings.Default.inventoryCheckTimer;
            }

            displayHiddenStats = Properties.Settings.Default.oxygenForAll;
            ArkOCR.OCR.screenCaptureApplicationName = Properties.Settings.Default.OCRApp;

            if (Properties.Settings.Default.showOCRButton)
            {
                Loc.ControlText(btReadValuesFromArk, tt);
            }
            else
            {
                btReadValuesFromArk.Text = "Import Exported Data";
                tt.SetToolTip(btReadValuesFromArk, "Displays all exported creatures in the default-folder (needs to be set in the settings).");
            }
            ArkOCR.OCR.waitBeforeScreenCapture = Properties.Settings.Default.waitBeforeScreenCapture;
            ocrControl1.SetWhiteThreshold(Properties.Settings.Default.OCRWhiteThreshold);

            int maxImprintingPercentage = creatureCollection.allowMoreThanHundredImprinting ? 100000 : 100;
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
                activeStats[s] = speciesSelector1.SelectedSpecies == null
                    ? (Species.displayedStatsDefault & 1 << s) != 0
                    : displayHiddenStats
                    ? speciesSelector1.SelectedSpecies.UsesStat(s)
                    : speciesSelector1.SelectedSpecies.DisplaysStat(s);
                statIOs[s].IsActive = activeStats[s];
                if (!activeStats[s]) statIOs[s].Input = 0;
            }
            if (tabControlMain.SelectedTab == tabPageStatTesting)
            {
                updateAllTesterValues();
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
                        + (string.IsNullOrEmpty(aTImportExportedFolderLocation.OwnerSuffix) ? "" : " - " + aTImportExportedFolderLocation.OwnerSuffix))
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
                    tsmi.Click += RunSavegameImport;
                    importingFromSavegameToolStripMenuItem.DropDownItems.Add(tsmi);
                }
            }
        }

        private void importingFromSavegameEmptyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSettingsDialog(2);
        }

        /// <summary>
        /// Checks each creature for tags and saves them in a list.
        /// </summary>
        private void CreateCreatureTagList()
        {
            creatureCollection.tags.Clear();
            foreach (Creature c in creatureCollection.creatures)
            {
                foreach (string t in c.tags)
                {
                    if (!creatureCollection.tags.Contains(t))
                        creatureCollection.tags.Add(t);
                }
            }
            creatureCollection.tags.Sort();

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
        private void UpdateCreatureListings(Species species = null)
        {
            // if speciesIndex == null consider all creatures, else recalculate only the indicated species if applicable
            List<Creature> creatures = creatureCollection.creatures;
            if (species != null)
            {
                creatures = creatures.Where(c => c.Species == species).ToList();
            }
            CreateOwnerList();
            CalculateTopStats(creatures);
            UpdateSpeciesLists(creatureCollection.creatures);
            FilterLib();
            UpdateStatusBar();
            breedingPlan1.breedingPlanNeedsUpdate = true;
            pedigree1.updateListView();
            raisingControl1.RecreateList();
        }

        /// <summary>
        /// This function should be called if the creatureCollection is changed, i.e. after loading a file or adding/removing a creature.
        /// It updates the listed species in the treelist and in the speciesSelector.
        /// </summary>
        private void UpdateSpeciesLists(List<Creature> creatures)
        {
            Species selectedSpecies = null;
            if (listBoxSpeciesLib.SelectedIndex > 0
                && listBoxSpeciesLib.SelectedItem.GetType() == typeof(Species))
                selectedSpecies = listBoxSpeciesLib.SelectedItem as Species;
            // clear specieslist
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
            listBoxSpeciesLib.Items.Insert(0, "All");
            foreach (Species s in availableSpecies)
                listBoxSpeciesLib.Items.Add(s);
            listBoxSpeciesLib.EndUpdate();

            if (selectedSpecies != null)
                listBoxSpeciesLib.SelectedIndex = listBoxSpeciesLib.Items.IndexOf(selectedSpecies);

            breedingPlan1.SetSpeciesList(availableSpecies, creatures);
            speciesSelector1.SetLibrarySpecies(availableSpecies);
        }

        private void CreateOwnerList()
        {
            filterListAllowed = false;

            // owner checkboxes
            checkedListBoxOwner.Items.Clear();
            bool removeWOOwner = true;
            checkedListBoxOwner.Items.Add("n/a", !creatureCollection.hiddenOwners.Contains("n/a"));
            foreach (Creature c in creatureCollection.creatures)
            {
                if (string.IsNullOrEmpty(c.owner))
                    removeWOOwner = false;
                else if (!checkedListBoxOwner.Items.Contains(c.owner))
                {
                    checkedListBoxOwner.Items.Add(c.owner, !creatureCollection.hiddenOwners.Contains(c.owner));
                    if (!tribesControl1.playerExists(c.owner))
                        tribesControl1.addPlayer(c.owner);
                }
            }
            if (removeWOOwner)
                checkedListBoxOwner.Items.RemoveAt(0);

            // server checkboxes
            var serverList = new Dictionary<string, int>();
            bool removeWOServer = true;
            foreach (Creature c in creatureCollection.creatures)
            {
                if (string.IsNullOrEmpty(c.server))
                    removeWOServer = false;
                else if (!serverList.ContainsKey(c.server))
                {
                    serverList.Add(c.server, 1);
                }
                else
                {
                    serverList[c.server]++;
                }
            }

            checkedListBoxFilterServers.Items.Clear();
            if (!removeWOServer)
                checkedListBoxFilterServers.Items.Add("n/a", !creatureCollection.hiddenServers.Contains("n/a"));
            foreach (KeyValuePair<string, int> s in serverList)
            {
                checkedListBoxFilterServers.Items.Add(s.Key + " (" + s.Value + ")", !creatureCollection.hiddenServers.Contains(s.Key));
            }

            // tag checkboxes
            checkedListBoxFilterTags.Items.Clear();
            bool removeWOTag = true;
            checkedListBoxFilterTags.Items.Add("n/a", !creatureCollection.dontShowTags.Contains("n/a"));
            foreach (Creature c in creatureCollection.creatures)
            {
                if (c.tags.Count == 0)
                    removeWOTag = false;
                else if (c.tags.Count > 0)
                {
                    for (int t = 0; t < c.tags.Count; t++)
                    {
                        if (!checkedListBoxFilterTags.Items.Contains(c.tags[t]))
                            checkedListBoxFilterTags.Items.Add(c.tags[t], !creatureCollection.dontShowTags.Contains(c.tags[t]));
                    }
                }
            }
            if (removeWOTag)
                checkedListBoxFilterTags.Items.RemoveAt(0);

            // owners
            string[] owners = tribesControl1.playerNames;
            creatureInfoInputExtractor.AutocompleteOwnerList = owners;
            creatureInfoInputTester.AutocompleteOwnerList = owners;

            // tribes
            string[] tribes = tribesControl1.tribeNames;
            creatureInfoInputExtractor.AutocompleteTribeList = tribes;
            creatureInfoInputTester.AutocompleteTribeList = tribes;

            // tribes of the owners (same index as owners)
            string[] ownersTribes = tribesControl1.ownersTribes;
            creatureInfoInputExtractor.OwnersTribes = ownersTribes;
            creatureInfoInputTester.OwnersTribes = ownersTribes;

            // server
            var serverArray = serverList.Select(s => s.Key).ToArray();
            creatureInfoInputExtractor.ServersList = serverArray;
            creatureInfoInputTester.ServersList = serverArray;

            creatureCollection.ownerList = owners;
            creatureCollection.serverList = serverArray;

            filterListAllowed = true;
        }

        private void checkForUpdatedStatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        #region check for update

        private async void CheckForUpdates(bool silentCheck = false)
        {
            if (Updater.IsProgramInstalled)
            {
                bool? installerUpdateRunning = await Updater.CheckForInstallerUpdate(silentCheck, collectionDirty);
                if (!installerUpdateRunning.HasValue) return; // error
                if (installerUpdateRunning.Value)
                {
                    Close();
                    return;
                }
            }
            else
            {
                bool? portableUpdaterRunning = await Updater.CheckForPortableUpdate(silentCheck, collectionDirty);
                if (!portableUpdaterRunning.HasValue) return; // error
                if (portableUpdaterRunning.Value)
                {
                    Close();
                    return;
                }
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
                    MessageBox.Show("Downloading and updating of the new species-stats was successful.",
                            "Success updating values", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Download of new stat successful, but files couldn't be loaded.\nTry again later, or redownload the tool.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            success.kibbleValuesLoaded = Kibbles.K.loadValues();
            if (!success.kibbleValuesLoaded)
            {
                MessageBox.Show("The kibbles-file couldn't be loaded, the kibble-recipes will not be available. " +
                        "You can redownload this application to get this file.",
                        "Error: Kibble-recipe-file not loaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            Creature creature = new Creature(speciesSelector1.SelectedSpecies, "", "", "", 0, GetCurrentWildLevels(fromExtractor), levelStep: creatureCollection.getWildLevelStep());
            List<Creature>[] parents = FindPossibleParents(creature);
            input.ParentsSimilarities = FindParentSimilarities(parents, creature);
            input.Parents = parents;
            input.parentListValid = true;
            input.NamesOfAllCreatures = creatureCollection.creatures.Select(c => c.name).ToList();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewCollection();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (collectionDirty && MessageBox.Show("Your Creature Collection has been modified since it was last saved, " +
                    "are you sure you want to discard your changes and quit without saving?",
                    "Discard Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                e.Cancel = true;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // savesettings save settings
            // save consideredStats
            int consideredStats = 0;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (considerStatHighlight[s])
                    consideredStats += 1 << s;
            }
            Properties.Settings.Default.consideredStats = consideredStats;

            // save window-position and size
            if (WindowState != FormWindowState.Minimized)
            {
                Properties.Settings.Default.formSize = Size;
                Properties.Settings.Default.formLocation = Location;
            }

            // save column-widths
            int[] cw = new int[listViewLibrary.Columns.Count];
            for (int c = 0; c < listViewLibrary.Columns.Count; c++)
                cw[c] = listViewLibrary.Columns[c].Width;
            Properties.Settings.Default.columnWidths = cw;

            // save listViewSorting
            ListViewColumnSorter lwvs = (ListViewColumnSorter)listViewLibrary.ListViewItemSorter;
            if (lwvs != null)
            {
                Properties.Settings.Default.listViewSortCol = lwvs.SortColumn;
                Properties.Settings.Default.listViewSortAsc = lwvs.Order == SortOrder.Ascending;
            }

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

            // save default owner and tribe name and if they're locked
            Properties.Settings.Default.DefaultOwnerName = creatureInfoInputExtractor.CreatureOwner;
            Properties.Settings.Default.DefaultTribeName = creatureInfoInputExtractor.CreatureTribe;
            Properties.Settings.Default.OwnerNameLocked = creatureInfoInputExtractor.OwnerLock;
            Properties.Settings.Default.TribeNameLocked = creatureInfoInputExtractor.TribeLock;

            /////// save settings for next session
            Properties.Settings.Default.Save();

            // remove old cache-files
            string imgCachePath = FileService.GetPath("img/cache");
            if (Directory.Exists(imgCachePath))
            {
                DirectoryInfo directory = new DirectoryInfo(imgCachePath);
                List<FileInfo> oldCacheFiles = directory.GetFiles().Where(f => f.LastAccessTime < DateTime.Now.AddDays(-5)).ToList();
                foreach (FileInfo f in oldCacheFiles)
                {
                    try
                    {
                        f.Delete();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            tt.Dispose();
            timerGlobal.Dispose();
        }

        /// <summary>
        /// Sets the text at the top to display infos.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="icon"></param>
        private void SetMessageLabelText(string text = "", MessageBoxIcon icon = MessageBoxIcon.None)
        {
            lbLibrarySelectionInfo.Text = text;
            switch (icon)
            {
                case MessageBoxIcon.Warning:
                    lbLibrarySelectionInfo.BackColor = Color.LightSalmon;
                    break;
                default:
                    lbLibrarySelectionInfo.BackColor = SystemColors.Control;
                    break;
            }
        }

        private void checkBoxShowDead_CheckedChanged(object sender, EventArgs e)
        {
            SetLibraryFilter("Dead", checkBoxShowDead.Checked);
        }

        private void checkBoxShowUnavailableCreatures_CheckedChanged(object sender, EventArgs e)
        {
            SetLibraryFilter("Unavailable", checkBoxShowUnavailableCreatures.Checked);
        }

        private void checkBoxShowNeuteredCreatures_CheckedChanged(object sender, EventArgs e)
        {
            SetLibraryFilter("Neutered", checkBoxShowNeuteredCreatures.Checked);
        }

        private void checkBoxShowMutatedCreatures_CheckedChanged(object sender, EventArgs e)
        {
            SetLibraryFilter("Mutated", checkBoxShowMutatedCreatures.Checked);
        }

        private void checkBoxShowObeliskCreatures_CheckedChanged(object sender, EventArgs e)
        {
            SetLibraryFilter("Obelisk", checkBoxShowObeliskCreatures.Checked);
        }

        private void checkBoxShowCryopodCreatures_CheckedChanged(object sender, EventArgs e)
        {
            SetLibraryFilter("Cryopod", checkBoxShowCryopodCreatures.Checked);
        }

        private void cbLibraryShowFemales_CheckedChanged(object sender, EventArgs e)
        {
            SetLibraryFilter("Females", cbLibraryShowFemales.Checked);
        }

        private void cbLibraryShowMales_CheckedChanged(object sender, EventArgs e)
        {
            SetLibraryFilter("Males", cbLibraryShowMales.Checked);
        }

        private void deadCreaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLibraryFilter("Dead", deadCreaturesToolStripMenuItem.Checked);
        }

        private void unavailableCreaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLibraryFilter("Unavailable", unavailableCreaturesToolStripMenuItem.Checked);
        }

        private void obeliskCreaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLibraryFilter("Obelisk", obeliskCreaturesToolStripMenuItem.Checked);
        }

        private void cryopodCreaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLibraryFilter("Cryopod", cryopodCreaturesToolStripMenuItem.Checked);
        }

        private void neuteredCreaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLibraryFilter("Neutered", neuteredCreaturesToolStripMenuItem.Checked);
        }

        private void mutatedCreaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLibraryFilter("Mutated", mutatedCreaturesToolStripMenuItem.Checked);
        }

        private void femalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLibraryFilter("Females", femalesToolStripMenuItem.Checked);
        }

        private void malesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLibraryFilter("Males", malesToolStripMenuItem.Checked);
        }

        private void checkBoxUseFiltersInTopStatCalculation_CheckedChanged(object sender, EventArgs e)
        {
            creatureCollection.useFiltersInTopStatCalculation = checkBoxUseFiltersInTopStatCalculation.Checked;
            CalculateTopStats(creatureCollection.creatures);
            FilterLib();
        }

        private void listBoxSpeciesLib_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterLib();
        }

        private void cbOwnerFilterAll_CheckedChanged(object sender, EventArgs e)
        {
            filterListAllowed = false;

            bool chck = cbOwnerFilterAll.Checked;
            creatureCollection.hiddenOwners.Clear();
            for (int i = 0; i < checkedListBoxOwner.Items.Count; i++)
            {
                checkedListBoxOwner.SetItemChecked(i, chck);
                if (!chck) creatureCollection.hiddenOwners.Add(checkedListBoxOwner.Items[i].ToString());
            }

            filterListAllowed = true;
            FilterLib();
        }

        private void cbServerFilterAll_CheckedChanged(object sender, EventArgs e)
        {
            filterListAllowed = false;

            bool chck = cbServerFilterAll.Checked;
            creatureCollection.hiddenServers.Clear();
            for (int i = 0; i < checkedListBoxFilterServers.Items.Count; i++)
            {
                checkedListBoxFilterServers.SetItemChecked(i, chck);
                if (!chck)
                {
                    string server = Regex.Match(checkedListBoxFilterServers.Items[i].ToString(), @"^(.+?)(?: \(\d+\))?$").Groups[1].Value;
                    creatureCollection.hiddenServers.Add(server);
                }
            }

            breedingPlan1.breedingPlanNeedsUpdate = true; // needed for serverFilterOption

            filterListAllowed = true;
            FilterLib();
        }

        private void checkedListBoxOwner_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (filterListAllowed)
            {
                // update shownOwners
                string owner = checkedListBoxOwner.Items[e.Index].ToString();
                if (e.NewValue == CheckState.Unchecked)
                {
                    creatureCollection.hiddenOwners.Add(owner);
                }
                else
                {
                    creatureCollection.hiddenOwners.Remove(owner);
                }

                RecalculateTopStatsIfNeeded();
                FilterLib();
            }
        }

        private void checkedListBoxFilterServers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (filterListAllowed)
            {
                // update shownServers
                string server = Regex.Match(checkedListBoxFilterServers.Items[e.Index].ToString(), @"^(.+?)(?: \(\d+\))?$").Groups[1].Value;
                if (!string.IsNullOrEmpty(server))
                {
                    if (e.NewValue == CheckState.Unchecked)
                    {
                        creatureCollection.hiddenServers.Add(server);
                    }
                    else
                    {
                        creatureCollection.hiddenServers.Remove(server);
                    }

                    breedingPlan1.breedingPlanNeedsUpdate = true; // needed for serverFilterOption

                    RecalculateTopStatsIfNeeded();
                    FilterLib();
                }
            }
        }

        private void cbFilterTagsAll_CheckedChanged(object sender, EventArgs e)
        {
            filterListAllowed = false;

            bool chck = cbFilterTagsAll.Checked;
            creatureCollection.dontShowTags.Clear();
            for (int i = 0; i < checkedListBoxFilterTags.Items.Count; i++)
            {
                checkedListBoxFilterTags.SetItemChecked(i, chck);
                if (!chck) creatureCollection.dontShowTags.Add(checkedListBoxFilterTags.Items[i].ToString());
            }

            filterListAllowed = true;
            FilterLib();
        }

        private void checkedListBoxFilterTags_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (filterListAllowed)
            {
                // update shownTags
                string tag = checkedListBoxFilterTags.Items[e.Index].ToString();
                if (e.NewValue == CheckState.Unchecked)
                {
                    creatureCollection.dontShowTags.Add(tag);
                }
                else
                {
                    creatureCollection.dontShowTags.Remove(tag);
                }

                RecalculateTopStatsIfNeeded();
                FilterLib();
            }
        }

        /// <summary>
        /// Recalculate topstats if filters are used in topstat-calculation
        /// </summary>
        private void RecalculateTopStatsIfNeeded()
        {
            if (creatureCollection.useFiltersInTopStatCalculation)
                CalculateTopStats(creatureCollection.creatures);
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
        /// <param name="sender"></param>
        /// <param name="creature"></param>
        private void CreatureBoxListView_FindParents(object sender, Creature creature)
        {
            List<Creature>[] parents = FindPossibleParents(creature);
            creatureBoxListView.parentListSimilarity = FindParentSimilarities(parents, creature);
            creatureBoxListView.parentList = parents;
        }

        private List<Creature>[] FindPossibleParents(Creature creature)
        {
            var fatherList = creatureCollection.creatures
                    .Where(cr => cr.Species == creature.Species && cr.sex == Sex.Male && cr != creature)
                    .OrderBy(cr => cr.name);
            var motherList = creatureCollection.creatures
                    .Where(cr => cr.Species == creature.Species && cr.sex == Sex.Female && cr != creature)
                    .OrderBy(cr => cr.name);

            // display new results
            return new List<Creature>[2] { motherList.ToList(), fatherList.ToList() };
        }

        private List<int>[] FindParentSimilarities(List<Creature>[] parents, Creature creature)
        {
            // similarities (number of equal wildlevels as creature, to find parents easier)
            int e; // number of equal wildlevels
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
                        if (parents[ps][p].status != CreatureStatus.Available)
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
            //creatureToolStripMenuItem.Enabled = (tabControlMain.SelectedTab == tabPageLibrary);
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
                updateAllTesterValues();
                statPotentials1.Species = speciesSelector1.SelectedSpecies;
                statPotentials1.SetLevels(testingIOs.Select(s => s.LevelWild).ToArray(), true);
            }
            else if (tabControlMain.SelectedTab == tabPageLibrary)
            {
                if (Properties.Settings.Default.ApplyGlobalSpeciesToLibrary && speciesSelector1.SelectedSpecies != null)
                    listBoxSpeciesLib.SelectedIndex = listBoxSpeciesLib.Items.IndexOf(speciesSelector1.SelectedSpecies);
                else if (libraryNeedsUpdate)
                    FilterLib();
            }
            else if (tabControlMain.SelectedTab == tabPagePedigree)
            {
                if (pedigreeNeedsUpdate && listViewLibrary.SelectedItems.Count > 0)
                {
                    Creature c = null;
                    c = (Creature)listViewLibrary.SelectedItems[0].Tag;
                    pedigree1.EnabledColorRegions = c.Species?.colors?.Select(n => !string.IsNullOrEmpty(n?.name)).ToArray() ?? new bool[6] { true, true, true, true, true, true };

                    pedigree1.setCreature(c, true);
                    pedigreeNeedsUpdate = false;
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

        private void ExtractBaby(Creature mother, Creature father)
        {
            if (mother == null || father == null)
                return;

            speciesSelector1.SetSpecies(mother.Species);
            rbBredExtractor.Checked = true;
            numericUpDownImprintingBonusTester.Value = 0;

            creatureInfoInputExtractor.mother = mother;
            creatureInfoInputExtractor.father = father;
            creatureInfoInputExtractor.CreatureOwner = mother.owner;
            creatureInfoInputExtractor.CreatureTribe = mother.tribe;
            creatureInfoInputExtractor.CreatureServer = mother.server;
            UpdateParentListInput(creatureInfoInputExtractor);
            tabControlMain.SelectedTab = tabPageExtractor;
        }

        private void NumericUpDownTestingTE_ValueChanged(object sender, EventArgs e)
        {
            updateAllTesterValues();
            lbWildLevelTester.Text = "PreTame Level: " + Math.Ceiling(Math.Round((testingIOs[(int)StatNames.Torpidity].LevelWild + 1) / (1 + NumericUpDownTestingTE.Value / 200), 6));
        }

        private void numericUpDownImprintingBonusTester_ValueChanged(object sender, EventArgs e)
        {
            updateAllTesterValues();
            // calculate number of imprintings
            if (speciesSelector1.SelectedSpecies.breeding != null && speciesSelector1.SelectedSpecies.breeding.maturationTimeAdjusted > 0)
                lbImprintedCount.Text = "(" + Math.Round((double)numericUpDownImprintingBonusTester.Value / (100 * Utils.imprintingGainPerCuddle(speciesSelector1.SelectedSpecies.breeding.maturationTimeAdjusted, Values.V.currentServerMultipliers.BabyCuddleIntervalMultiplier)), 2) + "×)";
            else lbImprintedCount.Text = "";
        }

        private void numericUpDownImprintingBonusExtractor_ValueChanged(object sender, EventArgs e)
        {
            // calculate number of imprintings
            if (speciesSelector1.SelectedSpecies.breeding != null && speciesSelector1.SelectedSpecies.breeding.maturationTimeAdjusted > 0)
                lbImprintingCuddleCountExtractor.Text = "(" + Math.Round((double)numericUpDownImprintingBonusExtractor.Value / (100 * Utils.imprintingGainPerCuddle(speciesSelector1.SelectedSpecies.breeding.maturationTimeAdjusted, Values.V.currentServerMultipliers.BabyCuddleIntervalMultiplier))) + "×)";
            else lbImprintingCuddleCountExtractor.Text = "";
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

        private void ExportSelectedCreatureToClipboard(bool breeding = true, bool ARKml = true)
        {
            if (tabControlMain.SelectedTab == tabPageStatTesting || tabControlMain.SelectedTab == tabPageExtractor)
            {
                bool fromExtractor = tabControlMain.SelectedTab == tabPageExtractor;
                if (!fromExtractor || extractor.validResults)
                {
                    CreatureInfoInput input;
                    bool bred;
                    double te, imprinting;
                    Species species = speciesSelector1.SelectedSpecies;
                    if (fromExtractor)
                    {
                        input = creatureInfoInputExtractor;
                        bred = rbBredExtractor.Checked;
                        te = extractor.UniqueTE();
                        imprinting = extractor.ImprintingBonus;
                    }
                    else
                    {
                        input = creatureInfoInputTester;
                        bred = rbBredTester.Checked;
                        te = (double)NumericUpDownTestingTE.Value / 100;
                        imprinting = (double)numericUpDownImprintingBonusTester.Value / 100;
                    }

                    var levelStep = creatureCollection.getWildLevelStep();
                    Creature creature = new Creature(species, input.CreatureName, input.CreatureOwner, input.CreatureTribe, input.CreatureSex, GetCurrentWildLevels(fromExtractor), GetCurrentDomLevels(fromExtractor), te, bred, imprinting, levelStep)
                    {
                        colors = input.RegionColors,
                        ArkId = input.ArkId
                    };
                    creature.RecalculateCreatureValues(levelStep);
                    ExportAsTextToClipboard(creature, breeding, ARKml);
                }
                else
                    MessageBox.Show("There is no valid extracted creature to export.", "No valid data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (listViewLibrary.SelectedItems.Count > 0)
                    ExportAsTextToClipboard((Creature)listViewLibrary.SelectedItems[0].Tag, breeding, ARKml);
                else
                    MessageBox.Show("No creatures in the library selected to copy to the clipboard", "No Creatures Selected",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (listViewLibrary.SelectedItems.Count > 0)
                CopyCreatureToClipboard((Creature)listViewLibrary.SelectedItems[0].Tag);
            else
                MessageBox.Show("No creatures in the library selected to copy to the clipboard", "No Creatures Selected",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void importValuesFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteCreatureValuesFromClipboard();
        }

        private void pasteCreatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pasteCreatureFromClipboardToTester();
        }

        private void CopyCreatureToClipboard(Creature c)
        {
            if (c != null)
            {
                var serializer = new XmlSerializer(typeof(Creature));
                var sb = new StringBuilder();
                using (var writer = new StringWriter(sb))
                {
                    try
                    {
                        serializer.Serialize(writer, c);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Couldn\'t serialize creature-object.\nErrormessage:\n\n{e.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                string clpb = sb.ToString();
                if (clpb.Length > 0)
                    Clipboard.SetText(clpb);
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
                        if (Utils.precision(s) == 3)// percentage values
                            sv[s] *= 0.01;
                    }

                    int.TryParse(m.Groups[3].Value, out int totalLevel);
                    double.TryParse(m.Groups[4].Value, out double te);
                    te *= .01;
                    double.TryParse(m.Groups[5].Value, out double ib);
                    ib *= .01;

                    if (Values.V.TryGetSpeciesByName(m.Groups[2].Value, out Species species))
                    {
                        var cv = new CreatureValues(species, m.Groups[1].Value, "", "", sex, sv, totalLevel, te, te, te > 0 || ib > 0, ib > 0, ib, CreatureFlags.None, null, null)
                        {
                            levelsWild = wl,
                            levelsDom = dl
                        };
                        if (tabControlMain.SelectedTab == tabPageStatTesting)
                            SetCreatureValuesToTester(cv);
                        else
                            SetCreatureValuesToExtractor(cv);
                    }
                    else MessageBox.Show("unknown species:\n" + m.Groups[2].Value, "Species not recognized", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonRecalculateTops_Click(object sender, EventArgs e)
        {
            for (int s = 0; s < Values.STATS_COUNT; s++)
                considerStatHighlight[Values.statsDisplayOrder[s]] = checkedListBoxConsiderStatTop.GetItemChecked(s);
            // recalculate topstats
            CalculateTopStats(creatureCollection.creatures);
            FilterLib();
        }

        private void removeCooldownGrowingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewLibrary.BeginUpdate();
            foreach (ListViewItem i in listViewLibrary.SelectedItems)
            {
                Creature c = (Creature)i.Tag;
                if (c.cooldownUntil > DateTime.Now)
                    c.cooldownUntil = null;
                if (c.growingUntil > DateTime.Now)
                    c.growingUntil = null;

                i.SubItems[11].Text = "-"; // LVI index
                // color for cooldown
                CooldownColors(c, out Color forecolor, out Color backcolor);
                i.SubItems[11].ForeColor = forecolor;
                i.SubItems[11].BackColor = backcolor;
            }
            breedingPlan1.breedingPlanNeedsUpdate = true;
            listViewLibrary.EndUpdate();
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
            if (cs.Count > 0)
                SetStatus(cs, s);
        }

        private void SetStatus(IEnumerable<Creature> cs, CreatureStatus s)
        {
            bool changed = false;
            List<string> speciesBlueprints = new List<string>();
            foreach (Creature c in cs)
            {
                if (c.status != s)
                {
                    changed = true;
                    c.status = s;
                    if (!speciesBlueprints.Contains(c.speciesBlueprint))
                        speciesBlueprints.Add(c.speciesBlueprint);
                }
            }
            if (changed)
            {
                // update list / recalculate topstats
                CalculateTopStats(creatureCollection.creatures.Where(c => speciesBlueprints.Contains(c.speciesBlueprint)).ToList());
                FilterLib();
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
            if (c.status != CreatureStatus.Available
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
        private void OpenSettingsDialog(int page = -1)
        {
            if (page == -1)
                page = settingsLastTabPageIndex;
            using (Settings settingsfrm = new Settings(creatureCollection, page))
            {
                if (settingsfrm.ShowDialog() == DialogResult.OK)
                {
                    ApplySettingsToValues();
                    if (settingsfrm.LanguageChanged) setLocalizations();
                    autoSave = Properties.Settings.Default.autosave;
                    autoSaveMinutes = Properties.Settings.Default.autosaveMinutes;
                    creatureBoxListView.maxDomLevel = creatureCollection.maxDomLevel;
                    fileSync.ChangeFile(currentFileName); // only to trigger the update, filename is not changed

                    bool enableExportWatcher = Utils.GetFirstImportExportFolder(out string exportFolderDefault)
                        && Properties.Settings.Default.AutoImportExportedCreatures;
                    filewatcherExports.SetWatchFolder(exportFolderDefault, enableExportWatcher);

                    InitializeSpeechRecognition();

                    SetCollectionChanged(true);
                }
                settingsLastTabPageIndex = settingsfrm.LastTabPageIndex;
            }
        }

        /// <summary>
        /// Display the wild-levels, assuming it's a wild creature. Used for quick checking
        /// </summary>
        /// <param name="sIO"></param>
        private void StatIOQuickWildLevelCheck(StatIO sIO)
        {
            clearExtractionCreatureData = true; // as soon as the user changes stat-values, it's assumed it's not an exported creature anymore
            if (cbQuickWildCheck.Checked)
            {
                int lvlWild = (int)Math.Round((sIO.Input - speciesSelector1.SelectedSpecies.stats[sIO.statIndex].BaseValue) / (speciesSelector1.SelectedSpecies.stats[sIO.statIndex].BaseValue * speciesSelector1.SelectedSpecies.stats[sIO.statIndex].IncPerWildLevel));
                sIO.LevelWild = lvlWild < 0 ? 0 : lvlWild;
                sIO.LevelDom = 0;
                if (sIO.statIndex == (int)StatNames.Torpidity)
                {
                    SetQuickTamingInfo(statIOs[(int)StatNames.Torpidity].LevelWild + 1);
                }
            }
        }

        // context-menu for library
        private void toolStripMenuItemEdit_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count > 0)
                editCreatureInTester((Creature)listViewLibrary.Items[listViewLibrary.SelectedIndices[0]].Tag);
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
                setCreatureValuesToExtractor((Creature)listViewLibrary.Items[listViewLibrary.SelectedIndices[0]].Tag);
        }

        private void wildValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count > 0)
                setCreatureValuesToExtractor((Creature)listViewLibrary.Items[listViewLibrary.SelectedIndices[0]].Tag, true);
        }

        private void CreateTimer(string name, DateTime time, Creature c, string group)
        {
            timerList1.AddTimer(name, time, c, group);
        }

        /// <summary>
        /// Performs an optical character recognition on either the specified image or a screenshot of the game and extracts the stat-values.
        /// </summary>
        /// <param name="imageFilePath">If specified, this image is taken instead of a screenshot.</param>
        /// <param name="manuallyTriggered"></param>
        public void DoOCR(string imageFilePath = "", bool manuallyTriggered = true)
        {
            cbQuickWildCheck.Checked = false;

            double[] OCRvalues = ArkOCR.OCR.doOCR(out string debugText, out string dinoName, out string speciesName, out string ownerName, out string tribeName, out Sex sex, imageFilePath, manuallyTriggered);

            ocrControl1.output.Text = debugText;
            if (OCRvalues.Length <= 1)
            {
                if (manuallyTriggered) MessageBox.Show(debugText, "ASB - OCR error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                statIOs[displayedStatIndices[i]].Input = statIOs[displayedStatIndices[i]].percent
                    ? OCRvalues[i] / 100.0
                    : OCRvalues[i];
            }

            // use imprinting if existing
            if (OCRvalues.Length > 8 && OCRvalues[8] >= 0 && (OCRvalues[8] <= 100 || creatureCollection.allowMoreThanHundredImprinting))
            {
                rbBredExtractor.Checked = true;
                if (!Properties.Settings.Default.OCRIgnoresImprintValue)
                    numericUpDownImprintingBonusExtractor.ValueSave = (decimal)OCRvalues[8];
            }
            else
            {
                rbTamedExtractor.Checked = true;
            }

            if (!manuallyTriggered
                || cbGuessSpecies.Checked
                || !Values.V.TryGetSpeciesByName(speciesName, out Species speciesByName))
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

                    if (lastOCRValues != null)
                        for (int i = 0; i < 10; i++)
                            if (OCRvalues[i] != lastOCRValues[i])
                            {
                                sameValues = false;
                                break;
                            }

                    // if there's more than one option, on manual we cycle through the options if we're trying multiple times
                    // on automated, we take the first one that yields an error-free level extraction
                    if (manuallyTriggered && sameValues)
                    {
                        int newindex = (possibleSpecies.IndexOf(lastOCRSpecies) + 1) % possibleSpecies.Count;
                        speciesSelector1.SetSpecies(possibleSpecies[newindex]);
                        lastOCRSpecies = possibleSpecies[newindex];
                        lastOCRValues = OCRvalues;
                        ExtractLevels(true);
                    }
                    else
                    { // automated, or first manual attempt at new values
                        bool foundPossiblyGood = false;
                        for (int dinooption = 0; dinooption < possibleSpecies.Count() && foundPossiblyGood == false; dinooption++)
                        {
                            // if the last OCR'ed values are the same as this one, the user may not be happy with the dino species selection and want another one
                            // so we'll cycle to the next one, but only if the OCR is manually triggered, on autotrigger (ie, overlay), don't change
                            speciesSelector1.SetSpecies(possibleSpecies[dinooption]);
                            lastOCRSpecies = possibleSpecies[dinooption];
                            lastOCRValues = OCRvalues;
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

            lastOCRValues = OCRvalues;
            if (tabControlMain.SelectedTab != TabPageOCR)
                tabControlMain.SelectedTab = tabPageExtractor;

            // in the new ui the current weight is not shown anymore
            //// current weight for babies (has to be after the correct species is set in the combobox)
            //if (OCRvalues.Length > 10 && OCRvalues[10] > 0)
            //{
            //    creatureInfoInputExtractor.babyWeight = OCRvalues[10];
            //}
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
            var scores = Values.V.species.Select(sp => new { Score = DiceCoefficient.diceCoefficient(sp.name.Replace(" ", ""), speciesName.Replace(" ", "")), Species = sp }).OrderByDescending(o => o.Score);
            if (scores.First().Score > 0.4)
            {
                possibleSpecies.Add(scores.First().Species);
                return possibleSpecies;
            }

            if (stats.Length > Values.STATS_COUNT && stats[Values.STATS_COUNT] > 0)
            {
                // creature is imprinted, the following algorithm cannot handle this yet. use current selected species
                possibleSpecies.Add(speciesSelector1.SelectedSpecies);
                return possibleSpecies;
            }

            foreach (var species in Values.V.species)
            {
                if (species == speciesSelector1.SelectedSpecies) continue; // the currently selected species is ignored here and set as top priority at the end

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
                        possibleLevel = (statIOs[s].Input - baseValue) / (baseValue * incWild);

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

                possibleLevel = (statIOs[(int)StatNames.Torpidity].Input - species.stats[(int)StatNames.Torpidity].AddWhenTamed - baseValue) / (baseValue * incWild);
                double possibleLevelWild = (statIOs[(int)StatNames.Torpidity].Input - baseValue) / (baseValue * incWild);

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
                possibleLevel = (statIOs[(int)StatNames.Oxygen].Input - species.stats[(int)StatNames.Oxygen].AddWhenTamed - baseValue) / (baseValue * incWild);

                if (possibleLevel < 0 || possibleLevel > (double)numericUpDownLevel.Value - 1)
                    continue;

                if (Math.Round(possibleLevel, 3) != (int)possibleLevel || possibleLevel > (double)numericUpDownLevel.Value / 2)
                    likely = false;

                if (statIOs[(int)StatNames.Oxygen].Input != 0 && baseValue == 0)
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
            if (overlay == null)
            {
                overlay = new ARKOverlay
                {
                    ExtractorForm = this,
                    InfoDuration = Properties.Settings.Default.OverlayInfoDuration,
                    checkInventoryStats = Properties.Settings.Default.inventoryCheckTimer
                };
                overlay.InitLabelPositions();
            }

            if (cbToggleOverlay.Checked)
            {
                Process[] p = Process.GetProcessesByName(Properties.Settings.Default.OCRApp);

                if (p.Length == 0)
                {
                    MessageBox.Show("Process for capturing screenshots and for overlay (e.g. the game, or a stream of the game) not found. " +
                            "Start the game or change the process in the settings.", "Game started?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cbToggleOverlay.Checked = false;
                    return;
                }

                IntPtr mwhd = p[0].MainWindowHandle;
                Screen scr = Screen.FromHandle(mwhd);
                overlay.Location = scr.WorkingArea.Location;
            }

            overlay.Visible = cbToggleOverlay.Checked;
            overlay.enableOverlayTimer = cbToggleOverlay.Checked;

            // disable speechrecognition if overlay is disabled. (no use if no data can be displayed)
            if (speechRecognition != null && !cbToggleOverlay.Checked)
                speechRecognition.Listen = false;
        }

        private void toolStripButtonCopy2Tester_Click(object sender, EventArgs e)
        {
            double te = extractor.UniqueTE();
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
                testingIOs[s].LevelWild = statIOs[s].LevelWild;
                testingIOs[s].LevelDom = statIOs[s].LevelDom;
                testingStatIOValueUpdate(testingIOs[s]);
            }
            setTesterInfoInputCreature();
            creatureInfoInputTester.CreatureSex = creatureInfoInputExtractor.CreatureSex;
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
                    testingIOs[s].LevelDom = 0;
                    testingIOs[s].LevelWild = 0;
                }
                creatureInfoInputTester.Clear();
            }
        }

        private void toolStripButtonCopy2Extractor_Click(object sender, EventArgs e)
        {
            ClearAll();
            // copy values from tester over to extractor
            for (int s = 0; s < Values.STATS_COUNT; s++)
                statIOs[s].Input = testingIOs[s].Input;
            if (rbBredTester.Checked)
                rbBredExtractor.Checked = true;
            else if (rbTamedTester.Checked)
                rbTamedExtractor.Checked = true;
            else
                rbWildExtractor.Checked = true;
            numericUpDownImprintingBonusExtractor.Value = numericUpDownImprintingBonusTester.Value;
            // set total level
            numericUpDownLevel.Value = testingIOs[(int)StatNames.Torpidity].LevelWild + GetCurrentDomLevels(false).Sum() + 1;

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
            if (overlay != null && overlay.checkInventoryStats)
            {
                var wildLevels = GetCurrentWildLevels();
                var tamedLevels = GetCurrentDomLevels();
                Color[] colors = new Color[Values.STATS_COUNT];


                for (int i = 0; i < Values.STATS_COUNT; i++)
                {
                    wildLevels[i] = wildLevels[i] > 0 ? wildLevels[i] : 0;
                    tamedLevels[i] = tamedLevels[i] > 0 ? tamedLevels[i] : 0;
                    colors[i] = statIOs[i].BackColor;
                }
                int levelWild = wildLevels[(int)StatNames.Torpidity] + 1;
                int levelDom = tamedLevels.Sum();

                string extraText = speciesSelector1.SelectedSpecies.name;
                if (!extractor.postTamed)
                {
                    string foodName = speciesSelector1.SelectedSpecies.taming.eats[0];
                    int foodNeeded = Taming.FoodAmountNeeded(speciesSelector1.SelectedSpecies, levelWild, Values.V.currentServerMultipliers.TamingSpeedMultiplier, foodName, speciesSelector1.SelectedSpecies.taming.nonViolent);
                    Taming.TamingTimes(speciesSelector1.SelectedSpecies, levelWild, Values.V.currentServerMultipliers.TamingSpeedMultiplier, Values.V.currentServerMultipliers.DinoCharacterFoodDrainMultiplier, foodName, foodNeeded, out _, out TimeSpan duration, out int narcoBerries, out int narcotics, out int bioToxines, out double te, out _, out int bonusLevel, out _);
                    string foodNameDisplay = foodName == "Kibble" ? speciesSelector1.SelectedSpecies.taming.favoriteKibble + " Egg Kibble" : foodName;
                    extraText += "\nTaming takes " + duration.ToString(@"hh\:mm\:ss") + " with " + foodNeeded + "×" + foodNameDisplay
                            + "\n" + narcoBerries + " Narcoberries or " + narcotics + " Narcotics or " + bioToxines + " Bio Toxines are needed"
                            + "\nTaming Effectiveness: " + Math.Round(100 * te, 1) + " % (+" + bonusLevel + " lvl)";
                }

                overlay.SetStatLevels(wildLevels, tamedLevels, levelWild, levelDom, colors);
                overlay.SetInfoText(extraText);
            }
        }

        private void findDuplicatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowDuplicateMergerAndCheckForDuplicates(creatureCollection.creatures);
        }

        private void ShowDuplicateMergerAndCheckForDuplicates(List<Creature> creatureList)
        {
            MessageBox.Show("This feature is not yet included.");
            return;
            // TODO
#pragma warning disable 162
            if (mergingDuplicatesWindow == null || mergingDuplicatesWindow.IsDisposed)
            {
                mergingDuplicatesWindow = new duplicates.MergingDuplicatesWindow();
                mergingDuplicatesWindow.RefreshLibrary += FilterLib;
            }
            mergingDuplicatesWindow.Show();
            mergingDuplicatesWindow.CheckForDuplicates(creatureList);
#pragma warning restore 162
        }

        private void btnReadValuesFromArk_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showOCRButton)
                DoOCR();
            else
                ImportExportedCreaturesDefaultFolder();
        }

        private void toolStripButtonAddTribe_Click(object sender, EventArgs e)
        {
            tribesControl1.addTribe();
        }

        private void button2TamingCalc_Click(object sender, EventArgs e)
        {
            tamingControl1.SetSpecies(speciesSelector1.SelectedSpecies);
            if (cbQuickWildCheck.Checked)
                tamingControl1.SetLevel(statIOs[(int)StatNames.Torpidity].LevelWild + 1);
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
                    double imprintingGainPerCuddle = Utils.imprintingGainPerCuddle(speciesSelector1.SelectedSpecies.breeding.maturationTimeAdjusted, Values.V.currentServerMultipliers.BabyCuddleIntervalMultiplier);
                    int cuddleCount = (int)Math.Round((double)numericUpDownImprintingBonusTester.Value / (100 * imprintingGainPerCuddle));
                    double imprintingBonus;
                    do
                    {
                        imprintingBonus = Math.Round(100 * cuddleCount * imprintingGainPerCuddle, 5);
                        cuddleCount--;
                    }
                    while (imprintingBonus > 100 && !creatureCollection.allowMoreThanHundredImprinting);
                    numericUpDownImprintingBonusTester.ValueSave = (decimal)imprintingBonus;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                double imprintingFactorTorpor = speciesSelector1.SelectedSpecies.statImprintMult[(int)StatNames.Torpidity] * creatureCollection.serverMultipliers.BabyImprintingStatScaleMultiplier;
                // set imprinting value so the set levels in the tester yield the value in the extractor
                double imprintingBonus = imprintingFactorTorpor != 0
                                         ? (statIOs[(int)StatNames.Torpidity].Input / StatValueCalculation.CalculateValue(speciesSelector1.SelectedSpecies, (int)StatNames.Torpidity, testingIOs[(int)StatNames.Torpidity].LevelWild, 0, true, 1, 0) - 1) / imprintingFactorTorpor
                                         : 0;
                if (imprintingBonus < 0)
                    imprintingBonus = 0;
                if (!creatureCollection.allowMoreThanHundredImprinting && imprintingBonus > 1)
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

            bool result = LoadModValueFiles(filePaths, showResult, applySettings, out List<Mod> mods);
            cc.ModList = mods;
            return result;
        }

        private void loadAdditionalValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var modValuesManager = new ModValuesManager
            {
                CreatureCollection = creatureCollection
            })
            {
                modValuesManager.ShowDialog();
            }

            // if the mods for the library changed,
            // first check if all mod value files are available and load missing files if possible,
            // then reload all values and modvalues
            if (creatureCollection.ModValueReloadNeeded
                && LoadModValuesOfCollection(creatureCollection, true, true))
                SetCollectionChanged(true);
        }

        private void toolStripButtonAddPlayer_Click(object sender, EventArgs e)
        {
            tribesControl1.addPlayer();
        }

        private void UpdateStatusBar()
        {
            var creatureCount = creatureCollection.creatures.Where(c => !c.flags.HasFlag(CreatureFlags.Placeholder));
            int total = creatureCount.Count();
            int obelisk = creatureCount.Count(c => c.status == CreatureStatus.Obelisk);
            int cryopod = creatureCount.Count(c => c.status == CreatureStatus.Cryopod);

            int modValueCount = creatureCollection.ModList?.Count ?? 0;

            toolStripStatusLabel.Text = total + " creatures in Library"
                + (total > 0 ? " ("
                + "available: " + creatureCount.Count(c => c.status == CreatureStatus.Available)
                            + ", unavailable: " + creatureCount.Count(c => c.status == CreatureStatus.Unavailable)
                + ", dead: " + creatureCount.Count(c => c.status == CreatureStatus.Dead)
                            + (obelisk > 0 ? ", obelisk: " + obelisk : "")
                + (cryopod > 0 ? ", cryopod: " + cryopod : "")
                            : "")
                    + ". v" + Application.ProductVersion /*+ "-BETA"*/ + " / values: " + Values.V.Version +
                    (modValueCount > 0 ? ", additional values from " + modValueCount.ToString() + " mods (" + string.Join(", ", creatureCollection.ModList.Select(m => m.title).ToArray()) + ")" : "");
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
            speechRecognition?.ToggleListening();
        }

        private void CreateIncubationTimer(Creature mother, Creature father, TimeSpan incubationDuration, bool incubationStarted)
        {
            raisingControl1.AddIncubationTimer(mother, father, incubationDuration, incubationStarted);
            libraryNeedsUpdate = true; // because mating-cooldown of mother was set
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
            Values.V.ApplyMultipliers(creatureCollection, cbEventMultipliers.Checked, false);

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

        private void OcrupdateWhiteThreshold(int value)
        {
            Properties.Settings.Default.OCRWhiteThreshold = value;
            ArkOCR.OCR.whiteThreshold = value;
        }

        private void toolStripCBTempCreatures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripCBTempCreatures.SelectedIndex >= 0 && toolStripCBTempCreatures.SelectedIndex < creatureCollection.creaturesValues.Count)
            {
                SetCreatureValuesToExtractor(creatureCollection.creaturesValues[toolStripCBTempCreatures.SelectedIndex]);
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
                testingIOs[s].LevelWild = cv.levelsWild[s];
                testingIOs[s].LevelDom = cv.levelsDom[s];
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
            input.CreatureOwner = cv.owner;
            input.CreatureTribe = cv.tribe;
            input.CreatureServer = cv.server;
            input.CreatureSex = cv.sex;
            input.CreatureGuid = cv.guid;
            input.creatureFlags = cv.flags;
            input.mother = cv.Mother;
            input.father = cv.Father;
            input.RegionColors = cv.colorIDs;
            input.SetArkId(cv.ARKID, cv.guid == Utils.ConvertArkIdToGuid(cv.ARKID));
            input.MutationCounterMother = cv.mutationCounterMother;
            input.MutationCounterFather = cv.mutationCounterFather;
            input.Grown = cv.growingUntil;
            input.Cooldown = cv.cooldownUntil;
            input.MotherArkId = cv.motherArkId;
            input.FatherArkId = cv.fatherArkId;
            input.CreatureNote = "";
            input.CreatureStatus = CreatureStatus.Available;
            input.SetTimersToChanged();
        }

        private void toolStripButtonSaveCreatureValuesTemp_Click(object sender, EventArgs e)
        {
            CreatureValues cv = new CreatureValues();
            for (int s = 0; s < Values.STATS_COUNT; s++)
                cv.statValues[s] = statIOs[s].Input;
            cv.speciesName = speciesSelector1.SelectedSpecies.name;
            cv.speciesBlueprint = speciesSelector1.SelectedSpecies.blueprintPath;
            cv.name = creatureInfoInputExtractor.CreatureName;
            cv.owner = creatureInfoInputExtractor.CreatureOwner;
            cv.tribe = creatureInfoInputExtractor.CreatureTribe;
            cv.server = creatureInfoInputExtractor.CreatureServer;
            cv.sex = creatureInfoInputExtractor.CreatureSex;
            cv.flags = creatureInfoInputExtractor.creatureFlags;
            cv.Mother = creatureInfoInputExtractor.mother;
            cv.Father = creatureInfoInputExtractor.father;
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

            creatureCollection.creaturesValues.Add(cv);
            SetCollectionChanged(true);

            UpdateTempCreatureDropDown();
        }

        private void toolStripButtonDeleteTempCreature_Click(object sender, EventArgs e)
        {
            if (toolStripCBTempCreatures.SelectedIndex >= 0
                    && MessageBox.Show("Remove the data of this cached creature?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                creatureCollection.creaturesValues.RemoveAt(toolStripCBTempCreatures.SelectedIndex);
                UpdateTempCreatureDropDown();
                SetCollectionChanged(true, null);
            }
        }

        /// <summary>
        /// Update the combolist with the temporary saved creature-values.
        /// </summary>
        private void UpdateTempCreatureDropDown()
        {
            toolStripCBTempCreatures.Items.Clear();
            foreach (CreatureValues cv in creatureCollection.creaturesValues)
                toolStripCBTempCreatures.Items.Add($"{cv.name} ({cv.Species?.name ?? "unknown species"})");
        }

        /// <summary>
        /// Collects the data needed for the name pattern editor.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="openPatternEditor"></param>
        private void CreatureInfoInput_CreatureDataRequested(CreatureInfoInput input, bool openPatternEditor, bool showDuplicateNameWarning)
        {
            Creature cr = new Creature
            {
                ArkId = input.ArkId,
                ArkIdImported = input.ArkIdImported
            };
            cr.guid = input.CreatureGuid;
            cr.name = input.CreatureName;
            if (input == creatureInfoInputExtractor)
            {
                cr.levelsWild = statIOs.Select(s => s.LevelWild).ToArray();
                cr.imprintingBonus = extractor.ImprintingBonus;
                cr.tamingEff = extractor.UniqueTE();
                cr.isBred = rbBredExtractor.Checked;
            }
            else
            {
                cr.levelsWild = testingIOs.Select(s => s.LevelWild).ToArray();
                cr.imprintingBonus = (double)numericUpDownImprintingBonusTester.Value / 100;
                cr.tamingEff = (double)NumericUpDownTestingTE.Value / 100;
                cr.isBred = rbBredTester.Checked;
            }
            Species species = speciesSelector1.SelectedSpecies;
            cr.Species = species;
            cr.RecalculateCreatureValues(creatureCollection.getWildLevelStep());
            if (topLevels.ContainsKey(species))
            {
                for (int si = 0; si < Values.STATS_COUNT; si++)
                {
                    cr.topBreedingStats[si] = species.UsesStat(si) && cr.levelsWild[si] >= topLevels[species][si];
                }
            }
            else
            {
                for (int si = 0; si < Values.STATS_COUNT; si++)
                {
                    cr.topBreedingStats[si] = species.UsesStat(si) && cr.levelsWild[si] > 0;
                }
            }

            if (openPatternEditor)
                input.OpenNamePatternEditor(cr, customReplacingsNamingPattern, ReloadNamePatternCustomReplacings);
            else
                input.GenerateCreatureName(cr, customReplacingsNamingPattern, showDuplicateNameWarning);
        }

        private void ExtractionTestControl1_CopyToTester(string speciesBP, int[] wildLevels, int[] domLevels, bool postTamed, bool bred, double te, double imprintingBonus, bool gotoTester, testCases.TestCaseControl tcc)
        {
            NewCollection();
            LoadMultipliersFromTestCase(tcc.testCase);
            Species species = Values.V.SpeciesByBlueprint(speciesBP);
            if (species != null)
            {
                editCreatureInTester(new Creature(species, "", "", "", Sex.Unknown, wildLevels, domLevels, te, bred, imprintingBonus), true);
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
                statIOs[s].DomLevelLockedZero = false;
                statIOs[s].Input = statValues[s];
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

            LoadMultipliersFromTestCase(tcc.testCase);

            var watch = Stopwatch.StartNew();
            ExtractLevels(true);
            watch.Stop();

            if (tcc != null)
            {
                bool success = extractor.validResults;
                if (!success)
                    tcc.setTestResult(false, (int)watch.ElapsedMilliseconds, 0, "extraction failed");
                else
                {
                    string testText = "";
                    // test if the expected levels are possible
                    int resultCount = -Values.STATS_COUNT; // one result per stat is allowed, only count the additional ones. // TODO only consider possible stats
                    for (int s = 0; s < Values.STATS_COUNT; s++)
                    {
                        resultCount += extractor.results[s].Count;
                        bool statValid = false;
                        for (int r = 0; r < extractor.results[s].Count; r++)
                        {
                            if (extractor.results[s][r].levelWild == -1 || s == (int)StatNames.SpeedMultiplier && extractor.results[s][r].levelWild == 0 || extractor.results[s][r].levelWild == tcc.testCase.levelsWild[s]
                                    && extractor.results[s][r].levelDom == tcc.testCase.levelsDom[s]
                                    && (extractor.results[s][r].TE.Max == -1 || extractor.results[s][r].TE.Includes(tcc.testCase.tamingEff))
                            )
                            {
                                statValid = true;
                                break;
                            }
                        }
                        if (!statValid)
                        {
                            success = false;
                            testText = Utils.statName(s, true) + " not expected value";
                            break;
                        }
                    }
                    tcc.setTestResult(success, (int)watch.ElapsedMilliseconds, resultCount, testText);
                }
            }
            if (gotoExtractor) tabControlMain.SelectedTab = tabPageExtractor;
        }

        private void LoadMultipliersFromTestCase(testCases.ExtractionTestCase etc)
        {
            // set all stat-multipliers from testcase
            creatureCollection.serverMultipliers = etc.serverMultipliers.Copy(true);
            creatureCollection.singlePlayerSettings = etc.singleplayerSettings;
            creatureCollection.allowMoreThanHundredImprinting = etc.allowMoreThanHundredPercentImprinting;
            creatureCollection.maxWildLevel = etc.maxWildLevel;

            if (Values.V.loadedModsHash == 0 || Values.V.loadedModsHash != etc.modListHash)
                LoadStatAndKibbleValues(false); // load original multipliers if they were changed

            if (etc.ModIDs.Count > 0)
                LoadModValueFiles(Values.V.modsManifest.modsByFiles.Where(mi => etc.ModIDs.Contains(mi.Value.mod.id)).Select(mi => mi.Value.mod.FileName).ToList(),
                    false, false, out _);

            Values.V.ApplyMultipliers(creatureCollection);
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
                etc.ModIDs = creatureCollection.modIDs?.ToList();
                etc.serverMultipliers = creatureCollection.serverMultipliers;
                etc.Species = speciesSelector1.SelectedSpecies;
                etc.singleplayerSettings = creatureCollection.singlePlayerSettings;
                etc.allowMoreThanHundredPercentImprinting = creatureCollection.allowMoreThanHundredImprinting;
                etc.maxWildLevel = creatureCollection.maxWildLevel;

                double[] statValues = new double[Values.STATS_COUNT];
                for (int s = 0; s < Values.STATS_COUNT; s++)
                    statValues[s] = statIOs[s].Input;
                etc.statValues = statValues;

                extractionTestControl1.AddTestCase(etc);
                tabControlMain.SelectedTab = tabPageExtractionTests;
            }
        }

        private void copyToMultiplierTesterToolStripButton_Click(object sender, EventArgs e)
        {
            double[] statValues = new double[Values.STATS_COUNT];
            for (int s = 0; s < Values.STATS_COUNT; s++)
                statValues[s] = statIOs[s].Input;

            bool fromExtractor = tabControlMain.SelectedTab == tabPageExtractor;

            statsMultiplierTesting1.SetCreatureValues(statValues,
                GetCurrentWildLevels(false),
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
            Values.V.ApplyMultipliers(creatureCollection);
            SetCollectionChanged(true);
        }

        private void openFolderOfCurrentFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFileName)) return;
            string path = Path.GetDirectoryName(currentFileName);
            if (string.IsNullOrEmpty(path)) return;
            Process.Start(path);
        }

        private void customStatOverridesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var frm = new mods.CustomStatOverridesEditor(Values.V.species, creatureCollection))
            {
                frm.ShowDialog();
                SetCollectionChanged(true);
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
                StringBuilder sb = new StringBuilder();
                for (int c = 0; c < 6; c++)
                {
                    if (cl[c] != 0) sb.Append($"setTargetDinoColor {c} {cl[c]}|");
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1); // remove pipe at the end
                    Clipboard.SetText(sb.ToString());
                }
                else
                    Clipboard.SetText(string.Empty);
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
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0) return;

            string filePath = files[0];
            string ext = Path.GetExtension(filePath).ToLower();
            if (File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
            {
                ShowExportedCreatureListControl();
                exportedCreatureList.loadFilesInFolder(filePath);
            }
            else if (ext == ".ini")
            {
                ExtractExportedFileInExtractor(filePath);
            }
            else if (ext == ".asb")
            {
                if (!collectionDirty
                    || MessageBox.Show("Your Creature Collection has been modified since it was last saved, " +
                            "are you sure you want to discard your changes and load the file without saving first?",
                            "Discard Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    LoadCollectionFile(filePath);
                }
            }
            else
                DoOCR(files[0]);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CopySelectedCreatureName();
        }

        /// <summary>
        /// Copies the name of the currently selected creature to the clipboard.
        /// </summary>
        private void CopySelectedCreatureName()
        {
            if (listViewLibrary.SelectedItems.Count > 0)
                Clipboard.SetText(((Creature)listViewLibrary.SelectedItems[0].Tag).name);
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

            List<Creature> sameSpecies = null;

            listViewLibrary.BeginUpdate();
            for (int s = 0; s < listViewLibrary.SelectedItems.Count; s++)
            {
                Creature cr = ((Creature)listViewLibrary.SelectedItems[s].Tag);

                if (sameSpecies == null || sameSpecies[0].Species != cr.Species)
                    sameSpecies = creatureCollection.creatures.Where(c => c.Species == cr.Species).ToList();

                // set new name
                cr.name = NamePatterns.GenerateCreatureName(cr, sameSpecies, customReplacingsNamingPattern, false);

                UpdateDisplayedCreatureValues(cr, false);
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
                UpdateDisplayedCreatureValues(cr, false);
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
                MessageBox.Show($"Folder not found\n{FileService.GetJsonPath()}\n\nException: {ex.Message}", "No data folder for ASB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Reloads the file for the custom replacings in the naming patterns.
        /// </summary>
        private void ReloadNamePatternCustomReplacings(PatternEditor pe = null)
        {
            string filePath = FileService.GetJsonPath(FileService.CustomReplacingsNamePattern);
            if (!FileService.LoadJSONFile(filePath, out customReplacingsNamingPattern, out string error))
            {
                if (!string.IsNullOrEmpty(error))
                    MessageBox.Show(error, "ASB Custom replacings file loading error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (pe != null) pe.SetCustomReplacings(customReplacingsNamingPattern);
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
    }
}
