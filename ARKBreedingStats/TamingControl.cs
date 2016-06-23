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
    public partial class TamingControl : UserControl
    {
        public TamingControl()
        {
            InitializeComponent();
        }

        public List<string> Species
        {
            set
            {
                comboBoxSpecies.Items.Clear();
                foreach (string s in value)
                {
                    comboBoxSpecies.Items.Add(s);
                }
                comboBoxSpecies.SelectedIndex = 0;
            }
        }

        private void comboBoxSpecies_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayTamingData();
        }

        private void nudLevel_ValueChanged(object sender, EventArgs e)
        {
            displayTamingData();
        }

        private void displayTamingData()
        {
            int sI = comboBoxSpecies.SelectedIndex;
            TimeSpan duration;
            int narcoBerries, narcotics;
            Taming.tamingTimes(sI, (int)nudLevel.Value, new List<string>() { "Raw Meat" }, new List<int>() { 1 }, out duration, out narcoBerries, out narcotics);
            label1.Text = "It takes " + duration.ToString(@"hh\:mm\:ss") + " to tame the creature";
        }
    }
}
