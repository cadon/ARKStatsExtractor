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
            SetStatOverrideEnabled(false);
            SetImprintingOverrideEnabled(false);
        }

        private void cbOverride_CheckedChanged(object sender, EventArgs e)
        {
            SetStatOverrideEnabled(cbOverride.Checked);
        }

        private void SetStatOverrideEnabled(bool enabled)
        {
            nudBase.Enabled = enabled;
            nudIw.Enabled = enabled;
            nudId.Enabled = enabled;
            nudTa.Enabled = enabled;
            nudTm.Enabled = enabled;
        }

        private void cbImprintingOverride_CheckedChanged(object sender, EventArgs e)
        {
            SetImprintingOverrideEnabled(cbImprintingOverride.Checked);
        }

        private void SetImprintingOverrideEnabled(bool enabled)
        {
            nudImprintingOverride.Enabled = enabled;
        }

        public void SetStatNameByIndex(int statIndex, Dictionary<string, string> customStatNames = null) => StatName = $"[{statIndex}] {Utils.StatName(statIndex, false, customStatNames)}";

        public string StatName
        {
            set => cbOverride.Text = value;
        }

        internal void SetStatOverrides(double[] defaultValues, double?[] statOverrides = null)
        {
            if (statOverrides == null || statOverrides.Length < 5)
            {
                cbOverride.Checked = false;
                nudBase.ValueSave = (decimal)(defaultValues?[0] ?? 0);
                nudIw.ValueSave = (decimal)(defaultValues?[1] ?? 0);
                nudId.ValueSave = (decimal)(defaultValues?[2] ?? 0);
                nudTa.ValueSave = (decimal)(defaultValues?[3] ?? 0);
                nudTm.ValueSave = (decimal)(defaultValues?[4] ?? 0);
                return;
            }
            cbOverride.Checked = true;
            nudBase.ValueSave = (decimal)(statOverrides[0] ?? defaultValues?[0] ?? 0);
            nudIw.ValueSave = (decimal)(statOverrides[1] ?? defaultValues?[1] ?? 0);
            nudId.ValueSave = (decimal)(statOverrides[2] ?? defaultValues?[2] ?? 0);
            nudTa.ValueSave = (decimal)(statOverrides[3] ?? defaultValues?[3] ?? 0);
            nudTm.ValueSave = (decimal)(statOverrides[4] ?? defaultValues?[4] ?? 0);
        }

        internal double?[] StatOverrides
        {
            get
            {
                if (!cbOverride.Checked) return null;
                return new double?[]{
                    (double)nudBase.Value,
                    (double)nudIw.Value,
                    (double)nudId.Value,
                    (double)nudTa.Value,
                    (double)nudTm.Value
                };
            }
        }

        internal double? ImprintingOverride
        {
            get => cbImprintingOverride.Checked ? (double)nudImprintingOverride.Value : default(double?);
        }

        internal void SetImprintingMultiplierOverride(double defaultImprintingMultiplier, double? overrideValue)
        {
            cbImprintingOverride.Checked = overrideValue.HasValue && overrideValue.Value != defaultImprintingMultiplier;
            nudImprintingOverride.ValueSave = (decimal)(overrideValue ?? defaultImprintingMultiplier);
        }
    }
}
