using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class CreatureInfoInput : UserControl
    {
        public delegate void Add2LibraryClickedEventHandler(CreatureInfoInput sender);
        public event Add2LibraryClickedEventHandler Add2Library_Clicked;
        public delegate void RequestParentListEventHandler(CreatureInfoInput sender);
        public event RequestParentListEventHandler ParentListRequested;
        public bool extractor;
        private Gender gender;
        private List<Creature>[] parents; // all creatures that could be parents (i.e. same species, separated by gender)
        public List<int>[] parentsSimilarity; // for all possible parents the number of equal stats (to find the parents easier)
        public bool parentListValid;
        private ToolTip tt = new ToolTip();

        public CreatureInfoInput()
        {
            InitializeComponent();
            comboBoxMother.Items.Add("- Mother n/a");
            comboBoxFather.Items.Add("- Father n/a");
            comboBoxMother.SelectedIndex = 0;
            comboBoxFather.SelectedIndex = 0;
        }

        private void buttonAdd2Library_Click(object sender, EventArgs e)
        {
            Add2Library_Clicked(this);
        }

        public string CreatureName { get { return textBoxName.Text; } }
        public string CreatureOwner { get { return textBoxOwner.Text; } }
        public Gender CreatureGender { get { return gender; } }
        public Creature mother
        {
            get
            {
                if (parents != null && parents[0] != null && comboBoxMother.SelectedIndex > 0 && parents[0].Count > comboBoxMother.SelectedIndex - 1)
                    return parents[0][comboBoxMother.SelectedIndex - 1];
                return null;
            }
        }
        public Creature father
        {
            get
            {
                if (parents != null && parents[1] != null && comboBoxFather.SelectedIndex > 0 && parents[1].Count > comboBoxFather.SelectedIndex - 1)
                    return parents[1][comboBoxFather.SelectedIndex - 1];
                return null;
            }
        }

        private void buttonGender_Click(object sender, EventArgs e)
        {
            switch (gender)
            {
                case Gender.Male:
                    gender = Gender.Unknown;
                    buttonGender.Text = "?";
                    break;
                case Gender.Female:
                    gender = Gender.Male;
                    buttonGender.Text = "♂";
                    break;
                default:
                    gender = Gender.Female;
                    buttonGender.Text = "♀";
                    break;
            }
        }

        public List<Creature>[] Parents
        {
            set
            {
                // save previously selected parents
                Creature ma = null;
                Creature pa = null;
                if (comboBoxMother.SelectedIndex > 0 && parents[0].Count > comboBoxMother.SelectedIndex - 1)
                    ma = parents[0][comboBoxMother.SelectedIndex - 1];
                if (comboBoxFather.SelectedIndex > 0 && parents[1].Count > comboBoxFather.SelectedIndex - 1)
                    pa = parents[1][comboBoxFather.SelectedIndex - 1];

                comboBoxMother.Items.Clear();
                comboBoxFather.Items.Clear();
                comboBoxMother.Items.Add("- Mother n/a");
                comboBoxFather.Items.Add("- Father n/a");
                int selInd = 0;
                parents = value;
                if (parents[0] != null && parents[1] != null)
                {
                    for (int c = 0; c < parents[0].Count; c++)
                    {
                        if (parentsSimilarity[0].Count <= c) parentsSimilarity[0][c] = 0;
                        comboBoxMother.Items.Add(parents[0][c].name + " (" + parentsSimilarity[0][c] + ")");
                        if (parents[0][c] == ma)
                            selInd = c + 1;
                    }
                    comboBoxMother.SelectedIndex = selInd;
                    selInd = 0;
                    for (int c = 0; c < parents[1].Count; c++)
                    {
                        if (parentsSimilarity[1].Count <= c) parentsSimilarity[1][c] = 0;
                        comboBoxFather.Items.Add(parents[1][c].name + " (" + parentsSimilarity[1][c] + ")");
                        if (parents[1][c] == pa)
                            selInd = c + 1;
                    }
                    comboBoxFather.SelectedIndex = selInd;
                }
                else parents = null;
            }
        }

        public bool ButtonEnabled { set { buttonAdd2Library.Enabled = value; } }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            if (!parentListValid)
                ParentListRequested(this);
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

            int p = 1;
            if (cb == comboBoxMother)
                p = 0;

            // Draw the background of the ComboBox control for each item.
            e.DrawBackground();
            // Define the default color of the brush as black.
            Brush myBrush = Brushes.Black;

            // colors of similarity
            Brush[] brushes = new Brush[] { Brushes.Black, Brushes.DarkRed, Brushes.DarkOrange, Brushes.Green, Brushes.Green, Brushes.Green, Brushes.Green, Brushes.Green };

            if (i == -1)
            {
                myBrush = Brushes.DarkGray; // no parent
            }
            else
            {
                // Determine the color of the brush to draw each item based on the similarity of the wildlevels
                myBrush = brushes[parentsSimilarity[p][i]];
            }

            string text = cb.Items[e.Index].ToString();
            // Draw the current item text
            e.Graphics.DrawString(text, e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);

            // show tooltip (for too long names)
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected && cb.DroppedDown)
            { tt.Show(text, cb, e.Bounds.Right, e.Bounds.Bottom); }
        }

        private void comboBoxParents_DropDownClosed(object sender, EventArgs e)
        {
            tt.Hide((ComboBox)sender);
        }
        
    }
}
