using ARKBreedingStats.species;
using ARKBreedingStats.utils;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.StatsOptions;

namespace ARKBreedingStats.uiControls
{
    internal class LevelGraphOptionsControl : TableLayoutPanel
    {
        private StatLevelGraphOptionsControl[] _statOptionsControls;
        private readonly ToolTip _tt = new ToolTip();
        private StatsOptions<StatLevelColors> _selectedStatsOptions;
        private readonly StatsOptionsSettings<StatLevelColors> _statsOptionsSettings;
        private Species _species;
        private ComboBox _cbbOptions;
        private ComboBox _cbbParent;
        private Button _btRemove;
        private TextBox _tbOptionsName;
        private Label _lbParent;
        public static Form DisplayedForm;

        public static void ShowWindow(Form parent, StatsOptionsSettings<StatLevelColors> settings)
        {
            if (DisplayedForm != null)
            {
                DisplayedForm.BringToFront();
                return;
            }

            var so = new LevelGraphOptionsControl(settings);
            var f = new Form
            {
                FormBorderStyle = FormBorderStyle.SizableToolWindow,
                Width = Properties.Settings.Default.LevelColorWindowRectangle.Width,
                Height = Properties.Settings.Default.LevelColorWindowRectangle.Height,
                StartPosition = FormStartPosition.Manual,
                Location = new Point(Properties.Settings.Default.LevelColorWindowRectangle.X, Properties.Settings.Default.LevelColorWindowRectangle.Y)
            };
            f.Controls.Add(so);
            so.Dock = DockStyle.Fill;
            DisplayedForm = f;
            f.Closed += F_Closed;
            f.Show(parent);
        }

        private static void F_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.LevelColorWindowRectangle =
                new Rectangle(DisplayedForm.Left, DisplayedForm.Top, DisplayedForm.Width, DisplayedForm.Height);
            DisplayedForm = null;
        }

        public LevelGraphOptionsControl(StatsOptionsSettings<StatLevelColors> settings)
        {
            _statsOptionsSettings = settings;
            InitializeControls();
            InitializeOptions();
        }

        private void InitializeControls()
        {
            AutoScroll = true;
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            RowStyles.Add(new RowStyle(SizeType.AutoSize));
            RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var flpHeaderControls = new FlowLayoutPanel { Dock = DockStyle.Fill };
            Controls.Add(flpHeaderControls, 0, 0);
            var flpStatControls = new FlowLayoutPanel { Dock = DockStyle.Fill };
            Controls.Add(flpStatControls, 0, 1);

            var btNew = new Button { Width = 20, Height = 20 };
            _btRemove = new Button { Width = 20, Height = 20 };
            flpHeaderControls.Controls.Add(btNew);
            flpHeaderControls.Controls.Add(_btRemove);
            btNew.Click += BtNew_Click;
            _btRemove.Click += BtRemove_Click;
            _tt.SetToolTip(btNew, "Create new setting");
            _tt.SetToolTip(_btRemove, "Delete setting");
            InitButtonImages(btNew, _btRemove);

            _cbbOptions = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            _cbbOptions.SelectedIndexChanged += CbbOptions_SelectedIndexChanged;
            flpHeaderControls.Controls.Add(_cbbOptions);

            _tbOptionsName = new TextBox();
            flpHeaderControls.Controls.Add(_tbOptionsName);
            _tbOptionsName.Leave += TbOptionsName_Leave;

            _lbParent = new Label { Text = "depends on", Margin = new Padding(5, 7, 5, 0), AutoSize = true };
            flpHeaderControls.Controls.Add(_lbParent);

            _cbbParent = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            _cbbParent.SelectedIndexChanged += CbbParent_SelectedIndexChanged;
            flpHeaderControls.Controls.Add(_cbbParent);

            InitializeStatControls(flpStatControls);
        }

