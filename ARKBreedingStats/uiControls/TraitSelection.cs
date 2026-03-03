using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.Traits;
using ARKBreedingStats.utils;
using System.ComponentModel;

namespace ARKBreedingStats.uiControls
{
    public partial class TraitSelection : Form
    {
        private List<CreatureTrait> _assignedTraits;
        private BindingSource _assignedTraitsBindingSource;
        private readonly TraitDefinition[] _availableTraitDefinitions;
        private readonly Debouncer _debouncerFilter = new Debouncer();
        private readonly ToolTip _tt = new ToolTip();

        public TraitSelection()
        {
            InitializeComponent();
            _availableTraitDefinitions = TraitDefinition.GetTraitDefinitions().Where(t => !t.IsBase).OrderBy(t => t.Name).ToArray();
            LbTraitsAvailable.DataSource = _availableTraitDefinitions;
            _tt.SetToolTip(BtAddTrait, "Add trait");
            _tt.SetToolTip(BtRemoveTrait, "Remove trait");
            _tt.SetToolTip(BtRemoveAll, "Remove all traits");

            ResizeEnd += AdjustLabelMaxSize;
            Load += AdjustLabelMaxSize;
            FormClosed += (s, e) => _tt.RemoveAllAndDispose();
        }

        private void AdjustLabelMaxSize(object sender, EventArgs e)
        {
            LbTraitName.MaximumSize = new Size(PnTraitDescription.Width, 0);
            LbTraitDescription.MaximumSize = new Size(PnTraitDescription.Width, 0);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<CreatureTrait> AssignedTraits
        {
            get => _assignedTraits;
            set
            {
                _assignedTraits = value ?? new List<CreatureTrait>();
                _assignedTraitsBindingSource = new BindingSource();
                _assignedTraitsBindingSource.DataSource = _assignedTraits;
                LbTraitsAssigned.DataSource = _assignedTraitsBindingSource;
            }
        }

        public void SetTitle(string title) => Text = title;

        private void BtAddTrait_Click(object sender, EventArgs e) => AddSelectedTrait();

        private void BtRemoveTrait_Click(object sender, EventArgs e) => RemoveSelectedTrait();

        private void LbTraitsAvailable_DoubleClick(object sender, EventArgs e) => AddSelectedTrait();

        private void LbTraitsAssigned_MouseDoubleClick(object sender, MouseEventArgs e) => RemoveSelectedTrait();

        private void AddSelectedTrait()
        {
            if (!(LbTraitsAvailable.SelectedItem is TraitDefinition traitDefinition)) return;
            _assignedTraitsBindingSource.Add(new CreatureTrait(traitDefinition));
            UpdateTierSelection();
        }

        private void RemoveSelectedTrait()
        {
            if (!(LbTraitsAssigned.SelectedItem is CreatureTrait trait)) return;
            _assignedTraitsBindingSource.Remove(trait);
            UpdateTierSelection();
        }

        private void BtRemoveAll_Click(object sender, EventArgs e)
        {
            _assignedTraitsBindingSource.Clear();
            UpdateTierSelection();
        }

        private void BtCancel_Click(object sender, EventArgs e) => Close();

        private void BtOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void RbTier1_Click(object sender, EventArgs e) => SetTier(0);

        private void RbTier2_Click(object sender, EventArgs e) => SetTier(1);

        private void RbTier3_Click(object sender, EventArgs e) => SetTier(2);

        private void SetTier(int tier)
        {
            if (!(LbTraitsAssigned.SelectedItem is CreatureTrait trait)) return;
            trait.Tier = (byte)tier;
            _assignedTraitsBindingSource.ResetCurrentItem();
        }

        private void LbTraitsAssigned_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTierSelection();
            UpdateDescription((LbTraitsAssigned.SelectedItem as CreatureTrait)?.TraitDefinition);
        }

        private void UpdateTierSelection()
        {
            if (!(LbTraitsAssigned.SelectedItem is CreatureTrait trait))
            {
                RbTier1.Checked = false;
                RbTier2.Checked = false;
                RbTier3.Checked = false;
                return;
            }

            switch (trait.Tier)
            {
                case 2:
                    RbTier3.Checked = true;
                    break;
                case 1:
                    RbTier2.Checked = true;
                    break;
                default:
                    RbTier1.Checked = true;
                    break;
            }
        }

        private void LbTraitsAvailable_SelectedIndexChanged(object sender, EventArgs e) => UpdateDescription(LbTraitsAvailable.SelectedItem as TraitDefinition);

        private void UpdateDescription(TraitDefinition traitDefinition)
        {
            LbTraitName.Text = traitDefinition?.Name ?? string.Empty;
            LbTraitDescription.Text = traitDefinition == null ? string.Empty : $"{Environment.NewLine}{traitDefinition.Description}{Environment.NewLine}{Environment.NewLine}{traitDefinition.Effect}";
        }

        private void BtClearFilter_Click(object sender, EventArgs e) => TbTraitFilter.Clear();

        private void TbTraitFilter_TextChanged(object sender, EventArgs e)
        {
            _debouncerFilter.Debounce(string.IsNullOrEmpty(TbTraitFilter.Text) ? 0 : 500, FilterTraits, Dispatcher.CurrentDispatcher);
        }

        private void FilterTraits()
        {
            var filter = TbTraitFilter.Text.ToLowerInvariant();
            if (string.IsNullOrEmpty(filter))
                LbTraitsAvailable.DataSource = _availableTraitDefinitions;
            else
                LbTraitsAvailable.DataSource = _availableTraitDefinitions
                    .Where(t => t.Name.ToLowerInvariant().Contains(filter))
                    .ToArray();
        }

        /// <summary>
        /// Displays the creature trait window with preset traits.
        /// </summary>
        /// <returns>True if user selected Ok</returns>
        public static bool ShowTraitSelectionWindow(List<CreatureTrait> presetCreatureTraits, string windowTitle, out List<CreatureTrait> selectedCreatureTraits)
        {
            selectedCreatureTraits = null;
            using (var traitSelection = new TraitSelection())
            {
                traitSelection.AssignedTraits = presetCreatureTraits;
                traitSelection.SetTitle(windowTitle);
                Utils.SetWindowRectangle(traitSelection, Properties.Settings.Default.WindowPositionTraitSelection);
                traitSelection.ShowDialog();
                (Properties.Settings.Default.WindowPositionTraitSelection, _) = Utils.GetWindowRectangle(traitSelection);
                if (traitSelection.DialogResult != DialogResult.OK) return false;

                if (traitSelection.AssignedTraits.Any())
                    selectedCreatureTraits = traitSelection.AssignedTraits;
            }

            return true;
        }
    }
}
