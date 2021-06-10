using System;
using System.Windows.Forms;

namespace ARKBreedingStats.library
{
    public partial class AddDummyCreaturesSettings : Form
    {
        public AddDummyCreaturesSettings()
        {
            InitializeComponent();
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtOk_Click(object sender, EventArgs e)
        {
            if (CreatureCount > 0)
                DialogResult = DialogResult.OK;
            Close();
        }

        public bool OnlySelectedSpecies => CbOnlySelectedSpecies.Checked;
        public int CreatureCount => (int)NudAmount.Value;
        public int SpeciesCount => (int)NudSpeciesAmount.Value;
    }
}
