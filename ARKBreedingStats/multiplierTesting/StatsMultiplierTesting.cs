using ARKBreedingStats.Library;
using ARKBreedingStats.miscClasses;
using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using ARKBreedingStats.utils;
using System.Linq;
using System.Text;
using System.Threading;
using ARKBreedingStats.importExportGun;

namespace ARKBreedingStats.multiplierTesting
{
    public partial class StatsMultiplierTesting : UserControl
    {
        public event Action OnApplyMultipliers;
        public event Form1.SetMessageLabelTextEventHandler SetMessageLabelText;

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
                var sc = new StatMultiplierTestingControl(_tt);
                if (Stats.IsPercentage(s))
                    sc.Percent = true;
                sc.OnLevelChanged += Sc_OnLevelChanged;
                sc.OnTECalculated += SetTE;
                sc.OnIBCalculated += SetIB;
                sc.OnIBMCalculated += SetIBM;
                _statControls[s] = sc;
            }
            // add controls in order like in-game
            foreach (var s in Stats.DisplayOrder)
            {
                flowLayoutPanel1.Controls.Add(_statControls[s]);
                flowLayoutPanel1.SetFlowBreak(_statControls[s], true);
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
            var te = (double)nudTE.Value / 100;
            for (int s = 0; s < Stats.StatsCount; s++)
                _statControls[s].TE = te;
            if (rbTamed.Checked)
                LbCalculatedWildLevel.Text = $"LW: {Creature.CalculatePreTameWildLevel(_statControls[Stats.Torpidity].LevelWild + 1, te)}";
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

            cbSingleplayerSettings.Checked = _cc.serverMultipliers.SinglePlayerSettings;
            CbAtlas.Checked = _cc.serverMultipliers.AtlasSettings;
            CbAllowSpeedLeveling.Checked = _cc.serverMultipliers.AllowSpeedLeveling;
            CbAllowFlyerSpeedLeveling.Checked = _cc.serverMultipliers.AllowFlyerSpeedLeveling;

            btUseMultipliersFromSettings.Visible = false;
        }

