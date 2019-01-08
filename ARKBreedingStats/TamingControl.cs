using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ARKBreedingStats.species;

namespace ARKBreedingStats
{
    public partial class TamingControl : UserControl
    {
        private readonly List<TamingFoodControl> foodControls = new List<TamingFoodControl>();
        private bool updateCalculation;
        private int speciesIndex;
        public event TimerControl.CreateTimerEventHandler CreateTimer;
        private DateTime wakeUpTime, starvingTime;
        private double tamingSpeedMultiplier;
        private double tamingFoodRateMultiplier;
        private string koNumbers;
        private string boneDamageAdjustersImmobilization = "";
        public string quickTamingInfos;
        private double foodDepletion;
        private string firstFeedingWaiting;
        private string kibbleRecipe;
        private readonly List<RadioButton> rbBoneDamageAdjusters;
        private readonly List<double> rbBoneDamageAdjusterValues;
        private double currentBoneDamageAdjuster;

        public TamingControl()
        {
            InitializeComponent();
            updateCalculation = true;
            wakeUpTime = DateTime.Now;
            starvingTime = DateTime.Now;
            rbBoneDamageAdjusters = new List<RadioButton>
            {
                    rbBoneDamageDefault
            };
            rbBoneDamageAdjusterValues = new List<double>
            {
                    1
            };
        }

        public void setLevel(int level, bool updateTamingData = true)
        {
            bool updateKeeper = updateCalculation;
            updateCalculation = updateTamingData;
            if (level >= nudLevel.Minimum && level <= nudLevel.Maximum)
                nudLevel.Value = level;
            updateCalculation = updateKeeper;
        }

        public void setSpeciesIndex(int speciesIndex)
        {
            if (speciesIndex >= 0 && Values.V.species[speciesIndex].taming != null && this.speciesIndex != speciesIndex)
            {
                SuspendLayout();

                this.speciesIndex = speciesIndex;

                // bone damage adjusters
                boneDamageAdjustersImmobilization = Taming.boneDamageAdjustersImmobilization(speciesIndex, 
                        out Dictionary<double, string> boneDamageAdjusters);

                int ib = 0;
                foreach (KeyValuePair<double, string> bd in boneDamageAdjusters)
                {
                    ib++;
                    if (ib >= rbBoneDamageAdjusters.Count)
                    {
                        RadioButton rbBD = new RadioButton();
                        gbWeaponDamage.Controls.Add(rbBD);
                        rbBD.Location = new Point(6, 199 + 19 * ib);
                        rbBD.AutoSize = false;
                        rbBD.Size = new Size(194, 17);

                        rbBoneDamageAdjusters.Add(rbBD);
                        rbBoneDamageAdjusterValues.Add(1);
                        rbBD.CheckedChanged += rbBoneDamage_CheckedChanged;
                    }
                    rbBoneDamageAdjusterValues[ib] = bd.Key;
                    rbBoneDamageAdjusters[ib].Text = $"{Loc.s(bd.Value)} ({bd.Key}×)";
                    rbBoneDamageAdjusters[ib].Visible = true;
                }
                for (int j = ib + 1; j < rbBoneDamageAdjusters.Count; j++)
                    rbBoneDamageAdjusters[j].Visible = false;
                rbBoneDamageAdjusters[0].Checked = true;
                // bone damage adjusters adjusted

                updateCalculation = false;
                TamingData td = Values.V.species[speciesIndex].taming;
                kibbleRecipe = "";

                if (td.favoriteKibble != null && Kibbles.K.kibble.ContainsKey(td.favoriteKibble))
                {
                    kibbleRecipe = "\n\nKibble:" + Kibbles.K.kibble[td.favoriteKibble].RecipeAsText();
                }

                foodDepletion = td.foodConsumptionBase * td.foodConsumptionMult * tamingFoodRateMultiplier;

                int i = 0;
                if (td.eats != null)
                {
                    for (i = 0; i < td.eats.Count; i++)
                    {
                        string f = td.eats[i];
                        TamingFoodControl tf;
                        if (i >= foodControls.Count)
                        {
                            tf = new TamingFoodControl(f);
                            tf.valueChanged += updateTamingData;
                            tf.Clicked += onlyOneFood;
                            foodControls.Add(tf);
                            flpTamingFood.Controls.Add(tf);
                        }
                        else
                        {
                            tf = foodControls[i];
                            tf.FoodName = f;
                            tf.Show();
                        }
                        if (f == "Kibble")
                            tf.foodNameDisplay = $"Kibble ({td.favoriteKibble} {Loc.s("Egg")})";
                        if (td.specialFoodValues != null && td.specialFoodValues.ContainsKey(f) && td.specialFoodValues[f].quantity > 1)
                            tf.foodNameDisplay = td.specialFoodValues[f].quantity + "× " + tf.foodNameDisplay;
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

        private void updateTamingData()
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
                    labelResult.Text = $"It takes {Utils.durationUntil(duration)} to tame the {Values.V.speciesNames[speciesIndex]}.\n\n" +
                            $"Taming Effectiveness: {Math.Round(100 * te, 1)} %\n" +
                            $"Bonus-Level: +{bonusLevel} (total level after Taming: {(nudLevel.Value + bonusLevel)})\n\n" +
                            $"Food has to drop by {hunger:F1} units.\n\n" +
                            $"{narcoBerries} Narcoberries or\n" +
                            $"{narcotics} Narcotics or\n" +
                            $"{bioToxines} Bio Toxines are needed{firstFeedingWaiting}";

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
                                + firstFeedingWaiting
                                + kibbleRecipe;
                    }

                    labelResult.Text += kibbleRecipe;
                }
                else if (foodAmountUsed.Count == 0)
                    labelResult.Text = Loc.s("noTamingData");
                else
                    labelResult.Text = Loc.s("notEnoughFoodToTame");

                numericUpDownCurrentTorpor.ValueSave = (decimal)(Values.V.species[speciesIndex].stats[7].BaseValue * (1 + Values.V.species[speciesIndex].stats[7].IncPerWildLevel * (level - 1)));

                // displays the time until the food has decreased enough to tame the creature in one go.
                var durationStarving = new TimeSpan(0, 0, (int)(hunger / foodDepletion));
                lbTimeUntilStarving.Text = $"{Loc.s("TimeUntilFeedingAllFood")}: {Utils.duration(durationStarving)}";
                if (Values.V.species[speciesIndex].stats[3].BaseValue * (1 + Values.V.species[speciesIndex].stats[3].IncPerWildLevel * (level / 7)) < hunger)
                {
                    lbTimeUntilStarving.Text += $"\n{Loc.s("WarningMoreStarvingThanFood")}";
                    lbTimeUntilStarving.ForeColor = Color.DarkRed;
                }
                else lbTimeUntilStarving.ForeColor = SystemColors.ControlText;

                starvingTime = DateTime.Now.Add(durationStarving);
            }
        }

