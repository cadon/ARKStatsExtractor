using ARKBreedingStats.miscClasses;
using ARKBreedingStats.uiControls;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;

namespace ARKBreedingStats.multiplierTesting
{
    public partial class StatMultiplierTestingControl : UserControl
    {
        /// <summary>
        /// Level of the stat changed.
        /// </summary>
        public event Action OnLevelChanged;
        /// <summary>
        /// Value of taming effectiveness was calculated.
        /// </summary>
        public event Action<MinMaxDouble> OnTECalculated;
        /// <summary>
        /// Value of imprinting bonus was calculated.
        /// </summary>
        public event Action<MinMaxDouble> OnIBCalculated;
        /// <summary>
        /// Value of imprinting bonus multiplier was calculated.
        /// </summary>
        public event Action<MinMaxDouble> OnIBMCalculated;

        public bool updateValues;
        private bool _tamed;
        private bool _bred;
        private double _IB;
        private double _IBM;
        private double _TE;
        /// <summary>
        /// Stat is shown in percentage
        /// </summary>
        private bool _percent;
        /// <summary>
        /// Value of wild and taming bonus, without dom levels
        /// </summary>
        private double Vd;
        /// <summary>
        /// Final value with all levels and bonus
        /// </summary>
        private double V;
        /// <summary>
        /// No imprinting bonus
        /// </summary>
        private bool _NoIB;
        /// <summary>
        /// Stat Imprinting Bonus Multiplier
        /// </summary>
        private double _sIBM;
        /// <summary>
        /// Singleplayer extra multiplier for increase per wild level.
        /// </summary>
        private double _spIw;
        /// <summary>
        /// Singleplayer extra multiplier for increase per domesticated level.
        /// </summary>
        private double _spId;
        /// <summary>
        /// Singleplayer extra multiplier for taming addition.
        /// </summary>
        private double _spTa;
        /// <summary>
        /// Singleplayer extra multiplier for taming multiplier.
        /// </summary>
        private double _spTm;
        /// <summary>
        /// Values of currently saved settings.
        /// </summary>
        private double[] _multipliersOfSettings;
        /// <summary>
        /// Values of default game values.
        /// </summary>
        public double[] StatMultipliersGameDefault;

        /// <summary>
        /// The values of this stat. 0: Base, 1: Iw, 2: Id, 3: Ta, 4: Tm
        /// </summary>
        private double[] _statValues;
        /// <summary>
        /// The factor the correct value is multiplied with to get the alt / Troodonism value.
        /// </summary>
        private double _altStatFactor;

        /// <summary>
        /// Extra multiplier that is applied to the base value when using the ATLAS multipliers.
        /// </summary>
        public double AtlasBaseMultiplier = 1;
        /// <summary>
        /// Extra multiplier that is used to the IdM when using the ATLAS multipliers.
        /// </summary>
        public double AtlasIdMultiplier = 1;

        private const int DecimalPlaces = 6;

        public StatMultiplierTestingControl()
        {
            InitializeComponent();
            Percent = false;
            _NoIB = false;
            _IBM = 1;
            nudTBHM.NeutralNumber = 1;
            SetSinglePlayerSettings();
            updateValues = true;
        }

