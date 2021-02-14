using System.Windows.Forms;

namespace ARKBreedingStats.settings
{
    public partial class MultiplierSetting : UserControl
    {
        public MultiplierSetting()
        {
            InitializeComponent();
        }

        public string StatName { set => labelStatName.Text = value; }

        public double[] Multipliers
        {
            get => new[] { (double)nudTameAdd.Value, (double)nudTameMult.Value, (double)nudDomLevel.Value, (double)nudWildLevel.Value };
            set
            {
                if (value?.Length == 4)
                {
                    nudTameAdd.ValueSave = (decimal)value[0];
                    nudTameMult.ValueSave = (decimal)value[1];
                    nudDomLevel.ValueSave = (decimal)value[2];
                    nudWildLevel.ValueSave = (decimal)value[3];
                }
                else
                {
                    nudTameAdd.ValueSave = 1;
                    nudTameMult.ValueSave = 1;
                    nudWildLevel.ValueSave = 1;
                    nudDomLevel.ValueSave = 1;
                }
            }
        }

        /// <summary>
        /// Set the values that are considered default and are a bit lowlighted.
        /// </summary>
        public void SetNeutralValues(double[] nv)
        {
            if (nv?.Length == 4)
            {
                nudTameAdd.NeutralNumber = (decimal)nv[0];
                nudTameMult.NeutralNumber = (decimal)nv[1];
                nudDomLevel.NeutralNumber = (decimal)nv[2];
                nudWildLevel.NeutralNumber = (decimal)nv[3];
            }
            else
            {
                nudTameAdd.NeutralNumber = 1;
                nudTameMult.NeutralNumber = 1;
                nudDomLevel.NeutralNumber = 1;
                nudWildLevel.NeutralNumber = 1;
            }
        }

        public void SetHighlighted(bool highlighted)
        {
            nudTameAdd.SetExtraHighlightNonDefault(highlighted);
            nudTameMult.SetExtraHighlightNonDefault(highlighted);
            nudDomLevel.SetExtraHighlightNonDefault(highlighted);
            nudWildLevel.SetExtraHighlightNonDefault(highlighted);
        }
    }
}
