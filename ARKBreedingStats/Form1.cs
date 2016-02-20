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

namespace ARKBreedingStats
{
    public partial class Form1 : Form
    {
        private CreatureCollection creatureCollection = new CreatureCollection();
        private String currentFileName = "";
        private bool collectionDirty = false;
        private ListViewColumnSorter lvwColumnSorter; // used for sorting columns in the listview
        private List<string> creatureNames = new List<string>();
        private string[] statNames = new string[] { "Health", "Stamina", "Oxygen", "Food", "Weight", "Damage", "Speed", "Torpor" };
        //private List<List<double[]>> stats = new List<List<double[]>>();
        private List<List<CreatureStat>> stats = new List<List<CreatureStat>>();
        private List<int> levelXP = new List<int>();
        private List<StatIO> statIOs = new List<StatIO>();
        private List<StatIO> testingIOs = new List<StatIO>();
        private List<List<double[]>> results = new List<List<double[]>>(); // stores the possible results of all stats as array (wildlevel, domlevel, tamingEff)
        private int cC = 0; // current creature
        private bool postTamed = false;
        private int activeStat = -1;
        private List<int> statsWithEff = new List<int>();
        private List<int> chosenResults = new List<int>();
        private int[] precisions = new int[] { 1, 1, 1, 1, 1, 3, 3, 1 }; // damage and speed are percentagevalues, need more precision
        private int[] levelDomFromTorporAndTotalRange = new int[] { 0, 0 }, levelWildFromTorporRange = new int[] { 0, 0 }; // 0: min, 1: max
        private bool[] activeStats = new bool[] { true, true, true, true, true, true, true, true };
        private List<Creature> creatures = new List<Creature>();
        private List<CreatureBox> creatureBoxes = new List<CreatureBox>();
        private int localFileVer = 0;

        public Form1()
        {
            InitializeComponent();

            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            lvwColumnSorter = new ListViewColumnSorter();
            this.listViewLibrary.ListViewItemSorter = lvwColumnSorter;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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
            for (int s = 0; s < statNames.Length; s++)
            {
                statIOs[s].Title = statNames[s];
                testingIOs[s].Title = statNames[s];
                if (precisions[s] == 3) { statIOs[s].Percent = true; testingIOs[s].Percent = true; }
            }
            statIOTorpor.ShowBar = false; // torpor should not show bar, it get's too wide and is not interesting for breeding
            statTestingTorpor.ShowBar = false;
            labelSumDomSB.Text = "";
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.checkBoxOutputRowHeader, "Include Headerrow");
            tt.SetToolTip(this.checkBoxJustTamed, "Check this if there was no server-restart or if you didn't logout since you tamed the creature.\nUncheck this if you know there was a server-restart (many servers restart every night).\nIf it is some days ago (IRL) you tamed the creature you should probably uncheck this checkbox.");
            tt.SetToolTip(checkBoxWildTamedAuto, "For most creatures the tool recognizes if they are wild or tamed.\nFor Giganotosaurus and maybe if you have custom server-settings you have to select manually if the creature is wild or tamed.");
            loadStatFile(true);
            if (creatureNames.Count > 0)
            {
                comboBoxCreatures.SelectedIndex = 0;
                cbbStatTestingRace.SelectedIndex = 0;
                comboBoxCreaturesLib.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Creatures-File could not be loaded.", "Error");
                Close();
            }
            comboBoxStatsLib.SelectedIndex = 0;
            // insert debug values. TODO: remove before release. It's only here to insert some working numbers to extract
            statIOs[0].Input = 265.7;
            statIOs[1].Input = 432;
            statIOs[2].Input = 253.5;
            statIOs[3].Input = 2704.8;
            statIOs[4].Input = 153;
            statIOs[5].Input = 186.6;
            statIOs[6].Input = 160.4;
            statIOs[7].Input = 293.3;
            numericUpDownLevel.Value = 48;
            comboBoxCreatures.SelectedIndex = 33;

            // load last save file:
            if (Properties.Settings.Default.LastSaveFile != "")
                loadCollectionFile(Properties.Settings.Default.LastSaveFile);
        }

