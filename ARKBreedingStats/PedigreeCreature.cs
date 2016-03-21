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

    public partial class PedigreeCreature : UserControl
    {
        private Creature creature;
        public event Pedigree.CreatureChangedEventHandler CreatureChanged;
        private List<Label> labels;
        ToolTip tt = new ToolTip();

        public PedigreeCreature()
        {
            InitC();
        }
        private void InitC()
        {
            InitializeComponent();
            tt.InitialDelay = 100;
            tt.SetToolTip(labelHP, "Health");
            tt.SetToolTip(labelSt, "Stamina");
            tt.SetToolTip(labelOx, "Oxygen");
            tt.SetToolTip(labelFo, "Food");
            tt.SetToolTip(labelWe, "Weight");
            tt.SetToolTip(labelDm, "Melee Damage");
            tt.SetToolTip(labelSp, "Speed");
            labels = new List<Label> { labelHP, labelSt, labelOx, labelFo, labelWe, labelDm, labelSp };
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.Cursor = Cursors.Hand;
        }

        public PedigreeCreature(Creature creature)
        {
            InitC();
            setCreature(creature);
        }
        public void setCreature(Creature creature)
        {
            this.creature = creature;
            groupBox1.Text = (creature.status == CreatureStatus.Dead ? "(†) " : "") + creature.name;
            if (creature.status == CreatureStatus.Dead)
            {
                groupBox1.ForeColor = SystemColors.GrayText;
                tt.SetToolTip(groupBox1, "Creature has passed away");
            }
            else
            {
                groupBox1.ForeColor = SystemColors.ControlText;
                tt.Hide(groupBox1);
            }

            for (int s = 0; s < 7; s++)
            {
                if (creature.levelsWild[s] < 0)
                {
                    labels[s].Text = "?";
                    labels[s].BackColor = Color.WhiteSmoke;
                    labels[s].ForeColor = Color.LightGray;
                }
                else
                {
                    labels[s].Text = creature.levelsWild[s].ToString();
                    labels[s].BackColor = Utils.getColorFromPercent((int)(creature.levelsWild[s] * 2.5), (creature.topBreedingStats[s] ? 0.2 : 0.7));
                }
            }
            switch (creature.gender)
            {
                case Gender.Female:
                    labelGender.Text = "♀";
                    labelGender.BackColor = Color.FromArgb(255, 230, 255);
                    break;
                case Gender.Male:
                    labelGender.Text = "♂";
                    labelGender.BackColor = Color.FromArgb(220, 235, 255);
                    break;
                default:
                    labelGender.Text = "?";
                    labelGender.BackColor = SystemColors.Control;
                    break;
            }
        }
        public bool highlight
        {
            set
            {
                panel1.Visible = value;
                if (value)
                    this.Cursor = Cursors.Default;
                else
                    this.Cursor = Cursors.Hand;
            }
        }

        private void PedigreeCreature_Click(object sender, EventArgs e)
        {
            CreatureChanged(this.creature, false);
        }

        private void element_Click(object sender, EventArgs e)
        {
            PedigreeCreature_Click(sender, e);
        }
    }
}
