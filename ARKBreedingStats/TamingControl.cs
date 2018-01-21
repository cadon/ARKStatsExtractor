﻿using System;
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
        private int speciesIndex;
        public event TimerControl.CreateTimerEventHandler CreateTimer;
        private DateTime wakeUpTime, starvingTime;
        private double tamingSpeedMultiplier;
        private double tamingFoodRateMultiplier;
        private string koNumbers;
        private string boneDamageAdjustersImmobilization;
        public string quickTamingInfos;
        private double foodDepletion;
        private string firstFeedingWaiting;


        public TamingControl()
        {
            InitializeComponent();
            updateCalculation = true;
            wakeUpTime = DateTime.Now;
            starvingTime = DateTime.Now;
        }

        public void setLevel(int level, bool updateTamingData = true)
        {
            bool updateKeeper = updateCalculation;
            updateCalculation = updateTamingData;
            if (level >= nudLevel.Minimum && level <= nudLevel.Maximum)
                nudLevel.Value = level;
            updateCalculation = updateKeeper;
        }
        public int level { set { } }

        public void setSpeciesIndex(int speciesIndex)
        {
            if (speciesIndex >= 0 && Values.V.species[speciesIndex].taming != null)
            {
                this.speciesIndex = speciesIndex;

                boneDamageAdjustersImmobilization = Taming.boneDamageAdjustersImmobilization(speciesIndex);

                updateCalculation = false;
                this.SuspendLayout();
                TamingData td = Values.V.species[speciesIndex].taming;


                foodDepletion = td.foodConsumptionBase * td.foodConsumptionMult * tamingFoodRateMultiplier;

                TamingFoodControl tf;
                int i = 0;
                if (td.eats != null)
                {
                    for (i = 0; i < td.eats.Count; i++)
                    {
                        string f = td.eats[i];
                        if (i >= foodControls.Count)
                        {
                            tf = new TamingFoodControl(f);
                            tf.Location = new Point(20, 80 + 45 * i);
                            tf.valueChanged += new TamingFoodControl.ValueChangedEventHandler(updateTamingData);
                            tf.Clicked += new TamingFoodControl.ClickedEventHandler(onlyOneFood);
                            foodControls.Add(tf);
                            Controls.Add(tf);
                        }
                        else
                        {
                            tf = foodControls[i];
                            tf.FoodName = f;
                            tf.Show();
                        }
                        if (f == "Kibble")
                            tf.foodNameDisplay = "Kibble (" + td.favoriteKibble + " Egg)";
                        if (td.specialFoodValues != null && td.specialFoodValues.ContainsKey(f) && td.specialFoodValues[f].quantity > 1)
                            tf.foodNameDisplay = td.specialFoodValues[f].quantity.ToString() + "× " + tf.foodNameDisplay;
                    }
                }

                for (int fci = foodControls.Count - 1; fci >= i; fci--)
                {
                    foodControls[fci].Hide();
                }

                if (i > 0)
                    foodControls[0].amount = Taming.foodAmountNeeded(speciesIndex, (int)nudLevel.Value, tamingSpeedMultiplier, foodControls[0].FoodName, td.nonViolent);

                updateCalculation = true;
                updateFirstFeedingWaiting();
                updateTamingData();

                ResumeLayout();
            }
        }

        private void nudLevel_ValueChanged(object sender, EventArgs e)
        {
            updateFirstFeedingWaiting();
            updateTamingData();
        }

        public void updateTamingData()
        {
            if (updateCalculation && speciesIndex >= 0)
            {
                updateKOCounting();

                TimeSpan duration = new TimeSpan();
                int narcoBerries = 0, narcotics = 0, bioToxines = 0, bonusLevel = 0;
                double te = 0, hunger = 0;
                bool enoughFood = false;
                var usedFood = new List<string>();
                var foodAmount = new List<int>();
                var foodAmountUsed = new List<int>();
                quickTamingInfos = "n/a";
                int level = (int)nudLevel.Value;

                if (Values.V.species[speciesIndex].taming.eats != null)
                {
                    int foodCounter = Values.V.species[speciesIndex].taming.eats.Count;
                    foreach (TamingFoodControl tfc in foodControls)
                    {
                        if (foodCounter == 0)
                            break;
                        foodCounter--;

                        usedFood.Add(tfc.FoodName);
                        foodAmount.Add(tfc.amount);
                        tfc.maxFood = Taming.foodAmountNeeded(speciesIndex, level, tamingSpeedMultiplier, tfc.FoodName, Values.V.species[speciesIndex].taming.nonViolent);
                        tfc.tamingDuration = Taming.tamingDuration(speciesIndex, tfc.maxFood, tfc.FoodName, tamingFoodRateMultiplier, Values.V.species[speciesIndex].taming.nonViolent);
                    }
                    Taming.tamingTimes(speciesIndex, level, tamingSpeedMultiplier, tamingFoodRateMultiplier, usedFood, foodAmount, out foodAmountUsed, out duration, out narcoBerries, out narcotics, out bioToxines, out te, out hunger, out bonusLevel, out enoughFood);

                    for (int f = 0; f < foodAmountUsed.Count; f++)
                    {
                        foodControls[f].foodUsed = foodAmountUsed[f];
                    }
                }

                if (enoughFood)
                {
                    labelResult.Text = "It takes " + Utils.durationUntil(duration)
                    + " to tame the " + Values.V.speciesNames[speciesIndex] + "."
                    + "\n\n"
                    + "Taming Effectiveness: " + Math.Round(100 * te, 1) + " %"
                    + "\nBonus-Level: +" + bonusLevel + " (total level after Taming: " + (nudLevel.Value + bonusLevel) + ")"
                    + "\n\n"
                    + $"Food has to drop by {hunger:F1} units."
                    + "\n\n"
                    + $"{narcoBerries} Narcoberries or\n"
                    + $"{narcotics} Narcotics or\n"
                    + $"{bioToxines} Bio Toxines are needed"
                    + firstFeedingWaiting;

                    if (foodAmountUsed.Count > 0)
                    {
                        quickTamingInfos = Taming.quickInfoOneFood(speciesIndex, level, tamingSpeedMultiplier, tamingFoodRateMultiplier, foodControls[0].FoodName, foodControls[0].maxFood, foodControls[0].foodNameDisplay);
                        // show raw meat or mejoberries as alternative (often used)
                        for (int i = 1; i < usedFood.Count; i++)
                        {
                            if (usedFood[i] == "Raw Meat" || usedFood[i] == "Mejoberry")
                            {
                                quickTamingInfos += "\n\n" + Taming.quickInfoOneFood(speciesIndex, level, tamingSpeedMultiplier, tamingFoodRateMultiplier, foodControls[i].FoodName, foodControls[i].maxFood, foodControls[i].foodNameDisplay);
                                break;
                            }
                        }

                        quickTamingInfos += "\n\n" + koNumbers
                            + "\n\n" + boneDamageAdjustersImmobilization
                            + firstFeedingWaiting;
                    }
                }
                else if (foodAmountUsed.Count == 0)
                    labelResult.Text = "no taming-data available";
                else
                    labelResult.Text = "Not enough food to tame the creature!";

                numericUpDownCurrentTorpor.Value = (decimal)(Values.V.species[speciesIndex].stats[7].BaseValue * (1 + Values.V.species[speciesIndex].stats[7].IncPerWildLevel * (level - 1)));
                nudCurrentFood.Value = (decimal)(Values.V.species[speciesIndex].stats[3].BaseValue * (1 + Values.V.species[speciesIndex].stats[3].IncPerWildLevel * (level / 7)));
            }
        }

        private void onlyOneFood(string food)
        {
            if (speciesIndex >= 0)
            {
                updateCalculation = false;
                foreach (TamingFoodControl tfc in foodControls)
                {
                    if (tfc.FoodName == food)
                        tfc.amount = tfc.maxFood;
                    else
                        tfc.amount = 0;
                }
                updateCalculation = true;
                updateTamingData();
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

        private void numericUpDownCurrentTorpor_ValueChanged(object sender, EventArgs e)
        {
            var duration = new TimeSpan(0, 0, Taming.secondsUntilWakingUp(speciesIndex, (int)nudLevel.Value, (double)numericUpDownCurrentTorpor.Value));
            labelTimeUntilWakingUp.Text = "Time until wake-up: " + Utils.duration(duration);
            if (duration.TotalSeconds < 30) labelTimeUntilWakingUp.ForeColor = Color.DarkRed;
            else if (duration.TotalSeconds < 120) labelTimeUntilWakingUp.ForeColor = Color.DarkGoldenrod;
            else labelTimeUntilWakingUp.ForeColor = Color.Black;
            wakeUpTime = DateTime.Now.Add(duration);
        }

        private void nudCurrentFood_ValueChanged(object sender, EventArgs e)
        {
            var duration = new TimeSpan(0, 0, (int)((double)nudCurrentFood.Value / foodDepletion));
            lblTimeUntilStarving.Text = "Time until starving: " + Utils.duration(duration);
            if (duration.TotalSeconds < 30) lblTimeUntilStarving.ForeColor = Color.DarkRed;
            else if (duration.TotalSeconds < 120) lblTimeUntilStarving.ForeColor = Color.DarkGoldenrod;
            else lblTimeUntilStarving.ForeColor = Color.Black;
            starvingTime = DateTime.Now.Add(duration);
        }

        private void nudWDm_ValueChanged(object sender, EventArgs e)
        {
            updateKOCounting();
        }

        private void chkbDm_CheckedChanged(object sender, EventArgs e)
        {
            updateKOCounting();
        }

        private void updateKOCounting()
        {
            bool knockoutNeeded;
            labelKOCount.Text = Taming.knockoutInfo(speciesIndex, (int)nudLevel.Value,
                chkbDmLongneck.Checked ? (double)nudWDmLongneck.Value / 100 : 0,
                chkbDmCrossbow.Checked ? (double)nudWDmCrossbow.Value / 100 : 0,
                chkbDmBow.Checked ? (double)nudWDmBow.Value / 100 : 0,
                chkbDmSlingshot.Checked ? (double)nudWDmSlingshot.Value / 100 : 0,
                chkbDmClub.Checked ? (double)nudWDmClub.Value / 100 : 0,
                chkbDmProd.Checked ? (double)nudWDmProd.Value / 100 : 0,
                out knockoutNeeded, out koNumbers);
            labelKOCount.ForeColor = knockoutNeeded ? SystemColors.ControlText : SystemColors.GrayText;
            if (!knockoutNeeded)
                koNumbers = "";
        }

        public double[] weaponDamages
        {
            set
            {
                if (value != null)
                {
                    NumericUpDown[] nuds = new NumericUpDown[] { nudWDmLongneck, nudWDmCrossbow, nudWDmBow, nudWDmSlingshot, nudWDmClub, nudWDmProd };
                    for (int i = 0; i < value.Length && i < nuds.Length; i++)
                    {
                        nuds[i].Value = (decimal)value[i];
                    }
                }
            }
            get
            {
                return new double[] { (double)nudWDmLongneck.Value, (double)nudWDmCrossbow.Value, (double)nudWDmBow.Value, (double)nudWDmSlingshot.Value, (double)nudWDmClub.Value, (double)nudWDmProd.Value };
            }
        }

        public int weaponDamagesEnabled
        {
            set
            {
                CheckBox[] ckbs = new CheckBox[] { chkbDmLongneck, chkbDmCrossbow, chkbDmBow, chkbDmSlingshot, chkbDmClub, chkbDmProd };
                for (int i = 0; i < ckbs.Length; i++) { ckbs[i].Checked = (value & (1 << i)) > 0; }
            }
            get
            {
                CheckBox[] ckbs = new CheckBox[] { chkbDmLongneck, chkbDmCrossbow, chkbDmBow, chkbDmSlingshot, chkbDmClub, chkbDmProd };
                int r = 0;
                for (int i = 0; i < ckbs.Length; i++) { r += (ckbs[i].Checked ? (1 << i) : 0); }
                return r;
            }
        }

        private void buttonAddTorporTimer_Click(object sender, EventArgs e)
        {
            if (speciesIndex >= 0)
                CreateTimer("Wakeup of " + Values.V.speciesNames[speciesIndex], wakeUpTime, null, TimerControl.TimerGroups.Wakeup.ToString());
        }

        private void btnAddStarvingTimer_Click(object sender, EventArgs e)
        {
            if (speciesIndex >= 0)
                CreateTimer("Starving of " + Values.V.speciesNames[speciesIndex], starvingTime, null, TimerControl.TimerGroups.Starving.ToString());
        }

        public void setTamingMultipliers(double tamingSpeedMultiplier, double tamingFoodRateMultiplier)
        {
            this.tamingSpeedMultiplier = tamingSpeedMultiplier;
            this.tamingFoodRateMultiplier = tamingFoodRateMultiplier;
            updateTamingData();
        }

        private void updateFirstFeedingWaiting()
        {
            int s = Taming.durationAfterFirstFeeding(speciesIndex, (int)nudLevel.Value, foodDepletion);
            if (s > 0)
                firstFeedingWaiting = "\n\nWaiting time after first feeding: ~" + Utils.duration(s);
            else firstFeedingWaiting = "";
        }
    }
}
