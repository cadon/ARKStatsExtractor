using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class MyColorPicker : Form
    {
        private int regionId;
        private int[] creatureColors;
        private List<int> naturalColorIDs;
        public bool isShown;
        private readonly ToolTip tt;

        public MyColorPicker()
        {
            InitializeComponent();
            tt = new ToolTip { AutomaticDelay = 200 };

            BtUnknownColor.Tag = 0; // id of unknown color
            BtUnknownColor.Text = Loc.s("Unknown");

            buttonCancel.Text = Loc.s("Cancel");

            Disposed += MyColorPicker_Disposed;

            checkBoxOnlyNatural.Text = Loc.s("showOnlyNaturalOccuring");
        }

        private void MyColorPicker_Disposed(object sender, EventArgs e)
        {
            tt.RemoveAll();
        }

        public void SetColors(int[] creatureColors, int regionId, string regionName, List<ARKColor> naturalColors = null)
        {
            label1.Text = regionName;
            this.regionId = regionId;
            var colors = values.Values.V.Colors.colorsList;

            this.creatureColors = creatureColors;
            this.naturalColorIDs = naturalColors?.Select(ac => ac.id).ToList();

            flowLayoutPanel1.SuspendLayout();

            for (int colorIndex = 1; colorIndex < colors.Count; colorIndex++)
            {
                int controlIndex = colorIndex - 1;
                if (flowLayoutPanel1.Controls.Count <= controlIndex)
                {
                    Panel np = new Panel
                    {
                        Width = 40,
                        Height = 20
                    };
                    np.Click += ColorChosen;
                    flowLayoutPanel1.Controls.Add(np);
                }
                Panel p = flowLayoutPanel1.Controls[controlIndex] as Panel;
                p.BackColor = colors[colorIndex].color;
                p.Tag = colors[colorIndex].id;
                p.BorderStyle = creatureColors[regionId] == colors[colorIndex].id ? BorderStyle.Fixed3D : BorderStyle.None;
                p.Visible = ColorVisible(colors[colorIndex].id);
                tt.SetToolTip(p, colors[colorIndex].id + ": " + colors[colorIndex].name);
            }

            flowLayoutPanel1.ResumeLayout();
            isShown = true;
        }

        private bool ColorVisible(int id) => id == 0 || !checkBoxOnlyNatural.Checked || (naturalColorIDs?.Contains(id) ?? false);

        private void ColorChosen(object sender, EventArgs e)
        {
            // store selected color-id in creature-array and close this window
            int i = (int)((Control)sender).Tag;
            if (i >= 0)
                creatureColors[regionId] = i;
            HideWindow(true);
        }

        private void MyColorPicker_Load(object sender, EventArgs e)
        {
            int y = Cursor.Position.Y - Height;
            if (y < 20) y = 20;
            SetDesktopLocation(Cursor.Position.X - 20, y);
        }

        private void MyColorPicker_MouseLeave(object sender, EventArgs e)
        {
            // mouse left, close
            if (!ClientRectangle.Contains(PointToClient(MousePosition)) || PointToClient(MousePosition).X == 0 || PointToClient(MousePosition).Y == 0)
            {
                HideWindow(false);
            }
        }

        private void MyColorPicker_Leave(object sender, EventArgs e)
        {
            HideWindow(false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HideWindow(false);
        }

        private void HideWindow(bool ok)
        {
            isShown = false;
            DialogResult = ok ? DialogResult.OK : DialogResult.Cancel;
        }

        private void checkBoxOnlyNatural_CheckedChanged(object sender, EventArgs e)
        {
            flowLayoutPanel1.SuspendLayout();
            for (int c = 0; c < flowLayoutPanel1.Controls.Count; c++)
                flowLayoutPanel1.Controls[c].Visible = ColorVisible((int)flowLayoutPanel1.Controls[c].Tag);
            flowLayoutPanel1.ResumeLayout();
        }
    }
}
