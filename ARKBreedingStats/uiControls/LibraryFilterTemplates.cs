using System;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class LibraryFilterTemplates : Form
    {
        public event Action<string> StringSelected;

        public LibraryFilterTemplates()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.Manual;

            BtRemove.Visible = false;
            BtMoveUp.Visible = false;
            BtMoveDown.Visible = false;
        }

        public string[] Presets
        {
            get
            {
                var presets = LbStrings.Items.Cast<string>().ToArray();
                return presets.Any() ? presets : null;
            }
            set
            {
                LbStrings.Items.Clear();
                if (value?.Any() ?? false)
                    LbStrings.Items.AddRange(value);
            }
        }

        public bool ContainsPreset(string preset) => LbStrings.Items.Contains(preset);

        private void LbStrings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!CbEdit.Checked && LbStrings.SelectedItem != null)
                StringSelected?.Invoke(LbStrings.SelectedItem.ToString());
        }

        public void AddPreset(string preset)
        {
            LbStrings.Items.Add(preset);
        }

        private void CbEdit_CheckedChanged(object sender, EventArgs e)
        {
            var editMode = CbEdit.Checked;
            BtRemove.Visible = editMode;
            BtMoveUp.Visible = editMode;
            BtMoveDown.Visible = editMode;
        }

        private void BtRemove_Click(object sender, EventArgs e)
        {
            var i = LbStrings.SelectedIndex;
            if (i == -1) return;
            LbStrings.Items.RemoveAt(i);
            var count = LbStrings.Items.Count;
            if (count == 0) return;
            if (count <= i) i = count - 1;
            LbStrings.SelectedIndex = i;
        }

        private void BtMoveUp_Click(object sender, EventArgs e) => MoveItem(LbStrings.SelectedIndex, -1);

        private void BtMoveDown_Click(object sender, EventArgs e) => MoveItem(LbStrings.SelectedIndex, 1);

        private void MoveItem(int index, int move)
        {
            if (index == -1) return;

            var newIndex = index + move;
            if (newIndex < 0) newIndex = 0;
            if (newIndex >= LbStrings.Items.Count) newIndex = LbStrings.Items.Count - 1;
            if (newIndex == index) return;

            var item = LbStrings.Items[index];
            LbStrings.Items.RemoveAt(index);
            LbStrings.Items.Insert(newIndex, item);
            LbStrings.SelectedIndex = newIndex;
        }

        private void BtCloseClick(object sender, EventArgs e) => ControlVisibility = false;

        /// <summary>
        /// Hides the control and unsets the edit mode
        /// </summary>
        public bool ControlVisibility
        {
            get => Visible;
            set
            {
                Visible = value;
                if (!value)
                    CbEdit.Checked = false;
            }
        }
    }
}