        private void UpdateCalculations(bool forceUpdate = false)
        {
            updateValues = updateValues || forceUpdate;
            if (!updateValues) return;

            // ValueWild
            double Vw = (double)nudB.Value * AtlasBaseMultiplier * (1 + (double)nudLw.Value * (double)nudIw.Value * _spIw * (double)nudIwM.Value);
            string VwDisplay = Math.Round(Vw * (_percent ? 100 : 1), DecimalPlaces) + (_percent ? "%" : string.Empty);
            tbVw.Text = $"{nudB.Value + (AtlasBaseMultiplier != 1 ? $" * {AtlasBaseMultiplier}" : string.Empty)} * ( 1 + {nudLw.Value} * {nudIw.Value}{(_spIw != 1 ? " * " + _spIw : string.Empty)} * {nudIwM.Value} ) = {VwDisplay}";
            if (_tamed || _bred)
            {
                // ValueDom
                Vd = (Vw * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1) + (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1))
                        * (1 + (nudTm.Value > 0 ? (_bred ? 1 : _TE) * (double)nudTm.Value * (double)nudTmM.Value * _spTm : (double)nudTm.Value));
                string VdDisplay = Math.Round(Vd * (_percent ? 100 : 1), DecimalPlaces) + (_percent ? "%" : string.Empty);
                tbVd.Text = "( " + VwDisplay + (nudTBHM.Value != 1 ? " * " + nudTBHM.Value : string.Empty) + (!_NoIB && _bred ? " * ( 1 + " + _IB + " * " + _IBM + $" * {_sIBM} )" : string.Empty)
                        + " + " + nudTa.Value + (nudTa.Value > 0 ? " * " + nudTaM.Value + (_spTa != 1 ? " * " + _spTa : string.Empty) : string.Empty) + " ) "
                        + " * ( 1 + " + (nudTm.Value > 0 ? (_bred ? 1 : _TE) + " * " + nudTm.Value + " * " + nudTmM.Value + (_spTm != 1 ? " * " + _spTm : string.Empty) : nudTm.Value.ToString()) + " )"
                        + " = " + VdDisplay;
                // Value
                V = Vd * (1 + (double)nudLd.Value * (double)nudId.Value * _spId * AtlasIdMultiplier * (double)nudIdM.Value);
                string VDisplay = Math.Round(V * (_percent ? 100 : 1), DecimalPlaces) + (_percent ? "%" : string.Empty);
                tbV.Text = $"{VdDisplay} * ( 1 + {nudLd.Value} * {nudId.Value + (_spId != 1 ? " * " + _spId : string.Empty) + (AtlasIdMultiplier != 1 ? " * " + AtlasIdMultiplier : string.Empty)} * {nudIdM.Value} ) = {VDisplay}";
            }
            else
            {
                Vd = Vw;
                V = Vw;
                tbVd.Clear();
                tbV.Text = VwDisplay;
            }

            UpdateMatchingColor();
        }

        public string StatName
        {
            set => lStatName.Text = value;
        }

        public double[] StatMultipliers
        {
            get => new[] { (double)nudTaM.Value, (double)nudTmM.Value, (double)nudIdM.Value, (double)nudIwM.Value };
            set
            {
                if (value == null || value.Length != 4) return;
                _multipliersOfSettings = value;
                var updateValuesKeeper = updateValues;
                updateValues = false;
                // 0:tamingAdd, 1:tamingMult, 2:levelupDom, 3:levelupWild
                nudTaM.Value = (decimal)value[0];
                nudTmM.Value = (decimal)value[1];
                nudIdM.Value = (decimal)value[2];
                nudIwM.Value = (decimal)value[3];
                UpdateCalculations(updateValuesKeeper);
            }
        }

        public void SetStatMultiplierWithoutChangingInputs(double[] multipliersOfSettings)
        {
            if (multipliersOfSettings == null || multipliersOfSettings.Length != 4) return;
            _multipliersOfSettings = multipliersOfSettings;
            // 0:tamingAdd, 1:tamingMult, 2:levelupDom, 3:levelupWild
            SetResetButtonColor(nudTaM, _multipliersOfSettings[0], btResetTaM);
            SetResetButtonColor(nudTmM, _multipliersOfSettings[1], btResetTmM);
            SetResetButtonColor(nudIdM, _multipliersOfSettings[2], btResetIdM);
            SetResetButtonColor(nudIwM, _multipliersOfSettings[3], btResetIwM);
        }

