using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.Library;
using ARKBreedingStats.Properties;
using ARKBreedingStats.raising;
using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.utils;
using ARKBreedingStats.values;

namespace ARKBreedingStats
{
    public partial class BreedingPlan : UserControl
    {
        public event Action<Creature, bool> EditCreature;
        public event Action<Creature> BestBreedingPartners;
        public event Action<Creature> DisplayInPedigree;
        public event PedigreeCreature.ExportToClipboardEventHandler ExportToClipboard;
        public event Raising.createIncubationEventHandler CreateIncubationTimer;
        public event Form1.SetMessageLabelTextEventHandler SetMessageLabelText;
        public event Action<Species> SetGlobalSpecies;
        private Creature[] _females;
        private Creature[] _males;
        private List<BreedingPair> _breedingPairs;
        private Species _currentSpecies;
        /// <summary>
        /// how much are the stats weighted when looking for the best
        /// </summary>
        private double[] _statWeights = new double[Values.STATS_COUNT];
        /// <summary>
        /// The best possible levels of the selected species for each stat.
        /// If the weighting is negative, a low level is considered better.
        /// </summary>
        private readonly int[] _bestLevels = new int[Values.STATS_COUNT];
        private readonly List<PedigreeCreature> _pcs = new List<PedigreeCreature>();
        private readonly List<PictureBox> _pbs = new List<PictureBox>();
        private bool[] _enabledColorRegions;
        private TimeSpan _incubationTime;
        private Creature _chosenCreature;
        private BreedingMode _breedingMode;
        public readonly StatWeighting statWeighting;
        public bool breedingPlanNeedsUpdate;
        private bool _speciesInfoNeedsUpdate;
        private Debouncer _breedingPlanDebouncer = new Debouncer();

        /// <summary>
        /// Set to false if settings are changed and update should only performed after that.
        /// </summary>
        private bool _updateBreedingPlanAllowed;
        public CreatureCollection CreatureCollection;
        private readonly ToolTip _tt = new ToolTip();

        #region inheritance probabilities
        public const double ProbabilityHigherLevel = 0.55; // probability of inheriting the higher level-stat
        public const double ProbabilityLowerLevel = 1 - ProbabilityHigherLevel; // probability of inheriting the lower level-stat
        private const double ProbabilityOfMutation = 0.025;
        //private const int maxMutationRolls = 3;
        /// <summary>
        /// A mutation is possible if the Mutations are less than this number.
        /// </summary>
        private const int MutationPossibleWithLessThan = 20;
        /// <summary>
        /// The probability that at least one mutation happens if both parents have a mutation counter of less than 20.
        /// </summary>
        private const double ProbabilityOfOneMutation = 1 - (1 - ProbabilityOfMutation) * (1 - ProbabilityOfMutation) * (1 - ProbabilityOfMutation);
        /// <summary>
        /// The approximate probability of at least one mutation if one parent has less and one parent has larger or equal 20 mutation.
        /// It's assumed that the stats of the mutated stat are the same for the parents.
        /// If they differ, the probability for a mutation from the parent with the higher stat is probabilityHigherLevel * probabilityOfMutation etc.
        /// </summary>
        private const double ProbabilityOfOneMutationFromOneParent = 1 - (1 - ProbabilityOfMutation / 2) * (1 - ProbabilityOfMutation / 2) * (1 - ProbabilityOfMutation / 2);
        #endregion

        public BreedingPlan()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            for (int i = 0; i < Values.STATS_COUNT; i++)
                _statWeights[i] = 1;

            _breedingMode = BreedingMode.TopStatsConservative;

            _breedingPairs = new List<BreedingPair>();
            pedigreeCreatureBest.SetIsVirtual(true);
            pedigreeCreatureWorst.SetIsVirtual(true);
            pedigreeCreatureBestPossibleInSpecies.SetIsVirtual(true);
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

            cbServerFilterLibrary.Checked = Settings.Default.UseServerFilterForBreedingPlan;
            cbOwnerFilterLibrary.Checked = Settings.Default.UseOwnerFilterForBreedingPlan;
            cbBPIncludeCooldowneds.Checked = Settings.Default.IncludeCooldownsInBreedingPlan;
            cbBPIncludeCryoCreatures.Checked = Settings.Default.IncludeCryoedInBreedingPlan;
            cbBPOnlyOneSuggestionForFemales.Checked = Settings.Default.BreedingPlanOnlyBestSuggestionForEachFemale;
            cbBPMutationLimitOnlyOnePartner.Checked = Settings.Default.BreedingPlanOnePartnerMoreMutationsThanLimit;

            tagSelectorList1.OnTagChanged += TagSelectorList1_OnTagChanged;
            _updateBreedingPlanAllowed = true;
        }

