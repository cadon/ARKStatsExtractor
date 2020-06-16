using ARKBreedingStats.Library;
using ARKBreedingStats.miscClasses;
using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.multiplierTesting
{
    public partial class StatsMultiplierTesting : UserControl
    {
        public event Action OnApplyMultipliers;

        private readonly List<StatMultiplierTestingControl> _statControls;
        private CreatureCollection _cc;
        private Species _selectedSpecies;
        private Nud _fineAdjustmentsNud;
        private MinMaxDouble _fineAdjustmentRange;
        private double _fineAdjustmentFactor;

        public StatsMultiplierTesting()
        {
            InitializeComponent();

            _statControls = new List<StatMultiplierTestingControl>();
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                var sc = new StatMultiplierTestingControl
                {
                    StatName = $"[{s}]{Utils.StatName(s, true)}"
                };
                if (Utils.Precision(s) == 3)
                    sc.Percent = true;
                sc.OnLevelChanged += Sc_OnLevelChanged;
                sc.OnTECalculated += SetTE;
                sc.OnIBCalculated += SetIB;
                sc.OnIBMCalculated += SetIBM;
                _statControls.Add(sc);
            }
            // add controls in order like in-game
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                flowLayoutPanel1.Controls.Add(_statControls[Values.statsDisplayOrder[s]]);
                flowLayoutPanel1.SetFlowBreak(_statControls[Values.statsDisplayOrder[s]], true);
            }

            // set level-control to last
            flowLayoutPanel1.Controls.Add(gbLevel);

            _fineAdjustmentRange = new MinMaxDouble(0);
            rbTamed.Checked = true;
            gbFineAdjustment.Hide();
        }

        private void Sc_OnLevelChanged()
        {
            UpdateLevelSums();
        }

        private void SetTE(double TE)
        {
            nudTE.ValueSave = (decimal)TE * 100;
            gbFineAdjustment.Hide();
        }

        private void SetTE(MinMaxDouble TE)
        {
            SetTE(TE.Mean);
            SetFineAdjustmentNUD(nudTE, "Taming Effectiveness (TE)", TE.Min, TE.Max);
        }

        private void SetIB(double IB)
        {
            nudIB.ValueSave = (decimal)IB * 100;
        }

        private void SetIB(MinMaxDouble IB)
        {
            SetIB(IB.Mean);
            SetFineAdjustmentNUD(nudIB, "Imprinting Bonus (IB)", IB.Min, IB.Max);
        }

        private void SetIBM(double IBM)
        {
            nudIBM.ValueSave = (decimal)IBM;
        }

        private void SetIBM(MinMaxDouble IBM)
        {
            SetIBM(IBM.Mean);
            SetFineAdjustmentNUD(nudIBM, "Imprinting Bonus Multiplier (IBM)", IBM.Min, IBM.Max);
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
                sumW += _statControls[s].LevelWild;
                sumD += _statControls[s].LevelDom;
            }
            lbLevelSumWild.Text = "Sum LevelWild = " + sumW;
            bool positive;
            int diff = sumW - _statControls[(int)StatNames.Torpidity].LevelWild;
            if (diff != 0)
            {
                positive = diff > 0;
                if (!positive) diff = -diff;
                lbLevelSumWild.Text += $" ({(positive ? "+" : "")}{(sumW - _statControls[(int)StatNames.Torpidity].LevelWild)})";

                if (diff > 50) diff = 50;
                lbLevelSumWild.BackColor = Utils.GetColorFromPercent(50 - diff, 0.6, !positive);
            }
            else { lbLevelSumWild.BackColor = SystemColors.Window; }

            lbLevelSumDom.Text = $"Sum LevelDom = {sumD}";
            diff = sumW + sumD + 1 - (int)nudCreatureLevel.Value;
            if (diff != 0)
            {
                positive = diff > 0;
                if (!positive) diff = -diff;
                lbLevelSumDom.Text += $" ({(positive ? "+" : "")}{sumW + sumD + 1 - (int)nudCreatureLevel.Value})";

                if (diff > 50) diff = 50;
                lbLevelSumDom.BackColor = Utils.GetColorFromPercent(50 - diff, 0.6, !positive);
            }
            else { lbLevelSumDom.BackColor = SystemColors.Window; }
        }

        private void nudTE_ValueChanged(object sender, EventArgs e)
        {
            for (int s = 0; s < Values.STATS_COUNT; s++)
                _statControls[s].TE = (double)nudTE.Value / 100;
        }

        private void nudIB_ValueChanged(object sender, EventArgs e)
        {
            for (int s = 0; s < Values.STATS_COUNT; s++)
                _statControls[s].IB = (double)nudIB.Value / 100;
        }

        private void nudIBM_ValueChanged(object sender, EventArgs e)
        {
            for (int s = 0; s < Values.STATS_COUNT; s++)
                _statControls[s].IBM = (double)nudIBM.Value;
        }

        private void rbWild_CheckedChanged(object sender, EventArgs e)
        {
            if (rbWild.Checked)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                    _statControls[s].Wild = rbWild.Checked;
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
                    _statControls[s].Tamed = rbTamed.Checked;
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
                    _statControls[s].Bred = rbBred.Checked;
                nudTE.BackColor = SystemColors.Window;
                nudIB.BackColor = Color.FromArgb(255, 186, 242);
                nudIBM.BackColor = Color.FromArgb(255, 153, 236);
            }
        }

        /// <summary>
        /// Set the used multipliers in the multiplier to the ones of the creature collection setting.
        /// </summary>
        private void SetStatMultipliersFromCC()
        {
            if (_cc?.serverMultipliers?.statMultipliers == null) return;

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                var m = new double[4];
                for (int i = 0; i < 4; i++)
                    m[i] = _cc.serverMultipliers.statMultipliers[s]?[i] ?? 1;
                _statControls[s].StatMultipliers = m;
            }
            SetIBM(_cc.serverMultipliers.BabyImprintingStatScaleMultiplier);

            cbSingleplayerSettings.Checked = _cc.singlePlayerSettings;

            btUseMultipliersFromSettings.Visible = false;
        }

        public void SetSpecies(Species species, bool forceUpdate = false)
        {
            if (species != null && (forceUpdate || cbUpdateOnSpeciesChange.Checked))
            {
                _selectedSpecies = species;

                double?[][] customStatOverrides = null;
                bool customStatsAvailable =
                    _cc?.CustomSpeciesStats?.TryGetValue(species.blueprintPath, out customStatOverrides) ?? false;

                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    _statControls[s].SetStatValues(_selectedSpecies.fullStatsRaw[s], customStatsAvailable ? customStatOverrides?[s] : null);
                    _statControls[s].StatImprintingBonusMultiplier = customStatsAvailable ? customStatOverrides?[Values.STATS_COUNT]?[s] ?? _selectedSpecies.StatImprintMultipliers[s] : _selectedSpecies.StatImprintMultipliers[s];
                    _statControls[s].Visible = species.UsesStat(s);
                }
                _statControls[(int)StatNames.Health].TBHM = _selectedSpecies.TamedBaseHealthMultiplier;
            }
        }

        private void btUpdateSpecies_Click(object sender, EventArgs e)
        {
            SetSpecies(_selectedSpecies, true);
        }

        /// <summary>
        /// Set the stats of a creature to the inputs.
        /// </summary>
        /// <param name="statValues"></param>
        /// <param name="levelsWild"></param>
        /// <param name="levelsDom"></param>
        /// <param name="totalLevel"></param>
        /// <param name="TE">Taming Effectiveness</param>
        /// <param name="IB">Imprinting Bonus of the creature</param>
        /// <param name="tamed"></param>
        /// <param name="bred"></param>
        public void SetCreatureValues(double[] statValues, int[] levelsWild, int[] levelsDom, int totalLevel, double TE, double IB, bool tamed, bool bred)
        {
            int level = 1;
            if (statValues != null)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                    _statControls[s].StatValue = statValues[s];
            }
            if (levelsWild != null)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                    _statControls[s].LevelWild = levelsWild[s];
                level += levelsWild[(int)StatNames.Torpidity];
            }
            if (levelsDom != null)
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    _statControls[s].LevelDom = levelsDom[s];
                    level += levelsDom[s];
                }
            }
            SetTE(TE);
            SetIB(IB);
            if (tamed)
                rbTamed.Checked = true;
            else if (bred)
                rbBred.Checked = true;
            else rbWild.Checked = true;

            nudCreatureLevel.Value = level > totalLevel ? level : totalLevel;

            CheckIfMultipliersAreEqualToSettings();
        }

        /// <summary>
        /// Checks if the currently set multipliers are the ones of the creatureCollection-settings. If not display a warning button.
        /// </summary>
        internal void CheckIfMultipliersAreEqualToSettings()
        {
            bool showWarning = false;
            if (_cc?.serverMultipliers?.statMultipliers != null)
            {
                showWarning = _cc.serverMultipliers.BabyImprintingStatScaleMultiplier != (double)nudIBM.Value
                                || _cc.singlePlayerSettings != cbSingleplayerSettings.Checked;
                if (!showWarning)
                {
                    for (int s = 0; s < Values.STATS_COUNT; s++)
                    {
                        for (int si = 0; si < 4; si++)
                        {
                            showWarning = _cc.serverMultipliers.statMultipliers[s][si] != _statControls[s].StatMultipliers[si];
                            if (showWarning) break;
                        }
                        if (showWarning) break;
                    }
                }
            }
            btUseMultipliersFromSettings.Visible = showWarning;
        }

        private void llStatCalculation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://ark.gamepedia.com/Creature_Stats_Calculation");
        }

        public CreatureCollection CreatureCollection
        {
            set
            {
                _cc = value;
                SetStatMultipliersFromCC();
            }
        }

        private void iwMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool error = false;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s != (int)StatNames.Torpidity)
                    error = !_statControls[s].CalculateIwM() || error;
            }
            if (error) MessageBox.Show("For some stats the IwM couldn't be calculated, because of a Divide by Zero-error, e.g. Lw and Iw needs to be >0.");
        }

        private void idMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool error = false;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s != (int)StatNames.Torpidity)
                    error = !_statControls[s].CalculateIdM() || error;
            }
            if (error) MessageBox.Show("For some stats the IdM couldn't be calculated, because of a Divide by Zero-error, e.g. Ld needs to be at least 1.");
        }

        private void taMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool error = false;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s != (int)StatNames.Torpidity)
                    error = !_statControls[s].CalculateTaM() || error;
            }
            if (error) MessageBox.Show("For some stats the TaM couldn't be calculated, because of a Divide by Zero-error, e.g. Ta needs to be at least 1.");
        }

        private void tmMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool error = false;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s != (int)StatNames.Torpidity)
                    error = !_statControls[s].CalculateTmM() || error;
            }
            if (error) MessageBox.Show("For some stats the TmM couldn't be calculated, because of a Divide by Zero-error, e.g. Tm needs to be at least 1.");
        }

        private void useStatMultipliersOfSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStatMultipliersFromCC();
        }

        private void useDefaultStatMultipliersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerMultipliers officialSM = Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.OFFICIAL);
            if (officialSM == null) return;
            for (int s = 0; s < Values.STATS_COUNT; s++)
                _statControls[s].StatMultipliers = officialSM.statMultipliers[s];
        }

        private void copyStatMultipliersToSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_cc?.serverMultipliers?.statMultipliers == null) return;

            for (int s = 0; s < Values.STATS_COUNT; s++)
                _cc.serverMultipliers.statMultipliers[s] = _statControls[s].StatMultipliers;
            _cc.serverMultipliers.BabyImprintingStatScaleMultiplier = (double)nudIBM.Value;
            _cc.singlePlayerSettings = cbSingleplayerSettings.Checked;
            OnApplyMultipliers?.Invoke();
            btUseMultipliersFromSettings.Visible = false;
        }

        private void tbFineAdjustments_Scroll(object sender, EventArgs e)
        {
            if (_fineAdjustmentsNud != null)
                _fineAdjustmentsNud.ValueSave = (decimal)((_fineAdjustmentRange.Min + (_fineAdjustmentRange.Max - _fineAdjustmentRange.Min) * 0.01 * tbFineAdjustments.Value) * _fineAdjustmentFactor);
        }

        private void SetFineAdjustmentNUD(Nud nud, string title, double min, double max)
        {
            gbFineAdjustment.Visible = true;
            _fineAdjustmentRange.Min = min;
            _fineAdjustmentRange.Max = max;
            _fineAdjustmentsNud = nud;
            gbFineAdjustment.Text = title;
            _fineAdjustmentFactor = nud == nudIB || nud == nudTE ? 100 : 1;
        }

        private void cbSingleplayerSettings_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSingleplayerSettings.Checked)
            {
                ServerMultipliers spM = Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.SINGLEPLAYER);
                if (spM != null)
                {
                    for (int s = 0; s < Values.STATS_COUNT; s++)
                    {
                        if (spM.statMultipliers[s] == null)
                            _statControls[s].SetSinglePlayerSettings();
                        else
                            _statControls[s].SetSinglePlayerSettings(spM.statMultipliers[s][3], spM.statMultipliers[s][2], spM.statMultipliers[s][0], spM.statMultipliers[s][1]);
                    }
                    return;
                }
            }
            for (int s = 0; s < Values.STATS_COUNT; s++)
                _statControls[s].SetSinglePlayerSettings();
        }

        private void allWildLvlToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Utils.ShowTextInput("Wild Level", out string nr, "", "0") && int.TryParse(nr, out int lv))
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                    if (_selectedSpecies.UsesStat(s)) _statControls[s].LevelWild = lv;
            }
        }

        private void allDomLvlToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Utils.ShowTextInput("Dom Level", out string nr, "", "0") && int.TryParse(nr, out int lv))
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                    if (_selectedSpecies.UsesStat(s)) _statControls[s].LevelDom = lv;
            }
        }

        private void setAllWildLevelsToTheClosestValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int s = 0; s < Values.STATS_COUNT; s++)
                if (_selectedSpecies.UsesStat(s)) _statControls[s].SetClosestWildLevel();
        }

        private void setAllDomLevelsToTheClosestValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int s = 0; s < Values.STATS_COUNT; s++)
                if (_selectedSpecies.UsesStat(s)) _statControls[s].SetClosestDomLevel();
        }

        private void btUseMultipliersFromSettings_Click(object sender, EventArgs e)
        {
            SetStatMultipliersFromCC();
        }
    }
}
