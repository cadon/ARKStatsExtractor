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

        public int selectedSpeciesIndex
        {
            set
            {
                if (value >= 0 && value < comboBoxSpecies.Items.Count) comboBoxSpecies.SelectedIndex = value;
                else comboBoxSpecies.SelectedIndex = -1;
            }
        }

        public int level
        {
            set
            {
                if (value >= nudLevel.Minimum && value <= nudLevel.Maximum)
                    nudLevel.Value = value;
            }
        }

        private void comboBoxSpecies_SelectedIndexChanged(object sender, EventArgs e)
        {
            int sI = comboBoxSpecies.SelectedIndex;
            if (sI >= 0 && Values.V.species[sI].taming != null)
            {
                TamingData td = Values.V.species[sI].taming;
                this.SuspendLayout();
                foreach (TamingFoodControl f in foodControls)
                    Controls.Remove(f);
                foodControls.Clear();
                TamingFoodControl tf;
                int i = 0;
                foreach (string f in td.eats)
                {
                    tf = new TamingFoodControl(f);
                    if (f == "Kibble")
                        tf.foodNameDisplay = "Kibble (" + td.favoriteKibble + " Egg)";
                    if (td.specialFoodValues != null && td.specialFoodValues.ContainsKey(f) && td.specialFoodValues[f].quantity > 1)
                        tf.foodNameDisplay = td.specialFoodValues[f].quantity.ToString() + "× " + tf.foodNameDisplay;
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
                    foodControls[0].amount = Taming.foodAmountNeeded(sI, (int)nudLevel.Value, foodControls[0].foodName, td.nonViolent);
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
                int narcoBerries, narcotics, bioToxines;
                double te;
                bool enoughFood;
                var usedFood = new List<string>();
                var foodAmount = new List<int>();
                var foodAmountUsed = new List<int>();
                foreach (TamingFoodControl tfc in foodControls)
                {
                    usedFood.Add(tfc.foodName);
                    foodAmount.Add(tfc.amount);
                    tfc.maxFood = Taming.foodAmountNeeded(sI, (int)nudLevel.Value, tfc.foodName, Values.V.species[sI].taming.nonViolent);
                }
                Taming.tamingTimes(sI, (int)nudLevel.Value, usedFood, foodAmount, out foodAmountUsed, out duration, out narcoBerries, out narcotics, out bioToxines, out te, out enoughFood);

                for (int f = 0; f < foodControls.Count; f++)
                {
                    foodControls[f].foodUsed = foodAmountUsed[f];
                }

                if (enoughFood)
                {
                    int bonusLevel = (int)Math.Floor((double)nudLevel.Value * te / 2);
                    labelResult.Text = "It takes " + duration.ToString(@"hh\:mm\:ss") + " (until " + (DateTime.Now + duration).ToShortTimeString() + ") to tame the " + comboBoxSpecies.SelectedItem.ToString() + "."
                                       + "\n\nTaming Effectiveness: " + Math.Round(100 * te, 1).ToString() + " %\nBonus-Level: " + bonusLevel + " (total level after Taming: " + (nudLevel.Value + bonusLevel).ToString() + ")"
                                       + "\n\n" + narcoBerries + " Narcoberries or\n" + narcotics + " Narcotics or\n" + bioToxines + " Bio Toxines are needed";
                }
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
                        tfc.amount = tfc.maxFood;
                    else
                        tfc.amount = 0;
                }
                updateCalculation = true;
                updateTamingData();
            }
        }

        public string tamingInfo
        {
            get
            {
                return "With " + foodControls[0].amount + " × " + foodControls[0].foodNameDisplay + "\n" + labelResult.Text;
            }
        }

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            NumericUpDown n = (NumericUpDown)sender;
            if (n != null)
            {
                n.Select(0, n.Text.Length);
            }
        }
    }
}