        private void StatWeighting_WeightingsChanged()
        {
            // check if sign of a weighting changed (then the best levels change)
            bool signChanged = false;
            var newWeightings = statWeighting.Weightings;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (Math.Sign(_statWeights[s]) != Math.Sign(newWeightings[s]))
                {
                    signChanged = true;
                    break;
                }
            }
            _statWeights = newWeightings;
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
            if (CreatureCollection == null) return;

            Species selectedSpecies = chosenCreature?.Species;
            _speciesInfoNeedsUpdate = false;
            if (selectedSpecies == null)
                selectedSpecies = setSpecies ?? _currentSpecies;
            if (selectedSpecies != null && _currentSpecies != selectedSpecies)
            {
                CurrentSpecies = selectedSpecies;
                _speciesInfoNeedsUpdate = true;

                EnabledColorRegions = _currentSpecies?.EnabledColorRegions;

                breedingPlanNeedsUpdate = true;
            }

            _statWeights = statWeighting.Weightings;

            if (forceUpdate || breedingPlanNeedsUpdate)
                Creatures = CreatureCollection.creatures
                        .Where(c => c.speciesBlueprint == _currentSpecies.blueprintPath
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

            _chosenCreature = chosenCreature;
            CalculateBreedingScoresAndDisplayPairs();
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
            if (_updateBreedingPlanAllowed && _currentSpecies != null)
                _breedingPlanDebouncer.Debounce(400, DoCalculateBreedingScoresAndDisplayPairs, Dispatcher.CurrentDispatcher);
        }

