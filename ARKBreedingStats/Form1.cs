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

namespace ARKBreedingStats
{
    public partial class Form1 : Form
    {
        private CreatureCollection creatureCollection = new CreatureCollection();
        private String currentFileName = "";
        private bool collectionDirty = false;
        private ListViewColumnSorter lvwColumnSorterLibrary; // used for sorting columns in the listviewLibrary
        private ListViewColumnSorter lvwColumnSorterPossibilities; // used for sorting columns in the listviewPossibility
        private Dictionary<string, Int32[]> topStats = new Dictionary<string, Int32[]>(); // list of top stats of all creatures per species
        private List<StatIO> statIOs = new List<StatIO>();
        private List<StatIO> testingIOs = new List<StatIO>();
        private int sE = 0; // current species for extractor
        private int activeStat = -1;
        private bool[] activeStats = new bool[] { true, true, true, true, true, true, true, true }; // stats used by the creature (some don't use oxygen)
        private bool pedigreeNeedsUpdate = false;
        private bool breedingPlanNeedsUpdate = false;
        public delegate void LevelChangedEventHandler(StatIO s);
        public delegate void InputValueChangedEventHandler(StatIO s);
        private bool updateTorporInTester, filterListAllowed;
        private bool[] considerStatHighlight = new bool[] { true, true, false, false, true, true, false, false }; // consider this stat for color-highlighting, topness etc
        private bool autoSave;
        private DateTime lastAutoSaveBackup = DateTime.Now.AddDays(-1);
        private int autoSaveMinutes;
        private Creature creatureTesterEdit;
        private FileSync fileSync;

        // OCR stuff
        public ARKOverlay overlay;
        private static float[] lastOCRValues;
        private int lastOCRSpecies;

        public Form1()
        {
            InitializeComponent();

            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            lvwColumnSorterLibrary = new ListViewColumnSorter();
            this.listViewLibrary.ListViewItemSorter = lvwColumnSorterLibrary;

            lvwColumnSorterPossibilities = new ListViewColumnSorter();
            listViewPossibilities.ListViewItemSorter = lvwColumnSorterPossibilities;

            toolStripStatusLabel.Text = Application.ProductVersion;

            pedigree1.EditCreature += new Pedigree.EditCreatureEventHandler(editCreatureInTester);
            pedigree1.BestBreedingPartners += new PedigreeCreature.CreaturePartnerEventHandler(showBestBreedingPartner);
            breedingPlan1.EditCreature += new BreedingPlan.EditCreatureEventHandler(editCreatureInTester); breedingPlan1.CreateTimer += new BreedingPlan.CreateTimerEventHandler(createTimer);
            breedingPlan1.BPRecalc += new BreedingPlan.BPRecalcEventHandler(recalculateBreedingPlan);
            breedingPlan1.BestBreedingPartners += new PedigreeCreature.CreaturePartnerEventHandler(showBestBreedingPartner);
            timerList1.onTimerChange += new TimerList.timerChanged(setCollectionChanged);

            ArkOCR.OCR.setDebugPanel(OCRDebugLayoutPanel);

            settingsToolStripMenuItem.ShortcutKeyDisplayString = ((new KeysConverter()).ConvertTo(Keys.Control, typeof(string))).ToString().Replace("None", ",");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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
            if (cw != null && cw.Length > 0)
            {
                for (int c = 0; c < cw.Length; c++)
                    listViewLibrary.Columns[c].Width = cw[c];
            }

            // load listviewLibSorting
            lvwColumnSorterLibrary.SortColumn = Properties.Settings.Default.listViewSortCol;
            lvwColumnSorterLibrary.Order = (Properties.Settings.Default.listViewSortAsc ? SortOrder.Ascending : SortOrder.Descending);

            // load statweights
            double[][] custWd = Properties.Settings.Default.customStatWeights;
            string[] custWs = Properties.Settings.Default.customStatWeightNames;
            Dictionary<string, double[]> custW = new Dictionary<string, double[]>();
            if (custWs != null && custWs.Length > 0)
            {
                for (int i = 0; i < custWs.Length; i++)
                {
                    if (i < custWd.Length)
                    {
                        custW.Add(custWs[i], custWd[i]);
                    }
                }
            }
            statWeighting1.CustomWeightings = custW;
            // last set values are saved at the end of the customweightings
            if (custWs != null && custWd != null && custWd.Length > custWs.Length)
                statWeighting1.Values = custWd[custWs.Length];

            autoSave = Properties.Settings.Default.autosave;
            autoSaveMinutes = Properties.Settings.Default.autosaveMinutes;

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
                testingIOs[s].LevelChanged += new LevelChangedEventHandler(this.statIOUpdateValue);
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

            assignCollectionClasses();
            filterListAllowed = true;

            // ToolTips
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.checkBoxJustTamed, "Check this if there was no server-restart or if you didn't logout since you tamed the creature.\nUncheck this if you know there was a server-restart (many servers restart every night).\nIf it is some days ago (IRL) you tamed the creature you should probably uncheck this checkbox.\nThe reason for this is a bug in the game, that displays a too high Torpor-value after a creature is tamed.");
            tt.SetToolTip(checkBoxWildTamedAuto, "For most creatures the tool recognizes if they are wild or tamed.\nFor Giganotosaurus and maybe if you have custom server-settings you have to select manually if the creature is wild or tamed.");
            tt.SetToolTip(checkBoxQuickWildCheck, "Check this if you just want a quick check of the levels of a wild (untamed) creature.\nThe levels are then shown without the extraction-process (and without validation).");
            tt.SetToolTip(radioButtonBPTopStatsCn, "Top Stats, Conservative.\nCheck for best long-term-results and if you want to go safe.\nThis mode will get to the best possible offspring steady and surely.\nSome offsprings might be worse than in High-Stats-Mode, but that's the mode you go if you want to have that perfect creature in some generations.");
            tt.SetToolTip(radioButtonBPTopStats, "Top Stats, Feeling Lucky.\nCheck for best long-term-results and if you're feeling lucky. It can be faster to get the perfect creature than in the Top-Stat-Conservative-Mode if you're lucky.\nSome offsprings might be worse than in High-Stats-Mode, but you also have a chance to the best possible offspring.");
            tt.SetToolTip(radioButtonBPHighStats, "Check for best next-generation-results.\nThe chance for an overall good creature is better.\nCheck if it's not important to have a Top-Stats-Offspring.");
            tt.SetToolTip(labelSum, "Sum of the levels that can be extracted (i.e. speed and sometimes oxygen is not included)");
            tt.SetToolTip(labelImprintedCount, "Number of cuddles given to get to this Imprinting-Bonus.\nClick to set to the closest valid integer.");
            tt.SetToolTip(labelImprintingCuddleCountExtractor, "Number of cuddles given to get to this Imprinting-Bonus.");

            creatureInfoInputExtractor.weightStat = statIOs[4];
            creatureInfoInputTester.weightStat = testingIOs[4];

            // Set up the file watcher
            fileSync = new FileSync(currentFileName, collectionChanged);

            if (Values.V.loadValues() && Values.V.speciesNames.Count > 0)
            {
                // load last save file:
                if (Properties.Settings.Default.LastSaveFile != "")
                    loadCollectionFile(Properties.Settings.Default.LastSaveFile);
                else
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

            clearAll();
            // UI loaded

            tamingControl1.selectedSpeciesIndex = -1;
            tamingControl1.selectedSpeciesIndex = 0;

            // check for updates
            DateTime lastUpdateCheck = Properties.Settings.Default.lastUpdateCheck;
            if (DateTime.Now.AddDays(-3) > lastUpdateCheck)
                checkForUpdates(true);

            //// TODO: debug-numbers
            //statIOs[0].Input = 3263.2;
            //statIOs[1].Input = 2625;
            //statIOs[2].Input = 525;
            //statIOs[3].Input = 5811;
            //statIOs[4].Input = 625.8;
            //statIOs[5].Input = 3.171;
            //statIOs[6].Input = 1.118;
            //statIOs[7].Input = 8234.3;
            //comboBoxSpeciesExtractor.SelectedIndex = Values.V.speciesNames.IndexOf("Argentavis");
            //numericUpDownLevel.Value = 189;
            //checkBoxAlreadyBred.Checked = true;
            //numericUpDownImprintingBonusExtractor.Value = 59;
            //tabControlMain.SelectedTab = tabPageExtractor;

            if (!Properties.Settings.Default.OCR)
            {
                tabControlMain.TabPages.Remove(TabPageOCR);
                btnReadValuesFromArk.Visible = false;
            }
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
            Extraction.E.Clear();
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
            this.checkBoxAlreadyBred.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxJustTamed.BackColor = System.Drawing.Color.Transparent;
            panelSums.BackColor = System.Drawing.Color.Transparent;
            panelWildTamedAuto.BackColor = System.Drawing.Color.Transparent;
            labelTE.BackColor = System.Drawing.Color.Transparent;
            labelTE.Text = "";
            creatureInfoInputExtractor.ButtonEnabled = false;
            activeStat = -1;
            labelSumDom.Text = "";
            labelSumWild.Text = "";
            labelSumWildSB.Text = "";
            for (int i = 0; i < 2; i++)
            {
                Extraction.E.levelWildFromTorporRange[i] = 0;
                Extraction.E.levelDomFromTorporAndTotalRange[i] = 0;
            }
            labelSumDomSB.Text = "";
            updateTorporInTester = true;
            buttonHelp.Visible = false;
            labelErrorHelp.Visible = false;
            labelImprintingFailInfo.Visible = false;
            groupBoxPossibilities.Visible = false;
            labelDoc.Visible = false;
            button2TamingCalc.Visible = checkBoxQuickWildCheck.Checked;
            groupBoxTamingInfo.Visible = false;
            creatureInfoInputExtractor.domesticatedAt = DateTime.Now;
        }

