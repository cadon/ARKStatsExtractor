using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Configuration;

namespace ARKBreedingStats
{
    public partial class Form1 : Form
    {
        private CreatureCollection creatureCollection = new CreatureCollection();
        private string currentFileName = "";
        private bool collectionDirty = false;
        private Dictionary<string, Int32[]> topStats = new Dictionary<string, Int32[]>(); // list of top stats of all creatures per species
        private List<StatIO> statIOs = new List<StatIO>();
        private List<StatIO> testingIOs = new List<StatIO>();
        private int speciesIndex = 0;
        private int activeStat = -1;
        private bool[] activeStats = new bool[] { true, true, true, true, true, true, true, true }; // stats used by the creature (some don't use oxygen)
        private bool pedigreeNeedsUpdate = false;
        private bool libraryNeedsUpdate = false;
        public delegate void LevelChangedEventHandler(StatIO s);
        public delegate void InputValueChangedEventHandler(StatIO s);
        public delegate void collectionChangedEventHandler(bool changed = true, string species = "0"); // if "0" is passed as species, breeding-related controls are not updated
        public delegate void setSpeciesIndexEventHandler(int speciesIndex);
        private bool updateTorporInTester, filterListAllowed;
        private bool[] considerStatHighlight = new bool[] { true, true, false, false, true, true, false, false }; // consider this stat for color-highlighting, topness etc
        private bool autoSave;
        private DateTime lastAutoSaveBackup = DateTime.Now.AddDays(-1);
        private int autoSaveMinutes;
        private Creature creatureTesterEdit;
        private FileSync fileSync;
        private Extraction extractor;
        private bool oxygenForAll;
        private SpeechRecognition speechRecognition;
        private Timer timer;

        // OCR stuff
        public ARKOverlay overlay;
        private static float[] lastOCRValues;
        private int lastOCRSpecies;

