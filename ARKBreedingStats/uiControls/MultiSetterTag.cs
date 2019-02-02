using System;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class MultiSetterTag : UserControl
    {
        public MultiSetterTag()
        {
            InitializeComponent();
        }

        public MultiSetterTag(string tag)
        {
            InitializeComponent();
            cbTagChecked.Text = tag;
            cbTagChecked.ForeColor = SystemColors.GrayText;
        }

        public bool Considered
        {
            get => cbConsider.Checked;
            set => cbConsider.Checked = value;
        }

        public CheckState TagCheckState
        {
            get => cbTagChecked.CheckState;
            set => cbTagChecked.CheckState = value;
        }

        public string TagName
        {
            get => cbTagChecked.Text;
            set => cbTagChecked.Text = value;
        }

        private void cbConsider_CheckedChanged(object sender, EventArgs e)
        {
            cbTagChecked.ForeColor = Considered ? SystemColors.ControlText : SystemColors.GrayText;
        }

        private void cbTagChecked_CheckedChanged(object sender, EventArgs e)
        {
            cbConsider.Checked = true;
        }
    }
}
