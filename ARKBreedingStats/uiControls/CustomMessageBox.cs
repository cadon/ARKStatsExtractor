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
        internal static DialogResult Show(string message, string title, string buttonYes, string buttonNo = null, string buttonCancel = null)
        {
            using (var f = new CustomMessageBox())
            {
                f.LabelMessage.Text = message;
                f.Text = title;
                if (string.IsNullOrEmpty(buttonYes))
                    f.ButtonYes.Visible = false;
                else
                    f.ButtonYes.Text = buttonYes;

                if (string.IsNullOrEmpty(buttonNo))
                    f.ButtonNo.Visible = false;
                else
                    f.ButtonNo.Text = buttonNo;

                if (string.IsNullOrEmpty(buttonCancel))
                    f.ButtonCancel.Visible = false;
                else
                    f.ButtonCancel.Text = buttonCancel;

                f.ButtonYes.DialogResult = DialogResult.Yes;
                f.ButtonNo.DialogResult = DialogResult.No;
                f.ButtonCancel.DialogResult = DialogResult.Cancel;

                f.DialogResult = DialogResult.Cancel;

                return f.ShowDialog();
            }
        }
    }
}
