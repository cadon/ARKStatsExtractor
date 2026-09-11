using System;
using ARKBreedingStats.uiControls;

namespace ARKBreedingStats.settings
{
    /// <summary>
    /// Represents the multipliers of a stat.
    /// </summary>
    internal class StatMultipliers
    {
        private Nud[] _inputControls;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputControls">Input controls for the stat multipliers in the order: tameAdd, tameMult, domLevel, wildLevel</param>
        public StatMultipliers(Nud[] inputControls)
        {
            if (inputControls?.Length != 4)
                throw new IndexOutOfRangeException(
                    $"When initializing {nameof(StatMultipliers)} exactly 4 input controls are required.");
            _inputControls = inputControls;
        }

        /// <summary>
        /// Stat multipliers. Setting a single value of the array won't do anything, use SetMultiplier() for that.
        /// Indices: 0: TameAdd, 1: TameMult, 2: DomLevel, 3: WildLevel
        /// </summary>
        public double[] Multipliers
        {
            get => new[] { (double)_inputControls[0].Value, (double)_inputControls[1].Value, (double)_inputControls[2].Value, (double)_inputControls[3].Value };
            set
            {
                if (value?.Length == 4)
                {
                    _inputControls[0].ValueSave = (decimal)value[0];
                    _inputControls[1].ValueSave = (decimal)value[1];
                    _inputControls[2].ValueSave = (decimal)value[2];
                    _inputControls[3].ValueSave = (decimal)value[3];
                }
                else
                {
                    _inputControls[0].ValueSave = 1;
                    _inputControls[1].ValueSave = 1;
                    _inputControls[2].ValueSave = 1;
                    _inputControls[3].ValueSave = 1;
                }
            }
        }

        /// <summary>
        /// Set value of a stat multiplier.
        /// </summary>
        /// <param name="index">0: TameAdd, 1: TameMult, 2: DomLevel, 3: WildLevel</param>
        /// <param name="value"></param>
        public void SetMultiplier(int index, double value)
        {
            if (index < 0 || index > 3) return;
            _inputControls[index].ValueSaveDouble = value;
        }

        /// <summary>
        /// Set the values that are considered default. These values are a dimmed so non default values are spotted easier.
        /// </summary>
        public void SetNeutralValues(double[] nv)
        {
            if (nv?.Length == 4)
            {
                _inputControls[0].NeutralNumber = (decimal)nv[0];
                _inputControls[1].NeutralNumber = (decimal)nv[1];
                _inputControls[2].NeutralNumber = (decimal)nv[2];
                _inputControls[3].NeutralNumber = (decimal)nv[3];
            }
            else
            {
                _inputControls[0].NeutralNumber = 1;
                _inputControls[1].NeutralNumber = 1;
                _inputControls[2].NeutralNumber = 1;
                _inputControls[3].NeutralNumber = 1;
            }
        }

        public void SetHighlighted(bool highlighted)
        {
            foreach (var c in _inputControls)
                c.SetExtraHighlightNonDefault(highlighted);
        }

    }
}
