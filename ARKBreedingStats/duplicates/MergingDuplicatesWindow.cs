using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ARKBreedingStats.duplicates
{
    public partial class MergingDuplicatesWindow : Form
    {
        private MergingDuplicates mergingDuplicates;
        public delegate void RefreshLibraryEventHandler();
        public event RefreshLibraryEventHandler RefreshLibrary;

        public MergingDuplicatesWindow()
        {
            InitializeComponent();
            mergingDuplicates = new MergingDuplicates();
            mergingDuplicates.progressBar = progressBar1;
        }

        public void CheckForDuplicates(List<Creature> creatureList)
        {
            mergingDuplicates.CheckForDuplicates(creatureList);
        }

        private void btUseLeft_Click(object sender, EventArgs e)
        {

        }

        private void btUseRight_Click(object sender, EventArgs e)
        {

        }

        private void btKeepBoth_Click(object sender, EventArgs e)
        {

        }
    }
}
