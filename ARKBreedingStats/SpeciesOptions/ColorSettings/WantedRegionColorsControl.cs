using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.SpeciesOptions.ColorSettings
{
    internal class WantedRegionColorsControl : SpeciesOptionsControl<WantedRegionColors, ColorOptions<WantedRegionColors>>
    {
        private readonly TextBox[] _controlsColorIds = new TextBox[Ark.ColorRegionCount];
        private readonly CheckBox[] _controlsOverrideParent = new CheckBox[Ark.ColorRegionCount];
        private readonly CheckBox _cbOverrideAll = new CheckBox();

        public WantedRegionColorsControl(SpeciesOptionsSettings<WantedRegionColors, ColorOptions<WantedRegionColors>> settings, ToolTip tt) : base(settings, tt) { }

        protected override void InitializeStatControls()
        {
            var lb = new Label { Text = "Set color ids per region that should be highlighted if a new creature is imported with one of them.", Margin = new Padding(3, 3, 3, 10) };
            OptionsContainer.Controls.Add(lb);
            OptionsContainer.SetFlowBreak(lb, true);
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
            OptionsContainer.SetFlowBreak(_cbOverrideAll, true);

            for (int ci = 0; ci < Ark.ColorRegionCount; ci++)
            {
                var locVar = ci;
                lb = new Label { Text = Loc.S("Region") + " " + ci, Margin = new Padding(6) };
                OptionsContainer.Controls.Add(lb);
                var labelError = new Label { ForeColor = Color.DarkRed, Margin = new Padding(6) };

                var c = new CheckBox { Text = "override" };
                Tt.SetToolTip(c, "Override settings of parent setting. If this is unchecked, the setting here is ignored and the setting of the parent setting is used.");
                c.Click += (s, e) =>
                    SelectedOptions.Options[locVar].OverrideParent = ((CheckBox)s).Checked;
                _controlsOverrideParent[ci] = c;
                OptionsContainer.Controls.Add(c);

                var tb = new TextBox { Width = 400 };
                tb.LostFocus += (s, e) =>
                {
                    var error = !SelectedOptions.Options[locVar].SetColorsWanted(tb.Text, true, out var errorMessage);
                    labelError.Text = error ? errorMessage : string.Empty;
                    if (!error)
                        tb.Text = SelectedOptions.Options[locVar].GetColorIdsCsv();
                };
                _controlsColorIds[ci] = tb;
                OptionsContainer.Controls.Add(tb);
                OptionsContainer.Controls.Add(labelError);
                OptionsContainer.SetFlowBreak(labelError, true);
            }
        }

        protected override void UpdateOptionControls(bool isNotRoot)
        {
            _cbOverrideAll.Visible = isNotRoot;
            for (int ci = 0; ci < Ark.ColorRegionCount; ci++)
            {
                _controlsOverrideParent[ci].Checked = !isNotRoot || SelectedOptions.Options[ci].OverrideParent;
                _controlsOverrideParent[ci].Visible = isNotRoot;
                _controlsColorIds[ci].Text = SelectedOptions.Options[ci].GetColorIdsCsv();
            }
        }
    }
}
