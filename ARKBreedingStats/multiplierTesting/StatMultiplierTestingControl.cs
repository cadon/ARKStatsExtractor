using ARKBreedingStats.miscClasses;
using ARKBreedingStats.uiControls;
using System;
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
        private double spIw;
        /// <summary>
        /// Singleplayer extra multiplier for increase per domesticated level.
        /// </summary>
        private double spId;
        /// <summary>
        /// Singleplayer extra multiplier for taming addition.
        /// </summary>
        private double spTa;
        /// <summary>
        /// Singleplayer extra multiplier for taming multiplier.
        /// </summary>
        private double spTm;
        private double[] multipliersOfSettings;

        public StatMultiplierTestingControl()
        {
            InitializeComponent();
            updateValues = true;
            Percent = false;
            _NoIB = false;
            _IBM = 1;
            nudTBHM.NeutralNumber = 1;
            SetSinglePlayerSettings();
        }

        private void UpdateCalculations(bool forceUpdate = false)
        {
            updateValues = updateValues || forceUpdate;
            if (!updateValues) return;

            // ValueWild
            double Vw = (double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * spIw * (double)nudIwM.Value);
            string VwDisplay = Math.Round(Vw * (_percent ? 100 : 1), 4) + (_percent ? "%" : string.Empty);
            tbVw.Text = $"{nudB.Value} * ( 1 + {nudLw.Value} * {nudIw.Value}{(spIw != 1 ? " * " + spIw : string.Empty)} * {nudIwM.Value} ) = {VwDisplay}";
            if (_tamed || _bred)
            {
                // ValueDom
                Vd = (Vw * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1) + (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * spTa : 1))
                        * (1 + (nudTm.Value > 0 ? (_bred ? 1 : _TE) * (double)nudTm.Value * (double)nudTmM.Value * spTm : (double)nudTm.Value));
                string VdDisplay = Math.Round(Vd * (_percent ? 100 : 1), 4) + (_percent ? "%" : string.Empty);
                tbVd.Text = "( " + VwDisplay + (nudTBHM.Value != 1 ? " * " + nudTBHM.Value : string.Empty) + (!_NoIB && _bred ? " * ( 1 + " + _IB + " * " + _IBM + $" * {_sIBM} )" : string.Empty)
                        + " + " + nudTa.Value + (nudTa.Value > 0 ? " * " + nudTaM.Value + (spTa != 1 ? " * " + spTa : string.Empty) : string.Empty) + " ) "
                        + " * ( 1 + " + (nudTm.Value > 0 ? (_bred ? 1 : _TE) + " * " + nudTm.Value + " * " + nudTmM.Value + (spTm != 1 ? " * " + spTm : string.Empty) : nudTm.Value.ToString()) + " )"
                        + " = " + VdDisplay;
                // Value
                V = Vd * (1 + (double)nudLd.Value * (double)nudId.Value * spId * (double)nudIdM.Value);
                string VDisplay = Math.Round(V * (_percent ? 100 : 1), 4) + (_percent ? "%" : string.Empty);
                tbV.Text = $"{VdDisplay} * ( 1 + {nudLd.Value} * {nudId.Value}{(spId != 1 ? " * " + spId : string.Empty)} * {nudIdM.Value} ) = {VDisplay}";
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
                    multipliersOfSettings = value;
                    updateValues = false;
                    // 0:tamingadd, 1:tamingmult, 2:levelupdom, 3:levelupwild
                    nudTaM.Value = (decimal)value[0];
                    nudTmM.Value = (decimal)value[1];
                    nudIdM.Value = (decimal)value[2];
                    nudIwM.Value = (decimal)value[3];
                    UpdateCalculations(true);
                }
            }
        }

        public void SetStatValues(double[] statValues, double?[] customOverrides)
        {
            if (statValues != null && statValues.Length == 5)
            {
                updateValues = false;
                nudB.Value = (decimal)(customOverrides?[0] ?? statValues[0]);
                nudIw.Value = (decimal)(customOverrides?[1] ?? statValues[1]);
                nudId.Value = (decimal)(customOverrides?[2] ?? statValues[2]);
                nudTa.Value = (decimal)(customOverrides?[3] ?? statValues[3]);
                nudTm.Value = (decimal)(customOverrides?[4] ?? statValues[4]);
                UpdateCalculations(true);
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
            this.spIw = spIw ?? 1;
            this.spId = spId ?? 1;
            this.spTa = spTa ?? 1;
            this.spTm = spTm ?? 1;
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
                decimal IwM = (decimal)((((double)nudStatValue.Value * (_percent ? 0.01 : 1) / (_tamed || _bred ? (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * spTm : 1)) * (1 + (double)nudLd.Value * (double)nudId.Value * spId * (double)nudIdM.Value) : 1) - (_tamed || _bred ? (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * spTa : 1) : 0)) / ((double)nudB.Value * (_tamed || _bred ? (double)nudTBHM.Value : 1) * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1)) - 1) / ((double)nudLw.Value * (double)nudIw.Value * spIw));
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
                decimal IdM = (nudStatValue.Value / (decimal)(Vd * (_percent ? 100 : 1)) - 1) / (nudLd.Value * nudId.Value * (decimal)spId);
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
                decimal TaM = (decimal)(((double)nudStatValue.Value * Vd / ((_percent ? 100 : 1) * V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * spTm : 1))) - (double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * spIw * (double)nudIwM.Value) * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1)) / ((double)nudTa.Value * spTa));
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
                decimal TmM = (decimal)(((double)nudStatValue.Value * Vd / ((_percent ? 100 : 1) * V * ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * spIw * (double)nudIwM.Value) * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1) + (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * spTa : 1))) - 1) / ((_bred ? 1 : _TE) * (double)nudTm.Value * spTm));
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
                        (statValue.Min * Vd / (V * ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * spIw * (double)nudIwM.Value) * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1) + (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * spTa : 1))) - 1) / ((double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * spTm : 1)),
                        (statValue.Max * Vd / (V * ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * spIw * (double)nudIwM.Value) * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1) + (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * spTa : 1))) - 1) / ((double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * spTm : 1))
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
                        ((statValue.Min * Vd / (V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * spTm : 1))) - (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * spTa : 1)) / ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * spIw * (double)nudIwM.Value) * (double)nudTBHM.Value) - 1) * 5 / _IBM,
                        ((statValue.Max * Vd / (V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * spTm : 1))) - (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * spTa : 1)) / ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * spIw * (double)nudIwM.Value) * (double)nudTBHM.Value) - 1) * 5 / _IBM
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
                        ((statValue.Min * Vd / (V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * spTm : 1))) - (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * spTa : 1)) / ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * spIw * (double)nudIwM.Value) * (double)nudTBHM.Value) - 1) * 5 / _IB,
                        ((statValue.Max * Vd / (V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value * spTm : 1))) - (double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * spTa : 1)) / ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * spIw * (double)nudIwM.Value) * (double)nudTBHM.Value) - 1) * 5 / _IB
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
            nudIwM.Value = (decimal)multipliersOfSettings[3];
        }

        private void resetTaMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nudTaM.Value = (decimal)multipliersOfSettings[0];
        }

        private void resetTmMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nudTmM.Value = (decimal)multipliersOfSettings[1];
        }

        private void resetIdMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nudIdM.Value = (decimal)multipliersOfSettings[2];
        }

        private void resetAllMultiplierOfThisStatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nudTaM.Value = (decimal)multipliersOfSettings[0];
            nudTmM.Value = (decimal)multipliersOfSettings[1];
            nudIdM.Value = (decimal)multipliersOfSettings[2];
            nudIwM.Value = (decimal)multipliersOfSettings[3];
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
            nudLw.ValueSave = (decimal)Math.Round((((double)nudStatValue.Value / ((_percent ? 100 : 1) * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTmM.Value > 0 ? (double)nudTmM.Value * spTm : 1)) * (1 + (double)nudLd.Value * (double)nudId.Value * spId * (double)nudIdM.Value)) - ((double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value * spTa : 1))) / ((double)nudB.Value * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * _sIBM : 1)) - 1) / denominator);
            UpdateCalculations(true);
        }

        /// <summary>
        /// Sets the domesticated level to the value so the final calculation result is closest to the stat value.
        /// </summary>
        public void SetClosestDomLevel()
        {
            double denominator = (double)nudId.Value * spId * (double)nudIdM.Value;
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
        }

        private void nudTm_ValueChanged(object sender, EventArgs e)
        {
            UpdateCalculations();
            if (_tamed || _bred)
                SetBackColorDependingOnNeutral(nudTm, Color.FromArgb(202, 227, 249),
                        nudTmM, Color.FromArgb(124, 181, 229));
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
            nudIwM.Value = (decimal)multipliersOfSettings[3];
        }

        private void btCalculateTaM_Click(object sender, EventArgs e)
        {
            CalculateTaM(false);
        }

        private void btResetTaM_Click(object sender, EventArgs e)
        {
            nudTaM.Value = (decimal)multipliersOfSettings[0];
        }

        private void btCalculateTmM_Click(object sender, EventArgs e)
        {
            CalculateTmM(false);
        }

        private void btResetTmM_Click(object sender, EventArgs e)
        {
            nudTmM.Value = (decimal)multipliersOfSettings[1];
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
            nudIdM.Value = (decimal)multipliersOfSettings[2];
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
    }
}