        private void InitializeStatControls(FlowLayoutPanel flpStatControls)
        {
            _statOptionsControls = new StatLevelGraphOptionsControl[Stats.StatsCount];
            foreach (var si in Stats.DisplayOrder)
            {
                var c = new StatLevelGraphOptionsControl($"[{si}] {Utils.StatName(si, true)}", si, _tt);
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
        }

        public void InitializeOptions()
        {
            _selectedStatsOptions = null;
            _cbbOptions.Items.Clear();
            _cbbParent.Items.Clear();

            var statsOptions = _statsOptionsSettings.StatsOptionsDict.Values.OrderBy(n => n.Name).ToArray();
            _cbbOptions.Items.AddRange(statsOptions);
            _cbbParent.Items.AddRange(statsOptions);
            if (_cbbOptions.Items.Count > 0)
                _cbbOptions.SelectedIndex = 0;
        }

        private void CbbOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedStatsOptions = _cbbOptions.SelectedItem as StatsOptions<StatLevelColors>;
            if (_selectedStatsOptions == null) return;

            this.SuspendDrawing();
            _tbOptionsName.Text = _selectedStatsOptions.ToString();
            var isNotRoot = _selectedStatsOptions.Name != string.Empty;
            _tbOptionsName.Enabled = isNotRoot;
            _lbParent.Visible = isNotRoot;
            _cbbParent.Visible = isNotRoot;
            _btRemove.Visible = isNotRoot;
            for (var si = 0; si < Stats.StatsCount; si++)
                _statOptionsControls[si].SetStatOptions(_selectedStatsOptions.StatOptions?[si], isNotRoot, _selectedStatsOptions.ParentOptions);

            _cbbParent.SelectedItem = _selectedStatsOptions.ParentOptions;
            this.ResumeDrawing();
        }

        private void CbbParent_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedStatsOptions = _cbbOptions.SelectedItem as StatsOptions<StatLevelColors>;
            if (_selectedStatsOptions == null) return;
            _selectedStatsOptions.ParentOptions = _cbbParent.SelectedItem as StatsOptions<StatLevelColors>;
            _statsOptionsSettings.ClearSpeciesCache();
        }

        private void TbOptionsName_Leave(object sender, EventArgs e)
        {
            var newNameBase = _tbOptionsName.Text;
            if (_selectedStatsOptions.Name == newNameBase) return; // nothing to change
            var newName = newNameBase;
            var suffix = 1;
            while (_statsOptionsSettings.StatsOptionsDict.ContainsKey(newName))
                newName = newNameBase + "_" + ++suffix;

            _tbOptionsName.Text = newName;
            _statsOptionsSettings.StatsOptionsDict.Remove(_selectedStatsOptions.Name);
            _selectedStatsOptions.Name = newName;
            _statsOptionsSettings.StatsOptionsDict.Add(newName, _selectedStatsOptions);
            // update text in combobox
            _cbbOptions.Items[_cbbOptions.SelectedIndex] = _selectedStatsOptions;
            _statsOptionsSettings.ClearSpeciesCache();
        }

        private void BtNew_Click(object sender, EventArgs e)
        {
            var newNameBase = _species?.name ?? "new entry";
            var newName = newNameBase;
            var suffix = 1;
            while (_statsOptionsSettings.StatsOptionsDict.ContainsKey(newName))
                newName = newNameBase + "_" + ++suffix;
            var newSettings = _statsOptionsSettings.GetDefaultStatOptions(newName);
            _statsOptionsSettings.StatsOptionsDict.Add(newName, newSettings);
            InitializeOptions();
            _cbbOptions.SelectedItem = newSettings;
            _tbOptionsName.Focus();
            _tbOptionsName.SelectAll();
        }

        private void BtRemove_Click(object sender, EventArgs e)
        {
            if (_selectedStatsOptions == null
                || MessageBox.Show("Delete stat options\n" + _selectedStatsOptions + "\n?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    != DialogResult.Yes) return;

            var index = _cbbOptions.SelectedIndex;
            // set parent of dependant options to parent of this setting
            foreach (var so in _statsOptionsSettings.StatsOptionsDict.Values)
            {
                if (so.ParentOptions == _selectedStatsOptions)
                    so.ParentOptions = _selectedStatsOptions.ParentOptions;
            }

            _statsOptionsSettings.StatsOptionsDict.Remove(_selectedStatsOptions.Name);

            InitializeOptions();
            if (_cbbOptions.Items.Count > 0)
                _cbbOptions.SelectedIndex = Math.Max(0, index - 1); // select item before deleted one
            _statsOptionsSettings.ClearSpeciesCache();
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

            _tbOptionsName.AutoCompleteCustomSource = autoCompleteList;
        }

        private static void InitButtonImages(Button btNew, Button btRemove)
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
            btNew.Image = bmp;

            bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            using (var p = new Pen(Brushes.DarkRed))
            {
                g.DrawRectangle(p, 0, size / 3, size - 1, size / 3 - 1);
                g.FillRectangle(Brushes.LightPink, 1, size / 3 + 1, size - 2, size / 3 - 2);
            }
            btRemove.Image = bmp;
        }
    }
}
