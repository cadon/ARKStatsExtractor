using ARKBreedingStats.species;
using ARKBreedingStats.utils;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class RegionColorInfo : UserControl
    {
        public event Action<int> RegionColorChosen;

        /// <summary>
        /// Index 0: region names, 1: mother colors, 2: father colors
        /// </summary>
        private readonly Label[][] _labelsRegions = [new Label[Ark.ColorRegionCount], new Label[Ark.ColorRegionCount], new Label[Ark.ColorRegionCount]];
        private readonly Button[] _buttonsColor = new Button[Ark.ColorRegionCount];
        private readonly ColoredCreatureImageWithPose _creatureImageControl;
        private Species _species;
        private ColorPickerWindow _colorPicker;
        private ColorRegion[] _colorRegions = new ColorRegion[Ark.ColorRegionCount];
        private byte[] _colorIds = Enumerable.Repeat((byte)0, Ark.ColorRegionCount).ToArray();
        private readonly ToolTip _tt = new();

        public RegionColorInfo()
        {
            InitializeComponent();

            AddHeaderLabel(Loc.S("region"), 0);
            AddHeaderLabel(Loc.S("mother"), 1);
            AddHeaderLabel(Loc.S("desired"), 2);
            AddHeaderLabel(Loc.S("father"), 3);

            void AddHeaderLabel(string text, int col)
            {
                var l = new Label { Text = text, AutoSize = true, Padding = new Padding(UiUtils.UiLengthInt(3)) };
                tableLayoutPanel1.SetCellPosition(l, new TableLayoutPanelCellPosition(col, 0));
                tableLayoutPanel1.Controls.Add(l);
            }

            for (var i = 0; i < Ark.ColorRegionCount; i++)
            {
                var row = i + 1;
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                var b = new Button
                {
                    Width = UiUtils.UiLengthInt(40),
                    Height = UiUtils.UiLengthInt(25),
                    Tag = i,
                    FlatStyle = FlatStyle.Flat
                };
                _buttonsColor[i] = b;
                b.Click += BtColorClick;
                tableLayoutPanel1.SetCellPosition(b, new TableLayoutPanelCellPosition(2, row));
                tableLayoutPanel1.Controls.Add(b);

                AddNewLabel(0, 0); // region name
                AddNewLabel(1, 1); // mother color
                AddNewLabel(2, 3); // father color


                void AddNewLabel(int indexArray, int colTable)
                {
                    var l = new Label { AutoSize = true, Padding = new Padding(UiUtils.UiLengthInt(4)), Dock = DockStyle.Fill };
                    _labelsRegions[indexArray][i] = l;
                    tableLayoutPanel1.SetCellPosition(l, new TableLayoutPanelCellPosition(colTable, row));
                    tableLayoutPanel1.Controls.Add(l);
                }
            }

            _creatureImageControl = new ColoredCreatureImageWithPose(UiUtils.UiLengthInt(256), _tt);
            tableLayoutPanel1.SetColumnSpan(_creatureImageControl, 4);
            tableLayoutPanel1.SetCellPosition(_creatureImageControl, new TableLayoutPanelCellPosition(0, Ark.ColorRegionCount + 1));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.Controls.Add(_creatureImageControl);

#if DEBUG
            if (DesignMode) return;
#endif
            _colorPicker = new ColorPickerWindow();
        }

        private void BtColorClick(object sender, EventArgs e)
        {
            if (sender is not Button { Tag: int region } bt) return;

            if (_colorPicker.isShown || _colorRegions == null || region >= Ark.ColorRegionCount) return;

            _colorPicker.Cp.PickColor(_colorIds[region], _colorRegions[region]?.name + " (region " + region + ")", _colorRegions[region]?.naturalColors);
            if (_colorPicker.ShowDialog() != DialogResult.OK) return;

            // color was chosen
            _colorIds[region] = _colorPicker.Cp.SelectedColorId;

            SetColorButton(bt, region);
            UpdateImage();
            RegionColorChosen?.Invoke(region);
        }

        private void UpdateImage() => _creatureImageControl.SetCreatureImage(_species, _colorIds);

        private void SetColorButton(Button bt, int region)
        {
            var colorId = _colorIds[region];
            bt.SetBackColorAndAccordingForeColor(CreatureColors.CreatureColor(colorId));
            bt.Text = colorId.ToString();
            // tooltip
            _tt.SetToolTip(bt, $"[{region}] {_colorRegions?[region]?.name}:\n{colorId}: {CreatureColors.CreatureColorName(colorId)}");
        }

        public void SetSpecies(Species species)
        {
            if (_species == species) return;
            _species = species;
            _colorRegions = species?.colors;
            UpdateImage();
            for (var i = 0; i < Ark.ColorRegionCount; i++)
                _labelsRegions[0][i].Text = $"[{i}] {species?.colors?[i]?.name ?? string.Empty}";
        }

        public void SetPair(BreedingPair pair)
        {
            if (pair == null)
            {
                Clear();
                return;
            }

            var fontRegular = new Font(_labelsRegions[0][0].Font, FontStyle.Regular);
            var fontBold = new Font(_labelsRegions[0][0].Font, FontStyle.Bold);

            for (var i = 0; i < Ark.ColorRegionCount; i++)
            {
                if (pair.Mother?.colors == null)
                {
                    _labelsRegions[1][i].Text = string.Empty;
                    _labelsRegions[1][i].Font = fontRegular;
                }
                else
                {
                    var color = CreatureColors.CreatureArkColor(pair.Mother.colors[i]);
                    _labelsRegions[1][i].Text = $"{color.Id}: {color.Name}";
                    _labelsRegions[1][i].SetBackColorAndAccordingForeColor(color.Color);
                    _labelsRegions[1][i].Font = pair.Mother.colors[i] == _colorIds[i] ? fontBold : fontRegular;
                }

                if (pair.Father?.colors == null)
                {
                    _labelsRegions[2][i].Text = string.Empty;
                    _labelsRegions[2][i].Font = fontRegular;
                }
                else
                {
                    var color = CreatureColors.CreatureArkColor(pair.Father.colors[i]);
                    _labelsRegions[2][i].Text = $"{color.Id}: {color.Name}";
                    _labelsRegions[2][i].SetBackColorAndAccordingForeColor(color.Color);
                    _labelsRegions[2][i].Font = pair.Father.colors[i] == _colorIds[i] ? fontBold : fontRegular;
                }
            }
        }

        public void Clear()
        {
            for (var i = 0; i < Ark.ColorRegionCount; i++)
            {
                _labelsRegions[0][i].Text = $"[{i}]";
                _labelsRegions[1][i].Text = string.Empty;
                _labelsRegions[2][i].Text = string.Empty;
            }
        }
    }
}
