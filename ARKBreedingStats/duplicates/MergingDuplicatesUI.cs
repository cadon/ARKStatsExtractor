using System;
using System.Windows.Forms;

namespace ARKBreedingStats.duplicates
{
    public partial class MergingDuplicatesUI : UserControl
    {
        private MergingDuplicates mergingDuplicates;

        public MergingDuplicatesUI()
        {
            InitializeComponent();
            mergingDuplicates = new MergingDuplicates();
        }

        private void btUseLeft_Click(object sender, EventArgs e) { }

        private void btUseRight_Click(object sender, EventArgs e) { }

        private void btKeepBoth_Click(object sender, EventArgs e) { }
    }
}
