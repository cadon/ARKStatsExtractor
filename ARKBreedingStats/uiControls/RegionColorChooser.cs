using ARKBreedingStats.species;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.library;
using ARKBreedingStats.utils;
using System.ComponentModel;

namespace ARKBreedingStats.uiControls
{
    public partial class RegionColorChooser : UserControl
    {
        /// <summary>
        /// The first parameter indicates if colors were changed, the second the region index (-1 if not applicable).
        /// </summary>
        public event Action<bool, int> RegionColorChosen;
        private readonly NoPaddingButton[] _buttonColors;
        private byte[] _selectedRegionColorIds;
        private byte[] _selectedColorIdsAlternative;
        private readonly ColorPickerWindow _colorPicker;
        private ColorRegion[] _colorRegions;
        private readonly ToolTip _tt = new();

        /// <summary>
        /// If true, the button text will display the region and color id.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VerboseButtonTexts { get; set; }

        public RegionColorChooser()
        {
            InitializeComponent();

            _buttonColors = new NoPaddingButton[Ark.ColorRegionCount];
            for (var i = 0; i < Ark.ColorRegionCount; i++)
            {
                var b = new NoPaddingButton { Width = UiUtils.UiLengthInt(32), Height = UiUtils.UiLengthInt(27), Margin = new Padding(UiUtils.UiLengthInt(1)), Text = i.ToString() };
                var ii = i;
                b.Click += (s, e) => ChooseColor(ii, b);
                _buttonColors[i] = b;
                flowLayoutPanel1.Controls.Add(b);
            }

            _selectedRegionColorIds = new byte[Ark.ColorRegionCount];
            _selectedColorIdsAlternative = new byte[Ark.ColorRegionCount];

            _colorPicker = new ColorPickerWindow();
            _tt.AutoPopDelay = 7000;
            Disposed += RegionColorChooser_Disposed;
        }

        public void SetOneButtonPerRow(bool onePerRow)
        {
            foreach (var b in _buttonColors)
                flowLayoutPanel1.SetFlowBreak(b, onePerRow);
        }

