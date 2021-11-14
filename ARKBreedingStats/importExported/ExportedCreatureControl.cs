using ARKBreedingStats.Library;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ARKBreedingStats.utils;

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
        public readonly string exportedFile;
        private readonly ToolTip _tt = new ToolTip();
        public bool validValues;
        public string speciesBlueprintPath;

        public ExportedCreatureControl()
        {
            InitializeComponent();
            validValues = true;
        }

        public ExportedCreatureControl(string filePath) : this()
        {
            exportedFile = filePath;
            creatureValues = ImportExported.ImportExportedCreature(filePath);

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

            _tt.SetToolTip(btRemoveFile, "Delete the exported game-file");
        }

        private void ExportedCreatureControl_Disposed(object sender, EventArgs e)
        {
            _tt.RemoveAll();
            _tt.Dispose();
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
                case ImportStatus.NeedsLevelChoosing:
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
            NeedsLevelChoosing,
            NotImported,
            JustImported,
            OldImported
        }

        private void btRemoveFile_Click(object sender, EventArgs e)
        {
            if (RemoveFile((ModifierKeys & Keys.Shift) == 0))
            {
                DisposeThis?.Invoke(this, null);
            }
        }

        public bool RemoveFile(bool getConfirmation = true)
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
                MessageBoxes.ShowMessageBox($"The file does not exist:\n{exportedFile}");
            }
            return successfullyDeleted;
        }
    }
}
