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

namespace ARKBreedingStats
{
    public partial class Form1
    {
        /// <summary>
        /// Set to false if many creatures are processed automatically, e.g. a bulk import.
        /// It will prevent visual feedback updates.
        /// </summary>
        private bool _updateExtractorVisualData;

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
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s == (int)StatNames.Torpidity)
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
                _statIOs[s].TopLevel = StatIOStatus.Neutral;
            }
            if (valid)
            {
                sumW -= allUnique || _statIOs[(int)StatNames.SpeedMultiplier].LevelWild < 0 ? 0 : _statIOs[(int)StatNames.SpeedMultiplier].LevelWild;
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

            bool torporLevelValid = numericUpDownLevel.Value > _statIOs[(int)StatNames.Torpidity].LevelWild;
            if (!torporLevelValid)
            {
                numericUpDownLevel.BackColor = Color.LightSalmon;
                _statIOs[(int)StatNames.Torpidity].Status = StatIOStatus.Error;
            }

            bool allValid = valid && inbound && torporLevelValid && _extractor.ValidResults;
            if (allValid)
            {
                radarChartExtractor.SetLevels(_statIOs.Select(s => s.LevelWild).ToArray());
                toolStripButtonSaveCreatureValuesTemp.Visible = false;
                cbExactlyImprinting.BackColor = Color.Transparent;
                if (_topLevels.TryGetValue(speciesSelector1.SelectedSpecies, out int[] topSpeciesLevels))
                {
                    for (int s = 0; s < Values.STATS_COUNT; s++)
                    {
                        if (s == (int)StatNames.Torpidity)
                            continue;
                        if (_statIOs[s].LevelWild > 0)
                        {
                            if (_statIOs[s].LevelWild == topSpeciesLevels[s])
                                _statIOs[s].TopLevel = StatIOStatus.TopLevel;
                            else if (topSpeciesLevels[s] != -1 && _statIOs[s].LevelWild > topSpeciesLevels[s])
                                _statIOs[s].TopLevel = StatIOStatus.NewTopLevel;
                        }
                    }
                }
            }
            creatureInfoInputExtractor.ButtonEnabled = allValid;
            groupBoxRadarChartExtractor.Visible = allValid;
            CreatureInfoInput_CreatureDataRequested(creatureInfoInputExtractor, false, true, false, 0);
        }

        /// <summary>
        /// Clears all the parameters of the extractor.
        /// </summary>
        /// <param name="clearExtraCreatureData">Also delete infos like the guid, parents and mutations</param>
        private void ClearAll(bool clearExtraCreatureData = true)
        {
            _extractor.Clear();
            listViewPossibilities.Items.Clear();
            for (int s = 0; s < Values.STATS_COUNT; s++)
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

            creatureInfoInputExtractor.UpdateExistingCreature = false;
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
        /// <returns></returns>
        private bool ExtractLevels(bool autoExtraction = false, bool statInputsHighPrecision = false, bool showLevelsInOverlay = false, Creature existingCreature = null)
        {
            int activeStatKeeper = _activeStatIndex;
            ClearAll(_clearExtractionCreatureData);

            _extractor.ExtractLevels(speciesSelector1.SelectedSpecies, (int)numericUpDownLevel.Value, _statIOs,
                    (double)numericUpDownLowerTEffBound.Value / 100, (double)numericUpDownUpperTEffBound.Value / 100,
                    rbTamedExtractor.Checked, rbBredExtractor.Checked,
                    (double)numericUpDownImprintingBonusExtractor.Value / 100, !cbExactlyImprinting.Checked,
                    _creatureCollection.allowMoreThanHundredImprinting, _creatureCollection.serverMultipliers.BabyImprintingStatScaleMultiplier,
                    Values.V.currentServerMultipliers.BabyCuddleIntervalMultiplier,
                    _creatureCollection.considerWildLevelSteps, _creatureCollection.wildLevelStep, statInputsHighPrecision, out bool imprintingBonusChanged);

            numericUpDownImprintingBonusExtractor.ValueSave = (decimal)_extractor.ImprintingBonus * 100;
            numericUpDownImprintingBonusExtractor_ValueChanged(null, null);

            var possibleExtractionIssues = IssueNotes.Issue.None;
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
            _extractor.RemoveImpossibleTEsAccordingToMaxWildLevel((int)Math.Ceiling(_creatureCollection.maxWildLevel * (speciesSelector1.SelectedSpecies.name.StartsWith("Tek ") ? 1.2 : 1)));

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
                _statIOs[(int)StatNames.Torpidity].Status = StatIOStatus.Error;
            }

            // get mean-level (most probable for the wild levels)
            // TODO handle species without wild levels in speed better (some flyers)
            double meanWildLevel = Math.Round((double)_extractor.LevelWildSum / 7, 1);
            bool nonUniqueStats = false;

            for (int s = 0; s < Values.STATS_COUNT; s++)
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
                                && _extractor.Results[s][b].TE.Includes(existingCreature.tamingEff))
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
                    if (s == (int)StatNames.Health
                        || s == (int)StatNames.MeleeDamageMultiplier)
                    {
                        possibleExtractionIssues |= IssueNotes.Issue.Singleplayer;
                    }
                }
            }
            if (!_extractor.ValidResults)
            {
                ExtractionFailed(possibleExtractionIssues | IssueNotes.Issue.Typo | IssueNotes.Issue.WildTamedBred | IssueNotes.Issue.LockedDom |
                                 IssueNotes.Issue.OutdatedIngameValues | IssueNotes.Issue.ImprintingNotUpdated |
                                 (_statIOs[(int)StatNames.Torpidity].LevelWild >= (int)numericUpDownLevel.Value ? IssueNotes.Issue.CreatureLevel : IssueNotes.Issue.None));
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
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s != (int)StatNames.Torpidity)
                    domLevelsChosenSum += _extractor.Results[s][_extractor.ChosenResults[s]].levelDom;
            }
            if (domLevelsChosenSum != _extractor.LevelDomSum)
            {
                // sum of domlevels is not correct. Try to find another combination
                domLevelsChosenSum -= _extractor.Results[(int)StatNames.MeleeDamageMultiplier][_extractor.ChosenResults[(int)StatNames.MeleeDamageMultiplier]].levelDom;
                bool changeChosenResult = false;
                int cR = 0;
                for (int r = 0; r < _extractor.Results[(int)StatNames.MeleeDamageMultiplier].Count; r++)
                {
                    if (domLevelsChosenSum + _extractor.Results[(int)StatNames.MeleeDamageMultiplier][r].levelDom == _extractor.LevelDomSum)
                    {
                        cR = r;
                        changeChosenResult = true;
                        break;
                    }
                }
                if (changeChosenResult)
                    SetLevelCombination((int)StatNames.MeleeDamageMultiplier, cR);
            }

            if (_extractor.PostTamed)
                SetUniqueTE();
            else
            {
                labelTE.Text = Loc.S("notYetTamed");
                labelTE.BackColor = Color.Transparent;
            }

            SetWildSpeedLevelAccordingToOthers();

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
                    ? _statIOs[(int)StatNames.Torpidity].LevelWild + 1
                    : (int)numericUpDownLevel.Value);
            }

            return true;
        }

        private void UpdateQuickTamingInfo()
        {
            bool showQuickTamingInfo = cbQuickWildCheck.Checked;
            if (showQuickTamingInfo)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    int lvlWild = (int)Math.Round((_statIOs[s].Input - speciesSelector1.SelectedSpecies.stats[s].BaseValue) / (speciesSelector1.SelectedSpecies.stats[s].BaseValue * speciesSelector1.SelectedSpecies.stats[s].IncPerWildLevel));
                    _statIOs[s].LevelWild = lvlWild < 0 ? 0 : lvlWild;
                    _statIOs[s].LevelDom = 0;
                }
                SetQuickTamingInfo(_statIOs[(int)StatNames.Torpidity].LevelWild + 1);
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
                    _statIOs[(int)StatNames.Torpidity].Status = StatIOStatus.Error;
                }
                if (issues.HasFlag(IssueNotes.Issue.ImprintingLocked))
                    cbExactlyImprinting.BackColor = Color.LightSalmon;
                if (issues.HasFlag(IssueNotes.Issue.ImprintingNotPossible))
                    numericUpDownImprintingBonusExtractor.BackColor = Color.LightSalmon;

                // don't show some issue notes if the input is not wrong
                if (issues.HasFlag(IssueNotes.Issue.LockedDom))
                {
                    bool oneStatIsDomLocked = false;
                    for (int s = 0; s < Values.STATS_COUNT; s++)
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

                labelErrorHelp.Text = $"{Loc.S("extractionFailedHeader")}:\n\n{IssueNotes.getHelpTexts(issues)}";
                labelErrorHelp.Visible = true;
                llOnlineHelpExtractionIssues.Visible = true;
                groupBoxPossibilities.Visible = false;
                groupBoxRadarChartExtractor.Visible = false;
                lbInfoYellowStats.Visible = false;
                BtCopyIssueDumpToClipboard.Visible = true;
                if (rbBredExtractor.Checked && numericUpDownImprintingBonusExtractor.Value > 0)
                {
                    lbImprintingFailInfo.Text = Loc.S("lbImprintingFailInfo");
                    lbImprintingFailInfo.Visible = true;
                }
                else if (rbTamedExtractor.Checked
                    && "Desert Titan,Desert Titan Flock,Ice Titan,Gacha,Aberrant Electrophorus,Electrophorus,Aberrant Pulmonoscorpius,Pulmonoscorpius,Aberrant Titanoboa,Titanoboa,Pegomastax,Procoptodon,Troodon"
                    .Split(',').ToList().Contains(speciesSelector1.SelectedSpecies.name))
                {
                    // creatures that display wrong stat-values after taming
                    lbImprintingFailInfo.Text = $"The {speciesSelector1.SelectedSpecies.name} is known for displaying wrong stat-values after taming. " +
                            "This can prevent a successful extraction. Currently there's no known fix for that issue.";
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
            double te = Math.Round(_extractor.UniqueTE(), 5);
            if (te >= 0)
            {
                labelTE.Text = $"Extracted: {Math.Round(100 * te, 2)} %";
                if (rbTamedExtractor.Checked && _extractor.PostTamed)
                    labelTE.Text += $" (wildlevel: {Creature.CalculatePreTameWildLevel(_statIOs[(int)StatNames.Torpidity].LevelWild + 1, te)})";
                labelTE.BackColor = Color.Transparent;
            }
            else
            {
                if (te == -1)
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
        /// When clicking on a stat show the possibilites in the listbox.
        /// </summary>
        /// <param name="statIndex"></param>
        private void SetActiveStat(int statIndex)
        {
            if (statIndex != _activeStatIndex)
            {
                _activeStatIndex = -1;
                listViewPossibilities.BeginUpdate();
                listViewPossibilities.Items.Clear();
                for (int s = 0; s < Values.STATS_COUNT; s++)
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
            _statIOs[s].BreedingValue = StatValueCalculation.CalculateValue(speciesSelector1.SelectedSpecies, s, _extractor.Results[s][i].levelWild, 0, true, 1, 0);
            _extractor.ChosenResults[s] = i;
            if (validateCombination)
            {
                SetUniqueTE();
                SetWildSpeedLevelAccordingToOthers();
                ShowSumOfChosenLevels();
            }
        }

        /// <summary>
        /// The wild speed level is calculated indirectly by using all unused stat-levels.
        /// </summary>
        private void SetWildSpeedLevelAccordingToOthers()
        {
            // wild speed level is wildTotalLevels - determinedWildLevels. sometimes the oxygenlevel cannot be determined as well
            bool unique = true;
            int notDeterminedLevels = _statIOs[(int)StatNames.Torpidity].LevelWild;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s == (int)StatNames.SpeedMultiplier || s == (int)StatNames.Torpidity)
                    continue;
                if (_statIOs[s].LevelWild >= 0)
                {
                    notDeterminedLevels -= _statIOs[s].LevelWild;
                }
                else
                {
                    unique = false;
                    break;
                }
            }
            if (unique)
            {
                // if all other stats are unique, set speedlevel
                _statIOs[(int)StatNames.SpeedMultiplier].LevelWild = Math.Max(0, notDeterminedLevels);
                _statIOs[(int)StatNames.SpeedMultiplier].BreedingValue = StatValueCalculation.CalculateValue(speciesSelector1.SelectedSpecies, (int)StatNames.SpeedMultiplier, _statIOs[(int)StatNames.SpeedMultiplier].LevelWild, 0, true, 1, 0);
            }
            else
            {
                // if not all other levels are unique, set speed and not known levels to unknown
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (s == (int)StatNames.SpeedMultiplier || !_activeStats[s])
                    {
                        _statIOs[s].LevelWild = -1;
                    }
                }
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
                double eff = _extractor.UniqueTE();
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
                for (int s = 0; s < Values.STATS_COUNT; s++)
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
        private bool? ExtractExportedFileInExtractor(string exportFilePath)
        {
            CreatureValues cv = null;

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
                        MessageBox.Show($"Exported creature-file couldn't be read.\n{exportFilePath}\n\n{ex.Message}",
                            $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return null;
                    }
                    Thread.Sleep(waitingTimeBase * (1 << i));
                }
            }

            if (cv == null)
            {
                MessageBox.Show($"Exported creature-file not recognized.\n{exportFilePath}", $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            // check if last exported file is a species that should be ignored, e.g. a raft
            if (Values.V.IgnoreSpeciesBlueprint(cv.speciesBlueprint))
            {
                MessageBox.Show("Species of last exported creature is ignored" + (cv.speciesBlueprint.Contains("Raft") ? " because it's a raft" : string.Empty) + ":\n" + cv.speciesBlueprint, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // check if species is supported.
            if (cv.Species == null)
            {
                CheckForMissingModFiles(_creatureCollection, new List<string> { cv.speciesBlueprint });

                int oldModHash = _creatureCollection.modListHash;
                // if mods were added, try to import the creature values again
                if (_creatureCollection.ModValueReloadNeeded
                    && LoadModValuesOfCollection(_creatureCollection, true, true)
                    && oldModHash != _creatureCollection.modListHash)
                {
                    return ExtractExportedFileInExtractor(exportFilePath);
                }

                return false;
            }

            tabControlMain.SelectedTab = tabPageExtractor;

            bool creatureExists = ExtractValuesInExtractor(cv, exportFilePath, true);

            if ((Properties.Settings.Default.applyNamePatternOnImportIfEmptyName
                && string.IsNullOrEmpty(creatureInfoInputExtractor.CreatureName))
                || (!creatureExists
                    && Properties.Settings.Default.applyNamePatternOnAutoImportForNewCreatures)
                )
            {
                CreatureInfoInput_CreatureDataRequested(creatureInfoInputExtractor, false, false, false, 0);
                if (Properties.Settings.Default.copyNameToClipboardOnImportWhenAutoNameApplied)
                {
                    Clipboard.SetText(string.IsNullOrEmpty(creatureInfoInputExtractor.CreatureName)
                        ? "<no name>"
                        : creatureInfoInputExtractor.CreatureName);
                }
            }
            return creatureExists;
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

            ExtractValuesInExtractor(ecc.creatureValues, ecc.exportedFile, false);

            // gets deleted in extractLevels()
            _exportedCreatureControl = ecc;

            if (!string.IsNullOrEmpty(_exportedCreatureList?.ownerSuffix))
                creatureInfoInputExtractor.CreatureOwner += _exportedCreatureList.ownerSuffix;
        }

        /// <summary>
        /// Sets the values of a creature to the extractor and extracts its levels.
        /// It returns if the creature is already present in the library.
        /// </summary>
        /// <param name="cv"></param>
        /// <param name="filePath"></param>
        /// <param name="autoExtraction"></param>
        /// <returns></returns>
        private bool ExtractValuesInExtractor(CreatureValues cv, string filePath, bool autoExtraction)
        {
            SetCreatureValuesToExtractor(cv, false);

            // exported stat-files have values for all stats, so activate all stats the species uses
            SetStatsActiveAccordingToUsage(cv.Species);

            bool creatureExists = IsCreatureAlreadyInLibrary(cv.guid, cv.ARKID, out Creature existingCreature);

            ExtractLevels(autoExtraction: autoExtraction, statInputsHighPrecision: true, existingCreature: existingCreature);
            SetCreatureValuesToInfoInput(cv, creatureInfoInputExtractor);
            UpdateParentListInput(creatureInfoInputExtractor); // this function is only used for single-creature extractions, e.g. LastExport
            creatureInfoInputExtractor.UpdateExistingCreature = creatureExists;
            SetMessageLabelText(Loc.S("creatureOfFile") + "\n" + filePath);
            return creatureExists;
        }

        /// <summary>
        /// Enable stats in extractor according to which stats the species uses.
        /// </summary>
        /// <param name="species"></param>
        private void SetStatsActiveAccordingToUsage(Species species)
        {
            for (int s = 0; s < Values.STATS_COUNT; s++)
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
                if (_creatureCollection.CreatureById(cv.motherGuid, cv.motherArkId, cv.Species, cv.sex, out Creature mother))
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
                if (_creatureCollection.CreatureById(cv.fatherGuid, cv.fatherArkId, cv.Species, cv.sex, out Creature father))
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
            for (int s = 0; s < Values.STATS_COUNT; s++)
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
        /// Gives feedback to the user if the current creature in the extractor is already in the library.
        /// This uses the ARK-ID and only works if exported creatures are imported
        /// </summary>
        private bool IsCreatureAlreadyInLibrary(Guid creatureGuid, long arkId, out Creature existingCreature)
        {
            existingCreature = null;
            bool creatureAlreadyExistsInLibrary = false;
            if (creatureGuid != Guid.Empty
                                                  && Utils.IsArkIdImported(arkId, creatureGuid))
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

        private void CreatureInfoInputExtractor_ColorsChanged(CreatureInfoInput input)
        {
            if (_updateExtractorVisualData)
                input.SetRegionColorsExisting(_creatureCollection.ColorAlreadyAvailable(speciesSelector1.SelectedSpecies, input.RegionColors));
        }
    }
}
