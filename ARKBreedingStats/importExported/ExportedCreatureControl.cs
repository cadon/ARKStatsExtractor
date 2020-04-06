using ARKBreedingStats.Library;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ARKBreedingStats.importExported
{
    public partial class ExportedCreatureControl : UserControl
    {
        public delegate void CopyValuesToExtractorEventHandler(ExportedCreatureControl exportedCreatureControl, bool addToLibrary, bool goToLibrary);
        public event CopyValuesToExtractorEventHandler CopyValuesToExtractor;
        public delegate void CheckArkIdInLibraryEventHandler(ExportedCreatureControl exportedCreatureControl);
        public event CheckArkIdInLibraryEventHandler CheckArkIdInLibrary;
        public event EventHandler DisposeThis;
        public readonly CreatureValues creatureValues;
        public ImportStatus Status { get; private set; }
        public DateTime? AddedToLibrary;
        public readonly string exportedFile;
        private ToolTip tt;
        public bool validValues;
        public string speciesBlueprintPath;

        public ExportedCreatureControl()
        {
            InitializeComponent();
            if (tt == null)
                tt = new ToolTip();
            validValues = true;
        }

        public ExportedCreatureControl(string filePath)
        {
            InitializeComponent();
            exportedFile = filePath;
            creatureValues = ImportExported.importExportedCreature(filePath);

            // check if the values are valid, i.e. if the read file was a creature-file at all.
            if (creatureValues?.Species == null)
            {
                speciesBlueprintPath = creatureValues?.speciesBlueprint;
                validValues = false;
                return;
            }

            groupBox1.Text = $"{creatureValues.name} ({(creatureValues.Species?.name ?? "unknown species")}, Lvl {creatureValues.level}), " +
                    $"exported at {Utils.ShortTimeDate(creatureValues.domesticatedAt)}. " +
                    $"Filename: {Path.GetFileName(filePath)}";
            Disposed += ExportedCreatureControl_Disposed;

            if (tt == null)
                tt = new ToolTip();
            tt.SetToolTip(btRemoveFile, "Delete the exported game-file");

            validValues = true;
        }

        private void ExportedCreatureControl_Disposed(object sender, EventArgs e)
        {
            tt.RemoveAll();
        }

        private void btLoadValues_Click(object sender, EventArgs e)
        {
            CopyValuesToExtractor?.Invoke(this, false, false);
        }

        public void extractAndAddToLibrary(bool goToLibrary = true)
        {
            CopyValuesToExtractor?.Invoke(this, true, goToLibrary);
        }

        public void setStatus(ImportStatus status, DateTime? addedToLibrary)
        {
            Status = status;
            AddedToLibrary = addedToLibrary;
            switch (status)
            {
                case ImportStatus.NotImported:
                    lbStatus.Text = "Not yet extracted";
                    groupBox1.BackColor = Color.LemonChiffon;
                    break;
                case ImportStatus.JustImported:
                    // if extracted in this session
                    lbStatus.Text = "Values were just extracted and creature is added to library";
                    groupBox1.BackColor = Color.LightGreen;
                    break;
                case ImportStatus.OldImported:
                    lbStatus.Text = "Already imported on " + Utils.ShortTimeDate(addedToLibrary, false);
                    groupBox1.BackColor = Color.YellowGreen;
                    break;
                case ImportStatus.NeedsLevelChosing:
                    lbStatus.Text = "Cannot be extracted automatically, you need to choose from level combinations";
                    groupBox1.BackColor = Color.Yellow;
                    break;
            }
        }

        public void DoCheckArkIdInLibrary()
        {
            CheckArkIdInLibrary?.Invoke(this);
        }

        public enum ImportStatus
        {
            NeedsLevelChosing,
            NotImported,
            JustImported,
            OldImported
        }

        private void btRemoveFile_Click(object sender, EventArgs e)
        {
            if (removeFile())
            {
                DisposeThis?.Invoke(this, null);
            }
        }

        public bool removeFile(bool getConfirmation = true)
        {
            bool successfullyDeleted = false;
            if (File.Exists(exportedFile))
            {
                if (!getConfirmation || MessageBox.Show("Are you sure to remove the exported file for this creature?\nThis cannot be undone.", "Remove file?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(exportedFile);
                        successfullyDeleted = true;
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            else
            {
                MessageBox.Show("The file does not exist:\n" + exportedFile, "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return successfullyDeleted;
        }
    }
}
