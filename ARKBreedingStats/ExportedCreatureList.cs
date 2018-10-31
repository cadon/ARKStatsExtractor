using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class ExportedCreatureList : Form
    {
        public event ExportedCreatureControl.CopyValuesToExtractorEventHandler CopyValuesToExtractor;
        public event ExportedCreatureControl.CheckArkIdInLibraryEventHandler CheckArkIdInLibrary;
        public delegate void ReadyForCreatureUpdatesEventHandler();
        public event ReadyForCreatureUpdatesEventHandler ReadyForCreatureUpdates;
        private List<ExportedCreatureControl> eccs;

        public ExportedCreatureList()
        {
            InitializeComponent();
            eccs = new List<ExportedCreatureControl>();

            // TODO implement
            updateDataOfLibraryCreaturesToolStripMenuItem.Visible = false;
            loadServerSettingsOfFolderToolStripMenuItem.Visible = false;
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
                if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.ExportCreatureFolder))
                {
                    dlg.SelectedPath = Properties.Settings.Default.ExportCreatureFolder;
                }
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    loadFilesInFolder(dlg.SelectedPath);
                }
            }
        }

        public void loadFilesInFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath, "DinoExport*.ini");
                // check if there are many files to import, then ask because that can take time
                if (files.Length > 40 &&
                    MessageBox.Show("There are many files to import (" + files.Length.ToString() + ") which can take some time.\nDo you really want to read all these files?",
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
                    ecc.Dock = DockStyle.Top;
                    ecc.CopyValuesToExtractor += CopyValuesToExtractor;
                    ecc.CheckArkIdInLibrary += CheckArkIdInLibrary;
                    ecc.DoCheckArkIdInLibrary();
                    eccs.Add(ecc);
                }

                // sort according to date and if already in library (order seems reversed here, because controls get added reversely)
                eccs = eccs.OrderByDescending(e => e.Status).ThenBy(e => e.AddedToLibrary).ToList();

                foreach (var ecc in eccs)
                    panel1.Controls.Add(ecc);

                Text = "Exported creatures in " + Utils.shortPath(folderPath, 50);
            }
        }

        private void ClearControls()
        {
            eccs.Clear();
            foreach (Control c in panel1.Controls)
                ((ExportedCreatureControl)c).Dispose();
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
            foreach (var c in panel1.Controls)
            {
                var ecc = (ExportedCreatureControl)c;
                if (ecc.Status == ExportedCreatureControl.ImportStatus.NotImported)
                    ecc.extractAndAddToLibrary();
            }
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
                if (deletedFilesCount > 0) MessageBox.Show(deletedFilesCount.ToString() + " imported files deleted.", "Deleted Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
                foreach (var ecc in eccs)
                {
                    if (ecc.removeFile(false))
                    {
                        deletedFilesCount++;
                        ecc.Dispose();
                    }
                }
                if (deletedFilesCount > 0) MessageBox.Show(deletedFilesCount.ToString() + " imported files deleted.", "Deleted Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            ResumeLayout();
        }

        private void ExportedCreatureList_DragEnter(object sender, DragEventArgs e)
        {
            DragDropEffects effects = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var path = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                if (Directory.Exists(path))
                    effects = DragDropEffects.Copy;
            }
            e.Effect = effects;
        }

        private void ExportedCreatureList_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var path = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                if (Directory.Exists(path))
                    loadFilesInFolder(path);
            }
        }
    }
}
