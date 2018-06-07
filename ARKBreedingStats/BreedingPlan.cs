using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.species;
using System.Threading;
using System.Threading.Tasks;

namespace ARKBreedingStats
{
    public partial class BreedingPlan : UserControl
    {
        public event PedigreeCreature.CreatureEditEventHandler EditCreature;
        public event PedigreeCreature.CreaturePartnerEventHandler BestBreedingPartners;
        public event PedigreeCreature.ExportToClipboardEventHandler exportToClipboard;
        public event Raising.createIncubationEventHandler createIncubationTimer;
        public event Form1.setMessageLabelTextEventHandler setMessageLabelText;
        private List<Creature> females = new List<Creature>();
        private List<Creature> males = new List<Creature>();
        private List<BreedingPair> breedingPairs;
        private string currentSpecies;
        private int speciesIndex;
        public double[] statWeights = new double[8]; // how much are the stats weighted when looking for the best
        private List<int> bestLevels = new List<int>();
        private List<PedigreeCreature> pcs = new List<PedigreeCreature>();
        private List<PictureBox> pbs = new List<PictureBox>();
        private bool[] enabledColorRegions;
        private TimeSpan incubationTime = TimeSpan.Zero;
        private Creature chosenCreature;
        private BreedingMode breedingMode;
        public StatWeighting statWeighting;
        public bool breedingPlanNeedsUpdate;
        public CreatureCollection creatureCollection;
        private CancellationTokenSource cancelSource;
        private ToolTip tt = new ToolTip();
        private double probHigherLvl = 0.55, probLowerLvl = 0.45; // probability of inheriting the higher level-stat

        public BreedingPlan()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            for (int i = 0; i < 8; i++)
                statWeights[i] = 1;

            breedingPairs = new List<BreedingPair>();
            pedigreeCreatureBest.IsVirtual = true;
            pedigreeCreatureWorst.IsVirtual = true;
            pedigreeCreatureBest.onlyLevels = true;
            pedigreeCreatureWorst.onlyLevels = true;
            pedigreeCreatureBest.Clear();
            pedigreeCreatureWorst.Clear();
            pedigreeCreatureBest.HandCursor = false;
            pedigreeCreatureWorst.HandCursor = false;

            statWeighting = statWeighting1;
            breedingPlanNeedsUpdate = false;

            tagSelectorList1.OnTagChanged += TagSelectorList1_OnTagChanged;
        }

        private void TagSelectorList1_OnTagChanged()
        {
            calculateBreedingScoresAndDisplayPairs();
        }

        public void bindEvents()
        {
            // has to be done after the BreedingPlan-class got the binding
            pedigreeCreatureBest.CreatureEdit += new PedigreeCreature.CreatureEditEventHandler(EditCreature);
            pedigreeCreatureWorst.CreatureEdit += new PedigreeCreature.CreatureEditEventHandler(EditCreature);
            pedigreeCreatureBest.exportToClipboard += new PedigreeCreature.ExportToClipboardEventHandler(exportToClipboard);
            pedigreeCreatureWorst.exportToClipboard += new PedigreeCreature.ExportToClipboardEventHandler(exportToClipboard);
            pedigreeCreatureBest.CreatureClicked += new PedigreeCreature.CreatureChangedEventHandler(CreatureClicked);
            pedigreeCreatureWorst.CreatureClicked += new PedigreeCreature.CreatureChangedEventHandler(CreatureClicked);
        }

        public void determineBestBreeding(Creature chosenCreature = null, bool forceUpdate = false)
        {
            if (creatureCollection == null) return;

            string selectedSpecies = (chosenCreature != null ? chosenCreature.species : "");
            bool newSpecies = false;
            if (selectedSpecies.Length == 0 && listViewSpeciesBP.SelectedIndices.Count > 0)
                selectedSpecies = listViewSpeciesBP.SelectedItems[0].Text;
            if (selectedSpecies.Length > 0 && CurrentSpecies != selectedSpecies)
            {
                CurrentSpecies = selectedSpecies;
                newSpecies = true;

                int s = Values.V.speciesNames.IndexOf(selectedSpecies);
                EnabledColorRegions = (s >= 0 ? Values.V.species[s].colors.Select(n => n.name != "").ToArray() : new bool[6] { true, true, true, true, true, true });

                breedingPlanNeedsUpdate = true;
            }
            if (forceUpdate || breedingPlanNeedsUpdate)
                Creatures = creatureCollection.creatures.Where(
                    c => c.species == selectedSpecies && c.status == CreatureStatus.Available && !c.neutered
                    && (cnBPIncludeCooldowneds.Checked || (c.cooldownUntil < DateTime.Now && c.growingUntil < DateTime.Now))
                    ).ToList();

            statWeights = statWeighting1.Weightings;

            this.chosenCreature = chosenCreature;
            calculateBreedingScoresAndDisplayPairs(breedingMode, newSpecies);
            breedingPlanNeedsUpdate = false;
        }

