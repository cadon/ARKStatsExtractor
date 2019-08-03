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
        private readonly List<Panel> panels = new List<Panel>();
        private int regionId;
        private int[] creatureColors;
        private List<int> naturalColorIDs;
        public bool isShown;
        private readonly ToolTip tt = new ToolTip();

        public MyColorPicker()
        {
            InitializeComponent();
        }

        public void SetColors(int[] creatureColors, int regionId, string name, List<ARKColor> naturalColors = null)
        {
            label1.Text = name;
            this.regionId = regionId;
            var colors = values.Values.V.Colors.colorsList;

            this.creatureColors = creatureColors;
            this.naturalColorIDs = naturalColors?.Select(ac => ac.id).ToList();
            SuspendLayout();
            // clear unused panels
            if (panels.Count - colors.Count > 0)
            {
                List<Panel> rm = panels.Skip(colors.Count).ToList();
                foreach (Panel p in rm)
                    p.Dispose();
                panels.RemoveRange(colors.Count, panels.Count - colors.Count);
            }

            for (int c = 0; c < colors.Count; c++)
            {
                if (panels.Count <= c)
                {
                    Panel p = new Panel
                    {
                        Width = 40,
                        Height = 20,
                        Location = new Point(5 + (c % 8) * 45, 25 + (c / 8) * 25)
                    };
                    p.Click += ColorChoosen;
                    panel1.Controls.Add(p);
                    panels.Add(p);
                }
                panels[c].BackColor = colors[c].color;
                panels[c].BorderStyle = (creatureColors[regionId] == c ? BorderStyle.Fixed3D : BorderStyle.None);
                panels[c].Visible = (!checkBoxOnlyNatural.Checked || naturalColorIDs == null || naturalColorIDs.Count == 0 || naturalColorIDs.Contains(c));
                tt.SetToolTip(panels[c], c + ": " + species.CreatureColors.creatureColorName(c));
            }
            ResumeLayout();
            isShown = true;
        }

        private void ColorChoosen(object sender, EventArgs e)
        {
            // store selected color-id in creature-array and close this window
            int i = panels.IndexOf((Panel)sender);
            if (i >= 0)
                creatureColors[regionId] = i;
            isShown = false;
            DialogResult = DialogResult.OK;
        }

        private void MyColorPicker_Load(object sender, EventArgs e)
        {
            int y = Cursor.Position.Y - Height;
            if (y < 20) y = 20;
            SetDesktopLocation(Cursor.Position.X - 20, y);
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            // mouse left, close
            if (!panel1.ClientRectangle.Contains(PointToClient(MousePosition)) || PointToClient(MousePosition).X == 0 || PointToClient(MousePosition).Y == 0)
            {
                isShown = false;
                DialogResult = DialogResult.Cancel;
            }
        }

        private void MyColorPicker_Leave(object sender, EventArgs e)
        {
            isShown = false;
            DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            isShown = false;
            DialogResult = DialogResult.Cancel;
        }

        private void checkBoxOnlyNatural_CheckedChanged(object sender, EventArgs e)
        {
            for (int c = 0; c < panels.Count; c++)
                panels[c].Visible = (!checkBoxOnlyNatural.Checked || naturalColorIDs == null || naturalColorIDs.Count == 0 || naturalColorIDs.Contains(c));
        }
    }
}
