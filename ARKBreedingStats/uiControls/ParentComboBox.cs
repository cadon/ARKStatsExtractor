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
        public Guid preselectedCreatureGuid;

        public ParentComboBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            DrawItem += comboBoxParents_DrawItem;
            DropDownClosed += comboBoxParents_DropDownClosed;
            tt = new ToolTip();
            preselectedCreatureGuid = Guid.Empty;
            naLabel = "n/a";
        }

        public Creature SelectedParent
        {
            get
            {
                // at index 0 is the n/a
                if (parentList != null && SelectedIndex > 0 && SelectedIndex - 1 < parentList.Count)
                    return parentList[SelectedIndex - 1];
                return null;
            }
        }

        public void Clear()
        {
            preselectedCreatureGuid = Guid.Empty;
            SelectedIndex = 0;
        }

        public List<Creature> ParentList
        {
            set
            {
                Items.Clear();
                Items.Add(naLabel);
                int selInd = 0;
                parentList = value;
                if (parentList != null)
                {
                    for (int c = 0; c < parentList.Count; c++)
                    {
                        string similarities = "";
                        string status = "";
                        if (parentsSimilarity != null && parentsSimilarity.Count > c)
                            similarities = " (" + parentsSimilarity[c] + ")";
                        if (parentList[c].status != CreatureStatus.Available)
                        {
                            status = " (" + Utils.statusSymbol(parentList[c].status) + ")";
                        }
                        Items.Add(parentList[c].name + status + similarities);
                        if (parentList[c].guid == preselectedCreatureGuid)
                            selInd = c + 1;
                    }
                    SelectedIndex = selInd;
                }
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
            else if (i >= 0 && parentsSimilarity != null && parentsSimilarity.Count > i)
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
