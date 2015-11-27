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
    public partial class CreatureBox : UserControl
    {
        private string creatureName, species, gender;
        private int level;
        private StatDisplay[] stats;

        public CreatureBox()
        {
            InitializeComponent();
            stats = new StatDisplay[] { statDisplayHP, statDisplaySt, statDisplayOx, statDisplayFo, statDisplayWe, statDisplayDm, statDisplaySp, statDisplayTo };
            stats[0].Title = "HP";
            stats[1].Title = "St";
            stats[2].Title = "Ox";
            stats[3].Title = "Fo";
            stats[4].Title = "We";
            stats[5].Title = "Dm";
            stats[6].Title = "Sp";
            stats[7].Title = "To";
            stats[5].Percent = true;
            stats[6].Percent = true;
        }

        public void setStat(int stat, int level, double statValue)
        {
            if (stat >= 0 && stat < 8)
            {
                stats[stat].Level = level;
                stats[stat].StatValue = statValue;
            }
        }
        public int Level
        {
            set
            {
                level = value;
                updateTitle();
            }
        }
        public string CreatureName
        {
            set
            {
                creatureName = value;
                textBoxName.Text = value;
                updateTitle();
            }
            get { return creatureName; }
        }
        
        private void setName(bool save)
        {
            textBoxName.Visible = false;
            if (save) { CreatureName = textBoxName.Text; }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            textBoxName.SelectAll();
            textBoxName.Visible = true;
            textBoxName.Focus();
        }

        private void textBoxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (false)
            {
                if (e.KeyChar == (char)Keys.Return)
                {
                    setName(true);
                    e.Handled = true;
                }
                else if (e.KeyChar == (char)Keys.Escape)
                {
                    setName(false);
                    e.Handled = true;
                }
            }
        }

        private void textBoxName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                setName(true);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                setName(false);
                e.Handled = true;
            }
        }

        public string Species
        {
            set
            {
                species = value;
                updateTitle();
            }
        }
        private void updateTitle()
        {
            groupBox1.Text = creatureName + " (" + species + ", Lvl " + level + ")";
        }
    }
}
