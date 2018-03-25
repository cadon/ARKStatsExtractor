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

        public bool Status
        {
            set
            {
                if (value)
                {
                    lbStatus.Text = "Values extracted and creature added to library";
                    lbStatus.BackColor = Color.LightGreen;
                }
                else
                {
                    lbStatus.Text = "Not yet extracted";
                    lbStatus.BackColor = Color.Transparent;
                }

            }
        }
    }
}
