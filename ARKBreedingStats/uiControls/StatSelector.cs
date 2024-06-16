using System;
using System.Windows.Forms;
using ARKBreedingStats.species;

namespace ARKBreedingStats.uiControls
{
    /// <summary>
    /// Allows to select a stat.
    /// </summary>
    public partial class StatSelector : UserControl
    {
        public event Action<int> StatIndexSelected;
        private Species _selectedSpecies;
        private ToolTip _tt;

        public StatSelector()
        {
            InitializeComponent();

            _tt = new ToolTip();

            var r = new RadioButton
            {
                Tag = -1,
                Text = "×",
                Appearance = Appearance.Button,
                AutoSize = true
            };
            r.CheckedChanged += RadioButtonCheckedChanged;
            flowLayoutPanel1.Controls.Add(r);

            foreach (var si in Stats.DisplayOrder)
            {
                if (si == Stats.Torpidity) continue;
                r = new RadioButton
                {
                    Tag = si,
                    Text = Utils.StatName(si, true),
                    Appearance = Appearance.Button,
                    AutoSize = true,
                    Visible = false
                };
                r.CheckedChanged += RadioButtonCheckedChanged;
                flowLayoutPanel1.Controls.Add(r);
            }

            Disposed += (s, e) => _tt.RemoveAll();
        }

        private void RadioButtonCheckedChanged(object sender, EventArgs e)
        {
            if (!(sender is RadioButton r && r.Checked && r.Tag is int statIndex)) return;
            StatIndexSelected?.Invoke(statIndex);
        }

        /// <summary>
        /// Call this method if the language or species was changed (some species have custom stat names).
        /// </summary>
        public void SetStatNames(Species species)
        {
            if (species == _selectedSpecies) return;
            _selectedSpecies = species;
            var statNames = species.statNames;

            foreach (Control c in flowLayoutPanel1.Controls)
            {
                if (!(c is RadioButton r && r.Tag is int statIndex)) continue;
                if (statIndex == -1)
                {
                    _tt.SetToolTip(c, Loc.S("clear"));
                    continue;
                }

                if (!species.UsesStat(statIndex))
                {
                    c.Visible = false;
                    continue;
                }

                c.Visible = true;
                c.Text = Utils.StatName(statIndex, true, statNames);
                _tt.SetToolTip(c, Utils.StatName(statIndex, false, statNames));
            }
        }
    }
}
