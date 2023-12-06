using ARKBreedingStats.Library;
using ARKBreedingStats.miscClasses;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ARKBreedingStats.importExportGun;
using ARKBreedingStats.utils;
using ARKBreedingStats.ocr;

namespace ARKBreedingStats
{
    public partial class Form1
    {
        /// <summary>
        /// Set to true if many creatures are processed automatically, e.g. a bulk import.
        /// It will prevent visual feedback updates.
        /// </summary>
        private bool _dontUpdateExtractorVisualData;

        private void UpdateExtractorDetails()
        {
            panelExtrTE.Visible = rbTamedExtractor.Checked;
            panelExtrImpr.Visible = rbBredExtractor.Checked;
            groupBoxDetailsExtractor.Visible = !rbWildExtractor.Checked;
            if (rbTamedExtractor.Checked)
            {
                groupBoxDetailsExtractor.Text = "Taming-Effectiveness";
            }
            else if (rbBredExtractor.Checked)
            {
                groupBoxDetailsExtractor.Text = "Imprinting-Quality";
            }
        }

        /// <summary>
        /// This displays the sum of the chosen levels. This is the last step before a creature-extraction is considered as valid or not valid.
        /// </summary>
        private void ShowSumOfChosenLevels()
        {
            // The speedLevel is not chosen, but calculated from the other chosen levels, and must not be included in the sum, except all the other levels are determined uniquely!

            // this method will show only the offset of the value, it's less confusing to the user and gives all the infos needed
            int sumW = 0, sumD = 0;
            bool valid = true, inbound = true, allUnique = true;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s == Stats.Torpidity)
                    continue;
                if (_extractor.Results[s].Count > _extractor.ChosenResults[s])
                {
                    sumW += _statIOs[s].LevelWild > 0 ? _statIOs[s].LevelWild : 0;
                    sumD += _statIOs[s].LevelDom;
                    if (_extractor.Results[s].Count != 1)
                    {
                        allUnique = false;
                    }
                }
                else
                {
                    valid = false;
                    break;
                }
                _statIOs[s].TopLevel = LevelStatus.Neutral;
            }
            if (valid)
            {
                sumW -= allUnique || _statIOs[Stats.SpeedMultiplier].LevelWild < 0 ? 0 : _statIOs[Stats.SpeedMultiplier].LevelWild;
                string offSetWild = "✓";
                lbSumDom.Text = sumD.ToString();
                if (sumW <= _extractor.LevelWildSum)
                {
                    lbSumWild.ForeColor = SystemColors.ControlText;
                }
                else
                {
                    lbSumWild.ForeColor = Color.Red;
                    offSetWild = "+" + (sumW - _extractor.LevelWildSum);
                    inbound = false;
                }
                if (sumD == _extractor.LevelDomSum)
                {
                    lbSumDom.ForeColor = SystemColors.ControlText;
                }
                else
                {
                    lbSumDom.ForeColor = Color.Red;
                    inbound = false;
                    // if there are no other combination options, the total level may be wrong
                    if (_extractor.UniqueResults)
                        numericUpDownLevel.BackColor = Color.LightSalmon;
                }
                lbSumWild.Text = offSetWild;
            }
            else
            {
                lbSumWild.Text = Loc.S("na");
                lbSumDom.Text = Loc.S("na");
            }
            panelSums.BackColor = inbound ? SystemColors.Control : Color.FromArgb(255, 200, 200);

            bool torporLevelValid = numericUpDownLevel.Value > _statIOs[Stats.Torpidity].LevelWild;
            if (!torporLevelValid)
            {
                numericUpDownLevel.BackColor = Color.LightSalmon;
                _statIOs[Stats.Torpidity].Status = StatIOStatus.Error;
            }

            bool allValid = valid && inbound && torporLevelValid && _extractor.ValidResults;
            if (allValid)
            {
                radarChartExtractor.SetLevels(_statIOs.Select(s => s.LevelWild).ToArray());
                cbExactlyImprinting.BackColor = Color.Transparent;
                var species = speciesSelector1.SelectedSpecies;
                var checkTopLevels = _topLevels.TryGetValue(species, out int[] topSpeciesLevels);
                var checkLowLevels = _lowestLevels.TryGetValue(species, out int[] lowSpeciesLevels);

                var customStatNames = species.statNames;
                var statWeights = breedingPlan1.StatWeighting.GetWeightingForSpecies(species);
                if (statWeights.Item1 == null) checkLowLevels = false;
                var analysisState = LevelStatus.Neutral;
                var newTopStatsText = new List<string>();
                var topStatsText = new List<string>();

                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (s == Stats.Torpidity
                        || _statIOs[s].LevelWild <= 0)
                        continue;

                    var levelStatus = LevelStatus.Neutral;

                    if (checkTopLevels && (statWeights.Item1?[s] ?? 0) >= 0)
                    {
                        // higher stats are considered to be good. If no custom weightings are available, consider higher levels to be better.

                        // check if higher level is only considered if even or odd
                        if ((statWeights.Item2?[s] ?? 0) == 0 // even/odd doesn't matter
                            || (statWeights.Item2[s] == 1 && _statIOs[s].LevelWild % 2 == 1)
                            || (statWeights.Item2[s] == 2 && _statIOs[s].LevelWild % 2 == 0)
                            )
                        {
                            if (_statIOs[s].LevelWild == topSpeciesLevels[s])
                            {
                                levelStatus = LevelStatus.TopLevel;
                                topStatsText.Add(Utils.StatName(s, false, customStatNames));
                                if (analysisState != LevelStatus.NewTopLevel)
                                    analysisState = LevelStatus.TopLevel;
                            }
                            else if (topSpeciesLevels[s] != -1 && _statIOs[s].LevelWild > topSpeciesLevels[s])
                            {
                                levelStatus = LevelStatus.NewTopLevel;
                                newTopStatsText.Add(Utils.StatName(s, false, customStatNames));
                                analysisState = LevelStatus.NewTopLevel;
                            }
                        }
                    }
                    else if (checkLowLevels && statWeights.Item1[s] < 0)
                    {
                        // lower stats are considered to be good
                        if (_statIOs[s].LevelWild == lowSpeciesLevels[s])
                        {
                            levelStatus = LevelStatus.TopLevel;
                            topStatsText.Add(Utils.StatName(s, false, customStatNames));
                            if (analysisState != LevelStatus.NewTopLevel)
                                analysisState = LevelStatus.TopLevel;
                        }
                        else if (_statIOs[s].LevelWild < lowSpeciesLevels[s])
                        {
                            levelStatus = LevelStatus.NewTopLevel;
                            newTopStatsText.Add(Utils.StatName(s, false, customStatNames));
                            analysisState = LevelStatus.NewTopLevel;
                        }
                    }

                    // ASA can have up to 511 levels because 255 mutation levels also contribute to the wild value. TODO separate to mutation levels
                    if (_creatureCollection.Game != Ark.Asa)
                    {
                        if (_statIOs[s].LevelWild > 255)
                            levelStatus |= LevelStatus.UltraMaxLevel;
                        else if (_statIOs[s].LevelWild == 255)
                            levelStatus |= LevelStatus.MaxLevel;
                        else if (_statIOs[s].LevelWild == 254)
                            levelStatus |= LevelStatus.MaxLevelForLevelUp;
                    }

                    if (levelStatus != LevelStatus.Neutral)
                        _statIOs[s].TopLevel = levelStatus;
                }

