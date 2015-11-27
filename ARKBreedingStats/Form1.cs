using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class Form1 : Form
    {
        private List<string> creatures = new List<string>();
        private string[] statNames = new string[] { "Health", "Stamina", "Oxygen", "Food", "Weight", "Damage", "Speed", "Torpor" };
        private List<List<double[]>> stats = new List<List<double[]>>();
        private List<int> levelXP = new List<int>();
        private List<StatIO> statIOs = new List<StatIO>();
        private List<List<double[]>> results = new List<List<double[]>>();
        private int c = 0; // current creature
        private bool postTamed = false;
        private int activeStat = -1;
        private List<int> statWithEff = new List<int>();
        private List<int> chosenResults = new List<int>();
        private int[] precisions = new int[] { 1, 1, 1, 1, 1, 3, 3, 1 }; // damage and speed are percentagevalues, need more precision

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            linkLabel1.Links.Add(0, 12, "https://github.com/cadon/ARKStatsExtractor");
            statIOs.Add(this.statIOHealth);
            statIOs.Add(this.statIOStamina);
            statIOs.Add(this.statIOOxygen);
            statIOs.Add(this.statIOFood);
            statIOs.Add(this.statIOWeight);
            statIOs.Add(this.statIODamage);
            statIOs.Add(this.statIOSpeed);
            statIOs.Add(this.statIOTorpor);
            for (int s = 0; s < statNames.Length; s++)
            {
                statIOs[s].Title = statNames[s];
                statIOs[s].Id = s;
                if (precisions[s] == 3) { statIOs[s].Percent = true; }
            }
            loadFile();
            comboBoxCreatures.SelectedIndex = 0;
            labelVersion.Text = "v0.9.1";
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.labelDomLevel, "level since domesticated");
            tt.SetToolTip(this.checkBoxOutputRowHeader, "Include Headerrow");
            tt.SetToolTip(this.checkBoxJustTamed, "Since taming no server-restart");
        }

        private void clearAll()
        {
            results.Clear();
            listBoxPossibilities.Items.Clear();
            chosenResults.Clear();
            for (int s = 0; s < 8; s++)
            {
                statIOs[s].Clear();
                chosenResults.Add(0);
                statIOs[s].BarLength = 0;
            }
            this.labelFootnote.Text = "";
            statWithEff.Clear();
            this.numericUpDownLevel.BackColor = SystemColors.Window;
            this.numericUpDownLowerTEffBound.BackColor = SystemColors.Window;
            this.numericUpDownUpperTEffBound.BackColor = SystemColors.Window;
            this.numericUpDownXP.BackColor = SystemColors.Window;
            this.checkBoxAlreadyBred.BackColor = System.Drawing.Color.Transparent;
            buttonCopyClipboard.Enabled = false;
            activeStat = -1;
        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            int activeStatKeeper = activeStat;
            clearAll();
            bool resultsValid = true;
            // torpor is directly proportional to wild level (after taming it's a too high estimate, making the upper bound worse)
            postTamed = (stats[c][7][0] + stats[c][7][0] * stats[c][7][1] * Math.Round((statIOs[7].Input - stats[c][7][0]) / (stats[c][7][0] * stats[c][7][1])) != statIOs[7].Input);
            for (int s = 0; s < statIOs.Count; s++)
            {
                results.Add(new List<double[]>());
                statIOs[s].PostTame = postTamed;
                double inputValue = Math.Round(statIOs[s].Input / (precisions[s] == 3 ? 100 : 1), precisions[s]);
                double maxLW = 0;
                if (stats[c][s][0] > 0 && stats[c][s][1] > 0)
                {
                    maxLW = Math.Floor((inputValue - stats[c][s][0]) / (stats[c][s][0] * stats[c][s][1]));
                }
                double maxLD = 0;
                if (stats[c][s][0] > 0 && stats[c][s][2] > 0 && postTamed)
                {
                    maxLD = Math.Round((inputValue - stats[c][s][0]) / (stats[c][s][0] * stats[c][s][2])); //floor is sometimes too unprecise
                }
                double vWildL = 0; // value with only wild levels
                double tamingEfficiency = -1, tEUpperBound = (double)this.numericUpDownUpperTEffBound.Value / 100, tELowerBound = (double)this.numericUpDownLowerTEffBound.Value / 100;
                if (checkBoxAlreadyBred.Checked)
                {
                    // bred creatures always have 100% TE
                    tEUpperBound = 1;
                    tELowerBound = 1;
                }
                bool withTEff = (postTamed && stats[c][s][4] > 0);
                if (withTEff) { statWithEff.Add(s); }
                for (int w = 0; w < maxLW + 1; w++)
                {
                    for (int d = 0; d < maxLD + 1; d++)
                    {
                        vWildL = stats[c][s][0] + stats[c][s][0] * stats[c][s][1] * w + (postTamed ? stats[c][s][3] : 0);
                        if (withTEff)
                        {
                            // taming bonus is percentual, this means the taming-efficiency plays a role
                            // get tamingEfficiency-possibility
                            tamingEfficiency = Math.Round((inputValue / (1 + stats[c][s][2] * d) - vWildL) / (vWildL * stats[c][s][4]), 3, MidpointRounding.AwayFromZero);
                            if (tamingEfficiency >= tELowerBound)
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
                        else if (Math.Round(vWildL + vWildL * stats[c][s][2] * d, precisions[s], MidpointRounding.AwayFromZero) == inputValue)
                        {
                            results[s].Add(new double[] { w, d, tamingEfficiency });
                            break; // no other solution possible
                        }
                    }
                }
            }
            // max level for wild according to torpor (torpor is depending on taming efficiency up to 5/3 times "too high" for level
            double torporLevelTamingMultMax = 1;
            // when just tamed there is a bug that gives too much torpor until server-restart
            if (postTamed && this.checkBoxJustTamed.Checked)
            {
                torporLevelTamingMultMax = (200 + (double)this.numericUpDownUpperTEffBound.Value) / (400 + (double)this.numericUpDownUpperTEffBound.Value);
            }
            int maxLW2 = (int)Math.Round((statIOs[7].Input - (postTamed ? stats[c][7][3] : 0) - stats[c][7][0]) * torporLevelTamingMultMax / (stats[c][7][0] * stats[c][7][1]), 0) - 1; // -1 because creature starts with level 1
            int levelDom = 0;
            // lower/upper Bound of each stat (wild has no upper bound as wild-speed is unknown)
            int[] lowerBoundExtraWs = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            int[] lowerBoundExtraDs = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            int[] upperBoundExtraDs = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            if (postTamed)
            {
                levelDom = getLevelFromXP();
            }
            int wildSpeedLevel = maxLW2;
            // substract all uniquely solved stat-levels
            for (int s = 0; s < 7; s++)
            {
                if (results[s].Count == 1)
                {
                    // result is uniquely solved
                    maxLW2 -= (int)results[s][0][0];
                    levelDom -= (int)results[s][0][1];
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
            if (maxLW2 < lowerBoundExtraWs.Sum() || levelDom < lowerBoundExtraDs.Sum())
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
                this.numericUpDownXP.BackColor = Color.LightSalmon;
                this.checkBoxAlreadyBred.BackColor = Color.LightSalmon;
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
                        if (results[s].Count > 1 && (results[s][r][0] > maxLW2 - lowerBoundExtraWs.Sum() + lowerBoundExtraWs[s] || results[s][r][1] > levelDom - lowerBoundExtraDs.Sum() + lowerBoundExtraDs[s] || results[s][r][1] < levelDom - upperBoundExtraDs.Sum() + upperBoundExtraDs[s]))
                        {
                            results[s].RemoveAt(r--);
                            // if result gets unique due to this, check if remaining result doesn't violate for max level
                            if (results[s].Count == 1)
                            {
                                maxLW2 -= (int)results[s][0][0];
                                levelDom -= (int)results[s][0][1];
                                lowerBoundExtraWs[s] = 0;
                                lowerBoundExtraDs[s] = 0;
                                if (maxLW2 < 0 || levelDom < 0)
                                {
                                    this.numericUpDownLevel.BackColor = Color.LightSalmon;
                                    statIOs[s].Warning = 2;
                                    statIOs[7].Warning = 2;
                                    results[s].Clear();
                                    break;
                                }
                            }
                        }
                    }
                }
                // if more than one parameter is affected by tamingEfficiency filter all numbers that occure only in one
                if (statWithEff.Count > 1)
                {
                    for (int es = 0; es < statWithEff.Count; es++)
                    {
                        for (int et = es + 1; et < statWithEff.Count; et++)
                        {
                            List<int> equalEffs1 = new List<int>();
                            List<int> equalEffs2 = new List<int>();
                            for (int ere = 0; ere < results[statWithEff[es]].Count; ere++)
                            {
                                for (int erf = 0; erf < results[statWithEff[et]].Count; erf++)
                                {
                                    // efficiency-calculation can be a bit off due to rounding-ingame, so treat them as equal when diff<0.002
                                    if (Math.Abs(results[statWithEff[es]][ere][2] - results[statWithEff[et]][erf][2]) < 0.002)
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
                                validResults1.Add(results[statWithEff[es]][equalEffs1[ev]]);
                            }
                            // replace long list with (hopefully) shorter list with valid entries
                            results[statWithEff[es]] = validResults1;
                            List<double[]> validResults2 = new List<double[]>();
                            for (int ev = 0; ev < equalEffs2.Count; ev++)
                            {
                                validResults2.Add(results[statWithEff[et]][equalEffs2[ev]]);
                            }
                            results[statWithEff[et]] = validResults2;
                        }
                        if (es >= statWithEff.Count - 2)
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
                        // display result with most levels in wild (most probable for the stats getting not unique results here)
                        int r = 0;
                        if (results[s][0][2] == -1) { r = results[s].Count - 1; }
                        setPossibility(s, r);
                        if (results[s].Count > 1)
                        {
                            statIOs[s].Warning = 1;
                        }
                    }
                    else
                    {
                        statIOs[s].Warning = 2;
                        results[s].Clear();
                        resultsValid = false;
                        if (!checkBoxAlreadyBred.Checked && this.numericUpDownLowerTEffBound.Value > 0)
                        {
                            this.numericUpDownLowerTEffBound.BackColor = Color.LightSalmon;
                        }
                        if (!checkBoxAlreadyBred.Checked && this.numericUpDownUpperTEffBound.Value < 100)
                        {
                            this.numericUpDownUpperTEffBound.BackColor = Color.LightSalmon;
                        }
                        this.checkBoxAlreadyBred.BackColor = Color.LightSalmon;
                    }
                }
            }
            if (results.Count == 8)
            {
                // speed gets remaining wild levels if all other are unique
                bool setSpeed = true;
                for (int s = 0; s < 6; s++)
                {
                    if (results[s].Count != 1)
                    {
                        setSpeed = false;
                        break;
                    }
                    wildSpeedLevel -= (int)results[s][0][0];

                }
                if (setSpeed)
                {
                    statIOs[6].LevelWild = wildSpeedLevel.ToString();
                }
            }
            if (resultsValid)
            {
                buttonCopyClipboard.Enabled = true;
                setActiveStat(activeStatKeeper);
            }
            if (!postTamed)
            {
                labelFootnote.Text = "*Creature is not yet tamed and may get better values then.";
            }
        }

        private void statIO_Click(object sender, EventArgs e)
        {
            StatIO se = (StatIO)sender;
            if (se != null)
            {
                setActiveStat(se.Id);
            }
        }

        private void setActiveStat(int stat)
        {
            this.listBoxPossibilities.Items.Clear();
            for (int s = 0; s < 8; s++)
            {
                if (s == stat && statIOs[s].Warning == 1)
                {
                    statIOs[s].Selected = true;
                    activeStat = s;
                    setCombobox(s);
                }
                else
                {
                    statIOs[s].Selected = false;
                }
            }
        }

        private void setCombobox(int s)
        {
            if (s < results.Count)
            {
                for (int r = 0; r < results[s].Count; r++)
                {
                    this.listBoxPossibilities.Items.Add(results[s][r][0].ToString() + "\t" + results[s][r][1].ToString() + (results[s][r][2] >= 0 ? "\t" + (results[s][r][2] * 100).ToString() + "%" : ""));
                }
            }
        }

        private void loadFile()
        {
            // read entities from file
            string path = "stats.csv";

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
                foreach (string row in rows)
                {
                    if (row.Length > 1 && row.Substring(0, 2) != "//")
                    {
                        values = row.Split(',');
                        if (values.Length == 1)
                        {
                            // new creature
                            List<double[]> cs = new List<double[]>();
                            for (s = 0; s < 8; s++)
                            {
                                cs.Add(new double[] { 0, 0, 0, 0, 0 });
                            }
                            s = 0;
                            stats.Add(cs);
                            this.comboBoxCreatures.Items.Add(values[0].Trim());
                            c++;
                        }
                        else if (values.Length > 1 && values.Length < 6)
                        {
                            for (int v = 0; v < values.Length; v++)
                            {
                                if (s == 5 && v == 0) { stats[c][5][0] = 1; } // damage and speed are handled as percentage of a hidden base value
                                else
                                {
                                    double value = 0;
                                    if (Double.TryParse(values[v], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value))
                                    {
                                        stats[c][s][v] = value;
                                    }
                                }
                            }
                            s++;
                        }
                    }
                }
            }


            // read needed xp for levels from file
            path = "level.txt";

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
                int xp = 0;
                foreach (string row in rows)
                {
                    if (row.Length > 0 && Int32.TryParse(row, out xp))
                    {
                        levelXP.Add(xp);
                    }
                }
            }
        }

        private void comboBoxCreatures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCreatures.SelectedIndex >= 0)
            {
                c = comboBoxCreatures.SelectedIndex;
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
            if (s == 7)
            {
                statIOs[s].LevelWild = "(" + results[s][i][0].ToString() + ")";
            }
            else
            {
                statIOs[s].LevelWild = results[s][i][0].ToString();
                statIOs[s].BarLength = (int)results[s][i][0];
            }
            statIOs[s].LevelDom = results[s][i][1].ToString();
            statIOs[s].BreedingValue = breedingValue(s, i);
            chosenResults[s] = i;
        }

        private void buttonCopyClipboard_Click(object sender, EventArgs e)
        {
            if (results.Count == 8 && chosenResults.Count == 8)
            {
                List<string> tsv = new List<string>();
                int wildLevels = 0;
                for (int s = 0; s < 7; s++) { wildLevels += (int)results[s][chosenResults[s]][0]; }
                string rowLevel = comboBoxCreatures.SelectedItem.ToString() + "\t\t" + wildLevels, rowValues = "";
                // if taming efficiency is unique, display it, too
                string effString = "";
                if (statWithEff.Count > 0)
                {
                    double eff = results[statWithEff[0]][chosenResults[statWithEff[0]]][2];
                    bool useEff = true;
                    for (int st = 1; st < statWithEff.Count; st++)
                    {
                        // efficiency-calculation can be a bit off due to ingame-rounding
                        if (Math.Abs(results[statWithEff[st]][chosenResults[statWithEff[st]]][2] - eff) > 0.002)
                        {
                            useEff = false;
                            break;
                        }
                    }
                    if (useEff)
                    {
                        effString = "\tTamingEff:\t" + (100 * eff).ToString() + "%";
                    }
                }
                // headerrow
                if (radioButtonOutputTable.Checked || checkBoxOutputRowHeader.Checked)
                {
                    if (radioButtonOutputTable.Checked)
                    {
                        tsv.Add(comboBoxCreatures.SelectedItem.ToString() + "\tLevel " + numericUpDownLevel.Value.ToString() + effString);
                        tsv.Add("Stat\tWildLevel\tDomLevel\tBreedingValue");
                    }
                    else { tsv.Add("Species\tName\tWild-Levels\tHP-Level\tSt-Level\tOx-Level\tFo-Level\tWe-Level\tDm-Level\tSp-Level\tTo-Level\tHP-Value\tSt-Value\tOx-Value\tFo-Value\tWe-Value\tDm-Value\tSp-Value\tTo-Value"); }
                }
                for (int s = 0; s < 8; s++)
                {
                    if (chosenResults[s] < results[s].Count)
                    {
                        string breedingV = "";
                        if (precisions[s] == 3)
                        {
                            breedingV = (100 * breedingValue(s, chosenResults[s])).ToString() + "%";
                        }
                        else
                        {
                            breedingV = breedingValue(s, chosenResults[s]).ToString();
                        }
                        if (radioButtonOutputTable.Checked)
                        {
                            tsv.Add(statNames[s] + "\t" + results[s][chosenResults[s]][0].ToString() + "\t" + results[s][chosenResults[s]][1].ToString() + "\t" + breedingV);
                        }
                        else
                        {
                            rowLevel += "\t" + results[s][chosenResults[s]][0].ToString();
                            rowValues += "\t" + breedingV;
                        }
                    }
                    else { return; }
                }
                if (radioButtonOutputRow.Checked) { tsv.Add(rowLevel + rowValues); }
                Clipboard.SetText(string.Join("\n", tsv));
            }
        }

        private double breedingValue(int s, int r)
        {
            if (s >= 0 && s < 8)
            {
                if (r >= 0 && r < results[s].Count)
                {
                    return Math.Round((stats[c][s][0] + stats[c][s][0] * stats[c][s][1] * results[s][r][0] + stats[c][s][3]) * (results[s][r][2] >= 0 ? (1 + stats[c][s][4]) : 1), precisions[s], MidpointRounding.AwayFromZero);
                }
            }
            return -1;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            if (n != null)
            {
                n.Select(0, n.Text.Length);
            }
        }

        private void numericUpDownXP_ValueChanged(object sender, EventArgs e)
        {
            this.labelDomLevel.Text = "DLevel " + getLevelFromXP();
        }

        private int getLevelFromXP()
        {
            int level = 0;
            while (levelXP.Count > level + 1 && this.numericUpDownXP.Value >= levelXP[level + 1]) { level++; }
            return level;
        }

        private void radioButtonOutputRow_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBoxOutputRowHeader.Enabled = radioButtonOutputRow.Checked;
        }

        private void checkBoxAlreadyBred_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxTE.Enabled = !checkBoxAlreadyBred.Checked;
            checkBoxJustTamed.Checked = checkBoxJustTamed.Checked&&!checkBoxAlreadyBred.Checked;
        }

        private void checkBoxJustTamed_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxAlreadyBred.Checked = checkBoxAlreadyBred.Checked && !checkBoxJustTamed.Checked;
        }
    }
}
