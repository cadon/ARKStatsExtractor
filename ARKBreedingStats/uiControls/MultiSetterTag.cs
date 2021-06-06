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

        public CheckState TagCheckState
        {
            get => cbTagChecked.CheckState;
            set => cbTagChecked.CheckState = value;
        }

        public string TagName => cbTagChecked.Text;
    }
}
