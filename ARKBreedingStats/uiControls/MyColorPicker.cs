using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.uiControls
{
    public partial class MyColorPicker : Form
    {
        public int SelectedColorId;
        private List<int> _naturalColorIDs;
        public bool isShown;
        private readonly ToolTip _tt;

        public MyColorPicker()
        {
            InitializeComponent();
            _tt = new ToolTip { AutomaticDelay = 200 };

            BtNoColor.Tag = 0; // id of no color
            BtNoColor.Text = Loc.S("noColor");
            _tt.SetToolTip(BtNoColor, "0: no color");

            buttonCancel.Text = Loc.S("Cancel");

            Disposed += MyColorPicker_Disposed;

            checkBoxOnlyNatural.Text = Loc.S("showOnlyNaturalOccurring");

            TopMost = true;
        }

        private void MyColorPicker_Disposed(object sender, EventArgs e)
        {
            _tt.RemoveAll();
            _tt.Dispose();
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
                    var np = new NoPaddingButton
                    {
                        Width = 40,
                        Height = 20
                    };
                    np.Click += ColorChosen;
                    flowLayoutPanel1.Controls.Add(np);
                }

                if (flowLayoutPanel1.Controls[controlIndex] is NoPaddingButton bt)
                {
                    bt.Visible = ColorVisible(colors[colorIndex].Id);
                    bt.Selected = SelectedColorId == colors[colorIndex].Id;
                    bt.SetBackColorAndAccordingForeColor(colors[colorIndex].Color);
                    bt.Tag = colors[colorIndex].Id;
                    bt.Text = colors[colorIndex].Id.ToString();
                    _tt.SetToolTip(bt, colors[colorIndex].Id + ": " + colors[colorIndex].Name);
                }
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

        private class NoPaddingButton : Button
        {
            public bool Selected { get; set; }

            protected override void OnPaint(PaintEventArgs pe)
            {
                using (var b = new SolidBrush(BackColor))
                    pe.Graphics.FillRectangle(b, ClientRectangle);

                if (Selected)
                {
                    using (var p = new Pen(Color.Black, 2))
                    {
                        var rec = new Rectangle(ClientRectangle.Location, ClientRectangle.Size);
                        rec.Inflate(-1, -1);
                        pe.Graphics.DrawRectangle(p, rec);
                        p.Color = Color.White;
                        rec.Inflate(-2, -2);
                        pe.Graphics.DrawRectangle(p, rec);
                    }
                }

                if (string.IsNullOrEmpty(Text)) return;
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                using (var b = new SolidBrush(ForeColor))
                    pe.Graphics.DrawString(Text, Font, b, ClientRectangle, stringFormat);
            }
        }
    }
}
