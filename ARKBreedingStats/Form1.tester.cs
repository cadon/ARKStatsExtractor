using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.library;
using ARKBreedingStats.utils;

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
                    _hiddenLevelsCreatureTester -= c.levelsWild[s];
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
            SetTesterInfoInputCreature(c, virtualCreature);
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
            int[] levelsMutations = _testingIOs.Select(s => s.LevelWild + s.LevelMut).ToArray();
            if (!_testingIOs[2].Enabled)
                levelsWild[2] = 0;
            radarChart1.SetLevels(levelsWild, levelsMutations);
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
                    rbBredTester.Checked ? (double)numericUpDownImprintingBonusTester.Value / 100 : 0);
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
            bool wildChanged = Math.Abs(_creatureTesterEdit.tamingEff - TamingEffectivenessTester) > .0005;
            if (!wildChanged)
            {
                int[] wildLevels = GetCurrentWildLevels(false);
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (wildLevels[s] != _creatureTesterEdit.levelsWild[s])
                    {
                        wildChanged = true;
                        break;
                    }
                }
            }
            if (wildChanged && MessageBox.Show("The wild levels or the taming-effectiveness were changed. Save values anyway?\n" +
                    "Only save if the wild levels or taming-effectiveness were extracted wrongly!\nIf you are not sure, don't save. " +
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
            _creatureTesterEdit.levelsDom = GetCurrentDomLevels(false);
            _creatureTesterEdit.tamingEff = TamingEffectivenessTester;
            _creatureTesterEdit.isBred = rbBredTester.Checked;
            _creatureTesterEdit.imprintingBonus = (double)numericUpDownImprintingBonusTester.Value / 100;

            creatureInfoInputTester.SetCreatureData(_creatureTesterEdit);

            if (wildChanged)
                CalculateTopStats(_creatureCollection.creatures.Where(c => c.Species == _creatureTesterEdit.Species).ToList());
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

            SetTesterInfoInputCreature();
            _libraryNeedsUpdate = true;
            tabControlMain.SelectedTab = tabPageLibrary;
        }

        /// <summary>
        /// Set the values in the creatureInfoInput control to the values of the creature or clears the inputs.
        /// </summary>
        private void SetTesterInfoInputCreature(Creature c = null, bool virtualCreature = false)
        {
            bool enable = c != null; // set to a creature, or clear
            creatureInfoInputTester.ShowSaveButton = enable && !virtualCreature;
            labelCurrentTesterCreature.Visible = enable;
            lbCurrentCreature.Visible = enable;
            if (enable)
            {
                labelCurrentTesterCreature.Text = c.name;
                creatureInfoInputTester.Mother = c.Mother;
                creatureInfoInputTester.Father = c.Father;
                creatureInfoInputTester.CreatureName = c.name;
                creatureInfoInputTester.CreatureSex = c.sex;
                creatureInfoInputTester.CreatureOwner = c.owner;
                creatureInfoInputTester.CreatureTribe = c.tribe;
                creatureInfoInputTester.CreatureServer = c.server;
                creatureInfoInputTester.CreatureStatus = c.Status;
                creatureInfoInputTester.CreatureNote = c.note;
                creatureInfoInputTester.CooldownUntil = c.cooldownUntil;
                creatureInfoInputTester.GrowingUntil = c.growingUntil;
                creatureInfoInputTester.DomesticatedAt = c.domesticatedAt;
                creatureInfoInputTester.AddedToLibraryAt = c.addedToLibrary;
                creatureInfoInputTester.CreatureFlags = c.flags;
                creatureInfoInputTester.RegionColors = c.colors;
                creatureInfoInputTester.CreatureGuid = c.guid;
                creatureInfoInputTester.SetArkId(c.ArkId, c.ArkIdImported);
                UpdateParentListInput(creatureInfoInputTester);
                creatureInfoInputTester.MutationCounterMother = c.mutationsMaternal;
                creatureInfoInputTester.MutationCounterFather = c.mutationsPaternal;
            }
            else
            {
                creatureInfoInputTester.Mother = null;
                creatureInfoInputTester.Father = null;
                creatureInfoInputTester.CreatureName = string.Empty;
                creatureInfoInputTester.CreatureSex = Sex.Unknown;
                creatureInfoInputTester.CreatureOwner = string.Empty;
                creatureInfoInputTester.CreatureTribe = string.Empty;
                creatureInfoInputTester.CreatureServer = string.Empty;
                creatureInfoInputTester.CreatureStatus = CreatureStatus.Available;
                creatureInfoInputTester.CreatureNote = string.Empty;
                creatureInfoInputTester.CooldownUntil = DateTime.Now.AddHours(-1);
                creatureInfoInputTester.GrowingUntil = DateTime.Now.AddHours(-1);
                creatureInfoInputTester.DomesticatedAt = null;
                creatureInfoInputTester.AddedToLibraryAt = null;
                creatureInfoInputTester.CreatureFlags = CreatureFlags.None;
                creatureInfoInputTester.RegionColors = new byte[Ark.ColorRegionCount];
                creatureInfoInputTester.CreatureGuid = Guid.Empty;
                creatureInfoInputTester.SetArkId(0, false);
                creatureInfoInputTester.MutationCounterMother = 0;
                creatureInfoInputTester.parentListValid = false;
            }
            _creatureTesterEdit = c;
        }

        private void SetCreatureValuesToExtractor(Creature c, bool onlyWild = false)
        {
            if (c == null) return;
            Species species = c.Species;
            if (species == null)
            {
                MessageBoxes.ShowMessageBox($"Unknown species\n{c.speciesBlueprint}\nTry to update the species-stats, or redownload the tool.");
                return;
            }

            ClearAll();
            speciesSelector1.SetSpecies(species);
            // copy values over to extractor
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _statIOs[s].Input = onlyWild
                    ? StatValueCalculation.CalculateValue(species, s, c.levelsWild[s], c.levelsMutated[s], 0, true, c.tamingEff,
                        c.imprintingBonus)
                    : c.valuesDom[s];
                if (c.levelsDom[s] > 0) _statIOs[s].DomLevelLockedZero = false;
            }

            if (c.isBred)
                rbBredExtractor.Checked = true;
            else if (c.isDomesticated)
                rbTamedExtractor.Checked = true;
            else
                rbWildExtractor.Checked = true;

            numericUpDownImprintingBonusExtractor.ValueSave = (decimal)c.imprintingBonus * 100;
            // set total level
            int level = onlyWild ? c.levelsWild[Stats.Torpidity] : c.Level;
            numericUpDownLevel.ValueSave = level;

            // set colors
            creatureInfoInputExtractor.RegionColors = c.colors;

            tabControlMain.SelectedTab = tabPageExtractor;
        }

        private void SetRandomWildLevels(object sender, EventArgs e)
        {
            var species = speciesSelector1.SelectedSpecies;
            if (species == null) return;

            var difficulty = (CreatureCollection.CurrentCreatureCollection?.maxWildLevel ?? 150) / 30;
            var creature = DummyCreatures.CreateCreature(species, difficulty, false);

            for (int si = 0; si < Stats.StatsCount; si++)
            {
                _testingIOs[si].LevelWild = creature.levelsWild[si];
            }
        }

        private void pictureBoxColorRegionsTester_Click(object sender, EventArgs e)
        {
            var creature = new Creature
            {
                Species = speciesSelector1.SelectedSpecies,
                levelsWild = GetCurrentWildLevels(false),
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
    }
}
