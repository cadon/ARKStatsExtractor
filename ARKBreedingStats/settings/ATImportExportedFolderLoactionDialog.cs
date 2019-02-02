using System;
using System.Windows.Forms;

namespace ARKBreedingStats.settings
{
    public partial class ATImportExportedFolderLocationDialog : Form
    {

        public ATImportExportedFolderLocation ATImportExportedFolderLocation
        {
            get => new ATImportExportedFolderLocation(textBox_ConvenientName.Text,
                    textBox_ownerSuffix.Text,
                    textBox_FolderPath.Text);
            set
            {
                textBox_ConvenientName.Text = value.ConvenientName;
                textBox_ownerSuffix.Text = value.OwnerSuffix;
                textBox_FolderPath.Text = value.FolderPath;
            }
        }

        public ATImportExportedFolderLocationDialog(ATImportExportedFolderLocation folderPath = null)
        {
            InitializeComponent();
            if (folderPath != null)
            {
                ATImportExportedFolderLocation = folderPath;
            }
        }

        private void button_FileSelect_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                string previousLocation = ATImportExportedFolderLocation.FolderPath;
                dlg.RootFolder = Environment.SpecialFolder.Desktop;
                if (!string.IsNullOrWhiteSpace(previousLocation))
                    dlg.SelectedPath = previousLocation;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    textBox_FolderPath.Text = dlg.SelectedPath;
                }
            }
        }
    }
}
