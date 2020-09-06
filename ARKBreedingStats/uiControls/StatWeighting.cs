using ARKBreedingStats.species;
using ARKBreedingStats.values;
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
        private Dictionary<string, double[]> _customWeightings = new Dictionary<string, double[]>();
        private Species _currentSpecies;
        private readonly Label[] _statLabels;
        private readonly Nud[] _weightNuds;
        public event Action WeightingsChanged;
        private readonly Debouncer _valueChangedDebouncer = new Debouncer();

        public StatWeighting()
        {
            InitializeComponent();
            ToolTip tt = new ToolTip();
            tt.SetToolTip(groupBox1, "Increase the weights for stats that are more important to you to be high in the offspring.\nRight click for Presets.");
            _currentSpecies = null;
            _weightNuds = new Nud[Values.STATS_COUNT];
            _statLabels = new Label[Values.STATS_COUNT];

            var displayedStats = new int[]{
                (int)StatNames.Health,
                (int)StatNames.Stamina,
                (int)StatNames.Oxygen,
                (int)StatNames.Food,
                (int)StatNames.Weight,
                (int)StatNames.MeleeDamageMultiplier,
                (int)StatNames.SpeedMultiplier,
                (int)StatNames.CraftingSpeedMultiplier
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
                tableLayoutPanelMain.Controls.Add(l);
                tableLayoutPanelMain.Controls.Add(n);
                tableLayoutPanelMain.SetRow(l, ds);
                tableLayoutPanelMain.SetRow(n, ds);
                tableLayoutPanelMain.SetColumn(n, 1);
                _statLabels[displayedStats[ds]] = l;
                _weightNuds[displayedStats[ds]] = n;
            }
        }

        public void SetSpecies(Species species)
        {
            if (species == null) return;

            _currentSpecies = species;
            for (int s = 0; s < Values.STATS_COUNT; s++)
                if (_statLabels[s] != null)
                    _statLabels[s].Text = Utils.StatName(s, true, species.statNames);
        }

        private void Input_ValueChanged(object sender, EventArgs e)
        {
            if (WeightingsChanged != null)
                _valueChangedDebouncer.Debounce(200, WeightingsChanged, Dispatcher.CurrentDispatcher);
        }

        public double[] Weightings
        {
            get
            {
                double[] w = WeightValues;
                double s = w.Sum() / Values.STATS_COUNT;
                if (s > 0)
                {
                    for (int i = 0; i < Values.STATS_COUNT; i++)
                        w[i] /= s;
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

                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (_weightNuds[s] != null)
                        _weightNuds[s].ValueSave = (decimal)value[s];
                }
            }
            get
            {
                double[] weights = new double[Values.STATS_COUNT];

                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (_weightNuds[s] != null)
                        weights[s] = (double)_weightNuds[s].Value;
                    else
                        weights[s] = 1;
                }

                return weights;
            }
        }

        private void btAllToOne_Click(object sender, EventArgs e)
        {
            cbbPresets.SelectedIndex = 0;
            double[] values = new double[Values.STATS_COUNT];
            for (int s = 0; s < Values.STATS_COUNT; s++) values[s] = 1;
            WeightValues = values;
        }

        private void cbbPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectPresetByName((sender as ComboBox)?.SelectedItem.ToString());
        }

        /// <summary>
        /// Sets the according preset. If not available, false is returned.
        /// </summary>
        /// <param name="presetName"></param>
        /// <returns></returns>
        public bool TrySetPresetByName(string presetName)
        {
            int index = presetName == null ? -1 : cbbPresets.Items.IndexOf(presetName);
            if (index == -1)
                return false;

            cbbPresets.SelectedIndex = index;
            return true;
        }

        /// <summary>
        /// Sets the statweighting to the preset with the given name, if that is available. If not available, nothing happens.
        /// </summary>
        /// <param name="presetName">Name of the preset</param>
        /// <returns>True if the preset was set, false if there is no preset with the given name</returns>
        private bool SelectPresetByName(string presetName)
        {
            if (_customWeightings.ContainsKey(presetName))
            {
                WeightValues = _customWeightings[presetName];
                return true;
            }
            return false;
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

        private void btSavePreset_Click(object sender, EventArgs e)
        {
            SavePreset();
        }

        private void SavePreset()
        {
            if (Utils.ShowTextInput("Preset-Name", out string presetName, "New Preset", _currentSpecies.name) && presetName.Length > 0)
            {
                if (_customWeightings.ContainsKey(presetName))
                {
                    if (MessageBox.Show("Preset-Name exists already, overwrite?", "Overwrite Preset?",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        _customWeightings[presetName] = WeightValues;
                    }
                    else
                        return;
                }
                else
                    _customWeightings.Add(presetName, WeightValues);
                CustomWeightings = _customWeightings;
                TrySetPresetByName(presetName);
            }
        }

        public Dictionary<string, double[]> CustomWeightings
        {
            get => _customWeightings;
            set
            {
                if (value != null)
                {
                    _customWeightings = value;
                    // clear custom presets
                    cbbPresets.Items.Clear();
                    cbbPresets.Items.Add("-");

                    foreach (KeyValuePair<string, double[]> e in _customWeightings)
                    {
                        cbbPresets.Items.Add(e.Key);
                    }
                    cbbPresets.SelectedIndex = 0;
                }
            }
        }
    }
}
