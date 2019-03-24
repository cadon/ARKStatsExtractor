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

        public bool WildMaxChanged; // is needed for the speech-recognition, if wildMax is changed, the grammar has to be rebuilt
        public bool LanguageChanged;

        public Settings(CreatureCollection cc, int page = 0)
        {
            initStuff();
            this.cc = cc;
            loadSettings(cc);
            tabControlSettings.SelectTab(page);
        }

        private void initStuff()
        {
            InitializeComponent();
            multSetter = new[] { multiplierSettingHP, multiplierSettingSt, multiplierSettingOx, multiplierSettingFo, multiplierSettingWe, multiplierSettingDm, multiplierSettingSp, multiplierSettingTo };
            int[] serverStatIndices = { 0, 1, 3, 4, 7, 8, 9, 2 };
            for (int s = 0; s < 8; s++)
            {
                multSetter[s].StatName = $"{Utils.statName(s)} [{serverStatIndices[s]}]";
            }

            // set neutral numbers for stat-multipliers to the default values to easier see what is non-default

            for (int s = 0; s < 8; s++)
            {
                multSetter[s].setNeutralValues(Values.V.statMultipliers[s]);
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
            WildMaxChanged = false;
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
            tt.SetToolTip(checkBoxOxygenForAll, "Enable if you have the oxygen-values of all creatures, e.g. by using a mod.");
            tt.SetToolTip(labelEvent, "These values are used if the Event-Checkbox under the species-selector is selected.");
            tt.SetToolTip(cbConsiderWildLevelSteps, "Enable to sort out all level-combinations that are not possible for naturally spawned creatures.\nThe step is max-wild-level / 30 by default, e.g. with a max wildlevel of 150, only creatures with levels that are a multiple of 5 are possible (can be different with mods).\nDisable if there are creatures that have other levels, e.g. spawned in by an admin.");
            tt.SetToolTip(cbSingleplayerSettings, "Check this if you have enabled the \"Singleplayer-Settings\" in your game. This settings adjusts some of the multipliers again.");
            tt.SetToolTip(buttonSetToOfficialMP, "Set all stat-multipliers to the default values");
            tt.SetToolTip(cbAllowMoreThanHundredImprinting, "Enable this if on your server more than 100% imprinting are possible, e.g. with the mod S+ with a Nanny");
            tt.SetToolTip(cbDevTools, "Shows extra tabs for multiplier-testing and extraction test-cases.");
            tt.SetToolTip(nudMaxServerLevel, "The max level allowed on the server. Currently creatures with more than 450 levels will be deleted on official servers.\nA creature that can be potentially have a higher level than this (if maximally leveled up) will be marked with a orange-red text in the library.\nSet to 0 to disable a warning in the loaded library.");
            tt.SetToolTip(lbMaxTotalLevel, "The max level allowed on the server. Currently creatures with more than 450 levels will be deleted on official servers.\nA creature that can be potentially have a higher level than this (if maximally leveled up) will be marked with a orange-red text in the library.\nSet to 0 to disable a warning in the loaded library.");

            // language
            languages = new Dictionary<string, string>
            {
                { "System language", ""},
                { Loc.s("en"), "en"},
                { Loc.s("de"), "de"},
                { Loc.s("fr"), "fr"}
            };
            foreach (string l in languages.Keys)
                cbbLanguage.Items.Add(l);
        }

        private void loadSettings(CreatureCollection cc)
        {
            if (cc.multipliers.Length > 7)
            {
                for (int s = 0; s < 8; s++)
                {
                    if (cc.multipliers[s].Length > 3)
                    {
                        multSetter[s].Multipliers = cc.multipliers[s];
                    }
                }
            }
            cbSingleplayerSettings.Checked = cc.singlePlayerSettings;

            nudEggHatchSpeed.ValueSave = (decimal)cc.EggHatchSpeedMultiplier;
            nudBabyMatureSpeed.ValueSave = (decimal)cc.BabyMatureSpeedMultiplier;
            numericUpDownDomLevelNr.ValueSave = cc.maxDomLevel;
            numericUpDownMaxBreedingSug.ValueSave = cc.maxBreedingSuggestions;
            numericUpDownMaxWildLevel.ValueSave = cc.maxWildLevel;
            nudMaxServerLevel.ValueSave = cc.maxServerLevel > 0 ? cc.maxServerLevel : 0;
            numericUpDownMaxChartLevel.ValueSave = cc.maxChartLevel;
            nudBabyImprintingStatScale.ValueSave = (decimal)cc.imprintingMultiplier;
            nudBabyCuddleInterval.ValueSave = (decimal)cc.babyCuddleIntervalMultiplier;
            nudTamingSpeed.ValueSave = (decimal)cc.tamingSpeedMultiplier;
            nudDinoCharacterFoodDrain.ValueSave = (decimal)cc.tamingFoodRateMultiplier;
            nudMatingInterval.ValueSave = (decimal)cc.MatingIntervalMultiplier;
            nudBabyFoodConsumptionSpeed.ValueSave = (decimal)cc.BabyFoodConsumptionSpeedMultiplier;
            // event-multiplier
            nudBabyCuddleIntervalEvent.ValueSave = (decimal)cc.babyCuddleIntervalMultiplierEvent;
            nudTamingSpeedEvent.ValueSave = (decimal)cc.tamingSpeedMultiplierEvent;
            nudDinoCharacterFoodDrainEvent.ValueSave = (decimal)cc.tamingFoodRateMultiplierEvent;
            nudMatingIntervalEvent.ValueSave = (decimal)cc.MatingIntervalMultiplierEvent;
            nudEggHatchSpeedEvent.ValueSave = (decimal)cc.EggHatchSpeedMultiplierEvent;
            nudBabyMatureSpeedEvent.ValueSave = (decimal)cc.BabyMatureSpeedMultiplierEvent;
            nudBabyFoodConsumptionSpeedEvent.ValueSave = (decimal)cc.BabyFoodConsumptionSpeedMultiplierEvent;

            checkBoxAutoSave.Checked = Properties.Settings.Default.autosave;
            numericUpDownAutosaveMinutes.ValueSave = Properties.Settings.Default.autosaveMinutes;
            nudWhiteThreshold.ValueSave = Properties.Settings.Default.OCRWhiteThreshold;
            chkbSpeechRecognition.Checked = Properties.Settings.Default.SpeechRecognition;
            nudOverlayInfoDuration.ValueSave = Properties.Settings.Default.OverlayInfoDuration;
            chkCollectionSync.Checked = Properties.Settings.Default.syncCollection;
            if (Properties.Settings.Default.celsius) radioButtonCelsius.Checked = true;
            else radioButtonFahrenheit.Checked = true;
            cbIgnoreSexInBreedingPlan.Checked = Properties.Settings.Default.IgnoreSexInBreedingPlan;
            checkBoxOxygenForAll.Checked = Properties.Settings.Default.oxygenForAll;
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

            // savegame paths
            if (Properties.Settings.Default.arkSavegamePaths != null)
            {
                foreach (string path in Properties.Settings.Default.arkSavegamePaths)
                {
                    aTImportFileLocationBindingSource.Add(ATImportFileLocation.CreateFromString(path));
                }
            }
            fileSelectorExtractedSaveFolder.Link = Properties.Settings.Default.savegameExtractionPath;

            cbImportUpdateCreatureStatus.Checked = Properties.Settings.Default.importChangeCreatureStatus;
            textBoxImportTribeNameFilter.Text = Properties.Settings.Default.ImportTribeNameFilter;

            cbDevTools.Checked = Properties.Settings.Default.DevTools;

            string langKey = languages.FirstOrDefault(x => x.Value == Properties.Settings.Default.language).Key ?? "";
            int langI = cbbLanguage.Items.IndexOf(langKey);
            cbbLanguage.SelectedIndex = langI == -1 ? 0 : langI;
        }

        private void saveSettings()
        {
            for (int s = 0; s < 8; s++)
            {
                for (int sm = 0; sm < 4; sm++)
                    cc.multipliers[s][sm] = multSetter[s].Multipliers[sm];
            }

            cc.singlePlayerSettings = cbSingleplayerSettings.Checked;
            cc.EggHatchSpeedMultiplier = (double)nudEggHatchSpeed.Value;
            cc.BabyMatureSpeedMultiplier = (double)nudBabyMatureSpeed.Value;
            cc.maxDomLevel = (int)numericUpDownDomLevelNr.Value;
            WildMaxChanged = WildMaxChanged || (cc.maxWildLevel != (int)numericUpDownMaxWildLevel.Value);
            cc.maxWildLevel = (int)numericUpDownMaxWildLevel.Value;
            cc.maxServerLevel = (int)nudMaxServerLevel.Value;
            cc.maxChartLevel = (int)numericUpDownMaxChartLevel.Value;
            cc.maxBreedingSuggestions = (int)numericUpDownMaxBreedingSug.Value;
            Properties.Settings.Default.IgnoreSexInBreedingPlan = cbIgnoreSexInBreedingPlan.Checked;
            cc.imprintingMultiplier = (double)nudBabyImprintingStatScale.Value;
            cc.babyCuddleIntervalMultiplier = (double)nudBabyCuddleInterval.Value;
            cc.tamingSpeedMultiplier = (double)nudTamingSpeed.Value;
            cc.tamingFoodRateMultiplier = (double)nudDinoCharacterFoodDrain.Value;
            cc.MatingIntervalMultiplier = (double)nudMatingInterval.Value;
            cc.BabyFoodConsumptionSpeedMultiplier = (double)nudBabyFoodConsumptionSpeed.Value;
            // event-multiplier
            cc.babyCuddleIntervalMultiplierEvent = (double)nudBabyCuddleIntervalEvent.Value;
            cc.tamingSpeedMultiplierEvent = (double)nudTamingSpeedEvent.Value;
            cc.tamingFoodRateMultiplierEvent = (double)nudDinoCharacterFoodDrainEvent.Value;
            cc.MatingIntervalMultiplierEvent = (double)nudMatingIntervalEvent.Value;
            cc.EggHatchSpeedMultiplierEvent = (double)nudEggHatchSpeedEvent.Value;
            cc.BabyMatureSpeedMultiplierEvent = (double)nudBabyMatureSpeedEvent.Value;
            cc.BabyFoodConsumptionSpeedMultiplierEvent = (double)nudBabyFoodConsumptionSpeedEvent.Value;

            Properties.Settings.Default.autosave = checkBoxAutoSave.Checked;
            Properties.Settings.Default.autosaveMinutes = (int)numericUpDownAutosaveMinutes.Value;
            Properties.Settings.Default.OCRWhiteThreshold = (int)nudWhiteThreshold.Value;
            Properties.Settings.Default.SpeechRecognition = chkbSpeechRecognition.Checked;
            Properties.Settings.Default.OverlayInfoDuration = (int)nudOverlayInfoDuration.Value;
            Properties.Settings.Default.syncCollection = chkCollectionSync.Checked;
            Properties.Settings.Default.celsius = radioButtonCelsius.Checked;
            Properties.Settings.Default.oxygenForAll = checkBoxOxygenForAll.Checked;
            Properties.Settings.Default.waitBeforeScreenCapture = (int)nudWaitBeforeScreenCapture.Value;

            Properties.Settings.Default.showOCRButton = cbShowOCRButton.Checked;
            string ocrApp = cbOCRApp.SelectedItem.ToString();
            if (ocrApp == "Custom")
                ocrApp = textBoxOCRCustom.Text;
            Properties.Settings.Default.OCRApp = ocrApp;

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

            // import exported
            Properties.Settings.Default.ExportCreatureFolders = aTExportFolderLocationsBindingSource.OfType<ATImportExportedFolderLocation>()
                    .Where(location => !string.IsNullOrWhiteSpace(location.FolderPath))
                    .Select(location => $"{location.ConvenientName}|{location.OwnerSuffix}|{location.FolderPath}").ToArray();

            Properties.Settings.Default.importChangeCreatureStatus = cbImportUpdateCreatureStatus.Checked;
            Properties.Settings.Default.ImportTribeNameFilter = textBoxImportTribeNameFilter.Text;

            Properties.Settings.Default.DevTools = cbDevTools.Checked;

            string oldLanguageSetting = Properties.Settings.Default.language;
            string lang = cbbLanguage.SelectedItem.ToString();
            Properties.Settings.Default.language = languages.ContainsKey(lang) ? languages[lang] : "";
            LanguageChanged = oldLanguageSetting != Properties.Settings.Default.language;

            Properties.Settings.Default.Save();
        }

        private void btAddSavegameFileLocation_Click(object sender, EventArgs e)
        {
            ATImportFileLocation atImportFileLocation = editFileLocation(new ATImportFileLocation());
            if (atImportFileLocation != null)
            {
                aTImportFileLocationBindingSource.Add(atImportFileLocation);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            saveSettings();
        }

        private void buttonAllToOne_Click(object sender, EventArgs e)
        {
            for (int s = 0; s < 8; s++)
            {
                multSetter[s].Multipliers = new double[] { 1, 1, 1, 1 };

            }
        }

        private void buttonSetToOfficial_Click(object sender, EventArgs e)
        {
            if (Values.V.statMultipliers.Length > 7)
            {
                for (int s = 0; s < 8; s++)
                {
                    multSetter[s].Multipliers = Values.V.statMultipliers[s];
                }
            }
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
            foreach (string file in files) extractSettingsFromFile(file);
        }

        private void extractSettingsFromFile(string file)
        {
            if (File.Exists(file))
            {
                string text = File.ReadAllText(file);
                double d;
                Match m;

                // as used in the server-config-files
                int[] statIndices = { 0, 1, 3, 4, 7, 8, 9, 2 };

                // get stat-multipliers
                // if there are stat-multipliers, set all to the official-values first
                if (text.IndexOf("PerLevelStatsMultiplier_Dino") >= 0)
                    buttonSetToOfficialMP.PerformClick();

                for (int s = 0; s < 8; s++)
                {
                    m = Regex.Match(text, @"PerLevelStatsMultiplier_DinoTamed_Add\[" + statIndices[s] + @"\] ?= ?(\d*\.?\d+)");
                    double[] multipliers;
                    if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
                    {
                        multipliers = multSetter[s].Multipliers;
                        multipliers[0] = d == 0 ? 1 : d;
                        multSetter[s].Multipliers = multipliers;
                    }
                    m = Regex.Match(text, @"PerLevelStatsMultiplier_DinoTamed_Affinity\[" + statIndices[s] + @"\] ?= ?(\d*\.?\d+)");
                    if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
                    {
                        multipliers = multSetter[s].Multipliers;
                        multipliers[1] = d == 0 ? 1 : d;
                        multSetter[s].Multipliers = multipliers;
                    }
                    m = Regex.Match(text, @"PerLevelStatsMultiplier_DinoTamed\[" + statIndices[s] + @"\] ?= ?(\d*\.?\d+)");
                    if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
                    {
                        multipliers = multSetter[s].Multipliers;
                        multipliers[2] = d == 0 ? 1 : d;
                        multSetter[s].Multipliers = multipliers;
                    }
                    m = Regex.Match(text, @"PerLevelStatsMultiplier_DinoWild\[" + statIndices[s] + @"\] ?= ?(\d*\.?\d+)");
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
        }

        private void cbOCRApp_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxOCRCustom.Visible = cbOCRApp.SelectedItem.ToString() == "Custom";
        }

        private void Settings_Disposed(object sender, EventArgs e)
        {
            tt.RemoveAll();
        }

        private void buttonAllTBMultipliersOne_Click(object sender, EventArgs e)
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
            ATImportExportedFolderLocation aTImportExportedFolderLocation = editFolderLocation(new ATImportExportedFolderLocation());
            if (aTImportExportedFolderLocation != null)
            {
                aTExportFolderLocationsBindingSource.Add(aTImportExportedFolderLocation);
            }
        }

        private void dataGridView_FileLocations_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvFileLocation_Change.Index)
            {
                ATImportFileLocation atImportFileLocation = editFileLocation((ATImportFileLocation)aTImportFileLocationBindingSource[e.RowIndex]);
                if (atImportFileLocation != null)
                {
                    aTImportFileLocationBindingSource[e.RowIndex] = atImportFileLocation;
                }
            }

            if (e.ColumnIndex == dgvFileLocation_Delete.Index)
            {
                aTImportFileLocationBindingSource.RemoveAt(e.RowIndex);
            }
        }

        private void dataGridViewExportFolders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvExportFolderChange.Index)
            {
                ATImportExportedFolderLocation aTImportExportedFolderLocation = editFolderLocation((ATImportExportedFolderLocation)aTExportFolderLocationsBindingSource[e.RowIndex]);
                if (aTImportExportedFolderLocation != null)
                {
                    aTExportFolderLocationsBindingSource[e.RowIndex] = aTImportExportedFolderLocation;
                }
            }

            if (e.ColumnIndex == dgvExportFolderDelete.Index)
            {
                aTExportFolderLocationsBindingSource.RemoveAt(e.RowIndex);
            }
        }

        private static ATImportFileLocation editFileLocation(ATImportFileLocation atImportFileLocation)
        {
            ATImportFileLocationDialog atImportFileLocationDialog = new ATImportFileLocationDialog(atImportFileLocation);

            return atImportFileLocationDialog.ShowDialog() == DialogResult.OK &&
                    !string.IsNullOrWhiteSpace(atImportFileLocationDialog.AtImportFileLocation.FileLocation) ?
                    atImportFileLocationDialog.AtImportFileLocation : null;
        }

        private static ATImportExportedFolderLocation editFolderLocation(ATImportExportedFolderLocation atExportFolderLocation)
        {
            ATImportExportedFolderLocationDialog aTImportExportedFolderLocationDialog = new ATImportExportedFolderLocationDialog(atExportFolderLocation);

            return aTImportExportedFolderLocationDialog.ShowDialog() == DialogResult.OK &&
                    !string.IsNullOrWhiteSpace(aTImportExportedFolderLocationDialog.ATImportExportedFolderLocation.FolderPath) ?
                    aTImportExportedFolderLocationDialog.ATImportExportedFolderLocation : null;
        }
    }
}
