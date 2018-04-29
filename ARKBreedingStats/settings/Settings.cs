using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using ARKBreedingStats;
using System.Collections.Generic;
using System.Linq;

namespace ARKBreedingStats.settings
{
    public partial class Settings : Form
    {
        private MultiplierSetting[] multSetter;
        private CreatureCollection cc;
        private ToolTip tt;

        public bool WildMaxChanged; // is needed for the speech-recognition, if wildMax is changed, the grammar has to be rebuilt

        public Settings()
        {
            initStuff();
        }

        public Settings(CreatureCollection cc)
        {
            initStuff();
            this.cc = cc;
            loadSettings(cc);
        }

        private void initStuff()
        {
            InitializeComponent();
            multSetter = new MultiplierSetting[] { multiplierSettingHP, multiplierSettingSt, multiplierSettingOx, multiplierSettingFo, multiplierSettingWe, multiplierSettingDm, multiplierSettingSp, multiplierSettingTo };
            int[] serverStatIndices = new int[] { 0, 1, 3, 4, 7, 8, 9, 2 };
            for (int s = 0; s < 8; s++)
            {
                multSetter[s].StatName = Utils.statName(s) + " [" + serverStatIndices[s].ToString() + "]";
            }

            customSCStarving.Title = "Starving: ";
            customSCWakeup.Title = "Wakeup: ";
            customSCBirth.Title = "Birth: ";
            customSCCustom.Title = "Custom: ";

            fileSelectorARKToolsExe.IsFile = true;
            fileSelectorExtractedSaveFolder.IsFile = false;
            fileSelectorImportExported.IsFile = false;
            fileSelectorARKToolsExe.fileFilter = "ARK-tools executable (ark-tools.exe)|ark-tools.exe";

            Disposed += Settings_Disposed;
            WildMaxChanged = false;

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
            tt.SetToolTip(nudMaxServerLevel, "The max level allowed on the server. Currently creatures with more than 450 levels will be deleted on official servers.\nSet to -1 to disable a warning in this app.");
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

            numericUpDownHatching.Value = (decimal)cc.EggHatchSpeedMultiplier;
            numericUpDownMaturation.Value = (decimal)cc.BabyMatureSpeedMultiplier;
            numericUpDownDomLevelNr.Value = cc.maxDomLevel;
            numericUpDownMaxBreedingSug.Value = cc.maxBreedingSuggestions;
            numericUpDownMaxWildLevel.Value = cc.maxWildLevel;
            nudMaxServerLevel.Value = cc.maxServerLevel;
            numericUpDownMaxChartLevel.Value = cc.maxChartLevel;
            numericUpDownImprintingM.Value = (decimal)cc.imprintingMultiplier;
            numericUpDownBabyCuddleIntervalMultiplier.Value = (decimal)cc.babyCuddleIntervalMultiplier;
            numericUpDownTamingSpeed.Value = (decimal)cc.tamingSpeedMultiplier;
            numericUpDownTamingFoodRate.Value = (decimal)cc.tamingFoodRateMultiplier;
            nudMatingInterval.Value = (decimal)cc.MatingIntervalMultiplier;
            nudBabyFoodConsumptionSpeed.Value = (decimal)cc.BabyFoodConsumptionSpeedMultiplier;
            // event-multiplier
            nudCuddleIntervalEvent.Value = (decimal)cc.babyCuddleIntervalMultiplierEvent;
            nudTamingSpeedEvent.Value = (decimal)cc.tamingSpeedMultiplierEvent;
            nudTamingFoodRateEvent.Value = (decimal)cc.tamingFoodRateMultiplierEvent;
            nudMatingIntervalEvent.Value = (decimal)cc.MatingIntervalMultiplierEvent;
            nudHatchingSpeedEvent.Value = (decimal)cc.EggHatchSpeedMultiplierEvent;
            nudMaturationSpeedEvent.Value = (decimal)cc.BabyMatureSpeedMultiplierEvent;
            nudBabyFoodConsumptionEvent.Value = (decimal)cc.BabyFoodConsumptionSpeedMultiplierEvent;

            checkBoxAutoSave.Checked = Properties.Settings.Default.autosave;
            numericUpDownAutosaveMinutes.Value = Properties.Settings.Default.autosaveMinutes;
            nudWhiteThreshold.Value = Properties.Settings.Default.OCRWhiteThreshold;
            chkbSpeechRecognition.Checked = Properties.Settings.Default.SpeechRecognition;
            nudOverlayInfoDuration.Value = Properties.Settings.Default.OverlayInfoDuration;
            chkCollectionSync.Checked = Properties.Settings.Default.syncCollection;
            if (Properties.Settings.Default.celsius) radioButtonCelsius.Checked = true;
            else radioButtonFahrenheit.Checked = true;
            cbIgnoreSexInBreedingPlan.Checked = Properties.Settings.Default.IgnoreSexInBreedingPlan;
            checkBoxOxygenForAll.Checked = Properties.Settings.Default.oxygenForAll;
            nudWaitBeforeScreenCapture.Value = Properties.Settings.Default.waitBeforeScreenCapture;

            string ocrApp = Properties.Settings.Default.OCRApp;
            int i = cbOCRApp.Items.IndexOf(ocrApp);
            if (i == -1)
            {
                textBoxOCRCustom.Text = ocrApp;
                cbOCRApp.SelectedIndex = cbOCRApp.Items.IndexOf("Custom");
            }
            else
                cbOCRApp.SelectedIndex = i;

            customSCStarving.SoundFile = Properties.Settings.Default.soundStarving;
            customSCWakeup.SoundFile = Properties.Settings.Default.soundWakeup;
            customSCBirth.SoundFile = Properties.Settings.Default.soundBirth;
            customSCCustom.SoundFile = Properties.Settings.Default.soundCustom;

            tbPlayAlarmsSeconds.Text = Properties.Settings.Default.playAlarmTimes;

            cbConsiderWildLevelSteps.Checked = cc.considerWildLevelSteps;
            nudWildLevelStep.Value = cc.wildLevelStep;
            cbInventoryCheck.Checked = Properties.Settings.Default.inventoryCheckTimer;
            cbAllowMoreThanHundredImprinting.Checked = cc.allowMoreThanHundredImprinting;
            cbCreatureColorsLibrary.Checked = Properties.Settings.Default.showColorsInLibrary;

            //ark-tools
            fileSelectorARKToolsExe.Link = Properties.Settings.Default.arkToolsPath;
            fileSelectorExtractedSaveFolder.Link = Properties.Settings.Default.savegameExtractionPath;
            if (Properties.Settings.Default.arkSavegamePaths != null)
            {
                foreach (string path in Properties.Settings.Default.arkSavegamePaths)
                {
                    aTImportFileLocationBindingSource.Add(ATImportFileLocation.CreateFromString(path));
                }
            }

            cbImportUpdateCreatureStatus.Checked = Properties.Settings.Default.importChangeCreatureStatus;

            fileSelectorImportExported.Link = Properties.Settings.Default.ExportCreatureFolder;

            cbDevTools.Checked = Properties.Settings.Default.DevTools;
        }

