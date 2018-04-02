using ARKBreedingStats.species;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class ExportedCreatureControl : UserControl
    {
        public delegate void CopyValuesToExtractorEventHandler(ExportedCreatureControl exportedCreatureControl);
        public event CopyValuesToExtractorEventHandler CopyValuesToExtractor;
        public delegate void CheckGuidInLibraryEventHandler(ExportedCreatureControl exportedCreatureControl);
        public event CheckGuidInLibraryEventHandler CheckGuidInLibrary;
        public CreatureValues creatureValues;

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
            CopyValuesToExtractor?.Invoke(this);
        }

        public void setStatus(bool extracted, DateTime addedToLibrary)
        {
            if (extracted)
            {
                // if extracted in this session
                lbStatus.Text = "Values extracted and creature added to library";
                lbStatus.BackColor = Color.LightGreen;
            }
            else
            {
                if (addedToLibrary != null && addedToLibrary.Year > 1)
                {
                    lbStatus.Text = "Already imported on " + Utils.shortTimeDate(addedToLibrary, false);
                    lbStatus.BackColor = Color.YellowGreen;
                }
                else
                {
                    lbStatus.Text = "Not yet extracted";
                    lbStatus.BackColor = Color.Transparent;
                }
            }
        }

        public void DoCheckGuidInLibrary()
        {
            CheckGuidInLibrary?.Invoke(this);
        }
    }
}
