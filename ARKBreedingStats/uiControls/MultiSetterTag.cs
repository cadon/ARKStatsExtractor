using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            get { return cbConsider.Checked; }
            set { cbConsider.Checked = value; }
        }

        public bool TagChecked
        {
            get { return cbTagChecked.Checked; }
            set { cbTagChecked.Checked = value; }
        }

        public string TagName
        {
            get { return cbTagChecked.Text; }
            set { cbTagChecked.Text = value; }
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