                string infoText = null;
                if (newTopStatsText.Any())
                {
                    infoText = $"New top stats: {string.Join(", ", newTopStatsText)}";
                }
                if (topStatsText.Any())
                {
                    infoText += $"{(infoText == null ? null : "\n")}Existing top stats: {string.Join(", ", topStatsText)}";
                }

                if (infoText == null) infoText = "No top stats";

                creatureAnalysis1.SetStatsAnalysis(analysisState, infoText);
            }
            creatureInfoInputExtractor.ButtonEnabled = allValid;
            groupBoxRadarChartExtractor.Visible = allValid;
            creatureAnalysis1.Visible = allValid;
            // update inheritance info
            CreatureInfoInput_CreatureDataRequested(creatureInfoInputExtractor, false, true, false, 0, null);
        }

        /// <summary>
        /// Clears all the parameters of the extractor.
        /// </summary>
        /// <param name="clearExtraCreatureData">Also delete infos like the guid, parents and mutations</param>
        private void ClearAll(bool clearExtraCreatureData = true)
        {
            _extractor.Clear();
            listViewPossibilities.Items.Clear();
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _statIOs[s].Clear();
            }
            ExtractionFailed(); // set background of controls to neutral
            labelFootnote.Text = string.Empty;
            labelFootnote.BackColor = Color.Transparent;
            labelTE.Text = string.Empty;
            _activeStatIndex = -1;
            lbSumDom.Text = string.Empty;
            lbSumWild.Text = string.Empty;
            lbSumDomSB.Text = string.Empty;
            _updateTorporInTester = true;
            creatureInfoInputExtractor.ButtonEnabled = false;
            groupBoxPossibilities.Visible = false;
            groupBoxRadarChartExtractor.Visible = false;
            creatureAnalysis1.Visible = false;
            creatureAnalysis1.Clear();
            lbInfoYellowStats.Visible = false;
            button2TamingCalc.Visible = cbQuickWildCheck.Checked;
            groupBoxTamingInfo.Visible = cbQuickWildCheck.Checked;
            UpdateQuickTamingInfo();
            labelTamingInfo.Text = string.Empty;
            SetMessageLabelText();
            if (clearExtraCreatureData)
            {
                creatureInfoInputExtractor.MutationCounterMother = 0;
                creatureInfoInputExtractor.MutationCounterFather = 0;
                creatureInfoInputExtractor.DomesticatedAt = DateTime.Now;
                creatureInfoInputExtractor.parentListValid = false;
                creatureInfoInputExtractor.CreatureGuid = Guid.Empty;
                _exportedCreatureControl = null;
                creatureInfoInputExtractor.SetArkId(0, false);
            }

            creatureInfoInputExtractor.AlreadyExistingCreature = null;
        }

        private void buttonExtract_Click(object sender, EventArgs e)
        {
            ExtractLevels();
        }

        /// <summary>
        /// Extract the levels by using the inputs of the statIOs and various other controls of the extractor.
        /// </summary>
        /// <param name="autoExtraction"></param>
        /// <param name="statInputsHighPrecision">Set to true if the data is from an export file which has a higher precision for stat-values so the tolerance of calculations can be smaller.</param>
        /// <param name="possiblyMutagenApplied">Set to true if the creature may have Mutagen applied.
        /// This needs to be set true if the creature is imported from an exported file, because there's no way to know if there is Mutagen applied, and if set falsely to false it can sort out level combinations during the extraction.</param>
        /// <returns></returns>
        private bool ExtractLevels(bool autoExtraction = false, bool statInputsHighPrecision = false, bool showLevelsInOverlay = false, Creature existingCreature = null, bool possiblyMutagenApplied = false)
        {
            int activeStatKeeper = _activeStatIndex;
            ClearAll(_clearExtractionCreatureData);
            var mutagenApplied = possiblyMutagenApplied || creatureInfoInputExtractor.CreatureFlags.HasFlag(CreatureFlags.MutagenApplied);
            var bred = rbBredExtractor.Checked;

            _extractor.ExtractLevels(speciesSelector1.SelectedSpecies, (int)numericUpDownLevel.Value, _statIOs,
                    (double)numericUpDownLowerTEffBound.Value / 100, (double)numericUpDownUpperTEffBound.Value / 100,
                    rbTamedExtractor.Checked, bred,
                    (double)numericUpDownImprintingBonusExtractor.Value / 100, !cbExactlyImprinting.Checked,
                    _creatureCollection.allowMoreThanHundredImprinting, _creatureCollection.serverMultipliers.BabyImprintingStatScaleMultiplier,
                    _creatureCollection.considerWildLevelSteps, _creatureCollection.wildLevelStep, statInputsHighPrecision, mutagenApplied, out bool imprintingBonusChanged);

            numericUpDownImprintingBonusExtractor.ValueSave = (decimal)_extractor.ImprintingBonus * 100;
            numericUpDownImprintingBonusExtractor_ValueChanged(null, null);

            var possibleExtractionIssues = IssueNotes.Issue.CreatureLevel;
            if (cbExactlyImprinting.Checked)
                possibleExtractionIssues |= IssueNotes.Issue.ImprintingLocked;

            // if values are entered manually, it could be a creature from a mod the user hasn't loaded in ASB
            if (!statInputsHighPrecision)
                possibleExtractionIssues |= IssueNotes.Issue.ModValues;

            if (imprintingBonusChanged && !autoExtraction)
                possibleExtractionIssues |= IssueNotes.Issue.ImprintingNotPossible;

            bool everyStatHasAtLeastOneResult = _extractor.EveryStatHasAtLeastOneResult;

            // remove all results that require a total wild-level higher than the max
            // Tek-variants have 20% higher levels
            var additionalWildLevelsDueToMutagen = mutagenApplied ? (bred ? 5 : 20) : 0;
            _extractor.RemoveImpossibleTEsAccordingToMaxWildLevel((int)Math.Ceiling((_creatureCollection.maxWildLevel + additionalWildLevelsDueToMutagen) * (speciesSelector1.SelectedSpecies.name.StartsWith("Tek ") ? 1.2 : 1)));

            if (everyStatHasAtLeastOneResult && !_extractor.EveryStatHasAtLeastOneResult)
            {
                MessageBox.Show(string.Format(Loc.S("issueMaxWildLevelTooLow"), _creatureCollection.maxWildLevel), "ASB: Maybe the wild max level is set too low", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (!_extractor.SetStatLevelBoundsAndFilter(out int statIssue))
            {
                if (statIssue == -1)
                {
                    ExtractionFailed(possibleExtractionIssues
                                     | IssueNotes.Issue.WildTamedBred
                                     | IssueNotes.Issue.CreatureLevel
                                     | (rbTamedExtractor.Checked ? IssueNotes.Issue.TamingEffectivenessRange : IssueNotes.Issue.None));
                    return false;
                }
                possibleExtractionIssues |= IssueNotes.Issue.Typo | IssueNotes.Issue.CreatureLevel;
                _statIOs[statIssue].Status = StatIOStatus.Error;
                _statIOs[Stats.Torpidity].Status = StatIOStatus.Error;
            }

            // get mean-level (most probable for the wild levels)
            // TODO handle species without wild levels in speed better (some flyers)
            double meanWildLevel = Math.Round((double)_extractor.LevelWildSum / 7, 1);
            bool nonUniqueStats = false;

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (!_activeStats[s])
                {
                    _statIOs[s].Status = StatIOStatus.Neutral;
                }
                else if (_extractor.Results[s].Any())
                {
                    if (existingCreature != null)
                    {
                        // set the wild levels to the existing ones
                        int r = 0;
                        for (int b = 1; b < _extractor.Results[s].Count; b++)
                        {
                            if (_extractor.Results[s][b].levelWild == existingCreature.levelsWild[s]
                                && _extractor.Results[s][b].levelDom >= existingCreature.levelsDom[s]
                                && (_extractor.Results[s][b].TE.Mean < 0 || _extractor.Results[s][b].TE.Includes(existingCreature.tamingEff)))
                            {
                                r = b;
                                break;
                            }
                        }
                        SetLevelCombination(s, r == -1 ? 0 : r);
                    }
                    else
                    {
                        // choose the most probable wild-level, aka the level nearest to the mean of the wild levels.
                        int r = 0;
                        for (int b = 1; b < _extractor.Results[s].Count; b++)
                        {
                            if (Math.Abs(meanWildLevel - _extractor.Results[s][b].levelWild) < Math.Abs(meanWildLevel - _extractor.Results[s][r].levelWild))
                                r = b;
                        }

                        SetLevelCombination(s, r);
                    }
                    if (_extractor.Results[s].Count > 1)
                    {
                        _statIOs[s].Status = StatIOStatus.NonUnique;
                        nonUniqueStats = true;
                    }
                    else
                    {
                        _statIOs[s].Status = StatIOStatus.Unique;
                    }
                }
                else
                {
                    // no results for this stat
                    _statIOs[s].Status = StatIOStatus.Error;
                    _extractor.ValidResults = false;
                    if (rbTamedExtractor.Checked && _extractor.StatsWithTE.Contains(s))
                    {
                        possibleExtractionIssues |= IssueNotes.Issue.TamingEffectivenessRange;
                    }
                    // if the stat is changed by singleplayer-settings, list that as a possible issue
                    if (s == Stats.Health
                        || s == Stats.MeleeDamageMultiplier)
                    {
                        possibleExtractionIssues |= IssueNotes.Issue.Singleplayer;
                    }
                }
            }
            if (!_extractor.ValidResults)
            {
                ExtractionFailed(possibleExtractionIssues | IssueNotes.Issue.Typo | IssueNotes.Issue.WildTamedBred | IssueNotes.Issue.LockedDom |
                                 IssueNotes.Issue.OutdatedIngameValues | IssueNotes.Issue.ImprintingNotUpdated |
                                 (_statIOs[Stats.Torpidity].LevelWild >= (int)numericUpDownLevel.Value ? IssueNotes.Issue.CreatureLevel : IssueNotes.Issue.None));
                return false;
            }
            _extractor.UniqueResults = !nonUniqueStats;
            if (!_extractor.UniqueResults)
            {
                groupBoxPossibilities.Visible = true;
                lbInfoYellowStats.Visible = true;
            }

            // if damage has a possibility for the dom-levels to make it a valid sum, take this
            int domLevelsChosenSum = 0;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s != Stats.Torpidity)
                    domLevelsChosenSum += _extractor.Results[s][_extractor.ChosenResults[s]].levelDom;
            }
            if (domLevelsChosenSum != _extractor.LevelDomSum)
            {
                // sum of domlevels is not correct. Try to find another combination
                domLevelsChosenSum -= _extractor.Results[Stats.MeleeDamageMultiplier][_extractor.ChosenResults[Stats.MeleeDamageMultiplier]].levelDom;
                bool changeChosenResult = false;
                int cR = 0;
                for (int r = 0; r < _extractor.Results[Stats.MeleeDamageMultiplier].Count; r++)
                {
                    if (domLevelsChosenSum + _extractor.Results[Stats.MeleeDamageMultiplier][r].levelDom == _extractor.LevelDomSum)
                    {
                        cR = r;
                        changeChosenResult = true;
                        break;
                    }
                }
                if (changeChosenResult)
                    SetLevelCombination(Stats.MeleeDamageMultiplier, cR);
            }

            if (_extractor.PostTamed)
                SetUniqueTE();
            else
            {
                labelTE.Text = Loc.S("notYetTamed");
                labelTE.BackColor = Color.Transparent;
            }

            SetWildUnknownLevelsAccordingToOthers();

            lbSumDomSB.Text = _extractor.LevelDomSum.ToString();
            ShowSumOfChosenLevels();
            if (showLevelsInOverlay)
                ShowLevelsInOverlay();

            SetActiveStat(activeStatKeeper);

            if (!_extractor.PostTamed)
            {
                labelFootnote.Text = Loc.S("lbNotYetTamed");
                button2TamingCalc.Visible = true;

                // display taming info
                SetQuickTamingInfo(cbQuickWildCheck.Checked
                    ? _statIOs[Stats.Torpidity].LevelWild + 1
                    : (int)numericUpDownLevel.Value);
            }

            return true;
        }

        private void UpdateQuickTamingInfo()
        {
            bool showQuickTamingInfo = cbQuickWildCheck.Checked;
            if (showQuickTamingInfo)
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    int lvlWild = (int)Math.Round((_statIOs[s].Input - speciesSelector1.SelectedSpecies.stats[s].BaseValue) / (speciesSelector1.SelectedSpecies.stats[s].BaseValue * speciesSelector1.SelectedSpecies.stats[s].IncPerWildLevel));
                    _statIOs[s].LevelWild = lvlWild < 0 ? 0 : lvlWild;
                    _statIOs[s].LevelMut = 0;
                    _statIOs[s].LevelDom = 0;
                }
                SetQuickTamingInfo(_statIOs[Stats.Torpidity].LevelWild + 1);
            }
            panelWildTamedBred.Enabled = !showQuickTamingInfo;
            groupBoxDetailsExtractor.Enabled = !showQuickTamingInfo;
            numericUpDownLevel.Enabled = !showQuickTamingInfo;
            button2TamingCalc.Visible = showQuickTamingInfo;
            groupBoxTamingInfo.Visible = showQuickTamingInfo;
        }

        private void SetQuickTamingInfo(int level)
        {
            tamingControl1.SetSpecies(speciesSelector1.SelectedSpecies);
            tamingControl1.SetLevel(level);
            labelTamingInfo.Text = tamingControl1.quickTamingInfos;
            groupBoxTamingInfo.Visible = true;
        }

        /// <summary>
        /// Call this method if an extraction failed. Possible causes of the failure are displayed for the user.
        /// </summary>
        /// <param name="issues"></param>
        private void ExtractionFailed(IssueNotes.Issue issues = IssueNotes.Issue.None)
        {
            if (issues == IssueNotes.Issue.None)
            {
                // set background of inputs to neutral
                numericUpDownLevel.BackColor = SystemColors.Window;
                numericUpDownLowerTEffBound.BackColor = SystemColors.Window;
                numericUpDownUpperTEffBound.BackColor = SystemColors.Window;
                numericUpDownImprintingBonusExtractor.BackColor = SystemColors.Window;
                cbExactlyImprinting.BackColor = Color.Transparent;
                panelSums.BackColor = Color.Transparent;
                panelWildTamedBred.BackColor = Color.Transparent;
                labelTE.BackColor = Color.Transparent;
                llOnlineHelpExtractionIssues.Visible = false;
                labelErrorHelp.Visible = false;
                lbImprintingFailInfo.Visible = false; // TODO move imprinting-fail to upper note-info
                BtCopyIssueDumpToClipboard.Visible = false;
                PbCreatureColorsExtractor.Visible = true;
            }
            else
            {
                // highlight controls which most likely need to be checked to solve the issue
                if (issues.HasFlag(IssueNotes.Issue.WildTamedBred))
                    panelWildTamedBred.BackColor = Color.LightSalmon;
                if (issues.HasFlag(IssueNotes.Issue.TamingEffectivenessRange))
                {
                    if (numericUpDownLowerTEffBound.Value > 0)
                        numericUpDownLowerTEffBound.BackColor = Color.LightSalmon;
                    if (numericUpDownUpperTEffBound.Value < 100)
                        numericUpDownUpperTEffBound.BackColor = Color.LightSalmon;
                    if (numericUpDownLowerTEffBound.Value == 0 && numericUpDownUpperTEffBound.Value == 100)
                        issues -= IssueNotes.Issue.TamingEffectivenessRange;
                }
                if (issues.HasFlag(IssueNotes.Issue.CreatureLevel))
                {
                    numericUpDownLevel.BackColor = Color.LightSalmon;
                    numericUpDownImprintingBonusExtractor.BackColor = Color.LightSalmon;
                    _statIOs[Stats.Torpidity].Status = StatIOStatus.Error;
                }
                if (issues.HasFlag(IssueNotes.Issue.ImprintingLocked))
                    cbExactlyImprinting.BackColor = Color.LightSalmon;
                if (issues.HasFlag(IssueNotes.Issue.ImprintingNotPossible))
                    numericUpDownImprintingBonusExtractor.BackColor = Color.LightSalmon;

                // don't show some issue notes if the input is not wrong
                if (issues.HasFlag(IssueNotes.Issue.LockedDom))
                {
                    bool oneStatIsDomLocked = false;
                    for (int s = 0; s < Stats.StatsCount; s++)
                    {
                        if (_statIOs[s].DomLevelLockedZero)
                        {
                            oneStatIsDomLocked = true;
                            break;
                        }
                    }
                    if (!oneStatIsDomLocked)
                    {
                        // no stat is domLocked, remove this note (which is ensured to be there)
                        issues -= IssueNotes.Issue.LockedDom;
                    }
                }

                if (!issues.HasFlag(IssueNotes.Issue.StatMultipliers))
                    issues |= IssueNotes.Issue.StatMultipliers; // add this always?

                if (rbTamedExtractor.Checked && _creatureCollection.considerWildLevelSteps)
                    issues |= IssueNotes.Issue.WildLevelSteps;

                if (_extractor.ResultWasSortedOutBecauseOfImpossibleTe)
                    issues |= IssueNotes.Issue.ImpossibleTe;

                labelErrorHelp.Text = $"{Loc.S("extractionFailedHeader")}:\n\n{IssueNotes.getHelpTexts(issues)}";
                labelErrorHelp.Visible = true;
                llOnlineHelpExtractionIssues.Visible = true;
                groupBoxPossibilities.Visible = false;
                groupBoxRadarChartExtractor.Visible = false;
                creatureAnalysis1.Visible = false;
                lbInfoYellowStats.Visible = false;
                BtCopyIssueDumpToClipboard.Visible = true;
                string redInfoText = null;
                if (rbBredExtractor.Checked && numericUpDownImprintingBonusExtractor.Value > 0)
                {
                    redInfoText = Loc.S("lbImprintingFailInfo");
                }
                if (!rbWildExtractor.Checked
                    && new[]{
                            "Desert Titan",
                            "Desert Titan Flock",
                            "Ice Titan",
                            "Gacha",
                            "Aberrant Electrophorus",
                            "Electrophorus",
                            "Aberrant Pulmonoscorpius",
                            "Pulmonoscorpius",
                            "Aberrant Titanoboa",
                            "Titanoboa",
                            "Pegomastax",
                            "Procoptodon",
                            "Troodon"
                            }.Contains(speciesSelector1.SelectedSpecies.name))
                {
                    // creatures that display wrong stat-values after taming
                    redInfoText = (string.IsNullOrEmpty(redInfoText) ? string.Empty : redInfoText + "\n")
                            + $"The {speciesSelector1.SelectedSpecies.name} is known for displaying wrong stat-values after taming. " +
                            "This can prevent a successful extraction. Currently there's no known fix for that issue.";
                }

                if (!string.IsNullOrEmpty(redInfoText))
                {
                    lbImprintingFailInfo.Text = redInfoText;
                    lbImprintingFailInfo.Visible = true;
                }

                toolStripButtonSaveCreatureValuesTemp.Visible = true;
                PbCreatureColorsExtractor.Visible = false;
                parentInheritanceExtractor.Visible = false;

                // check for updates
                if (DateTime.Now.AddHours(-5) > Properties.Settings.Default.lastUpdateCheck)
                    CheckForUpdates(true);
            }
        }

        /// <summary>
        /// If a stat has multiple possibilities for its level distribution, the taming effectiveness may be affected by that.
        /// Set the TE of the selected levels.
        /// </summary>
        private void SetUniqueTE()
        {
            double te = Math.Round(_extractor.UniqueTamingEffectiveness(), 5);
            if (te >= 0)
            {
                labelTE.Text = $"Extracted: {Math.Round(100 * te, 2)} %";
                if (rbTamedExtractor.Checked && _extractor.PostTamed)
                {
                    var postTameLevelWithoutMutagen = _statIOs[Stats.Torpidity].LevelWild + 1
                                                      - (creatureInfoInputExtractor.CreatureFlags.HasFlag(CreatureFlags.MutagenApplied) ? Ark.MutagenTotalLevelUpsNonBred : 0);
                    labelTE.Text += $" (wildlevel: {Creature.CalculatePreTameWildLevel(postTameLevelWithoutMutagen, te)})";
                }
                labelTE.BackColor = Color.Transparent;
            }
            else
            {
                if (te == -2)
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

        /// <summary>
        /// When clicking on a stat show the possibilities in the listbox.
        /// </summary>
        /// <param name="statIndex"></param>
        private void SetActiveStat(int statIndex)
        {
            if (statIndex == _activeStatIndex) return;
            _activeStatIndex = -1;
            listViewPossibilities.BeginUpdate();
            listViewPossibilities.Items.Clear();
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s == statIndex && _statIOs[s].Status == StatIOStatus.NonUnique)
                {
                    _statIOs[s].Selected = true;
                    SetPossibilitiesListview(s);
                    _activeStatIndex = statIndex;
                }
                else
                {
                    _statIOs[s].Selected = false;
                }
            }
            listViewPossibilities.EndUpdate();
        }

        /// <summary>
        /// Fill listbox with possible results of stat
        /// </summary>
        /// <param name="s"></param>
        private void SetPossibilitiesListview(int s)
        {
            if (s < _extractor.Results.Length)
            {
                bool resultsValid = _extractor.FilterResultsByFixed(s) == -1;
                for (int r = 0; r < _extractor.Results[s].Count; r++)
                {
                    List<string> subItems = new List<string>();
                    double te = Math.Round(_extractor.Results[s][r].TE.Mean, 5);
                    subItems.Add(_extractor.Results[s][r].levelWild.ToString());
                    subItems.Add(_extractor.Results[s][r].levelDom.ToString());
                    subItems.Add(te >= 0 ? (te * 100).ToString() : string.Empty);

                    subItems.Add(te > 0 ? Creature.CalculatePreTameWildLevel(_extractor.LevelWildSum + 1, te).ToString() : string.Empty);

                    ListViewItem lvi = new ListViewItem(subItems.ToArray());
                    if (!resultsValid || _extractor.Results[s][r].currentlyNotValid)
                        lvi.BackColor = Color.LightSalmon;
                    if (_extractor.FixedResults[s] && _extractor.ChosenResults[s] == r)
                    {
                        lvi.BackColor = Color.LightSkyBlue;
                    }

                    lvi.Tag = r;

                    listViewPossibilities.Items.Add(lvi);
                }
            }
        }

        private void listViewPossibilities_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewPossibilities.SelectedIndices.Count > 0)
            {
                int index = (int)listViewPossibilities.SelectedItems[0].Tag;
                if (index >= 0 && _activeStatIndex >= 0)
                {
                    SetLevelCombination(_activeStatIndex, index, true);
                    _extractor.FixedResults[_activeStatIndex] = true;
                }
            }
            else if (_activeStatIndex >= 0)
                _extractor.FixedResults[_activeStatIndex] = false;
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnSorter.DoSort((ListView)sender, e.Column);
        }

        /// <summary>
        /// Set an option for a stat that has multiple possibilities.
        /// </summary>
        /// <param name="s">Stat index</param>
        /// <param name="i">Option index</param>
        /// <param name="validateCombination"></param>
        private void SetLevelCombination(int s, int i, bool validateCombination = false)
        {
            _statIOs[s].LevelWild = _extractor.Results[s][i].levelWild;
            _statIOs[s].LevelDom = _extractor.Results[s][i].levelDom;
            _statIOs[s].BreedingValue = StatValueCalculation.CalculateValue(speciesSelector1.SelectedSpecies, s, _extractor.Results[s][i].levelWild, 0, 0, true, 1, 0);
            _extractor.ChosenResults[s] = i;
            if (validateCombination)
            {
                SetUniqueTE();
                SetWildUnknownLevelsAccordingToOthers();
                ShowSumOfChosenLevels();
            }
        }

        /// <summary>
        /// Some wild stat levels have no effect on the stat value, often that's speed or sometimes oxygen.
        /// The wild levels of these ineffective stats can be calculated indirectly if there is only one of them.
        /// </summary>
        private void SetWildUnknownLevelsAccordingToOthers()
        {
            // wild speed level is wildTotalLevels - determinedWildLevels. sometimes the oxygen level cannot be determined as well
            var unknownLevelIndices = new List<int>();
            int notDeterminedLevels = _statIOs[Stats.Torpidity].LevelWild;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s == Stats.Torpidity || !speciesSelector1.SelectedSpecies.UsesStat(s))
                {
                    continue;
                }

                if (_statIOs[s].LevelWild < 0)
                {
                    unknownLevelIndices.Add(s);
                    continue;
                }
                notDeterminedLevels -= _statIOs[s].LevelWild;
            }

            switch (unknownLevelIndices.Count)
            {
                case 0:
                    // no unknown levels, nothing to do
                    return;
                case 1:
                    // if all other stats are unique, set level
                    var statIndex = unknownLevelIndices[0];
                    _statIOs[statIndex].LevelWild = Math.Max(0, notDeterminedLevels);
                    _statIOs[statIndex].BreedingValue = StatValueCalculation.CalculateValue(speciesSelector1.SelectedSpecies, statIndex, _statIOs[statIndex].LevelWild, 0, 0, true, 1, 0);
                    return;
                default:
                    // if not all other levels are unique, set the indifferent stats to unknown
                    foreach (var s in unknownLevelIndices)
                    {
                        _statIOs[s].LevelWild = -1;
                    }
                    return;
            }
        }

        private void CopyExtractionToClipboard()
        {
            bool header = true;
            bool table = MessageBox.Show("Results can be copied as own table or as a long table-row. Should it be copied as own table?",
                    "Copy as own table?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            if (_extractor.ValidResults && speciesSelector1.SelectedSpecies != null)
            {
                List<string> tsv = new List<string>();
                string rowLevel = speciesSelector1.SelectedSpecies.name + "\t\t";
                string rowValues = string.Empty;
                // if taming effectiveness is unique, display it, too
                string effString = string.Empty;
                double eff = _extractor.UniqueTamingEffectiveness();
                if (eff >= 0)
                {
                    effString = "\tTamingEff:\t" + (100 * eff) + "%";
                }
                // headerrow
                if (table || header)
                {
                    if (table)
                    {
                        tsv.Add(speciesSelector1.SelectedSpecies.name + "\tLevel " + numericUpDownLevel.Value + effString);
                        tsv.Add("Stat\tWildLevel\tDomLevel\tBreedingValue");
                    }
                    else
                    {
                        tsv.Add("Species\tName\tSex\tHP-Level\tSt-Level\tOx-Level\tFo-Level\tWe-Level\tDm-Level\tSp-Level\tTo-Level\tHP-Value\tSt-Value\tOx-Value\tFo-Value\tWe-Value\tDm-Value\tSp-Value\tTo-Value");
                    }
                }
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (_extractor.ChosenResults[s] < _extractor.Results[s].Count)
                    {
                        string breedingV = string.Empty;
                        if (_activeStats[s])
                        {
                            breedingV = _statIOs[s].BreedingValue.ToString();
                        }
                        if (table)
                        {
                            tsv.Add(Utils.StatName(s) + "\t" + (_statIOs[s].LevelWild >= 0 ? _statIOs[s].LevelWild.ToString() : string.Empty) + "\t" + (_statIOs[s].LevelWild >= 0 ? _statIOs[s].LevelWild.ToString() : string.Empty) + "\t" + breedingV);
                        }
                        else
                        {
                            rowLevel += "\t" + (_activeStats[s] ? _statIOs[s].LevelWild.ToString() : string.Empty);
                            rowValues += "\t" + breedingV;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                if (!table)
                {
                    tsv.Add(rowLevel + rowValues);
                }
                Clipboard.SetText(string.Join("\n", tsv));
            }
        }

        /// <summary>
        /// Export the given creature export file in the extractor.
        /// Returns true if the creature already exists in the library.
        /// Returns null if file couldn't be loaded.
        /// </summary>
        private bool? ExtractExportedFileInExtractor(string exportFilePath, out bool nameCopiedToClipboard, out Creature alreadyExistingCreature)
        {
            CreatureValues cv = null;
            nameCopiedToClipboard = false;
            alreadyExistingCreature = null;

            // if the file is blocked, try it again
            const int waitingTimeBase = 200;
            const int tryCount = 3;
            for (int i = 0; i < tryCount; i++)
            {
                try
                {
                    cv = importExported.ImportExported.ImportExportedCreature(exportFilePath);
                    break;
                }
                catch (IOException ex)
                {
                    if (i == tryCount - 1)
                    {
                        MessageBoxes.ExceptionMessageBox(ex, $"Exported creature-file couldn't be read.\n{exportFilePath}");
                        return null;
                    }
                    Thread.Sleep(waitingTimeBase * (1 << i));
                }
            }

            if (cv == null)
            {
                MessageBoxes.ShowMessageBox($"Exported creature-file not recognized.\n{exportFilePath}");
                return null;
            }
            // check if last exported file is a species that should be ignored, e.g. a raft
            if (Values.V.IgnoreSpeciesBlueprint(cv.speciesBlueprint))
            {
                MessageBoxes.ShowMessageBox(
                    $"Species of last exported creature is ignored{(cv.speciesBlueprint.Contains("Raft") ? " because it's a raft" : null)}:\n{cv.speciesBlueprint}\n\nMaybe the export folder is not configured correctly in this application.\nThe file is located in\n{exportFilePath}",
                    "Species is ignored");
                return false;
            }

            // check if species is supported.
            if (cv.Species == null)
            {
                CheckForMissingModFiles(_creatureCollection, new List<string> { cv.speciesBlueprint }, exportFilePath, cv.name);

                int oldModHash = _creatureCollection.modListHash;
                // if mods were added, try to import the creature values again
                if (_creatureCollection.ModValueReloadNeeded
                    && LoadModValuesOfCollection(_creatureCollection, true, true)
                    && oldModHash != _creatureCollection.modListHash)
                {
                    return ExtractExportedFileInExtractor(exportFilePath, out nameCopiedToClipboard, out alreadyExistingCreature);
                }

                return false;
            }

            tabControlMain.SelectedTab = tabPageExtractor;

            bool creatureAlreadyExists = ExtractValuesInExtractor(cv, exportFilePath, true, true, out alreadyExistingCreature);
            nameCopiedToClipboard = GenerateCreatureNameAndCopyNameToClipboardIfSet(alreadyExistingCreature);

            return creatureAlreadyExists;
        }

        /// <summary>
        /// Copies the creature name to the clipboard if the conditions according to the user settings are fulfilled.
        /// </summary>
        private bool GenerateCreatureNameAndCopyNameToClipboardIfSet(Creature alreadyExistingCreature)
        {
            if (Properties.Settings.Default.applyNamePatternOnAutoImportAlways
                || (Properties.Settings.Default.applyNamePatternOnImportIfEmptyName
                    && string.IsNullOrEmpty(creatureInfoInputExtractor.CreatureName))
                || (alreadyExistingCreature == null
                    && Properties.Settings.Default.applyNamePatternOnAutoImportForNewCreatures)
            )
            {
                CreatureInfoInput_CreatureDataRequested(creatureInfoInputExtractor, false, false, false, 0, alreadyExistingCreature);
                return CopyCreatureNameToClipboardOnImportIfSetting(creatureInfoInputExtractor.CreatureName);
            }

            return false;
        }

        /// <summary>
        /// Export the creature export file of the given exportedCreature control.
        /// </summary>
        /// <param name="ecc"></param>
        /// <param name="updateParentVisuals"></param>
        private void ExtractExportedFileInExtractor(importExported.ExportedCreatureControl ecc, bool updateParentVisuals = false)
        {
            if (ecc == null)
                return;

            ExtractValuesInExtractor(ecc.creatureValues, ecc.exportedFile, false, true, out var alreadyExistingCreature);
            GenerateCreatureNameAndCopyNameToClipboardIfSet(alreadyExistingCreature);

            // gets deleted in extractLevels()
            _exportedCreatureControl = ecc;

            if (!string.IsNullOrEmpty(_exportedCreatureList?.ownerSuffix))
                creatureInfoInputExtractor.CreatureOwner += _exportedCreatureList.ownerSuffix;
        }

        /// <summary>
        /// Sets the values of a creature to the extractor and extracts its levels.
        /// It returns true if the creature is already present in the library.
        /// </summary>
        /// <param name="cv"></param>
        /// <param name="filePath">If given, the file path will be displayed as info.</param>
        /// <param name="autoExtraction"></param>
        /// <param name="highPrecisionValues">If values from an export file with increased precision are given, extraction can be improved by using that.</param>
        /// <returns></returns>
        private bool ExtractValuesInExtractor(CreatureValues cv, string filePath, bool autoExtraction, bool highPrecisionValues, out Creature alreadyExistingCreature)
        {
            bool creatureExists = IsCreatureAlreadyInLibrary(cv.guid, cv.ARKID, out alreadyExistingCreature);

            if (creatureExists)
            {
                // don't clear existing info of the creature
                //if (string.IsNullOrEmpty(cv.server) && !string.IsNullOrEmpty(existingCreature.server))
                //    cv.server = existingCreature.server;

                SetExistingValueIfNewValueIsEmpty(ref cv.server, ref alreadyExistingCreature.server);
                SetExistingValueIfNewValueIsEmpty(ref cv.tribe, ref alreadyExistingCreature.tribe);
                SetExistingValueIfNewValueIsEmpty(ref cv.note, ref alreadyExistingCreature.note);

                void SetExistingValueIfNewValueIsEmpty(ref string newValue, ref string oldValue)
                {
                    if (string.IsNullOrEmpty(newValue) && !string.IsNullOrEmpty(oldValue))
                        newValue = oldValue;
                }

                // ARK doesn't export parent and mutation info always
                // if export file doesn't contain parent info, use the existing ones
                if (cv.Mother == null && cv.motherArkId == 0 && alreadyExistingCreature.Mother != null)
                    cv.Mother = alreadyExistingCreature.Mother;
                if (cv.Father == null && cv.fatherArkId == 0 && alreadyExistingCreature.Father != null)
                    cv.Father = alreadyExistingCreature.Father;

                // if export file doesn't contain mutation info and existing creature does, use that
                if (cv.mutationCounterMother == 0 && alreadyExistingCreature.mutationsMaternal != 0)
                    cv.mutationCounterMother = alreadyExistingCreature.mutationsMaternal;
                if (cv.mutationCounterFather == 0 && alreadyExistingCreature.mutationsPaternal != 0)
                    cv.mutationCounterFather = alreadyExistingCreature.mutationsPaternal;

                // if existing creature has no altColorIds, don't add them again
                if (alreadyExistingCreature.ColorIdsAlsoPossible == null)
                    cv.ColorIdsAlsoPossible = null;
                else if (cv.ColorIdsAlsoPossible != null)
                {
                    var l = Math.Min(cv.ColorIdsAlsoPossible.Length, alreadyExistingCreature.ColorIdsAlsoPossible.Length);
                    for (int i = 0; i < l; i++)
                    {
                        cv.ColorIdsAlsoPossible[i] = alreadyExistingCreature.ColorIdsAlsoPossible[i];
                    }
                }
            }

            SetCreatureValuesToExtractor(cv, false);

            // exported stat-files have values for all stats, so activate all stats the species uses
            SetStatsActiveAccordingToUsage(cv.Species);

            ExtractLevels(autoExtraction, highPrecisionValues, existingCreature: alreadyExistingCreature, possiblyMutagenApplied: cv.flags.HasFlag(CreatureFlags.MutagenApplied));
            SetCreatureValuesToInfoInput(cv, creatureInfoInputExtractor);
            UpdateParentListInput(creatureInfoInputExtractor); // this function is only used for single-creature extractions, e.g. LastExport
            creatureInfoInputExtractor.AlreadyExistingCreature = alreadyExistingCreature;
            if (!string.IsNullOrEmpty(filePath))
                SetMessageLabelText(Loc.S("creatureOfFile") + Environment.NewLine + filePath, path: filePath);
            return creatureExists;
        }

        /// <summary>
        /// Enable stats in extractor according to which stats the species uses.
        /// </summary>
        /// <param name="species"></param>
        private void SetStatsActiveAccordingToUsage(Species species)
        {
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _activeStats[s] = species.UsesStat(s);
                _statIOs[s].IsActive = _activeStats[s];
            }
        }

        /// <summary>
        /// Set the stat-values to the extractor.
        /// </summary>
        /// <param name="cv"></param>
        /// <param name="setInfoInput"></param>
        private void SetCreatureValuesToExtractor(CreatureValues cv, bool setInfoInput = true)
        {
            // at this point, if the creatureValues has parent-ArkIds, make sure these parent-creatures exist
            if (cv.Mother == null)
            {
                // placeholder creatures might have an Ark id of 0, so use the generated guid to find them reliably
                var useGuid = cv.motherGuid != Guid.Empty ? cv.motherGuid : Utils.ConvertArkIdToGuid(cv.motherArkId);
                if (_creatureCollection.CreatureById(useGuid, cv.motherArkId, out Creature mother))
                {
                    cv.Mother = mother;
                }
                else if (cv.motherArkId != 0)
                {
                    cv.Mother = new Creature(cv.motherArkId);
                    _creatureCollection.creatures.Add(cv.Mother);
                }
            }
            if (cv.Father == null)
            {
                var useGuid = cv.fatherGuid != Guid.Empty ? cv.fatherGuid : Utils.ConvertArkIdToGuid(cv.fatherArkId);
                if (_creatureCollection.CreatureById(useGuid, cv.fatherArkId, out Creature father))
                {
                    cv.Father = father;
                }
                else if (cv.fatherArkId != 0)
                {
                    cv.Father = new Creature(cv.fatherArkId);
                    _creatureCollection.creatures.Add(cv.Father);
                }
            }

            ClearAll();
            speciesSelector1.SetSpecies(Values.V.SpeciesByBlueprint(cv.speciesBlueprint));
            for (int s = 0; s < Stats.StatsCount; s++)
                _statIOs[s].Input = cv.statValues[s];

            if (setInfoInput)
                SetCreatureValuesToInfoInput(cv, creatureInfoInputExtractor);

            numericUpDownLevel.ValueSave = cv.level;
            numericUpDownLowerTEffBound.ValueSave = (decimal)cv.tamingEffMin * 100;
            numericUpDownUpperTEffBound.ValueSave = (decimal)cv.tamingEffMax * 100;

            if (cv.isBred)
                rbBredExtractor.Checked = true;
            else if (cv.isTamed)
                rbTamedExtractor.Checked = true;
            else
                rbWildExtractor.Checked = true;
            numericUpDownImprintingBonusExtractor.ValueSave = (decimal)cv.imprintingBonus * 100;
        }

        /// <summary>
        /// Creates a creature from the infos in inputs, extractor or tester.
        /// </summary>
        /// <param name="fromExtractor"></param>
        /// <param name="species"></param>
        /// <param name="levelStep"></param>
        /// <param name="motherArkId">Use this Ark Id instead of the ones of the input if not 0</param>
        /// <param name="fatherArkId">Use this Ark Id instead of the ones of the input if not 0</param>
        /// <returns></returns>
        private Creature GetCreatureFromInput(bool fromExtractor, Species species, int? levelStep, long motherArkId = 0, long fatherArkId = 0)
        {
            CreatureInfoInput input;
            bool bred;
            double te, imprinting;
            if (fromExtractor)
            {
                input = creatureInfoInputExtractor;
                bred = rbBredExtractor.Checked;
                te = rbWildExtractor.Checked ? -3 : _extractor.UniqueTamingEffectiveness();
                imprinting = _extractor.ImprintingBonus;
            }
            else
            {
                input = creatureInfoInputTester;
                bred = rbBredTester.Checked;
                te = TamingEffectivenessTester;
                imprinting = (double)numericUpDownImprintingBonusTester.Value / 100;
            }

            Creature creature = new Creature(species, input.CreatureName, input.CreatureOwner, input.CreatureTribe, input.CreatureSex,
                GetCurrentWildLevels(fromExtractor), GetCurrentDomLevels(fromExtractor), GetCurrentMutLevels(fromExtractor), te, bred, imprinting, levelStep: levelStep)
            {
                // set parents
                Mother = input.Mother,
                Father = input.Father,

                // cooldown-, growing-time
                cooldownUntil = input.CooldownUntil,
                growingUntil = input.GrowingUntil,

                flags = input.CreatureFlags,
                note = input.CreatureNote,
                server = input.CreatureServer,

                domesticatedAt = input.DomesticatedAt.HasValue && input.DomesticatedAt.Value.Year > 2014 ? input.DomesticatedAt.Value : default(DateTime?),
                addedToLibrary = DateTime.Now,
                mutationsMaternal = input.MutationCounterMother,
                mutationsPaternal = input.MutationCounterFather,
                Status = input.CreatureStatus,
                colors = input.RegionColors,
                ColorIdsAlsoPossible = input.ColorIdsAlsoPossible,
                guid = fromExtractor && input.CreatureGuid != Guid.Empty ? input.CreatureGuid : Guid.NewGuid(),
                ArkId = input.ArkId
            };

            creature.ArkIdImported = Utils.IsArkIdImported(creature.ArkId, creature.guid);
            creature.InitializeArkInGame();

            // parent guids
            if (motherArkId != 0)
                creature.motherGuid = Utils.ConvertArkIdToGuid(motherArkId);
            else if (input.MotherArkId != 0)
                creature.motherGuid = Utils.ConvertArkIdToGuid(input.MotherArkId);
            if (fatherArkId != 0)
                creature.fatherGuid = Utils.ConvertArkIdToGuid(fatherArkId);
            else if (input.FatherArkId != 0)
                creature.fatherGuid = Utils.ConvertArkIdToGuid(input.FatherArkId);

            return creature;
        }

        /// <summary>
        /// Gives feedback to the user if the current creature in the extractor is already in the library.
        /// This uses the ARK-ID and only works if exported creatures are imported
        /// </summary>
        private bool IsCreatureAlreadyInLibrary(Guid creatureGuid, long arkId, out Creature existingCreature)
        {
            existingCreature = null;
            bool creatureAlreadyExistsInLibrary = false;
            if (creatureGuid != Guid.Empty && Utils.IsArkIdImported(arkId, creatureGuid))
            {
                existingCreature = _creatureCollection.creatures.FirstOrDefault(c => c.guid == creatureGuid
                                                           && !c.flags.HasFlag(CreatureFlags.Placeholder)
                                                                  );
                if (existingCreature != null)
                    creatureAlreadyExistsInLibrary = true;
            }
            return creatureAlreadyExistsInLibrary;
        }

        private void creatureInfoInputExtractor_Add2Library_Clicked(CreatureInfoInput sender)
        {
            AddCreatureToCollection();
        }

        private void CreatureInfoInputColorsChanged(CreatureInfoInput input)
        {
            if (_dontUpdateExtractorVisualData)
            {
                input.ColorAlreadyExistingInformation = null;
                return;
            }
            var colorAlreadyExisting = _creatureCollection.ColorAlreadyAvailable(speciesSelector1.SelectedSpecies, input.RegionColors, out string infoText);
            var newColorStatus = input.SetRegionColorsExisting(colorAlreadyExisting);
            input.ColorAlreadyExistingInformation = colorAlreadyExisting;

            if (input == creatureInfoInputExtractor)
                creatureAnalysis1.SetColorAnalysis(newColorStatus.newInSpecies ? LevelStatus.NewTopLevel : newColorStatus.newInRegion ? LevelStatus.TopLevel : LevelStatus.Neutral, infoText);
        }

        private void copyLibrarydumpToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDebugFile();
        }

        private void BtCopyIssueDumpToClipboard_Click(object sender, EventArgs e)
        {
            SaveDebugFile();
        }

        private void LbBlueprintPath_Click(object sender, EventArgs e)
        {
            // copy blueprint path to clipboard
            if (speciesSelector1.SelectedSpecies?.blueprintPath is string bp
                && !string.IsNullOrEmpty(bp))
                Clipboard.SetText(bp);
        }

        #region OCR label sets

        private void InitializeOcrLabelSets()
        {
            TsCbbLabelSets.Items.Clear();
            var labelSetNames = ArkOcr.Ocr.ocrConfig?.LabelRectangles?.Keys.ToArray();
            var displayControl = (labelSetNames?.Length ?? 0) > 1;

            TsCbbLabelSets.Visible = displayControl;
            TsLbLabelSet.Visible = displayControl;
            TsSpOcrLabel.Visible = displayControl;

            if (!displayControl)
                return;

            TsCbbLabelSets.Items.AddRange(labelSetNames);
            TsCbbLabelSets.SelectedItem = ArkOcr.Ocr.ocrConfig.SelectedLabelSetName;
        }

        private void SetCurrentOcrLabelSet()
        {
            TsCbbLabelSets.SelectedItem = ArkOcr.Ocr.ocrConfig.SelectedLabelSetName;
        }

        private void TsCbbLabelSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ArkOcr.Ocr.ocrConfig == null) return;
            ArkOcr.Ocr.ocrConfig.SetLabelSet(((ToolStripComboBox)sender).SelectedItem.ToString());
            ocrControl1.SetOcrLabelSetToCurrent();
        }

        #endregion
    }
}
