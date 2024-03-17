using System;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    internal class PopupMessage : Form
    {
        private Label _label;

        public PopupMessage()
        {
            Width = 600;
            Height = 400;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            BackColor = Color.LightGray;
            Padding = new Padding(2);
            StartPosition = FormStartPosition.CenterParent;

            var lClose = new Label
            {
                Text = Loc.S("click to close"),
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.LightSalmon
            };
            Controls.Add(lClose);
            lClose.Click += CloseClick;

            _label = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.Gainsboro,
                BackColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(_label);
            _label.Click += CloseClick;
        }

        public PopupMessage(string message, float fontSize = 8.25f) : this()
        {
            if (fontSize != 8.25f)
                _label.Font = new Font(_label.Font.FontFamily, fontSize);
            _label.Text = message;
        }

        public static void Show(Form parent, string message, float fontSize = 8.25f)
        {
            new PopupMessage(message, fontSize).ShowDialog(parent);
        }

        private void CloseClick(object sender, EventArgs e) => Close();
    }
}
