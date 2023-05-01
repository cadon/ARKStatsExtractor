using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.uiControls
{
    public partial class StatWeighting : UserControl
    {
        private Dictionary<string, (double[], byte[])> _customWeightings = new Dictionary<string, (double[], byte[])>();
        private Species _currentSpecies;
        private readonly Label[] _statLabels = new Label[Stats.StatsCount];
        private readonly Nud[] _weightNuds = new Nud[Stats.StatsCount];
        private readonly TriButton[] _statEvenOddButtons = new TriButton[Stats.StatsCount];
        public event Action WeightingsChanged;
        private readonly Debouncer _valueChangedDebouncer = new Debouncer();
        private readonly ToolTip _tt = new ToolTip();
        private const string NoPreset = "-";
        private const string DefaultPreset = "Default";

        public StatWeighting()
        {
            InitializeComponent();
            _currentSpecies = null;

            var displayedStats = new int[]{
                Stats.Health,
                Stats.Stamina,
                Stats.Oxygen,
                Stats.Food,
                Stats.Weight,
                Stats.MeleeDamageMultiplier,
                Stats.SpeedMultiplier,
                Stats.CraftingSpeedMultiplier
            };

            for (int ds = 0; ds < displayedStats.Length; ds++)
            {
                if (ds > 0) // first row exists due to designer
                    tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                Label l = new Label { TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                Nud n = new Nud
                {
                    DecimalPlaces = 1,
                    Increment = 0.1M,
                    Maximum = 1000,
                    Minimum = -1000
                };
                n.ValueChanged += Input_ValueChanged;
                var tBt = new TriButton(_tt)
                {
                    Margin = new Padding()
                };
                tBt.StateChanged += WeightingsChangedNotifier;
                tableLayoutPanelMain.Controls.Add(l);
                tableLayoutPanelMain.Controls.Add(n);
                tableLayoutPanelMain.Controls.Add(tBt);
                tableLayoutPanelMain.SetRow(l, ds);
                tableLayoutPanelMain.SetRow(n, ds);
                tableLayoutPanelMain.SetRow(tBt, ds);
                tableLayoutPanelMain.SetColumn(n, 1);
                tableLayoutPanelMain.SetColumn(tBt, 2);
                _statLabels[displayedStats[ds]] = l;
                _weightNuds[displayedStats[ds]] = n;
                _statEvenOddButtons[displayedStats[ds]] = tBt;
            }
        }

        public void SetSpecies(Species species)
        {
            if (species == null) return;

            _currentSpecies = species;
            for (int s = 0; s < Stats.StatsCount; s++)
                if (_statLabels[s] != null)
                {
                    _statLabels[s].Text = Utils.StatName(s, true, species.statNames);
                    _tt.SetToolTip(_statLabels[s], Utils.StatName(s, false, species.statNames));
                }
        }

        private void Input_ValueChanged(object sender, EventArgs e)
        {
            WeightingsChangedNotifier();
        }

        private void WeightingsChangedNotifier()
        {
            if (WeightingsChanged != null)
                _valueChangedDebouncer.Debounce(500, WeightingsChanged, Dispatcher.CurrentDispatcher);
        }

        public double[] Weightings
        {
            get
            {
                double[] w = WeightValues;
                double sum = w.Sum() / Stats.StatsCount;
                if (sum > 0)
                {
                    for (int i = 0; i < Stats.StatsCount; i++)
                        w[i] /= sum;
                }
                return w;
            }
        }

        public double[] WeightValues
        {
            set
            {
                if (value == null)
                    return;

                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (_weightNuds[s] != null)
                        _weightNuds[s].ValueSave = (decimal)value[s];
                }
            }
            get
            {
                double[] weights = new double[Stats.StatsCount];

                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (_weightNuds[s] != null)
                        weights[s] = (double)_weightNuds[s].Value;
                    else
                        weights[s] = 1;
                }

                return weights;
            }
        }

        /// <summary>
        /// Array that for each stat indicates if the level, if high, should be only considered if odd (1), even (2), or always (0).
        /// </summary>
        public byte[] AnyOddEven
        {
            set
            {
                if (value == null)
                    return;

                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (_statEvenOddButtons[s] != null)
                        _statEvenOddButtons[s].ButtonState = value[s];
                }
            }
            get => _statEvenOddButtons.Select(b => b?.ButtonState ?? 0).ToArray();
        }

        private void btAllToOne_Click(object sender, EventArgs e)
        {
            ResetValues();
        }

        /// <summary>
        /// Sets weightings to species. First the blueprint path is checked, then the full species name inclusive mod and variant, then only the base name.
        /// </summary>
        public bool TrySetPresetBySpecies(Species species, bool useDefaultBackupIfAvailable = true)
        {
            if (TrySetPresetByName(species.blueprintPath)) return true;
            if (TrySetPresetByName(species.DescriptiveNameAndMod)) return true;
            if (TrySetPresetByName(species.DescriptiveName)) return true;
            if (TrySetPresetByName(species.name)) return true;
            return useDefaultBackupIfAvailable
                   && TrySetPresetByName(DefaultPreset);
        }

        /// <summary>
        /// Sets the according preset. If not available, false is returned.
        /// </summary>
        public bool TrySetPresetByName(string presetName)
        {
            if (presetName == null) return false;
            if (cbbPresets.SelectedItem as string == presetName) return true;

            int index = cbbPresets.Items.IndexOf(presetName);
            if (index == -1)
                return false;

            cbbPresets.SelectedIndex = index;
            return true;
        }

        private void cbbPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectPresetByName((sender as ComboBox)?.SelectedItem.ToString());
        }

        /// <summary>
        /// Sets the statWeighting to the preset with the given name, if that is available. If not available, nothing happens.
        /// </summary>
        /// <param name="presetName">Name of the preset</param>
        /// <returns>True if the preset was set, false if there is no preset with the given name</returns>
        private bool SelectPresetByName(string presetName)
        {
            if (presetName == NoPreset)
            {
                return true;
            }
            if (!_customWeightings.TryGetValue(presetName, out var weightings)) return false;
            WeightValues = weightings.Item1;
            AnyOddEven = weightings.Item2;
            return true;
        }

        /// <summary>
        /// Resets all weightings.
        /// </summary>
        private void ResetValues()
        {
            WeightValues = Enumerable.Repeat(1d, Stats.StatsCount).ToArray();
            AnyOddEven = Enumerable.Repeat((byte)0, Stats.StatsCount).ToArray();
        }

        /// <summary>
        /// Returns weightings for species. First the blueprint path is checked, then the full species name inclusive mod and variant, then only the base name.
        /// </summary>
        public (double[], byte[]) GetWeightingForSpecies(Species species, bool useDefaultBackupIfAvailable = true)
        {
            if (_customWeightings.TryGetValue(species.blueprintPath, out var weightings)) return weightings;
            if (_customWeightings.TryGetValue(species.DescriptiveNameAndMod, out weightings)) return weightings;
            if (_customWeightings.TryGetValue(species.DescriptiveName, out weightings)) return weightings;
            if (_customWeightings.TryGetValue(species.name, out weightings)) return weightings;
            return useDefaultBackupIfAvailable
                   && _customWeightings.TryGetValue(DefaultPreset, out weightings) ? weightings : (null, null);
        }

        public (double[], byte[]) GetWeightingByPresetName(string presetName, bool useDefaultBackupIfAvailable = true)
        {
            if (_customWeightings.TryGetValue(presetName, out var weightings)) return weightings;
            return useDefaultBackupIfAvailable
                && _customWeightings.TryGetValue(DefaultPreset, out weightings) ? weightings : (null, null);
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            DeletePresetByName(cbbPresets.SelectedItem.ToString());
        }

        private void DeletePresetByName(string presetName)
        {
            if (_customWeightings.ContainsKey(presetName)
                    && MessageBox.Show($"Delete the stat-weight-preset \"{presetName}\"?", "Delete?",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes
            )
            {
                _customWeightings.Remove(presetName);
                CustomWeightings = _customWeightings;
            }
        }

        private void BtSavePreset_Click(object sender, EventArgs e)
        {
            var presetName = cbbPresets.SelectedItem.ToString();
            if (string.IsNullOrEmpty(presetName) || presetName == NoPreset)
                SavePresetAs(_currentSpecies?.name);
            else _customWeightings[presetName] = (WeightValues, AnyOddEven);
        }

        private void btSavePresetAs_Click(object sender, EventArgs e)
        {
            var presetName = cbbPresets.SelectedItem.ToString();
            if (string.IsNullOrEmpty(presetName) || presetName == NoPreset || presetName == DefaultPreset)
                SavePresetAs(_currentSpecies?.name);
            else SavePresetAs(presetName);
        }

        private void SavePresetAs(string presetName)
        {
            string[] suggestions;
            if (_currentSpecies != null)
            {
                suggestions = new[]
                {
                    DefaultPreset,
                    _currentSpecies.name,
                    _currentSpecies.DescriptiveName,
                    _currentSpecies.DescriptiveNameAndMod,
                    _currentSpecies.blueprintPath
                };
            }
            else
                suggestions = new[] { DefaultPreset };


            if (Utils.ShowTextInput("Preset Name", out var presetNameUser, "New Preset", presetName, suggestions)
                && presetNameUser.Length > 0)
            {
                if (_customWeightings.ContainsKey(presetNameUser))
                {
                    if (MessageBox.Show("Preset name exists already, overwrite?", "Overwrite Preset?",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        _customWeightings[presetNameUser] = (WeightValues, AnyOddEven);
                    }
                    else
                        return;
                }
                else
                    _customWeightings.Add(presetNameUser, (WeightValues, AnyOddEven));
                CustomWeightings = _customWeightings;
                TrySetPresetByName(presetNameUser);
            }
        }

        public Dictionary<string, (double[], byte[])> CustomWeightings
        {
            get => _customWeightings;
            set
            {
                if (value == null) return;
                _customWeightings = value;
                // clear custom presets
                cbbPresets.Items.Clear();
                cbbPresets.Items.Add(NoPreset);
                cbbPresets.Items.AddRange(_customWeightings.Keys.OrderBy(s => s).ToArray());
                cbbPresets.SelectedIndex = 0;
                SetComboboxDropdownWidthToMaxItemWidth(cbbPresets);
            }
        }

        private void SetComboboxDropdownWidthToMaxItemWidth(ComboBox cbb)
        {
            var g = cbb.CreateGraphics();
            var verticalScrollBarWidth = cbb.Items.Count > cbb.MaxDropDownItems
                    ? SystemInformation.VerticalScrollBarWidth : 0;

            var maxWidth = cbb.Items.Cast<string>().Select(s => (int)g.MeasureString(s, cbb.Font).Width + verticalScrollBarWidth).Max();
            maxWidth = Math.Min(600, maxWidth);
            if (maxWidth > cbb.DropDownWidth)
                cbb.DropDownWidth = maxWidth;
        }

        private class TriButton : Button
        {
            private readonly ToolTip _tt;
            private byte _buttonState;
            public event Action StateChanged;

            public byte ButtonState
            {
                get => _buttonState;
                set => SetState(value);
            }

            public TriButton(ToolTip tt)
            {
                _tt = tt;
                Click += BtClicked;
            }

            private void BtClicked(object sender, EventArgs e)
            {
                SetState(++_buttonState);
                StateChanged?.Invoke();
            }

            private void SetState(byte state)
            {
                _buttonState = state;
                switch (state)
                {
                    case 1:
                        Text = "1";
                        _tt.SetToolTip(this, "high level has to be odd to be a top stat");
                        break;
                    case 2:
                        Text = "2";
                        _tt.SetToolTip(this, "high level has to be even to be a top stat");
                        break;
                    default:
                        _buttonState = 0;
                        Text = string.Empty;
                        _tt.SetToolTip(this, "high level can be even or odd");
                        break;
                }
            }
        }
    }
}
