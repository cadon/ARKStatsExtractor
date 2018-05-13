using ARKBreedingStats.species;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class ExportedCreatureControl : UserControl
    {
        public delegate void CopyValuesToExtractorEventHandler(ExportedCreatureControl exportedCreatureControl, bool addToLibrary);
        public event CopyValuesToExtractorEventHandler CopyValuesToExtractor;
        public delegate void CheckGuidInLibraryEventHandler(ExportedCreatureControl exportedCreatureControl);
        public event CheckGuidInLibraryEventHandler CheckGuidInLibrary;
        public CreatureValues creatureValues;
        public ImportStatus Status { get; internal set; }
        public DateTime AddedToLibrary;

        public ExportedCreatureControl()
        {
            InitializeComponent();
        }

        public ExportedCreatureControl(CreatureValues creatureValues)
        {
            InitializeComponent();
            this.creatureValues = creatureValues;
            groupBox1.Text = creatureValues.name + " (" + creatureValues.species + ", Lv " + creatureValues.level + "), exported at " + Utils.shortTimeDate(creatureValues.domesticatedAt);
        }

        private void btLoadValues_Click(object sender, EventArgs e)
        {
            CopyValuesToExtractor?.Invoke(this, false);
        }

        public void extractAndAddToLibrary()
        {
            CopyValuesToExtractor?.Invoke(this, true);
        }

        public void setStatus(ImportStatus status, DateTime addedToLibrary)
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
                    lbStatus.Text = "Already imported on " + Utils.shortTimeDate(addedToLibrary, false);
                    groupBox1.BackColor = Color.YellowGreen;
                    break;
                case ImportStatus.NeedsLevelChosing:
                    lbStatus.Text = "Cannot be extracted automatically, you need to choose from level combinations";
                    groupBox1.BackColor = Color.Yellow;
                    break;
            }
        }

        public void DoCheckGuidInLibrary()
        {
            CheckGuidInLibrary?.Invoke(this);
        }

        public enum ImportStatus
        {
            NeedsLevelChosing,
            NotImported,
            JustImported,
            OldImported
        }
    }
}
