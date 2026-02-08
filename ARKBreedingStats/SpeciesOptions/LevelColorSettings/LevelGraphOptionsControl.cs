using System;
using System.Windows.Forms;

namespace ARKBreedingStats.SpeciesOptions.LevelColorSettings
{
    internal class LevelGraphOptionsControl : SpeciesOptionsControl<StatLevelColors, StatsOptions<StatLevelColors>>
    {
        private StatLevelGraphOptionsControl[] _statOptionsControls;

        public LevelGraphOptionsControl(SpeciesOptionsSettings<StatLevelColors, StatsOptions<StatLevelColors>> settings, ToolTip tt) : base(settings, tt) { }

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
                Text = @"The numbers on the left and right of the colors represent the level range the colors are mapped to, levels lower or higher use the color of the lowest or highest level, respectively.
Drag color gradient with mouse for fast editing.
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
                OptionsContainer.Controls.Add(c);
                OptionsContainer.SetFlowBreak(c, true);
            }
        }

        protected override void UpdateOptionControls(bool isNotRoot)
        {
            for (var si = 0; si < Stats.StatsCount; si++)
                _statOptionsControls[si].SetStatOptions(SelectedOptions.Options?[si], isNotRoot, SelectedOptions.ParentOptions);
        }

        private void ResetCurrentSettingsToDefault(object sender, EventArgs e)
        {
            if (SelectedOptions == null) return;
            if (SelectedOptions.Options == null)
                SelectedOptions.Options = new StatLevelColors[Stats.StatsCount];

            var isNotRoot = !string.IsNullOrEmpty(SelectedOptions.Name);
            for (var si = 0; si < Stats.StatsCount; si++)
            {
                SelectedOptions.Options[si] = new StatLevelColors
                {
                    LevelGraphRepresentation = LevelGraphRepresentation.GetDefault,
                    LevelGraphRepresentationMutation = LevelGraphRepresentation.GetDefaultMutationLevel,
                    UseDifferentColorsForMutationLevels = true
                };
                _statOptionsControls[si].SetStatOptions(SelectedOptions.Options[si], isNotRoot, SelectedOptions.ParentOptions);
            }
        }
    }
}
