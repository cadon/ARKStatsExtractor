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
        public delegate void BPRecalcEventHandler();
        public event BPRecalcEventHandler BPRecalc;
        public event PedigreeCreature.CreaturePartnerEventHandler BestBreedingPartners;
        private List<Creature> females = new List<Creature>();
        private List<Creature> males = new List<Creature>();
        private List<int>[] combinedTops = new List<int>[2];
        private List<double> comboScore = new List<double>();
        private List<int> comboOrder = new List<int>();
        public string currentSpecies;
        public int speciesIndex;
        public double[] statWeights = new double[8]; // how much are the stats weighted when looking for the best
        private List<int> bestLevels = new List<int>();
        private List<PedigreeCreature> pcs = new List<PedigreeCreature>();
        private List<PictureBox> pbs = new List<PictureBox>();
        private bool[] enabledColorRegions;
        private TimeSpan incubation = new TimeSpan(0), growing = new TimeSpan(0);
        public int maxSuggestions;
        public Creature chosenCreature = null;

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

            ToolTip tt = new ToolTip();
            tt.SetToolTip(labelBreedingScore, "The Breeding-Score of a paring is not comparable to the Breeding-Score of another breeding-mode.\nThe numbers in the different modes are generated in incompatible ways.");
        }

        public void drawBestParents(BreedingMode breedingMode, bool updateBreedingData = false)
        {
            SuspendLayout();
            Cursor.Current = Cursors.WaitCursor;
            ClearControls();

            // chosen Creature (only consider this one for its gender)
            bool considerChosenCreature = chosenCreature != null;
            Gender chosenCG = (considerChosenCreature ? chosenCreature.gender : Gender.Unknown);

            labelTitle.Text = currentSpecies + (considerChosenCreature ? " (only pairings with \"" + chosenCreature.name + "\")" : "");
            if (females != null && males != null && females.Count > 0 && males.Count > 0)
            {
                combinedTops[0].Clear();
                combinedTops[1].Clear();
                comboScore.Clear();
                comboOrder.Clear();
                double t = 0, tt = 0, pTS = 1;
                int o = 0, nrTS = 0;
                Int16[] bestPossLevels = new Int16[7]; // best possible levels

                for (int f = 0; f < females.Count; f++)
                {
                    if (considerChosenCreature && chosenCG == Gender.Female && females[f] != chosenCreature)
                        continue;
                    for (int m = 0; m < males.Count; m++)
                    {
                        if (considerChosenCreature && chosenCG == Gender.Male && males[m] != chosenCreature)
                            continue;

                        combinedTops[0].Add(f);
                        combinedTops[1].Add(m);
                        t = 0;
                        nrTS = 0; // number of possible top-stats
                        pTS = 1;
                        for (int s = 0; s < 7; s++)
                        {
                            bestPossLevels[s] = 0;
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
                                bestPossLevels[s] = (Int16)Math.Max(females[f].levelsWild[s], males[m].levelsWild[s]);
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
                            if (females[f].topStatsCountBP < nrTS && males[m].topStatsCountBP < nrTS)
                                t += nrTS * pTS;
                            else
                                t += .1 * nrTS * pTS;
                            // check if the best possible stat outcome already exists in a male
                            bool maleExists = false;
                            foreach (Creature cr in males)
                            {
                                maleExists = true;
                                for (int s = 0; s < 7; s++)
                                {
                                    if (cr.levelsWild[s] != bestPossLevels[s])
                                    {
                                        maleExists = false;
                                        break;
                                    }
                                }
                                if (maleExists)
                                    break;
                            }
                            if (maleExists)
                                t *= .2; // another male with the same stats is not worth much
                            else
                            {
                                // check if the best possible stat outcome already exists in a female
                                bool femaleExists = false;
                                foreach (Creature cr in females)
                                {
                                    femaleExists = true;
                                    for (int s = 0; s < 7; s++)
                                    {
                                        if (cr.levelsWild[s] != bestPossLevels[s])
                                        {
                                            femaleExists = false;
                                            break;
                                        }
                                    }
                                    if (femaleExists)
                                        break;
                                }
                                if (femaleExists)
                                    t *= .5; // another female with the same stats may be useful, but not so much in conservative breeding
                            }
                            t *= 2; // scale conservative mode as it rather displays improvement, but only scarcely
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
                Bitmap bm;
                Graphics g;
                PictureBox pb;

                for (int i = 0; i < maxSuggestions && i < comboOrder.Count; i++)
                {
                    pc = new PedigreeCreature(females[combinedTops[0][comboOrder[i]]], enabledColorRegions, comboOrder[i]);
                    pc.Location = new Point(10 + xS, 5 + 35 * row + yS);
                    pc.CreatureClicked += new PedigreeCreature.CreatureChangedEventHandler(CreatureClicked);
                    pc.CreatureEdit += new PedigreeCreature.CreatureEditEventHandler(CreatureEdit);
                    pc.BPRecalc += new BPRecalcEventHandler(BPRecalc);
                    pc.BestBreedingPartners += new PedigreeCreature.CreaturePartnerEventHandler(BestBreedingPartners);
                    panelCombinations.Controls.Add(pc);
                    pcs.Add(pc);
                    pc = new PedigreeCreature(males[combinedTops[1][comboOrder[i]]], enabledColorRegions, comboOrder[i]);
                    pc.Location = new Point(350 + xS, 5 + 35 * row + yS);
                    pc.CreatureClicked += new PedigreeCreature.CreatureChangedEventHandler(CreatureClicked);
                    pc.CreatureEdit += new PedigreeCreature.CreatureEditEventHandler(CreatureEdit);
                    pc.BPRecalc += new BPRecalcEventHandler(BPRecalc);
                    pc.BestBreedingPartners += new PedigreeCreature.CreaturePartnerEventHandler(BestBreedingPartners);
                    panelCombinations.Controls.Add(pc);
                    pcs.Add(pc);

                    // draw score
                    pb = new PictureBox();
                    pbs.Add(pb);
                    panelCombinations.Controls.Add(pb);
                    pb.Size = new Size(87, 15);
                    pb.Location = new Point(261 + xS, 19 + 35 * row + yS);
                    bm = new Bitmap(pb.Width, pb.Height);
                    g = Graphics.FromImage(bm);
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    Brush br = new SolidBrush(Utils.getColorFromPercent((int)(comboScore[comboOrder[i]] * 12.5), 0.5));
                    Brush brd = new SolidBrush(Utils.getColorFromPercent((int)(comboScore[comboOrder[i]] * 12.5), -.2));
                    g.FillRectangle(brd, 0, 5, 87, 5);
                    g.FillRectangle(brd, 20, 0, 47, 15);
                    g.FillRectangle(br, 1, 6, 85, 3);
                    g.FillRectangle(br, 21, 1, 45, 13);
                    g.DrawString(comboScore[comboOrder[i]].ToString("N4"), new System.Drawing.Font("Microsoft Sans Serif", 8.25f), new System.Drawing.SolidBrush(System.Drawing.Color.Black), 24, 2);
                    pb.Image = bm;
                    g.Dispose();
                    row++;
                }

                if (updateBreedingData)
                    setBreedingData(currentSpecies);
                if (comboOrder.Count > 0)
                    setParents(comboOrder[0]);
                else
                    setParents(-1);
            }
            else
            {
                labelInfo.Text = "No possible pairings found for " + currentSpecies + ". Make sure at least one female and male are available in your library.";
                labelInfo.Visible = true;
                if (updateBreedingData)
                    setBreedingData(currentSpecies);
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
            foreach (PictureBox pb in pbs)
                pb.Dispose();
            pbs.Clear();
            pedigreeCreatureBest.Clear();
            pedigreeCreatureWorst.Clear();
            labelInfo.Visible = false;
            labelProbabilityBest.Text = "";
        }

        public void Clear()
        {
            ClearControls();
            setBreedingData();
            listView1.Items.Clear();
            currentSpecies = "";
            males.Clear();
            females.Clear();
            labelTitle.Text = "Select a species to see suggestions for the chosen breeding-mode";
        }

        private void setBreedingData(string species = "")
        {
            int si = Values.V.speciesNames.IndexOf(species);
            listView1.Items.Clear();
            if (si < 0 || Values.V.species[si].breeding == null)
            {
                listView1.Items.Add("n/a yet");
            }
            else
            {
                BreedingData breeding = Values.V.species[si].breeding;

                string firstTime = "Pregnancy";
                if (breeding.pregnancyTimeAdjusted <= 0)
                    firstTime = "Incubation";


                int babyTime = (int)Math.Ceiling(breeding.maturationTimeAdjusted * .1);
                double fullTime = breeding.maturationTimeAdjusted;

                string[] rowNames = new string[] { firstTime, "Baby", "Maturation" };
                for (int k = 0; k < 3; k++)
                {
                    int t1, totalTime = 0;
                    switch (k)
                    {
                        default:
                        case 0: t1 = breeding.pregnancyTimeAdjusted == 0 ? breeding.incubationTimeAdjusted : breeding.pregnancyTimeAdjusted; totalTime = t1; break;
                        case 1: t1 = (int)(.1f * breeding.maturationTimeAdjusted); totalTime += t1; break;
                        case 2: t1 = breeding.maturationTimeAdjusted; totalTime = breeding.pregnancyTimeAdjusted + breeding.incubationTimeAdjusted + breeding.maturationTimeAdjusted; break;
                    }

                    string[] subitems = new string[] { rowNames[k],
                                                        new TimeSpan(0, 0, t1).ToString("d':'hh':'mm':'ss"),
                                                            new TimeSpan(0, 0, totalTime).ToString("d':'hh':'mm':'ss"),
                                                            DateTime.Now.AddSeconds(totalTime).ToShortTimeString() + ", " + DateTime.Now.AddSeconds(totalTime).ToShortDateString()
                                                    };
                    listView1.Items.Add(new ListViewItem(subitems));
                }
                incubation = new TimeSpan(0, 0, breeding.pregnancyTimeAdjusted + breeding.incubationTimeAdjusted);
                growing = new TimeSpan(0, 0, breeding.maturationTimeAdjusted);
                buttonHatching.Text = firstTime;

                // further info
                string breedingInfo = "";
                if (breeding.eggTempMin > 0)
                    breedingInfo += "Egg-Temperature:\n" 
                        + (Properties.Settings.Default.celsius ? breeding.eggTempMin : Math.Round(breeding.eggTempMin * 1.8 + 32, 1)) + " - " 
                        + (Properties.Settings.Default.celsius ? breeding.eggTempMax : Math.Round(breeding.eggTempMax * 1.8 + 32, 1)) 
                        + (Properties.Settings.Default.celsius? " °C" : " °F");
                if (breeding.eggTempMin > 0 && breeding.matingCooldownMinAdjusted > 0)
                    breedingInfo += "\n\n";
                if (breeding.matingCooldownMinAdjusted > 0)
                    breedingInfo += "Time until next mating is possible:\n" + new TimeSpan(0, 0, breeding.matingCooldownMinAdjusted).ToString("d':'hh':'mm") + " - " + new TimeSpan(0, 0, breeding.matingCooldownMaxAdjusted).ToString("d':'hh':'mm");
                labelBreedingInfos.Text = breedingInfo;
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
            if (comboIndex < 0 || comboIndex > combinedTops[0].Count)
            {
                pedigreeCreatureBest.Clear();
                pedigreeCreatureWorst.Clear();
                labelInfo.Visible = false;
                labelProbabilityBest.Text = "";
                return;
            }

            Creature crB = new Creature(currentSpecies, "", "", 0, new int[8], null, 100, true);
            Creature crW = new Creature(currentSpecies, "", "", 0, new int[8], null, 100, true);
            Creature mother = females[combinedTops[0][comboIndex]];
            Creature father = males[combinedTops[1][comboIndex]];
            crB.Mother = mother;
            crB.Father = father;
            crW.Mother = mother;
            crW.Father = father;
            double probabilityBest = 1;
            bool totalLevelUnknown = false; // if stats are unknown, total level is as well (==> oxygen, speed)
            for (int s = 0; s < 7; s++)
            {
                crB.levelsWild[s] = Math.Max(mother.levelsWild[s], father.levelsWild[s]);
                crB.valuesBreeding[s] = Stats.calculateValue(speciesIndex, s, crB.levelsWild[s], 0, true, 1, 0);
                crB.topBreedingStats[s] = (crB.levelsWild[s] == bestLevels[s]);
                crW.levelsWild[s] = Math.Min(mother.levelsWild[s], father.levelsWild[s]);
                crW.valuesBreeding[s] = Stats.calculateValue(speciesIndex, s, crW.levelsWild[s], 0, true, 1, 0);
                crW.topBreedingStats[s] = (crW.levelsWild[s] == bestLevels[s]);
                if (crB.levelsWild[s] == -1 || crW.levelsWild[s] == -1)
                    totalLevelUnknown = true;
                if (crB.levelsWild[s] > crW.levelsWild[s])
                    probabilityBest *= .7;
            }
            crB.levelsWild[7] = crB.levelsWild.Sum();
            crW.levelsWild[7] = crW.levelsWild.Sum();
            crB.name = "Best Possible (" + crB.levelHatched + (totalLevelUnknown ? "+" : "") + ")";
            crW.name = "Worst Possible (" + crW.levelHatched + (totalLevelUnknown ? "+" : "") + ")";
            pedigreeCreatureBest.setCreature(crB);
            pedigreeCreatureWorst.setCreature(crW);
            labelProbabilityBest.Text = "Probability for this Best Possible outcome: " + Math.Round(100 * probabilityBest, 1).ToString() + "%";
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

        private void buttonBabyPhase_Click(object sender, EventArgs e)
        {
            if (CreateTimer != null && currentSpecies != "")
                CreateTimer(currentSpecies + " Baby-Phase", DateTime.Now.Add(new TimeSpan(0, 0, (int)growing.TotalSeconds / 10)));
        }

        public enum BreedingMode
        {
            BestNextGen,
            TopStatsLucky,
            TopStatsConservative
        }
    }
}