        private void onlyOneFood(string food)
        {
            if (speciesIndex >= 0)
            {
                updateCalculation = false;
                foreach (TamingFoodControl tfc in foodControls)
                {
                    tfc.amount = tfc.FoodName == food ? tfc.maxFood : 0;
                }
                updateCalculation = true;
                updateTamingData();
            }
        }

        private void numericUpDownCurrentTorpor_ValueChanged(object sender, EventArgs e)
        {
            var duration = new TimeSpan(0, 0, Taming.secondsUntilWakingUp(speciesIndex, (int)nudLevel.Value, (double)numericUpDownCurrentTorpor.Value));
            lbTimeUntilWakingUp.Text = string.Format(Loc.s("lbTimeUntilWakingUp"), Utils.duration(duration));
            if (duration.TotalSeconds < 30) lbTimeUntilWakingUp.ForeColor = Color.DarkRed;
            else if (duration.TotalSeconds < 120) lbTimeUntilWakingUp.ForeColor = Color.DarkGoldenrod;
            else lbTimeUntilWakingUp.ForeColor = Color.Black;
            wakeUpTime = DateTime.Now.Add(duration);
        }

        private void nudWDm_ValueChanged(object sender, EventArgs e)
        {
            updateKOCounting();
        }

        private void chkbDm_CheckedChanged(object sender, EventArgs e)
        {
            updateKOCounting();
        }

        private void updateKOCounting(double boneDamageAdjuster = 0)
        {
            if (boneDamageAdjuster == 0)
                boneDamageAdjuster = currentBoneDamageAdjuster;
            lbKOInfo.Text = Taming.knockoutInfo(speciesIndex, (int)nudLevel.Value,
                    chkbDmLongneck.Checked ? (double)nudWDmLongneck.Value / 100 : 0,
                    chkbDmCrossbow.Checked ? (double)nudWDmCrossbow.Value / 100 : 0,
                    chkbDmBow.Checked ? (double)nudWDmBow.Value / 100 : 0,
                    chkbDmSlingshot.Checked ? (double)nudWDmSlingshot.Value / 100 : 0,
                    chkbDmClub.Checked ? (double)nudWDmClub.Value / 100 : 0,
                    chkbDmProd.Checked ? (double)nudWDmProd.Value / 100 : 0,
                    chkbDmHarpoon.Checked ? (double)nudWDmHarpoon.Value / 100 : 0,
                    boneDamageAdjuster,
                    out bool knockoutNeeded, out koNumbers) + (boneDamageAdjustersImmobilization.Length > 0 ? "\n\n" + boneDamageAdjustersImmobilization : "");
            lbKOInfo.ForeColor = knockoutNeeded ? SystemColors.ControlText : SystemColors.GrayText;
            if (!knockoutNeeded)
                koNumbers = "";
        }

