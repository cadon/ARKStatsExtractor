using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class MyColorPicker : Form
    {
        private List<Panel> panels = new List<Panel>();
        private int regionId;
        private int[] creatureColors;
        private int[] colorIds;
        private List<int> naturalIds;
        public bool isShown;
        private ToolTip tt = new ToolTip();

        public MyColorPicker()
        {
            InitializeComponent();
        }

        public void SetColors(int[] creatureColors, int regionId, string name, List<int> naturalIds = null)
        {
            label1.Text = name;
            this.regionId = regionId;
            this.colorIds = new int[57];
            for (int c = 0; c < colorIds.Length; c++)
                colorIds[c] = c;
            this.creatureColors = creatureColors;
            this.naturalIds = naturalIds;
            SuspendLayout();
            // clear unused panels
            if (panels.Count - colorIds.Length > 0)
            {
                List<Panel> rm = panels.Skip(colorIds.Length).ToList();
                foreach (Panel p in rm)
                    p.Dispose();
                panels.RemoveRange(colorIds.Length, panels.Count - colorIds.Length);
            }

            for (int c = 0; c < colorIds.Length; c++)
            {
                if (panels.Count <= c)
                {
                    Panel p = new Panel();
                    p.Width = 40;
                    p.Height = 20;
                    p.Location = new Point(5 + (c % 8) * 45, 25 + (c / 8) * 25);
                    p.Click += new System.EventHandler(this.ColorChoosen);
                    panel1.Controls.Add(p);
                    panels.Add(p);
                }
                panels[c].BackColor = species.CreatureColors.creatureColor(colorIds[c]);
                panels[c].BorderStyle = (creatureColors[regionId] == colorIds[c] ? BorderStyle.Fixed3D : BorderStyle.None);
                panels[c].Visible = (!checkBoxOnlyNatural.Checked || naturalIds == null || naturalIds.Count == 0 || naturalIds.IndexOf(c) >= 0);
                tt.SetToolTip(panels[c], c.ToString() + ": " + species.CreatureColors.creatureColorName(colorIds[c]));
            }
            ResumeLayout();
            isShown = true;
        }

        private void ColorChoosen(object sender, EventArgs e)
        {
            // store selected color-id in creature-array and close this window
            int i = panels.IndexOf((Panel)sender);
            if (i >= 0)
                creatureColors[regionId] = colorIds[i];
            isShown = false;
            DialogResult = DialogResult.OK;
        }

        private void MyColorPicker_Load(object sender, EventArgs e)
        {
            SetDesktopLocation(Cursor.Position.X - 20, Cursor.Position.Y - 20);
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
                panels[c].Visible = (!checkBoxOnlyNatural.Checked || naturalIds == null || naturalIds.Count == 0 || naturalIds.IndexOf(c) >= 0);
        }
    }
}
