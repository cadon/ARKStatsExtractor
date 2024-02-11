using System;
using System.Drawing;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;
using ARKBreedingStats.values;

namespace ARKBreedingStats.uiControls
{
    internal class LibraryInfoControl : UserControl
    {
        private readonly TableLayoutPanel _tlbMain = new TableLayoutPanel();
        public readonly TableLayoutPanel TlpColorInfoText = new TableLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, MinimumSize = new Size(450, 300) };
        private Button[] _colorRegionButtons;
        private ColorPickerControl _colorPicker;
        private Species _species;
        private readonly PictureBox _speciesPictureBox = new PictureBox
        {
            Width = ColoredCreatureSize,
            Height = ColoredCreatureSize,
            Margin = new Padding(10)
        };
        private readonly ToolTip _tt = new ToolTip();
        private byte[] _selectedColors;
        private int _selectedColorRegion;
        private const int ColoredCreatureSize = 384;

        public LibraryInfoControl()
        {
            InitializeControls();
        }

        private void InitializeControls()
        {
            _tlbMain.AutoScroll = true;
            _tlbMain.RowCount = 2;
            _tlbMain.ColumnCount = 3;
            for (int i = 0; i < _tlbMain.RowCount; i++)
                _tlbMain.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            for (int i = 0; i < _tlbMain.ColumnCount; i++)
                _tlbMain.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _tlbMain.Dock = DockStyle.Fill;
            Controls.Add(_tlbMain);

            _tlbMain.Controls.Add(TlpColorInfoText, 0, 0);
            _tlbMain.SetRowSpan(TlpColorInfoText, 2);

            // color region buttons
            var flpButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, Height = 180 };
            _colorRegionButtons = new Button[Ark.ColorRegionCount];
            for (int i = 0; i < Ark.ColorRegionCount; i++)
            {
                var bt = new Button
                {
                    Text = i.ToString(),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Tag = i,
                    Width = 142,
                    Height = 70
                };
                _colorRegionButtons[i] = bt;
                bt.Click += ButtonRegionClick;
                flpButtons.Controls.Add(bt);
            }

            var colorsButton = new Button
            {
                Text = Loc.S("Clear"),
                TextAlign = ContentAlignment.MiddleCenter,
                Width = 142,
                Height = 25
            };
            colorsButton.Click += ButtonClearColorsClick;
            flpButtons.Controls.Add(colorsButton);

            colorsButton = new Button
            {
                Text = Loc.S("Random natural"),
                TextAlign = ContentAlignment.MiddleCenter,
                Width = 142,
                Height = 25
            };
            colorsButton.Click += ButtonRandomNaturalColorsClick;
            flpButtons.Controls.Add(colorsButton);

            colorsButton = new Button
            {
                Text = Loc.S("Random"),
                TextAlign = ContentAlignment.MiddleCenter,
                Width = 142,
                Height = 25
            };
            colorsButton.Click += ButtonRandomColorsClick;
            flpButtons.Controls.Add(colorsButton);

            _tlbMain.Controls.Add(flpButtons, 1, 0);
            _colorPicker = new ColorPickerControl();
            _colorPicker.CbOnlyNatural.Checked = false;
            _colorPicker.DisableAlternativeColor();
            _tlbMain.Controls.Add(_colorPicker, 1, 1);
            _colorPicker.UserMadeSelection += ColorPickerColorChosen;

            _tlbMain.Controls.Add(_speciesPictureBox, 2, 0);
            _tlbMain.SetRowSpan(_speciesPictureBox, 2);
        }

        private void ButtonClearColorsClick(object sender, EventArgs e)
        {
            SetColors(new byte[Ark.ColorRegionCount]);
        }

        private void ButtonRandomNaturalColorsClick(object sender, EventArgs e)
        {
            SetColors(_species?.RandomSpeciesColors());
        }

        private void ButtonRandomColorsClick(object sender, EventArgs e)
        {
            SetColors(Values.V.Colors.GetRandomColors());
        }

        private void SetColors(byte[] colors)
        {
            if (_species == null) return;
            _selectedColors = colors;
            for (int i = 0; i < Ark.ColorRegionCount; i++)
                SetRegionColorButton(i);
            _colorRegionButtons[0].PerformClick();
            UpdateCreatureImage();
        }

        private void ColorPickerColorChosen(bool colorSelected)
        {
            if (!colorSelected) return;
            var newColor = _colorPicker.SelectedColorId;
            if (_selectedColors[_selectedColorRegion] == newColor) return;
            _selectedColors[_selectedColorRegion] = newColor;
            SetRegionColorButton(_selectedColorRegion);
            UpdateCreatureImage();
        }

        private void ButtonRegionClick(object sender, EventArgs e)
        {
            _selectedColorRegion = (int)((Button)sender).Tag;
            _colorPicker.PickColor(_selectedColors[_selectedColorRegion], $"[{_selectedColorRegion}] {_species.colors?[_selectedColorRegion]?.name}", _species.colors?[_selectedColorRegion]?.naturalColors);
        }

        public void SetSpecies(Species species)
        {
            if (_species == species) return;
            _species = species;
            _selectedColors = new byte[Ark.ColorRegionCount];
            for (int i = 0; i < Ark.ColorRegionCount; i++)
                SetRegionColorButton(i);
            _colorRegionButtons[0].PerformClick();
            UpdateCreatureImage();
        }

        public void SetRegionColorButton(int region)
        {
            var bt = _colorRegionButtons[region];
            var color = Values.V.Colors.ById(_selectedColors[region]);
            var buttonText = $"[{region}] {_species.colors?[region]?.name}\n{color.Id}: {color.Name}";
            bt.Text = buttonText;
            _tt.SetToolTip(_colorRegionButtons[region], buttonText);
            bt.SetBackColorAndAccordingForeColor(color.Color);
        }

        public void UpdateCreatureImage()
        {
            // todo button for gender
            _speciesPictureBox.SetImageAndDisposeOld(CreatureColored.GetColoredCreature(_selectedColors, _species, _species.EnabledColorRegions, ColoredCreatureSize, onlyImage: true, creatureSex: Sex.Male));
        }
    }
}
