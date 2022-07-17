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
        public byte SelectedColorId;
        public byte SelectedColorIdAlternative;
        private byte[] _naturalColorIDs;
        public bool isShown;
        private readonly ToolTip _tt;
        /// <summary>
        /// Used to determine if new mods were loaded that could contain new color definitions.
        /// </summary>
        private int modListHash;

        public MyColorPicker()
        {
            InitializeComponent();
            _tt = new ToolTip { AutomaticDelay = 200 };

            BtNoColor.Tag = (byte)0; // id of no color
            BtNoColor.Text = Loc.S("noColor");
            LbAlternativeColor.Text = Loc.S("LbAlternativeColor");
            _tt.SetToolTip(BtNoColor, "0: no color");

            BtUndefinedColor.Tag = Ark.UndefinedColorId; // one possible id of undefined color, currently used by Ark
            _tt.SetToolTip(BtUndefinedColor, $"{Ark.UndefinedColorId}: undefined color");

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

        /// <summary>
        /// Clears color buttons. Call if color definitions changed, e.g. when a mod with colors is loaded or unloaded.
        /// </summary>
        private void ResetColors()
        {
            if (flowLayoutPanel1.Controls.Count == 0) return;
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                _tt.SetToolTip(c, null);
                c.Dispose();
            }
            flowLayoutPanel1.Controls.Clear();
        }

        public void PickColor(byte selectedColorId, string headerText, List<ArkColor> naturalColors = null, byte selectedColorIdAlternative = 0)
        {
            label1.Text = headerText;

            if (modListHash != values.Values.V.loadedModsHash)
                ResetColors();
            modListHash = values.Values.V.loadedModsHash;

            var colors = values.Values.V.Colors.ColorsList;

            SelectedColorId = selectedColorId;
            SelectedColorIdAlternative = selectedColorIdAlternative;
            _naturalColorIDs = naturalColors?.Select(ac => ac.Id).ToArray();
            checkBoxOnlyNatural.Visible = _naturalColorIDs != null;
            if (_naturalColorIDs == null)
                checkBoxOnlyNatural.Checked = true;

            flowLayoutPanel1.SuspendLayout();

            for (int colorIndex = 1; colorIndex < colors.Length; colorIndex++)
            {
                int controlIndex = colorIndex - 1;
                if (flowLayoutPanel1.Controls.Count <= controlIndex)
                {
                    var np = new NoPaddingButton
                    {
                        Width = 44,
                        Height = 24,
                        Margin = new Padding(0)
                    };
                    np.Click += ColorChosen;
                    flowLayoutPanel1.Controls.Add(np);
                }

                if (flowLayoutPanel1.Controls[controlIndex] is NoPaddingButton bt)
                {
                    bt.Visible = ColorVisible(colors[colorIndex].Id);
                    bt.Selected = SelectedColorId == colors[colorIndex].Id;
                    bt.SelectedAlternative = SelectedColorIdAlternative == colors[colorIndex].Id;
                    bt.SetBackColorAndAccordingForeColor(colors[colorIndex].Color);
                    bt.Tag = colors[colorIndex].Id;
                    bt.Text = colors[colorIndex].Id.ToString();
                    _tt.SetToolTip(bt, colors[colorIndex].Id + ": " + colors[colorIndex].Name);
                }
            }

            Height = (int)Math.Ceiling(colors.Length / 10d) * 24 + 99;
            flowLayoutPanel1.ResumeLayout();
            isShown = true;
        }

        private bool ColorVisible(byte id) => !checkBoxOnlyNatural.Checked || (_naturalColorIDs?.Contains(id) ?? true);

        /// <summary>
        /// Color was chosen and saved in the property SelectedColorId. Window then will be hidden.
        /// </summary>
        private void ColorChosen(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Control) != 0)
            {
                // only set alternative color
                SelectedColorIdAlternative = (byte)((Control)sender).Tag;

                foreach (var ct in flowLayoutPanel1.Controls)
                {
                    if (ct is NoPaddingButton bt)
                    {
                        var selectedColorIdAlternative = SelectedColorIdAlternative == (byte)bt.Tag;
                        if (bt.SelectedAlternative != selectedColorIdAlternative)
                        {
                            bt.SelectedAlternative = selectedColorIdAlternative;
                            bt.Invalidate();
                        }
                    }
                }

                return;
            }

            SelectedColorId = (byte)((Control)sender).Tag;
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
                flowLayoutPanel1.Controls[c].Visible = ColorVisible((byte)flowLayoutPanel1.Controls[c].Tag);
            flowLayoutPanel1.ResumeLayout();
        }

        private class NoPaddingButton : Button
        {
            public bool Selected { get; set; }
            public bool SelectedAlternative { get; set; }

            protected override void OnPaint(PaintEventArgs pe)
            {
                pe.Graphics.Clear(SystemColors.Control);

                var defaultVisibleRectangle = ClientRectangle;
                defaultVisibleRectangle.Inflate(-3, -3);
                using (var b = new SolidBrush(BackColor))
                    pe.Graphics.FillRectangle(b, defaultVisibleRectangle);

                if (Selected)
                {
                    using (var p = new Pen(Color.Black, 2))
                    {
                        defaultVisibleRectangle.Inflate(2, 2);
                        pe.Graphics.DrawRectangle(p, defaultVisibleRectangle);
                        p.Color = Color.White;
                        defaultVisibleRectangle.Inflate(-2, -2);
                        pe.Graphics.DrawRectangle(p, defaultVisibleRectangle);
                    }
                }
                else if (SelectedAlternative)
                {
                    using (var p = new Pen(Color.Red, 2))
                    {
                        defaultVisibleRectangle.Inflate(2, 2);
                        pe.Graphics.DrawRectangle(p, defaultVisibleRectangle);
                        p.Color = Color.White;
                        defaultVisibleRectangle.Inflate(-2, -2);
                        pe.Graphics.DrawRectangle(p, defaultVisibleRectangle);
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
