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
        private List<int>[] parentsSimilarity; // for all possible parents the number of equal stats (to find the parents easier)
        public bool parentListValid;
        private ToolTip tt = new ToolTip();

        public CreatureInfoInput()
        {
            InitializeComponent();
            parentComboBoxMother.naLabel = " - Mother n/a";
            parentComboBoxMother.Items.Add("- Mother n/a");
            parentComboBoxFather.naLabel = " - Father n/a";
            parentComboBoxFather.Items.Add("- Father n/a");
            parentComboBoxMother.SelectedIndex = 0;
            parentComboBoxFather.SelectedIndex = 0;
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
                return parentComboBoxMother.SelectedParent;
            }
        }
        public Creature father
        {
            get
            {
                return parentComboBoxFather.SelectedParent;
            }
        }

        private void buttonGender_Click(object sender, EventArgs e)
        {
            gender = Utils.nextGender(gender);
            buttonGender.Text = Utils.genderSymbol(gender);
        }

        public List<Creature>[] Parents
        {
            set
            {
                if (value != null)
                {
                    parentComboBoxMother.ParentList = value[0];
                    parentComboBoxFather.ParentList = value[1];
                }
            }
        }
        public List<int>[] ParentsSimilarities
        {
            set
            {
                if (value != null)
                {
                    parentComboBoxMother.parentsSimilarity = value[0];
                    parentComboBoxFather.parentsSimilarity = value[1];
                }
            }
        }

        public bool ButtonEnabled { set { buttonAdd2Library.Enabled = value; } }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            if (!parentListValid)
                ParentListRequested(this);
        }

    }
}
