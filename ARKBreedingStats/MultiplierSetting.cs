using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
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

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            if (n != null)
            {
                n.Select(0, n.Text.Length);
            }
        }
    }
}
