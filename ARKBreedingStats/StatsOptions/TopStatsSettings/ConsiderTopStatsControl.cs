using System.Windows.Forms;

namespace ARKBreedingStats.StatsOptions.TopStatsSettings
{
    internal class ConsiderTopStatsControl : StatsOptionsControl<ConsiderTopStats>
    {
        private readonly CheckBox[] _controlsConsiderAsTopStats = new CheckBox[Stats.StatsCount];
        private readonly CheckBox[] _controlsOverrideParent = new CheckBox[Stats.StatsCount];
        private readonly CheckBox _cbOverrideAll = new CheckBox();

        public ConsiderTopStatsControl(StatsOptionsSettings<ConsiderTopStats> settings, ToolTip tt) : base(settings, tt) { }

        protected override void InitializeStatControls()
        {
            _cbOverrideAll.Text = "override all";
            _cbOverrideAll.Click += (s, e) =>
            {
                var isChecked = ((CheckBox)s).Checked;
                for (var i = 0; i < _controlsOverrideParent.Length; i++)
                {
                    _controlsOverrideParent[i].Checked = isChecked;
                    SelectedStatsOptions.StatOptions[i].OverrideParent = isChecked;
                }
            };
            StatsContainer.Controls.Add(_cbOverrideAll);

            var cb = new CheckBox { Text = "consider all" };
            cb.Click += (s, e) =>
            {
                var isChecked = ((CheckBox)s).Checked;
                for (var i = 0; i < _controlsConsiderAsTopStats.Length; i++)
                {
                    _controlsConsiderAsTopStats[i].Checked = isChecked;
                    SelectedStatsOptions.StatOptions[i].ConsiderStat = isChecked;
                }
            };
            StatsContainer.Controls.Add(cb);
            StatsContainer.SetFlowBreak(cb, true);

            foreach (var si in Stats.DisplayOrder)
            {
                var locVar = si;
                var c = new CheckBox { Text = "override" };
                c.Click += (s, e) =>
                    SelectedStatsOptions.StatOptions[locVar].OverrideParent = ((CheckBox)s).Checked;
                _controlsOverrideParent[si] = c;
                StatsContainer.Controls.Add(c);

                c = new CheckBox { Text = $"[{si}] {Utils.StatName(si)}" };
                c.Click += (s, e) =>
                {
                    SelectedStatsOptions.StatOptions[locVar].ConsiderStat = ((CheckBox)s).Checked;
                    SelectedStatsOptions.StatOptions[locVar].OverrideParent = true;
                };
                _controlsConsiderAsTopStats[si] = c;
                StatsContainer.Controls.Add(c);
                StatsContainer.SetFlowBreak(c, true);
            }
        }

        protected override void UpdateStatsControls(bool isNotRoot)
        {
            _cbOverrideAll.Visible = isNotRoot;
            for (var si = 0; si < Stats.StatsCount; si++)
            {
                _controlsOverrideParent[si].Checked = !isNotRoot || SelectedStatsOptions.StatOptions[si].OverrideParent;
                _controlsOverrideParent[si].Visible = isNotRoot;
                _controlsConsiderAsTopStats[si].Checked = SelectedStatsOptions.StatOptions[si].ConsiderStat;
            }
        }
    }
}