        private void toolStripButtonExtract_Click(object sender, EventArgs e)
        {
            extractLevels();
        }

        private void buttonExtract_Click(object sender, EventArgs e)
        {
            extractLevels();
        }

        private bool extractLevels()
        {
            SuspendLayout();
            int activeStatKeeper = activeStat;
            clearAll();

            Extraction.E.extractLevels(sE, (int)numericUpDownLevel.Value, statIOs,
                (double)numericUpDownLowerTEffBound.Value / 100, (double)numericUpDownUpperTEffBound.Value / 100,
                checkBoxWildTamedAuto.Checked, radioButtonTamed.Checked, checkBoxJustTamed.Checked, checkBoxAlreadyBred.Checked,
                (double)numericUpDownImprintingBonusExtractor.Value / 100, creatureCollection.imprintingMultiplier, creatureCollection.babyCuddleIntervalMultiplier);

            if (!checkBoxAlreadyBred.Checked)
                checkBoxJustTamed.Checked = Extraction.E.justTamed;
            numericUpDownImprintingBonusExtractor.Value = (decimal)Extraction.E.imprintingBonus * 100;

            // remove all results that require a total wild-level higher than the max
            if (!checkBoxAlreadyBred.Checked
                && creatureCollection.maxWildLevel > 0
                && Extraction.E.levelWildFromTorporRange[0] > creatureCollection.maxWildLevel
                )
            {
                double minTECheck = 2d * (Extraction.E.levelWildFromTorporRange[0] - creatureCollection.maxWildLevel) / creatureCollection.maxWildLevel;

                if (minTECheck < 1)
                {
                    // if min TE is equal or greater than 1, that indicates it can't possibly be anything but bred, and there cannot be any results that should be sorted out

                    for (int s = 0; s < 8; s++)
                    {
                        if (Extraction.E.results[s].Count == 0 || Extraction.E.results[s][0].TE < 0)
                            continue;
                        for (int r = 0; r < Extraction.E.results[s].Count; r++)
                        {
                            if (Extraction.E.results[s][r].TE < minTECheck)
                                Extraction.E.results[s].RemoveAt(r--);
                        }
                    }
                }
            }

            if (!Extraction.E.setStatLevelBounds())
            {
                this.numericUpDownLevel.BackColor = Color.LightSalmon;
                if (!checkBoxAlreadyBred.Checked && this.numericUpDownLowerTEffBound.Value > 0)
                    this.numericUpDownLowerTEffBound.BackColor = Color.LightSalmon;
                if (!checkBoxAlreadyBred.Checked && this.numericUpDownUpperTEffBound.Value < 100)
                    this.numericUpDownUpperTEffBound.BackColor = Color.LightSalmon;
                this.checkBoxAlreadyBred.BackColor = Color.LightSalmon;
                this.checkBoxJustTamed.BackColor = Color.LightSalmon;
                panelWildTamedAuto.BackColor = Color.LightSalmon;
                extractionFailed();
                return false;
            }

            int removeOOBResult = Extraction.E.removeOutOfBoundsResults();
            if (removeOOBResult >= 0)
            {
                this.numericUpDownLevel.BackColor = Color.LightSalmon;
                statIOs[removeOOBResult].Status = StatIOStatus.Error;
                statIOs[7].Status = StatIOStatus.Error;
            }

            // get mean-level (most probable for the wild levels)
            double meanWildLevel = Math.Round((double)Extraction.E.levelWildFromTorporRange[1] / 7, 1);
            bool nonUniqueStats = false;

            for (int s = 0; s < 8; s++)
            {
                if (Extraction.E.results[s].Count > 0)
                {
                    // choose the most probable wild-level, aka the level nearest to the mean of the wild levels.
                    int r = 0;
                    for (int b = 1; b < Extraction.E.results[s].Count; b++)
                    {
                        if (Math.Abs(meanWildLevel - Extraction.E.results[s][b].levelWild) < Math.Abs(meanWildLevel - Extraction.E.results[s][r].levelWild)) r = b;
                    }

                    setPossibility(s, r);
                    if (Extraction.E.results[s].Count > 1)
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
                    Extraction.E.validResults = false;
                    if (!checkBoxAlreadyBred.Checked && Extraction.E.statsWithEff.IndexOf(s) >= 0 && this.numericUpDownLowerTEffBound.Value > 0)
                    {
                        this.numericUpDownLowerTEffBound.BackColor = Color.LightSalmon;
                    }
                    if (!checkBoxAlreadyBred.Checked && Extraction.E.statsWithEff.IndexOf(s) >= 0 && this.numericUpDownUpperTEffBound.Value < 100)
                    {
                        this.numericUpDownUpperTEffBound.BackColor = Color.LightSalmon;
                    }
                    this.checkBoxAlreadyBred.BackColor = Color.LightSalmon;
                    this.checkBoxJustTamed.BackColor = Color.LightSalmon;
                    panelWildTamedAuto.BackColor = Color.LightSalmon;
                }
            }
            if (!Extraction.E.validResults)
            {
                extractionFailed();
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
                domLevelsChosenSum += Extraction.E.results[s][Extraction.E.chosenResults[s]].levelDom;
            }
            if (domLevelsChosenSum < Extraction.E.levelDomFromTorporAndTotalRange[0] || domLevelsChosenSum > Extraction.E.levelDomFromTorporAndTotalRange[1])
            {
                // sum of domlevels is not correct. Try to find another combination
                domLevelsChosenSum -= Extraction.E.results[5][Extraction.E.chosenResults[5]].levelDom;
                bool changeChosenResult = false;
                int cR = 0;
                for (int r = 0; r < Extraction.E.results[5].Count; r++)
                {
                    if (domLevelsChosenSum + Extraction.E.results[5][r].levelDom >= Extraction.E.levelDomFromTorporAndTotalRange[0] && domLevelsChosenSum + Extraction.E.results[5][r].levelDom <= Extraction.E.levelDomFromTorporAndTotalRange[1])
                    {
                        cR = r;
                        changeChosenResult = true;
                        break;
                    }
                }
                if (changeChosenResult)
                    setPossibility(5, cR);
            }

            if (Extraction.E.postTamed) { setUniqueTE(); }
            else
            {
                labelTE.Text = "not yet tamed";
                labelTE.BackColor = System.Drawing.Color.Transparent;
            }

            setWildSpeedLevelAccordingToOthers();

            labelSumWildSB.Text = "≤" + Extraction.E.levelWildFromTorporRange[1].ToString();
            labelSumDomSB.Text = (Extraction.E.levelDomFromTorporAndTotalRange[0] != Extraction.E.levelDomFromTorporAndTotalRange[1] ? Extraction.E.levelDomFromTorporAndTotalRange[0].ToString() + "-" : "") + Extraction.E.levelDomFromTorporAndTotalRange[1].ToString();
            showSumOfChosenLevels();
            showStatsInOverlay();

            setActiveStat(activeStatKeeper);

            if (!Extraction.E.postTamed)
            {
                labelFootnote.Text = "*Creature is not yet tamed and may get better values then.";
                button2TamingCalc.Visible = true;
                // display taming info

                if (checkBoxQuickWildCheck.Checked)
                    tamingControl1.level = statIOs[7].LevelWild + 1;
                else
                    tamingControl1.level = (int)numericUpDownLevel.Value;
                tamingControl1.selectedSpeciesIndex = comboBoxSpeciesExtractor.SelectedIndex;
                labelTamingInfo.Text = tamingControl1.tamingInfo;
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
            labelDoc.Visible = false;
            if (checkBoxAlreadyBred.Checked && numericUpDownImprintingBonusExtractor.Value > 0)
                labelImprintingFailInfo.Visible = true;
        }

        private void setUniqueTE()
        {
            double te = Extraction.E.uniqueTE();
            statIOs[7].LevelWild = Extraction.E.trueTorporLevel(te);
            if (te >= 0)
            {
                labelTE.Text = "Extracted: " + Math.Round(100 * te, 1) + " %";
                if (Extraction.E.postTamed && !checkBoxAlreadyBred.Checked)
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
            int sumW = 0, sumD = 0;
            bool valid = true, inbound = true, allUnique = true;
            for (int s = 0; s < 7; s++)
            {
                if (Extraction.E.results[s].Count > Extraction.E.chosenResults[s])
                {
                    sumW += statIOs[s].LevelWild;
                    sumD += statIOs[s].LevelDom;
                    if (Extraction.E.results[s].Count != 1) { allUnique = false; }
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
                labelSumWild.Text = sumW.ToString();
                labelSumDom.Text = sumD.ToString();
                if (sumW <= Extraction.E.levelWildFromTorporRange[1]) { labelSumWild.ForeColor = SystemColors.ControlText; }
                else
                {
                    labelSumWild.ForeColor = Color.Red;
                    inbound = false;
                }
                if (sumD <= Extraction.E.levelDomFromTorporAndTotalRange[1] && sumD >= Extraction.E.levelDomFromTorporAndTotalRange[0]) { labelSumDom.ForeColor = SystemColors.ControlText; }
                else
                {
                    labelSumDom.ForeColor = Color.Red;
                    inbound = false;
                }
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
            bool allValid = valid && inbound && Extraction.E.validResults;
            if (allValid)
            {
                creatureInfoInputExtractor.parentListValid = false;
            }
            creatureInfoInputExtractor.ButtonEnabled = allValid;
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
            if (s < Extraction.E.results.Length)
            {
                bool resultsValid = Extraction.E.filterResultsByFixed(s) == -1;
                ListViewItem lvi;
                List<string> subItems = new List<string>();
                double te;
                for (int r = 0; r < Extraction.E.results[s].Count; r++)
                {
                    subItems.Clear();
                    te = Extraction.E.results[s][r].TE;
                    subItems.Add(Extraction.E.results[s][r].levelWild.ToString());
                    subItems.Add(Extraction.E.results[s][r].levelDom.ToString());
                    subItems.Add((te >= 0 ? (te * 100).ToString() : ""));

                    subItems.Add((te > 0 ? Math.Ceiling((Extraction.E.trueTorporLevel(te) + 1) / (1 + te / 2)).ToString() : ""));

                    lvi = new ListViewItem(subItems.ToArray());
                    if (!resultsValid || Extraction.E.results[s][r].currentlyNotValid)
                        lvi.BackColor = Color.LightSalmon;
                    if (Extraction.E.fixedResults[s] && Extraction.E.chosenResults[s] == r)
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
            comboBoxSpeciesExtractor.Items.Clear();
            cbbStatTestingSpecies.Items.Clear();
            for (int s = 0; s < Values.V.speciesNames.Count; s++)
            {
                comboBoxSpeciesExtractor.Items.Add(Values.V.speciesNames[s]);
                cbbStatTestingSpecies.Items.Add(Values.V.speciesNames[s]);
            }
            comboBoxSpeciesExtractor.SelectedIndex = 0;
            cbbStatTestingSpecies.SelectedIndex = 0;
            tamingControl1.Species = Values.V.speciesNames;
        }

        private void comboBoxCreatures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSpeciesExtractor.SelectedIndex >= 0)
            {
                sE = comboBoxSpeciesExtractor.SelectedIndex;
                for (int s = 0; s < 8; s++)
                {
                    activeStats[s] = (Values.V.species[sE].stats[s].BaseValue > 0);
                    statIOs[s].Enabled = activeStats[s];
                }
                // if torpor has no tamed-add-bonus, the automatic tamed-recognition does not work => enable manual selection
                if (Values.V.species[sE].stats[7].AddWhenTamed == 0)
                {
                    checkBoxWildTamedAuto.Checked = false;
                    radioButtonTamed.Checked = true;
                }
                else
                    checkBoxWildTamedAuto.Checked = true;
                creatureInfoInputExtractor.SpeciesIndex = sE;
                clearAll();
            }
        }

        private void cbbStatTestingRace_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = cbbStatTestingSpecies.SelectedIndex;
            if (i >= 0)
            {
                for (int s = 0; s < 8; s++)
                {
                    testingIOs[s].Enabled = (Values.V.species[i].stats[s].BaseValue > 0);
                }
                updateAllTesterValues();
                creatureInfoInputTester.SpeciesIndex = i;
            }
            //breedingInfo1.displayData(i);
            setTesterEditCreature();
        }

        private void listViewPossibilities_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewPossibilities.SelectedIndices.Count > 0)
            {
                int index = (int)listViewPossibilities.SelectedItems[0].Tag;
                if (index >= 0 && activeStat >= 0)
                {
                    setPossibility(activeStat, index, true);
                    Extraction.E.fixedResults[activeStat] = true;
                }
            }
            else if (activeStat >= 0)
                Extraction.E.fixedResults[activeStat] = false;
        }

        private void setPossibility(int s, int i, bool validateCombination = false)
        {
            statIOs[s].LevelWild = (Int32)Extraction.E.results[s][i].levelWild;
            statIOs[s].LevelDom = (Int32)Extraction.E.results[s][i].levelDom;
            statIOs[s].TamingEffectiveness = (Int32)Extraction.E.results[s][i].TE;
            statIOs[s].BreedingValue = breedingValue(s, i);
            Extraction.E.chosenResults[s] = i;
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
             * wild speed level is current level - (wild levels + dom levels) - 1. sometimes the oxygenlevel cannot be determined
             */
            // TODO: take notDetermined Levels from Torpor (with torpor-bug adjustment), then subtract only the wildlevels (this solves Plesio-issue)
            //int notDeterminedLevels = (int)numericUpDownLevel.Value - 1 - (Values.V.speciesNames[sE] == "Plesiosaur" ? 34 : 0);
            int notDeterminedLevels = statIOs[7].LevelWild;
            bool unique = true;
            for (int s = 0; s < 6; s++)
            {
                if (activeStats[s])
                {
                    //notDeterminedLevels -= statIOs[s].LevelDom;
                    notDeterminedLevels -= statIOs[s].LevelWild;
                }
                else { unique = false; break; }
            }
            if (unique)
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
            if (Extraction.E.validResults)
            {
                List<string> tsv = new List<string>();
                string rowLevel = comboBoxSpeciesExtractor.SelectedItem.ToString() + "\t\t", rowValues = "";
                // if taming effectiveness is unique, display it, too
                string effString = "";
                double eff = Extraction.E.uniqueTE();
                if (eff >= 0)
                {
                    effString = "\tTamingEff:\t" + (100 * eff).ToString() + "%";
                }
                // headerrow
                if (table || header)
                {
                    if (table)
                    {
                        tsv.Add(comboBoxSpeciesExtractor.SelectedItem.ToString() + "\tLevel " + numericUpDownLevel.Value.ToString() + effString);
                        tsv.Add("Stat\tWildLevel\tDomLevel\tBreedingValue");
                    }
                    else { tsv.Add("Species\tName\tSex\tHP-Level\tSt-Level\tOx-Level\tFo-Level\tWe-Level\tDm-Level\tSp-Level\tTo-Level\tHP-Value\tSt-Value\tOx-Value\tFo-Value\tWe-Value\tDm-Value\tSp-Value\tTo-Value"); }
                }
                for (int s = 0; s < 8; s++)
                {
                    if (Extraction.E.chosenResults[s] < Extraction.E.results[s].Count)
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
                if (r >= 0 && r < Extraction.E.results[s].Count)
                {
                    return Stats.calculateValue(sE, s, Extraction.E.results[s][r].levelWild, 0, true, 1, 0);
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

        private void checkBoxAlreadyBred_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxTE.Enabled = !checkBoxAlreadyBred.Checked;
            checkBoxJustTamed.Checked = checkBoxJustTamed.Checked && !checkBoxAlreadyBred.Checked;
            panelWildTamedAuto.Enabled = !checkBoxAlreadyBred.Checked;
            numericUpDownImprintingBonusExtractor.Visible = checkBoxAlreadyBred.Checked;
            labelImprintingBonus.Visible = checkBoxAlreadyBred.Checked;
            labelImprintingCuddleCountExtractor.Visible = checkBoxAlreadyBred.Checked;
        }

        private void checkBoxJustTamed_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxAlreadyBred.Checked = checkBoxAlreadyBred.Checked && !checkBoxJustTamed.Checked;
        }

        private void checkBoxWildTamedAuto_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonTamed.Enabled = !checkBoxWildTamedAuto.Checked;
            radioButtonWild.Enabled = !checkBoxWildTamedAuto.Checked;
        }

        /// <summary>
        /// call this function to recalculate all stat-values of Creature c according to its levels
        /// </summary>
        private void recalculateCreatureValues(Creature c)
        {
            int speciesIndex = Values.V.speciesNames.IndexOf(c.species);
            if (speciesIndex >= 0)
            {
                for (int s = 0; s < 8; s++)
                {
                    c.valuesBreeding[s] = Stats.calculateValue(speciesIndex, s, c.levelsWild[s], 0, true, 1, 0);
                    c.valuesDom[s] = Stats.calculateValue(speciesIndex, s, c.levelsWild[s], c.levelsDom[s], true, c.tamingEff, c.imprintingBonus);
                }
            }
            c.calculateLevelFound();
        }

        private void recalculateAllCreaturesValues()
        {
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = creatureCollection.creatures.Count();
            toolStripProgressBar1.Visible = true;
            foreach (Creature c in creatureCollection.creatures)
            {
                recalculateCreatureValues(c);
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
                species = Values.V.speciesNames[sE];
                bred = checkBoxAlreadyBred.Checked;
                te = Extraction.E.uniqueTE();
                imprinting = Extraction.E.imprintingBonus;
            }
            else
            {
                input = creatureInfoInputTester;
                species = Values.V.speciesNames[cbbStatTestingSpecies.SelectedIndex];
                bred = checkBoxStatTestingBred.Checked;
                te = (double)NumericUpDownTestingTE.Value / 100;
                imprinting = (double)numericUpDownImprintingBonusTester.Value / 100;
            }

            Creature creature = new Creature(species, input.CreatureName, input.CreatureOwner, input.CreatureSex, getCurrentWildLevels(fromExtractor), getCurrentDomLevels(fromExtractor), te, bred, imprinting);

            // set parents
            creature.Mother = input.mother;
            creature.Father = input.father;

            // cooldown-, growing-time
            creature.cooldownUntil = input.Cooldown;
            creature.growingUntil = input.Grown;

            creature.domesticatedAt = input.domesticatedAt;
            creature.mutationCounter = input.MutationCounter;

            recalculateCreatureValues(creature);
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

        private void assignCollectionClasses()
        {
            pedigree1.creatures = creatureCollection.creatures;
            breedingPlan1.maxSuggestions = creatureCollection.maxBreedingSuggestions;
            tribesControl1.Tribes = creatureCollection.tribes;
            tribesControl1.Players = creatureCollection.players;
            timerList1.TimerListEntries = creatureCollection.timerListEntries;
        }

        private void applyMultipliersToValues()
        {
            Values.V.applyMultipliersToStats(creatureCollection.multipliers);
            Values.V.applyMultipliersToBreedingTimes(creatureCollection.breedingMultipliers);
            Values.V.imprintingMultiplier = creatureCollection.imprintingMultiplier;
            Values.V.tamingSpeedMultiplier = creatureCollection.tamingSpeedMultiplier;
            Values.V.tamingFoodRateMultiplier = creatureCollection.tamingFoodRateMultiplier;
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

        private void loadCollectionFile(string fileName, bool keepCurrentCreatures = false)
        {
            XmlSerializer reader = new XmlSerializer(typeof(CreatureCollection));

            if (!System.IO.File.Exists(fileName))
            {
                MessageBox.Show("Save file with name \"" + fileName + "\" does not exist!", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<Creature> oldCreatures = null;
            if (keepCurrentCreatures)
                oldCreatures = creatureCollection.creatures;

            System.IO.FileStream file = System.IO.File.OpenRead(fileName);

            // for the case the collectionfile has no multipliers, keep the current ones
            double[][] oldMultipliers = creatureCollection.multipliers;

            try
            {
                creatureCollection = (CreatureCollection)reader.Deserialize(file);
            }
            catch (Exception e)
            {
                MessageBox.Show("File Couldn't be opened, we thought you should know.\nErrormessage:\n\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                file.Close();
                return;
            }
            file.Close();

            if (creatureCollection.multipliers == null)
            {
                creatureCollection.multipliers = oldMultipliers;
                if (creatureCollection.multipliers == null)
                    creatureCollection.multipliers = Values.V.getOfficialMultipliers();
            }

            applyMultipliersToValues();
            assignCollectionClasses();

            bool creatureWasAdded = false;

            if (keepCurrentCreatures)
                creatureWasAdded = creatureCollection.mergeCreatureList(oldCreatures);
            else
            {
                currentFileName = fileName;
                fileSync.changeFile(currentFileName);
                creatureBoxListView.Clear();
            }
            filterListAllowed = false;
            checkBoxShowDead.Checked = creatureCollection.showDeads;
            checkBoxShowUnavailableCreatures.Checked = creatureCollection.showUnavailable;
            checkBoxShowNeuteredCreatures.Checked = creatureCollection.showNeutered;
            filterListAllowed = true;

            setCollectionChanged(creatureWasAdded); // setCollectionChanged only if there really were creatures added from the old library to the just opened one
            // creatures loaded.

            applyLevelSettings();

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

            updateParents(creatureCollection.creatures);
            updateCreatureListings();

            // apply last sorting
            this.listViewLibrary.Sort();

            Properties.Settings.Default.LastSaveFile = fileName;
        }

        /// <summary>
        /// This function should be called if the creatureCollection is changed, i.e. after loading a file or adding/removing a creature
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
            toolStripStatusLabel.Text = creatureCollection.creatures.Count() + " creatures in Library. Version " + Application.ProductVersion + " / " + Values.V.version.ToString();
            breedingPlan1.CurrentSpecies = ""; // set to empty so creatures are loaded again if breeding plan is created
            pedigree1.updateListView();
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
            // add node to show all
            listBoxSpeciesLib.Items.Add("All");

            foreach (Creature cr in creatures)
            {
                // add new item for species if not existent
                if (listBoxSpeciesLib.Items.IndexOf(cr.species) == -1)
                {
                    // add new node alphabetically
                    int nn = 0;
                    while (nn < listBoxSpeciesLib.Items.Count && String.Compare(listBoxSpeciesLib.Items[nn].ToString(), cr.species, true) < 0) { nn++; }
                    listBoxSpeciesLib.Items.Insert(nn, cr.species);
                }
            }
            if (selectedSpecies.Length > 0)
                listBoxSpeciesLib.SelectedIndex = listBoxSpeciesLib.Items.IndexOf(selectedSpecies);

            // set the same species to breedingplaner, except the 'all'
            selectedSpecies = "";
            if (listViewSpeciesBP.SelectedIndices.Count > 0)
                selectedSpecies = listViewSpeciesBP.SelectedIndices[0].ToString();
            listViewSpeciesBP.Items.Clear();

            ListViewItem lvi;
            for (int i = 1; i < listBoxSpeciesLib.Items.Count; i++)
            {
                lvi = new ListViewItem(listBoxSpeciesLib.Items[i].ToString());
                lvi.Tag = listBoxSpeciesLib.Items[i].ToString();
                // check if species has both available males and females
                if (creatures.Count(c => c.species == listBoxSpeciesLib.Items[i].ToString() && c.status == CreatureStatus.Available && c.gender == Sex.Female) > 0 && creatures.Count(c => c.species == listBoxSpeciesLib.Items[i].ToString() && c.status == CreatureStatus.Available && c.gender == Sex.Male) == 0)
                    lvi.ForeColor = Color.LightGray;
                listViewSpeciesBP.Items.Add(lvi);
            }

            // select previous selecteded again
            if (selectedSpecies.Length > 0)
            {
                for (int i = 0; i < listViewSpeciesBP.Items.Count; i++)
                {
                    if ((string)listViewSpeciesBP.Items[i].Tag == selectedSpecies)
                    {
                        listViewSpeciesBP.Items[i].Focused = true;
                        listViewSpeciesBP.Items[i].Selected = true;
                        break;
                    }
                }
            }
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
            recalculateCreatureValues(cr);
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
            int topStatsCount = cr.topStatsCount;
            string[] subItems = (new string[] { cr.name + (cr.status != CreatureStatus.Available ? " (" + Utils.statusSymbol(cr.status) + ")" : ""), cr.owner, Utils.sexSymbol(cr.gender), cr.domesticatedAt.ToString("yyyy'-'MM'-'dd HH':'mm"), cr.topness.ToString(), topStatsCount.ToString(), cr.generation.ToString(), cr.levelFound.ToString(), cr.mutationCounter.ToString() }).Concat(cr.levelsWild.Select(x => x.ToString()).ToArray()).ToArray();
            ListViewItem lvi = new ListViewItem(subItems, g);
            for (int s = 0; s < 8; s++)
            {
                // color unknown levels
                if (cr.levelsWild[s] < 0)
                {
                    lvi.SubItems[s + 9].ForeColor = Color.WhiteSmoke;
                    lvi.SubItems[s + 9].BackColor = Color.WhiteSmoke;
                }
                else
                    lvi.SubItems[s + 9].BackColor = Utils.getColorFromPercent((int)(cr.levelsWild[s] * (s == 7 ? .357 : 2.5)), (considerStatHighlight[s] ? (cr.topBreedingStats[s] ? 0.2 : 0.7) : 0.93));
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
            if (topStatsCount > 0)
            {
                if (cr.topBreedingCreature)
                    lvi.BackColor = Color.LightGreen;
                lvi.SubItems[5].BackColor = Utils.getColorFromPercent(topStatsCount * 8 + 44, 0.7);
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

            lvi.Tag = cr;
            return lvi;
        }

        // user wants to check if a new version of stats.txt is available and then download it
        private void checkForUpdatedStatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkForUpdates();
        }

        private void checkForUpdates(bool silentCheck = false)
        {
            bool updated = false;
            bool newToolVersionAvailable = false;
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
                int remoteFileVer;
                string filename = "values.json";

                remoteFileVer = 0;
                if (Int32.TryParse(remoteVers[0], out remoteFileVer) && Values.V.version < remoteFileVer)
                {
                    // backup the current version (to safe user added custom commands)
                    if (MessageBox.Show("There is a new version of the values-file \"" + filename + "\", do you want to update it?\n\nIf you play on a console (Xbox or PS4) make a backup of the current file before you click on Yes, as the updated values may not work with the console-version for some time.\nUsually it takes some days to weeks until the changes are valid on the consoles as well.", "Update Values-File?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // System.IO.File.Copy(filename, filename + "_backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json");
                        // Download the Web resource and save it into the current filesystem folder.
                        myWebClient.DownloadFile(remoteUri + filename, filename);
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
                    applyMultipliersToValues();
                    updateSpeciesComboboxes();
                    MessageBox.Show("Download and update of new creature-stats successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    toolStripStatusLabel.Text = creatureCollection.creatures.Count() + " creatures in Library. Version " + Application.ProductVersion + " / " + Values.V.version.ToString();
                }
                else
                    MessageBox.Show("Download of new stat successful, but files couldn't be loaded.\nTry again later, revert the backuped files (..._backup_[timestamp].json) or redownload the tool.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!silentCheck)
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
            string species = (fromExtractor ? Values.V.speciesNames[sE] : cbbStatTestingSpecies.SelectedItem.ToString());
            Creature creature = new Creature(species, "", "", 0, getCurrentWildLevels(fromExtractor));
            List<Creature>[] parents = findParents(creature);
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

            if (creatureCollection.multipliers == null)
                creatureCollection.multipliers = Values.V.getOfficialMultipliers();
            // use previously used multipliers again in the new file
            double[][] oldMultipliers = creatureCollection.multipliers;

            creatureCollection = new CreatureCollection();
            creatureCollection.multipliers = oldMultipliers;
            pedigree1.Clear();
            breedingPlan1.Clear();
            applyMultipliersToValues();
            assignCollectionClasses();

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
            Properties.Settings.Default.listViewSortCol = lvwColumnSorterLibrary.SortColumn;
            Properties.Settings.Default.listViewSortAsc = (lvwColumnSorterLibrary.Order == SortOrder.Ascending);

            // save custom statweights
            List<string> custWs = new List<string>();
            List<double[]> custWd = new List<double[]>();
            foreach (KeyValuePair<string, double[]> w in statWeighting1.CustomWeightings)
            {
                custWs.Add(w.Key);
                custWd.Add(w.Value);
            }
            custWd.Add(statWeighting1.Values); // add current values
            Properties.Settings.Default.customStatWeights = custWd.ToArray();
            Properties.Settings.Default.customStatWeightNames = custWs.ToArray();

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
            ListView lw = (ListView)sender;
            ListViewColumnSorter lwcs = (ListViewColumnSorter)lw.ListViewItemSorter;
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lwcs.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lwcs.Order == SortOrder.Ascending)
                {
                    lwcs.Order = SortOrder.Descending;
                }
                else
                {
                    lwcs.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to descending (except the name and owner column).
                lwcs.LastSortColumn = lwcs.SortColumn;
                lwcs.LastOrder = lwcs.Order;
                lwcs.SortColumn = e.Column;
                if (e.Column > 1)
                    lwcs.Order = SortOrder.Descending;
                else
                    lwcs.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            lw.Sort();
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
            filterLib();
        }

        private void checkBoxShowUnavailableCreatures_CheckedChanged(object sender, EventArgs e)
        {
            creatureCollection.showUnavailable = checkBoxShowUnavailableCreatures.Checked;
            filterLib();
        }

        private void checkBoxShowNeuteredCreatures_CheckedChanged(object sender, EventArgs e)
        {
            creatureCollection.showNeutered = checkBoxShowNeuteredCreatures.Checked;
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

                filterLib();
            }
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

                // if only certain owner's creatures should be shown
                bool hideWOOwner = (creatureCollection.hiddenOwners.IndexOf("n/a") >= 0);
                filteredList = filteredList.Where(c => !creatureCollection.hiddenOwners.Contains(c.owner) && (!hideWOOwner || c.owner != ""));

                // show also dead creatures?
                if (!checkBoxShowDead.Checked)
                    filteredList = filteredList.Where(c => c.status != CreatureStatus.Dead);

                // show also unavailable creatures?
                if (!checkBoxShowUnavailableCreatures.Checked)
                    filteredList = filteredList.Where(c => c.status != CreatureStatus.Unavailable);

                // show also neutered creatures?
                if (!checkBoxShowNeuteredCreatures.Checked)
                    filteredList = filteredList.Where(c => !c.neutered);

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

                    // if not available, continue
                    if (c.status != CreatureStatus.Available)
                        continue;

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
            List<Creature>[] parents = findParents(creature);
            creatureBoxListView.parentListSimilarity = findParentSimilarities(parents, creature);
            creatureBoxListView.parentList = parents;
        }

        private List<Creature>[] findParents(Creature creature)
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

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            timerList1.UpdateTimes = (tabControlMain.SelectedTab == tabPageTimer);
            toolStripButtonCopy2Extractor.Visible = (tabControlMain.SelectedTab == tabPageStatTesting);
            toolStripButtonCopy2Tester.Visible = (tabControlMain.SelectedTab == tabPageExtractor);
            toolStripButtonExtract.Visible = (tabControlMain.SelectedTab == tabPageExtractor);
            toolStripButtonAddPlayer.Visible = (tabControlMain.SelectedTab == tabPagePlayerTribes);
            toolStripButtonAddTribe.Visible = (tabControlMain.SelectedTab == tabPagePlayerTribes);
            toolStripButtonClear.Visible = (tabControlMain.SelectedTab == tabPageExtractor || tabControlMain.SelectedTab == tabPageStatTesting);
            forARKChatToolStripMenuItem.Visible = (tabControlMain.SelectedTab == tabPageLibrary);
            //creatureToolStripMenuItem.Enabled = (tabControl1.SelectedTab == tabPageLibrary);
            if (tabControlMain.SelectedTab == tabPagePedigree && pedigreeNeedsUpdate && listViewLibrary.SelectedItems.Count > 0)
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
            else if (tabControlMain.SelectedTab == tabPageBreedingPlan && breedingPlanNeedsUpdate)
            {
                determineBestBreeding(breedingPlan1.chosenCreature);
            }
        }

        private void setCollectionChanged(bool changed, string species = null)
        {
            if (changed)
            {
                if (species == null || (pedigree1.creature != null && pedigree1.creature.species == species))
                    pedigreeNeedsUpdate = true;
                if (species == null || breedingPlan1.CurrentSpecies == species)
                    breedingPlanNeedsUpdate = true;
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
                cbbStatTestingSpecies.SelectedIndex = Values.V.speciesNames.IndexOf(c.species);
                NumericUpDownTestingTE.Value = (c.tamingEff >= 0 ? (decimal)c.tamingEff * 100 : 0);
                numericUpDownImprintingBonusTester.Value = (decimal)c.imprintingBonus * 100;
                checkBoxStatTestingBred.Checked = c.isBred;

                for (int s = 0; s < 7; s++)
                {
                    testingIOs[s].LevelWild = c.levelsWild[s];
                    testingIOs[s].LevelDom = c.levelsDom[s];
                }
                tabControlMain.SelectedTab = tabPageStatTesting;
                setTesterEditCreature(c, virtualCreature);
            }
        }

        private void updateAllTesterValues()
        {
            updateTorporInTester = false;
            for (int s = 0; s < 7; s++)
            {
                if (s == 6)
                    updateTorporInTester = true;
                statIOUpdateValue(testingIOs[s]);
            }
            statIOUpdateValue(testingIOs[7]);
        }

        private void NumericUpDownTestingTE_ValueChanged(object sender, EventArgs e)
        {
            updateAllTesterValues();
        }

        private void numericUpDownImprintingBonusTester_ValueChanged(object sender, EventArgs e)
        {
            updateAllTesterValues();
            // calculate number of imprintings
            if (Values.V.species[cbbStatTestingSpecies.SelectedIndex].breeding != null && Values.V.species[cbbStatTestingSpecies.SelectedIndex].breeding.maturationTimeAdjusted > 0)
                labelImprintedCount.Text = "(" + Math.Round((double)numericUpDownImprintingBonusTester.Value * Values.V.species[cbbStatTestingSpecies.SelectedIndex].breeding.maturationTimeAdjusted / (1440000 * creatureCollection.babyCuddleIntervalMultiplier), 2) + "×)";
            else labelImprintedCount.Text = "";
        }

        private void numericUpDownImprintingBonusExtractor_ValueChanged(object sender, EventArgs e)
        {
            // calculate number of imprintings
            if (Values.V.species[comboBoxSpeciesExtractor.SelectedIndex].breeding != null && Values.V.species[comboBoxSpeciesExtractor.SelectedIndex].breeding.maturationTimeAdjusted > 0)
                labelImprintingCuddleCountExtractor.Text = "(" + Math.Round((double)numericUpDownImprintingBonusExtractor.Value * Values.V.species[comboBoxSpeciesExtractor.SelectedIndex].breeding.maturationTimeAdjusted / (1440000 * creatureCollection.babyCuddleIntervalMultiplier)) + "×)";
            else labelImprintingCuddleCountExtractor.Text = "";
        }

        private void checkBoxStatTestingTamed_CheckedChanged(object sender, EventArgs e)
        {
            setTesterInputsTamed(checkBoxStatTestingTamed.Checked);
            updateAllTesterValues();
        }

        private void checkBoxStatTestingBred_CheckedChanged(object sender, EventArgs e)
        {
            setTesterInputsTamed(checkBoxStatTestingBred.Checked || checkBoxStatTestingTamed.Checked);
            checkBoxStatTestingTamed.Enabled = !checkBoxStatTestingBred.Checked;
            NumericUpDownTestingTE.Enabled = !checkBoxStatTestingBred.Checked;
            numericUpDownImprintingBonusTester.Enabled = checkBoxStatTestingBred.Checked;
            labelImprintingTester.Enabled = checkBoxStatTestingBred.Checked;
            updateAllTesterValues();
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
            checkBoxAlreadyBred.Enabled = enabled;
            panelWildTamedAuto.Enabled = enabled;
            checkBoxJustTamed.Enabled = enabled;
            groupBoxTE.Enabled = enabled;
            numericUpDownLevel.Enabled = enabled;
            button2TamingCalc.Visible = !enabled;
        }

        /// <summary>
        /// Updates the values in the testing-statIOs
        /// </summary>
        /// <param name="sIo"></param>
        private void statIOUpdateValue(StatIO sIo)
        {
            sIo.BreedingValue = Stats.calculateValue(cbbStatTestingSpecies.SelectedIndex, sIo.statIndex, sIo.LevelWild, 0, true, 1, 0);
            sIo.Input = Stats.calculateValue(cbbStatTestingSpecies.SelectedIndex, sIo.statIndex, sIo.LevelWild, sIo.LevelDom, (checkBoxStatTestingTamed.Checked || checkBoxStatTestingBred.Checked), (checkBoxStatTestingBred.Checked ? 1 : (double)NumericUpDownTestingTE.Value / 100), (checkBoxStatTestingBred.Checked ? (double)numericUpDownImprintingBonusTester.Value / 100 : 0));

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
        }

        private void onlinehelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/cadon/ARKStatsExtractor/wiki/manual");
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

        private void copyForARKChat(bool breeding = true)
        {
            if (listViewLibrary.SelectedItems.Count > 0)
            {
                Creature c = (Creature)listViewLibrary.SelectedItems[0].Tag;
                string output = Utils.getARKml(c.species, 50, 172, 255) + " (lvl " + (breeding ? c.levelHatched : c.level) + (c.gender != Sex.Unknown ? ", " + c.gender.ToString() : "") + "): ";
                for (int s = 0; s < 8; s++)
                {
                    output += Utils.statName(s, true) + ": " + ((breeding ? c.valuesBreeding[s] : c.valuesDom[s]) * (Utils.precision(s) == 3 ? 100 : 1)) + (Utils.precision(s) == 3 ? "%" : "") + " (" + Utils.getARKmlFromPercent(c.levelsWild[s].ToString(), (int)(c.levelsWild[s] * (s == 7 ? .357 : 2.5))) + (breeding || s == 7 ? "" : ", " + Utils.getARKmlFromPercent(c.levelsDom[s].ToString(), (int)(c.levelsDom[s] * 2.5))) + "); ";
                }
                Clipboard.SetText(output);
            }
            else
                MessageBox.Show("No creatures in the library selected to copy to the clipboard", "No Creatures Selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void forSpreadsheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportForSpreadsheet();
        }

        private void forARKChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyForARKChat();
        }

        private void forARKChatcurrentValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyForARKChat(false);
        }

        private void buttonRecalculateTops_Click(object sender, EventArgs e)
        {
            for (int s = 0; s < 8; s++)
                considerStatHighlight[s] = checkedListBoxConsiderStatTop.GetItemChecked(s);
            // recalculate topstats
            calculateTopStats(creatureCollection.creatures);
            filterLib();
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
                if (!multipleSpecies) parents = findParents(c);

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
                    filterLib();
                }
                ms.Dispose();
            }
        }

        private void buttonDetBestBreeding_Click(object sender, EventArgs e)
        {
            determineBestBreeding();
        }

        private void listViewSpeciesBP_SelectedIndexChanged(object sender, EventArgs e)
        {
            determineBestBreeding();
        }

        private void radioButtonBPTopStatsCn_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonBPTopStatsCn.Checked)
                breedingPlan1.drawBestParents(BreedingPlan.BreedingMode.TopStatsConservative);
        }

        private void radioButtonBPTopStats_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonBPTopStats.Checked)
                breedingPlan1.drawBestParents(BreedingPlan.BreedingMode.TopStatsLucky);
        }

        private void radioButtonBPHighStats_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonBPHighStats.Checked)
                breedingPlan1.drawBestParents(BreedingPlan.BreedingMode.BestNextGen);
        }

