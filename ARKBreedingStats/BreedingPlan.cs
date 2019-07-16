using ARKBreedingStats.raising;
using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class BreedingPlan : UserControl
    {
        public event PedigreeCreature.CreatureEditEventHandler EditCreature;
        public event PedigreeCreature.CreaturePartnerEventHandler BestBreedingPartners;
        public event PedigreeCreature.ExportToClipboardEventHandler exportToClipboard;
        public event Raising.createIncubationEventHandler createIncubationTimer;
        public event Form1.SetMessageLabelTextEventHandler setMessageLabelText;
        public event Form1.SetSpeciesEventHandler SetGlobalSpecies;
        private List<Creature> females = new List<Creature>();
        private List<Creature> males = new List<Creature>();
        private List<BreedingPair> breedingPairs;
        private Species currentSpecies;
        public double[] statWeights = new double[Values.STATS_COUNT]; // how much are the stats weighted when looking for the best
        private readonly List<int> bestLevels = new List<int>();
        private readonly List<PedigreeCreature> pcs = new List<PedigreeCreature>();
        private readonly List<PictureBox> pbs = new List<PictureBox>();
        private bool[] enabledColorRegions;
        private TimeSpan incubationTime = TimeSpan.Zero;
        private Creature chosenCreature;
        private BreedingMode breedingMode;
        public readonly StatWeighting statWeighting;
        public bool breedingPlanNeedsUpdate;
        private bool dontUpdateBreedingPlan; // set to true if settings are changed and update should only performed after that
        public CreatureCollection creatureCollection;
        private CancellationTokenSource cancelSource;
        private ToolTip tt = new ToolTip();
        private const double probHigherLvl = 0.55; // probability of inheriting the higher level-stat
        private const double probLowerLvl = 0.45; // probability of inheriting the higher level-stat

        public BreedingPlan()
        {
            InitializeComponent();
            dontUpdateBreedingPlan = true;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            for (int i = 0; i < Values.STATS_COUNT; i++)
                statWeights[i] = 1;

            breedingMode = BreedingMode.TopStatsConservative;

            breedingPairs = new List<BreedingPair>();
            pedigreeCreatureBest.IsVirtual = true;
            pedigreeCreatureWorst.IsVirtual = true;
            pedigreeCreatureBestPossibleInSpecies.IsVirtual = true;
            pedigreeCreatureBest.onlyLevels = true;
            pedigreeCreatureWorst.onlyLevels = true;
            pedigreeCreatureBestPossibleInSpecies.onlyLevels = true;
            pedigreeCreatureBest.Clear();
            pedigreeCreatureWorst.Clear();
            pedigreeCreatureBestPossibleInSpecies.Clear();
            pedigreeCreatureBest.HandCursor = false;
            pedigreeCreatureWorst.HandCursor = false;
            pedigreeCreatureBestPossibleInSpecies.HandCursor = false;

            statWeighting = statWeighting1;
            breedingPlanNeedsUpdate = false;

            cbServerFilterLibrary.Checked = Properties.Settings.Default.UseServerFilterForBreedingPlan;
            cbOwnerFilterLibrary.Checked = Properties.Settings.Default.UseOwnerFilterForBreedingPlan;
            cbBPIncludeCooldowneds.Checked = Properties.Settings.Default.IncludeCooldownsInBreedingPlan;

            tagSelectorList1.OnTagChanged += TagSelectorList1_OnTagChanged;
            dontUpdateBreedingPlan = false;
        }

        private void TagSelectorList1_OnTagChanged()
        {
            calculateBreedingScoresAndDisplayPairs();
        }

        public void bindSubControlEvents()
        {
            // has to be done after the BreedingPlan-class got the binding
            pedigreeCreatureBest.CreatureEdit += EditCreature;
            pedigreeCreatureWorst.CreatureEdit += EditCreature;
            pedigreeCreatureBestPossibleInSpecies.CreatureEdit += EditCreature;
            pedigreeCreatureBest.exportToClipboard += exportToClipboard;
            pedigreeCreatureWorst.exportToClipboard += exportToClipboard;
            pedigreeCreatureBestPossibleInSpecies.exportToClipboard += exportToClipboard;
            pedigreeCreatureBest.CreatureClicked += CreatureClicked;
            pedigreeCreatureWorst.CreatureClicked += CreatureClicked;
        }

        public void determineBestBreeding(Creature chosenCreature = null, bool forceUpdate = false, Species setSpecies = null)
        {
            if (creatureCollection == null) return;

            Species selectedSpecies = (chosenCreature != null ? chosenCreature.Species : null);
            bool newSpecies = false;
            if (selectedSpecies == null)
                selectedSpecies = setSpecies != null ? setSpecies : currentSpecies;
            if (selectedSpecies != null && currentSpecies != selectedSpecies)
            {
                CurrentSpecies = selectedSpecies;
                newSpecies = true;

                EnabledColorRegions = currentSpecies?.colors.Select(n => n.name != "").ToArray() ?? new bool[6] { true, true, true, true, true, true };

                breedingPlanNeedsUpdate = true;
            }
            if (forceUpdate || breedingPlanNeedsUpdate)
                Creatures = creatureCollection.creatures
                        .Where(c => c.speciesBlueprint == currentSpecies.blueprintPath &&
                                c.status == CreatureStatus.Available &&
                                !c.neutered &&
                                (cbBPIncludeCooldowneds.Checked || c.cooldownUntil < DateTime.Now && c.growingUntil < DateTime.Now))
                        .ToList();

            statWeights = statWeighting1.Weightings;

            this.chosenCreature = chosenCreature;
            calculateBreedingScoresAndDisplayPairs(breedingMode, newSpecies);
            breedingPlanNeedsUpdate = false;
        }

        private List<Creature> filterByTags(List<Creature> cl)
        {
            List<string> excludingTagList = tagSelectorList1.excludingTags;
            List<string> includingTagList = tagSelectorList1.includingTags;

            List<Creature> filteredList = new List<Creature>();

            if (excludingTagList.Count > 0 || cbBPTagExcludeDefault.Checked)
            {
                foreach (Creature c in cl)
                {
                    bool exclude = cbBPTagExcludeDefault.Checked;
                    if (!exclude && excludingTagList.Count > 0)
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
                    if (exclude && includingTagList.Count > 0)
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
            return cl;
        }

        private void calculateBreedingScoresAndDisplayPairs()
        {
            if (!dontUpdateBreedingPlan)
                calculateBreedingScoresAndDisplayPairs(breedingMode);
        }

        private async void calculateBreedingScoresAndDisplayPairs(BreedingMode breedingMode, bool updateBreedingData = false)
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

            // filter by servers
            if (cbServerFilterLibrary.Checked)
            {
                chosenF = chosenF.Where(c => (c.server == "" && !creatureCollection.hiddenServers.Contains("n/a"))
                                              || (c.server != "" && !creatureCollection.hiddenServers.Contains(c.server))).ToList();
                chosenM = chosenM.Where(c => (c.server == "" && !creatureCollection.hiddenServers.Contains("n/a"))
                                              || (c.server != "" && !creatureCollection.hiddenServers.Contains(c.server))).ToList();
            }
            // filter by owner
            if (cbOwnerFilterLibrary.Checked)
            {
                chosenF = chosenF.Where(c => (c.owner == "" && !creatureCollection.hiddenOwners.Contains("n/a"))
                                              || (c.owner != "" && !creatureCollection.hiddenOwners.Contains(c.owner))).ToList();
                chosenM = chosenM.Where(c => (c.owner == "" && !creatureCollection.hiddenOwners.Contains("n/a"))
                                              || (c.owner != "" && !creatureCollection.hiddenOwners.Contains(c.owner))).ToList();
            }

            bool creaturesTagFilteredOut = (crCountF != chosenF.Count)
                                        || (crCountM != chosenM.Count);

            crCountF = chosenF.Count;
            crCountM = chosenM.Count;
            if (nudBPMutationLimit.Value >= 0)
            {
                chosenF = chosenF.Where(c => c.Mutations <= nudBPMutationLimit.Value).ToList();
                chosenM = chosenM.Where(c => c.Mutations <= nudBPMutationLimit.Value).ToList();
            }
            bool creaturesMutationsFilteredOut = (crCountF != chosenF.Count)
                                              || (crCountM != chosenM.Count);

            if (considerChosenCreature)
            {
                if (chosenCreature.sex == Sex.Female)
                    chosenF.Add(chosenCreature);
                if (chosenCreature.sex == Sex.Male)
                    chosenM.Add(chosenCreature);
            }

            lbBreedingPlanHeader.Text = currentSpecies.NameAndMod + (considerChosenCreature ? " (" + string.Format(Loc.s("onlyPairingsWith"), chosenCreature.name) + ")" : "");
            if (considerChosenCreature && (chosenCreature.neutered || chosenCreature.status != CreatureStatus.Available))
                lbBreedingPlanHeader.Text += $"! {Loc.s("BreedingNotPossible")} ! ({(chosenCreature.neutered ? Loc.s("Neutered") : Loc.s("notAvailable"))})";

            string warningText = "";
            if (creaturesTagFilteredOut) warningText = Loc.s("BPsomeCreaturesAreFilteredOutTags");
            if (creaturesMutationsFilteredOut) warningText += (warningText.Length > 0 ? " " + Loc.s("BPorMutations") : Loc.s("BPsomeCreaturesAreFilteredOutMutations"));
            if (warningText.Length > 0) setMessageLabelText(warningText + ".\n" + Loc.s("BPTopStatsShownMightNotTotalTopStats"), MessageBoxIcon.Warning);

            var combinedCreatures = new List<Creature>(chosenF);
            combinedCreatures.AddRange(chosenM);
            // determine top-stats for choosen creatures.
            int[] topStats = new int[Values.STATS_COUNT];
            foreach (Creature c in combinedCreatures)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
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
                short[] bestPossLevels = new short[Values.STATS_COUNT]; // best possible levels

                foreach (Creature female in chosenF)
                {
                    foreach (Creature male in chosenM)
                    {
                        if (male == female // happens if Properties.Settings.Default.IgnoreSexInBreedingPlan (when using S+ mutator)
                            || (nudBPMutationLimit.Value >= 0 && female.Mutations > nudBPMutationLimit.Value && male.Mutations > nudBPMutationLimit.Value) // if one pair is below the limit, show this pair
                            ) continue;
                        double t = 0;
                        int nrTS = 0;
                        double eTS = 0;

                        int topfemale = 0;
                        int topmale = 0;

                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            if (s == (int)StatNames.Torpidity) continue;
                            bestPossLevels[s] = 0;
                            int higherLevel = Math.Max(female.levelsWild[s], male.levelsWild[s]);
                            int lowerlevel = Math.Min(female.levelsWild[s], male.levelsWild[s]);
                            if (higherLevel < 0) higherLevel = 0;
                            if (lowerlevel < 0) lowerlevel = 0;

                            double tt = statWeights[s] * (probHigherLvl * higherLevel + probLowerLvl * lowerlevel) / 40;
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
                                    bestPossLevels[s] = (short)Math.Max(female.levelsWild[s], male.levelsWild[s]);
                                    tt *= .01;
                                    if (female.levelsWild[s] == topStats[s] || male.levelsWild[s] == topStats[s])
                                    {
                                        nrTS++;
                                        eTS += female.levelsWild[s] == topStats[s] && male.levelsWild[s] == topStats[s] ? 1 : probHigherLvl;
                                        if (female.levelsWild[s] == topStats[s])
                                            topfemale++;
                                        if (male.levelsWild[s] == topStats[s])
                                            topmale++;
                                    }
                                }
                            }
                            t += tt;
                        }

                        if (breedingMode == BreedingMode.TopStatsConservative)
                        {
                            if (topfemale < nrTS && topmale < nrTS)
                                t += eTS;
                            else
                                t += .1 * eTS;
                            // check if the best possible stat outcome already exists in a male
                            bool maleExists = false;

                            foreach (Creature cr in chosenM)
                            {
                                maleExists = true;
                                for (int s = 0; s < Values.STATS_COUNT; s++)
                                {
                                    if (s == (int)StatNames.Torpidity || cr.valuesDom[s] == 0) continue; // TODO check if stat is used with cr.species.statsUsed[s]
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
                                    for (int s = 0; s < Values.STATS_COUNT; s++)
                                    {
                                        if (s == (int)StatNames.Torpidity || cr.valuesDom[s] == 0) continue; // TODO check if stat is used with cr.species.statsUsed[s]
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

                for (int i = 0; i < breedingPairs.Count && i < creatureCollection.maxBreedingSuggestions; i++)
                {
                    PedigreeCreature pc;
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
                        pc.CreatureClicked += CreatureClicked;
                        pc.CreatureEdit += CreatureEdit;
                        pc.BPRecalc += recalculateBreedingPlan;
                        pc.BestBreedingPartners += BestBreedingPartners;
                        pc.exportToClipboard += exportToClipboard;
                        flowLayoutPanelPairs.Controls.Add(pc);
                        pcs.Add(pc);
                    }

                    // draw score
                    PictureBox pb;
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
                        pc.CreatureClicked += CreatureClicked;
                        pc.CreatureEdit += CreatureEdit;
                        pc.BPRecalc += recalculateBreedingPlan;
                        pc.BestBreedingPartners += BestBreedingPartners;
                        pc.exportToClipboard += exportToClipboard;
                        flowLayoutPanelPairs.Controls.Add(pc);
                        flowLayoutPanelPairs.SetFlowBreak(pc, true);
                        pcs.Add(pc);
                    }

                    Bitmap bm = new Bitmap(pb.Width, pb.Height);
                    Graphics g;
                    using (g = Graphics.FromImage(bm))
                    {
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        Brush br = new SolidBrush(Utils.getColorFromPercent((int)(breedingPairs[i].BreedingScore * 12.5), 0.5));
                        Brush brd = new SolidBrush(Utils.getColorFromPercent((int)(breedingPairs[i].BreedingScore * 12.5), -.2));
                        g.FillRectangle(brd, 0, 15, 87, 5);
                        g.FillRectangle(brd, 20, 10, 47, 15);
                        g.FillRectangle(br, 1, 16, 85, 3);
                        g.FillRectangle(br, 21, 11, 45, 13);
                        g.DrawString(breedingPairs[i].BreedingScore.ToString("N4"), new Font("Microsoft Sans Serif", 8.25f), new SolidBrush(Color.Black), 24, 12);
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
                        bool noWildSpeedLevels = currentSpecies.NoImprintingForSpeed == true;
                        foreach (Creature cr in choosenFemalesAndMales)
                        {
                            bestCreatureAlreadyAvailable = true;
                            for (int s = 0; s < Values.STATS_COUNT; s++)
                            {
                                if (!cr.topBreedingStats[s] && !(s == (int)StatNames.SpeedMultiplier && noWildSpeedLevels))
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
                            setMessageLabelText(string.Format(Loc.s("AlreadyCreatureWithTopStats"), bestCreature.name, Utils.sexSymbol(bestCreature.sex)), MessageBoxIcon.Warning);
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
                lbBreedingPlanInfo.Text = string.Format(Loc.s("NoPossiblePairingForSpeciesFound"), currentSpecies);
                lbBreedingPlanInfo.Visible = true;
                if (updateBreedingData)
                    setBreedingData(currentSpecies);
            }
            Cursor.Current = Cursors.Default;
            this.ResumeDrawing();

            if (considerChosenCreature) btShowAllCreatures.Text = "Unset Restriction to " + chosenCreature.name;
            btShowAllCreatures.Visible = considerChosenCreature;
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
            currentSpecies = null;
            males.Clear();
            females.Clear();
            lbBreedingPlanHeader.Text = Loc.s("SelectSpeciesBreedingPlanner");
        }

        private void setBreedingData(Species species = null)
        {
            listViewRaisingTimes.Items.Clear();
            if (species == null || species.breeding == null)
            {
                listViewRaisingTimes.Items.Add(Loc.s("naYet"));
                labelBreedingInfos.Text = "";
            }
            else
            {
                bool isGlowSpecies = Values.V.IsGlowSpecies(species.name);
                pedigreeCreature1.IsGlowSpecies = isGlowSpecies;
                pedigreeCreature2.IsGlowSpecies = isGlowSpecies;
                if (Raising.getRaisingTimes(species, out string incubationMode, out incubationTime, out TimeSpan babyTime, out TimeSpan maturationTime, out TimeSpan nextMatingMin, out TimeSpan nextMatingMax))
                {
                    TimeSpan totalTime = incubationTime;
                    DateTime until = DateTime.Now.Add(totalTime);
                    string[] times = { incubationMode, incubationTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.shortTimeDate(until) };
                    listViewRaisingTimes.Items.Add(new ListViewItem(times));

                    totalTime += babyTime;
                    until = DateTime.Now.Add(totalTime);
                    times = new[] { Loc.s("Baby"), babyTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.shortTimeDate(until) };
                    listViewRaisingTimes.Items.Add(new ListViewItem(times));

                    totalTime = incubationTime + maturationTime;
                    until = DateTime.Now.Add(totalTime);
                    times = new[] { Loc.s("Maturation"), maturationTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.shortTimeDate(until) };
                    listViewRaisingTimes.Items.Add(new ListViewItem(times));

                    string eggInfo = Raising.eggTemperature(species);
                    if (eggInfo.Length > 0)
                        eggInfo = "\n\n" + eggInfo;

                    labelBreedingInfos.Text = $"{Loc.s("TimeBetweenMating")}: {nextMatingMin:d':'hh':'mm':'ss} to {nextMatingMax:d':'hh':'mm':'ss}{eggInfo}";
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
                for (int s = 0; s < Values.STATS_COUNT; s++)
                    bestLevels.Add(-1);

                foreach (Creature c in value)
                {
                    for (int s = 0; s < Values.STATS_COUNT; s++)
                    {
                        if ((s == (int)StatNames.Torpidity || statWeights[s] > 0) && c.levelsWild[s] > bestLevels[s])
                            bestLevels[s] = c.levelsWild[s];
                        else if (s != (int)StatNames.Torpidity && statWeights[s] < 0 && c.levelsWild[s] >= 0 && (c.levelsWild[s] < bestLevels[s] || bestLevels[s] < 0))
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
            Creature crB = new Creature(currentSpecies, "", "", "", 0, new int[Values.STATS_COUNT], null, 100, true, levelStep: levelStep);
            Creature crW = new Creature(currentSpecies, "", "", "", 0, new int[Values.STATS_COUNT], null, 100, true, levelStep: levelStep);
            Creature mother = breedingPairs[comboIndex].Female;
            Creature father = breedingPairs[comboIndex].Male;
            crB.Mother = mother;
            crB.Father = father;
            crW.Mother = mother;
            crW.Father = father;
            double probabilityBest = 1;
            bool totalLevelUnknown = false; // if stats are unknown, total level is as well (==> oxygen, speed)
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s == (int)StatNames.Torpidity) continue;
                crB.levelsWild[s] = statWeights[s] < 0 ? Math.Min(mother.levelsWild[s], father.levelsWild[s]) : Math.Max(mother.levelsWild[s], father.levelsWild[s]);
                crB.valuesBreeding[s] = Stats.calculateValue(currentSpecies, s, crB.levelsWild[s], 0, true, 1, 0);
                crB.topBreedingStats[s] = (crB.levelsWild[s] == bestLevels[s]);
                crW.levelsWild[s] = statWeights[s] < 0 ? Math.Max(mother.levelsWild[s], father.levelsWild[s]) : Math.Min(mother.levelsWild[s], father.levelsWild[s]);
                crW.valuesBreeding[s] = Stats.calculateValue(currentSpecies, s, crW.levelsWild[s], 0, true, 1, 0);
                crW.topBreedingStats[s] = (crW.levelsWild[s] == bestLevels[s]);
                if (crB.levelsWild[s] == -1 || crW.levelsWild[s] == -1)
                    totalLevelUnknown = true;
                if (crB.levelsWild[s] > crW.levelsWild[s])
                    probabilityBest *= probHigherLvl;
            }
            crB.levelsWild[(int)StatNames.Torpidity] = crB.levelsWild.Sum();
            crW.levelsWild[(int)StatNames.Torpidity] = crW.levelsWild.Sum();
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
            lbBPProbabilityBest.Text = $"{Loc.s("ProbabilityForBest")}: {Math.Round(100 * probabilityBest, 1)}%";

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
                    enabledColorRegions = new[] { true, true, true, true, true, true };
                }
            }
        }

        private void buttonJustMated_Click(object sender, EventArgs e)
        {
            createIncubationEntry();
        }

        public Species CurrentSpecies
        {
            get => currentSpecies;
            set
            {
                currentSpecies = value;
                statWeighting1.currentSpeciesName = value?.name ?? string.Empty;
            }
        }

        private void listViewSpeciesBP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewSpeciesBP.SelectedIndices.Count > 0
                && listViewSpeciesBP.SelectedItems[0].Text != currentSpecies.NameAndMod)
            {
                SetGlobalSpecies?.Invoke((Species)((ListViewItem)listViewSpeciesBP.SelectedItems[0]).Tag);
            }
        }

        public int maxWildLevels
        {
            set => offspringPossibilities1.maxWildLevel = value;
        }

        private void buttonApplyNewWeights_Click(object sender, EventArgs e)
        {
            determineBestBreeding();
        }

        public void SetSpecies(Species species)
        {
            if (currentSpecies == species) return;

            // automatically set preset if preset with the speciesname exists
            dontUpdateBreedingPlan = true;
            if (!statWeighting1.SelectPresetByName(species.name))
                statWeighting1.SelectPresetByName("Default");
            dontUpdateBreedingPlan = false;

            determineBestBreeding(setSpecies: species);

            if (bestLevels.Count > 6)
            {
                // display top levels in species
                int? levelStep = creatureCollection.getWildLevelStep();
                Creature crB = new Creature(currentSpecies, "", "", "", 0, new int[Values.STATS_COUNT], null, 100, true, levelStep: levelStep);
                crB.name = "Best possible " + currentSpecies.name + " for this library";
                bool totalLevelUnknown = false;
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (s == (int)StatNames.Torpidity) continue;
                    crB.levelsWild[s] = bestLevels[s];
                    crB.valuesBreeding[s] = Stats.calculateValue(currentSpecies, s, crB.levelsWild[s], 0, true, 1, 0);
                    if (crB.levelsWild[s] == -1)
                        totalLevelUnknown = true;
                    crB.topBreedingStats[s] = (crB.levelsWild[s] > 0);
                }
                crB.levelsWild[(int)StatNames.Torpidity] = crB.levelsWild.Sum();
                crB.recalculateCreatureValues(levelStep);
                pedigreeCreatureBestPossibleInSpecies.totalLevelUnknown = totalLevelUnknown;
                pedigreeCreatureBestPossibleInSpecies.Creature = crB;
            }

            //// update listviewSpeciesBP
            // deselect currently selected species
            if (listViewSpeciesBP.SelectedItems.Count > 0)
                listViewSpeciesBP.SelectedItems[0].Selected = false;
            for (int i = 0; i < listViewSpeciesBP.Items.Count; i++)
            {
                if (listViewSpeciesBP.Items[i].Text == currentSpecies.NameAndMod)
                {
                    listViewSpeciesBP.Items[i].Focused = true;
                    listViewSpeciesBP.Items[i].Selected = true;
                    break;
                }
            }
        }

        private void checkBoxIncludeCooldowneds_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IncludeCooldownsInBreedingPlan = cbBPIncludeCooldowneds.Checked;
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

        public void setSpeciesList(List<Species> species, List<Creature> creatures)
        {
            string selectedSpeciesName = "";
            if (listViewSpeciesBP.SelectedIndices.Count > 0)
                selectedSpeciesName = listViewSpeciesBP.SelectedIndices[0].ToString();
            listViewSpeciesBP.Items.Clear();

            foreach (Species s in species)
            {
                ListViewItem lvi = new ListViewItem { Text = s.NameAndMod, Tag = s };
                // check if species has both available males and females
                if (s == null || s.breeding == null || !creatures.Any(c => c.Species == s && c.status == CreatureStatus.Available && c.sex == Sex.Female) || !creatures.Any(c => c.Species == s && c.status == CreatureStatus.Available && c.sex == Sex.Male))
                    lvi.ForeColor = Color.LightGray;
                listViewSpeciesBP.Items.Add(lvi);
            }

            // select previous selecteded again
            if (selectedSpeciesName.Length > 0)
            {
                for (int i = 0; i < listViewSpeciesBP.Items.Count; i++)
                {
                    if (listViewSpeciesBP.Items[i].Text == selectedSpeciesName)
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
            if (pedigreeCreatureBest.Creature?.Mother != null && pedigreeCreatureBest.Creature.Father != null)
            {
                createIncubationTimer?.Invoke(pedigreeCreatureBest.Creature.Mother, pedigreeCreatureBest.Creature.Father, incubationTime, startNow);

                // set cooldown for mother
                Species species = pedigreeCreatureBest.Creature.Mother.Species;
                if (species?.breeding != null)
                {
                    pedigreeCreatureBest.Creature.Mother.cooldownUntil = DateTime.Now.AddSeconds(species.breeding.matingCooldownMinAdjusted);
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
            get => (int)nudBPMutationLimit.Value;
            set => nudBPMutationLimit.Value = value;
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
            Loc.ControlText(cbBPIncludeCooldowneds);
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

        private void cbServerFilterLibrary_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UseServerFilterForBreedingPlan = cbServerFilterLibrary.Checked;
            calculateBreedingScoresAndDisplayPairs();
        }

        private void btShowAllCreatures_Click(object sender, EventArgs e)
        {
            // remove restriction on one creature TODO
            chosenCreature = null;
            calculateBreedingScoresAndDisplayPairs();
        }

        private void cbOwnerFilterLibrary_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UseOwnerFilterForBreedingPlan = cbOwnerFilterLibrary.Checked;
            calculateBreedingScoresAndDisplayPairs();
        }
    }
}
