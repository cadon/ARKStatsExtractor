using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.Library;
using ARKBreedingStats.Pedigree;
using ARKBreedingStats.Properties;
using ARKBreedingStats.raising;
using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.utils;
using ARKBreedingStats.values;

namespace ARKBreedingStats.BreedingPlanning
{
    public partial class BreedingPlan : UserControl
    {
        public event Action<Creature, bool> EditCreature;
        public event Action<Creature> BestBreedingPartners;
        public event Action<Creature> DisplayInPedigree;
        public event Raising.createIncubationEventHandler CreateIncubationTimer;
        public event Form1.SetMessageLabelTextEventHandler SetMessageLabelText;
        public event Action<Species> SetGlobalSpecies;
        private Creature[] _females;
        private Creature[] _males;
        private List<BreedingPair> _breedingPairs;
        private Species _currentSpecies;
        /// <summary>
        /// How much are the stats weighted when looking for the best
        /// </summary>
        private double[] _statWeights = new double[Stats.StatsCount];
        /// <summary>
        /// Indicates if high stats are only considered if any, odd or even.
        /// </summary>
        private byte[] _statOddEvens = new byte[Stats.StatsCount];
        /// <summary>
        /// The best possible levels of the selected species for each stat.
        /// If the weighting is negative, a low level is considered better.
        /// </summary>
        private readonly int[] _bestLevels = new int[Stats.StatsCount];
        /// <summary>
        /// The best possible levels of the selected species for each stat, after the filters are applied.
        /// If the weighting is negative, a low level is considered better.
        /// </summary>
        private readonly int[] _bestLevelsFiltered = new int[Stats.StatsCount];
        private readonly List<PedigreeCreature> _pcs = new List<PedigreeCreature>();
        private readonly List<PictureBox> _pbs = new List<PictureBox>();
        private bool[] _enabledColorRegions;
        private TimeSpan _incubationTime;
        private Creature _chosenCreature;
        private BreedingMode _breedingMode;
        public readonly StatWeighting StatWeighting;
        public bool BreedingPlanNeedsUpdate;
        private bool _speciesInfoNeedsUpdate;
        private readonly Debouncer _breedingPlanDebouncer = new Debouncer();
        /// <summary>
        /// If that is true, not all creatures of a species are considered, just a manually selected subset.
        /// </summary>
        private bool _onlyShowingASubset;

        /// <summary>
        /// Set to false if settings are changed and update should only performed after that.
        /// </summary>
        private bool _updateBreedingPlanAllowed;
        public CreatureCollection CreatureCollection;
        private readonly ToolTip _tt = new ToolTip { AutoPopDelay = 10000 };

        public BreedingPlan()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            for (int i = 0; i < Stats.StatsCount; i++)
                _statWeights[i] = 1;

            _breedingMode = BreedingMode.TopStatsConservative;

            _breedingPairs = new List<BreedingPair>();
            pedigreeCreatureBest.SetIsVirtual(true);
            pedigreeCreatureWorst.SetIsVirtual(true);
            pedigreeCreatureBestPossibleInSpecies.SetIsVirtual(true);
            pedigreeCreatureBestPossibleInSpeciesFiltered.SetIsVirtual(true);
            pedigreeCreatureBest.OnlyLevels = true;
            pedigreeCreatureWorst.OnlyLevels = true;
            pedigreeCreatureBestPossibleInSpecies.OnlyLevels = true;
            pedigreeCreatureBestPossibleInSpeciesFiltered.OnlyLevels = true;
            pedigreeCreatureBest.Clear();
            pedigreeCreatureWorst.Clear();
            pedigreeCreatureBestPossibleInSpecies.Clear();
            pedigreeCreatureBestPossibleInSpeciesFiltered.Clear();
            pedigreeCreatureBest.HandCursor = false;
            pedigreeCreatureWorst.HandCursor = false;
            pedigreeCreatureBestPossibleInSpecies.HandCursor = false;
            pedigreeCreatureBestPossibleInSpeciesFiltered.HandCursor = false;