        private void recalculateBreedingPlan()
        {
            breedingPlanNeedsUpdate = true;
            determineBestBreeding();
        }

        private void determineBestBreeding(Creature chosenCreature = null)
        {
            string selectedSpecies = (chosenCreature != null ? chosenCreature.species : "");
            bool newSpecies = false;
            if (selectedSpecies.Length == 0 && listViewSpeciesBP.SelectedIndices.Count > 0)
                selectedSpecies = (string)listViewSpeciesBP.SelectedItems[0].Tag;
            if (selectedSpecies.Length > 0 && breedingPlan1.CurrentSpecies != selectedSpecies)
            {
                breedingPlan1.CurrentSpecies = selectedSpecies;
                newSpecies = true;

                int s = Values.V.speciesNames.IndexOf(selectedSpecies);
                breedingPlan1.EnabledColorRegions = (s >= 0 ? Values.V.species[s].colors.Select(n => n.name != "").ToArray() : new bool[6] { true, true, true, true, true, true });

                breedingPlanNeedsUpdate = true;
            }
            if (breedingPlanNeedsUpdate)
                breedingPlan1.Creatures = creatureCollection.creatures.Where(c => (c == chosenCreature && c != null) || (c.species == selectedSpecies && c.status == CreatureStatus.Available && !c.neutered && c.cooldownUntil < DateTime.Now && c.growingUntil < DateTime.Now)).ToList();

            breedingPlan1.statWeights = statWeighting1.Weightings;
            BreedingPlan.BreedingMode bm = BreedingPlan.BreedingMode.TopStatsConservative;
            if (radioButtonBPTopStats.Checked)
                bm = BreedingPlan.BreedingMode.TopStatsLucky;
            else if (radioButtonBPHighStats.Checked)
                bm = BreedingPlan.BreedingMode.BestNextGen;

            breedingPlan1.chosenCreature = chosenCreature;
            breedingPlan1.drawBestParents(bm, newSpecies);
            breedingPlanNeedsUpdate = false;
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
            if (c.status != CreatureStatus.Available)
            {
                if (MessageBox.Show("Selected Creature is currently not marked as \"Available\" and thus cannot be considered for breeding. Do you want to change its status to \"Available\"?", "Selected Creature not Available", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    setStatus(new List<Creature>() { c }, CreatureStatus.Available);
                }
            }
            determineBestBreeding(c);
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
                    bool statusChanged = creatureTesterEdit.status != creatureInfoInputTester.CreatureStatus;
                    bool parentsChanged = (creatureTesterEdit.Mother != creatureInfoInputTester.mother || creatureTesterEdit.Father != creatureInfoInputTester.father);
                    creatureTesterEdit.levelsWild = getCurrentWildLevels(false);
                    creatureTesterEdit.levelsDom = getCurrentDomLevels(false);
                    creatureTesterEdit.tamingEff = (double)NumericUpDownTestingTE.Value / 100;
                    creatureTesterEdit.isBred = checkBoxStatTestingBred.Checked;
                    creatureTesterEdit.imprintingBonus = (double)numericUpDownImprintingBonusTester.Value / 100;

                    creatureTesterEdit.name = creatureInfoInputTester.CreatureName;
                    creatureTesterEdit.gender = creatureInfoInputTester.CreatureSex;
                    creatureTesterEdit.owner = creatureInfoInputTester.CreatureOwner;
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

                    setTesterEditCreature();
                    tabControlMain.SelectedTab = tabPageLibrary;
                }
            }
        }

