using ARKBreedingStats.library;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;
using ARKBreedingStats.values;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;

namespace ARKBreedingStats.uiControls
{
    internal class LibraryInfoControl : TableLayoutPanel
    {
        public readonly TableLayoutPanel TlpColorInfoText = new TableLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, MinimumSize = new Size(450, 300) };
        public readonly ListView LvColors = new ListView();
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
            this.AutoScroll = true;
            this.RowCount = 2;
            this.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            this.ColumnCount = 5;
            for (int i = 0; i < this.ColumnCount; i++)
                this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            this.Controls.Add(TlpColorInfoText, 0, 0);
            this.SetRowSpan(TlpColorInfoText, 2);

            const int buttonsTotalWidth = 850;
            const int buttonMargins = 6;
            // color region buttons
            var flpButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, Height = 103 };
            _colorRegionButtons = new Button[Ark.ColorRegionCount];
            for (int i = 0; i < Ark.ColorRegionCount; i++)
            {
                var bt = new Button
                {
                    Text = i.ToString(),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Tag = i,
                    Width = buttonsTotalWidth / 6 - buttonMargins,
                    Height = 70
                };
                _colorRegionButtons[i] = bt;
                bt.Click += ButtonRegionClick;
                flpButtons.Controls.Add(bt);
            }
            flpButtons.SetFlowBreak(_colorRegionButtons.Last(), true);

            var colorsButton = new Button
            {
                Text = Loc.S("Clear"),
                TextAlign = ContentAlignment.MiddleCenter,
                Width = buttonsTotalWidth / 4 - buttonMargins,
                Height = 25
            };
            colorsButton.Click += ButtonClearColorsClick;
            flpButtons.Controls.Add(colorsButton);

            colorsButton = new Button
            {
                Text = Loc.S("Random natural"),
                TextAlign = ContentAlignment.MiddleCenter,
                Width = buttonsTotalWidth / 4 - buttonMargins,
                Height = 25
            };
            colorsButton.Click += ButtonRandomNaturalColorsClick;
            flpButtons.Controls.Add(colorsButton);

            colorsButton = new Button
            {
                Text = Loc.S("Random library"),
                TextAlign = ContentAlignment.MiddleCenter,
                Width = buttonsTotalWidth / 4 - buttonMargins,
                Height = 25
            };
            _tt.SetToolTip(colorsButton, "Random colors available in the library");
            colorsButton.Click += ButtonRandomLibraryColorsClick;
            flpButtons.Controls.Add(colorsButton);

            colorsButton = new Button
            {
                Text = Loc.S("Random"),
                TextAlign = ContentAlignment.MiddleCenter,
                Width = buttonsTotalWidth / 4 - buttonMargins,
                Height = 25
            };
            colorsButton.Click += ButtonRandomColorsClick;
            flpButtons.Controls.Add(colorsButton);

            this.Controls.Add(flpButtons, 1, 0);
            this.SetColumnSpan(flpButtons, 2);
            _colorPicker = new ColorPickerControl(false);
            _colorPicker.DisableAlternativeColor();
            this.Controls.Add(_colorPicker, 1, 1);
            _colorPicker.UserMadeSelection += ColorPickerColorChosen;

            this.Controls.Add(_speciesPictureBox, 2, 1);
            this.SetRowSpan(_speciesPictureBox, 2);

            _speciesPictureBox.Click += _speciesPictureBoxClick;
            _tt.SetToolTip(_speciesPictureBox, "Click to copy image to the clipboard\nLeft click: plain image\nRight click: image with color info");

            LvColors.View = View.Details;
            LvColors.FullRowSelect = true;
            LvColors.ShowItemToolTips = true;
            LvColors.Columns.Add("Id", 28);
            LvColors.Columns[0].TextAlign = HorizontalAlignment.Right; // doesn't work
            for (var ci = 0; ci < Ark.ColorRegionCount; ci++)
                LvColors.Columns.Add($"{ci}", 20);
            Controls.Add(LvColors, 3, 0);
            SetRowSpan(LvColors, 2);
            LvColors.MinimumSize = new Size(152 + SystemInformation.VerticalScrollBarWidth, 0);
            LvColors.Dock = DockStyle.Right;
        }

        private void ButtonClearColorsClick(object sender, EventArgs e)
        {
            SetColors(new byte[Ark.ColorRegionCount]);
        }

        private void ButtonRandomNaturalColorsClick(object sender, EventArgs e)
        {
            SetColors(_species?.RandomSpeciesColors());
        }

        /// <summary>
        /// Set to random colors available in the library
        /// </summary>
        private void ButtonRandomLibraryColorsClick(object sender, EventArgs e)
        {
            var colorIds = new byte[Ark.ColorRegionCount];
            var rand = new Random();
            for (int ri = 0; ri < Ark.ColorRegionCount; ri++)
            {
                var colorsInRegion = LibraryInfo.ColorsExistPerRegion?[ri]?.ToArray();
                var colorsCountInRegion = colorsInRegion?.Length ?? 0;
                if (colorsCountInRegion == 0) continue;
                colorIds[ri] = colorsInRegion[rand.Next(colorsCountInRegion)];
            }
            SetColors(colorIds);
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
            _colorPicker.PickColor(_selectedColors[_selectedColorRegion],
                $"[{_selectedColorRegion}] {_species.colors?[_selectedColorRegion]?.name}",
                _species.colors?[_selectedColorRegion]?.naturalColors,
               existingColors: LibraryInfo.ColorsExistPerRegion?[_selectedColorRegion]);
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
            this.PerformLayout();
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
            _speciesPictureBox.SetImageAndDisposeOld(CreatureColored.GetColoredCreature(_selectedColors, _species, _species.EnabledColorRegions, ColoredCreatureSize,
                onlyImage: true, creatureSex: Sex.Male, game: CreatureCollection.CurrentCreatureCollection?.Game));
        }

        private void _speciesPictureBoxClick(object sender, EventArgs e)
        {
            if (_speciesPictureBox.Image == null) return;
            if (e is MouseEventArgs me && me.Button == MouseButtons.Right)
                Clipboard.SetImage(CreatureInfoGraphic.GetImageWithColors(_speciesPictureBox.Image, _selectedColors, _species));
            else
                Clipboard.SetImage(_speciesPictureBox.Image);
        }
    }
}
