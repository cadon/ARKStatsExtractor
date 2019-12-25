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
            Disposed += MyColorPicker_Disposed;
        }

        private void MyColorPicker_Disposed(object sender, EventArgs e)
        {
            tt.RemoveAll();
        }

        public void SetColors(int[] creatureColors, int regionId, string name, List<ARKColor> naturalColors = null)
        {
            label1.Text = name;
            this.regionId = regionId;
            var colors = values.Values.V.Colors.colorsList;

            this.creatureColors = creatureColors;
            this.naturalColorIDs = naturalColors?.Select(ac => ac.id).ToList();

            flowLayoutPanel1.SuspendLayout();

            for (int c = 0; c < colors.Count; c++)
            {
                if (flowLayoutPanel1.Controls.Count <= c)
                {
                    Panel np = new Panel
                    {
                        Width = 40,
                        Height = 20
                    };
                    np.Click += ColorChoosen;
                    flowLayoutPanel1.Controls.Add(np);
                }
                Panel p = flowLayoutPanel1.Controls[c] as Panel;
                p.BackColor = colors[c].color;
                p.Tag = colors[c].id;
                p.BorderStyle = creatureColors[regionId] == c ? BorderStyle.Fixed3D : BorderStyle.None;
                p.Visible = ColorPossible(colors[c].id);
                tt.SetToolTip(p, colors[c].id + ": " + colors[c].name);
            }

            flowLayoutPanel1.ResumeLayout();
            isShown = true;
        }

        private bool ColorPossible(int id) => !checkBoxOnlyNatural.Checked || naturalColorIDs == null || naturalColorIDs.Count == 0 || naturalColorIDs.Contains(id);

        private void ColorChoosen(object sender, EventArgs e)
        {
            // store selected color-id in creature-array and close this window
            int i = (int)((Panel)sender).Tag;
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
                flowLayoutPanel1.Controls[c].Visible = ColorPossible((int)flowLayoutPanel1.Controls[c].Tag);
            flowLayoutPanel1.ResumeLayout();
        }
    }
}
