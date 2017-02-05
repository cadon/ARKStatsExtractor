using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace ARKBreedingStats
{
    public partial class Settings : Form
    {

        private MultiplierSetting[] multSetter;
        private CreatureCollection cc;

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
            for (int s = 0; s < 8; s++)
            {
                multSetter[s].StatName = Utils.statName(s);
            }

            // Tooltips
            ToolTip tt = new ToolTip();
            tt.SetToolTip(numericUpDownAutosaveMinutes, "To disable set to 0");
            tt.SetToolTip(chkExperimentalOCR, "Experimental! Works well for 1920 and mostly for 1680. May not work for other resolutions at all.");
            tt.SetToolTip(chkCollectionSync, "If checked, the tool automatically reloads the library if it was changed. Use if multiple persons editing the file, e.g. via a shared folder.\nIt's recommened to check this along with \"Auto Save\"");
            tt.SetToolTip(checkBoxAutoSave, "If checked, the library is saved after each change automatically.\nIt's recommened to check this along with \"Auto Update Collection File\"");
            tt.SetToolTip(numericUpDownMaxChartLevel, "This number defines the level that is shown as maximum in the charts. Usually it's good to set this value to one third of the max wild level.");
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
            numericUpDownHatching.Value = (decimal)cc.breedingMultipliers[0];
            numericUpDownMaturation.Value = (decimal)cc.breedingMultipliers[1];
            numericUpDownDomLevelNr.Value = cc.maxDomLevel;
            numericUpDownMaxBreedingSug.Value = cc.maxBreedingSuggestions;
            numericUpDownMaxWildLevel.Value = cc.maxWildLevel;
            numericUpDownMaxChartLevel.Value = cc.maxChartLevel;
            numericUpDownImprintingM.Value = (decimal)cc.imprintingMultiplier;
            numericUpDownBabyCuddleIntervalMultiplier.Value = (decimal)cc.babyCuddleIntervalMultiplier;
            numericUpDownTamingSpeed.Value = (decimal)cc.tamingSpeedMultiplier;
            numericUpDownTamingFoodRate.Value = (decimal)cc.tamingFoodRateMultiplier;
            checkBoxAutoSave.Checked = Properties.Settings.Default.autosave;
            numericUpDownAutosaveMinutes.Value = Properties.Settings.Default.autosaveMinutes;
            chkExperimentalOCR.Checked = Properties.Settings.Default.OCR;
            chkCollectionSync.Checked = Properties.Settings.Default.syncCollection;
            if (Properties.Settings.Default.celsius) radioButtonCelsius.Checked = true;
            else radioButtonFahrenheit.Checked = true;
        }

        private void saveSettings()
        {
            for (int s = 0; s < 8; s++)
            {
                for (int sm = 0; sm < 4; sm++)
                    cc.multipliers[s][sm] = multSetter[s].Multipliers[sm];
            }
            cc.breedingMultipliers[0] = (double)numericUpDownHatching.Value;
            cc.breedingMultipliers[1] = (double)numericUpDownMaturation.Value;
            cc.maxDomLevel = (int)numericUpDownDomLevelNr.Value;
            cc.maxWildLevel = (int)numericUpDownMaxWildLevel.Value;
            cc.maxChartLevel = (int)numericUpDownMaxChartLevel.Value;
            cc.maxBreedingSuggestions = (int)numericUpDownMaxBreedingSug.Value;
            cc.imprintingMultiplier = (double)numericUpDownImprintingM.Value;
            cc.babyCuddleIntervalMultiplier = (double)numericUpDownBabyCuddleIntervalMultiplier.Value;
            cc.tamingSpeedMultiplier = (double)numericUpDownTamingSpeed.Value;
            cc.tamingFoodRateMultiplier = (double)numericUpDownTamingFoodRate.Value;
            Properties.Settings.Default.autosave = checkBoxAutoSave.Checked;
            Properties.Settings.Default.autosaveMinutes = (int)numericUpDownAutosaveMinutes.Value;
            Properties.Settings.Default.OCR = chkExperimentalOCR.Checked;
            Properties.Settings.Default.syncCollection = chkCollectionSync.Checked;
            Properties.Settings.Default.celsius = radioButtonCelsius.Checked;
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

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            if (n != null)
            {
                n.Select(0, n.Text.Length);
            }
        }

        private void Settings_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Settings_DragDrop(object sender, DragEventArgs e)
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

                int[] statIndices = new int[] { 0, 1, 3, 4, 7, 8, 9, 2 };

                // get stat-multipliers
                for (int s = 0; s < 8; s++)
                {
                    m = Regex.Match(text, @"PerLevelStatsMultiplier_DinoTamed_Add\[" + statIndices[s] + @"\] ?= ?(\d*\.?\d+)");
                    if (m.Success && double.TryParse(m.Groups[1].Value, out d))
                    {
                        multipliers = multSetter[s].Multipliers;
                        multipliers[0] = d;
                        multSetter[s].Multipliers = multipliers;
                    }
                    m = Regex.Match(text, @"PerLevelStatsMultiplier_DinoTamed_Affinity\[" + statIndices[s] + @"\] ?= ?(\d*\.?\d+)");
                    if (m.Success && double.TryParse(m.Groups[1].Value, out d))
                    {
                        multipliers = multSetter[s].Multipliers;
                        multipliers[1] = d;
                        multSetter[s].Multipliers = multipliers;
                    }
                    m = Regex.Match(text, @"PerLevelStatsMultiplier_DinoTamed\[" + statIndices[s] + @"\] ?= ?(\d*\.?\d+)");
                    if (m.Success && double.TryParse(m.Groups[1].Value, out d))
                    {
                        multipliers = multSetter[s].Multipliers;
                        multipliers[2] = d;
                        multSetter[s].Multipliers = multipliers;
                    }
                    m = Regex.Match(text, @"PerLevelStatsMultiplier_DinoWild\[" + statIndices[s] + @"\] ?= ?(\d*\.?\d+)");
                    if (m.Success && double.TryParse(m.Groups[1].Value, out d))
                    {
                        multipliers = multSetter[s].Multipliers;
                        multipliers[3] = d;
                        multSetter[s].Multipliers = multipliers;
                    }
                }

                m = Regex.Match(text, @"EggHatchSpeedMultiplier ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, out d))
                {
                    numericUpDownHatching.Value = (decimal)d;
                }
                m = Regex.Match(text, @"BabyMatureSpeedMultiplier ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, out d))
                {
                    numericUpDownMaturation.Value = (decimal)d;
                }
                m = Regex.Match(text, @"BabyImprintingStatScaleMultiplier ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, out d))
                {
                    numericUpDownImprintingM.Value = (decimal)d;
                }
                m = Regex.Match(text, @"BabyCuddleIntervalMultiplier ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, out d))
                {
                    numericUpDownBabyCuddleIntervalMultiplier.Value = (decimal)d;
                }

                // GameUserSettings.ini

                m = Regex.Match(text, @"TamingSpeedMultiplier ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, out d))
                {
                    numericUpDownTamingSpeed.Value = (decimal)d;
                }
                m = Regex.Match(text, @"DinoCharacterFoodDrainMultiplier ?= ?(\d*\.?\d+)");
                if (m.Success && double.TryParse(m.Groups[1].Value, out d))
                {
                    numericUpDownTamingFoodRate.Value = (decimal)d;
                }
            }
        }
    }
}
