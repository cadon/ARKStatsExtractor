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

        internal void SetOverrides(double[] statOverrides, bool isOverridden = true)
        {
            if (statOverrides == null || statOverrides.Length < 5)
            {
                cbOverride.Checked = false;
                nudBase.ValueSave = 0;
                nudIw.ValueSave = 0;
                nudId.ValueSave = 0;
                nudTa.ValueSave = 0;
                nudTm.ValueSave = 0;
                return;
            }
            cbOverride.Checked = isOverridden;
            nudBase.ValueSave = (decimal)statOverrides[0];
            nudIw.ValueSave = (decimal)statOverrides[1];
            nudId.ValueSave = (decimal)statOverrides[2];
            nudTa.ValueSave = (decimal)statOverrides[3];
            nudTm.ValueSave = (decimal)statOverrides[4];
        }

        internal double[] Overrides
        {
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
