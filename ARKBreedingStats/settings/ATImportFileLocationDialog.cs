using System;
using System.IO;
using System.Windows.Forms;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.settings
{
    public partial class ATImportFileLocationDialog : Form
    {

        public ATImportFileLocation AtImportFileLocation
        {
            get => new ATImportFileLocation(textBox_ConvenientName.Text,
                    textBox_ServerName.Text, textBox_FileLocation.Text.Trim());
            set
            {
                textBox_ConvenientName.Text = value.ConvenientName;
                textBox_ServerName.Text = value.ServerName;
                textBox_FileLocation.Text = value.FileLocation;
            }
        }

        public ATImportFileLocationDialog(ATImportFileLocation fileLocation = null)
        {
            InitializeComponent();
            if (fileLocation != null)
            {
                AtImportFileLocation = fileLocation;
            }
        }

        private void button_FileSelect_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                if (!string.IsNullOrWhiteSpace(textBox_FileLocation.Text))
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(textBox_FileLocation.Text);
                }
                else if (ArkInstallationPath.GetListOfExportFolders(out var folders, out _))
                {
                    foreach (var f in folders)
                    {
                        var savesFolderPath = Directory.GetParent(f.path)?.Parent?.FullName;
                        if (savesFolderPath != null && Directory.Exists(savesFolderPath))
                        {
                            dlg.InitialDirectory = savesFolderPath;
                            break;
                        }
                    }
                }

                dlg.FileName = Path.GetFileName(textBox_FileLocation.Text);
                dlg.Filter = "ARK savegame (*.ark)|*.ark|All files (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    textBox_FileLocation.Text = dlg.FileName;
                }
            }
        }

        private void LlFtpHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RepositoryInfo.OpenWikiPage("Ftp-access");
        }
    }
}
