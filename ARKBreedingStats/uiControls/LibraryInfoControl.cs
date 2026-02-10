using ARKBreedingStats.library;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Input;
using ARKBreedingStats.SpeciesImages;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

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

        private readonly Label _lbPose = new Label();
        private Sex _sex = Sex.Male;
        private readonly ToolTip _tt = new ToolTip();
        public byte[] SelectedColors { get; private set; }
        private int _selectedColorRegion;
        private const int ColoredCreatureSize = 384;
        public readonly HashSet<Species> SpeciesChangedPoses = new HashSet<Species>();

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
            this.ColumnCount = 4;
            this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            this.Controls.Add(TlpColorInfoText, 0, 0);
            this.SetRowSpan(TlpColorInfoText, 2);

            const int buttonsTotalWidth = 850;
            const int buttonMargins = 6;
            // color region buttons
            var flpButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, Height = 103 };
            _colorRegionButtons = new Button[Ark.ColorRegionCount];
            for (var i = 0; i < Ark.ColorRegionCount; i++)
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

            Button AllRegionButton(string text) => new Button
            {
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = buttonsTotalWidth / 7 - buttonMargins,
                Height = 25
            };

            var colorsButton = AllRegionButton(Loc.S("Clear"));
            colorsButton.Click += (s, e) => SetColors(new byte[Ark.ColorRegionCount]);
            flpButtons.Controls.Add(colorsButton);

            var btAll = AllRegionButton("choose for all regions");
            btAll.Tag = -1;
            btAll.Click += ButtonRegionClick;
            flpButtons.Controls.Add(btAll);

            colorsButton = AllRegionButton(Loc.S("Random natural"));
            colorsButton.Click += (s, e) => SetColors(_species?.RandomSpeciesColors());
            flpButtons.Controls.Add(colorsButton);

            colorsButton = AllRegionButton(Loc.S("Random library"));
            _tt.SetToolTip(colorsButton, "Random colors available in the library");
            colorsButton.Click += ButtonRandomLibraryColorsClick;
            flpButtons.Controls.Add(colorsButton);

            colorsButton = AllRegionButton(Loc.S("Random"));
            colorsButton.Click += (s, e) => SetColors(Values.V.Colors.GetRandomColors());
            flpButtons.Controls.Add(colorsButton);

            colorsButton = AllRegionButton("1–6");
            _tt.SetToolTip(colorsButton, "Sets region 0 to color id 1, region 1 to color id 2, etc. i.e. resulting in color ids [1,2,3,4,5,6], which is RBGYCM\nHold Ctrl to set it to [1,3,2,5,4,6] which is RGBCYM (this is used for the color masks)");
            colorsButton.Click += (s, e) =>
            {
                SetColors(Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control)
                    ? new byte[] { 1, 3, 2, 5, 4, 6 }
                    : Enumerable.Range(1, Ark.ColorRegionCount).Select(i => (byte)i).ToArray());
            };
            flpButtons.Controls.Add(colorsButton);

            colorsButton = AllRegionButton("Parse Clipboard");
            _tt.SetToolTip(colorsButton, "Uses the color ids of a console command in the clipboard, e.g. setTargetDinoColor 4 54.\nHold Ctrl to only set recognized regions, else unspecified regions will be set to 0");
            colorsButton.Click += (s, e) => ParseClipboardColors();
            flpButtons.Controls.Add(colorsButton);

            this.Controls.Add(flpButtons, 1, 0);
            this.SetColumnSpan(flpButtons, 2);
            _colorPicker = new ColorPickerControl(false);
            _colorPicker.DisableAlternativeColor();
            this.Controls.Add(_colorPicker, 1, 1);
            _colorPicker.UserMadeSelection += ColorPickerColorChosen;

            AddPictureBox();

            _speciesPictureBox.Click += _speciesPictureBoxClick;
            _tt.SetToolTip(_speciesPictureBox, "Click to copy image to the clipboard\nLeft click: plain image\nRight click: image with color info");

            LvColors.View = View.Details;
            LvColors.FullRowSelect = true;
            LvColors.ShowItemToolTips = true;
            LvColors.Columns.Add("Id", 28);
            // right align in first column only possible with custom drawing
            LvColors.OwnerDraw = true;
            LvColors.DrawSubItem += LvColors_DrawSubItem;
            LvColors.DrawColumnHeader += LvColors_DrawColumnHeader;
            for (var ci = 0; ci < Ark.ColorRegionCount; ci++)
                LvColors.Columns.Add($"{ci}", 20);
            Controls.Add(LvColors, 3, 0);
            SetRowSpan(LvColors, 2);
            LvColors.MinimumSize = new Size(152 + SystemInformation.VerticalScrollBarWidth, 0);
            LvColors.Dock = DockStyle.Right;
        }

        private static readonly Regex reConsoleColorCommand = new Regex(@"setTargetDinoColor (\d+) (\d+)");

        private void ParseClipboardColors()
        {
            var clipboardText = Clipboard.GetText();
            if (string.IsNullOrEmpty(clipboardText)) return;
            var matches = reConsoleColorCommand.Matches(clipboardText);
            if (matches.Count == 0) return;
            var colorIds = Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control) ? SelectedColors.ToArray() : new byte[Ark.ColorRegionCount];
            foreach (Match m in matches)
            {
                var region = int.Parse(m.Groups[1].Value);
                var colorId = (byte)int.Parse(m.Groups[2].Value);
                if (region >= 0 && region < Ark.ColorRegionCount)
                    colorIds[region] = colorId;
            }
            SetColors(colorIds);
        }

        private void LvColors_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            // custom drawing is needed because the first column cannot be right aligned else
            e.DrawBackground();
            var flags = e.ColumnIndex == 0 ? TextFormatFlags.Right : TextFormatFlags.HorizontalCenter;
            // e.DrawText() uses different bounds (too large), so use custom
            TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.SubItem.Font, e.Bounds, e.SubItem.ForeColor, flags);
        }

        private void LvColors_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e) => e.DrawDefault = true;

        private void AddPictureBox()
        {
            var tlp = new TableLayoutPanel();
            tlp.AutoSize = true;
            tlp.RowCount = 2;
            tlp.ColumnCount = 4;
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            tlp.Controls.Add(_speciesPictureBox);
            tlp.SetColumnSpan(_speciesPictureBox, 4);

            var bt = new Button { Text = Utils.SexSymbol(_sex) };
            bt.Dock = DockStyle.Left;
            tlp.Controls.Add(bt, 0, 1);
            bt.Click += ChangeSex;
            bt = new Button { Text = "←" };
            bt.Dock = DockStyle.Left;
            tlp.Controls.Add(bt, 1, 1);
            bt.Click += BtPosePreviousClick;
            bt = new Button { Text = "→" };
            bt.Dock = DockStyle.Right;
            tlp.Controls.Add(bt, 3, 1);
            bt.Click += BtPoseNextClick;

            _lbPose.Dock = DockStyle.Fill;
            _lbPose.TextAlign = ContentAlignment.MiddleCenter;
            tlp.Controls.Add(_lbPose, 2, 1);

            _tt.SetToolTip(_lbPose, "Some species may have more than one pose, this can be set here.");

            this.Controls.Add(tlp, 2, 1);
        }

        private void ChangeSex(object sender, EventArgs e)
        {
            _sex = Utils.NextSex(_sex, false);
            ((Button)sender).Text = Utils.SexSymbol(_sex);
            UpdateCreatureImage();
        }

        private void BtPosePreviousClick(object sender, EventArgs e)
        {
            var delta = Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Shift) ? 5 : 1;
            var previouslySelectedPose = Poses.GetPose(_species);
            var previousPose = Math.Max(0, previouslySelectedPose - delta);
            if (previousPose == previouslySelectedPose) return;

            Poses.SetPose(_species, previousPose);
            UpdateCreatureImage();
            SpeciesChangedPoses.Add(_species);
        }

        private void BtPoseNextClick(object sender, EventArgs e)
        {
            var delta = Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Shift) ? 5 : 1;
            Poses.SetPose(_species, Poses.GetPose(_species) + delta);
            UpdateCreatureImage();
            SpeciesChangedPoses.Add(_species);
        }

        /// <summary>
        /// Set to random colors available in the library
        /// </summary>
        private void ButtonRandomLibraryColorsClick(object sender, EventArgs e)
        {
            var colorIds = new byte[Ark.ColorRegionCount];
            var rand = new Random();
            for (var ri = 0; ri < Ark.ColorRegionCount; ri++)
            {
                var colorsInRegion = LibraryInfo.ColorsExistPerRegion?[ri]?.ToArray();
                var colorsCountInRegion = colorsInRegion?.Length ?? 0;
                if (colorsInRegion != null && colorsCountInRegion > 0)
                    colorIds[ri] = colorsInRegion[rand.Next(colorsCountInRegion)];
            }
            SetColors(colorIds);
        }

        public void SetColors(byte[] colors = null)
        {
            if (_species == null) return;
            SelectedColors = colors ?? new byte[Ark.ColorRegionCount];
            for (var i = 0; i < Ark.ColorRegionCount; i++)
                SetRegionColorButton(i);
            _colorRegionButtons[0].PerformClick();
            UpdateCreatureImage();
        }

        private void ColorPickerColorChosen(bool colorSelected)
        {
            if (!colorSelected) return;
            var newColor = _colorPicker.SelectedColorId;
            if (_selectedColorRegion >= 0)
            {
                if (SelectedColors[_selectedColorRegion] == newColor) return;
                SelectedColors[_selectedColorRegion] = newColor;
            }
            else
            {
                if (SelectedColors.All(ci => ci == newColor)) return;
                SelectedColors = Enumerable.Repeat(newColor, Ark.ColorRegionCount).ToArray();
            }
            SetRegionColorButton(_selectedColorRegion);
            UpdateCreatureImage();
        }

        private void ButtonRegionClick(object sender, EventArgs e)
        {
            _selectedColorRegion = (int)((Button)sender).Tag;
            if (_selectedColorRegion >= 0)
                _colorPicker.PickColor(SelectedColors[_selectedColorRegion],
                    $"[{_selectedColorRegion}] {_species.colors?[_selectedColorRegion]?.name}",
                    _species.colors?[_selectedColorRegion]?.naturalColors,
                   existingColors: LibraryInfo.ColorsExistPerRegion?[_selectedColorRegion]);
            else
                _colorPicker.PickColor(SelectedColors[0],
                    "all regions");
        }

        public void SetSpecies(Species species, bool clearColors = true)
        {
            if (_species == species) return;
            _species = species;
            if (clearColors)
                SetColors();
        }

        public void SetRegionColorButton(int region)
        {
            if (region < 0)
            {
                for (var ci = 0; ci < Ark.ColorRegionCount; ci++)
                    SetRegionColorButton(ci);
                return;
            }
            var bt = _colorRegionButtons[region];
            var color = Values.V.Colors.ById(SelectedColors[region]);
            var buttonText = $"[{region}] {_species.colors?[region]?.name}\n{color.Id}: {color.Name}";
            bt.Text = buttonText;
            _tt.SetToolTip(_colorRegionButtons[region], buttonText);
            bt.SetBackColorAndAccordingForeColor(color.Color);
        }

        public void UpdateCreatureImage()
        {
            CreatureColored.GetColoredCreatureWithCallback(SetImage, this,
                SelectedColors, _species, _species.EnabledColorRegions, ColoredCreatureSize,
                onlyImage: true, creatureSex: _sex, game: CreatureCollection.CurrentCreatureCollection?.Game);
        }

        private void SetImage(Bitmap bmp)
        {
            _speciesPictureBox.SetImageAndDisposeOld(bmp);
            _lbPose.Text = $"Pose: {Poses.GetPose(_species)}";
        }

        private void _speciesPictureBoxClick(object sender, EventArgs e)
        {
            if (_speciesPictureBox.Image == null) return;
            if (e is MouseEventArgs me && me.Button == MouseButtons.Right)
                ClipboardHandler.SetImageWithAlphaToClipboard(CreatureInfoGraphic.GetImageWithColors(_speciesPictureBox.Image, SelectedColors, _species));
            else
                ClipboardHandler.SetImageWithAlphaToClipboard(_speciesPictureBox.Image, false);
        }
    }
}
