using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ARKBreedingStats.valueClasses;
using ARKBreedingStats.uiControls;

namespace ARKBreedingStats.multiplierTesting
{
    public partial class StatMultiplierTestingControl : UserControl
    {
        public delegate void LevelChangedEventHandler();
        public event LevelChangedEventHandler OnLevelChanged;
        public delegate void SetValueEventHandler(MinMaxDouble value);
        public event SetValueEventHandler OnValueChangedTE;
        public event SetValueEventHandler OnValueChangedIB;
        public event SetValueEventHandler OnValueChangedIBM;

        public bool updateValues;
        private bool _tamed;
        private bool _bred;
        private double _IB;
        private double _IBM;
        private double _TE;
        private bool _percent;
        private double V, Vd;
        private bool _NoIB;

        public StatMultiplierTestingControl()
        {
            InitializeComponent();
            updateValues = true;
            Percent = false;
            _NoIB = false;
            _IBM = 1;
            nudTBHM.NeutralNumber = 1;
        }

        private void updateCalculations(bool forceUpdate = false)
        {
            updateValues = updateValues || forceUpdate;
            if (updateValues)
            {
                // ValueWild
                double Vw = (double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * (double)nudIwM.Value);
                string VwDisplay = Math.Round(Vw * (_percent ? 100 : 1), 3).ToString() + (_percent ? "%" : "");
                tbVw.Text = nudB.Value.ToString() + " * ( 1 + " + nudLw.Value.ToString() + " * " + nudIw.Value.ToString() + " * " + nudIwM.Value.ToString()
                    + " ) = " + VwDisplay;
                if (_tamed || _bred)
                {
                    // ValueDom
                    Vd = (Vw * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * 0.2 : 1) + ((double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value : 1)))
                        * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value : 1));
                    string VdDisplay = Math.Round(Vd * (_percent ? 100 : 1), 3).ToString() + (_percent ? "%" : "");
                    tbVd.Text = "( " + VwDisplay + (nudTBHM.Value != 1 ? " * " + nudTBHM.Value.ToString() : "") + (!_NoIB && _bred ? " * ( 1 + " + _IB + " * " + _IBM + " * 0.2 )" : "")
                        + " + " + nudTa.Value.ToString() + (nudTa.Value > 0 ? " * " + nudTaM.Value.ToString() : "") + " ) "
                        + " * ( 1 + " + (_bred ? 1 : _TE) + " * " + nudTm.Value + (nudTm.Value > 0 ? " * " + nudTmM.Value : "") + " )"
                        + " = " + VdDisplay;
                    // Value
                    V = Vd * (1 + (double)nudLd.Value * (double)nudId.Value * (double)nudIdM.Value);
                    string VDisplay = Math.Round(V * (_percent ? 100 : 1), 3).ToString() + (_percent ? "%" : "");
                    tbV.Text = VdDisplay + " * ( 1 + " + nudLd.Value + " * " + nudId.Value + " * " + nudIdM.Value + " )"
                        + " = " + VDisplay;
                }
                else
                {
                    Vd = Vw;
                    V = Vw;
                    tbVd.Text = "";
                    tbV.Text = VwDisplay;
                }

