using System;
using System.IO;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class FileSelector : UserControl
    {
        public string fileFilter;
        private bool _isFile;
        private string _linkPath;
        private readonly ToolTip _tt;

        public FileSelector()
        {
            InitializeComponent();
            _tt = new ToolTip();
            Disposed += OnDisposed;
        }

        private void btChooseFile_Click(object sender, EventArgs e)
        {
            if (_isFile)
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
            if (MessageBox.Show("Delete the selection of this " + (_isFile ? "file" : "folder"), "Remove?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Link = "";
        }

        public string Link
        {
            get => _linkPath;
            set
            {
                _linkPath = value;
                if (_linkPath.Length > 90)
                {
                    lbLink.Text = _linkPath.Substring(0, 30) + "…" + _linkPath.Substring(_linkPath.Length - 59);
                    _tt.SetToolTip(lbLink, _linkPath);
                }
                else lbLink.Text = _linkPath;
            }
        }

        public bool IsFile
        {
            // file or folder
            set
            {
                _isFile = value;
                btChooseFile.Text = _isFile ? "Choose File…" : "Choose Folder…";
            }
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            _tt.RemoveAll();
            _tt.Dispose();
        }
    }
}