        private void DoCalculateBreedingScoresAndDisplayPairs()
        {
            if (_currentSpecies == null
                || _females == null
                || _males == null
                )
                return;

            SuspendLayout();
            this.SuspendDrawing();
            ClearControls();

            // chosen Creature (only consider this one for its sex)
            bool considerChosenCreature = _chosenCreature != null;

            bool considerMutationLimit = nudBPMutationLimit.Value >= 0;

            bool creaturesMutationsFilteredOut = false;
            // filter by tags
            int crCountF = _females.Length;
            int crCountM = _males.Length;
            IEnumerable<Creature> selectFemales;
            IEnumerable<Creature> selectMales;
            if (considerChosenCreature && _chosenCreature.sex == Sex.Female)
                selectFemales = new List<Creature>();
            else if (!cbBPMutationLimitOnlyOnePartner.Checked && considerMutationLimit)
            {
                selectFemales = FilterByTags(_females.Where(c => c.Mutations <= nudBPMutationLimit.Value));
                creaturesMutationsFilteredOut = _females.Any(c => c.Mutations > nudBPMutationLimit.Value);
            }
            else selectFemales = FilterByTags(_females);
            if (considerChosenCreature && _chosenCreature.sex == Sex.Male)
                selectMales = new List<Creature>();
            else if (!cbBPMutationLimitOnlyOnePartner.Checked && considerMutationLimit)
            {
                selectMales = FilterByTags(_males.Where(c => c.Mutations <= nudBPMutationLimit.Value));
                creaturesMutationsFilteredOut = creaturesMutationsFilteredOut || _males.Any(c => c.Mutations > nudBPMutationLimit.Value);
            }
            else selectMales = FilterByTags(_males);

            // filter by servers
            if (cbServerFilterLibrary.Checked && (Settings.Default.FilterHideServers?.Any() ?? false))
            {
                selectFemales = selectFemales.Where(c => !Settings.Default.FilterHideServers.Contains(c.server));
                selectMales = selectMales.Where(c => !Settings.Default.FilterHideServers.Contains(c.server));
            }
            // filter by owner
            if (cbOwnerFilterLibrary.Checked && (Settings.Default.FilterHideOwners?.Any() ?? false))
            {
                selectFemales = selectFemales.Where(c => !Settings.Default.FilterHideOwners.Contains(c.owner));
                selectMales = selectMales.Where(c => !Settings.Default.FilterHideOwners.Contains(c.owner));
            }
            // filter by tribe
            if (cbTribeFilterLibrary.Checked && (Settings.Default.FilterHideTribes?.Any() ?? false))
            {
                selectFemales = selectFemales.Where(c => !Settings.Default.FilterHideTribes.Contains(c.tribe));
                selectMales = selectMales.Where(c => !Settings.Default.FilterHideTribes.Contains(c.tribe));
            }

            Creature[] selectedFemales = selectFemales.ToArray();
            Creature[] selectedMales = selectMales.ToArray();

            bool creaturesTagFilteredOut = (crCountF != selectedFemales.Length)
                                              || (crCountM != selectedMales.Length);

            bool displayFilterWarning = true;

            lbBreedingPlanHeader.Text = _currentSpecies.DescriptiveNameAndMod + (considerChosenCreature ? " (" + string.Format(Loc.S("onlyPairingsWith"), _chosenCreature.name) + ")" : string.Empty);
            if (considerChosenCreature && (_chosenCreature.flags.HasFlag(CreatureFlags.Neutered) || _chosenCreature.Status != CreatureStatus.Available))
                lbBreedingPlanHeader.Text += $"{Loc.S("BreedingNotPossible")} ! ({(_chosenCreature.flags.HasFlag(CreatureFlags.Neutered) ? Loc.S("Neutered") : Loc.S("notAvailable"))})";

            var combinedCreatures = new List<Creature>(selectedFemales);
            combinedCreatures.AddRange(selectedMales);

            if (Settings.Default.IgnoreSexInBreedingPlan)
            {
                selectedFemales = combinedCreatures.ToArray();
                selectedMales = combinedCreatures.ToArray();
            }

            // if only pairings for one specific creatures are shown, add the creature after the filtering
            if (considerChosenCreature)
            {
                if (_chosenCreature.sex == Sex.Female)
                    selectedFemales = new[] { _chosenCreature };
                if (_chosenCreature.sex == Sex.Male)
                    selectedMales = new[] { _chosenCreature };
            }

            if (selectedFemales.Any() && selectedMales.Any())
            {
                pedigreeCreature1.Show();
                pedigreeCreature2.Show();
                lbBPBreedingScore.Show();

                _breedingPairs.Clear();
                short[] bestPossLevels = new short[Values.STATS_COUNT]; // best possible levels

                for (int fi = 0; fi < selectedFemales.Length; fi++)
                {
                    var female = selectedFemales[fi];
                    for (int mi = 0; mi < selectedMales.Length; mi++)
                    {
                        var male = selectedMales[mi];
                        // if Properties.Settings.Default.IgnoreSexInBreedingPlan (useful when using S+ mutator), skip pair if
                        // creatures are the same, or pair has already been added
                        if (Settings.Default.IgnoreSexInBreedingPlan)
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

                        int topFemale = 0;
                        int topMale = 0;

                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            if (s == (int)StatNames.Torpidity) continue;
                            bestPossLevels[s] = 0;
                            int higherLevel = Math.Max(female.levelsWild[s], male.levelsWild[s]);
                            int lowerLevel = Math.Min(female.levelsWild[s], male.levelsWild[s]);
                            if (higherLevel < 0) higherLevel = 0;
                            if (lowerLevel < 0) lowerLevel = 0;

                            bool higherIsBetter = _statWeights[s] >= 0;

                            double tt = _statWeights[s] * (ProbabilityHigherLevel * higherLevel + ProbabilityLowerLevel * lowerLevel) / 40;
                            if (tt != 0)
                            {
                                if (_breedingMode == BreedingMode.TopStatsLucky)
                                {
                                    if (female.levelsWild[s] == _bestLevels[s] || male.levelsWild[s] == _bestLevels[s])
                                    {
                                        if (female.levelsWild[s] == _bestLevels[s] && male.levelsWild[s] == _bestLevels[s])
                                            tt *= 1.142;
                                    }
                                    else if (_bestLevels[s] > 0)
                                        tt *= .01;
                                }
                                else if (_breedingMode == BreedingMode.TopStatsConservative && _bestLevels[s] > 0)
                                {
                                    bestPossLevels[s] = (short)(higherIsBetter ? Math.Max(female.levelsWild[s], male.levelsWild[s]) : Math.Min(female.levelsWild[s], male.levelsWild[s]));
                                    tt *= .01;
                                    if (female.levelsWild[s] == _bestLevels[s] || male.levelsWild[s] == _bestLevels[s])
                                    {
                                        nrTS++;
                                        eTS += female.levelsWild[s] == _bestLevels[s] && male.levelsWild[s] == _bestLevels[s] ? 1 : ProbabilityHigherLevel;
                                        if (female.levelsWild[s] == _bestLevels[s])
                                            topFemale++;
                                        if (male.levelsWild[s] == _bestLevels[s])
                                            topMale++;
                                    }
                                }
                            }
                            t += tt;
                        }

                        if (_breedingMode == BreedingMode.TopStatsConservative)
                        {
                            if (topFemale < nrTS && topMale < nrTS)
                                t += eTS;
                            else
                                t += .1 * eTS;
                            // check if the best possible stat outcome already exists in a male
                            bool maleExists = false;

                            foreach (Creature cr in selectedMales)
                            {
                                maleExists = true;
                                for (int s = 0; s < Values.STATS_COUNT; s++)
                                {
                                    if (s == (int)StatNames.Torpidity
                                        || !cr.Species.UsesStat(s)
                                        || cr.levelsWild[s] == bestPossLevels[s])
                                        continue;

                                    maleExists = false;
                                    break;
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
                                foreach (Creature cr in selectedFemales)
                                {
                                    femaleExists = true;
                                    for (int s = 0; s < Values.STATS_COUNT; s++)
                                    {
                                        if (s == (int)StatNames.Torpidity
                                            || !cr.Species.UsesStat(s)
                                            || cr.levelsWild[s] == bestPossLevels[s])
                                            continue;

                                        femaleExists = false;
                                        break;
                                    }
                                    if (femaleExists)
                                        break;
                                }
                                if (femaleExists)
                                    t *= .8; // another female with the same stats may be useful, but not so much in conservative breeding
                            }
                            //t *= 2; // scale conservative mode as it rather displays improvement, but only scarcely
                        }


                        int mutationPossibleFrom = female.Mutations < MutationPossibleWithLessThan && male.Mutations < MutationPossibleWithLessThan ? 2
                            : female.Mutations < MutationPossibleWithLessThan || male.Mutations < MutationPossibleWithLessThan ? 1 : 0;

                        _breedingPairs.Add(new BreedingPair(female, male, t * 1.25, (mutationPossibleFrom == 2 ? ProbabilityOfOneMutation : mutationPossibleFrom == 1 ? ProbabilityOfOneMutationFromOneParent : 0)));
                    }
                }

                _breedingPairs = _breedingPairs.OrderByDescending(p => p.BreedingScore).ToList();

                if (cbBPOnlyOneSuggestionForFemales.Checked)
                {
                    var onlyOneSuggestionPerFemale = new List<BreedingPair>();
                    foreach (var bp in _breedingPairs)
                    {
                        if (!onlyOneSuggestionPerFemale.Any(p => p.Female == bp.Female))
                            onlyOneSuggestionPerFemale.Add(bp);
                    }
                    _breedingPairs = onlyOneSuggestionPerFemale;
                }

                double minScore = _breedingPairs.LastOrDefault()?.BreedingScore ?? 0;
                if (minScore < 0)
                {
                    foreach (BreedingPair bp in _breedingPairs)
                        bp.BreedingScore -= minScore;
                }

                // draw best parents
                for (int i = 0; i < _breedingPairs.Count && i < CreatureCollection.maxBreedingSuggestions; i++)
                {
                    PedigreeCreature pc;
                    if (2 * i < _pcs.Count)
                    {
                        _pcs[2 * i].Creature = _breedingPairs[i].Female;
                        _pcs[2 * i].enabledColorRegions = _enabledColorRegions;
                        _pcs[2 * i].comboId = i;
                        _pcs[2 * i].Show();
                    }
                    else
                    {
                        pc = new PedigreeCreature(_breedingPairs[i].Female, _enabledColorRegions, i, true);
                        pc.CreatureClicked += CreatureClicked;
                        pc.CreatureEdit += CreatureEdit;
                        pc.RecalculateBreedingPlan += RecalculateBreedingPlan;
                        pc.BestBreedingPartners += BestBreedingPartners;
                        pc.DisplayInPedigree += DisplayInPedigree;
                        pc.ExportToClipboard += ExportToClipboard;
                        flowLayoutPanelPairs.Controls.Add(pc);
                        _pcs.Add(pc);
                    }

                    // draw score
                    PictureBox pb;
                    if (i < _pbs.Count)
                    {
                        pb = _pbs[i];
                        _pbs[i].Show();
                    }
                    else
                    {
                        pb = new PictureBox
                        {
                            Size = new Size(87, 35)
                        };
                        _pbs.Add(pb);
                        flowLayoutPanelPairs.Controls.Add(pb);
                    }

                    if (2 * i + 1 < _pcs.Count)
                    {
                        _pcs[2 * i + 1].Creature = _breedingPairs[i].Male;
                        _pcs[2 * i + 1].enabledColorRegions = _enabledColorRegions;
                        _pcs[2 * i + 1].comboId = i;
                        _pcs[2 * i + 1].Show();
                    }
                    else
                    {
                        pc = new PedigreeCreature(_breedingPairs[i].Male, _enabledColorRegions, i, true);
                        pc.CreatureClicked += CreatureClicked;
                        pc.CreatureEdit += CreatureEdit;
                        pc.RecalculateBreedingPlan += RecalculateBreedingPlan;
                        pc.BestBreedingPartners += BestBreedingPartners;
                        pc.DisplayInPedigree += DisplayInPedigree;
                        pc.ExportToClipboard += ExportToClipboard;
                        flowLayoutPanelPairs.Controls.Add(pc);
                        flowLayoutPanelPairs.SetFlowBreak(pc, true);
                        _pcs.Add(pc);
                    }

                    Bitmap bm = new Bitmap(pb.Width, pb.Height);
                    using (Graphics g = Graphics.FromImage(bm))
                    {
                        g.TextRenderingHint = TextRenderingHint.AntiAlias;
                        using (Brush br = new SolidBrush(Utils.GetColorFromPercent((int)(_breedingPairs[i].BreedingScore * 12.5), 0.5)))
                        using (Brush brOutline = new SolidBrush(Utils.GetColorFromPercent((int)(_breedingPairs[i].BreedingScore * 12.5), -.2)))
                        using (Brush bb = new SolidBrush(Color.Black))
                        using (Brush bMut = new SolidBrush(Utils.MutationColor))
                        {
                            if (_breedingPairs[i].Female.Mutations < MutationPossibleWithLessThan)
                                g.FillRectangle(bMut, 0, 5, 10, 10);
                            if (_breedingPairs[i].Male.Mutations < MutationPossibleWithLessThan)
                                g.FillRectangle(bMut, 77, 5, 10, 10);
                            g.FillRectangle(brOutline, 0, 15, 87, 5);
                            g.FillRectangle(brOutline, 20, 10, 47, 15);
                            g.FillRectangle(br, 1, 16, 85, 3);
                            g.FillRectangle(br, 21, 11, 45, 13);
                            g.DrawString(_breedingPairs[i].BreedingScore.ToString("N4"), new Font("Microsoft Sans Serif", 8.25f), bb, 24, 12);
                        }
                        pb.Image = bm;
                    }
                }
                // hide unused controls
                for (int i = CreatureCollection.maxBreedingSuggestions; 2 * i + 1 < _pcs.Count && i < _pbs.Count; i++)
                {
                    _pcs[2 * i].Hide();
                    _pcs[2 * i + 1].Hide();
                    _pbs[i].Hide();
                }

                if (_breedingPairs.Any())
                {
                    SetParents(0);

                    // if breeding mode is conservative and a creature with top-stats already exists, the scoring might seem off
                    if (_breedingMode == BreedingMode.TopStatsConservative)
                    {
                        bool bestCreatureAlreadyAvailable = true;
                        Creature bestCreature = null;
                        List<Creature> chosenFemalesAndMales = selectedFemales.Concat(selectedMales).ToList();
                        foreach (Creature cr in chosenFemalesAndMales)
                        {
                            bestCreatureAlreadyAvailable = true;
                            for (int s = 0; s < Values.STATS_COUNT; s++)
                            {
                                // if the stat is not a top stat and the stat is leveled in wild creatures
                                if (cr.Species.UsesStat(s) && cr.levelsWild[s] != _bestLevels[s])
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
                for (int i = 0; i < CreatureCollection.maxBreedingSuggestions && 2 * i + 1 < _pcs.Count && i < _pbs.Count; i++)
                {
                    _pcs[2 * i].Hide();
                    _pcs[2 * i + 1].Hide();
                    _pbs[i].Hide();
                }
                lbBreedingPlanInfo.Text = string.Format(Loc.S("NoPossiblePairingForSpeciesFound"), _currentSpecies);
                lbBreedingPlanInfo.Visible = true;
            }

            if (_speciesInfoNeedsUpdate)
                SetBreedingData(_currentSpecies);

            if (displayFilterWarning)
            {
                // display warning if breeding pairs are filtered out
                string warningText = null;
                if (creaturesTagFilteredOut) warningText = Loc.S("BPsomeCreaturesAreFilteredOutTags") + ".\n" + Loc.S("BPTopStatsShownMightNotTotalTopStats");
                if (creaturesMutationsFilteredOut) warningText = (!string.IsNullOrEmpty(warningText) ? warningText + "\n" : string.Empty) + Loc.S("BPsomePairingsAreFilteredOutMutations");
                if (!string.IsNullOrEmpty(warningText)) SetMessageLabelText(warningText, MessageBoxIcon.Warning);
            }

            this.ResumeDrawing();

            if (considerChosenCreature) btShowAllCreatures.Text = string.Format(Loc.S("BPCancelRestrictionOn"), _chosenCreature.name);
            btShowAllCreatures.Visible = considerChosenCreature;
            ResumeLayout();
        }

        /// <summary>
        /// Recreates the breeding plan after the same library is loaded.
        /// </summary>
        /// <param name="isActiveControl">if true, the data is updated immediately.</param>
        public void RecreateAfterLoading(bool isActiveControl = false)
        {
            if (_chosenCreature != null)
                _chosenCreature = CreatureCollection.creatures.FirstOrDefault(c => c.guid == _chosenCreature.guid);

            if (_currentSpecies != null)
            {
                _currentSpecies = Values.V.SpeciesByBlueprint(_currentSpecies.blueprintPath);
                if (isActiveControl)
                    DetermineBestBreeding(_chosenCreature, true);
                else
                    breedingPlanNeedsUpdate = true;
            }
        }

        private void RecalculateBreedingPlan()
        {
            DetermineBestBreeding(_chosenCreature, true);
        }

        internal void UpdateIfNeeded()
        {
            if (breedingPlanNeedsUpdate)
                DetermineBestBreeding(_chosenCreature);
        }

        private void ClearControls()
        {
            // hide unused controls
            for (int i = 0; i < CreatureCollection.maxBreedingSuggestions && 2 * i + 1 < _pcs.Count && i < _pbs.Count; i++)
            {
                _pcs[2 * i].Hide();
                _pcs[2 * i + 1].Hide();
                _pbs[i].Hide();
            }

            // remove controls outside of the limit
            if (_pbs.Count > CreatureCollection.maxBreedingSuggestions)
            {
                for (int i = _pbs.Count - 1; i > CreatureCollection.maxBreedingSuggestions && i >= 0; i--)
                {
                    _pcs[2 * i + 1].Dispose();
                    _pcs.RemoveAt(2 * i + 1);
                    _pcs[2 * i].Dispose();
                    _pcs.RemoveAt(2 * i);
                    _pbs[i].Dispose();
                    _pbs.RemoveAt(i);
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
            _currentSpecies = null;
            _males = null;
            _females = null;
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
                pedigreeCreature1.SetCustomStatNames(species.statNames);
                pedigreeCreature2.SetCustomStatNames(species.statNames);
                if (Raising.GetRaisingTimes(species, out TimeSpan matingTime, out string incubationMode, out _incubationTime, out TimeSpan babyTime, out TimeSpan maturationTime, out TimeSpan nextMatingMin, out TimeSpan nextMatingMax))
                {
                    if (matingTime != TimeSpan.Zero)
                        listViewRaisingTimes.Items.Add(new ListViewItem(new[] { Loc.S("matingTime"), matingTime.ToString("d':'hh':'mm':'ss") }));

                    TimeSpan totalTime = _incubationTime;
                    DateTime until = DateTime.Now.Add(totalTime);
                    string[] times = { incubationMode, _incubationTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.ShortTimeDate(until) };
                    listViewRaisingTimes.Items.Add(new ListViewItem(times));

                    totalTime += babyTime;
                    until = DateTime.Now.Add(totalTime);
                    times = new[] { Loc.S("Baby"), babyTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.ShortTimeDate(until) };
                    listViewRaisingTimes.Items.Add(new ListViewItem(times));

                    totalTime = _incubationTime + maturationTime;
                    until = DateTime.Now.Add(totalTime);
                    times = new[] { Loc.S("Maturation"), maturationTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.ShortTimeDate(until) };
                    listViewRaisingTimes.Items.Add(new ListViewItem(times));

                    string eggInfo = Raising.EggTemperature(species);

                    labelBreedingInfos.Text = (nextMatingMin != TimeSpan.Zero ? $"{Loc.S("TimeBetweenMating")}: {nextMatingMin:d':'hh':'mm':'ss} to {nextMatingMax:d':'hh':'mm':'ss}" : string.Empty)
                        + ((!string.IsNullOrEmpty(eggInfo) ? "\n" + eggInfo : string.Empty));
                }
            }

            _speciesInfoNeedsUpdate = false;
        }

        public void CreateTagList()
        {
            tagSelectorList1.tags = CreatureCollection.tags;
            foreach (string t in CreatureCollection.tagsInclude)
                tagSelectorList1.setTagStatus(t, TagSelector.tagStatus.include);
            foreach (string t in CreatureCollection.tagsExclude)
                tagSelectorList1.setTagStatus(t, TagSelector.tagStatus.exclude);
        }

        private List<Creature> Creatures
        {
            set
            {
                if (value == null) return;
                _females = value.Where(c => c.sex == Sex.Female).ToArray();
                _males = value.Where(c => c.sex == Sex.Male).ToArray();

                DetermineBestLevels(value);
            }
        }

        private void DetermineBestLevels(List<Creature> creatures = null)
        {
            if (creatures == null)
            {
                if (_females == null || _males == null) return;
                creatures = _females.ToList();
                creatures.AddRange(_males);
            }
            if (!creatures.Any()) return;

            for (int s = 0; s < Values.STATS_COUNT; s++)
                _bestLevels[s] = -1;

            foreach (Creature c in creatures)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if ((s == (int)StatNames.Torpidity || _statWeights[s] >= 0) && c.levelsWild[s] > _bestLevels[s])
                        _bestLevels[s] = c.levelsWild[s];
                    else if (s != (int)StatNames.Torpidity && _statWeights[s] < 0 && c.levelsWild[s] >= 0 && (c.levelsWild[s] < _bestLevels[s] || _bestLevels[s] < 0))
                        _bestLevels[s] = c.levelsWild[s];
                }
            }

            // display top levels in species
            int? levelStep = CreatureCollection.getWildLevelStep();
            Creature crB = new Creature(_currentSpecies, string.Empty, string.Empty, string.Empty, 0, new int[Values.STATS_COUNT], null, 100, true, levelStep: levelStep)
            {
                name = string.Format(Loc.S("BestPossibleSpeciesLibrary"), _currentSpecies.name)
            };
            bool totalLevelUnknown = false;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s == (int)StatNames.Torpidity) continue;
                crB.levelsWild[s] = _bestLevels[s];
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
            if (comboIndex < 0 || comboIndex >= _breedingPairs.Count)
            {
                pedigreeCreatureBest.Clear();
                pedigreeCreatureWorst.Clear();
                lbBreedingPlanInfo.Visible = false;
                lbBPProbabilityBest.Text = string.Empty;
                lbMutationProbability.Text = string.Empty;
                return;
            }

            int? levelStep = CreatureCollection.getWildLevelStep();
            Creature crB = new Creature(_currentSpecies, string.Empty, string.Empty, string.Empty, 0, new int[Values.STATS_COUNT], null, 100, true, levelStep: levelStep);
            Creature crW = new Creature(_currentSpecies, string.Empty, string.Empty, string.Empty, 0, new int[Values.STATS_COUNT], null, 100, true, levelStep: levelStep);
            Creature mother = _breedingPairs[comboIndex].Female;
            Creature father = _breedingPairs[comboIndex].Male;
            crB.Mother = mother;
            crB.Father = father;
            crW.Mother = mother;
            crW.Father = father;
            double probabilityBest = 1;
            bool totalLevelUnknown = false; // if stats are unknown, total level is as well (==> oxygen, speed)
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s == (int)StatNames.Torpidity) continue;
                crB.levelsWild[s] = _statWeights[s] < 0 ? Math.Min(mother.levelsWild[s], father.levelsWild[s]) : Math.Max(mother.levelsWild[s], father.levelsWild[s]);
                crB.valuesBreeding[s] = StatValueCalculation.CalculateValue(_currentSpecies, s, crB.levelsWild[s], 0, true, 1, 0);
                crB.topBreedingStats[s] = (_currentSpecies.stats[s].IncPerTamedLevel != 0 && crB.levelsWild[s] == _bestLevels[s]);
                crW.levelsWild[s] = _statWeights[s] < 0 ? Math.Max(mother.levelsWild[s], father.levelsWild[s]) : Math.Min(mother.levelsWild[s], father.levelsWild[s]);
                crW.valuesBreeding[s] = StatValueCalculation.CalculateValue(_currentSpecies, s, crW.levelsWild[s], 0, true, 1, 0);
                crW.topBreedingStats[s] = (_currentSpecies.stats[s].IncPerTamedLevel != 0 && crW.levelsWild[s] == _bestLevels[s]);
                if (crB.levelsWild[s] == -1 || crW.levelsWild[s] == -1)
                    totalLevelUnknown = true;
                if (crB.levelsWild[s] > crW.levelsWild[s])
                    probabilityBest *= ProbabilityHigherLevel;
                else if (crB.levelsWild[s] < crW.levelsWild[s])
                    probabilityBest *= ProbabilityLowerLevel;
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
            lbMutationProbability.Text = $"{Loc.S("ProbabilityForOneMutation")}: {Math.Round(100 * _breedingPairs[comboIndex].MutationProbability, 1)} %";

            // set probability barChart
            offspringPossibilities1.Calculate(_currentSpecies, mother.levelsWild, father.levelsWild);

            // highlight parents
            int hiliId = comboIndex * 2;
            for (int i = 0; i < _pcs.Count; i++)
                _pcs[i].Highlight = (i == hiliId || i == hiliId + 1);
        }

        public bool[] EnabledColorRegions
        {
            set
            {
                if (value != null && value.Length == 6)
                {
                    _enabledColorRegions = value;
                }
                else
                {
                    _enabledColorRegions = new[] { true, true, true, true, true, true };
                }
            }
        }

        private void buttonJustMated_Click(object sender, EventArgs e)
        {
            CreateIncubationEntry();
        }

        public Species CurrentSpecies
        {
            get => _currentSpecies;
            set
            {
                _currentSpecies = value;
                statWeighting.SetSpecies(value);
            }
        }

        private void listViewSpeciesBP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewSpeciesBP.SelectedIndices.Count > 0
                && !string.IsNullOrEmpty(listViewSpeciesBP.SelectedItems[0]?.Text)
                && (_currentSpecies == null
                    || listViewSpeciesBP.SelectedItems[0].Text != _currentSpecies.DescriptiveNameAndMod)
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
            if (_currentSpecies == species) return;

            // automatically set preset if preset with the speciesname exists
            _updateBreedingPlanAllowed = false;
            if (!statWeighting.TrySetPresetByName(species.name))
                statWeighting.TrySetPresetByName("Default");
            _updateBreedingPlanAllowed = true;

            DetermineBestBreeding(setSpecies: species);

            //// update listviewSpeciesBP
            // deselect currently selected species
            if (listViewSpeciesBP.SelectedItems.Count > 0)
                listViewSpeciesBP.SelectedItems[0].Selected = false;
            for (int i = 0; i < listViewSpeciesBP.Items.Count; i++)
            {
                if (listViewSpeciesBP.Items[i].Text == _currentSpecies.DescriptiveNameAndMod)
                {
                    listViewSpeciesBP.Items[i].Focused = true;
                    listViewSpeciesBP.Items[i].Selected = true;
                    break;
                }
            }
        }

