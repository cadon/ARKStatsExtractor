using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.mods
{
    public partial class CustomStatOverridesEditor : Form
    {
        private StatBaseValuesEdit[] overrideEdits;
        private Species selectedSpecies;
        private CreatureCollection cc;
        private List<Species> species;
        public bool StatOverridesChanged;
        private Timer throttlingTimer;

        public CustomStatOverridesEditor(List<Species> species, CreatureCollection cc)
        {
            InitializeComponent();
            overrideEdits = new StatBaseValuesEdit[Stats.StatsCount];
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                var se = new StatBaseValuesEdit();
                se.SetStatNameByIndex(s);
                overrideEdits[s] = se;
                flowLayoutPanelOverrideEdits.Controls.Add(se);
                flowLayoutPanelOverrideEdits.SetFlowBreak(se, true);
            }

            this.cc = cc;
            this.species = species;
            throttlingTimer = new Timer { Interval = 200 };
            throttlingTimer.Tick += ThrottlingTimer_Tick;

            UpdateList(species);
        }

        private void UpdateList(IEnumerable<Species> displayedSpecies)
        {
            lvSpecies.Items.Clear();
            if (displayedSpecies == null) return;

            lvSpecies.Items.AddRange(displayedSpecies.Select(s => new ListViewItem(new string[] { s.DescriptiveNameAndMod, s.blueprintPath })
            {
                Tag = s,
                BackColor = RowBackColor(cc?.CustomSpeciesStats?.ContainsKey(s.blueprintPath) ?? false)
            }).ToArray());
        }

        /// <summary>
        /// Backcolor of the list row depending if the species is overridden.
        /// </summary>
        private Color RowBackColor(bool hasOverride) => hasOverride ? Color.PeachPuff : SystemColors.Window;

        private void lvSpecies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSpecies.SelectedItems.Count == 0)
            {
                return;
            }

            SuspendLayout();

            if (!(lvSpecies.SelectedItems[0].Tag is Species species)) return;
            selectedSpecies = species;

            double?[][] overrides = cc?.CustomSpeciesStats?.ContainsKey(selectedSpecies.blueprintPath) ?? false ? cc.CustomSpeciesStats[selectedSpecies.blueprintPath] : null;

            // set control values to overridden values or to default values.
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                overrideEdits[s].SetStatNameByIndex(s, species.statNames);
                overrideEdits[s].SetStatOverrides(selectedSpecies.fullStatsRaw[s], overrides?[s]);
                overrideEdits[s].SetImprintingMultiplierOverride(selectedSpecies.StatImprintingMultipliersDefault[s], overrides != null && overrides.Length > Stats.StatsCount ? overrides[Stats.StatsCount]?[s] : null);
            }
            ResumeLayout();
        }

        private void btRemoveOverride_Click(object sender, EventArgs e)
        {
            if (selectedSpecies != null
                && (cc?.CustomSpeciesStats?.Remove(selectedSpecies.blueprintPath) ?? false))
            {
                if (lvSpecies.SelectedItems.Count != 0)
                    lvSpecies.SelectedItems[0].BackColor = RowBackColor(false);
                lvSpecies_SelectedIndexChanged(null, null);
                StatOverridesChanged = true;
            }
        }

        private void btSaveOverride_Click(object sender, EventArgs e)
        {
            if (cc == null) return;
            if (cc.CustomSpeciesStats == null) cc.CustomSpeciesStats = new Dictionary<string, double?[][]>();
            if (!cc.CustomSpeciesStats.ContainsKey(selectedSpecies.blueprintPath))
                cc.CustomSpeciesStats.Add(selectedSpecies.blueprintPath, new double?[Stats.StatsCount + 1][]);

            // if current array doesn't consider statImprintingMultipliers, add an element
            if (cc.CustomSpeciesStats[selectedSpecies.blueprintPath].Length == Stats.StatsCount)
            {
                cc.CustomSpeciesStats[selectedSpecies.blueprintPath] = cc.CustomSpeciesStats[selectedSpecies.blueprintPath].Append(null).ToArray();
            }

            var overrides = cc.CustomSpeciesStats[selectedSpecies.blueprintPath];
            double?[] imprintingOverrides = new double?[Stats.StatsCount];

            bool hasOverride = false;
            bool hasImprintingOverride = false;
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                overrides[s] = overrideEdits[s].StatOverrides;
                if (overrides[s] != null) hasOverride = true;

                // stat imprinting multipliers
                imprintingOverrides[s] = overrideEdits[s].ImprintingOverride;
                if (imprintingOverrides[s] != null) hasImprintingOverride = true;
            }

            cc.CustomSpeciesStats[selectedSpecies.blueprintPath][Stats.StatsCount] = hasImprintingOverride ? imprintingOverrides : null;

            if (lvSpecies.SelectedItems.Count != 0)
                lvSpecies.SelectedItems[0].BackColor = RowBackColor(hasOverride || hasImprintingOverride);
            StatOverridesChanged = true;
        }

        private void loadOverrideFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadCustomOverrideFile(false);
        }

        private void addOverrideFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadCustomOverrideFile(true);
        }

        private void LoadCustomOverrideFile(bool append)
        {
            using (OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "ASB custom stat override file (*.json)|*.json"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                if (!FileService.LoadJsonFile(dlg.FileName, out Dictionary<string, double?[][]> dict, out string error))
                {
                    MessageBoxes.ShowMessageBox(error, $"Error loading file");
                    return;
                }

                int newOverrides = 0;
                int overwrittenOverrides = 0;
                bool? overwriteCurrent = null;
                if (append)
                {
                    foreach (var c in dict)
                    {
                        if (cc.CustomSpeciesStats.ContainsKey(c.Key))
                        {
                            if (overwriteCurrent == null)
                            {
                                switch (MessageBox.Show("Some stat overrides are already set in the current library.\nClick Yes to overwrite the existing overrides with the loaded ones.\nClick No to keep the existing ones.", "Overwrite existing overrides?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                                {
                                    case DialogResult.Cancel: return;
                                    case DialogResult.Yes: overwriteCurrent = true; break;
                                    case DialogResult.No: overwriteCurrent = false; break;
                                }
                            }
                            if (overwriteCurrent == true)
                            {
                                cc.CustomSpeciesStats[c.Key] = c.Value;
                                overwrittenOverrides++;
                            }
                        }
                        else
                        {
                            cc.CustomSpeciesStats.Add(c.Key, c.Value);
                            newOverrides++;
                        }
                    }
                    MessageBox.Show($"{newOverrides} overrides added.\n{overwrittenOverrides} overrides replaced.", "Import done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    cc.CustomSpeciesStats = dict;
                    MessageBox.Show($"{dict.Count} overrides imported.", "Import done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                FilterList();
                StatOverridesChanged = true;
            }
        }

        private void exportOverrideFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "ASB custom stat override file (*.json)|*.json"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                if (!FileService.SaveJsonFile(dlg.FileName, cc.CustomSpeciesStats, out string error))
                {
                    MessageBoxes.ShowMessageBox(error, $"Error saving file");
                }
            }
        }

        private void cbOnlyDisplayOverriddenSpecies_CheckedChanged(object sender, EventArgs e)
        {
            FilterList();
        }

        private void btClearFilter_Click(object sender, EventArgs e)
        {
            tbFilterSpecies.Clear();
        }

        private void tbFilterSpecies_TextChanged(object sender, EventArgs e)
        {
            // throttle
            if (throttlingTimer.Enabled) return;
            throttlingTimer.Start();
        }

        /// <summary>
        /// Filters the displayed species to only show species that match the text.
        /// </summary>
        private void FilterList()
        {
            IEnumerable<Species> filteredSpecies;
            if (string.IsNullOrEmpty(tbFilterSpecies.Text))
                filteredSpecies = species;
            else
                filteredSpecies = species.Where(s => s.blueprintPath.IndexOf(tbFilterSpecies.Text, StringComparison.OrdinalIgnoreCase) != -1 || s.DescriptiveNameAndMod.IndexOf(tbFilterSpecies.Text, StringComparison.OrdinalIgnoreCase) != -1);

            if (cbOnlyDisplayOverriddenSpecies.Checked)
            {
                if (cc?.CustomSpeciesStats == null)
                    UpdateList(null);
                else UpdateList(filteredSpecies.Where(s => cc.CustomSpeciesStats.ContainsKey(s.blueprintPath)));
            }
            else
                UpdateList(filteredSpecies);
        }

        private void ThrottlingTimer_Tick(object sender, EventArgs e)
        {
            throttlingTimer.Stop();
            FilterList();
        }
    }
}
