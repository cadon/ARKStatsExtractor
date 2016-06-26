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
        private bool updateCalculation;

        public TamingControl()
        {
            InitializeComponent();
            updateCalculation = true;
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
                    tf.Clicked += new TamingFoodControl.ClickedEventHandler(onlyOneFood);
                    foodControls.Add(tf);
                    Controls.Add(tf);
                    i++;
                }
                this.ResumeLayout();

                if (foodControls.Count > 0)
                {
                    foodControls[0].amount = Taming.foodAmountNeeded(sI, (int)nudLevel.Value, foodControls[0].foodName, Values.V.species[sI].taming.nonViolent);
                }
            }
        }

        private void nudLevel_ValueChanged(object sender, EventArgs e)
        {
            updateTamingData();
        }

        private void updateTamingData()
        {
            if (updateCalculation)
            {
                int sI = comboBoxSpecies.SelectedIndex;
                TimeSpan duration;
                int narcoBerries, narcotics;
                double te;
                bool enoughFood;
                var usedFood = new List<string>();
                var foodAmount = new List<int>();
                var foodAmountUsed = new List<int>();
                foreach (TamingFoodControl tfc in foodControls)
                {
                    usedFood.Add(tfc.foodName);
                    foodAmount.Add(tfc.amount);
                }
                Taming.tamingTimes(sI, (int)nudLevel.Value, usedFood, foodAmount, out foodAmountUsed, out duration, out narcoBerries, out narcotics, out te, out enoughFood);

                for (int f = 0; f < foodControls.Count; f++)
                {
                    foodControls[f].foodUsed = foodAmountUsed[f];
                }

                if (enoughFood)
                    labelResult.Text = "It takes " + duration.ToString(@"hh\:mm\:ss") + " to tame the creature.\n" + narcoBerries + " Narcoberries or " + narcotics + " Narcotics are needed\nTaming Effectiveness: " + Math.Round(100 * te, 1).ToString() + " %\nBonus-Level: " + Math.Floor((double)nudLevel.Value * te / 2).ToString();
                else
                    labelResult.Text = "Not enough food to tame the creature!";
            }
        }

        private void onlyOneFood(string food)
        {
            int sI = comboBoxSpecies.SelectedIndex;
            if (sI >= 0)
            {
                updateCalculation = false;
                foreach (TamingFoodControl tfc in foodControls)
                {
                    if (tfc.foodName == food)
                        tfc.amount = Taming.foodAmountNeeded(sI, (int)nudLevel.Value, food, Values.V.species[sI].taming.nonViolent);
                    else
                        tfc.amount = 0;
                }
                updateCalculation = true;
                updateTamingData();
            }
        }
    }
}