        public void SetSpecies(Species species, bool forceUpdate = false)
        {
            if (species == null ||
                (!forceUpdate && (_selectedSpecies == species || !cbUpdateOnSpeciesChange.Checked))) return;

            _selectedSpecies = species;
            BtResetSpeciesValues.Text = $"Reset species values - {_selectedSpecies.DescriptiveNameAndMod}";
            LbBlueprintPath.Text = $"BlueprintPath: {species.blueprintPath}";

            double?[][] customStatOverrides = null;
            bool customStatsAvailable =
                _cc?.CustomSpeciesStats?.TryGetValue(species.blueprintPath, out customStatOverrides) ?? false;

            var statImprintMultipliers = _selectedSpecies.StatImprintMultipliers;

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                _statControls[s].SetStatValues(_selectedSpecies.fullStatsRaw[s], customStatsAvailable ? customStatOverrides?[s] : null,
                    _selectedSpecies.altBaseStatsRaw != null && _selectedSpecies.altBaseStatsRaw.TryGetValue(s, out var altV) ? altV / _selectedSpecies.fullStatsRaw[s][Species.StatsRawIndexBase] : 1,
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

            if (bred)
                rbBred.Checked = true;
            else if (tamed)
                rbTamed.Checked = true;
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
                                || _cc.serverMultipliers.SinglePlayerSettings != cbSingleplayerSettings.Checked
                                || _cc.serverMultipliers.AtlasSettings != CbAtlas.Checked
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
            if (error) SetMessageLabelText?.Invoke("For some stats the IwM couldn't be calculated, because of a Divide by Zero-error, e.g. Lw and Iw needs to be >0.", MessageBoxIcon.Error);
        }

        private void idMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool error = false;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s != Stats.Torpidity)
                    error = !_statControls[s].CalculateIdM() || error;
            }
            if (error) SetMessageLabelText?.Invoke("For some stats the IdM couldn't be calculated, because of a Divide by Zero-error, e.g. Ld needs to be at least 1.", MessageBoxIcon.Error);
        }

        private void taMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool error = false;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s != Stats.Torpidity)
                    error = !_statControls[s].CalculateTaM() || error;
            }
            if (error) SetMessageLabelText?.Invoke("For some stats the TaM couldn't be calculated, because of a Divide by Zero-error, e.g. Ta needs to be at least 1.", MessageBoxIcon.Error);
        }

        private void tmMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool error = false;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s != Stats.Torpidity)
                    error = !_statControls[s].CalculateTmM() || error;
            }
            if (error) SetMessageLabelText?.Invoke("For some stats the TmM couldn't be calculated, because of a Divide by Zero-error, e.g. Tm needs to be at least 1.", MessageBoxIcon.Error);
        }

        private void allIwToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool error = false;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                error = !_statControls[s].CalculateIw() || error;
            }
            if (error) SetMessageLabelText?.Invoke("Divide by Zero-error, e.g. Lw or IwM needs to be greater than 0.", MessageBoxIcon.Error);
        }

        private void allIdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool error = false;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s != Stats.Torpidity)
                    error = !_statControls[s].CalculateId() || error;
            }
            if (error) SetMessageLabelText?.Invoke("Divide by Zero-error, e.g. Ld needs to be at least 1.", MessageBoxIcon.Error);

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
            _cc.serverMultipliers.SinglePlayerSettings = cbSingleplayerSettings.Checked;
            _cc.serverMultipliers.AtlasSettings = CbAtlas.Checked;
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
                            _statControls[s].SetSinglePlayerSettings(spM.statMultipliers[s][ServerMultipliers.IndexLevelWild],
                                spM.statMultipliers[s][ServerMultipliers.IndexLevelDom],
                                spM.statMultipliers[s][ServerMultipliers.IndexTamingAdd],
                                spM.statMultipliers[s][ServerMultipliers.IndexTamingMult]);
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
                    _selectedSpecies.altBaseStatsRaw != null
                    && _selectedSpecies.altBaseStatsRaw.TryGetValue(Stats.SpeedMultiplier, out var altV)
                        ? altV / _selectedSpecies.fullStatsRaw[Stats.SpeedMultiplier][Species.StatsRawIndexBase] : 1,
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
            _tt.SetToolTip(LbCalculatedWildLevel, "Calculated pre tame level, dependent on the taming effectiveness and the post tame level");
            _tt.SetToolTip(LbSpeciesValuesExtractor, @"Drop export gun files or folders with export gun files on this label to extract the species stat values.
If one of the files is an export gun server multiplier file, its values are used.
To determine all species values, the files with the following creature combinations are needed
* wild level 1 creature for base values
* wild creature with at least one level in all possible stats
* two tamed creature with no applied levels and different TE (TE difference should be large to avoid rounding errors, at least 10 %points difference should be good) and different wild levels in HP (for TBHM)
* a tamed creature with at least one level in all possible stats
* a creature with imprinting (probably an imprinting value of at least 10 % should result in good results) to determine which stats are effected by imprinting in what extend
");
        }

        private void StatsMultiplierTesting_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        /// <summary>
        /// Use stat values of file, and if it's an export gun files also the levels.
        /// </summary>
        private void StatsMultiplierTesting_DragDrop(object sender, DragEventArgs e)
        {
            if (!(e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Any()))
                return;
            ProcessDroppedFiles(files);
        }

        private void ProcessDroppedFiles(string[] files)
        {
            var creatureImported = false; // only import one creature (second would only overwrite first)
            var serverMultipliersImported = false;
            var errorOccured = false;
            var results = new List<string>();
            string lastFilePath = null;
            foreach (var filePath in files)
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) continue;

                var extension = Path.GetExtension(filePath);
                lastFilePath = filePath;
                switch (extension)
                {
                    case ".ini":
                        // export file
                        if (creatureImported) continue;
                        var cv = importExported.ImportExported.ReadExportedCreature(filePath);
                        SetCreatureValueValues(cv);
                        results.Add($"imported creature values from {filePath}");
                        continue;
                    case ".sav":
                    case ".json":
                        // export gun file of ASE
                        string lastError = null;
                        string serverMultipliersHash = null;
                        Creature creature = null;
                        double[] statValues = null;
                        if (!creatureImported)
                            creature = ImportExportGun.LoadCreature(filePath, out lastError, out serverMultipliersHash, out statValues, true);
                        if (creature != null)
                        {
                            SetCreatureValuesAndLevels(creature, statValues);
                            results.Add($"imported creature values and levels from {filePath}{(string.IsNullOrEmpty(lastError) ? string.Empty : $". {lastError}")}");
                            creatureImported = true;
                        }
                        else if (lastError != null)
                        {
                            if (!serverMultipliersImported)
                            {
                                // file could be a server multiplier file, try to read it that way
                                var esm = ImportExportGun.ReadServerMultipliers(filePath, out var serverImportResult);
                                if (esm != null)
                                {
                                    SetServerMultipliers(esm);
                                    results.Add($"imported server multipliers from {filePath}");
                                    serverMultipliersImported = true;
                                    continue;
                                }
                                if (string.IsNullOrEmpty(serverImportResult))
                                    results.Add(serverImportResult);
                                continue;
                            }

                            results.Add($"error when importing file {filePath}: {lastError}");
                            errorOccured = true;
                        }

                        break;
                }
            }
            SetMessageLabelText?.Invoke(string.Join(Environment.NewLine, results),
                errorOccured ? MessageBoxIcon.Error : MessageBoxIcon.Information, lastFilePath);
        }

        private void SetCreatureValueValues(CreatureValues cv)
        {
            SetCreatureValues(cv.statValues, null, null, cv.level, (cv.tamingEffMax - cv.tamingEffMin) / 2, cv.imprintingBonus, cv.isTamed, cv.isBred, cv.Species);
        }

        private void SetCreatureValuesAndLevels(Creature cr, double[] statValues = null)
        {
            var levelsWildAndMutated = cr.levelsWild.ToArray();
            if (cr.levelsMutated != null)
            {
                for (int si = 0; si < Stats.StatsCount; si++)
                    levelsWildAndMutated[si] = cr.levelsWild[si] + cr.levelsMutated[si];
            }
            SetCreatureValues(statValues ?? cr.valuesDom, levelsWildAndMutated, cr.levelsDom, cr.Level, cr.tamingEff, cr.imprintingBonus, cr.isDomesticated, cr.isBred, cr.Species);
        }

        private void SetServerMultipliers(ExportGunServerFile esm)
        {
            if (esm?.WildLevel == null) return;

            for (int si = 0; si < Stats.StatsCount; si++)
            {
                _statControls[si].StatMultipliers = new[]
                    { esm.TameAdd[si], esm.TameAff[si], esm.TameLevel[si], esm.WildLevel[si] };
            }
            SetIBM(esm.BabyImprintingStatScaleMultiplier);

            cbSingleplayerSettings.Checked = esm.UseSingleplayerSettings;
            CbAtlas.Checked = false; // atlas has no export gun mod
            CbAllowSpeedLeveling.Checked = esm.AllowSpeedLeveling;
            CbAllowFlyerSpeedLeveling.Checked = esm.AllowFlyerSpeedLeveling;

            CheckIfMultipliersAreEqualToSettings();
        }

        /// <summary>
        /// Returns the currently set server multipliers in an exportGunServerFile.
        /// </summary>
        private ExportGunServerFile GetServerMultipliers()
        {
            var esm = new ExportGunServerFile
            {
                BabyImprintingStatScaleMultiplier = nudIBM.ValueDouble,
                UseSingleplayerSettings = cbSingleplayerSettings.Checked,
                AllowSpeedLeveling = CbAllowSpeedLeveling.Checked,
                AllowFlyerSpeedLeveling = CbAllowFlyerSpeedLeveling.Checked,
                WildLevel = new double[Stats.StatsCount],
                TameLevel = new double[Stats.StatsCount],
                TameAdd = new double[Stats.StatsCount],
                TameAff = new double[Stats.StatsCount]
            };

            for (int si = 0; si < Stats.StatsCount; si++)
            {
                var mults = _statControls[si].StatMultipliers;
                esm.WildLevel[si] = mults[3];
                esm.TameLevel[si] = mults[2];
                esm.TameAdd[si] = mults[0];
                esm.TameAff[si] = mults[1];
            }

            return esm;
        }

        private void copyStatValuesToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySpeciesStatsToClipboard();
        }

        private void CopySpeciesStatsToClipboard(string speciesBlueprintPath = null, double[] speciesImprintingMultipliers = null)
        {
            // copy stat values in the format of the values.json to clipboard
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(speciesBlueprintPath))
                sb.AppendLine($"\"blueprintPath\": \"{speciesBlueprintPath}\",");
            sb.AppendLine("\"fullStatsRaw\": [");
            var currentCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            for (var s = 0; s < Stats.StatsCount; s++)
            {
                var sv = _statControls[s].StatValues;
                if (sv == null || sv.All(v => v == 0))
                {
                    sb.Append("    null");
                }
                else
                {
                    sb.Append($"    [ {string.Join(", ", sv)} ]");
                }

                sb.AppendLine(s + 1 < Stats.StatsCount ? "," : string.Empty);
            }
            sb.AppendLine("]");

            if (speciesImprintingMultipliers != null)
            {
                sb.AppendLine($"\"statImprintMult\": [ {string.Join(", ", speciesBlueprintPath)} ]");
            }

            CultureInfo.CurrentCulture = currentCulture;
            Clipboard.SetText(sb.ToString());
            SetMessageLabelText?.Invoke("Raw stat values copied to clipboard.", MessageBoxIcon.Information);
        }

        private void LbSpeciesValuesExtractor_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            e.Effect = DragDropEffects.Copy;
            LbSpeciesValuesExtractor.BackColor = Color.LightGreen;
        }

        private void LbSpeciesValuesExtractor_DragLeave(object sender, EventArgs e)
        {
            LbSpeciesValuesExtractor.BackColor = Color.White;
        }

        private void LbSpeciesValuesExtractor_DragDrop(object sender, DragEventArgs e)
        {
            LbSpeciesValuesExtractor.BackColor = Color.White;
            if (!(e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Any()))
                return;
            ExtractSpeciesValuesFromExportFiles(files);
        }

        private void ExtractSpeciesValuesFromExportFiles(string[] files)
        {
            // only export gun files are supported
            var creatureFiles = new List<ExportGunCreatureFile>();
            ExportGunServerFile serverMultipliersFile = null;
            string lastError = null;

            var filePaths = new List<string>();
            foreach (var filePath in files)
            {
                if (File.Exists(filePath))
                    filePaths.Add(filePath);
                else if (Directory.Exists(filePath))
                    filePaths.AddRange(Directory.GetFiles(filePath, "*", SearchOption.AllDirectories));
            }

            foreach (var filePath in filePaths)
            {
                var creature = ImportExportGun.LoadCreatureFile(filePath, out lastError, out _);
                if (creature != null)
                {
                    creatureFiles.Add(creature);
                    continue;
                }

                var svMults = ImportExportGun.ReadServerMultipliers(filePath, out _);
                if (svMults != null)
                    serverMultipliersFile = svMults;
            }

            if (!creatureFiles.Any())
            {
                if (!string.IsNullOrEmpty(lastError))
                    lastError = Environment.NewLine + lastError;
                MessageBoxes.ShowMessageBox("No creature files could be read" + lastError);
                return;
            }

            if (serverMultipliersFile != null)
                SetServerMultipliers(serverMultipliersFile);

            var sm = new ServerMultipliers(true);
            ImportExportGun.SetServerMultipliers(sm, serverMultipliersFile ?? GetServerMultipliers());

            SpeciesStatsExtractor.ExtractStatValues(creatureFiles, sm, out var species, out var resultText, out var isError);
            SetSpecies(species);

            if (isError)
            {
                SetMessageLabelText?.Invoke("Error while trying to determine the species stats." + Environment.NewLine + resultText, MessageBoxIcon.Error);
                return;
            }

            CopySpeciesStatsToClipboard(species.blueprintPath, species.StatImprintMultipliersRaw);
            if (!string.IsNullOrEmpty(resultText))
                resultText += Environment.NewLine;
            SetMessageLabelText?.Invoke(resultText +
                "Extracted the species values and copied them to the clipboard. Note the TBHM and singleplayer is not supported and may lead to wrong values.",
                MessageBoxIcon.Information);
        }

        private void openWikiPageOnStatCalculationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArkWiki.OpenPage("Creature stats calculation");
        }
    }
}
