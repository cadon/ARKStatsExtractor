using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class StatWeighting : UserControl
    {
        private Dictionary<string, double[]> customWeightings = new Dictionary<string, double[]>();

        public StatWeighting()
        {
            InitializeComponent();
            ToolTip tt = new ToolTip();
            tt.SetToolTip(groupBox1, "Increase the weights for stats that are more important to you to be high in the offspring.\nRightclick for Presets.");
        }

        public double[] Weightings
        {
            get
            {
                double[] w = {
                        (double)numericUpDown1.Value,
                        (double)numericUpDown2.Value,
                        (double)numericUpDown3.Value,
                        (double)numericUpDown4.Value,
                        (double)numericUpDown5.Value,
                        (double)numericUpDown6.Value,
                        (double)numericUpDown7.Value
                };
                double s = w.Sum() / 7;
                if (s > 0)
                {
                    for (int i = 0; i < 7; i++)
                        w[i] /= s;
                }
                return w;
            }
        }

        public double[] Values
        {
            set
            {
                if (value != null && value.Length > 6)
                {
                    numericUpDown1.Value = (decimal)value[0];
                    numericUpDown2.Value = (decimal)value[1];
                    numericUpDown3.Value = (decimal)value[2];
                    numericUpDown4.Value = (decimal)value[3];
                    numericUpDown5.Value = (decimal)value[4];
                    numericUpDown6.Value = (decimal)value[5];
                    numericUpDown7.Value = (decimal)value[6];
                }
            }
            get => new[]
            {
                    (double)numericUpDown1.Value,
                    (double)numericUpDown2.Value,
                    (double)numericUpDown3.Value,
                    (double)numericUpDown4.Value,
                    (double)numericUpDown5.Value,
                    (double)numericUpDown6.Value,
                    (double)numericUpDown7.Value
            };
        }

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            n?.Select(0, n.Text.Length);
        }

        private void setAllWeightsTo1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Values = new double[] { 1, 1, 1, 1, 1, 1, 1 };
        }

        private void ToolStripMenuItemCustom_Click(object sender, EventArgs e)
        {
            SelectPresetByName(sender.ToString());
        }

        /// <summary>
        /// Sets the statweighting to the preset with the given name, if that is available. If not available, nothing happens.
        /// </summary>
        /// <param name="presetName">Name of the preset</param>
        /// <returns>True if the preset was set, false if there is no preset with the given name</returns>
        public bool SelectPresetByName(string presetName)
        {
            if (customWeightings.ContainsKey(presetName))
            {
                // TODO set title or tooltip to selected preset?
                // TODO support csv presets for multiple species?
                Values = customWeightings[presetName];
                return true;
            }
            return false;
        }

        private void ToolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            string presetName = sender.ToString();
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
            if (Utils.ShowTextInput("Preset-Name", out string s, "New Preset") && s.Length > 0)
            {
                if (customWeightings.ContainsKey(s))
                {
                    if (MessageBox.Show("Preset-Name exists already, overwrite?", "Overwrite Preset?", 
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        customWeightings[s] = Values;
                    }
                    else
                        return;
                }
                else
                    customWeightings.Add(s, Values);
                CustomWeightings = customWeightings;
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
                    for (int i = contextMenuStrip1.Items.Count - 4; i > 1; i--)
                    {
                        contextMenuStrip1.Items.RemoveAt(i);
                    }
                    deletePresetToolStripMenuItem.DropDownItems.Clear();

                    foreach (KeyValuePair<string, double[]> e in customWeightings)
                    {
                        ToolStripMenuItem ti = new ToolStripMenuItem(e.Key);
                        ti.Click += ToolStripMenuItemCustom_Click;
                        contextMenuStrip1.Items.Insert(contextMenuStrip1.Items.Count - 3, ti);
                        // menuItem for delete preset
                        ti = new ToolStripMenuItem(e.Key);
                        ti.Click += ToolStripMenuItemDelete_Click;
                        deletePresetToolStripMenuItem.DropDownItems.Add(ti);
                    }
                }
            }
        }
    }
}