        public void SetSpecies(Species species, byte[] colorIDs = null, bool showAllRegions = false)
        {
            _selectedRegionColorIds = (colorIDs ?? _selectedRegionColorIds ?? Enumerable.Repeat((byte)0, Ark.ColorRegionCount)).ToArray();
            _selectedColorIdsAlternative = null;
            bool[] colorRegionsUseds;

            if (species?.colors != null)
            {
                _colorRegions = species.colors;
                colorRegionsUseds = showAllRegions ? Enumerable.Repeat(true, Ark.ColorRegionCount).ToArray() : species.EnabledColorRegions;
            }
            else
            {
                // species-info is not available, show all region-buttons
                colorRegionsUseds = new bool[Ark.ColorRegionCount];
                _colorRegions = new ColorRegion[Ark.ColorRegionCount];
                for (var i = 0; i < Ark.ColorRegionCount; i++)
                {
                    _colorRegions[i] = new ColorRegion();
                    colorRegionsUseds[i] = true;
                }
            }

            for (var r = 0; r < Ark.ColorRegionCount; r++)
            {
                _buttonColors[r].Visible = colorRegionsUseds[r];

                if (colorRegionsUseds[r])
                {
                    _buttonColors[r].AlternativeColorPossible = false;
                    SetColorButton(_buttonColors[r], r);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public byte[] ColorIdsAlsoPossible
        {
            get => _selectedColorIdsAlternative?.ToArray();
            set
            {
                _selectedColorIdsAlternative = value;
                if (_selectedColorIdsAlternative == null)
                {
                    foreach (var bt in _buttonColors)
                        bt.AlternativeColorPossible = false;

                    return;
                }
                for (var i = 0; i < Ark.ColorRegionCount; i++)
                    _buttonColors[i].AlternativeColorPossible = _selectedColorIdsAlternative.Length > i && _selectedColorIdsAlternative[i] != 0;
            }
        }

        public void Clear()
        {
            _selectedColorIdsAlternative = null;
            ColorIds = new byte[Ark.ColorRegionCount];
        }

        /// <summary>
        /// Set colors to random ids of the available colors.
        /// </summary>
        internal void RandomColors()
        {
            _selectedColorIdsAlternative = null;
            ColorIds = values.Values.V.Colors.GetRandomColors();
        }

        /// <summary>
        /// Set colors to random values in the set of natural occurring colors of the species.
        /// </summary>
        internal void RandomNaturalColors(Species species)
        {
            _selectedColorIdsAlternative = null;
            ColorIds = species?.RandomSpeciesColors();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public byte[] ColorIds
        {
            get => _selectedRegionColorIds.ToArray();
            set
            {
                if (value == null)
                {
                    Clear();
                    return;
                }

                for (var r = 0; r < Ark.ColorRegionCount; r++)
                {
                    _selectedRegionColorIds[r] = value.Length > r ? value[r] : (byte)0;
                    _buttonColors[r].AlternativeColorPossible = false;
                    SetColorButton(_buttonColors[r], r);
                }
                RegionColorChosen?.Invoke(true, -1);
            }
        }

        private void ChooseColor(int region, Button sender)
        {
            if (_colorPicker.isShown || _colorRegions == null || region >= Ark.ColorRegionCount) return;

            _colorPicker.Cp.PickColor(_selectedRegionColorIds[region], _colorRegions[region]?.name + " (region " + region + ")", _colorRegions[region]?.naturalColors, _selectedColorIdsAlternative?[region] ?? 0);
            if (_colorPicker.ShowDialog() != DialogResult.OK) return;

            // color was chosen
            _selectedRegionColorIds[region] = _colorPicker.Cp.SelectedColorId;
            if (_colorPicker.Cp.SelectedColorIdAlternative != 0)
            {
                if (_selectedColorIdsAlternative == null)
                    _selectedColorIdsAlternative = new byte[Ark.ColorRegionCount];
                _selectedColorIdsAlternative[region] = _colorPicker.Cp.SelectedColorIdAlternative;
                _buttonColors[region].AlternativeColorPossible = true;
            }
            else
            {
                _buttonColors[region].AlternativeColorPossible = false;
                _selectedColorIdsAlternative?[region] = 0;
            }
            SetColorButton(sender, region);
            RegionColorChosen?.Invoke(true, region);
        }

        /// <summary>
        /// Select color that is set for all regions.
        /// </summary>
        internal void ChooseAllColors()
        {
            if (_colorPicker.isShown || _colorRegions == null) return;
            _colorPicker.Cp.PickColor(_selectedRegionColorIds[0], "all regions");
            if (_colorPicker.ShowDialog() != DialogResult.OK) return;
            ColorIds = Enumerable.Repeat(_colorPicker.Cp.SelectedColorId, Ark.ColorRegionCount).ToArray();
        }

        private void SetColorButton(Button bt, int region)
        {
            var colorId = _selectedRegionColorIds[region];
            bt.SetBackColorAndAccordingForeColor(CreatureColors.CreatureColor(colorId));
            if (VerboseButtonTexts)
                bt.Text = $"[{region}]: {colorId}";
            else if (Properties.Settings.Default.ShowColorIdOnRegionButtons)
                bt.Text = colorId.ToString();
            else bt.Text = region.ToString();
            // tooltip
            _tt.SetToolTip(bt, $"[{region}] {_colorRegions?[region]?.name}:\n{colorId}: {CreatureColors.CreatureColorName(colorId)}");
        }

        private void RegionColorChooser_Disposed(object sender, EventArgs e) => _tt?.RemoveAllAndDispose();

        /// <summary>
        /// True if a color is new in this species.
        /// </summary>
        internal bool ColorNewInSpecies;
        /// <summary>
        /// True if color is new in this region (but exists in other region in this species).
        /// </summary>
        internal bool ColorNewInRegion;

        internal void SetRegionColorsExisting(LevelColorStatusFlags.ColorStatus[] colorAlreadyAvailable)
        {
            ColorNewInRegion = false;
            ColorNewInSpecies = false;

            var parameter = LevelColorStatusFlags.ColorStatus.None;
            for (var ci = 0; ci < Ark.ColorRegionCount; ci++)
            {
                if (colorAlreadyAvailable != null)
                    parameter = colorAlreadyAvailable[ci];
                switch (parameter)
                {
                    case LevelColorStatusFlags.ColorStatus.NewColor:
                        _buttonColors[ci].ColorStatus = LevelColorStatusFlags.ColorStatus.NewColor;
                        ColorNewInSpecies = true;
                        break;
                    case LevelColorStatusFlags.ColorStatus.NewRegionColor:
                        _buttonColors[ci].ColorStatus = LevelColorStatusFlags.ColorStatus.NewRegionColor;
                        ColorNewInRegion = true;
                        break;
                    default:
                        _buttonColors[ci].ColorStatus = LevelColorStatusFlags.ColorStatus.ExistsInRegion;
                        break;
                }
                _buttonColors[ci].Invalidate();
            }
        }

        private class NoPaddingButton : Button
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public LevelColorStatusFlags.ColorStatus ColorStatus { get; set; }
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public bool AlternativeColorPossible { get; set; }

            public NoPaddingButton()
            {
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
            }

            protected override void OnPaint(PaintEventArgs pe)
            {
                Color statusColor;
                switch (ColorStatus)
                {
                    case LevelColorStatusFlags.ColorStatus.NewColor:
                        statusColor = UiColors.Current.NewColorInSpecies;
                        break;
                    case LevelColorStatusFlags.ColorStatus.NewRegionColor:
                        statusColor = UiColors.Current.NewColorInRegion;
                        break;
                    default:
                        statusColor = SystemColors.Control;
                        break;
                }

                using (var b = new SolidBrush(statusColor))
                {
                    var defaultVisibleRectangle = ClientRectangle;
                    var shrinkStatus = -4;
                    if (AlternativeColorPossible)
                    {
                        b.Color = UiColors.Current.ErrorText;
                        pe.Graphics.FillRectangle(b, defaultVisibleRectangle);
                        defaultVisibleRectangle.Inflate(-1, -1);
                        shrinkStatus = -3;
                    }
                    b.Color = statusColor;
                    pe.Graphics.FillRectangle(b, defaultVisibleRectangle);

                    b.Color = Color.Gray;
                    defaultVisibleRectangle.Inflate(shrinkStatus, shrinkStatus);
                    pe.Graphics.FillRectangle(b, defaultVisibleRectangle);
                    defaultVisibleRectangle.Inflate(-1, -1);
                    b.Color = BackColor;
                    pe.Graphics.FillRectangle(b, defaultVisibleRectangle);
                }

                if (string.IsNullOrEmpty(Text)) return;
                var stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                using (var b = new SolidBrush(ForeColor))
                    pe.Graphics.DrawString(Text, Font, b, ClientRectangle, stringFormat);
            }
        }

    }
}