        private void saveSettings()
        {
            for (int s = 0; s < 8; s++)
            {
                for (int sm = 0; sm < 4; sm++)
                    cc.multipliers[s][sm] = multSetter[s].Multipliers[sm];
            }

            cc.singlePlayerSettings = cbSingleplayerSettings.Checked;
            cc.EggHatchSpeedMultiplier = (double)numericUpDownHatching.Value;
            cc.BabyMatureSpeedMultiplier = (double)numericUpDownMaturation.Value;
            cc.maxDomLevel = (int)numericUpDownDomLevelNr.Value;
            WildMaxChanged = WildMaxChanged || (cc.maxWildLevel != (int)numericUpDownMaxWildLevel.Value);
            cc.maxWildLevel = (int)numericUpDownMaxWildLevel.Value;
            cc.maxServerLevel = (int)nudMaxServerLevel.Value;
            cc.maxChartLevel = (int)numericUpDownMaxChartLevel.Value;
            cc.maxBreedingSuggestions = (int)numericUpDownMaxBreedingSug.Value;
            Properties.Settings.Default.IgnoreSexInBreedingPlan = cbIgnoreSexInBreedingPlan.Checked;
            cc.imprintingMultiplier = (double)numericUpDownImprintingM.Value;
            cc.babyCuddleIntervalMultiplier = (double)numericUpDownBabyCuddleIntervalMultiplier.Value;
            cc.tamingSpeedMultiplier = (double)numericUpDownTamingSpeed.Value;
            cc.tamingFoodRateMultiplier = (double)numericUpDownTamingFoodRate.Value;
            cc.MatingIntervalMultiplier = (double)nudMatingInterval.Value;
            cc.BabyFoodConsumptionSpeedMultiplier = (double)nudBabyFoodConsumptionSpeed.Value;
            // event-multiplier
            cc.babyCuddleIntervalMultiplierEvent = (double)nudCuddleIntervalEvent.Value;
            cc.tamingSpeedMultiplierEvent = (double)nudTamingSpeedEvent.Value;
            cc.tamingFoodRateMultiplierEvent = (double)nudTamingFoodRateEvent.Value;
            cc.MatingIntervalMultiplierEvent = (double)nudMatingIntervalEvent.Value;
            cc.EggHatchSpeedMultiplierEvent = (double)nudHatchingSpeedEvent.Value;
            cc.BabyMatureSpeedMultiplierEvent = (double)nudMaturationSpeedEvent.Value;
            cc.BabyFoodConsumptionSpeedMultiplierEvent = (double)nudBabyFoodConsumptionEvent.Value;

            Properties.Settings.Default.autosave = checkBoxAutoSave.Checked;
            Properties.Settings.Default.autosaveMinutes = (int)numericUpDownAutosaveMinutes.Value;
            Properties.Settings.Default.OCRWhiteThreshold = (int)nudWhiteThreshold.Value;
            Properties.Settings.Default.SpeechRecognition = chkbSpeechRecognition.Checked;
            Properties.Settings.Default.OverlayInfoDuration = (int)nudOverlayInfoDuration.Value;
            Properties.Settings.Default.syncCollection = chkCollectionSync.Checked;
            Properties.Settings.Default.celsius = radioButtonCelsius.Checked;
            Properties.Settings.Default.oxygenForAll = checkBoxOxygenForAll.Checked;
            Properties.Settings.Default.waitBeforeScreenCapture = (int)nudWaitBeforeScreenCapture.Value;

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

            //ark-tools
            Properties.Settings.Default.arkToolsPath = fileSelectorARKToolsExe.Link;
            Properties.Settings.Default.savegameExtractionPath = fileSelectorExtractedSaveFolder.Link;

            Properties.Settings.Default.arkSavegamePaths = aTImportFileLocationBindingSource.OfType<ATImportFileLocation>()
                    .Where(location => !string.IsNullOrWhiteSpace(location.FileLocation))
                    .Select(location => $"{location.ConvenientName}|{location.ServerName}|{location.FileLocation}").ToArray();

            Properties.Settings.Default.importChangeCreatureStatus = cbImportUpdateCreatureStatus.Checked;

            Properties.Settings.Default.ExportCreatureFolder = fileSelectorImportExported.Link;

            Properties.Settings.Default.DevTools = cbDevTools.Checked;
        }