        public void SetStatValues(double[] statValues, double?[] customOverrides, double altStatFactor, bool ignoreIncreaseDom)
        {
            if (statValues != null && statValues.Length == 5)
            {
                _statValues = new double[5];

                var updateValuesKeeper = updateValues;
                updateValues = false;
                nudB.Value = (decimal)(_statValues[0] = customOverrides?[0] ?? statValues[0]);
                nudIw.Value = (decimal)(_statValues[1] = customOverrides?[1] ?? statValues[1]);
                nudId.Value = (decimal)(ignoreIncreaseDom ? 0 : _statValues[2] = customOverrides?[2] ?? statValues[2]);
                nudTa.Value = (decimal)(_statValues[3] = customOverrides?[3] ?? statValues[3]);
                nudTm.Value = (decimal)(_statValues[4] = customOverrides?[4] ?? statValues[4]);

                _altStatFactor = altStatFactor;

                bool altValuesExist = _altStatFactor != 1;

                CbTrodB.Visible = altValuesExist;
                CbTrodIw.Visible = altValuesExist;
                CbTrodId.Visible = altValuesExist;
                // not sure yet if and how exactly these values are affected by the altBaseValue
                CbTrodTa.Visible = false;
                CbTrodTm.Visible = false;

                UpdateCalculations(updateValuesKeeper);
            }
        }

        public double StatValue
        {
            set => nudStatValue.ValueSave = (decimal)value * (_percent ? 100 : 1);
        }

        public int LevelWild
        {
            get => (int)nudLw.Value;
            set => nudLw.ValueSave = value > 0 ? value : 0;
        }

        public int LevelDom
        {
            get => (int)nudLd.Value;
            set => nudLd.ValueSave = value > 0 ? value : 0;
        }

        public bool Wild
        {
            set
            {
                _tamed = false;
                _bred = false;
                UpdateCalculations();
                SetControlsInUse(ControlsInUse.wild);
            }
        }

        public bool Tamed
        {
            set
            {
                if (_tamed != value || _bred)
                {
                    _tamed = value;
                    _bred = false;
                    UpdateCalculations();
                    SetControlsInUse(ControlsInUse.tamed);
                }
            }
        }

        public bool Bred
        {
            set
            {
                if (_bred != value)
                {
                    _bred = value;
                    UpdateCalculations();
                    SetControlsInUse(ControlsInUse.bred);
                }
            }
        }

        public double IB
        {
            set
            {
                if (_IB != value)
                {
                    _IB = value;
                    UpdateCalculations();
                }
            }
        }

        public double IBM
        {
            set
            {
                if (_IBM != value)
                {
                    _IBM = value;
                    UpdateCalculations();
                }
            }
        }

        public double TE
        {
            set
            {
                if (_TE != value)
                {
                    _TE = value;
                    UpdateCalculations();
                }
            }
        }

        public bool Percent
        {
            set
            {
                _percent = value;
                lPercent.Text = _percent ? "%" : string.Empty;
            }
        }

        /// <summary>
        /// Taming Bonus Health Multiplier
        /// </summary>
        public float TBHM
        {
            set => nudTBHM.Value = (decimal)value;
        }

        /// <summary>
        /// Stat Imprint Bonus Multiplier, default is 0.2
        /// </summary>
        public double StatImprintingBonusMultiplier
        {
            set
            {
                _sIBM = value;
                _NoIB = value == 0;
            }
        }

        public void SetSinglePlayerSettings(double? spIw = 1, double? spId = 1, double? spTa = 1, double? spTm = 1)
        {
            _spIw = spIw ?? 1;
            _spId = spId ?? 1;
            _spTa = spTa ?? 1;
            _spTm = spTm ?? 1;
            UpdateCalculations();
        }

        private void nudStatValue_ValueChanged(object sender, EventArgs e)
        {
            UpdateMatchingColor();
        }

        private void UpdateMatchingColor()
        {
            // if matching, color green
            // consider float precision precision and precision loss by calculation
            double inputValue = (double)nudStatValue.Value * (_percent ? .01 : 1);
            float toleranceForThisStat = StatValueCalculation.DisplayedAberration(inputValue, _percent ? 3 : 1);
            MinMaxDouble statValue = new MinMaxDouble(inputValue - toleranceForThisStat, inputValue + toleranceForThisStat);
            if (statValue.Includes(V))
                nudStatValue.BackColor = Color.LightGreen;
            else
            {
                // if not, color red if the value is too low, and blue, if the value is too big
                int proximity = (int)Math.Abs(V - statValue.Mean) / 2;
                if (proximity > 50) proximity = 50;
                nudStatValue.BackColor = Utils.GetColorFromPercent(50 - proximity, 0.6, V < statValue.Mean);
            }
        }

