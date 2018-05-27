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
        public event ExportedCreatureControl.CheckGuidInLibraryEventHandler CheckGuidInLibrary;
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
            if (!String.IsNullOrWhiteSpace(folderPath))
            {
                ClearControls();

                string[] files = Directory.GetFiles(folderPath, "DinoExport*.ini");
                foreach (string f in files)
                {
                    ExportedCreatureControl ecc = new ExportedCreatureControl(f);
                    ecc.Dock = DockStyle.Top;
                    ecc.CopyValuesToExtractor += CopyValuesToExtractor;
                    ecc.CheckGuidInLibrary += CheckGuidInLibrary;
                    ecc.DoCheckGuidInLibrary();
                    eccs.Add(ecc);
                }

                // sort according to date and if already in library (order seems reversed here, because controls get added reversely)
                eccs = eccs.OrderByDescending(e => e.Status).ThenBy(e => e.AddedToLibrary).ToList();

                foreach (var ecc in eccs)
                    panel1.Controls.Add(ecc);
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
    }
}
