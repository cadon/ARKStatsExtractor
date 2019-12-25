using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats.mods
{
    public partial class StatBaseValuesEdit : UserControl
    {
        public StatBaseValuesEdit()
        {
            InitializeComponent();
            SetEnabled(false);
        }

        private void cbOverride_CheckedChanged(object sender, EventArgs e)
        {
            SetEnabled(cbOverride.Checked);
        }

        private void SetEnabled(bool enabled)
        {
            nudBase.Enabled = enabled;
            nudIw.Enabled = enabled;
            nudId.Enabled = enabled;
            nudTa.Enabled = enabled;
            nudTm.Enabled = enabled;
        }

        public string StatName
        {
            set => cbOverride.Text = "Override " + value;
        }

        internal double[] Overrides
        {
            set
            {
                if (value == null || value.Length < 5)
                {
                    cbOverride.Checked = false;
                    return;
                }
                cbOverride.Checked = true;
                nudBase.ValueSave = (decimal)value[0];
                nudIw.ValueSave = (decimal)value[1];
                nudId.ValueSave = (decimal)value[2];
                nudTa.ValueSave = (decimal)value[3];
                nudTm.ValueSave = (decimal)value[4];
            }
            get
            {
                if (!cbOverride.Checked) return null;
                return new double[]{
                    (double)nudBase.Value,
                    (double)nudIw.Value,
                    (double)nudId.Value,
                    (double)nudTa.Value,
                    (double)nudTm.Value
                };
            }
        }
    }
}
