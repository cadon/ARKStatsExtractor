using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class CustomMessageBox : Form
    {
        private CustomMessageBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Displays a modal dialog with up to three buttons with custom texts that can return Yes, No or Cancel.
        /// </summary>
        internal static DialogResult Show(string message, string title, string buttonYes, string buttonNo = null,
            string buttonCancel = null, MessageBoxIcon icon = MessageBoxIcon.None, bool showCopyToClipboard = false)
        {
            using (var f = new CustomMessageBox())
            {
                f.LabelMessage.Text = message;
                f.Text = $"{title} - {Utils.ApplicationNameVersion}";
                if (string.IsNullOrEmpty(buttonYes))
                    f.ButtonYes.Visible = false;
                else
                {
                    f.ButtonYes.Text = buttonYes;
                    f.ButtonYes.DialogResult = DialogResult.Yes;
                }

                if (string.IsNullOrEmpty(buttonNo))
                    f.ButtonNo.Visible = false;
                else
                {
                    f.ButtonNo.Text = buttonNo;
                    f.ButtonNo.DialogResult = DialogResult.No;
                }

                if (string.IsNullOrEmpty(buttonCancel))
                    f.ButtonCancel.Visible = false;
                else
                {
                    f.ButtonCancel.Text = buttonCancel;
                    f.ButtonCancel.DialogResult = DialogResult.Cancel;
                }

                // default
                f.DialogResult = DialogResult.Cancel;

                switch (icon)
                {
                    case MessageBoxIcon.Information:
                        f.PbIcon.Image = SystemIcons.Information.ToBitmap();
                        break;
                    case MessageBoxIcon.Warning:
                        f.PbIcon.Image = SystemIcons.Warning.ToBitmap();
                        break;
                    case MessageBoxIcon.Error:
                        f.PbIcon.Image = SystemIcons.Error.ToBitmap();
                        break;
                }

                f.BtCopyToClipboard.Visible = showCopyToClipboard;

                return f.ShowDialog();
            }
        }

        private void BtCopyToClipboard_Click(object sender, System.EventArgs e)
        {
            var message = LabelMessage.Text;
            if (string.IsNullOrEmpty(message)) return;

            Clipboard.SetText(message);
        }
    }
}
