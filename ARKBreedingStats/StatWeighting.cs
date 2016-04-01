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
    public partial class StatWeighting : UserControl
    {
        public StatWeighting()
        {
            InitializeComponent();
            ToolTip tt = new ToolTip();
            tt.SetToolTip(groupBox1, "Increase the weights for stats that are more important to you to be high in the offspring.\nRightclick for Suggestions.");
        }

        public double[] Weightings
        {
            get
            {
                double[] w = new double[] { (double)numericUpDown1.Value,
                    (double)numericUpDown2.Value,
                    (double)numericUpDown3.Value,
                    (double)numericUpDown4.Value,
                    (double)numericUpDown5.Value,
                    (double)numericUpDown6.Value,
                    (double)numericUpDown7.Value};
                double s = w.Sum() / 7;
                if (s > 0)
                {
                    for (int i = 0; i < 7; i++)
                        w[i] /= s;
                }
                return w;
            }
        }

        public double[] Values
        {
            set
            {
                if (value.Length > 6)
                {
                    numericUpDown1.Value = (decimal)value[0];
                    numericUpDown2.Value = (decimal)value[1];
                    numericUpDown3.Value = (decimal)value[2];
                    numericUpDown4.Value = (decimal)value[3];
                    numericUpDown5.Value = (decimal)value[4];
                    numericUpDown6.Value = (decimal)value[5];
                    numericUpDown7.Value = (decimal)value[6];
                }
            }
            get
            {
                return new double[] { (double)numericUpDown1.Value,
                    (double)numericUpDown2.Value,
                    (double)numericUpDown3.Value,
                    (double)numericUpDown4.Value,
                    (double)numericUpDown5.Value,
                    (double)numericUpDown6.Value,
                    (double)numericUpDown7.Value};
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

        private void setAllWeightsTo1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = 1;
            numericUpDown2.Value = 1;
            numericUpDown3.Value = 1;
            numericUpDown4.Value = 1;
            numericUpDown5.Value = 1;
            numericUpDown6.Value = 1;
            numericUpDown7.Value = 1;
        }

        private void focusOnHPAndMeleeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = 1;
            numericUpDown2.Value = (decimal).1;
            numericUpDown3.Value = (decimal).1;
            numericUpDown4.Value = (decimal).1;
            numericUpDown5.Value = (decimal).1;
            numericUpDown6.Value = 1;
            numericUpDown7.Value = (decimal).1;
        }

        private void focusOnHPDmBitWeAndStToolStripMenuItem_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = 1;
            numericUpDown2.Value = (decimal).5;
            numericUpDown3.Value = (decimal).1;
            numericUpDown4.Value = (decimal).1;
            numericUpDown5.Value = (decimal).5;
            numericUpDown6.Value = 1;
            numericUpDown7.Value = (decimal).1;
        }
    }
}
