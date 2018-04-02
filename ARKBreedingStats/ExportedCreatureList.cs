using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using System;
using System.IO;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class ExportedCreatureList : Form
    {
        public event ExportedCreatureControl.CopyValuesToExtractorEventHandler CopyValuesToExtractor;
        public event ExportedCreatureControl.CheckGuidInLibraryEventHandler CheckGuidInLibrary;
        public delegate void ReadyForCreatureUpdatesEventHandler();
        public event ReadyForCreatureUpdatesEventHandler ReadyForCreatureUpdates;

        public ExportedCreatureList()
        {
            InitializeComponent();
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

                string[] files = Directory.GetFiles(folderPath, "*.ini");
                foreach (string f in files)
                {
                    addCreatureValuesControl(ImportExported.importExportedCreature(f));
                }
            }
        }

        private void addCreatureValuesControl(CreatureValues cv)
        {
            ExportedCreatureControl ecc = new ExportedCreatureControl(cv);
            ecc.Dock = DockStyle.Top;
            ecc.CopyValuesToExtractor += CopyValuesToExtractor;
            ecc.CheckGuidInLibrary += CheckGuidInLibrary;
            ecc.DoCheckGuidInLibrary();
            panel1.Controls.Add(ecc);
        }

        private void ClearControls()
        {
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
    }
}
