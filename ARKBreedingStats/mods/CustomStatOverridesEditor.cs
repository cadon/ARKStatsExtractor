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

            lvSpecies.Items.AddRange(species.Select(s => new ListViewItem(new string[] { s.DescriptiveNameAndMod, s.blueprintPath })
            {
                Tag = s,
                BackColor = BackColor(cc?.CustomSpeciesStats?.ContainsKey(s.blueprintPath) ?? false)
            }).ToArray());
            this.cc = cc;
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
    }
}
