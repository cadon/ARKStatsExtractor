using System;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;
using System.ComponentModel;

namespace ARKBreedingStats.BreedingPlanning
{
    public partial class ColorPatternForm : Form
    {
        public Action<ColorPattern> ColorPatternChanged;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Species Species
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                coloredCreatureImageWithPose1.SetCreatureImage(Species, regionColorChooser1.ColorIds);
                regionColorChooser1.SetSpecies(value, showAllRegions: true);
            }
        }

        private const string FileNamePatterns = "colorPatterns.json";
        private BindingList<ColorPattern> _speciesColorPatterns;
        private readonly CheckBox[] _cbConsideredRegions;

        public ColorPatternForm()
        {
            InitializeComponent();
            _cbConsideredRegions = [CbRegion0, CbRegion1, CbRegion2, CbRegion3, CbRegion4, CbRegion5];
            LbColorPatterns.SelectedIndexChanged += LbColorPatterns_SelectedIndexChanged;
            regionColorChooser1.RegionColorChosen += RegionColorChooser1_RegionColorChosen;
            FormClosing += ColorPatternForm_FormClosing;
        }

        public ColorPatternForm(BindingList<ColorPattern> colorPatterns, ColorPattern selectedColorPattern) : this()
        {
            SetPatterns(colorPatterns, selectedColorPattern);
        }

        private void ColorPatternForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            (Properties.Settings.Default.ColorPatternsWindowRect, _) = Utils.GetWindowRectangle(this);
            SavePatterns();
        }

        private void RegionColorChooser1_RegionColorChosen(bool colorChanged, int regionId)
        {
            if (!colorChanged) return;
            coloredCreatureImageWithPose1.SetCreatureImage(Species, regionColorChooser1.ColorIds);
            if (regionId >= 0 && regionId < _cbConsideredRegions.Length)
                _cbConsideredRegions[regionId].Checked = true;
        }

        public static BindingList<ColorPattern> LoadPatterns()
        {
            var filePath = FileService.GetJsonPath(FileNamePatterns);
            try
            {
                return FileService.LoadJsonFileIfAvailable<BindingList<ColorPattern>>(filePath);
            }
            catch (Exception ex)
            {
                MessageBoxes.ExceptionMessageBox(ex, $"Error when loading the species color patterns from the file {filePath}");
            }

            return null;
        }

        public void SetPatterns(BindingList<ColorPattern> colorPatterns, ColorPattern selectedColorPattern)
        {
            _speciesColorPatterns = colorPatterns;
            LbColorPatterns.BeginUpdate();
            LbColorPatterns.SelectedIndexChanged -= LbColorPatterns_SelectedIndexChanged;
            LbColorPatterns.DataSource = _speciesColorPatterns;
            LbColorPatterns.SelectedIndexChanged += LbColorPatterns_SelectedIndexChanged;
            LbColorPatterns.SelectedIndex = _speciesColorPatterns?.IndexOf(selectedColorPattern) ?? -1;
            LbColorPatterns.EndUpdate();
        }

        private void SavePatterns()
        {
            if (_speciesColorPatterns == null) return;
            var filePath = FileService.GetJsonPath(FileNamePatterns);
            FileService.SaveJsonFile(filePath, _speciesColorPatterns, out var error);
            if (!string.IsNullOrEmpty(error))
                MessageBoxes.ShowMessageBox($"Error when saving species color patterns to file {filePath}." + Environment.NewLine + error);
        }

        private void BtDelete_Click(object sender, EventArgs e)
        {
            if (LbColorPatterns.SelectedItem is not ColorPattern selectedColorPattern) return;
            if (MessageBox.Show($"Delete the color pattern {selectedColorPattern.Name}?", "Delete?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            var index = LbColorPatterns.SelectedIndex;
            _speciesColorPatterns.Remove(selectedColorPattern);
            LbColorPatterns.SelectedIndex = -1;
            var itemCount = LbColorPatterns.Items.Count;
            if (itemCount > 0)
                LbColorPatterns.SelectedIndex = Math.Min(itemCount - 1, index);
        }

        private void BtSave_Click(object sender, EventArgs e)
        {
            if (LbColorPatterns.SelectedItem is not ColorPattern selectedColorPattern)
            {
                AddNewPattern();
                return;
            }
            selectedColorPattern.ColorIds = regionColorChooser1.ColorIds.ToArray();
            selectedColorPattern.ConsideredRegions = [CbRegion0.Checked, CbRegion1.Checked, CbRegion2.Checked, CbRegion3.Checked, CbRegion4.Checked, CbRegion5.Checked];
            selectedColorPattern.Name = TbPatternName.Text; // this needs to be called last because it will trigger indexChanged on ListBox due to binding
            ColorPatternChanged?.Invoke(selectedColorPattern);
        }

        private void BtSaveAsNewPattern_Click(object sender, EventArgs e)
        {
            AddNewPattern();
        }

        /// <summary>
        /// Adds new color pattern using the currently selected colors.
        /// </summary>
        private void AddNewPattern()
        {
            var colorPatternCopy = new ColorPattern
            {
                Name = TbPatternName.Text,
                ColorIds = regionColorChooser1.ColorIds.ToArray(),
                ConsideredRegions = [CbRegion0.Checked, CbRegion1.Checked, CbRegion2.Checked, CbRegion3.Checked, CbRegion4.Checked, CbRegion5.Checked]
            };
            _speciesColorPatterns.Add(colorPatternCopy);
            LbColorPatterns.SelectedItem = colorPatternCopy;
        }

        private void LbColorPatterns_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedColorPattern = LbColorPatterns.SelectedItem as ColorPattern;
            regionColorChooser1.ColorIds = selectedColorPattern?.ColorIds;
            TbPatternName.Text = selectedColorPattern?.Name ?? string.Empty;
            var i = 0;
            foreach (var cb in _cbConsideredRegions)
                cb.Checked = selectedColorPattern?.ConsideredRegions?[i++] != false;
            ColorPatternChanged?.Invoke(selectedColorPattern);
        }

        private void BtAllRegionColors_Click(object sender, EventArgs e)
        {
            regionColorChooser1.ChooseAllColors();
        }

        private void CbRegionAll_CheckedChanged(object sender, EventArgs e)
        {
            var setToChecked = CbRegionAll.Checked;
            foreach (var cb in _cbConsideredRegions) cb.Checked = setToChecked;
        }
    }
}
