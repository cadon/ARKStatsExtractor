using System.Windows.Forms;

namespace ARKBreedingStats.StatsOptions.LevelColorSettings
{
    internal class LevelGraphOptionsControl : StatsOptionsControl<StatLevelColors>
    {
        private StatLevelGraphOptionsControl[] _statOptionsControls;

        public LevelGraphOptionsControl(StatsOptionsSettings<StatLevelColors> settings, ToolTip tt) : base(settings, tt) { }

        protected override void InitializeStatControls()
        {
            _statOptionsControls = new StatLevelGraphOptionsControl[Stats.StatsCount];
            foreach (var si in Stats.DisplayOrder)
            {
                var c = new StatLevelGraphOptionsControl($"[{si}] {Utils.StatName(si, true)}", si, Tt);
                _statOptionsControls[si] = c;
                StatsContainer.Controls.Add(c);
                StatsContainer.SetFlowBreak(c, true);
            }
            StatsContainer.Controls.Add(new Label
            {
                Text = @"Drag color gradient with mouse for fast editing.
On color gradients use shift + right click to copy and shift + left click to paste color settings.
Ctrl + left click to reset colors.",
                AutoSize = true
            });
        }

        protected override void UpdateStatsControls(bool isNotRoot)
        {
            for (var si = 0; si < Stats.StatsCount; si++)
                _statOptionsControls[si].SetStatOptions(SelectedStatsOptions.StatOptions?[si], isNotRoot, SelectedStatsOptions.ParentOptions);
        }
    }
}