        public Form1()
        {
            InitializeComponent();

            // Create an instance of a ListView column sorter and assign it 
            // to the ListView controls
            this.listViewLibrary.ListViewItemSorter = new ListViewColumnSorter();
            listViewPossibilities.ListViewItemSorter = new ListViewColumnSorter();
            timerList1.ColumnSorter = new ListViewColumnSorter();

            this.listViewLibrary.DoubleBuffered(true);

            toolStripStatusLabel.Text = Application.ProductVersion;

            pedigree1.EditCreature += new Pedigree.EditCreatureEventHandler(editCreatureInTester);
            pedigree1.BestBreedingPartners += new PedigreeCreature.CreaturePartnerEventHandler(showBestBreedingPartner);
            pedigree1.exportToClipboard += new PedigreeCreature.ExportToClipboardEventHandler(exportAsTextToClipboard);
            breedingPlan1.EditCreature += new PedigreeCreature.CreatureEditEventHandler(editCreatureInTester);
            breedingPlan1.createIncubationTimer += new Raising.createIncubationEventHandler(createIncubationTimer);
            breedingPlan1.BestBreedingPartners += new PedigreeCreature.CreaturePartnerEventHandler(showBestBreedingPartner);
            breedingPlan1.exportToClipboard += new PedigreeCreature.ExportToClipboardEventHandler(exportAsTextToClipboard);
            breedingPlan1.bindEvents();
            timerList1.onTimerChange += new collectionChangedEventHandler(setCollectionChanged);
            raisingControl1.onChange += new collectionChangedEventHandler(setCollectionChanged);
            creatureBoxListView.EditCreature += new CreatureBox.EventHandler(editBoxCreatureInTester);
            tamingControl1.CreateTimer += new TimerControl.CreateTimerEventHandler(createTimer);
            raisingControl1.extractBaby += new RaisingControl.ExtractBabyEventHandler(extractBaby);
            raisingControl1.setSpeciesIndex += new setSpeciesIndexEventHandler(setSpeciesIndex);
            raisingControl1.timerControl = timerList1;
            notesControl1.changed += new collectionChangedEventHandler(setCollectionChanged);

            extractor = new Extraction();

            ArkOCR.OCR.setOCRControl(ocrControl1);
            ocrControl1.updateWhiteThreshold += OcrupdateWhiteThreshold;
            ocrControl1.dragEnter += testEnteredDrag;
            ocrControl1.dragDrop += doOCRofDroppedImage;

            settingsToolStripMenuItem.ShortcutKeyDisplayString = ((new KeysConverter()).ConvertTo(Keys.Control, typeof(string))).ToString().Replace("None", ",");

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // test if settings are corrupted. thanks to https://www.codeproject.com/Articles/30216/Handling-Corrupt-user-config-Settings
            // TODO does not yet work. has to use ConfigurationErrorsException?
            try
            {
                bool testVariable = Properties.Settings.Default.autosave;
            }
            catch (ConfigurationException ex)
            { //(requires System.Configuration)
                string filename = ((System.Configuration.ConfigurationException)ex.InnerException).Filename;

                if (MessageBox.Show("Smart Breeding has detected that your user settings file has become corrupted. " +
                                      "This may be due to a crash or improper exiting" +
                                      " of the program. Smart Breeding must reset your " +
                                      "user settings in order to continue.\n\nClick" +
                                      " Yes to reset your user settings and continue.\n\n" +
                                      "Click No if you wish to exit the application and attempt manual repair" +
                                      " or to rescue information before proceeding.",
                                      "Corrupt user settings",
                                      MessageBoxButtons.YesNo,
                                      MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    File.Delete(filename);
                    Properties.Settings.Default.Reset();
                    Properties.Settings.Default.Reload();
                }
                else
                    Process.GetCurrentProcess().Kill();
                // avoid the inevitable crash
            }


            // load window-position and size
            this.Size = Properties.Settings.Default.formSize;
            if (this.Size.Height < 200)
                this.Size = new System.Drawing.Size(this.Size.Width, 200);
            if (this.Size.Width < 400)
                this.Size = new System.Drawing.Size(400, this.Size.Height);
            this.Location = Properties.Settings.Default.formLocation;
            // check if form is on screen
            Screen[] screens = Screen.AllScreens;
            bool isOnScreen = false;
            foreach (Screen screen in screens)
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
                lwvs.Order = (Properties.Settings.Default.listViewSortAsc ? SortOrder.Ascending : SortOrder.Descending);
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

            statIOs.Add(this.statIOHealth);
            statIOs.Add(this.statIOStamina);
            statIOs.Add(this.statIOOxygen);
            statIOs.Add(this.statIOFood);
            statIOs.Add(this.statIOWeight);
            statIOs.Add(this.statIODamage);
            statIOs.Add(this.statIOSpeed);
            statIOs.Add(this.statIOTorpor);
            testingIOs.Add(this.statTestingHealth);
            testingIOs.Add(this.statTestingStamina);
            testingIOs.Add(this.statTestingOxygen);
            testingIOs.Add(this.statTestingFood);
            testingIOs.Add(this.statTestingWeight);
            testingIOs.Add(this.statTestingDamage);
            testingIOs.Add(this.statTestingSpeed);
            testingIOs.Add(this.statTestingTorpor);
            for (int s = 0; s < 8; s++)
            {
                statIOs[s].Title = Utils.statName(s);
                testingIOs[s].Title = Utils.statName(s);
                if (Utils.precision(s) == 3) { statIOs[s].Percent = true; testingIOs[s].Percent = true; }
                statIOs[s].statIndex = s;
                testingIOs[s].statIndex = s;
                testingIOs[s].LevelChanged += new LevelChangedEventHandler(this.testingStatIOValueUpdate);
                testingIOs[s].InputType = StatIOInputType.LevelsInputType;
                statIOs[s].InputValueChanged += new InputValueChangedEventHandler(this.statIOQuickWildLevelCheck);
                considerStatHighlight[s] = ((Properties.Settings.Default.consideredStats & (1 << s)) > 0);
                checkedListBoxConsiderStatTop.SetItemChecked(s, considerStatHighlight[s]);
            }
            statIOTorpor.ShowBarAndLock = false; // torpor should not show bar, it get's too wide and is not interesting for breeding
            statTestingTorpor.ShowBarAndLock = false;

            // enable 0-lock for dom-levels of oxygen, food (most often they are not leveld up)
            statIOs[2].DomLevelZero = true;
            statIOs[3].DomLevelZero = true;

            initializeCollection();
            filterListAllowed = true;

            // ToolTips
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.checkBoxJustTamed, "Check this if there was no server-restart or if you didn't logout since you tamed the creature.\nUncheck this if you know there was a server-restart (many servers restart every night).\nIf it is some days ago (IRL) you tamed the creature you should probably uncheck this checkbox.\nThe reason for this is a bug in the game, that displays a too high Torpor-value after a creature is tamed.");
            //tt.SetToolTip(checkBoxWildTamedAuto, "For most creatures the tool recognizes if they are wild or tamed.\nFor Giganotosaurus and maybe if you have custom server-settings you have to select manually if the creature is wild or tamed.");
            tt.SetToolTip(checkBoxQuickWildCheck, "Check this if you just want a quick check of the levels of a wild (untamed) creature.\nThe levels are then shown without the extraction-process (and without validation).");
            tt.SetToolTip(labelImprintedCount, "Number of cuddles given to get to this Imprinting-Bonus.\nClick to set to the closest valid integer.");
            tt.SetToolTip(labelImprintingCuddleCountExtractor, "Number of cuddles given to get to this Imprinting-Bonus.");
            tt.SetToolTip(labelSumWild, "This is an indicator if the sum of the wild levels is valid.\nIf a number with a plus sign is shown, the sum is too high and you need to choose another possibility from the lists of yellow stats.");
            tt.SetToolTip(labelSumDom, "This is the sum of all manual levelups of this creature, it should exactly match the number below.\nIf it's not matching, click on a stat that is yellow and choose another possible level distribution.");
            tt.SetToolTip(labelSumDomSB, "This is the number that the sum of all manual levelups should be equal to.");
            tt.SetToolTip(labelListening, "red: listening, grey: deactivated\nSay \"[species] [level]\", e.g. \"Rex level 30\" or \"Brontosaurus 50\"\nto get taming-infos in the overlay");

            creatureInfoInputExtractor.weightStat = statIOs[4];
            creatureInfoInputTester.weightStat = testingIOs[4];

            // Set up the file watcher
            fileSync = new FileSync(currentFileName, collectionChanged);

            if (Values.V.loadValues() && Values.V.speciesNames.Count > 0)
            {
                // load last save file:
                if (Properties.Settings.Default.LastSaveFile == "" || !loadCollectionFile(Properties.Settings.Default.LastSaveFile))
                    newCollection();

                // set species comboboxes
                updateSpeciesComboboxes();

                for (int s = 0; s < 8; s++)
                {
                    statIOs[s].Input = 0;
                }
            }
            else
            {
                MessageBox.Show("The values-file couldn't be loaded, this application does not work without. Try redownloading the tool.", "Error: Values-file not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }

            int selectedSpecies = Values.V.speciesNames.IndexOf(Properties.Settings.Default.lastSpecies);
            if (selectedSpecies >= 0)
            {
                comboBoxSpeciesGlobal.SelectedIndex = selectedSpecies;
                tamingControl1.setSpeciesIndex(selectedSpecies);
            }
            else if (comboBoxSpeciesGlobal.Items.Count > 0)
                comboBoxSpeciesGlobal.SelectedIndex = 0;

            extractor.activeStats = activeStats;

            // OCR
            ocrControl1.setWhiteThreshold(Properties.Settings.Default.OCRWhiteThreshold);

            ocr.OCRTemplate ocrConfig = ArkOCR.OCR.ocrConfig.loadFile(Properties.Settings.Default.ocrFile);
            if (ocrConfig != null)
            {
                ArkOCR.OCR.ocrConfig = ocrConfig;
                ocrControl1.setOCRFile(Properties.Settings.Default.ocrFile);
            }
            ocrControl1.initLabelEntries();

            // initialize speech recognition if enabled
            if (Properties.Settings.Default.SpeechRecognition)
            {
                speechRecognition = new SpeechRecognition(300, labelListening);
                speechRecognition.speechRecognized += new SpeechRecognition.SpeechRecognizedEventHandler(tellTamingData);
                speechRecognition.speechCommandRecognized += new SpeechRecognition.SpeechCommandRecognizedEventHandler(speechCommand);
            }
            else labelListening.Visible = false;

            clearAll();
            // UI loaded

            radarChart1.initializeVariables(creatureCollection.maxChartLevel);
            radarChartExtractor.initializeVariables(creatureCollection.maxChartLevel);

            // check for updates
            DateTime lastUpdateCheck = Properties.Settings.Default.lastUpdateCheck;
            if (DateTime.Now.AddDays(-2) > lastUpdateCheck)
                checkForUpdates(true);

            timer.Start();

            //// TODO: debug-numbers
            //statIOs[0].Input = 3263.2;
            //statIOs[1].Input = 2625;
            //statIOs[2].Input = 525;
            //statIOs[3].Input = 5811;
            //statIOs[4].Input = 625.8;
            //statIOs[5].Input = 3.171;
            //statIOs[6].Input = 1.118;
            //statIOs[7].Input = 8234.3;
            //comboBoxSpeciesGlobal.SelectedIndex = Values.V.speciesNames.IndexOf("Argentavis");
            //numericUpDownLevel.Value = 189;
            //checkBoxAlreadyBred.Checked = true;
            //numericUpDownImprintingBonusExtractor.Value = 59;
            //tabControlMain.SelectedTab = tabPageExtractor;
        }

        private void setSpeciesIndex(int speciesIndex)
        {
            if (speciesIndex >= 0 && speciesIndex < comboBoxSpeciesGlobal.Items.Count)
                comboBoxSpeciesGlobal.SelectedIndex = speciesIndex;
        }

        private void tellTamingData(string species, int level)
        {
            int sI = Values.V.speciesNames.IndexOf(species);
            if (sI >= 0 && Values.V.species[sI].taming != null && Values.V.species[sI].taming.eats != null && Values.V.species[sI].taming.eats.Count > 0)
            {
                comboBoxSpeciesGlobal.SelectedIndex = sI;
                tamingControl1.level = level;
                tamingControl1.setSpeciesIndex(sI);
                if (overlay != null)
                    overlay.setInfoText(species + " (level " + level.ToString() + ")" + ":\n" + tamingControl1.quickTamingInfos);
            }
        }

        private void speechCommand(SpeechRecognition.Commands command)
        {
            if (command == SpeechRecognition.Commands.Extract)
                doOCR();
        }

        delegate void collectionChangedCallback();

        public void collectionChanged()
        {
            if (creatureBoxListView.InvokeRequired)
            {
                collectionChangedCallback d = new collectionChangedCallback(collectionChanged);
                this.Invoke(d);
            }
            else
            {
                loadCollectionFile(currentFileName, true);
            }
        }

        private void clearAll()
        {
            extractor.Clear();
            listViewPossibilities.Items.Clear();
            for (int s = 0; s < 8; s++)
            {
                statIOs[s].Clear();
            }
            this.labelFootnote.Text = "";
            labelFootnote.BackColor = System.Drawing.Color.Transparent;
            this.numericUpDownLevel.BackColor = SystemColors.Window;
            this.numericUpDownLowerTEffBound.BackColor = SystemColors.Window;
            this.numericUpDownUpperTEffBound.BackColor = SystemColors.Window;
            this.checkBoxJustTamed.BackColor = System.Drawing.Color.Transparent;
            panelSums.BackColor = System.Drawing.Color.Transparent;
            panelWildTamedBred.BackColor = System.Drawing.Color.Transparent;
            labelTE.BackColor = System.Drawing.Color.Transparent;
            labelTE.Text = "";
            creatureInfoInputExtractor.ButtonEnabled = false;
            activeStat = -1;
            labelSumDom.Text = "";
            labelSumWild.Text = "";
            // labelSumWildSB.Text = ""; TODO remove
            for (int i = 0; i < 2; i++)
            {
                extractor.levelWildFromTorporRange[i] = 0;
                extractor.levelDomFromTorporAndTotalRange[i] = 0;
            }
            labelSumDomSB.Text = "";
            updateTorporInTester = true;
            buttonHelp.Visible = false;
            labelErrorHelp.Visible = false;
            labelImprintingFailInfo.Visible = false;
            groupBoxPossibilities.Visible = false;
            groupBoxRadarChartExtractor.Visible = false;
            labelDoc.Visible = false;
            button2TamingCalc.Visible = checkBoxQuickWildCheck.Checked;
            groupBoxTamingInfo.Visible = false;
            creatureInfoInputExtractor.domesticatedAt = DateTime.Now;
            creatureInfoInputExtractor.parentListValid = false;
        }

        private void toolStripButtonExtract_Click(object sender, EventArgs e)
        {
            extractLevels();
        }

        private void buttonExtract_Click(object sender, EventArgs e)
        {
            extractLevels();
        }

        private bool extractLevels(bool autoExtraction = false)
        {
            SuspendLayout();
            int activeStatKeeper = activeStat;
            clearAll();

            bool imprintingBonusChanged;
            double babyCuddleIntervalMultiplier = cbEventMultipliers.Checked ? creatureCollection.babyCuddleIntervalMultiplierEvent : creatureCollection.babyCuddleIntervalMultiplier;

            extractor.extractLevels(speciesIndex, (int)numericUpDownLevel.Value, statIOs,
                (double)numericUpDownLowerTEffBound.Value / 100, (double)numericUpDownUpperTEffBound.Value / 100,
                !radioButtonBred.Checked, radioButtonTamed.Checked, checkBoxJustTamed.Checked, radioButtonBred.Checked,
                (double)numericUpDownImprintingBonusExtractor.Value / 100, creatureCollection.imprintingMultiplier, babyCuddleIntervalMultiplier,
                creatureCollection.considerWildLevelSteps, creatureCollection.wildLevelStep, out imprintingBonusChanged);

            if (radioButtonTamed.Checked)
                checkBoxJustTamed.Checked = extractor.justTamed;
            numericUpDownImprintingBonusExtractor.Value = (decimal)extractor.imprintingBonus * 100;
            numericUpDownImprintingBonusExtractor_ValueChanged(null, null);

            if (imprintingBonusChanged && !autoExtraction)
                MessageBox.Show("The imprinting-percentage given is not possible with the current multipliers and may cause wrong values during the extraction-process.\n\nMake sure the BabyCuddleIntervallMultiplier and the BabyMatureSpeedMultiplier are set correctly.\nThey may have to be set to the value when the creature hatched/was born, even if they were changed.", "Imprinting-Value or multipliers probably wrong", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // remove all results that require a total wild-level higher than the max
            if (!radioButtonBred.Checked
                && creatureCollection.maxWildLevel > 0
                && extractor.levelWildFromTorporRange[0] > creatureCollection.maxWildLevel
                )
            {
                double minTECheck = 2d * (extractor.levelWildFromTorporRange[0] - creatureCollection.maxWildLevel) / creatureCollection.maxWildLevel;

                if (minTECheck < 1)
                {
                    // if min TE is equal or greater than 1, that indicates it can't possibly be anything but bred, and there cannot be any results that should be sorted out

                    for (int s = 0; s < 8; s++)
                    {
                        if (extractor.results[s].Count == 0 || extractor.results[s][0].TE < 0)
                            continue;
                        for (int r = 0; r < extractor.results[s].Count; r++)
                        {
                            if (extractor.results[s][r].TE < minTECheck)
                                extractor.results[s].RemoveAt(r--);
                        }
                    }
                }
            }

            if (!extractor.setStatLevelBounds())
            {
                this.numericUpDownLevel.BackColor = Color.LightSalmon;
                if (radioButtonTamed.Checked && this.numericUpDownLowerTEffBound.Value > 0)
                    this.numericUpDownLowerTEffBound.BackColor = Color.LightSalmon;
                if (radioButtonTamed.Checked && this.numericUpDownUpperTEffBound.Value < 100)
                    this.numericUpDownUpperTEffBound.BackColor = Color.LightSalmon;
                this.checkBoxJustTamed.BackColor = Color.LightSalmon;
                panelWildTamedBred.BackColor = Color.LightSalmon;
                extractionFailed();
                ResumeLayout();
                return false;
            }

            int removeOOBResult = extractor.removeOutOfBoundsResults();
            if (removeOOBResult >= 0)
            {
                this.numericUpDownLevel.BackColor = Color.LightSalmon;
                statIOs[removeOOBResult].Status = StatIOStatus.Error;
                statIOs[7].Status = StatIOStatus.Error;
            }

            // get mean-level (most probable for the wild levels)
            double meanWildLevel = Math.Round((double)extractor.levelWildFromTorporRange[1] / 7, 1);
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

                    setPossibility(s, r);
                    if (extractor.results[s].Count > 1)
                    {
                        statIOs[s].Status = StatIOStatus.Nonunique;
                        nonUniqueStats = true;
                    }
                    else { statIOs[s].Status = StatIOStatus.Unique; }
                }
                else
                {
                    // no results for this stat
                    statIOs[s].Status = StatIOStatus.Error;
                    extractor.validResults = false;
                    if (radioButtonTamed.Checked && extractor.statsWithEff.IndexOf(s) >= 0 && this.numericUpDownLowerTEffBound.Value > 0)
                    {
                        this.numericUpDownLowerTEffBound.BackColor = Color.LightSalmon;
                    }
                    if (radioButtonTamed.Checked && extractor.statsWithEff.IndexOf(s) >= 0 && this.numericUpDownUpperTEffBound.Value < 100)
                    {
                        this.numericUpDownUpperTEffBound.BackColor = Color.LightSalmon;
                    }
                    panelWildTamedBred.BackColor = Color.LightSalmon;
                    checkBoxJustTamed.BackColor = Color.LightSalmon;
                }
            }
            if (!extractor.validResults)
            {
                extractionFailed();
                ResumeLayout();
                return false;
            }
            if (nonUniqueStats)
            {
                groupBoxPossibilities.Visible = true;
                labelDoc.Visible = true;
            }

            // if damage (s==5) has a possibility for the dom-levels to make it a valid sum, take this
            int domLevelsChosenSum = 0;
            for (int s = 0; s < 7; s++)
            {
                domLevelsChosenSum += extractor.results[s][extractor.chosenResults[s]].levelDom;
            }
            if (domLevelsChosenSum < extractor.levelDomFromTorporAndTotalRange[0] || domLevelsChosenSum > extractor.levelDomFromTorporAndTotalRange[1])
            {
                // sum of domlevels is not correct. Try to find another combination
                domLevelsChosenSum -= extractor.results[5][extractor.chosenResults[5]].levelDom;
                bool changeChosenResult = false;
                int cR = 0;
                for (int r = 0; r < extractor.results[5].Count; r++)
                {
                    if (domLevelsChosenSum + extractor.results[5][r].levelDom >= extractor.levelDomFromTorporAndTotalRange[0] && domLevelsChosenSum + extractor.results[5][r].levelDom <= extractor.levelDomFromTorporAndTotalRange[1])
                    {
                        cR = r;
                        changeChosenResult = true;
                        break;
                    }
                }
                if (changeChosenResult)
                    setPossibility(5, cR);
            }

            if (extractor.postTamed) setUniqueTE();
            else
            {
                labelTE.Text = "not yet tamed";
                labelTE.BackColor = System.Drawing.Color.Transparent;
            }

            setWildSpeedLevelAccordingToOthers();

            labelSumDomSB.Text = (extractor.levelDomFromTorporAndTotalRange[0] != extractor.levelDomFromTorporAndTotalRange[1] ? extractor.levelDomFromTorporAndTotalRange[0].ToString() + "-" : "") + extractor.levelDomFromTorporAndTotalRange[1].ToString();
            showSumOfChosenLevels();
            showStatsInOverlay();

            setActiveStat(activeStatKeeper);

            if (!extractor.postTamed)
            {
                labelFootnote.Text = "*Creature is not yet tamed and may get better values then.";
                button2TamingCalc.Visible = true;

                // display taming info
                if (checkBoxQuickWildCheck.Checked)
                    tamingControl1.level = statIOs[7].LevelWild + 1;
                else
                    tamingControl1.level = (int)numericUpDownLevel.Value;
                tamingControl1.updateTamingData();
                labelTamingInfo.Text = tamingControl1.quickTamingInfos;
                groupBoxTamingInfo.Visible = true;
            }
            ResumeLayout();
            return true;
        }

        private void extractionFailed()
        {
            buttonHelp.Visible = true;
            labelErrorHelp.Visible = true;
            groupBoxPossibilities.Visible = false;
            groupBoxRadarChartExtractor.Visible = false;
            labelDoc.Visible = false;
            if (radioButtonBred.Checked && numericUpDownImprintingBonusExtractor.Value > 0)
                labelImprintingFailInfo.Visible = true;
        }

        private void setUniqueTE()
        {
            double te = extractor.uniqueTE();
            statIOs[7].LevelWild = extractor.trueTorporLevel(te);
            if (te >= 0)
            {
                labelTE.Text = "Extracted: " + Math.Round(100 * te, 1) + " %";
                if (radioButtonTamed.Checked && extractor.postTamed)
                    labelTE.Text += " (wildlevel: " + Math.Ceiling(Math.Round((statIOs[7].LevelWild + 1) / (1 + te / 2), 6)) + ")";
                labelTE.BackColor = System.Drawing.Color.Transparent;
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
                    sumW += statIOs[s].LevelWild;
                    sumD += statIOs[s].LevelDom;
                    if (extractor.results[s].Count != 1) { allUnique = false; }
                }
                else
                {
                    valid = false;
                    break;
                }
            }
            if (valid)
            {
                sumW -= (allUnique ? 0 : statIOs[6].LevelWild);
                string offSetWild = "✓";
                labelSumDom.Text = sumD.ToString();
                if (sumW <= extractor.levelWildFromTorporRange[1]) { labelSumWild.ForeColor = SystemColors.ControlText; }
                else
                {
                    labelSumWild.ForeColor = Color.Red;
                    offSetWild = "+" + (sumW - extractor.levelWildFromTorporRange[1]).ToString();
                    inbound = false;
                }
                if (sumD <= extractor.levelDomFromTorporAndTotalRange[1] && sumD >= extractor.levelDomFromTorporAndTotalRange[0]) { labelSumDom.ForeColor = SystemColors.ControlText; }
                else
                {
                    labelSumDom.ForeColor = Color.Red;
                    inbound = false;
                }
                labelSumWild.Text = offSetWild;
            }
            else
            {
                labelSumWild.Text = "n/a";
                labelSumDom.Text = "n/a";
            }
            if (inbound)
            {
                panelSums.BackColor = SystemColors.Control;
            }
            else
            {
                panelSums.BackColor = Color.FromArgb(255, 200, 200);
            }
            bool allValid = valid && inbound && extractor.validResults;
            if (allValid)
            {
                creatureInfoInputExtractor.parentListValid = false;
                radarChartExtractor.setLevels(statIOs.Select(s => s.LevelWild).ToArray());
            }
            creatureInfoInputExtractor.ButtonEnabled = allValid;
            groupBoxRadarChartExtractor.Visible = allValid;
        }

