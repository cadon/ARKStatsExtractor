using ARKBreedingStats.Library;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ARKBreedingStats.settings
{
    public partial class Settings : Form
    {
        private MultiplierSetting[] multSetter;
        private readonly CreatureCollection cc;
        private ToolTip tt;
        private Dictionary<string, string> languages;
        public int LastTabPageIndex;
        public bool LanguageChanged;

        public Settings(CreatureCollection cc, int page = 0)
        {
            InitializeData();
            this.cc = cc;
            LoadSettings(cc);
            tabControlSettings.SelectTab(page);
        }

        private void InitializeData()
        {
            InitializeComponent();
            DisplayServerMultiplierPresets();
            multSetter = new MultiplierSetting[Values.STATS_COUNT];
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                multSetter[s] = new MultiplierSetting
                {
                    StatName = $"{Utils.statName(s)} [{s}]"
                };
                flowLayoutPanelStatMultipliers.Controls.Add(multSetter[s]);
            }

            // set neutral numbers for stat-multipliers to the default values to easier see what is non-default
            ServerMultipliers officialMultipliers = Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.OFFICIAL);
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s < officialMultipliers.statMultipliers.Length)
                    multSetter[s].setNeutralValues(officialMultipliers.statMultipliers[s]);
                else multSetter[s].setNeutralValues(null);
            }
            nudTamingSpeed.NeutralNumber = 1;
            nudDinoCharacterFoodDrain.NeutralNumber = 1;
            nudMatingInterval.NeutralNumber = 1;
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
            tt = new ToolTip();
            tt.SetToolTip(numericUpDownAutosaveMinutes, "To disable set to 0");
            tt.SetToolTip(chkCollectionSync, "If checked, the tool automatically reloads the library if it was changed. Use if multiple persons editing the file, e.g. via a shared folder.\nIt's recommened to check this along with \"Auto Save\"");
            tt.SetToolTip(checkBoxAutoSave, "If checked, the library is saved after each change automatically.\nIt's recommened to check this along with \"Auto Update Collection File\"");
            tt.SetToolTip(numericUpDownMaxChartLevel, "This number defines the level that is shown as maximum in the charts.\nUsually it's good to set this value to one third of the max wild level.");
            tt.SetToolTip(labelTameAdd, "PerLevelStatsMultiplier_DinoTamed_Add");
            tt.SetToolTip(labelTameAff, "PerLevelStatsMultiplier_DinoTamed_Affinity");
            tt.SetToolTip(labelWildLevel, "PerLevelStatsMultiplier_DinoWild");
            tt.SetToolTip(labelTameLevel, "PerLevelStatsMultiplier_DinoTamed");
            tt.SetToolTip(chkbSpeechRecognition, "If the overlay is enabled, you can ask via the microphone for taming-infos,\ne.g.\"Argentavis level 30\" to display basic taming-infos in the overlay");
            tt.SetToolTip(labelBabyFoodConsumptionSpeed, "BabyFoodConsumptionSpeedMultiplier");
            tt.SetToolTip(checkBoxDisplayHiddenStats, "Enable if you have the oxygen-values of all creatures, e.g. by using a mod.");
            tt.SetToolTip(labelEvent, "These values are used if the Event-Checkbox under the species-selector is selected.");
            tt.SetToolTip(cbConsiderWildLevelSteps, "Enable to sort out all level-combinations that are not possible for naturally spawned creatures.\nThe step is max-wild-level / 30 by default, e.g. with a max wildlevel of 150, only creatures with levels that are a multiple of 5 are possible (can be different with mods).\nDisable if there are creatures that have other levels, e.g. spawned in by an admin.");
            tt.SetToolTip(cbSingleplayerSettings, "Check this if you have enabled the \"Singleplayer-Settings\" in your game. This settings adjusts some of the multipliers again.");
            tt.SetToolTip(cbAllowMoreThanHundredImprinting, "Enable this if on your server more than 100% imprinting are possible, e.g. with the mod S+ with a Nanny");
            tt.SetToolTip(cbDevTools, "Shows extra tabs for multiplier-testing and extraction test-cases.");
            tt.SetToolTip(nudMaxServerLevel, "The max level allowed on the server. Currently creatures with more than 450 levels will be deleted on official servers.\nA creature that can be potentially have a higher level than this (if maximally leveled up) will be marked with a orange-red text in the library.\nSet to 0 to disable a warning in the loaded library.");
            tt.SetToolTip(lbMaxTotalLevel, "The max level allowed on the server. Currently creatures with more than 450 levels will be deleted on official servers.\nA creature that can be potentially have a higher level than this (if maximally leveled up) will be marked with a orange-red text in the library.\nSet to 0 to disable a warning in the loaded library.");

            // language
            languages = new Dictionary<string, string>
            {
                { "System language", ""},
                { Loc.s("de"), "de"},
                { Loc.s("en"), "en"},
                { Loc.s("es"), "es"},
                { Loc.s("fr"), "fr"},
                { Loc.s("it"), "it"},
            };
            foreach (string l in languages.Keys)
                cbbLanguage.Items.Add(l);
        }

        private void LoadSettings(CreatureCollection cc)
        {
            if (cc.serverMultipliers?.statMultipliers != null)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (s < cc.serverMultipliers.statMultipliers.Length && cc.serverMultipliers.statMultipliers[s].Length > 3)
                    {
                        multSetter[s].Multipliers = cc.serverMultipliers.statMultipliers[s];
                    }
                    else multSetter[s].Multipliers = null;
                }
            }
            cbSingleplayerSettings.Checked = cc.singlePlayerSettings;

            numericUpDownDomLevelNr.ValueSave = cc.maxDomLevel;
            numericUpDownMaxBreedingSug.ValueSave = cc.maxBreedingSuggestions;
            numericUpDownMaxWildLevel.ValueSave = cc.maxWildLevel;
            nudMaxServerLevel.ValueSave = cc.maxServerLevel > 0 ? cc.maxServerLevel : 0;
            numericUpDownMaxChartLevel.ValueSave = cc.maxChartLevel;
            // non-event-multipliers
            nudEggHatchSpeed.ValueSave = (decimal)cc.serverMultipliers.EggHatchSpeedMultiplier;
            nudBabyMatureSpeed.ValueSave = (decimal)cc.serverMultipliers.BabyMatureSpeedMultiplier;
            nudBabyImprintingStatScale.ValueSave = (decimal)cc.serverMultipliers.BabyImprintingStatScaleMultiplier;
            nudBabyCuddleInterval.ValueSave = (decimal)cc.serverMultipliers.BabyCuddleIntervalMultiplier;
            nudTamingSpeed.ValueSave = (decimal)cc.serverMultipliers.TamingSpeedMultiplier;
            nudDinoCharacterFoodDrain.ValueSave = (decimal)cc.serverMultipliers.DinoCharacterFoodDrainMultiplier;
            nudMatingInterval.ValueSave = (decimal)cc.serverMultipliers.MatingIntervalMultiplier;
            nudBabyFoodConsumptionSpeed.ValueSave = (decimal)cc.serverMultipliers.BabyFoodConsumptionSpeedMultiplier;
            // event-multiplier
            ServerMultipliers serverMultipliersEvent = cc.serverMultipliersEvents ?? cc.serverMultipliers;
            nudBabyCuddleIntervalEvent.ValueSave = (decimal)serverMultipliersEvent.BabyCuddleIntervalMultiplier;
            nudTamingSpeedEvent.ValueSave = (decimal)serverMultipliersEvent.TamingSpeedMultiplier;
            nudDinoCharacterFoodDrainEvent.ValueSave = (decimal)serverMultipliersEvent.DinoCharacterFoodDrainMultiplier;
            nudMatingIntervalEvent.ValueSave = (decimal)serverMultipliersEvent.MatingIntervalMultiplier;
            nudEggHatchSpeedEvent.ValueSave = (decimal)serverMultipliersEvent.EggHatchSpeedMultiplier;
            nudBabyMatureSpeedEvent.ValueSave = (decimal)serverMultipliersEvent.BabyMatureSpeedMultiplier;
            nudBabyFoodConsumptionSpeedEvent.ValueSave = (decimal)serverMultipliersEvent.BabyFoodConsumptionSpeedMultiplier;

            checkBoxAutoSave.Checked = Properties.Settings.Default.autosave;
            numericUpDownAutosaveMinutes.ValueSave = Properties.Settings.Default.autosaveMinutes;
            nudWhiteThreshold.ValueSave = Properties.Settings.Default.OCRWhiteThreshold;
            chkbSpeechRecognition.Checked = Properties.Settings.Default.SpeechRecognition;
            nudOverlayInfoDuration.ValueSave = Properties.Settings.Default.OverlayInfoDuration;
            chkCollectionSync.Checked = Properties.Settings.Default.syncCollection;
            if (Properties.Settings.Default.celsius) radioButtonCelsius.Checked = true;
            else radioButtonFahrenheit.Checked = true;
            cbIgnoreSexInBreedingPlan.Checked = Properties.Settings.Default.IgnoreSexInBreedingPlan;
            checkBoxDisplayHiddenStats.Checked = Properties.Settings.Default.oxygenForAll;
            nudWaitBeforeScreenCapture.ValueSave = Properties.Settings.Default.waitBeforeScreenCapture;

            cbShowOCRButton.Checked = Properties.Settings.Default.showOCRButton;
            string ocrApp = Properties.Settings.Default.OCRApp;
            int ocrI = cbOCRApp.Items.IndexOf(ocrApp);
            if (ocrI == -1)
            {
                textBoxOCRCustom.Text = ocrApp;
                cbOCRApp.SelectedIndex = cbOCRApp.Items.IndexOf("Custom");
            }
            else
                cbOCRApp.SelectedIndex = ocrI;

            cbOCRIgnoreImprintValue.Checked = Properties.Settings.Default.OCRIgnoresImprintValue;

            customSCStarving.SoundFile = Properties.Settings.Default.soundStarving;
            customSCWakeup.SoundFile = Properties.Settings.Default.soundWakeup;
            customSCBirth.SoundFile = Properties.Settings.Default.soundBirth;
            customSCCustom.SoundFile = Properties.Settings.Default.soundCustom;

            tbPlayAlarmsSeconds.Text = Properties.Settings.Default.playAlarmTimes;

            cbConsiderWildLevelSteps.Checked = cc.considerWildLevelSteps;
            nudWildLevelStep.ValueSave = cc.wildLevelStep;
            cbInventoryCheck.Checked = Properties.Settings.Default.inventoryCheckTimer;
            cbAllowMoreThanHundredImprinting.Checked = cc.allowMoreThanHundredImprinting;
            cbCreatureColorsLibrary.Checked = Properties.Settings.Default.showColorsInLibrary;
            cbApplyGlobalSpeciesToLibrary.Checked = Properties.Settings.Default.ApplyGlobalSpeciesToLibrary;

            // export paths
            if (Properties.Settings.Default.ExportCreatureFolders != null)
            {
                foreach (string path in Properties.Settings.Default.ExportCreatureFolders)
                {
                    aTExportFolderLocationsBindingSource.Add(ATImportExportedFolderLocation.CreateFromString(path));
                }
            }
            nudWarnImportMoreThan.Value = Properties.Settings.Default.WarnWhenImportingMoreCreaturesThan;

            // savegame paths
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

            cbDevTools.Checked = Properties.Settings.Default.DevTools;

            cbPrettifyJSON.Checked = Properties.Settings.Default.prettifyCollectionJson;

            string langKey = languages.FirstOrDefault(x => x.Value == Properties.Settings.Default.language).Key ?? "";
            int langI = cbbLanguage.Items.IndexOf(langKey);
            cbbLanguage.SelectedIndex = langI == -1 ? 0 : langI;
        }

        private void SaveSettings()
        {
            if (cc.serverMultipliers == null)
            {
                cc.serverMultipliers = new ServerMultipliers();
            }
            if (cc.serverMultipliers.statMultipliers == null)
            {
                cc.serverMultipliers.statMultipliers = new double[Values.STATS_COUNT][];
            }
            if (cc.serverMultipliers?.statMultipliers != null)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (cc.serverMultipliers.statMultipliers[s] == null)
                        cc.serverMultipliers.statMultipliers[s] = new double[4];
                    for (int sm = 0; sm < 4; sm++)
                        cc.serverMultipliers.statMultipliers[s][sm] = multSetter[s].Multipliers[sm];
                }
            }

            // Torpidity is handled differently by the game, IwM has no effect. Set IwM to 1.
            // Also see https://github.com/cadon/ARKStatsExtractor/issues/942 for more infos about this.
            cc.serverMultipliers.statMultipliers[(int)species.StatNames.Torpidity][3] = 1;

            cc.singlePlayerSettings = cbSingleplayerSettings.Checked;
            cc.maxDomLevel = (int)numericUpDownDomLevelNr.Value;
            cc.maxWildLevel = (int)numericUpDownMaxWildLevel.Value;
            cc.maxServerLevel = (int)nudMaxServerLevel.Value;
            cc.maxChartLevel = (int)numericUpDownMaxChartLevel.Value;
            cc.maxBreedingSuggestions = (int)numericUpDownMaxBreedingSug.Value;
            Properties.Settings.Default.IgnoreSexInBreedingPlan = cbIgnoreSexInBreedingPlan.Checked;
            // non-event-multiplier
            cc.serverMultipliers.TamingSpeedMultiplier = (double)nudTamingSpeed.Value;
            cc.serverMultipliers.DinoCharacterFoodDrainMultiplier = (double)nudDinoCharacterFoodDrain.Value;
            cc.serverMultipliers.MatingIntervalMultiplier = (double)nudMatingInterval.Value;
            cc.serverMultipliers.EggHatchSpeedMultiplier = (double)nudEggHatchSpeed.Value;
            cc.serverMultipliers.BabyCuddleIntervalMultiplier = (double)nudBabyCuddleInterval.Value;
            cc.serverMultipliers.BabyImprintingStatScaleMultiplier = (double)nudBabyImprintingStatScale.Value;
            cc.serverMultipliers.BabyMatureSpeedMultiplier = (double)nudBabyMatureSpeed.Value;
            cc.serverMultipliers.BabyFoodConsumptionSpeedMultiplier = (double)nudBabyFoodConsumptionSpeed.Value;
            // event-multiplier
            if (cc.serverMultipliersEvents == null) cc.serverMultipliersEvents = new ServerMultipliers();
            cc.serverMultipliersEvents.TamingSpeedMultiplier = (double)nudTamingSpeedEvent.Value;
            cc.serverMultipliersEvents.DinoCharacterFoodDrainMultiplier = (double)nudDinoCharacterFoodDrainEvent.Value;
            cc.serverMultipliersEvents.MatingIntervalMultiplier = (double)nudMatingIntervalEvent.Value;
            cc.serverMultipliersEvents.EggHatchSpeedMultiplier = (double)nudEggHatchSpeedEvent.Value;
            cc.serverMultipliersEvents.BabyCuddleIntervalMultiplier = (double)nudBabyCuddleIntervalEvent.Value;
            cc.serverMultipliersEvents.BabyImprintingStatScaleMultiplier = (double)nudBabyImprintingStatScale.Value;
            cc.serverMultipliersEvents.BabyMatureSpeedMultiplier = (double)nudBabyMatureSpeedEvent.Value;
            cc.serverMultipliersEvents.BabyFoodConsumptionSpeedMultiplier = (double)nudBabyFoodConsumptionSpeedEvent.Value;

            Properties.Settings.Default.autosave = checkBoxAutoSave.Checked;
            Properties.Settings.Default.autosaveMinutes = (int)numericUpDownAutosaveMinutes.Value;
            Properties.Settings.Default.OCRWhiteThreshold = (int)nudWhiteThreshold.Value;
            Properties.Settings.Default.SpeechRecognition = chkbSpeechRecognition.Checked;
            Properties.Settings.Default.OverlayInfoDuration = (int)nudOverlayInfoDuration.Value;
            Properties.Settings.Default.syncCollection = chkCollectionSync.Checked;
            Properties.Settings.Default.celsius = radioButtonCelsius.Checked;
            Properties.Settings.Default.oxygenForAll = checkBoxDisplayHiddenStats.Checked;
            Properties.Settings.Default.waitBeforeScreenCapture = (int)nudWaitBeforeScreenCapture.Value;

            Properties.Settings.Default.showOCRButton = cbShowOCRButton.Checked;
            string ocrApp = cbOCRApp.SelectedItem.ToString();
            if (ocrApp == "Custom")
                ocrApp = textBoxOCRCustom.Text;
            Properties.Settings.Default.OCRApp = ocrApp;

            Properties.Settings.Default.OCRIgnoresImprintValue = cbOCRIgnoreImprintValue.Checked;

            Properties.Settings.Default.soundStarving = customSCStarving.SoundFile;
            Properties.Settings.Default.soundWakeup = customSCWakeup.SoundFile;
            Properties.Settings.Default.soundBirth = customSCBirth.SoundFile;
            Properties.Settings.Default.soundCustom = customSCCustom.SoundFile;

            Properties.Settings.Default.playAlarmTimes = tbPlayAlarmsSeconds.Text;

            cc.considerWildLevelSteps = cbConsiderWildLevelSteps.Checked;
            cc.wildLevelStep = (int)nudWildLevelStep.Value;
            Properties.Settings.Default.inventoryCheckTimer = cbInventoryCheck.Checked;
            cc.allowMoreThanHundredImprinting = cbAllowMoreThanHundredImprinting.Checked;
            Properties.Settings.Default.showColorsInLibrary = cbCreatureColorsLibrary.Checked;
            Properties.Settings.Default.ApplyGlobalSpeciesToLibrary = cbApplyGlobalSpeciesToLibrary.Checked;

            //import savegame
            Properties.Settings.Default.savegameExtractionPath = fileSelectorExtractedSaveFolder.Link;
            Properties.Settings.Default.arkSavegamePaths = aTImportFileLocationBindingSource.OfType<ATImportFileLocation>()
                    .Where(location => !string.IsNullOrWhiteSpace(location.FileLocation))
                    .Select(location => $"{location.ConvenientName}|{location.ServerName}|{location.FileLocation}").ToArray();

            Properties.Settings.Default.WarnWhenImportingMoreCreaturesThan = (int)nudWarnImportMoreThan.Value;

            // import exported
            Properties.Settings.Default.ExportCreatureFolders = aTExportFolderLocationsBindingSource.OfType<ATImportExportedFolderLocation>()
                    .Where(location => !string.IsNullOrWhiteSpace(location.FolderPath))
                    .Select(location => $"{location.ConvenientName}|{location.OwnerSuffix}|{location.FolderPath}").ToArray();

            cc.changeCreatureStatusOnSavegameImport = cbImportUpdateCreatureStatus.Checked;
            Properties.Settings.Default.ImportTribeNameFilter = textBoxImportTribeNameFilter.Text;

            Properties.Settings.Default.DevTools = cbDevTools.Checked;

            Properties.Settings.Default.prettifyCollectionJson = cbPrettifyJSON.Checked;

            string oldLanguageSetting = Properties.Settings.Default.language;
            string lang = cbbLanguage.SelectedItem.ToString();
            Properties.Settings.Default.language = languages.ContainsKey(lang) ? languages[lang] : "";
            LanguageChanged = oldLanguageSetting != Properties.Settings.Default.language;

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
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void tabPage2_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) ExtractSettingsFromFile(file);
        }

        private void ExtractSettingsFromFile(string file)
        {
            if (!File.Exists(file))
                return;

            string text = File.ReadAllText(file);
            double d;
            Match m;

            // get stat-multipliers
            // if there are stat-multipliers, set all to the official-values first
            if (text.IndexOf("PerLevelStatsMultiplier_Dino") != -1)
                ApplyMultiplierPreset(Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.OFFICIAL), onlyStatMultipliers: true);

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                m = Regex.Match(text, @"PerLevelStatsMultiplier_DinoTamed_Add\[" + s + @"\] ?= ?(\d*\.?\d+)");
                double[] multipliers;
                if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
                {
                    multipliers = multSetter[s].Multipliers;
                    multipliers[0] = d == 0 ? 1 : d;
                    multSetter[s].Multipliers = multipliers;
                }
                m = Regex.Match(text, @"PerLevelStatsMultiplier_DinoTamed_Affinity\[" + s + @"\] ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
                {
                    multipliers = multSetter[s].Multipliers;
                    multipliers[1] = d == 0 ? 1 : d;
                    multSetter[s].Multipliers = multipliers;
                }
                m = Regex.Match(text, @"PerLevelStatsMultiplier_DinoTamed\[" + s + @"\] ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
                {
                    multipliers = multSetter[s].Multipliers;
                    multipliers[2] = d == 0 ? 1 : d;
                    multSetter[s].Multipliers = multipliers;
                }
                m = Regex.Match(text, @"PerLevelStatsMultiplier_DinoWild\[" + s + @"\] ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
                {
                    multipliers = multSetter[s].Multipliers;
                    multipliers[3] = d == 0 ? 1 : d;
                    multSetter[s].Multipliers = multipliers;
                }
            }

            m = Regex.Match(text, @"MatingIntervalMultiplier ?= ?(\d*\.?\d+)");
            if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
            {
                nudMatingInterval.ValueSave = (decimal)d;
            }

            m = Regex.Match(text, @"EggHatchSpeedMultiplier ?= ?(\d*\.?\d+)");
            if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
            {
                nudEggHatchSpeed.ValueSave = (decimal)d;
            }
            m = Regex.Match(text, @"BabyMatureSpeedMultiplier ?= ?(\d*\.?\d+)");
            if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
            {
                nudBabyMatureSpeed.ValueSave = (decimal)d;
            }
            m = Regex.Match(text, @"BabyImprintingStatScaleMultiplier ?= ?(\d*\.?\d+)");
            if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
            {
                nudBabyImprintingStatScale.ValueSave = (decimal)d;
            }
            m = Regex.Match(text, @"BabyCuddleIntervalMultiplier ?= ?(\d*\.?\d+)");
            if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
            {
                nudBabyCuddleInterval.ValueSave = (decimal)d;
            }
            m = Regex.Match(text, @"BabyFoodConsumptionSpeedMultiplier ?= ?(\d*\.?\d+)");
            if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
            {
                nudBabyFoodConsumptionSpeed.ValueSave = (decimal)d;
            }
            m = Regex.Match(text, @"bUseSingleplayerSettings ?= ?(true|false)", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                cbSingleplayerSettings.Checked = (m.Groups[1].Value.ToLower() == "true");
            }
            m = Regex.Match(text, @"DestroyTamesOverLevelClamp ?= ?(\d+)");
            if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
            {
                nudMaxServerLevel.ValueSave = (decimal)d;
            }

            // GameUserSettings.ini

            m = Regex.Match(text, @"TamingSpeedMultiplier ?= ?(\d*\.?\d+)");
            if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
            {
                nudTamingSpeed.ValueSave = (decimal)d;
            }
            m = Regex.Match(text, @"DinoCharacterFoodDrainMultiplier ?= ?(\d*\.?\d+)");
            if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
            {
                nudDinoCharacterFoodDrain.ValueSave = (decimal)d;
            }
        }

        private void cbOCRApp_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxOCRCustom.Visible = cbOCRApp.SelectedItem.ToString() == "Custom";
        }

        private void Settings_Disposed(object sender, EventArgs e)
        {
            tt.RemoveAll();
            tt.Dispose();
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
        /// <param name="sm"></param>
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
                multSetter[s].Multipliers = sm.statMultipliers[s];
            }
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            LastTabPageIndex = tabControlSettings.SelectedIndex;
        }
    }
}
