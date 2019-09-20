using ARKBreedingStats.Library;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ARKBreedingStats.duplicates
{
    public partial class MergingDuplicatesWindow : Form
    {
        private readonly MergingDuplicates mergingDuplicates;
        public delegate void RefreshLibraryEventHandler();
#pragma warning disable 67
        public event RefreshLibraryEventHandler RefreshLibrary;
#pragma warning restore 67

        public MergingDuplicatesWindow()
        {
            InitializeComponent();
            mergingDuplicates = new MergingDuplicates
            {
                progressBar = progressBar1
            };
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