        private void btAddSavegameFileLocation_Click(object sender, EventArgs e)
        {
            ATImportFileLocation atImportFileLocation = editFileLocation(new ATImportFileLocation());
            if (atImportFileLocation != null)
            {
                aTImportFileLocationBindingSource.Add(atImportFileLocation);
            }
        }

        private string setSoundFile(string soundFilePath)
        {
            return soundFilePath;
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
                double[] multipliers;
                Match m;

                // as used in the server-config-files
                int[] statIndices = new int[] { 0, 1, 3, 4, 7, 8, 9, 2 };

                // get stat-multipliers
                // if there are stat-multipliers, set all to the official-values first
                if (text.IndexOf("PerLevelStatsMultiplier_Dino") >= 0)
                    buttonSetToOfficialMP.PerformClick();

                for (int s = 0; s < 8; s++)
                {
                    m = Regex.Match(text, @"PerLevelStatsMultiplier_DinoTamed_Add\[" + statIndices[s] + @"\] ?= ?(\d*\.?\d+)");
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
                    nudMatingInterval.Value = (decimal)d;
                }

                m = Regex.Match(text, @"EggHatchSpeedMultiplier ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
                {
                    numericUpDownHatching.Value = (decimal)d;
                }
                m = Regex.Match(text, @"BabyMatureSpeedMultiplier ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
                {
                    numericUpDownMaturation.Value = (decimal)d;
                }
                m = Regex.Match(text, @"BabyImprintingStatScaleMultiplier ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
                {
                    numericUpDownImprintingM.Value = (decimal)d;
                }
                m = Regex.Match(text, @"BabyCuddleIntervalMultiplier ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
                {
                    numericUpDownBabyCuddleIntervalMultiplier.Value = (decimal)d;
                }
                m = Regex.Match(text, @"BabyFoodConsumptionSpeedMultiplier ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
                {
                    nudBabyFoodConsumptionSpeed.Value = (decimal)d;
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
                    numericUpDownTamingSpeed.Value = (decimal)d;
                }
                m = Regex.Match(text, @"DinoCharacterFoodDrainMultiplier ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out d))
                {
                    numericUpDownTamingFoodRate.Value = (decimal)d;
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
            numericUpDownTamingSpeed.Value = 1;
            numericUpDownTamingFoodRate.Value = 1;
            nudMatingInterval.Value = 1;
            numericUpDownHatching.Value = 1;
            numericUpDownMaturation.Value = 1;
            numericUpDownImprintingM.Value = 1;
            numericUpDownBabyCuddleIntervalMultiplier.Value = 1;
            nudBabyFoodConsumptionSpeed.Value = 1;
        }

        private void buttonEventToDefault_Click(object sender, EventArgs e)
        {
            nudTamingSpeedEvent.Value = numericUpDownTamingSpeed.Value;
            nudTamingFoodRateEvent.Value = numericUpDownTamingFoodRate.Value;
            nudMatingIntervalEvent.Value = nudMatingInterval.Value;
            nudHatchingSpeedEvent.Value = numericUpDownHatching.Value;
            nudMaturationSpeedEvent.Value = numericUpDownMaturation.Value;
            nudCuddleIntervalEvent.Value = numericUpDownBabyCuddleIntervalMultiplier.Value;
            nudBabyFoodConsumptionEvent.Value = nudBabyFoodConsumptionSpeed.Value;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/cadon/ARKStatsExtractor/wiki/Name-Generator");
        }

        private void linkLabelDLARKTools_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Qowyn/ark-tools/releases/latest");
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

        private static ATImportFileLocation editFileLocation(ATImportFileLocation atImportFileLocation)
        {
            ATImportFileLocationDialog atImportFileLocationDialog =
                    new ATImportFileLocationDialog(atImportFileLocation);

            return atImportFileLocationDialog.ShowDialog() == DialogResult.OK &&
                    !string.IsNullOrWhiteSpace(atImportFileLocationDialog.AtImportFileLocation.FileLocation) ?
                    atImportFileLocationDialog.AtImportFileLocation : null;
        }
    }
}
