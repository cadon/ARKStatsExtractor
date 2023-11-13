using ARKBreedingStats.Library;
using ARKBreedingStats.miscClasses;
using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.values;
using System;
using System.Drawing;
using System.Windows.Forms;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.multiplierTesting
{
    public partial class StatsMultiplierTesting : UserControl
    {
        public event Action OnApplyMultipliers;

        private readonly StatMultiplierTestingControl[] _statControls;
        private CreatureCollection _cc;
        private Species _selectedSpecies;
        private Nud _fineAdjustmentsNud;
        private MinMaxDouble _fineAdjustmentRange;
        private double _fineAdjustmentFactor;
        private ToolTip _tt = new ToolTip();

        public StatsMultiplierTesting()
        {
            InitializeComponent();

            _statControls = new StatMultiplierTestingControl[Stats.StatsCount];
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                var sc = new StatMultiplierTestingControl();
                if (Utils.Precision(s) == 3)
                    sc.Percent = true;
                sc.OnLevelChanged += Sc_OnLevelChanged;
                sc.OnTECalculated += SetTE;
                sc.OnIBCalculated += SetIB;
                sc.OnIBMCalculated += SetIBM;
                _statControls[s] = sc;
            }
            // add controls in order like in-game
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                flowLayoutPanel1.Controls.Add(_statControls[Stats.DisplayOrder[s]]);
                flowLayoutPanel1.SetFlowBreak(_statControls[Stats.DisplayOrder[s]], true);
            }

            // set bottom controls to bottom
            flowLayoutPanel1.Controls.Add(gbLevel);
            flowLayoutPanel1.Controls.Add(LbAbbreviations);

            _fineAdjustmentRange = new MinMaxDouble(0);
            rbTamed.Checked = true;
            gbFineAdjustment.Hide();
            SetToolTips();
        }

        internal void SetGameDefaultMultiplier()
        {
            var officialSm = Values.V.serverMultipliersPresets?.GetPreset(ServerMultipliersPresets.Official);
            if (officialSm != null)
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                    _statControls[s].StatMultipliersGameDefault = officialSm.statMultipliers[s];
            }
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
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s == Stats.Torpidity) continue;
                sumW += _statControls[s].LevelWild;
                sumD += _statControls[s].LevelDom;
            }
            lbLevelSumWild.Text = "Sum LevelWild = " + sumW;
            bool positive;
            int diff = sumW - _statControls[Stats.Torpidity].LevelWild;
            if (diff != 0)
            {
                positive = diff > 0;
                if (!positive) diff = -diff;
                lbLevelSumWild.Text += $" ({(positive ? "+" : "")}{(sumW - _statControls[Stats.Torpidity].LevelWild)})";

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
            for (int s = 0; s < Stats.StatsCount; s++)
                _statControls[s].TE = (double)nudTE.Value / 100;
            if (rbTamed.Checked)
                LbCalculatedWildLevel.Text = $"LW: {Creature.CalculatePreTameWildLevel(_statControls[Stats.Torpidity].LevelWild + 1, (double)nudTE.Value / 100)}";
        }

        private void nudIB_ValueChanged(object sender, EventArgs e)
        {
            for (int s = 0; s < Stats.StatsCount; s++)
                _statControls[s].IB = (double)nudIB.Value / 100;
        }

        private void nudIBM_ValueChanged(object sender, EventArgs e)
        {
            for (int s = 0; s < Stats.StatsCount; s++)
                _statControls[s].IBM = (double)nudIBM.Value;
        }

        private void rbWild_CheckedChanged(object sender, EventArgs e)
        {
            if (rbWild.Checked)
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                    _statControls[s].Wild = true;
                nudTE.BackColor = SystemColors.Window;
                nudIB.BackColor = SystemColors.Window;
                nudIBM.BackColor = SystemColors.Window;
                LbCalculatedWildLevel.Visible = false;
            }
        }

        private void rbTamed_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTamed.Checked)
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                    _statControls[s].Tamed = true;
                nudTE.BackColor = Color.FromArgb(215, 186, 255);
                nudIB.BackColor = SystemColors.Window;
                nudIBM.BackColor = SystemColors.Window;
                LbCalculatedWildLevel.Visible = true;
            }
        }

        private void rbBred_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBred.Checked)
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                    _statControls[s].Bred = true;
                nudTE.BackColor = SystemColors.Window;
                nudIB.BackColor = Color.FromArgb(255, 186, 242);
                nudIBM.BackColor = Color.FromArgb(255, 153, 236);
                LbCalculatedWildLevel.Visible = false;
            }
        }

        /// <summary>
        /// Set the used multipliers in the multiplier to the ones of the creature collection setting.
        /// </summary>
        private void SetStatMultipliersFromCC()
        {
            if (_cc?.serverMultipliers?.statMultipliers == null) return;

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                var m = new double[4];
                for (int i = 0; i < 4; i++)
                    m[i] = _cc.serverMultipliers.statMultipliers[s]?[i] ?? 1;
                _statControls[s].StatMultipliers = m;
            }
            SetIBM(_cc.serverMultipliers.BabyImprintingStatScaleMultiplier);

            cbSingleplayerSettings.Checked = _cc.singlePlayerSettings;
            CbAtlas.Checked = _cc.AtlasSettings;
            CbAllowSpeedLeveling.Checked = _cc.serverMultipliers.AllowSpeedLeveling;
            CbAllowFlyerSpeedLeveling.Checked = _cc.serverMultipliers.AllowFlyerSpeedLeveling;

            btUseMultipliersFromSettings.Visible = false;
        }

        public void SetSpecies(Species species, bool forceUpdate = false)
        {
            if (species == null ||
                (!forceUpdate && (_selectedSpecies == species || !cbUpdateOnSpeciesChange.Checked))) return;

            _selectedSpecies = species;
            LbBlueprintPath.Text = $"BlueprintPath: {species.blueprintPath}";

            double?[][] customStatOverrides = null;
            bool customStatsAvailable =
                _cc?.CustomSpeciesStats?.TryGetValue(species.blueprintPath, out customStatOverrides) ?? false;

            var statImprintMultipliers = _selectedSpecies.StatImprintMultipliers;

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _statControls[s].SetStatValues(_selectedSpecies.fullStatsRaw[s], customStatsAvailable ? customStatOverrides?[s] : null,
                    _selectedSpecies.altBaseStatsRaw != null && _selectedSpecies.altBaseStatsRaw.TryGetValue(s, out var altV) ? altV / _selectedSpecies.fullStatsRaw[s][0] : 1,
                    s == Stats.SpeedMultiplier && !(CbAllowSpeedLeveling.Checked && (CbAllowFlyerSpeedLeveling.Checked || !species.isFlyer)));
                _statControls[s].StatImprintingBonusMultiplier = customStatsAvailable ? customStatOverrides?[Stats.StatsCount]?[s] ?? statImprintMultipliers[s] : statImprintMultipliers[s];
                _statControls[s].Visible = species.UsesStat(s);
                _statControls[s].StatName = $"[{s}]{Utils.StatName(s, true, species.statNames)}";
            }
            _statControls[Stats.Health].TBHM = _selectedSpecies.TamedBaseHealthMultiplier ?? 1;
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
        public void SetCreatureValues(double[] statValues, int[] levelsWild, int[] levelsDom, int totalLevel, double TE, double IB, bool tamed, bool bred, Species species)
        {
            int level = 1;

            for (int s = 0; s < Stats.StatsCount; s++)
                _statControls[s].BeginUpdate();

            SetSpecies(species);

            if (statValues != null)
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                    _statControls[s].StatValue = statValues[s];
            }
            if (levelsWild != null)
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                    _statControls[s].LevelWild = levelsWild[s];
                level += levelsWild[Stats.Torpidity];
            }
            if (levelsDom != null)
            {
                for (int s = 0; s < Stats.StatsCount; s++)
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

            for (int s = 0; s < Stats.StatsCount; s++)
                _statControls[s].EndUpdate(true);

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
                                || _cc.singlePlayerSettings != cbSingleplayerSettings.Checked
                                || _cc.AtlasSettings != CbAtlas.Checked
                                || _cc.serverMultipliers.AllowSpeedLeveling != CbAllowSpeedLeveling.Checked
                                || _cc.serverMultipliers.AllowFlyerSpeedLeveling != CbAllowFlyerSpeedLeveling.Checked;
                if (!showWarning)
                {
                    for (int s = 0; s < Stats.StatsCount; s++)
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
            ArkWiki.OpenPage("Creature_stats_calculation");
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
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s != Stats.Torpidity)
                    error = !_statControls[s].CalculateIwM() || error;
            }
            if (error) MessageBox.Show("For some stats the IwM couldn't be calculated, because of a Divide by Zero-error, e.g. Lw and Iw needs to be >0.");
        }

        private void idMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool error = false;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s != Stats.Torpidity)
                    error = !_statControls[s].CalculateIdM() || error;
            }
            if (error) MessageBox.Show("For some stats the IdM couldn't be calculated, because of a Divide by Zero-error, e.g. Ld needs to be at least 1.");
        }

        private void taMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool error = false;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s != Stats.Torpidity)
                    error = !_statControls[s].CalculateTaM() || error;
            }
            if (error) MessageBox.Show("For some stats the TaM couldn't be calculated, because of a Divide by Zero-error, e.g. Ta needs to be at least 1.");
        }

        private void tmMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool error = false;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s != Stats.Torpidity)
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
            ServerMultipliers officialSM = Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.Official);
            if (officialSM == null) return;
            for (int s = 0; s < Stats.StatsCount; s++)
                _statControls[s].StatMultipliers = officialSM.statMultipliers[s];
        }

        private void copyStatMultipliersToSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_cc?.serverMultipliers?.statMultipliers == null) return;

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _cc.serverMultipliers.statMultipliers[s] = _statControls[s].StatMultipliers;
                _statControls[s].SetStatMultiplierWithoutChangingInputs(_statControls[s].StatMultipliers);
            }
            _cc.serverMultipliers.BabyImprintingStatScaleMultiplier = (double)nudIBM.Value;
            _cc.singlePlayerSettings = cbSingleplayerSettings.Checked;
            _cc.AtlasSettings = CbAtlas.Checked;
            _cc.serverMultipliers.AllowSpeedLeveling = CbAllowSpeedLeveling.Checked;
            _cc.serverMultipliers.AllowFlyerSpeedLeveling = CbAllowFlyerSpeedLeveling.Checked;
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
                ServerMultipliers spM = Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.Singleplayer);
                if (spM != null)
                {
                    for (int s = 0; s < Stats.StatsCount; s++)
                    {
                        if (spM.statMultipliers[s] == null)
                            _statControls[s].SetSinglePlayerSettings();
                        else
                            _statControls[s].SetSinglePlayerSettings(spM.statMultipliers[s][Stats.IndexLevelWild], spM.statMultipliers[s][Stats.IndexLevelDom], spM.statMultipliers[s][Stats.IndexTamingAdd], spM.statMultipliers[s][Stats.IndexTamingMult]);
                    }
                    return;
                }
            }
            for (int s = 0; s < Stats.StatsCount; s++)
                _statControls[s].SetSinglePlayerSettings();
        }

        private void CbAtlas_CheckedChanged(object sender, EventArgs e)
        {
            var useAtlas = CbAtlas.Checked;
            _statControls[Stats.Health].AtlasBaseMultiplier = useAtlas ? 1.25 : 1;
            _statControls[Stats.Health].AtlasIdMultiplier = useAtlas ? 1.5 : 1;
            _statControls[Stats.Weight].AtlasIdMultiplier = useAtlas ? 1.5 : 1;
            _statControls[Stats.MeleeDamageMultiplier].AtlasIdMultiplier = useAtlas ? 1.5 : 1;
        }

        private void CbAllowSpeedLeveling_CheckedChanged(object sender, EventArgs e)
        {
            SetAllowSpeedLeveling(CbAllowSpeedLeveling.Checked, CbAllowFlyerSpeedLeveling.Checked);
        }

        private void CbAllowFlyerSpeedLeveling_CheckedChanged(object sender, EventArgs e)
        {
            SetAllowSpeedLeveling(CbAllowSpeedLeveling.Checked, CbAllowFlyerSpeedLeveling.Checked);
        }

        private void SetAllowSpeedLeveling(bool allowSpeedLeveling, bool allowFlyerSpeedleveling)
        {
            if (_selectedSpecies == null) return;
            var speedLevelingAllowed = allowSpeedLeveling && (allowFlyerSpeedleveling || !_selectedSpecies.isFlyer);

            double?[][] customStatOverrides = null;
            bool customStatsAvailable =
                _cc?.CustomSpeciesStats?.TryGetValue(_selectedSpecies.blueprintPath, out customStatOverrides) ?? false;

            _statControls[Stats.SpeedMultiplier].SetStatValues(_selectedSpecies.fullStatsRaw[Stats.SpeedMultiplier], customStatsAvailable ? customStatOverrides?[Stats.SpeedMultiplier] : null,
                    _selectedSpecies.altBaseStatsRaw != null && _selectedSpecies.altBaseStatsRaw.TryGetValue(Stats.SpeedMultiplier, out var altV) ? altV / _selectedSpecies.fullStatsRaw[Stats.SpeedMultiplier][0] : 1,
                    !speedLevelingAllowed);
        }

        private void allWildLvlToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Utils.ShowTextInput("Wild Level", out string nr, "", "0") && int.TryParse(nr, out int lv))
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                    if (_selectedSpecies.UsesStat(s)) _statControls[s].LevelWild = lv;
            }
        }

        private void allDomLvlToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Utils.ShowTextInput("Dom Level", out string nr, "", "0") && int.TryParse(nr, out int lv))
            {
                for (int s = 0; s < Stats.StatsCount; s++)
                    if (_selectedSpecies.UsesStat(s) && s != Stats.Torpidity) _statControls[s].LevelDom = lv;
            }
        }

        private void setAllWildLevelsToTheClosestValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int s = 0; s < Stats.StatsCount; s++)
                if (_selectedSpecies.UsesStat(s)) _statControls[s].SetClosestWildLevel();
        }

        private void setAllDomLevelsToTheClosestValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int s = 0; s < Stats.StatsCount; s++)
                if (_selectedSpecies.UsesStat(s)) _statControls[s].SetClosestDomLevel();
        }

        private void btUseMultipliersFromSettings_Click(object sender, EventArgs e)
        {
            SetStatMultipliersFromCC();
        }

        private void SetToolTips()
        {
            _tt.SetToolTip(LbBaseValue, "Base value | Max status value");
            _tt.SetToolTip(LbLw, "Wild levels | Points applied wild");
            _tt.SetToolTip(LbIw, "Increase per wild level | Amount max gained per level up value wild");
            _tt.SetToolTip(LbIwM, "Increase per wild level global multiplier | per level stats multiplier dino wild");
            _tt.SetToolTip(LbTBHM, "Tamed base health multiplier");
            _tt.SetToolTip(LbTa, "Additive taming bonus | Taming max stat additions");
            _tt.SetToolTip(LbTaM, "Additive taming bonus global multiplier | per level stats multiplier dino tamed add");
            _tt.SetToolTip(LbTm, "Multiplicative taming bonus | Taming max stat multiplier");
            _tt.SetToolTip(LbTmM, "Multiplicative taming bonus global multiplier | per level stats multiplier dino tamed affinity");
            _tt.SetToolTip(LbLd, "Domesticate levels | Points applied tamed");
            _tt.SetToolTip(LbId, "Increase per domesticate level | Amount max gained per level up value tamed");
            _tt.SetToolTip(LbIdM, "Increase per domestic level global multiplier | per level stats multiplier dino tamed");
            _tt.SetToolTip(LbFinalValue, "Final stat value displayed in the game");
        }
    }
}
