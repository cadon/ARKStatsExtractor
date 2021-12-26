using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.species;

namespace ARKBreedingStats.uiControls
{
    public partial class VariantSelector : Form
    {
        internal List<string> DisabledVariants;

        public VariantSelector()
        {
            InitializeComponent();
            CheckBoxAll.Text = Loc.S("All");
            Text = Loc.S("VariantSelector");
            ButtonCancel.Text = Loc.S("Cancel");
            ButtonOk.Text = Loc.S("OK");
            label1.Text = Loc.S("VariantSelectorInfo");
            DisabledVariants = new List<string>();
        }

        /// <summary>
        /// Call this method before the form is shown to set the checkboxes to their correct state and the location to the cursor.
        /// </summary>
        internal void InitializeCheckStates()
        {
            var checkAll = DisabledVariants == null || !DisabledVariants.Any();
            int c = ClbVariants.Items.Count;
            for (int i = 0; i < c; i++)
                ClbVariants.SetItemChecked(i, checkAll || !DisabledVariants.Contains(ClbVariants.Items[i].ToString()));

            SetDesktopLocation(Cursor.Position.X, Cursor.Position.Y);
        }

        private void CheckBoxAll_CheckedChanged(object sender, EventArgs e)
        {
            var isChecked = CheckBoxAll.Checked;
            int c = ClbVariants.Items.Count;
            for (int i = 0; i < c; i++)
                ClbVariants.SetItemChecked(i, isChecked);
        }

        internal void SetVariants(List<Species> species)
        {
            var variantCount = ClbVariants.Items.Count;
            if (variantCount == 0)
            {
                DisabledVariants = Properties.Settings.Default.DisabledVariants?.ToList() ?? DefaultVariantDeselection();
            }
            else
            {
                DisabledVariants = new List<string>();
                for (int i = 0; i < variantCount; i++)
                {
                    var v = ClbVariants.Items[i].ToString();
                    if (!ClbVariants.GetItemChecked(i) && !DisabledVariants.Contains(v))
                        DisabledVariants.Add(v);
                }
            }

            ClbVariants.Items.Clear();
            // get all variants
            var variants = species.Where(s => s.variants != null).SelectMany(s => s.variants).Distinct().OrderBy(s => s).ToList();
            variants.Insert(0, string.Empty);

            var checkAll = DisabledVariants == null || !DisabledVariants.Any();
            foreach (var v in variants)
                ClbVariants.Items.Add(v, checkAll || !DisabledVariants.Contains(v));
        }

        internal void FilterToDefault()
        {
            DisabledVariants = DefaultVariantDeselection();
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            if (DisabledVariants == null)
                DisabledVariants = new List<string>();
            else DisabledVariants.Clear();
            int c = ClbVariants.Items.Count;
            for (int i = 0; i < c; i++)
                if (!ClbVariants.GetItemChecked(i))
                    DisabledVariants.Add(ClbVariants.Items[i].ToString());

            DialogResult = DialogResult.OK;
            Close();
        }

        private static List<string> DefaultVariantDeselection()
        {
            var filePath = FileService.GetJsonPath("variantsDefaultUnselected.txt");
            if (!File.Exists(filePath)) return null;

            return File.ReadAllLines(filePath).Where(l => !string.IsNullOrEmpty(l)).ToList();
        }
    }
}
