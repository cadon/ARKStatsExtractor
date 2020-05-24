using ARKBreedingStats.species;

namespace ARKBreedingStats
{
    public partial class Form1
    {
        private void initLocalization()
        {
            Loc.LoadResourceFile();
            Utils.InitializeLocalizations();
        }

        private void setLocalizations(bool init = true)
        {
            if (init)
                initLocalization();

            // menu
            Loc.ControlText(fileToolStripMenuItem);
            Loc.ControlText(newToolStripMenuItem);
            Loc.ControlText(loadToolStripMenuItem);
            Loc.ControlText(loadAndAddToolStripMenuItem);
            Loc.ControlText(saveToolStripMenuItem);
            Loc.ControlText(saveAsToolStripMenuItem);
            Loc.ControlText(importingFromSavegameToolStripMenuItem);
            Loc.ControlText(importingFromSavegameEmptyToolStripMenuItem);
            //Loc.ControlText(runDefaultExtractionAndImportFileToolStripMenuItem);
            //Loc.ControlText(runDefaultExtractionToolStripMenuItem);
            //Loc.ControlText(importCreatedJsonfileToolStripMenuItem);
            Loc.ControlText(importExportedCreaturesToolStripMenuItem);
            //Loc.ControlText(runDefaultExtractionAndImportFileToolStripMenuItem);
            //Loc.ControlText(runDefaultExtractionToolStripMenuItem);
            //Loc.ControlText(importCreatedJsonfileToolStripMenuItem);
            Loc.ControlText(modValueManagerToolStripMenuItem);
            Loc.ControlText(settingsToolStripMenuItem);
            Loc.ControlText(openSettingsToolStripMenuItem);
            Loc.ControlText(quitToolStripMenuItem);
            Loc.ControlText(editToolStripMenuItem);
            Loc.ControlText(exportValuesToClipboardToolStripMenuItem);
            Loc.ControlText(importValuesFromClipboardToolStripMenuItem);
            Loc.ControlText(setStatusToolStripMenuItem);
            Loc.ControlText(multiSetterToolStripMenuItem);
            Loc.ControlText(deleteSelectedToolStripMenuItem);
            Loc.ControlText(findDuplicatesToolStripMenuItem);
            Loc.ControlText(copyCreatureToolStripMenuItem);
            Loc.ControlText(pasteCreatureToolStripMenuItem);
            Loc.ControlText(libraryFilterToolStripMenuItem);
            Loc.ControlText(helpToolStripMenuItem);
            Loc.ControlText(aboutToolStripMenuItem);
            Loc.ControlText(onlinehelpToolStripMenuItem);
            Loc.ControlText(BreedingPlanHelpToolStripMenuItem);
            Loc.ControlText(extractionIssuesToolStripMenuItem);
            Loc.ControlText(checkForUpdatedStatsToolStripMenuItem);
            Loc.ControlText(toolStripButtonCopy2Tester);
            Loc.ControlText(toolStripButtonCopy2Extractor);
            Loc.ControlText(toolStripButtonClear);
            Loc.ControlText(toolStripButtonAddNote);
            Loc.ControlText(toolStripButtonRemoveNote);
            Loc.ControlText(toolStripButtonDeleteExpiredIncubationTimers);
            Loc.ControlText(toolStripButtonSaveCreatureValuesTemp);
            Loc.ControlText(toolStripButtonDeleteTempCreature);
            Loc.ControlText(tsBtAddAsExtractionTest);
            Loc.ControlText(copyToMultiplierTesterToolStripButton);

            // top bar
            Loc.ControlText(cbGuessSpecies, tt);
            Loc.ControlText(btReadValuesFromArk, tt);
            Loc.ControlText(btImportLastExported, tt);
            Loc.ControlText(cbToggleOverlay);

            // tester
            Loc.ControlText(tabPageStatTesting, "statTesting");
            Loc.ControlText(rbWildTester, "wild");
            Loc.ControlText(rbTamedTester, "tamed");
            Loc.ControlText(rbBredTester, "bred");
            Loc.ControlText(lbTesterWildLevel, "wildLvl");
            Loc.ControlText(lbTesterDomLevel, "domLvl");
            Loc.ControlText(lbCurrentValue, "currentValue");
            Loc.ControlText(lbBreedingValueTester, "breedingValue");
            Loc.ControlText(lbNotYetTamed);
            Loc.ControlText(gpPreviewEdit);
            Loc.ControlText(lbTestingInfo);
            Loc.ControlText(gbStatChart, "statChart");
            Loc.ControlText(lbCurrentCreature, "CurrentCreature");
            Loc.setToolTip(lbImprintedCount, tt);
            Loc.setToolTip(lbTesterDomLevel, "domLevelExplanation", tt);
            Loc.setToolTip(lbTesterWildLevel, "wildLevelExplanation", tt);

            // extractor
            Loc.ControlText(tabPageExtractor, "extractor");
            Loc.ControlText(lbCurrentStatEx, "currentStatValue");
            Loc.ControlText(lbExtractorWildLevel, "wildLvl");
            Loc.ControlText(lbExtractorDomLevel, "domLvl");
            Loc.ControlText(lbSum);
            Loc.ControlText(lbShouldBe);
            Loc.ControlText(lbImprintingFailInfo);
            Loc.ControlText(cbExactlyImprinting, tt);
            Loc.ControlText(btExtractLevels);
            Loc.ControlText(cbQuickWildCheck, tt);
            Loc.ControlText(rbWildExtractor, "wild");
            Loc.ControlText(rbTamedExtractor, "tamed");
            Loc.ControlText(rbBredExtractor, "bred");
            Loc.setToolTip(lbImprintingCuddleCountExtractor, tt);
            Loc.setToolTip(lbSumWild, tt);
            Loc.setToolTip(lbSumDom, tt);
            Loc.setToolTip(lbSumDomSB, tt);
            Loc.setToolTip(lbListening, tt);
            Loc.setToolTip(lbExtractorDomLevel, "domLevelExplanation", tt);
            Loc.setToolTip(lbExtractorWildLevel, "wildLevelExplanation", tt);

            // library
            Loc.ControlText(tabPageLibrary, "library");
            columnHeaderName.Text = Loc.s("Name");
            columnHeaderOwner.Text = Loc.s("Owner");
            columnHeaderTribe.Text = Loc.s("Tribe");
            columnHeaderNote.Text = Loc.s("Note");
            columnHeaderServer.Text = Loc.s("Server");
            columnHeaderHP.Text = Utils.StatName(StatNames.Health, true);
            columnHeaderSt.Text = Utils.StatName(StatNames.Stamina, true);
            columnHeaderOx.Text = Utils.StatName(StatNames.Oxygen, true);
            columnHeaderFo.Text = Utils.StatName(StatNames.Food, true);
            columnHeaderWe.Text = Utils.StatName(StatNames.Weight, true);
            columnHeaderDm.Text = Utils.StatName(StatNames.MeleeDamageMultiplier, true);
            columnHeaderSp.Text = Utils.StatName(StatNames.SpeedMultiplier, true);
            columnHeaderTo.Text = Utils.StatName(StatNames.Torpidity, true);
            columnHeaderWa.Text = Utils.StatName(StatNames.CraftingSpeedMultiplier, true);
            columnHeaderTemp.Text = Utils.StatName(StatNames.Temperature, true);
            columnHeaderCr.Text = Utils.StatName(StatNames.Water, true);
            columnHeaderFr.Text = Utils.StatName(StatNames.TemperatureFortitude, true);
            columnHeaderTopStatsNr.Text = Loc.s("Top");
            columnHeaderTopness.Text = Loc.s("topPercentage");
            columnHeaderGen.Text = Loc.s("Generation_Abb");
            columnHeaderLW.Text = Loc.s("LevelWild_Abb");
            columnHeaderMutations.Text = Loc.s("Mutations_Abb");
            columnHeaderAdded.Text = Loc.s("added");
            columnHeaderCooldown.Text = Loc.s("cooldownGrowing");
            columnHeaderColor0.Text = Loc.s("C0");
            columnHeaderColor1.Text = Loc.s("C1");
            columnHeaderColor2.Text = Loc.s("C2");
            columnHeaderColor3.Text = Loc.s("C3");
            columnHeaderColor4.Text = Loc.s("C4");
            columnHeaderColor5.Text = Loc.s("C5");

            // other tabs
            Loc.ControlText(tabPagePedigree, "pedigree");
            Loc.ControlText(tabPageTaming, "Taming");
            Loc.ControlText(tabPageBreedingPlan, "BreedingPlan");
            Loc.ControlText(tabPageRaising, "Raising");
            Loc.ControlText(tabPagePlayerTribes, "Player");

            // other controls
            creatureInfoInputTester.SetLocalizations();
            creatureInfoInputExtractor.SetLocalizations();
            pedigree1.SetLocalizations();
            tamingControl1.SetLocalizations();
            breedingPlan1.SetLocalizations();
        }
    }
}