            StatWeighting = statWeighting1;
            StatWeighting.WeightingsChanged += StatWeighting_WeightingsChanged;
            BreedingPlanNeedsUpdate = false;

            cbServerFilterLibrary.Checked = Settings.Default.UseServerFilterForBreedingPlan;
            cbOwnerFilterLibrary.Checked = Settings.Default.UseOwnerFilterForBreedingPlan;
            cbBPIncludeCooldowneds.Checked = Settings.Default.IncludeCooldownsInBreedingPlan;
            cbBPIncludeCryoCreatures.Checked = Settings.Default.IncludeCryoedInBreedingPlan;
            cbBPOnlyOneSuggestionForFemales.Checked = Settings.Default.BreedingPlanOnlyBestSuggestionForEachFemale;
            cbBPMutationLimitOnlyOnePartner.Checked = Settings.Default.BreedingPlanOnePartnerMoreMutationsThanLimit;
            CbIgnoreSexInPlanning.Checked = Settings.Default.IgnoreSexInBreedingPlan;
            CbDontSuggestOverLimitOffspring.Checked = Settings.Default.BreedingPlanDontSuggestOverLimitOffspring;

            tagSelectorList1.OnTagChanged += TagSelectorList1_OnTagChanged;

            nudBPMutationLimit.NeutralNumber = -1;
            _updateBreedingPlanAllowed = true;
        }

