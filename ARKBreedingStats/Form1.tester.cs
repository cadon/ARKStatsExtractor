using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ARKBreedingStats
{
    public partial class Form1
    {
        /// <summary>
        /// Updates the labels for the creature stats outside of levels and info in the creatureInfoInput, i.e. wild/tame/bred, TE, Imprinting.
        /// </summary>
        private void UpdateTesterDetails()
        {
            setTesterInputsTamed(!rbWildTester.Checked);
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
            if (c != null)
            {
                speciesSelector1.SetSpecies(c.Species);
                NumericUpDownTestingTE.ValueSave = c.tamingEff >= 0 ? (decimal)c.tamingEff * 100 : 0;
                numericUpDownImprintingBonusTester.ValueSave = (decimal)c.imprintingBonus * 100;
                if (c.isBred)
                    rbBredTester.Checked = true;
                else if (c.tamingEff > 0 || c.tamingEff == -2) // -2 is unknown (e.g. Giganotosaurus)
                    rbTamedTester.Checked = true;
                else
                    rbWildTester.Checked = true;

                hiddenLevelsCreatureTester = c.levelsWild[(int)StatNames.Torpidity];
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (s != (int)StatNames.Torpidity && c.levelsWild[s] > 0)
                        hiddenLevelsCreatureTester -= c.levelsWild[s];
                }

                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (s == (int)StatNames.Torpidity)
                        continue;
                    testingIOs[s].LevelWild = c.levelsWild[s];
                    testingIOs[s].LevelDom = c.levelsDom[s];
                }
                tabControlMain.SelectedTab = tabPageStatTesting;
                SetTesterInfoInputCreature(c, virtualCreature);
            }
        }

        private void UpdateAllTesterValues()
        {
            updateTorporInTester = false;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s == (int)StatNames.Torpidity)
                    continue;
                if (s == Values.STATS_COUNT - 2) // update torpor after last stat-update
                    updateTorporInTester = true;
                testingStatIOsRecalculateValue(testingIOs[s]);
            }
            testingStatIOsRecalculateValue(testingIOs[(int)StatNames.Torpidity]);
        }

        private void setTesterInputsTamed(bool tamed)
        {
            for (int s = 0; s < Values.STATS_COUNT; s++)
                testingIOs[s].postTame = tamed;
            lbNotYetTamed.Visible = !tamed;
        }

        /// <summary>
        /// Updates the values in the testing-statIOs
        /// </summary>
        /// <param name="sIo"></param>
        private void testingStatIOValueUpdate(StatIO sIo)
        {
            testingStatIOsRecalculateValue(sIo);

            // update Torpor-level if changed value is not from torpor-StatIO
            if (updateTorporInTester && sIo.statIndex != (int)StatNames.Torpidity)
            {
                int torporLvl = 0;
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (s != (int)StatNames.Torpidity)
                        torporLvl += testingIOs[s].LevelWild > 0 ? testingIOs[s].LevelWild : 0;
                }
                testingIOs[(int)StatNames.Torpidity].LevelWild = torporLvl + hiddenLevelsCreatureTester;
            }

            int domLevels = 0;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                domLevels += testingIOs[s].LevelDom;
            }
            labelDomLevelSum.Text = $"Dom Levels: {domLevels}/{creatureCollection.maxDomLevel}";
            labelDomLevelSum.BackColor = domLevels > creatureCollection.maxDomLevel ? Color.LightSalmon : Color.Transparent;
            labelTesterTotalLevel.Text = $"Total Levels: {testingIOs[(int)StatNames.Torpidity].LevelWild + domLevels + 1}/{testingIOs[(int)StatNames.Torpidity].LevelWild + 1 + creatureCollection.maxDomLevel}";
            creatureInfoInputTester.parentListValid = false;

            int[] levelsWild = testingIOs.Select(s => s.LevelWild).ToArray();
            if (!testingIOs[2].Enabled)
                levelsWild[2] = 0;
            radarChart1.setLevels(levelsWild);
            statPotentials1.SetLevels(levelsWild, false);
            //statGraphs1.setGraph(sE, 0, testingIOs[0].LevelWild, testingIOs[0].LevelDom, !radioButtonTesterWild.Checked, (double)NumericUpDownTestingTE.Value / 100, (double)numericUpDownImprintingBonusTester.Value / 100);

            if (sIo.statIndex == (int)StatNames.Torpidity)
                lbWildLevelTester.Text = "PreTame Level: " + Math.Ceiling(Math.Round((testingIOs[(int)StatNames.Torpidity].LevelWild + 1) / (1 + NumericUpDownTestingTE.Value / 200), 6));
        }

        private void testingStatIOsRecalculateValue(StatIO sIo)
        {
            sIo.BreedingValue = StatValueCalculation.CalculateValue(speciesSelector1.SelectedSpecies, sIo.statIndex, sIo.LevelWild, 0, true, 1, 0);
            sIo.Input = StatValueCalculation.CalculateValue(speciesSelector1.SelectedSpecies, sIo.statIndex, sIo.LevelWild, sIo.LevelDom,
                    rbTamedTester.Checked || rbBredTester.Checked,
                    rbBredTester.Checked ? 1 : (double)NumericUpDownTestingTE.Value / 100,
                    rbBredTester.Checked ? (double)numericUpDownImprintingBonusTester.Value / 100 : 0);
        }

        private void creatureInfoInputTester_Save2Library_Clicked(CreatureInfoInput sender)
        {
            if (creatureTesterEdit == null)
                return;
            // check if wild levels are changed, if yes warn that the creature can become invalid
            bool wildChanged = Math.Abs(creatureTesterEdit.tamingEff - (double)NumericUpDownTestingTE.Value / 100) > .0005;
            if (!wildChanged)
            {
                int[] wildLevels = GetCurrentWildLevels(false);
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (wildLevels[s] != creatureTesterEdit.levelsWild[s])
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

            bool statusChanged = creatureTesterEdit.status != creatureInfoInputTester.CreatureStatus
                    || creatureTesterEdit.owner != creatureInfoInputTester.CreatureOwner
                    || creatureTesterEdit.mutationsMaternal != creatureInfoInputTester.MutationCounterMother
                    || creatureTesterEdit.mutationsPaternal != creatureInfoInputTester.MutationCounterFather;
            bool parentsChanged = creatureTesterEdit.Mother != creatureInfoInputTester.mother || creatureTesterEdit.Father != creatureInfoInputTester.father;
            creatureTesterEdit.levelsWild = GetCurrentWildLevels(false);
            creatureTesterEdit.levelsDom = GetCurrentDomLevels(false);
            creatureTesterEdit.tamingEff = (double)NumericUpDownTestingTE.Value / 100;
            creatureTesterEdit.isBred = rbBredTester.Checked;
            creatureTesterEdit.imprintingBonus = (double)numericUpDownImprintingBonusTester.Value / 100;

            creatureTesterEdit.name = creatureInfoInputTester.CreatureName;
            creatureTesterEdit.sex = creatureInfoInputTester.CreatureSex;
            creatureTesterEdit.owner = creatureInfoInputTester.CreatureOwner;
            creatureTesterEdit.tribe = creatureInfoInputTester.CreatureTribe;
            creatureTesterEdit.server = creatureInfoInputTester.CreatureServer;
            creatureTesterEdit.Mother = creatureInfoInputTester.mother;
            creatureTesterEdit.Father = creatureInfoInputTester.father;
            creatureTesterEdit.note = creatureInfoInputTester.CreatureNote;
            creatureTesterEdit.status = creatureInfoInputTester.CreatureStatus;
            creatureTesterEdit.cooldownUntil = creatureInfoInputTester.Cooldown;
            creatureTesterEdit.growingUntil = creatureInfoInputTester.Grown;
            creatureTesterEdit.domesticatedAt = creatureInfoInputTester.domesticatedAt;
            creatureTesterEdit.flags = creatureInfoInputTester.creatureFlags;
            creatureTesterEdit.mutationsMaternal = creatureInfoInputTester.MutationCounterMother;
            creatureTesterEdit.mutationsPaternal = creatureInfoInputTester.MutationCounterFather;
            creatureTesterEdit.colors = creatureInfoInputTester.RegionColors;
            creatureTesterEdit.ArkId = creatureInfoInputTester.ArkId;

            if (wildChanged)
                CalculateTopStats(creatureCollection.creatures.Where(c => c.Species == creatureTesterEdit.Species).ToList());
            UpdateDisplayedCreatureValues(creatureTesterEdit, statusChanged);

            if (parentsChanged)
                creatureTesterEdit.RecalculateAncestorGenerations();

            // if maturation was changed, update raising-timers
            if (creatureTesterEdit.growingUntil != creatureInfoInputTester.Grown)
            {
                raisingControl1.RecreateList();
                creatureTesterEdit.StartStopMatureTimer(true);
            }

            SetTesterInfoInputCreature();
            tabControlMain.SelectedTab = tabPageLibrary;
        }

        /// <summary>
        /// Set the values in the creatureInfoInput control to the values of the creature.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="virtualCreature"></param>
        private void SetTesterInfoInputCreature(Creature c = null, bool virtualCreature = false)
        {
            bool enable = c != null; // set to a creature, or clear
            creatureInfoInputTester.ShowSaveButton = enable && !virtualCreature;
            labelCurrentTesterCreature.Visible = enable;
            lbCurrentCreature.Visible = enable;
            if (enable)
            {
                labelCurrentTesterCreature.Text = c.name;
                creatureInfoInputTester.mother = c.Mother;
                creatureInfoInputTester.father = c.Father;
                creatureInfoInputTester.CreatureName = c.name;
                creatureInfoInputTester.CreatureSex = c.sex;
                creatureInfoInputTester.CreatureOwner = c.owner;
                creatureInfoInputTester.CreatureTribe = c.tribe;
                creatureInfoInputTester.CreatureServer = c.server;
                creatureInfoInputTester.CreatureStatus = c.status;
                creatureInfoInputTester.CreatureNote = c.note;
                creatureInfoInputTester.Cooldown = c.cooldownUntil;
                creatureInfoInputTester.Grown = c.growingUntil;
                creatureInfoInputTester.domesticatedAt = c.domesticatedAt;
                creatureInfoInputTester.AddedToLibraryAt = c.addedToLibrary;
                creatureInfoInputTester.creatureFlags = c.flags;
                creatureInfoInputTester.RegionColors = c.colors;
                creatureInfoInputTester.CreatureGuid = c.guid;
                creatureInfoInputTester.SetArkId(c.ArkId, c.ArkIdImported);
                UpdateParentListInput(creatureInfoInputTester);
                creatureInfoInputTester.MutationCounterMother = c.mutationsMaternal;
                creatureInfoInputTester.MutationCounterFather = c.mutationsPaternal;
            }
            else
            {
                creatureInfoInputTester.mother = null;
                creatureInfoInputTester.father = null;
                creatureInfoInputTester.CreatureName = "";
                creatureInfoInputTester.CreatureSex = Sex.Unknown;
                creatureInfoInputTester.CreatureOwner = "";
                creatureInfoInputTester.CreatureTribe = "";
                creatureInfoInputTester.CreatureServer = "";
                creatureInfoInputTester.CreatureStatus = CreatureStatus.Available;
                creatureInfoInputTester.CreatureNote = "";
                creatureInfoInputTester.Cooldown = DateTime.Now.AddHours(-1);
                creatureInfoInputTester.Grown = DateTime.Now.AddHours(-1);
                creatureInfoInputTester.domesticatedAt = null;
                creatureInfoInputTester.AddedToLibraryAt = null;
                creatureInfoInputTester.creatureFlags = CreatureFlags.None;
                creatureInfoInputTester.RegionColors = new int[6];
                creatureInfoInputTester.CreatureGuid = Guid.Empty;
                creatureInfoInputTester.SetArkId(0, false);
                creatureInfoInputTester.MutationCounterMother = 0;
                creatureInfoInputTester.parentListValid = false;
            }
            creatureTesterEdit = c;
        }

        private void SetCreatureValuesToExtractor(Creature c, bool onlyWild = false)
        {
            if (c != null)
            {
                Species species = c.Species;
                if (species != null)
                {
                    ClearAll();
                    // copy values over to extractor
                    for (int s = 0; s < Values.STATS_COUNT; s++)
                        statIOs[s].Input = onlyWild ? StatValueCalculation.CalculateValue(species, s, c.levelsWild[s], 0, true, c.tamingEff, c.imprintingBonus) : c.valuesDom[s];
                    speciesSelector1.SetSpecies(species);

                    if (c.isBred)
                        rbBredExtractor.Checked = true;
                    else if (c.tamingEff >= 0)
                        rbTamedExtractor.Checked = true;
                    else
                        rbWildExtractor.Checked = true;

                    numericUpDownImprintingBonusExtractor.ValueSave = (decimal)c.imprintingBonus * 100;
                    // set total level
                    int level = onlyWild ? c.levelsWild[(int)StatNames.Torpidity] : c.Level;
                    numericUpDownLevel.ValueSave = level;

                    tabControlMain.SelectedTab = tabPageExtractor;
                }
                else
                    MessageBox.Show("Unknown Species. Try to update the species-stats, or redownload the tool.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
