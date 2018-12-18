using ARKBreedingStats.duplicates;
using ARKBreedingStats.miscClasses;
using ARKBreedingStats.ocr;
using ARKBreedingStats.settings;
using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ARKBreedingStats
{
    public partial class Form1 : Form
    {
        private CreatureCollection creatureCollection = new CreatureCollection();
        private string currentFileName = "";
        private bool collectionDirty;
        private readonly Dictionary<string, int[]> topLevels = new Dictionary<string, int[]>(); // list of top stats of all creatures per species
        private readonly List<StatIO> statIOs = new List<StatIO>();
        private readonly List<StatIO> testingIOs = new List<StatIO>();
        private int activeStat = -1;
        private readonly bool[] activeStats = { true, true, true, true, true, true, true, true }; // stats used by the creature (some don't use oxygen)
        private bool pedigreeNeedsUpdate;
        private bool libraryNeedsUpdate;

        public delegate void LevelChangedEventHandler(StatIO s);
        public delegate void InputValueChangedEventHandler(StatIO s);
        public delegate void collectionChangedEventHandler(bool changed = true, string species = "0"); // if "0" is passed as species, breeding-related controls are not updated
        public delegate void setSpeciesIndexEventHandler(int speciesIndex);
        public delegate void setSpeciesNameEventHandler(string speciesName);
        public delegate void setMessageLabelTextEventHandler(string text, MessageBoxIcon icon);

        private bool updateTorporInTester, filterListAllowed;
        private readonly bool[] considerStatHighlight = { true, true, false, false, true, true, false, false }; // consider this stat for color-highlighting, topness etc
        private bool autoSave;
        private DateTime lastAutoSaveBackup = DateTime.Now.AddDays(-1);
        private int autoSaveMinutes;
        private Creature creatureTesterEdit;
        private int hiddenLevelsCreatureTester;
        private FileSync fileSync;
        private readonly Extraction extractor = new Extraction();
        private bool oxygenForAll;
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

        // OCR stuff
        public ARKOverlay overlay;
        private static double[] lastOCRValues;
        private int lastOCRSpecies;

        public Form1()
        {
            // load settings of older version if possible after an upgrade
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
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

            pedigree1.EditCreature += editCreatureInTester;
            pedigree1.BestBreedingPartners += showBestBreedingPartner;
            pedigree1.exportToClipboard += exportAsTextToClipboard;
            breedingPlan1.EditCreature += editCreatureInTester;
            breedingPlan1.createIncubationTimer += createIncubationTimer;
            breedingPlan1.BestBreedingPartners += showBestBreedingPartner;
            breedingPlan1.exportToClipboard += exportAsTextToClipboard;
            breedingPlan1.setMessageLabelText += setMessageLabelText;
            breedingPlan1.setSpeciesName += setSpeciesName;
            timerList1.onTimerChange += setCollectionChanged;
            breedingPlan1.bindSubControlEvents();
            raisingControl1.onChange += setCollectionChanged;
            creatureBoxListView.EditCreature += editBoxCreatureInTester;
            tamingControl1.CreateTimer += createTimer;
            raisingControl1.extractBaby += extractBaby;
            raisingControl1.setSpeciesIndex += setSpeciesIndex;
            raisingControl1.timerControl = timerList1;
            notesControl1.changed += setCollectionChanged;
            creatureInfoInputExtractor.CreatureDataRequested += CreatureInfoInput_CreatureDataRequested;
            creatureInfoInputTester.CreatureDataRequested += CreatureInfoInput_CreatureDataRequested;
            speciesSelector1.onSpeciesChanged += SpeciesSelector1_onSpeciesChanged;
            statsMultiplierTesting1.OnApplyMultipliers += StatsMultiplierTesting1_OnApplyMultipliers;

            speciesSelector1.SetTextBox(tbSpeciesGlobal);

            ArkOCR.OCR.setOCRControl(ocrControl1);
            ocrControl1.updateWhiteThreshold += OcrupdateWhiteThreshold;
            ocrControl1.dragEnter += testEnteredDrag;
            ocrControl1.dragDrop += fileDropedOnExtractor;

            settingsToolStripMenuItem.ShortcutKeyDisplayString = new KeysConverter().ConvertTo(Keys.Control, typeof(string))?.ToString().Replace("None", ",");

            timerGlobal.Interval = 1000;
            timerGlobal.Tick += TimerGlobal_Tick;

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
                for (int c = 0; c < cw.Length; c++)
                    listViewLibrary.Columns[c].Width = cw[c];
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
                for (int i = 0; i < custWs.Length; i++)
                {
                    if (i < custWd.Length)
                        custW.Add(custWs[i], custWd[i]);
                }
            }
            breedingPlan1.statWeighting.CustomWeightings = custW;
            // last set values are saved at the end of the customweightings
            if (custWs != null && custWd != null && custWd.Length > custWs.Length)
                breedingPlan1.statWeighting.Values = custWd[custWs.Length];

            autoSave = Properties.Settings.Default.autosave;
            autoSaveMinutes = Properties.Settings.Default.autosaveMinutes;

            // load weapondamages
            tamingControl1.weaponDamages = Properties.Settings.Default.weaponDamages;
            tamingControl1.weaponDamagesEnabled = Properties.Settings.Default.weaponDamagesEnabled;

            statIOs.Add(statIOHealth);
            statIOs.Add(statIOStamina);
            statIOs.Add(statIOOxygen);
            statIOs.Add(statIOFood);
            statIOs.Add(statIOWeight);
            statIOs.Add(statIODamage);
            statIOs.Add(statIOSpeed);
            statIOs.Add(statIOTorpor);
            testingIOs.Add(statTestingHealth);
            testingIOs.Add(statTestingStamina);
            testingIOs.Add(statTestingOxygen);
            testingIOs.Add(statTestingFood);
            testingIOs.Add(statTestingWeight);
            testingIOs.Add(statTestingDamage);
            testingIOs.Add(statTestingSpeed);
            testingIOs.Add(statTestingTorpor);
            for (int s = 0; s < 8; s++)
            {
                statIOs[s].Title = Utils.statName(s);
                testingIOs[s].Title = Utils.statName(s);
                if (Utils.precision(s) == 3)
                {
                    statIOs[s].Percent = true;
                    testingIOs[s].Percent = true;
                }
                statIOs[s].statIndex = s;
                testingIOs[s].statIndex = s;
                testingIOs[s].LevelChanged += testingStatIOValueUpdate;
                testingIOs[s].InputType = StatIOInputType.LevelsInputType;
                statIOs[s].InputValueChanged += statIOQuickWildLevelCheck;
                considerStatHighlight[s] = (Properties.Settings.Default.consideredStats & (1 << s)) > 0;
                checkedListBoxConsiderStatTop.SetItemChecked(s, considerStatHighlight[s]);
            }
            statIOTorpor.ShowBarAndLock = false; // torpor should not show bar, it get's too wide and is not interesting for breeding
            statTestingTorpor.ShowBarAndLock = false;

            breedingPlan1.MutationLimit = Properties.Settings.Default.MutationLimitBreedingPlanner;

            // enable 0-lock for dom-levels of oxygen, food (most often they are not leveld up)
            statIOs[2].DomLevelLockedZero = true;
            statIOs[3].DomLevelLockedZero = true;

            initializeCollection();
            filterListAllowed = true;

            // Set up the file watcher
            fileSync = new FileSync(currentFileName, collectionChanged);

            if (Values.V.loadValues() && Values.V.speciesNames.Count > 0)
            {
                // load last save file:
                if (Properties.Settings.Default.LastSaveFile == "" || !loadCollectionFile(Properties.Settings.Default.LastSaveFile))
                    newCollection();

                for (int s = 0; s < 8; s++)
                {
                    statIOs[s].Input = 0;
                }
            }
            else
            {
                MessageBox.Show("The values-file couldn't be loaded, this application does not work without. Try redownloading the tool.",
                        "Error: Values-file not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }

            if (!Kibbles.K.loadValues())
            {
                MessageBox.Show("The kibbles-file couldn't be loaded, the kibble-recipes will not be available. " +
                        "You can redownload the tool to get this file.", "Error: Kibble-file not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            speciesSelector1.setSpeciesLists(Values.V.speciesNames, Values.V.speciesWithAliasesList);
            speciesSelector1.LastSpecies = Properties.Settings.Default.lastSpecies;
            speciesSelector1.lastTabPage = tabPageExtractor;

            if (Properties.Settings.Default.lastSpecies != null && Properties.Settings.Default.lastSpecies.Length > 0 && Values.V.speciesNames.IndexOf(Properties.Settings.Default.lastSpecies[0]) >= 0)
            {
                speciesSelector1.setSpecies(Properties.Settings.Default.lastSpecies[0]);
                tamingControl1.setSpeciesIndex(speciesSelector1.speciesIndex);
            }
            else if (Values.V.speciesNames.Count > 0)
                speciesSelector1.setSpecies(Values.V.speciesNames[0]);

            // OCR
            ocrControl1.Initialize();

            // initialize speech recognition if enabled
            if (Properties.Settings.Default.SpeechRecognition)
            {
                // var speechRecognitionAvailable = (AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.Substring(0, 13) == "System.Speech")); // TODO doens't work as intended. Should only require System.Speech if available to allow running it on MONO

                speechRecognition = new SpeechRecognition(creatureCollection.maxWildLevel, creatureCollection.considerWildLevelSteps ? creatureCollection.wildLevelStep : 1, Values.V.speciesWithAliasesList, lbListening);
                speechRecognition.speechRecognized += tellTamingData;
                speechRecognition.speechCommandRecognized += speechCommand;
            }
            else lbListening.Visible = false;

            // default owner and tribe
            creatureInfoInputExtractor.CreatureOwner = Properties.Settings.Default.DefaultOwnerName;
            creatureInfoInputExtractor.CreatureTribe = Properties.Settings.Default.DefaultTribeName;
            creatureInfoInputExtractor.OwnerLock = Properties.Settings.Default.OwnerNameLocked;
            creatureInfoInputExtractor.TribeLock = Properties.Settings.Default.TribeNameLocked;

            clearAll();
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
                extractionTestControl1.loadExtractionTestCases(Properties.Settings.Default.LastSaveFileTestCases);
            }

            // set TLS-protocol (github needs at least TLS 1.2) for update-check
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            // check for updates
            if (DateTime.Now.AddHours(-12) > Properties.Settings.Default.lastUpdateCheck)
                checkForUpdates(true);

            timerGlobal.Start();
        }

        private void setSpeciesIndex(int speciesIndex)
        {
            speciesSelector1.setSpeciesIndex(speciesIndex);
        }

        private void setSpeciesName(string speciesName)
        {
            speciesSelector1.setSpecies(speciesName);
        }

        private void tellTamingData(string species, int level)
        {
            speciesSelector1.setSpecies(species);
            int sI = speciesSelector1.speciesIndex;
            if (sI >= 0 && Values.V.species[sI].taming != null &&
                    Values.V.species[sI].taming.eats != null &&
                    Values.V.species[sI].taming.eats.Count > 0)
            {
                tamingControl1.setLevel(level, false);
                tamingControl1.setSpeciesIndex(sI);
                overlay?.setInfoText($"{species} ({Loc.s("level")} {level}):\n{tamingControl1.quickTamingInfos}");
            }
        }

        private void speechCommand(SpeechRecognition.Commands command)
        {
            if (command == SpeechRecognition.Commands.Extract)
                doOCR();
        }

        delegate void collectionChangedCallback();

        private void collectionChanged()
        {
            if (creatureBoxListView.InvokeRequired)
            {
                collectionChangedCallback d = collectionChanged;
                Invoke(d);
            }
            else
            {
                loadCollectionFile(currentFileName, true, true);
            }
        }

        private void clearAll(bool clearExtraCreatureData = true)
        {
            extractor.Clear();
            listViewPossibilities.Items.Clear();
            for (int s = 0; s < 8; s++)
            {
                statIOs[s].Clear();
            }
            extractionFailed(); // set background of controls to neutral
            labelFootnote.Text = "";
            labelFootnote.BackColor = Color.Transparent;
            labelTE.Text = "";
            activeStat = -1;
            lbSumDom.Text = "";
            lbSumWild.Text = "";
            lbSumDomSB.Text = "";
            updateTorporInTester = true;
            creatureInfoInputExtractor.ButtonEnabled = false;
            groupBoxPossibilities.Visible = false;
            groupBoxRadarChartExtractor.Visible = false;
            lbInfoYellowStats.Visible = false;
            button2TamingCalc.Visible = cbQuickWildCheck.Checked;
            groupBoxTamingInfo.Visible = false;
            setMessageLabelText();
            if (clearExtraCreatureData)
            {
                creatureInfoInputExtractor.MutationCounterMother = 0;
                creatureInfoInputExtractor.MutationCounterFather = 0;
                creatureInfoInputExtractor.domesticatedAt = DateTime.Now;
                creatureInfoInputExtractor.parentListValid = false;
                creatureInfoInputExtractor.CreatureGuid = Guid.Empty;
                exportedCreatureControl = null;
            }
        }

        private void buttonExtract_Click(object sender, EventArgs e)
        {
            extractLevels();
        }

        private bool extractLevels(bool autoExtraction = false)
        {
            SuspendLayout();
            int activeStatKeeper = activeStat;
            clearAll(clearExtractionCreatureData);

            if (cbExactlyImprinting.Checked)
                extractor.possibleIssues |= IssueNotes.Issue.ImprintingLocked;

            extractor.extractLevels(speciesSelector1.speciesIndex, (int)numericUpDownLevel.Value, statIOs,
                    (double)numericUpDownLowerTEffBound.Value / 100, (double)numericUpDownUpperTEffBound.Value / 100,
                    !rbBredExtractor.Checked, rbTamedExtractor.Checked, rbBredExtractor.Checked,
                    (double)numericUpDownImprintingBonusExtractor.Value / 100, !cbExactlyImprinting.Checked,
                    creatureCollection.allowMoreThanHundredImprinting, creatureCollection.imprintingMultiplier,
                    Values.V.babyCuddleIntervalMultiplier,
                    creatureCollection.considerWildLevelSteps, creatureCollection.wildLevelStep, out bool imprintingBonusChanged);

            numericUpDownImprintingBonusExtractor.ValueSave = (decimal)extractor.imprintingBonus * 100;
            numericUpDownImprintingBonusExtractor_ValueChanged(null, null);

            if (imprintingBonusChanged && !autoExtraction)
            {
                extractor.possibleIssues |= IssueNotes.Issue.ImprintingNotPossible;
            }

            bool everyStatHasAtLeastOneResult = extractor.EveryStatHasAtLeastOneResult;

            // remove all results that require a total wild-level higher than the max
            // Tek-variants have 20% higher levels
            extractor.RemoveImpossibleTEsAccordingToMaxWildLevel((int)Math.Ceiling(creatureCollection.maxWildLevel * (speciesSelector1.species.StartsWith("Tek ") ? 1.2 : 1)));

            if (everyStatHasAtLeastOneResult && !extractor.EveryStatHasAtLeastOneResult)
            {
                MessageBox.Show(string.Format(Loc.s("issueMaxWildLevelTooLow"), creatureCollection.maxWildLevel));
            }

            if (!extractor.setStatLevelBoundsAndFilter(out int statIssue))
            {
                if (statIssue == -1)
                {
                    extractionFailed(IssueNotes.Issue.WildTamedBred
                            | IssueNotes.Issue.CreatureLevel
                            | (rbTamedExtractor.Checked ? IssueNotes.Issue.TamingEffectivenessRange : IssueNotes.Issue.None));
                    ResumeLayout();
                    return false;
                }
                extractor.possibleIssues |= IssueNotes.Issue.Typo | IssueNotes.Issue.CreatureLevel;
                statIOs[statIssue].Status = StatIOStatus.Error;
                statIOs[7].Status = StatIOStatus.Error;
            }

            // get mean-level (most probable for the wild levels)
            // TODO handle species without wild levels in speed better (some flyers)
            double meanWildLevel = Math.Round((double)extractor.levelWildSum / 7, 1);
            bool nonUniqueStats = false;

            for (int s = 0; s < 8; s++)
            {
                if (extractor.results[s].Count > 0)
                {
                    // choose the most probable wild-level, aka the level nearest to the mean of the wild levels.
                    int r = 0;
                    for (int b = 1; b < extractor.results[s].Count; b++)
                    {
                        if (Math.Abs(meanWildLevel - extractor.results[s][b].levelWild) < Math.Abs(meanWildLevel - extractor.results[s][r].levelWild)) r = b;
                    }

                    setLevelCombination(s, r);
                    if (extractor.results[s].Count > 1)
                    {
                        statIOs[s].Status = StatIOStatus.Nonunique;
                        nonUniqueStats = true;
                    }
                    else
                    {
                        statIOs[s].Status = StatIOStatus.Unique;
                    }
                }
                else
                {
                    // no results for this stat
                    statIOs[s].Status = StatIOStatus.Error;
                    extractor.validResults = false;
                    if (rbTamedExtractor.Checked && extractor.statsWithTE.Contains(s))
                    {
                        extractor.possibleIssues |= IssueNotes.Issue.TamingEffectivenessRange;
                    }
                }
            }
            if (!extractor.validResults)
            {
                extractionFailed(IssueNotes.Issue.Typo | IssueNotes.Issue.WildTamedBred | IssueNotes.Issue.LockedDom |
                        IssueNotes.Issue.OutdatedIngameValues | IssueNotes.Issue.ImprintingNotUpdated |
                        (statIOTorpor.LevelWild >= (int)numericUpDownLevel.Value ? IssueNotes.Issue.CreatureLevel : IssueNotes.Issue.None));
                ResumeLayout();
                return false;
            }
            extractor.uniqueResults = !nonUniqueStats;
            if (!extractor.uniqueResults)
            {
                groupBoxPossibilities.Visible = true;
                lbInfoYellowStats.Visible = true;
            }

            // if damage (s==5) has a possibility for the dom-levels to make it a valid sum, take this
            int domLevelsChosenSum = 0;
            for (int s = 0; s < 7; s++)
            {
                domLevelsChosenSum += extractor.results[s][extractor.chosenResults[s]].levelDom;
            }
            if (domLevelsChosenSum != extractor.levelDomSum)
            {
                // sum of domlevels is not correct. Try to find another combination
                domLevelsChosenSum -= extractor.results[5][extractor.chosenResults[5]].levelDom;
                bool changeChosenResult = false;
                int cR = 0;
                for (int r = 0; r < extractor.results[5].Count; r++)
                {
                    if (domLevelsChosenSum + extractor.results[5][r].levelDom == extractor.levelDomSum)
                    {
                        cR = r;
                        changeChosenResult = true;
                        break;
                    }
                }
                if (changeChosenResult)
                    setLevelCombination(5, cR);
            }

            if (extractor.postTamed) setUniqueTE();
            else
            {
                labelTE.Text = Loc.s("notYetTamed");
                labelTE.BackColor = Color.Transparent;
            }

            setWildSpeedLevelAccordingToOthers();

            lbSumDomSB.Text = extractor.levelDomSum.ToString();
            showSumOfChosenLevels();
            showStatsInOverlay();

            setActiveStat(activeStatKeeper);

            if (!extractor.postTamed)
            {
                labelFootnote.Text = Loc.s("lbNotYetTamed");
                button2TamingCalc.Visible = true;

                // display taming info
                if (cbQuickWildCheck.Checked)
                    tamingControl1.setLevel(statIOs[7].LevelWild + 1);
                else
                    tamingControl1.setLevel((int)numericUpDownLevel.Value);
                labelTamingInfo.Text = tamingControl1.quickTamingInfos;
                groupBoxTamingInfo.Visible = true;
            }

            ResumeLayout();
            return true;
        }

        private void extractionFailed(IssueNotes.Issue issues = IssueNotes.Issue.None)
        {
            issues |= extractor.possibleIssues; // add all issues that arised during extraction
            if (issues == IssueNotes.Issue.None)
            {
                // set background of inputs to neutral
                numericUpDownLevel.BackColor = SystemColors.Window;
                numericUpDownLowerTEffBound.BackColor = SystemColors.Window;
                numericUpDownUpperTEffBound.BackColor = SystemColors.Window;
                numericUpDownImprintingBonusExtractor.BackColor = SystemColors.Window;
                cbExactlyImprinting.BackColor = Color.Transparent;
                panelSums.BackColor = Color.Transparent;
                panelWildTamedBred.BackColor = Color.Transparent;
                labelTE.BackColor = Color.Transparent;
                llOnlineHelpExtractionIssues.Visible = false;
                labelErrorHelp.Visible = false;
                labelImprintingFailInfo.Visible = false; // TODO move imprinting-fail to upper note-info
                extractor.possibleIssues = IssueNotes.Issue.None;
            }
            else
            {
                // highlight controls which most likely need to be checked to solve the issue
                if (issues.HasFlag(IssueNotes.Issue.WildTamedBred))
                    panelWildTamedBred.BackColor = Color.LightSalmon;
                if (issues.HasFlag(IssueNotes.Issue.TamingEffectivenessRange))
                {
                    if (numericUpDownLowerTEffBound.Value > 0)
                        numericUpDownLowerTEffBound.BackColor = Color.LightSalmon;
                    if (numericUpDownUpperTEffBound.Value < 100)
                        numericUpDownUpperTEffBound.BackColor = Color.LightSalmon;
                    if (numericUpDownLowerTEffBound.Value == 0 && numericUpDownUpperTEffBound.Value == 100)
                        issues -= IssueNotes.Issue.TamingEffectivenessRange;
                }
                if (issues.HasFlag(IssueNotes.Issue.CreatureLevel))
                {
                    numericUpDownLevel.BackColor = Color.LightSalmon;
                    numericUpDownImprintingBonusExtractor.BackColor = Color.LightSalmon;
                    statIOTorpor.Status = StatIOStatus.Error;
                }
                if (issues.HasFlag(IssueNotes.Issue.ImprintingLocked))
                    cbExactlyImprinting.BackColor = Color.LightSalmon;
                if (issues.HasFlag(IssueNotes.Issue.ImprintingNotPossible))
                    numericUpDownImprintingBonusExtractor.BackColor = Color.LightSalmon;

                // don't show some issue notes if the input is not wrong
                if (issues.HasFlag(IssueNotes.Issue.LockedDom))
                {
                    bool oneStatIsDomLocked = false;
                    for (int s = 0; s < 8; s++)
                    {
                        if (statIOs[s].DomLevelLockedZero)
                        {
                            oneStatIsDomLocked = true;
                            break;
                        }
                    }
                    if (!oneStatIsDomLocked)
                    {
                        // no stat is domLocked, remove this note (which is ensured to be there)
                        issues -= IssueNotes.Issue.LockedDom;
                    }
                }

                if (!issues.HasFlag(IssueNotes.Issue.StatMultipliers))
                    issues |= IssueNotes.Issue.StatMultipliers; // add this always?

                labelErrorHelp.Text = "The extraction failed. See the following list of possible causes:\n\n" +
                        IssueNotes.getHelpTexts(issues);
                labelErrorHelp.Visible = true;
                llOnlineHelpExtractionIssues.Visible = true;
                groupBoxPossibilities.Visible = false;
                groupBoxRadarChartExtractor.Visible = false;
                lbInfoYellowStats.Visible = false;
                if (rbBredExtractor.Checked && numericUpDownImprintingBonusExtractor.Value > 0)
                {
                    labelImprintingFailInfo.Text = "If the creature is imprinted the extraction may fail because the game sometimes \"forgets\" " +
                            "to increase some stat-values during the imprinting-process. Usually it works after a server-restart.";
                    labelImprintingFailInfo.Visible = true;
                }
                else if (rbTamedExtractor.Checked && "Procoptodon,Pulmonoscorpius,Troodon".Split(',').ToList().Contains(speciesSelector1.species))
                {
                    // creatures that display wrong stat-values after taming
                    labelImprintingFailInfo.Text = $"The {speciesSelector1.species} is known for displaying wrong stat-values after taming. " +
                            "Please try the extraction again after the server restarted.";
                    labelImprintingFailInfo.Visible = true;
                }
                toolStripButtonSaveCreatureValuesTemp.Visible = true;

                // check for updates
                if (DateTime.Now.AddHours(-5) > Properties.Settings.Default.lastUpdateCheck)
                    checkForUpdates(true);
            }
        }

        private void setUniqueTE()
        {
            double te = extractor.uniqueTE();
            if (te >= 0)
            {
                labelTE.Text = $"Extracted: {Math.Round(100 * te, 2)} %";
                if (rbTamedExtractor.Checked && extractor.postTamed)
                    labelTE.Text += $" (wildlevel: {Math.Ceiling(Math.Round((statIOs[7].LevelWild + 1) / (1 + te / 2), 6))})";
                labelTE.BackColor = Color.Transparent;
            }
            else
            {
                if (te == -1)
                {
                    labelTE.Text = "TE differs in chosen possibilities";
                    labelTE.BackColor = Color.LightSalmon;
                }
                else
                {
                    labelTE.Text = "TE unknown";
                }
            }
        }

        private void showSumOfChosenLevels()
        {
            // this displays the sum of the chosen levels. This is the last step before a creature-extraction is considered as valid or not valid.
            // The speedlevel is not chosen, but calculated from the other chosen levels, and must not be included in the sum, except all the other levels are determined uniquely!

            // this function will show only the offset of the value, it's less confusing to the user and gives all the infos needed
            int sumW = 0, sumD = 0;
            bool valid = true, inbound = true, allUnique = true;
            for (int s = 0; s < 7; s++)
            {
                if (extractor.results[s].Count > extractor.chosenResults[s])
                {
                    sumW += statIOs[s].LevelWild > 0 ? statIOs[s].LevelWild : 0;
                    sumD += statIOs[s].LevelDom;
                    if (extractor.results[s].Count != 1)
                    {
                        allUnique = false;
                    }
                }
                else
                {
                    valid = false;
                    break;
                }
                statIOs[s].TopLevel = StatIOStatus.Neutral;
            }
            if (valid)
            {
                sumW -= allUnique || statIOs[6].LevelWild < 0 ? 0 : statIOs[6].LevelWild;
                string offSetWild = "✓";
                lbSumDom.Text = sumD.ToString();
                if (sumW <= extractor.levelWildSum)
                {
                    lbSumWild.ForeColor = SystemColors.ControlText;
                }
                else
                {
                    lbSumWild.ForeColor = Color.Red;
                    offSetWild = "+" + (sumW - extractor.levelWildSum);
                    inbound = false;
                }
                if (sumD == extractor.levelDomSum)
                {
                    lbSumDom.ForeColor = SystemColors.ControlText;
                }
                else
                {
                    lbSumDom.ForeColor = Color.Red;
                    inbound = false;
                }
                lbSumWild.Text = offSetWild;
            }
            else
            {
                lbSumWild.Text = "n/a";
                lbSumDom.Text = "n/a";
            }
            panelSums.BackColor = inbound ? SystemColors.Control : Color.FromArgb(255, 200, 200);

            bool torporLevel = numericUpDownLevel.Value > statIOs[7].LevelWild;
            if (!torporLevel)
            {
                numericUpDownLevel.BackColor = Color.LightSalmon;
                statIOs[7].Status = StatIOStatus.Error;
            }

            bool allValid = valid && inbound && torporLevel && extractor.validResults;
            if (allValid)
            {
                radarChartExtractor.setLevels(statIOs.Select(s => s.LevelWild).ToArray());
                toolStripButtonSaveCreatureValuesTemp.Visible = false;
                cbExactlyImprinting.BackColor = Color.Transparent;
                if (topLevels.ContainsKey(speciesSelector1.species))
                {
                    for (int s = 0; s < 7; s++)
                    {
                        if (statIOs[s].LevelWild > 0)
                        {
                            if (statIOs[s].LevelWild == topLevels[speciesSelector1.species][s])
                                statIOs[s].TopLevel = StatIOStatus.TopLevel;
                            else if (statIOs[s].LevelWild > topLevels[speciesSelector1.species][s])
                                statIOs[s].TopLevel = StatIOStatus.NewTopLevel;
                        }
                    }
                }
            }
            creatureInfoInputExtractor.ButtonEnabled = allValid;
            groupBoxRadarChartExtractor.Visible = allValid;
        }

        private void radioButtonWild_CheckedChanged(object sender, EventArgs e)
        {
            if (rbWildExtractor.Checked)
                updateExtrDetails();
        }

        private void radioButtonTamed_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTamedExtractor.Checked)
                updateExtrDetails();
        }

        private void radioButtonBred_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBredExtractor.Checked)
                updateExtrDetails();
        }

        private void updateExtrDetails()
        {
            panelExtrTE.Visible = rbTamedExtractor.Checked;
            panelExtrImpr.Visible = rbBredExtractor.Checked;
            groupBoxDetailsExtractor.Visible = !rbWildExtractor.Checked;
            if (rbTamedExtractor.Checked)
            {
                groupBoxDetailsExtractor.Text = "Taming-Effectiveness";
            }
            else if (rbBredExtractor.Checked)
            {
                groupBoxDetailsExtractor.Text = "Imprinting-Quality";
            }
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

        private void updateTesterDetails()
        {
            setTesterInputsTamed(!rbWildTester.Checked);
            NumericUpDownTestingTE.Enabled = rbTamedTester.Checked;
            labelTesterTE.Enabled = rbTamedTester.Checked;
            numericUpDownImprintingBonusTester.Enabled = rbBredTester.Checked;
            labelImprintingTester.Enabled = rbBredTester.Checked;
            lbImprintedCount.Enabled = rbBredTester.Checked;

            updateAllTesterValues();
        }

        private void statIO_Click(object sender, EventArgs e)
        {
            StatIO se = (StatIO)sender;
            if (se != null)
            {
                setActiveStat(statIOs.IndexOf(se));
            }
        }

        // when clicking on a stat show the possibilites in the listbox
        private void setActiveStat(int stat)
        {
            if (stat != activeStat)
            {
                activeStat = -1;
                listViewPossibilities.BeginUpdate();
                listViewPossibilities.Items.Clear();
                for (int s = 0; s < 8; s++)
                {
                    if (s == stat && statIOs[s].Status == StatIOStatus.Nonunique)
                    {
                        statIOs[s].Selected = true;
                        setPossibilitiesListview(s);
                        activeStat = stat;
                    }
                    else
                    {
                        statIOs[s].Selected = false;
                    }
                }
                listViewPossibilities.EndUpdate();
            }
        }

        // fill listbox with possible results of stat
        private void setPossibilitiesListview(int s)
        {
            if (s < extractor.results.Length)
            {
                bool resultsValid = extractor.filterResultsByFixed(s) == -1;
                List<string> subItems = new List<string>();
                for (int r = 0; r < extractor.results[s].Count; r++)
                {
                    subItems.Clear();
                    double te = Math.Round(extractor.results[s][r].TE.Mean, 4);
                    subItems.Add(extractor.results[s][r].levelWild.ToString());
                    subItems.Add(extractor.results[s][r].levelDom.ToString());
                    subItems.Add(te >= 0 ? (te * 100).ToString() : "");

                    subItems.Add(te > 0 ? Math.Ceiling((extractor.levelWildSum + 1) / (1 + te / 2)).ToString() : "");

                    ListViewItem lvi = new ListViewItem(subItems.ToArray());
                    if (!resultsValid || extractor.results[s][r].currentlyNotValid)
                        lvi.BackColor = Color.LightSalmon;
                    if (extractor.fixedResults[s] && extractor.chosenResults[s] == r)
                    {
                        lvi.BackColor = Color.LightSkyBlue;
                    }

                    lvi.Tag = r;

                    listViewPossibilities.Items.Add(lvi);
                }
            }
        }

        private void tbSpeciesGlobal_Click(object sender, EventArgs e)
        {
            toggleViewSpeciesSelector(true);
        }

        private void tbSpeciesGlobal_Enter(object sender, EventArgs e)
        {
            toggleViewSpeciesSelector(true);
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
                toggleViewSpeciesSelector(false);
            }
        }

        private void toggleViewSpeciesSelector(bool showSpeciesSelector)
        {
            tabControlMain.Visible = !showSpeciesSelector;
            speciesSelector1.Visible = showSpeciesSelector;
        }

        private void tbSpeciesGlobal_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                speciesSelector1.setSpecies(tbSpeciesGlobal.Text);
            }
        }

        // global species changed / globalspecieschanged
        private void SpeciesSelector1_onSpeciesChanged()
        {
            clearExtractionCreatureData = true; // as soon as the user changes the species, it's assumed it's not an exported creature anymore
            tbSpeciesGlobal.Text = speciesSelector1.species;
            pbSpecies.Image = speciesSelector1.speciesImage();
            toggleViewSpeciesSelector(false);

            creatureInfoInputExtractor.SpeciesIndex = speciesSelector1.speciesIndex;
            creatureInfoInputTester.SpeciesIndex = speciesSelector1.speciesIndex;
            bool isglowSpecies = Values.V.glowSpecies.Contains(speciesSelector1.species);
            for (int s = 0; s < 8; s++)
            {
                activeStats[s] = Values.V.species[speciesSelector1.speciesIndex].stats[s].BaseValue > 0 && (s != 2 || !Values.V.species[speciesSelector1.speciesIndex].doesNotUseOxygen || oxygenForAll);
                statIOs[s].Enabled = activeStats[s];
                if (!activeStats[s]) statIOs[s].Input = 0;
                statIOs[s].Title = Utils.statName(s, false, glow: isglowSpecies);
                testingIOs[s].Title = Utils.statName(s, false, isglowSpecies);
                if (isglowSpecies && (s == 1 || s == 2 || s == 5))
                {
                    statIOs[s].DomLevelLockedZero = false;
                }
            }
            if (tabControlMain.SelectedTab == tabPageExtractor)
            {
                clearAll();
            }
            else if (tabControlMain.SelectedTab == tabPageStatTesting)
            {
                updateAllTesterValues();
                statPotentials1.speciesIndex = speciesSelector1.speciesIndex;
                statPotentials1.setLevels(testingIOs.Select(s => s.LevelWild).ToArray(), true);
                setTesterInfoInputCreature();
            }
            else if (tabControlMain.SelectedTab == tabPageTaming)
            {
                tamingControl1.setSpeciesIndex(speciesSelector1.speciesIndex);
            }
            else if (tabControlMain.SelectedTab == tabPageRaising)
            {
                raisingControl1.updateRaisingData(speciesSelector1.speciesIndex);
            }
            else if (tabControlMain.SelectedTab == tabPageMultiplierTesting)
            {
                statsMultiplierTesting1.setSpeciesIndex(speciesSelector1.speciesIndex);
            }
            else if (tabControlMain.SelectedTab == tabPageBreedingPlan)
            {
                if (breedingPlan1.CurrentSpecies == speciesSelector1.species)
                    breedingPlan1.updateIfNeeded();
                else
                {
                    breedingPlan1.setSpecies(speciesSelector1.species);
                }
            }

            hiddenLevelsCreatureTester = 0;
        }

        private void listViewPossibilities_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewPossibilities.SelectedIndices.Count > 0)
            {
                int index = (int)listViewPossibilities.SelectedItems[0].Tag;
                if (index >= 0 && activeStat >= 0)
                {
                    setLevelCombination(activeStat, index, true);
                    extractor.fixedResults[activeStat] = true;
                }
            }
            else if (activeStat >= 0)
                extractor.fixedResults[activeStat] = false;
        }

        private void setLevelCombination(int s, int i, bool validateCombination = false)
        {
            statIOs[s].LevelWild = extractor.results[s][i].levelWild;
            statIOs[s].LevelDom = extractor.results[s][i].levelDom;
            statIOs[s].BreedingValue = Stats.calculateValue(speciesSelector1.speciesIndex, s, extractor.results[s][i].levelWild, 0, true, 1, 0);
            extractor.chosenResults[s] = i;
            if (validateCombination)
            {
                setUniqueTE();
                setWildSpeedLevelAccordingToOthers();
                showSumOfChosenLevels();
            }
        }

        private void setWildSpeedLevelAccordingToOthers()
        {
            // wild speed level is wildTotalLevels - determinedWildLevels. sometimes the oxygenlevel cannot be determined as well
            bool unique = true;
            int notDeterminedLevels = statIOs[7].LevelWild;
            for (int s = 0; s < 6; s++)
            {
                if (statIOs[s].LevelWild >= 0)
                {
                    notDeterminedLevels -= statIOs[s].LevelWild;
                }
                else
                {
                    unique = false;
                    break;
                }
            }
            if (unique)
            {
                // if all other stats are unique, set speedlevel
                statIOs[6].LevelWild = Math.Max(0, notDeterminedLevels);
                statIOs[6].BreedingValue = Stats.calculateValue(speciesSelector1.speciesIndex, 6, statIOs[6].LevelWild, 0, true, 1, 0);
            }
            else
            {
                // if not all other levels are unique, set speed and not known levels to unknown
                for (int s = 0; s < 7; s++)
                {
                    if (s == 6 || !activeStats[s])
                    {
                        statIOs[s].LevelWild = -1;
                    }
                }
            }
        }

        private void CopyExtractionToClipboard()
        {
            bool header = true;
            bool table = MessageBox.Show("Results can be copied as own table or as a long table-row. Should it be copied as own table?",
                    "Copy as own table?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            if (extractor.validResults && speciesSelector1.speciesIndex >= 0)
            {
                List<string> tsv = new List<string>();
                string rowLevel = Values.V.speciesNames[speciesSelector1.speciesIndex] + "\t\t", rowValues = "";
                // if taming effectiveness is unique, display it, too
                string effString = "";
                double eff = extractor.uniqueTE();
                if (eff >= 0)
                {
                    effString = "\tTamingEff:\t" + (100 * eff) + "%";
                }
                // headerrow
                if (table || header)
                {
                    if (table)
                    {
                        tsv.Add(Values.V.speciesNames[speciesSelector1.speciesIndex] + "\tLevel " + numericUpDownLevel.Value + effString);
                        tsv.Add("Stat\tWildLevel\tDomLevel\tBreedingValue");
                    }
                    else
                    {
                        tsv.Add("Species\tName\tSex\tHP-Level\tSt-Level\tOx-Level\tFo-Level\tWe-Level\tDm-Level\tSp-Level\tTo-Level\tHP-Value\tSt-Value\tOx-Value\tFo-Value\tWe-Value\tDm-Value\tSp-Value\tTo-Value");
                    }
                }
                for (int s = 0; s < 8; s++)
                {
                    if (extractor.chosenResults[s] < extractor.results[s].Count)
                    {
                        string breedingV = "";
                        if (activeStats[s])
                        {
                            breedingV = statIOs[s].BreedingValue.ToString();
                        }
                        if (table)
                        {
                            tsv.Add(Utils.statName(s) + "\t" + (statIOs[s].LevelWild >= 0 ? statIOs[s].LevelWild.ToString() : "") + "\t" + (statIOs[s].LevelWild >= 0 ? statIOs[s].LevelWild.ToString() : "") + "\t" + breedingV);
                        }
                        else
                        {
                            rowLevel += "\t" + (activeStats[s] ? statIOs[s].LevelWild.ToString() : "");
                            rowValues += "\t" + breedingV;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                if (!table)
                {
                    tsv.Add(rowLevel + rowValues);
                }
                Clipboard.SetText(string.Join("\n", tsv));
            }
        }

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            n?.Select(0, n.Text.Length);
        }

        private void recalculateAllCreaturesValues()
        {
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = creatureCollection.creatures.Count();
            toolStripProgressBar1.Visible = true;
            int? levelStep = creatureCollection.getWildLevelStep();
            foreach (Creature c in creatureCollection.creatures)
            {
                c.recalculateCreatureValues(levelStep);
                toolStripProgressBar1.Value++;
            }
            toolStripProgressBar1.Visible = false;
        }

        private void creatureInfoInputExtractor_Add2Library_Clicked(CreatureInfoInput sender)
        {
            add2Lib();
        }

        private void creatureInfoInputTester_Add2Library_Clicked(CreatureInfoInput sender)
        {
            add2Lib(false);
        }

        /// <summary>
        /// Add a new creature to the library based from the data of the extractor or tester
        /// </summary>
        /// <param name="fromExtractor">if true, take data from extractor-infoinput, else from tester</param>
        /// <param name="motherArkId">only pass if from import. Used for creating placeholder parents</param>
        /// <param name="fatherArkId">only pass if from import. Used for creating placeholder parents</param>
        private void add2Lib(bool fromExtractor = true, long motherArkId = 0, long fatherArkId = 0)
        {
            CreatureInfoInput input;
            bool bred;
            double te, imprinting;
            string species;
            if (fromExtractor)
            {
                input = creatureInfoInputExtractor;
                species = Values.V.speciesNames[speciesSelector1.speciesIndex];
                bred = rbBredExtractor.Checked;
                te = extractor.uniqueTE();
                imprinting = extractor.imprintingBonus;
            }
            else
            {
                input = creatureInfoInputTester;
                species = Values.V.speciesNames[speciesSelector1.speciesIndex];
                bred = rbBredTester.Checked;
                te = (double)NumericUpDownTestingTE.Value / 100;
                imprinting = (double)numericUpDownImprintingBonusTester.Value / 100;
            }

            var levelStep = creatureCollection.getWildLevelStep();
            Creature creature = new Creature(species, input.CreatureName, input.CreatureOwner, input.CreatureTribe, input.CreatureSex, getCurrentWildLevels(fromExtractor), getCurrentDomLevels(fromExtractor), te, bred, imprinting, levelStep: levelStep)
            {
                // set parents
                Mother = input.mother,
                Father = input.father,

                // cooldown-, growing-time
                cooldownUntil = input.Cooldown,
                growingUntil = input.Grown,

                note = input.CreatureNote,
                server = input.CreatureServer,

                domesticatedAt = input.domesticatedAt,
                addedToLibrary = DateTime.Now,
                mutationsMaternal = input.MutationCounterMother,
                mutationsPaternal = input.MutationCounterFather,
                status = input.CreatureStatus,
                colors = input.RegionColors
            };

            // Ids: ArkId and Guid
            creature.guid = input.CreatureGuid != Guid.Empty ? input.CreatureGuid : Guid.NewGuid();

            creature.ArkId = input.ArkId;
            creature.ArkIdImported = creature.ArkId != 0
                    && creature.guid == Utils.ConvertArkIdToGuid(creature.ArkId);

            // parent guids
            if (motherArkId != 0)
                creature.motherGuid = Utils.ConvertArkIdToGuid(motherArkId);
            if (fatherArkId != 0)
                creature.fatherGuid = Utils.ConvertArkIdToGuid(fatherArkId);

            // if creature is placeholder: add it
            // if creature's ArkId is already in library, suggest updating of the creature
            //if (!IsArkIdUniqueOrOnlyPlaceHolder(creature))
            //{
            //    // if creature is already in library, suggest updating or dismissing

            //    //ShowDuplicateMergerAndCheckForDuplicates()

            //    return;
            //}

            creature.recalculateCreatureValues(levelStep);
            creature.recalculateAncestorGenerations();

            creatureCollection.mergeCreatureList(new List<Creature> { creature }, update: true);

            // if new creature is parent of existing creatures, update link
            var motherOf = creatureCollection.creatures.Where(c => c.motherGuid == creature.guid).ToList();
            var fatherOf = creatureCollection.creatures.Where(c => c.fatherGuid == creature.guid).ToList();
            foreach (Creature c in motherOf) c.Mother = creature;
            foreach (Creature c in fatherOf) c.Father = creature;

            // link new creature to its parents if they're available, or creature placeholders
            if (creature.Mother == null || creature.Father == null)
                updateParents(new List<Creature> { creature });

            updateCreatureListings(Values.V.speciesNames.IndexOf(species));
            // show only the added creatures' species
            listBoxSpeciesLib.SelectedIndex = listBoxSpeciesLib.Items.IndexOf(creature.species);
            tabControlMain.SelectedTab = tabPageLibrary;

            creatureInfoInputExtractor.parentListValid = false;
            creatureInfoInputTester.parentListValid = false;

            // set status of exportedCreatureControl if available
            exportedCreatureControl?.setStatus(uiControls.ExportedCreatureControl.ImportStatus.JustImported, DateTime.Now);

            setCollectionChanged(true, species);
        }

        /// <summary>
        /// Checks if the ArkId of the given creature is already in the collection. If a placeholder has this id, the placeholder is removed and the placeholder.Guid is set to the creature.
        /// </summary>
        /// <param name="creature">Creature whose ArkId will be checked</param>
        /// <returns>True if the ArkId is unique (or only a placeholder had it). False if there is a conflict.</returns>
        private bool IsArkIdUniqueOrOnlyPlaceHolder(Creature creature)
        {
            bool arkIdIsUnique = true;

            if (creature.ArkId != 0 && creatureCollection.ArkIdAlreadyExist(creature.ArkId, creature, out Creature guidCreature))
            {
                // if the creature is a placeholder replace the placeholder with the real creature
                if (guidCreature.IsPlaceholder && creature.sex == guidCreature.sex && creature.species == guidCreature.species)
                {
                    // remove placeholder-creature from collection (is replaced by new creature)
                    creatureCollection.creatures.Remove(guidCreature);
                }
                else
                {
                    // creature is not a placeholder, warn about id-conflict and don't add creature.
                    // TODO offer merging of the two creatures if they are similar (e.g. same species). merge automatically if only the dom-levels are different?
                    MessageBox.Show("The entered ARK-ID is already existing in this library " +
                            $"({guidCreature.species} (lvl {guidCreature.level}): {guidCreature.name}).\n" +
                            "You have to choose a different ARK-ID or delete the other creature first.",
                            "ARK-ID already existing",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    arkIdIsUnique = false;
                }
            }

            return arkIdIsUnique;
        }

        private int[] getCurrentWildLevels(bool fromExtractor = true)
        {
            int[] levelsWild = new int[8];
            for (int s = 0; s < 8; s++)
            {
                levelsWild[s] = fromExtractor ? statIOs[s].LevelWild : testingIOs[s].LevelWild;
            }
            return levelsWild;
        }

        private int[] getCurrentDomLevels(bool fromExtractor = true)
        {
            int[] levelsDom = new int[8];
            for (int s = 0; s < 8; s++)
            {
                levelsDom[s] = fromExtractor ? statIOs[s].LevelDom : testingIOs[s].LevelDom;
            }
            return levelsDom;
        }

        /// <summary>
        /// Call after the creatureCollection-object was created anew (e.g. after loading a file)
        /// </summary>
        private void initializeCollection()
        {
            // set pointer to current collection
            pedigree1.creatures = creatureCollection.creatures;
            breedingPlan1.creatureCollection = creatureCollection;
            tribesControl1.Tribes = creatureCollection.tribes;
            tribesControl1.Players = creatureCollection.players;
            timerList1.CreatureCollection = creatureCollection;
            notesControl1.NoteList = creatureCollection.noteList;
            raisingControl1.creatureCollection = creatureCollection;
            statsMultiplierTesting1.CreatureCollection = creatureCollection;

            createCreatureTagList();

            pedigree1.Clear();
            breedingPlan1.Clear();

            updateTempCreatureDropDown();
        }

        private void applySettingsToValues()
        {
            // apply multipliers
            Values.V.applyMultipliers(creatureCollection, cbEventMultipliers.Checked);
            tamingControl1.setTamingMultipliers(Values.V.tamingSpeedMultiplier,
                    cbEventMultipliers.Checked ? creatureCollection.tamingFoodRateMultiplierEvent : creatureCollection.tamingFoodRateMultiplier);
            breedingPlan1.updateBreedingData();
            raisingControl1.updateRaisingData();

            // apply level settings
            creatureBoxListView.BarMaxLevel = creatureCollection.maxChartLevel;
            for (int s = 0; s < 8; s++)
            {
                statIOs[s].barMaxLevel = creatureCollection.maxChartLevel;
                testingIOs[s].barMaxLevel = creatureCollection.maxChartLevel;
            }
            breedingPlan1.maxWildLevels = creatureCollection.maxWildLevel;
            radarChart1.initializeVariables(creatureCollection.maxChartLevel);
            radarChartExtractor.initializeVariables(creatureCollection.maxChartLevel);
            radarChartLibrary.initializeVariables(creatureCollection.maxChartLevel);
            statPotentials1.levelDomMax = creatureCollection.maxDomLevel;
            statPotentials1.levelGraphMax = creatureCollection.maxChartLevel;

            speechRecognition?.setMaxLevelAndSpecies(creatureCollection.maxWildLevel, creatureCollection.considerWildLevelSteps ? creatureCollection.wildLevelStep : 1, Values.V.speciesWithAliasesList);
            if (overlay != null)
            {
                overlay.InfoDuration = Properties.Settings.Default.OverlayInfoDuration;
                overlay.checkInventoryStats = Properties.Settings.Default.inventoryCheckTimer;
            }

            oxygenForAll = Properties.Settings.Default.oxygenForAll;
            ArkOCR.OCR.screenCaptureApplicationName = Properties.Settings.Default.OCRApp;
            Values.V.celsius = Properties.Settings.Default.celsius;

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
            ocrControl1.setWhiteThreshold(Properties.Settings.Default.OCRWhiteThreshold);

            int maxImprintingPercentage = creatureCollection.allowMoreThanHundredImprinting ? 1000 : 100;
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

            if (tabControlMain.SelectedTab == tabPageExtractor)
            {
                clearAll();
                // update enabled stats
                for (int s = 0; s < 8; s++)
                {
                    activeStats[s] = Values.V.species[speciesSelector1.speciesIndex].stats[s].BaseValue > 0 && (s != 2 || !Values.V.species[speciesSelector1.speciesIndex].doesNotUseOxygen || oxygenForAll);
                    statIOs[s].Enabled = activeStats[s];
                    if (!activeStats[s]) statIOs[s].Input = 0;
                }
            }
            else if (tabControlMain.SelectedTab == tabPageStatTesting)
            {
                updateAllTesterValues();
            }

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
                    tsmi.Click += runSavegameImport;
                    importingFromSavegameToolStripMenuItem.DropDownItems.Add(tsmi);
                }
            }
        }

        private void importingFromSavegameEmptyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openSettingsDialog(2);
        }

        private async void runSavegameImport(object sender, EventArgs e)
        {
            try
            {
                ATImportFileLocation atImportFileLocation = (ATImportFileLocation)((ToolStripMenuItem)sender).Tag;

                string workingCopyfilename = Properties.Settings.Default.savegameExtractionPath;

                // working dir not configured? use temp dir
                // luser configured savegame folder as working dir? use temp dir instead
                if (string.IsNullOrWhiteSpace(workingCopyfilename) ||
                        Path.GetDirectoryName(atImportFileLocation.FileLocation) == workingCopyfilename)
                {
                    workingCopyfilename = Path.GetTempPath();
                }
                workingCopyfilename = Path.Combine(workingCopyfilename, Path.GetFileName(atImportFileLocation.FileLocation));

                File.Copy(atImportFileLocation.FileLocation, workingCopyfilename, true);

                await ImportSavegame.ImportCollectionFromSavegame(creatureCollection, workingCopyfilename, atImportFileLocation.ServerName);

                updateParents(creatureCollection.creatures);

                foreach (var creature in creatureCollection.creatures)
                {
                    creature.recalculateAncestorGenerations();
                }

                updateIncubationParents(creatureCollection);

                // calculate creature values
                recalculateAllCreaturesValues();

                // update UI
                setCollectionChanged(true);
                updateCreatureListings();

                if (creatureCollection.creatures.Count > 0)
                    tabControlMain.SelectedTab = tabPageLibrary;

                // reapply last sorting
                listViewLibrary.Sort();

                updateTempCreatureDropDown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured while importing. Message: \n\n{ex.Message}",
                        "Import Error", MessageBoxButtons.OK);
            }
        }

        private void createCreatureTagList()
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

            breedingPlan1.createTagList();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.Show();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadCollection();
        }

        private void loadAndAddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadCollection(true);
        }

        private void loadCollection(bool add = false)
        {
            if (!add && collectionDirty)
            {
                if (MessageBox.Show("Your Creature Collection has been modified since it was last saved, are you sure you want to load without saving first?", "Discard Changes?", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Creature Collection File (*.xml)|*.xml"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                loadCollectionFile(dlg.FileName, add);
            }
        }


        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveCollection();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveNewCollection();
        }

        private void saveCollection()
        {
            if (currentFileName == "")
                saveNewCollection();
            else
            {
                saveCollectionToFileName(currentFileName);
            }
        }

        private void saveNewCollection()
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Creature Collection File (*.xml)|*.xml"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                currentFileName = dlg.FileName;
                fileSync.changeFile(currentFileName);
                saveCollectionToFileName(currentFileName);
            }
        }

        private void saveCollectionToFileName(string fileName)
        {
            // Wait until the file is writeable
            const int numberOfRetries = 5;
            const int delayOnRetry = 1000;
            bool fileSaved = false;
            FileStream file = null;

            for (int i = 1; i <= numberOfRetries; ++i)
            {
                try
                {
                    file = File.Create(fileName);
                    XmlSerializer writer = new XmlSerializer(typeof(CreatureCollection));
                    fileSync.justSaving();
                    writer.Serialize(file, creatureCollection);
                    fileSaved = true;
                    Properties.Settings.Default.LastSaveFile = fileName;

                    break; // when file is saved, break
                }
                catch (IOException)
                {
                    // if file is not saveable
                    Thread.Sleep(delayOnRetry);
                }
                catch (InvalidOperationException e)
                {
                    MessageBox.Show($"Error during serialization.\nErrormessage:\n\n{e.Message}", "Serialization-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    file?.Close();
                }
            }

            if (fileSaved)
                setCollectionChanged(false);
            else
                MessageBox.Show($"This file couldn\'t be saved:\n{fileName}\nMaybe the file is used by another application.", "Error during saving", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool loadCollectionFile(string fileName, bool keepCurrentCreatures = false, bool stayInCurrentTab = false)
        {
            XmlSerializer reader = new XmlSerializer(typeof(CreatureCollection));

            if (!File.Exists(fileName))
            {
                MessageBox.Show($"Save file with name \"{fileName}\" does not exist!", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            List<Creature> oldCreatures = null;
            if (keepCurrentCreatures)
                oldCreatures = creatureCollection.creatures;

            FileStream file = null;

            // for the case the collectionfile has no multipliers, keep the current ones
            double[][] oldMultipliers = creatureCollection.multipliers;

            // Wait until the file is readable
            const int numberOfRetries = 5;
            const int delayOnRetry = 1000;

            for (int i = 1; i <= numberOfRetries; ++i)
            {
                try
                {
                    file = File.OpenRead(fileName);
                    creatureCollection = (CreatureCollection)reader.Deserialize(file);
                }
                catch (IOException)
                {
                    // if file is not readable
                    Thread.Sleep(delayOnRetry);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"The library-file\n{fileName}\ncouldn\'t be opened, we thought you should know.\nErrormessage:\n\n{e.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                finally
                {
                    file?.Close();
                }
            }

            creatureCollectionFixes(creatureCollection);

            if (Values.V.modValuesFile != "" && Values.V.modValuesFile != creatureCollection.additionalValues)
            {
                // load original multipliers if they were changed
                Values.V.loadValues();
                if (speechRecognition != null) speechRecognition.updateNeeded = true;
            }
            if (!string.IsNullOrEmpty(creatureCollection.additionalValues) && Values.V.modValuesFile != creatureCollection.additionalValues)
                loadAdditionalValues(FileService.GetJsonPath(creatureCollection.additionalValues), false, false);

            if (creatureCollection.multipliers == null)
            {
                creatureCollection.multipliers = oldMultipliers ?? Values.V.getOfficialMultipliers();
            }

            if (speciesSelector1.LastSpecies != null && speciesSelector1.LastSpecies.Length > 0)
            {
                string lastSpecies = speciesSelector1.LastSpecies[0];
                tamingControl1.setSpeciesIndex(Values.V.speciesIndex(lastSpecies));
            }

            applySettingsToValues();

            bool creatureWasAdded = false;

            if (keepCurrentCreatures)
                creatureWasAdded = creatureCollection.mergeCreatureList(oldCreatures);
            else
            {
                currentFileName = fileName;
                fileSync.changeFile(currentFileName);
                creatureBoxListView.Clear();
            }

            updateParents(creatureCollection.creatures);
            updateIncubationParents(creatureCollection);
            initializeCollection();

            filterListAllowed = false;
            setLibraryFilter("Dead", creatureCollection.showDeads);
            setLibraryFilter("Unavailable", creatureCollection.showUnavailable);
            setLibraryFilter("Neutered", creatureCollection.showNeutered);
            setLibraryFilter("Obelisk", creatureCollection.showObelisk);
            setLibraryFilter("Cryopod", creatureCollection.showCryopod);
            setLibraryFilter("Mutated", creatureCollection.showMutated);
            checkBoxUseFiltersInTopStatCalculation.Checked = creatureCollection.useFiltersInTopStatCalculation;
            filterListAllowed = true;

            setCollectionChanged(creatureWasAdded); // setCollectionChanged only if there really were creatures added from the old library to the just opened one

            ///// creatures loaded.

            // calculate creature values
            recalculateAllCreaturesValues();

            if (!stayInCurrentTab && creatureCollection.creatures.Count > 0)
                tabControlMain.SelectedTab = tabPageLibrary;

            creatureBoxListView.maxDomLevel = creatureCollection.maxDomLevel;

            updateCreatureListings();

            // apply last sorting
            listViewLibrary.Sort();

            updateTempCreatureDropDown();

            Properties.Settings.Default.LastSaveFile = fileName;
            lastAutoSaveBackup = DateTime.Now.AddMinutes(-10);

            return true;
        }

        /// <summary>
        /// This function should be called if the creatureCollection was changed, i.e. after loading a file or adding/removing a creature
        /// </summary>
        private void updateCreatureListings(int speciesIndex = -1)
        {
            // if speciesIndex==-1 consider all creatures, else recalculate only the indicated species if applicable
            List<Creature> creatures = creatureCollection.creatures;
            if (speciesIndex >= 0)
            {
                creatures = creatures.Where(c => c.species == Values.V.speciesNames[speciesIndex]).ToList();
            }
            createOwnerList();
            calculateTopStats(creatures);
            updateSpeciesLists(creatureCollection.creatures);
            filterLib();
            updateStatusBar();
            breedingPlan1.CurrentSpecies = ""; // set to empty so creatures are loaded again if breeding plan is created
            pedigree1.updateListView();
            raisingControl1.recreateList();
        }

        /// <summary>
        /// This function should be called if the creatureCollection is changed, i.e. after loading a file or adding/removing a creature.
        /// It updates the listed species in the treelist and in the speciesSelector.
        /// </summary>
        private void updateSpeciesLists(List<Creature> creatures)
        {
            string selectedSpecies = "";
            if (listBoxSpeciesLib.SelectedIndex >= 0)
                selectedSpecies = listBoxSpeciesLib.SelectedItem.ToString();
            // clear specieslist
            listBoxSpeciesLib.Items.Clear();
            List<string> availableSpeciesNames = new List<string>();

            foreach (Creature cr in creatures)
            {
                // add new item for species if not existent
                if (!listBoxSpeciesLib.Items.Contains(cr.species))
                {
                    availableSpeciesNames.Add(cr.species);
                }
            }

            // sort species according to selected order (can be modified by json/sortNames.txt)
            availableSpeciesNames = Values.V.speciesNames.Where(sn => availableSpeciesNames.Contains(sn)).ToList();

            // add node to show all
            listBoxSpeciesLib.Items.Insert(0, "All");
            listBoxSpeciesLib.Items.AddRange(availableSpeciesNames.ToArray());

            if (!string.IsNullOrEmpty(selectedSpecies))
                listBoxSpeciesLib.SelectedIndex = listBoxSpeciesLib.Items.IndexOf(selectedSpecies);

            breedingPlan1.setSpeciesList(availableSpeciesNames, creatures);
            speciesSelector1.setLibrarySpecies(availableSpeciesNames);
        }

        private void createOwnerList()
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

            filterListAllowed = true;
        }

        private void showCreaturesInListView(List<Creature> creatures)
        {
            listViewLibrary.BeginUpdate();

            // clear ListView
            listViewLibrary.Items.Clear();
            listViewLibrary.Groups.Clear();

            // add groups for each species (so they are sorted alphabetically)
            foreach (string s in Values.V.speciesNames)
            {
                listViewLibrary.Groups.Add(new ListViewGroup(s));
            }

            List<ListViewItem> items = new List<ListViewItem>();
            foreach (Creature cr in creatures)
            {
                // check if group of species exists
                ListViewGroup g = null;
                foreach (ListViewGroup lvg in listViewLibrary.Groups)
                {
                    if (lvg.Header == cr.species)
                    {
                        g = lvg;
                        break;
                    }
                }
                if (g == null)
                {
                    g = new ListViewGroup(cr.species);
                    listViewLibrary.Groups.Add(g);
                }
                items.Add(createCreatureLVItem(cr, g));
            }
            listViewLibrary.Items.AddRange(items.ToArray());
            listViewLibrary.EndUpdate();
        }

        /// <summary>
        /// Call this function to update the displayed values of a creature. Usually called after a creature was edited.
        /// </summary>
        /// <param name="cr">Creature that was changed</param>
        /// <param name="creatureStatusChanged"></param>
        private void updateCreatureValues(Creature cr, bool creatureStatusChanged)
        {
            // if row is selected, save and reselect later
            List<Creature> selectedCreatures = new List<Creature>();
            foreach (ListViewItem i in listViewLibrary.SelectedItems)
                selectedCreatures.Add((Creature)i.Tag);

            // data of the selected creature changed, update listview
            cr.recalculateCreatureValues(creatureCollection.getWildLevelStep());
            // if creaturestatus (available/dead) changed, recalculate topstats (dead creatures are not considered there)
            if (creatureStatusChanged)
            {
                calculateTopStats(creatureCollection.creatures.Where(c => c.species == cr.species).ToList());
                filterLib();
            }
            else
            {
                // int listViewLibrary replace old row with new one
                int ci = -1;
                for (int i = 0; i < listViewLibrary.Items.Count; i++)
                {
                    if ((Creature)listViewLibrary.Items[i].Tag == cr)
                    {
                        ci = i;
                        break;
                    }
                }
                if (ci >= 0)
                    listViewLibrary.Items[ci] = createCreatureLVItem(cr, listViewLibrary.Items[ci].Group);
            }
            // recreate ownerlist
            createOwnerList();
            setCollectionChanged(true, cr.species);

            // select previous selecteded again
            int selectedCount = selectedCreatures.Count;
            if (selectedCount > 0)
            {
                for (int i = 0; i < listViewLibrary.Items.Count; i++)
                {
                    if (selectedCreatures.Contains((Creature)listViewLibrary.Items[i].Tag))
                    {
                        listViewLibrary.Items[i].Focused = true;
                        listViewLibrary.Items[i].Selected = true;
                        if (--selectedCount == 0)
                        {
                            listViewLibrary.EnsureVisible(i);
                            break;
                        }
                    }
                }
            }
        }

        private ListViewItem createCreatureLVItem(Creature cr, ListViewGroup g)
        {
            double colorFactor = 100d / creatureCollection.maxChartLevel;
            DateTime cldGr = cr.cooldownUntil > cr.growingUntil ? cr.cooldownUntil : cr.growingUntil;
            bool cld = cr.cooldownUntil > cr.growingUntil;

            string[] subItems = new[]
                    {
                            cr.name + (cr.status != CreatureStatus.Available ? $" ({Utils.statusSymbol(cr.status)})" : ""),
                            cr.owner + (string.IsNullOrEmpty(cr.tribe) ? "" : $" ({cr.tribe})"),
                            cr.note,
                            cr.server,
                            Utils.sexSymbol(cr.sex),
                            cr.domesticatedAt.ToString("yyyy'-'MM'-'dd HH':'mm"),
                            cr.topness.ToString(),
                            cr.topStatsCount.ToString(),
                            cr.generation.ToString(),
                            cr.levelFound.ToString(),
                            cr.Mutations.ToString(),
                            DateTime.Now.CompareTo(cldGr) < 0 ? cldGr.ToString() : "-"
                    }
                    .Concat(cr.levelsWild.Select(x => x.ToString()).ToArray())
                    .ToArray();

            if (Properties.Settings.Default.showColorsInLibrary)
                subItems = subItems.Concat(cr.colors.Select(cl => cl.ToString()).ToArray()).ToArray();

            ListViewItem lvi = new ListViewItem(subItems, g);
            for (int s = 0; s < 8; s++)
            {
                // color unknown levels
                if (cr.levelsWild[s] < 0)
                {
                    lvi.SubItems[s + 12].ForeColor = Color.WhiteSmoke;
                    lvi.SubItems[s + 12].BackColor = Color.WhiteSmoke;
                }
                else
                    lvi.SubItems[s + 12].BackColor = Utils.getColorFromPercent((int)(cr.levelsWild[s] * (s == 7 ? colorFactor / 7 : colorFactor)),
                            considerStatHighlight[s] ? cr.topBreedingStats[s] ? 0.2 : 0.7 : 0.93);
            }
            lvi.SubItems[4].BackColor = cr.neutered ? SystemColors.GrayText :
                    cr.sex == Sex.Female ? Color.FromArgb(255, 230, 255) :
                    cr.sex == Sex.Male ? Color.FromArgb(220, 235, 255) : SystemColors.Window;

            if (cr.status == CreatureStatus.Dead)
            {
                lvi.SubItems[0].ForeColor = SystemColors.GrayText;
                lvi.BackColor = Color.FromArgb(255, 250, 240);
            }
            else if (cr.status == CreatureStatus.Unavailable)
            {
                lvi.SubItems[0].ForeColor = SystemColors.GrayText;
            }
            else if (cr.status == CreatureStatus.Obelisk)
            {
                lvi.SubItems[0].ForeColor = Color.DarkBlue;
            }
            else if (creatureCollection.maxServerLevel > 0
                    && cr.levelsWild[7] + 1 + creatureCollection.maxDomLevel > creatureCollection.maxServerLevel)
            {
                lvi.SubItems[0].ForeColor = Color.OrangeRed; // this creature may pass the max server level and could be deleted by the game
            }

            lvi.UseItemStyleForSubItems = false;

            // color for top-stats-nr
            if (cr.topStatsCount > 0)
            {
                if (cr.topBreedingCreature)
                {
                    if (cr.topStatsCount == considerStatHighlight.Count(ts => ts))
                        lvi.BackColor = Color.Gold;
                    else
                        lvi.BackColor = Color.LightGreen;
                }
                lvi.SubItems[7].BackColor = Utils.getColorFromPercent(cr.topStatsCount * 8 + 44, 0.7);
            }
            else
            {
                lvi.SubItems[7].ForeColor = Color.LightGray;
            }

            // color for timestamp added
            if (cr.domesticatedAt.Year < 2015)
            {
                lvi.SubItems[5].Text = "n/a";
                lvi.SubItems[5].ForeColor = Color.LightGray;
            }

            // color for topness
            lvi.SubItems[6].BackColor = Utils.getColorFromPercent(cr.topness * 2 - 100, 0.8); // topness is in percent. gradient from 50-100

            // color for generation
            if (cr.generation == 0)
                lvi.SubItems[8].ForeColor = Color.LightGray;

            // color of WildLevelColumn
            if (cr.levelFound == 0)
                lvi.SubItems[9].ForeColor = Color.LightGray;

            // color for mutation
            if (cr.Mutations > 0)
            {
                if (cr.Mutations > 19)
                    lvi.SubItems[10].BackColor = Utils.MutationColorOverLimit;
                else
                    lvi.SubItems[10].BackColor = Utils.MutationColor;
            }
            else
                lvi.SubItems[10].ForeColor = Color.LightGray;

            // color for cooldown
            cooldownColors(cr, out Color forecolor, out Color backcolor);
            lvi.SubItems[11].ForeColor = forecolor;
            lvi.SubItems[11].BackColor = backcolor;

            if (Properties.Settings.Default.showColorsInLibrary)
            {
                // color for colors
                for (int cl = 0; cl < 6; cl++)
                {
                    if (cr.colors[cl] != 0)
                    {
                        lvi.SubItems[20 + cl].BackColor = CreatureColors.creatureColor(cr.colors[cl]);
                        lvi.SubItems[20 + cl].ForeColor = Utils.ForeColor(lvi.SubItems[20 + cl].BackColor);
                    }
                    else
                    {
                        lvi.SubItems[20 + cl].ForeColor = Color.White;
                    }
                }
            }

            lvi.Tag = cr;
            return lvi;
        }

        private void cooldownColors(Creature c, out Color forecolor, out Color backcolor)
        {
            DateTime cldGr = c.cooldownUntil > c.growingUntil ? c.cooldownUntil : c.growingUntil;
            bool cooldown = c.cooldownUntil > c.growingUntil;
            double minCld = cldGr.Subtract(DateTime.Now).TotalMinutes;
            forecolor = SystemColors.ControlText;
            backcolor = SystemColors.Window;

            if (minCld <= 0)
                forecolor = Color.LightGray;
            else
            {
                if (cooldown)
                {
                    // mating-cooldown
                    if (minCld < 1)
                        backcolor = Color.FromArgb(235, 255, 109);
                    else if (minCld < 10)
                        backcolor = Color.FromArgb(255, 250, 109);
                    else
                        backcolor = Color.FromArgb(255, 179, 109);
                }
                else
                {
                    // growing
                    if (minCld < 1)
                        backcolor = Color.FromArgb(168, 187, 255);
                    else if (minCld < 10)
                        backcolor = Color.FromArgb(197, 168, 255);
                    else
                        backcolor = Color.FromArgb(236, 168, 255);
                }
            }
        }

        private void checkForUpdatedStatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkForUpdates();
        }

        #region check for update

        private async void checkForUpdates(bool silentCheck = false)
        {
            if (Updater.IsProgramInstalled)
            {
                if (await Updater.CheckForInstallerUpdate(silentCheck, collectionDirty))
                {
                    Close();
                    return;
                }
            }
            else
            {
                if (await Updater.CheckForUpdaterUpdate(silentCheck, collectionDirty))
                {
                    Close();
                    return;
                }
            }

            // check if values.json can be updated
            (bool? wantsValuesUpdate, bool valuesUpdated) = await Updater.CheckForValuesUpdate(silentCheck);

            // update last successful updateCheck
            Properties.Settings.Default.lastUpdateCheck = DateTime.Now;

            if (valuesUpdated)
            {
                updateLoadNewValues();
            }
            else if (!silentCheck && wantsValuesUpdate == null)
            {
                MessageBox.Show("You already have the newest version of both the program and values file.\n\n" +
                        "If your stats are outdated and no new version is available, we probably don\'t have the new ones either.",
                        "No new Version available", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void updateLoadNewValues()
        {
            if (Values.V.loadValues())
            {
                if (speechRecognition != null)
                    speechRecognition.updateNeeded = true;
                applySettingsToValues();
                speciesSelector1.setSpeciesLists(Values.V.speciesNames, Values.V.speciesWithAliasesList);
                MessageBox.Show("Download and update of new creature-stats successful",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                updateStatusBar();
            }
            else
                MessageBox.Show("Download of new stat successful, but files couldn't be loaded.\nTry again later, or redownload the tool.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (!Kibbles.K.loadValues())
                MessageBox.Show("The kibbles-file couldn't be loaded, the kibble-recipes will not be available. " +
                        "You can redownload the tool to get this file.",
                        "Error: Kibble-file not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        private void creatureInfoInput_ParentListRequested(CreatureInfoInput sender)
        {
            updateParentListInput(sender);
        }

        private void updateParentListInput(CreatureInfoInput input)
        {
            // set possible parents
            bool fromExtractor = input == creatureInfoInputExtractor;
            Creature creature = new Creature(speciesSelector1.species, "", "", "", 0, getCurrentWildLevels(fromExtractor), levelStep: creatureCollection.getWildLevelStep());
            List<Creature>[] parents = findPossibleParents(creature);
            input.ParentsSimilarities = findParentSimilarities(parents, creature);
            input.Parents = parents;
            input.parentListValid = true;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newCollection();
        }

        private void newCollection()
        {
            if (collectionDirty)
            {
                if (MessageBox.Show("Your Creature Collection has been modified since it was last saved, " +
                        "are you sure you want to discard your changes and create a new Library without saving?",
                        "Discard Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
            }

            if (creatureCollection.additionalValues.Length > 0)
            {
                // if old collection had additionalValues, load the original ones.
                Values.V.loadValues();
                if (speechRecognition != null) speechRecognition.updateNeeded = true;
            }

            if (creatureCollection.multipliers == null)
                creatureCollection.multipliers = Values.V.getOfficialMultipliers();
            // use previously used multipliers again in the new file
            double[][] oldMultipliers = creatureCollection.multipliers;

            creatureCollection = new CreatureCollection
            {
                multipliers = oldMultipliers
            };
            pedigree1.Clear();
            breedingPlan1.Clear();
            applySettingsToValues();
            initializeCollection();

            updateCreatureListings();
            creatureBoxListView.Clear();
            Properties.Settings.Default.LastSaveFile = "";
            Properties.Settings.Default.LastImportFile = "";
            currentFileName = "";
            fileSync.changeFile(currentFileName);
            setCollectionChanged(false);
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
            for (int s = 0; s < 8; s++)
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
            custWd.Add(breedingPlan1.statWeighting.Values); // add current values
            Properties.Settings.Default.customStatWeights = custWd.ToArray();
            Properties.Settings.Default.customStatWeightNames = custWs.ToArray();

            // save weapondamages for ko-calculation
            Properties.Settings.Default.weaponDamages = tamingControl1.weaponDamages;
            Properties.Settings.Default.weaponDamagesEnabled = tamingControl1.weaponDamagesEnabled;

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
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnSorter.doSort((ListView)sender, e.Column);
        }

        // onlibrarychange
        private async void listViewLibrary_SelectedIndexChanged(object sender, EventArgs e)
        {
            cancelTokenLibrarySelection?.Cancel();
            using (cancelTokenLibrarySelection = new CancellationTokenSource())
            {
                try
                {
                    reactOnSelectionChange = false;
                    await Task.Delay(20, cancelTokenLibrarySelection.Token); // recalculate breedingplan at most a certain interval
                    reactOnSelectionChange = true;
                    librarySelectedIndexChanged();
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
            cancelTokenLibrarySelection = null;
        }

        private void librarySelectedIndexChanged()
        {
            if (reactOnSelectionChange)
            {
                int cnt = listViewLibrary.SelectedItems.Count;
                if (cnt > 0)
                {
                    if (cnt == 1)
                    {
                        Creature c = (Creature)listViewLibrary.SelectedItems[0].Tag;
                        creatureBoxListView.setCreature(c);
                        if (tabControlLibFilter.SelectedTab == tabPageLibRadarChart)
                            radarChartLibrary.setLevels(c.levelsWild);
                        pedigreeNeedsUpdate = true;
                    }

                    // display infos about the selected creatures
                    List<Creature> selCrs = new List<Creature>();
                    for (int i = 0; i < cnt; i++)
                        selCrs.Add((Creature)listViewLibrary.SelectedItems[i].Tag);

                    List<string> tagList = new List<string>();
                    foreach (Creature cr in selCrs)
                    {
                        foreach (string t in cr.tags)
                            if (!tagList.Contains(t))
                                tagList.Add(t);
                    }
                    tagList.Sort();

                    setMessageLabelText($"{cnt} creatures selected, " +
                            $"{selCrs.Count(cr => cr.sex == Sex.Female)} females, " +
                            $"{selCrs.Count(cr => cr.sex == Sex.Male)} males\n" +
                            $"level-range: {selCrs.Min(cr => cr.level)} - {selCrs.Max(cr => cr.level)}\n" +
                            $"Tags: {string.Join(", ", tagList)}");
                }
                else
                {
                    setMessageLabelText();
                    creatureBoxListView.Clear();
                }
            }
        }

        private void setMessageLabelText(string text = "", MessageBoxIcon icon = MessageBoxIcon.None)
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
            setLibraryFilter("Dead", checkBoxShowDead.Checked);
        }

        private void checkBoxShowUnavailableCreatures_CheckedChanged(object sender, EventArgs e)
        {
            setLibraryFilter("Unavailable", checkBoxShowUnavailableCreatures.Checked);
        }

        private void checkBoxShowNeuteredCreatures_CheckedChanged(object sender, EventArgs e)
        {
            setLibraryFilter("Neutered", checkBoxShowNeuteredCreatures.Checked);
        }

        private void checkBoxShowMutatedCreatures_CheckedChanged(object sender, EventArgs e)
        {
            setLibraryFilter("Mutated", checkBoxShowMutatedCreatures.Checked);
        }

        private void checkBoxShowObeliskCreatures_CheckedChanged(object sender, EventArgs e)
        {
            setLibraryFilter("Obelisk", checkBoxShowObeliskCreatures.Checked);
        }

        private void checkBoxShowCryopodCreatures_CheckedChanged(object sender, EventArgs e)
        {
            setLibraryFilter("Cryopod", checkBoxShowCryopodCreatures.Checked);
        }

        private void cbLibraryShowFemales_CheckedChanged(object sender, EventArgs e)
        {
            setLibraryFilter("Females", cbLibraryShowFemales.Checked);
        }

        private void cbLibraryShowMales_CheckedChanged(object sender, EventArgs e)
        {
            setLibraryFilter("Males", cbLibraryShowMales.Checked);
        }

        private void deadCreaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setLibraryFilter("Dead", deadCreaturesToolStripMenuItem.Checked);
        }

        private void unavailableCreaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setLibraryFilter("Unavailable", unavailableCreaturesToolStripMenuItem.Checked);
        }

        private void obeliskCreaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setLibraryFilter("Obelisk", obeliskCreaturesToolStripMenuItem.Checked);
        }

        private void cryopodCreaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setLibraryFilter("Cryopod", cryopodCreaturesToolStripMenuItem.Checked);
        }

        private void neuteredCreaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setLibraryFilter("Neutered", neuteredCreaturesToolStripMenuItem.Checked);
        }

        private void mutatedCreaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setLibraryFilter("Mutated", mutatedCreaturesToolStripMenuItem.Checked);
        }

        private void femalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setLibraryFilter("Females", femalesToolStripMenuItem.Checked);
        }

        private void malesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setLibraryFilter("Males", malesToolStripMenuItem.Checked);
        }

        private void setLibraryFilter(string param, bool show)
        {
            if (libraryViews.ContainsKey(param) && libraryViews[param] != show)
            {
                libraryViews[param] = show;

                switch (param)
                {
                    case "Dead":
                        creatureCollection.showDeads = show;
                        checkBoxShowDead.Checked = show;
                        deadCreaturesToolStripMenuItem.Checked = show;
                        break;
                    case "Unavailable":
                        creatureCollection.showUnavailable = show;
                        checkBoxShowUnavailableCreatures.Checked = show;
                        unavailableCreaturesToolStripMenuItem.Checked = show;
                        break;
                    case "Neutered":
                        creatureCollection.showNeutered = show;
                        checkBoxShowNeuteredCreatures.Checked = show;
                        neuteredCreaturesToolStripMenuItem.Checked = show;
                        break;
                    case "Obelisk":
                        creatureCollection.showObelisk = show;
                        checkBoxShowObeliskCreatures.Checked = show;
                        obeliskCreaturesToolStripMenuItem.Checked = show;
                        break;
                    case "Cryopod":
                        creatureCollection.showCryopod = show;
                        checkBoxShowCryopodCreatures.Checked = show;
                        cryopodCreaturesToolStripMenuItem.Checked = show;
                        break;
                    case "Mutated":
                        creatureCollection.showMutated = show;
                        checkBoxShowMutatedCreatures.Checked = show;
                        mutatedCreaturesToolStripMenuItem.Checked = show;
                        break;
                    case "Females":
                        checkBoxShowMutatedCreatures.Checked = show;
                        mutatedCreaturesToolStripMenuItem.Checked = show;
                        break;
                    case "Males":
                        checkBoxShowMutatedCreatures.Checked = show;
                        mutatedCreaturesToolStripMenuItem.Checked = show;
                        break;
                }

                recalculateTopStatsIfNeeded();
                filterLib();
            }
        }

        private void checkBoxUseFiltersInTopStatCalculation_CheckedChanged(object sender, EventArgs e)
        {
            creatureCollection.useFiltersInTopStatCalculation = checkBoxUseFiltersInTopStatCalculation.Checked;
            calculateTopStats(creatureCollection.creatures);
            filterLib();
        }

        private void listBoxSpeciesLib_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterLib();
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
            filterLib();
        }

        private void cbServerFilterAll_CheckedChanged(object sender, EventArgs e)
        {
            filterListAllowed = false;

            bool chck = cbServerFilterAll.Checked;
            creatureCollection.hiddenServers.Clear();
            for (int i = 0; i < checkedListBoxFilterServers.Items.Count; i++)
            {
                checkedListBoxFilterServers.SetItemChecked(i, chck);
                if (!chck) creatureCollection.hiddenServers.Add(checkedListBoxFilterServers.Items[i].ToString());
            }

            filterListAllowed = true;
            filterLib();
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

                recalculateTopStatsIfNeeded();
                filterLib();
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

                    recalculateTopStatsIfNeeded();
                    filterLib();
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
            filterLib();
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

                recalculateTopStatsIfNeeded();
                filterLib();
            }
        }

        /// <summary>
        /// Recalculate topstats if filters are used in topstat-calculation
        /// </summary>
        private void recalculateTopStatsIfNeeded()
        {
            if (creatureCollection.useFiltersInTopStatCalculation)
                calculateTopStats(creatureCollection.creatures);
        }

        /// <summary>
        /// Call this list to set the listview to the current filters
        /// </summary>
        private void filterLib()
        {
            if (filterListAllowed)
            {
                // save selected creatures to re-select them after the filtering
                List<Creature> selectedCreatures = new List<Creature>();
                foreach (ListViewItem i in listViewLibrary.SelectedItems)
                    selectedCreatures.Add((Creature)i.Tag);

                var filteredList = from creature in creatureCollection.creatures
                                   where !creature.IsPlaceholder
                                   select creature;

                // if only one species should be shown
                bool chargeStatsHeaders = false;
                if (listBoxSpeciesLib.SelectedItem != null)
                {
                    string selectedSpecies = listBoxSpeciesLib.SelectedItem.ToString();
                    if (selectedSpecies != "All")
                    {
                        filteredList = filteredList.Where(c => c.species == selectedSpecies);
                        if (Values.V.glowSpecies.Contains(selectedSpecies)) chargeStatsHeaders = true;
                    }
                }
                for (int s = 0; s < 8; s++)
                    listViewLibrary.Columns[12 + s].Text = Utils.statName(s, true, chargeStatsHeaders);

                filteredList = applyLibraryFilterSettings(filteredList);

                // display new results
                showCreaturesInListView(filteredList.OrderBy(c => c.name).ToList());

                // update creaturebox
                creatureBoxListView.updateLabel();

                // select previous selecteded again
                int selectedCount = selectedCreatures.Count;
                if (selectedCount > 0)
                {
                    for (int i = 0; i < listViewLibrary.Items.Count; i++)
                    {
                        if (selectedCreatures.Contains((Creature)listViewLibrary.Items[i].Tag))
                        {
                            listViewLibrary.Items[i].Focused = true;
                            listViewLibrary.Items[i].Selected = true;
                            if (--selectedCount == 0)
                            {
                                listViewLibrary.EnsureVisible(i);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Apply library filter settings to a creature collection
        /// </summary>
        private IEnumerable<Creature> applyLibraryFilterSettings(IEnumerable<Creature> creatures)
        {
            if (creatures == null) return null;

            // if only certain owner's creatures should be shown
            bool hideWOOwner = creatureCollection.hiddenOwners.Contains("n/a");
            creatures = creatures.Where(c => !creatureCollection.hiddenOwners.Contains(c.owner) && (!hideWOOwner || c.owner != ""));

            // server filter
            bool hideWOServer = creatureCollection.hiddenServers.Contains("n/a");
            creatures = creatures.Where(c => !creatureCollection.hiddenServers.Contains(c.server) && (!hideWOServer || c.server != ""));

            // tags filter
            bool dontShowWOTags = creatureCollection.dontShowTags.Contains("n/a");
            creatures = creatures.Where(c => !dontShowWOTags && c.tags.Count == 0 || c.tags.Except(creatureCollection.dontShowTags).Any());

            // show also dead creatures?
            if (!libraryViews["Dead"])
                creatures = creatures.Where(c => c.status != CreatureStatus.Dead);

            // show also unavailable creatures?
            if (!libraryViews["Unavailable"])
                creatures = creatures.Where(c => c.status != CreatureStatus.Unavailable);

            // show also in obelisks uploaded creatures?
            if (!libraryViews["Obelisk"])
                creatures = creatures.Where(c => c.status != CreatureStatus.Obelisk);

            // show also creatures in cryopods?
            if (!libraryViews["Cryopod"])
                creatures = creatures.Where(c => c.status != CreatureStatus.Cryopod);

            // show also neutered creatures?
            if (!libraryViews["Neutered"])
                creatures = creatures.Where(c => !c.neutered);

            // show also creatures with mutations?
            if (!libraryViews["Mutated"])
                creatures = creatures.Where(c => c.Mutations <= 0);

            // show also different sexes?
            if (!libraryViews["Females"])
                creatures = creatures.Where(c => c.sex != Sex.Female);
            if (!libraryViews["Males"])
                creatures = creatures.Where(c => c.sex != Sex.Male);

            return creatures;
        }

        private void deleteSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteSelectedCreatures();
        }

        private void deleteSelectedCreatures()
        {
            if (tabControlMain.SelectedTab == tabPageLibrary)
            {
                if (listViewLibrary.SelectedItems.Count > 0)
                {
                    if (MessageBox.Show("Do you really want to delete the entry and all data for " +
                            $"\"{((Creature)listViewLibrary.SelectedItems[0].Tag).name}\"" +
                            $"{(listViewLibrary.SelectedItems.Count > 1 ? " and " + (listViewLibrary.SelectedItems.Count - 1) + " other creatures" : "")}?",
                            "Delete Creature?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        bool onlyOneSpecies = true;
                        string species = ((Creature)listViewLibrary.SelectedItems[0].Tag).species;
                        int speciesIndex = Values.V.speciesNames.IndexOf(species);
                        foreach (ListViewItem i in listViewLibrary.SelectedItems)
                        {
                            if (onlyOneSpecies)
                            {
                                if (species != ((Creature)i.Tag).species)
                                    onlyOneSpecies = false;
                            }
                            creatureCollection.creatures.Remove((Creature)i.Tag);
                        }
                        updateCreatureListings(onlyOneSpecies ? speciesIndex : -1);
                        setCollectionChanged(true, onlyOneSpecies ? species : null);
                    }
                }
            }
            else if (tabControlMain.SelectedTab == tabPagePlayerTribes)
            {
                tribesControl1.removeSelected();
            }
        }

        /// <summary>
        /// calculates the top-stats in each species, sets the top-stat-flags in the creatures
        /// </summary>
        /// <param name="creatures">creatures to consider</param>
        private void calculateTopStats(List<Creature> creatures)
        {
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = Values.V.speciesNames.Count();
            toolStripProgressBar1.Visible = true;

            Creature[] filteredCreatures = creatureCollection.useFiltersInTopStatCalculation ? applyLibraryFilterSettings(creatures).ToArray() : null;
            foreach (string species in Values.V.speciesNames)
            {
                toolStripProgressBar1.Value++;
                int[] bestStat = { -1, -1, -1, -1, -1, -1, -1, -1 };
                List<Creature>[] bestCreatures = new List<Creature>[Enum.GetNames(typeof(StatName)).Count()];
                bool noCreaturesInThisSpecies = true;
                foreach (Creature c in creatures)
                {
                    if (c.species != species)
                        continue;

                    noCreaturesInThisSpecies = false;
                    // reset topBreeding stats for this creature
                    c.topBreedingStats = new bool[8];
                    c.topBreedingCreature = false;

                    // only consider creature if it's available for breeding
                    if (!creatureCollection.useFiltersInTopStatCalculation
                        && !(
                            c.status == CreatureStatus.Available
                            || c.status == CreatureStatus.Cryopod
                            || c.status == CreatureStatus.Obelisk
                            )
                       )
                    {
                        continue;
                    }
                    else
                    {
                        //if not in the filtered collection (using library filter settings), continue
                        if (!filteredCreatures.Contains(c))
                            continue;
                    }

                    for (int s = 0; s < Enum.GetNames(typeof(StatName)).Count(); s++)
                    {
                        if (c.levelsWild[s] > 0)
                        {
                            if (c.levelsWild[s] == bestStat[s])
                            {
                                bestCreatures[s].Add(c);
                            }
                            else if (c.levelsWild[s] > bestStat[s])
                            {
                                bestCreatures[s] = new List<Creature>
                                        { c };
                                bestStat[s] = c.levelsWild[s];
                            }
                        }
                    }
                }
                if (noCreaturesInThisSpecies)
                {
                    continue;
                }

                if (!topLevels.ContainsKey(species))
                {
                    topLevels.Add(species, bestStat);
                }
                else
                {
                    topLevels[species] = bestStat;
                }

                // beststat and bestcreatures now contain the best stats and creatures for each stat.

                // set topness of each creature (== mean wildlevels/mean top wildlevels in permille)
                int sumTopLevels = 0;
                for (int s = 0; s < 8; s++)
                {
                    if (considerStatHighlight[s])
                        sumTopLevels += bestStat[s];
                }
                if (sumTopLevels > 0)
                {
                    foreach (Creature c in creatures)
                    {
                        if (c.species != species)
                            continue;
                        int sumCreatureLevels = 0;
                        for (int s = 0; s < 8; s++)
                        {
                            if (considerStatHighlight[s])
                                sumCreatureLevels += c.levelsWild[s] > 0 ? c.levelsWild[s] : 0;
                        }
                        c.topness = (short)(100 * sumCreatureLevels / sumTopLevels);
                    }
                }

                // if any male is in more than 1 category, remove any male from the topBreedingCreatures that is not top in at least 2 categories himself
                for (int s = 0; s < Enum.GetNames(typeof(StatName)).Count(); s++)
                {
                    if (bestCreatures[s] == null || bestCreatures[s].Count == 0)
                    {
                        continue; // no creature has levelups in this stat or the stat is not used for this species
                    }

                    if (bestCreatures[s].Count == 1)
                    {
                        bestCreatures[s][0].topBreedingCreature = true;
                        continue;
                    }

                    for (int c = 0; c < bestCreatures[s].Count; c++)
                    {
                        bestCreatures[s][c].topBreedingCreature = true;
                        if (bestCreatures[s][c].sex != Sex.Male)
                            continue;

                        Creature currentCreature = bestCreatures[s][c];
                        // check how many best stat the male has
                        int maxval = 0;
                        for (int cs = 0; cs < Enum.GetNames(typeof(StatName)).Count(); cs++)
                        {
                            if (currentCreature.levelsWild[cs] == bestStat[cs])
                                maxval++;
                        }

                        if (maxval > 1)
                        {
                            // check now if the other males have only 1.
                            for (int oc = 0; oc < bestCreatures[s].Count; oc++)
                            {
                                if (bestCreatures[s][oc].sex != Sex.Male)
                                    continue;

                                if (oc == c)
                                    continue;

                                Creature otherMale = bestCreatures[s][oc];

                                int othermaxval = 0;
                                for (int ocs = 0; ocs < Enum.GetNames(typeof(StatName)).Count(); ocs++)
                                {
                                    if (otherMale.levelsWild[ocs] == bestStat[ocs])
                                        othermaxval++;
                                }
                                if (othermaxval == 1)
                                    bestCreatures[s][oc].topBreedingCreature = false;
                            }
                        }
                    }
                }
                if (noCreaturesInThisSpecies)
                {
                    continue;
                }

                // now we have a list of all candidates for breeding. Iterate on stats.
                for (int s = 0; s < Enum.GetNames(typeof(StatName)).Count(); s++)
                {
                    if (bestCreatures[s] != null)
                    {
                        for (int c = 0; c < bestCreatures[s].Count; c++)
                        {
                            // flag topstats in creatures
                            bestCreatures[s][c].topBreedingStats[s] = true;
                        }
                    }
                }
                foreach (Creature c in creatures)
                    c.setTopStatCount(considerStatHighlight);
            }
            toolStripProgressBar1.Visible = false;
        }

        /// <summary>
        /// Sets the parents according to the guids. Call after a file is loaded.
        /// </summary>
        /// <param name="creatures"></param>
        private void updateParents(IEnumerable<Creature> creatures)
        {
            List<Creature> placeholderAncestors = new List<Creature>();

            foreach (Creature c in creatures)
            {
                if (c.motherGuid != Guid.Empty || c.fatherGuid != Guid.Empty)
                {
                    Creature mother = null;
                    Creature father = null;
                    foreach (Creature p in creatureCollection.creatures)
                    {
                        if (c.motherGuid != Guid.Empty && c.motherGuid == p.guid)
                        {
                            mother = p;
                            if (father != null || c.fatherGuid == Guid.Empty)
                                break;
                        }
                        else if (c.fatherGuid != Guid.Empty && c.fatherGuid == p.guid)
                        {
                            father = p;
                            if (mother != null || c.motherGuid == Guid.Empty)
                                break;
                        }
                    }

                    if (mother == null) mother = ensurePlaceholderCreature(placeholderAncestors, c, c.motherArkId, c.motherGuid, c.motherName, Sex.Female);
                    if (father == null) father = ensurePlaceholderCreature(placeholderAncestors, c, c.fatherArkId, c.fatherGuid, c.fatherName, Sex.Male);

                    c.Mother = mother;
                    c.Father = father;
                }
            }

            creatureCollection.creatures.AddRange(placeholderAncestors);
        }

        /// <summary>
        /// Ensures the given placeholder ancestor exists in the list of placeholders.
        /// Does nothing when the details are not well specified.
        /// </summary>
        /// <param name="placeholders">List of placeholders to amend</param>
        /// <param name="tmpl">Descendant creature to use as a template</param>
        /// <param name="arkId">ArkId of creature to create. Only pass this if it's from an import</param>
        /// <param name="guid">GUID of creature to create</param>
        /// <param name="name">Name of the creature to create</param>
        /// <param name="sex">Sex of the creature to create</param>
        /// <returns></returns>
        private Creature ensurePlaceholderCreature(List<Creature> placeholders, Creature tmpl, long arkId, Guid guid, string name, Sex sex)
        {
            if (guid == Guid.Empty && arkId == 0) return null;
            var existing = placeholders.SingleOrDefault(ph => ph.guid == guid);
            if (existing != null) return existing;

            if (string.IsNullOrEmpty(name))
                name = (sex == Sex.Female ? "Mother" : "Father") + " of " + tmpl.name;

            var creature = new Creature(tmpl.species, name, tmpl.owner, tmpl.tribe, sex, new[] { -1, -1, -1, -1, -1, -1, -1, -1 },
                    levelStep: creatureCollection.getWildLevelStep())
            {
                guid = arkId != 0 ? Utils.ConvertArkIdToGuid(arkId) : guid,
                status = CreatureStatus.Unavailable,
                IsPlaceholder = true,
                ArkId = arkId
            };

            placeholders.Add(creature);

            return creature;
        }

        /// <summary>
        /// Sets the parentsof the incubation-timers according to the guids. Call after a file is loaded.
        /// </summary>
        /// <param name="cc"></param>
        private void updateIncubationParents(CreatureCollection cc)
        {
            foreach (Creature c in cc.creatures)
            {
                if (c.guid != Guid.Empty)
                {
                    foreach (IncubationTimerEntry it in cc.incubationListEntries)
                    {
                        if (c.guid == it.motherGuid)
                            it.mother = c;
                        else if (c.guid == it.fatherGuid)
                            it.father = c;
                    }
                }
            }
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
        private void creatureBoxListView_FindParents(object sender, Creature creature)
        {
            List<Creature>[] parents = findPossibleParents(creature);
            creatureBoxListView.parentListSimilarity = findParentSimilarities(parents, creature);
            creatureBoxListView.parentList = parents;
        }

        private List<Creature>[] findPossibleParents(Creature creature)
        {
            var fatherList = creatureCollection.creatures
                    .Where(cr => cr.species == creature.species && cr.sex == Sex.Male && cr != creature)
                    .OrderBy(cr => cr.name);
            var motherList = creatureCollection.creatures
                    .Where(cr => cr.species == creature.species && cr.sex == Sex.Female && cr != creature)
                    .OrderBy(cr => cr.name);

            // display new results
            return new List<Creature>[2] { motherList.ToList(), fatherList.ToList() };
        }

        private List<int>[] findParentSimilarities(List<Creature>[] parents, Creature creature)
        {
            // similarities (number of equal wildlevels as creature, to find parents easier)
            int e; // number of equal wildlevels
            List<int> motherListSimilarities = new List<int>();
            List<int> fatherListSimilarities = new List<int>();
            List<int>[] parentListSimilarities = new List<int>[2] { motherListSimilarities, fatherListSimilarities };

            if (parents[0] != null && parents[1] != null)
            {
                for (int ps = 0; ps < 2; ps++)
                {
                    foreach (Creature c in parents[ps])
                    {
                        e = 0;
                        for (int s = 0; s < 7; s++)
                        {
                            if (creature.levelsWild[s] >= 0 && creature.levelsWild[s] == c.levelsWild[s])
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
            setMessageLabelText();
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
                statPotentials1.speciesIndex = speciesSelector1.speciesIndex;
                statPotentials1.setLevels(testingIOs.Select(s => s.LevelWild).ToArray(), true);
            }
            else if (tabControlMain.SelectedTab == tabPageLibrary)
            {
                if (libraryNeedsUpdate)
                    filterLib();
            }
            else if (tabControlMain.SelectedTab == tabPagePedigree)
            {
                if (pedigreeNeedsUpdate && listViewLibrary.SelectedItems.Count > 0)
                {
                    Creature c = null;
                    if (listViewLibrary.SelectedItems.Count > 0)
                    {
                        c = (Creature)listViewLibrary.SelectedItems[0].Tag;
                        int s = Values.V.speciesNames.IndexOf(c.species);
                        pedigree1.EnabledColorRegions = s >= 0 ? Values.V.species[s].colors.Select(n => n.name != "").ToArray() : new bool[6] { true, true, true, true, true, true };
                    }
                    pedigree1.setCreature(c, true);
                    pedigreeNeedsUpdate = false;
                }
            }
            else if (tabControlMain.SelectedTab == tabPageTaming)
            {
                tamingControl1.setSpeciesIndex(speciesSelector1.speciesIndex);
            }
            else if (tabControlMain.SelectedTab == tabPageBreedingPlan)
            {
                if (breedingPlan1.CurrentSpecies == speciesSelector1.species)
                    breedingPlan1.updateIfNeeded();
                else
                {
                    breedingPlan1.setSpecies(speciesSelector1.species);
                }
            }
            else if (tabControlMain.SelectedTab == tabPageRaising)
            {
                raisingControl1.updateRaisingData(speciesSelector1.speciesIndex);
            }
            else if (tabControlMain.SelectedTab == tabPageMultiplierTesting)
            {
                statsMultiplierTesting1.setSpeciesIndex(speciesSelector1.speciesIndex);
            }
        }

        /// <summary>
        /// Call if the collection has changed and needs to be saved.
        /// </summary>
        /// <param name="changed">is the collection changed?</param>
        /// <param name="species">set to a specific species if only this species needs updates in the pedigree / breeding-planner. Set to "" if no species needs updates</param>
        private void setCollectionChanged(bool changed, string species = null)
        {
            if (changed)
            {
                if (species == null || pedigree1.creature != null && pedigree1.creature.species == species)
                    pedigreeNeedsUpdate = true;
                if (species == null || breedingPlan1.CurrentSpecies == species)
                    breedingPlan1.breedingPlanNeedsUpdate = true;
            }

            if (autoSave && changed)
            {
                // save changes automatically
                if (currentFileName != "" && autoSaveMinutes > 0 && (DateTime.Now - lastAutoSaveBackup).TotalMinutes > autoSaveMinutes)
                {
                    string filenameWOExt = Path.GetFileNameWithoutExtension(currentFileName);
                    File.Copy(currentFileName, Path.GetDirectoryName(currentFileName) + "\\" + filenameWOExt + "_backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xml");
                    lastAutoSaveBackup = DateTime.Now;
                    // delete oldest backupfile if more than a certain number
                    var directory = new DirectoryInfo(Path.GetDirectoryName(currentFileName));
                    var oldBackupfiles = directory.GetFiles()
                            .Where(f => f.Name.Length > filenameWOExt.Length + 8 &&
                                    f.Name.Substring(0, filenameWOExt.Length + 8) == filenameWOExt + "_backup_")
                            .OrderByDescending(f => f.LastWriteTime)
                            .Skip(3)
                            .ToList();
                    foreach (FileInfo f in oldBackupfiles)
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

                // save changes
                saveCollection();
                return; // function is called soon again from savecollection()
            }
            collectionDirty = changed;
            string fileName = Path.GetFileName(currentFileName);
            Text = $"ARK Smart Breeding{(fileName.Length > 0 ? " - " + fileName : "")}{(changed ? " *" : "")}";
        }

        /// <summary>
        /// Call this function with a creature c to put all its stats in the levelup-tester (and go to the tester-tab) to see what it could become
        /// </summary>
        /// <param name="c">the creature to test</param>
        /// <param name="virtualCreature">set to true if the creature is not in the library</param>
        private void editCreatureInTester(Creature c, bool virtualCreature = false)
        {
            if (c != null)
            {
                speciesSelector1.setSpecies(c.species);
                NumericUpDownTestingTE.Value = c.tamingEff >= 0 ? (decimal)c.tamingEff * 100 : 0;
                numericUpDownImprintingBonusTester.Value = (decimal)c.imprintingBonus * 100;
                if (c.isBred)
                    rbBredTester.Checked = true;
                else if (c.tamingEff > 0 || c.tamingEff == -2) // -2 is unknown (e.g. Giganotosaurus)
                    rbTamedTester.Checked = true;
                else rbWildTester.Checked = true;

                hiddenLevelsCreatureTester = c.levelsWild[7];
                for (int s = 0; s < 7; s++)
                {
                    if (c.levelsWild[s] > 0)
                        hiddenLevelsCreatureTester -= c.levelsWild[s];
                }

                for (int s = 0; s < 7; s++)
                {
                    testingIOs[s].LevelWild = c.levelsWild[s];
                    testingIOs[s].LevelDom = c.levelsDom[s];
                }
                tabControlMain.SelectedTab = tabPageStatTesting;
                setTesterInfoInputCreature(c, virtualCreature);
            }
        }

        private void extractBaby(Creature mother, Creature father)
        {
            if (mother != null && father != null)
            {
                speciesSelector1.setSpecies(mother.species);
                rbBredExtractor.Checked = true;
                numericUpDownImprintingBonusTester.Value = 0;

                creatureInfoInputExtractor.mother = mother;
                creatureInfoInputExtractor.father = father;
                creatureInfoInputExtractor.CreatureOwner = mother.owner;
                creatureInfoInputExtractor.CreatureTribe = mother.tribe;
                creatureInfoInputExtractor.CreatureServer = mother.server;
                updateParentListInput(creatureInfoInputExtractor);
                tabControlMain.SelectedTab = tabPageExtractor;
            }
        }

        private void updateAllTesterValues()
        {
            updateTorporInTester = false;
            for (int s = 0; s < 7; s++)
            {
                if (s == 6)
                    updateTorporInTester = true;
                testingStatIOsRecalculateValue(testingIOs[s]);
            }
            testingStatIOsRecalculateValue(testingIOs[7]);
        }

        private void NumericUpDownTestingTE_ValueChanged(object sender, EventArgs e)
        {
            updateAllTesterValues();
            lbWildLevelTester.Text = "PreTame Level: " + Math.Ceiling(Math.Round((testingIOs[7].LevelWild + 1) / (1 + NumericUpDownTestingTE.Value / 200), 6));
        }

        private void numericUpDownImprintingBonusTester_ValueChanged(object sender, EventArgs e)
        {
            updateAllTesterValues();
            // calculate number of imprintings
            if (Values.V.species[speciesSelector1.speciesIndex].breeding != null && Values.V.species[speciesSelector1.speciesIndex].breeding.maturationTimeAdjusted > 0)
                lbImprintedCount.Text = "(" + Math.Round((double)numericUpDownImprintingBonusTester.Value / (100 * Utils.imprintingGainPerCuddle(Values.V.species[speciesSelector1.speciesIndex].breeding.maturationTimeAdjusted, Values.V.babyCuddleIntervalMultiplier)), 2) + "×)";
            else lbImprintedCount.Text = "";
        }

        private void numericUpDownImprintingBonusExtractor_ValueChanged(object sender, EventArgs e)
        {
            // calculate number of imprintings
            if (Values.V.species[speciesSelector1.speciesIndex].breeding != null && Values.V.species[speciesSelector1.speciesIndex].breeding.maturationTimeAdjusted > 0)
                lbImprintingCuddleCountExtractor.Text = "(" + Math.Round((double)numericUpDownImprintingBonusExtractor.Value / (100 * Utils.imprintingGainPerCuddle(Values.V.species[speciesSelector1.speciesIndex].breeding.maturationTimeAdjusted, Values.V.babyCuddleIntervalMultiplier))) + "×)";
            else lbImprintingCuddleCountExtractor.Text = "";
        }

        private void setTesterInputsTamed(bool tamed)
        {
            for (int s = 0; s < 8; s++)
                testingIOs[s].postTame = tamed;
            lbNotYetTamed.Visible = !tamed;
        }

        private void checkBoxQuickWildCheck_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = !cbQuickWildCheck.Checked;
            if (!enabled)
            {
                clearAll();

                for (int s = 0; s < 8; s++)
                {
                    int lvlWild = (int)Math.Round((statIOs[s].Input - Values.V.species[speciesSelector1.speciesIndex].stats[s].BaseValue) / (Values.V.species[speciesSelector1.speciesIndex].stats[s].BaseValue * Values.V.species[speciesSelector1.speciesIndex].stats[s].IncPerWildLevel));
                    statIOs[s].LevelWild = lvlWild < 0 ? 0 : lvlWild;
                    statIOs[s].LevelDom = 0;
                }

                tamingControl1.setLevel(statIOs[7].LevelWild + 1, false);
                tamingControl1.setSpeciesIndex(speciesSelector1.speciesIndex);
                labelTamingInfo.Text = tamingControl1.quickTamingInfos;
            }
            panelWildTamedBred.Enabled = enabled;
            groupBoxDetailsExtractor.Enabled = enabled;
            numericUpDownLevel.Enabled = enabled;
            button2TamingCalc.Visible = !enabled;
            groupBoxTamingInfo.Visible = !enabled;
        }

        /// <summary>
        /// Updates the values in the testing-statIOs
        /// </summary>
        /// <param name="sIo"></param>
        private void testingStatIOValueUpdate(StatIO sIo)
        {
            testingStatIOsRecalculateValue(sIo);

            // update Torpor-level if changed value is not from torpor-StatIO
            if (updateTorporInTester && sIo != statTestingTorpor)
            {
                int torporLvl = 0;
                for (int s = 0; s < 7; s++)
                {
                    torporLvl += testingIOs[s].LevelWild > 0 ? testingIOs[s].LevelWild : 0;
                }
                testingIOs[7].LevelWild = torporLvl + hiddenLevelsCreatureTester;
            }

            int domLevels = 0;
            for (int s = 0; s < 8; s++)
            {
                domLevels += testingIOs[s].LevelDom;
            }
            labelDomLevelSum.Text = $"Dom Levels: {domLevels}/{creatureCollection.maxDomLevel}";
            labelDomLevelSum.BackColor = domLevels > creatureCollection.maxDomLevel ? Color.LightSalmon : Color.Transparent;
            labelTesterTotalLevel.Text = $"Total Levels: {testingIOs[7].LevelWild + domLevels + 1}/{testingIOs[7].LevelWild + 1 + creatureCollection.maxDomLevel}";
            creatureInfoInputTester.parentListValid = false;

            int[] levelsWild = testingIOs.Select(s => s.LevelWild).ToArray();
            if (!testingIOs[2].Enabled) levelsWild[2] = 0;
            radarChart1.setLevels(levelsWild);
            statPotentials1.setLevels(levelsWild, false);
            //statGraphs1.setGraph(sE, 0, testingIOs[0].LevelWild, testingIOs[0].LevelDom, !radioButtonTesterWild.Checked, (double)NumericUpDownTestingTE.Value / 100, (double)numericUpDownImprintingBonusTester.Value / 100);

            if (sIo == statTestingTorpor)
                lbWildLevelTester.Text = "PreTame Level: " + Math.Ceiling(Math.Round((testingIOs[7].LevelWild + 1) / (1 + NumericUpDownTestingTE.Value / 200), 6));
        }

        private void testingStatIOsRecalculateValue(StatIO sIo)
        {
            sIo.BreedingValue = Stats.calculateValue(speciesSelector1.speciesIndex, sIo.statIndex, sIo.LevelWild, 0, true, 1, 0);
            sIo.Input = Stats.calculateValue(speciesSelector1.speciesIndex, sIo.statIndex, sIo.LevelWild, sIo.LevelDom,
                    rbTamedTester.Checked || rbBredTester.Checked,
                    rbBredTester.Checked ? 1 : (double)NumericUpDownTestingTE.Value / 100,
                    rbBredTester.Checked ? (double)numericUpDownImprintingBonusTester.Value / 100 : 0);
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

        private void listViewLibrary_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteSelectedCreatures();
            }
            else if (e.KeyCode == Keys.F2)
            {
                if (listViewLibrary.SelectedIndices.Count > 0)
                    editCreatureInTester((Creature)listViewLibrary.Items[listViewLibrary.SelectedIndices[0]].Tag);
            }
            else if (e.KeyCode == Keys.F3)
            {
                if (listViewLibrary.SelectedIndices.Count > 0)
                    showMultiSetter();
            }
            else if (e.KeyCode == Keys.A && e.Control)
            {
                // select all list-entries
                reactOnSelectionChange = false;
                listViewLibrary.BeginUpdate();
                foreach (ListViewItem i in listViewLibrary.Items)
                    i.Selected = true;
                listViewLibrary.EndUpdate();
                reactOnSelectionChange = true;
                listViewLibrary_SelectedIndexChanged(null, null);
            }
        }

        private void exportForSpreadsheet()
        {
            if (tabControlMain.SelectedTab == tabPageLibrary)
            {
                if (listViewLibrary.SelectedItems.Count > 0)
                {
                    // header
                    string output = "Species\tName\tSex\tOwner\t" +
                            "HPw\tStw\tOxw\tFow\tWew\tDmw\tSpw\tTow\tHPd\tStd\tOxd\tFod\tWed\tDmd\tSpd\tTod\t" +
                            "HPb\tStb\tOxb\tFob\tWeb\tDmb\tSpb\tTob\tHPc\tStc\tOxc\tFoc\tWec\tDmc\tSpc\tToc\t" +
                            "mother\tfather\tNotes\tColor0\tColor1\tColor2\tColor3\tColor4\tColor5";

                    foreach (ListViewItem l in listViewLibrary.SelectedItems)
                    {
                        Creature c = (Creature)l.Tag;
                        output += "\n" + c.species + "\t" + c.name + "\t" + c.sex + "\t" + c.owner;
                        for (int s = 0; s < 8; s++)
                        {
                            output += "\t" + c.levelsWild[s];
                        }
                        for (int s = 0; s < 8; s++)
                        {
                            output += "\t" + c.levelsDom[s];
                        }
                        for (int s = 0; s < 8; s++)
                        {
                            output += $"\t{c.valuesBreeding[s] * (Utils.precision(s) == 3 ? 100 : 1)}{(Utils.precision(s) == 3 ? "%" : "")}";
                        }
                        for (int s = 0; s < 8; s++)
                        {
                            output += $"\t{c.valuesDom[s] * (Utils.precision(s) == 3 ? 100 : 1)}{(Utils.precision(s) == 3 ? "%" : "")}";
                        }
                        output += $"\t{(c.Mother != null ? c.Mother.name : "")}\t{(c.Father != null ? c.Father.name : "")}\t{(c.note != null ? c.note.Replace("\r", "").Replace("\n", " ") : "")}";
                        for (int cl = 0; cl < 6; cl++)
                        {
                            output += "\t" + c.colors[cl];
                        }
                    }
                    Clipboard.SetText(output);
                }
                else
                    MessageBox.Show("No creatures in the library selected to copy to the clipboard", "No Creatures Selected",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (tabControlMain.SelectedTab == tabPageExtractor)
                CopyExtractionToClipboard();
        }

        private void exportAsTextToClipboard(Creature c, bool breeding = true, bool ARKml = true)
        {
            if (c != null)
            {
                double colorFactor = 100d / creatureCollection.maxChartLevel;
                bool wild = c.tamingEff == -2;
                string modifierText = "";
                if (!breeding)
                {
                    if (wild) modifierText = ", wild";
                    else if (c.tamingEff < 1) modifierText = ", TE: " + Math.Round(100 * c.tamingEff, 1) + "%";
                    else if (c.imprintingBonus > 0) modifierText = ", Impr: " + Math.Round(100 * c.imprintingBonus, 2) + "%";
                }

                string output = c.name + " (" + (ARKml ? Utils.getARKml(c.species, 50, 172, 255) : c.species)
                        + ", Lvl " + (breeding ? c.levelHatched : c.level) + modifierText + (c.sex != Sex.Unknown ? ", " + c.sex : "") + "): ";
                for (int s = 0; s < 8; s++)
                {
                    if (c.levelsWild[s] >= 0) // ignore unknown oxygen / speed
                        output += Utils.statName(s, true) + ": " + (breeding ? c.valuesBreeding[s] : c.valuesDom[s]) * (Utils.precision(s) == 3 ? 100 : 1) + (Utils.precision(s) == 3 ? "%" : "") +
                                " (" + (ARKml ? Utils.getARKmlFromPercent(c.levelsWild[s].ToString(), (int)(c.levelsWild[s] * (s == 7 ? colorFactor / 7 : colorFactor))) : c.levelsWild[s].ToString()) +
                                (ARKml ? breeding || s == 7 ? "" : ", " + Utils.getARKmlFromPercent(c.levelsDom[s].ToString(), (int)(c.levelsDom[s] * colorFactor)) : breeding || s == 7 ? "" : ", " + c.levelsDom[s]) + "); ";
                }
                Clipboard.SetText(output.Substring(0, output.Length - 1));
            }
        }

        private void exportSelectedCreatureToClipboard(bool breeding = true, bool ARKml = true)
        {
            if (tabControlMain.SelectedTab == tabPageStatTesting || tabControlMain.SelectedTab == tabPageExtractor)
            {
                bool fromExtractor = tabControlMain.SelectedTab == tabPageExtractor;
                if (!fromExtractor || extractor.validResults)
                {
                    CreatureInfoInput input;
                    bool bred;
                    double te, imprinting;
                    string species;
                    if (fromExtractor)
                    {
                        input = creatureInfoInputExtractor;
                        species = speciesSelector1.species;
                        bred = rbBredExtractor.Checked;
                        te = extractor.uniqueTE();
                        imprinting = extractor.imprintingBonus;
                    }
                    else
                    {
                        input = creatureInfoInputTester;
                        species = speciesSelector1.species;
                        bred = rbBredTester.Checked;
                        te = (double)NumericUpDownTestingTE.Value / 100;
                        imprinting = (double)numericUpDownImprintingBonusTester.Value / 100;
                    }

                    var levelStep = creatureCollection.getWildLevelStep();
                    Creature creature = new Creature(species, input.CreatureName, input.CreatureOwner, input.CreatureTribe, input.CreatureSex, getCurrentWildLevels(fromExtractor), getCurrentDomLevels(fromExtractor), te, bred, imprinting, levelStep)
                    {
                        colors = input.RegionColors,
                        ArkId = input.ArkId
                    };
                    creature.recalculateCreatureValues(levelStep);
                    exportAsTextToClipboard(creature, breeding, ARKml);
                }
                else
                    MessageBox.Show("There is no valid extracted creature to export.", "No valid data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (listViewLibrary.SelectedItems.Count > 0)
                    exportAsTextToClipboard((Creature)listViewLibrary.SelectedItems[0].Tag, breeding, ARKml);
                else
                    MessageBox.Show("No creatures in the library selected to copy to the clipboard", "No Creatures Selected",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void forSpreadsheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportForSpreadsheet();
        }

        private void plainTextbreedingValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportSelectedCreatureToClipboard(true, false);
        }

        private void plainTextbreedingValuesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            exportSelectedCreatureToClipboard(true, false);
        }

        private void plainTextcurrentValuesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            exportSelectedCreatureToClipboard(false, false);
        }

        private void plainTextcurrentValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportSelectedCreatureToClipboard(false, false);
        }

        private void copyCreatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedItems.Count > 0)
                copyCreatureToClipboard((Creature)listViewLibrary.SelectedItems[0].Tag);
            else
                MessageBox.Show("No creatures in the library selected to copy to the clipboard", "No Creatures Selected",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void importValuesFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pasteCreatureValuesFromClipboard();
        }

        private void pasteCreatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pasteCreatureFromClipboardToTester();
        }

        private void copyCreatureToClipboard(Creature c)
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
        /// import XML-data with all creature data
        /// </summary>
        private void pasteCreatureFromClipboardToTester()
        {
            string clpb = Clipboard.GetText();
            if (clpb.Length > 0)
            {
                Creature c;
                var serializer = new XmlSerializer(typeof(Creature));
                using (var reader = new StringReader(clpb))
                {
                    try
                    {
                        c = (Creature)serializer.Deserialize(reader);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Invalid Data in clipboard. Couldn\'t paste creature-data\nErrormessage:\n\n{e.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                updateParents(new List<Creature> { c });
                editCreatureInTester(c, true);
            }
        }

        /// <summary>
        /// import creature values from plain text
        /// </summary>
        private void pasteCreatureValuesFromClipboard()
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
                    double[] sv = new double[8];
                    int[] wl = new int[8];
                    int[] dl = new int[8];
                    for (int s = 0; s < 8; s++)
                    {
                        double.TryParse(m.Groups[7 + 3 * s].Value, out sv[s]);
                        int.TryParse(m.Groups[8 + 3 * s].Value, out wl[s]);
                        if (s < 7)
                            int.TryParse(m.Groups[9 + 3 * s].Value, out dl[s]);
                    }
                    // percentage values
                    sv[5] *= 0.01;
                    sv[6] *= 0.01;

                    int.TryParse(m.Groups[3].Value, out int totalLevel);
                    double.TryParse(m.Groups[4].Value, out double te);
                    te *= .01;
                    double.TryParse(m.Groups[5].Value, out double ib);
                    ib *= .01;

                    var cv = new CreatureValues(m.Groups[2].Value, m.Groups[1].Value, "", "", sex, sv, totalLevel, te, te, te > 0 || ib > 0, ib > 0, ib, false, null, null)
                    {
                        levelsWild = wl,
                        levelsDom = dl
                    };
                    if (tabControlMain.SelectedTab == tabPageStatTesting)
                        setCreatureValuesToTester(cv);
                    else
                        setCreatureValuesToExtractor(cv);
                }
            }
        }

        private void buttonRecalculateTops_Click(object sender, EventArgs e)
        {
            for (int s = 0; s < 8; s++)
                considerStatHighlight[s] = checkedListBoxConsiderStatTop.GetItemChecked(s);
            // recalculate topstats
            calculateTopStats(creatureCollection.creatures);
            filterLib();
        }

        private void removeCooldownGrowingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewLibrary.BeginUpdate();
            foreach (ListViewItem i in listViewLibrary.SelectedItems)
            {
                Creature c = (Creature)i.Tag;
                if (c.cooldownUntil > DateTime.Now)
                    c.cooldownUntil = DateTime.Now;
                if (c.growingUntil > DateTime.Now)
                    c.growingUntil = DateTime.Now;

                i.SubItems[11].Text = "-"; // LVI index
                // color for cooldown
                cooldownColors(c, out Color forecolor, out Color backcolor);
                i.SubItems[11].ForeColor = forecolor;
                i.SubItems[11].BackColor = backcolor;
            }
            breedingPlan1.breedingPlanNeedsUpdate = true;
            listViewLibrary.EndUpdate();
        }

        private void aliveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setStatusOfSelected(CreatureStatus.Available);
        }

        private void deadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setStatusOfSelected(CreatureStatus.Dead);
        }

        private void unavailableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setStatusOfSelected(CreatureStatus.Unavailable);
        }

        private void obeliskToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            setStatusOfSelected(CreatureStatus.Obelisk);
        }

        private void setStatusOfSelected(CreatureStatus s)
        {
            List<Creature> cs = new List<Creature>();
            foreach (ListViewItem i in listViewLibrary.SelectedItems)
                cs.Add((Creature)i.Tag);
            if (cs.Count > 0)
                setStatus(cs, s);
        }

        private void setStatus(IEnumerable<Creature> cs, CreatureStatus s)
        {
            bool changed = false;
            List<string> species = new List<string>();
            foreach (Creature c in cs)
            {
                if (c.status != s)
                {
                    changed = true;
                    c.status = s;
                    if (!species.Contains(c.species))
                        species.Add(c.species);
                }
            }
            if (changed)
            {
                // update list / recalculate topstats
                calculateTopStats(creatureCollection.creatures.Where(c => species.Contains(c.species)).ToList());
                filterLib();
                setCollectionChanged(true, species.Count == 1 ? species[0] : null);
            }
        }

        private void editAllSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showMultiSetter();
        }

        private void multiSetterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showMultiSetter();
        }

        private void showMultiSetter()
        {
            // shows a dialog to set multiple settings to all selected creatures
            if (listViewLibrary.SelectedIndices.Count <= 0)
                return;
            Creature c = new Creature();
            List<bool> appliedSettings = new List<bool>();
            for (int i = 0; i < 13; i++)
                appliedSettings.Add(false);
            List<Creature> selectedCreatures = new List<Creature>();

            // check if multiple species are selected
            bool multipleSpecies = false;
            string sp = ((Creature)listViewLibrary.SelectedItems[0].Tag).species;
            c.species = sp;
            foreach (ListViewItem i in listViewLibrary.SelectedItems)
            {
                selectedCreatures.Add((Creature)i.Tag);
                if (((Creature)i.Tag).species != sp)
                {
                    multipleSpecies = true;
                }
            }
            List<Creature>[] parents = null;
            if (!multipleSpecies) parents = findPossibleParents(c);

            MultiSetter ms = new MultiSetter(selectedCreatures, appliedSettings, parents, creatureCollection.tags, Values.V.speciesNames);

            if (ms.ShowDialog() == DialogResult.OK)
            {
                if (ms.ParentsChanged)
                    updateParents(selectedCreatures);
                if (ms.TagsChanged)
                    createCreatureTagList();
                if (ms.SpeciesChanged)
                    updateSpeciesLists(creatureCollection.creatures);
                createOwnerList();
                setCollectionChanged(true, !multipleSpecies ? sp : null);
                recalculateTopStatsIfNeeded();
                filterLib();
            }
            ms.Dispose();
        }

        private void bestBreedingPartnersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count > 0)
            {
                Creature sc = (Creature)listViewLibrary.Items[listViewLibrary.SelectedIndices[0]].Tag;
                showBestBreedingPartner(sc);
            }
        }

        private void showBestBreedingPartner(Creature c)
        {
            if (c.status != CreatureStatus.Available
                    && MessageBox.Show("Selected Creature is currently not marked as \"Available\" and probably cannot be used for breeding right now. " +
                            "Do you want to change its status to \"Available\"?",
                            "Selected Creature not Available",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                setStatus(new List<Creature> { c }, CreatureStatus.Available);
                breedingPlan1.breedingPlanNeedsUpdate = false;
            }
            else
            {
                breedingPlan1.breedingPlanNeedsUpdate = true;
            }
            speciesSelector1.setSpecies(c.species);
            breedingPlan1.determineBestBreeding(c);
            tabControlMain.SelectedTab = tabPageBreedingPlan;
        }

        private void creatureInfoInputTester_Save2Library_Clicked(CreatureInfoInput sender)
        {
            if (creatureTesterEdit == null)
                return;
            // check if wild levels are changed, if yes warn that the creature can become invalid
            bool wildChanged = Math.Abs(creatureTesterEdit.tamingEff - (double)NumericUpDownTestingTE.Value / 100) > .0005;
            if (!wildChanged)
            {
                int[] wildLevels = getCurrentWildLevels(false);
                for (int s = 0; s < 8; s++)
                {
                    if (wildLevels[s] != creatureTesterEdit.levelsWild[s])
                    {
                        wildChanged = true;
                        break;
                    }
                }
            }
            if (wildChanged && MessageBox.Show("The wild levels or the taming-effectiveness were changed. Save values anyway?\n" +
                    "Only save if the wild levels or taming-effectiveness were extracted wrongly!\nIf you are not sure, don't save. " +
                    "The breeding-values could become invalid.",
                    "Wild levels have been changed",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Exclamation) != DialogResult.OK)
            {
                return;
            }

            // Ids: ArkId and Guid
            //if (!IsArkIdUniqueOrOnlyPlaceHolder(creatureTesterEdit)) { return; }

            bool statusChanged = creatureTesterEdit.status != creatureInfoInputTester.CreatureStatus
                    || creatureTesterEdit.owner != creatureInfoInputTester.CreatureOwner
                    || creatureTesterEdit.mutationsMaternal != creatureInfoInputTester.MutationCounterMother
                    || creatureTesterEdit.mutationsPaternal != creatureInfoInputTester.MutationCounterFather;
            bool parentsChanged = creatureTesterEdit.Mother != creatureInfoInputTester.mother || creatureTesterEdit.Father != creatureInfoInputTester.father;
            creatureTesterEdit.levelsWild = getCurrentWildLevels(false);
            creatureTesterEdit.levelsDom = getCurrentDomLevels(false);
            creatureTesterEdit.tamingEff = (double)NumericUpDownTestingTE.Value / 100;
            creatureTesterEdit.isBred = rbBredTester.Checked;
            creatureTesterEdit.imprintingBonus = (double)numericUpDownImprintingBonusTester.Value / 100;

            creatureTesterEdit.name = creatureInfoInputTester.CreatureName;
            creatureTesterEdit.sex = creatureInfoInputTester.CreatureSex;
            creatureTesterEdit.owner = creatureInfoInputTester.CreatureOwner;
            creatureTesterEdit.tribe = creatureInfoInputTester.CreatureTribe;
            creatureTesterEdit.server = creatureInfoInputTester.CreatureServer;
            creatureTesterEdit.Mother = creatureInfoInputTester.mother;
            creatureTesterEdit.Father = creatureInfoInputTester.father;
            creatureTesterEdit.note = creatureInfoInputTester.CreatureNote;
            creatureTesterEdit.status = creatureInfoInputTester.CreatureStatus;
            creatureTesterEdit.cooldownUntil = creatureInfoInputTester.Cooldown;
            creatureTesterEdit.growingUntil = creatureInfoInputTester.Grown;
            creatureTesterEdit.domesticatedAt = creatureInfoInputTester.domesticatedAt;
            creatureTesterEdit.neutered = creatureInfoInputTester.Neutered;
            creatureTesterEdit.mutationsMaternal = creatureInfoInputTester.MutationCounterMother;
            creatureTesterEdit.mutationsPaternal = creatureInfoInputTester.MutationCounterFather;
            creatureTesterEdit.colors = creatureInfoInputTester.RegionColors;
            creatureTesterEdit.ArkId = creatureInfoInputTester.ArkId;

            if (wildChanged)
                calculateTopStats(creatureCollection.creatures.Where(c => c.species == creatureTesterEdit.species).ToList());
            updateCreatureValues(creatureTesterEdit, statusChanged);

            if (parentsChanged)
                creatureTesterEdit.recalculateAncestorGenerations();

            setTesterInfoInputCreature();
            tabControlMain.SelectedTab = tabPageLibrary;
        }

        private void setTesterInfoInputCreature(Creature c = null, bool virtualCreature = false)
        {
            bool enable = c != null; // set to a creature, or clear
            creatureInfoInputTester.ShowSaveButton = enable && !virtualCreature;
            labelCurrentTesterCreature.Text = enable ? "Current Creature: " + c.name : "";
            if (enable)
            {
                creatureInfoInputTester.mother = c.Mother;
                creatureInfoInputTester.father = c.Father;
                creatureInfoInputTester.CreatureName = c.name;
                creatureInfoInputTester.CreatureSex = c.sex;
                creatureInfoInputTester.CreatureOwner = c.owner;
                creatureInfoInputTester.CreatureTribe = c.tribe;
                creatureInfoInputTester.CreatureServer = c.server;
                creatureInfoInputTester.CreatureStatus = c.status;
                creatureInfoInputTester.CreatureNote = c.note;
                creatureInfoInputTester.Cooldown = c.cooldownUntil;
                creatureInfoInputTester.Grown = c.growingUntil;
                creatureInfoInputTester.domesticatedAt = c.domesticatedAt.Year < 2000 ? DateTime.Now : c.domesticatedAt;
                creatureInfoInputTester.Neutered = c.neutered;
                creatureInfoInputTester.RegionColors = c.colors;
                creatureInfoInputTester.SetArkId(c.ArkId, c.ArkIdImported);
                updateParentListInput(creatureInfoInputTester);
                creatureInfoInputTester.MutationCounterMother = c.mutationsMaternal;
                creatureInfoInputTester.MutationCounterFather = c.mutationsPaternal;
            }
            else
            {
                creatureInfoInputTester.mother = null;
                creatureInfoInputTester.father = null;
                creatureInfoInputTester.CreatureName = "";
                creatureInfoInputTester.CreatureSex = Sex.Unknown;
                creatureInfoInputTester.CreatureOwner = "";
                creatureInfoInputTester.CreatureTribe = "";
                creatureInfoInputTester.CreatureServer = "";
                creatureInfoInputTester.CreatureStatus = CreatureStatus.Available;
                creatureInfoInputTester.CreatureNote = "";
                creatureInfoInputTester.Cooldown = DateTime.Now.AddHours(-1);
                creatureInfoInputTester.Grown = DateTime.Now.AddHours(-1);
                creatureInfoInputTester.domesticatedAt = DateTime.Now;
                creatureInfoInputTester.Neutered = false;
                creatureInfoInputTester.RegionColors = new int[6];
                creatureInfoInputTester.SetArkId(0, false);
                creatureInfoInputTester.MutationCounterMother = 0;
                creatureInfoInputTester.parentListValid = false;
            }
            creatureTesterEdit = c;
        }

        private void toolStripButtonSettings_Click(object sender, EventArgs e)
        {
            openSettingsDialog();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openSettingsDialog();
        }

        private void openSettingsDialog(int page = 0)
        {
            using (Settings settingsfrm = new Settings(creatureCollection, page))
            {
                if (settingsfrm.ShowDialog() == DialogResult.OK)
                {
                    if (speechRecognition != null && settingsfrm.WildMaxChanged)
                        speechRecognition.updateNeeded = true;
                    applySettingsToValues();
                    if (settingsfrm.LanguageChanged) setLocalizations();
                    autoSave = Properties.Settings.Default.autosave;
                    autoSaveMinutes = Properties.Settings.Default.autosaveMinutes;
                    creatureBoxListView.maxDomLevel = creatureCollection.maxDomLevel;
                    fileSync.changeFile(currentFileName); // only to trigger the update, filename is not changed

                    setCollectionChanged(true);
                }
            }
        }

        /// <summary>
        /// Display the wild-levels, assuming it's a wild creature. Used for quick checking
        /// </summary>
        /// <param name="sIO"></param>
        private void statIOQuickWildLevelCheck(StatIO sIO)
        {
            clearExtractionCreatureData = true; // as soon as the user changes stat-values, it's assumed it's not an exported creature anymore
            if (cbQuickWildCheck.Checked)
            {
                int lvlWild = (int)Math.Round((sIO.Input - Values.V.species[speciesSelector1.speciesIndex].stats[sIO.statIndex].BaseValue) / (Values.V.species[speciesSelector1.speciesIndex].stats[sIO.statIndex].BaseValue * Values.V.species[speciesSelector1.speciesIndex].stats[sIO.statIndex].IncPerWildLevel));
                sIO.LevelWild = lvlWild < 0 ? 0 : lvlWild;
                sIO.LevelDom = 0;
                if (sIO.statIndex == 7)
                {
                    tamingControl1.setLevel(statIOs[7].LevelWild + 1, false);
                    tamingControl1.setSpeciesIndex(speciesSelector1.speciesIndex);
                    labelTamingInfo.Text = tamingControl1.quickTamingInfos;
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
            deleteSelectedCreatures();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            setStatusOfSelected(CreatureStatus.Available);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            setStatusOfSelected(CreatureStatus.Unavailable);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            setStatusOfSelected(CreatureStatus.Dead);
        }

        private void obeliskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setStatusOfSelected(CreatureStatus.Obelisk);
        }

        private void cryopodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setStatusOfSelected(CreatureStatus.Cryopod);
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

        private void createTimer(string name, DateTime time, Creature c, string group)
        {
            timerList1.addTimer(name, time, c, group);
        }

        private void setCreatureValuesToExtractor(Creature c, bool onlyWild = false)
        {
            if (c != null)
            {
                int sI = Values.V.speciesNames.IndexOf(c.species);
                if (sI >= 0)
                {
                    clearAll();
                    // copy values over to extractor
                    for (int s = 0; s < 8; s++)
                        statIOs[s].Input = onlyWild ? Stats.calculateValue(sI, s, c.levelsWild[s], 0, true, c.tamingEff, c.imprintingBonus) : c.valuesDom[s];
                    speciesSelector1.setSpeciesIndex(sI);

                    if (c.isBred) rbBredExtractor.Checked = true;
                    else if (c.tamingEff >= 0) rbTamedExtractor.Checked = true;
                    else rbWildExtractor.Checked = true;

                    numericUpDownImprintingBonusExtractor.ValueSave = (decimal)c.imprintingBonus * 100;
                    // set total level
                    int level = onlyWild ? c.levelsWild[7] : c.level;
                    numericUpDownLevel.ValueSave = level;

                    tabControlMain.SelectedTab = tabPageExtractor;
                }
                else
                    MessageBox.Show("Unknown Species. Try to update the species-stats, or redownload the tool.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void testEnteredDrag(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void fileDropedOnExtractor(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                string file = files[0];
                if (File.GetAttributes(file).HasFlag(FileAttributes.Directory))
                {
                    showExportedCreatureListControl();
                    exportedCreatureList.loadFilesInFolder(file);
                }
                else if (file.Substring(file.Length - 4).ToLower() == ".ini")
                {
                    extractExportedFileInExtractor(file);
                }
                else
                    doOCR(files[0]);
            }
        }

        public void doOCR(string imageFilePath = "", bool manuallyTriggered = true)
        {
            cbQuickWildCheck.Checked = false;

            double[] OCRvalues = ArkOCR.OCR.doOCR(out string debugText, out string dinoName, out string species, out string ownerName, out string tribeName, out Sex sex, imageFilePath, manuallyTriggered);

            ocrControl1.output.Text = debugText;
            if (OCRvalues.Length <= 1)
                return;
            numericUpDownLevel.ValueSave = (decimal)OCRvalues[9];

            creatureInfoInputExtractor.CreatureName = dinoName;
            if (!creatureInfoInputExtractor.OwnerLock)
                creatureInfoInputExtractor.CreatureOwner = ownerName;
            if (!creatureInfoInputExtractor.TribeLock)
                creatureInfoInputExtractor.CreatureTribe = tribeName;
            creatureInfoInputExtractor.CreatureSex = sex;
            creatureInfoInputExtractor.RegionColors = new int[6];
            creatureInfoInputTester.SetArkId(0, false);

            for (int i = 0; i < 8; i++)
            {
                if (statIOs[i].percent)
                    statIOs[i].Input = OCRvalues[i] / 100.0;
                else
                    statIOs[i].Input = OCRvalues[i];
            }

            // use imprinting if existing
            if (OCRvalues.Length > 8 && OCRvalues[8] >= 0 && (OCRvalues[8] <= 100 || creatureCollection.allowMoreThanHundredImprinting))
            {
                rbBredExtractor.Checked = true;
                numericUpDownImprintingBonusExtractor.ValueSave = (decimal)OCRvalues[8];
            }
            else
            {
                rbTamedExtractor.Checked = true;
            }

            // fixing ocr species names. TODO: global name-fixing?
            switch (species)
            {
                case "DireBear":
                    species = "Dire Bear";
                    break;
                case "DirePolarBear":
                    species = "Dire Polar Bear";
                    break;
                case "DirePoiarBear":
                    species = "Dire Polar Bear";
                    break;
                case "Paraceratherium":
                    species = "Paracer";
                    break;
            }

            if (!manuallyTriggered || cbGuessSpecies.Checked)
            {
                List<int> possibleDinos = determineSpeciesFromStats(OCRvalues, species);

                if (possibleDinos.Count == 1)
                {
                    if (possibleDinos[0] >= 0 && possibleDinos[0] < Values.V.speciesNames.Count)
                        speciesSelector1.setSpeciesIndex(possibleDinos[0]);
                    extractLevels(true); // only one possible dino, use that one
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
                        int newindex = (possibleDinos.IndexOf(lastOCRSpecies) + 1) % possibleDinos.Count;
                        speciesSelector1.setSpeciesIndex(possibleDinos[newindex]);
                        lastOCRSpecies = possibleDinos[newindex];
                        lastOCRValues = OCRvalues;
                        extractLevels(true);
                    }
                    else
                    { // automated, or first manual attempt at new values
                        bool foundPossiblyGood = false;
                        for (int dinooption = 0; dinooption < possibleDinos.Count() && foundPossiblyGood == false; dinooption++)
                        {
                            // if the last OCR'ed values are the same as this one, the user may not be happy with the dino species selection and want another one
                            // so we'll cycle to the next one, but only if the OCR is manually triggered, on autotrigger (ie, overlay), don't change
                            speciesSelector1.setSpeciesIndex(possibleDinos[dinooption]);
                            lastOCRSpecies = possibleDinos[dinooption];
                            lastOCRValues = OCRvalues;
                            foundPossiblyGood = extractLevels();
                        }
                    }
                }
            }
            else
            {
                extractLevels();
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

        private List<int> determineSpeciesFromStats(double[] stats, string species)
        {
            // todo implement https://en.wikipedia.org/wiki/Levenshtein_distance
            List<int> possibleDinos = new List<int>();

            // for wild dinos, we can get the name directly.
            System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
            species = textInfo.ToTitleCase(species.ToLower());
            int sI = Values.V.speciesNames.IndexOf(species);
            if (sI >= 0)
            {
                possibleDinos.Add(sI);
                return possibleDinos;
            }

            // if dice-coefficient is promising, just take that
            var scores = Values.V.speciesNames.Select(n => new { Score = DiceCoefficient.diceCoefficient(n.Replace(" ", ""), species.Replace(" ", "")), Name = n }).OrderByDescending(o => o.Score);
            if (scores.First().Score > 0.4)
            {
                possibleDinos.Add(Values.V.speciesNames.IndexOf(scores.First().Name));
                return possibleDinos;
            }

            if (stats.Length > 8 && stats[8] > 0)
            {
                // creature is imprinted, the following algorithm cannot handle this yet. use current selected species
                possibleDinos.Add(speciesSelector1.speciesIndex);
                return possibleDinos;
            }

            for (int i = 0; i < Values.V.species.Count; i++)
            {
                if (i == speciesSelector1.speciesIndex) continue; // the currently selected species is ignored here and set as top priority at the end

                bool possible = true;
                // check that all stats are possible (no negative levels)
                double baseValue;
                double incWild;
                double possibleLevel;
                for (int s = 7; s >= 0; s--)
                {
                    baseValue = Values.V.species[i].stats[s].BaseValue;
                    incWild = Values.V.species[i].stats[s].IncPerWildLevel;
                    if (incWild > 0)
                    {
                        //possibleLevel = ((statIOs[s].Input - Values.V.species[i].stats[s].AddWhenTamed) - baseValue) / (baseValue * incWild); // this fails if creature is wild
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
                baseValue = Values.V.species[i].stats[7].BaseValue;
                incWild = Values.V.species[i].stats[7].IncPerWildLevel;

                possibleLevel = (statIOs[7].Input - Values.V.species[i].stats[7].AddWhenTamed - baseValue) / (baseValue * incWild);
                double possibleLevelWild = (statIOs[7].Input - baseValue) / (baseValue * incWild);

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
                baseValue = Values.V.species[i].stats[4].BaseValue;
                incWild = Values.V.species[i].stats[4].IncPerWildLevel;
                possibleLevel = (statIOs[4].Input - Values.V.species[i].stats[4].AddWhenTamed - baseValue) / (baseValue * incWild);

                if (possibleLevel < 0 || possibleLevel > (double)numericUpDownLevel.Value - 1)
                    continue;

                if (Math.Round(possibleLevel, 3) != (int)possibleLevel || possibleLevel > (double)numericUpDownLevel.Value / 2)
                    likely = false;

                if (statIOs[4].Input != 0 && baseValue == 0)
                    likely = false; // having an oxygen value for non-oxygen dino is a disqualifier

                if (likely)
                    possibleDinos.Insert(0, i);
                else
                    possibleDinos.Add(i);
            }

            if (speciesSelector1.speciesIndex >= 0)
                possibleDinos.Insert(0, speciesSelector1.speciesIndex); // adding the currently selected creature in the combobox as first priority. the user might already have that selected
            return possibleDinos;
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
                overlay.initLabelPositions();
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
                var tttt = Screen.AllScreens;
                Screen scr = Screen.FromHandle(mwhd);
                overlay.Location = scr.WorkingArea.Location;
            }

            overlay.Visible = cbToggleOverlay.Checked;
            overlay.enableOverlayTimer = overlay.Visible;

            if (speechRecognition != null)
                speechRecognition.Listen = cbToggleOverlay.Checked;
        }

        private void toolStripButtonCopy2Tester_Click_1(object sender, EventArgs e)
        {
            double te = extractor.uniqueTE();
            NumericUpDownTestingTE.Value = (decimal)(te >= 0 ? te * 100 : 80);
            numericUpDownImprintingBonusTester.Value = numericUpDownImprintingBonusExtractor.Value;
            if (rbBredExtractor.Checked)
                rbBredTester.Checked = true;
            else if (rbTamedExtractor.Checked)
                rbTamedTester.Checked = true;
            else
                rbWildTester.Checked = true;
            for (int s = 0; s < 8; s++)
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

        private void toolStripButtonClear_Click_1(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageExtractor)
            {
                clearAll();
                numericUpDownLevel.Value = 1;
                creatureInfoInputExtractor.Clear();
            }
            else if (tabControlMain.SelectedTab == tabPageStatTesting)
            {
                for (int s = 0; s < 8; s++)
                {
                    testingIOs[s].LevelDom = 0;
                    testingIOs[s].LevelWild = 0;
                }
                creatureInfoInputTester.Clear();
            }
        }

        private void toolStripButtonCopy2Extractor_Click(object sender, EventArgs e)
        {
            clearAll();
            // copy values from tester over to extractor
            for (int s = 0; s < 8; s++)
                statIOs[s].Input = testingIOs[s].Input;
            if (rbBredTester.Checked)
                rbBredExtractor.Checked = true;
            else if (rbTamedTester.Checked)
                rbTamedExtractor.Checked = true;
            else
                rbWildExtractor.Checked = true;
            numericUpDownImprintingBonusExtractor.Value = numericUpDownImprintingBonusTester.Value;
            // set total level
            numericUpDownLevel.Value = getCurrentWildLevels(false).Sum() - testingIOs[7].LevelWild + getCurrentDomLevels(false).Sum() + 1;

            creatureInfoInputExtractor.CreatureSex = creatureInfoInputTester.CreatureSex;
            creatureInfoInputExtractor.RegionColors = creatureInfoInputTester.RegionColors;
            tabControlMain.SelectedTab = tabPageExtractor;
        }

        private void newToolStripButton1_Click(object sender, EventArgs e)
        {
            newCollection();
        }

        private void openToolStripButton1_Click(object sender, EventArgs e)
        {
            loadCollection();
        }

        private void saveToolStripButton1_Click(object sender, EventArgs e)
        {
            saveCollection();
        }

        private void showStatsInOverlay()
        {
            if (overlay != null && overlay.checkInventoryStats) // TODO, also check if the extraction wasn't done by clicking on "Import last exported"
            {
                float[] wildLevels = new float[10];
                float[] tamedLevels = new float[10];
                Color[] colors = new Color[8];

                for (int i = 0; i < 8; i++)
                {
                    wildLevels[i] = statIOs[i].LevelWild > 0 ? statIOs[i].LevelWild : 0;
                    tamedLevels[i] = statIOs[i].LevelDom;
                    colors[i] = statIOs[i].BackColor;

                    if (i < 7)
                    {
                        wildLevels[9] += statIOs[i].LevelWild > 0 ? statIOs[i].LevelWild : 0;
                        tamedLevels[9] += statIOs[i].LevelDom;
                    }
                }
                wildLevels[9]++; // startlevel

                string extraText = Values.V.speciesNames[speciesSelector1.speciesIndex];
                if (!extractor.postTamed)
                {
                    string foodName = Values.V.species[speciesSelector1.speciesIndex].taming.eats[0];
                    double tamingFoodRateMultiplier = cbEventMultipliers.Checked ? creatureCollection.tamingFoodRateMultiplierEvent : creatureCollection.tamingFoodRateMultiplier;
                    int foodNeeded = Taming.foodAmountNeeded(speciesSelector1.speciesIndex, (int)wildLevels[9], Values.V.tamingSpeedMultiplier, foodName, Values.V.species[speciesSelector1.speciesIndex].taming.nonViolent);
                    Taming.tamingTimes(speciesSelector1.speciesIndex, (int)wildLevels[9], Values.V.tamingSpeedMultiplier, tamingFoodRateMultiplier, foodName, foodNeeded, out List<int> foodAmountUsed, out TimeSpan duration, out int narcoBerries, out int narcotics, out int bioToxines, out double te, out double hunger, out int bonusLevel, out bool enoughFood);
                    string foodNameDisplay = foodName == "Kibble" ? Values.V.species[speciesSelector1.speciesIndex].taming.favoriteKibble + " Egg Kibble" : foodName;
                    extraText += "\nTaming takes " + duration.ToString(@"hh\:mm\:ss") + " with " + foodNeeded + "×" + foodNameDisplay
                            + "\n" + narcoBerries + " Narcoberries or " + narcotics + " Narcotics or " + bioToxines + " Bio Toxines are needed"
                            + "\nTaming Effectiveness: " + Math.Round(100 * te, 1) + " % (+" + bonusLevel + " lvl)";
                }

                overlay.setStatLevels(wildLevels, tamedLevels, colors);
                overlay.setExtraText(extraText);

                // currently disabled, as current weight is not shown. TODO remove if there's no way to tell maturating-progress
                //if (Values.V.species[speciesSelector1.speciesIndex].breeding != null && lastOCRValues != null && lastOCRValues.Length > 10 && lastOCRValues[10] > 0)
                //{
                //    int maxTime = (int)Values.V.species[speciesSelector1.speciesIndex].breeding.maturationTimeAdjusted;
                //    if (maxTime > 0 && lastOCRValues[4] > 0)
                //        overlay.setBreedingProgressValues((float)Math.Round(lastOCRValues[10] / lastOCRValues[4], 1), maxTime); // current weight
                //    else
                //        overlay.setBreedingProgressValues(1, 0); // 100% breeding time shows nothing
                //}
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
                mergingDuplicatesWindow.RefreshLibrary += filterLib;
            }
            mergingDuplicatesWindow.Show();
            mergingDuplicatesWindow.CheckForDuplicates(creatureList);
#pragma warning restore 162
        }

        private void btnReadValuesFromArk_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showOCRButton)
                doOCR();
            else
                importExportedCreaturesDefaultFolder();
        }

        private void toolStripButtonAddTribe_Click(object sender, EventArgs e)
        {
            tribesControl1.addTribe();
        }

        private void button2TamingCalc_Click(object sender, EventArgs e)
        {
            tamingControl1.setSpeciesIndex(speciesSelector1.speciesIndex);
            if (cbQuickWildCheck.Checked)
                tamingControl1.setLevel(statIOs[7].LevelWild + 1);
            else
                tamingControl1.setLevel((int)numericUpDownLevel.Value);
            tabControlMain.SelectedTab = tabPageTaming;
        }

        private void labelImprintedCount_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // set imprinting-count to closes integer
                if (Values.V.species[speciesSelector1.speciesIndex].breeding != null && Values.V.species[speciesSelector1.speciesIndex].breeding.maturationTimeAdjusted > 0)
                {
                    double imprintingGainPerCuddle = Utils.imprintingGainPerCuddle(Values.V.species[speciesSelector1.speciesIndex].breeding.maturationTimeAdjusted, Values.V.babyCuddleIntervalMultiplier);
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
                // set imprinting value so the set levels in the tester yield the value in the extractor
                double imprintingBonus = (statIOs[7].Input / Stats.calculateValue(speciesSelector1.speciesIndex, 7, testingIOs[7].LevelWild, 0, true, 1, 0) - 1) / (0.2 * creatureCollection.imprintingMultiplier);
                if (imprintingBonus < 0)
                    imprintingBonus = 0;
                if (!creatureCollection.allowMoreThanHundredImprinting && imprintingBonus > 1)
                    imprintingBonus = 1;
                numericUpDownImprintingBonusTester.ValueSave = 100 * (decimal)imprintingBonus;
            }
        }

        private bool loadAdditionalValues(string file, bool showResult = false, bool applySettings = true)
        {
            if (Values.V.loadAdditionalValues(file, showResult))
            {
                if (speechRecognition != null)
                    speechRecognition.updateNeeded = true;
                if (applySettings)
                    applySettingsToValues();
                speciesSelector1.setSpeciesLists(Values.V.speciesNames, Values.V.speciesWithAliasesList);
                creatureCollection.additionalValues = Path.GetFileName(file);
                updateStatusBar();
                return true;
            }
            return false;
        }

        private void loadAdditionalValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Additional values-file (*.json)|*.json",
                InitialDirectory = FileService.GetJsonPath()
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string filename = dlg.FileName;
                // copy to json folder if loaded from somewhere else
                if (!filename.StartsWith(FileService.GetJsonPath()))
                {
                    try
                    {
                        string destination = FileService.GetJsonPath(Path.GetFileName(filename));
                        File.Copy(filename, destination);
                        filename = destination;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Trying to copy the file to the application's json folder failed.\n" +
                                "The program won't be able to load it at its next start.\n\n" +
                                "Error message:\n\n" + ex.Message, "Copy file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                if (loadAdditionalValues(filename, true))
                    setCollectionChanged(true);
            }
        }

        private void toolStripButtonAddPlayer_Click(object sender, EventArgs e)
        {
            tribesControl1.addPlayer();
        }

        private void updateStatusBar()
        {
            var creatureCount = creatureCollection.creatures.Where(c => !c.IsPlaceholder);
            int total = creatureCount.Count();
            int obelisk = creatureCount.Count(c => c.status == CreatureStatus.Obelisk);
            int cryopod = creatureCount.Count(c => c.status == CreatureStatus.Cryopod);
            toolStripStatusLabel.Text = total + " creatures in Library"
                + (total > 0 ? " ("
                + "available: " + creatureCount.Count(c => c.status == CreatureStatus.Available)
                            + ", unavailable: " + creatureCount.Count(c => c.status == CreatureStatus.Unavailable)
                + ", dead: " + creatureCount.Count(c => c.status == CreatureStatus.Dead)
                            + (obelisk > 0 ? ", obelisk: " + obelisk : "")
                + (cryopod > 0 ? ", cryopod: " + cryopod : "")
                            : "")
                    + ". v" + Application.ProductVersion + " / values: " + Values.V.version +
                    (creatureCollection.additionalValues.Length > 0 && Values.V.modVersion != null && Values.V.modVersion.ToString().Length > 0 ? ", additional values from " + creatureCollection.additionalValues + " v" + Values.V.modVersion : "");
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
            speechRecognition?.toggleListening();
        }

        private void editBoxCreatureInTester(object sender, Creature c)
        {
            editCreatureInTester(c);
        }

        private void createIncubationTimer(Creature mother, Creature father, TimeSpan incubationDuration, bool incubationStarted)
        {
            raisingControl1.addIncubationTimer(mother, father, incubationDuration, incubationStarted);
            libraryNeedsUpdate = true; // mating-cooldown of mother was set
        }

        private void TimerGlobal_Tick(object sender, EventArgs e)
        {
            timerList1.Tick();
            raisingControl1.Tick();
        }

        private void cbEvolutionEvent_CheckedChanged(object sender, EventArgs e)
        {
            applyEvolutionMultipliers();
        }

        private void applyEvolutionMultipliers()
        {
            Values.V.applyMultipliers(creatureCollection, cbEventMultipliers.Checked, false);

            tamingControl1.setTamingMultipliers(Values.V.tamingSpeedMultiplier,
                    cbEventMultipliers.Checked ? creatureCollection.tamingFoodRateMultiplierEvent : creatureCollection.tamingFoodRateMultiplier);
            breedingPlan1.updateBreedingData();
            raisingControl1.updateRaisingData();
        }

        private void toolStripButtonDeleteExpiredIncubationTimers_Click(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageRaising)
                raisingControl1.deleteAllExpiredIncubationTimers();
            else if (tabControlMain.SelectedTab == tabPageTimer)
                timerList1.deleteAllExpiredTimers();
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
                setCreatureValuesToExtractor(creatureCollection.creaturesValues[toolStripCBTempCreatures.SelectedIndex]);
                toolStripButtonDeleteTempCreature.Visible = true;
            }
            else
            {
                toolStripButtonDeleteTempCreature.Visible = false;
            }
        }

        private void setCreatureValuesToExtractor(CreatureValues cv, bool onlyWild = false, bool setInfoInput = true)
        {
            // at this point, if the creatureValues has parent-ArkIds, make sure these parent-creatures exist
            if (cv.Mother == null)
            {
                if (creatureCollection.CreatureById(cv.motherGuid, cv.motherArkId, cv.species, cv.sex, out Creature mother))
                {
                    cv.Mother = mother;
                }
                else if (cv.motherArkId != 0)
                {
                    cv.Mother = new Creature(cv.motherArkId);
                    creatureCollection.creatures.Add(cv.Mother);
                }
            }
            if (cv.Father == null)
            {
                if (creatureCollection.CreatureById(cv.fatherGuid, cv.fatherArkId, cv.species, cv.sex, out Creature father))
                {
                    cv.Father = father;
                }
                else if (cv.fatherArkId != 0)
                {
                    cv.Father = new Creature(cv.fatherArkId);
                    creatureCollection.creatures.Add(cv.Father);
                }
            }

            clearAll();
            speciesSelector1.setSpecies(cv.species);
            for (int s = 0; s < 8; s++)
                statIOs[s].Input = cv.statValues[s];

            if (setInfoInput)
                setCreatureValuesToInfoInput(cv, creatureInfoInputExtractor);

            numericUpDownLevel.ValueSave = cv.level;
            numericUpDownLowerTEffBound.ValueSave = (decimal)cv.tamingEffMin * 100;
            numericUpDownUpperTEffBound.ValueSave = (decimal)cv.tamingEffMax * 100;

            if (cv.isBred)
                rbBredExtractor.Checked = true;
            else if (cv.isTamed)
                rbTamedExtractor.Checked = true;
            else rbWildExtractor.Checked = true;
            numericUpDownImprintingBonusExtractor.ValueSave = (decimal)cv.imprintingBonus * 100;
        }

        private void setCreatureValuesToTester(CreatureValues cv)
        {
            speciesSelector1.setSpecies(cv.species);
            for (int s = 0; s < 8; s++)
            {
                testingIOs[s].LevelWild = cv.levelsWild[s];
                testingIOs[s].LevelDom = cv.levelsDom[s];
            }
            setCreatureValuesToInfoInput(cv, creatureInfoInputTester);

            NumericUpDownTestingTE.ValueSave = (decimal)cv.tamingEffMin * 100;

            if (cv.isBred)
                rbBredTester.Checked = true;
            else if (cv.isTamed)
                rbTamedTester.Checked = true;
            else rbWildTester.Checked = true;
            numericUpDownImprintingBonusTester.ValueSave = (decimal)cv.imprintingBonus * 100;
        }

        private void setCreatureValuesToInfoInput(CreatureValues cv, CreatureInfoInput input)
        {
            input.CreatureName = cv.name;
            input.CreatureOwner = cv.owner;
            input.CreatureTribe = cv.tribe;
            input.CreatureServer = cv.server;
            input.CreatureSex = cv.sex;
            input.CreatureGuid = cv.guid;
            input.Neutered = cv.neutered;
            input.mother = cv.Mother;
            input.father = cv.Father;
            input.RegionColors = cv.colorIDs;
            input.SetArkId(cv.ARKID, cv.guid == Utils.ConvertArkIdToGuid(cv.ARKID));
            input.MutationCounterMother = cv.mutationCounterMother;
            input.MutationCounterFather = cv.mutationCounterFather;
            input.Grown = cv.growingUntil;
            input.Cooldown = cv.cooldownUntil;
            input.mother = cv.Mother;
            input.father = cv.Father;
            input.MotherArkId = cv.motherArkId;
            input.FatherArkId = cv.fatherArkId;
        }

        private void toolStripButtonSaveCreatureValuesTemp_Click(object sender, EventArgs e)
        {
            CreatureValues cv = new CreatureValues();
            for (int s = 0; s < 8; s++)
                cv.statValues[s] = statIOs[s].Input;
            cv.species = speciesSelector1.species;
            cv.name = creatureInfoInputExtractor.CreatureName;
            cv.owner = creatureInfoInputExtractor.CreatureOwner;
            cv.tribe = creatureInfoInputExtractor.CreatureTribe;
            cv.server = creatureInfoInputExtractor.CreatureServer;
            cv.sex = creatureInfoInputExtractor.CreatureSex;
            cv.neutered = creatureInfoInputExtractor.Neutered;
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
            setCollectionChanged(true, "");

            updateTempCreatureDropDown();
        }

        private void toolStripButtonDeleteTempCreature_Click(object sender, EventArgs e)
        {
            if (toolStripCBTempCreatures.SelectedIndex >= 0
                    && MessageBox.Show("Remove the data of this cached creature?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                creatureCollection.creaturesValues.RemoveAt(toolStripCBTempCreatures.SelectedIndex);
                updateTempCreatureDropDown();
                setCollectionChanged(true, "");
            }
        }

        private void updateTempCreatureDropDown()
        {
            toolStripCBTempCreatures.Items.Clear();
            foreach (CreatureValues cv in creatureCollection.creaturesValues)
                toolStripCBTempCreatures.Items.Add($"{cv.name} ({cv.species})");
        }

        private void CreatureInfoInput_CreatureDataRequested(CreatureInfoInput sender, bool patternEditor)
        {
            Creature cr = new Creature();
            if (sender == creatureInfoInputExtractor)
            {
                cr.levelsWild = statIOs.Select(s => s.LevelWild).ToArray();
                cr.imprintingBonus = extractor.imprintingBonus / 100;
                cr.tamingEff = extractor.uniqueTE();
                cr.isBred = rbBredExtractor.Checked;
            }
            else
            {
                cr.levelsWild = testingIOs.Select(s => s.LevelWild).ToArray();
                cr.imprintingBonus = (double)numericUpDownImprintingBonusTester.Value / 100;
                cr.tamingEff = (double)NumericUpDownTestingTE.Value / 100;
                cr.isBred = rbBredTester.Checked;
            }
            if (patternEditor)
                sender.openNamePatternEditor(cr);
            else
                sender.generateCreatureName(cr);
        }

        private void ExtractionTestControl1_CopyToTester(string species, int[] wildLevels, int[] domLevels, bool postTamed, bool bred, double te, double imprintingBonus, bool gotoTester, testCases.TestCaseControl tcc)
        {
            newCollection();
            loadMultipliersFromTestCase(tcc.testCase);
            editCreatureInTester(new Creature(species, "", "", "", Sex.Unknown, wildLevels, domLevels, te, bred, imprintingBonus), true);
            if (gotoTester) tabControlMain.SelectedTab = tabPageStatTesting;
        }

        private void ExtractionTestControl1_CopyToExtractor(string species, int level, double[] statValues, bool postTamed, bool bred, double imprintingBonus, bool gotoExtractor, testCases.TestCaseControl tcc)
        {
            // test if the testcase can be extracted
            newCollection();
            clearAll();
            for (int s = 0; s < 8; s++)
            {
                statIOs[s].DomLevelLockedZero = false;
                statIOs[s].Input = statValues[s];
            }
            speciesSelector1.setSpecies(species);

            numericUpDownLevel.ValueSave = level;
            numericUpDownLowerTEffBound.Value = 0;
            numericUpDownUpperTEffBound.Value = 100;

            if (bred)
                rbBredExtractor.Checked = true;
            else if (postTamed)
                rbTamedExtractor.Checked = true;
            else rbWildExtractor.Checked = true;
            numericUpDownImprintingBonusExtractor.ValueSave = (decimal)imprintingBonus * 100;

            loadMultipliersFromTestCase(tcc.testCase);

            var watch = Stopwatch.StartNew();
            extractLevels(true);
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
                    int resultCount = -8; // one result per stat is allowed, only count the additional ones
                    for (int s = 0; s < 8; s++)
                    {
                        resultCount += extractor.results[s].Count;
                        bool statValid = false;
                        for (int r = 0; r < extractor.results[s].Count; r++)
                        {
                            if (extractor.results[s][r].levelWild == -1 || s == 6 && extractor.results[s][r].levelWild == 0 || extractor.results[s][r].levelWild == tcc.testCase.levelsWild[s]
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

        private void loadMultipliersFromTestCase(testCases.ExtractionTestCase etc)
        {
            // set all stat-multipliers from testcase
            creatureCollection.BabyMatureSpeedMultiplier = etc.matureSpeedMultiplier;
            creatureCollection.babyCuddleIntervalMultiplier = etc.cuddleIntervalMultiplier;
            creatureCollection.imprintingMultiplier = etc.imprintingStatScaleMultiplier;
            creatureCollection.singlePlayerSettings = etc.singleplayerSettings;
            creatureCollection.allowMoreThanHundredImprinting = etc.allowMoreThanHundredPercentImprinting;
            creatureCollection.maxWildLevel = etc.maxWildLevel;

            if (Values.V.modValuesFile != "" && Values.V.modValuesFile != etc.multiplierModifierFile)
                Values.V.loadValues(); // load original multipliers if they were changed

            creatureCollection.multipliers = etc.multipliers;
            if ((string.IsNullOrWhiteSpace(etc.multiplierModifierFile) || string.IsNullOrWhiteSpace(Properties.Settings.Default.LastSaveFileTestCases)) && Values.V.modValuesFile == etc.multiplierModifierFile)
            {
                Values.V.applyMultipliers(creatureCollection);
            }
            else
            {
                loadAdditionalValues(Path.Combine(Path.GetDirectoryName(Properties.Settings.Default.LastSaveFileTestCases), etc.multiplierModifierFile), false, false);
                Values.V.applyMultipliers(creatureCollection);
            }
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
                etc.levelsDom = getCurrentDomLevels(false);
                etc.levelsWild = getCurrentWildLevels(false);
                etc.multiplierModifierFile = creatureCollection.additionalValues;
                etc.multipliers = creatureCollection.multipliers;
                etc.species = speciesSelector1.species;
                etc.matureSpeedMultiplier = creatureCollection.BabyMatureSpeedMultiplier;
                etc.cuddleIntervalMultiplier = creatureCollection.babyCuddleIntervalMultiplier;
                etc.imprintingStatScaleMultiplier = creatureCollection.imprintingMultiplier;
                etc.singleplayerSettings = creatureCollection.singlePlayerSettings;
                etc.allowMoreThanHundredPercentImprinting = creatureCollection.allowMoreThanHundredImprinting;
                etc.maxWildLevel = creatureCollection.maxWildLevel;

                double[] statValues = new double[8];
                for (int s = 0; s < 8; s++)
                    statValues[s] = statIOs[s].Input;
                etc.statValues = statValues;

                extractionTestControl1.addTestCase(etc);
                tabControlMain.SelectedTab = tabPageExtractionTests;
            }
        }

        private void importAllCreaturesInFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importExportedCreaturesDefaultFolder();
        }

        private void importExportedCreaturesDefaultFolder()
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.ExportCreatureFolder))
                MessageBox.Show("There is no default folder set where the exported creatures are located. Set this folder in the settings.",
                        "No default export-folder set", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                showExportedCreatureListControl();
                exportedCreatureList.loadFilesInFolder(Properties.Settings.Default.ExportCreatureFolder);
            }
        }

        private void importAllCreaturesInSelectedFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showExportedCreatureListControl();
            exportedCreatureList.chooseFolderAndImport();
        }

        private void btImportLastExported_Click(object sender, EventArgs e)
        {
            importLastExportedCreature();
        }

        private void importLastExportedCreature()
        {
            string folder = Properties.Settings.Default.ExportCreatureFolder;
            if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
            {
                MessageBox.Show("There is no folder set where the exported creatures are located. Set this folder in the settings. " +
                        "Usually the folder is\n" + @"…\Steam\steamapps\common\ARK\ShooterGame\Saved\DinoExports\<ID>",
                        "No export-folder set", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var files = Directory.GetFiles(folder);
            if (files.Length > 0)
            {
                extractExportedFileInExtractor(files.OrderByDescending(f => File.GetLastWriteTime(f)).First());
            }
            else
                MessageBox.Show($"No exported creature-file found in the set folder\n{folder}\nYou have to export a creature first ingame.\n\n" +
                        "You may also want to check the set folder in the settings. Usually the folder is\n" +
                        "…\\Steam\\steamapps\\common\\ARK\\ShooterGame\\Saved\\DinoExports\\<ID>",
                        "No files found", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ExportedCreatureList_CopyValuesToExtractor(uiControls.ExportedCreatureControl exportedCreatureControl, bool addToLibraryIfUnique)
        {
            tabControlMain.SelectedTab = tabPageExtractor;
            extractExportedFileInExtractor(exportedCreatureControl);

            // add to library automatically if batch-extracting exportedImported values and uniqueLevels
            if (addToLibraryIfUnique)
            {
                if (extractor.uniqueResults)
                    add2Lib(true, exportedCreatureControl.creatureValues.motherArkId, exportedCreatureControl.creatureValues.fatherArkId);
                else
                    exportedCreatureControl.setStatus(uiControls.ExportedCreatureControl.ImportStatus.NeedsLevelChosing, DateTime.Now);
            }
        }

        private void extractExportedFileInExtractor(string exportFile)
        {
            var cv = ImportExported.importExportedCreature(exportFile);
            setCreatureValuesToExtractor(cv, false, false);
            tabControlMain.SelectedTab = tabPageExtractor;
            extractLevels(true);
            setCreatureValuesToInfoInput(cv, creatureInfoInputExtractor);

            setMessageLabelText("Creature of the exported file\n" + exportFile);
        }

        private void extractExportedFileInExtractor(uiControls.ExportedCreatureControl ecc)
        {
            setCreatureValuesToExtractor(ecc.creatureValues, false, false);
            extractLevels();
            // gets deleted in extractLevels()
            exportedCreatureControl = ecc;
            setCreatureValuesToInfoInput(exportedCreatureControl.creatureValues, creatureInfoInputExtractor);

            setMessageLabelText("Creature of the exported file\n" + exportedCreatureControl.exportedFile);
        }

        private void ExportedCreatureList_CheckGuidInLibrary(uiControls.ExportedCreatureControl exportedCreatureControl)
        {
            try
            {
                Creature cr = creatureCollection.creatures.Single(c => c.guid == exportedCreatureControl.creatureValues.guid);

                exportedCreatureControl.setStatus(uiControls.ExportedCreatureControl.ImportStatus.OldImported, cr.addedToLibrary);
            }
            catch (InvalidOperationException)
            {
                exportedCreatureControl.setStatus(uiControls.ExportedCreatureControl.ImportStatus.NotImported, DateTime.Now);
            }
        }

        private void llOnlineHelpExtractionIssues_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/cadon/ARKStatsExtractor/wiki/Extraction-issues");
        }

        private void importExportedCreaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showExportedCreatureListControl();
        }

        private void showExportedCreatureListControl()
        {
            if (exportedCreatureList == null || exportedCreatureList.IsDisposed)
            {
                exportedCreatureList = new ExportedCreatureList();
                exportedCreatureList.CopyValuesToExtractor += ExportedCreatureList_CopyValuesToExtractor;
                exportedCreatureList.CheckArkIdInLibrary += ExportedCreatureList_CheckGuidInLibrary;
            }
            exportedCreatureList.Show();
        }

        private void copyToMultiplierTesterToolStripButton_Click(object sender, EventArgs e)
        {
            double[] statValues = new double[8];
            for (int s = 0; s < 8; s++)
                statValues[s] = statIOs[s].Input;
            statsMultiplierTesting1.setCreatureValues(statValues, getCurrentWildLevels(false), getCurrentDomLevels(false), (double)NumericUpDownTestingTE.Value / 100, (double)numericUpDownImprintingBonusTester.Value / 100, rbTamedTester.Checked, rbBredTester.Checked);
            tabControlMain.SelectedTab = tabPageMultiplierTesting;
        }

        private void StatsMultiplierTesting1_OnApplyMultipliers()
        {
            Values.V.applyMultipliers(creatureCollection);
            setCollectionChanged(true);
        }

        private void initLocalization()
        {
            Loc.LoadResourceFile();
            Utils.initializeLocalizations();
        }

        private void setLocalizations(bool init = true)
        {
            if (init) initLocalization();

            // menu
            Loc.ControlText(fileToolStripMenuItem);
            Loc.ControlText(newToolStripMenuItem);
            Loc.ControlText(loadToolStripMenuItem);
            Loc.ControlText(loadAndAddToolStripMenuItem);
            Loc.ControlText(saveToolStripMenuItem);
            Loc.ControlText(saveAsToolStripMenuItem);
            Loc.ControlText(importingFromSavegameToolStripMenuItem);
            Loc.ControlText(importingFromSavegameEmptyToolStripMenuItem);
            //Loc.ControlText(runDefaultExtractionAndImportFileToolStripMenuItem);
            //Loc.ControlText(runDefaultExtractionToolStripMenuItem);
            //Loc.ControlText(importCreatedJsonfileToolStripMenuItem);
            Loc.ControlText(importExportedCreaturesToolStripMenuItem);
            //Loc.ControlText(runDefaultExtractionAndImportFileToolStripMenuItem);
            //Loc.ControlText(runDefaultExtractionToolStripMenuItem);
            //Loc.ControlText(importCreatedJsonfileToolStripMenuItem);
            Loc.ControlText(loadAdditionalValuesToolStripMenuItem);
            Loc.ControlText(settingsToolStripMenuItem);
            Loc.ControlText(quitToolStripMenuItem);
            Loc.ControlText(editToolStripMenuItem);
            Loc.ControlText(exportValuesToClipboardToolStripMenuItem);
            Loc.ControlText(importValuesFromClipboardToolStripMenuItem);
            Loc.ControlText(setStatusToolStripMenuItem);
            Loc.ControlText(multiSetterToolStripMenuItem);
            Loc.ControlText(deleteSelectedToolStripMenuItem);
            Loc.ControlText(findDuplicatesToolStripMenuItem);
            Loc.ControlText(copyCreatureToolStripMenuItem);
            Loc.ControlText(pasteCreatureToolStripMenuItem);
            Loc.ControlText(viewToolStripMenuItem);
            Loc.ControlText(deadCreaturesToolStripMenuItem);
            Loc.ControlText(unavailableCreaturesToolStripMenuItem);
            Loc.ControlText(obeliskCreaturesToolStripMenuItem);
            Loc.ControlText(neuteredCreaturesToolStripMenuItem);
            Loc.ControlText(mutatedCreaturesToolStripMenuItem);
            Loc.ControlText(femalesToolStripMenuItem);
            Loc.ControlText(malesToolStripMenuItem);
            Loc.ControlText(helpToolStripMenuItem);
            Loc.ControlText(aboutToolStripMenuItem);
            Loc.ControlText(onlinehelpToolStripMenuItem);
            Loc.ControlText(BreedingPlanHelpToolStripMenuItem);
            Loc.ControlText(extractionIssuesToolStripMenuItem);
            Loc.ControlText(checkForUpdatedStatsToolStripMenuItem);
            Loc.ControlText(toolStripButtonCopy2Tester);
            Loc.ControlText(toolStripButtonCopy2Extractor);
            Loc.ControlText(toolStripButtonClear);
            Loc.ControlText(toolStripButtonAddNote);
            Loc.ControlText(toolStripButtonRemoveNote);
            Loc.ControlText(toolStripButtonDeleteExpiredIncubationTimers);
            Loc.ControlText(toolStripButtonSaveCreatureValuesTemp);
            Loc.ControlText(toolStripButtonDeleteTempCreature);
            Loc.ControlText(tsBtAddAsExtractionTest);
            Loc.ControlText(copyToMultiplierTesterToolStripButton);

            // top bar
            Loc.ControlText(cbGuessSpecies, tt);
            Loc.ControlText(btReadValuesFromArk, tt);
            Loc.ControlText(btImportLastExported, tt);
            Loc.ControlText(cbToggleOverlay);

            // tester
            Loc.ControlText(tabPageStatTesting, "statTesting");
            Loc.ControlText(rbWildTester, "wild");
            Loc.ControlText(rbTamedTester, "tamed");
            Loc.ControlText(rbBredTester, "bred");
            Loc.ControlText(lbTesterWildLevel, "wildLvl");
            Loc.ControlText(lbTesterDomLevel, "domLvl");
            Loc.ControlText(lbCurrentValue, "currentValue");
            Loc.ControlText(lbBreedingValueTester, "breedingValue");
            Loc.ControlText(lbNotYetTamed);
            Loc.ControlText(gpPreviewEdit);
            Loc.ControlText(lbTestingInfo);
            Loc.ControlText(gbStatChart, "statChart");
            Loc.setToolTip(lbImprintedCount, tt);
            Loc.setToolTip(lbTesterDomLevel, "domLevelExplanation", tt);
            Loc.setToolTip(lbTesterWildLevel, "wildLevelExplanation", tt);

            // extractor
            Loc.ControlText(tabPageExtractor, "extractor");
            Loc.ControlText(lbCurrentStatEx, "currentStatValue");
            Loc.ControlText(lbExtractorWildLevel, "wildLvl");
            Loc.ControlText(lbExtractorDomLevel, "domLvl");
            Loc.ControlText(lbSum);
            Loc.ControlText(lbShouldBe);
            Loc.ControlText(cbExactlyImprinting, tt);
            Loc.ControlText(btExtractLevels);
            Loc.ControlText(cbQuickWildCheck, tt);
            Loc.ControlText(rbWildExtractor, "wild");
            Loc.ControlText(rbTamedExtractor, "tamed");
            Loc.ControlText(rbBredExtractor, "bred");
            Loc.setToolTip(lbImprintingCuddleCountExtractor, tt);
            Loc.setToolTip(lbSumWild, tt);
            Loc.setToolTip(lbSumDom, tt);
            Loc.setToolTip(lbSumDomSB, tt);
            Loc.setToolTip(lbListening, tt);
            Loc.setToolTip(lbExtractorDomLevel, "domLevelExplanation", tt);
            Loc.setToolTip(lbExtractorWildLevel, "wildLevelExplanation", tt);

            // library
            Loc.ControlText(tabPageLibrary, "library");
            columnHeaderName.Text = Loc.s("Name");
            columnHeaderOwner.Text = Loc.s("Owner");
            columnHeaderNote.Text = Loc.s("Note");
            columnHeaderServer.Text = Loc.s("Server");
            columnHeaderHP.Text = Utils.statName(0, true);
            columnHeaderSt.Text = Utils.statName(1, true);
            columnHeaderOx.Text = Utils.statName(2, true);
            columnHeaderFo.Text = Utils.statName(3, true);
            columnHeaderWe.Text = Utils.statName(4, true);
            columnHeaderDm.Text = Utils.statName(5, true);
            columnHeaderSp.Text = Utils.statName(6, true);
            columnHeaderTo.Text = Utils.statName(7, true);
            columnHeaderTopStatsNr.Text = Loc.s("Top");
            columnHeaderTopness.Text = Loc.s("topPercentage");
            columnHeaderGen.Text = Loc.s("Generation_Abb");
            columnHeaderLW.Text = Loc.s("LevelWild_Abb");
            columnHeaderMutations.Text = Loc.s("Mutations_Abb");
            columnHeaderAdded.Text = Loc.s("added");
            columnHeaderCooldown.Text = Loc.s("cooldownGrowing");
            columnHeaderColor0.Text = Loc.s("C0");
            columnHeaderColor1.Text = Loc.s("C1");
            columnHeaderColor2.Text = Loc.s("C2");
            columnHeaderColor3.Text = Loc.s("C3");
            columnHeaderColor4.Text = Loc.s("C4");
            columnHeaderColor5.Text = Loc.s("C5");

            // other tabs
            Loc.ControlText(tabPagePedigree, "pedigree");
            Loc.ControlText(tabPageTaming, "Taming");
            Loc.ControlText(tabPageBreedingPlan, "BreedingPlan");
            Loc.ControlText(tabPageRaising, "Raising");
            Loc.ControlText(tabPagePlayerTribes, "Player");

            // other controls
            creatureInfoInputTester.SetLocalizations();
            creatureInfoInputExtractor.SetLocalizations();
            pedigree1.SetLocalizations();
            tamingControl1.SetLocalizations();
            breedingPlan1.SetLocalizations();
        }

        /// <summary>
        /// fixes typos saved in earlier versions. is called right after loading a library
        /// </summary>
        /// <param name="cc">CreatureCollection to be checked on typos</param>
        private void creatureCollectionFixes(CreatureCollection cc)
        {
            foreach (Creature c in cc.creatures)
            {
                c.species = c.species.Trim();
                switch (c.species)
                {
                    case "Angler":
                        c.species = "Anglerfish";
                        break;
                    case "Compsognathus":
                        c.species = "Compy";
                        break;
                    case "Pachycephalosaurus":
                        c.species = "Pachy";
                        break;
                    case "Polar Bear":
                        c.species = "Dire Polar Bear";
                        break;
                    case "Quetzalcoatl":
                        c.species = "Quetzal";
                        break;
                    case "Sarcosuchus":
                        c.species = "Sarco";
                        break;
                    case "Spinosaur":
                        c.species = "Spino";
                        break;
                    case "Therizinosaurus":
                        c.species = "Therizinosaur";
                        break;
                    case "Tyrannosaurus":
                        c.species = "Rex";
                        break;
                    case "Wooly Rhino":
                        c.species = "Woolly Rhino";
                        break;
                }
            }
        }
    }
}
