using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class StatWeighting : UserControl
    {
        private Dictionary<string, double[]> customWeightings = new Dictionary<string, double[]>();
        private Species currentSpecies;
        private Label[] statLabels;
        private Nud[] weightNuds;
        public event EventHandler WeightingsChanged;
        private int[] displayedStats;
        private CancellationTokenSource cancelSource;

        public StatWeighting()
        {
            InitializeComponent();
            ToolTip tt = new ToolTip();
            tt.SetToolTip(groupBox1, "Increase the weights for stats that are more important to you to be high in the offspring.\nRightclick for Presets.");
            currentSpecies = null;
            weightNuds = new Nud[Values.STATS_COUNT];
            statLabels = new Label[Values.STATS_COUNT];

            displayedStats = new int[]{
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
                statLabels[displayedStats[ds]] = l;
                weightNuds[displayedStats[ds]] = n;
            }
        }

        public void SetSpecies(Species species)
        {
            if (species == null) return;

            currentSpecies = species;
            for (int s = 0; s < Values.STATS_COUNT; s++)
                if (statLabels[s] != null)
                    statLabels[s].Text = Utils.StatName(s, true, species.statNames);
        }

        private async void Input_ValueChanged(object sender, EventArgs e)
        {
            cancelSource?.Cancel();
            using (cancelSource = new CancellationTokenSource())
            {
                try
                {
                    await Task.Delay(600, cancelSource.Token); // only invoke in intervals
                    WeightingsChanged?.Invoke(null, null);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
            cancelSource = null;
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
                    if (weightNuds[s] != null)
                        weightNuds[s].ValueSave = (decimal)value[s];
                }
            }
            get
            {
                double[] weights = new double[Values.STATS_COUNT];

                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (weightNuds[s] != null)
                        weights[s] = (double)weightNuds[s].Value;
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
            SelectPresetByName((sender as ComboBox).SelectedItem.ToString());
        }

        /// <summary>
        /// Sets the according preset. If not available, false is returned.
        /// </summary>
        /// <param name="presetName"></param>
        /// <returns></returns>
        public bool TrySetPresetByName(string presetName)
        {
            int index = cbbPresets.Items.IndexOf(presetName);
            if (index >= 0)
            {
                cbbPresets.SelectedIndex = index;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the statweighting to the preset with the given name, if that is available. If not available, nothing happens.
        /// </summary>
        /// <param name="presetName">Name of the preset</param>
        /// <returns>True if the preset was set, false if there is no preset with the given name</returns>
        private bool SelectPresetByName(string presetName)
        {
            if (customWeightings.ContainsKey(presetName))
            {
                WeightValues = customWeightings[presetName];
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
            if (customWeightings.ContainsKey(presetName)
                    && MessageBox.Show("Delete the stat-weight-preset \"" + presetName + "\"?", "Delete?",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes
            )
            {
                customWeightings.Remove(presetName);
                CustomWeightings = customWeightings;
            }
        }

        private void saveAsPresetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SavePreset();
        }

        private void btSavePreset_Click(object sender, EventArgs e)
        {
            SavePreset();
        }

        private void SavePreset()
        {
            if (Utils.ShowTextInput("Preset-Name", out string presetName, "New Preset", currentSpecies.name) && presetName.Length > 0)
            {
                if (customWeightings.ContainsKey(presetName))
                {
                    if (MessageBox.Show("Preset-Name exists already, overwrite?", "Overwrite Preset?",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        customWeightings[presetName] = WeightValues;
                    }
                    else
                        return;
                }
                else
                    customWeightings.Add(presetName, WeightValues);
                CustomWeightings = customWeightings;
                TrySetPresetByName(presetName);
            }
        }

        public Dictionary<string, double[]> CustomWeightings
        {
            get => customWeightings;
            set
            {
                if (value != null)
                {
                    customWeightings = value;
                    // clear custom presets
                    cbbPresets.Items.Clear();
                    cbbPresets.Items.Add("-");

                    foreach (KeyValuePair<string, double[]> e in customWeightings)
                    {
                        cbbPresets.Items.Add(e.Key);
                    }
                    cbbPresets.SelectedIndex = 0;
                }
            }
        }
    }
}
