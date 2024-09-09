using System;
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
                AddWithFlowBreak(c);
            }

            AddWithFlowBreak(new Label
            {
                Text = @"Drag color gradient with mouse for fast editing.
On color gradients use shift + right click to copy and shift + left click to paste color settings.
Ctrl + left click to cycle through presets.",
                AutoSize = true
            });

            var btReset = new Button { Text = "Reset all stats to default colors and ranges", AutoSize = true };
            btReset.Click += ResetCurrentSettingsToDefault;
            AddWithFlowBreak(btReset);

            return;

            void AddWithFlowBreak(Control c)
            {
                StatsContainer.Controls.Add(c);
                StatsContainer.SetFlowBreak(c, true);
            }
        }

        protected override void UpdateStatsControls(bool isNotRoot)
        {
            for (var si = 0; si < Stats.StatsCount; si++)
                _statOptionsControls[si].SetStatOptions(SelectedStatsOptions.StatOptions?[si], isNotRoot, SelectedStatsOptions.ParentOptions);
        }

        private void ResetCurrentSettingsToDefault(object sender, EventArgs e)
        {
            if (SelectedStatsOptions == null) return;
            if (SelectedStatsOptions.StatOptions == null)
                SelectedStatsOptions.StatOptions = new StatLevelColors[Stats.StatsCount];

            var isNotRoot = !string.IsNullOrEmpty(SelectedStatsOptions.Name);
            for (var si = 0; si < Stats.StatsCount; si++)
            {
                SelectedStatsOptions.StatOptions[si] = new StatLevelColors
                {
                    LevelGraphRepresentation = LevelGraphRepresentation.GetDefaultValue,
                    LevelGraphRepresentationMutation = LevelGraphRepresentation.GetDefaultMutationLevelValue,
                    UseDifferentColorsForMutationLevels = true
                };
                _statOptionsControls[si].SetStatOptions(SelectedStatsOptions.StatOptions[si], isNotRoot, SelectedStatsOptions.ParentOptions);
            }
        }
    }
}