        /// <summary>
        /// Set IwM to the value that solves the equation, assuming all other values are correct
        /// </summary>
        public bool CalculateIwM(bool silent = true)
        {
            if (nudLw.Value != 0 && nudIw.Value != 0)
            {
                var iwM = CalculateMultipliers.IwM((double)nudStatValue.Value * (_percent ? 0.01 : 1), (double)nudB.Value * AtlasBaseMultiplier, (int)nudLw.Value, (double)nudIw.Value,
                    (double)nudIwM.Value, _spIw, (double)nudTBHM.Value, (double)nudTa.Value, (double)nudTaM.Value, _spTa,
                    (double)nudTm.Value, (double)nudTmM.Value, _spTm, _tamed, _bred, _NoIB, _TE, (int)nudLd.Value, (double)nudId.Value, (double)nudIdM.Value * AtlasIdMultiplier,
                    _spId, _IB, _IBM, _sIBM) ?? 0;
                nudIwM.ValueSaveDouble = Math.Round(iwM, 5);
                return true;
            }
            if (!silent) MessageBox.Show("Divide by Zero-error, e.g. Lw or Iw needs to be at least 1.");
            return false;
        }

        public bool CalculateIdM(bool silent = true)
        {
            var idM = CalculateMultipliers.IdM((double)nudStatValue.Value * (_percent ? 0.01 : 1), Vd, (int)nudLd.Value, (double)nudId.Value * AtlasIdMultiplier, _spId);
            if (idM != null)
            {
                nudIdM.ValueSaveDouble = Math.Round(idM.Value, 5);
                return true;
            }

            if (!silent) MessageBox.Show("Divide by Zero-error, e.g. Ld needs to be at least 1.");
            return false;
        }

        public bool CalculateTaM(bool silent = true)
        {
            var taM = CalculateMultipliers.TaM((double)nudStatValue.Value * (_percent ? 0.01 : 1), (double)nudB.Value * AtlasBaseMultiplier, (int)nudLw.Value, (double)nudIw.Value,
                (double)nudIwM.Value, _spIw, (double)nudTBHM.Value, (double)nudTa.Value, (double)nudTaM.Value, _spTa,
                (double)nudTm.Value, (double)nudTmM.Value, _spTm, _tamed, _bred, _NoIB, _TE, (int)nudLd.Value, (double)nudId.Value, (double)nudIdM.Value * AtlasIdMultiplier,
                _spId, _IB, _IBM, _sIBM);


            if (taM.HasValue)
            {
                nudTaM.ValueSaveDouble = Math.Round(taM.Value, 5);
                return true;
            }
            if (!silent) MessageBox.Show("Divide by Zero-error, e.g. Ta needs to be > 0.");
            return false;
        }

        public bool CalculateTmM(bool silent = true)
        {
            if ((_bred || _TE > 0) && nudTm.Value > 0)
            {
                var tmM = CalculateMultipliers.TmM((double)nudStatValue.Value * (_percent ? 0.01 : 1), (double)nudB.Value * AtlasBaseMultiplier, (int)nudLw.Value, (double)nudIw.Value,
                    (double)nudIwM.Value, _spIw, (double)nudTBHM.Value, (double)nudTa.Value, (double)nudTaM.Value, _spTa,
                    (double)nudTm.Value, (double)nudTmM.Value, _spTm, _tamed, _bred, _NoIB, _TE, (int)nudLd.Value, (double)nudId.Value, (double)nudIdM.Value * AtlasIdMultiplier,
                    _spId, _IB, _IBM, _sIBM) ?? 0;
                nudTmM.ValueSaveDouble = Math.Round(tmM, 5);
                return true;
            }
            if (!silent) MessageBox.Show("Divide by Zero-error, e.g. Tm and TE needs to be > 0.");
            return false;
        }

