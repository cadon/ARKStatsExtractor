using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.uiControls
{
    public partial class ColorPickerControl : UserControl
    {
        public byte SelectedColorId;
        public byte SelectedColorIdAlternative;
        private byte[] _naturalColorIDs;
        private NoPaddingButton _buttonSelectedColor;
        /// <summary>
        /// Window if the control is shown in a separate window.
        /// </summary>
        public ColorPickerWindow Window;
        private readonly ToolTip _tt;
        /// <summary>
        /// Used to determine if new mods were loaded that could contain new color definitions.
        /// </summary>
        private int modListHash;

        public event Action<bool> UserMadeSelection;
        public event Action<int> HeightChanged;

        public ColorPickerControl()
        {
            InitializeComponent();
            _tt = new ToolTip { AutomaticDelay = 200 };

            BtNoColor.Tag = (byte)0; // id of no color
            BtNoColor.Text = Loc.S("noColor");
            LbAlternativeColor.Text = Loc.S("LbAlternativeColor");
            _tt.SetToolTip(BtNoColor, "0: no color");
            SetUndefinedColorId();
            buttonCancel.Visible = false;
            buttonCancel.Text = Loc.S("Cancel");

            Disposed += MyColorPicker_Disposed;

            checkBoxOnlyNatural.Text = Loc.S("showOnlyNaturalOccurring");
        }

        private void MyColorPicker_Disposed(object sender, EventArgs e)
        {
            _tt.RemoveAll();
            _tt.Dispose();
        }

        public void CancelButtonVisible(bool visible)
        {
            buttonCancel.Visible = visible;
        }

        public CheckBox CbOnlyNatural => checkBoxOnlyNatural;

        /// <summary>
        /// Disables handling of alternative colors.
        /// </summary>
        public void DisableAlternativeColor()
        {
            LbAlternativeColor.Visible = false;
        }

        public void SetUndefinedColorId()
        {
            BtUndefinedColor.Tag = Ark.UndefinedColorId; // one possible id of undefined color, currently used by Ark
            _tt.SetToolTip(BtUndefinedColor, $"{Ark.UndefinedColorId}: undefined color");
        }

        /// <summary>
        /// Clears color buttons. Call if color definitions changed, e.g. when a mod with colors is loaded or unloaded.
        /// </summary>
        private void ResetColors()
        {
            SetUndefinedColorId();
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                _tt.SetToolTip(c, null);
                c.Dispose();
            }
            flowLayoutPanel1.Controls.Clear();
        }

        public void PickColor(byte selectedColorId, string headerText, List<ArkColor> naturalColors = null,
            byte selectedColorIdAlternative = 0, HashSet<byte> existingColors = null)
        {
            flowLayoutPanel1.SuspendDrawing();
            flowLayoutPanel1.SuspendLayout();

            label1.Text = headerText;

            if (modListHash != values.Values.V.loadedModsHash)
            {
                ResetColors();
                modListHash = values.Values.V.loadedModsHash;
            }

            var colors = values.Values.V.Colors.ColorsList;

            if (_buttonSelectedColor != null && _buttonSelectedColor.Status != NoPaddingButton.ColorStatus.None)
            {
                _buttonSelectedColor.Status = NoPaddingButton.ColorStatus.None;
                _buttonSelectedColor.Invalidate();
            }
            SelectedColorId = selectedColorId;
            SelectedColorIdAlternative = selectedColorIdAlternative;
            _naturalColorIDs = naturalColors?.Select(ac => ac.Id).ToArray();
            checkBoxOnlyNatural.Visible = _naturalColorIDs != null;

            const int colorButtonHeight = 24;

            for (int colorIndex = 1; colorIndex < colors.Length; colorIndex++)
            {
                int controlIndex = colorIndex - 1;
                if (flowLayoutPanel1.Controls.Count <= controlIndex)
                {
                    var np = new NoPaddingButton
                    {
                        Width = 44,
                        Height = colorButtonHeight,
                        Margin = new Padding(0)
                    };
                    np.Click += ColorChosen;
                    flowLayoutPanel1.Controls.Add(np);
                }

                if (flowLayoutPanel1.Controls[controlIndex] is NoPaddingButton bt)
                {
                    var color = colors[colorIndex];
                    var colorId = color.Id;
                    bt.Visible = ColorVisible(colorId);
                    SetButtonStatus(bt, colorId);
                    bt.SetBackColorAndAccordingForeColor(color.Color);
                    bt.Tag = colorId;
                    bt.Text = colorId.ToString();
                    _tt.SetToolTip(bt, colorId + ": " + color.Name);
                }
            }

            SetButtonStatus(BtNoColor, 0);
            BtNoColor.Invalidate();
            SetButtonStatus(BtUndefinedColor, (byte)BtUndefinedColor.Tag);
            BtUndefinedColor.Invalidate();

            void SetButtonStatus(NoPaddingButton bt, byte colorId)
            {
                bt.Status = NoPaddingButton.ColorStatus.None;
                if (SelectedColorId == colorId)
                {
                    _buttonSelectedColor = bt;
                    bt.Status = NoPaddingButton.ColorStatus.SelectedColor;
                }
                if (SelectedColorIdAlternative == colorId && colorId != 0)
                {
                    bt.Status |= NoPaddingButton.ColorStatus.SelectedAlternative;
                }
                if (existingColors != null)
                {
                    if (existingColors.Contains(colorId))
                        bt.Status |= NoPaddingButton.ColorStatus.ExistingColor;
                    else
                        bt.Status |= NoPaddingButton.ColorStatus.NonExistingColor;
                }
            }

            var controlHeight = (int)Math.Ceiling(colors.Length / 10d) * colorButtonHeight + 99;
            Height = controlHeight;
            HeightChanged?.Invoke(controlHeight);
            flowLayoutPanel1.ResumeLayout();
            flowLayoutPanel1.ResumeDrawing();
            if (Window != null)
                Window.isShown = true;
        }

        private bool ColorVisible(byte id) => !checkBoxOnlyNatural.Checked || (_naturalColorIDs?.Contains(id) ?? true);

        /// <summary>
        /// Color was chosen and saved in the property SelectedColorId. Window then will be hidden.
        /// </summary>
        private void ColorChosen(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Control) != 0 && LbAlternativeColor.Visible)
            {
                // only set alternative color
                SelectedColorIdAlternative = (byte)((Control)sender).Tag;

                foreach (var ct in flowLayoutPanel1.Controls)
                {
                    if (ct is NoPaddingButton bt)
                    {
                        var buttonIsColorAlternative = SelectedColorIdAlternative == (byte)bt.Tag;
                        if (bt.Status.HasFlag(NoPaddingButton.ColorStatus.SelectedAlternative) != buttonIsColorAlternative)
                        {
                            if (buttonIsColorAlternative)
                                bt.Status |= NoPaddingButton.ColorStatus.SelectedAlternative;
                            else
                                bt.Status &= ~NoPaddingButton.ColorStatus.SelectedAlternative;
                            bt.Invalidate();
                        }
                    }
                }

                UserMadeSelection?.Invoke(true);
                return;
            }
            // remove selection around current button
            if (_buttonSelectedColor != null)
            {
                _buttonSelectedColor.Status &= ~NoPaddingButton.ColorStatus.SelectedColor;
                _buttonSelectedColor.Invalidate();
            }

            SelectedColorId = (byte)((Button)sender).Tag;
            // if selected color was alternative selected color, remove alternative color
            if (SelectedColorId == SelectedColorIdAlternative)
                SelectedColorIdAlternative = 0;

            if (sender is NoPaddingButton bts)
            {
                _buttonSelectedColor = bts;
                _buttonSelectedColor.Status |= NoPaddingButton.ColorStatus.SelectedColor;
                _buttonSelectedColor.Invalidate();
            }
            else
            {
                _buttonSelectedColor = null;
            }

            UserMadeSelection?.Invoke(true);
        }

        private void ButtonCancelClick(object sender, EventArgs e) => UserMadeSelection?.Invoke(false);

        private void checkBoxOnlyNatural_CheckedChanged(object sender, EventArgs e)
        {
            flowLayoutPanel1.SuspendDrawing();
            flowLayoutPanel1.SuspendLayout();
            for (int c = 0; c < flowLayoutPanel1.Controls.Count; c++)
                flowLayoutPanel1.Controls[c].Visible = ColorVisible((byte)flowLayoutPanel1.Controls[c].Tag);
            flowLayoutPanel1.ResumeLayout();
            flowLayoutPanel1.ResumeDrawing();
        }

        private class NoPaddingButton : Button
        {
            public ColorStatus Status;

            protected override void OnPaint(PaintEventArgs pe)
            {
                pe.Graphics.Clear(SystemColors.Control);

                var defaultVisibleRectangle = ClientRectangle;
                if (Status.HasFlag(ColorStatus.NonExistingColor))
                    defaultVisibleRectangle.Inflate(-6, -6);
                else
                    defaultVisibleRectangle.Inflate(-3, -3);
                using (var b = new SolidBrush(BackColor))
                    pe.Graphics.FillRectangle(b, defaultVisibleRectangle);

                if (Status.HasFlag(ColorStatus.SelectedColor))
                {
                    DrawRectangleAroundButton(Color.Black, ClientRectangle);
                }
                else if (Status.HasFlag(ColorStatus.SelectedAlternative))
                {
                    DrawRectangleAroundButton(Color.Red, ClientRectangle);
                }
                else if (Status.HasFlag(ColorStatus.ExistingColor))
                {
                    DrawRectangleAroundButton(Color.Green, ClientRectangle);
                }

                void DrawRectangleAroundButton(Color color, Rectangle rect)
                {
                    using (var p = new Pen(color, 2))
                    {
                        rect.Inflate(-1, -1);
                        pe.Graphics.DrawRectangle(p, rect);
                        p.Color = Color.White;
                        rect.Inflate(-2, -2);
                        pe.Graphics.DrawRectangle(p, rect);
                    }
                }

                if (string.IsNullOrEmpty(Text)) return;
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                using (var b = new SolidBrush(ForeColor))
                    pe.Graphics.DrawString(Text, Font, b, ClientRectangle, stringFormat);
            }

            [Flags]
            public enum ColorStatus
            {
                None = 0,
                SelectedColor = 1 << 0,
                SelectedAlternative = 1 << 1,
                ExistingColor = 1 << 2,
                NonExistingColor = 1 << 3
            }
        }
    }
}