        public List<Creature> filterByTags(List<Creature> cl)
        {
            List<string> excludingTagList = tagSelectorList1.excludingTags;
            List<string> includingTagList = tagSelectorList1.includingTags;

            List<Creature> filteredList = new List<Creature>();

            if (excludingTagList.Count > 0)
            {
                foreach (Creature c in cl)
                {
                    bool exclude = cbBPTagExcludeDefault.Checked;
                    if (!exclude)
                    {
                        foreach (string t in c.tags)
                        {
                            if (excludingTagList.Contains(t))
                            {
                                exclude = true;
                                break;
                            }
                        }
                    }
                    if (exclude)
                    {
                        foreach (string t in c.tags)
                        {
                            if (includingTagList.Contains(t))
                            {
                                exclude = false;
                                break;
                            }
                        }
                    }
                    if (!exclude)
                        filteredList.Add(c);
                }
                return filteredList;
            }
            else
            {
                return cl;
            }
        }

        public void calculateBreedingScoresAndDisplayPairs()
        {
            calculateBreedingScoresAndDisplayPairs(breedingMode);
        }

        public async void calculateBreedingScoresAndDisplayPairs(BreedingMode breedingMode, bool updateBreedingData = false)
        {
            cancelSource?.Cancel();
            using (cancelSource = new CancellationTokenSource())
            {
                try
                {
                    await Task.Delay(400, cancelSource.Token); // recalculate breedingplan at most a certain interval
                    AsyncCalculateBreedingScoresAndDisplayPairs(breedingMode, updateBreedingData);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
            cancelSource = null;
        }

        private void AsyncCalculateBreedingScoresAndDisplayPairs(BreedingMode breedingMode, bool updateBreedingData = false)
        {
            SuspendLayout();
            this.SuspendDrawing();
            Cursor.Current = Cursors.WaitCursor;
            ClearControls();

            // chosen Creature (only consider this one for its sex)
            bool considerChosenCreature = chosenCreature != null;

            // filter by tags
            int crCountF = females.Count;
            int crCountM = males.Count;
            List<Creature> chosenF, chosenM;
            if (considerChosenCreature && chosenCreature.sex == Sex.Female)
                chosenF = new List<Creature>();
            else chosenF = filterByTags(females);
            if (considerChosenCreature && chosenCreature.sex == Sex.Male)
                chosenM = new List<Creature>();
            else chosenM = filterByTags(males);

            bool creaturesTagFilteredOut = (crCountF != chosenF.Count)
                                        || (crCountM != chosenM.Count);

            crCountF = chosenF.Count;
            crCountM = chosenM.Count;
            //if (nudBPMutationLimit.Value >= 0)
            //{
            //    chosenF = chosenF.Where(c => c.mutationsMaternal + c.mutationsPaternal <= nudBPMutationLimit.Value).ToList();
            //    chosenM = chosenM.Where(c => c.mutationsMaternal + c.mutationsPaternal <= nudBPMutationLimit.Value).ToList();
            //}
            bool creaturesMutationsFilteredOut = (crCountF != chosenF.Count)
                                              || (crCountM != chosenM.Count);

            if (considerChosenCreature)
            {
                if (chosenCreature.sex == Sex.Female)
                    chosenF.Add(chosenCreature);
                if (chosenCreature.sex == Sex.Male)
                    chosenM.Add(chosenCreature);
            }

            lbBreedingPlanHeader.Text = currentSpecies + (considerChosenCreature ? " (" + String.Format(Loc.s("onlyPairingsWith"), chosenCreature.name) + ")" : "");
            if (considerChosenCreature && (chosenCreature.neutered || chosenCreature.status != CreatureStatus.Available))
                lbBreedingPlanHeader.Text += "! " + Loc.s("BreedingNotPossible") + " ! (" + (chosenCreature.neutered ? Loc.s("Neutered") : Loc.s("notAvailable")) + ")";

            string warningText = "";
            if (creaturesTagFilteredOut) warningText = Loc.s("BPsomeCreaturesAreFilteredOutTags");
            if (creaturesMutationsFilteredOut) warningText += (warningText.Length > 0 ? " " + Loc.s("BPorMutations") : Loc.s("BPsomeCreaturesAreFilteredOutMutations"));
            if (warningText.Length > 0) setMessageLabelText(warningText + ".\n" + Loc.s("BPTopStatsShownMightNotTotalTopStats"), MessageBoxIcon.Warning);

            var combinedCreatures = new List<Creature>(chosenF);
            combinedCreatures.AddRange(chosenM);
            // determine top-stats for choosen creatures.
            int[] topStats = new int[7];
            foreach (Creature c in combinedCreatures)
            {
                for (int s = 0; s < 7; s++)
                {
                    if (topStats[s] < c.levelsWild[s])
                        topStats[s] = c.levelsWild[s];
                }
            }

            if (Properties.Settings.Default.IgnoreSexInBreedingPlan)
            {
                chosenF = new List<Creature>(combinedCreatures);
                chosenM = new List<Creature>(combinedCreatures);
            }

            if (chosenF.Count > 0 && chosenM.Count > 0)
            {
                pedigreeCreature1.Show();
                pedigreeCreature2.Show();
                lbBPBreedingScore.Show();

                breedingPairs.Clear();
                double t = 0, tt = 0, eTS;
                int nrTS;
                Int16[] bestPossLevels = new Int16[7]; // best possible levels

                foreach (Creature female in chosenF)
                {
                    foreach (Creature male in chosenM)
                    {
                        if (male == female // happens if Properties.Settings.Default.IgnoreSexInBreedingPlan (when using S+ mutator)
                            || (nudBPMutationLimit.Value >= 0 && female.Mutations > nudBPMutationLimit.Value && male.Mutations > nudBPMutationLimit.Value) // if one pair is below the limit, show this pair
                            ) continue;
                        t = 0;
                        nrTS = 0; // number of possible top-stats
                        eTS = 0; // expected number of top stats

                        for (int s = 0; s < 7; s++)
                        {
                            bestPossLevels[s] = 0;
                            int higherLevel = Math.Max(female.levelsWild[s], male.levelsWild[s]);
                            int lowerlevel = Math.Min(female.levelsWild[s], male.levelsWild[s]);
                            if (higherLevel < 0) higherLevel = 0;
                            if (lowerlevel < 0) lowerlevel = 0;

                            tt = statWeights[s] * (probHigherLvl * higherLevel + probLowerLvl * lowerlevel) / 40;
                            if (tt > 0)
                            {
                                if (breedingMode == BreedingMode.TopStatsLucky)
                                {
                                    if (female.levelsWild[s] == topStats[s] || male.levelsWild[s] == topStats[s])
                                    {
                                        if (female.levelsWild[s] == topStats[s] && male.levelsWild[s] == topStats[s])
                                            tt *= 1.142;
                                    }
                                    else if (bestLevels[s] > 0)
                                        tt *= .01;
                                }
                                else if (breedingMode == BreedingMode.TopStatsConservative && bestLevels[s] > 0)
                                {
                                    bestPossLevels[s] = (Int16)Math.Max(female.levelsWild[s], male.levelsWild[s]);
                                    tt *= .01;
                                    if (female.levelsWild[s] == topStats[s] || male.levelsWild[s] == topStats[s])
                                    {
                                        nrTS++;
                                        eTS += ((female.levelsWild[s] == topStats[s] && male.levelsWild[s] == topStats[s]) ? 1 : probHigherLvl);
                                    }
                                }
                            }
                            t += tt;
                        }

                        if (breedingMode == BreedingMode.TopStatsConservative)
                        {
                            if (female.topStatsCountBP < nrTS && male.topStatsCountBP < nrTS)
                                t += eTS;
                            else
                                t += .1 * eTS;
                            // check if the best possible stat outcome already exists in a male
                            bool maleExists = false;

                            foreach (Creature cr in chosenM)
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
                                t *= .4; // another male with the same stats is not worth much, the mating-cooldown of males is short.
                            else
                            {
                                // check if the best possible stat outcome already exists in a female
                                bool femaleExists = false;
                                foreach (Creature cr in chosenF)
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
                                    t *= .8; // another female with the same stats may be useful, but not so much in conservative breeding
                            }
                            //t *= 2; // scale conservative mode as it rather displays improvement, but only scarcely
                        }

                        breedingPairs.Add(new BreedingPair(female, male, t * 1.25));
                    }
                }

                breedingPairs = breedingPairs.OrderByDescending(p => p.BreedingScore).ToList();
                double minScore = (breedingPairs.Count > 0 ? breedingPairs[breedingPairs.Count - 1].BreedingScore : 0);
                if (minScore < 0)
                {
                    foreach (BreedingPair bp in breedingPairs)
                        bp.BreedingScore -= minScore;
                }

                // draw best parents
                int row = 0;
                // scrolloffsets
                int xS = AutoScrollPosition.X;
                int yS = AutoScrollPosition.Y;
                PedigreeCreature pc;
                Bitmap bm;
                Graphics g;
                PictureBox pb;

                for (int i = 0; i < breedingPairs.Count && i < creatureCollection.maxBreedingSuggestions; i++)
                {
                    if (2 * i < pcs.Count)
                    {
                        pcs[2 * i].Creature = breedingPairs[i].Female;
                        pcs[2 * i].enabledColorRegions = enabledColorRegions;
                        pcs[2 * i].comboId = i;
                        pcs[2 * i].Show();
                    }
                    else
                    {
                        pc = new PedigreeCreature(breedingPairs[i].Female, enabledColorRegions, i);
                        //pc.Location = new Point(10 + xS, 5 + 35 * row + yS);
                        pc.CreatureClicked += new PedigreeCreature.CreatureChangedEventHandler(CreatureClicked);
                        pc.CreatureEdit += new PedigreeCreature.CreatureEditEventHandler(CreatureEdit);
                        pc.BPRecalc += new PedigreeCreature.BPRecalcEventHandler(recalculateBreedingPlan);
                        pc.BestBreedingPartners += new PedigreeCreature.CreaturePartnerEventHandler(BestBreedingPartners);
                        pc.exportToClipboard += new PedigreeCreature.ExportToClipboardEventHandler(exportToClipboard);
                        flowLayoutPanelPairs.Controls.Add(pc);
                        pcs.Add(pc);
                    }

                    // draw score
                    if (i < pbs.Count)
                    {
                        pb = pbs[i];
                        pbs[i].Show();
                    }
                    else
                    {
                        pb = new PictureBox
                        {
                            Size = new Size(87, 35)
                        };
                        //pb.Location = new Point(308 + xS, 19 + 35 * row + yS);
                        pbs.Add(pb);
                        flowLayoutPanelPairs.Controls.Add(pb);
                    }

                    if (2 * i + 1 < pcs.Count)
                    {
                        pcs[2 * i + 1].Creature = breedingPairs[i].Male;
                        pcs[2 * i + 1].enabledColorRegions = enabledColorRegions;
                        pcs[2 * i + 1].comboId = i;
                        pcs[2 * i + 1].Show();
                    }
                    else
                    {
                        pc = new PedigreeCreature(breedingPairs[i].Male, enabledColorRegions, i);
                        //pc.Location = new Point(397 + xS, 5 + 35 * row + yS);
                        pc.CreatureClicked += new PedigreeCreature.CreatureChangedEventHandler(CreatureClicked);
                        pc.CreatureEdit += new PedigreeCreature.CreatureEditEventHandler(CreatureEdit);
                        pc.BPRecalc += new PedigreeCreature.BPRecalcEventHandler(recalculateBreedingPlan);
                        pc.BestBreedingPartners += new PedigreeCreature.CreaturePartnerEventHandler(BestBreedingPartners);
                        pc.exportToClipboard += new PedigreeCreature.ExportToClipboardEventHandler(exportToClipboard);
                        flowLayoutPanelPairs.Controls.Add(pc);
                        flowLayoutPanelPairs.SetFlowBreak(pc, true);
                        pcs.Add(pc);
                    }

                    bm = new Bitmap(pb.Width, pb.Height);
                    using (g = Graphics.FromImage(bm))
                    {
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        Brush br = new SolidBrush(Utils.getColorFromPercent((int)(breedingPairs[i].BreedingScore * 12.5), 0.5));
                        Brush brd = new SolidBrush(Utils.getColorFromPercent((int)(breedingPairs[i].BreedingScore * 12.5), -.2));
                        g.FillRectangle(brd, 0, 15, 87, 5);
                        g.FillRectangle(brd, 20, 10, 47, 15);
                        g.FillRectangle(br, 1, 16, 85, 3);
                        g.FillRectangle(br, 21, 11, 45, 13);
                        g.DrawString(breedingPairs[i].BreedingScore.ToString("N4"), new System.Drawing.Font("Microsoft Sans Serif", 8.25f), new System.Drawing.SolidBrush(System.Drawing.Color.Black), 24, 12);
                        pb.Image = bm;
                    }

                    row++;
                }
                // hide unused controls
                for (int i = breedingPairs.Count; i < creatureCollection.maxBreedingSuggestions && 2 * i + 1 < pcs.Count && i < pbs.Count; i++)
                {
                    pcs[2 * i].Hide();
                    pcs[2 * i + 1].Hide();
                    pbs[i].Hide();
                }

                if (updateBreedingData)
                    setBreedingData(currentSpecies);
                if (breedingPairs.Count > 0)
                {
                    setParents(0);

                    // if breeding mode is conservative and a creature with top-stats already exists, the scoring might seem off
                    if (breedingMode == BreedingMode.TopStatsConservative)
                    {
                        bool bestCreatureAlreadyAvailable = true;
                        Creature bestCreature = null;
                        List<Creature> choosenFemalesAndMales = chosenF.Concat(chosenM).ToList();
                        bool noWildSpeedLevels = Values.V.species[speciesIndex].NoImprintingForSpeed == true;
                        foreach (Creature cr in choosenFemalesAndMales)
                        {
                            bestCreatureAlreadyAvailable = true;
                            for (int s = 0; s < 7; s++)
                            {
                                if (!cr.topBreedingStats[s] && !(s == 6 && noWildSpeedLevels))
                                {
                                    bestCreatureAlreadyAvailable = false;
                                    break;
                                }
                            }
                            if (bestCreatureAlreadyAvailable)
                            {
                                bestCreature = cr;
                                break;
                            }
                        }

                        if (bestCreatureAlreadyAvailable)
                            setMessageLabelText(String.Format(Loc.s("AlreadyCreatureWithTopStats"), bestCreature.name, Utils.sexSymbol(bestCreature.sex)), MessageBoxIcon.Warning);
                    }
                }
                else
                    setParents(-1);
            }
            else
            {
                // hide unused controls
                pedigreeCreature1.Hide();
                pedigreeCreature2.Hide();
                lbBPBreedingScore.Hide();
                for (int i = 0; i < creatureCollection.maxBreedingSuggestions && 2 * i + 1 < pcs.Count && i < pbs.Count; i++)
                {
                    pcs[2 * i].Hide();
                    pcs[2 * i + 1].Hide();
                    pbs[i].Hide();
                }
                lbBreedingPlanInfo.Text = String.Format(Loc.s("NoPossiblePairingForSpeciesFound"), currentSpecies);
                lbBreedingPlanInfo.Visible = true;
                if (updateBreedingData)
                    setBreedingData(currentSpecies);
            }
            Cursor.Current = Cursors.Default;
            this.ResumeDrawing();
            ResumeLayout();
        }

        private void recalculateBreedingPlan()
        {
            determineBestBreeding(chosenCreature, true);
        }

        internal void updateIfNeeded()
        {
            if (breedingPlanNeedsUpdate)
                determineBestBreeding(chosenCreature);
        }

        public void ClearControls()
        {
            // hide unused controls
            for (int i = 0; i < creatureCollection.maxBreedingSuggestions && 2 * i + 1 < pcs.Count && i < pbs.Count; i++)
            {
                pcs[2 * i].Hide();
                pcs[2 * i + 1].Hide();
                pbs[i].Hide();
            }

            // remove controls outside of the limit
            if (pcs.Count > 2 * creatureCollection.maxBreedingSuggestions)
                for (int i = pcs.Count - 1; i > 2 * creatureCollection.maxBreedingSuggestions - 1 && i >= 0; i--)
                {
                    pcs[i].Dispose();
                    pcs.RemoveAt(i);
                }

            pedigreeCreatureBest.Clear();
            pedigreeCreatureWorst.Clear();
            lbBreedingPlanInfo.Visible = false;
            lbBPProbabilityBest.Text = "";
            offspringPossibilities1.Clear();
            setMessageLabelText("", MessageBoxIcon.None);
        }

        public void Clear()
        {
            ClearControls();
            setBreedingData();
            listViewRaisingTimes.Items.Clear();
            currentSpecies = "";
            males.Clear();
            females.Clear();
            lbBreedingPlanHeader.Text = Loc.s("SelectSpeciesBreedingPlanner");
        }

        private void setBreedingData(string species = "")
        {
            int si = Values.V.speciesNames.IndexOf(species);
            listViewRaisingTimes.Items.Clear();
            if (si < 0 || Values.V.species[si].breeding == null)
            {
                listViewRaisingTimes.Items.Add(Loc.s("naYet"));
                labelBreedingInfos.Text = "";
            }
            else
            {
                if (Raising.getRaisingTimes(speciesIndex, out string incubationMode, out incubationTime, out TimeSpan babyTime, out TimeSpan maturationTime, out TimeSpan nextMatingMin, out TimeSpan nextMatingMax))
                {
                    TimeSpan totalTime = incubationTime;
                    DateTime until = DateTime.Now.Add(totalTime);
                    string[] times = new string[] { incubationMode, incubationTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.shortTimeDate(until) };
                    listViewRaisingTimes.Items.Add(new ListViewItem(times));

                    totalTime += babyTime;
                    until = DateTime.Now.Add(totalTime);
                    times = new string[] { Loc.s("Baby"), babyTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.shortTimeDate(until) };
                    listViewRaisingTimes.Items.Add(new ListViewItem(times));

                    totalTime = incubationTime + maturationTime;
                    until = DateTime.Now.Add(totalTime);
                    times = new string[] { Loc.s("Maturation"), maturationTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.shortTimeDate(until) };
                    listViewRaisingTimes.Items.Add(new ListViewItem(times));

                    string eggInfo = Raising.eggTemperature(speciesIndex);
                    if (eggInfo.Length > 0)
                        eggInfo = "\n\n" + eggInfo;

                    labelBreedingInfos.Text = Loc.s("TimeBetweenMating") + ": " + nextMatingMin.ToString("d':'hh':'mm':'ss") + " to " + nextMatingMax.ToString("d':'hh':'mm':'ss")
                        + eggInfo;
                }
            }
        }

        public void createTagList()
        {
            tagSelectorList1.tags = creatureCollection.tags;
            foreach (string t in creatureCollection.tagsInclude)
                tagSelectorList1.setTagStatus(t, uiControls.TagSelector.tagStatus.include);
            foreach (string t in creatureCollection.tagsExclude)
                tagSelectorList1.setTagStatus(t, uiControls.TagSelector.tagStatus.exclude);
        }

        public List<Creature> Creatures
        {
            set
            {
                females = value.Where(c => c.sex == Sex.Female).ToList();
                males = value.Where(c => c.sex == Sex.Male).ToList();

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
            EditCreature?.Invoke(c, isVirtual);
        }

        private void setParents(int comboIndex)
        {
            if (comboIndex < 0 || comboIndex >= breedingPairs.Count)
            {
                pedigreeCreatureBest.Clear();
                pedigreeCreatureWorst.Clear();
                lbBreedingPlanInfo.Visible = false;
                lbBPProbabilityBest.Text = "";
                return;
            }

            int? levelStep = creatureCollection.getWildLevelStep();
            Creature crB = new Creature(currentSpecies, "", "", "", 0, new int[8], null, 100, true, levelStep: levelStep);
            Creature crW = new Creature(currentSpecies, "", "", "", 0, new int[8], null, 100, true, levelStep: levelStep);
            Creature mother = breedingPairs[comboIndex].Female;
            Creature father = breedingPairs[comboIndex].Male;
            crB.Mother = mother;
            crB.Father = father;
            crW.Mother = mother;
            crW.Father = father;
            double probabilityBest = 1;
            bool totalLevelUnknown = false; // if stats are unknown, total level is as well (==> oxygen, speed)
            for (int s = 0; s < 7; s++)
            {
                crB.levelsWild[s] = statWeights[s] < 0 ? Math.Min(mother.levelsWild[s], father.levelsWild[s]) : Math.Max(mother.levelsWild[s], father.levelsWild[s]);
                crB.valuesBreeding[s] = Stats.calculateValue(speciesIndex, s, crB.levelsWild[s], 0, true, 1, 0);
                crB.topBreedingStats[s] = (crB.levelsWild[s] == bestLevels[s]);
                crW.levelsWild[s] = statWeights[s] < 0 ? Math.Max(mother.levelsWild[s], father.levelsWild[s]) : Math.Min(mother.levelsWild[s], father.levelsWild[s]);
                crW.valuesBreeding[s] = Stats.calculateValue(speciesIndex, s, crW.levelsWild[s], 0, true, 1, 0);
                crW.topBreedingStats[s] = (crW.levelsWild[s] == bestLevels[s]);
                if (crB.levelsWild[s] == -1 || crW.levelsWild[s] == -1)
                    totalLevelUnknown = true;
                if (crB.levelsWild[s] > crW.levelsWild[s])
                    probabilityBest *= .7;
            }
            crB.levelsWild[7] = crB.levelsWild.Sum();
            crW.levelsWild[7] = crW.levelsWild.Sum();
            crB.name = Loc.s("BestPossible");
            crW.name = Loc.s("WorstPossible");
            crB.recalculateCreatureValues(levelStep);
            crW.recalculateCreatureValues(levelStep);
            pedigreeCreatureBest.totalLevelUnknown = totalLevelUnknown;
            pedigreeCreatureWorst.totalLevelUnknown = totalLevelUnknown;
            int mutationCounterMaternal = mother.Mutations;
            int mutationCounterPaternal = father.Mutations;
            crB.mutationsMaternal = mutationCounterMaternal;
            crB.mutationsPaternal = mutationCounterPaternal;
            crW.mutationsMaternal = mutationCounterMaternal;
            crW.mutationsPaternal = mutationCounterPaternal;
            pedigreeCreatureBest.Creature = crB;
            pedigreeCreatureWorst.Creature = crW;
            lbBPProbabilityBest.Text = Loc.s("ProbabilityForBest") + ": " + Math.Round(100 * probabilityBest, 1).ToString() + "%";

            // set probability barChart
            offspringPossibilities1.wildLevels1 = mother.levelsWild;
            offspringPossibilities1.wildLevels2 = father.levelsWild;
            offspringPossibilities1.calculate();

            // highlight parents
            int hiliId = comboIndex * 2;
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

        private void buttonJustMated_Click(object sender, EventArgs e)
        {
            createIncubationEntry();
        }

        public string CurrentSpecies
        {
            set
            {
                currentSpecies = value;
                speciesIndex = Values.V.speciesNames.IndexOf(currentSpecies);
            }
            get { return currentSpecies; }
        }

        public void setSpecies(string species)
        {
            for (int i = 0; i < listViewSpeciesBP.Items.Count; i++)
            {
                if ((string)listViewSpeciesBP.Items[i].Text == species)
                {
                    listViewSpeciesBP.Items[i].Focused = true;
                    listViewSpeciesBP.Items[i].Selected = true;
                    break;
                }
            }
        }

        public int maxWildLevels { set { offspringPossibilities1.maxWildLevel = value; } }

        private void buttonApplyNewWeights_Click(object sender, EventArgs e)
        {
            determineBestBreeding();
        }

        private void listViewSpeciesBP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewSpeciesBP.SelectedIndices.Count > 0)
                determineBestBreeding();
        }

        private void checkBoxIncludeCooldowneds_CheckedChanged(object sender, EventArgs e)
        {
            determineBestBreeding(chosenCreature, true);
        }

        private void nudMutationLimit_ValueChanged(object sender, EventArgs e)
        {
            determineBestBreeding(chosenCreature, true);
        }

        private void radioButtonBPTopStatsCn_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBPTopStatsCn.Checked)
            {
                breedingMode = BreedingMode.TopStatsConservative;
                calculateBreedingScoresAndDisplayPairs();
            }
        }