        private void CalculateTE()
        {
            // set TE to the value that solves the equation, assuming all other values are correct
            if (nudTm.Value > 0 && (_tamed || _bred))
            {
                MinMaxDouble statValue = new MinMaxDouble((double)nudStatValue.Value - 0.05, (double)nudStatValue.Value + 0.05);
                statValue.Min *= _percent ? 0.01 : 1;
                statValue.Max *= _percent ? 0.01 : 1;
                OnTECalculated?.Invoke(new MinMaxDouble(
                        (statValue.Min * Vd / (V * ((double)nudB.Value * AtlasBaseMultiplier * (1 + (double)nudLw.Value * (double)nudIw.Value * _spIw * (double)nudIwM.Value) * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1) + (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1))) - 1) / ((double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * _spTm : 1)),
                        (statValue.Max * Vd / (V * ((double)nudB.Value * AtlasBaseMultiplier * (1 + (double)nudLw.Value * (double)nudIw.Value * _spIw * (double)nudIwM.Value) * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1) + (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1))) - 1) / ((double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * _spTm : 1))
                ));
            }
            else MessageBox.Show("Divide by Zero-error, e.g. Tm needs to be > 0, the stat has to be affected by TE and the creature has to be tamed or bred.");
        }

        private void CalculateIB()
        {
            // set TE to the value that solves the equation, assuming all other values are correct
            if (_bred && !_NoIB && _IBM > 0)
            {
                MinMaxDouble statValue = new MinMaxDouble((double)nudStatValue.Value - 0.05, (double)nudStatValue.Value + 0.05);
                statValue.Min *= _percent ? 0.01 : 1;
                statValue.Max *= _percent ? 0.01 : 1;
                OnIBCalculated?.Invoke(new MinMaxDouble(
                        ((statValue.Min * Vd / (V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * _spTm : 1))) - (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1)) / ((double)nudB.Value * AtlasBaseMultiplier * (1 + (double)nudLw.Value * (double)nudIw.Value * _spIw * (double)nudIwM.Value) * (double)nudTBHM.Value) - 1) * 5 / _IBM,
                        ((statValue.Max * Vd / (V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * _spTm : 1))) - (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1)) / ((double)nudB.Value * AtlasBaseMultiplier * (1 + (double)nudLw.Value * (double)nudIw.Value * _spIw * (double)nudIwM.Value) * (double)nudTBHM.Value) - 1) * 5 / _IBM
                ));
            }
            else MessageBox.Show("Divide by Zero-error, e.g. IBM needs to be > 0, creature has to be bred and stat has to be affected by IB.");
        }

        private void CalculateIBM()
        {
            // set TE to the value that solves the equation, assuming all other values are correct
            if (_bred && !_NoIB && _IB > 0)
            {
                MinMaxDouble statValue = new MinMaxDouble((double)nudStatValue.Value - 0.05, (double)nudStatValue.Value + 0.05);
                statValue.Min *= _percent ? 0.01 : 1;
                statValue.Max *= _percent ? 0.01 : 1;
                OnIBMCalculated?.Invoke(new MinMaxDouble(
                        ((statValue.Min * Vd / (V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * _spTm : 1))) - (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1)) / ((double)nudB.Value * AtlasBaseMultiplier * (1 + (double)nudLw.Value * (double)nudIw.Value * _spIw * (double)nudIwM.Value) * (double)nudTBHM.Value) - 1) * 5 / _IB,
                        ((statValue.Max * Vd / (V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * _spTm : 1))) - (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1)) / ((double)nudB.Value * AtlasBaseMultiplier * (1 + (double)nudLw.Value * (double)nudIw.Value * _spIw * (double)nudIwM.Value) * (double)nudTBHM.Value) - 1) * 5 / _IB
                ));
            }
            else MessageBox.Show("Divide by Zero-error, e.g. IB needs to be > 0, creature has to be bred and stat has to be affected by IB.");
        }

        private void calculateIwMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CalculateIwM(false);
        }

        private void calculateIdMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CalculateIdM(false);
        }

        private void calculateTaMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CalculateTaM(false);
        }

        private void calculateTmMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CalculateTmM(false);
        }

        private void calculateTEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CalculateTE();
        }

        private void calculateIBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CalculateIB();
        }

        private void calculateIBMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CalculateIBM();
        }

        private void resetIwMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetMultiplier(3, Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control));
        }

        private void resetTaMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetMultiplier(0, Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control));
        }

        private void resetTmMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetMultiplier(1, Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control));
        }

        private void resetIdMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetMultiplier(2, Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control));
        }

        private void resetAllMultiplierOfThisStatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetMultiplier(-1, Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control));
        }

        /// <summary>
        /// Reset multiplier value to the current value in the settings, if gameDefault is true, the game default values are used.
        /// </summary>
        private void ResetMultiplier(int index, bool gameDefault = false)
        {
            var multipliersToUse = gameDefault ? StatMultipliersGameDefault : _multipliersOfSettings;
            switch (index)
            {
                case 0:
                    nudTaM.Value = (decimal)multipliersToUse[0];
                    return;
                case 1:
                    nudTmM.Value = (decimal)multipliersToUse[1];
                    return;
                case 2:
                    nudIdM.Value = (decimal)multipliersToUse[2];
                    return;
                case 3:
                    nudIwM.Value = (decimal)multipliersToUse[3];
                    return;
                case -1:
                    // set all
                    nudTaM.Value = (decimal)multipliersToUse[0];
                    nudTmM.Value = (decimal)multipliersToUse[1];
                    nudIdM.Value = (decimal)multipliersToUse[2];
                    nudIwM.Value = (decimal)multipliersToUse[3];
                    return;
            }
        }

        private void setWildLevelToClosestValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetClosestWildLevel();
        }

        private void setDomLevelToClosestValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetClosestDomLevel();
        }

        /// <summary>
        /// Sets the wild level to the value so the final calculation result is closest to the stat value.
        /// </summary>
        public void SetClosestWildLevel()
        {
            double denominator = (double)nudIw.Value * (double)nudIwM.Value;
            if (denominator == 0) return;
            nudLw.ValueSave = (decimal)Math.Round((((double)nudStatValue.Value / ((_percent ? 100 : 1) * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTmM.Value > 0 ? (double)nudTmM.Value * _spTm : 1)) * (1 + (double)nudLd.Value * (double)nudId.Value * _spId * AtlasIdMultiplier * (double)nudIdM.Value)) - ((double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1))) / ((double)nudB.Value * AtlasBaseMultiplier * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1)) - 1) / denominator);
            UpdateCalculations(true);
        }

        /// <summary>
        /// Sets the domesticated level to the value so the final calculation result is closest to the stat value.
        /// </summary>
        public void SetClosestDomLevel()
        {
            double denominator = (double)nudId.Value * _spId * AtlasIdMultiplier * (double)nudIdM.Value;
            if (denominator == 0) return;
            nudLd.ValueSave = (decimal)Math.Round(((double)nudStatValue.Value / ((_percent ? 100 : 1) * Vd) - 1) / denominator);
            UpdateCalculations(true);
        }

        private void SetControlsInUse(ControlsInUse preset)
        {
            switch (preset)
            {
                case ControlsInUse.wild:
                    nudB.BackColor = Color.FromArgb(238, 255, 155);
                    SetBackColorDependingOnNeutral(nudLw, Color.FromArgb(0, 170, 28),
                            nudIw, Color.FromArgb(219, 253, 201),
                            nudIwM, Color.FromArgb(167, 246, 125));
                    nudTBHM.BackColor = SystemColors.Window;
                    nudTa.BackColor = SystemColors.Window;
                    nudTaM.BackColor = SystemColors.Window;
                    nudTm.BackColor = SystemColors.Window;
                    nudTmM.BackColor = SystemColors.Window;
                    nudLd.BackColor = SystemColors.Window;
                    nudId.BackColor = SystemColors.Window;
                    nudIdM.BackColor = SystemColors.Window;
                    break;
                case ControlsInUse.tamed:
                case ControlsInUse.bred:
                    nudB.BackColor = Color.FromArgb(238, 255, 155);
                    SetBackColorDependingOnNeutral(nudLw, Color.FromArgb(0, 170, 28),
                            nudIw, Color.FromArgb(219, 253, 201),
                            nudIwM, Color.FromArgb(167, 246, 125));
                    SetBackColorDependingOnNeutral(nudTBHM, Color.FromArgb(255, 241, 164), 1);
                    SetBackColorDependingOnNeutral(nudTa, Color.FromArgb(255, 233, 203),
                            nudTaM, Color.FromArgb(255, 202, 129));
                    SetBackColorDependingOnNeutral(nudTm, Color.FromArgb(202, 227, 249),
                            nudTmM, Color.FromArgb(124, 181, 229));
                    SetBackColorDependingOnNeutral(nudLd, Color.FromArgb(242, 41, 81),
                            nudId, Color.FromArgb(254, 202, 213),
                            nudIdM, Color.FromArgb(250, 127, 153));
                    break;
            }
        }

        private void SetBackColorDependingOnNeutral(Nud nud, Color color, decimal neutralNumber = 0)
        {
            nud.BackColor = nud.Value != neutralNumber ? color : SystemColors.Window;
        }

        private void SetBackColorDependingOnNeutral(Nud nud1, Color color1, Nud nud2, Color color2, decimal neutralNumber = 0)
        {
            if (nud1.Value != neutralNumber && nud2.Value != neutralNumber)
            {
                nud1.BackColor = color1;
                nud2.BackColor = color2;
            }
            else
            {
                nud1.BackColor = SystemColors.Window;
                nud2.BackColor = SystemColors.Window;
            }
        }

        private void SetBackColorDependingOnNeutral(Nud nud1, Color color1, Nud nud2, Color color2, Nud nud3, Color color3, decimal neutralNumber = 0)
        {
            if (nud1.Value != neutralNumber && nud2.Value != neutralNumber && nud3.Value != neutralNumber)
            {
                nud1.BackColor = color1;
                nud2.BackColor = color2;
                nud3.BackColor = color3;
            }
            else
            {
                nud1.BackColor = SystemColors.Window;
                nud2.BackColor = SystemColors.Window;
                nud3.BackColor = SystemColors.Window;
            }
        }

        /// <summary>
        /// If the value of the Nud is not the default value, highlight the reset button.
        /// </summary>
        private void SetResetButtonColor(Nud nud, double defaultValue, Button resetButton)
        {
            resetButton.BackColor = (double)nud.Value == defaultValue ? SystemColors.Control : Color.LightSalmon;
        }

        private void nudB_ValueChanged(object sender, EventArgs e)
        {
            UpdateCalculations();
        }

        private void nudLw_ValueChanged(object sender, EventArgs e)
        {
            OnLevelChanged?.Invoke();
            UpdateCalculations();
            SetBackColorDependingOnNeutral(nudLw, Color.FromArgb(0, 170, 28),
                    nudIw, Color.FromArgb(219, 253, 201),
                    nudIwM, Color.FromArgb(167, 246, 125));
        }

        private void nudIw_ValueChanged(object sender, EventArgs e)
        {
            UpdateCalculations();
            SetBackColorDependingOnNeutral(nudLw, Color.FromArgb(0, 170, 28),
                    nudIw, Color.FromArgb(219, 253, 201),
                    nudIwM, Color.FromArgb(167, 246, 125));
            SetResetButtonColor(nudIwM, _multipliersOfSettings[3], btResetIwM);
        }

        private void nudTBHM_ValueChanged(object sender, EventArgs e)
        {
            UpdateCalculations();
            if (_tamed || _bred) SetBackColorDependingOnNeutral(nudTBHM, Color.FromArgb(255, 241, 164), 1);
        }

        private void nudTa_ValueChanged(object sender, EventArgs e)
        {
            UpdateCalculations();
            if (_tamed || _bred)
                SetBackColorDependingOnNeutral(nudTa, Color.FromArgb(255, 233, 203),
                        nudTaM, Color.FromArgb(255, 202, 129));
            SetResetButtonColor(nudTaM, _multipliersOfSettings[0], btResetTaM);
        }

        private void nudTm_ValueChanged(object sender, EventArgs e)
        {
            UpdateCalculations();
            if (_tamed || _bred)
                SetBackColorDependingOnNeutral(nudTm, Color.FromArgb(202, 227, 249),
                        nudTmM, Color.FromArgb(124, 181, 229));
            SetResetButtonColor(nudTmM, _multipliersOfSettings[1], btResetTmM);
        }

        private void nudLd_ValueChanged(object sender, EventArgs e)
        {
            OnLevelChanged?.Invoke();
            UpdateCalculations();
            SetBackColorDependingOnNeutral(nudLd, Color.FromArgb(242, 41, 81),
                    nudId, Color.FromArgb(254, 202, 213),
                    nudIdM, Color.FromArgb(250, 127, 153));
        }

        private void nudId_ValueChanged(object sender, EventArgs e)
        {
            UpdateCalculations();
            SetBackColorDependingOnNeutral(nudLd, Color.FromArgb(242, 41, 81),
                    nudId, Color.FromArgb(254, 202, 213),
                    nudIdM, Color.FromArgb(250, 127, 153));
            SetResetButtonColor(nudIdM, _multipliersOfSettings[2], btResetIdM);
        }

        private enum ControlsInUse
        {
            wild,
            tamed,
            bred
        }

        private void btCalculateWildLevel_Click(object sender, EventArgs e)
        {
            SetClosestWildLevel();
        }

        private void btCalculateIwM_Click(object sender, EventArgs e)
        {
            CalculateIwM(false);
        }

        private void btResetIwM_Click(object sender, EventArgs e)
        {
            ResetMultiplier(3, Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control));
        }

        private void btCalculateTaM_Click(object sender, EventArgs e)
        {
            CalculateTaM(false);
        }

        private void btResetTaM_Click(object sender, EventArgs e)
        {
            ResetMultiplier(0, Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control));
        }

        private void btCalculateTmM_Click(object sender, EventArgs e)
        {
            CalculateTmM(false);
        }

        private void btResetTmM_Click(object sender, EventArgs e)
        {
            ResetMultiplier(1, Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control));
        }

        private void btCalculateDomLevel_Click(object sender, EventArgs e)
        {
            SetClosestDomLevel();
        }

        private void btCalculateIdM_Click(object sender, EventArgs e)
        {
            CalculateIdM(false);
        }

        private void btResetIdM_Click(object sender, EventArgs e)
        {
            ResetMultiplier(2, Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control));
        }

        private void btCalculateTE_Click(object sender, EventArgs e)
        {
            CalculateTE();
        }

        private void btCalculateIB_Click(object sender, EventArgs e)
        {
            CalculateIB();
        }

        private void btCalculateIBM_Click(object sender, EventArgs e)
        {
            CalculateIBM();
        }

        // A Troodonism checkbox changed, set according values and recalculate.
        private void CbTrodB_CheckedChanged(object sender, EventArgs e)
        {
            nudB.Value = (decimal)((((CheckBox)sender).Checked ? _altStatFactor : 1) * _statValues[0]);
            UpdateCalculations(true);
        }

        private void CbTrodIw_CheckedChanged(object sender, EventArgs e)
        {
            nudIw.Value = (decimal)((((CheckBox)sender).Checked ? _altStatFactor : 1) * _statValues[1]);
            UpdateCalculations(true);
        }

        private void CbTrodId_CheckedChanged(object sender, EventArgs e)
        {
            nudId.Value = (decimal)((((CheckBox)sender).Checked ? _altStatFactor : 1) * _statValues[2]);
            UpdateCalculations(true);
        }

        private void CbTrodTa_CheckedChanged(object sender, EventArgs e)
        {
            nudTa.Value = (decimal)((((CheckBox)sender).Checked ? _altStatFactor : 1) * _statValues[3]);
            UpdateCalculations(true);
        }

        private void CbTrodTm_CheckedChanged(object sender, EventArgs e)
        {
            nudTm.Value = (decimal)((((CheckBox)sender).Checked ? _altStatFactor : 1) * _statValues[4]);
            UpdateCalculations(true);
        }

        /// <summary>
        /// Calculations are not performed until EndUpdate() is called or a forceUpdate is performed.
        /// </summary>
        public void BeginUpdate()
        {
            updateValues = false;
        }

        /// <summary>
        /// Updates are possible by value changes again.
        /// </summary>
        public void EndUpdate(bool doUpdate = false)
        {
            updateValues = true;
            if (doUpdate)
                UpdateCalculations(true);
        }
    }
}
