using ARKBreedingStats.miscClasses;
using ARKBreedingStats.uiControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

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
        /// Value of wild and taming boni, without dom levels
        /// </summary>
        private double Vd;
        /// <summary>
        /// Final value with all levels and boni
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
        private double[] _multipliersOfSettings;

        /// <summary>
        /// The values of this stat. 0: Base, 1: Iw, 2: Id, 3: Ta, 4: Tm
        /// </summary>
        private double[] _statValues;
        /// <summary>
        /// The factor the correct value is multiplied with to get the alt / Troodonism value.
        /// </summary>
        private double _altStatFactor;

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
            double Vw = (double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * _spIw * (double)nudIwM.Value);
            string VwDisplay = Math.Round(Vw * (_percent ? 100 : 1), DecimalPlaces) + (_percent ? "%" : string.Empty);
            tbVw.Text = $"{nudB.Value} * ( 1 + {nudLw.Value} * {nudIw.Value}{(_spIw != 1 ? " * " + _spIw : string.Empty)} * {nudIwM.Value} ) = {VwDisplay}";
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
                V = Vd * (1 + (double)nudLd.Value * (double)nudId.Value * _spId * (double)nudIdM.Value);
                string VDisplay = Math.Round(V * (_percent ? 100 : 1), DecimalPlaces) + (_percent ? "%" : string.Empty);
                tbV.Text = $"{VdDisplay} * ( 1 + {nudLd.Value} * {nudId.Value}{(_spId != 1 ? " * " + _spId : string.Empty)} * {nudIdM.Value} ) = {VDisplay}";
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
                if (value != null && value.Length == 4)
                {
                    _multipliersOfSettings = value;
                    var updateValuesKeeper = updateValues;
                    updateValues = false;
                    // 0:tamingadd, 1:tamingmult, 2:levelupdom, 3:levelupwild
                    nudTaM.Value = (decimal)value[0];
                    nudTmM.Value = (decimal)value[1];
                    nudIdM.Value = (decimal)value[2];
                    nudIwM.Value = (decimal)value[3];
                    UpdateCalculations(updateValuesKeeper);
                }
            }
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
        public float? TBHM
        {
            set => nudTBHM.Value = value != null ? (decimal)value : 1;
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
            this._spIw = spIw ?? 1;
            this._spId = spId ?? 1;
            this._spTa = spTa ?? 1;
            this._spTm = spTm ?? 1;
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

        // calculate values according to the stat-formula
        // (double)nudStatValue.Value = ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * (double)nudIwM.Value) * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1) + ((double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value : 1))) * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTmM.Value > 0 ? (double)nudTmM.Value*spTm : 1)) * (1 + (double)nudLd.Value * (double)nudId.Value * spId * (double)nudIdM.Value);

        public bool CalculateIwM(bool silent = true)
        {
            // set IwM to the value that solves the equation, assuming all other values are correct
            if (nudLw.Value != 0 && nudIw.Value != 0)
            {
                decimal IwM = (decimal)((((double)nudStatValue.Value * (_percent ? 0.01 : 1) / (_tamed || _bred ? (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * _spTm : 1)) * (1 + (double)nudLd.Value * (double)nudId.Value * _spId * (double)nudIdM.Value) : 1) - (_tamed || _bred ? (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1) : 0)) / ((double)nudB.Value * (_tamed || _bred ? (double)nudTBHM.Value : 1) * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1)) - 1) / ((double)nudLw.Value * (double)nudIw.Value * _spIw));
                nudIwM.ValueSave = Math.Round(IwM, 5);
                return true;
            }
            if (!silent) MessageBox.Show("Divide by Zero-error, e.g. Lw or Iw needs to be at least 1.");
            return false;
        }

        public bool CalculateIdM(bool silent = true)
        {
            // set IdM to the value that solves the equation, assuming all other values are correct
            if (Vd != 0 && nudLd.Value != 0 && nudId.Value != 0)
            {
                decimal IdM = (nudStatValue.Value / (decimal)(Vd * (_percent ? 100 : 1)) - 1) / (nudLd.Value * nudId.Value * (decimal)_spId);
                nudIdM.ValueSave = Math.Round(IdM, 5);
                return true;
            }
            if (!silent) MessageBox.Show("Divide by Zero-error, e.g. Ld needs to be at least 1.");
            return false;
        }

        public bool CalculateTaM(bool silent = true)
        {
            // set TaM to the value that solves the equation, assuming all other values are correct
            if (nudTa.Value > 0)
            {
                decimal TaM = (decimal)(((double)nudStatValue.Value * Vd / ((_percent ? 100 : 1) * V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * _spTm : 1))) - (double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * _spIw * (double)nudIwM.Value) * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1)) / ((double)nudTa.Value * _spTa));
                nudTaM.ValueSave = Math.Round(TaM, 5);
                return true;
            }
            if (!silent) MessageBox.Show("Divide by Zero-error, e.g. Ta needs to be > 0.");
            return false;
        }

        public bool CalculateTmM(bool silent = true)
        {
            // set TmM to the value that solves the equation, assuming all other values are correct
            if ((_bred || _TE > 0) && nudTm.Value > 0)
            {
                // TODO formula wrong?
                decimal TmM = (decimal)(((double)nudStatValue.Value * Vd / ((_percent ? 100 : 1) * V * ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * _spIw * (double)nudIwM.Value) * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1) + (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1))) - 1) / ((_bred ? 1 : _TE) * (double)nudTm.Value * _spTm));
                nudTmM.ValueSave = Math.Round(TmM, 5);
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
                        (statValue.Min * Vd / (V * ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * _spIw * (double)nudIwM.Value) * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1) + (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1))) - 1) / ((double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * _spTm : 1)),
                        (statValue.Max * Vd / (V * ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * _spIw * (double)nudIwM.Value) * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1) + (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1))) - 1) / ((double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * _spTm : 1))
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
                        ((statValue.Min * Vd / (V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * _spTm : 1))) - (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1)) / ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * _spIw * (double)nudIwM.Value) * (double)nudTBHM.Value) - 1) * 5 / _IBM,
                        ((statValue.Max * Vd / (V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * _spTm : 1))) - (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1)) / ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * _spIw * (double)nudIwM.Value) * (double)nudTBHM.Value) - 1) * 5 / _IBM
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
                        ((statValue.Min * Vd / (V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * _spTm : 1))) - (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1)) / ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * _spIw * (double)nudIwM.Value) * (double)nudTBHM.Value) - 1) * 5 / _IB,
                        ((statValue.Max * Vd / (V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * _spTm : 1))) - (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1)) / ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * _spIw * (double)nudIwM.Value) * (double)nudTBHM.Value) - 1) * 5 / _IB
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
            nudIwM.Value = (decimal)_multipliersOfSettings[3];
        }

        private void resetTaMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nudTaM.Value = (decimal)_multipliersOfSettings[0];
        }

        private void resetTmMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nudTmM.Value = (decimal)_multipliersOfSettings[1];
        }

        private void resetIdMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nudIdM.Value = (decimal)_multipliersOfSettings[2];
        }

        private void resetAllMultiplierOfThisStatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nudTaM.Value = (decimal)_multipliersOfSettings[0];
            nudTmM.Value = (decimal)_multipliersOfSettings[1];
            nudIdM.Value = (decimal)_multipliersOfSettings[2];
            nudIwM.Value = (decimal)_multipliersOfSettings[3];
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
            nudLw.ValueSave = (decimal)Math.Round((((double)nudStatValue.Value / ((_percent ? 100 : 1) * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTmM.Value > 0 ? (double)nudTmM.Value * _spTm : 1)) * (1 + (double)nudLd.Value * (double)nudId.Value * _spId * (double)nudIdM.Value)) - ((double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * _spTa : 1))) / ((double)nudB.Value * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1)) - 1) / denominator);
            UpdateCalculations(true);
        }

        /// <summary>
        /// Sets the domesticated level to the value so the final calculation result is closest to the stat value.
        /// </summary>
        public void SetClosestDomLevel()
        {
            double denominator = (double)nudId.Value * _spId * (double)nudIdM.Value;
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
        /// <param name="nud"></param>
        /// <param name="defaultValue"></param>
        /// <param name="resetIwM"></param>
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
            nudIwM.Value = (decimal)_multipliersOfSettings[3];
        }

        private void btCalculateTaM_Click(object sender, EventArgs e)
        {
            CalculateTaM(false);
        }

        private void btResetTaM_Click(object sender, EventArgs e)
        {
            nudTaM.Value = (decimal)_multipliersOfSettings[0];
        }

        private void btCalculateTmM_Click(object sender, EventArgs e)
        {
            CalculateTmM(false);
        }

        private void btResetTmM_Click(object sender, EventArgs e)
        {
            nudTmM.Value = (decimal)_multipliersOfSettings[1];
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
            nudIdM.Value = (decimal)_multipliersOfSettings[2];
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
