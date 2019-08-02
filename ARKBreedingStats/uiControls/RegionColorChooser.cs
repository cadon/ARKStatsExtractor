using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.species;

namespace ARKBreedingStats.uiControls
{
    public partial class RegionColorChooser : UserControl
    {
        public delegate void RegionColorChosenEventHandler();

        public event RegionColorChosenEventHandler RegionColorChosen;
        private readonly Button[] buttonColors;
        private int[] _colorIDs;
        public readonly bool[] ColorRegionsUseds;
        private readonly MyColorPicker colorPicker;
        private List<ColorRegion> colorRegions;
        private readonly ToolTip tt = new ToolTip();

        public RegionColorChooser()
        {
            InitializeComponent();
            buttonColors = new[] { buttonColor0, buttonColor1, buttonColor2, buttonColor3, buttonColor4, buttonColor5 };
            _colorIDs = new int[6];
            ColorRegionsUseds = new bool[6];
            colorPicker = new MyColorPicker();
            tt.AutoPopDelay = 7000;
            Disposed += RegionColorChooser_Disposed;
        }

        public void SetSpecies(Species species, int[] colorIDs)
        {
            _colorIDs = colorIDs.ToArray();

            if (species?.colors != null)
                colorRegions = species.colors;
            else
            {
                // species-info is not available, show all region-buttons
                colorRegions = new List<ColorRegion>();
                for (int i = 0; i < 6; i++)
                {
                    colorRegions.Add(new ColorRegion());
                    colorRegions[i].name = "n/a";
                }
            }
            for (int r = 0; r < buttonColors.Length; r++)
            {
                ColorRegionsUseds[r] = !string.IsNullOrEmpty(colorRegions[r]?.name);
                buttonColors[r].Visible = ColorRegionsUseds[r];

                if (ColorRegionsUseds[r])
                {
                    setColorButton(buttonColors[r], r);
                }
            }
        }

        public int[] colorIDs => _colorIDs.ToArray();

        public void Clear()
        {
            for (int r = 0; r < buttonColors.Length; r++)
            {
                _colorIDs[r] = 0;
                setColorButton(buttonColors[r], r);
            }
        }

        private void buttonColor0_Click(object sender, EventArgs e)
        {
            chooseColor(0, buttonColor0);
        }

        private void buttonColor1_Click(object sender, EventArgs e)
        {
            chooseColor(1, buttonColor1);
        }

        private void buttonColor2_Click(object sender, EventArgs e)
        {
            chooseColor(2, buttonColor2);
        }

        private void buttonColor3_Click(object sender, EventArgs e)
        {
            chooseColor(3, buttonColor3);
        }

        private void buttonColor4_Click(object sender, EventArgs e)
        {
            chooseColor(4, buttonColor4);
        }

        private void buttonColor5_Click(object sender, EventArgs e)
        {
            chooseColor(5, buttonColor5);
        }

        private void chooseColor(int region, Button sender)
        {
            if (!colorPicker.isShown && colorRegions != null)
            {
                colorPicker.SetColors(_colorIDs, region, colorRegions[region].name, colorRegions[region].colorIds);
                if (colorPicker.ShowDialog() == DialogResult.OK)
                {
                    // color was chosen
                    setColorButton(sender, region);
                    RegionColorChosen?.Invoke();
                }
            }
        }

        private void setColorButton(Button bt, int region)
        {
            int colorId = _colorIDs[region];
            Color cl = CreatureColors.creatureColor(colorId);
            bt.BackColor = cl;
            bt.ForeColor = Utils.ForeColor(cl);
            // tooltip
            if (colorRegions != null)
                tt.SetToolTip(bt, $"{colorRegions[region].name} ({region}):\n{CreatureColors.creatureColorName(colorId)} ({colorId})");
        }

        private void RegionColorChooser_Disposed(object sender, EventArgs e)
        {
            tt.RemoveAll();
        }
    }
}
