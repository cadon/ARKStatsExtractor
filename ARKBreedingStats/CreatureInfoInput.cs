using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

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
        private Sex sex;
        private CreatureStatus status;
        public bool parentListValid;
        private int speciesIndex;
        public StatIO weightStat;
        private ToolTip tt = new ToolTip();
        private bool mutationManuallyChanged;
        private bool updateMaturation;

        public CreatureInfoInput()
        {
            InitializeComponent();
            parentComboBoxMother.naLabel = " - Mother n/a";
            parentComboBoxMother.Items.Add(" - Mother n/a");
            parentComboBoxFather.naLabel = " - Father n/a";
            parentComboBoxFather.Items.Add(" - Father n/a");
            parentComboBoxMother.SelectedIndex = 0;
            parentComboBoxFather.SelectedIndex = 0;
            tt.SetToolTip(buttonSex, "Sex");
            tt.SetToolTip(buttonStatus, "Status");
            tt.SetToolTip(dateTimePickerAdded, "Domesticated at");
            tt.SetToolTip(numericUpDownMutations, "Mutation-Counter");
            updateMaturation = true;
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
        public Sex CreatureSex
        {
            get { return sex; }
            set
            {
                sex = value;
                buttonSex.Text = Utils.sexSymbol(sex);
                buttonSex.BackColor = Utils.sexColor(sex);
                tt.SetToolTip(buttonSex, "Sex: " + sex.ToString());
                if (sex == Sex.Female)
                    checkBoxNeutered.Text = "Spayed";
                else
                    checkBoxNeutered.Text = "Neutered";
            }
        }
        public CreatureStatus CreatureStatus
        {
            get { return status; }
            set
            {
                status = value;
                buttonStatus.Text = Utils.statusSymbol(status);
                tt.SetToolTip(buttonStatus, "Status: " + status.ToString());
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
            CreatureSex = Utils.nextSex(sex);
        }

        private void buttonStatus_Click(object sender, EventArgs e)
        {
            CreatureStatus = Utils.nextStatus(status);
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
                buttonAdd2Library.Location = new Point((value ? 154 : 88), 258);
                buttonAdd2Library.Size = new Size((value ? 68 : 134), 37);
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            if (!parentListValid)
                ParentListRequested(this);
        }

        private void dhmInputGrown_TextChanged(object sender, EventArgs e)
        {
            if (updateMaturation)
            {
                updateMaturation = false;
                numericUpDownWeight.Value = Values.V.species[speciesIndex].breeding != null && Values.V.species[speciesIndex].breeding.maturationTimeAdjusted > 0 ?
                    (decimal)(weightStat.Input * dhmInputGrown.Timespan.TotalSeconds / Values.V.species[speciesIndex].breeding.maturationTimeAdjusted) : 0;
                updateMaturationPercentage();
                updateMaturation = true;
            }
        }

        private void numericUpDownWeight_ValueChanged(object sender, EventArgs e)
        {
            if (updateMaturation)
            {
                updateMaturation = false;
                if (Values.V.species[speciesIndex].breeding != null && weightStat.Input > 0)
                    dhmInputGrown.Timespan = new TimeSpan(0, 0, (int)(Values.V.species[speciesIndex].breeding.maturationTimeAdjusted * (1 - (double)numericUpDownWeight.Value / weightStat.Input)));
                else dhmInputGrown.Timespan = new TimeSpan(0);
                updateMaturationPercentage();
                updateMaturation = true;
            }
        }

        private void updateMaturationPercentage()
        {
            labelGrownPercent.Text = dhmInputGrown.Timespan.TotalMinutes > 0 && weightStat.Input > 0 ?
                Math.Round(100 * (double)numericUpDownWeight.Value / weightStat.Input, 1) + " %" : "";
        }

        public DateTime Cooldown
        {
            set
            {
                dhmInputCooldown.Timespan = value - DateTime.Now;
            }
            get { return dhmInputCooldown.changed ? DateTime.Now.Add(dhmInputCooldown.Timespan) : DateTime.Now; }
        }

        public DateTime Grown
        {
            set
            {
                dhmInputGrown.Timespan = value - DateTime.Now;
            }
            get { return dhmInputGrown.changed ? DateTime.Now.Add(dhmInputGrown.Timespan) : DateTime.Now; }
        }

        public string[] AutocompleteOwnerList
        {
            set
            {
                var l = new AutoCompleteStringCollection();
                l.AddRange(value);
                textBoxOwner.AutoCompleteCustomSource = l;
            }
        }

        public DateTime domesticatedAt
        {
            set
            {
                if (value < dateTimePickerAdded.MinDate)
                    dateTimePickerAdded.Value = dateTimePickerAdded.MinDate;
                else
                    dateTimePickerAdded.Value = value;
            }
            get { return dateTimePickerAdded.Value; }
        }

        public bool Neutered
        {
            set { checkBoxNeutered.Checked = value; }
            get { return checkBoxNeutered.Checked; }
        }

        public int MutationCounter
        {
            set
            {
                numericUpDownMutations.Value = value;
                mutationManuallyChanged = false;
            }
            get { return (int)numericUpDownMutations.Value; }
        }

        public double babyWeight { set { numericUpDownWeight.Value = (decimal)value; } }

        public int SpeciesIndex
        {
            set
            {
                speciesIndex = value;
                bool breedingPossible = Values.V.species[speciesIndex].breeding != null;

                dhmInputCooldown.Visible = breedingPossible;
                dhmInputGrown.Visible = breedingPossible;
                numericUpDownWeight.Visible = breedingPossible;
                label4.Visible = breedingPossible;
                label5.Visible = breedingPossible;
                label6.Visible = breedingPossible;
                labelGrownPercent.Visible = breedingPossible;
                numericUpDownMutations.Visible = breedingPossible;
                labelMutations.Visible = breedingPossible;
                if (!breedingPossible)
                {
                    numericUpDownWeight.Value = 0;
                    dhmInputGrown.Timespan = new TimeSpan(0);
                    dhmInputCooldown.Timespan = new TimeSpan(0);
                }
            }
        }

        private void parentComboBoxMother_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateMutations();
        }

        private void parentComboBoxFather_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateMutations();
        }

        private void updateMutations()
        {
            if (!mutationManuallyChanged)
            {
                numericUpDownMutations.Value = (parentComboBoxMother.SelectedParent != null ? parentComboBoxMother.SelectedParent.mutationCounter : 0) +
                    (parentComboBoxFather.SelectedParent != null ? parentComboBoxFather.SelectedParent.mutationCounter : 0);
                mutationManuallyChanged = false;
            }
        }

        private void numericUpDownMutations_ValueChanged(object sender, EventArgs e)
        {
            mutationManuallyChanged = true;
        }
    }
}