                updateMatchingColor();
            }
        }

        public string StatName
        {
            set { lStatName.Text = value; }
        }

        public double[] StatMultipliers
        {
            set
            {
                if (value != null && value.Length == 4)
                {
                    updateValues = false;
                    // 0:tamingadd, 1:tamingmult, 2:levelupdom, 3:levelupwild
                    nudTaM.Value = (decimal)value[0];
                    nudTmM.Value = (decimal)value[1];
                    nudIdM.Value = (decimal)value[2];
                    nudIwM.Value = (decimal)value[3];
                    updateCalculations(true);
                }
            }
            get
            {
                return new double[] { (double)nudTaM.Value, (double)nudTmM.Value, (double)nudIdM.Value, (double)nudIwM.Value };
            }
        }

        public void setStatValues(double?[] statValues)
        {
            if (statValues != null && statValues.Length == 5)
            {
                updateValues = false;
                nudB.Value = statValues[0] == null ? 0 : ((decimal)statValues[0]);
                nudIw.Value = statValues[1] == null ? 0 : (decimal)statValues[1];
                nudId.Value = statValues[2] == null ? 0 : (decimal)statValues[2];
                nudTa.Value = statValues[3] == null ? 0 : (decimal)statValues[3];
                nudTm.Value = statValues[4] == null ? 0 : (decimal)statValues[4];
                updateCalculations(true);
            }
        }

        public double statValue
        {
            set
            {
                decimal v = (decimal)value * (_percent ? 100 : 1);
                nudStatValue.Value = v > nudStatValue.Maximum ? nudStatValue.Maximum : v;
            }
        }
        public int levelWild { set { nudLw.Value = value > 0 ? value : 0; } get { return (int)nudLw.Value; } }
        public int levelDom { set { nudLd.Value = value > 0 ? value : 0; } get { return (int)nudLd.Value; } }

        public bool Wild
        {
            set
            {
                _tamed = false;
                _bred = false;
                updateCalculations();
                setControlsInUse(ControlsInUse.wild);
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
                    updateCalculations();
                    setControlsInUse(ControlsInUse.tamed);
                }
            }
        }
        public bool Bred
        {
            set
            {
                if (_bred != value)
                {
                    _bred = value; updateCalculations();
                    setControlsInUse(ControlsInUse.bred);
                }
            }
        }

        public double IB { set { if (_IB != value) { _IB = value; updateCalculations(); } } }
        public double IBM { set { if (_IBM != value) { _IBM = value; updateCalculations(); } } }
        public double TE { set { if (_TE != value) { _TE = value; updateCalculations(); } } }
        public bool Percent { set { _percent = value; lPercent.Text = _percent ? "%" : ""; } }

        public float? TBHM { set { nudTBHM.Value = (value != null ? (decimal)value : 1); } }
        public bool? NoIB { set { _NoIB = value == true; } }

        private void nudStatValue_ValueChanged(object sender, EventArgs e)
        {
            updateMatchingColor();
        }

        private void updateMatchingColor()
        {
            // if matching, color green
            double statValue = (double)nudStatValue.Value;
            double VP = Math.Round(V * (_percent ? 100 : 1), 1, MidpointRounding.AwayFromZero);
            if (VP == statValue)
                nudStatValue.BackColor = Color.LightGreen;
            else
            {
                // if not, color redder if the value is too low, and blue, if the value is too big
                int proximity = (int)Math.Abs(VP - statValue) / 2;
                if (proximity > 50) proximity = 50;
                nudStatValue.BackColor = Utils.getColorFromPercent(50 - proximity, 0.6, VP - statValue < 0);
            }
        }

        // calculate values according to the stat-formula
        // (double)nudStatValue.Value = ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * (double)nudIwM.Value) * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * 0.2 : 1) + ((double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value : 1))) * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTmM.Value > 0 ? (double)nudTmM.Value : 1)) * (1 + (double)nudLd.Value * (double)nudId.Value * (double)nudIdM.Value)

        public bool calculateIwM(bool silent = true)
        {
            // set IwM to the value that solves the equation, assuming all other values are correct
            if (nudLw.Value != 0 && nudIw.Value != 0)
            {
                var IwM = (decimal)((((double)nudStatValue.Value * (_percent ? 0.01 : 1) / ((1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value : 1)) * (1 + (double)nudLd.Value * (double)nudId.Value * (double)nudIdM.Value)) - ((double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value : 1))) / ((double)nudB.Value * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * 0.2 : 1)) - 1) / ((double)nudLw.Value * (double)nudIw.Value));
                nudIwM.Value = IwM < 0 ? 0 : (IwM > nudIwM.Maximum ? nudIwM.Maximum : Math.Round(IwM, 5));
                return true;
            }
            else if (!silent) MessageBox.Show("Divide by Zero-error, e.g. Lw or Iw needs to be at least 1.");
            return false;
        }

        public bool calculateIdM(bool silent = true)
        {
            // set IdM to the value that solves the equation, assuming all other values are correct
            if (Vd != 0 && nudLd.Value != 0 && nudId.Value != 0)
            {
                var IdM = ((nudStatValue.Value / (decimal)(Vd * (_percent ? 100 : 1)) - 1) / (nudLd.Value * nudId.Value));
                nudIdM.Value = IdM < 0 ? 0 : (IdM > nudIdM.Maximum ? nudIdM.Maximum : Math.Round(IdM, 5));
                return true;
            }
            else if (!silent) MessageBox.Show("Divide by Zero-error, e.g. Ld needs to be at least 1.");
            return false;
        }

        public bool calculateTaM(bool silent = true)
        {
            // set TaM to the value that solves the equation, assuming all other values are correct
            if (nudTa.Value > 0)
            {
                var TaM = (decimal)(((double)nudStatValue.Value * Vd / ((_percent ? 100 : 1) * V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value : 1))) - (double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * (double)nudIwM.Value) * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * 0.2 : 1)) / (double)nudTa.Value);
                nudTaM.Value = TaM < nudTaM.Minimum ? nudTaM.Minimum : (TaM > nudTaM.Maximum ? nudTaM.Maximum : Math.Round(TaM, 5));
                return true;
            }
            else if (!silent) MessageBox.Show("Divide by Zero-error, e.g. Ta needs to be > 0.");
            return false;
        }

        public bool calculateTmM(bool silent = true)
        {
            // set TmM to the value that solves the equation, assuming all other values are correct
            if ((_bred || _TE > 0) && nudTm.Value > 0)
            {
                // TODO formula wrong?
                var TmM = (decimal)(((double)nudStatValue.Value * Vd / ((_percent ? 100 : 1) * V * ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * (double)nudIwM.Value) * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * 0.2 : 1) + ((double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value : 1)))) - 1) / ((_bred ? 1 : _TE) * (double)nudTm.Value));
                nudTmM.Value = TmM < nudTmM.Minimum ? nudTmM.Minimum : (TmM > nudTmM.Maximum ? nudTmM.Maximum : Math.Round(TmM, 5));
                return true;
            }
            else if (!silent) MessageBox.Show("Divide by Zero-error, e.g. Tm and TE needs to be > 0.");
            return false;
        }

        private void calculateTE()
        {
            // set TE to the value that solves the equation, assuming all other values are correct
            if (nudTm.Value > 0 && (_tamed || _bred))
            {
                var statValue = new MinMaxDouble((double)nudStatValue.Value - 0.05, (double)nudStatValue.Value + 0.05);
                statValue.Min *= (_percent ? 0.01 : 1);
                statValue.Max *= (_percent ? 0.01 : 1);
                OnValueChangedTE?.Invoke(new MinMaxDouble(
                    (statValue.Min * Vd / (V * ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * (double)nudIwM.Value) * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * 0.2 : 1) + ((double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value : 1)))) - 1) / ((double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value : 1)),
                    (statValue.Max * Vd / (V * ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * (double)nudIwM.Value) * (double)nudTBHM.Value * (!_NoIB && _bred ? 1 + _IB * _IBM * 0.2 : 1) + ((double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value : 1)))) - 1) / ((double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value : 1))
                    ));
            }
            else MessageBox.Show("Divide by Zero-error, e.g. Tm needs to be > 0, the stat has to be affected by TE and the creature has to be tamed or bred.");
        }

        private void calculateIB()
        {
            // set TE to the value that solves the equation, assuming all other values are correct
            if (_bred && !_NoIB && _IBM > 0)
            {
                var statValue = new MinMaxDouble((double)nudStatValue.Value - 0.05, (double)nudStatValue.Value + 0.05);
                statValue.Min *= (_percent ? 0.01 : 1);
                statValue.Max *= (_percent ? 0.01 : 1);
                OnValueChangedIB?.Invoke(new MinMaxDouble(
                    ((statValue.Min * Vd / (V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value : 1))) - ((double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value : 1))) / ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * (double)nudIwM.Value) * (double)nudTBHM.Value) - 1) * 5 / _IBM,
                    ((statValue.Max * Vd / (V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value : 1))) - ((double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value : 1))) / ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * (double)nudIwM.Value) * (double)nudTBHM.Value) - 1) * 5 / _IBM
                    ));
            }
            else MessageBox.Show("Divide by Zero-error, e.g. IBM needs to be > 0, creature has to be bred and stat has to be affected by IB.");
        }

        private void calculateIBM()
        {
            // set TE to the value that solves the equation, assuming all other values are correct
            if (_bred && !_NoIB && _IB > 0)
            {
                var statValue = new MinMaxDouble((double)nudStatValue.Value - 0.05, (double)nudStatValue.Value + 0.05);
                statValue.Min *= (_percent ? 0.01 : 1);
                statValue.Max *= (_percent ? 0.01 : 1);
                OnValueChangedIBM?.Invoke(new MinMaxDouble(
                    ((statValue.Min * Vd / (V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value : 1))) - ((double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value : 1))) / ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * (double)nudIwM.Value) * (double)nudTBHM.Value) - 1) * 5 / _IB,
                    ((statValue.Max * Vd / (V * (1 + (_bred ? 1 : _TE) * (double)nudTm.Value * (nudTm.Value > 0 ? (double)nudTmM.Value : 1))) - ((double)nudTa.Value * (nudTa.Value > 0 ? (double)nudTaM.Value : 1))) / ((double)nudB.Value * (1 + (double)nudLw.Value * (double)nudIw.Value * (double)nudIwM.Value) * (double)nudTBHM.Value) - 1) * 5 / _IB
                    ));
            }
            else MessageBox.Show("Divide by Zero-error, e.g. IB needs to be > 0, creature has to be bred and stat has to be affected by IB.");
        }

        private void calculateIwMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            calculateIwM(false);
        }

        private void calculateIdMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            calculateIdM(false);
        }

        private void calculateTaMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            calculateTaM(false);
        }

        private void calculateTmMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            calculateTmM(false);
        }

        private void calculateTEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            calculateTE();
        }

        private void calculateIBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            calculateIB();
        }

        private void calculateIBMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            calculateIBM();
        }

        private void setControlsInUse(ControlsInUse preset)
        {
            /*
            nudB.BackColor = SystemColors.Window;
            nudLw.BackColor = SystemColors.Window;
            nudIw.BackColor = SystemColors.Window;
            nudIwM.BackColor = SystemColors.Window;
            nudTBHM.BackColor = SystemColors.Window;
            nudTa.BackColor = SystemColors.Window;
            nudTaM.BackColor = SystemColors.Window;
            nudTm.BackColor = SystemColors.Window;
            nudTmM.BackColor = SystemColors.Window;
            nudLd.BackColor = SystemColors.Window;
            nudId.BackColor = SystemColors.Window;
            nudIdM.BackColor = SystemColors.Window;
            nudStatValue.BackColor = SystemColors.Window;
            */
            switch (preset)
            {
                case ControlsInUse.wild:
                    nudB.BackColor = Color.FromArgb(238, 255, 155);
                    setBackColorDependingOnNeutral(nudLw, Color.FromArgb(0, 170, 28),
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
                    setBackColorDependingOnNeutral(nudLw, Color.FromArgb(0, 170, 28),
                        nudIw, Color.FromArgb(219, 253, 201),
                        nudIwM, Color.FromArgb(167, 246, 125));
                    setBackColorDependingOnNeutral(nudTBHM, Color.FromArgb(255, 241, 164), 1);
                    setBackColorDependingOnNeutral(nudTa, Color.FromArgb(255, 233, 203),
                        nudTaM, Color.FromArgb(255, 202, 129));
                    setBackColorDependingOnNeutral(nudTm, Color.FromArgb(202, 227, 249),
                        nudTmM, Color.FromArgb(124, 181, 229));
                    setBackColorDependingOnNeutral(nudLd, Color.FromArgb(242, 41, 81),
                        nudId, Color.FromArgb(254, 202, 213),
                        nudIdM, Color.FromArgb(250, 127, 153));
                    break;
            }
        }

        private void setBackColorDependingOnNeutral(Nud nud, Color color, decimal neutralNumber = 0)
        {
            if (nud.Value != neutralNumber) nud.BackColor = color;
            else nud.BackColor = SystemColors.Window;
        }

        private void setBackColorDependingOnNeutral(Nud nud1, Color color1, Nud nud2, Color color2, decimal neutralNumber = 0)
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

        private void setBackColorDependingOnNeutral(Nud nud1, Color color1, Nud nud2, Color color2, Nud nud3, Color color3, decimal neutralNumber = 0)
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
            updateCalculations();
        }

        private void nudLw_ValueChanged(object sender, EventArgs e)
        {
            OnLevelChanged?.Invoke();
            updateCalculations();
            setBackColorDependingOnNeutral(nudLw, Color.FromArgb(0, 170, 28),
                nudIw, Color.FromArgb(219, 253, 201),
                nudIwM, Color.FromArgb(167, 246, 125));
        }

        private void nudIw_ValueChanged(object sender, EventArgs e)
        {
            updateCalculations();
            setBackColorDependingOnNeutral(nudLw, Color.FromArgb(0, 170, 28),
                nudIw, Color.FromArgb(219, 253, 201),
                nudIwM, Color.FromArgb(167, 246, 125));
        }

        private void nudTBHM_ValueChanged(object sender, EventArgs e)
        {
            updateCalculations();
            if (_tamed || _bred) setBackColorDependingOnNeutral(nudTBHM, Color.FromArgb(255, 241, 164), 1);
        }

        private void nudTa_ValueChanged(object sender, EventArgs e)
        {
            updateCalculations();
            if (_tamed || _bred) setBackColorDependingOnNeutral(nudTa, Color.FromArgb(255, 233, 203),
                nudTaM, Color.FromArgb(255, 202, 129));
        }

        private void nudTm_ValueChanged(object sender, EventArgs e)
        {
            updateCalculations();
            if (_tamed || _bred) setBackColorDependingOnNeutral(nudTm, Color.FromArgb(202, 227, 249),
                nudTmM, Color.FromArgb(124, 181, 229));
        }

        private void nudLd_ValueChanged(object sender, EventArgs e)
        {
            OnLevelChanged?.Invoke();
            updateCalculations();
            setBackColorDependingOnNeutral(nudLd, Color.FromArgb(242, 41, 81),
                nudId, Color.FromArgb(254, 202, 213),
                nudIdM, Color.FromArgb(250, 127, 153));
        }

        private void nudId_ValueChanged(object sender, EventArgs e)
        {
            updateCalculations();
            setBackColorDependingOnNeutral(nudLd, Color.FromArgb(242, 41, 81),
                nudId, Color.FromArgb(254, 202, 213),
                nudIdM, Color.FromArgb(250, 127, 153));
        }

        private enum ControlsInUse
        {
            wild, tamed, bred
        }
    }
}
