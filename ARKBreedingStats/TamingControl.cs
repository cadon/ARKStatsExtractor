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
        private List<TamingFoodControl> foodControls = new List<TamingFoodControl>();
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
            int sI = comboBoxSpecies.SelectedIndex;
            if (sI >= 0 && Values.V.species[sI].taming != null)
            {
                this.SuspendLayout();
                foreach (TamingFoodControl f in foodControls)
                    Controls.Remove(f);
                foodControls.Clear();
                TamingFoodControl tf;
                int i = 0;
                foreach (string f in Values.V.species[sI].taming.eats)
                {
                    tf = new TamingFoodControl(f);
                    tf.Location = new Point(20, 80 + 45 * i);
                    tf.valueChanged += new TamingFoodControl.ValueChangedEventHandler(updateTamingData);
                    foodControls.Add(tf);
                    Controls.Add(tf);
                    i++;
                }
                this.ResumeLayout();
                updateTamingData();
            }
        }

        private void nudLevel_ValueChanged(object sender, EventArgs e)
        {
            updateTamingData();
        }

        private void updateTamingData()
        {
            int sI = comboBoxSpecies.SelectedIndex;
            TimeSpan duration;
            int narcoBerries, narcotics;
            bool enoughFood;
            var usedFood = new List<string>();
            var foodAmount = new List<int>();
            foreach (TamingFoodControl tfc in foodControls)
            {
                usedFood.Add(tfc.foodName);
                foodAmount.Add(tfc.amount);
            }
            Taming.tamingTimes(sI, (int)nudLevel.Value, usedFood, foodAmount, out duration, out narcoBerries, out narcotics, out enoughFood);
            if (enoughFood)
                labelResult.Text = "It takes " + duration.ToString(@"hh\:mm\:ss") + " to tame the creature. " + narcoBerries + " Narcoberries or " + narcotics + " Narcotics are needed";
            else
                labelResult.Text = "Not enough food to tame the creature!";
        }
    }
}
