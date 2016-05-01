using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class BreedingPlan : UserControl
    {
        public delegate void EditCreatureEventHandler(Creature creature, bool virtualCreature);
        public event EditCreatureEventHandler EditCreature;
        public delegate void CreateTimerEventHandler(string name, DateTime time);
        public event CreateTimerEventHandler CreateTimer;
        private List<Creature> females = new List<Creature>();
        private List<Creature> males = new List<Creature>();
        private List<int>[] combinedTops = new List<int>[2];
        private List<double> comboScore = new List<double>();
        private List<int> comboOrder = new List<int>();
        public string currentSpecies;
        public double[] statWeights = new double[8]; // how much are the stats weighted when looking for the best
        private List<int> bestLevels = new List<int>();
        private List<PedigreeCreature> pcs = new List<PedigreeCreature>();
        private bool[] enabledColorRegions;
        public double[] breedingMultipliers;
        private TimeSpan incubation = new TimeSpan(0), growing = new TimeSpan(0);

        public BreedingPlan()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            for (int i = 0; i < 8; i++)
                statWeights[i] = 1;
            combinedTops[0] = new List<int>();
            combinedTops[1] = new List<int>();
            pedigreeCreatureBest.CreatureClicked += new PedigreeCreature.CreatureChangedEventHandler(CreatureClicked);
            pedigreeCreatureWorst.CreatureClicked += new PedigreeCreature.CreatureChangedEventHandler(CreatureClicked);
            pedigreeCreatureBest.CreatureEdit += new PedigreeCreature.CreatureEditEventHandler(CreatureEdit);
            pedigreeCreatureWorst.CreatureEdit += new PedigreeCreature.CreatureEditEventHandler(CreatureEdit);
            pedigreeCreatureBest.IsVirtual = true;
            pedigreeCreatureWorst.IsVirtual = true;
            pedigreeCreatureBest.onlyLevels = true;
            pedigreeCreatureWorst.onlyLevels = true;
            pedigreeCreatureBest.Clear();
            pedigreeCreatureWorst.Clear();
            pedigreeCreatureBest.HandCursor = false;
            pedigreeCreatureWorst.HandCursor = false;
        }

        public void drawBestParents(BreedingMode breedingMode, bool updateBreedingData = false)
        {
            SuspendLayout();
            Cursor.Current = Cursors.WaitCursor;
            ClearControls();
            if (females != null && males != null && females.Count > 0 && males.Count > 0)
            {
                labelTitle.Text = currentSpecies;

                // scoring is higher if the offspring has the best levels (weighted also for probability)

                combinedTops[0].Clear();
                combinedTops[1].Clear();
                comboScore.Clear();
                comboOrder.Clear();
                double t = 0, tt = 0, pTS = 1;
                int o = 0, nrTS = 0;
                for (int f = 0; f < females.Count; f++)
                {
                    for (int m = 0; m < males.Count; m++)
                    {
                        combinedTops[0].Add(f);
                        combinedTops[1].Add(m);
                        t = 0;
                        nrTS = 0;
                        pTS = 1;
                        for (int s = 0; s < 7; s++)
                        {
                            tt = statWeights[s] * (0.7 * Math.Max(females[f].levelsWild[s], males[m].levelsWild[s]) + 0.3 * Math.Min(females[f].levelsWild[s], males[m].levelsWild[s])) / 40;
                            if (tt <= 0) { tt = 0; }
                            else if (breedingMode == BreedingMode.TopStatsLucky)
                            {
                                if (females[f].topBreedingStats[s] || males[m].topBreedingStats[s])
                                {
                                    if (females[f].topBreedingStats[s] && males[m].topBreedingStats[s])
                                        tt *= 1.142;
                                }
                                else if (bestLevels[s] > 0)
                                    tt *= .01;
                            }
                            else if (breedingMode == BreedingMode.TopStatsConservative && bestLevels[s] > 0)
                            {
                                tt *= .01;
                                if (females[f].topBreedingStats[s] || males[m].topBreedingStats[s])
                                {
                                    nrTS++;
                                    pTS *= ((females[f].topBreedingStats[s] && males[m].topBreedingStats[s]) ? 1 : 0.7);
                                }
                            }
                            t += tt;
                        }
                        if (breedingMode == BreedingMode.TopStatsConservative)
                        {
                            if (females[f].topStatsCount < nrTS && males[m].topStatsCount < nrTS)
                                t += nrTS * pTS;
                            else
                                t += .1 * nrTS * pTS;
                        }

                        comboScore.Add(t * 1.25);
                        comboOrder.Add(o++);
                    }
                }
                comboOrder = comboOrder.OrderByDescending(c => comboScore[c]).ToList();

                // draw best parents
                int row = 0;
                // scrolloffsets
                int xS = AutoScrollPosition.X;
                int yS = AutoScrollPosition.Y;
                PedigreeCreature pc;
                Bitmap bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                Graphics g = Graphics.FromImage(bm);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString("Breeding-Score", new System.Drawing.Font("Microsoft Sans Serif", 8.25F), new System.Drawing.SolidBrush(System.Drawing.Color.Black), 0, 3);

                for (int i = 0; i < 10 && i < comboOrder.Count; i++)
                {
                    pc = new PedigreeCreature(females[combinedTops[0][comboOrder[i]]], enabledColorRegions, comboOrder[i]);
                    pc.Location = new Point(10 + xS, 67 + 35 * row + yS);
                    pc.CreatureClicked += new PedigreeCreature.CreatureChangedEventHandler(CreatureClicked);
                    pc.CreatureEdit += new PedigreeCreature.CreatureEditEventHandler(CreatureEdit);
                    panelCombinations.Controls.Add(pc);
                    pcs.Add(pc);
                    pc = new PedigreeCreature(males[combinedTops[1][comboOrder[i]]], enabledColorRegions, comboOrder[i]);
                    pc.Location = new Point(350 + xS, 67 + 35 * row + yS);
                    pc.CreatureClicked += new PedigreeCreature.CreatureChangedEventHandler(CreatureClicked);
                    pc.CreatureEdit += new PedigreeCreature.CreatureEditEventHandler(CreatureEdit);
                    panelCombinations.Controls.Add(pc);
                    pcs.Add(pc);

                    // draw score
                    Brush br = new SolidBrush(Utils.getColorFromPercent((int)(comboScore[comboOrder[i]] * 12.5), 0.5));
                    Brush brd = new SolidBrush(Utils.getColorFromPercent((int)(comboScore[comboOrder[i]] * 12.5), -.2));
                    g.FillRectangle(brd, 0, 33 + 35 * row, 87, 5);
                    g.FillRectangle(brd, 20, 28 + 35 * row, 47, 15);
                    g.FillRectangle(br, 1, 34 + 35 * row, 85, 3);
                    g.FillRectangle(br, 21, 29 + 35 * row, 45, 13);
                    g.DrawString(comboScore[comboOrder[i]].ToString("N4"), new System.Drawing.Font("Microsoft Sans Serif", 8.25f), new System.Drawing.SolidBrush(System.Drawing.Color.Black), 24, 30 + 35 * row);
                    row++;
                }
                pictureBox1.Image = bm;
                g.Dispose();

                if (updateBreedingData)
                    setBreedingData(currentSpecies);
                setParents(comboOrder[0]);
            }
            else
            {
                labelTitle.Text = "No possible parents found for this species. Add them to the library first.";
                pictureBox1.Image = null;
                setBreedingData();
            }
            Cursor.Current = Cursors.Default;
            ResumeLayout();
        }

        public void ClearControls()
        {
            // clear Listings     
            foreach (PedigreeCreature pc in pcs)
                pc.Dispose();
            pcs.Clear();
            pedigreeCreatureBest.Clear();
            pedigreeCreatureWorst.Clear();
        }

        public void Clear()
        {
            ClearControls();
            pictureBox1.Image = null;
            setBreedingData();
            listView1.Items.Clear();
            currentSpecies = "";
            males.Clear();
            females.Clear();
            labelTitle.Text = "Select a species to see suggestions for the choosen breeding-mode";
        }

        private void setBreedingData(string species = "")
        {
            listView1.Items.Clear();
            if (species.Length > 0)
            {
                string file = "breedingTimes.txt";
                // check if file exists
                if (System.IO.File.Exists(file))
                {
                    string[] rows;
                    rows = System.IO.File.ReadAllLines(file);
                    string[] values;
                    int value = 0;
                    int[] times = new int[3];
                    string firstTime = "Pregnancy";
                    bool dataFound = false;
                    foreach (string row in rows)
                    {
                        if (row.Length > 1 && row.Substring(0, 2) != "//")
                        {
                            values = row.Split(',');
                            if (values[0] == species && values.Length > 3)
                            {
                                dataFound = true;
                                int t = 0;
                                for (int c = 1; c < 4 && t < 2; c++)
                                {
                                    value = 0;
                                    Int32.TryParse(values[c], out value);
                                    if (value > 0)
                                    {
                                        times[t] = value;
                                        t++;
                                    }
                                    else if (c == 1)
                                        firstTime = "Incubation";
                                }
                                break;
                            }
                        }
                    }
                    if (dataFound)
                    {
                        if (breedingMultipliers != null && breedingMultipliers.Length > 1)
                        {
                            for (int k = 0; k < 2; k++)
                                times[k] = (int)Math.Ceiling(times[k] / breedingMultipliers[k]);
                        }

                        int babyTime = (int)Math.Ceiling(times[1] * .1);
                        times[2] = times[1];
                        times[1] = babyTime;

                        string[] rowNames = new string[] { firstTime, "Baby", "Maturation" };
                        int totalTime = 0;
                        for (int k = 0; k < 3; k++)
                        {
                            if (k == 2)
                                totalTime -= times[1];
                            totalTime += times[k];
                            string[] subitems = new string[] { rowNames[k], new TimeSpan(0, 0, times[k]).ToString("d':'hh':'mm':'ss"), new TimeSpan(0, 0, totalTime).ToString("d':'hh':'mm':'ss"), DateTime.Now.AddSeconds(totalTime).ToShortTimeString() + ", " + DateTime.Now.AddSeconds(totalTime).ToShortDateString() };
                            listView1.Items.Add(new ListViewItem(subitems));
                        }
                        incubation = new TimeSpan(0, 0, times[0]);
                        growing = new TimeSpan(0, 0, times[2]);
                        buttonHatching.Text = firstTime;
                    }
                    else
                    {
                        listView1.Items.Add("n/a yet");
                    }
                }
                else
                {
                    MessageBox.Show("Breeding-File '" + file + "' not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public List<Creature> Creatures
        {
            set
            {
                females = value.Where(c => c.gender == Gender.Female).ToList();
                males = value.Where(c => c.gender == Gender.Male).ToList();

                bestLevels.Clear();
                for (int s = 0; s < 8; s++)
                    bestLevels.Add(0);

                foreach (Creature c in value)
                {
                    for (int s = 0; s < 8; s++)
                    {
                        if (c.levelsWild[s] > bestLevels[s])
                            bestLevels[s] = c.levelsWild[s];
                    }
                }
            }
        }

        private void CreatureClicked(Creature c, int comboIndex, MouseEventArgs e)
        {
            if (comboIndex >= 0)
                setParents(comboIndex);
        }

        private void CreatureEdit(Creature c, bool isVirtual)
        {
            if (EditCreature != null)
                EditCreature(c, isVirtual);
        }

        private void setParents(int comboIndex)
        {
            Creature crB = new Creature(currentSpecies, "", "", 0, new int[8], null, 100, true);
            Creature crW = new Creature(currentSpecies, "", "", 0, new int[8], null, 100, true);
            Creature mother = females[combinedTops[0][comboIndex]];
            Creature father = males[combinedTops[1][comboIndex]];
            crB.Mother = mother;
            crB.Father = father;
            crW.Mother = mother;
            crW.Father = father;
            bool totalLevelUnknown = false; // if stats are unknown, total level is as well (==> oxygen, speed)
            for (int s = 0; s < 7; s++)
            {
                crB.levelsWild[s] = Math.Max(mother.levelsWild[s], father.levelsWild[s]);
                crB.topBreedingStats[s] = (crB.levelsWild[s] == bestLevels[s]);
                crW.levelsWild[s] = Math.Min(mother.levelsWild[s], father.levelsWild[s]);
                crW.topBreedingStats[s] = (crW.levelsWild[s] == bestLevels[s]);
                if (crB.levelsWild[s] == -1 || crW.levelsWild[s] == -1)
                    totalLevelUnknown = true;
            }
            crB.levelsWild[7] = crB.levelsWild.Sum();
            crW.levelsWild[7] = crW.levelsWild.Sum();
            crB.name = "Best Possible (" + crB.levelHatched + (totalLevelUnknown ? "+" : "") + ")";
            crW.name = "Worst Possible (" + crW.levelHatched + (totalLevelUnknown ? "+" : "") + ")";
            pedigreeCreatureBest.setCreature(crB);
            pedigreeCreatureWorst.setCreature(crW);
            // highlight parents
            int hiliId = comboOrder.IndexOf(comboIndex) * 2;
            for (int i = 0; i < pcs.Count; i++)
                pcs[i].highlight = (i == hiliId || i == hiliId + 1);
        }

        public bool[] EnabledColorRegions
        {
            set
            {
                if (value != null && value.Length == 6)
                {
                    enabledColorRegions = value;
                }
                else
                {
                    enabledColorRegions = new bool[] { true, true, true, true, true, true };
                }
            }
        }

        private void buttonHatching_Click(object sender, EventArgs e)
        {
            if (CreateTimer != null && currentSpecies != "")
                CreateTimer(currentSpecies + " " + buttonHatching.Text, DateTime.Now.Add(incubation));
        }

        public enum BreedingMode
        {
            BestNextGen,
            TopStatsLucky,
            TopStatsConservative
        }
    }
}