        private void clearAll()
        {
            results.Clear();
            statsWithEff.Clear();
            listBoxPossibilities.Items.Clear();
            chosenResults.Clear();
            for (int s = 0; s < 8; s++)
            {
                statIOs[s].Clear();
                chosenResults.Add(0);
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
            buttonCopyClipboard.Enabled = false;
            buttonAdd2Library.Enabled = false;
            activeStat = -1;
            labelTE.Text = "Extracted: n/a";
            labelSumDom.Text = "";
            labelSumWild.Text = "";
            labelSumWildSB.Text = "";
            for (int i = 0; i < 2; i++)
            {
                levelWildFromTorporRange[i] = 0;
                levelDomFromTorporAndTotalRange[i] = 0;
            }
        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            int activeStatKeeper = activeStat;
            clearAll();
            bool resultsValid = true;
            if (checkBoxWildTamedAuto.Checked)
            {
                // torpor is directly proportional to wild level. Check if creature is wild or tamed (doesn't work with Giganotosaurus because it has no additional bonus on torpor)
                postTamed = (stats[cC][7].BaseValue + stats[cC][7].BaseValue * stats[cC][7].IncPerWildLevel * Math.Round((statIOs[7].Input - stats[cC][7].BaseValue) / (stats[cC][7].BaseValue * stats[cC][7].IncPerWildLevel)) != statIOs[7].Input);
            }
            else
            {
                postTamed = radioButtonTamed.Checked;
            }
            // max level for wild according to torpor (possible bug ingame: torpor is depending on taming efficiency 5/3 - 2 times "too high" for level after taming until server-restart (not only the bonus levels are added, but also the existing levels again)
            double torporLevelTamingMultMax = 1, torporLevelTamingMultMin = 1;
            if (postTamed && this.checkBoxJustTamed.Checked)
            {
                torporLevelTamingMultMax = (200 + (double)this.numericUpDownUpperTEffBound.Value) / (400 + (double)this.numericUpDownUpperTEffBound.Value);
                torporLevelTamingMultMin = (200 + (double)this.numericUpDownLowerTEffBound.Value) / (400 + (double)this.numericUpDownLowerTEffBound.Value);
            }
            levelWildFromTorporRange[0] = (int)Math.Round((statIOs[7].Input - (postTamed ? stats[cC][7].AddWhenTamed : 0) - stats[cC][7].BaseValue) * torporLevelTamingMultMin / (stats[cC][7].BaseValue * stats[cC][7].IncPerWildLevel), 0);
            levelWildFromTorporRange[1] = (int)Math.Round((statIOs[7].Input - (postTamed ? stats[cC][7].AddWhenTamed : 0) - stats[cC][7].BaseValue) * torporLevelTamingMultMax / (stats[cC][7].BaseValue * stats[cC][7].IncPerWildLevel), 0);
            int[] levelDomRange = new int[] { 0, 0 };
            // lower/upper Bound of each stat (wild has no upper bound as wild-speed is unknown)
            if (postTamed)
            {
                for (int i = 0; i < 2; i++)
                {
                    levelDomRange[i] = (int)numericUpDownLevel.Value - levelWildFromTorporRange[1 - i] - 1; // creatures starts with level 1
                }
            }
            for (int i = 0; i < 2; i++) { levelDomFromTorporAndTotalRange[i] = levelDomRange[i]; }

            for (int s = 0; s < 8; s++)
            {
                results.Add(new List<double[]>());
                if (activeStats[s])
                {
                    statIOs[s].PostTame = postTamed;
                    double inputValue = statIOs[s].Input / (precisions[s] == 3 ? 100 : 1);
                    double tamingEfficiency = -1, tEUpperBound = (double)this.numericUpDownUpperTEffBound.Value / 100, tELowerBound = (double)this.numericUpDownLowerTEffBound.Value / 100;
                    double vWildL = 0; // value with only wild levels
                    if (checkBoxAlreadyBred.Checked)
                    {
                        // bred creatures always have 100% TE
                        tEUpperBound = 1;
                        tELowerBound = 1;
                    }
                    bool withTEff = (postTamed && stats[cC][s].MultAffinity > 0);
                    if (withTEff) { statsWithEff.Add(s); }
                    double maxLW = 0;
                    if (stats[cC][s].BaseValue > 0 && stats[cC][s].IncPerWildLevel > 0)
                    {
                        maxLW = Math.Round(((inputValue / (postTamed ? 1 + tELowerBound * stats[cC][s].MultAffinity : 1) - (postTamed ? stats[cC][s].AddWhenTamed : 0)) / stats[cC][s].BaseValue - 1) / stats[cC][s].IncPerWildLevel); // floor is too unprecise
                    }
                    if (s != 7 && maxLW > levelWildFromTorporRange[1]) { maxLW = levelWildFromTorporRange[1]; } // torpor level can be too high right after taming (bug ingame?)

                    double maxLD = 0;
                    if (stats[cC][s].BaseValue > 0 && stats[cC][s].IncPerTamedLevel > 0 && postTamed)
                    {
                        maxLD = Math.Round((inputValue / ((stats[cC][s].BaseValue + stats[cC][s].AddWhenTamed) * (1 + tELowerBound * stats[cC][s].MultAffinity)) - 1) / stats[cC][s].IncPerTamedLevel); //floor is sometimes too unprecise
                    }
                    if (maxLD > levelDomRange[1]) { maxLD = levelDomRange[1]; }

                    for (int w = 0; w < maxLW + 1; w++)
                    {
                        vWildL = stats[cC][s].BaseValue + stats[cC][s].BaseValue * stats[cC][s].IncPerWildLevel * w + (postTamed ? stats[cC][s].AddWhenTamed : 0);
                        for (int d = 0; d < maxLD + 1; d++)
                        {
                            if (withTEff)
                            {
                                // taming bonus is dependant on taming-efficiency
                                // get tamingEfficiency-possibility
                                // rounding errors need to increase error-range
                                tamingEfficiency = Math.Round((inputValue / (1 + stats[cC][s].IncPerTamedLevel * d) - vWildL) / (vWildL * stats[cC][s].MultAffinity), 3, MidpointRounding.AwayFromZero);
                                if (tamingEfficiency > 1 && tamingEfficiency < 1.005) { tamingEfficiency = 1; }
                                if (tamingEfficiency >= tELowerBound - 0.005)
                                {
                                    if (tamingEfficiency <= tEUpperBound)
                                    {
                                        results[s].Add(new double[] { w, d, tamingEfficiency });
                                    }
                                    else { continue; }
                                }
                                else
                                {
                                    // if tamingEff < lowerBound, break, as in this loop it's getting only smaller
                                    break;
                                }
                            }
                            else if (Math.Abs((vWildL + vWildL * stats[cC][s].IncPerTamedLevel * d - inputValue) * (precisions[s] == 3 ? 100 : 1)) < 0.2)
                            {
                                results[s].Add(new double[] { w, d, tamingEfficiency });
                                break; // no other solution possible
                            }
                        }
                    }
                }
                else
                {
                    results[s].Add(new double[] { 0, 0, -1 });
                }
            }
            int maxLW2 = levelWildFromTorporRange[1];
            int[] lowerBoundExtraWs = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            int[] lowerBoundExtraDs = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            int[] upperBoundExtraDs = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            // substract all uniquely solved stat-levels
            for (int s = 0; s < 7; s++)
            {
                if (results[s].Count == 1)
                {
                    // result is uniquely solved
                    maxLW2 -= (int)results[s][0][0];
                    levelDomRange[0] -= (int)results[s][0][1];
                    levelDomRange[1] -= (int)results[s][0][1];
                    upperBoundExtraDs[s] = (int)results[s][0][1];
                }
                else if (results[s].Count > 1)
                {
                    // get the smallest and larges value
                    int minW = (int)results[s][0][0], minD = (int)results[s][0][1], maxD = (int)results[s][0][1];
                    for (int r = 1; r < results[s].Count; r++)
                    {
                        if (results[s][r][0] < minW) { minW = (int)results[s][r][0]; }
                        if (results[s][r][1] < minD) { minD = (int)results[s][r][1]; }
                        if (results[s][r][1] > maxD) { maxD = (int)results[s][r][1]; }
                    }
                    // save min/max-possible value
                    lowerBoundExtraWs[s] = minW;
                    lowerBoundExtraDs[s] = minD;
                    upperBoundExtraDs[s] = maxD;
                }
            }
            if (maxLW2 < lowerBoundExtraWs.Sum() || levelDomRange[1] < lowerBoundExtraDs.Sum())
            {
                this.numericUpDownLevel.BackColor = Color.LightSalmon;
                if (!checkBoxAlreadyBred.Checked && this.numericUpDownLowerTEffBound.Value > 0)
                {
                    this.numericUpDownLowerTEffBound.BackColor = Color.LightSalmon;
                }
                if (!checkBoxAlreadyBred.Checked && this.numericUpDownUpperTEffBound.Value < 100)
                {
                    this.numericUpDownUpperTEffBound.BackColor = Color.LightSalmon;
                }
                this.checkBoxAlreadyBred.BackColor = Color.LightSalmon;
                this.checkBoxJustTamed.BackColor = Color.LightSalmon;
                panelWildTamedAuto.BackColor = Color.LightSalmon;
                results.Clear();
                resultsValid = false;
            }
            else
            {
                // remove all results that are violate restrictions
                for (int s = 0; s < 7; s++)
                {
                    for (int r = 0; r < results[s].Count; r++)
                    {
                        if (results[s].Count > 1 && (results[s][r][0] > maxLW2 - lowerBoundExtraWs.Sum() + lowerBoundExtraWs[s] || results[s][r][1] > levelDomRange[1] - lowerBoundExtraDs.Sum() + lowerBoundExtraDs[s] || results[s][r][1] < levelDomRange[0] - upperBoundExtraDs.Sum() + upperBoundExtraDs[s]))
                        {
                            results[s].RemoveAt(r--);
                            // if result gets unique due to this, check if remaining result doesn't violate for max level
                            if (results[s].Count == 1)
                            {
                                maxLW2 -= (int)results[s][0][0];
                                levelDomRange[0] -= (int)results[s][0][1];
                                levelDomRange[1] -= (int)results[s][0][1];
                                lowerBoundExtraWs[s] = 0;
                                lowerBoundExtraDs[s] = 0;
                                if (maxLW2 < 0 || levelDomRange[1] < 0)
                                {
                                    this.numericUpDownLevel.BackColor = Color.LightSalmon;
                                    statIOs[s].Status = -2;
                                    statIOs[7].Status = -2;
                                    results[s].Clear();
                                    resultsValid = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                // if more than one parameter is affected by tamingEfficiency filter all numbers that occure only in one
                if (statsWithEff.Count > 1)
                {
                    for (int es = 0; es < statsWithEff.Count; es++)
                    {
                        for (int et = es + 1; et < statsWithEff.Count; et++)
                        {
                            List<int> equalEffs1 = new List<int>();
                            List<int> equalEffs2 = new List<int>();
                            for (int ere = 0; ere < results[statsWithEff[es]].Count; ere++)
                            {
                                for (int erf = 0; erf < results[statsWithEff[et]].Count; erf++)
                                {
                                    // efficiency-calculation can be a bit off due to rounding-ingame, so treat them as equal when diff<0.002
                                    if (Math.Abs(results[statsWithEff[es]][ere][2] - results[statsWithEff[et]][erf][2]) < 0.002)
                                    {
                                        // if entry is not yet in whitelist, add it
                                        if (equalEffs1.IndexOf(ere) == -1) { equalEffs1.Add(ere); }
                                        if (equalEffs2.IndexOf(erf) == -1) { equalEffs2.Add(erf); }
                                    }
                                }
                            }
                            // copy all results that have an efficiency that occurs more than once and replace the others
                            List<double[]> validResults1 = new List<double[]>();
                            for (int ev = 0; ev < equalEffs1.Count; ev++)
                            {
                                validResults1.Add(results[statsWithEff[es]][equalEffs1[ev]]);
                            }
                            // replace long list with (hopefully) shorter list with valid entries
                            results[statsWithEff[es]] = validResults1;
                            List<double[]> validResults2 = new List<double[]>();
                            for (int ev = 0; ev < equalEffs2.Count; ev++)
                            {
                                validResults2.Add(results[statsWithEff[et]][equalEffs2[ev]]);
                            }
                            results[statsWithEff[et]] = validResults2;
                        }
                        if (es >= statsWithEff.Count - 2)
                        {
                            // only one stat left, not enough to compare it
                            break;
                        }
                    }
                }
                for (int s = 0; s < 8; s++)
                {
                    if (results[s].Count > 0)
                    {
                        // display result with most levels in wild, for hp and dm with the most levels in tamed
                        int r = 0;
                        if (s != 0 && s != 5) { r = results[s].Count - 1; }
                        setPossibility(s, r);
                        if (results[s].Count > 1)
                        {
                            statIOs[s].Status = -1;
                        }
                        else { statIOs[s].Status = 1; }
                    }
                    else
                    {
                        statIOs[s].Status = -2;
                        results[s].Clear();
                        resultsValid = false;
                        if (!checkBoxAlreadyBred.Checked && statsWithEff.IndexOf(s) >= 0 && this.numericUpDownLowerTEffBound.Value > 0)
                        {
                            this.numericUpDownLowerTEffBound.BackColor = Color.LightSalmon;
                        }
                        if (!checkBoxAlreadyBred.Checked && statsWithEff.IndexOf(s) >= 0 && this.numericUpDownUpperTEffBound.Value < 100)
                        {
                            this.numericUpDownUpperTEffBound.BackColor = Color.LightSalmon;
                        }
                        this.checkBoxAlreadyBred.BackColor = Color.LightSalmon;
                        this.checkBoxJustTamed.BackColor = Color.LightSalmon;
                    }
                }
            }
            if (resultsValid)
            {
                setSpeedLevelAccordingToOthers();
                buttonCopyClipboard.Enabled = true;
                buttonAdd2Library.Enabled = true;
                setActiveStat(activeStatKeeper);
                if (postTamed) { setUniqueTE(); }
                else
                {
                    labelTE.Text = "not yet tamed";
                    labelTE.BackColor = System.Drawing.Color.Transparent;
                }
                showSumOfChosenLevels();
                labelSumWildSB.Text = "≤" + levelWildFromTorporRange[1].ToString();
                labelSumDomSB.Text = (levelDomFromTorporAndTotalRange[0] != levelDomFromTorporAndTotalRange[1] ? levelDomFromTorporAndTotalRange[0].ToString() + "-" : "") + levelDomFromTorporAndTotalRange[1].ToString();
            }
            if (!postTamed)
            {
                labelFootnote.Text = "*Creature is not yet tamed and may get better values then.";
            }
        }

        private void setUniqueTE()
        {
            double eff = uniqueTE();
            if (eff >= 0)
            {
                labelTE.Text = "Extracted: " + Math.Round(100 * eff, 1) + " %";
                labelTE.BackColor = System.Drawing.Color.Transparent;
            }
            else
            {
                labelTE.Text = "TE differs in chosen possibilities";
                labelTE.BackColor = Color.LightSalmon;
            }
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
            this.listBoxPossibilities.Items.Clear();
            for (int s = 0; s < 8; s++)
            {
                if (s == stat && statIOs[s].Status == -1)
                {
                    statIOs[s].Selected = true;
                    activeStat = s;
                    setPossibilitiesListbox(s);
                }
                else
                {
                    statIOs[s].Selected = false;
                }
            }
        }

        // fill listbox with possible results of stat
        private void setPossibilitiesListbox(int s)
        {
            if (s < results.Count)
            {
                for (int r = 0; r < results[s].Count; r++)
                {
                    this.listBoxPossibilities.Items.Add(results[s][r][0].ToString() + "\t" + results[s][r][1].ToString() + (results[s][r][2] >= 0 ? "\t" + (results[s][r][2] * 100).ToString() + "%" : ""));
                }
            }
        }

        private bool loadStatFile(bool loadSettings)
        {
            string path = "";
            if (loadSettings)
            {
                // read settings from file
                path = "settings.txt";

                // check if file exists
                if (System.IO.File.Exists(path))
                {
                    string[] rows;
                    rows = System.IO.File.ReadAllLines(path);
                    string[] values;
                    int s = 0;
                    double value = 0;
                    foreach (string row in rows)
                    {
                        if (row.Length > 1 && row.Substring(0, 2) != "//")
                        {

                            if (row.Substring(0, 1) == "!")
                            {
                                if (!Int32.TryParse(row.Substring(1), out localFileVer))
                                {
                                    localFileVer = 0; // file-version unknown
                                }
                            }
                            else
                            {
                                values = row.Split(',');
                                if (values.Length == 3)
                                {
                                    value = 0;
                                    if (Double.TryParse(values[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value))
                                    {
                                        statIOs[s].MultAdd = value;
                                        testingIOs[s].MultAdd = value;
                                    }
                                    value = 0;
                                    if (Double.TryParse(values[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value))
                                    {
                                        statIOs[s].MultAff = value;
                                        testingIOs[s].MultAff = value;
                                    }
                                    value = 0;
                                    if (Double.TryParse(values[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value))
                                    {
                                        statIOs[s].MultLevel = value;
                                        testingIOs[s].MultLevel = value;
                                    }
                                    s++;
                                }
                            }
                        }
                    }
                }
            }

            // read entities from file
            path = "stats.txt";

            // check if file exists
            if (!System.IO.File.Exists(path))
            {
                MessageBox.Show("Creatures-File '" + path + "' not found.", "Error");
                Close();
            }
            else
            {
                string[] rows;
                rows = System.IO.File.ReadAllLines(path);
                string[] values;
                int c = -1;
                int s = 0;
                comboBoxCreatures.Items.Clear();
                stats.Clear();
                foreach (string row in rows)
                {
                    if (row.Length > 1 && row.Substring(0, 2) != "//")
                    {
                        values = row.Split(',');
                        if (values.Length == 1)
                        {
                            // new creature
                            //List<double[]> cs = new List<double[]>();
                            List<CreatureStat> css = new List<CreatureStat>();
                            for (s = 0; s < 8; s++)
                            {
                                //cs.Add(new double[] { 0, 0, 0, 0, 0 });
                                css.Add(new CreatureStat((StatName)s));
                            }
                            s = 0;
                            //stats.Add(cs);
                            stats.Add(css);
                            string creatureName = values[0].Trim();
                            creatureNames.Add(creatureName);
                            this.comboBoxCreatures.Items.Add(creatureName);
                            this.cbbStatTestingRace.Items.Add(creatureName);
                            this.comboBoxCreaturesLib.Items.Add(creatureName);
                            c++;
                        }
                        else if (values.Length > 1 && values.Length < 6)
                        {
                            double[] stat = new double[values.Length];
                            for (int v = 0; v < values.Length; v++)
                            {
                                if ((s == 5 || s == 6) && v == 0)
                                {
                                    stat[0] = 1;
                                } // damage and speed are handled as percentage of a hidden base value, this tool uses 100% as base, as seen ingame
                                else
                                {
                                    double value = 0;
                                    if (Double.TryParse(values[v], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value))
                                    {
                                        switch (v)
                                        {
                                            case 2:
                                                value *= statIOs[s].MultLevel; // apply multipliers of settings.txt to values
                                                break;
                                            case 3:
                                                if (value > 0) // don't apply if MultAdd is negative (currently the only case is the Giganotosaurus, which does not get the subtraction multiplied)
                                                {
                                                    value *= statIOs[s].MultAdd;
                                                }
                                                break;
                                            case 4:
                                                value *= statIOs[s].MultAff;
                                                break;
                                        }
                                        stat[v] = value;
                                    }
                                }
                            }
                            stats[c][s].setValues(stat);
                            s++;
                        }
                    }
                }
            }
            return true;
        }

        private void comboBoxCreatures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCreatures.SelectedIndex >= 0)
            {
                cC = comboBoxCreatures.SelectedIndex;
                for (int s = 0; s < 8; s++)
                {
                    activeStats[s] = (stats[cC][s].BaseValue > 0);
                    statIOs[s].Enabled = activeStats[s];
                }
                clearAll();
            }
        }

        private void listBoxPossibilities_MouseClick(object sender, MouseEventArgs e)
        {
            int index = this.listBoxPossibilities.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches && activeStat >= 0)
            {
                setPossibility(activeStat, index);
            }
        }

        private void setPossibility(int s, int i)
        {
            statIOs[s].LevelWild = (Int32)results[s][i][0];
            statIOs[s].LevelDom = (Int32)results[s][i][1];
            statIOs[s].TamingEfficiency = (Int32)results[s][i][2];
            statIOs[s].BreedingValue = breedingValue(s, i);
            chosenResults[s] = i;
            setUniqueTE();
            showSumOfChosenLevels();
            setSpeedLevelAccordingToOthers();
        }

        private void setSpeedLevelAccordingToOthers()
        {
            /*
             * Regardless of anything else, wild speed level is always current level - (wild levels + dom levels)
             */
            int totalLevels = (int)numericUpDownLevel.Value;
            int sumLevels = 0;
            for (int c = 0; c < statIOs.Count - 1; c++)
            {
                sumLevels += statIOs[c].LevelDom;
                sumLevels += (c == 6 ? 0 : statIOs[c].LevelWild);
            }
            statIOs[6].LevelWild = Math.Max(0, totalLevels - sumLevels) - 1;
        }

        private void buttonCopyClipboard_Click(object sender, EventArgs e)
        {
            if (results.Count == 8 && chosenResults.Count == 8)
            {
                List<string> tsv = new List<string>();
                string rowLevel = comboBoxCreatures.SelectedItem.ToString() + "\t\t", rowValues = "";
                // if taming efficiency is unique, display it, too
                string effString = "";
                double eff = uniqueTE();
                if (eff >= 0)
                {
                    effString = "\tTamingEff:\t" + (100 * eff).ToString() + "%";
                }
                // headerrow
                if (radioButtonOutputTable.Checked || checkBoxOutputRowHeader.Checked)
                {
                    if (radioButtonOutputTable.Checked)
                    {
                        tsv.Add(comboBoxCreatures.SelectedItem.ToString() + "\tLevel " + numericUpDownLevel.Value.ToString() + effString);
                        tsv.Add("Stat\tWildLevel\tDomLevel\tBreedingValue");
                    }
                    else { tsv.Add("Species\tName\tSex\tHP-Level\tSt-Level\tOx-Level\tFo-Level\tWe-Level\tDm-Level\tSp-Level\tTo-Level\tHP-Value\tSt-Value\tOx-Value\tFo-Value\tWe-Value\tDm-Value\tSp-Value\tTo-Value"); }
                }
                for (int s = 0; s < 8; s++)
                {
                    if (chosenResults[s] < results[s].Count)
                    {
                        string breedingV = "";
                        if (activeStats[s])
                        {
                            breedingV = statIOs[s].BreedingValue.ToString();
                        }
                        if (radioButtonOutputTable.Checked)
                        {
                            tsv.Add(statNames[s] + "\t" + (activeStats[s] ? statIOs[cC].LevelWild.ToString() : "") + "\t" + (activeStats[s] ? statIOs[cC].LevelWild.ToString() : "") + "\t" + breedingV);
                        }
                        else
                        {
                            rowLevel += "\t" + (activeStats[s] ? statIOs[cC].LevelWild.ToString() : "");
                            rowValues += "\t" + breedingV;
                        }
                    }
                    else { return; }
                }
                if (radioButtonOutputRow.Checked) { tsv.Add(rowLevel + rowValues); }
                Clipboard.SetText(string.Join("\n", tsv));
            }
        }

        private double uniqueTE()
        {
            if (statsWithEff.Count > 0 && results[statsWithEff[0]].Count > chosenResults[statsWithEff[0]])
            {
                double eff = results[statsWithEff[0]][chosenResults[statsWithEff[0]]][2];
                for (int st = 1; st < statsWithEff.Count; st++)
                {
                    // efficiency-calculation can be a bit off due to ingame-rounding
                    if (results[statsWithEff[st]].Count <= chosenResults[statsWithEff[st]] || Math.Abs(results[statsWithEff[st]][chosenResults[statsWithEff[st]]][2] - eff) > 0.002)
                    {
                        return -1;
                    }
                }
                return eff;
            }
            return -1;
        }

        private double breedingValue(int s, int r)
        {
            if (s >= 0 && s < 8)
            {
                if (r >= 0 && r < results[s].Count)
                {
                    return calculateValue(cC, s, (int)results[s][r][0], 0, true, results[s][r][2]);
                }
            }
            return -1;
        }

        private double calculateValue(int creature, int stat, int levelWild, int levelDom, bool dom, double tamingEff)
        {
            double add = 0, domMult = 1;
            if (dom)
            {
                add = stats[cC][stat].AddWhenTamed;
                domMult = (tamingEff >= 0 ? (1 + stats[creature][stat].MultAffinity) : 1) * (1 + levelDom * stats[creature][stat].IncPerTamedLevel);
            }
            return Math.Round((stats[creature][stat].BaseValue * (1 + stats[creature][stat].IncPerWildLevel * levelWild) + add) * domMult, precisions[stat], MidpointRounding.AwayFromZero);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/cadon/ARKStatsExtractor");
        }

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            if (n != null)
            {
                n.Select(0, n.Text.Length);
            }
        }

        private void radioButtonOutputRow_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBoxOutputRowHeader.Enabled = radioButtonOutputRow.Checked;
        }

        private void checkBoxAlreadyBred_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxTE.Enabled = !checkBoxAlreadyBred.Checked;
            checkBoxJustTamed.Checked = checkBoxJustTamed.Checked && !checkBoxAlreadyBred.Checked;
            panelWildTamedAuto.Enabled = !checkBoxAlreadyBred.Checked;
        }

        private void checkBoxJustTamed_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxAlreadyBred.Checked = checkBoxAlreadyBred.Checked && !checkBoxJustTamed.Checked;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            clearAll();
            numericUpDownLevel.Value = 1;
        }

        private void checkBoxSettings_CheckedChanged(object sender, EventArgs e)
        {
            this.SuspendLayout();
            bool t = checkBoxSettings.Checked;
            for (int s = 0; s < 8; s++)
            {
                statIOs[s].Settings = t;
            }
            checkBoxSettings.Text = (t ? "OK" : "Settings");
            if (!t)
            {
                // save settings to file
                string path = "settings.txt";
                string[] content = new string[9];
                content[0] = "// csv of multiplicators: MultAdd,MultAffinity,MultLevel. Order of stats (one per row): Health, Stamina, Oxygen, Food, Weight, Damage, Speed, Torpor";
                for (int s = 0; s < 8; s++)
                {
                    content[s + 1] = statIOs[s].MultAdd.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + statIOs[s].MultAff.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + statIOs[s].MultLevel.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }

                System.IO.File.WriteAllLines(path, content);

                // update stats according to settings
                loadStatFile(false);
            }
            this.ResumeLayout();
        }

        private void checkBoxWildTamedAuto_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonTamed.Enabled = !checkBoxWildTamedAuto.Checked;
            radioButtonWild.Enabled = !checkBoxWildTamedAuto.Checked;
        }

        private void checkForUpdatedStatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void loadUpdatedStats()
        {
            if (MessageBox.Show("Do you want to check for a new version of the stats-file?\nYour current stat-file will be overwritten, consider creating a backup if you changed it manually.", "Update stat-file?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string remoteUri = "https://raw.githubusercontent.com/cadon/ARKStatsExtractor/master/ARKBreedingStats/";
                    string fileName = "stats.txt";
                    // Create a new WebClient instance.
                    System.Net.WebClient myWebClient = new System.Net.WebClient();
                    // Download the Web resource and save it into the current filesystem folder.
                    myWebClient.DownloadFile(remoteUri + fileName, fileName);
                    // load new settings
                    loadStatFile(false);
                    MessageBox.Show("Download and update successful");
                }
                catch
                {
                    MessageBox.Show("Error while trying to check or download new stats", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonAdd2Library_Click(object sender, EventArgs e)
        {
            Creature creature = new Creature(creatureNames[cC], "Bob", 0, getCurrentWildLevels(), getCurrentDomLevels(), uniqueTE(), getCurrentBreedingValues(), getCurrentDomValues());
            CreatureBox cb = new CreatureBox(creature);
            flowLayoutPanelCreatures.Controls.Add(cb);
            flowLayoutPanelCreatures.Controls.SetChildIndex(cb, 0); // move to the top of the heap
            creatureBoxes.Add(cb);
            tabControl1.SelectedIndex = 2;
            creatureCollection.creatures.Add(creature);
            cb.buttonEdit_Click(sender, e);
            collectionDirty = true;
        }

        private int[] getCurrentWildLevels()
        {
            int[] levelsWild = new int[8];
            for (int s = 0; s < 8; s++)
            {
                levelsWild[s] = statIOs[s].LevelWild;
            }
            return levelsWild;
        }
        private int[] getCurrentDomLevels()
        {
            int[] levelsDom = new int[8];
            for (int s = 0; s < 8; s++) { levelsDom[s] = statIOs[s].LevelDom; }
            return levelsDom;
        }
        private double[] getCurrentBreedingValues()
        {
            double[] valuesBreeding = new double[8];
            for (int s = 0; s < 8; s++) { valuesBreeding[s] = statIOs[s].BreedingValue; }
            return valuesBreeding;
        }
        private double[] getCurrentDomValues()
        {
            double[] valuesBreeding = new double[8];
            for (int s = 0; s < 8; s++) { valuesBreeding[s] = calculateValue(cC, s, (int)results[s][chosenResults[s]][0], (int)results[s][chosenResults[s]][1], true, results[s][chosenResults[s]][2]); }
            return valuesBreeding;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.Show();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (collectionDirty == true)
            {
                if (MessageBox.Show("Your Creature Collection has been modified since it was last saved, are you sure you want to load without saving first?", "Discard Changes?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    return;
            }
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Creature Collection File (*.xml)|*.xml";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                loadCollectionFile(dlg.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (currentFileName == "")
                saveAsToolStripMenuItem_Click(sender, e);
            else
            {
                saveCollectionToFileName(currentFileName);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Creature Collection File (*.xml)|*.xml";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                saveCollectionToFileName(dlg.FileName);
                currentFileName = dlg.FileName;
            }
        }

        private void saveCollectionToFileName(String fileName)
        {
            XmlSerializer writer = new XmlSerializer(typeof(CreatureCollection));
            System.IO.FileStream file = System.IO.File.Create(fileName);
            try
            {
                writer.Serialize(file, creatureCollection);
                Properties.Settings.Default.LastSaveFile = fileName;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error during serialization.\nErrormessage:\n\n" + e.Message);
                // TODO handle serialization problems.
            }
            file.Close();
        }

        private void loadCollectionFile(String fileName)
        {
            XmlSerializer reader = new XmlSerializer(typeof(CreatureCollection));

            if (!System.IO.File.Exists(fileName))
            {
                MessageBox.Show("Save file with name \"" + fileName + "\" does not exist!");
                return;
            }

            System.IO.FileStream file = System.IO.File.OpenRead(fileName);
            try
            {
                creatureCollection = (CreatureCollection)reader.Deserialize(file);
                currentFileName = fileName;
                collectionDirty = false;
                DetermineParentsToBreed();
                showTheseCreatures(creatureCollection.creatures, true);
                Properties.Settings.Default.LastSaveFile = fileName;
            }
            catch (Exception e)
            {
                MessageBox.Show("File Couldn't be opened, we thought you should know.\nErrormessage:\n\n" + e.Message);
                //TODO: handle serialization errors
            }

        }

        private void showCreaturesInTreeview(List<Creature> creatures)
        {
            foreach (Creature cr in creatures)
            {
                TreeNode[] r = treeViewCreatureLib.Nodes.Find(cr.species, false);
                if (r.Length == 0)
                {
                    // add new node alphabetically
                    int nn = 0;
                    while (nn < treeViewCreatureLib.Nodes.Count && String.Compare(treeViewCreatureLib.Nodes[nn].Text, cr.species, true) < 0) { nn++; }
                    treeViewCreatureLib.Nodes.Insert(nn, cr.species);
                    treeViewCreatureLib.Nodes[nn].Name = cr.species;
                    r = treeViewCreatureLib.Nodes.Find(cr.species, false);
                }
                if (r.Length > 0)
                {
                    r[0].Nodes.Add(cr.name);
                }

                ////// add to listView
                // check if group of species exists
                ListViewGroup g = null;
                foreach (ListViewGroup lvg in listViewLibrary.Groups)
                {
                    if (lvg.Header == cr.species)
                    {
                        g = lvg;
                    }
                }
                if (g == null)
                {
                    g = new ListViewGroup(cr.species);
                    listViewLibrary.Groups.Add(g);
                }
                string[] subItems = (new string[] { cr.name, cr.gender.ToString().Substring(0, 1) }).Concat(cr.levelsWild.Select(x => x.ToString()).ToArray()).ToArray();
                ListViewItem lvi = new ListViewItem(subItems, g);
                for (int s = 0; s < 7; s++)
                {
                    lvi.SubItems[s + 2].BackColor = Utils.getColorFromPercent((int)(cr.levelsWild[s] * 2.5), (cr.topBreedingStats[s] ? 0 : 0.7));
                }
                lvi.UseItemStyleForSubItems = false;
                if (cr.isTopCreature) lvi.BackColor = Color.LightGreen;
                lvi.Tag = cr;
                listViewLibrary.Items.Add(lvi);
            }
        }

        private void checkForUpdatedStatsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to check for a new version of the stats.txt- and settings.txt-file?\nYour current files will be backuped.\n\nIf your stats are outdated and no new version is available, we probably don't have the new ones either.", "Update stat-files?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string remoteUri = "https://github.com/cadon/ARKStatsExtractor/raw/master/ARKBreedingStats/";
                    // Create a new WebClient instance.
                    System.Net.WebClient myWebClient = new System.Net.WebClient();
                    string remoteVerS = myWebClient.DownloadString(remoteUri + "ver.txt");
                    int remoteFileVer = 0;
                    if (Int32.TryParse(remoteVerS, out remoteFileVer) && localFileVer < remoteFileVer)
                    {
                        string fileName = "stats.txt";
                        // backup the current version (to safe user added custom commands)
                        System.IO.File.Copy(fileName, fileName + "_backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt");
                        // Download the Web resource and save it into the current filesystem folder.
                        myWebClient.DownloadFile(remoteUri + fileName, fileName);
                        fileName = "settings.txt";
                        // backup the current version (to safe user added custom commands)
                        System.IO.File.Copy(fileName, fileName + "_backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt");
                        // Download the Web resource and save it into the current filesystem folder.
                        myWebClient.DownloadFile(remoteUri + fileName, fileName);
                        // load new settings
                        if (loadStatFile(true))
                        {
                            MessageBox.Show("Download and update of entries successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("You already have the newest version of the entities.txt.\n\nIf you want to add custom commands, you can easily modify the file yourself.", "No new Version", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while trying to check or download:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void showSumOfChosenLevels()
        {
            // this displays the sum of the chosen levels.
            // The speedlevel is not chosen, but calculated from the other chosen levels, and must not be included in the sum, except all the other levels are determined uniquely!
            int sumW = 0, sumD = 0;
            bool valid = true, inbound = true, allUnique = true;
            for (int s = 0; s < 7; s++)
            {
                if (results[s].Count > chosenResults[s])
                {
                    sumW += statIOs[s].LevelWild;
                    sumD += statIOs[s].LevelDom;
                    if (results[s].Count != 1) { allUnique = false; }
                }
                else
                {
                    valid = false;
                    break;
                }
            }
            if (valid)
            {
                labelSumWild.Text = (sumW - (allUnique ? 0 : statIOs[6].LevelWild)).ToString();
                labelSumDom.Text = sumD.ToString();
                if (sumW - statIOs[6].LevelWild <= levelWildFromTorporRange[1]) { labelSumWild.ForeColor = SystemColors.ControlText; }
                else
                {
                    labelSumWild.ForeColor = Color.Red;
                    inbound = false;
                }
                if (sumD <= levelDomFromTorporAndTotalRange[1] && sumD >= levelDomFromTorporAndTotalRange[0]) { labelSumDom.ForeColor = SystemColors.ControlText; }
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

        }

        private void btnPerfectKibbleTame_Click(object sender, EventArgs e)
        {

        }

        private void buttonShowAllLib_Click(object sender, EventArgs e)
        {
            showTheseCreatures(creatureCollection.creatures, true);
        }

        private void buttonGetResultsLib_Click(object sender, EventArgs e)
        {
            if (comboBoxCreaturesLib.SelectedIndex >= 0 && comboBoxStatsLib.SelectedIndex >= 0)
            {
                var getCustomCreatureList = from creature in creatureCollection.creatures
                                            where creature.species == (string)comboBoxCreaturesLib.SelectedItem
                                            orderby creature.levelsWild[comboBoxStatsLib.SelectedIndex] ascending
                                            select creature;

                // display new results
                showTheseCreatures(getCustomCreatureList.ToList(), true);

            }
        }

        private void showTheseCreatures(List<Creature> creatures, bool clearBeforeAdding = true)
        {
            flowLayoutPanelCreatures.SuspendLayout();
            if (clearBeforeAdding)
            {
                // clear current boxes
                for (int b = 0; b < creatureBoxes.Count; b++) { creatureBoxes[b].Dispose(); }
                creatureBoxes.Clear();
            }
            // the list of creatures is appended to the list
            foreach (Creature cr in creatures)
            {
                CreatureBox cb = new CreatureBox(cr);
                flowLayoutPanelCreatures.Controls.Add(cb);
                creatureBoxes.Add(cb);
            }
            flowLayoutPanelCreatures.ResumeLayout();
            showCreaturesInTreeview(creatureCollection.creatures);
        }

        private void btnStatTestingCompute_Click(object sender, EventArgs e)
        {
            int a = 0;
            int thisCreature = cbbStatTestingRace.SelectedIndex;

            for (int i = 0; i < Enum.GetNames(typeof(StatName)).Count(); i++)
            {
                StatIO io = testingIOs[i];
                //io.computeStatValueFromLevelsWithTamingEfficiency(stats[thisCreature][i], (double)statTestingTamingEfficiency.Value);
                io.Input = Math.Pow(10, precisions[i] - 1) * calculateValue(thisCreature, i, io.LevelWild, io.LevelDom, true, (double)statTestingTamingEfficiency.Value);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (collectionDirty == true)
            {
                if (MessageBox.Show("Your Creature Collection has been modified since it was last saved, are you sure you want to discard your changes?", "Discard Changes?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    return;
            }

            creatureCollection = new CreatureCollection();
            showTheseCreatures(creatureCollection.creatures);
            Properties.Settings.Default.LastSaveFile = "";

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void listViewLibrary_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to descending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Descending;
            }

            // Perform the sort with these new sort options.
            this.listViewLibrary.Sort();
        }

        private void listViewLibrary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedItems.Count > 0)
            {
                creatureBoxListView.setCreature((Creature)listViewLibrary.SelectedItems[0].Tag);
            }
        }

        private void DetermineParentsToBreed()
        {
            Int32[] bestStat;
            List<Creature>[] bestCreatures;
            bool noCreaturesInThisSpecies;
            foreach (string species in creatureNames)
            {
                bestStat = new Int32[Enum.GetNames(typeof(StatName)).Count()];
                bestCreatures = new List<Creature>[Enum.GetNames(typeof(StatName)).Count()];
                noCreaturesInThisSpecies = true;
                foreach (Creature c in creatureCollection.creatures)
                {
                    if (c.species != species)
                        continue;

                    noCreaturesInThisSpecies = false;
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

                //beststat and bestcreatures now contain the best creatures for each stat and the best values.
                //if any male is in more than 1 category, remove any male in that category that is not in at least 2 categories himself
                for (int s = 0; s < Enum.GetNames(typeof(StatName)).Count(); s++)
                {
                    if (bestCreatures[s] == null || bestCreatures[s].Count == 0)
                    {
                        noCreaturesInThisSpecies = true;
                        break; // no creatures?
                    }

                    if (bestCreatures[s].Count == 1)
                        continue;

                    for (int c = 0; c < bestCreatures[s].Count; c++)
                    {
                        if (bestCreatures[s][c].gender != Gender.Male)
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
                                if (bestCreatures[s][oc].gender != Gender.Male)
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
                                    bestCreatures[s][oc] = null;
                            }

                        }

                    }

                }
                if (noCreaturesInThisSpecies)
                {
                    continue;
                }

                // now we have a list of all candidates for breeding. Iterate on stats. 
                for (int s = 0; s < Enum.GetNames(typeof(StatName)).Count() - 1; s++)
                {
                    for (int c = 0; c < bestCreatures[s].Count; c++)
                    {
                        Console.WriteLine(bestCreatures[s][c].gender + " " + bestCreatures[s][c].name + " for " + (StatName)s + " value of " + bestCreatures[s][c].levelsWild[s] + " (" + bestCreatures[s][c].valuesBreeding[s] + ")");
                        // flag topstats in creatures
                        bestCreatures[s][c].topBreedingStats[s] = true;
                    }
                }
            }
        }

    }
}
