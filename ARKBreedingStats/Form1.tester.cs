﻿using ARKBreedingStats.Library;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.library;
using ARKBreedingStats.uiControls;

namespace ARKBreedingStats
{
    public partial class Form1
    {
        /// <summary>
        /// Updates the labels for the creature stats outside of levels and info in the creatureInfoInput, i.e. wild/tame/bred, TE, Imprinting.
        /// </summary>
        private void UpdateTesterDetails()
        {
            SetTesterInputsTamed(!rbWildTester.Checked);
            NumericUpDownTestingTE.Enabled = rbTamedTester.Checked;
            labelTesterTE.Enabled = rbTamedTester.Checked;
            numericUpDownImprintingBonusTester.Enabled = rbBredTester.Checked;
            labelImprintingTester.Enabled = rbBredTester.Checked;
            lbImprintedCount.Enabled = rbBredTester.Checked;

            UpdateAllTesterValues();
        }

        /// <summary>
        /// Call this function with a creature c to put all its stats in the levelup-tester (and go to the tester-tab) to see what it could become
        /// </summary>
        /// <param name="c">the creature to test</param>
        /// <param name="virtualCreature">set to true if the creature is not in the library</param>
        private void EditCreatureInTester(Creature c, bool virtualCreature = false)
        {
            if (c == null)
                return;

            speciesSelector1.SetSpecies(c.Species);
            TamingEffectivenessTester = c.tamingEff;
            numericUpDownImprintingBonusTester.ValueSave = (decimal)c.imprintingBonus * 100;
            if (c.isBred)
                rbBredTester.Checked = true;
            else if (c.isDomesticated)
                rbTamedTester.Checked = true;
            else
                rbWildTester.Checked = true;

            _hiddenLevelsCreatureTester = c.levelsWild[Stats.Torpidity];
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s != Stats.Torpidity && c.levelsWild[s] > 0)
                    _hiddenLevelsCreatureTester -= c.levelsWild[s] + (c.levelsMutated?[s] ?? 0);
            }

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s == Stats.Torpidity)
                    continue;
                _testingIOs[s].LevelWild = c.levelsWild[s];
                _testingIOs[s].LevelMut = c.levelsMutated?[s] ?? 0;
                _testingIOs[s].LevelDom = c.levelsDom[s];
            }
            tabControlMain.SelectedTab = tabPageStatTesting;
            SetInfoInputCreature(c, virtualCreature);
        }

        private void UpdateAllTesterValues()
        {
            _updateTorporInTester = false;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s == Stats.Torpidity)
                    continue;
                if (s == Stats.StatsCount - 2) // update torpor after last stat-update
                    _updateTorporInTester = true;
                TestingStatIOsRecalculateValue(_testingIOs[s]);
            }
            TestingStatIOsRecalculateValue(_testingIOs[Stats.Torpidity]);
        }

        private void SetTesterInputsTamed(bool tamed)
        {
            for (int s = 0; s < Stats.StatsCount; s++)
                _testingIOs[s].postTame = tamed;
            lbNotYetTamed.Visible = !tamed;
        }

        /// <summary>
        /// Updates the values in the testing-statIOs
        /// </summary>
        /// <param name="sIo"></param>
        private void TestingStatIoValueUpdate(StatIO sIo)
        {
            TestingStatIOsRecalculateValue(sIo);

            // update Torpor-level if changed value is not from torpor-StatIO
            if (_updateTorporInTester && sIo.statIndex != Stats.Torpidity)
            {
                int torporLvl = 0;
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (s != Stats.Torpidity)
                        torporLvl += (_testingIOs[s].LevelWild > 0 ? _testingIOs[s].LevelWild : 0)
                            + _testingIOs[s].LevelMut;
                }
                _testingIOs[Stats.Torpidity].LevelWild = torporLvl + _hiddenLevelsCreatureTester;
            }

            var wildLevel255 = false;
            var levelGreaterThan255 = false;

            int domLevels = 0;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                domLevels += _testingIOs[s].LevelDom;
                if (s == Stats.Torpidity) continue;
                if (_testingIOs[s].LevelWild == 255)
                    wildLevel255 = true;
                if (_testingIOs[s].LevelWild > 255
                    || _testingIOs[s].LevelDom > 255)
                    levelGreaterThan255 = true;
            }
            labelDomLevelSum.Text = $"Dom Levels: {domLevels}/{_creatureCollection.maxDomLevel}";
            labelDomLevelSum.BackColor = domLevels > _creatureCollection.maxDomLevel ? Color.LightSalmon : Color.Transparent;
            labelTesterTotalLevel.Text = $"Total Levels: {_testingIOs[Stats.Torpidity].LevelWild + domLevels + 1}/{_testingIOs[Stats.Torpidity].LevelWild + 1 + _creatureCollection.maxDomLevel}";
            creatureInfoInputTester.parentListValid = false;

            int[] levelsWild = _testingIOs.Select(s => s.LevelWild).ToArray();
            int[] levelsMutations = _testingIOs.Select(s => s.LevelMut).ToArray();
            if (!_testingIOs[Stats.Torpidity].Enabled)
                levelsWild[Stats.Torpidity] = 0;
            radarChart1.SetLevels(levelsWild, levelsMutations, speciesSelector1.SelectedSpecies);
            statPotentials1.SetLevels(levelsWild, levelsMutations, false);
            //statGraphs1.setGraph(sE, 0, testingIOs[0].LevelWild, testingIOs[0].LevelDom, !radioButtonTesterWild.Checked, (double)NumericUpDownTestingTE.Value / 100, (double)numericUpDownImprintingBonusTester.Value / 100);

            if (sIo.statIndex == Stats.Torpidity)
            {
                DisplayPreTamedLevelTester();
            }

            var levelWarning = string.Empty;
            if (wildLevel255)
                levelWarning += "A stat with a wild level of 255 cannot be leveled anymore. ";
            if (levelGreaterThan255)
                levelWarning += "A level higher than 255 will not be saved correctly in ARK and may be reset to a lower level than 256 after loading.";
            if (string.IsNullOrEmpty(levelWarning))
                LbWarningLevel255.Visible = false;
            else
            {
                LbWarningLevel255.Text = levelWarning;
                LbWarningLevel255.Visible = true;
            }
        }

        private void TestingStatIOsRecalculateValue(StatIO sIo)
        {
            sIo.BreedingValue = StatValueCalculation.CalculateValue(speciesSelector1.SelectedSpecies, sIo.statIndex, sIo.LevelWild, sIo.LevelMut, 0, true, 1, 0);
            sIo.Input = StatValueCalculation.CalculateValue(speciesSelector1.SelectedSpecies, sIo.statIndex, sIo.LevelWild, sIo.LevelMut, sIo.LevelDom,
                    rbTamedTester.Checked || rbBredTester.Checked,
                    rbBredTester.Checked ? 1 : Math.Max(0, TamingEffectivenessTester),
                    rbBredTester.Checked ? (double)numericUpDownImprintingBonusTester.Value / 100 : 0, roundToIngamePrecision: false);
        }

        private void creatureInfoInputTester_Add2Library_Clicked(CreatureInfoInput sender)
        {
            AddCreatureToCollection(false);
        }

        private void creatureInfoInputTester_Save2Library_Clicked(CreatureInfoInput sender)
        {
            if (_creatureTesterEdit == null)
                return;
            // check if wild levels are changed, if yes warn that the creature can become invalid
            // TODO adjust check if mutated levels have a different multiplier than wild levels
            bool wildChanged = Math.Abs(_creatureTesterEdit.tamingEff - TamingEffectivenessTester) > .0005;
            if (!wildChanged)
            {
                var wildLevels = GetCurrentWildLevels(false);
                var mutatedLevels = GetCurrentMutLevels(false);
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (wildLevels[s] + mutatedLevels[s] != _creatureTesterEdit.levelsWild[s] + (_creatureTesterEdit.levelsMutated?[s] ?? 0))
                    {
                        wildChanged = true;
                        break;
                    }
                }
            }
            if (wildChanged && MessageBox.Show("The wild or mutated levels or the taming-effectiveness were changed. Save values anyway?\n" +
                    "Only save if the wild or mutated levels or the taming-effectiveness were extracted wrongly!\nIf you are not sure, don't save. " +
                    "The breeding-values could become invalid.",
                    "Wild levels have been changed",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Exclamation) != DialogResult.OK)
            {
                return;
            }

            // Ids: ArkId and Guid
            //if (!IsArkIdUniqueOrOnlyPlaceHolder(creatureTesterEdit)) { return; }

            bool statusChanged = _creatureTesterEdit.Status != creatureInfoInputTester.CreatureStatus
                    || _creatureTesterEdit.owner != creatureInfoInputTester.CreatureOwner
                    || _creatureTesterEdit.mutationsMaternal != creatureInfoInputTester.MutationCounterMother
                    || _creatureTesterEdit.mutationsPaternal != creatureInfoInputTester.MutationCounterFather;
            bool parentsChanged = _creatureTesterEdit.Mother != creatureInfoInputTester.Mother || _creatureTesterEdit.Father != creatureInfoInputTester.Father;
            _creatureTesterEdit.levelsWild = GetCurrentWildLevels(false);
            _creatureTesterEdit.levelsMutated = GetCurrentMutLevels(false);
            _creatureTesterEdit.levelsDom = GetCurrentDomLevels(false);
            _creatureTesterEdit.tamingEff = TamingEffectivenessTester;
            _creatureTesterEdit.isBred = rbBredTester.Checked;
            _creatureTesterEdit.imprintingBonus = (double)numericUpDownImprintingBonusTester.Value / 100;

            creatureInfoInputTester.SetCreatureData(_creatureTesterEdit);

            if (wildChanged)
                CalculateTopStats(_creatureCollection.creatures.Where(c => c.Species == _creatureTesterEdit.Species).ToList(), _creatureTesterEdit.Species);
            UpdateDisplayedCreatureValues(_creatureTesterEdit, statusChanged, true);

            if (parentsChanged)
                _creatureTesterEdit.RecalculateAncestorGenerations();

            _creatureTesterEdit.RecalculateNewMutations();

            // if maturation was changed, update raising-timers
            var newGrownUpAt = creatureInfoInputTester.GrowingUntil;
            if (newGrownUpAt != null && _creatureTesterEdit.growingUntil != newGrownUpAt)
            {
                raisingControl1.RecreateList();
            }

            SetInfoInputCreature();
            _libraryNeedsUpdate = true;
            tabControlMain.SelectedTab = tabPageLibrary;
        }

        /// <summary>
        /// Set the values in the creatureInfoInput control to the values of the creature or clears the inputs.
        /// </summary>
        private void SetInfoInputCreature(Creature c = null, bool virtualCreature = false, bool tester = true)
        {
            bool enable = c != null; // set to a creature, or clear
            var infoInput = creatureInfoInputExtractor;
            if (tester)
            {
                infoInput = creatureInfoInputTester;
                creatureInfoInputTester.ShowSaveButton = enable && !virtualCreature;
                labelCurrentTesterCreature.Visible = enable;
                if (enable)
                    labelCurrentTesterCreature.Text = c.name;
                lbCurrentCreature.Visible = enable;
                _creatureTesterEdit = c;
            }

            if (enable)
            {
                infoInput.Mother = c.Mother;
                infoInput.Father = c.Father;
                infoInput.CreatureName = c.name;
                infoInput.CreatureSex = c.sex;
                infoInput.CreatureOwner = c.owner;
                infoInput.CreatureTribe = c.tribe;
                infoInput.CreatureServer = c.server;
                infoInput.CreatureStatus = c.Status;
                infoInput.CreatureNote = c.note;
                infoInput.CooldownUntil = c.cooldownUntil;
                infoInput.GrowingUntil = c.growingUntil;
                infoInput.DomesticatedAt = c.domesticatedAt;
                infoInput.AddedToLibraryAt = c.addedToLibrary;
                infoInput.CreatureFlags = c.flags;
                infoInput.RegionColors = c.colors;
                infoInput.ColorIdsAlsoPossible = c.ColorIdsAlsoPossible;
                infoInput.CreatureGuid = c.guid;
                infoInput.SetArkId(c.ArkId, c.ArkIdImported);
                UpdateParentListInput(infoInput);
                infoInput.MutationCounterMother = c.mutationsMaternal;
                infoInput.MutationCounterFather = c.mutationsPaternal;
            }
            else
            {
                infoInput.Clear();
            }
        }

        /// <summary>
        /// Set values in extractor to values of given creature
        /// </summary>
        /// <param name="c"></param>
        private void SetCreatureValuesLevelsAndInfoToExtractor(Creature c)
        {
            IsCreatureAlreadyInLibrary(c.guid, c.ArkId, out var alreadyExistingCreature);
            SetNameOfImportedCreature(c, null, out _, alreadyExistingCreature);
            SetInfoInputCreature(c, tester: false);
            SetCreatureValuesToExtractor(c);
            creatureInfoInputExtractor.CreatureGuid = c.guid;
            creatureInfoInputExtractor.SetArkId(c.ArkId, c.ArkIdImported);
            SetCreatureLevelsToExtractor(c);
            SetAllExtractorLevelsToStatus(StatIOStatus.Unique);
            creatureInfoInputExtractor.AlreadyExistingCreature = alreadyExistingCreature;
            UpdateStatusInfoOfExtractorCreature();
            UpdateAddToLibraryButtonAccordingToExtractorValidity(true);
        }

        private void SetRandomWildLevels(object sender, EventArgs e)
        {
            var species = speciesSelector1.SelectedSpecies;
            if (species == null) return;

            var difficulty = (CreatureCollection.CurrentCreatureCollection?.maxWildLevel ?? 150) / 30;
            var creature = DummyCreatures.CreateCreature(species, difficulty, !rbWildTester.Checked);

            for (int si = 0; si < Stats.StatsCount; si++)
            {
                _testingIOs[si].LevelWild = creature.levelsWild[si];
            }

            if (rbTamedTester.Checked)
                NumericUpDownTestingTE.ValueSaveDouble = creature.tamingEff * 100;
        }

        private void pictureBoxColorRegionsTester_Click(object sender, EventArgs e)
        {
            var creature = new Creature
            {
                Species = speciesSelector1.SelectedSpecies,
                levelsWild = GetCurrentWildLevels(false),
                levelsMutated = CreatureCollection.CurrentCreatureCollection.Game == Ark.Asa ? GetCurrentMutLevels(false) : null,
                levelsDom = GetCurrentDomLevels(false),
                tamingEff = TamingEffectivenessTester,
                isBred = rbBredTester.Checked,
                imprintingBonus = (double)numericUpDownImprintingBonusTester.Value / 100
            };

            creatureInfoInputTester.SetCreatureData(creature);
            creature.RecalculateAncestorGenerations();
            creature.RecalculateNewMutations();
            creature.RecalculateCreatureValues(CreatureCollection.CurrentCreatureCollection.wildLevelStep);

            creature.ExportInfoGraphicToClipboard(CreatureCollection.CurrentCreatureCollection);
        }

        private void PbCreatureColorsExtractor_Click(object sender, EventArgs e)
        {
            var creature = new Creature
            {
                Species = speciesSelector1.SelectedSpecies,
                levelsWild = GetCurrentWildLevels(true),
                levelsMutated = CreatureCollection.CurrentCreatureCollection.Game == Ark.Asa ? GetCurrentMutLevels(true) : null,
                levelsDom = GetCurrentDomLevels(true),
                tamingEff = _extractor.UniqueTamingEffectiveness(),
                isBred = rbBredExtractor.Checked,
                imprintingBonus = _extractor.ImprintingBonus
            };

            creatureInfoInputExtractor.SetCreatureData(creature);
            creature.RecalculateAncestorGenerations();
            creature.RecalculateNewMutations();
            creature.RecalculateCreatureValues(CreatureCollection.CurrentCreatureCollection.wildLevelStep);

            creature.ExportInfoGraphicToClipboard(CreatureCollection.CurrentCreatureCollection);
        }

        private void NumericUpDownTestingTE_ValueChanged(object sender, EventArgs e)
        {
            UpdateAllTesterValues();
            DisplayPreTamedLevelTester();
        }

        private void DisplayPreTamedLevelTester()
        {
            if (TamingEffectivenessTester >= 0)
                lbWildLevelTester.Text =
                    $"{Loc.S("preTameLevel")}: {Creature.CalculatePreTameWildLevel(_testingIOs[Stats.Torpidity].LevelWild + 1, TamingEffectivenessTester)}";
            else
                lbWildLevelTester.Text =
                    $"{Loc.S("preTameLevel")}: {Loc.S("unknown")}";
        }

        /// <summary>
        /// Taming effectiveness for the creature in the Tester (range 0-1).
        /// -1 indicates unknown, -3 a wild creature.
        /// </summary>
        private double TamingEffectivenessTester
        {
            get => rbWildTester.Checked ? -3 : (double)NumericUpDownTestingTE.Value / 100;
            set => NumericUpDownTestingTE.ValueSave = (decimal)(value >= 0 ? value * 100 : -1);
        }

        private void CbLinkWildMutatedLevelsTester_CheckedChanged(object sender, EventArgs e)
        {
            var linkWildMutated = CbLinkWildMutatedLevelsTester.Checked;
            for (int s = 0; s < Stats.StatsCount; s++)
                _testingIOs[s].LinkWildMutated = linkWildMutated;
            Properties.Settings.Default.TesterLinkWildMutatedLevels = linkWildMutated;
        }

        private void BtSetImprinting100Tester_Click(object sender, EventArgs e)
        {
            // set imprinting to 100 %, or if already at 100 % to 0
            numericUpDownImprintingBonusTester.ValueSaveDouble =
                numericUpDownImprintingBonusTester.Value == 100 ? 0 : 100;
        }
    }
}
