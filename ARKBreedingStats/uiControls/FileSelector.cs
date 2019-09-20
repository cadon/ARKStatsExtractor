using System;
using System.IO;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class FileSelector : UserControl
    {
        public string fileFilter;
        private bool isFile;
        private string linkPath;
        private readonly ToolTip tt;

        public FileSelector()
        {
            InitializeComponent();
            tt = new ToolTip();
            Disposed += OnDispose;
        }

        private void btChooseFile_Click(object sender, EventArgs e)
        {
            if (isFile)
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    string previousLocation = Link;
                    if (!string.IsNullOrWhiteSpace(previousLocation)) dlg.InitialDirectory = Path.GetDirectoryName(previousLocation);
                    dlg.FileName = Path.GetFileName(previousLocation);
                    dlg.Filter = fileFilter;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        Link = dlg.FileName;
                    }
                }
            }
            else
            {
                using (FolderBrowserDialog dlg = new FolderBrowserDialog())
                {
                    string previousLocation = Link;
                    if (!string.IsNullOrWhiteSpace(previousLocation))
                    {
                        dlg.RootFolder = Environment.SpecialFolder.Desktop;
                        dlg.SelectedPath = Path.GetDirectoryName(previousLocation);
                    }
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        Link = dlg.SelectedPath;
                    }
                }
            }
        }

        private void btDeleteLink_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete the selection of this " + (isFile ? "file" : "folder"), "Remove?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Link = "";
        }

        public string Link
        {
            get => linkPath;
            set
            {
                linkPath = value;
                if (linkPath.Length > 90)
                {
                    lbLink.Text = linkPath.Substring(0, 30) + "…" + linkPath.Substring(linkPath.Length - 59);
                    tt.SetToolTip(lbLink, linkPath);
                }
                else lbLink.Text = linkPath;
            }
        }

        public bool IsFile
        {
            // file or folder
            set
            {
                isFile = value;
                btChooseFile.Text = isFile ? "Choose File…" : "Choose Folder…";
            }
        }

        private void OnDispose(object sender, EventArgs e)
        {
            tt.RemoveAll();
        }
    }
}
