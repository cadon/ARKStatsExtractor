using System.Windows.Forms;

namespace ARKBreedingStats.settings
{
    public partial class MultiplierSetting : UserControl
    {
        public MultiplierSetting()
        {
            InitializeComponent();
        }

        public string StatName { set { labelStatName.Text = value; } }

        public double[] Multipliers
        {
            get { return new double[] { (double)numericUpDownTameAdd.Value, (double)numericUpDownTameMult.Value, (double)numericUpDownDomLevel.Value, (double)numericUpDownWildLevel.Value }; }
            set
            {
                if (value.Length > 3)
                {
                    numericUpDownTameAdd.Value = (decimal)value[0];
                    numericUpDownTameMult.Value = (decimal)value[1];
                    numericUpDownWildLevel.Value = (decimal)value[3];
                    numericUpDownDomLevel.Value = (decimal)value[2];
                }
            }
        }
    }
}
