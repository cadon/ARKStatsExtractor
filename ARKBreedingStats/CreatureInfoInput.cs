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
        public delegate void Save2LibraryClickedEventHandler(CreatureInfoInput sender);
        public event Save2LibraryClickedEventHandler Save2Library_Clicked;
        public delegate void RequestParentListEventHandler(CreatureInfoInput sender);
        public event RequestParentListEventHandler ParentListRequested;
        public bool extractor;
        private Gender gender;
        private CreatureStatus status;
        private List<Creature>[] parents; // all creatures that could be parents (i.e. same species, separated by gender)
        private List<int>[] parentsSimilarity; // for all possible parents the number of equal stats (to find the parents easier)
        public bool parentListValid;
        private ToolTip tt = new ToolTip();
        private DateTime cooldown, grown;
        private bool cooldownChanged, grownChanged;

        public CreatureInfoInput()
        {
            InitializeComponent();
            parentComboBoxMother.naLabel = " - Mother n/a";
            parentComboBoxMother.Items.Add(" - Mother n/a");
            parentComboBoxFather.naLabel = " - Father n/a";
            parentComboBoxFather.Items.Add(" - Father n/a");
            parentComboBoxMother.SelectedIndex = 0;
            parentComboBoxFather.SelectedIndex = 0;
        }

        private void buttonAdd2Library_Click(object sender, EventArgs e)
        {
            Add2Library_Clicked(this);
        }

        private void buttonSaveChanges_Click(object sender, EventArgs e)
        {
            Save2Library_Clicked(this);
        }

        public string CreatureName
        {
            get { return textBoxName.Text; }
            set { textBoxName.Text = value; }
        }
        public string CreatureOwner
        {
            get { return textBoxOwner.Text; }
            set { textBoxOwner.Text = value; }
        }
        public Gender CreatureGender
        {
            get { return gender; }
            set
            {
                gender = value;
                buttonGender.Text = Utils.genderSymbol(gender);
            }
        }
        public CreatureStatus CreatureStatus
        {
            get { return status; }
            set
            {
                status = value;
                buttonStatus.Text = Utils.statusSymbol(status);
            }
        }
        public Creature mother
        {
            get
            {
                return parentComboBoxMother.SelectedParent;
            }
            set
            {
                parentComboBoxMother.preselectedCreatureGuid = (value == null ? Guid.Empty : value.guid);
            }
        }
        public Creature father
        {
            get
            {
                return parentComboBoxFather.SelectedParent;
            }
            set
            {
                parentComboBoxFather.preselectedCreatureGuid = (value == null ? Guid.Empty : value.guid);
            }
        }
        public string CreatureNote
        {
            get { return textBoxNote.Text; }
            set { textBoxNote.Text = value; }
        }

        private void buttonGender_Click(object sender, EventArgs e)
        {
            gender = Utils.nextGender(gender);
            buttonGender.Text = Utils.genderSymbol(gender);
        }

        private void buttonStatus_Click(object sender, EventArgs e)
        {
            status = Utils.nextStatus(status);
            buttonStatus.Text = Utils.statusSymbol(status);
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

        public bool ShowSaveButton
        {
            set
            {
                buttonSaveChanges.Visible = value;
                buttonAdd2Library.Location = new Point((value ? 154 : 88), 175);
                buttonAdd2Library.Size = new Size((value ? 68 : 134), 37);
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            if (!parentListValid)
                ParentListRequested(this);
        }

        private void numericUpDownHoursCooldown_ValueChanged(object sender, EventArgs e)
        {
            cooldownChanged = true;
        }

        private void numericUpDownHoursGrowing_ValueChanged(object sender, EventArgs e)
        {
            grownChanged = true;
        }

        public DateTime Cooldown
        {
            set
            {
                cooldown = value;
                cooldownChanged = false;
                numericUpDownHoursCooldown.Value = (DateTime.Now > value ? 0 : (int)Math.Round((value - DateTime.Now).TotalHours));
            }
            get { return (cooldownChanged ? (DateTime.Now.AddHours((double)numericUpDownHoursCooldown.Value)) : cooldown); }
        }

        public DateTime Grown
        {
            set
            {
                grown = value;
                grownChanged = false;
                numericUpDownHoursGrowing.Value = (DateTime.Now > value ? 0 : (int)Math.Round((value - DateTime.Now).TotalHours));
            }
            get { return (grownChanged ? (DateTime.Now.AddHours((double)numericUpDownHoursGrowing.Value)) : grown); }
        }
    }
}
