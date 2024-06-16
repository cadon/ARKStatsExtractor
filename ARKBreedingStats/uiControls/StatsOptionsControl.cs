using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.library;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.uiControls
{
    public partial class StatsOptionsControl : UserControl
    {
        private StatOptionsControl[] _statOptionsControls;
        private readonly ToolTip _tt = new ToolTip();
        private StatsOptions _selectedStatsOptions;
        private Species _species;

        public StatsOptionsControl()
        {
            InitializeComponent();
            InitializeStatControls();
            InitButtonImages();
        }

        private void InitializeStatControls()
        {
            _statOptionsControls = new StatOptionsControl[Stats.StatsCount];
            foreach (var si in Stats.DisplayOrder)
            {
                var c = new StatOptionsControl($"[{si}]{Utils.StatName(si, true)}", si, _tt);
                _statOptionsControls[si] = c;
                flpStatControls.Controls.Add(c);
                flpStatControls.SetFlowBreak(c, true);
            }
            flpStatControls.Controls.Add(new Label
            {
                Text = @"Drag color gradient with mouse for fast editing.
On color gradients use shift + right click to copy and shift + left click to paste color settings.
Ctrl + left click to reset colors.",
                AutoSize = true
            });
            _tt.SetToolTip(BtNew, "Create new setting");
            _tt.SetToolTip(BtRemove, "Delete setting");
        }

        public void InitializeOptions()
        {
            _selectedStatsOptions = null;
            CbbOptions.Items.Clear();
            CbbParent.Items.Clear();

            var statsOptions = StatsOptions.StatsOptionsDict.Values.OrderBy(n => n.Name).ToArray();
            CbbOptions.Items.AddRange(statsOptions);
            CbbParent.Items.AddRange(statsOptions);
            if (CbbOptions.Items.Count > 0)
                CbbOptions.SelectedIndex = 0;
        }

        private void CbbOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedStatsOptions = CbbOptions.SelectedItem as StatsOptions;
            if (_selectedStatsOptions == null) return;

            this.SuspendDrawing();
            TbOptionsName.Text = _selectedStatsOptions.ToString();
            var isNotRoot = _selectedStatsOptions.Name != string.Empty;
            TbOptionsName.Enabled = isNotRoot;
            LbParent.Visible = isNotRoot;
            CbbParent.Visible = isNotRoot;
            BtRemove.Visible = isNotRoot;
            for (var si = 0; si < Stats.StatsCount; si++)
                _statOptionsControls[si].SetStatOptions(_selectedStatsOptions.StatOptions?[si], isNotRoot, _selectedStatsOptions.ParentOptions);

            CbbParent.SelectedItem = _selectedStatsOptions.ParentOptions;
            this.ResumeDrawing();
        }

        private void CbbParent_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedStatsOptions = CbbOptions.SelectedItem as StatsOptions;
            if (_selectedStatsOptions == null) return;
            _selectedStatsOptions.ParentOptions = CbbParent.SelectedItem as StatsOptions;
        }

        private void TbOptionsName_Leave(object sender, EventArgs e)
        {
            var newNameBase = TbOptionsName.Text;
            if (_selectedStatsOptions.Name == newNameBase) return; // nothing to change
            var newName = newNameBase;
            var suffix = 1;
            while (StatsOptions.StatsOptionsDict.ContainsKey(newName))
                newName = newNameBase + "_" + ++suffix;

            TbOptionsName.Text = newName;
            StatsOptions.StatsOptionsDict.Remove(_selectedStatsOptions.Name);
            _selectedStatsOptions.Name = newName;
            StatsOptions.StatsOptionsDict.Add(newName, _selectedStatsOptions);
            // update text in combobox
            CbbOptions.Items[CbbOptions.SelectedIndex] = _selectedStatsOptions;
        }

        private void BtNew_Click(object sender, EventArgs e)
        {
            var newNameBase = _species?.name ?? "new entry";
            var newName = newNameBase;
            var suffix = 1;
            while (StatsOptions.StatsOptionsDict.ContainsKey(newName))
                newName = newNameBase + "_" + ++suffix;
            var newSettings = StatsOptions.GetDefaultStatOptions(newName);
            StatsOptions.StatsOptionsDict.Add(newName, newSettings);
            InitializeOptions();
            CbbOptions.SelectedItem = newSettings;
        }

        private void BtRemove_Click(object sender, EventArgs e)
        {
            if (_selectedStatsOptions == null
                || MessageBox.Show("Delete stat options\n" + _selectedStatsOptions + "\n?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    != DialogResult.Yes) return;

            var index = CbbOptions.SelectedIndex;
            // set parent of dependant options to parent of this setting
            foreach (var so in StatsOptions.StatsOptionsDict.Values)
            {
                if (so.ParentOptions == _selectedStatsOptions)
                    so.ParentOptions = _selectedStatsOptions.ParentOptions;
            }

            StatsOptions.StatsOptionsDict.Remove(_selectedStatsOptions.Name);

            InitializeOptions();
            if (CbbOptions.Items.Count > 0)
                CbbOptions.SelectedIndex = Math.Max(0, index - 1); // select item before deleted one
        }

        public void SetSpecies(Species s)
        {
            _species = s;
            if (_species == null) return;
            var autoCompleteList = new AutoCompleteStringCollection();
            autoCompleteList.AddRange(new[]
            {
                _species.name,
                _species.DescriptiveName,
                _species.DescriptiveNameAndMod,
                _species.blueprintPath
            });

            TbOptionsName.AutoCompleteCustomSource = autoCompleteList;
        }

        private void InitButtonImages()
        {
            const int size = 12;
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            using (var p = new Pen(Brushes.DarkGreen))
            {
                g.DrawRectangle(p, size / 3, 0, size / 3 - 1, size - 1);
                g.DrawRectangle(p, 0, size / 3, size - 1, size / 3 - 1);
                g.FillRectangle(Brushes.LightGreen, size / 3 + 1, 1, size / 3 - 2, size - 2);
                g.FillRectangle(Brushes.LightGreen, 1, size / 3 + 1, size - 2, size / 3 - 2);
            }
            BtNew.Image = bmp;

            bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            using (var p = new Pen(Brushes.DarkRed))
            {
                g.DrawRectangle(p, 0, size / 3, size - 1, size / 3 - 1);
                g.FillRectangle(Brushes.LightPink, 1, size / 3 + 1, size - 2, size / 3 - 2);
            }
            BtRemove.Image = bmp;
        }
    }
}