        private void radioButtonWild_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonWild.Checked)
                updateExtrDetails();
        }

        private void radioButtonTamed_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTamed.Checked)
                updateExtrDetails();
        }

        private void radioButtonBred_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonBred.Checked)
                updateExtrDetails();
        }

        private void updateExtrDetails()
        {
            panelExtrTE.Visible = radioButtonTamed.Checked;
            panelExtrImpr.Visible = radioButtonBred.Checked;
            groupBoxDetailsExtractor.Visible = !radioButtonWild.Checked;
            checkBoxJustTamed.Checked = checkBoxJustTamed.Checked && radioButtonTamed.Checked;
            checkBoxJustTamed.Visible = radioButtonTamed.Checked;
            cbEventMultipliers.Visible = radioButtonBred.Checked;
            if (radioButtonTamed.Checked)
                groupBoxDetailsExtractor.Text = "Taming-Effectiveness";
            else if (radioButtonBred.Checked)
                groupBoxDetailsExtractor.Text = "Imprinting-Quality";
        }

        private void radioButtonTesterWild_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTesterWild.Checked)
                updateTesterDetails();
        }

        private void radioButtonTesterTamed_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTesterTamed.Checked)
                updateTesterDetails();
        }

        private void radioButtonTesterBred_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTesterBred.Checked)
                updateTesterDetails();
        }

        private void updateTesterDetails()
        {
            setTesterInputsTamed(!radioButtonTesterWild.Checked);
            NumericUpDownTestingTE.Enabled = radioButtonTesterTamed.Checked;
            labelTesterTE.Enabled = radioButtonTesterTamed.Checked;
            numericUpDownImprintingBonusTester.Enabled = radioButtonTesterBred.Checked;
            labelImprintingTester.Enabled = radioButtonTesterBred.Checked;
            labelImprintedCount.Enabled = radioButtonTesterBred.Checked;

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
                this.listViewPossibilities.Items.Clear();
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
                ListViewItem lvi;
                List<string> subItems = new List<string>();
                double te;
                for (int r = 0; r < extractor.results[s].Count; r++)
                {
                    subItems.Clear();
                    te = extractor.results[s][r].TE;
                    subItems.Add(extractor.results[s][r].levelWild.ToString());
                    subItems.Add(extractor.results[s][r].levelDom.ToString());
                    subItems.Add((te >= 0 ? (te * 100).ToString() : ""));

                    subItems.Add((te > 0 ? Math.Ceiling((extractor.trueTorporLevel(te) + 1) / (1 + te / 2)).ToString() : ""));

                    lvi = new ListViewItem(subItems.ToArray());
                    if (!resultsValid || extractor.results[s][r].currentlyNotValid)
                        lvi.BackColor = Color.LightSalmon;
                    if (extractor.fixedResults[s] && extractor.chosenResults[s] == r)
                    {
                        lvi.BackColor = Color.LightSkyBlue;
                    }

                    lvi.Tag = r;

                    this.listViewPossibilities.Items.Add(lvi);
                }
            }
        }

        private void updateSpeciesComboboxes()
        {
            comboBoxSpeciesGlobal.Items.Clear();
            for (int s = 0; s < Values.V.speciesNames.Count; s++)
                comboBoxSpeciesGlobal.Items.Add(Values.V.speciesNames[s]);

            if (comboBoxSpeciesGlobal.Items.Count > 0)
                comboBoxSpeciesGlobal.SelectedIndex = 0;
        }

        private void comboBoxSpeciesGlobal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSpeciesGlobal.SelectedIndex >= 0)
            {
                speciesIndex = comboBoxSpeciesGlobal.SelectedIndex;
                creatureInfoInputExtractor.SpeciesIndex = speciesIndex;
                creatureInfoInputTester.SpeciesIndex = speciesIndex;
                for (int s = 0; s < 8; s++)
                {
                    activeStats[s] = (Values.V.species[speciesIndex].stats[s].BaseValue > 0) && (s != 2 || !Values.V.species[speciesIndex].doesNotUseOxygen || oxygenForAll);
                    statIOs[s].Enabled = activeStats[s];
                }
                if (tabControlMain.SelectedTab == tabPageExtractor)
                {
                    clearAll();
                }
                else if (tabControlMain.SelectedTab == tabPageStatTesting)
                {
                    updateAllTesterValues();
                    statPotentials1.speciesIndex = speciesIndex;
                    statPotentials1.setLevels(testingIOs.Select(s => s.LevelWild).ToArray(), true);
                    setTesterInfoInputCreature();
                }
                else if (tabControlMain.SelectedTab == tabPageTaming)
                {
                    tamingControl1.setSpeciesIndex(comboBoxSpeciesGlobal.SelectedIndex);
                }
                else if (tabControlMain.SelectedTab == tabPageRaising)
                {
                    raisingControl1.updateRaisingData(comboBoxSpeciesGlobal.SelectedIndex);
                }
            }
        }

        private void listViewPossibilities_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewPossibilities.SelectedIndices.Count > 0)
            {
                int index = (int)listViewPossibilities.SelectedItems[0].Tag;
                if (index >= 0 && activeStat >= 0)
                {
                    setPossibility(activeStat, index, true);
                    extractor.fixedResults[activeStat] = true;
                }
            }
            else if (activeStat >= 0)
                extractor.fixedResults[activeStat] = false;
        }

        private void setPossibility(int s, int i, bool validateCombination = false)
        {
            statIOs[s].LevelWild = (Int32)extractor.results[s][i].levelWild;
            statIOs[s].LevelDom = (Int32)extractor.results[s][i].levelDom;
            statIOs[s].TamingEffectiveness = (Int32)extractor.results[s][i].TE;
            statIOs[s].BreedingValue = breedingValue(s, i);
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
            /*
             * wild speed level is wildTotalLevels - determinedWildLevels. sometimes the oxygenlevel cannot be determined
             * if TE cannot be determined, speed cannot as well
             */
            bool unique = true;
            int notDeterminedLevels = statIOs[7].LevelWild;
            if (extractor.lastTEUnique)
            {
                for (int s = 0; s < 6; s++)
                {
                    if (activeStats[s])
                    {
                        //notDeterminedLevels -= statIOs[s].LevelDom;
                        notDeterminedLevels -= statIOs[s].LevelWild;
                    }
                    else { unique = false; break; }
                }
            }
            if (unique && extractor.lastTEUnique)
            {
                // if all other stats are unique, set speedlevel
                statIOs[6].LevelWild = Math.Max(0, notDeterminedLevels);
                statIOs[6].Unknown = false;
            }
            else
            {
                // if not all other levels are unique, set speed and not known levels to unknown
                for (int s = 0; s < 7; s++)
                {
                    if (s == 6 || !activeStats[s])
                    {
                        statIOs[s].LevelWild = 0;
                        statIOs[s].Unknown = true;
                    }
                }
            }
        }

        private void CopyExtractionToClipboard()
        {
            bool header = true;
            bool table = (MessageBox.Show("Results can be copied as own table or as a long table-row. Should it be copied as own table?", "Copy as own table?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
            if (extractor.validResults && speciesIndex >= 0)
            {
                List<string> tsv = new List<string>();
                string rowLevel = Values.V.speciesNames[speciesIndex] + "\t\t", rowValues = "";
                // if taming effectiveness is unique, display it, too
                string effString = "";
                double eff = extractor.uniqueTE();
                if (eff >= 0)
                {
                    effString = "\tTamingEff:\t" + (100 * eff).ToString() + "%";
                }
                // headerrow
                if (table || header)
                {
                    if (table)
                    {
                        tsv.Add(Values.V.speciesNames[speciesIndex] + "\tLevel " + numericUpDownLevel.Value.ToString() + effString);
                        tsv.Add("Stat\tWildLevel\tDomLevel\tBreedingValue");
                    }
                    else { tsv.Add("Species\tName\tSex\tHP-Level\tSt-Level\tOx-Level\tFo-Level\tWe-Level\tDm-Level\tSp-Level\tTo-Level\tHP-Value\tSt-Value\tOx-Value\tFo-Value\tWe-Value\tDm-Value\tSp-Value\tTo-Value"); }
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
                            tsv.Add(Utils.statName(s) + "\t" + (activeStats[s] ? statIOs[s].LevelWild.ToString() : "") + "\t" + (activeStats[s] ? statIOs[s].LevelWild.ToString() : "") + "\t" + breedingV);
                        }
                        else
                        {
                            rowLevel += "\t" + (activeStats[s] ? statIOs[s].LevelWild.ToString() : "");
                            rowValues += "\t" + breedingV;
                        }
                    }
                    else { return; }
                }
                if (!table) { tsv.Add(rowLevel + rowValues); }
                Clipboard.SetText(string.Join("\n", tsv));
            }
        }

        private double breedingValue(int s, int r)
        {
            if (s >= 0 && s < 8)
            {
                if (r >= 0 && r < extractor.results[s].Count)
                {
                    return Stats.calculateValue(speciesIndex, s, extractor.results[s][r].levelWild, 0, true, 1, 0);
                }
            }
            return -1;
        }

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            if (n != null)
            {
                n.Select(0, n.Text.Length);
            }
        }

        private void recalculateAllCreaturesValues()
        {
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = creatureCollection.creatures.Count();
            toolStripProgressBar1.Visible = true;
            foreach (Creature c in creatureCollection.creatures)
            {
                c.recalculateCreatureValues();
                toolStripProgressBar1.Value++;
            }
            toolStripProgressBar1.Visible = false;
        }

        private void creatureInfoInput1_Add2Library_Clicked(CreatureInfoInput sender)
        {
            add2Lib(true);
        }

        private void creatureInfoInputTester_Add2Library_Clicked(CreatureInfoInput sender)
        {
            add2Lib(false);
        }

        private void add2Lib(bool fromExtractor = true)
        {
            CreatureInfoInput input;
            bool bred;
            double te, imprinting;
            string species;
            if (fromExtractor)
            {
                input = creatureInfoInputExtractor;
                species = Values.V.speciesNames[speciesIndex];
                bred = radioButtonBred.Checked;
                te = extractor.uniqueTE();
                imprinting = extractor.imprintingBonus;
            }
            else
            {
                input = creatureInfoInputTester;
                species = Values.V.speciesNames[speciesIndex];
                bred = radioButtonTesterBred.Checked;
                te = (double)NumericUpDownTestingTE.Value / 100;
                imprinting = (double)numericUpDownImprintingBonusTester.Value / 100;
            }

            Creature creature = new Creature(species, input.CreatureName, input.CreatureOwner, input.CreatureTribe, input.CreatureSex, getCurrentWildLevels(fromExtractor), getCurrentDomLevels(fromExtractor), te, bred, imprinting);

            // set parents
            creature.Mother = input.mother;
            creature.Father = input.father;

            // cooldown-, growing-time
            creature.cooldownUntil = input.Cooldown;
            creature.growingUntil = input.Grown;

            creature.domesticatedAt = input.domesticatedAt;
            creature.mutationCounter = input.MutationCounter;

            creature.recalculateCreatureValues();
            creature.recalculateAncestorGenerations();
            creature.guid = Guid.NewGuid();
            creatureCollection.creatures.Add(creature);
            setCollectionChanged(true, species);
            updateCreatureListings(Values.V.speciesNames.IndexOf(species));
            // show only the added creatures' species
            listBoxSpeciesLib.SelectedIndex = listBoxSpeciesLib.Items.IndexOf(creature.species);
            tabControlMain.SelectedTab = tabPageLibrary;

            creatureInfoInputExtractor.parentListValid = false;
            creatureInfoInputTester.parentListValid = false;
        }

        private int[] getCurrentWildLevels(bool fromExtractor = true)
        {
            int[] levelsWild = new int[8];
            for (int s = 0; s < 8; s++) { levelsWild[s] = (fromExtractor ? (statIOs[s].Unknown ? -1 : statIOs[s].LevelWild) : testingIOs[s].LevelWild); }
            return levelsWild;
        }

        private int[] getCurrentDomLevels(bool fromExtractor = true)
        {
            int[] levelsDom = new int[8];
            for (int s = 0; s < 8; s++) { levelsDom[s] = (fromExtractor ? statIOs[s].LevelDom : testingIOs[s].LevelDom); }
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
        }

        private void applySettingsToValues()
        {
            // apply multipliers
            Values.V.applyMultipliers(creatureCollection);
            applyEvolutionMultipliers();

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
            statPotentials1.levelDomMax = creatureCollection.maxDomLevel;
            statPotentials1.levelGraphMax = creatureCollection.maxChartLevel;

            if (speechRecognition != null)
                speechRecognition.setMaxLevel(creatureCollection.maxWildLevel);
            if (overlay != null)
                overlay.InfoDuration = Properties.Settings.Default.OverlayInfoDuration;

            oxygenForAll = Properties.Settings.Default.oxygenForAll;
            ArkOCR.OCR.screenCaptureApplicationName = Properties.Settings.Default.OCRApp;
            Values.V.celsius = Properties.Settings.Default.celsius;
            ArkOCR.OCR.waitBeforeScreenCapture = Properties.Settings.Default.waitBeforeScreenCapture;

            ocrControl1.setWhiteThreshold(Properties.Settings.Default.OCRWhiteThreshold);

            // sound-files
            timerList1.sounds = new System.Media.SoundPlayer[] {
                        File.Exists(Properties.Settings.Default.soundStarving) ? new System.Media.SoundPlayer(Properties.Settings.Default.soundStarving) : null,
                        File.Exists(Properties.Settings.Default.soundWakeup) ? new System.Media.SoundPlayer(Properties.Settings.Default.soundWakeup) : null,
                        File.Exists(Properties.Settings.Default.soundBirth) ? new System.Media.SoundPlayer(Properties.Settings.Default.soundBirth) : null
                        };
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
                if (MessageBox.Show("Your Creature Collection has been modified since it was last saved, are you sure you want to load without saving first?", "Discard Changes?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    return;
            }
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Creature Collection File (*.xml)|*.xml";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Creature Collection File (*.xml)|*.xml";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                currentFileName = dlg.FileName;
                fileSync.changeFile(currentFileName);
                saveCollectionToFileName(currentFileName);
            }
        }

        private void saveCollectionToFileName(string fileName)
        {
            XmlSerializer writer = new XmlSerializer(typeof(CreatureCollection));
            try
            {
                fileSync.justSaving();
                System.IO.FileStream file = System.IO.File.Create(fileName);
                writer.Serialize(file, creatureCollection);
                file.Close();
                Properties.Settings.Default.LastSaveFile = fileName;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error during serialization.\nErrormessage:\n\n" + e.Message, "Serialization-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            setCollectionChanged(false);
        }

        private bool loadCollectionFile(string fileName, bool keepCurrentCreatures = false)
        {
            XmlSerializer reader = new XmlSerializer(typeof(CreatureCollection));

            if (!System.IO.File.Exists(fileName))
            {
                MessageBox.Show("Save file with name \"" + fileName + "\" does not exist!", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            List<Creature> oldCreatures = null;
            if (keepCurrentCreatures)
                oldCreatures = creatureCollection.creatures;

            System.IO.FileStream file = System.IO.File.OpenRead(fileName);

            // for the case the collectionfile has no multipliers, keep the current ones
            double[][] oldMultipliers = creatureCollection.multipliers;

            // if the old collection had additional values, reload the original values
            bool hadAdditionalValues = creatureCollection != null && creatureCollection.additionalValues.Length > 0;

            try
            {
                creatureCollection = (CreatureCollection)reader.Deserialize(file);
            }
            catch (Exception e)
            {
                MessageBox.Show("File Couldn't be opened, we thought you should know.\nErrormessage:\n\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                file.Close();
                return false;
            }
            file.Close();

            nameFixes(creatureCollection);

            if (hadAdditionalValues) Values.V.loadValues();
            if (creatureCollection.additionalValues.Length > 0) loadAdditionalValues(Path.GetDirectoryName(fileName) + @"\" + creatureCollection.additionalValues);

            if (creatureCollection.multipliers == null)
            {
                creatureCollection.multipliers = oldMultipliers;
                if (creatureCollection.multipliers == null)
                    creatureCollection.multipliers = Values.V.getOfficialMultipliers();
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
            checkBoxShowDead.Checked = creatureCollection.showDeads;
            checkBoxShowUnavailableCreatures.Checked = creatureCollection.showUnavailable;
            checkBoxShowNeuteredCreatures.Checked = creatureCollection.showNeutered;
            checkBoxShowMutatedCreatures.Checked = creatureCollection.showMutated;
            checkBoxUseFiltersInTopStatCalculation.Checked = creatureCollection.useFiltersInTopStatCalculation;
            filterListAllowed = true;

            setCollectionChanged(creatureWasAdded); // setCollectionChanged only if there really were creatures added from the old library to the just opened one
            // creatures loaded.

            lastAutoSaveBackup = DateTime.Now.AddMinutes(-10);

            // calculate creature values
            recalculateAllCreaturesValues();

            if (creatureCollection.creatures.Count > 0)
                tabControlMain.SelectedTab = tabPageLibrary;

            creatureBoxListView.maxDomLevel = creatureCollection.maxDomLevel;

            // pedigree
            pedigree1.Clear();
            // breedingPlan
            breedingPlan1.Clear();

            updateCreatureListings();

            // apply last sorting
            this.listViewLibrary.Sort();

            Properties.Settings.Default.LastSaveFile = fileName;
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
            updateTreeListSpecies(creatureCollection.creatures);
            filterLib();
            updateStatusBar();
            breedingPlan1.CurrentSpecies = ""; // set to empty so creatures are loaded again if breeding plan is created
            pedigree1.updateListView();
            raisingControl1.recreateList();
        }

        /// <summary>
        /// This function should be called if the creatureCollection is changed, i.e. after loading a file or adding/removing a creature. It updated the listed species in the treelist.
        /// </summary>
        private void updateTreeListSpecies(List<Creature> creatures)
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
                if (listBoxSpeciesLib.Items.IndexOf(cr.species) == -1)
                {
                    // add new node alphabetically
                    int nn = 0;
                    while (nn < listBoxSpeciesLib.Items.Count && string.Compare(listBoxSpeciesLib.Items[nn].ToString(), cr.species, true) < 0) nn++;
                    listBoxSpeciesLib.Items.Insert(nn, cr.species);
                    availableSpeciesNames.Insert(nn, cr.species);
                }
            }
            // add node to show all
            listBoxSpeciesLib.Items.Insert(0, "All");

            if (selectedSpecies.Length > 0)
                listBoxSpeciesLib.SelectedIndex = listBoxSpeciesLib.Items.IndexOf(selectedSpecies);

            breedingPlan1.setSpeciesList(availableSpeciesNames, creatures);
        }

        private void createOwnerList()
        {
            filterListAllowed = false;
            checkedListBoxOwner.Items.Clear();
            bool removeWOOwner = true;
            checkedListBoxOwner.Items.Add("n/a", (creatureCollection.hiddenOwners.IndexOf("n/a") == -1));
            foreach (Creature c in creatureCollection.creatures)
            {
                if (c.owner == null || c.owner.Length == 0)
                    removeWOOwner = false;
                else if (c.owner.Length > 0 && checkedListBoxOwner.Items.IndexOf(c.owner) == -1)
                {
                    checkedListBoxOwner.Items.Add(c.owner, (creatureCollection.hiddenOwners.IndexOf(c.owner) == -1));
                    if (!tribesControl1.playerExists(c.owner))
                        tribesControl1.addPlayer(c.owner);
                }
            }
            if (removeWOOwner)
                checkedListBoxOwner.Items.RemoveAt(0);
            string[] owners = tribesControl1.playerNames;
            creatureInfoInputExtractor.AutocompleteOwnerList = owners;
            creatureInfoInputTester.AutocompleteOwnerList = owners;
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
                listViewLibrary.Items.Add(createCreatureLVItem(cr, g));
            }
            listViewLibrary.EndUpdate();
        }

        /// <summary>
        /// Call this function to update the displayed values of a creature. Usually called after a creature was edited.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cr">Creature that was changed</param>
        /// <param name="creatureStatusChanged"></param>
        private void updateCreatureValues(Creature cr, bool creatureStatusChanged)
        {
            // data of the selected creature changed, update listview
            cr.recalculateCreatureValues();
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
        }

        private ListViewItem createCreatureLVItem(Creature cr, ListViewGroup g)
        {
            double colorFactor = 100d / creatureCollection.maxChartLevel;
            DateTime cldGr = cr.cooldownUntil > cr.growingUntil ? cr.cooldownUntil : cr.growingUntil;
            bool cld = cr.cooldownUntil > cr.growingUntil;

            string[] subItems = (new string[] { cr.name + (cr.status != CreatureStatus.Available ? " (" + Utils.statusSymbol(cr.status) + ")" : ""),
                cr.owner + (cr.tribe.Length > 0 ? " (" + cr.tribe + ")" : ""),
                Utils.sexSymbol(cr.gender),
                cr.domesticatedAt.ToString("yyyy'-'MM'-'dd HH':'mm"),
                cr.topness.ToString(),
                cr.topStatsCount.ToString(),
                cr.generation.ToString(),
                cr.levelFound.ToString(),
                cr.mutationCounter.ToString(),
                (DateTime.Now.CompareTo(cldGr) < 0 ? cldGr.ToString() : "-") }).Concat(cr.levelsWild.Select(x => x.ToString()).ToArray()).ToArray();

            ListViewItem lvi = new ListViewItem(subItems, g);
            for (int s = 0; s < 8; s++)
            {
                // color unknown levels
                if (cr.levelsWild[s] < 0)
                {
                    lvi.SubItems[s + 10].ForeColor = Color.WhiteSmoke;
                    lvi.SubItems[s + 10].BackColor = Color.WhiteSmoke;
                }
                else
                    lvi.SubItems[s + 10].BackColor = Utils.getColorFromPercent((int)(cr.levelsWild[s] * (s == 7 ? colorFactor / 7 : colorFactor)), (considerStatHighlight[s] ? (cr.topBreedingStats[s] ? 0.2 : 0.7) : 0.93));
            }
            lvi.SubItems[2].BackColor = cr.neutered ? SystemColors.GrayText : (cr.gender == Sex.Female ? Color.FromArgb(255, 230, 255) : (cr.gender == Sex.Male ? Color.FromArgb(220, 235, 255) : SystemColors.Window));
            if (cr.status == CreatureStatus.Dead)
            {
                lvi.SubItems[0].ForeColor = SystemColors.GrayText;
                lvi.BackColor = Color.FromArgb(255, 250, 240);
            }
            if (cr.status == CreatureStatus.Unavailable)
            {
                lvi.SubItems[0].ForeColor = SystemColors.GrayText;
            }

            lvi.UseItemStyleForSubItems = false;

            // color for top-stats-nr
            if (cr.topStatsCount > 0)
            {
                if (cr.topBreedingCreature)
                    lvi.BackColor = Color.LightGreen;
                lvi.SubItems[5].BackColor = Utils.getColorFromPercent(cr.topStatsCount * 8 + 44, 0.7);
            }
            else
            {
                lvi.SubItems[5].ForeColor = Color.LightGray;
            }

            // color for timestamp added
            if (cr.domesticatedAt.Year < 2015)
            {
                lvi.SubItems[3].Text = "n/a";
                lvi.SubItems[3].ForeColor = Color.LightGray;
            }

            // color for topness
            lvi.SubItems[4].BackColor = Utils.getColorFromPercent(cr.topness * 2 - 100, 0.8); // topness is in percent. gradient from 50-100

            // color for generation
            if (cr.generation == 0)
                lvi.SubItems[6].ForeColor = Color.LightGray;

            // color of WildLevelColumn
            if (cr.levelFound == 0)
                lvi.SubItems[7].ForeColor = Color.LightGray;

            // color for mutation
            if (cr.mutationCounter > 0)
                lvi.SubItems[8].BackColor = Color.FromArgb(225, 192, 255);
            else
                lvi.SubItems[8].ForeColor = Color.LightGray;

            // color for cooldown
            Color forecolor, backcolor;
            cooldownColors(cr, out forecolor, out backcolor);
            lvi.SubItems[9].ForeColor = forecolor;
            lvi.SubItems[9].BackColor = backcolor;

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

        private void checkForUpdates(bool silentCheck = false)
        {
            bool updated = false;
            bool newToolVersionAvailable = false;
            bool newValuesAvailable = false;
            try
            {
                string remoteUri = "https://github.com/cadon/ARKStatsExtractor/raw/master/ARKBreedingStats/";
                // Create a new WebClient instance.
                System.Net.WebClient myWebClient = new System.Net.WebClient();
                // first number is stat-version, second is multiplier-version
                string[] remoteVers = myWebClient.DownloadString(remoteUri + "ver.txt").Split(',');

                // update last updateCheck
                Properties.Settings.Default.lastUpdateCheck = DateTime.Now;

                if (remoteVers.Length != 2)
                {
                    if (MessageBox.Show("Error while checking for new version, bad remote-format. Try checking for an updated version of this tool. Do you want to visit the homepage of the tool?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                        System.Diagnostics.Process.Start("https://github.com/cadon/ARKStatsExtractor/releases/latest");
                    return;
                }

                // check if a new version of the tool is available
                Version localVersion, remoteVersion;
                try
                {
                    localVersion = new Version(Application.ProductVersion);
                    remoteVersion = new Version(remoteVers[1].Trim());
                }
                catch
                {
                    if (MessageBox.Show("Error while checking for new tool-version, bad remote-format. Try checking for an updated version of this tool. Do you want to visit the homepage of the tool?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                        System.Diagnostics.Process.Start("https://github.com/cadon/ARKStatsExtractor/releases/latest");
                    return;
                }
                if (localVersion.CompareTo(remoteVersion) < 0)
                {
                    if (MessageBox.Show("A new version of ARK Smart Breeding is available. Do you want to visit the homepage to check it out?", "New version available", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("https://github.com/cadon/ARKStatsExtractor/releases/latest");
                        return;
                    }
                    newToolVersionAvailable = true;
                }

                // check if values.json can be updated
                string filename = "values.json";
                try
                {
                    localVersion = Values.V.version;
                    remoteVersion = new Version(remoteVers[0].Trim());
                }
                catch
                {
                    if (MessageBox.Show("Error while checking for values-version, bad remote-format. Try checking for an updated version of this tool. Do you want to visit the homepage of the tool?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                        System.Diagnostics.Process.Start("https://github.com/cadon/ARKStatsExtractor/releases/latest");
                    return;
                }
                if (localVersion.CompareTo(remoteVersion) < 0)
                {
                    newValuesAvailable = true;
                    if (MessageBox.Show("There is a new version of the values-file \"" + filename + "\", do you want to update it?\n\nIf you play on a console (Xbox or PS4) make a backup of the current file before you click on Yes, as the updated values may not work with the console-version for some time.\nUsually it takes up to some days or weeks until the patch is released for the consoles as well and the changes are valid on there, too.", "Update Values-File?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // System.IO.File.Copy(filename, filename + "_backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json");
                        // Download the Web resource and save it into the current filesystem folder.
                        myWebClient.DownloadFile(remoteUri + "json/" + filename, "json/" + filename);
                        updated = true;
                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                if (!silentCheck)
                    MessageBox.Show("Error while checking for new version or downloading it:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (updated)
            {
                if (Values.V.loadValues())
                {
                    applySettingsToValues();
                    updateSpeciesComboboxes();
                    MessageBox.Show("Download and update of new creature-stats successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    updateStatusBar();
                }
                else
                    MessageBox.Show("Download of new stat successful, but files couldn't be loaded.\nTry again later, or redownload the tool.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!silentCheck && !newToolVersionAvailable && !newValuesAvailable)
            {
                MessageBox.Show("You already have the newest version of the" + (!newToolVersionAvailable ? " tool and the" : "") + " values-file.\n\nIf your stats are outdated and no new version is available, we probably don't have the new ones either.", "No new Version", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void creatureInfoInput_ParentListRequested(CreatureInfoInput sender)
        {
            updateParentListInput(sender);
        }

        private void updateParentListInput(CreatureInfoInput input)
        {
            // set possible parents
            bool fromExtractor = input == creatureInfoInputExtractor;
            string species = Values.V.speciesNames[speciesIndex];
            Creature creature = new Creature(species, "", "", "", 0, getCurrentWildLevels(fromExtractor));
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
                if (MessageBox.Show("Your Creature Collection has been modified since it was last saved, are you sure you want to discard your changes and create a new Library without saving?", "Discard Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                    return;
            }

            if (creatureCollection.additionalValues.Length > 0) Values.V.loadValues(); // if old collection had additionalValues, load the original ones.

            if (creatureCollection.multipliers == null)
                creatureCollection.multipliers = Values.V.getOfficialMultipliers();
            // use previously used multipliers again in the new file
            double[][] oldMultipliers = creatureCollection.multipliers;

            creatureCollection = new CreatureCollection();
            creatureCollection.multipliers = oldMultipliers;
            pedigree1.Clear();
            breedingPlan1.Clear();
            applySettingsToValues();
            initializeCollection();

            updateCreatureListings();
            creatureBoxListView.Clear();
            Properties.Settings.Default.LastSaveFile = "";
            currentFileName = "";
            fileSync.changeFile(currentFileName);
            setCollectionChanged(false);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (collectionDirty && (MessageBox.Show("Your Creature Collection has been modified since it was last saved, are you sure you want to discard your changes and quit without saving?", "Discard Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No))
                e.Cancel = true;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // save consideredStats
            int consideredStats = 0;
            for (int s = 0; s < 8; s++)
            {
                if (considerStatHighlight[s])
                    consideredStats += (1 << s);
            }
            Properties.Settings.Default.consideredStats = consideredStats;

            // save window-position and size
            if (this.WindowState != FormWindowState.Minimized)
            {
                Properties.Settings.Default.formSize = this.Size;
                Properties.Settings.Default.formLocation = this.Location;
            }

            // save column-widths
            Int32[] cw = new Int32[listViewLibrary.Columns.Count];
            for (int c = 0; c < listViewLibrary.Columns.Count; c++)
                cw[c] = listViewLibrary.Columns[c].Width;
            Properties.Settings.Default.columnWidths = cw;

            // save listViewSorting
            ListViewColumnSorter lwvs = (ListViewColumnSorter)listViewLibrary.ListViewItemSorter;
            if (lwvs != null)
            {
                Properties.Settings.Default.listViewSortCol = lwvs.SortColumn;
                Properties.Settings.Default.listViewSortAsc = (lwvs.Order == SortOrder.Ascending);
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
            if (comboBoxSpeciesGlobal.SelectedIndex >= 0)
                Properties.Settings.Default.lastSpecies = comboBoxSpeciesGlobal.SelectedItem.ToString();

            // save settings for next session
            Properties.Settings.Default.Save();

            // remove old cache-files
            if (Directory.Exists("img/cache"))
            {
                var directory = new System.IO.DirectoryInfo("img/cache");
                var oldCacheFiles = directory.GetFiles().Where(f => f.LastAccessTime < DateTime.Now.AddDays(-5)).ToList();
                foreach (FileInfo f in oldCacheFiles)
                {
                    f.Delete();
                }
            }
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnSorter.doSort((ListView)sender, e.Column);
        }

        private void listViewLibrary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedItems.Count == 1)
            {
                Creature c = (Creature)listViewLibrary.SelectedItems[0].Tag;
                creatureBoxListView.setCreature(c);
                pedigreeNeedsUpdate = true;
            }
        }

        private void checkBoxShowDead_CheckedChanged(object sender, EventArgs e)
        {
            creatureCollection.showDeads = checkBoxShowDead.Checked;
            recalculateTopStatsIfNeeded();
            filterLib();
        }

        private void checkBoxShowUnavailableCreatures_CheckedChanged(object sender, EventArgs e)
        {
            creatureCollection.showUnavailable = checkBoxShowUnavailableCreatures.Checked;
            recalculateTopStatsIfNeeded();
            filterLib();
        }

        private void checkBoxShowNeuteredCreatures_CheckedChanged(object sender, EventArgs e)
        {
            creatureCollection.showNeutered = checkBoxShowNeuteredCreatures.Checked;
            recalculateTopStatsIfNeeded();
            filterLib();
        }

        private void checkBoxShowMutatedCreatures_CheckedChanged(object sender, EventArgs e)
        {
            creatureCollection.showMutated = checkBoxShowMutatedCreatures.Checked;
            recalculateTopStatsIfNeeded();
            filterLib();
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

        private void checkedListBoxOwner_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (filterListAllowed)
            {
                // update shownOwners
                string owner = checkedListBoxOwner.Items[e.Index].ToString();
                if (e.NewValue == CheckState.Unchecked) { creatureCollection.hiddenOwners.Add(owner); }
                else { creatureCollection.hiddenOwners.Remove(owner); }

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
                List<Creature> selectedCreatures = new List<Creature>();
                foreach (ListViewItem i in listViewLibrary.SelectedItems)
                    selectedCreatures.Add((Creature)i.Tag);

                var filteredList = from creature in creatureCollection.creatures
                                   select creature;

                // if only one species should be shown
                if (listBoxSpeciesLib.SelectedItem != null && listBoxSpeciesLib.SelectedItem.ToString() != "All")
                    filteredList = filteredList.Where(c => c.species == listBoxSpeciesLib.SelectedItem.ToString());

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
            bool hideWOOwner = (creatureCollection.hiddenOwners.IndexOf("n/a") >= 0);
            creatures = creatures.Where(c => !creatureCollection.hiddenOwners.Contains(c.owner) && (!hideWOOwner || c.owner != ""));

            // show also dead creatures?
            if (!checkBoxShowDead.Checked)
                creatures = creatures.Where(c => c.status != CreatureStatus.Dead);

            // show also unavailable creatures?
            if (!checkBoxShowUnavailableCreatures.Checked)
                creatures = creatures.Where(c => c.status != CreatureStatus.Unavailable);

            // show also neutered creatures?
            if (!checkBoxShowNeuteredCreatures.Checked)
                creatures = creatures.Where(c => !c.neutered);

            // show also creatures with mutations?
            if (!checkBoxShowMutatedCreatures.Checked)
                creatures = creatures.Where(c => c.mutationCounter <= 0);

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
                    if (MessageBox.Show("Do you really want to delete the entry and all data for \"" + ((Creature)listViewLibrary.SelectedItems[0].Tag).name + "\"" + (listViewLibrary.SelectedItems.Count > 1 ? " and " + (listViewLibrary.SelectedItems.Count - 1) + " other creatures" : "") + "?", "Delete Creature?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
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
                        updateCreatureListings((onlyOneSpecies ? speciesIndex : -1));
                        setCollectionChanged(true, (onlyOneSpecies ? species : null));
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
            Int32[] bestStat;
            List<Creature>[] bestCreatures;
            bool noCreaturesInThisSpecies;
            int specInd;
            foreach (string species in Values.V.speciesNames)
            {
                specInd = Values.V.speciesNames.IndexOf(species);
                toolStripProgressBar1.Value++;
                bestStat = new Int32[] { -1, -1, -1, -1, -1, -1, -1, -1 };
                bestCreatures = new List<Creature>[Enum.GetNames(typeof(StatName)).Count()];
                noCreaturesInThisSpecies = true;
                foreach (Creature c in creatures)
                {
                    if (c.species != species)
                        continue;

                    noCreaturesInThisSpecies = false;
                    // reset topBreeding stats for this creature
                    c.topBreedingStats = new bool[8];
                    c.topBreedingCreature = false;

                    if (!creatureCollection.useFiltersInTopStatCalculation)
                    {
                        // if not available, continue
                        if (c.status != CreatureStatus.Available)
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
                        if (c.levelsWild[s] == bestStat[s] && c.levelsWild[s] > 0)
                        {
                            bestCreatures[s].Add(c);
                        }
                        else if (c.levelsWild[s] > bestStat[s])
                        {
                            bestCreatures[s] = new List<Creature>();
                            bestCreatures[s].Add(c);
                            bestStat[s] = c.levelsWild[s];
                        }
                    }
                }
                if (noCreaturesInThisSpecies)
                {
                    continue;
                }

                if (!topStats.ContainsKey(species))
                {
                    topStats.Add(species, bestStat);
                }

                // beststat and bestcreatures now contain the best stats and creatures for each stat.

                // set topness of each creature (== mean wildlevels/mean top wildlevels in permille)
                int sumTopLevels = 0, sumCreatureLevels;
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
                        sumCreatureLevels = 0;
                        for (int s = 0; s < 8; s++)
                        {
                            if (considerStatHighlight[s])
                                sumCreatureLevels += c.levelsWild[s];
                        }
                        c.topness = (Int16)(100 * sumCreatureLevels / sumTopLevels);
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
                        if (bestCreatures[s][c].gender != Sex.Male)
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
                                if (bestCreatures[s][oc].gender != Sex.Male)
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
        private void updateParents(List<Creature> creatures)
        {
            Creature mother, father;
            foreach (Creature c in creatures)
            {
                if (c.motherGuid != Guid.Empty || c.fatherGuid != Guid.Empty)
                {
                    mother = null;
                    father = null;
                    foreach (Creature p in creatureCollection.creatures)
                    {
                        if (c.motherGuid == p.guid)
                        {
                            mother = p;
                            if (father != null)
                                break;
                        }
                        else if (c.fatherGuid == p.guid)
                        {
                            father = p;
                            if (mother != null)
                                break;
                        }
                    }
                    c.Mother = mother;
                    c.Father = father;
                }
            }
        }
        /// <summary>
        /// Sets the parentsof the incubation-timers according to the guids. Call after a file is loaded.
        /// </summary>
        /// <param name="creatures"></param>
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
            this.Close();
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
            var fatherList = from cr in creatureCollection.creatures
                             where cr.species == creature.species
                                        && cr.gender == Sex.Male
                                        && cr != creature
                             orderby cr.name ascending
                             select cr;
            var motherList = from cr in creatureCollection.creatures
                             where cr.species == creature.species
                                        && cr.gender == Sex.Female
                                        && cr != creature
                             orderby cr.name ascending
                             select cr;

            // display new results
            return new List<Creature>[2] { motherList.ToList(), fatherList.ToList() };
        }
        private List<int>[] findParentSimilarities(List<Creature>[] parents, Creature creature)
        {
            // similarities (number of equal wildlevels as creature, to find parents easier)
            int e;// number of equal wildlevels
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

        //tabcontrolmainchange, tabchange
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            timerList1.updateTimer = (tabControlMain.SelectedTab == tabPageTimer);
            toolStripButtonCopy2Extractor.Visible = (tabControlMain.SelectedTab == tabPageStatTesting);
            toolStripButtonCopy2Tester.Visible = (tabControlMain.SelectedTab == tabPageExtractor);
            toolStripButtonExtract.Visible = (tabControlMain.SelectedTab == tabPageExtractor);
            toolStripButtonAddPlayer.Visible = (tabControlMain.SelectedTab == tabPagePlayerTribes);
            toolStripButtonAddTribe.Visible = (tabControlMain.SelectedTab == tabPagePlayerTribes);
            toolStripButtonClear.Visible = (tabControlMain.SelectedTab == tabPageExtractor || tabControlMain.SelectedTab == tabPageStatTesting);
            forARKChatToolStripMenuItem.Visible = (tabControlMain.SelectedTab == tabPageLibrary);
            //creatureToolStripMenuItem.Enabled = (tabControl1.SelectedTab == tabPageLibrary);
            copyCreatureToolStripMenuItem.Visible = (tabControlMain.SelectedTab == tabPageLibrary);
            toolStripButtonAddNote.Visible = tabControlMain.SelectedTab == tabPageNotes;
            toolStripButtonRemoveNote.Visible = tabControlMain.SelectedTab == tabPageNotes;
            raisingControl1.updateListView = tabControlMain.SelectedTab == tabPageRaising;
            toolStripButtonDeleteExpiredIncubationTimers.Visible = tabControlMain.SelectedTab == tabPageRaising || tabControlMain.SelectedTab == tabPageTimer;

            if (tabControlMain.SelectedTab == tabPageStatTesting)
            {
                updateAllTesterValues();
                statPotentials1.speciesIndex = speciesIndex;
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
                        pedigree1.EnabledColorRegions = (s >= 0 ? Values.V.species[s].colors.Select(n => n.name != "").ToArray() : new bool[6] { true, true, true, true, true, true });
                    }
                    pedigree1.setCreature(c, true);
                    pedigreeNeedsUpdate = false;
                }
            }
            else if (tabControlMain.SelectedTab == tabPageTaming)
            {
                tamingControl1.setSpeciesIndex(comboBoxSpeciesGlobal.SelectedIndex);
            }
            else if (tabControlMain.SelectedTab == tabPageBreedingPlan)
            {
                breedingPlan1.updateIfNeeded();
            }
            else if (tabControlMain.SelectedTab == tabPageRaising)
            {
                raisingControl1.updateRaisingData(speciesIndex);
            }
        }

        private void setCollectionChanged(bool changed, string species = null)
        {
            if (changed)
            {
                if (species == null || (pedigree1.creature != null && pedigree1.creature.species == species))
                    pedigreeNeedsUpdate = true;
                if (species == null || breedingPlan1.CurrentSpecies == species)
                    breedingPlan1.breedingPlanNeedsUpdate = true;
            }

            if (autoSave && changed)
            {
                // save changes automatically
                // backup currentFile if older than 5 min
                if (currentFileName != "" && autoSaveMinutes > 0 && (DateTime.Now - lastAutoSaveBackup).TotalMinutes > autoSaveMinutes)
                {
                    string filenameWOExt = Path.GetFileNameWithoutExtension(currentFileName);
                    File.Copy(currentFileName, Path.GetDirectoryName(currentFileName) + "\\" + filenameWOExt + "_backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xml");
                    lastAutoSaveBackup = DateTime.Now;
                    // delete oldest backupfile if more than a certain number
                    var directory = new System.IO.DirectoryInfo(Path.GetDirectoryName(currentFileName));
                    var oldBackupfiles = directory.GetFiles().Where(f => (f.Name.Length > filenameWOExt.Length + 8 && f.Name.Substring(0, filenameWOExt.Length + 8) == filenameWOExt + "_backup_")).OrderByDescending(f => f.LastWriteTime).Skip(3).ToList();
                    foreach (FileInfo f in oldBackupfiles)
                    {
                        f.Delete();
                    }
                }

                // save changes
                saveCollection();
                return; // function is called soon again from savecollection()
            }
            collectionDirty = changed;
            string fileName = System.IO.Path.GetFileName(currentFileName);
            this.Text = "ARK Smart Breeding" + (fileName.Length > 0 ? " - " + fileName : "") + (changed ? " *" : "");
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
                comboBoxSpeciesGlobal.SelectedIndex = Values.V.speciesNames.IndexOf(c.species);
                NumericUpDownTestingTE.Value = (c.tamingEff >= 0 ? (decimal)c.tamingEff * 100 : 0);
                numericUpDownImprintingBonusTester.Value = (decimal)c.imprintingBonus * 100;
                if (c.isBred)
                    radioButtonTesterBred.Checked = true;
                else if (c.tamingEff > 0)
                    radioButtonTesterTamed.Checked = true;
                else radioButtonTesterWild.Checked = true;

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
                comboBoxSpeciesGlobal.SelectedIndex = Values.V.speciesNames.IndexOf(mother.species);
                radioButtonBred.Checked = true;
                numericUpDownImprintingBonusTester.Value = 0;

                creatureInfoInputExtractor.mother = mother;
                creatureInfoInputExtractor.father = father;
                creatureInfoInputExtractor.CreatureOwner = mother.owner;
                creatureInfoInputExtractor.CreatureTribe = mother.tribe;
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
        }

        private void numericUpDownImprintingBonusTester_ValueChanged(object sender, EventArgs e)
        {
            updateAllTesterValues();
            // calculate number of imprintings
            if (Values.V.species[speciesIndex].breeding != null && Values.V.species[speciesIndex].breeding.maturationTimeAdjusted > 0)
                labelImprintedCount.Text = "(" + Math.Round((double)numericUpDownImprintingBonusTester.Value * Values.V.species[speciesIndex].breeding.maturationTimeAdjusted / (1440000 * creatureCollection.babyCuddleIntervalMultiplier), 2) + "×)";
            else labelImprintedCount.Text = "";
        }

        private void numericUpDownImprintingBonusExtractor_ValueChanged(object sender, EventArgs e)
        {
            // calculate number of imprintings
            if (Values.V.species[speciesIndex].breeding != null && Values.V.species[speciesIndex].breeding.maturationTimeAdjusted > 0)
                labelImprintingCuddleCountExtractor.Text = "(" + Math.Round((double)numericUpDownImprintingBonusExtractor.Value * Values.V.species[speciesIndex].breeding.maturationTimeAdjusted / (1440000 * creatureCollection.babyCuddleIntervalMultiplier)) + "×)";
            else labelImprintingCuddleCountExtractor.Text = "";
        }

        private void setTesterInputsTamed(bool tamed)
        {
            for (int s = 0; s < 8; s++)
                testingIOs[s].postTame = tamed;
            labelNotTamedNoteTesting.Visible = !tamed;
        }

        private void checkBoxQuickWildCheck_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = !checkBoxQuickWildCheck.Checked;
            if (!enabled)
            {
                clearAll();
            }
            toolStripButtonExtract.Enabled = enabled;
            panelWildTamedBred.Enabled = enabled;
            checkBoxJustTamed.Enabled = enabled;
            groupBoxDetailsExtractor.Enabled = enabled;
            numericUpDownLevel.Enabled = enabled;
            button2TamingCalc.Visible = !enabled;
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
                    torporLvl += testingIOs[s].LevelWild;
                }
                testingIOs[7].LevelWild = torporLvl;
            }
            int domLevels = 0;
            for (int s = 0; s < 8; s++)
            {
                domLevels += testingIOs[s].LevelDom;
            }
            labelDomLevelSum.Text = "Dom Levels: " + domLevels.ToString() + "/" + creatureCollection.maxDomLevel;
            if (domLevels > creatureCollection.maxDomLevel)
                labelDomLevelSum.BackColor = Color.LightSalmon;
            else
                labelDomLevelSum.BackColor = Color.Transparent;
            labelTesterTotalLevel.Text = "Total Levels: " + (testingIOs[7].LevelWild + domLevels + 1) + "/" + (testingIOs[7].LevelWild + 1 + creatureCollection.maxDomLevel);
            creatureInfoInputTester.parentListValid = false;

            int[] levelsWild = testingIOs.Select(s => s.LevelWild).ToArray();
            if (!testingIOs[2].Enabled) levelsWild[2] = 0;
            radarChart1.setLevels(levelsWild);
            statPotentials1.setLevels(levelsWild, false);
            //statGraphs1.setGraph(sE, 0, testingIOs[0].LevelWild, testingIOs[0].LevelDom, !radioButtonTesterWild.Checked, (double)NumericUpDownTestingTE.Value / 100, (double)numericUpDownImprintingBonusTester.Value / 100);
        }

        private void testingStatIOsRecalculateValue(StatIO sIo)
        {
            sIo.BreedingValue = Stats.calculateValue(speciesIndex, sIo.statIndex, sIo.LevelWild, 0, true, 1, 0);
            sIo.Input = Stats.calculateValue(speciesIndex, sIo.statIndex, sIo.LevelWild, sIo.LevelDom, (radioButtonTesterTamed.Checked || radioButtonTesterBred.Checked), (radioButtonTesterBred.Checked ? 1 : (double)NumericUpDownTestingTE.Value / 100), (radioButtonTesterBred.Checked ? (double)numericUpDownImprintingBonusTester.Value / 100 : 0));
        }

        private void onlinehelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/cadon/ARKStatsExtractor/wiki/Manual");
        }

        private void listViewLibrary_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteSelectedCreatures();
            }
        }

        private void exportForSpreadsheet()
        {
            if (tabControlMain.SelectedTab == tabPageLibrary)
            {
                if (listViewLibrary.SelectedItems.Count > 0)
                {
                    // header
                    string output = "Species\tName\tGender\tOwner\tHPw\tStw\tOxw\tFow\tWew\tDmw\tSpw\tTow\tHPd\tStd\tOxd\tFod\tWed\tDmd\tSpd\tTod\tHPb\tStb\tOxb\tFob\tWeb\tDmb\tSpb\tTob\tHPc\tStc\tOxc\tFoc\tWec\tDmc\tSpc\tToc\tmother\tfather\tNotes";

                    Creature c = null;
                    foreach (ListViewItem l in listViewLibrary.SelectedItems)
                    {
                        c = (Creature)l.Tag;
                        output += "\n" + c.species + "\t" + c.name + "\t" + c.gender.ToString() + "\t" + c.owner;
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
                            output += "\t" + (c.valuesBreeding[s] * (Utils.precision(s) == 3 ? 100 : 1)) + (Utils.precision(s) == 3 ? "%" : "");
                        }
                        for (int s = 0; s < 8; s++)
                        {
                            output += "\t" + (c.valuesDom[s] * (Utils.precision(s) == 3 ? 100 : 1)) + (Utils.precision(s) == 3 ? "%" : "");
                        }
                        output += "\t" + (c.Mother != null ? c.Mother.name : "") + "\t" + (c.Father != null ? c.Father.name : "") + "\t" + (c.note != null ? c.note.Replace("\r", "").Replace("\n", " ") : "");
                    }
                    Clipboard.SetText(output);
                }
                else
                    MessageBox.Show("No creatures in the library selected to copy to the clipboard", "No Creatures Selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                string output = (ARKml ? Utils.getARKml(c.species, 50, 172, 255) : c.species)
                    + " (Lvl " + (breeding ? c.levelHatched : c.level) + modifierText + (c.gender != Sex.Unknown ? ", " + c.gender.ToString() : "") + "): ";
                for (int s = 0; s < 8; s++)
                {
                    if (c.levelsWild[s] >= 0) // ignore unknown oxygen / speed
                        output += Utils.statName(s, true) + ": " + ((breeding ? c.valuesBreeding[s] : c.valuesDom[s]) * (Utils.precision(s) == 3 ? 100 : 1)) + (Utils.precision(s) == 3 ? "%" : "") +
                            " (" + (ARKml ? Utils.getARKmlFromPercent(c.levelsWild[s].ToString(), (int)(c.levelsWild[s] * (s == 7 ? colorFactor / 7 : colorFactor))) : c.levelsWild[s].ToString()) +
                            (ARKml ? (breeding || s == 7 ? "" : ", " + Utils.getARKmlFromPercent(c.levelsDom[s].ToString(), (int)(c.levelsDom[s] * colorFactor))) : (breeding || s == 7 ? "" : ", " + c.levelsDom[s].ToString())) + "); ";
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
                        species = Values.V.speciesNames[speciesIndex];
                        bred = radioButtonBred.Checked;
                        te = extractor.uniqueTE();
                        imprinting = extractor.imprintingBonus;
                    }
                    else
                    {
                        input = creatureInfoInputTester;
                        species = Values.V.speciesNames[speciesIndex];
                        bred = radioButtonTesterBred.Checked;
                        te = (double)NumericUpDownTestingTE.Value / 100;
                        imprinting = (double)numericUpDownImprintingBonusTester.Value / 100;
                    }

                    Creature creature = new Creature(species, input.CreatureName, input.CreatureOwner, input.CreatureTribe, input.CreatureSex, getCurrentWildLevels(fromExtractor), getCurrentDomLevels(fromExtractor), te, bred, imprinting);
                    creature.recalculateCreatureValues();
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
                    MessageBox.Show("No creatures in the library selected to copy to the clipboard", "No Creatures Selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void forSpreadsheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportForSpreadsheet();
        }

        private void forARKChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportSelectedCreatureToClipboard(true, true);
        }

        private void forARKChatcurrentValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportSelectedCreatureToClipboard(false, true);
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
                MessageBox.Show("No creatures in the library selected to copy to the clipboard", "No Creatures Selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        MessageBox.Show("Couldn't serialize creature-object.\nErrormessage:\n\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                string clpb = sb.ToString();
                if (clpb.Length > 0)
                    Clipboard.SetText(clpb);
            }
        }

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
                        MessageBox.Show("Invalid Data in clipboard. Couldn't paste creature-data\nErrormessage:\n\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                updateParents(new List<Creature> { c });
                editCreatureInTester(c, true);
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

                i.SubItems[9].Text = "-";
                // color for cooldown
                Color forecolor, backcolor;
                cooldownColors(c, out forecolor, out backcolor);
                i.SubItems[9].ForeColor = forecolor;
                i.SubItems[9].BackColor = backcolor;
            }
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

        private void setStatusOfSelected(CreatureStatus s)
        {
            List<Creature> cs = new List<Creature>();
            foreach (ListViewItem i in listViewLibrary.SelectedItems)
                cs.Add((Creature)i.Tag);
            if (cs.Count > 0)
                setStatus(cs, s);
        }

        private void setStatus(List<Creature> cs, CreatureStatus s)
        {
            bool changed = false;
            List<string> species = new List<string>();
            foreach (Creature c in cs)
            {
                if (c.status != s)
                {
                    changed = true;
                    c.status = s;
                    if (species.IndexOf(c.species) == -1)
                        species.Add(c.species);
                }
            }
            if (changed)
            {
                // update list / recalculate topstats
                calculateTopStats(creatureCollection.creatures.Where(c => species.Contains(c.species)).ToList());
                filterLib();
                setCollectionChanged(true, (species.Count == 1 ? species[0] : null));
            }
        }

        private void editAllSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            multiSetterToolStripMenuItem_Click(sender, e);
        }

        private void multiSetterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // shows a dialog to set multiple settings to all selected creatures
            if (listViewLibrary.SelectedIndices.Count > 0)
            {
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

                MultiSetter ms = new MultiSetter(c, appliedSettings, parents);

                if (ms.ShowDialog() == DialogResult.OK)
                {
                    foreach (Creature sc in selectedCreatures)
                    {
                        if (appliedSettings[0])
                            sc.owner = c.owner;
                        if (appliedSettings[1])
                            sc.status = c.status;
                        if (appliedSettings[2])
                            sc.gender = c.gender;
                        if (appliedSettings[3])
                            sc.isBred = c.isBred;
                        if (appliedSettings[4])
                            sc.motherGuid = c.motherGuid;
                        if (appliedSettings[5])
                            sc.fatherGuid = c.fatherGuid;
                        if (appliedSettings[6])
                            sc.note = c.note;
                        if (appliedSettings[7])
                            sc.colors[0] = c.colors[0];
                        if (appliedSettings[8])
                            sc.colors[1] = c.colors[1];
                        if (appliedSettings[9])
                            sc.colors[2] = c.colors[2];
                        if (appliedSettings[10])
                            sc.colors[3] = c.colors[3];
                        if (appliedSettings[11])
                            sc.colors[4] = c.colors[4];
                        if (appliedSettings[12])
                            sc.colors[5] = c.colors[5];
                    }
                    if (appliedSettings[4] || appliedSettings[5])
                        updateParents(selectedCreatures);
                    createOwnerList();
                    setCollectionChanged(true, (!multipleSpecies ? sp : null));
                    recalculateTopStatsIfNeeded();
                    filterLib();
                }
                ms.DisposeToolTips();
                ms.Dispose();
            }
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
                && MessageBox.Show("Selected Creature is currently not marked as \"Available\" and thus cannot be considered for breeding. Do you want to change its status to \"Available\"?", "Selected Creature not Available", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                setStatus(new List<Creature>() { c }, CreatureStatus.Available);
            breedingPlan1.determineBestBreeding(c);
            tabControlMain.SelectedTab = tabPageBreedingPlan;
        }

        private void creatureInfoInputTester_Save2Library_Clicked(CreatureInfoInput sender)
        {
            if (creatureTesterEdit != null)
            {
                bool wildChanged = (Math.Abs(creatureTesterEdit.tamingEff - (double)NumericUpDownTestingTE.Value / 100) > .0005);
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
                if (!wildChanged || MessageBox.Show("The wild levels or the taming-effectiveness were changed. Save values anyway?\nOnly save if the wild levels or taming-effectiveness were extracted wrongly!\nIf you are not sure, don't save. The breeding-values could become invalid.", "Wild levels have been changed", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    bool statusChanged = creatureTesterEdit.status != creatureInfoInputTester.CreatureStatus
                        || creatureTesterEdit.owner != creatureInfoInputTester.CreatureOwner
                        || creatureTesterEdit.mutationCounter != creatureInfoInputTester.MutationCounter;
                    bool parentsChanged = (creatureTesterEdit.Mother != creatureInfoInputTester.mother || creatureTesterEdit.Father != creatureInfoInputTester.father);
                    creatureTesterEdit.levelsWild = getCurrentWildLevels(false);
                    creatureTesterEdit.levelsDom = getCurrentDomLevels(false);
                    creatureTesterEdit.tamingEff = (double)NumericUpDownTestingTE.Value / 100;
                    creatureTesterEdit.isBred = radioButtonTesterBred.Checked;
                    creatureTesterEdit.imprintingBonus = (double)numericUpDownImprintingBonusTester.Value / 100;

                    creatureTesterEdit.name = creatureInfoInputTester.CreatureName;
                    creatureTesterEdit.gender = creatureInfoInputTester.CreatureSex;
                    creatureTesterEdit.owner = creatureInfoInputTester.CreatureOwner;
                    creatureTesterEdit.tribe = creatureInfoInputTester.CreatureTribe;
                    creatureTesterEdit.Mother = creatureInfoInputTester.mother;
                    creatureTesterEdit.Father = creatureInfoInputTester.father;
                    creatureTesterEdit.note = creatureInfoInputTester.CreatureNote;
                    creatureTesterEdit.status = creatureInfoInputTester.CreatureStatus;
                    creatureTesterEdit.cooldownUntil = creatureInfoInputTester.Cooldown;
                    creatureTesterEdit.growingUntil = creatureInfoInputTester.Grown;
                    creatureTesterEdit.domesticatedAt = creatureInfoInputTester.domesticatedAt;
                    creatureTesterEdit.neutered = creatureInfoInputTester.Neutered;
                    creatureTesterEdit.mutationCounter = creatureInfoInputTester.MutationCounter;

                    if (wildChanged)
                        calculateTopStats(creatureCollection.creatures.Where(c => c.species == creatureTesterEdit.species).ToList());
                    updateCreatureValues(creatureTesterEdit, statusChanged);

                    if (parentsChanged)
                        creatureTesterEdit.recalculateAncestorGenerations();

                    setTesterInfoInputCreature();
                    tabControlMain.SelectedTab = tabPageLibrary;
                }
            }
        }

        private void setTesterInfoInputCreature(Creature c = null, bool virtualCreature = false)
        {
            bool enable = (c != null); // set to a creature, or clear
            creatureInfoInputTester.ShowSaveButton = enable && !virtualCreature;
            labelCurrentTesterCreature.Text = (enable ? "Current Creature: " + c.name : "");
            if (enable)
            {
                creatureInfoInputTester.mother = c.Mother;
                creatureInfoInputTester.father = c.Father;
                creatureInfoInputTester.CreatureName = c.name;
                creatureInfoInputTester.CreatureSex = c.gender;
                creatureInfoInputTester.CreatureOwner = c.owner;
                creatureInfoInputTester.CreatureTribe = c.tribe;
                creatureInfoInputTester.CreatureStatus = c.status;
                creatureInfoInputTester.CreatureNote = c.note;
                creatureInfoInputTester.Cooldown = c.cooldownUntil;
                creatureInfoInputTester.Grown = c.growingUntil;
                creatureInfoInputTester.domesticatedAt = c.domesticatedAt.Year < 2000 ? DateTime.Now : c.domesticatedAt;
                creatureInfoInputTester.Neutered = c.neutered;
                updateParentListInput(creatureInfoInputTester);
                creatureInfoInputTester.MutationCounter = c.mutationCounter;
            }
            else
            {
                creatureInfoInputTester.mother = null;
                creatureInfoInputTester.father = null;
                creatureInfoInputTester.CreatureName = "";
                creatureInfoInputTester.CreatureSex = Sex.Unknown;
                creatureInfoInputTester.CreatureStatus = CreatureStatus.Available;
                creatureInfoInputTester.Cooldown = DateTime.Now.AddHours(-1);
                creatureInfoInputTester.Grown = DateTime.Now.AddHours(-1);
                creatureInfoInputTester.domesticatedAt = DateTime.Now;
                creatureInfoInputTester.Neutered = false;
                creatureInfoInputTester.MutationCounter = 0;
                creatureInfoInputTester.parentListValid = false;
            }
            creatureTesterEdit = c;
        }

        private void toolStripButtonSettings_Click(object sender, EventArgs e)
        {
            settingsToolStripMenuItem_Click(sender, e);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings.Settings settingsfrm = new settings.Settings(creatureCollection);
            if (settingsfrm.ShowDialog() == DialogResult.OK)
            {
                applySettingsToValues();
                autoSave = Properties.Settings.Default.autosave;
                autoSaveMinutes = Properties.Settings.Default.autosaveMinutes;
                creatureBoxListView.maxDomLevel = creatureCollection.maxDomLevel;
                fileSync.changeFile(currentFileName); // only to trigger the update, filename is not changed

                setCollectionChanged(true);
            }
            settingsfrm.DisposeToolTips();
        }

        /// <summary>
        /// Display the wild-levels, assuming it's a wild creature. Used for quick checking
        /// </summary>
        /// <param name="sIO"></param>
        private void statIOQuickWildLevelCheck(StatIO sIO)
        {
            if (checkBoxQuickWildCheck.Checked)
            {
                int lvlWild = (int)Math.Round((sIO.Input - Values.V.species[speciesIndex].stats[sIO.statIndex].BaseValue) / (Values.V.species[speciesIndex].stats[sIO.statIndex].BaseValue * Values.V.species[speciesIndex].stats[sIO.statIndex].IncPerWildLevel));
                sIO.LevelWild = (lvlWild < 0 ? 0 : lvlWild);
                sIO.LevelDom = 0;
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
                    // copy values over to extractor
                    for (int s = 0; s < 8; s++)
                        statIOs[s].Input = (onlyWild ? Stats.calculateValue(sI, s, c.levelsWild[s], 0, true, c.tamingEff, c.imprintingBonus) : c.valuesDom[s]);
                    comboBoxSpeciesGlobal.SelectedIndex = sI;

                    if (c.isBred) radioButtonBred.Checked = true;
                    else if (c.tamingEff >= 0) radioButtonTamed.Checked = true;
                    else radioButtonWild.Checked = true;

                    numericUpDownImprintingBonusExtractor.Value = (decimal)c.imprintingBonus * 100;
                    // set total level
                    int level = (onlyWild ? c.levelsWild[7] : c.level);
                    if (level >= 0 && level <= numericUpDownLevel.Maximum)
                        numericUpDownLevel.Value = level;
                    else numericUpDownLevel.Value = 0;

                    tabControlMain.SelectedTab = tabPageExtractor;
                }
                else
                    MessageBox.Show("Unknown Species. Try to update the species-stats, or redownload the tool.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void testEnteredDrag(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void doOCRofDroppedImage(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
                doOCR(files[0], true);
        }

        public void doOCR(string imageFilePath = "", bool manuallyTriggered = true)
        {
            checkBoxQuickWildCheck.Checked = false;

            string debugText;
            string dinoName, ownerName, tribeName, species;
            Sex sex;
            float[] OCRvalues = ArkOCR.OCR.doOCR(out debugText, out dinoName, out species, out ownerName, out tribeName, out sex, imageFilePath, manuallyTriggered);

            ocrControl1.output.Text = debugText;
            if (OCRvalues.Length <= 1)
                return;
            if ((decimal)OCRvalues[9] <= numericUpDownLevel.Maximum)
                numericUpDownLevel.Value = (decimal)OCRvalues[9];

            creatureInfoInputExtractor.CreatureName = dinoName;
            creatureInfoInputExtractor.CreatureOwner = ownerName;
            creatureInfoInputExtractor.CreatureTribe = tribeName;
            creatureInfoInputExtractor.CreatureSex = sex;

            for (int i = 0; i < 8; i++)
            {
                if (statIOs[i].percent)
                    statIOs[i].Input = OCRvalues[i] / 100.0;
                else
                    statIOs[i].Input = OCRvalues[i];
            }

            // use imprinting if existing
            if (OCRvalues.Length > 8 && OCRvalues[8] > 0 && OCRvalues[8] <= 100)
            {
                radioButtonBred.Checked = true;
                numericUpDownImprintingBonusExtractor.Value = (decimal)OCRvalues[8];
            }
            else { radioButtonTamed.Checked = true; }

            if (species == "Paraceratherium") //todo do more species
                species = "Paracer";

            List<int> possibleDinos = determineSpeciesFromStats(OCRvalues, species);

            if (possibleDinos.Count == 1)
            {
                if (possibleDinos[0] >= 0 && possibleDinos[0] < comboBoxSpeciesGlobal.Items.Count)
                    comboBoxSpeciesGlobal.SelectedIndex = possibleDinos[0];
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
                    comboBoxSpeciesGlobal.SelectedIndex = possibleDinos[newindex];
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
                        comboBoxSpeciesGlobal.SelectedIndex = possibleDinos[dinooption];
                        lastOCRSpecies = possibleDinos[dinooption];
                        lastOCRValues = OCRvalues;
                        foundPossiblyGood = extractLevels();
                    }
                }
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

        private List<int> determineSpeciesFromStats(float[] stats, string species)
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

            if (stats.Length > 8 && stats[8] > 0)
            {
                // creature is imprinted, the following algorithm cannot handle this yet. use current selected species
                possibleDinos.Add(comboBoxSpeciesGlobal.SelectedIndex);
                return possibleDinos;
            }

            double baseValue;
            double incWild;
            double possibleLevel;
            bool possible;

            for (int i = 0; i < Values.V.species.Count; i++)
            {
                if (i == comboBoxSpeciesGlobal.SelectedIndex) continue; // the currently selected species is ignored here and set as top priority at the end

                possible = true;
                // check that all stats are possible (no negative levels)
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

                possibleLevel = ((statIOs[7].Input - Values.V.species[i].stats[7].AddWhenTamed) - baseValue) / (baseValue * incWild);
                double possibleLevelWild = (statIOs[7].Input - baseValue) / (baseValue * incWild);

                if (possibleLevelWild < 0 || Math.Round(possibleLevel, 3) > (double)numericUpDownLevel.Value - 1 || (Math.Round(possibleLevel, 3) % 1 > 0.001 && Math.Round(possibleLevelWild, 3) % 1 > 0.001))
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
                possibleLevel = ((statIOs[4].Input - Values.V.species[i].stats[4].AddWhenTamed) - baseValue) / (baseValue * incWild);

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

            if (comboBoxSpeciesGlobal.SelectedIndex >= 0)
                possibleDinos.Insert(0, comboBoxSpeciesGlobal.SelectedIndex); // adding the currently selected creature in the combobox as first priority. the user might already have that selected
            return possibleDinos;
        }

        private void chkbToggleOverlay_CheckedChanged(object sender, EventArgs e)
        {
            if (overlay == null)
            {
                overlay = new ARKOverlay();
                overlay.ExtractorForm = this;
                overlay.InfoDuration = Properties.Settings.Default.OverlayInfoDuration;
                overlay.initLabelPositions();
            }

            overlay.Visible = chkbToggleOverlay.Checked;
            overlay.inventoryCheckTimer.Enabled = overlay.Visible;

            if (speechRecognition != null)
                speechRecognition.Listen = chkbToggleOverlay.Checked;
        }

        private void toolStripButtonCopy2Tester_Click_1(object sender, EventArgs e)
        {
            double te = extractor.uniqueTE();
            NumericUpDownTestingTE.Value = (decimal)(te >= 0 ? te * 100 : 80);
            numericUpDownImprintingBonusTester.Value = numericUpDownImprintingBonusExtractor.Value;
            if (radioButtonBred.Checked)
                radioButtonTesterBred.Checked = true;
            else if (radioButtonTamed.Checked)
                radioButtonTesterTamed.Checked = true;
            else
                radioButtonTesterWild.Checked = true;
            for (int s = 0; s < 8; s++)
            {
                testingIOs[s].LevelWild = statIOs[s].LevelWild;
                testingIOs[s].LevelDom = statIOs[s].LevelDom;
                testingStatIOValueUpdate(testingIOs[s]);
            }
            tabControlMain.SelectedTab = tabPageStatTesting;
            setTesterInfoInputCreature();
        }

        private void toolStripButtonClear_Click_1(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageExtractor)
            {
                clearAll();
                numericUpDownLevel.Value = 1;
            }
            else if (tabControlMain.SelectedTab == tabPageStatTesting)
            {
                for (int s = 0; s < 8; s++)
                {
                    testingIOs[s].LevelDom = 0;
                    testingIOs[s].LevelWild = 0;
                }
            }
        }

        private void toolStripButtonCopy2Extractor_Click(object sender, EventArgs e)
        {
            // copy values from tester over to extractor
            for (int s = 0; s < 8; s++)
                statIOs[s].Input = testingIOs[s].Input;
            if (radioButtonTesterBred.Checked)
                radioButtonBred.Checked = true;
            else if (radioButtonTesterTamed.Checked)
                radioButtonTamed.Checked = true;
            else
                radioButtonWild.Checked = true;
            numericUpDownImprintingBonusExtractor.Value = numericUpDownImprintingBonusTester.Value;
            // set total level
            numericUpDownLevel.Value = getCurrentWildLevels(false).Sum() - testingIOs[7].LevelWild + getCurrentDomLevels(false).Sum() + 1;
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

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/cadon/ARKStatsExtractor/wiki/Extraction-issues");
        }

        private void showStatsInOverlay()
        {
            if (overlay != null)
            {
                float[] wildLevels = new float[10];
                float[] tamedLevels = new float[10];
                Color[] colors = new Color[8];

                for (int i = 0; i < 8; i++)
                {
                    wildLevels[i] = statIOs[i].LevelWild;
                    tamedLevels[i] = statIOs[i].LevelDom;
                    colors[i] = statIOs[i].BackColor;

                    if (i < 7)
                    {
                        wildLevels[9] += statIOs[i].LevelWild;
                        tamedLevels[9] += statIOs[i].LevelDom;
                    }
                }
                wildLevels[9]++; // startlevel

                string extraText = Values.V.speciesNames[speciesIndex];
                if (!extractor.postTamed)
                {
                    string foodName = Values.V.species[speciesIndex].taming.eats[0];
                    double tamingSpeedMultiplier = cbEventMultipliers.Checked ? creatureCollection.tamingSpeedMultiplierEvent : creatureCollection.tamingSpeedMultiplier;
                    double tamingFoodRateMultiplier = cbEventMultipliers.Checked ? creatureCollection.tamingFoodRateMultiplierEvent : creatureCollection.tamingFoodRateMultiplier;
                    int foodNeeded = Taming.foodAmountNeeded(speciesIndex, (int)wildLevels[9], tamingSpeedMultiplier, foodName, Values.V.species[speciesIndex].taming.nonViolent);
                    List<int> foodAmountUsed;
                    bool enoughFood;
                    double te, hunger;
                    TimeSpan duration;
                    int narcotics, narcoBerries, bioToxines, bonusLevel;
                    Taming.tamingTimes(speciesIndex, (int)wildLevels[9], tamingSpeedMultiplier, tamingFoodRateMultiplier, foodName, foodNeeded, out foodAmountUsed, out duration, out narcoBerries, out narcotics, out bioToxines, out te, out hunger, out bonusLevel, out enoughFood);
                    string foodNameDisplay = (foodName == "Kibble" ? Values.V.species[speciesIndex].taming.favoriteKibble + " Egg Kibble" : foodName);
                    extraText += "\nTaming takes " + duration.ToString(@"hh\:mm\:ss") + " with " + foodNeeded + "×" + foodNameDisplay
                        + "\n" + narcoBerries + " Narcoberries or " + narcotics + " Narcotics or " + bioToxines + " Bio Toxines are needed"
                        + "\nTaming Effectiveness: " + Math.Round(100 * te, 1).ToString() + " % (+" + bonusLevel.ToString() + " lvl)";
                }

                overlay.setStatLevels(wildLevels, tamedLevels, colors);
                overlay.setExtraText(extraText);

                // currently disabled, as current weight is not shown. TODO remove if there's no way to tell maturating-progress
                //if (Values.V.species[speciesIndex].breeding != null && lastOCRValues != null && lastOCRValues.Length > 10 && lastOCRValues[10] > 0)
                //{
                //    int maxTime = (int)Values.V.species[speciesIndex].breeding.maturationTimeAdjusted;
                //    if (maxTime > 0 && lastOCRValues[4] > 0)
                //        overlay.setBreedingProgressValues((float)Math.Round(lastOCRValues[10] / lastOCRValues[4], 1), maxTime); // current weight
                //    else
                //        overlay.setBreedingProgressValues(1, 0); // 100% breeding time shows nothing
                //}
            }
        }

        private void findDuplicatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<int> dups1 = new List<int>();
            List<int> dups2 = new List<int>();
            bool notEqual = false;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = creatureCollection.creatures.Count;
            toolStripProgressBar1.Visible = true;
            for (int i = 0; i < creatureCollection.creatures.Count; i++)
            {
                for (int j = i + 1; j < creatureCollection.creatures.Count; j++)
                {
                    if (creatureCollection.creatures[i].species != creatureCollection.creatures[j].species)
                        continue;
                    notEqual = false;
                    for (int s = 0; s < 8; s++)
                    {
                        if (creatureCollection.creatures[i].levelsWild[s] != creatureCollection.creatures[j].levelsWild[s])
                        {
                            notEqual = true;
                            break;
                        }
                    }
                    if (!notEqual)
                    {
                        dups1.Add(i);
                        dups2.Add(j);
                    }
                }
                toolStripProgressBar1.Value++;
            }
            toolStripProgressBar1.Visible = false;
            if (dups1.Count == 0)
            {
                MessageBox.Show("No possible duplicates found", "No Duplicates", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show(dups1.Count.ToString() + " possible duplicates found. Show them?\nThis function is currently under development and does currently not more than showing a messagebox for each possible duplicate.", "Duplicates found", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                for (int i = 0; i < dups1.Count; i++)
                    MessageBox.Show("Possible duplicate found (all wild levels are equal, the creatures also could be siblings).\n" + creatureCollection.creatures[dups1[i]].species + "\n\"" + creatureCollection.creatures[dups1[i]].name + "\" and \"" + creatureCollection.creatures[dups2[i]].name + "\"", "Possible duplicate found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnReadValuesFromArk_Click(object sender, EventArgs e)
        {
            doOCR("", true);
        }

        private void toolStripButtonAddTribe_Click(object sender, EventArgs e)
        {
            tribesControl1.addTribe();
        }

        private void button2TamingCalc_Click(object sender, EventArgs e)
        {
            tamingControl1.setSpeciesIndex(comboBoxSpeciesGlobal.SelectedIndex);
            if (checkBoxQuickWildCheck.Checked)
                tamingControl1.level = statIOs[7].LevelWild + 1;
            else
                tamingControl1.level = (int)numericUpDownLevel.Value;
            tabControlMain.SelectedTab = tabPageTaming;
        }

        private void labelImprintedCount_Click(object sender, EventArgs e)
        {
            // set imprinting-count to closes integer
            if (Values.V.species[speciesIndex].breeding != null && Values.V.species[speciesIndex].breeding.maturationTimeAdjusted > 0)
            {
                int cuddleCount = (int)Math.Round((double)numericUpDownImprintingBonusTester.Value * Values.V.species[speciesIndex].breeding.maturationTimeAdjusted / (1440000 * creatureCollection.babyCuddleIntervalMultiplier));
                double imprintingBonus;
                do
                {
                    imprintingBonus = Math.Round(cuddleCount * 1440000 * creatureCollection.babyCuddleIntervalMultiplier / Values.V.species[speciesIndex].breeding.maturationTimeAdjusted, 3);
                    cuddleCount--;
                } while (imprintingBonus > 100);
                numericUpDownImprintingBonusTester.Value = (decimal)imprintingBonus;
            }
        }

        private bool loadAdditionalValues(string file, bool showResult = false)
        {
            if (Values.V.loadAdditionalValues(file, showResult))
            {
                applySettingsToValues();
                updateSpeciesComboboxes();
                creatureCollection.additionalValues = Path.GetFileName(file);
                updateStatusBar();
                return true;
            }
            return false;
        }

        private void loadAdditionalValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Additional values-file (*.json)|*.json";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (loadAdditionalValues(dlg.FileName, true))
                    setCollectionChanged(true);
            }
        }

        private void toolStripButtonAddPlayer_Click(object sender, EventArgs e)
        {
            tribesControl1.addPlayer();
        }

        private void updateStatusBar()
        {
            int total = creatureCollection.creatures.Count();
            toolStripStatusLabel.Text = total + " creatures in Library"
                + (total > 0 ? " (alive: " + creatureCollection.creatures.Count(c => c.status == CreatureStatus.Alive).ToString()
                + ", dead: " + creatureCollection.creatures.Count(c => c.status == CreatureStatus.Dead).ToString()
                + ", available: " + creatureCollection.creatures.Count(c => c.status == CreatureStatus.Available).ToString()
                + ", unavailable: " + creatureCollection.creatures.Count(c => c.status == CreatureStatus.Unavailable).ToString()
                + ")" : "")
                + ". v" + Application.ProductVersion + " / values: " + Values.V.version.ToString() +
                   (creatureCollection.additionalValues.Length > 0 ? ", additional values from " + creatureCollection.additionalValues + " v" + Values.V.modVersion : "");
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
            if (speechRecognition != null)
                speechRecognition.toggleListening();
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

        private void Timer_Tick(object sender, EventArgs e)
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

            tamingControl1.setTamingMultipliers(cbEventMultipliers.Checked ? creatureCollection.tamingSpeedMultiplierEvent : creatureCollection.tamingSpeedMultiplier,
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
            ArkOCR.OCR.whiteThreshold = value;
        }

        /// <summary>
        /// fixes typos saved in earlier versions. is called right after loading a library
        /// </summary>
        /// <param name="cc">CreatureCollection to be checked on typos</param>
        private void nameFixes(CreatureCollection cc)
        {
            foreach (Creature c in cc.creatures)
            {
                if (c.species == "Wooly Rhino")
                    c.species = "Woolly Rhino";
            }
        }
    }
}