        public double[] weaponDamages
        {
            get => new[] { (double)nudWDmLongneck.Value, (double)nudWDmCrossbow.Value, (double)nudWDmBow.Value, (double)nudWDmSlingshot.Value, (double)nudWDmClub.Value, (double)nudWDmProd.Value, (double)nudWDmHarpoon.Value };
            set {
                if (value != null)
                {
                    NumericUpDown[] nuds = { nudWDmLongneck, nudWDmCrossbow, nudWDmBow, nudWDmSlingshot, nudWDmClub, nudWDmProd, nudWDmHarpoon };
                    for (int i = 0; i < value.Length && i < nuds.Length; i++)
                    {
                        nuds[i].Value = (decimal)value[i];
                    }
                }
            }
        }

        public int weaponDamagesEnabled
        {
            set
            {
                CheckBox[] ckbs = { chkbDmLongneck, chkbDmCrossbow, chkbDmBow, chkbDmSlingshot, chkbDmClub, chkbDmProd, chkbDmHarpoon };
                for (int i = 0; i < ckbs.Length; i++)
                {
                    ckbs[i].Checked = (value & (1 << i)) > 0;
                }
            }
            get
            {
                CheckBox[] ckbs = { chkbDmLongneck, chkbDmCrossbow, chkbDmBow, chkbDmSlingshot, chkbDmClub, chkbDmProd, chkbDmHarpoon };
                int r = 0;
                for (int i = 0; i < ckbs.Length; i++)
                {
                    r += (ckbs[i].Checked ? 1 << i : 0);
                }
                return r;
            }
        }

        private void buttonAddTorporTimer_Click(object sender, EventArgs e)
        {
            if (speciesIndex >= 0)
                CreateTimer(Loc.s("timerWakeupOf") + " " + Values.V.speciesNames[speciesIndex], wakeUpTime, null, TimerControl.TimerGroups.Wakeup.ToString());
        }

        private void btnAddStarvingTimer_Click(object sender, EventArgs e)
        {
            if (speciesIndex >= 0)
                CreateTimer(Loc.s("timerStarvingOf") + " " + Values.V.speciesNames[speciesIndex], starvingTime, null, TimerControl.TimerGroups.Starving.ToString());
        }

        public void setTamingMultipliers(double tamingSpeedMultiplier, double tamingFoodRateMultiplier)
        {
            this.tamingSpeedMultiplier = tamingSpeedMultiplier;
            this.tamingFoodRateMultiplier = tamingFoodRateMultiplier;
            updateTamingData();
        }

        private void rbBoneDamage_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Checked)
            {
                int i = rbBoneDamageAdjusters.IndexOf(rb);
                if (i >= 0)
                    currentBoneDamageAdjuster = rbBoneDamageAdjusterValues[i];
                else
                    currentBoneDamageAdjuster = 1;
                updateKOCounting();
            }
        }

        private void updateFirstFeedingWaiting()
        {
            int s = Taming.durationAfterFirstFeeding(speciesIndex, (int)nudLevel.Value, foodDepletion);
            if (s > 0)
                firstFeedingWaiting = "\n\n" + string.Format(Loc.s("waitingAfterFirstFeeding"), Utils.duration(s));
            else firstFeedingWaiting = "";
        }

        public void SetLocalizations()
        {
            Loc.ControlText(lbMax);
            Loc.ControlText(lbUsed);
            Loc.ControlText(lbTamingTime);
            Loc.ControlText(gpTorporTime);
            Loc.ControlText(lbCurrentTorpor);
            Loc.ControlText(lbTimeUntilWakingUp);
            Loc.ControlText(btAddWakeUpTimer);
            Loc.ControlText(gpStarvingTime);
            Loc.ControlText(btnAddStarvingTimer);
            Loc.ControlText(gbWeaponDamage);
            Loc.ControlText(chkbDmHarpoon, "HarpoonLauncher");
            Loc.ControlText(chkbDmProd, "ElectricProd");
            Loc.ControlText(chkbDmLongneck, "Longneck");
            Loc.ControlText(chkbDmCrossbow, "Crossbow");
            Loc.ControlText(chkbDmBow, "Bow");
            Loc.ControlText(chkbDmSlingshot, "Slingshot");
            Loc.ControlText(chkbDmClub, "Club");
            Loc.ControlText(gbKOInfo);
        }
    }
}
