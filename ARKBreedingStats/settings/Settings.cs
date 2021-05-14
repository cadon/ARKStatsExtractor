using ARKBreedingStats.Library;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ARKBreedingStats.library;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.settings
{
    public partial class Settings : Form
    {
        private MultiplierSetting[] _multSetter;
        private readonly CreatureCollection _cc;
        private ToolTip _tt;
        private Dictionary<string, string> _languages;
        public SettingsTabPages LastTabPageIndex;
        public bool LanguageChanged;

        public Settings(CreatureCollection cc, SettingsTabPages page)
        {
            InitializeData();
            _cc = cc;
            CreateListOfProcesses();
            LoadSettings(cc);
            Localization();
            tabControlSettings.SelectTab((int)page);
        }

        private const string DefaultOcrProcessName = "ShooterGame";
        /// <summary>
        /// Creates the list of currently running processes for an easy selection for the process the OCR uses to capture.
        /// </summary>
        private void CreateListOfProcesses()
        {
            // Wine doesn't support the Process.ProcessName getter and OCR doesn't work there currently
            try
            {
                cbbOCRApp.DataSource = System.Diagnostics.Process.GetProcesses().Select(p => new ProcessSelector
                { ProcessName = p.ProcessName, MainWindowTitle = p.MainWindowTitle })
                    .Distinct().Where(pn =>
                        !string.IsNullOrEmpty(pn.MainWindowTitle) && pn.ProcessName != "System" &&
                        pn.ProcessName != "idle").OrderBy(pn => pn.ProcessName).ToArray();
            }
            catch (InvalidOperationException)
            {
                // OCR currently doesn't work on Wine, so hide settings tab page
                tabControlSettings.TabPages.Remove(tabPageOCR);
            }
        }

        private struct ProcessSelector
        {
            public string ProcessName;
            public string MainWindowTitle;
            public override string ToString() => $"{ProcessName} ({MainWindowTitle})";
        }

        private void cbOCRApp_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbOCRCaptureApp.Text = (cbbOCRApp.SelectedItem is ProcessSelector ps) ? ps.ProcessName : null;
        }

        private void InitializeData()
        {
            InitializeComponent();
            DisplayServerMultiplierPresets();
            _multSetter = new MultiplierSetting[Values.STATS_COUNT];
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                _multSetter[s] = new MultiplierSetting
                {
                    StatName = $"[{s}] {Utils.StatName(s)}"
                };
                flowLayoutPanelStatMultipliers.Controls.Add(_multSetter[s]);
            }

            // set neutral numbers for stat-multipliers to the default values to easier see what is non-default
            ServerMultipliers officialMultipliers = Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.Official);
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s < officialMultipliers.statMultipliers.Length)
                    _multSetter[s].SetNeutralValues(officialMultipliers.statMultipliers[s]);
                else _multSetter[s].SetNeutralValues(null);
            }
            nudTamingSpeed.NeutralNumber = 1;
            nudDinoCharacterFoodDrain.NeutralNumber = 1;
            nudMatingInterval.NeutralNumber = 1;
            nudMatingSpeed.NeutralNumber = 1;
            nudEggHatchSpeed.NeutralNumber = 1;
            nudBabyMatureSpeed.NeutralNumber = 1;
            nudBabyCuddleInterval.NeutralNumber = 1;
            nudBabyImprintAmount.NeutralNumber = 1;
            nudBabyImprintingStatScale.NeutralNumber = 1;
            nudBabyFoodConsumptionSpeed.NeutralNumber = 1;
            // event
            nudTamingSpeedEvent.NeutralNumber = 1.5M;
            nudDinoCharacterFoodDrainEvent.NeutralNumber = 1;
            nudMatingIntervalEvent.NeutralNumber = 1;
            nudEggHatchSpeedEvent.NeutralNumber = 1;
            nudBabyMatureSpeedEvent.NeutralNumber = 1;
            nudBabyCuddleIntervalEvent.NeutralNumber = 1;
            nudBabyImprintAmountEvent.NeutralNumber = 1;
            nudBabyFoodConsumptionSpeedEvent.NeutralNumber = 1;

            customSCStarving.Title = "Starving: ";
            customSCWakeup.Title = "Wakeup: ";
            customSCBirth.Title = "Birth: ";
            customSCCustom.Title = "Custom: ";

            fileSelectorExtractedSaveFolder.IsFile = false;

            Disposed += Settings_Disposed;
            LanguageChanged = false;

            // Tooltips
            _tt = new ToolTip();
            _tt.SetToolTip(NudBackupEveryMinutes, "If the value is 0 then every time something is changed a backup file is created.\nThis can create very similar backup files and potential data losses could be overwritten fast.\nA value of 5 is recommended.");
            _tt.SetToolTip(chkCollectionSync, "If checked, the tool automatically reloads the library if it was changed. Use this if multiple persons edit the file, e.g. via a shared folder.\nIt's recommended to check this along with \"Auto save\"");
            _tt.SetToolTip(checkBoxAutoSave, "If checked, the library is saved after each change automatically.\nIt's recommended to check this along with \"Auto load collection file\"");
            _tt.SetToolTip(nudMaxGraphLevel, "This number defines the level that is shown as maximum in the charts.\nUsually it's good to set this value to one third of the max wild level.");
            _tt.SetToolTip(labelTameAdd, "PerLevelStatsMultiplier_DinoTamed_Add");
            _tt.SetToolTip(labelTameAff, "PerLevelStatsMultiplier_DinoTamed_Affinity");
            _tt.SetToolTip(labelWildLevel, "PerLevelStatsMultiplier_DinoWild");
            _tt.SetToolTip(labelTameLevel, "PerLevelStatsMultiplier_DinoTamed");
            _tt.SetToolTip(chkbSpeechRecognition, "If the overlay is enabled, you can ask via the microphone for taming-infos,\ne.g.\"Argentavis level 30\" to display basic taming-infos in the overlay");
            _tt.SetToolTip(labelBabyFoodConsumptionSpeed, "BabyFoodConsumptionSpeedMultiplier");
            _tt.SetToolTip(checkBoxDisplayHiddenStats, "Enable if you have the oxygen-values of all creatures, e.g. by using a mod.");
            _tt.SetToolTip(labelEvent, "These values are used if the Event-Checkbox under the species-selector is selected.");
            _tt.SetToolTip(cbConsiderWildLevelSteps, "Enable to sort out all level-combinations that are not possible for naturally spawned creatures.\nThe step is max-wild-level / 30 by default, e.g. with a max wildlevel of 150, only creatures with levels that are a multiple of 5 are possible (can be different with mods).\nDisable if there are creatures that have other levels, e.g. spawned in by an admin.");
            _tt.SetToolTip(cbSingleplayerSettings, "Check this if you have enabled the \"Singleplayer-Settings\" in your game. This settings adjusts some of the multipliers again.");
            _tt.SetToolTip(cbAllowMoreThanHundredImprinting, "Enable this if on your server more than 100% imprinting are possible, e.g. with the mod S+ with a Nanny");
            _tt.SetToolTip(cbDevTools, "Shows extra tabs for multiplier-testing and extraction test-cases.");
            _tt.SetToolTip(nudMaxServerLevel, "The max level allowed on the server. Currently creatures with more than 450 levels will be deleted on official servers.\nA creature that can be potentially have a higher level than this (if maximally leveled up) will be marked with a orange-red text in the library.\nSet to 0 to disable a warning in the loaded library.");
            _tt.SetToolTip(lbMaxTotalLevel, "The max level allowed on the server. Currently creatures with more than 450 levels will be deleted on official servers.\nThis limit can be enabled on unoffical servers with the setting DestroyTamesOverLevelClamp.\nA creature in this library that can be potentially have a higher level than this (if maximally leveled up) will be marked with a orange-red text in the library.\nSet to 0 to disable a warning in the loaded library.");
            _tt.SetToolTip(CbExportFileRenameAfterImport, "Use a pattern to create the new file name, a subset of the keywords and functions from the naming pattern work.");
            _tt.SetToolTip(CbHighlightAdjustedMultipliers, "Highlight multipliers that are set to non-official values.\nDoes not update on multiplier change, this button needs to be rechecked then.\nCan be used to share screenshots of these settings.");

            // localizations / translations
            // for a new translation
            // * a file local/strings.[languageCode].resx needs to exist.
            // * the compiler created dll-file needs to be added to the installer files, for that edit the file setup.iss and setup-debug.iss in the repository base folder.
            // * the entry in the next dictionary needs to be added
            _languages = new Dictionary<string, string>
            {
                { Loc.S("SystemLanguage"), string.Empty},
                { "Deutsch", "de"},
                { "English", "en"},
                { "Español", "es"},
                { "Français", "fr"},
                { "Italiano", "it"},
                { "日本語", "ja"},
                { "Polski", "pl"},
                { "русский", "ru"},
                { "中文", "zh"}
            };
            foreach (string l in _languages.Keys)
                cbbLanguage.Items.Add(l);

            foreach (var cm in Enum.GetNames(typeof(ColorModeColors.AsbColorMode)))
                CbbColorMode.Items.Add(cm);
        }

        private void LoadSettings(CreatureCollection cc)
        {
            if (cc.serverMultipliers?.statMultipliers != null)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (s < cc.serverMultipliers.statMultipliers.Length && cc.serverMultipliers.statMultipliers[s].Length > 3)
                    {
                        _multSetter[s].Multipliers = cc.serverMultipliers.statMultipliers[s];
                    }
                    else _multSetter[s].Multipliers = null;
                }
            }
            cbSingleplayerSettings.Checked = cc.singlePlayerSettings;

            nudMaxDomLevels.ValueSave = cc.maxDomLevel;
            numericUpDownMaxBreedingSug.ValueSave = cc.maxBreedingSuggestions;
            nudMaxWildLevels.ValueSave = cc.maxWildLevel;
            nudMaxServerLevel.ValueSave = cc.maxServerLevel > 0 ? cc.maxServerLevel : 0;
            nudMaxGraphLevel.ValueSave = cc.maxChartLevel;
            CbAllowFlyerSpeedLeveling.Checked = cc.serverMultipliers?.AllowFlyerSpeedLeveling ?? false;
            #region Non-event multiplier
            var multipliers = cc.serverMultipliers;
            if (multipliers == null)
            {
                multipliers = new ServerMultipliers();
                multipliers.SetDefaultValues(new StreamingContext());
            }
            nudMatingSpeed.ValueSave = (decimal)multipliers.MatingSpeedMultiplier;
            nudMatingInterval.ValueSave = (decimal)multipliers.MatingIntervalMultiplier;
            nudEggHatchSpeed.ValueSave = (decimal)multipliers.EggHatchSpeedMultiplier;
            nudBabyMatureSpeed.ValueSave = (decimal)multipliers.BabyMatureSpeedMultiplier;
            nudBabyImprintingStatScale.ValueSave = (decimal)multipliers.BabyImprintingStatScaleMultiplier;
            nudBabyCuddleInterval.ValueSave = (decimal)multipliers.BabyCuddleIntervalMultiplier;
            nudBabyImprintAmount.ValueSave = (decimal)multipliers.BabyImprintAmountMultiplier;
            nudTamingSpeed.ValueSave = (decimal)multipliers.TamingSpeedMultiplier;
            nudDinoCharacterFoodDrain.ValueSave = (decimal)multipliers.DinoCharacterFoodDrainMultiplier;
            nudBabyFoodConsumptionSpeed.ValueSave = (decimal)multipliers.BabyFoodConsumptionSpeedMultiplier;
            #endregion
            #region event-multiplier
            multipliers = cc.serverMultipliersEvents ?? multipliers;
            nudBabyCuddleIntervalEvent.ValueSave = (decimal)multipliers.BabyCuddleIntervalMultiplier;
            nudBabyImprintAmountEvent.ValueSave = (decimal)multipliers.BabyImprintAmountMultiplier;
            nudTamingSpeedEvent.ValueSave = (decimal)multipliers.TamingSpeedMultiplier;
            nudDinoCharacterFoodDrainEvent.ValueSave = (decimal)multipliers.DinoCharacterFoodDrainMultiplier;
            nudMatingIntervalEvent.ValueSave = (decimal)multipliers.MatingIntervalMultiplier;
            nudEggHatchSpeedEvent.ValueSave = (decimal)multipliers.EggHatchSpeedMultiplier;
            nudBabyMatureSpeedEvent.ValueSave = (decimal)multipliers.BabyMatureSpeedMultiplier;
            nudBabyFoodConsumptionSpeedEvent.ValueSave = (decimal)multipliers.BabyFoodConsumptionSpeedMultiplier;
            #endregion

            checkBoxAutoSave.Checked = Properties.Settings.Default.autosave;
            chkCollectionSync.Checked = Properties.Settings.Default.syncCollection;
            NudWaitBeforeAutoLoad.ValueSave = Properties.Settings.Default.WaitBeforeAutoLoadMs;
            NudBackupEveryMinutes.ValueSave = Properties.Settings.Default.BackupEveryMinutes;
            NudKeepBackupFilesCount.ValueSave = Properties.Settings.Default.BackupFileCount;
            SetFolderSelectionButton(BtBackupFolder, Properties.Settings.Default.BackupFolder, true);

            chkbSpeechRecognition.Checked = Properties.Settings.Default.SpeechRecognition;
            if (Properties.Settings.Default.celsius) radioButtonCelsius.Checked = true;
            else radioButtonFahrenheit.Checked = true;
            cbIgnoreSexInBreedingPlan.Checked = Properties.Settings.Default.IgnoreSexInBreedingPlan;
            checkBoxDisplayHiddenStats.Checked = Properties.Settings.Default.DisplayHiddenStats;
            tbDefaultFontName.Text = Properties.Settings.Default.DefaultFontName;
            nudDefaultFontSize.Value = (decimal)Properties.Settings.Default.DefaultFontSize;

            GbImgCacheLocalAppData.Visible = !Updater.Updater.IsProgramInstalled; // setting is only relevant for portable app
            CbImgCacheUseLocalAppData.Checked = Properties.Settings.Default.ImgCacheUseLocalAppData || Updater.Updater.IsProgramInstalled;

            #region overlay
            nudOverlayInfoDuration.ValueSave = Properties.Settings.Default.OverlayInfoDuration;
            nudOverlayTimerPosX.ValueSave = Properties.Settings.Default.OverlayTimerPosition.X;
            nudOverlayTimerPosY.ValueSave = Properties.Settings.Default.OverlayTimerPosition.Y;
            nudOverlayInfoPosDFR.ValueSave = Properties.Settings.Default.OverlayInfoPosition.X;
            nudOverlayInfoPosY.ValueSave = Properties.Settings.Default.OverlayInfoPosition.Y;
            cbCustomOverlayLocation.Checked = Properties.Settings.Default.UseCustomOverlayLocation;
            nudCustomOverlayLocX.ValueSave = Properties.Settings.Default.CustomOverlayLocation.X;
            nudCustomOverlayLocY.ValueSave = Properties.Settings.Default.CustomOverlayLocation.Y;
            CbOverlayDisplayInheritance.Checked = Properties.Settings.Default.DisplayInheritanceInOverlay;
            #endregion

            #region Timers
            cbTimersInOverlayAutomatically.Checked = Properties.Settings.Default.DisplayTimersInOverlayAutomatically;
            cbKeepExpiredTimersInOverlay.Checked = Properties.Settings.Default.KeepExpiredTimersInOverlay;
            cbDeleteExpiredTimersOnSaving.Checked = Properties.Settings.Default.DeleteExpiredTimersOnSaving;
            #endregion

            #region OCR
            cbShowOCRButton.Checked = Properties.Settings.Default.showOCRButton;
            nudWaitBeforeScreenCapture.ValueSave = Properties.Settings.Default.waitBeforeScreenCapture;
            nudWhiteThreshold.ValueSave = Properties.Settings.Default.OCRWhiteThreshold;
            tbOCRCaptureApp.Text = Properties.Settings.Default.OCRApp;

            cbOCRIgnoreImprintValue.Checked = Properties.Settings.Default.OCRIgnoresImprintValue;
            #endregion

            customSCStarving.SoundFile = Properties.Settings.Default.soundStarving;
            customSCWakeup.SoundFile = Properties.Settings.Default.soundWakeup;
            customSCBirth.SoundFile = Properties.Settings.Default.soundBirth;
            customSCCustom.SoundFile = Properties.Settings.Default.soundCustom;

            tbPlayAlarmsSeconds.Text = Properties.Settings.Default.playAlarmTimes;

            cbConsiderWildLevelSteps.Checked = cc.considerWildLevelSteps;
            nudWildLevelStep.ValueSave = cc.wildLevelStep;
            cbInventoryCheck.Checked = Properties.Settings.Default.inventoryCheckTimer;
            cbAllowMoreThanHundredImprinting.Checked = cc.allowMoreThanHundredImprinting;
            CbHighlightLevel255.Checked = Properties.Settings.Default.Highlight255Level;
            CbHighlightLevelEvenOdd.Checked = Properties.Settings.Default.HighlightEvenOdd;

            #region InfoGraphic

            nudInfoGraphicWidth.ValueSave = Properties.Settings.Default.InfoGraphicWidth;
            CbInfoGraphicDisplayMaxWildLevel.Checked = Properties.Settings.Default.InfoGraphicShowMaxWildLevel;
            CbInfoGraphicDomLevels.Checked = Properties.Settings.Default.InfoGraphicWithDomLevels;
            TbInfoGraphicFontName.Text = Properties.Settings.Default.InfoGraphicFontName;
            CbInfoGraphicMutations.Checked = Properties.Settings.Default.InfoGraphicDisplayMutations;
            CbInfoGraphicGenerations.Checked = Properties.Settings.Default.InfoGraphicDisplayGeneration;
            BtInfoGraphicBackColor.SetBackColorAndAccordingForeColor(Properties.Settings.Default.InfoGraphicBackColor);
            BtInfoGraphicForeColor.SetBackColorAndAccordingForeColor(Properties.Settings.Default.InfoGraphicForeColor);
            BtInfoGraphicBorderColor.SetBackColorAndAccordingForeColor(Properties.Settings.Default.InfoGraphicBorderColor);

            #endregion

            #region library

            CbPauseGrowingTimerAfterAdding.Checked = Properties.Settings.Default.PauseGrowingTimerAfterAddingBaby;
            cbCreatureColorsLibrary.Checked = Properties.Settings.Default.showColorsInLibrary;
            cbApplyGlobalSpeciesToLibrary.Checked = Properties.Settings.Default.ApplyGlobalSpeciesToLibrary;
            CbLibrarySelectSelectedSpeciesOnLoad.Checked = Properties.Settings.Default.LibrarySelectSelectedSpeciesOnLoad;
            cbLibraryHighlightTopCreatures.Checked = Properties.Settings.Default.LibraryHighlightTopCreatures;
            CbConsiderWastedStatsForTopCreatures.Checked = Properties.Settings.Default.ConsiderWastedStatsForTopCreatures;

            #endregion

            #region import exported
            if (Properties.Settings.Default.ExportCreatureFolders != null)
            {
                foreach (string path in Properties.Settings.Default.ExportCreatureFolders)
                {
                    aTExportFolderLocationsBindingSource.Add(ATImportExportedFolderLocation.CreateFromString(path));
                }

            }
            dataGridViewExportFolders.DataBindingComplete += (s, e) => HighlightDefaultImportExportFolderEntry();
            nudWarnImportMoreThan.Value = Properties.Settings.Default.WarnWhenImportingMoreCreaturesThan;
            CbApplyNamingPatternOnImportAlways.Checked = Properties.Settings.Default.applyNamePatternOnAutoImportAlways;
            cbApplyNamePatternOnImportOnEmptyNames.Checked = Properties.Settings.Default.applyNamePatternOnImportIfEmptyName;
            cbApplyNamePatternOnImportOnNewCreatures.Checked = Properties.Settings.Default.applyNamePatternOnAutoImportForNewCreatures;
            cbCopyPatternNameToClipboard.Checked = Properties.Settings.Default.copyNameToClipboardOnImportWhenAutoNameApplied;
            cbAutoImportExported.Checked = Properties.Settings.Default.AutoImportExportedCreatures;
            cbPlaySoundOnAutomaticImport.Checked = Properties.Settings.Default.PlaySoundOnAutoImport;
            cbMoveImportedFileToSubFolder.Checked = Properties.Settings.Default.MoveAutoImportedFileToSubFolder;
            SetFolderSelectionButton(BtImportArchiveFolder, Properties.Settings.Default.ImportExportedArchiveFolder);
            cbDeleteAutoImportedFile.Checked = Properties.Settings.Default.DeleteAutoImportedFile;
            CbExportFileRenameAfterImport.Checked = Properties.Settings.Default.AutoImportedExportFileRename;
            TbExportFileRename.Text = Properties.Settings.Default.AutoImportedExportFileRenamePattern;
            CbAutoImportSuccessGotoLibrary.Checked = Properties.Settings.Default.AutoImportGotoLibraryAfterSuccess;
            nudImportLowerBoundTE.ValueSave = (decimal)Properties.Settings.Default.ImportLowerBoundTE * 100;
            if (Properties.Settings.Default.ImportExportUseTamerStringForOwner)
                RbTamerStringForOwner.Checked = true;
            else
                RbTamerStringForTribe.Checked = true;
            #endregion

            #region import savegame
            if (Properties.Settings.Default.arkSavegamePaths != null)
            {
                foreach (string path in Properties.Settings.Default.arkSavegamePaths)
                {
                    aTImportFileLocationBindingSource.Add(ATImportFileLocation.CreateFromString(path));
                }
            }
            fileSelectorExtractedSaveFolder.Link = Properties.Settings.Default.savegameExtractionPath;

            cbImportUpdateCreatureStatus.Checked = cc.changeCreatureStatusOnSavegameImport;
            textBoxImportTribeNameFilter.Text = Properties.Settings.Default.ImportTribeNameFilter;
            cbIgnoreUnknownBPOnSaveImport.Checked = Properties.Settings.Default.IgnoreUnknownBlueprintsOnSaveImport;
            cbSaveImportCryo.Checked = Properties.Settings.Default.SaveImportCryo;
            CbImportUnclaimedBabies.Checked = Properties.Settings.Default.SaveFileImportUnclaimedBabies;
            #endregion

            #region Export for spreadsheet

            var exportFields = Properties.Settings.Default.CreatureTableExportFields;
            if (exportFields != null)
            {
                foreach (ExportCreatures.TableExportFields f in exportFields)
                    ClbExportSpreadsheetFields.Items.Add(f, true);
            }

            foreach (ExportCreatures.TableExportFields f in Enum.GetValues(typeof(ExportCreatures.TableExportFields)))
            {
                if (exportFields?.Contains((int)f) ?? false) continue;
                ClbExportSpreadsheetFields.Items.Add(f, false);
            }

            #endregion

            NudSpeciesSelectorCountLastUsed.ValueSave = Properties.Settings.Default.SpeciesSelectorCountLastSpecies;

            cbDevTools.Checked = Properties.Settings.Default.DevTools;

            cbPrettifyJSON.Checked = Properties.Settings.Default.prettifyCollectionJson;

            cbAdminConsoleCommandWithCheat.Checked = Properties.Settings.Default.AdminConsoleCommandWithCheat;

            string langKey = _languages.FirstOrDefault(x => x.Value == Properties.Settings.Default.language).Key ?? string.Empty;
            int langI = cbbLanguage.Items.IndexOf(langKey);
            cbbLanguage.SelectedIndex = langI == -1 ? 0 : langI;

            CbbColorMode.SelectedIndex = Math.Min(CbbColorMode.Items.Count, Math.Max(0, Properties.Settings.Default.ColorMode));
        }

        private void SaveSettings()
        {
            if (_cc.serverMultipliers == null)
            {
                _cc.serverMultipliers = new ServerMultipliers();
            }
            if (_cc.serverMultipliers.statMultipliers == null)
            {
                _cc.serverMultipliers.statMultipliers = new double[Values.STATS_COUNT][];
            }
            if (_cc.serverMultipliers?.statMultipliers != null)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (_cc.serverMultipliers.statMultipliers[s] == null)
                        _cc.serverMultipliers.statMultipliers[s] = new double[4];
                    for (int sm = 0; sm < 4; sm++)
                        _cc.serverMultipliers.statMultipliers[s][sm] = _multSetter[s].Multipliers[sm];
                }
            }

            // Torpidity is handled differently by the game, IwM has no effect. Set IwM to 1.
            // Also see https://github.com/cadon/ARKStatsExtractor/issues/942 for more infos about this.
            _cc.serverMultipliers.statMultipliers[(int)species.StatNames.Torpidity][3] = 1;

            _cc.singlePlayerSettings = cbSingleplayerSettings.Checked;
            _cc.maxDomLevel = (int)nudMaxDomLevels.Value;
            _cc.maxWildLevel = (int)nudMaxWildLevels.Value;
            _cc.maxServerLevel = (int)nudMaxServerLevel.Value;
            _cc.maxChartLevel = (int)nudMaxGraphLevel.Value;
            _cc.serverMultipliers.AllowFlyerSpeedLeveling = CbAllowFlyerSpeedLeveling.Checked;
            _cc.maxBreedingSuggestions = (int)numericUpDownMaxBreedingSug.Value;
            Properties.Settings.Default.IgnoreSexInBreedingPlan = cbIgnoreSexInBreedingPlan.Checked;

            #region non-event-multiplier
            _cc.serverMultipliers.TamingSpeedMultiplier = (double)nudTamingSpeed.Value;
            _cc.serverMultipliers.DinoCharacterFoodDrainMultiplier = (double)nudDinoCharacterFoodDrain.Value;
            _cc.serverMultipliers.MatingSpeedMultiplier = (double)nudMatingSpeed.Value;
            _cc.serverMultipliers.MatingIntervalMultiplier = (double)nudMatingInterval.Value;
            _cc.serverMultipliers.EggHatchSpeedMultiplier = (double)nudEggHatchSpeed.Value;
            _cc.serverMultipliers.BabyCuddleIntervalMultiplier = (double)nudBabyCuddleInterval.Value;
            _cc.serverMultipliers.BabyImprintAmountMultiplier = (double)nudBabyImprintAmount.Value;
            _cc.serverMultipliers.BabyImprintingStatScaleMultiplier = (double)nudBabyImprintingStatScale.Value;
            _cc.serverMultipliers.BabyMatureSpeedMultiplier = (double)nudBabyMatureSpeed.Value;
            _cc.serverMultipliers.BabyFoodConsumptionSpeedMultiplier = (double)nudBabyFoodConsumptionSpeed.Value;
            #endregion

            #region event-multiplier
            if (_cc.serverMultipliersEvents == null) _cc.serverMultipliersEvents = new ServerMultipliers();
            _cc.serverMultipliersEvents.TamingSpeedMultiplier = (double)nudTamingSpeedEvent.Value;
            _cc.serverMultipliersEvents.DinoCharacterFoodDrainMultiplier = (double)nudDinoCharacterFoodDrainEvent.Value;
            _cc.serverMultipliersEvents.MatingIntervalMultiplier = (double)nudMatingIntervalEvent.Value;
            _cc.serverMultipliersEvents.EggHatchSpeedMultiplier = (double)nudEggHatchSpeedEvent.Value;
            _cc.serverMultipliersEvents.BabyCuddleIntervalMultiplier = (double)nudBabyCuddleIntervalEvent.Value;
            _cc.serverMultipliersEvents.BabyImprintAmountMultiplier = (double)nudBabyImprintAmountEvent.Value;
            _cc.serverMultipliersEvents.BabyImprintingStatScaleMultiplier = (double)nudBabyImprintingStatScale.Value;
            _cc.serverMultipliersEvents.BabyMatureSpeedMultiplier = (double)nudBabyMatureSpeedEvent.Value;
            _cc.serverMultipliersEvents.BabyFoodConsumptionSpeedMultiplier = (double)nudBabyFoodConsumptionSpeedEvent.Value;
            #endregion

            Properties.Settings.Default.autosave = checkBoxAutoSave.Checked;
            Properties.Settings.Default.syncCollection = chkCollectionSync.Checked;
            Properties.Settings.Default.WaitBeforeAutoLoadMs = (int)NudWaitBeforeAutoLoad.Value;
            Properties.Settings.Default.BackupEveryMinutes = (int)NudBackupEveryMinutes.Value;
            Properties.Settings.Default.BackupFileCount = (int)NudKeepBackupFilesCount.Value;
            Properties.Settings.Default.BackupFolder = BtBackupFolder.Tag as string;

            Properties.Settings.Default.SpeechRecognition = chkbSpeechRecognition.Checked;
            Properties.Settings.Default.celsius = radioButtonCelsius.Checked;
            Properties.Settings.Default.DisplayHiddenStats = checkBoxDisplayHiddenStats.Checked;
            Properties.Settings.Default.DefaultFontName = tbDefaultFontName.Text;
            Properties.Settings.Default.DefaultFontSize = (float)nudDefaultFontSize.Value;

            Properties.Settings.Default.ImgCacheUseLocalAppData = CbImgCacheUseLocalAppData.Checked;

            #region overlay
            Properties.Settings.Default.OverlayInfoDuration = (int)nudOverlayInfoDuration.Value;
            Properties.Settings.Default.OverlayTimerPosition = new Point((int)nudOverlayTimerPosX.Value, (int)nudOverlayTimerPosY.Value);
            Properties.Settings.Default.OverlayInfoPosition = new Point((int)nudOverlayInfoPosDFR.Value, (int)nudOverlayInfoPosY.Value);
            Properties.Settings.Default.UseCustomOverlayLocation = cbCustomOverlayLocation.Checked;
            Properties.Settings.Default.CustomOverlayLocation = new Point((int)nudCustomOverlayLocX.Value, (int)nudCustomOverlayLocY.Value);
            Properties.Settings.Default.DisplayInheritanceInOverlay = CbOverlayDisplayInheritance.Checked;
            #endregion

            #region Timers
            Properties.Settings.Default.DisplayTimersInOverlayAutomatically = cbTimersInOverlayAutomatically.Checked;
            Properties.Settings.Default.KeepExpiredTimersInOverlay = cbKeepExpiredTimersInOverlay.Checked;
            Properties.Settings.Default.DeleteExpiredTimersOnSaving = cbDeleteExpiredTimersOnSaving.Checked;
            #endregion

            #region OCR
            Properties.Settings.Default.showOCRButton = cbShowOCRButton.Checked;
            Properties.Settings.Default.waitBeforeScreenCapture = (int)nudWaitBeforeScreenCapture.Value;
            Properties.Settings.Default.OCRWhiteThreshold = (byte)nudWhiteThreshold.Value;
            Properties.Settings.Default.OCRApp = tbOCRCaptureApp.Text;

            Properties.Settings.Default.OCRIgnoresImprintValue = cbOCRIgnoreImprintValue.Checked;
            #endregion

            Properties.Settings.Default.soundStarving = customSCStarving.SoundFile;
            Properties.Settings.Default.soundWakeup = customSCWakeup.SoundFile;
            Properties.Settings.Default.soundBirth = customSCBirth.SoundFile;
            Properties.Settings.Default.soundCustom = customSCCustom.SoundFile;

            Properties.Settings.Default.playAlarmTimes = tbPlayAlarmsSeconds.Text;

            _cc.considerWildLevelSteps = cbConsiderWildLevelSteps.Checked;
            _cc.wildLevelStep = (int)nudWildLevelStep.Value;
            Properties.Settings.Default.inventoryCheckTimer = cbInventoryCheck.Checked;
            _cc.allowMoreThanHundredImprinting = cbAllowMoreThanHundredImprinting.Checked;
            Properties.Settings.Default.Highlight255Level = CbHighlightLevel255.Checked;
            Properties.Settings.Default.HighlightEvenOdd = CbHighlightLevelEvenOdd.Checked;

            #region InfoGraphic

            Properties.Settings.Default.InfoGraphicWidth = (int)nudInfoGraphicWidth.Value;
            Properties.Settings.Default.InfoGraphicShowMaxWildLevel = CbInfoGraphicDisplayMaxWildLevel.Checked;
            Properties.Settings.Default.InfoGraphicWithDomLevels = CbInfoGraphicDomLevels.Checked;
            Properties.Settings.Default.InfoGraphicFontName = TbInfoGraphicFontName.Text;
            Properties.Settings.Default.InfoGraphicDisplayMutations = CbInfoGraphicMutations.Checked;
            Properties.Settings.Default.InfoGraphicDisplayGeneration = CbInfoGraphicGenerations.Checked;
            Properties.Settings.Default.InfoGraphicBackColor = BtInfoGraphicBackColor.BackColor;
            Properties.Settings.Default.InfoGraphicForeColor = BtInfoGraphicForeColor.BackColor;
            Properties.Settings.Default.InfoGraphicBorderColor = BtInfoGraphicBorderColor.BackColor;

            #endregion

            #region library

            Properties.Settings.Default.PauseGrowingTimerAfterAddingBaby = CbPauseGrowingTimerAfterAdding.Checked;
            Properties.Settings.Default.showColorsInLibrary = cbCreatureColorsLibrary.Checked;
            Properties.Settings.Default.ApplyGlobalSpeciesToLibrary = cbApplyGlobalSpeciesToLibrary.Checked;
            Properties.Settings.Default.LibrarySelectSelectedSpeciesOnLoad = CbLibrarySelectSelectedSpeciesOnLoad.Checked;
            Properties.Settings.Default.LibraryHighlightTopCreatures = cbLibraryHighlightTopCreatures.Checked;
            Properties.Settings.Default.ConsiderWastedStatsForTopCreatures = CbConsiderWastedStatsForTopCreatures.Checked;

            #endregion

            #region import exported
            Properties.Settings.Default.WarnWhenImportingMoreCreaturesThan = (int)nudWarnImportMoreThan.Value;
            Properties.Settings.Default.ExportCreatureFolders = aTExportFolderLocationsBindingSource.OfType<ATImportExportedFolderLocation>()
                    .Where(location => !string.IsNullOrWhiteSpace(location.FolderPath))
                    .Select(location => $"{location.ConvenientName}|{location.OwnerSuffix}|{location.FolderPath}").ToArray();

            Properties.Settings.Default.applyNamePatternOnAutoImportAlways = CbApplyNamingPatternOnImportAlways.Checked;
            Properties.Settings.Default.applyNamePatternOnImportIfEmptyName = cbApplyNamePatternOnImportOnEmptyNames.Checked;
            Properties.Settings.Default.applyNamePatternOnAutoImportForNewCreatures = cbApplyNamePatternOnImportOnNewCreatures.Checked;
            Properties.Settings.Default.copyNameToClipboardOnImportWhenAutoNameApplied = cbCopyPatternNameToClipboard.Checked;
            Properties.Settings.Default.AutoImportExportedCreatures = cbAutoImportExported.Checked;
            Properties.Settings.Default.PlaySoundOnAutoImport = cbPlaySoundOnAutomaticImport.Checked;
            Properties.Settings.Default.MoveAutoImportedFileToSubFolder = cbMoveImportedFileToSubFolder.Checked;
            Properties.Settings.Default.ImportExportedArchiveFolder = BtImportArchiveFolder.Tag as string;
            Properties.Settings.Default.DeleteAutoImportedFile = cbDeleteAutoImportedFile.Checked;
            Properties.Settings.Default.AutoImportedExportFileRename = CbExportFileRenameAfterImport.Checked;
            Properties.Settings.Default.AutoImportedExportFileRenamePattern = TbExportFileRename.Text;
            Properties.Settings.Default.AutoImportGotoLibraryAfterSuccess = CbAutoImportSuccessGotoLibrary.Checked;
            Properties.Settings.Default.ImportLowerBoundTE = (double)nudImportLowerBoundTE.Value / 100;

            _cc.changeCreatureStatusOnSavegameImport = cbImportUpdateCreatureStatus.Checked;
            Properties.Settings.Default.ImportTribeNameFilter = textBoxImportTribeNameFilter.Text;
            Properties.Settings.Default.ImportExportUseTamerStringForOwner = RbTamerStringForOwner.Checked;
            #endregion

            #region import savegame
            Properties.Settings.Default.savegameExtractionPath = fileSelectorExtractedSaveFolder.Link;
            Properties.Settings.Default.arkSavegamePaths = aTImportFileLocationBindingSource.OfType<ATImportFileLocation>()
                    .Where(location => !string.IsNullOrWhiteSpace(location.FileLocation))
                    .Select(location => location.ToString()).ToArray();

            Properties.Settings.Default.IgnoreUnknownBlueprintsOnSaveImport = cbIgnoreUnknownBPOnSaveImport.Checked;
            Properties.Settings.Default.SaveImportCryo = cbSaveImportCryo.Checked;
            Properties.Settings.Default.SaveFileImportUnclaimedBabies = CbImportUnclaimedBabies.Checked;
            #endregion

            #region Export for spreadsheet

            var exportFields = new List<int>();
            var exportFieldCount = ClbExportSpreadsheetFields.Items.Count;
            for (int i = 0; i < exportFieldCount; i++)
            {
                if (ClbExportSpreadsheetFields.GetItemChecked(i))
                    exportFields.Add((int)Enum.Parse(typeof(ExportCreatures.TableExportFields), ClbExportSpreadsheetFields.Items[i].ToString()));
            }
            Properties.Settings.Default.CreatureTableExportFields = exportFields.ToArray();

            #endregion

            Properties.Settings.Default.SpeciesSelectorCountLastSpecies = (int)NudSpeciesSelectorCountLastUsed.Value;

            Properties.Settings.Default.DevTools = cbDevTools.Checked;

            Properties.Settings.Default.prettifyCollectionJson = cbPrettifyJSON.Checked;

            Properties.Settings.Default.AdminConsoleCommandWithCheat = cbAdminConsoleCommandWithCheat.Checked;

            string oldLanguageSetting = Properties.Settings.Default.language;
            string lang = cbbLanguage.SelectedItem.ToString();
            Properties.Settings.Default.language = _languages.ContainsKey(lang) ? _languages[lang] : string.Empty;
            LanguageChanged = oldLanguageSetting != Properties.Settings.Default.language;

            Properties.Settings.Default.ColorMode = Math.Max(0, CbbColorMode.SelectedIndex);

            Properties.Settings.Default.Save();
        }

        private void btAddSavegameFileLocation_Click(object sender, EventArgs e)
        {
            ATImportFileLocation atImportFileLocation = EditFileLocation(new ATImportFileLocation());
            if (atImportFileLocation != null)
            {
                aTImportFileLocationBindingSource.Add(atImportFileLocation);
                CheckSaveImportPath(atImportFileLocation.FileLocation);
            }
        }

        private void CheckSaveImportPath(string filePath)
        {
            if (!filePath.EndsWith(".ark"))
            {
                MessageBoxes.ShowMessageBox($"The file location must include the path and the filename of the save file. The set path\n{filePath}\ndoesn't end with \".ark\" and seems to miss the file name.", "Possibly wrong path", MessageBoxIcon.Warning);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void tabPage2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)
                || e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
        }

        private void tabPage2_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files) ExtractSettingsFromFile(file);
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                ExtractSettingsFromText(e.Data.GetData(DataFormats.Text) as string);
            }
        }

        private void ExtractSettingsFromFile(string file)
        {
            if (!File.Exists(file))
                return;

            ExtractSettingsFromText(File.ReadAllText(file));
        }

        private void ExtractSettingsFromText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            // ignore lines that start with a semicolon (comments)
            text = Regex.Replace(text, @"(?:\A|[\r\n]+);[^\r\n]*", string.Empty);

            double d;
            Match m;
            var cultureForStrings = System.Globalization.CultureInfo.GetCultureInfo("en-US");

            // get stat-multipliers
            // if there are stat-multipliers, set all to the official-values first
            if (text.Contains("PerLevelStatsMultiplier_Dino"))
                ApplyMultiplierPreset(Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.Official),
                    onlyStatMultipliers: true);

            // if an ini file is imported the server is most likely unofficial wit no level cap, if the server has a max level, it will be parsed.
            nudMaxServerLevel.ValueSave = 0;

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                ParseAndSetStatMultiplier(0, @"PerLevelStatsMultiplier_DinoTamed_Add\[" + s + @"\] ?= ?(\d*\.?\d+)");
                ParseAndSetStatMultiplier(1,
                    @"PerLevelStatsMultiplier_DinoTamed_Affinity\[" + s + @"\] ?= ?(\d*\.?\d+)");
                ParseAndSetStatMultiplier(2, @"PerLevelStatsMultiplier_DinoTamed\[" + s + @"\] ?= ?(\d*\.?\d+)");
                ParseAndSetStatMultiplier(3, @"PerLevelStatsMultiplier_DinoWild\[" + s + @"\] ?= ?(\d*\.?\d+)");

                void ParseAndSetStatMultiplier(int multiplierIndex, string regexPattern)
                {
                    m = Regex.Match(text, regexPattern);
                    if (m.Success && double.TryParse(m.Groups[1].Value,
                        System.Globalization.NumberStyles.AllowDecimalPoint, cultureForStrings, out d))
                    {
                        var multipliers = _multSetter[s].Multipliers;
                        multipliers[multiplierIndex] = d == 0 ? 1 : d;
                        _multSetter[s].Multipliers = multipliers;
                    }
                }
            }

            // breeding
            ParseAndSetValue(nudMatingInterval, @"MatingIntervalMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudEggHatchSpeed, @"EggHatchSpeedMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudMatingSpeed, @"MatingSpeedMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudBabyMatureSpeed, @"BabyMatureSpeedMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudBabyImprintingStatScale, @"BabyImprintingStatScaleMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudBabyCuddleInterval, @"BabyCuddleIntervalMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudBabyFoodConsumptionSpeed, @"BabyFoodConsumptionSpeedMultiplier ?= ?(\d*\.?\d+)");

            ParseAndSetCheckbox(cbSingleplayerSettings, @"bUseSingleplayerSettings ?= ?(true|false)");
            ParseAndSetValue(nudMaxServerLevel, @"DestroyTamesOverLevelClamp ?= ?(\d+)");

            // GameUserSettings.ini
            ParseAndSetValue(nudTamingSpeed, @"TamingSpeedMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudDinoCharacterFoodDrain, @"DinoCharacterFoodDrainMultiplier ?= ?(\d*\.?\d+)");

            //// the settings below don't appear in ARK server config files directly or not at all and are used only in ASB
            // max levels
            ParseAndSetValue(nudMaxWildLevels, @"ASBMaxWildLevels_Dinos ?= ?(\d+)");
            ParseAndSetValue(nudMaxDomLevels, @"ASBMaxDomLevels_Dinos ?= ?(\d+)");
            ParseAndSetValue(nudMaxGraphLevel, @"ASBMaxGraphLevels ?= ?(\d+)");
            // extractor
            if (ParseAndSetValue(nudWildLevelStep, @"ASBExtractorWildLevelSteps ?= ?(\d+)"))
                cbConsiderWildLevelSteps.Checked = nudWildLevelStep.Value != 1;
            ParseAndSetCheckbox(cbAllowMoreThanHundredImprinting, @"ASBAllowHyperImprinting ?= ?(true|false)");

            // event multipliers breeding
            ParseAndSetValue(nudMatingIntervalEvent, @"ASBEvent_MatingIntervalMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudEggHatchSpeedEvent, @"ASBEvent_EggHatchSpeedMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudBabyMatureSpeedEvent, @"ASBEvent_BabyMatureSpeedMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudBabyCuddleIntervalEvent, @"ASBEvent_BabyCuddleIntervalMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudBabyFoodConsumptionSpeedEvent,
                @"ASBEvent_BabyFoodConsumptionSpeedMultiplier ?= ?(\d*\.?\d+)");
            // event multipliers taming
            ParseAndSetValue(nudTamingSpeedEvent, @"ASBEvent_TamingSpeedMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudDinoCharacterFoodDrainEvent,
                @"ASBEvent_DinoCharacterFoodDrainMultiplier ?= ?(\d*\.?\d+)");

            bool ParseAndSetValue(uiControls.Nud nud, string regexPattern)
            {
                m = Regex.Match(text, regexPattern);
                if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint,
                    cultureForStrings, out d))
                {
                    nud.ValueSave = (decimal)d;
                    return true;
                }

                return false;
            }

            void ParseAndSetCheckbox(CheckBox cb, string regexPattern)
            {
                m = Regex.Match(text, regexPattern, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    cb.Checked = m.Groups[1].Value.ToLower() == "true";
                }
            }

            // parse max dino levels. First occurrence is for the player, second for the creatures
            var regexMaxLevelup =
                new Regex(@"LevelExperienceRampOverrides.*LevelExperienceRampOverrides ?= ?\(([^\)]+)",
                    RegexOptions.Singleline);
            m = regexMaxLevelup.Match(text);
            if (m.Success)
                nudMaxDomLevels.ValueSave = Regex.Matches(m.Groups[1].Value, "ExperiencePointsForLevel").Count + 1;

            // parse max wild dino levels
            if (text.Contains("DifficultyOffset") || text.Contains("OverrideOfficialDifficulty"))
            {
                // default values
                var difficultyOffset = 0.2;
                var officialDifficulty = 5d;

                m = Regex.Match(text, @"DifficultyOffset ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint,
                    cultureForStrings, out d))
                    difficultyOffset = d;

                m = Regex.Match(text, @"OverrideOfficialDifficulty ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint,
                    cultureForStrings, out d))
                    officialDifficulty = d;

                var difficultyValue = 1d;
                if (difficultyOffset > 0)
                {
                    difficultyValue = difficultyOffset * (officialDifficulty - 0.5) + 0.5;
                }

                nudMaxWildLevels.ValueSave = (int)(difficultyValue * 30);
            }
        }

        private void Settings_Disposed(object sender, EventArgs e)
        {
            _tt.RemoveAll();
            _tt.Dispose();
        }

        private void buttonEventToDefault_Click(object sender, EventArgs e)
        {
            nudTamingSpeedEvent.ValueSave = nudTamingSpeed.Value;
            nudDinoCharacterFoodDrainEvent.ValueSave = nudDinoCharacterFoodDrain.Value;
            nudMatingIntervalEvent.ValueSave = nudMatingInterval.Value;
            nudEggHatchSpeedEvent.ValueSave = nudEggHatchSpeed.Value;
            nudBabyMatureSpeedEvent.ValueSave = nudBabyMatureSpeed.Value;
            nudBabyCuddleIntervalEvent.ValueSave = nudBabyCuddleInterval.Value;
            nudBabyFoodConsumptionSpeedEvent.ValueSave = nudBabyFoodConsumptionSpeed.Value;
        }

        private void btAddExportFolder_Click(object sender, EventArgs e)
        {
            ATImportExportedFolderLocation aTImportExportedFolderLocation = EditFolderLocation(new ATImportExportedFolderLocation());
            if (aTImportExportedFolderLocation != null)
            {
                aTExportFolderLocationsBindingSource.Add(aTImportExportedFolderLocation);
            }
        }

        private void dataGridView_FileLocations_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.RowIndex >= aTImportFileLocationBindingSource.Count)
            {
                return;
            }

            if (e.ColumnIndex == dgvFileLocation_Change.Index)
            {
                ATImportFileLocation atImportFileLocation = EditFileLocation((ATImportFileLocation)aTImportFileLocationBindingSource[e.RowIndex]);
                if (atImportFileLocation != null)
                {
                    aTImportFileLocationBindingSource[e.RowIndex] = atImportFileLocation;
                    CheckSaveImportPath(atImportFileLocation.FileLocation);
                }
            }
            else if (e.ColumnIndex == dgvFileLocation_Delete.Index)
            {
                aTImportFileLocationBindingSource.RemoveAt(e.RowIndex);
            }
        }

        private void dataGridViewExportFolders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.RowIndex >= aTExportFolderLocationsBindingSource.Count)
            {
                return;
            }

            if (e.ColumnIndex == dgvExportFolderChange.Index)
            {
                ATImportExportedFolderLocation aTImportExportedFolderLocation = EditFolderLocation((ATImportExportedFolderLocation)aTExportFolderLocationsBindingSource[e.RowIndex]);
                if (aTImportExportedFolderLocation != null)
                {
                    aTExportFolderLocationsBindingSource[e.RowIndex] = aTImportExportedFolderLocation;
                }
            }
            else if (e.ColumnIndex == dgvExportFolderDelete.Index)
            {
                aTExportFolderLocationsBindingSource.RemoveAt(e.RowIndex);
            }
            else if (e.ColumnIndex == dgvExportMakeDefault.Index)
            {
                if (e.RowIndex != 0)
                {
                    var r = aTExportFolderLocationsBindingSource[e.RowIndex];
                    aTExportFolderLocationsBindingSource.RemoveAt(e.RowIndex);
                    aTExportFolderLocationsBindingSource.Insert(0, r);
                }
            }
        }

        private static ATImportFileLocation EditFileLocation(ATImportFileLocation atImportFileLocation)
        {
            ATImportFileLocation atifl = null;
            using (ATImportFileLocationDialog atImportFileLocationDialog = new ATImportFileLocationDialog(atImportFileLocation))
            {
                if (atImportFileLocationDialog.ShowDialog() == DialogResult.OK &&
                        !string.IsNullOrWhiteSpace(atImportFileLocationDialog.AtImportFileLocation.FileLocation))
                    atifl = atImportFileLocationDialog.AtImportFileLocation;
            }
            return atifl;
        }

        private static ATImportExportedFolderLocation EditFolderLocation(ATImportExportedFolderLocation atExportFolderLocation)
        {
            ATImportExportedFolderLocation atiefl = null;
            using (ATImportExportedFolderLocationDialog aTImportExportedFolderLocationDialog = new ATImportExportedFolderLocationDialog(atExportFolderLocation))
            {
                if (aTImportExportedFolderLocationDialog.ShowDialog() == DialogResult.OK &&
                                        !string.IsNullOrWhiteSpace(aTImportExportedFolderLocationDialog.ATImportExportedFolderLocation.FolderPath))
                    atiefl = aTImportExportedFolderLocationDialog.ATImportExportedFolderLocation;
            }
            return atiefl;
        }

        /// <summary>
        /// Displays the server multiplier presets in a combobox
        /// </summary>
        private void DisplayServerMultiplierPresets()
        {
            cbbStatMultiplierPresets.Items.Clear();
            foreach (var sm in Values.V.serverMultipliersPresets.PresetNameList)
            {
                if (!string.IsNullOrWhiteSpace(sm) && sm != ServerMultipliersPresets.Singleplayer)
                    cbbStatMultiplierPresets.Items.Add(sm);
            }
            if (cbbStatMultiplierPresets.Items.Count > 0)
                cbbStatMultiplierPresets.SelectedIndex = 0;
        }

        private void BtApplyPreset_Click(object sender, EventArgs e)
        {
            ServerMultipliers multiplierPreset = Values.V.serverMultipliersPresets.GetPreset(cbbStatMultiplierPresets.SelectedItem.ToString());
            if (multiplierPreset == null) return;

            // first set multipliers to default/official values, then set different values of preset
            ApplyMultiplierPreset(Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.Official));
            ApplyMultiplierPreset(multiplierPreset);
        }

        /// <summary>
        /// Applies the multipliers of the preset.
        /// </summary>
        private void ApplyMultiplierPreset(ServerMultipliers sm, bool onlyStatMultipliers = false)
        {
            if (sm == null) return;

            if (!onlyStatMultipliers)
            {
                nudTamingSpeed.ValueSave = (decimal)sm.TamingSpeedMultiplier;
                nudDinoCharacterFoodDrain.ValueSave = (decimal)sm.DinoCharacterFoodDrainMultiplier;
                nudEggHatchSpeed.ValueSave = (decimal)sm.EggHatchSpeedMultiplier;
                nudBabyMatureSpeed.ValueSave = (decimal)sm.BabyMatureSpeedMultiplier;
                nudBabyImprintingStatScale.ValueSave = (decimal)sm.BabyImprintingStatScaleMultiplier;
                nudBabyCuddleInterval.ValueSave = (decimal)sm.BabyCuddleIntervalMultiplier;
                nudBabyImprintAmount.ValueSave = (decimal)sm.BabyImprintAmountMultiplier;
                nudMatingInterval.ValueSave = (decimal)sm.MatingIntervalMultiplier;
                nudMatingSpeed.ValueSave = (decimal)sm.MatingSpeedMultiplier;
                nudBabyFoodConsumptionSpeed.ValueSave = (decimal)sm.BabyFoodConsumptionSpeedMultiplier;

                ////numericUpDownDomLevelNr.ValueSave = ;
                //numericUpDownMaxBreedingSug.ValueSave = cc.maxBreedingSuggestions;
                //numericUpDownMaxWildLevel.ValueSave = cc.maxWildLevel;
                //nudMaxServerLevel.ValueSave = cc.maxServerLevel > 0 ? cc.maxServerLevel : 0;
            }

            if (sm.statMultipliers == null) return;
            int loopTo = Math.Min(Values.STATS_COUNT, sm.statMultipliers.Length);
            for (int s = 0; s < loopTo; s++)
            {
                _multSetter[s].Multipliers = sm.statMultipliers[s];
            }
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            LastTabPageIndex = (SettingsTabPages)tabControlSettings.SelectedIndex;
        }

        private void cbMoveImportedFileToSubFolder_CheckedChanged(object sender, EventArgs e)
        {
            if (cbMoveImportedFileToSubFolder.Checked)
                cbDeleteAutoImportedFile.Checked = false;
        }

        private void cbDeleteAutoImportedFile_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDeleteAutoImportedFile.Checked)
                cbMoveImportedFileToSubFolder.Checked = false;
        }

        private void btExportMultipliers_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "ARK Multiplier File (*.ini)|*.ini",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                FileName = "ASBMultipliers"
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    SaveMultiplierSettingsToFile(dlg.FileName);
                }
            }
        }

        /// <summary>
        /// Saves the multipliers for the stats, taming and breeding to an ini-file.
        /// </summary>
        /// <param name="fileName"></param>
        private void SaveMultiplierSettingsToFile(string fileName)
        {
            var sb = new System.Text.StringBuilder();
            var cultureForStrings = System.Globalization.CultureInfo.GetCultureInfo("en-US");

            // stat multipliers
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                sb.AppendLine($"PerLevelStatsMultiplier_DinoTamed_Add[{s}] = {_multSetter[s].Multipliers[0].ToString(cultureForStrings)}");
                sb.AppendLine($"PerLevelStatsMultiplier_DinoTamed_Affinity[{s}] = {_multSetter[s].Multipliers[1].ToString(cultureForStrings)}");
                sb.AppendLine($"PerLevelStatsMultiplier_DinoTamed[{s}] = {_multSetter[s].Multipliers[2].ToString(cultureForStrings)}");
                sb.AppendLine($"PerLevelStatsMultiplier_DinoWild[{s}] = {_multSetter[s].Multipliers[3].ToString(cultureForStrings)}");
            }

            // breeding multipliers
            sb.AppendLine($"MatingIntervalMultiplier = {nudMatingInterval.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"EggHatchSpeedMultiplier = {nudEggHatchSpeed.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"MatingSpeedMultiplier = {nudMatingSpeed.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"BabyMatureSpeedMultiplier = {nudBabyMatureSpeed.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"BabyImprintingStatScaleMultiplier = {nudBabyImprintingStatScale.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"BabyCuddleIntervalMultiplier = {nudBabyCuddleInterval.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"BabyFoodConsumptionSpeedMultiplier = {nudBabyFoodConsumptionSpeed.Value.ToString(cultureForStrings)}");

            sb.AppendLine($"bUseSingleplayerSettings = {(cbSingleplayerSettings.Checked ? "true" : "false")}");
            sb.AppendLine($"DestroyTamesOverLevelClamp = {nudMaxServerLevel.Value.ToString(cultureForStrings)}");

            // taming multipliers
            sb.AppendLine($"TamingSpeedMultiplier = {nudTamingSpeed.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"DinoCharacterFoodDrainMultiplier = {nudDinoCharacterFoodDrain.Value.ToString(cultureForStrings)}");

            //// the settings below are not settings that appear in ARK server config files and are used only in ASB
            // max levels
            sb.AppendLine($"ASBMaxWildLevels_Dinos = {nudMaxWildLevels.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"ASBMaxDomLevels_Dinos = {nudMaxDomLevels.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"ASBMaxGraphLevels = {nudMaxGraphLevel.Value.ToString(cultureForStrings)}");

            // extractor
            sb.AppendLine($"ASBExtractorWildLevelSteps = {(cbConsiderWildLevelSteps.Checked ? nudWildLevelStep.Value.ToString(cultureForStrings) : "1")}");
            sb.AppendLine($"ASBAllowHyperImprinting = {(cbAllowMoreThanHundredImprinting.Checked ? "true" : "false")}");

            // event multipliers
            sb.AppendLine($"ASBEvent_MatingIntervalMultiplier = {nudMatingIntervalEvent.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"ASBEvent_EggHatchSpeedMultiplier = {nudEggHatchSpeedEvent.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"ASBEvent_BabyMatureSpeedMultiplier = {nudBabyMatureSpeedEvent.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"ASBEvent_BabyCuddleIntervalMultiplier = {nudBabyCuddleIntervalEvent.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"ASBEvent_BabyFoodConsumptionSpeedMultiplier = {nudBabyFoodConsumptionSpeedEvent.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"ASBEvent_TamingSpeedMultiplier = {nudTamingSpeedEvent.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"ASBEvent_DinoCharacterFoodDrainMultiplier = {nudDinoCharacterFoodDrainEvent.Value.ToString(cultureForStrings)}");

            try
            {
                File.WriteAllText(fileName, sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBoxes.ExceptionMessageBox(ex, "Error while writing settings file:", "File writing error");
            }
        }

        public enum SettingsTabPages
        {
            Unknown = -1,
            Multipliers = 0,
            General = 1,
            SaveImport = 2,
            ExportedImport = 3,
            Timers = 4,
            Overlay = 5,
            Ocr = 6,
        }

        private void cbCustomOverlayLocation_CheckedChanged(object sender, EventArgs e)
        {
            pCustomOverlayLocation.Enabled = cbCustomOverlayLocation.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tbOCRCaptureApp.Text = DefaultOcrProcessName;
        }

        private void Localization()
        {
            Loc.ControlText(buttonOK, "OK");
            Loc.ControlText(buttonCancel, "Cancel");
            Loc.ControlText(BtBeepFailure, _tt);
            Loc.ControlText(BtBeepSuccess, _tt);
            Loc.ControlText(BtBeepTop, _tt);
            Loc.ControlText(BtBeepNewTop, _tt);
            Loc.ControlText(BtGetExportFolderAutomatically);
        }

        private void cbSingleplayerSettings_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSingleplayerSettings.Checked)
            {
                if (nudMaxDomLevels.Value != CreatureCollection.MaxDomLevelSinglePlayerDefault)
                    LbDefaultLevelups.Text = $"default: {CreatureCollection.MaxDomLevelSinglePlayerDefault}";
                else LbDefaultLevelups.Text = string.Empty;
            }
            else
            {
                if (nudMaxDomLevels.Value != CreatureCollection.MaxDomLevelDefault)
                    LbDefaultLevelups.Text = $"default: {CreatureCollection.MaxDomLevelDefault}";
                else LbDefaultLevelups.Text = string.Empty;
            }
        }

        private void BtBeepFailure_Click(object sender, EventArgs e)
        {
            SoundFeedback.BeepSignal(SoundFeedback.FeedbackSounds.Failure);
        }

        private void BtBeepSuccess_Click(object sender, EventArgs e)
        {
            SoundFeedback.BeepSignal(SoundFeedback.FeedbackSounds.Success);
        }

        private void BtBeepTop_Click(object sender, EventArgs e)
        {
            SoundFeedback.BeepSignal(SoundFeedback.FeedbackSounds.Good);
        }

        private void BtBeepNewTop_Click(object sender, EventArgs e)
        {
            SoundFeedback.BeepSignal(SoundFeedback.FeedbackSounds.Great);
        }

        private void BtImportArchiveFolder_Click(object sender, EventArgs e)
        {
            // get folder of first export path
            SelectFolder(BtImportArchiveFolder,
                BtImportArchiveFolder.Tag is string lastFolder && !string.IsNullOrEmpty(lastFolder)
                ? lastFolder
                : aTExportFolderLocationsBindingSource.OfType<ATImportExportedFolderLocation>()
                    .Where(location => !string.IsNullOrWhiteSpace(location.FolderPath))
                    .Select(location => location.FolderPath).FirstOrDefault());
        }

        private void SelectFolder(Button folderButton, string initialFolder = null, bool displayFullPathOnButton = false)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.RootFolder = Environment.SpecialFolder.Desktop;
                if (!string.IsNullOrEmpty(initialFolder) && Directory.Exists(initialFolder))
                    dlg.SelectedPath = initialFolder;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    SetFolderSelectionButton(folderButton, dlg.SelectedPath, displayFullPathOnButton);
                }
            }
        }

        /// <summary>
        /// Sets the Text and Tag of a button to a folder path
        /// </summary>
        private void SetFolderSelectionButton(Button button, string folderPath = null, bool displayFullPathOnButton = false)
        {
            button.Text = string.IsNullOrEmpty(folderPath) ? $"<{Loc.S("na")}>" : displayFullPathOnButton ? folderPath : Path.GetFileName(folderPath);
            button.Tag = folderPath;
            if (!button.AutoEllipsis)
                _tt.SetToolTip(button, folderPath);
        }

        private void BtGetExportFolderAutomatically_Click(object sender, EventArgs e)
        {
            if (ExportFolderLocation.GetListOfExportFolders(out (string path, string steamPlayerName)[] arkInstallFolders, out string error))
            {
                int i = 0;
                foreach (var p in arkInstallFolders)
                    aTExportFolderLocationsBindingSource.Insert(i++, ATImportExportedFolderLocation.CreateFromString(
                        $"default ({p.steamPlayerName})||{p.path}"));
            }
            else
            {
                MessageBox.Show(
                    Loc.S("ExportFolderDetectionFailed") + (string.IsNullOrEmpty(error) ? string.Empty : "\n\n" + error),
                    "Folder detection failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Opens a colorDialog and sets the BackColor of the button to the according color.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorButtonClick(object sender, EventArgs e)
        {
            if (!(sender is Button bt)) return;

            colorDialog1.Color = bt.BackColor;
            if (colorDialog1.ShowDialog() != DialogResult.OK) return;

            bt.SetBackColorAndAccordingForeColor(colorDialog1.Color);
        }

        private void BtBackupFolder_Click(object sender, EventArgs e)
        {
            SelectFolder(BtBackupFolder, BtBackupFolder.Tag as string, true);
        }

        private void BtClearBackupFolder_Click(object sender, EventArgs e)
        {
            SetFolderSelectionButton(BtBackupFolder);
        }

        private void CbHighlightAdjustedMultipliers_CheckedChanged(object sender, EventArgs e)
        {
            bool highlight = CbHighlightAdjustedMultipliers.Checked;
            for (int s = 0; s < Values.STATS_COUNT; s++)
                _multSetter[s].SetHighlighted(highlight);
            nudTamingSpeed.SetExtraHighlightNonDefault(highlight);
            nudDinoCharacterFoodDrain.SetExtraHighlightNonDefault(highlight);
            nudMatingSpeed.SetExtraHighlightNonDefault(highlight);
            nudMatingInterval.SetExtraHighlightNonDefault(highlight);
            nudEggHatchSpeed.SetExtraHighlightNonDefault(highlight);
            nudBabyMatureSpeed.SetExtraHighlightNonDefault(highlight);
            nudBabyCuddleInterval.SetExtraHighlightNonDefault(highlight);
            nudBabyImprintAmount.SetExtraHighlightNonDefault(highlight);
            nudBabyImprintingStatScale.SetExtraHighlightNonDefault(highlight);
            nudBabyFoodConsumptionSpeed.SetExtraHighlightNonDefault(highlight);
            cbSingleplayerSettings.BackColor = highlight && cbSingleplayerSettings.Checked ? Color.FromArgb(190, 40, 20) : Color.Transparent;
            cbSingleplayerSettings.ForeColor = Utils.ForeColor(cbSingleplayerSettings.BackColor);
        }

        private void BExportSpreadsheetMoveUp_Click(object sender, EventArgs e)
        {
            ExportSpreadSheetMoveItem(-1);
        }

        private void BExportSpreadsheetMoveDown_Click(object sender, EventArgs e)
        {
            ExportSpreadSheetMoveItem(1);
        }

        private void ExportSpreadSheetMoveItem(int moveDifference)
        {
            if (ClbExportSpreadsheetFields.SelectedIndex < 0)
                return;

            // Calculate new index using moveDifference
            var oldIndex = ClbExportSpreadsheetFields.SelectedIndex;
            var newIndex = oldIndex + moveDifference;

            // Checking bounds of the range
            if (newIndex < 0) newIndex = 0;
            if (newIndex >= ClbExportSpreadsheetFields.Items.Count)
                newIndex = ClbExportSpreadsheetFields.Items.Count - 1;

            if (newIndex == oldIndex)
                return;

            var selected = ClbExportSpreadsheetFields.SelectedItem;
            var isChecked = ClbExportSpreadsheetFields.GetItemChecked(oldIndex);

            // Removing removable element
            ClbExportSpreadsheetFields.Items.Remove(selected);
            // Insert it in new position
            ClbExportSpreadsheetFields.Items.Insert(newIndex, selected);
            // Restore selection
            ClbExportSpreadsheetFields.SetSelected(newIndex, true);
            // Restore checked state
            ClbExportSpreadsheetFields.SetItemChecked(newIndex, isChecked);
        }

        private void CbExportTableFieldsAll_CheckedChanged(object sender, EventArgs e)
        {
            var setTo = CbExportTableFieldsAll.Checked;
            var count = ClbExportSpreadsheetFields.Items.Count;
            for (int i = 0; i < count; i++)
            {
                ClbExportSpreadsheetFields.SetItemChecked(i, setTo);
            }
        }

        private readonly DataGridViewCellStyle _styleDefaultEntry = new DataGridViewCellStyle { BackColor = Color.FromArgb(211, 247, 211) };

        private void HighlightDefaultImportExportFolderEntry()
        {
            var rowCount = dataGridViewExportFolders.RowCount;
            if (rowCount == 0) return;

            dataGridViewExportFolders.Rows[0].DefaultCellStyle = _styleDefaultEntry;
            for (int i = 1; i < rowCount; i++)
                dataGridViewExportFolders.Rows[i].DefaultCellStyle = null;
        }
    }
}
