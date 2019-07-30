using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ARKBreedingStats.miscClasses;
using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.values;

namespace ARKBreedingStats.multiplierTesting
{
    public partial class StatsMultiplierTesting : UserControl
    {
        public delegate void ApplyMultipliersEventHandler();
        public event ApplyMultipliersEventHandler OnApplyMultipliers;

        private readonly List<StatMultiplierTestingControl> statControls;
        private CreatureCollection cc;
        private Species selectedSpecies;
        private Nud fineAdjustmentsNud;
        private readonly MinMaxDouble fineAdjustmentRange;
        private double fineAdjustmentFactor;

        public StatsMultiplierTesting()
        {
            InitializeComponent();

            statControls = new List<StatMultiplierTestingControl>();
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                var sc = new StatMultiplierTestingControl
                {
                    StatName = Utils.statName(s, true)
                };
                if (Utils.precision(s) == 3)
                    sc.Percent = true;
                sc.OnLevelChanged += Sc_OnLevelChanged;
                sc.OnValueChangedTE += setTE;
                sc.OnValueChangedIB += setIB;
                sc.OnValueChangedIBM += setIBM;
                statControls.Add(sc);
            }
            // add controls in order like ingame
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                flowLayoutPanel1.Controls.Add(statControls[Values.statsDisplayOrder[s]]);
                flowLayoutPanel1.SetFlowBreak(statControls[Values.statsDisplayOrder[s]], true);
            }

            // set level-control to last
            flowLayoutPanel1.Controls.Add(gbLevel);

            statControls[(int)StatNames.Stamina].NoIB = true;
            statControls[(int)StatNames.Oxygen].NoIB = true;

