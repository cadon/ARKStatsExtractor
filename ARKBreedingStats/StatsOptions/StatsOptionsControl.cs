using ARKBreedingStats.species;
using ARKBreedingStats.utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats.StatsOptions
{
    /// <summary>
    /// Base control for stats options. Displays selector for species settings.
    /// </summary>
    internal class StatsOptionsControl<T> : TableLayoutPanel where T : StatOptionsBase
    {
        protected ComboBox CbbOptions;
        protected ComboBox CbbParent;
        protected Species Species;
        protected Button BtRemove;
        protected TextBox TbOptionsName;
        protected Label LbParent;
        protected Label LbParentParent;
        protected Label LbAffectedSpecies;
        protected StatsOptions<T> SelectedStatsOptions;
        protected StatsOptionsSettings<T> StatsOptionsSettings;
        protected TextBox TbAffectedSpecies;
        protected FlowLayoutPanel StatsContainer;
        protected ToolTip Tt;
        private bool _ignoreIndexChange;

        public StatsOptionsControl() { }

        public StatsOptionsControl(StatsOptionsSettings<T> settings, ToolTip tt)
        {
            InitializeControls(settings, tt);
        }

        protected void InitializeControls(StatsOptionsSettings<T> settings, ToolTip tt)
        {
            if (settings == null) return;
            StatsOptionsSettings = settings;
            Tt = tt;

            AutoScroll = true;
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            RowStyles.Add(new RowStyle(SizeType.AutoSize));
            RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var flpHeaderControls = new FlowLayoutPanel { Dock = DockStyle.Fill };
            Controls.Add(flpHeaderControls, 0, 0);
            StatsContainer = new FlowLayoutPanel { Dock = DockStyle.Fill };
            StatsContainer.AutoScroll = true;
            Controls.Add(StatsContainer, 0, 1);

            var btNew = new Button { Width = 20, Height = 20 };
            BtRemove = new Button { Width = 20, Height = 20 };
            flpHeaderControls.Controls.Add(btNew);
            flpHeaderControls.Controls.Add(BtRemove);
            btNew.Click += BtNew_Click;
            BtRemove.Click += BtRemove_Click;
            tt.SetToolTip(btNew, "Create new setting");
            tt.SetToolTip(BtRemove, "Delete setting");
            InitButtonImages(btNew, BtRemove);

            CbbOptions = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            CbbOptions.SelectedIndexChanged += CbbOptions_SelectedIndexChanged;
            flpHeaderControls.Controls.Add(CbbOptions);

            TbOptionsName = new TextBox();
            flpHeaderControls.Controls.Add(TbOptionsName);
            TbOptionsName.Leave += TbOptionsName_Leave;

            LbParent = new Label { Text = "depends on", Margin = new Padding(5, 7, 5, 0), AutoSize = true };
            tt.SetToolTip(LbParent, "If the current setting has no value for a stat, the parent's values are used.");
            flpHeaderControls.Controls.Add(LbParent);

            CbbParent = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            CbbParent.SelectedIndexChanged += CbbParent_SelectedIndexChanged;
            flpHeaderControls.Controls.Add(CbbParent);

            var marginLabelDefault = new Padding(5, 7, 5, 0);

            LbParentParent = new Label { Margin = marginLabelDefault, AutoSize = true };
            tt.SetToolTip(LbParentParent, "If the parent setting has no value for a stat, the parent's parent's values are used etc.");
            flpHeaderControls.Controls.Add(LbParentParent);
            flpHeaderControls.SetFlowBreak(LbParentParent, true);

            LbAffectedSpecies = new Label { Text = "Affected species: ", Margin = marginLabelDefault, AutoSize = true };
            tt.SetToolTip(LbAffectedSpecies, @"Comma separated list of species affected by this setting.
More specific identifier will be used first. Specificity order is
BlueprintPath > DescriptiveNameAndMod > DescriptiveName > Name");
            flpHeaderControls.Controls.Add(LbAffectedSpecies);
            TbAffectedSpecies = new TextBox { AutoSize = true, MinimumSize = new Size(50, 0) };
            flpHeaderControls.Controls.Add(TbAffectedSpecies);
            TbAffectedSpecies.Leave += TbAffectedSpeciesLeave;

            InitializeStatControls();
            InitializeOptions();
        }

        protected virtual void InitializeStatControls() { }

        protected void InitializeOptions(bool reselectItem = false)
        {
            _ignoreIndexChange = true;
            CbbOptions.Items.Clear();
            CbbParent.Items.Clear();

            var statsOptions = TreeOrder(StatsOptionsSettings.StatsOptionsDict);
            CbbOptions.Items.AddRange(statsOptions);
            CbbParent.Items.AddRange(statsOptions);

            if (reselectItem)
            {
                CbbOptions.SelectedItem = SelectedStatsOptions;
                CbbParent.SelectedItem = SelectedStatsOptions.ParentOptions;
            }
            _ignoreIndexChange = false;
            if (CbbOptions.SelectedItem == null && CbbOptions.Items.Count > 0)
                CbbOptions.SelectedIndex = 0;
        }

        private void BtNew_Click(object sender, EventArgs e)
        {
            var newNameBase = Species?.name ?? "new entry";
            var newName = newNameBase;
            var suffix = 1;
            while (StatsOptionsSettings.StatsOptionsDict.ContainsKey(newName))
                newName = newNameBase + "_" + ++suffix;
            var newSettings = StatsOptionsSettings.GetDefaultStatOptions(newName);
            StatsOptionsSettings.StatsOptionsDict.Add(newName, newSettings);
            InitializeOptions();
            CbbOptions.SelectedItem = newSettings;
            TbOptionsName.Focus();
            TbOptionsName.SelectAll();
        }

        private void BtRemove_Click(object sender, EventArgs e)
        {
            if (SelectedStatsOptions == null
                || MessageBox.Show("Delete stat options\n" + SelectedStatsOptions.Name + "\n?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != DialogResult.Yes) return;

            var index = CbbOptions.SelectedIndex;
            // set parent of dependant options to parent of this setting
            foreach (var so in StatsOptionsSettings.StatsOptionsDict.Values)
            {
                if (so.ParentOptions == SelectedStatsOptions)
                    so.ParentOptions = SelectedStatsOptions.ParentOptions;
            }

            StatsOptionsSettings.StatsOptionsDict.Remove(SelectedStatsOptions.Name);

            InitializeOptions();
            if (CbbOptions.Items.Count > 0)
                CbbOptions.SelectedIndex = Math.Max(0, index - 1); // select item before deleted one
            StatsOptionsSettings.ClearSpeciesCache();
        }

        private void CbbOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ignoreIndexChange) return;
            SelectedStatsOptions = CbbOptions.SelectedItem as StatsOptions<T>;
            if (SelectedStatsOptions == null) return;

            this.SuspendDrawing();
            TbOptionsName.Text = SelectedStatsOptions.Name;
            var isNotRoot = SelectedStatsOptions.Name != string.Empty;
            TbOptionsName.Enabled = isNotRoot;
            LbParent.Visible = isNotRoot;
            CbbParent.Visible = isNotRoot;
            BtRemove.Visible = isNotRoot;
            LbParentParent.Text = ParentsParentText(SelectedStatsOptions.ParentOptions);
            LbAffectedSpecies.Visible = isNotRoot;
            TbAffectedSpecies.Visible = isNotRoot;
            TbAffectedSpecies.Text = SelectedStatsOptions.AffectedSpecies == null ? string.Empty : string.Join(", ", SelectedStatsOptions.AffectedSpecies);

            UpdateStatsControls(isNotRoot);

            CbbParent.SelectedItem = SelectedStatsOptions.ParentOptions;
            this.ResumeDrawing();
        }

        private void TbAffectedSpeciesLeave(object sender, EventArgs e)
        {
            if (SelectedStatsOptions == null) return;
            var sp = TbAffectedSpecies.Text
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .ToArray();
            SelectedStatsOptions.AffectedSpecies = sp.Any() ? sp : null;
        }

        private string ParentsParentText(StatsOptions<T> selectedStatsOptions)
        {
            var maxGenerationsShown = 5;
            var currentParent = selectedStatsOptions?.ParentOptions;
            var parentText = string.Empty;
            while (currentParent != null)
            {
                if (maxGenerationsShown-- <= 0)
                {
                    parentText += " \u2794 …";
                    break;
                }
                parentText += " \u2794 " + (string.IsNullOrEmpty(currentParent.Name) ? currentParent.ToString() : currentParent.Name);
                currentParent = currentParent.ParentOptions;
            }

            return parentText;
        }

        /// <summary>
        /// Override this method to update the UI of the stat controls.
        /// </summary>
        protected virtual void UpdateStatsControls(bool isNotRoot) { }

        private void CbbParent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ignoreIndexChange) return;
            SelectedStatsOptions = CbbOptions.SelectedItem as StatsOptions<T>;
            if (SelectedStatsOptions == null) return;
            var selectedParent = CbbParent.SelectedItem as StatsOptions<T>;
            if (SelectedStatsOptions == selectedParent) return; // ignore if node itself is selected as parent
            SelectedStatsOptions.ParentOptions = selectedParent;
            InitializeOptions(true);
            StatsOptionsSettings.ClearSpeciesCache();
        }

        private void TbOptionsName_Leave(object sender, EventArgs e)
        {
            var newNameBase = TbOptionsName.Text;
            if (SelectedStatsOptions.Name == newNameBase) return; // nothing to change
            var newName = newNameBase;
            var suffix = 1;
            while (StatsOptionsSettings.StatsOptionsDict.ContainsKey(newName))
                newName = newNameBase + "_" + ++suffix;

            TbOptionsName.Text = newName;
            if (SelectedStatsOptions.AffectedSpecies?.Any() != false)
            {
                SelectedStatsOptions.AffectedSpecies = new[] { newNameBase };
                TbAffectedSpecies.Text = newNameBase;
            }
            StatsOptionsSettings.StatsOptionsDict.Remove(SelectedStatsOptions.Name);
            SelectedStatsOptions.Name = newName;
            StatsOptionsSettings.StatsOptionsDict.Add(newName, SelectedStatsOptions);
            // update text in combobox
            CbbOptions.Items[CbbOptions.SelectedIndex] = SelectedStatsOptions;
            var cbbParentIndex = CbbParent.Items.IndexOf(SelectedStatsOptions);
            if (cbbParentIndex >= 0)
                CbbParent.Items[cbbParentIndex] = SelectedStatsOptions;
            StatsOptionsSettings.ClearSpeciesCache();
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

        public void SetSpecies(Species s)
        {
            Species = s;
            if (Species == null) return;
            var autoCompleteList = new AutoCompleteStringCollection();
            autoCompleteList.AddRange(new[]
            {
                Species.name,
                Species.DescriptiveName,
                Species.DescriptiveNameAndMod,
                Species.blueprintPath
            });

            TbOptionsName.AutoCompleteCustomSource = autoCompleteList;
        }

        /// <summary>
        /// Returns array ordered like the tree.
        /// </summary>
        private StatsOptions<T>[] TreeOrder(Dictionary<string, StatsOptions<T>> dict)
        {
            var nodeChildren = dict.ToDictionary(kv => kv.Value, kv => new List<StatsOptions<T>>());
            foreach (var item in dict)
            {
                if (item.Value.ParentOptions != null && nodeChildren.TryGetValue(item.Value.ParentOptions, out var parent))
                    parent.Add(item.Value);
            }

            if (!dict.TryGetValue(string.Empty, out var rootNode))
                return Array.Empty<StatsOptions<T>>();

            var sortedList = new List<StatsOptions<T>> { rootNode };
            var level = 0;
            AddChildren(rootNode);

            void AddChildren(StatsOptions<T> n)
            {
                if (!nodeChildren.TryGetValue(n, out var children)) return;

                level++;
                foreach (var item in children.OrderBy(cn => cn.Name))
                {
                    item.HierarchyLevel = level;
                    sortedList.Add(item);
                    AddChildren(item);
                }
                level--;
            }

            return sortedList.ToArray();
        }
    }
}