        private void checkBoxIncludeCooldowneds_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.IncludeCooldownsInBreedingPlan = cbBPIncludeCooldowneds.Checked;
            DetermineBestBreeding(_chosenCreature, true);
        }

        private void cbBPIncludeCryoCreatures_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.IncludeCryoedInBreedingPlan = cbBPIncludeCryoCreatures.Checked;
            DetermineBestBreeding(_chosenCreature, true);
        }

        private void nudMutationLimit_ValueChanged(object sender, EventArgs e)
        {
            DetermineBestBreeding(_chosenCreature, true);
        }

        private void radioButtonBPTopStatsCn_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBPTopStatsCn.Checked)
            {
                _breedingMode = BreedingMode.TopStatsConservative;
                CalculateBreedingScoresAndDisplayPairs();
            }
        }

        private void radioButtonBPTopStats_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBPTopStats.Checked)
            {
                _breedingMode = BreedingMode.TopStatsLucky;
                CalculateBreedingScoresAndDisplayPairs();
            }
        }

        private void radioButtonBPHighStats_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBPHighStats.Checked)
            {
                _breedingMode = BreedingMode.BestNextGen;
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
                if (s.breeding == null || !creatures.Any(c => c.Species == s && c.Status == CreatureStatus.Available && c.sex == Sex.Female) || !creatures.Any(c => c.Species == s && c.Status == CreatureStatus.Available && c.sex == Sex.Male))
                    lvi.ForeColor = Color.LightGray;
                listViewSpeciesBP.Items.Add(lvi);
            }

            // select previous selected species again
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
                CreateIncubationTimer?.Invoke(pedigreeCreatureBest.Creature.Mother, pedigreeCreatureBest.Creature.Father, _incubationTime, startNow);

                // set cooldown for mother
                Species species = pedigreeCreatureBest.Creature.Mother.Species;
                if (species?.breeding != null)
                {
                    pedigreeCreatureBest.Creature.Mother.cooldownUntil = DateTime.Now.AddSeconds(species.breeding.matingCooldownMinAdjusted);
                    // update breeding plan
                    DetermineBestBreeding(_chosenCreature, true);
                }
            }
        }

        public void UpdateBreedingData()
        {
            SetBreedingData(_currentSpecies);
        }

        public int MutationLimit
        {
            get => (int)nudBPMutationLimit.Value;
            set => nudBPMutationLimit.Value = value;
        }

        private enum BreedingMode
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
            Loc.ControlText(lbBPBreedingScore, _tt);
            Loc.ControlText(rbBPTopStatsCn, _tt);
            Loc.ControlText(rbBPTopStats, _tt);
            Loc.ControlText(rbBPHighStats, _tt);
            Loc.ControlText(btBPJustMated, _tt);
            Loc.SetToolTip(nudBPMutationLimit, _tt);
            Loc.SetToolTip(cbBPTagExcludeDefault, _tt);
        }

        private void cbServerFilterLibrary_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.UseServerFilterForBreedingPlan = cbServerFilterLibrary.Checked;
            CalculateBreedingScoresAndDisplayPairs();
        }

        private void btShowAllCreatures_Click(object sender, EventArgs e)
        {
            // remove restriction on one creature
            _chosenCreature = null;
            CalculateBreedingScoresAndDisplayPairs();
        }

        private void cbOwnerFilterLibrary_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.UseOwnerFilterForBreedingPlan = cbOwnerFilterLibrary.Checked;
            CalculateBreedingScoresAndDisplayPairs();
        }

        private void cbOnlyOneSuggestionForFemales_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.BreedingPlanOnlyBestSuggestionForEachFemale = cbBPOnlyOneSuggestionForFemales.Checked;
            CalculateBreedingScoresAndDisplayPairs();
        }

        private void cbMutationLimitOnlyOnePartner_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.BreedingPlanOnePartnerMoreMutationsThanLimit = cbBPMutationLimitOnlyOnePartner.Checked;
            CalculateBreedingScoresAndDisplayPairs();
        }
    }
}