            fineAdjustmentRange = new MinMaxDouble(0);
            rbTamed.Checked = true;
            gbFineAdjustment.Hide();
        }

        private void Sc_OnLevelChanged()
        {
            UpdateLevelSums();
        }

        private void setTE(double TE)
        {
            nudTE.ValueSave = (decimal)TE * 100;
            gbFineAdjustment.Hide();
        }

        private void setTE(MinMaxDouble TE)
        {
            setTE(TE.Mean);
            setFineAdjustmentNUD(nudTE, "Taming Effectiveness (TE)", TE.Min, TE.Max);
        }

        private void setIB(double IB)
        {
            nudIB.ValueSave = (decimal)IB * 100;
        }

        private void setIB(MinMaxDouble IB)
        {
            setIB(IB.Mean);
            setFineAdjustmentNUD(nudIB, "Imprinting Bonus (IB)", IB.Min, IB.Max);
        }

        private void setIBM(double IBM)
        {
            nudIBM.ValueSave = (decimal)IBM;
        }

        private void setIBM(MinMaxDouble IBM)
        {
            setIBM(IBM.Mean);
            setFineAdjustmentNUD(nudIBM, "Imprinting Bonus Multiplier (IBM)", IBM.Min, IBM.Max);
        }

        private void nudCreatureLevel_ValueChanged(object sender, EventArgs e)
        {
            UpdateLevelSums();
        }

        private void UpdateLevelSums()
        {
            int sumW = 0, sumD = 0;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s == (int)StatNames.Torpidity) continue;
                sumW += statControls[s].levelWild;
                sumD += statControls[s].levelDom;
            }
            lbLevelSumWild.Text = "Sum LevelWild = " + sumW;
            bool positive;
            int diff = sumW - statControls[(int)StatNames.Torpidity].levelWild;
            if (diff != 0)
            {
                positive = diff > 0;
                if (!positive) diff = -diff;
                lbLevelSumWild.Text += " (" + (positive ? "+" : "") + (sumW - statControls[(int)StatNames.Torpidity].levelWild) + ")";

                if (diff > 50) diff = 50;
                lbLevelSumWild.BackColor = Utils.getColorFromPercent(50 - diff, 0.6, !positive);
            }
            else { lbLevelSumWild.BackColor = SystemColors.Window; }

            lbLevelSumDom.Text = "Sum LevelDom = " + sumD;
            diff = sumW + sumD + 1 - (int)nudCreatureLevel.Value;
            if (diff != 0)
            {
                positive = diff > 0;
                if (!positive) diff = -diff;
                lbLevelSumDom.Text += " (" + (positive ? "+" : "") + (sumW + sumD + 1 - (int)nudCreatureLevel.Value) + ")";

                if (diff > 50) diff = 50;
                lbLevelSumDom.BackColor = Utils.getColorFromPercent(50 - diff, 0.6, !positive);
            }
            else { lbLevelSumDom.BackColor = SystemColors.Window; }
        }

        private void nudTE_ValueChanged(object sender, EventArgs e)
        {
            for (int s = 0; s < Values.STATS_COUNT; s++)
                statControls[s].TE = (double)nudTE.Value / 100;
        }

        private void nudIB_ValueChanged(object sender, EventArgs e)
        {
            for (int s = 0; s < Values.STATS_COUNT; s++)
                statControls[s].IB = (double)nudIB.Value / 100;
        }

        private void nudIBM_ValueChanged(object sender, EventArgs e)
        {
            for (int s = 0; s < Values.STATS_COUNT; s++)
                statControls[s].IBM = (double)nudIBM.Value;
        }

        private void rbWild_CheckedChanged(object sender, EventArgs e)
        {
            if (rbWild.Checked)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                    statControls[s].Wild = rbWild.Checked;
                nudTE.BackColor = SystemColors.Window;
                nudIB.BackColor = SystemColors.Window;
                nudIBM.BackColor = SystemColors.Window;
            }
        }

        private void rbTamed_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTamed.Checked)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                    statControls[s].Tamed = rbTamed.Checked;
                nudTE.BackColor = Color.FromArgb(215, 186, 255);
                nudIB.BackColor = SystemColors.Window;
                nudIBM.BackColor = SystemColors.Window;
            }
        }

        private void rbBred_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBred.Checked)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                    statControls[s].Bred = rbBred.Checked;
                nudTE.BackColor = SystemColors.Window;
                nudIB.BackColor = Color.FromArgb(255, 186, 242);
                nudIBM.BackColor = Color.FromArgb(255, 153, 236);
            }
        }

        private void setStatMultipliersFromCC()
        {
            if (cc?.serverMultipliers?.statMultipliers == null) return;

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                var m = new double[4];
                for (int i = 0; i < 4; i++)
                    m[i] = cc.serverMultipliers.statMultipliers[s]?[i] ?? 1;
                statControls[s].StatMultipliers = m;
            }
            setIBM(cc.serverMultipliers.BabyImprintingStatScaleMultiplier);

            cbSingleplayerSettings.Checked = cc.singlePlayerSettings;
        }

        public void SetSpecies(Species species, bool forceUpdate = false)
        {
            if (species != null && (forceUpdate || cbUpdateOnSpeciesChange.Checked))
            {
                selectedSpecies = species;
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    statControls[s].setStatValues(selectedSpecies.fullStatsRaw[s]);
                }
                statControls[(int)StatNames.Health].TBHM = selectedSpecies.TamedBaseHealthMultiplier;
                statControls[(int)StatNames.SpeedMultiplier].NoIB = selectedSpecies.NoImprintingForSpeed;
            }
        }

        private void btUpdateSpecies_Click(object sender, EventArgs e)
        {
            SetSpecies(selectedSpecies, true);
        }

        public void setCreatureValues(double[] statValues, int[] levelsWild, int[] levelsDom, double TE, double IB, bool tamed, bool bred)
        {
            int level = 1;
            if (statValues != null)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                    statControls[s].statValue = statValues[s];
            }
            if (levelsWild != null)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                    statControls[s].levelWild = levelsWild[s];
                level += levelsWild[(int)StatNames.Torpidity];
            }
            if (levelsDom != null)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    statControls[s].levelDom = levelsDom[s];
                    level += levelsDom[s];
                }
            }
            setTE(TE);
            setIB(IB);
            if (tamed)
                rbTamed.Checked = true;
            else if (bred)
                rbBred.Checked = true;
            else rbWild.Checked = true;

            nudCreatureLevel.Value = level;
        }

        private void llStatCalculation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://ark.gamepedia.com/Creature_Stats_Calculation");
        }

        public CreatureCollection CreatureCollection
        {
            set
            {
                cc = value;
                setStatMultipliersFromCC();
            }
        }

        private void iwMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool error = false;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s != (int)StatNames.Torpidity)
                    error = !statControls[s].calculateIwM() || error;
            }
            if (error) MessageBox.Show("For some stats the IwM couldn't be calculated, because of a Divide by Zero-error, e.g. Lw and Iw needs to be >0.");
        }

        private void idMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool error = false;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s != (int)StatNames.Torpidity)
                    error = !statControls[s].calculateIdM() || error;
            }
            if (error) MessageBox.Show("For some stats the IdM couldn't be calculated, because of a Divide by Zero-error, e.g. Ld needs to be at least 1.");
        }

        private void taMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool error = false;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s != (int)StatNames.Torpidity)
                    error = !statControls[s].calculateTaM() || error;
            }
            if (error) MessageBox.Show("For some stats the TaM couldn't be calculated, because of a Divide by Zero-error, e.g. Ta needs to be at least 1.");
        }

        private void tmMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool error = false;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s != (int)StatNames.Torpidity)
                    error = !statControls[s].calculateTmM() || error;
            }
            if (error) MessageBox.Show("For some stats the TmM couldn't be calculated, because of a Divide by Zero-error, e.g. Tm needs to be at least 1.");
        }

        private void useStatMultipliersOfSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setStatMultipliersFromCC();
        }

        private void useDefaultStatMultipliersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerMultipliers officialSM = Values.V.serverMultipliersPresets.GetPreset("official");
            if (officialSM == null) return;
            for (int s = 0; s < Values.STATS_COUNT; s++)
                statControls[s].StatMultipliers = officialSM.statMultipliers[s];
        }

        private void copyStatMultipliersToSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cc?.serverMultipliers?.statMultipliers == null) return;

            for (int s = 0; s < Values.STATS_COUNT; s++)
                cc.serverMultipliers.statMultipliers[s] = statControls[s].StatMultipliers;
            cc.serverMultipliers.BabyImprintingStatScaleMultiplier = (double)nudIBM.Value;
            OnApplyMultipliers?.Invoke();
        }

        private void tbFineAdjustments_Scroll(object sender, EventArgs e)
        {
            if (fineAdjustmentsNud != null)
                fineAdjustmentsNud.ValueSave = (decimal)((fineAdjustmentRange.Min + (fineAdjustmentRange.Max - fineAdjustmentRange.Min) * 0.01 * tbFineAdjustments.Value) * fineAdjustmentFactor);
        }

        private void setFineAdjustmentNUD(Nud nud, string title, double min, double max)
        {
            gbFineAdjustment.Visible = true;
            fineAdjustmentRange.Min = min;
            fineAdjustmentRange.Max = max;
            fineAdjustmentsNud = nud;
            gbFineAdjustment.Text = title;
            fineAdjustmentFactor = nud == nudIB || nud == nudTE ? 100 : 1;
        }

        private void cbSingleplayerSettings_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSingleplayerSettings.Checked)
            {
                ServerMultipliers spM = Values.V.serverMultipliersPresets.GetPreset("singleplayer");
                if (spM != null)
                {
                    for (int s = 0; s < Values.STATS_COUNT; s++)
                    {
                        if (spM.statMultipliers[s] == null)
                            statControls[s].SetSinglePlayerSettings();
                        else
                            statControls[s].SetSinglePlayerSettings(spM.statMultipliers[s][3], spM.statMultipliers[s][2], spM.statMultipliers[s][0], spM.statMultipliers[s][1]);
                    }
                    return;
                }
            }
            for (int s = 0; s < Values.STATS_COUNT; s++)
                statControls[s].SetSinglePlayerSettings();
        }

        private void allWildLvlToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Utils.ShowTextInput("Wild Level", out string nr, "", "0") && int.TryParse(nr, out int lv))
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                    statControls[s].levelWild = lv;
            }
        }

        private void allDomLvlToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Utils.ShowTextInput("Dom Level", out string nr, "", "0") && int.TryParse(nr, out int lv))
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                    statControls[s].levelDom = lv;
            }
        }
    }
}
