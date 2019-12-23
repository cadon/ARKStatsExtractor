using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats.mods
{
    public partial class CustomStatOverridesEditor : Form
    {
        private StatBaseValuesEdit[] overrideEdits;
        private Species selectedSpecies;
        private CreatureCollection cc;
        private List<Species> species;

        public CustomStatOverridesEditor(List<Species> species, CreatureCollection cc)
        {
            InitializeComponent();
            overrideEdits = new StatBaseValuesEdit[Values.STATS_COUNT];
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                var se = new StatBaseValuesEdit() { StatName = Utils.statName(s, true) };
                overrideEdits[s] = se;
                flowLayoutPanelOverrideEdits.Controls.Add(se);
                flowLayoutPanelOverrideEdits.SetFlowBreak(se, true);
            }

            this.cc = cc;
            this.species = species;
            UpdateList();
        }

        private void UpdateList()
        {
            lvSpecies.Items.Clear();
            lvSpecies.Items.AddRange(species.Select(s => new ListViewItem(new string[] { s.DescriptiveNameAndMod, s.blueprintPath })
            {
                Tag = s,
                BackColor = BackColor(cc?.CustomSpeciesStats?.ContainsKey(s.blueprintPath) ?? false)
            }).ToArray());
        }

        private Color BackColor(bool hasOverride) => hasOverride ? Color.Lavender : SystemColors.Window;

        private void lvSpecies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSpecies.SelectedItems.Count == 0)
            {
                //groupBox2.Enabled = false;
                return;
            }
            SuspendLayout();
            //groupBox2.Enabled = true;

            if (!(lvSpecies.SelectedItems[0].Tag is Species species)) return;
            selectedSpecies = species;

            double[][] overrides = cc?.CustomSpeciesStats?.ContainsKey(selectedSpecies.blueprintPath) ?? false ? cc.CustomSpeciesStats[selectedSpecies.blueprintPath] : null;

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                overrideEdits[s].Overrides = overrides?[s] ?? null;
            }
            ResumeLayout();
        }

        private void btRemoveOverride_Click(object sender, EventArgs e)
        {
            if (selectedSpecies != null
                && (cc?.CustomSpeciesStats?.Remove(selectedSpecies.blueprintPath) ?? false))
            {
                if (lvSpecies.SelectedItems.Count != 0)
                    lvSpecies.SelectedItems[0].BackColor = BackColor(false);
                lvSpecies_SelectedIndexChanged(null, null);
            }
        }

        private void btSaveOverride_Click(object sender, EventArgs e)
        {
            if (cc == null) return;
            if (cc.CustomSpeciesStats == null) cc.CustomSpeciesStats = new Dictionary<string, double[][]>();
            if (!cc.CustomSpeciesStats.ContainsKey(selectedSpecies.blueprintPath))
                cc.CustomSpeciesStats.Add(selectedSpecies.blueprintPath, new double[Values.STATS_COUNT][]);

            var overrides = cc.CustomSpeciesStats[selectedSpecies.blueprintPath];

            bool hasOverride = false;
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                overrides[s] = overrideEdits[s].Overrides;
                if (overrides[s] != null) hasOverride = true;
            }

            if (lvSpecies.SelectedItems.Count != 0)
                lvSpecies.SelectedItems[0].BackColor = BackColor(hasOverride);
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
                if (!FileService.LoadJSONFile(dlg.FileName, out Dictionary<string, double[][]> dict, out string error))
                {
                    MessageBox.Show(error, "Error loading file", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                UpdateList();
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
                if (!FileService.SaveJSONFile(dlg.FileName, cc.CustomSpeciesStats, out string error))
                {
                    MessageBox.Show(error, "Error saving file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
