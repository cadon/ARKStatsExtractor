using ARKBreedingStats.species;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.uiControls
{
    public partial class RegionColorChooser : UserControl
    {
        public event Action RegionColorChosen;
        private readonly NoPaddingButton[] _buttonColors;
        private byte[] _selectedRegionColorIds;
        private byte[] _selectedColorIdsAlternative;
        public bool[] ColorRegionsUseds;
        private readonly MyColorPicker _colorPicker;
        private ColorRegion[] _colorRegions;
        private readonly ToolTip _tt = new ToolTip();
        /// <summary>
        /// If true, the button text will display the region and color id.
        /// </summary>
        public bool VerboseButtonTexts { get; set; }

        public RegionColorChooser()
        {
            InitializeComponent();

            _buttonColors = new NoPaddingButton[Ark.ColorRegionCount];
            for (int i = 0; i < Ark.ColorRegionCount; i++)
            {
                var b = new NoPaddingButton { Width = 27, Height = 27, Margin = new Padding(1), Text = i.ToString() };
                var ii = i;
                b.Click += (s, e) => ChooseColor(ii, b);
                _buttonColors[i] = b;
                flowLayoutPanel1.Controls.Add(b);
            }

            _selectedRegionColorIds = new byte[Ark.ColorRegionCount];
            _selectedColorIdsAlternative = new byte[Ark.ColorRegionCount];

            _colorPicker = new MyColorPicker();
            _tt.AutoPopDelay = 7000;
            Disposed += RegionColorChooser_Disposed;
        }

        public void SetOneButtonPerRow(bool onePerRow)
        {
            foreach (var b in _buttonColors)
                flowLayoutPanel1.SetFlowBreak(b, onePerRow);
        }

        public void SetSpecies(Species species, byte[] colorIDs)
        {
            _selectedRegionColorIds = colorIDs.ToArray();
            _selectedColorIdsAlternative = null;

            if (species?.colors != null)
            {
                _colorRegions = species.colors;
                ColorRegionsUseds = species.EnabledColorRegions;
            }
            else
            {
                // species-info is not available, show all region-buttons
                ColorRegionsUseds = new bool[Ark.ColorRegionCount];
                _colorRegions = new ColorRegion[Ark.ColorRegionCount];
                for (int i = 0; i < Ark.ColorRegionCount; i++)
                {
                    _colorRegions[i] = new ColorRegion();
                    ColorRegionsUseds[i] = true;
                }
            }

            for (int r = 0; r < _buttonColors.Length; r++)
            {
                _buttonColors[r].Visible = ColorRegionsUseds[r];

                if (ColorRegionsUseds[r])
                {
                    _buttonColors[r].AlternativeColorPossible = false;
                    SetColorButton(_buttonColors[r], r);
                }
            }
        }

        public byte[] ColorIds => _selectedRegionColorIds.ToArray();
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
                for (int i = 0; i < _buttonColors.Length; i++)
                    _buttonColors[i].AlternativeColorPossible = _selectedColorIdsAlternative.Length > i && _selectedColorIdsAlternative[i] != 0;
            }
        }

        public void Clear()
        {
            _selectedColorIdsAlternative = null;
            SetColorIds(new byte[_buttonColors.Length]);
        }

        /// <summary>
        /// Set colors to random ids of the available colors.
        /// </summary>
        internal void RandomColors()
        {
            _selectedColorIdsAlternative = null;
            SetColorIds(values.Values.V.Colors.GetRandomColors());
        }

        /// <summary>
        /// Set colors to random values in the set of natural occurring colors of the species.
        /// </summary>
        internal void RandomNaturalColors(Species species)
        {
            _selectedColorIdsAlternative = null;
            SetColorIds(species?.RandomSpeciesColors());
        }

        private void SetColorIds(byte[] colorIds)
        {
            if (colorIds == null)
            {
                Clear();
                return;
            }

            for (int r = 0; r < _buttonColors.Length; r++)
            {
                _selectedRegionColorIds[r] = colorIds.Length > r ? colorIds[r] : (byte)0;
                _buttonColors[r].AlternativeColorPossible = false;
                SetColorButton(_buttonColors[r], r);
            }
            RegionColorChosen?.Invoke();
        }

        private void ChooseColor(int region, Button sender)
        {
            if (!_colorPicker.isShown && _colorRegions != null && region < Ark.ColorRegionCount)
            {
                _colorPicker.PickColor(_selectedRegionColorIds[region], _colorRegions[region]?.name + " (region " + region + ")", _colorRegions[region]?.naturalColors, _selectedColorIdsAlternative?[region] ?? 0);
                if (_colorPicker.ShowDialog() == DialogResult.OK)
                {
                    // color was chosen
                    _selectedRegionColorIds[region] = _colorPicker.SelectedColorId;
                    if (_colorPicker.SelectedColorIdAlternative != 0)
                    {
                        if (_selectedColorIdsAlternative == null)
                            _selectedColorIdsAlternative = new byte[Ark.ColorRegionCount];
                        _selectedColorIdsAlternative[region] = _colorPicker.SelectedColorIdAlternative;
                        _buttonColors[region].AlternativeColorPossible = true;
                    }
                    else
                    {
                        _buttonColors[region].AlternativeColorPossible = false;
                        if (_selectedColorIdsAlternative != null)
                            _selectedColorIdsAlternative[region] = 0;
                    }
                    SetColorButton(sender, region);
                    RegionColorChosen?.Invoke();
                }
            }
        }

        private void SetColorButton(Button bt, int region)
        {
            byte colorId = _selectedRegionColorIds[region];
            bt.SetBackColorAndAccordingForeColor(CreatureColors.CreatureColor(colorId));
            if (VerboseButtonTexts)
                bt.Text = $"[{region}]: {colorId}";
            else if (Properties.Settings.Default.ShowColorIdOnRegionButtons)
                bt.Text = colorId.ToString();
            else bt.Text = region.ToString();
            // tooltip
            _tt.SetToolTip(bt, $"[{region}] {_colorRegions?[region]?.name}:\n{colorId}: {CreatureColors.CreatureColorName(colorId)}");
        }

        private void RegionColorChooser_Disposed(object sender, EventArgs e)
        {
            _tt.RemoveAll();
            _tt.Dispose();
        }

        /// <summary>
        /// True if a color is new in this species.
        /// </summary>
        internal bool ColorNewInSpecies;
        /// <summary>
        /// True if color is new in this region (but exists in other region in this species).
        /// </summary>
        internal bool ColorNewInRegion;

        internal void SetRegionColorsExisting(CreatureCollection.ColorExisting[] colorAlreadyAvailable)
        {
            ColorNewInRegion = false;
            ColorNewInSpecies = false;

            var parameter = CreatureCollection.ColorExisting.Unknown;
            for (int ci = 0; ci < Ark.ColorRegionCount; ci++)
            {
                if (colorAlreadyAvailable != null)
                    parameter = colorAlreadyAvailable[ci];
                switch (parameter)
                {
                    case CreatureCollection.ColorExisting.ColorIsNew:
                        _buttonColors[ci].ColorStatus = CreatureCollection.ColorExisting.ColorIsNew;
                        ColorNewInSpecies = true;
                        break;
                    case CreatureCollection.ColorExisting.ColorExistingInOtherRegion:
                        _buttonColors[ci].ColorStatus = CreatureCollection.ColorExisting.ColorExistingInOtherRegion;
                        ColorNewInRegion = true;
                        break;
                    default:
                        _buttonColors[ci].ColorStatus = CreatureCollection.ColorExisting.ColorExistingInRegion;
                        break;
                }
                _buttonColors[ci].Invalidate();
            }
        }

        private class NoPaddingButton : Button
        {
            public CreatureCollection.ColorExisting ColorStatus { get; set; }
            public bool AlternativeColorPossible { get; set; }

            protected override void OnPaint(PaintEventArgs pe)
            {
                Color statusColor;
                switch (ColorStatus)
                {
                    case CreatureCollection.ColorExisting.ColorIsNew:
                        statusColor = Color.Gold;
                        break;
                    case CreatureCollection.ColorExisting.ColorExistingInOtherRegion:
                        statusColor = Color.DarkGreen;
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
                        b.Color = Color.Red;
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
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                using (var b = new SolidBrush(ForeColor))
                    pe.Graphics.DrawString(Text, Font, b, ClientRectangle, stringFormat);
            }
        }

    }
}