        private void setTesterEditCreature(Creature c = null, bool virtualCreature = false)
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
                creatureInfoInputTester.CreatureStatus = c.status;
                creatureInfoInputTester.CreatureNote = c.note;
                creatureInfoInputTester.Cooldown = c.cooldownUntil;
                creatureInfoInputTester.Grown = c.growingUntil;
                creatureInfoInputTester.domesticatedAt = c.domesticatedAt.Year < 2000 ? DateTime.Now : c.domesticatedAt;
                creatureInfoInputTester.Neutered = c.neutered;
                creatureInfoInputTester.MutationCounter = c.mutationCounter;
                updateParentListInput(creatureInfoInputTester);
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
            }
            creatureTesterEdit = c;
        }

        private void toolStripButtonSettings_Click(object sender, EventArgs e)
        {
            settingsToolStripMenuItem_Click(sender, e);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settingsfrm = new Settings(creatureCollection);
            if (settingsfrm.ShowDialog() == DialogResult.OK)
            {
                applyMultipliersToValues();
                autoSave = Properties.Settings.Default.autosave;
                autoSaveMinutes = Properties.Settings.Default.autosaveMinutes;
                creatureBoxListView.maxDomLevel = creatureCollection.maxDomLevel;
                breedingPlan1.maxSuggestions = creatureCollection.maxBreedingSuggestions;
                fileSync.changeFile(currentFileName); // only to trigger the update, filename is not changed
                applyLevelSettings();

                setCollectionChanged(true);
            }
        }

        private void applyLevelSettings()
        {
            creatureBoxListView.BarMaxLevel = creatureCollection.maxWildLevel / 3;
            for (int s = 0; s < 8; s++)
            {
                statIOs[s].barMaxLevel = creatureCollection.maxWildLevel / 3;
                testingIOs[s].barMaxLevel = creatureCollection.maxWildLevel / 3;
            }
            breedingPlan1.maxWildLevels = creatureCollection.maxWildLevel;
        }

        /// <summary>
        /// Display the wild-levels, assuming it's a wild creature. Used for quick checking
        /// </summary>
        /// <param name="sIO"></param>
        private void statIOQuickWildLevelCheck(StatIO sIO)
        {
            if (checkBoxQuickWildCheck.Checked)
            {
                int lvlWild = (int)Math.Round((sIO.Input - Values.V.species[sE].stats[sIO.statIndex].BaseValue) / (Values.V.species[sE].stats[sIO.statIndex].BaseValue * Values.V.species[sE].stats[sIO.statIndex].IncPerWildLevel));
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

        private void createTimer(string name, DateTime time)
        {
            timerList1.addTimer(name, time);
        }

        private void setCreatureValuesToExtractor(Creature c, bool onlyWild = false)
        {
            if (c != null)
            {
                int speciesIndex = Values.V.speciesNames.IndexOf(c.species);
                if (speciesIndex >= 0)
                {
                    // copy values over to extractor
                    for (int s = 0; s < 8; s++)
                        statIOs[s].Input = (onlyWild ? Stats.calculateValue(speciesIndex, s, c.levelsWild[s], 0, true, c.tamingEff, c.imprintingBonus) : c.valuesDom[s]);
                    comboBoxSpeciesExtractor.SelectedIndex = speciesIndex;
                    checkBoxAlreadyBred.Checked = c.isBred;
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

        private void btnTestOCR_Click(object sender, EventArgs e)
        {
            string debugText;
            string dinoName;
            string ownerName;
            float[] OCRvalues = ArkOCR.OCR.doOCR(out debugText, out dinoName, out ownerName);

            txtOCROutput.Text = debugText;
        }

        private void OCRDebugLayoutPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void OCRDebugLayoutPanel_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
                doOCR(files[0], true);
        }

        private void btnFillValuesFromARK_Click(object sender, EventArgs e)
        {
            doOCR("", true);
        }

        public void doOCR(string imageFilePath = "", bool manuallyTriggered = true)
        {
            string debugText;
            string dinoName, ownerName;
            float[] OCRvalues = ArkOCR.OCR.doOCR(out debugText, out dinoName, out ownerName, imageFilePath, manuallyTriggered);

            txtOCROutput.Text = debugText;
            if (OCRvalues.Length <= 1)
                return;
            if ((decimal)OCRvalues[0] <= numericUpDownLevel.Maximum)
                numericUpDownLevel.Value = (decimal)OCRvalues[0];

            creatureInfoInputExtractor.CreatureName = dinoName;

            for (int i = 0; i < 8; i++)
            {
                if (statIOs[i].percent)
                    statIOs[i].Input = OCRvalues[i + 1] / 100.0;
                else
                    statIOs[i].Input = OCRvalues[i + 1];
            }

            // use imprinting if existing
            if (OCRvalues.Length > 9 && OCRvalues[10] > 0 && OCRvalues[10] <= 100)
            {
                checkBoxAlreadyBred.Checked = true;
                numericUpDownImprintingBonusExtractor.Value = (decimal)OCRvalues[10];
            }

            List<int> possibleDinos = determineDinoRaceFromStats(OCRvalues, dinoName);

            if (possibleDinos.Count == 1)
                extractLevels(); // only one possible dino, use that one
            else
            {
                bool sameValues = true;

                if (lastOCRValues != null)
                    for (int i = 0; i < 9; i++)
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
                    comboBoxSpeciesExtractor.SelectedIndex = possibleDinos[newindex];
                    lastOCRSpecies = possibleDinos[newindex];
                    lastOCRValues = OCRvalues;
                    extractLevels();
                }
                else
                { // automated, or first manual attempt at new values
                    bool foundPossiblyGood = false;
                    for (int dinooption = 0; dinooption < possibleDinos.Count() && foundPossiblyGood == false; dinooption++)
                    {
                        // if the last OCR'ed values are the same as this one, the user may not be happy with the dino species selection and want another one
                        // so we'll cycle to the next one, but only if the OCR is manually triggered, on autotrigger (ie, overlay), don't change
                        comboBoxSpeciesExtractor.SelectedIndex = possibleDinos[dinooption];
                        lastOCRSpecies = possibleDinos[dinooption];
                        lastOCRValues = OCRvalues;
                        foundPossiblyGood = extractLevels();
                    }
                }
            }

            lastOCRValues = OCRvalues;
            tabControlMain.SelectedTab = tabPageExtractor;

            // current weight for babies (has to be after the correct species is set in the combobox)
            if (OCRvalues.Length > 8 && OCRvalues[9] > 0)
            {
                creatureInfoInputExtractor.babyWeight = OCRvalues[9];
            }
        }

        private List<int> determineDinoRaceFromStats(float[] stats, string name)
        {
            List<int> possibleDinos = new List<int>();

            // for wild dinos, we can get the name directly.
            System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
            name = textInfo.ToTitleCase(name.ToLower());
            int sI = Values.V.speciesNames.IndexOf(name);
            if (sI >= 0)
            {
                possibleDinos.Add(sI);
                return possibleDinos;
            }

            if (stats.Length > 9 && stats[10] > 0)
            {
                // creature is imprinted, the following algorithm cannot handle this yet. use current selected species
                possibleDinos.Add(comboBoxSpeciesExtractor.SelectedIndex);
                return possibleDinos;
            }

            double baseValue;
            double incWild;
            double possibleLevel;
            bool possible;

            for (int i = 0; i < Values.V.species.Count; i++)
            {
                if (i == comboBoxSpeciesExtractor.SelectedIndex) continue; // the currently selected species is ignored here and set as top priority at the end

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

            if (comboBoxSpeciesExtractor.SelectedIndex >= 0)
                possibleDinos.Insert(0, comboBoxSpeciesExtractor.SelectedIndex); // adding the currently selected creature in the combobox as first priority. the user might already have that selected
            return possibleDinos;
        }

        private void checkBoxToggleOverlay_CheckedChanged(object sender, EventArgs e)
        {
            if (overlay == null)
            {
                overlay = new ARKOverlay();
                overlay.ExtractorForm = this;

                /*
                Process[] p = Process.GetProcessesByName("ShooterGame");
                if (p.Length > 0)
                    overlay.ARKProcess = p[0];
                */
            }

            overlay.Visible = checkBoxToggleOverlay.Checked;
            overlay.inventoryCheckTimer.Enabled = overlay.Visible;
            ArkOCR.OCR.calibrate(null);
        }

        private void toolStripButtonCopy2Tester_Click_1(object sender, EventArgs e)
        {
            cbbStatTestingSpecies.SelectedIndex = comboBoxSpeciesExtractor.SelectedIndex;
            double te = Extraction.E.uniqueTE();
            NumericUpDownTestingTE.Value = (decimal)(te >= 0 ? te * 100 : 80);
            numericUpDownImprintingBonusTester.Value = numericUpDownImprintingBonusExtractor.Value;
            checkBoxStatTestingBred.Checked = checkBoxAlreadyBred.Checked;
            if (!checkBoxWildTamedAuto.Checked)
                checkBoxStatTestingTamed.Checked = radioButtonTamed.Checked;
            for (int s = 0; s < 8; s++)
            {
                testingIOs[s].LevelWild = statIOs[s].LevelWild;
                testingIOs[s].LevelDom = statIOs[s].LevelDom;
                statIOUpdateValue(testingIOs[s]);
            }
            tabControlMain.SelectedTab = tabPageStatTesting;
            setTesterEditCreature();
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
            // copy values over to extractor
            for (int s = 0; s < 8; s++)
                statIOs[s].Input = testingIOs[s].Input;
            comboBoxSpeciesExtractor.SelectedIndex = cbbStatTestingSpecies.SelectedIndex;
            checkBoxAlreadyBred.Checked = checkBoxStatTestingBred.Checked;
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
                float[] wildLevels = new float[9];
                float[] tamedLevels = new float[9];
                Color[] colors = new Color[9];

                for (int i = 0; i < 8; i++)
                {
                    wildLevels[i + 1] = statIOs[i].LevelWild;
                    tamedLevels[i + 1] = statIOs[i].LevelDom;
                    colors[i + 1] = statIOs[i].BackColor;

                    if (i < 7)
                    {
                        wildLevels[0] += statIOs[i].LevelWild;
                        tamedLevels[0] += statIOs[i].LevelDom;
                    }
                }

                int speciesIndex = comboBoxSpeciesExtractor.SelectedIndex;

                string extraText = Values.V.speciesNames[speciesIndex];
                if (!Extraction.E.postTamed)
                {
                    string foodName = Values.V.species[speciesIndex].taming.eats[0];
                    int foodNeeded = Taming.foodAmountNeeded(speciesIndex, (int)wildLevels[0], foodName, Values.V.species[speciesIndex].taming.nonViolent);
                    List<int> foodAmountUsed;
                    bool enoughFood;
                    double te;
                    TimeSpan duration;
                    int narcotics, narcoBerries, bioToxines;
                    Taming.tamingTimes(speciesIndex, (int)wildLevels[0], new List<string>() { foodName }, new List<int>() { foodNeeded }, out foodAmountUsed, out duration, out narcoBerries, out narcotics, out bioToxines, out te, out enoughFood);
                    string foodNameDisplay = (foodName == "Kibble" ? Values.V.species[speciesIndex].taming.favoriteKibble + " Egg Kibble" : foodName);
                    extraText += "\nIt takes " + duration.ToString(@"hh\:mm\:ss") + " to tame the creature with " + foodNeeded + "×" + foodNameDisplay
                        + "\n" + narcoBerries + " Narcoberries or " + narcotics + " Narcotics or " + bioToxines + " Bio Toxines are needed"
                        + "\nTaming Effectiveness: " + Math.Round(100 * te, 1).ToString() + " % (+" + Math.Floor(wildLevels[0] * te / 2).ToString() + " lvl)";
                }

                overlay.setValues(wildLevels, tamedLevels, colors);
                overlay.setExtraText(extraText);
                int maxTime = (int)Values.V.species[speciesIndex].breeding.maturationTimeAdjusted;
                if (maxTime > 0)
                    overlay.setBreedingProgressValues(lastOCRValues[9] / lastOCRValues[5], maxTime); // current weight
                else
                    overlay.setBreedingProgressValues(1, 0); // 100% breeding time shows nothing
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
            if (checkBoxQuickWildCheck.Checked)
                tamingControl1.level = statIOs[7].LevelWild + 1;
            else
                tamingControl1.level = (int)numericUpDownLevel.Value;
            tamingControl1.selectedSpeciesIndex = comboBoxSpeciesExtractor.SelectedIndex;
            tabControlMain.SelectedTab = tabPageTaming;
        }

        private void labelImprintedCount_Click(object sender, EventArgs e)
        {
            // set imprinting-count to closes integer
            if (Values.V.species[cbbStatTestingSpecies.SelectedIndex].breeding != null && Values.V.species[cbbStatTestingSpecies.SelectedIndex].breeding.maturationTimeAdjusted > 0)
            {
                int cuddleCount = (int)Math.Round((double)numericUpDownImprintingBonusTester.Value * Values.V.species[cbbStatTestingSpecies.SelectedIndex].breeding.maturationTimeAdjusted / (1440000 * creatureCollection.babyCuddleIntervalMultiplier));
                double imprintingBonus;
                do
                {
                    imprintingBonus = Math.Round(cuddleCount * 1440000 * creatureCollection.babyCuddleIntervalMultiplier / Values.V.species[cbbStatTestingSpecies.SelectedIndex].breeding.maturationTimeAdjusted, 3);
                    cuddleCount--;
                } while (imprintingBonus > 100);
                numericUpDownImprintingBonusTester.Value = (decimal)imprintingBonus;
            }
        }

        private void toolStripButtonAddPlayer_Click(object sender, EventArgs e)
        {
            tribesControl1.addPlayer();
        }
    }
}
