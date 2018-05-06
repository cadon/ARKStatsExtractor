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
            get { return new double[] { (double)nudTameAdd.Value, (double)nudTameMult.Value, (double)nudDomLevel.Value, (double)nudWildLevel.Value }; }
            set
            {
                if (value.Length > 3)
                {
                    nudTameAdd.ValueSave = (decimal)value[0];
                    nudTameMult.ValueSave = (decimal)value[1];
                    nudWildLevel.ValueSave = (decimal)value[3];
                    nudDomLevel.ValueSave = (decimal)value[2];
                }
            }
        }

        public void setNeutralValues(double[] nv)
        {
            if (nv.Length == 4)
            {
                nudTameAdd.NeutralNumber = (decimal)nv[0];
                nudTameMult.NeutralNumber = (decimal)nv[1];
                nudDomLevel.NeutralNumber = (decimal)nv[2];
                nudWildLevel.NeutralNumber = (decimal)nv[3];
            }
        }
    }
}
