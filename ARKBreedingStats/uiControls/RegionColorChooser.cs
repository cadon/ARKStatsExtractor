using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ARKBreedingStats.species;

namespace ARKBreedingStats.uiControls
{
    public partial class RegionColorChooser : UserControl
    {
        public delegate void RegionColorChosenEventHandler();
        public event RegionColorChosenEventHandler RegionColorChosen;
        private Button[] buttons;
        private int[] colorIDs;
        public bool[] ColorRegionsUseds;
        private MyColorPicker colorPicker;
        private List<ColorRegion> colorRegions;
        private ToolTip tt;

        public RegionColorChooser()
        {
            InitializeComponent();
            buttons = new Button[] { buttonColor0, buttonColor1, buttonColor2, buttonColor3, buttonColor4, buttonColor5 };
            colorIDs = new int[6];
            ColorRegionsUseds = new bool[6];
            colorPicker = new MyColorPicker();
            tt = new ToolTip();
            Disposed += RegionColorChooser_Disposed;
        }

        public void setCreature(string species, int[] colorIDs)
        {
            this.colorIDs = colorIDs;

            int si = Values.V.speciesNames.IndexOf(species);
            if (si >= 0 && Values.V.species[si].colors != null)
                colorRegions = Values.V.species[si].colors;
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
            for (int r = 0; r < buttons.Length; r++)
            {
                ColorRegionsUseds[r] = colorRegions[r].name != null;
                buttons[r].Visible = ColorRegionsUseds[r];

                if (buttons[r].Visible)
                {
                    setColorButton(buttons[r], CreatureColors.creatureColor(colorIDs[r]));
                    tt.SetToolTip(buttons[r], colorRegions[r].name);
                }
            }
        }

        public void Clear()
        {
            for (int r = 0; r < buttons.Length; r++)
                buttons[r].Visible = false;
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
            if (!colorPicker.isShown)
            {
                colorPicker.SetColors(colorIDs, region, colorRegions[region].name, colorRegions[region].colorIds);
                if (colorPicker.ShowDialog() == DialogResult.OK)
                {
                    // color was chosen
                    setColorButton(sender, species.CreatureColors.creatureColor(colorIDs[region]));
                    RegionColorChosen?.Invoke();
                }
            }
        }

        private void setColorButton(Button bt, Color cl)
        {
            bt.BackColor = cl;
            bt.ForeColor = Utils.ForeColor(cl);
        }

        private void RegionColorChooser_Disposed(object sender, EventArgs e)
        {
            tt.RemoveAll();
        }

    }
}
