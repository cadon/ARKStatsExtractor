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
        public int SelectedColorId;
        private List<int> _naturalColorIDs;
        public bool isShown;
        private readonly ToolTip tt;

        public MyColorPicker()
        {
            InitializeComponent();
            tt = new ToolTip { AutomaticDelay = 200 };

            BtNoColor.Tag = 0; // id of no color
            BtNoColor.Text = Loc.S("noColor");

            buttonCancel.Text = Loc.S("Cancel");

            Disposed += MyColorPicker_Disposed;

            checkBoxOnlyNatural.Text = Loc.S("showOnlyNaturalOccuring");

            TopMost = true;
        }

        private void MyColorPicker_Disposed(object sender, EventArgs e)
        {
            tt.RemoveAll();
        }

        public void SetColors(int selectedColorId, string regionName, List<ArkColor> naturalColors = null)
        {
            label1.Text = regionName;
            var colors = values.Values.V.Colors.colorsList;

            SelectedColorId = selectedColorId;
            _naturalColorIDs = naturalColors?.Select(ac => ac.Id).ToList();
            checkBoxOnlyNatural.Visible = _naturalColorIDs != null;
            if (_naturalColorIDs == null)
                checkBoxOnlyNatural.Checked = true;

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
                p.BackColor = colors[colorIndex].Color;
                p.Tag = colors[colorIndex].Id;
                p.BorderStyle = SelectedColorId == colors[colorIndex].Id ? BorderStyle.Fixed3D : BorderStyle.None;
                p.Visible = ColorVisible(colors[colorIndex].Id);
                tt.SetToolTip(p, colors[colorIndex].Id + ": " + colors[colorIndex].Name);
            }

            flowLayoutPanel1.ResumeLayout();
            isShown = true;
        }

        private bool ColorVisible(int id) => !checkBoxOnlyNatural.Checked || (_naturalColorIDs?.Contains(id) ?? true);

        /// <summary>
        /// Color was chosen and saved in the property SelectedColorId. Window then will be hidden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorChosen(object sender, EventArgs e)
        {
            SelectedColorId = (int)((Control)sender).Tag;
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
