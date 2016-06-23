using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            setControls(cc);
            checkBoxAutoSave.Checked = Properties.Settings.Default.autosave;
            numericUpDownAutosaveMinutes.Value = Properties.Settings.Default.autosaveMinutes;
            chkExperimentalOCR.Checked = Properties.Settings.Default.OCR;
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
        }

        private void setControls(CreatureCollection cc)
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
            numericUpDownImprintingM.Value = (decimal)cc.imprintingMultiplier;
        }

        private void saveValues()
        {
            for (int s = 0; s < 8; s++)
            {
                cc.multipliers[s] = multSetter[s].Multipliers;
            }
            cc.breedingMultipliers[0] = (double)numericUpDownHatching.Value;
            cc.breedingMultipliers[1] = (double)numericUpDownMaturation.Value;
            cc.maxDomLevel = (int)numericUpDownDomLevelNr.Value;
            cc.maxWildLevel = (int)numericUpDownMaxWildLevel.Value;
            cc.maxBreedingSuggestions = (int)numericUpDownMaxBreedingSug.Value;
            cc.imprintingMultiplier = (double)numericUpDownImprintingM.Value;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // save settings
            saveValues();
            Properties.Settings.Default.autosave = checkBoxAutoSave.Checked;
            Properties.Settings.Default.autosaveMinutes = (int)numericUpDownAutosaveMinutes.Value;
            Properties.Settings.Default.OCR = chkExperimentalOCR.Checked;
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
            cc.multipliers = Values.V.statMultipliers;
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
    }
}
