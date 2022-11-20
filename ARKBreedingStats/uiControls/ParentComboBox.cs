using ARKBreedingStats.Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    class ParentComboBox : ComboBox
    {
        private List<Creature> parentList;
        public List<int> parentsSimilarity;
        private ToolTip tt;
        public string naLabel;
        private Guid _preselectedCreatureGuid;

        public ParentComboBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            DrawItem += comboBoxParents_DrawItem;
            DropDownClosed += comboBoxParents_DropDownClosed;
            tt = new ToolTip();
            _preselectedCreatureGuid = Guid.Empty;
            naLabel = "n/a";
        }

        public Creature SelectedParent
        {
            get
            {
                if (parentList == null) return null;
                // at index 0 is the n/a
                if (SelectedIndex > 0 && SelectedIndex - 1 < parentList.Count)
                    return parentList[SelectedIndex - 1];
                return null;
            }
        }

        public void Clear()
        {
            _preselectedCreatureGuid = Guid.Empty;
            SelectedIndex = 0;
        }

        public Guid PreselectedCreatureGuid
        {
            get => _preselectedCreatureGuid;
            set
            {
                _preselectedCreatureGuid = value;
                if (_preselectedCreatureGuid == Guid.Empty || parentList == null)
                {
                    if (Items.Count != 0)
                        SelectedIndex = 0;
                    return;
                }

                int selIndex = 0;
                for (int c = 0; c < parentList.Count; c++)
                {
                    if (parentList[c].guid == _preselectedCreatureGuid)
                    {
                        selIndex = c + 1; // index 0 is "none"
                        break;
                    }
                }
                SelectedIndex = selIndex;
            }
        }

        public List<Creature> ParentList
        {
            set
            {
                Items.Clear();
                Items.Add(naLabel);
                int selInd = 0;
                parentList = value;
                if (value == null) return;

                for (int c = 0; c < parentList.Count; c++)
                {
                    string similarities = string.Empty;
                    string status = string.Empty;
                    if (parentsSimilarity != null && parentsSimilarity.Count > c)
                        similarities = " (" + parentsSimilarity[c] + ")";
                    if (parentList[c].Status != CreatureStatus.Available)
                    {
                        status = " (" + Utils.StatusSymbol(parentList[c].Status) + ")";
                    }
                    Items.Add(parentList[c].name + status + similarities);
                    if (parentList[c].guid == _preselectedCreatureGuid)
                        selInd = c + 1;
                }
                SelectedIndex = selInd;
            }
        }

        private void comboBoxParents_DrawItem(object sender, DrawItemEventArgs e)
        {
            // index of item in parentListSimilarity
            int i = e.Index - 1;
            if (i < -1)
            {
                return;
            }

            ComboBox cb = (ComboBox)sender;

            // Draw the background of the ComboBox control for each item.
            e.DrawBackground();
            // Define the default color of the brush as black.
            Brush myBrush = Brushes.Black;

            // colors of similarity, dependant on the equal stats
            Brush[] brushes = { Brushes.Black, Brushes.Black, Brushes.Black, Brushes.Black, Brushes.Black, Brushes.DarkRed, Brushes.DarkOrange, Brushes.Green, Brushes.Green, Brushes.Green, Brushes.Green, Brushes.Green };

            if (i == -1)
            {
                myBrush = Brushes.DarkGray; // no parent selected
            }
            else if (parentsSimilarity != null && parentsSimilarity.Count > i)
            {
                // Determine the color of the brush to draw each item based on the similarity of the wildlevels
                myBrush = brushes[parentsSimilarity[i]];
            }

            string text = cb.Items[e.Index].ToString();
            // Draw the current item text
            e.Graphics.DrawString(text, e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);

            // show tooltip (for too long names)
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected && cb.DroppedDown)
            {
                tt.Show(text, cb, e.Bounds.Right, e.Bounds.Bottom);
            }
        }

        private void comboBoxParents_DropDownClosed(object sender, EventArgs e)
        {
            tt.Hide((ComboBox)sender);
        }
    }
}
