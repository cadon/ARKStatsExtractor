using System.Windows.Forms;

namespace ARKBreedingStats.SpeciesOptions.TopStatsSettings
{
    internal class ConsiderTopStatsControl : SpeciesOptionsControl<ConsiderTopStats, StatsOptions<ConsiderTopStats>>
    {
        private readonly CheckBox[] _controlsConsiderAsTopStats = new CheckBox[Stats.StatsCount];
        private readonly CheckBox[] _controlsOverrideParent = new CheckBox[Stats.StatsCount];
        private readonly CheckBox _cbOverrideAll = new CheckBox();

        public ConsiderTopStatsControl(SpeciesOptionsSettings<ConsiderTopStats, StatsOptions<ConsiderTopStats>> settings, ToolTip tt) : base(settings, tt) { }

        protected override void InitializeStatControls()
        {
            _cbOverrideAll.Text = "override all";
            _cbOverrideAll.Click += (s, e) =>
            {
                var isChecked = ((CheckBox)s).Checked;
                for (var i = 0; i < _controlsOverrideParent.Length; i++)
                {
                    _controlsOverrideParent[i].Checked = isChecked;
                    SelectedOptions.Options[i].OverrideParent = isChecked;
                }
            };
            OptionsContainer.Controls.Add(_cbOverrideAll);

            var cb = new CheckBox { Text = "consider all" };
            cb.Click += (s, e) =>
            {
                var isChecked = ((CheckBox)s).Checked;
                for (var i = 0; i < _controlsConsiderAsTopStats.Length; i++)
                {
                    _controlsConsiderAsTopStats[i].Checked = isChecked;
                    SelectedOptions.Options[i].ConsiderStat = isChecked;
                }
            };
            OptionsContainer.Controls.Add(cb);
            OptionsContainer.SetFlowBreak(cb, true);

            foreach (var si in Stats.DisplayOrder)
            {
                var locVar = si;
                var c = new CheckBox { Text = "override" };
                Tt.SetToolTip(c, "Override settings of parent setting. If this is unchecked, the setting here is ignored and the setting of the parent setting is used.");
                c.Click += (s, e) =>
                    SelectedOptions.Options[locVar].OverrideParent = ((CheckBox)s).Checked;
                _controlsOverrideParent[si] = c;
                OptionsContainer.Controls.Add(c);

                c = new CheckBox { Text = $"[{si}] {Utils.StatName(si)}", AutoSize = true };
                c.Click += (s, e) =>
                {
                    SelectedOptions.Options[locVar].ConsiderStat = ((CheckBox)s).Checked;
                    SelectedOptions.Options[locVar].OverrideParent = true;
                };
                _controlsConsiderAsTopStats[si] = c;
                OptionsContainer.Controls.Add(c);
                OptionsContainer.SetFlowBreak(c, true);
            }
        }

        protected override void UpdateOptionControls(bool isNotRoot)
        {
            _cbOverrideAll.Visible = isNotRoot;
            for (var si = 0; si < Stats.StatsCount; si++)
            {
                _controlsOverrideParent[si].Checked = !isNotRoot || SelectedOptions.Options[si].OverrideParent;
                _controlsOverrideParent[si].Visible = isNotRoot;
                _controlsConsiderAsTopStats[si].Checked = SelectedOptions.Options[si]?.ConsiderStat ?? true;
            }
        }
    }
}