        private void radioButtonBPTopStats_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBPTopStats.Checked)
            {
                breedingMode = BreedingMode.TopStatsLucky;
                calculateBreedingScoresAndDisplayPairs();
            }
        }

        private void radioButtonBPHighStats_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBPHighStats.Checked)
            {
                breedingMode = BreedingMode.BestNextGen;
                calculateBreedingScoresAndDisplayPairs();
            }
        }

        public void setSpeciesList(List<string> speciesNames, List<Creature> creatures)
        {
            // set the same species to breedingplaner, except the 'all'
            string selectedSpecies = "";
            if (listViewSpeciesBP.SelectedIndices.Count > 0)
                selectedSpecies = listViewSpeciesBP.SelectedIndices[0].ToString();
            listViewSpeciesBP.Items.Clear();

            ListViewItem lvi;
            foreach (string species in speciesNames)
            {
                int si = Values.V.speciesNames.IndexOf(species);
                lvi = new ListViewItem(species);
                // check if species has both available males and females
                if (si < 0 || Values.V.species[si].breeding == null || creatures.Count(c => c.species == species && c.status == CreatureStatus.Available && c.sex == Sex.Female) == 0 || creatures.Count(c => c.species == species && c.status == CreatureStatus.Available && c.sex == Sex.Male) == 0)
                    lvi.ForeColor = Color.LightGray;
                listViewSpeciesBP.Items.Add(lvi);
            }

            // select previous selecteded again
            if (selectedSpecies.Length > 0)
            {
                for (int i = 0; i < listViewSpeciesBP.Items.Count; i++)
                {
                    if ((string)listViewSpeciesBP.Items[i].Text == selectedSpecies)
                    {
                        listViewSpeciesBP.Items[i].Focused = true;
                        listViewSpeciesBP.Items[i].Selected = true;
                        break;
                    }
                }
            }
        }

        private void createIncubationEntry(bool startNow = true)
        {
            if (pedigreeCreatureBest.Creature != null && pedigreeCreatureBest.Creature.Mother != null && pedigreeCreatureBest.Creature.Father != null)
            {
                createIncubationTimer?.Invoke(pedigreeCreatureBest.Creature.Mother, pedigreeCreatureBest.Creature.Father, incubationTime, startNow);

                // set cooldown for mother
                int sI = Values.V.speciesNames.IndexOf(pedigreeCreatureBest.Creature.Mother.species);
                if (sI >= 0 && Values.V.species[sI].breeding != null)
                {
                    pedigreeCreatureBest.Creature.Mother.cooldownUntil = DateTime.Now.AddSeconds(Values.V.species[sI].breeding.matingCooldownMinAdjusted);
                    // update breeding plan
                    determineBestBreeding(chosenCreature, true);
                }
            }
        }

        public void updateBreedingData()
        {
            setBreedingData(currentSpecies);
        }

        public int MutationLimit
        {
            set { nudBPMutationLimit.Value = value; }
            get { return (int)nudBPMutationLimit.Value; }
        }

        public enum BreedingMode
        {
            BestNextGen,
            TopStatsLucky,
            TopStatsConservative
        }

        private void cbTagExcludeDefault_CheckedChanged(object sender, EventArgs e)
        {
            calculateBreedingScoresAndDisplayPairs();
        }

        public void SetLocalizations()
        {
            Loc.ControlText(lbBreedingPlanHeader, "SelectSpeciesBreedingPlanner");
            Loc.ControlText(gbBPOffspring);
            Loc.ControlText(gbBPBreedingMode);
            Loc.ControlText(rbBPTopStatsCn);
            Loc.ControlText(rbBPTopStats);
            Loc.ControlText(rbBPHighStats);
            Loc.ControlText(cnBPIncludeCooldowneds);
            Loc.ControlText(btBPApplyNewWeights);
            Loc.ControlText(gbBPBreedingMode);
            Loc.ControlText(lbBPBreedingTimes);
            Loc.ControlText(btBPJustMated);
            columnHeader2.Text = Loc.s("Time");
            columnHeader3.Text = Loc.s("TotalTime");
            columnHeader4.Text = Loc.s("FinishedAt");

            // tooltips
            Loc.ControlText(lbBPBreedingScore, tt);
            Loc.ControlText(rbBPTopStatsCn, tt);
            Loc.ControlText(rbBPTopStats, tt);
            Loc.ControlText(rbBPHighStats, tt);
            Loc.ControlText(btBPJustMated, tt);
            Loc.setToolTip(nudBPMutationLimit, tt);
            Loc.setToolTip(cbBPTagExcludeDefault, tt);
        }
    }
}
