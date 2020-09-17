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
            this._cc = cc;
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
            cbbOCRApp.DataSource = System.Diagnostics.Process.GetProcesses().Select(p => new ProcessSelector { ProcessName = p.ProcessName, MainWindowTitle = p.MainWindowTitle })
                .Distinct().Where(pn => !string.IsNullOrEmpty(pn.MainWindowTitle) && pn.ProcessName != "System" && pn.ProcessName != "idle").OrderBy(pn => pn.ProcessName).ToArray();
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
            ServerMultipliers officialMultipliers = Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.OFFICIAL);
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s < officialMultipliers.statMultipliers.Length)
                    _multSetter[s].setNeutralValues(officialMultipliers.statMultipliers[s]);
                else _multSetter[s].setNeutralValues(null);
            }
            nudTamingSpeed.NeutralNumber = 1;
            nudDinoCharacterFoodDrain.NeutralNumber = 1;
            nudMatingInterval.NeutralNumber = 1;
            nudMatingSpeed.NeutralNumber = 1;
            nudEggHatchSpeed.NeutralNumber = 1;
            nudBabyMatureSpeed.NeutralNumber = 1;
            nudBabyCuddleInterval.NeutralNumber = 1;
            nudBabyImprintingStatScale.NeutralNumber = 1;
            nudBabyFoodConsumptionSpeed.NeutralNumber = 1;
            // event
            nudTamingSpeedEvent.NeutralNumber = 1.5M;
            nudDinoCharacterFoodDrainEvent.NeutralNumber = 1;
            nudMatingIntervalEvent.NeutralNumber = 1;
            nudEggHatchSpeedEvent.NeutralNumber = 1;
            nudBabyMatureSpeedEvent.NeutralNumber = 1;
            nudBabyCuddleIntervalEvent.NeutralNumber = 1;
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
            _tt.SetToolTip(numericUpDownAutosaveMinutes, "To disable set to 0");
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
            _tt.SetToolTip(lbMaxTotalLevel, "The max level allowed on the server. Currently creatures with more than 450 levels will be deleted on official servers.\nA creature that can be potentially have a higher level than this (if maximally leveled up) will be marked with a orange-red text in the library.\nSet to 0 to disable a warning in the loaded library.");

            // localizations / translations
            // for a new translation
            // * a file local/strings.[languageCode].resx needs to exist.
            // * that file needs to be added to the installer files, for that edit the file setup.iss and setup-debug.iss in the repository base folder.
            // * the entry in the next dictionary needs to be added
            _languages = new Dictionary<string, string>
            {
                { Loc.S("SystemLanguage"), string.Empty},
                { "Deutsch", "de"},
                { "English", "en"},
                { "Español", "es"},
                { "Français", "fr"},
                { "Italiano", "it"},
                { "中文", "zh"},
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
            nudTamingSpeed.ValueSave = (decimal)multipliers.TamingSpeedMultiplier;
            nudDinoCharacterFoodDrain.ValueSave = (decimal)multipliers.DinoCharacterFoodDrainMultiplier;
            nudBabyFoodConsumptionSpeed.ValueSave = (decimal)multipliers.BabyFoodConsumptionSpeedMultiplier;
            #endregion
            #region event-multiplier
            multipliers = cc.serverMultipliersEvents ?? multipliers;
            nudBabyCuddleIntervalEvent.ValueSave = (decimal)multipliers.BabyCuddleIntervalMultiplier;
            nudTamingSpeedEvent.ValueSave = (decimal)multipliers.TamingSpeedMultiplier;
            nudDinoCharacterFoodDrainEvent.ValueSave = (decimal)multipliers.DinoCharacterFoodDrainMultiplier;
            nudMatingIntervalEvent.ValueSave = (decimal)multipliers.MatingIntervalMultiplier;
            nudEggHatchSpeedEvent.ValueSave = (decimal)multipliers.EggHatchSpeedMultiplier;
            nudBabyMatureSpeedEvent.ValueSave = (decimal)multipliers.BabyMatureSpeedMultiplier;
            nudBabyFoodConsumptionSpeedEvent.ValueSave = (decimal)multipliers.BabyFoodConsumptionSpeedMultiplier;
            #endregion

            checkBoxAutoSave.Checked = Properties.Settings.Default.autosave;
            numericUpDownAutosaveMinutes.ValueSave = Properties.Settings.Default.autosaveMinutes;
            chkbSpeechRecognition.Checked = Properties.Settings.Default.SpeechRecognition;
            chkCollectionSync.Checked = Properties.Settings.Default.syncCollection;
            if (Properties.Settings.Default.celsius) radioButtonCelsius.Checked = true;
            else radioButtonFahrenheit.Checked = true;
            cbIgnoreSexInBreedingPlan.Checked = Properties.Settings.Default.IgnoreSexInBreedingPlan;
            checkBoxDisplayHiddenStats.Checked = Properties.Settings.Default.DisplayHiddenStats;
            tbDefaultFontName.Text = Properties.Settings.Default.DefaultFontName;
            nudDefaultFontSize.Value = (decimal)Properties.Settings.Default.DefaultFontSize;

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

            #region library
            cbCreatureColorsLibrary.Checked = Properties.Settings.Default.showColorsInLibrary;
            cbApplyGlobalSpeciesToLibrary.Checked = Properties.Settings.Default.ApplyGlobalSpeciesToLibrary;
            CbLibrarySelectSelectedSpeciesOnLoad.Checked = Properties.Settings.Default.LibrarySelectSelectedSpeciesOnLoad;
            cbLibraryHighlightTopCreatures.Checked = Properties.Settings.Default.LibraryHighlightTopCreatures;
            #endregion

            #region import exported
            if (Properties.Settings.Default.ExportCreatureFolders != null)
            {
                foreach (string path in Properties.Settings.Default.ExportCreatureFolders)
                {
                    aTExportFolderLocationsBindingSource.Add(ATImportExportedFolderLocation.CreateFromString(path));
                }
            }
            nudWarnImportMoreThan.Value = Properties.Settings.Default.WarnWhenImportingMoreCreaturesThan;
            cbApplyNamePatternOnImportOnEmptyNames.Checked = Properties.Settings.Default.applyNamePatternOnImportIfEmptyName;
            cbApplyNamePatternOnImportOnNewCreatures.Checked = Properties.Settings.Default.applyNamePatternOnAutoImportForNewCreatures;
            cbCopyPatternNameToClipboard.Checked = Properties.Settings.Default.copyNameToClipboardOnImportWhenAutoNameApplied;
            cbAutoImportExported.Checked = Properties.Settings.Default.AutoImportExportedCreatures;
            cbPlaySoundOnAutomaticImport.Checked = Properties.Settings.Default.PlaySoundOnAutoImport;
            cbMoveImportedFileToSubFolder.Checked = Properties.Settings.Default.MoveAutoImportedFileToSubFolder;
            SetImportExportArchiveFolder(Properties.Settings.Default.ImportExportedArchiveFolder);
            cbDeleteAutoImportedFile.Checked = Properties.Settings.Default.DeleteAutoImportedFile;
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
            _cc.maxBreedingSuggestions = (int)numericUpDownMaxBreedingSug.Value;
            Properties.Settings.Default.IgnoreSexInBreedingPlan = cbIgnoreSexInBreedingPlan.Checked;

            #region non-event-multiplier
            _cc.serverMultipliers.TamingSpeedMultiplier = (double)nudTamingSpeed.Value;
            _cc.serverMultipliers.DinoCharacterFoodDrainMultiplier = (double)nudDinoCharacterFoodDrain.Value;
            _cc.serverMultipliers.MatingSpeedMultiplier = (double)nudMatingSpeed.Value;
            _cc.serverMultipliers.MatingIntervalMultiplier = (double)nudMatingInterval.Value;
            _cc.serverMultipliers.EggHatchSpeedMultiplier = (double)nudEggHatchSpeed.Value;
            _cc.serverMultipliers.BabyCuddleIntervalMultiplier = (double)nudBabyCuddleInterval.Value;
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
            _cc.serverMultipliersEvents.BabyImprintingStatScaleMultiplier = (double)nudBabyImprintingStatScale.Value;
            _cc.serverMultipliersEvents.BabyMatureSpeedMultiplier = (double)nudBabyMatureSpeedEvent.Value;
            _cc.serverMultipliersEvents.BabyFoodConsumptionSpeedMultiplier = (double)nudBabyFoodConsumptionSpeedEvent.Value;
            #endregion

            Properties.Settings.Default.autosave = checkBoxAutoSave.Checked;
            Properties.Settings.Default.autosaveMinutes = (int)numericUpDownAutosaveMinutes.Value;
            Properties.Settings.Default.SpeechRecognition = chkbSpeechRecognition.Checked;
            Properties.Settings.Default.syncCollection = chkCollectionSync.Checked;
            Properties.Settings.Default.celsius = radioButtonCelsius.Checked;
            Properties.Settings.Default.DisplayHiddenStats = checkBoxDisplayHiddenStats.Checked;
            Properties.Settings.Default.DefaultFontName = tbDefaultFontName.Text;
            Properties.Settings.Default.DefaultFontSize = (float)nudDefaultFontSize.Value;

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
            Properties.Settings.Default.OCRWhiteThreshold = (int)nudWhiteThreshold.Value;
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

            #region library
            Properties.Settings.Default.showColorsInLibrary = cbCreatureColorsLibrary.Checked;
            Properties.Settings.Default.ApplyGlobalSpeciesToLibrary = cbApplyGlobalSpeciesToLibrary.Checked;
            Properties.Settings.Default.LibrarySelectSelectedSpeciesOnLoad = CbLibrarySelectSelectedSpeciesOnLoad.Checked;
            Properties.Settings.Default.LibraryHighlightTopCreatures = cbLibraryHighlightTopCreatures.Checked;
            #endregion

            #region import exported
            Properties.Settings.Default.WarnWhenImportingMoreCreaturesThan = (int)nudWarnImportMoreThan.Value;
            Properties.Settings.Default.ExportCreatureFolders = aTExportFolderLocationsBindingSource.OfType<ATImportExportedFolderLocation>()
                    .Where(location => !string.IsNullOrWhiteSpace(location.FolderPath))
                    .Select(location => $"{location.ConvenientName}|{location.OwnerSuffix}|{location.FolderPath}").ToArray();

            Properties.Settings.Default.applyNamePatternOnImportIfEmptyName = cbApplyNamePatternOnImportOnEmptyNames.Checked;
            Properties.Settings.Default.applyNamePatternOnAutoImportForNewCreatures = cbApplyNamePatternOnImportOnNewCreatures.Checked;
            Properties.Settings.Default.copyNameToClipboardOnImportWhenAutoNameApplied = cbCopyPatternNameToClipboard.Checked;
            Properties.Settings.Default.AutoImportExportedCreatures = cbAutoImportExported.Checked;
            Properties.Settings.Default.PlaySoundOnAutoImport = cbPlaySoundOnAutomaticImport.Checked;
            Properties.Settings.Default.MoveAutoImportedFileToSubFolder = cbMoveImportedFileToSubFolder.Checked;
            Properties.Settings.Default.ImportExportedArchiveFolder = BtImportArchiveFolder.Tag as string;
            Properties.Settings.Default.DeleteAutoImportedFile = cbDeleteAutoImportedFile.Checked;
            Properties.Settings.Default.ImportLowerBoundTE = (double)nudImportLowerBoundTE.Value / 100;

            _cc.changeCreatureStatusOnSavegameImport = cbImportUpdateCreatureStatus.Checked;
            Properties.Settings.Default.ImportTribeNameFilter = textBoxImportTribeNameFilter.Text;
            Properties.Settings.Default.ImportExportUseTamerStringForOwner = RbTamerStringForOwner.Checked;
            #endregion

            #region import savegame
            Properties.Settings.Default.savegameExtractionPath = fileSelectorExtractedSaveFolder.Link;
            Properties.Settings.Default.arkSavegamePaths = aTImportFileLocationBindingSource.OfType<ATImportFileLocation>()
                    .Where(location => !string.IsNullOrWhiteSpace(location.FileLocation))
                    .Select(location => $"{location.ConvenientName}|{location.ServerName}|{location.FileLocation}").ToArray();

            Properties.Settings.Default.IgnoreUnknownBlueprintsOnSaveImport = cbIgnoreUnknownBPOnSaveImport.Checked;
            Properties.Settings.Default.SaveImportCryo = cbSaveImportCryo.Checked;
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
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void checkBoxAutoSave_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownAutosaveMinutes.Enabled = checkBoxAutoSave.Checked;
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
            text = Regex.Replace(text, @"(?:\A|[\r\n]+);[^\r\n]*", "");

            double d;
            Match m;
            var cultureForStrings = System.Globalization.CultureInfo.GetCultureInfo("en-US");

            // get stat-multipliers
            // if there are stat-multipliers, set all to the official-values first
            if (text.Contains("PerLevelStatsMultiplier_Dino"))
                ApplyMultiplierPreset(Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.OFFICIAL), onlyStatMultipliers: true);

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                ParseAndSetStatMultiplier(0, @"PerLevelStatsMultiplier_DinoTamed_Add\[" + s + @"\] ?= ?(\d*\.?\d+)");
                ParseAndSetStatMultiplier(1, @"PerLevelStatsMultiplier_DinoTamed_Affinity\[" + s + @"\] ?= ?(\d*\.?\d+)");
                ParseAndSetStatMultiplier(2, @"PerLevelStatsMultiplier_DinoTamed\[" + s + @"\] ?= ?(\d*\.?\d+)");
                ParseAndSetStatMultiplier(3, @"PerLevelStatsMultiplier_DinoWild\[" + s + @"\] ?= ?(\d*\.?\d+)");

                void ParseAndSetStatMultiplier(int multiplierIndex, string regexPattern)
                {
                    m = Regex.Match(text, regexPattern);
                    if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, cultureForStrings, out d))
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

            //// the settings below are not settings that appear in ARK server config files and are used only in ASB
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
            ParseAndSetValue(nudBabyFoodConsumptionSpeedEvent, @"ASBEvent_BabyFoodConsumptionSpeedMultiplier ?= ?(\d*\.?\d+)");
            // event multipliers taming
            ParseAndSetValue(nudTamingSpeedEvent, @"ASBEvent_TamingSpeedMultiplier ?= ?(\d*\.?\d+)");
            ParseAndSetValue(nudDinoCharacterFoodDrainEvent, @"ASBEvent_DinoCharacterFoodDrainMultiplier ?= ?(\d*\.?\d+)");

            bool ParseAndSetValue(uiControls.Nud nud, string regexPattern)
            {
                m = Regex.Match(text, regexPattern);
                if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, cultureForStrings, out d))
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
        }

        private void Settings_Disposed(object sender, EventArgs e)
        {
            _tt.RemoveAll();
            _tt.Dispose();
        }

        private void buttonAllTBMultipliersOne_Click(object sender, EventArgs e)
        {
            SetBreedingTamingToOne();
        }

        private void SetBreedingTamingToOne()
        {
            nudTamingSpeed.ValueSave = 1;
            nudDinoCharacterFoodDrain.ValueSave = 1;
            nudMatingInterval.ValueSave = 1;
            nudEggHatchSpeed.ValueSave = 1;
            nudBabyMatureSpeed.ValueSave = 1;
            nudBabyImprintingStatScale.ValueSave = 1;
            nudBabyCuddleInterval.ValueSave = 1;
            nudBabyFoodConsumptionSpeed.ValueSave = 1;
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
                if (!string.IsNullOrWhiteSpace(sm) && sm != "singleplayer")
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
            ApplyMultiplierPreset(Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.OFFICIAL));
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
                MessageBox.Show("Error while writing settings file:\n\n" + ex.Message, $"File writing error - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public enum SettingsTabPages
        {
            Unknown = -1,
            Multipliers = 0,
            General = 1,
            SaveImport = 2,
            ExportedImport = 3,
            OCR = 4,
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
            Utils.BeepSignal(0);
        }

        private void BtBeepSuccess_Click(object sender, EventArgs e)
        {
            Utils.BeepSignal(1);
        }

        private void BtBeepTop_Click(object sender, EventArgs e)
        {
            Utils.BeepSignal(2);
        }

        private void BtBeepNewTop_Click(object sender, EventArgs e)
        {
            Utils.BeepSignal(3);
        }

        private void BtImportArchiveFolder_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                // get folder of first export path
                var exportFolder = BtImportArchiveFolder.Tag is string lastFolder && !string.IsNullOrEmpty(lastFolder) ? lastFolder
                    : aTExportFolderLocationsBindingSource.OfType<ATImportExportedFolderLocation>()
                     .Where(location => !string.IsNullOrWhiteSpace(location.FolderPath))
                     .Select(location => location.FolderPath).FirstOrDefault();

                dlg.RootFolder = Environment.SpecialFolder.Desktop;
                if (exportFolder != null && Directory.Exists(exportFolder))
                    dlg.SelectedPath = exportFolder;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    SetImportExportArchiveFolder(dlg.SelectedPath);
                }
            }
        }

        private void SetImportExportArchiveFolder(string folderPath)
        {
            BtImportArchiveFolder.Text = string.IsNullOrEmpty(folderPath) ? "…" : Path.GetFileName(folderPath);
            BtImportArchiveFolder.Tag = folderPath;
            _tt.SetToolTip(BtImportArchiveFolder, folderPath);
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
    }
}
