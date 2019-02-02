using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats.importExported
{
    public partial class ExportedCreatureList : Form
    {
        public event ExportedCreatureControl.CopyValuesToExtractorEventHandler CopyValuesToExtractor;
        public event ExportedCreatureControl.CheckArkIdInLibraryEventHandler CheckArkIdInLibrary;

        public delegate void ReadyForCreatureUpdatesEventHandler();

        public event ReadyForCreatureUpdatesEventHandler ReadyForCreatureUpdates;
        private List<ExportedCreatureControl> eccs;
        private string selectedFolder;
        public string ownerSuffix;

        public ExportedCreatureList()
        {
            InitializeComponent();
            eccs = new List<ExportedCreatureControl>();

            Size = Properties.Settings.Default.importExportedSize;

            FormClosing += ExportedCreatureList_FormClosing;

            // TODO implement
            updateDataOfLibraryCreaturesToolStripMenuItem.Visible = false;
            loadServerSettingsOfFolderToolStripMenuItem.Visible = false;
        }

        private void ExportedCreatureList_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.importExportedLocation = Location;
            Properties.Settings.Default.importExportedSize = Size;
        }

        private void chooseFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chooseFolderAndImport();
        }

        public void chooseFolderAndImport()
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.RootFolder = Environment.SpecialFolder.Desktop;
                if (Utils.GetFirstImportExportFolder(out string exportFolder))
                {
                    dlg.SelectedPath = exportFolder;
                }
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    loadFilesInFolder(dlg.SelectedPath);
                }
            }
        }

        public void loadFilesInFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                selectedFolder = folderPath;

                string[] files = Directory.GetFiles(folderPath, "*dinoexport*.ini");
                // check if there are many files to import, then ask because that can take time
                if (files.Length > 50 &&
                        MessageBox.Show($"There are many files to import ({files.Length}) which can take some time.\n" +
                                "Do you really want to read all these files?",
                                "Many files to import", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

                ClearControls();

                // load game.ini and gameusersettings.ini if available and use the settings.
                if (File.Exists(folderPath + @"\game.ini") || File.Exists(folderPath + @"\gameusersettings.ini"))
                {
                    // set multipliers to default
                    // TODO
                    // set settings to values of files
                    // TODO
                }

                foreach (string f in files)
                {
                    ExportedCreatureControl ecc = new ExportedCreatureControl(f);
                    if (ecc.validValues)
                    {
                        ecc.Dock = DockStyle.Top;
                        ecc.CopyValuesToExtractor += CopyValuesToExtractor;
                        ecc.CheckArkIdInLibrary += CheckArkIdInLibrary;
                        ecc.DoCheckArkIdInLibrary();
                        eccs.Add(ecc);
                    }
                }

                // sort according to date and if already in library (order seems reversed here, because controls get added reversely)
                eccs = eccs.OrderByDescending(e => e.Status).ThenBy(e => e.AddedToLibrary).ToList();

                foreach (var ecc in eccs)
                    panel1.Controls.Add(ecc);

                Text = "Exported creatures in " + Utils.shortPath(folderPath, 100);
                UpdateStatusBarLabel();
            }
        }

        private void ClearControls()
        {
            eccs.Clear();

            // foreach (Control c in panel1.Controls)
            //     ((ExportedCreatureControl)c).Dispose();

            SuspendLayout();
            try
            {
                while (panel1.Controls.Count > 0)
                    panel1.Controls[0].Dispose();
            }
            finally
            {
                ResumeLayout();
            }
        }

        private void UpdateStatusBarLabel()
        {
            int justImported = 0,
                oldImported = 0,
                notImported = 0,
                issuesWhileImporting = 0,
                totalFiles = 0;

            foreach (var ecc in eccs)
            {
                totalFiles++;
                switch (ecc.Status)
                {
                    case ExportedCreatureControl.ImportStatus.JustImported: justImported++; break;
                    case ExportedCreatureControl.ImportStatus.NeedsLevelChosing: issuesWhileImporting++; break;
                    case ExportedCreatureControl.ImportStatus.NotImported: notImported++; break;
                    case ExportedCreatureControl.ImportStatus.OldImported: oldImported++; break;
                }
            }

            toolStripStatusLabel1.Text = totalFiles.ToString() + " total files. " +
                (notImported + issuesWhileImporting).ToString() + " not imported, " +
                issuesWhileImporting.ToString() + " of these need manual level selection. " +
                justImported.ToString() + " just imported. " +
                oldImported.ToString() + " old imported.";
        }

        private void updateDataOfLibraryCreaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadyForCreatureUpdates?.Invoke();
        }

        public void UpdateCreatureData(CreatureCollection cc)
        {
            // TODO
        }

        private void loadServerSettingsOfFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // check if a game.ini and or gameuser.ini is available and set the settings accordingly
        }

        private void importAllUnimportedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importAllUnimported();
        }

        /// <summary>
        /// Tries to import all listed creatures and adds them to the library if the extraction is unique.
        /// </summary>
        private void importAllUnimported()
        {
            foreach (var ecc in eccs)
            {
                if (ecc.Status == ExportedCreatureControl.ImportStatus.NotImported)
                    ecc.extractAndAddToLibrary(goToLibrary: false);
            }
            UpdateStatusBarLabel();
        }

        private void deleteAllImportedFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteAllImportedFiles();
        }

        private void deleteAllImportedFiles()
        {
            SuspendLayout();
            if (MessageBox.Show("Delete all exported files in the current folder that are already imported in this library?\nThis cannot be undone!",
                    "Delete imported files?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int deletedFilesCount = 0;
                foreach (var ecc in eccs)
                {
                    if (ecc.Status == ExportedCreatureControl.ImportStatus.JustImported || ecc.Status == ExportedCreatureControl.ImportStatus.OldImported)
                    {
                        if (ecc.removeFile(false))
                        {
                            deletedFilesCount++;
                            ecc.Dispose();
                        }
                    }
                }
                if (deletedFilesCount > 0)
                    MessageBox.Show(deletedFilesCount + " imported files deleted.", "Deleted Files",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            UpdateStatusBarLabel();
            ResumeLayout();
        }

        private void deleteAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteAllFiles();
        }

        private void deleteAllFiles()
        {
            SuspendLayout();
            if (MessageBox.Show("Delete all files in the current folder, regardless if they are imported or not imported?\nThis cannot be undone!",
                    "Delete ALL files?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int deletedFilesCount = 0;
                foreach (ExportedCreatureControl ecc in eccs)
                {
                    if (ecc.removeFile(false))
                    {
                        deletedFilesCount++;
                        ecc.Dispose();
                    }
                }
                if (deletedFilesCount > 0)
                    MessageBox.Show(deletedFilesCount + " imported files deleted.", "Deleted Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            UpdateStatusBarLabel();
            ResumeLayout();
        }

        private void ExportedCreatureList_DragEnter(object sender, DragEventArgs e)
        {
            DragDropEffects effects = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string path = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                if (Directory.Exists(path))
                    effects = DragDropEffects.Copy;
            }
            e.Effect = effects;
        }

        private void ExportedCreatureList_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string path = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                if (Directory.Exists(path))
                    loadFilesInFolder(path);
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedFolder) && Directory.Exists(selectedFolder))
                System.Diagnostics.Process.Start(selectedFolder);
        }

        private void toolStripCbHideImported_Click(object sender, EventArgs e)
        {
            bool showImported = !toolStripCbHideImported.Checked;
            toolStripCbHideImported.Text = toolStripCbHideImported.Checked ? "Show imported" : "Hide imported";

            SuspendLayout();
            foreach (var ecc in eccs)
                if (ecc.Status == ExportedCreatureControl.ImportStatus.JustImported || ecc.Status == ExportedCreatureControl.ImportStatus.OldImported) ecc.Visible = showImported;
            ResumeLayout();
        }

        private void setUserSuffixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Utils.ShowTextInput("Owner suffix (will be appended to the owner)", out string ownerSfx, "Owner Suffix", ownerSuffix))
                ownerSuffix = ownerSfx;
        }
    }
}
