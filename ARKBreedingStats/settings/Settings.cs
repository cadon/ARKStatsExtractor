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
using System.Windows.Threading;
using ARKBreedingStats.importExportGun;
using ARKBreedingStats.library;
using ARKBreedingStats.uiControls;
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
        public bool ColorRegionDisplayChanged;

        public Settings(CreatureCollection cc, SettingsTabPages page)
        {
            InitializeData();
            _cc = cc;
            CreateListOfProcesses();
            LoadSettings(cc);
            Localization();
            tabControlSettings.SelectTab((int)page);
            DialogResult = DialogResult.Ignore;
        }

        private const string DefaultOcrProcessNameAse = "ShooterGame";
        private const string DefaultOcrProcessNameAsa = "ArkAscended";

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
            _multSetter = new MultiplierSetting[Stats.StatsCount];
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _multSetter[s] = new MultiplierSetting
                {
                    StatName = $"[{s}] {Utils.StatName(s)}"
                };
                flowLayoutPanelStatMultipliers.Controls.Add(_multSetter[s]);
            }

            CbHideInvisibleColorRegions.Visible = Values.V.InvisibleColorRegionsExist;

            // set neutral numbers for stat-multipliers to the default values to easier see what is non-default
            ServerMultipliers officialMultipliers = Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.Official);
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s < officialMultipliers.statMultipliers.Length)
                    _multSetter[s].SetNeutralValues(officialMultipliers.statMultipliers[s]);
                else _multSetter[s].SetNeutralValues(null);
            }
            nudTamingSpeed.NeutralNumber = 1;
            nudDinoCharacterFoodDrain.NeutralNumber = 1;
            NudWildDinoTorporDrainMultiplier.NeutralNumber = 1;
            nudTamedDinoCharacterFoodDrain.NeutralNumber = 1;
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
            nudTamedDinoCharacterFoodDrainEvent.NeutralNumber = 1;
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
            _tt.SetToolTip(CbAtlasSettings, "Check this if you use this tool with creatures from the game ATLAS. This settings adjusts some of the multipliers to match the ones of ATLAS.");
            _tt.SetToolTip(cbAllowMoreThanHundredImprinting, "Enable this if on your server more than 100% imprinting are possible, e.g. with the mod S+ with a Nanny");
            _tt.SetToolTip(cbDevTools, "Shows extra tabs for multiplier-testing and extraction test-cases.");
            _tt.SetToolTip(nudMaxServerLevel, "The max level allowed on the server. Currently creatures with more than 450 levels will be deleted on official servers.\nA creature that can be potentially have a higher level than this (if maximally leveled up) will be marked with a orange-red text in the library.\nSet to 0 to disable a warning in the loaded library.");
            _tt.SetToolTip(lbMaxTotalLevel, "The max level allowed on the server. Currently creatures with more than 450 levels will be deleted on official servers.\nThis limit can be enabled on unoffical servers with the setting DestroyTamesOverLevelClamp.\nA creature in this library that can be potentially have a higher level than this (if maximally leveled up) will be marked with a orange-red text in the library.\nSet to 0 to disable a warning in the loaded library.");
            _tt.SetToolTip(CbExportFileRenameAfterImport, "Use a pattern to create the new file name, a subset of the keywords and functions from the naming pattern work.");
            _tt.SetToolTip(CbHighlightAdjustedMultipliers, "Highlight multipliers that are set to non-official values.\nDoes not update on multiplier change, this button needs to be rechecked then.\nCan be used to share screenshots of these settings.");
            _tt.SetToolTip(LbLanguage2, "Here you can specify a different language for exported data, e.g. the info graphics.");

            // localizations / translations
            // for a new translation
            // * a file local/strings.[languageCode].resx needs to exist.
            // * reference translator in aboutBox
            // * the compiler created dll-file needs to be added to the installer files: edit the file setup.iss in the repository base folder.
            // * the entry in the dictionary below needs to be added
            _languages = new Dictionary<string, string>
            {
                { "Deutsch", "de"},
                { "English", "en"},
                { "Español", "es"},
                { "Français", "fr"},
                { "Italiano", "it"},
                { "日本語", "ja"},
                { "Polski", "pl"},
                { "Português do Brasil", "pt-BR"},
                { "русский", "ru"},
                { "Türkçe", "tr"},
                { "简体中文", "zh"},
                { "繁體中文", "zh-tw"}
            };

            CbbLanguage.Items.Add(Loc.S("SystemLanguage"));
            CbbLanguage2.Items.Add("-"); // indicates no secondary language, i.e. the same as primary

            foreach (string l in _languages.Keys)
            {
                CbbLanguage.Items.Add(l);
                CbbLanguage2.Items.Add(l);
            }

            _languages[Loc.S("SystemLanguage")] = string.Empty;

            foreach (var cm in Enum.GetNames(typeof(ColorModeColors.AsbColorMode)))
                CbbColorMode.Items.Add(cm);

            var availableFonts = FontFamily.Families.Select(f => f.Name).ToArray();
            CbbInfoGraphicFontName.Items.AddRange(availableFonts);
            CbbAppDefaultFontName.Items.AddRange(availableFonts);
        }

        private void LoadSettings(CreatureCollection cc)
        {
            if (cc.serverMultipliers?.statMultipliers != null)
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (s < cc.serverMultipliers.statMultipliers.Length && cc.serverMultipliers.statMultipliers[s].Length > 3)
                    {
                        _multSetter[s].Multipliers = cc.serverMultipliers.statMultipliers[s];
                    }
                    else _multSetter[s].Multipliers = null;
                }
            }
            cbSingleplayerSettings.Checked = cc.singlePlayerSettings;
            CbAtlasSettings.Checked = _cc.AtlasSettings;
            if (_cc.Game == Ark.Asa) RbGameAsa.Checked = true;
            else RbGameAse.Checked = true;

            nudMaxDomLevels.ValueSave = cc.maxDomLevel;
            numericUpDownMaxBreedingSug.ValueSave = cc.maxBreedingSuggestions;
            nudMaxWildLevels.ValueSave = cc.maxWildLevel;
            nudMaxServerLevel.ValueSave = cc.maxServerLevel > 0 ? cc.maxServerLevel : 0;
            nudMaxGraphLevel.ValueSave = cc.maxChartLevel;
            CbAllowSpeedLeveling.Checked = cc.serverMultipliers?.AllowSpeedLeveling ?? false;
            CbAllowFlyerSpeedLeveling.Checked = cc.serverMultipliers?.AllowFlyerSpeedLeveling ?? false;
            #region Non-event multiplier
            var multipliers = cc.serverMultipliers;
            if (multipliers == null)
            {
                multipliers = new ServerMultipliers();
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
            NudWildDinoTorporDrainMultiplier.ValueSave = (decimal)multipliers.WildDinoTorporDrainMultiplier;
            nudTamedDinoCharacterFoodDrain.ValueSave = (decimal)multipliers.TamedDinoCharacterFoodDrainMultiplier;
            nudBabyFoodConsumptionSpeed.ValueSave = (decimal)multipliers.BabyFoodConsumptionSpeedMultiplier;
            #endregion
            #region event-multiplier
            multipliers = cc.serverMultipliersEvents ?? multipliers;
            nudBabyCuddleIntervalEvent.ValueSave = (decimal)multipliers.BabyCuddleIntervalMultiplier;
            nudBabyImprintAmountEvent.ValueSave = (decimal)multipliers.BabyImprintAmountMultiplier;
            nudTamingSpeedEvent.ValueSave = (decimal)multipliers.TamingSpeedMultiplier;
            nudDinoCharacterFoodDrainEvent.ValueSave = (decimal)multipliers.DinoCharacterFoodDrainMultiplier;
            nudTamedDinoCharacterFoodDrainEvent.ValueSave = (decimal)multipliers.TamedDinoCharacterFoodDrainMultiplier;
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
            CbbAppDefaultFontName.Text = Properties.Settings.Default.DefaultFontName;
            nudDefaultFontSize.Value = (decimal)Properties.Settings.Default.DefaultFontSize;

            CbKeepMultipliersForNewLibrary.Checked = Properties.Settings.Default.KeepMultipliersForNewLibrary;

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
            CbOCRFromClipboard.Checked = Properties.Settings.Default.OCRFromClipboard;
            var rec = Properties.Settings.Default.OCRFromRectangle;
            NudOCRClipboardCropLeft.ValueSave = rec.Left;
            NudOCRClipboardCropTop.ValueSave = rec.Top;
            NudOCRClipboardCropWidth.ValueSave = rec.Width;
            NudOCRClipboardCropHeight.ValueSave = rec.Height;
            cbOCRIgnoreImprintValue.Checked = Properties.Settings.Default.OCRIgnoresImprintValue;
            NudOverlayRelativeFontSize.ValueSave = (decimal)Properties.Settings.Default.OverlayRelativeFontSize;
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
            nudChartLevelEvenMin.ValueSave = Properties.Settings.Default.ChartHueEvenMin;
            nudChartLevelEvenMax.ValueSave = Properties.Settings.Default.ChartHueEvenMax;
            nudChartLevelOddMin.ValueSave = Properties.Settings.Default.ChartHueOddMin;
            nudChartLevelOddMax.ValueSave = Properties.Settings.Default.ChartHueOddMax;

            #region InfoGraphic

            nudInfoGraphicHeight.ValueSave = Properties.Settings.Default.InfoGraphicHeight;
            CbInfoGraphicDisplayMaxWildLevel.Checked = Properties.Settings.Default.InfoGraphicShowMaxWildLevel;
            CbInfoGraphicDomLevels.Checked = Properties.Settings.Default.InfoGraphicWithDomLevels;
            CbbInfoGraphicFontName.Text = Properties.Settings.Default.InfoGraphicFontName;
            CbInfoGraphicMutations.Checked = Properties.Settings.Default.InfoGraphicDisplayMutations;
            CbInfoGraphicGenerations.Checked = Properties.Settings.Default.InfoGraphicDisplayGeneration;
            CbInfoGraphicCreatureName.Checked = Properties.Settings.Default.InfoGraphicDisplayName;
            BtInfoGraphicBackColor.SetBackColorAndAccordingForeColor(Properties.Settings.Default.InfoGraphicBackColor);
            BtInfoGraphicForeColor.SetBackColorAndAccordingForeColor(Properties.Settings.Default.InfoGraphicForeColor);
            BtInfoGraphicBorderColor.SetBackColorAndAccordingForeColor(Properties.Settings.Default.InfoGraphicBorderColor);
            CbInfoGraphicAddRegionNames.Checked = Properties.Settings.Default.InfoGraphicExtraRegionNames;
            CbInfoGraphicColorRegionNamesIfNoImage.Checked = Properties.Settings.Default.InfoGraphicShowRegionNamesIfNoImage;
            CbInfoGraphicStatValues.Checked = Properties.Settings.Default.InfoGraphicShowStatValues;

            #endregion

            #region library

            CbPauseGrowingTimerAfterAdding.Checked = Properties.Settings.Default.PauseGrowingTimerAfterAddingBaby;
            cbCreatureColorsLibrary.Checked = Properties.Settings.Default.showColorsInLibrary;
            cbApplyGlobalSpeciesToLibrary.Checked = Properties.Settings.Default.ApplyGlobalSpeciesToLibrary;
            CbLibrarySelectSelectedSpeciesOnLoad.Checked = Properties.Settings.Default.LibrarySelectSelectedSpeciesOnLoad;
            cbLibraryHighlightTopCreatures.Checked = Properties.Settings.Default.LibraryHighlightTopCreatures;
            CbConsiderWastedStatsForTopCreatures.Checked = Properties.Settings.Default.ConsiderWastedStatsForTopCreatures;
            CbNaturalSorting.Checked = Properties.Settings.Default.UseNaturalSort;
            CbNaturalSortIgnoreSpaces.Enabled = Properties.Settings.Default.UseNaturalSort;
            CbNaturalSortIgnoreSpaces.Checked = Properties.Settings.Default.NaturalSortIgnoreSpaces;
            CbDisplayLibraryCreatureIndex.Checked = Properties.Settings.Default.DisplayLibraryCreatureIndex;

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
            dataGridView_FileLocations.DataBindingComplete += (s, e) => HighlightMissingFilesInImportSaveFileView();
            nudWarnImportMoreThan.Value = Properties.Settings.Default.WarnWhenImportingMoreCreaturesThan;
            CbApplyNamingPatternOnImportAlways.Checked = Properties.Settings.Default.applyNamePatternOnAutoImportAlways;
            cbApplyNamePatternOnImportOnEmptyNames.Checked = Properties.Settings.Default.applyNamePatternOnImportIfEmptyName;
            cbApplyNamePatternOnImportOnNewCreatures.Checked = Properties.Settings.Default.applyNamePatternOnAutoImportForNewCreatures;
            cbCopyPatternNameToClipboard.Checked = Properties.Settings.Default.copyNameToClipboardOnImportWhenAutoNameApplied;
            cbAutoImportExported.Checked = Properties.Settings.Default.AutoImportExportedCreatures;
            CbAutoExtractAddToLibrary.Checked = Properties.Settings.Default.OnAutoImportAddToLibrary;
            cbPlaySoundOnAutomaticImport.Checked = Properties.Settings.Default.PlaySoundOnAutoImport;
            cbMoveImportedFileToSubFolder.Checked = Properties.Settings.Default.MoveAutoImportedFileToSubFolder;
            SetFolderSelectionButton(BtImportArchiveFolder, Properties.Settings.Default.ImportExportedArchiveFolder);
            cbDeleteAutoImportedFile.Checked = Properties.Settings.Default.DeleteAutoImportedFile;
            CbExportFileRenameAfterImport.Checked = Properties.Settings.Default.AutoImportedExportFileRename;
            TbExportFileRename.Text = Properties.Settings.Default.AutoImportedExportFileRenamePattern;
            CbAutoImportSuccessGotoLibrary.Checked = Properties.Settings.Default.AutoImportGotoLibraryAfterSuccess;
            CbBringToFrontOnImportExportIssue.Checked = Properties.Settings.Default.ImportExportedBringToFrontOnIssue;
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
                foreach (ExportImportCreatures.TableExportFields f in exportFields)
                    ClbExportSpreadsheetFields.Items.Add(f, true);
            }

            foreach (ExportImportCreatures.TableExportFields f in Enum.GetValues(typeof(ExportImportCreatures.TableExportFields)))
            {
                if (exportFields?.Contains((int)f) ?? false) continue;
                ClbExportSpreadsheetFields.Items.Add(f, false);
            }

            #endregion

            NudSpeciesSelectorCountLastUsed.ValueSave = Properties.Settings.Default.SpeciesSelectorCountLastSpecies;

            cbDevTools.Checked = Properties.Settings.Default.DevTools;

            cbPrettifyJSON.Checked = Properties.Settings.Default.prettifyCollectionJson;

            cbAdminConsoleCommandWithCheat.Checked = Properties.Settings.Default.AdminConsoleCommandWithCheat;

            CbAskSaveSettingsOnClose.Checked = Properties.Settings.Default.AskSaveSettingsOnClose;

            string langKey = _languages.FirstOrDefault(x => x.Value == Properties.Settings.Default.language).Key ?? string.Empty;
            int langI = CbbLanguage.Items.IndexOf(langKey);
            CbbLanguage.SelectedIndex = langI == -1 ? 0 : langI;

            langKey = _languages.FirstOrDefault(x => x.Value == Properties.Settings.Default.language2).Key ?? string.Empty;
            langI = CbbLanguage2.Items.IndexOf(langKey);
            CbbLanguage2.SelectedIndex = langI == -1 ? 0 : langI;

            CbHideInvisibleColorRegions.Checked = Properties.Settings.Default.HideInvisibleColorRegions;
            CbAlwaysShowAllColorRegions.Checked = Properties.Settings.Default.AlwaysShowAllColorRegions;
            CbColorIdOnColorRegionButton.Checked = Properties.Settings.Default.ShowColorIdOnRegionButtons;

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
                _cc.serverMultipliers.statMultipliers = new double[Stats.StatsCount][];
            }
            if (_cc.serverMultipliers?.statMultipliers != null)
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (_cc.serverMultipliers.statMultipliers[s] == null)
                        _cc.serverMultipliers.statMultipliers[s] = new double[4];
                    for (int sm = 0; sm < 4; sm++)
                        _cc.serverMultipliers.statMultipliers[s][sm] = _multSetter[s].Multipliers[sm];
                }
            }

            // Torpidity is handled differently by the game, IwM has no effect. Set IwM to 1.
            // See https://github.com/cadon/ARKStatsExtractor/issues/942 for more infos about this.
            _cc.serverMultipliers.statMultipliers[Stats.Torpidity][Stats.IndexLevelWild] = 1;

            _cc.singlePlayerSettings = cbSingleplayerSettings.Checked;
            _cc.AtlasSettings = CbAtlasSettings.Checked;
            _cc.Game = RbGameAsa.Checked ? Ark.Asa : Ark.Ase;

            _cc.maxDomLevel = (int)nudMaxDomLevels.Value;
            _cc.maxWildLevel = (int)nudMaxWildLevels.Value;
            _cc.maxServerLevel = (int)nudMaxServerLevel.Value;
            _cc.maxChartLevel = (int)nudMaxGraphLevel.Value;
            _cc.serverMultipliers.AllowSpeedLeveling = CbAllowSpeedLeveling.Checked || RbGameAse.Checked;
            _cc.serverMultipliers.AllowFlyerSpeedLeveling = CbAllowFlyerSpeedLeveling.Checked;
            _cc.maxBreedingSuggestions = (int)numericUpDownMaxBreedingSug.Value;
            Properties.Settings.Default.IgnoreSexInBreedingPlan = cbIgnoreSexInBreedingPlan.Checked;

            Properties.Settings.Default.KeepMultipliersForNewLibrary = CbKeepMultipliersForNewLibrary.Checked;

            #region non-event-multiplier
            _cc.serverMultipliers.TamingSpeedMultiplier = (double)nudTamingSpeed.Value;
            _cc.serverMultipliers.DinoCharacterFoodDrainMultiplier = (double)nudDinoCharacterFoodDrain.Value;
            _cc.serverMultipliers.WildDinoTorporDrainMultiplier = (double)NudWildDinoTorporDrainMultiplier.Value;
            _cc.serverMultipliers.TamedDinoCharacterFoodDrainMultiplier = (double)nudTamedDinoCharacterFoodDrain.Value;
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
            _cc.serverMultipliersEvents.TamedDinoCharacterFoodDrainMultiplier = (double)nudTamedDinoCharacterFoodDrainEvent.Value;
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
            Properties.Settings.Default.DefaultFontName = CbbAppDefaultFontName.Text;
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
            Properties.Settings.Default.OCRFromClipboard = CbOCRFromClipboard.Checked;
            Properties.Settings.Default.OCRFromRectangle = new Rectangle((int)NudOCRClipboardCropLeft.Value, (int)NudOCRClipboardCropTop.Value, (int)NudOCRClipboardCropWidth.Value, (int)NudOCRClipboardCropHeight.Value);
            Properties.Settings.Default.OCRIgnoresImprintValue = cbOCRIgnoreImprintValue.Checked;
            Properties.Settings.Default.OverlayRelativeFontSize = (float)NudOverlayRelativeFontSize.Value;
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
            Properties.Settings.Default.ChartHueEvenMin = (int)nudChartLevelEvenMin.Value;
            Properties.Settings.Default.ChartHueEvenMax = (int)nudChartLevelEvenMax.Value;
            Properties.Settings.Default.ChartHueOddMin = (int)nudChartLevelOddMin.Value;
            Properties.Settings.Default.ChartHueOddMax = (int)nudChartLevelOddMax.Value;

            #region InfoGraphic

            Properties.Settings.Default.InfoGraphicHeight = (int)nudInfoGraphicHeight.Value;
            Properties.Settings.Default.InfoGraphicShowMaxWildLevel = CbInfoGraphicDisplayMaxWildLevel.Checked;
            Properties.Settings.Default.InfoGraphicWithDomLevels = CbInfoGraphicDomLevels.Checked;
            Properties.Settings.Default.InfoGraphicFontName = CbbInfoGraphicFontName.Text;
            Properties.Settings.Default.InfoGraphicDisplayMutations = CbInfoGraphicMutations.Checked;
            Properties.Settings.Default.InfoGraphicDisplayGeneration = CbInfoGraphicGenerations.Checked;
            Properties.Settings.Default.InfoGraphicDisplayName = CbInfoGraphicCreatureName.Checked;
            Properties.Settings.Default.InfoGraphicBackColor = BtInfoGraphicBackColor.BackColor;
            Properties.Settings.Default.InfoGraphicForeColor = BtInfoGraphicForeColor.BackColor;
            Properties.Settings.Default.InfoGraphicBorderColor = BtInfoGraphicBorderColor.BackColor;
            Properties.Settings.Default.InfoGraphicExtraRegionNames = CbInfoGraphicAddRegionNames.Checked;
            Properties.Settings.Default.InfoGraphicShowRegionNamesIfNoImage = CbInfoGraphicColorRegionNamesIfNoImage.Checked;
            Properties.Settings.Default.InfoGraphicShowStatValues = CbInfoGraphicStatValues.Checked;

            #endregion

            #region library

            Properties.Settings.Default.PauseGrowingTimerAfterAddingBaby = CbPauseGrowingTimerAfterAdding.Checked;
            Properties.Settings.Default.showColorsInLibrary = cbCreatureColorsLibrary.Checked;
            Properties.Settings.Default.ApplyGlobalSpeciesToLibrary = cbApplyGlobalSpeciesToLibrary.Checked;
            Properties.Settings.Default.LibrarySelectSelectedSpeciesOnLoad = CbLibrarySelectSelectedSpeciesOnLoad.Checked;
            Properties.Settings.Default.LibraryHighlightTopCreatures = cbLibraryHighlightTopCreatures.Checked;
            Properties.Settings.Default.ConsiderWastedStatsForTopCreatures = CbConsiderWastedStatsForTopCreatures.Checked;
            Properties.Settings.Default.UseNaturalSort = CbNaturalSorting.Checked;
            Properties.Settings.Default.NaturalSortIgnoreSpaces = CbNaturalSortIgnoreSpaces.Checked;
            Properties.Settings.Default.DisplayLibraryCreatureIndex = CbDisplayLibraryCreatureIndex.Checked;

            #endregion

            #region import exported
            Properties.Settings.Default.WarnWhenImportingMoreCreaturesThan = (int)nudWarnImportMoreThan.Value;
            Properties.Settings.Default.ExportCreatureFolders = aTExportFolderLocationsBindingSource.OfType<ATImportExportedFolderLocation>()
                    .Where(location => !string.IsNullOrWhiteSpace(location.FolderPath))
                    .Select(location => location.ToString()).ToArray();

            Properties.Settings.Default.applyNamePatternOnAutoImportAlways = CbApplyNamingPatternOnImportAlways.Checked;
            Properties.Settings.Default.applyNamePatternOnImportIfEmptyName = cbApplyNamePatternOnImportOnEmptyNames.Checked;
            Properties.Settings.Default.applyNamePatternOnAutoImportForNewCreatures = cbApplyNamePatternOnImportOnNewCreatures.Checked;
            Properties.Settings.Default.copyNameToClipboardOnImportWhenAutoNameApplied = cbCopyPatternNameToClipboard.Checked;
            Properties.Settings.Default.AutoImportExportedCreatures = cbAutoImportExported.Checked;
            Properties.Settings.Default.OnAutoImportAddToLibrary = CbAutoExtractAddToLibrary.Checked;
            Properties.Settings.Default.PlaySoundOnAutoImport = cbPlaySoundOnAutomaticImport.Checked;
            Properties.Settings.Default.MoveAutoImportedFileToSubFolder = cbMoveImportedFileToSubFolder.Checked;
            Properties.Settings.Default.ImportExportedArchiveFolder = BtImportArchiveFolder.Tag as string;
            Properties.Settings.Default.DeleteAutoImportedFile = cbDeleteAutoImportedFile.Checked;
            Properties.Settings.Default.AutoImportedExportFileRename = CbExportFileRenameAfterImport.Checked;
            Properties.Settings.Default.AutoImportedExportFileRenamePattern = TbExportFileRename.Text;
            Properties.Settings.Default.AutoImportGotoLibraryAfterSuccess = CbAutoImportSuccessGotoLibrary.Checked;
            Properties.Settings.Default.ImportExportedBringToFrontOnIssue = CbBringToFrontOnImportExportIssue.Checked;
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
                    exportFields.Add((int)Enum.Parse(typeof(ExportImportCreatures.TableExportFields), ClbExportSpreadsheetFields.Items[i].ToString()));
            }
            Properties.Settings.Default.CreatureTableExportFields = exportFields.ToArray();

            #endregion

            Properties.Settings.Default.SpeciesSelectorCountLastSpecies = (int)NudSpeciesSelectorCountLastUsed.Value;

            Properties.Settings.Default.DevTools = cbDevTools.Checked;

            Properties.Settings.Default.prettifyCollectionJson = cbPrettifyJSON.Checked;

            Properties.Settings.Default.AdminConsoleCommandWithCheat = cbAdminConsoleCommandWithCheat.Checked;

            Properties.Settings.Default.AskSaveSettingsOnClose = CbAskSaveSettingsOnClose.Checked;

            string oldLanguageSetting = Properties.Settings.Default.language;
            Properties.Settings.Default.language = _languages.TryGetValue(CbbLanguage.SelectedItem.ToString(), out var languageId) ? languageId : string.Empty;
            string oldLanguage2Setting = Properties.Settings.Default.language2;
            Properties.Settings.Default.language2 = _languages.TryGetValue(CbbLanguage2.SelectedItem.ToString(), out languageId) ? languageId : string.Empty;

            LanguageChanged = oldLanguageSetting != Properties.Settings.Default.language || oldLanguage2Setting != Properties.Settings.Default.language2;

            ColorRegionDisplayChanged = CbHideInvisibleColorRegions.Checked != Properties.Settings.Default.HideInvisibleColorRegions
                || Properties.Settings.Default.AlwaysShowAllColorRegions != CbAlwaysShowAllColorRegions.Checked;
            Properties.Settings.Default.HideInvisibleColorRegions = CbHideInvisibleColorRegions.Checked;
            Properties.Settings.Default.AlwaysShowAllColorRegions = CbAlwaysShowAllColorRegions.Checked;
            Properties.Settings.Default.ShowColorIdOnRegionButtons = CbColorIdOnColorRegionButton.Checked;

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

        /// <summary>
        /// Checks if the set path to a savegame is actually an ark save file and warns if not.
        /// </summary>
        private void CheckSaveImportPath(string filePath)
        {
            if (!filePath.EndsWith(".ark") && !filePath.EndsWith(".gz") && !filePath.Contains("*") && !filePath.Contains("(?<"))
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
                bool doMergeSettings = false; // only ask for the first dropped file if settings should be reset, for the later ones always do merge
                foreach (string filePath in files)
                {
                    switch (Path.GetExtension(filePath))
                    {
                        case ".sav":
                        case ".json":
                            LoadServerMultipliersFromSavFile(filePath);
                            break;
                        default:
                            ExtractSettingsFromFile(filePath, doMergeSettings);
                            break;
                    }

                    doMergeSettings = true;
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                ExtractSettingsFromText(e.Data.GetData(DataFormats.Text) as string);
            }
        }

        private void ExtractSettingsFromFile(string file, bool doMergeSettings = false)
        {
            if (!File.Exists(file))
                return;

            ExtractSettingsFromText(File.ReadAllText(file), doMergeSettings);
        }

        /// <summary>
        /// Parse the text and set the recognized settings accordingly.
        /// </summary>
        /// <param name="text">Text containing the settings</param>
        /// <param name="doMergeSettings">If true the user is not asked if the settings should be reset before applying the settings.</param>
        private void ExtractSettingsFromText(string text, bool doMergeSettings = false)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            // ignore lines that start with a semicolon (comments)
            text = Regex.Replace(text, @"(?:\A|[\r\n]+);[^\r\n]*", string.Empty);

            double d;
            Match m;
            var cultureForStrings = System.Globalization.CultureInfo.GetCultureInfo("en-US");

            // reset values to the default

            if (text.Contains("ASBMaxGraphLevels") || doMergeSettings)
            {
                // the file is exported by this application and contains all needed values
                // or it's not the first file of an import (i.e. user was already asked if to reset or merge)
            }
            else
            {
                var result = CustomMessageBox.Show(
                    "Do you wish to reset all multipliers to their defaults before importing this file, or merge with the current ones?",
                    "Importing settings", "Reset then import", "Merge with current", Loc.S("Cancel"),
                    MessageBoxIcon.Information);

                switch (result)
                {
                    case DialogResult.Yes:
                        ApplyMultiplierPreset(Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.Official));
                        break;
                    case DialogResult.Cancel: return;
                }

                if (result != DialogResult.Yes && text.Contains("PerLevelStatsMultiplier_Dino"))
                {
                    // the file contains stat multipliers, reset all non-existing to the default first
                    ApplyMultiplierPreset(Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.Official), true);
                }
            }

            // get stat-multipliers
            // if an ini file is imported the server is most likely unofficial with no level cap, if the server has a max level, it will be parsed.
            nudMaxServerLevel.ValueSave = 0;

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                ParseAndSetStatMultiplier(0, @"PerLevelStatsMultiplier_DinoTamed_Add\[" + s + @"\] ?= ?(\d*\.?\d+)");
                ParseAndSetStatMultiplier(1, @"PerLevelStatsMultiplier_DinoTamed_Affinity\[" + s + @"\] ?= ?(\d*\.?\d+)");
                ParseAndSetStatMultiplier(2, @"PerLevelStatsMultiplier_DinoTamed\[" + s + @"\] ?= ?(\d*\.?\d+)");
                ParseAndSetStatMultiplier(3, @"PerLevelStatsMultiplier_DinoWild\[" + s + @"\] ?= ?(\d*\.?\d+)");

                void ParseAndSetStatMultiplier(int multiplierIndex, string regexPattern)
                {
                    m = Regex.Match(text, regexPattern);
                    if (m.Success && double.TryParse(m.Groups[1].Value,
                        System.Globalization.NumberStyles.AllowDecimalPoint, cultureForStrings, out d))
                    {
                        _multSetter[s].SetMultiplier(multiplierIndex, d == 0 ? 1 : d);
                    }
                }
            }

            // breeding
            ParseAndSetValue(nudMatingInterval, @"MatingIntervalMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudEggHatchSpeed, @"EggHatchSpeedMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudMatingSpeed, @"MatingSpeedMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudBabyMatureSpeed, @"BabyMatureSpeedMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudBabyImprintingStatScale, @"BabyImprintingStatScaleMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudBabyImprintAmount, @"BabyImprintAmountMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudBabyCuddleInterval, @"BabyCuddleIntervalMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudBabyFoodConsumptionSpeed, @"BabyFoodConsumptionSpeedMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudTamedDinoCharacterFoodDrain, @"TamedDinoCharacterFoodDrainMultiplier ?= ?(\d*\.?\d+)");

            ParseAndSetCheckbox(cbSingleplayerSettings, @"bUseSingleplayerSettings ?= ?(true|false)");
            ParseAndSetValue(nudMaxServerLevel, @"DestroyTamesOverLevelClamp ?= ?(\d+)");

            // GameUserSettings.ini
            ParseAndSetValue(nudTamingSpeed, @"TamingSpeedMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudDinoCharacterFoodDrain, @"DinoCharacterFoodDrainMultiplier ?= ?(\d*\.?\d+)");
            // Game.ini
            ParseAndSetValue(NudWildDinoTorporDrainMultiplier, @"WildDinoTorporDrainMultiplier ?= ?(\d*\.?\d+)");

            //// the settings below don't appear in ARK server config files directly or not at all and are used only in ASB
            // max levels
            ParseAndSetValue(nudMaxWildLevels, @"ASBMaxWildLevels_Dinos ?= ?(\d+)");
            ParseAndSetValue(nudMaxDomLevels, @"ASBMaxDomLevels_Dinos ?= ?(\d+)");
            ParseAndSetValue(nudMaxGraphLevel, @"ASBMaxGraphLevels ?= ?(\d+)");
            // extractor
            if (ParseAndSetValue(nudWildLevelStep, @"ASBExtractorWildLevelSteps ?= ?(\d+)"))
                cbConsiderWildLevelSteps.Checked = nudWildLevelStep.Value != 1;
            ParseAndSetCheckbox(cbAllowMoreThanHundredImprinting, @"ASBAllowHyperImprinting ?= ?(true|false)");
            ParseAndSetCheckbox(CbAllowSpeedLeveling, @"ASBAllowSpeedLeveling ?= ?(true|false)");
            ParseAndSetCheckbox(CbAllowFlyerSpeedLeveling, @"ASBAllowFlyerSpeedLeveling ?= ?(true|false)");

            // event multipliers breeding
            ParseAndSetValue(nudMatingIntervalEvent, @"ASBEvent_MatingIntervalMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudEggHatchSpeedEvent, @"ASBEvent_EggHatchSpeedMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudBabyMatureSpeedEvent, @"ASBEvent_BabyMatureSpeedMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudBabyCuddleIntervalEvent, @"ASBEvent_BabyCuddleIntervalMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudBabyFoodConsumptionSpeedEvent, @"ASBEvent_BabyFoodConsumptionSpeedMultiplier ?= ?(\d*\.?\d+)");
            // event multipliers taming
            ParseAndSetValue(nudTamingSpeedEvent, @"ASBEvent_TamingSpeedMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudDinoCharacterFoodDrainEvent, @"ASBEvent_DinoCharacterFoodDrainMultiplier ?= ?(\d*\.?\d+)");

            bool ParseAndSetValue(Nud nud, string regexPattern)
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

        /// <summary>
        /// Load server multipliers from a file created by the export gun mod.
        /// </summary>
        private void LoadServerMultipliersFromSavFile(string filePath)
        {
            var esm = ImportExportGun.ReadServerMultipliers(filePath, out _);
            if (esm == null) return;

            const int roundToDigits = 6;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _multSetter[s].SetMultiplier(0, Math.Round(esm.TameAdd[s], roundToDigits));
                _multSetter[s].SetMultiplier(1, Math.Round(esm.TameAff[s], roundToDigits));
                _multSetter[s].SetMultiplier(2, Math.Round(esm.TameLevel[s], roundToDigits));
                _multSetter[s].SetMultiplier(3, Math.Round(esm.WildLevel[s], roundToDigits));
            }

            nudMaxWildLevels.ValueSave = esm.MaxWildLevel;
            nudMaxServerLevel.ValueSave = esm.DestroyTamesOverLevelClamp;
            nudTamingSpeed.ValueSaveDouble = Math.Round(esm.TamingSpeedMultiplier, roundToDigits);
            nudDinoCharacterFoodDrain.ValueSaveDouble = Math.Round(esm.DinoCharacterFoodDrainMultiplier, roundToDigits);
            NudWildDinoTorporDrainMultiplier.ValueSaveDouble = Math.Round(esm.WildDinoTorporDrainMultiplier, roundToDigits);
            nudMatingSpeed.ValueSaveDouble = Math.Round(esm.MatingSpeedMultiplier, roundToDigits);
            nudMatingInterval.ValueSaveDouble = Math.Round(esm.MatingIntervalMultiplier, roundToDigits);
            nudEggHatchSpeed.ValueSaveDouble = Math.Round(esm.EggHatchSpeedMultiplier, roundToDigits);
            nudBabyMatureSpeed.ValueSaveDouble = Math.Round(esm.BabyMatureSpeedMultiplier, roundToDigits);
            nudBabyCuddleInterval.ValueSaveDouble = Math.Round(esm.BabyCuddleIntervalMultiplier, roundToDigits);
            nudBabyImprintAmount.ValueSaveDouble = Math.Round(esm.BabyImprintAmountMultiplier, roundToDigits);
            nudBabyImprintingStatScale.ValueSaveDouble = Math.Round(esm.BabyImprintingStatScaleMultiplier, roundToDigits);
            nudBabyFoodConsumptionSpeed.ValueSaveDouble = Math.Round(esm.BabyFoodConsumptionSpeedMultiplier, roundToDigits);
            nudTamedDinoCharacterFoodDrain.ValueSaveDouble = Math.Round(esm.TamedDinoCharacterFoodDrainMultiplier, roundToDigits);
            CbAllowSpeedLeveling.Checked = esm.AllowSpeedLeveling;
            CbAllowFlyerSpeedLeveling.Checked = esm.AllowFlyerSpeedLeveling;
            cbSingleplayerSettings.Checked = esm.UseSingleplayerSettings;
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
            nudBabyImprintAmountEvent.ValueSave = nudBabyImprintAmount.Value;
            nudBabyFoodConsumptionSpeedEvent.ValueSave = nudBabyFoodConsumptionSpeed.Value;
            nudTamedDinoCharacterFoodDrainEvent.ValueSave = nudTamedDinoCharacterFoodDrain.Value;
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
            else if (e.ColumnIndex == ImportWithQuickImport.Index)
            {
                // the control itself locks the checkbox to readonly, it seems only possible like this
                if (aTImportFileLocationBindingSource[e.RowIndex] is ATImportFileLocation il)
                    il.ImportWithQuickImport = !il.ImportWithQuickImport;
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
                NudWildDinoTorporDrainMultiplier.ValueSave = (decimal)sm.WildDinoTorporDrainMultiplier;
                nudTamedDinoCharacterFoodDrain.ValueSave = (decimal)sm.TamedDinoCharacterFoodDrainMultiplier;
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
            int loopTo = Math.Min(Stats.StatsCount, sm.statMultipliers.Length);
            for (int s = 0; s < loopTo; s++)
            {
                _multSetter[s].Multipliers = sm.statMultipliers[s];
            }
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing
                && ActiveControl != buttonOK
                && ActiveControl != buttonCancel
                && Properties.Settings.Default.AskSaveSettingsOnClose)
            {
                switch (MessageBox.Show("Save settings?", "Save settings?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case DialogResult.Yes:
                        SaveSettings();
                        break;
                    case DialogResult.No: break;
                    default:
                        e.Cancel = true;
                        return;
                }
            }
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
                if (dlg.ShowDialog() != DialogResult.OK) return;
                try
                {
                    File.WriteAllText(dlg.FileName, GetMultiplierSettings());
                }
                catch (Exception ex)
                {
                    MessageBoxes.ExceptionMessageBox(ex, "Error while writing settings file:", "File writing error");
                }
            }
        }

        private void BtSettingsToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(GetMultiplierSettings());
        }

        /// <summary>
        /// Returns the multipliers for the stats, taming and breeding in a string.
        /// </summary>
        private string GetMultiplierSettings()
        {
            var sb = new System.Text.StringBuilder();
            var cultureForStrings = System.Globalization.CultureInfo.GetCultureInfo("en-US");

            // stat multipliers
            for (int s = 0; s < Stats.StatsCount; s++)
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
            sb.AppendLine($"BabyImprintAmountMultiplier = {nudBabyImprintAmount.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"BabyCuddleIntervalMultiplier = {nudBabyCuddleInterval.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"BabyFoodConsumptionSpeedMultiplier = {nudBabyFoodConsumptionSpeed.Value.ToString(cultureForStrings)}");

            sb.AppendLine($"bUseSingleplayerSettings = {(cbSingleplayerSettings.Checked ? "true" : "false")}");
            sb.AppendLine($"DestroyTamesOverLevelClamp = {nudMaxServerLevel.Value.ToString(cultureForStrings)}");

            // taming multipliers
            sb.AppendLine($"TamingSpeedMultiplier = {nudTamingSpeed.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"DinoCharacterFoodDrainMultiplier = {nudDinoCharacterFoodDrain.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"WildDinoTorporDrainMultiplier = {NudWildDinoTorporDrainMultiplier.Value.ToString(cultureForStrings)}");

            //// the settings below are not settings that appear in ARK server config files and are used only in ASB
            // max levels
            sb.AppendLine($"ASBMaxWildLevels_Dinos = {nudMaxWildLevels.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"ASBMaxDomLevels_Dinos = {nudMaxDomLevels.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"ASBMaxGraphLevels = {nudMaxGraphLevel.Value.ToString(cultureForStrings)}");

            // extractor
            sb.AppendLine($"ASBExtractorWildLevelSteps = {(cbConsiderWildLevelSteps.Checked ? nudWildLevelStep.Value.ToString(cultureForStrings) : "1")}");
            sb.AppendLine($"ASBAllowHyperImprinting = {(cbAllowMoreThanHundredImprinting.Checked ? "true" : "false")}");
            sb.AppendLine($"ASBAllowSpeedLeveling = {(CbAllowSpeedLeveling.Checked ? "true" : "false")}");
            sb.AppendLine($"ASBAllowFlyerSpeedLeveling = {(CbAllowFlyerSpeedLeveling.Checked ? "true" : "false")}");

            // event multipliers
            sb.AppendLine($"ASBEvent_MatingIntervalMultiplier = {nudMatingIntervalEvent.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"ASBEvent_EggHatchSpeedMultiplier = {nudEggHatchSpeedEvent.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"ASBEvent_BabyMatureSpeedMultiplier = {nudBabyMatureSpeedEvent.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"ASBEvent_BabyCuddleIntervalMultiplier = {nudBabyCuddleIntervalEvent.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"ASBEvent_BabyFoodConsumptionSpeedMultiplier = {nudBabyFoodConsumptionSpeedEvent.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"ASBEvent_TamingSpeedMultiplier = {nudTamingSpeedEvent.Value.ToString(cultureForStrings)}");
            sb.AppendLine($"ASBEvent_DinoCharacterFoodDrainMultiplier = {nudDinoCharacterFoodDrainEvent.Value.ToString(cultureForStrings)}");

            return sb.ToString();
        }

        public enum SettingsTabPages
        {
            Unknown = -1,
            Multipliers = 0,
            General = 1,
            InfoGraphicPreview = 2,
            SaveImport = 3,
            ExportedImport = 4,
            Timers = 5,
            Overlay = 6,
            Ocr = 7
        }

        private void cbCustomOverlayLocation_CheckedChanged(object sender, EventArgs e)
        {
            pCustomOverlayLocation.Enabled = cbCustomOverlayLocation.Checked;
        }

        private void BtGameNameAse_Click(object sender, EventArgs e)
        {
            tbOCRCaptureApp.Text = DefaultOcrProcessNameAse;
        }

        private void BtGameNameAsa_Click(object sender, EventArgs e)
        {
            tbOCRCaptureApp.Text = DefaultOcrProcessNameAsa;
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
            if (ArkInstallationPath.GetListOfExportFolders(out (string path, string steamPlayerName)[] arkExportFolders, out string error))
            {
                var anyFolderExists = false;
                // only add folders if they exist and are not yet in the list
                var exportFolderLocations = aTExportFolderLocationsBindingSource.OfType<ATImportExportedFolderLocation>().ToList();
                foreach (var location in arkExportFolders)
                {
                    if (Directory.Exists(location.path))
                    {
                        anyFolderExists = true;
                        if (exportFolderLocations.All(f => f.FolderPath != location.path))
                            exportFolderLocations.Add(ATImportExportedFolderLocation.CreateFromString(
                                   $"{location.steamPlayerName}||{location.path}"));
                    }
                }

                if (!anyFolderExists) MessageBoxes.ShowMessageBox("No export folders found. Did you already export a creature in game?\nTo do that, walk to a creature, hold the E key and select Options - Export Data.\nThis works only on the Steam and the Epic version of the game.");

                if (!exportFolderLocations.Any()) return;

                // order the entries so that the folder with the newest file is the default
                var orderedList = ArkInstallationPath.OrderByNewestFileInFolders(exportFolderLocations.Select(l => (l.FolderPath, l)));

                aTExportFolderLocationsBindingSource.Clear();

                foreach (var iel in orderedList)
                    aTExportFolderLocationsBindingSource.Add(iel);
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
            ShowInfoGraphicPreview();
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
            for (int s = 0; s < Stats.StatsCount; s++)
                _multSetter[s].SetHighlighted(highlight);
            nudTamingSpeed.SetExtraHighlightNonDefault(highlight);
            nudDinoCharacterFoodDrain.SetExtraHighlightNonDefault(highlight);
            NudWildDinoTorporDrainMultiplier.SetExtraHighlightNonDefault(highlight);
            nudMatingSpeed.SetExtraHighlightNonDefault(highlight);
            nudMatingInterval.SetExtraHighlightNonDefault(highlight);
            nudEggHatchSpeed.SetExtraHighlightNonDefault(highlight);
            nudBabyMatureSpeed.SetExtraHighlightNonDefault(highlight);
            nudBabyCuddleInterval.SetExtraHighlightNonDefault(highlight);
            nudBabyImprintAmount.SetExtraHighlightNonDefault(highlight);
            nudBabyImprintingStatScale.SetExtraHighlightNonDefault(highlight);
            nudBabyFoodConsumptionSpeed.SetExtraHighlightNonDefault(highlight);
            HighlightCheckbox(cbSingleplayerSettings);
            HighlightCheckbox(CbAllowSpeedLeveling);
            HighlightCheckbox(CbAllowFlyerSpeedLeveling);
            HighlightCheckbox(CbAtlasSettings);

            void HighlightCheckbox(CheckBox cb, bool defaultUnchecked = true)
            {
                cb.SetBackColorAndAccordingForeColor(highlight && cb.Checked == defaultUnchecked ? Color.FromArgb(190, 40, 20) : Color.Transparent);
            }
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
        private readonly DataGridViewCellStyle _styleFolderNotFound = new DataGridViewCellStyle { BackColor = Color.FromArgb(247, 215, 211) };

        private void HighlightDefaultImportExportFolderEntry()
        {
            var rowCount = dataGridViewExportFolders.RowCount;
            if (rowCount == 0) return;

            dataGridViewExportFolders.Rows[0].DefaultCellStyle = DirectoryExists(0) ? _styleDefaultEntry : _styleFolderNotFound;
            for (int i = 1; i < rowCount; i++)
                dataGridViewExportFolders.Rows[i].DefaultCellStyle = DirectoryExists(i) ? null : _styleFolderNotFound;

            bool DirectoryExists(int r) => dataGridViewExportFolders.Rows[r].Cells[2].Value is string path &&
                                           Directory.Exists(path);
        }

        private void HighlightMissingFilesInImportSaveFileView()
        {
            var rowCount = dataGridView_FileLocations.RowCount;
            for (int i = 0; i < rowCount; i++)
            {
                dataGridView_FileLocations.Rows[i].DefaultCellStyle = dataGridView_FileLocations.Rows[i].Cells[2].Value is string filePath
                    && !filePath.StartsWith("ftp") && !File.Exists(filePath)
                    ? _styleFolderNotFound
                    : null;
            }
        }

        private void nudChartLevelEvenMin_ValueChanged(object sender, EventArgs e)
        {
            UpdateChartLevelColors(pbChartEvenRange, (int)nudChartLevelEvenMin.Value, (int)nudChartLevelEvenMax.Value);
        }

        private void nudChartLevelEvenMax_ValueChanged(object sender, EventArgs e)
        {
            UpdateChartLevelColors(pbChartEvenRange, (int)nudChartLevelEvenMin.Value, (int)nudChartLevelEvenMax.Value);
        }

        private void nudChartLevelOddMin_ValueChanged(object sender, EventArgs e)
        {
            UpdateChartLevelColors(pbChartOddRange, (int)nudChartLevelOddMin.Value, (int)nudChartLevelOddMax.Value);
        }

        private void nudChartLevelOddMax_ValueChanged(object sender, EventArgs e)
        {
            UpdateChartLevelColors(pbChartOddRange, (int)nudChartLevelOddMin.Value, (int)nudChartLevelOddMax.Value);
        }

        private void UpdateChartLevelColors(PictureBox pb, int minHue, int maxHue)
        {
            var img = new Bitmap(pb.Width, pb.Height);
            using (var g = Graphics.FromImage(img))
            using (var brush = new SolidBrush(Color.Black))
            {
                var hueRange = maxHue - minHue;
                const int segments = 10;
                var segmentWidth = img.Width / segments;
                for (int i = 0; i < segments; i++)
                {
                    brush.Color = Utils.ColorFromHue(minHue + hueRange * i / segments);
                    g.FillRectangle(brush, i * segmentWidth, 0, segmentWidth, img.Height);
                }
            }
            pb.SetImageAndDisposeOld(img);
        }

        #region InfoGraphic Preview

        private Creature _infoGraphicPreviewCreature;
        private readonly Debouncer _infoGraphicPreviewDebouncer = new Debouncer();

        private void CbInfoGraphicCheckBoxChanged(object sender, EventArgs e)
        {
            _infoGraphicPreviewDebouncer.Debounce(300, ShowInfoGraphicPreview, Dispatcher.CurrentDispatcher);
        }

        private void ShowInfoGraphicPreview()
        {
            if (_infoGraphicPreviewCreature == null)
                CreateInfoGraphicCreature();

            var speciesImage = _infoGraphicPreviewCreature?.InfoGraphic(_cc,
                (int)nudInfoGraphicHeight.Value,
                CbbInfoGraphicFontName.Text,
                BtInfoGraphicForeColor.BackColor,
                BtInfoGraphicBackColor.BackColor,
                BtInfoGraphicBorderColor.BackColor,
                CbInfoGraphicCreatureName.Checked,
                CbInfoGraphicDomLevels.Checked,
                CbInfoGraphicMutations.Checked,
                CbInfoGraphicGenerations.Checked,
                CbInfoGraphicStatValues.Checked,
                CbInfoGraphicDisplayMaxWildLevel.Checked,
                CbInfoGraphicAddRegionNames.Checked,
                CbInfoGraphicColorRegionNamesIfNoImage.Checked
            );

            if (speciesImage == null) return;

            PbInfoGraphicPreview.Size = speciesImage.Size;
            PbInfoGraphicPreview.SetImageAndDisposeOld(speciesImage);
        }
        private void BtNewRandomInfoGraphicCreature_Click(object sender, EventArgs e)
        {
            _infoGraphicPreviewCreature = null;
            ShowInfoGraphicPreview();
        }

        private void CreateInfoGraphicCreature()
        {
            _infoGraphicPreviewCreature = DummyCreatures.CreateCreatures(1)?.FirstOrDefault();
            if (_infoGraphicPreviewCreature == null) return;
            // add some dom levels
            var rand = new Random();
            _infoGraphicPreviewCreature.levelsDom[Stats.Health] = rand.Next(20);
            _infoGraphicPreviewCreature.levelsDom[Stats.Stamina] = rand.Next(20);
            _infoGraphicPreviewCreature.levelsDom[Stats.Weight] = rand.Next(20);
            _infoGraphicPreviewCreature.levelsDom[Stats.MeleeDamageMultiplier] = rand.Next(20);
            _infoGraphicPreviewCreature.RecalculateCreatureValues(_cc.wildLevelStep);
        }

        private void nudInfoGraphicHeight_ValueChanged(object sender, EventArgs e)
        {
            _infoGraphicPreviewDebouncer.Debounce(500, ShowInfoGraphicPreview, Dispatcher.CurrentDispatcher);
        }

        private void CbbInfoGraphicFontName_SelectedIndexChanged(object sender, EventArgs e)
        {
            _infoGraphicPreviewDebouncer.Debounce(300, ShowInfoGraphicPreview, Dispatcher.CurrentDispatcher);
        }

        #endregion

        private void CbNaturalSorting_CheckedChanged(object sender, EventArgs e)
        {
            var isChecked = ((CheckBox)sender).Checked;
            if (!isChecked)
                CbNaturalSortIgnoreSpaces.Checked = false;
            CbNaturalSortIgnoreSpaces.Enabled = isChecked;
        }

        private void BtImportSettingsSelectFile_Click(object sender, EventArgs e)
        {
            // import settings from text file
            using (var dlg = new OpenFileDialog
            {
                Filter = "ARK Multiplier File (*.ini)|*.ini",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                CheckFileExists = true
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                ExtractSettingsFromFile(dlg.FileName);
            }
        }

        private void CbAllowFlyerSpeedLeveling_CheckedChanged(object sender, EventArgs e)
        {
            if (CbAllowFlyerSpeedLeveling.Checked)
                CbAllowSpeedLeveling.Checked = true;
        }

        private void BtAutoImportLocalSettings_Click(object sender, EventArgs e)
        {
            // detect the game.ini and gameUserSettings.ini in the local installation and ask which to import

            if (!ArkInstallationPath.GetLocalArkConfigPaths(out (string, Ark.Game)[] localConfigPaths, out var error))
            {
                MessageBoxes.ShowMessageBox(
                    "The local Ark installation config files couldn't be found, currently auto import is only supported for the Steam edition.\nYou can try to import the files by manually drag&drop them onto the settings window\n\n"
                    + error, "Config auto import error");
                return;
            }

            localConfigPaths = localConfigPaths.OrderBy(c => c.Item2 == Ark.Game.ASE).ToArray(); // display ASA first

            // ask which configs to import
            var importIndex = Utils.ShowListInput(localConfigPaths.Select(c => $"{c.Item2}: {c.Item1.Replace("\\", "\\ ")}").ToArray(), // adding zero width spaces to allow word wrapping
                "Select one of the configs to import.", "Auto import configs", 40);
            if (importIndex == -1) return;

            ExtractSettingsFromFile(Path.Combine(localConfigPaths[importIndex].Item1, "game.ini"), true);
            ExtractSettingsFromFile(Path.Combine(localConfigPaths[importIndex].Item1, "gameUserSettings.ini"), true);

            if (localConfigPaths[importIndex].Item2 == Ark.Game.ASA) RbGameAsa.Checked = true;
            else RbGameAse.Checked = true;
        }
    }
}
