using ARKBreedingStats.Library;
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
        public event Action<Creature, bool> EditCreature;
        public event Action<Creature> BestBreedingPartners;
        public event PedigreeCreature.ExportToClipboardEventHandler ExportToClipboard;
        public event Raising.createIncubationEventHandler CreateIncubationTimer;
        public event Form1.SetMessageLabelTextEventHandler SetMessageLabelText;
        public event Action<Species> SetGlobalSpecies;
        private List<Creature> females = new List<Creature>();
        private List<Creature> males = new List<Creature>();
        private List<BreedingPair> breedingPairs;
        private Species currentSpecies;
        /// <summary>
        /// how much are the stats weighted when looking for the best
        /// </summary>
        public double[] statWeights = new double[Values.STATS_COUNT];
        /// <summary>
        /// The best possible levels of the selected species for each stat.
        /// </summary>
        private readonly int[] bestLevels = new int[Values.STATS_COUNT];
        private readonly List<PedigreeCreature> pcs = new List<PedigreeCreature>();
        private readonly List<PictureBox> pbs = new List<PictureBox>();
        private bool[] enabledColorRegions;
        private TimeSpan incubationTime = TimeSpan.Zero;
        private Creature chosenCreature;
        private BreedingMode breedingMode;
        public readonly StatWeighting statWeighting;
        public bool breedingPlanNeedsUpdate;
        private bool updateBreedingPlanAllowed; // set to false if settings are changed and update should only performed after that
        public CreatureCollection creatureCollection;
        private CancellationTokenSource cancelSource;
        private ToolTip tt = new ToolTip();
        public const double probabilityHigherLevel = 0.55; // probability of inheriting the higher level-stat
        public const double probabilityLowerLevel = 1 - probabilityHigherLevel; // probability of inheriting the lower level-stat
        private const double probabilityOfMutation = 0.025;
        //private const int maxMutationRolls = 3;
        /// <summary>
        /// A mutation is possible if the Mutations are less than this number.
        /// </summary>
        private const int mutationPossibleWithLessThan = 20;
        /// <summary>
        /// The probability that at least one mutation happens if both parents have a mutation counter of less than 20.
        /// </summary>
        private const double probabilityOfOneMutation = 1 - (1 - probabilityOfMutation) * (1 - probabilityOfMutation) * (1 - probabilityOfMutation);
        /// <summary>
        /// The approximate probability of at least one mutation if one parent has less and one parent has larger or equal 20 mutation.
        /// It's assumed that the stats of the mutated stat are the same for the parents.
        /// If they differ, the probability for a mutation from the parent with the higher stat is probabilityHigherLevel * probabilityOfMutation etc.
        /// </summary>
        private const double probabilityOfOneMutationFromOneParent = 1 - (1 - probabilityOfMutation / 2) * (1 - probabilityOfMutation / 2) * (1 - probabilityOfMutation / 2);

        public BreedingPlan()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            for (int i = 0; i < Values.STATS_COUNT; i++)
                statWeights[i] = 1;

            breedingMode = BreedingMode.TopStatsConservative;

            breedingPairs = new List<BreedingPair>();
            pedigreeCreatureBest.IsVirtual = true;
            pedigreeCreatureWorst.IsVirtual = true;
            pedigreeCreatureBestPossibleInSpecies.IsVirtual = true;
            pedigreeCreatureBest.OnlyLevels = true;
            pedigreeCreatureWorst.OnlyLevels = true;
            pedigreeCreatureBestPossibleInSpecies.OnlyLevels = true;
            pedigreeCreatureBest.Clear();
            pedigreeCreatureWorst.Clear();
            pedigreeCreatureBestPossibleInSpecies.Clear();
            pedigreeCreatureBest.HandCursor = false;
            pedigreeCreatureWorst.HandCursor = false;
            pedigreeCreatureBestPossibleInSpecies.HandCursor = false;

            statWeighting = statWeighting1;
            statWeighting.WeightingsChanged += StatWeighting_WeightingsChanged;
            breedingPlanNeedsUpdate = false;

            cbServerFilterLibrary.Checked = Properties.Settings.Default.UseServerFilterForBreedingPlan;
            cbOwnerFilterLibrary.Checked = Properties.Settings.Default.UseOwnerFilterForBreedingPlan;
            cbBPIncludeCooldowneds.Checked = Properties.Settings.Default.IncludeCooldownsInBreedingPlan;
            cbBPIncludeCryoCreatures.Checked = Properties.Settings.Default.IncludeCryoedInBreedingPlan;
            cbBPOnlyOneSuggestionForFemales.Checked = Properties.Settings.Default.BreedingPlanOnlyBestSuggestionForEachFemale;
            cbBPMutationLimitOnlyOnePartner.Checked = Properties.Settings.Default.BreedingPlanOnePartnerMoreMutationsThanLimit;

            tagSelectorList1.OnTagChanged += TagSelectorList1_OnTagChanged;
            updateBreedingPlanAllowed = true;
        }

        private void StatWeighting_WeightingsChanged(object sender, EventArgs e)
        {
            // check if sign of a weighting changed (then the best levels change)
            bool signChanged = false;
            var newWeightings = statWeighting.Weightings;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (Math.Sign(statWeights[s]) != Math.Sign(newWeightings[s]))
                {
                    signChanged = true;
                    break;
                }
            }
            statWeights = newWeightings;
            if (signChanged) DetermineBestLevels();

            CalculateBreedingScoresAndDisplayPairs();
        }

        private void TagSelectorList1_OnTagChanged()
        {
            CalculateBreedingScoresAndDisplayPairs();
        }

        public void BindChildrenControlEvents()
        {
            // has to be done after the BreedingPlan-class got the binding
            pedigreeCreatureBest.CreatureEdit += EditCreature;
            pedigreeCreatureWorst.CreatureEdit += EditCreature;
            pedigreeCreatureBestPossibleInSpecies.CreatureEdit += EditCreature;
            pedigreeCreatureBest.ExportToClipboard += ExportToClipboard;
            pedigreeCreatureWorst.ExportToClipboard += ExportToClipboard;
            pedigreeCreatureBestPossibleInSpecies.ExportToClipboard += ExportToClipboard;
            pedigreeCreatureBest.CreatureClicked += CreatureClicked;
            pedigreeCreatureWorst.CreatureClicked += CreatureClicked;
        }

        /// <summary>
        /// Set species or specific creature and calculate the breeding pairs.
        /// </summary>
        /// <param name="chosenCreature"></param>
        /// <param name="forceUpdate"></param>
        /// <param name="setSpecies"></param>
        public void DetermineBestBreeding(Creature chosenCreature = null, bool forceUpdate = false, Species setSpecies = null)
        {
            if (creatureCollection == null) return;

            Species selectedSpecies = chosenCreature?.Species;
            bool newSpecies = false;
            if (selectedSpecies == null)
                selectedSpecies = setSpecies ?? currentSpecies;
            if (selectedSpecies != null && currentSpecies != selectedSpecies)
            {
                CurrentSpecies = selectedSpecies;
                newSpecies = true;

                EnabledColorRegions = currentSpecies?.colors.Select(n => !string.IsNullOrEmpty(n?.name)).ToArray() ?? new bool[6] { true, true, true, true, true, true };

                breedingPlanNeedsUpdate = true;
            }

            statWeights = statWeighting.Weightings;

            if (forceUpdate || breedingPlanNeedsUpdate)
                Creatures = creatureCollection.creatures
                        .Where(c => c.speciesBlueprint == currentSpecies.blueprintPath
                                && (c.Status == CreatureStatus.Available
                                    || (c.Status == CreatureStatus.Cryopod && cbBPIncludeCryoCreatures.Checked))
                                && !c.flags.HasFlag(CreatureFlags.Neutered)
                                && (cbBPIncludeCooldowneds.Checked
                                    || !(c.cooldownUntil > DateTime.Now
                                       || c.growingUntil > DateTime.Now
                                       )
                                   )
                               )
                        .ToList();

            this.chosenCreature = chosenCreature;
            CalculateBreedingScoresAndDisplayPairsAsync(breedingMode, newSpecies);
            breedingPlanNeedsUpdate = false;
        }

        private IEnumerable<Creature> FilterByTags(IEnumerable<Creature> cl)
        {
            List<string> excludingTagList = tagSelectorList1.excludingTags;
            List<string> includingTagList = tagSelectorList1.includingTags;

            List<Creature> filteredList = new List<Creature>();

            if (excludingTagList.Any() || cbBPTagExcludeDefault.Checked)
            {
                foreach (Creature c in cl)
                {
                    bool exclude = cbBPTagExcludeDefault.Checked;
                    if (!exclude && excludingTagList.Any())
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
                    if (exclude && includingTagList.Any())
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

        /// <summary>
        /// Update breeding plan with current settings and current species.
        /// </summary>
        private void CalculateBreedingScoresAndDisplayPairs()
        {
            if (updateBreedingPlanAllowed && currentSpecies != null)
                CalculateBreedingScoresAndDisplayPairsAsync(breedingMode);
        }

        private async void CalculateBreedingScoresAndDisplayPairsAsync(BreedingMode breedingMode, bool updateBreedingData = false)
        {
            cancelSource?.Cancel();
            using (cancelSource = new CancellationTokenSource())
            {
                try
                {
                    await Task.Delay(400, cancelSource.Token); // recalculate breedingplan at most a certain interval
                    CalculateBreedingScoresAndDisplayPairs(breedingMode, updateBreedingData);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
            cancelSource = null;
        }

        private void CalculateBreedingScoresAndDisplayPairs(BreedingMode breedingMode, bool updateBreedingData = false)
        {
            if (currentSpecies == null) return;

            SuspendLayout();
            this.SuspendDrawing();
            ClearControls();

            // chosen Creature (only consider this one for its sex)
            bool considerChosenCreature = chosenCreature != null;

            bool considerMutationLimit = nudBPMutationLimit.Value >= 0;

            // filter by tags
            int crCountF = females.Count;
            int crCountM = males.Count;
            IEnumerable<Creature> selectFemales;
            IEnumerable<Creature> selectMales;
            if (considerChosenCreature && chosenCreature.sex == Sex.Female)
                selectFemales = new List<Creature>();
            else if (!cbBPMutationLimitOnlyOnePartner.Checked && considerMutationLimit) selectFemales = FilterByTags(females.Where(c => c.Mutations <= nudBPMutationLimit.Value));
            else selectFemales = FilterByTags(females);
            if (considerChosenCreature && chosenCreature.sex == Sex.Male)
                selectMales = new List<Creature>();
            else if (!cbBPMutationLimitOnlyOnePartner.Checked && considerMutationLimit) selectMales = FilterByTags(males.Where(c => c.Mutations <= nudBPMutationLimit.Value));
            else selectMales = FilterByTags(males);

            // filter by servers
            if (cbServerFilterLibrary.Checked && (Properties.Settings.Default.FilterHideServers?.Any() ?? false))
            {
                selectFemales = selectFemales.Where(c => !Properties.Settings.Default.FilterHideServers.Contains(c.server));
                selectMales = selectMales.Where(c => !Properties.Settings.Default.FilterHideServers.Contains(c.server));
            }
            // filter by owner
            if (cbOwnerFilterLibrary.Checked && (Properties.Settings.Default.FilterHideOwners?.Any() ?? false))
            {
                selectFemales = selectFemales.Where(c => !Properties.Settings.Default.FilterHideOwners.Contains(c.owner));
                selectMales = selectMales.Where(c => !Properties.Settings.Default.FilterHideOwners.Contains(c.owner));
            }
            // filter by tribe
            if (cbTribeFilterLibrary.Checked && (Properties.Settings.Default.FilterHideTribes?.Any() ?? false))
            {
                selectFemales = selectFemales.Where(c => !Properties.Settings.Default.FilterHideTribes.Contains(c.tribe));
                selectMales = selectMales.Where(c => !Properties.Settings.Default.FilterHideTribes.Contains(c.tribe));
            }

            Creature[] selectedFemales = selectFemales.ToArray();
            Creature[] selectedMales = selectMales.ToArray();

            bool creaturesTagFilteredOut = (crCountF != selectedFemales.Length)
                                              || (crCountM != selectedMales.Length);

            bool creaturesMutationsFilteredOut = false;
            bool displayFilterWarning = true;

            lbBreedingPlanHeader.Text = currentSpecies.DescriptiveNameAndMod + (considerChosenCreature ? " (" + string.Format(Loc.S("onlyPairingsWith"), chosenCreature.name) + ")" : string.Empty);
            if (considerChosenCreature && (chosenCreature.flags.HasFlag(CreatureFlags.Neutered) || chosenCreature.Status != CreatureStatus.Available))
                lbBreedingPlanHeader.Text += $"{Loc.S("BreedingNotPossible")} ! ({(chosenCreature.flags.HasFlag(CreatureFlags.Neutered) ? Loc.S("Neutered") : Loc.S("notAvailable"))})";

            var combinedCreatures = new List<Creature>(selectedFemales);
            combinedCreatures.AddRange(selectedMales);
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
                selectedFemales = combinedCreatures.ToArray();
                selectedMales = combinedCreatures.ToArray();
            }

            // if only pairings for one specific creatures are shown, add the creature after the filtering
            if (considerChosenCreature)
            {
                if (chosenCreature.sex == Sex.Female)
                    selectedFemales = new Creature[] { chosenCreature };
                if (chosenCreature.sex == Sex.Male)
                    selectedMales = new Creature[] { chosenCreature };
            }

            if (selectedFemales.Any() && selectedMales.Any())
            {
                pedigreeCreature1.Show();
                pedigreeCreature2.Show();
                lbBPBreedingScore.Show();

                breedingPairs.Clear();
                short[] bestPossLevels = new short[Values.STATS_COUNT]; // best possible levels

                for (int fi = 0; fi < selectedFemales.Length; fi++)
                {
                    var female = selectedFemales[fi];
                    for (int mi = 0; mi < selectedMales.Length; mi++)
                    {
                        var male = selectedMales[mi];
                        // if Properties.Settings.Default.IgnoreSexInBreedingPlan (useful when using S+ mutator), skip pair if
                        // creatures are the same, or pair has already been added
                        if (Properties.Settings.Default.IgnoreSexInBreedingPlan)
                        {
                            if (considerChosenCreature)
                            {
                                if (male == female)
                                    continue;
                            }
                            else if (fi == mi)
                                break;
                        }
                        // if mutation limit is set, only skip pairs where both parents exceed that limit. One parent is enough to trigger a mutation.
                        if (considerMutationLimit && female.Mutations > nudBPMutationLimit.Value && male.Mutations > nudBPMutationLimit.Value)
                        {
                            creaturesMutationsFilteredOut = true;
                            continue;
                        }

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

                            double tt = statWeights[s] * (probabilityHigherLevel * higherLevel + probabilityLowerLevel * lowerlevel) / 40;
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
                                        eTS += female.levelsWild[s] == topStats[s] && male.levelsWild[s] == topStats[s] ? 1 : probabilityHigherLevel;
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

                            foreach (Creature cr in selectMales)
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
                                foreach (Creature cr in selectFemales)
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


                        int mutationPossibleFrom = female.Mutations < mutationPossibleWithLessThan && male.Mutations < mutationPossibleWithLessThan ? 2
                            : female.Mutations < mutationPossibleWithLessThan || male.Mutations < mutationPossibleWithLessThan ? 1 : 0;

                        breedingPairs.Add(new BreedingPair(female, male, t * 1.25, (mutationPossibleFrom == 2 ? probabilityOfOneMutation : mutationPossibleFrom == 1 ? probabilityOfOneMutationFromOneParent : 0)));
                    }
                }

                breedingPairs = breedingPairs.OrderByDescending(p => p.BreedingScore).ToList();

                if (cbBPOnlyOneSuggestionForFemales.Checked)
                {
                    var onlyOneSuggestionPerFemale = new List<BreedingPair>();
                    foreach (var bp in breedingPairs)
                    {
                        if (!onlyOneSuggestionPerFemale.Any(p => p.Female == bp.Female))
                            onlyOneSuggestionPerFemale.Add(bp);
                    }
                    breedingPairs = onlyOneSuggestionPerFemale;
                }

                double minScore = breedingPairs.LastOrDefault()?.BreedingScore ?? 0;
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
                        pc.RecalculateBreedingPlan += RecalculateBreedingPlan;
                        pc.BestBreedingPartners += BestBreedingPartners;
                        pc.ExportToClipboard += ExportToClipboard;
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
                        pc.RecalculateBreedingPlan += RecalculateBreedingPlan;
                        pc.BestBreedingPartners += BestBreedingPartners;
                        pc.ExportToClipboard += ExportToClipboard;
                        flowLayoutPanelPairs.Controls.Add(pc);
                        flowLayoutPanelPairs.SetFlowBreak(pc, true);
                        pcs.Add(pc);
                    }

                    Bitmap bm = new Bitmap(pb.Width, pb.Height);
                    using (Graphics g = Graphics.FromImage(bm))
                    {
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        using (Brush br = new SolidBrush(Utils.GetColorFromPercent((int)(breedingPairs[i].BreedingScore * 12.5), 0.5)))
                        using (Brush brOutline = new SolidBrush(Utils.GetColorFromPercent((int)(breedingPairs[i].BreedingScore * 12.5), -.2)))
                        using (Brush bb = new SolidBrush(Color.Black))
                        using (Brush bMut = new SolidBrush(Utils.MutationColor))
                        {
                            if (breedingPairs[i].Female.Mutations < mutationPossibleWithLessThan)
                                g.FillRectangle(bMut, 0, 5, 10, 10);
                            if (breedingPairs[i].Male.Mutations < mutationPossibleWithLessThan)
                                g.FillRectangle(bMut, 77, 5, 10, 10);
                            g.FillRectangle(brOutline, 0, 15, 87, 5);
                            g.FillRectangle(brOutline, 20, 10, 47, 15);
                            g.FillRectangle(br, 1, 16, 85, 3);
                            g.FillRectangle(br, 21, 11, 45, 13);
                            g.DrawString(breedingPairs[i].BreedingScore.ToString("N4"), new Font("Microsoft Sans Serif", 8.25f), bb, 24, 12);
                        }
                        pb.Image = bm;
                    }

                    row++;
                }
                // hide unused controls
                for (int i = creatureCollection.maxBreedingSuggestions; 2 * i + 1 < pcs.Count && i < pbs.Count; i++)
                {
                    pcs[2 * i].Hide();
                    pcs[2 * i + 1].Hide();
                    pbs[i].Hide();
                }

                if (updateBreedingData)
                    SetBreedingData(currentSpecies);
                if (breedingPairs.Any())
                {
                    SetParents(0);

                    // if breeding mode is conservative and a creature with top-stats already exists, the scoring might seem off
                    if (breedingMode == BreedingMode.TopStatsConservative)
                    {
                        bool bestCreatureAlreadyAvailable = true;
                        Creature bestCreature = null;
                        List<Creature> choosenFemalesAndMales = selectFemales.Concat(selectMales).ToList();
                        foreach (Creature cr in choosenFemalesAndMales)
                        {
                            bestCreatureAlreadyAvailable = true;
                            for (int s = 0; s < Values.STATS_COUNT; s++)
                            {
                                // if the stat is not a top stat and the stat is leveled in wild creatures
                                if (!cr.topBreedingStats[s] && cr.Species.stats[s].IncPerWildLevel != 0)
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
                        {
                            displayFilterWarning = false;
                            SetMessageLabelText(string.Format(Loc.S("AlreadyCreatureWithTopStats"), bestCreature.name, Utils.SexSymbol(bestCreature.sex)), MessageBoxIcon.Warning);
                        }
                    }
                }
                else
                    SetParents(-1);
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
                lbBreedingPlanInfo.Text = string.Format(Loc.S("NoPossiblePairingForSpeciesFound"), currentSpecies);
                lbBreedingPlanInfo.Visible = true;
                if (updateBreedingData)
                    SetBreedingData(currentSpecies);
            }

            if (displayFilterWarning)
            {
                // display warning if breeding pairs are filtered out
                string warningText = null;
                if (creaturesTagFilteredOut) warningText = Loc.S("BPsomeCreaturesAreFilteredOutTags") + ".\n" + Loc.S("BPTopStatsShownMightNotTotalTopStats");
                if (creaturesMutationsFilteredOut) warningText = (!string.IsNullOrEmpty(warningText) ? warningText + "\n" : string.Empty) + Loc.S("BPsomePairingsAreFilteredOutMutations");
                if (!string.IsNullOrEmpty(warningText)) SetMessageLabelText(warningText, MessageBoxIcon.Warning);
            }

            this.ResumeDrawing();

            if (considerChosenCreature) btShowAllCreatures.Text = "Unset Restriction to " + chosenCreature.name;
            btShowAllCreatures.Visible = considerChosenCreature;
            ResumeLayout();
        }

        private void RecalculateBreedingPlan()
        {
            DetermineBestBreeding(chosenCreature, true);
        }

        internal void UpdateIfNeeded()
        {
            if (breedingPlanNeedsUpdate)
                DetermineBestBreeding(chosenCreature);
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
            if (pbs.Count > creatureCollection.maxBreedingSuggestions)
            {
                for (int i = pbs.Count - 1; i > creatureCollection.maxBreedingSuggestions && i >= 0; i--)
                {
                    pcs[2 * i + 1].Dispose();
                    pcs.RemoveAt(2 * i + 1);
                    pcs[2 * i].Dispose();
                    pcs.RemoveAt(2 * i);
                    pbs[i].Dispose();
                    pbs.RemoveAt(i);
                }
            }

            pedigreeCreatureBest.Clear();
            pedigreeCreatureWorst.Clear();
            lbBreedingPlanInfo.Visible = false;
            lbBPProbabilityBest.Text = string.Empty;
            lbMutationProbability.Text = string.Empty;
            offspringPossibilities1.Clear();
            SetMessageLabelText();
        }

        public void Clear()
        {
            ClearControls();
            SetBreedingData();
            listViewRaisingTimes.Items.Clear();
            currentSpecies = null;
            males.Clear();
            females.Clear();
            lbBreedingPlanHeader.Text = Loc.S("SelectSpeciesBreedingPlanner");
        }

        private void SetBreedingData(Species species = null)
        {
            listViewRaisingTimes.Items.Clear();
            if (species?.breeding == null)
            {
                listViewRaisingTimes.Items.Add(Loc.S("naYet"));
                labelBreedingInfos.Text = string.Empty;
            }
            else
            {
                bool isGlowSpecies = species.IsGlowSpecies;
                pedigreeCreature1.IsGlowSpecies = isGlowSpecies;
                pedigreeCreature2.IsGlowSpecies = isGlowSpecies;
                if (Raising.GetRaisingTimes(species, out TimeSpan matingTime, out string incubationMode, out incubationTime, out TimeSpan babyTime, out TimeSpan maturationTime, out TimeSpan nextMatingMin, out TimeSpan nextMatingMax))
                {
                    if (matingTime != TimeSpan.Zero)
                        listViewRaisingTimes.Items.Add(new ListViewItem(new[] { Loc.S("matingTime"), matingTime.ToString("d':'hh':'mm':'ss") }));

                    TimeSpan totalTime = incubationTime;
                    DateTime until = DateTime.Now.Add(totalTime);
                    string[] times = { incubationMode, incubationTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.ShortTimeDate(until) };
                    listViewRaisingTimes.Items.Add(new ListViewItem(times));

                    totalTime += babyTime;
                    until = DateTime.Now.Add(totalTime);
                    times = new[] { Loc.S("Baby"), babyTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.ShortTimeDate(until) };
                    listViewRaisingTimes.Items.Add(new ListViewItem(times));

                    totalTime = incubationTime + maturationTime;
                    until = DateTime.Now.Add(totalTime);
                    times = new[] { Loc.S("Maturation"), maturationTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.ShortTimeDate(until) };
                    listViewRaisingTimes.Items.Add(new ListViewItem(times));

                    string eggInfo = Raising.EggTemperature(species);

                    labelBreedingInfos.Text = (nextMatingMin != TimeSpan.Zero ? $"{Loc.S("TimeBetweenMating")}: {nextMatingMin:d':'hh':'mm':'ss} to {nextMatingMax:d':'hh':'mm':'ss}" : string.Empty)
                        + ((!string.IsNullOrEmpty(eggInfo) ? "\n" + eggInfo : string.Empty));
                }
            }
        }

        public void CreateTagList()
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
                if (value == null) return;
                females = value.Where(c => c.sex == Sex.Female).ToList();
                males = value.Where(c => c.sex == Sex.Male).ToList();

                DetermineBestLevels(value);
            }
        }

        private void DetermineBestLevels(List<Creature> creatures = null)
        {
            if (creatures == null)
            {
                creatures = females.ToList();
                creatures.AddRange(males);
            }
            if (!creatures.Any()) return;

            for (int s = 0; s < Values.STATS_COUNT; s++)
                bestLevels[s] = -1;

            foreach (Creature c in creatures)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if ((s == (int)StatNames.Torpidity || statWeights[s] >= 0) && c.levelsWild[s] > bestLevels[s])
                        bestLevels[s] = c.levelsWild[s];
                    else if (s != (int)StatNames.Torpidity && statWeights[s] < 0 && c.levelsWild[s] >= 0 && (c.levelsWild[s] < bestLevels[s] || bestLevels[s] < 0))
                        bestLevels[s] = c.levelsWild[s];
                }
            }

            // display top levels in species
            int? levelStep = creatureCollection.getWildLevelStep();
            Creature crB = new Creature(currentSpecies, string.Empty, string.Empty, string.Empty, 0, new int[Values.STATS_COUNT], null, 100, true, levelStep: levelStep)
            {
                name = "Best possible " + currentSpecies.name + " for this library"
            };
            bool totalLevelUnknown = false;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s == (int)StatNames.Torpidity) continue;
                crB.levelsWild[s] = bestLevels[s];
                //crB.valuesBreeding[s] = StatValueCalculation.CalculateValue(currentSpecies, s, crB.levelsWild[s], 0, true, 1, 0);
                if (crB.levelsWild[s] == -1)
                    totalLevelUnknown = true;
                crB.topBreedingStats[s] = (crB.levelsWild[s] > 0);
            }
            crB.levelsWild[(int)StatNames.Torpidity] = crB.levelsWild.Sum();
            crB.RecalculateCreatureValues(levelStep);
            pedigreeCreatureBestPossibleInSpecies.TotalLevelUnknown = totalLevelUnknown;
            pedigreeCreatureBestPossibleInSpecies.Creature = crB;
        }

        private void CreatureClicked(Creature c, int comboIndex, MouseEventArgs e)
        {
            if (comboIndex >= 0)
                SetParents(comboIndex);
        }

        private void CreatureEdit(Creature c, bool isVirtual)
        {
            EditCreature?.Invoke(c, isVirtual);
        }

        private void SetParents(int comboIndex)
        {
            if (comboIndex < 0 || comboIndex >= breedingPairs.Count)
            {
                pedigreeCreatureBest.Clear();
                pedigreeCreatureWorst.Clear();
                lbBreedingPlanInfo.Visible = false;
                lbBPProbabilityBest.Text = string.Empty;
                lbMutationProbability.Text = string.Empty;
                return;
            }

            int? levelStep = creatureCollection.getWildLevelStep();
            Creature crB = new Creature(currentSpecies, string.Empty, string.Empty, string.Empty, 0, new int[Values.STATS_COUNT], null, 100, true, levelStep: levelStep);
            Creature crW = new Creature(currentSpecies, string.Empty, string.Empty, string.Empty, 0, new int[Values.STATS_COUNT], null, 100, true, levelStep: levelStep);
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
                crB.valuesBreeding[s] = StatValueCalculation.CalculateValue(currentSpecies, s, crB.levelsWild[s], 0, true, 1, 0);
                crB.topBreedingStats[s] = (crB.levelsWild[s] == bestLevels[s]);
                crW.levelsWild[s] = statWeights[s] < 0 ? Math.Max(mother.levelsWild[s], father.levelsWild[s]) : Math.Min(mother.levelsWild[s], father.levelsWild[s]);
                crW.valuesBreeding[s] = StatValueCalculation.CalculateValue(currentSpecies, s, crW.levelsWild[s], 0, true, 1, 0);
                crW.topBreedingStats[s] = (crW.levelsWild[s] == bestLevels[s]);
                if (crB.levelsWild[s] == -1 || crW.levelsWild[s] == -1)
                    totalLevelUnknown = true;
                if (crB.levelsWild[s] > crW.levelsWild[s])
                    probabilityBest *= probabilityHigherLevel;
            }
            crB.levelsWild[(int)StatNames.Torpidity] = crB.levelsWild.Sum();
            crW.levelsWild[(int)StatNames.Torpidity] = crW.levelsWild.Sum();
            crB.name = Loc.S("BestPossible");
            crW.name = Loc.S("WorstPossible");
            crB.RecalculateCreatureValues(levelStep);
            crW.RecalculateCreatureValues(levelStep);
            pedigreeCreatureBest.TotalLevelUnknown = totalLevelUnknown;
            pedigreeCreatureWorst.TotalLevelUnknown = totalLevelUnknown;
            int mutationCounterMaternal = mother.Mutations;
            int mutationCounterPaternal = father.Mutations;
            crB.mutationsMaternal = mutationCounterMaternal;
            crB.mutationsPaternal = mutationCounterPaternal;
            crW.mutationsMaternal = mutationCounterMaternal;
            crW.mutationsPaternal = mutationCounterPaternal;
            pedigreeCreatureBest.Creature = crB;
            pedigreeCreatureWorst.Creature = crW;
            lbBPProbabilityBest.Text = $"{Loc.S("ProbabilityForBest")}: {Math.Round(100 * probabilityBest, 1)} %";
            lbMutationProbability.Text = $"{Loc.S("ProbabilityForOneMutation")}: {Math.Round(100 * breedingPairs[comboIndex].MutationProbability, 1)} %";

            // set probability barChart
            offspringPossibilities1.Calculate(currentSpecies, mother.levelsWild, father.levelsWild);

            // highlight parents
            int hiliId = comboIndex * 2;
            for (int i = 0; i < pcs.Count; i++)
                pcs[i].Highlight = (i == hiliId || i == hiliId + 1);
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
            CreateIncubationEntry();
        }

        public Species CurrentSpecies
        {
            get => currentSpecies;
            set
            {
                currentSpecies = value;
                statWeighting.SetSpecies(value);
            }
        }

        private void listViewSpeciesBP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewSpeciesBP.SelectedIndices.Count > 0
                && !string.IsNullOrEmpty(listViewSpeciesBP.SelectedItems[0]?.Text)
                && (currentSpecies == null
                    || listViewSpeciesBP.SelectedItems[0].Text != currentSpecies.DescriptiveNameAndMod)
                )
            {
                SetGlobalSpecies?.Invoke((Species)listViewSpeciesBP.SelectedItems[0].Tag);
            }
        }

        public int MaxWildLevels
        {
            set => offspringPossibilities1.maxWildLevel = value;
        }

        public void SetSpecies(Species species)
        {
            if (currentSpecies == species) return;

            // automatically set preset if preset with the speciesname exists
            updateBreedingPlanAllowed = false;
            if (!statWeighting.TrySetPresetByName(species.name))
                statWeighting.TrySetPresetByName("Default");
            updateBreedingPlanAllowed = true;

            DetermineBestBreeding(setSpecies: species);

            //// update listviewSpeciesBP
            // deselect currently selected species
            if (listViewSpeciesBP.SelectedItems.Count > 0)
                listViewSpeciesBP.SelectedItems[0].Selected = false;
            for (int i = 0; i < listViewSpeciesBP.Items.Count; i++)
            {
                if (listViewSpeciesBP.Items[i].Text == currentSpecies.DescriptiveNameAndMod)
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
            DetermineBestBreeding(chosenCreature, true);
        }

        private void cbBPIncludeCryoCreatures_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IncludeCryoedInBreedingPlan = cbBPIncludeCryoCreatures.Checked;
            DetermineBestBreeding(chosenCreature, true);
        }

        private void nudMutationLimit_ValueChanged(object sender, EventArgs e)
        {
            DetermineBestBreeding(chosenCreature, true);
        }

        private void radioButtonBPTopStatsCn_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBPTopStatsCn.Checked)
            {
                breedingMode = BreedingMode.TopStatsConservative;
                CalculateBreedingScoresAndDisplayPairs();
            }
        }

        private void radioButtonBPTopStats_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBPTopStats.Checked)
            {
                breedingMode = BreedingMode.TopStatsLucky;
                CalculateBreedingScoresAndDisplayPairs();
            }
        }

        private void radioButtonBPHighStats_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBPHighStats.Checked)
            {
                breedingMode = BreedingMode.BestNextGen;
                CalculateBreedingScoresAndDisplayPairs();
            }
        }

        public void SetSpeciesList(List<Species> species, List<Creature> creatures)
        {
            Species previouslySelectedSpecies = listViewSpeciesBP.SelectedItems.Count > 0 ? listViewSpeciesBP.SelectedItems[0].Tag as Species : null;

            listViewSpeciesBP.Items.Clear();

            foreach (Species s in species)
            {
                ListViewItem lvi = new ListViewItem { Text = s.DescriptiveNameAndMod, Tag = s };
                // check if species has both available males and females
                if (s == null || s.breeding == null || !creatures.Any(c => c.Species == s && c.Status == CreatureStatus.Available && c.sex == Sex.Female) || !creatures.Any(c => c.Species == s && c.Status == CreatureStatus.Available && c.sex == Sex.Male))
                    lvi.ForeColor = Color.LightGray;
                listViewSpeciesBP.Items.Add(lvi);
            }

            // select previous selecteded again
            if (previouslySelectedSpecies != null)
            {
                for (int i = 0; i < listViewSpeciesBP.Items.Count; i++)
                {
                    if (listViewSpeciesBP.Items[i].Tag is Species s
                        && s == previouslySelectedSpecies)
                    {
                        listViewSpeciesBP.Items[i].Focused = true;
                        listViewSpeciesBP.Items[i].Selected = true;
                        break;
                    }
                }
            }
        }

        private void CreateIncubationEntry(bool startNow = true)
        {
            if (pedigreeCreatureBest.Creature?.Mother != null && pedigreeCreatureBest.Creature.Father != null)
            {
                CreateIncubationTimer?.Invoke(pedigreeCreatureBest.Creature.Mother, pedigreeCreatureBest.Creature.Father, incubationTime, startNow);

                // set cooldown for mother
                Species species = pedigreeCreatureBest.Creature.Mother.Species;
                if (species?.breeding != null)
                {
                    pedigreeCreatureBest.Creature.Mother.cooldownUntil = DateTime.Now.AddSeconds(species.breeding.matingCooldownMinAdjusted);
                    // update breeding plan
                    DetermineBestBreeding(chosenCreature, true);
                }
            }
        }

        public void UpdateBreedingData()
        {
            SetBreedingData(currentSpecies);
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
            CalculateBreedingScoresAndDisplayPairs();
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
            Loc.ControlText(gbBPBreedingMode);
            Loc.ControlText(lbBPBreedingTimes);
            Loc.ControlText(btBPJustMated);
            Loc.ControlText(cbBPOnlyOneSuggestionForFemales);
            Loc.ControlText(cbBPMutationLimitOnlyOnePartner);
            columnHeader2.Text = Loc.S("Time");
            columnHeader3.Text = Loc.S("TotalTime");
            columnHeader4.Text = Loc.S("FinishedAt");

            // tooltips
            Loc.ControlText(lbBPBreedingScore, tt);
            Loc.ControlText(rbBPTopStatsCn, tt);
            Loc.ControlText(rbBPTopStats, tt);
            Loc.ControlText(rbBPHighStats, tt);
            Loc.ControlText(btBPJustMated, tt);
            Loc.SetToolTip(nudBPMutationLimit, tt);
            Loc.SetToolTip(cbBPTagExcludeDefault, tt);
        }

        private void cbServerFilterLibrary_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UseServerFilterForBreedingPlan = cbServerFilterLibrary.Checked;
            CalculateBreedingScoresAndDisplayPairs();
        }

        private void btShowAllCreatures_Click(object sender, EventArgs e)
        {
            // remove restriction on one creature TODO
            chosenCreature = null;
            CalculateBreedingScoresAndDisplayPairs();
        }

        private void cbOwnerFilterLibrary_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UseOwnerFilterForBreedingPlan = cbOwnerFilterLibrary.Checked;
            CalculateBreedingScoresAndDisplayPairs();
        }

        private void cbOnlyOneSuggestionForFemales_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.BreedingPlanOnlyBestSuggestionForEachFemale = cbBPOnlyOneSuggestionForFemales.Checked;
            CalculateBreedingScoresAndDisplayPairs();
        }

        private void cbMutationLimitOnlyOnePartner_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.BreedingPlanOnePartnerMoreMutationsThanLimit = cbBPMutationLimitOnlyOnePartner.Checked;
            CalculateBreedingScoresAndDisplayPairs();
        }
    }
}
