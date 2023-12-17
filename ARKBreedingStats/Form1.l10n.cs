namespace ARKBreedingStats
{
    public partial class Form1
    {
        private void InitLocalization()
        {
            Loc.LoadResourceFile(Properties.Settings.Default.language, Properties.Settings.Default.language2);
            Utils.InitializeLocalizations();
        }

        /// <summary>
        /// Sets the localizations, i.e. translated texts in the application.
        /// </summary>
        /// <param name="initialize">If the language was changed set to true to load the according strings.</param>
        private void SetLocalizations(bool initialize = true)
        {
            if (initialize)
                InitLocalization();

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
            Loc.ControlText(toolStripButtonDeleteExpiredIncubationTimers);
            Loc.ControlText(toolStripButtonSaveCreatureValuesTemp);
            Loc.ControlText(toolStripButtonDeleteTempCreature);
            Loc.ControlText(tsBtAddAsExtractionTest);
            Loc.ControlText(copyToMultiplierTesterToolStripButton);
            Loc.ControlText(copyInfographicToClipboardToolStripMenuItem, "copyInfoGraphicToClipboard");

            // top bar
            Loc.ControlText(cbEventMultipliers, "Event");
            Loc.ControlText(cbGuessSpecies, _tt);
            Loc.ControlText(btReadValuesFromArk, _tt);
            Loc.ControlText(btImportLastExported, _tt);
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
            Loc.SetToolTip(lbImprintedCount, _tt);
            Loc.SetToolTip(lbTesterDomLevel, "domLevelExplanation", _tt);
            Loc.SetToolTip(lbTesterWildLevel, "wildLevelExplanation", _tt);

            // extractor
            Loc.ControlText(tabPageExtractor, "extractor");
            Loc.ControlText(lbCurrentStatEx, "currentStatValue");
            Loc.ControlText(lbExtractorWildLevel, "wildLvl");
            Loc.ControlText(lbExtractorDomLevel, "domLvl");
            Loc.ControlText(lbSum);
            Loc.ControlText(lbShouldBe);
            Loc.ControlText(lbImprintingFailInfo);
            Loc.ControlText(cbExactlyImprinting, _tt);
            Loc.ControlText(btExtractLevels);
            Loc.ControlText(cbQuickWildCheck, _tt);
            Loc.ControlText(rbWildExtractor, "wild");
            Loc.ControlText(rbTamedExtractor, "tamed");
            Loc.ControlText(rbBredExtractor, "bred");
            Loc.SetToolTip(lbImprintingCuddleCountExtractor, _tt);
            Loc.SetToolTip(lbSumWild, _tt);
            Loc.SetToolTip(lbSumDom, _tt);
            Loc.SetToolTip(lbSumDomSB, _tt);
            Loc.SetToolTip(lbListening, _tt);
            Loc.SetToolTip(lbExtractorDomLevel, "domLevelExplanation", _tt);
            Loc.SetToolTip(lbExtractorWildLevel, "wildLevelExplanation", _tt);
            var statNames = speciesSelector1.SelectedSpecies?.statNames;
            for (int si = 0; si < _statIOs.Length; si++)
            {
                _statIOs[si].Title = Utils.StatName(si, false, statNames);
                _testingIOs[si].Title = Utils.StatName(si, false, statNames);
            }
            parentInheritanceExtractor.SetLocalizations();
            statPotentials1.SetLocalization();

            // library
            Loc.ControlText(tabPageLibrary, "library");
            columnHeaderName.Text = Loc.S("Name");
            columnHeaderOwner.Text = Loc.S("Owner");
            columnHeaderTribe.Text = Loc.S("Tribe");
            columnHeaderNote.Text = Loc.S("Note");
            columnHeaderServer.Text = Loc.S("Server");
            columnHeaderHP.Text = Utils.StatName(Stats.Health, true);
            columnHeaderSt.Text = Utils.StatName(Stats.Stamina, true);
            columnHeaderOx.Text = Utils.StatName(Stats.Oxygen, true);
            columnHeaderFo.Text = Utils.StatName(Stats.Food, true);
            columnHeaderWe.Text = Utils.StatName(Stats.Weight, true);
            columnHeaderDm.Text = Utils.StatName(Stats.MeleeDamageMultiplier, true);
            columnHeaderSp.Text = Utils.StatName(Stats.SpeedMultiplier, true);
            columnHeaderTo.Text = Utils.StatName(Stats.Torpidity, true);
            columnHeaderWa.Text = Utils.StatName(Stats.CraftingSpeedMultiplier, true);
            columnHeaderTm.Text = Utils.StatName(Stats.Temperature, true);
            columnHeaderCr.Text = Utils.StatName(Stats.Water, true);
            columnHeaderFr.Text = Utils.StatName(Stats.TemperatureFortitude, true);
            columnHeaderHPM.Text = Utils.StatName(Stats.Health, true) + "M";
            columnHeaderStM.Text = Utils.StatName(Stats.Stamina, true) + "M";
            columnHeaderOxM.Text = Utils.StatName(Stats.Oxygen, true) + "M";
            columnHeaderFoM.Text = Utils.StatName(Stats.Food, true) + "M";
            columnHeaderWeM.Text = Utils.StatName(Stats.Weight, true) + "M";
            columnHeaderDmM.Text = Utils.StatName(Stats.MeleeDamageMultiplier, true) + "M";
            columnHeaderSpM.Text = Utils.StatName(Stats.SpeedMultiplier, true) + "M";
            columnHeaderToM.Text = Utils.StatName(Stats.Torpidity, true) + "M";
            columnHeaderWaM.Text = Utils.StatName(Stats.CraftingSpeedMultiplier, true) + "M";
            columnHeaderTmM.Text = Utils.StatName(Stats.Temperature, true) + "M";
            columnHeaderCrM.Text = Utils.StatName(Stats.Water, true) + "M";
            columnHeaderFrM.Text = Utils.StatName(Stats.TemperatureFortitude, true) + "M";
            columnHeaderTopStatsNr.Text = Loc.S("Top");
            columnHeaderTopness.Text = Loc.S("topPercentage");
            columnHeaderGen.Text = Loc.S("Generation_Abb");
            columnHeaderLW.Text = Loc.S("LevelWild_Abb");
            columnHeaderMutations.Text = Loc.S("Mutations_Abb");
            columnHeaderDomesticated.Text = Loc.S("domesticatedAt");
            columnHeaderCooldown.Text = Loc.S("cooldownGrowing");
            columnHeaderColor0.Text = Loc.S("C0");
            columnHeaderColor1.Text = Loc.S("C1");
            columnHeaderColor2.Text = Loc.S("C2");
            columnHeaderColor3.Text = Loc.S("C3");
            columnHeaderColor4.Text = Loc.S("C4");
            columnHeaderColor5.Text = Loc.S("C5");
            Loc.ControlText(ToolStripLabelFilter);

            _tt.SetToolTip(LbBlueprintPath, "Click to copy blueprint path to clipboard.");
            _tt.SetToolTip(listBoxSpeciesLib, "Ctrl + Click on a species to select / unselect as favorite.\nFavorites are sorted at the top.");

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
            raisingControl1.SetLocalizations();
            creatureBoxListView.SetLocalizations();
            notesControl1.SetLocalizations();
            _overlay?.SetLocalizations();
        }
    }
}