        private void StatWeighting_WeightingsChanged()
        {
            // check if sign of a weighting changed (then the best levels change)
            bool signChangedOrOddEven = false;
            var newWeightings = StatWeighting.Weightings;
            var newOddEvens = StatWeighting.AnyOddEven;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (_statOddEvens[s] != newOddEvens[s]
                    || Math.Sign(_statWeights[s]) != Math.Sign(newWeightings[s]))
                {
                    signChangedOrOddEven = true;
                    break;
                }
            }
            _statWeights = newWeightings;
            _statOddEvens = newOddEvens;
            if (signChangedOrOddEven) DetermineBestLevels();

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
            pedigreeCreatureBestPossibleInSpeciesFiltered.CreatureEdit += EditCreature;
            pedigreeCreatureBest.CreatureClicked += SetBreedingPair;
            pedigreeCreatureWorst.CreatureClicked += SetBreedingPair;
        }

        /// <summary>
        /// Set species or specific creature and calculate the breeding pairs.
        /// </summary>
        public void DetermineBestBreeding(Creature chosenCreature = null, bool forceUpdate = false, Species setSpecies = null, List<Creature> onlyConsiderTheseCreatures = null)
        {
            if (CreatureCollection == null) return;

            _onlyShowingASubset = onlyConsiderTheseCreatures != null && onlyConsiderTheseCreatures.Count > 1;

            Species selectedSpecies = null;
            if (_onlyShowingASubset)
                selectedSpecies = onlyConsiderTheseCreatures[0].Species;
            if (chosenCreature != null)
                selectedSpecies = chosenCreature.Species;
            _speciesInfoNeedsUpdate = false;
            if (selectedSpecies == null)
                selectedSpecies = setSpecies ?? _currentSpecies;
            if (selectedSpecies != null && _currentSpecies != selectedSpecies)
            {
                CurrentSpecies = selectedSpecies;
                _speciesInfoNeedsUpdate = true;

                EnabledColorRegions = _currentSpecies?.EnabledColorRegions;

                BreedingPlanNeedsUpdate = true;
            }

            _statWeights = StatWeighting.Weightings;
            _statOddEvens = StatWeighting.AnyOddEven;

            if (forceUpdate || BreedingPlanNeedsUpdate || _onlyShowingASubset)
            {
                if (_onlyShowingASubset)
                {
                    Creatures = onlyConsiderTheseCreatures.Where(c => c.speciesBlueprint == _currentSpecies.blueprintPath
                                                                      && !c.flags.HasFlag(CreatureFlags.Neutered)
                                                                      && !c.flags.HasFlag(CreatureFlags.Placeholder)
                        )
                        .ToList();
                }
                else
                {
                    Creatures = CreatureCollection.creatures
                        .Where(c => c.speciesBlueprint == _currentSpecies.blueprintPath
                                    && !c.flags.HasFlag(CreatureFlags.Neutered)
                                    && !c.flags.HasFlag(CreatureFlags.Placeholder)
                                    && (c.Status == CreatureStatus.Available
                                        || (c.Status == CreatureStatus.Cryopod && cbBPIncludeCryoCreatures.Checked))
                                    && (cbBPIncludeCooldowneds.Checked
                                        || !(c.cooldownUntil > DateTime.Now
                                             || c.growingUntil > DateTime.Now
                                            )
                                    )
                        )
                        .ToList();
                }
            }

            _chosenCreature = chosenCreature;
            CalculateBreedingScoresAndDisplayPairs();
            BreedingPlanNeedsUpdate = false;
        }

        private IEnumerable<Creature> FilterByTags(IEnumerable<Creature> cl)
        {
            if (cl == null) return null;

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
            int crCountM = _males?.Length ?? 0;
            IEnumerable<Creature> selectFemales;
            IEnumerable<Creature> selectMales = null;
            if (considerChosenCreature && (_chosenCreature.sex == Sex.Female || _currentSpecies.noGender))
            {
                selectFemales = new List<Creature>(); // the specific creature is added after the filtering
            }
            else if (!cbBPMutationLimitOnlyOnePartner.Checked && considerMutationLimit)
            {
                selectFemales = FilterByTags(_females.Where(c => c.Mutations <= nudBPMutationLimit.Value));
                creaturesMutationsFilteredOut = _females.Any(c => c.Mutations > nudBPMutationLimit.Value);
            }
            else selectFemales = FilterByTags(_females);

            if (considerChosenCreature && !_currentSpecies.noGender && _chosenCreature.sex == Sex.Male)
            {
                selectMales = new List<Creature>(); // the specific creature is added after the filtering
            }
            else if (!cbBPMutationLimitOnlyOnePartner.Checked && considerMutationLimit)
            {
                if (_males != null)
                {
                    selectMales = FilterByTags(_males.Where(c => c.Mutations <= nudBPMutationLimit.Value));
                    creaturesMutationsFilteredOut = creaturesMutationsFilteredOut ||
                                                    _males.Any(c => c.Mutations > nudBPMutationLimit.Value);
                }
            }
            else selectMales = FilterByTags(_males);

            // filter by servers
            if (cbServerFilterLibrary.Checked && (Settings.Default.FilterHideServers?.Any() ?? false))
            {
                selectFemales = selectFemales.Where(c => !Settings.Default.FilterHideServers.Contains(c.server));
                selectMales = selectMales?.Where(c => !Settings.Default.FilterHideServers.Contains(c.server));
            }
            // filter by owner
            if (cbOwnerFilterLibrary.Checked && (Settings.Default.FilterHideOwners?.Any() ?? false))
            {
                selectFemales = selectFemales.Where(c => !Settings.Default.FilterHideOwners.Contains(c.owner));
                selectMales = selectMales?.Where(c => !Settings.Default.FilterHideOwners.Contains(c.owner));
            }
            // filter by tribe
            if (cbTribeFilterLibrary.Checked && (Settings.Default.FilterHideTribes?.Any() ?? false))
            {
                selectFemales = selectFemales.Where(c => !Settings.Default.FilterHideTribes.Contains(c.tribe));
                selectMales = selectMales?.Where(c => !Settings.Default.FilterHideTribes.Contains(c.tribe));
            }

            Creature[] selectedFemales = selectFemales.ToArray();
            Creature[] selectedMales = selectMales?.ToArray();

            bool creaturesTagFilteredOut = (crCountF != selectedFemales.Length)
                                              || (crCountM != (selectedMales?.Length ?? 0));

            bool displayFilterWarning = true;

            lbBreedingPlanHeader.Text = _currentSpecies.DescriptiveNameAndMod
                                        + (considerChosenCreature ? " (" + string.Format(Loc.S("onlyPairingsWith"), _chosenCreature.name) + ")" : string.Empty)
                                        + (_onlyShowingASubset ? " (only subset)" : string.Empty);
            if (considerChosenCreature && (_chosenCreature.flags.HasFlag(CreatureFlags.Neutered) || _chosenCreature.Status != CreatureStatus.Available))
                lbBreedingPlanHeader.Text += $"{Loc.S("BreedingNotPossible")} ! ({(_chosenCreature.flags.HasFlag(CreatureFlags.Neutered) ? Loc.S("Neutered") : Loc.S("notAvailable"))})";

            var combinedCreatures = new List<Creature>(selectedFemales);
            if (selectedMales != null)
                combinedCreatures.AddRange(selectedMales);

            if (Settings.Default.IgnoreSexInBreedingPlan || _currentSpecies.noGender)
            {
                selectedFemales = combinedCreatures.ToArray();
                selectedMales = combinedCreatures.ToArray();
            }

            if (creaturesTagFilteredOut)
            {
                // if creatures are filtered out, set the best possible creature according to the filtering
                SetBestLevels(_bestLevelsFiltered, combinedCreatures, false);
                pedigreeCreatureBestPossibleInSpeciesFiltered.Visible = true;
            }
            else
            {
                pedigreeCreatureBestPossibleInSpeciesFiltered.Visible = false;
            }

            // if only pairings for one specific creatures are shown, add the creature after the filtering
            if (considerChosenCreature)
            {
                if (_chosenCreature.sex == Sex.Female)
                    selectedFemales = new[] { _chosenCreature };
                if (_chosenCreature.sex == Sex.Male)
                    selectedMales = new[] { _chosenCreature };
            }

            if (!selectedFemales.Any() || !selectedMales.Any())
            {
                NoPossiblePairingsFound(creaturesMutationsFilteredOut);
            }
            else
            {
                pedigreeCreature1.Show();
                pedigreeCreature2.Show();
                lbBPBreedingScore.Show();

                short[] bestPossLevels = new short[Stats.StatsCount]; // best possible levels

                var levelLimitWithOutDomLevels = (CreatureCollection.CurrentCreatureCollection?.maxServerLevel ?? 0) - (CreatureCollection.CurrentCreatureCollection?.maxDomLevel ?? 0);
                if (levelLimitWithOutDomLevels < 0) levelLimitWithOutDomLevels = 0;

                _breedingPairs = BreedingScore.CalculateBreedingScores(selectedFemales, selectedMales, _currentSpecies,
                    bestPossLevels, _statWeights, _bestLevels, _breedingMode,
                    considerChosenCreature, considerMutationLimit, (int)nudBPMutationLimit.Value,
                    ref creaturesMutationsFilteredOut, levelLimitWithOutDomLevels, CbDontSuggestOverLimitOffspring.Checked,
                    cbBPOnlyOneSuggestionForFemales.Checked, _statOddEvens);

                //double minScore = _breedingPairs.LastOrDefault()?.BreedingScore ?? 0;
                //if (minScore < 0)
                //{
                //    foreach (BreedingPair bp in _breedingPairs)
                //        bp.BreedingScore -= minScore;
                //}

                var sb = new StringBuilder();
                // draw best parents
                using (var brush = new SolidBrush(Color.Black))
                {
                    for (int i = 0; i < _breedingPairs.Count && i < CreatureCollection.maxBreedingSuggestions; i++)
                    {
                        PedigreeCreature pc;
                        if (2 * i < _pcs.Count)
                        {
                            _pcs[2 * i].Creature = _breedingPairs[i].Mother;
                            _pcs[2 * i].enabledColorRegions = _enabledColorRegions;
                            _pcs[2 * i].comboId = i;
                            _pcs[2 * i].Show();
                        }
                        else
                        {
                            pc = new PedigreeCreature(_breedingPairs[i].Mother, _enabledColorRegions, i, true);
                            pc.CreatureClicked += SetBreedingPair;
                            pc.CreatureEdit += CreatureEdit;
                            pc.RecalculateBreedingPlan += RecalculateBreedingPlan;
                            pc.BestBreedingPartners += BestBreedingPartners;
                            pc.DisplayInPedigree += DisplayInPedigree;
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
                            pb = new PictureBox { Size = new Size(87, 35) };
                            _pbs.Add(pb);
                            flowLayoutPanelPairs.Controls.Add(pb);
                        }

                        if (2 * i + 1 < _pcs.Count)
                        {
                            _pcs[2 * i + 1].Creature = _breedingPairs[i].Father;
                            _pcs[2 * i + 1].enabledColorRegions = _enabledColorRegions;
                            _pcs[2 * i + 1].comboId = i;
                            _pcs[2 * i + 1].Show();
                        }
                        else
                        {
                            pc = new PedigreeCreature(_breedingPairs[i].Father, _enabledColorRegions, i, true);
                            pc.CreatureClicked += SetBreedingPair;
                            pc.CreatureEdit += CreatureEdit;
                            pc.RecalculateBreedingPlan += RecalculateBreedingPlan;
                            pc.BestBreedingPartners += BestBreedingPartners;
                            pc.DisplayInPedigree += DisplayInPedigree;
                            flowLayoutPanelPairs.Controls.Add(pc);
                            flowLayoutPanelPairs.SetFlowBreak(pc, true);
                            _pcs.Add(pc);
                        }

                        sb.Clear();

                        Bitmap bm = new Bitmap(pb.Width, pb.Height);
                        using (Graphics g = Graphics.FromImage(bm))
                        {
                            g.TextRenderingHint = TextRenderingHint.AntiAlias;
                            brush.Color = Utils.MutationColor;
                            if (_breedingPairs[i].Mother.Mutations < Ark.MutationPossibleWithLessThan)
                            {
                                g.FillRectangle(brush, 0, 5, 10, 10);
                                sb.AppendLine(_breedingPairs[i].Mother + " can produce a mutation.");
                            }
                            if (_breedingPairs[i].Father.Mutations < Ark.MutationPossibleWithLessThan)
                            {
                                g.FillRectangle(brush, 77, 5, 10, 10);
                                sb.AppendLine(_breedingPairs[i].Father + " can produce a mutation.");
                            }

                            var colorPercent = (int)(_breedingPairs[i].BreedingScore.OneNumber * 12.5);
                            // outline
                            brush.Color = Utils.GetColorFromPercent(colorPercent, -.2);
                            g.FillRectangle(brush, 0, 15, 87, 5);
                            g.FillRectangle(brush, 20, 10, 47, 15);
                            // fill
                            brush.Color =
                                Utils.GetColorFromPercent(colorPercent, 0.5);
                            g.FillRectangle(brush, 1, 16, 85, 3);
                            g.FillRectangle(brush, 21, 11, 45, 13);
                            if (_breedingPairs[i].HighestOffspringOverLevelLimit)
                            {
                                brush.Color = Color.Red;
                                g.FillRectangle(brush, 15, 26, 55, 3);
                                sb.AppendLine("The highest possible and fully leveled offspring is over the level limit!");
                            }
                            // breeding score text
                            brush.Color = Color.Black;
                            g.DrawString(_breedingPairs[i].BreedingScore.ToString("N4"),
                                new Font("Microsoft Sans Serif", 8.25f), brush, 24, 12);
                            pb.Image = bm;
                        }

                        _tt.SetToolTip(pb, sb.Length > 0 ? sb.ToString() : null);
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
                        var chosenFemalesAndMales = selectedFemales.Concat(selectedMales);
                        var usedBestStats = creaturesTagFilteredOut ? _bestLevelsFiltered : _bestLevels;
                        foreach (Creature cr in chosenFemalesAndMales)
                        {
                            bestCreatureAlreadyAvailable = true;
                            for (int s = 0; s < Stats.StatsCount; s++)
                            {
                                // if the stat is not a top stat and the stat is leveled in wild creatures
                                if (cr.Species.UsesStat(s) && cr.levelsWild[s] != usedBestStats[s])
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
                            SetMessageLabelText(
                                string.Format(Loc.S("AlreadyCreatureWithTopStats"), bestCreature.name,
                                    Utils.SexSymbol(bestCreature.sex)), MessageBoxIcon.Warning);
                        }
                    }
                }
                else
                    SetParents(-1);
            }

            if (_speciesInfoNeedsUpdate)
                SetBreedingData(_currentSpecies);

            if (displayFilterWarning)
            {
                // display warning if breeding pairs are filtered out
                string warningText = null;
                if (creaturesTagFilteredOut) warningText = Loc.S("BPsomeCreaturesAreFilteredOutTags") + ".\r\n" + Loc.S("BPTopStatsShownMightNotTotalTopStats");
                if (creaturesMutationsFilteredOut) warningText = (!string.IsNullOrEmpty(warningText) ? warningText + "\r\n" : string.Empty) + Loc.S("BPsomePairingsAreFilteredOutMutations");
                if (!string.IsNullOrEmpty(warningText)) SetMessageLabelText(warningText, MessageBoxIcon.Warning);
            }

            this.ResumeDrawing();

            if (considerChosenCreature) btShowAllCreatures.Text = string.Format(Loc.S("BPCancelRestrictionOn"), _chosenCreature.name);
            if (_onlyShowingASubset) btShowAllCreatures.Text = string.Format(Loc.S("BPCancelRestrictionOn"), "subset");
            btShowAllCreatures.Visible = considerChosenCreature || _onlyShowingASubset;
            ResumeLayout();
        }

        /// <summary>
        /// Hide unused controls and display info.
        /// </summary>
        private void NoPossiblePairingsFound(bool creaturesMutationsFilteredOut)
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
            if (!cbBPIncludeCryoCreatures.Checked)
                cbBPIncludeCryoCreatures.BackColor = Color.LightSalmon;
            if (creaturesMutationsFilteredOut)
                nudBPMutationLimit.BackColor = Color.LightSalmon;
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
                    BreedingPlanNeedsUpdate = true;
            }
        }

        private void RecalculateBreedingPlan()
        {
            DetermineBestBreeding(_chosenCreature, true);
        }

        internal void UpdateIfNeeded()
        {
            if (BreedingPlanNeedsUpdate)
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
            SetMessageLabelText?.Invoke();
            cbBPIncludeCryoCreatures.BackColor = Color.Transparent;
            nudBPMutationLimit.BackColor = SystemColors.Window;
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

                if (_currentSpecies.noGender)
                {
                    _females = value.ToArray();
                    _males = null;
                }
                else
                {
                    _females = value.Where(c => c.sex == Sex.Female).ToArray();
                    _males = value.Where(c => c.sex == Sex.Male).ToArray();
                }

                DetermineBestLevels(value);
            }
        }

        private void DetermineBestLevels(List<Creature> creatures = null)
        {
            pedigreeCreatureBestPossibleInSpecies.Clear();
            if (creatures == null)
            {
                if (_females == null) return;
                creatures = _females.ToList();
                if (_males != null)
                    creatures.AddRange(_males);
            }

            SetBestLevels(_bestLevels, creatures, true);
        }

        /// <summary>
        /// Sets the best levels in the passed array according to the stat weights and the passed creature list.
        /// </summary>
        /// <param name="bestLevels"></param>
        /// <param name="creatures"></param>
        /// <param name="bestInSpecies">If true, the display of the best species library will be updated, if false the best filtered species will be updated.</param>
        private void SetBestLevels(int[] bestLevels, IEnumerable<Creature> creatures, bool bestInSpecies)
        {
            BreedingScore.SetBestLevels(creatures, bestLevels, _statWeights, _statOddEvens);

            // display top levels in species
            int? levelStep = CreatureCollection.getWildLevelStep();

            var bestLevelsOfWhat = _onlyShowingASubset ? "Best of manual selection"
                : string.Format(Loc.S(bestInSpecies ? "BestPossibleSpeciesLibrary" : "BestPossibleSpeciesLibraryFiltered"), _currentSpecies.name);

            Creature crB = new Creature(_currentSpecies, bestLevelsOfWhat,
                null, null, 0, new int[Stats.StatsCount], null, null, 1, true, levelStep: levelStep);
            bool totalLevelUnknown = false;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s == Stats.Torpidity) continue;
                crB.levelsWild[s] = bestLevels[s];
                if (crB.levelsWild[s] == -1)
                    totalLevelUnknown = true;
                crB.topBreedingStats[s] = crB.levelsWild[s] > 0 && crB.levelsWild[s] == _bestLevels[s];
            }
            crB.levelsWild[Stats.Torpidity] = crB.levelsWild.Sum();
            crB.RecalculateCreatureValues(levelStep);
            var pc = bestInSpecies
                ? pedigreeCreatureBestPossibleInSpecies
                : pedigreeCreatureBestPossibleInSpeciesFiltered;
            pc.TotalLevelUnknown = totalLevelUnknown;
            pc.Creature = crB;
        }

        private void SetBreedingPair(Creature c, int comboIndex, MouseEventArgs e)
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
            Creature crB = new Creature(_currentSpecies, string.Empty, levelsWild: new int[Stats.StatsCount], levelsMutated: new int[Stats.StatsCount], isBred: true, levelStep: levelStep);
            Creature crW = new Creature(_currentSpecies, string.Empty, levelsWild: new int[Stats.StatsCount], levelsMutated: new int[Stats.StatsCount], isBred: true, levelStep: levelStep);
            Creature mother = _breedingPairs[comboIndex].Mother;
            Creature father = _breedingPairs[comboIndex].Father;
            crB.Mother = mother;
            crB.Father = father;
            crW.Mother = mother;
            crW.Father = father;
            double probabilityBest = 1;
            bool totalLevelUnknown = false; // if stats are unknown, total level is as well (==> oxygen, speed)
            bool topStatBreedingMode = _breedingMode == BreedingMode.TopStatsConservative || _breedingMode == BreedingMode.TopStatsLucky;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s == Stats.Torpidity) continue;
                crB.levelsWild[s] = _statWeights[s] < 0 ? Math.Min(mother.levelsWild[s], father.levelsWild[s]) : BreedingScore.GetHigherBestLevel(mother.levelsWild[s], father.levelsWild[s], _statOddEvens[s]);
                crB.levelsMutated[s] = (crB.levelsWild[s] == mother.levelsWild[s] ? mother : father).levelsMutated?[s] ?? 0;
                crB.valuesBreeding[s] = StatValueCalculation.CalculateValue(_currentSpecies, s, crB.levelsWild[s], crB.levelsMutated[s], 0, true, 1, 0);
                crB.topBreedingStats[s] = (_currentSpecies.stats[s].IncPerTamedLevel != 0 && crB.levelsWild[s] == _bestLevels[s]);
                crW.levelsWild[s] = _statWeights[s] < 0 ? Math.Max(mother.levelsWild[s], father.levelsWild[s]) : Math.Min(mother.levelsWild[s], father.levelsWild[s]);
                crB.levelsMutated[s] = (crW.levelsWild[s] == mother.levelsWild[s] ? mother : father).levelsMutated?[s] ?? 0;
                crW.valuesBreeding[s] = StatValueCalculation.CalculateValue(_currentSpecies, s, crW.levelsWild[s], crW.levelsMutated[s], 0, true, 1, 0);
                crW.topBreedingStats[s] = (_currentSpecies.stats[s].IncPerTamedLevel != 0 && crW.levelsWild[s] == _bestLevels[s]);
                if (crB.levelsWild[s] == -1 || crW.levelsWild[s] == -1)
                    totalLevelUnknown = true;
                // in top stats breeding mode consider only probability of top stats
                if (crB.levelsWild[s] > crW.levelsWild[s]
                    && (!topStatBreedingMode || crB.topBreedingStats[s]))
                    probabilityBest *= Ark.ProbabilityInheritHigherLevel;
                else if (crB.levelsWild[s] < crW.levelsWild[s]
                         && (!topStatBreedingMode || crB.topBreedingStats[s]))
                    probabilityBest *= Ark.ProbabilityInheritLowerLevel;
            }
            crB.levelsWild[Stats.Torpidity] = crB.levelsWild.Sum();
            crW.levelsWild[Stats.Torpidity] = crW.levelsWild.Sum();
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

        private bool[] EnabledColorRegions
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
                StatWeighting.SetSpecies(value);
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

            // automatically set preset if preset with the species name exists
            _updateBreedingPlanAllowed = false;
            StatWeighting.TrySetPresetBySpecies(species);
            _updateBreedingPlanAllowed = true;

            DetermineBestBreeding(setSpecies: species);

            // update listViewSpeciesBP
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

        public void SetSpeciesList(IList<Species> species, List<Creature> creatures)
        {
            Species previouslySelectedSpecies = listViewSpeciesBP.SelectedItems.Count > 0 ? listViewSpeciesBP.SelectedItems[0].Tag as Species : null;

            listViewSpeciesBP.BeginUpdate();
            listViewSpeciesBP.Items.Clear();

            var breedableSpecies = new List<ListViewItem>();
            var unbreedableSpecies = new List<ListViewItem>();

            var availableCreaturesBySpecies = creatures
                .Where(c => c.Species != null && (c.Status == CreatureStatus.Available || c.Status == CreatureStatus.Cryopod))
                .GroupBy(c => c.Species).ToDictionary(g => g.Key, g => g.ToArray());

            var ignoreSex = Properties.Settings.Default.IgnoreSexInBreedingPlan;

            foreach (Species s in species)
            {
                ListViewItem lvi = new ListViewItem { Text = s.DescriptiveNameAndMod, Tag = s };
                var ignoreSexInSpecies = ignoreSex || s.noGender;

                // check if species has both available males and females
                if (availableCreaturesBySpecies.TryGetValue(s, out var cs)
                    && ((ignoreSexInSpecies && cs.Length > 1)
                        || (cs.Any(c => c.sex == Sex.Female) && cs.Any(c => c.sex == Sex.Male))))
                {
                    breedableSpecies.Add(lvi);
                }
                else
                {
                    lvi.ForeColor = Color.LightGray;
                    unbreedableSpecies.Add(lvi);
                }
            }
            if (breedableSpecies.Any())
                listViewSpeciesBP.Items.AddRange(breedableSpecies.ToArray());
            if (unbreedableSpecies.Any())
                listViewSpeciesBP.Items.AddRange(unbreedableSpecies.ToArray());

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
            listViewSpeciesBP.EndUpdate();
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

        public bool IgnoreSexInBreedingPlan
        {
            set => CbIgnoreSexInPlanning.Checked = value;
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
            Loc.ControlText(lbBPBreedingScore, _tt);
            Loc.ControlText(rbBPTopStatsCn, _tt);
            Loc.ControlText(rbBPTopStats, _tt);
            Loc.ControlText(rbBPHighStats, _tt);
            Loc.ControlText(btBPJustMated, _tt);
            Loc.SetToolTip(nudBPMutationLimit, _tt);
            Loc.SetToolTip(cbBPTagExcludeDefault, _tt);
        }

        private void btShowAllCreatures_Click(object sender, EventArgs e)
        {
            // remove restriction on manually selected creatures
            _chosenCreature = null;
            if (_onlyShowingASubset)
            {
                BreedingPlanNeedsUpdate = true;
                DetermineBestBreeding();
            }
            else
                CalculateBreedingScoresAndDisplayPairs();
        }

        private void cbServerFilterLibrary_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.UseServerFilterForBreedingPlan = cbServerFilterLibrary.Checked;
            CalculateBreedingScoresAndDisplayPairs();
        }

        private void cbOwnerFilterLibrary_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.UseOwnerFilterForBreedingPlan = cbOwnerFilterLibrary.Checked;
            CalculateBreedingScoresAndDisplayPairs();
        }

        private void cbTribeFilterLibrary_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.UseTribeFilterForBreedingPlan = cbTribeFilterLibrary.Checked;
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

        private void CbIgnoreSexInPlanning_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.IgnoreSexInBreedingPlan = CbIgnoreSexInPlanning.Checked;
            CalculateBreedingScoresAndDisplayPairs();
        }

        private void CbDontSuggestOverLimitOffspring_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.BreedingPlanDontSuggestOverLimitOffspring = CbDontSuggestOverLimitOffspring.Checked;
            CalculateBreedingScoresAndDisplayPairs();
        }
    }
}
