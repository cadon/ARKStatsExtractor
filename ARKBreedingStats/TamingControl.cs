using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class TamingControl : UserControl
    {
        private readonly List<TamingFoodControl> foodControls = new List<TamingFoodControl>();
        private bool updateCalculation;
        private Species selectedSpecies;
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
        private double neededHunger;

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
            flcBodyDamageMultipliers.SetFlowBreak(rbBoneDamageDefault, true);
            rbBoneDamageAdjusterValues = new List<double>
            {
                    1
            };
        }

        public void SetLevel(int level, bool updateTamingData = true)
        {
            if (nudLevel.Value == level) return;

            bool updateKeeper = updateCalculation;
            updateCalculation = updateTamingData;
            nudLevel.ValueSave = level;
            updateCalculation = updateKeeper;
        }

        public void SetSpecies(Species species, bool forceRefresh = false)
        {
            if (species == null || (selectedSpecies == species && !forceRefresh))
                return;

            selectedSpecies = species;

            if (species.taming == null)
            {
                NoTamingData();
                return;
            }

            SuspendLayout();

            string speciesName = species.name;
            linkLabelWikiPage.Text = "Wiki: " + speciesName;
            linkLabelWikiPage.Tag = speciesName;

            // bone damage adjusters
            boneDamageAdjustersImmobilization = Taming.BoneDamageAdjustersImmobilization(selectedSpecies,
                out Dictionary<string, double> boneDamageAdjusters);

            int ib = 0;
            foreach (KeyValuePair<string, double> bd in boneDamageAdjusters)
            {
                ib++;
                if (ib >= rbBoneDamageAdjusters.Count)
                {
                    RadioButton rbBD = new RadioButton();
                    flcBodyDamageMultipliers.Controls.Add(rbBD);
                    flcBodyDamageMultipliers.SetFlowBreak(rbBD, true);
                    rbBD.AutoSize = true;

                    rbBoneDamageAdjusters.Add(rbBD);
                    rbBoneDamageAdjusterValues.Add(1);
                    rbBD.CheckedChanged += rbBoneDamage_CheckedChanged;
                }
                rbBoneDamageAdjusterValues[ib] = bd.Value;
                rbBoneDamageAdjusters[ib].Text = $"{Loc.S(bd.Key)} (× {bd.Value})";
                rbBoneDamageAdjusters[ib].Visible = true;
            }
            for (int j = ib + 1; j < rbBoneDamageAdjusters.Count; j++)
                rbBoneDamageAdjusters[j].Visible = false;
            rbBoneDamageAdjusters[0].Checked = true;
            // bone damage adjusters adjusted

            updateCalculation = false;
            TamingData td = species.taming;
            kibbleRecipe = "";


            // TODO replace with new kibble recipes
            //if (td.favoriteKibble != null && Kibbles.K.kibble.ContainsKey(td.favoriteKibble))
            //{
            //    kibbleRecipe = "\n\nKibble:" + Kibbles.K.kibble[td.favoriteKibble].RecipeAsText();
            //}

            foodDepletion = td.foodConsumptionBase * td.foodConsumptionMult * tamingFoodRateMultiplier;

            int i = 0;
            if (td.eats != null)
            {
                for (i = 0; i < td.eats.Length; i++)
                {
                    string f = td.eats[i];
                    TamingFoodControl tf;

                    // if Augmented are not wanted, and food control already exist, update it and hide it.
                    if (!checkBoxAugmented.Checked && f.Contains("Augmented"))
                    {
                        if (i < foodControls.Count)
                        {
                            tf = foodControls[i];
                            tf.FoodName = f;
                            tf.Hide();
                        }
                        continue;
                    }

                    if (i >= foodControls.Count)
                    {
                        tf = new TamingFoodControl(f);
                        tf.valueChanged += UpdateTamingData;
                        tf.Clicked += OnlyOneFood;
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
                        tf.foodNameDisplay = $"Kibble ({td.favoriteKibble} {Loc.S("Egg")})";
                    if (td.specialFoodValues != null && td.specialFoodValues.ContainsKey(f) && td.specialFoodValues[f].quantity > 1)
                        tf.foodNameDisplay = td.specialFoodValues[f].quantity + "× " + tf.foodNameDisplay;
                }
            }

            for (int fci = foodControls.Count - 1; fci >= i; fci--)
            {
                foodControls[fci].Hide();
            }

            if (i > 0)
                foodControls[0].amount = Taming.FoodAmountNeeded(species, (int)nudLevel.Value, tamingSpeedMultiplier, foodControls[0].FoodName, td.nonViolent);

            updateCalculation = true;
            UpdateFirstFeedingWaiting();
            UpdateTamingData();

            ResumeLayout();
        }

        private void nudLevel_ValueChanged(object sender, EventArgs e)
        {
            UpdateFirstFeedingWaiting();
            UpdateTamingData();
        }

        private void UpdateTamingData()
        {
            if (!updateCalculation || selectedSpecies == null)
            {
                return;
            }
            if (selectedSpecies.taming == null)
            {
                NoTamingData();
                return;
            }

            this.Enabled = true;
            UpdateKOCounting();

            TimeSpan duration = new TimeSpan();
            int narcoBerries = 0, ascerbicMushrooms = 0, narcotics = 0, bioToxines = 0, bonusLevel = 0;
            double te = 0;
            neededHunger = 0;
            bool enoughFood = false;
            var usedFood = new List<string>();
            var foodAmount = new List<int>();
            var foodAmountUsed = new List<int>();
            quickTamingInfos = "n/a";
            int level = (int)nudLevel.Value;
            bool tameable = selectedSpecies.taming.nonViolent || selectedSpecies.taming.violent;

            if (tameable && selectedSpecies.taming.eats != null)
            {
                int foodCounter = selectedSpecies.taming.eats.Length;
                foreach (TamingFoodControl tfc in foodControls)
                {
                    if (foodCounter == 0)
                        break;
                    foodCounter--;

                    usedFood.Add(tfc.FoodName);
                    foodAmount.Add(tfc.amount);
                    tfc.maxFood = Taming.FoodAmountNeeded(selectedSpecies, level, tamingSpeedMultiplier, tfc.FoodName, selectedSpecies.taming.nonViolent);
                    tfc.tamingDuration = Taming.TamingDuration(selectedSpecies, tfc.maxFood, tfc.FoodName, tamingFoodRateMultiplier, selectedSpecies.taming.nonViolent);
                }
                Taming.TamingTimes(selectedSpecies, level, tamingSpeedMultiplier, tamingFoodRateMultiplier, usedFood, foodAmount, out foodAmountUsed, out duration, out narcoBerries, out ascerbicMushrooms, out narcotics, out bioToxines, out te, out neededHunger, out bonusLevel, out enoughFood);

                for (int f = 0; f < foodAmountUsed.Count; f++)
                {
                    foodControls[f].foodUsed = foodAmountUsed[f];
                }
            }

            labelResult.ForeColor = SystemColors.ControlText;
            if (!tameable)
            {
                labelResult.Text = Loc.S("speciesNotTameable");
                labelResult.ForeColor = Color.Red;
            }
            else if (enoughFood)
            {
                labelResult.Text = $"It takes {Utils.DurationUntil(duration)} to tame the {selectedSpecies.name}.\n\n" +
                        $"Taming Effectiveness: {Math.Round(100 * te, 1)} %\n" +
                        $"Bonus-Level: +{bonusLevel} (total level after Taming: {(nudLevel.Value + bonusLevel)})\n\n" +
                        $"Food has to drop by {neededHunger:F1} units.\n\n" +
                        $"{narcoBerries} Narcoberries or\n" +
                        $"{ascerbicMushrooms} Ascerbic Mushrooms or\n" +
                        $"{narcotics} Narcotics or\n" +
                        $"{bioToxines} Bio Toxines are needed{firstFeedingWaiting}";

                labelResult.Text += kibbleRecipe;
            }
            else if (foodAmountUsed.Count == 0)
                labelResult.Text = Loc.S("noTamingData");
            else
                labelResult.Text = Loc.S("notEnoughFoodToTame");

            numericUpDownCurrentTorpor.ValueSave = (decimal)(selectedSpecies.stats[(int)StatNames.Torpidity].BaseValue * (1 + selectedSpecies.stats[(int)StatNames.Torpidity].IncPerWildLevel * (level - 1)));

            nudTotalFood.Value = (decimal)(selectedSpecies.stats[(int)StatNames.Food].BaseValue * (1 + selectedSpecies.stats[(int)StatNames.Food].IncPerWildLevel * (level / 7))); // approximating the food level
            nudCurrentFood.Value = nudTotalFood.Value;
            UpdateTimeToFeedAll(enoughFood);

            //// quicktame infos
            if (foodAmountUsed.Any())
            {
                quickTamingInfos = Taming.QuickInfoOneFood(selectedSpecies, level, tamingSpeedMultiplier, tamingFoodRateMultiplier, foodControls[0].FoodName, foodControls[0].maxFood, foodControls[0].foodNameDisplay);
                // show raw meat or mejoberries as alternative (often used)
                for (int i = 1; i < usedFood.Count; i++)
                {
                    if (usedFood[i] == "Raw Meat" || usedFood[i] == "Mejoberry")
                    {
                        quickTamingInfos += "\n\n" + Taming.QuickInfoOneFood(selectedSpecies, level, tamingSpeedMultiplier, tamingFoodRateMultiplier, foodControls[i].FoodName, foodControls[i].maxFood, foodControls[i].foodNameDisplay);
                        break;
                    }
                }

                quickTamingInfos += "\n\n" + koNumbers
                        + "\n\n" + boneDamageAdjustersImmobilization
                        + firstFeedingWaiting
                        + kibbleRecipe;
            }
        }

        /// <summary>
        /// Update the info when all food can be fed at once.
        /// </summary>
        private void UpdateTimeToFeedAll(bool enoughFood = true)
        {
            double hunger = (double)(nudTotalFood.Value - nudCurrentFood.Value);
            if (hunger < 0) hunger = 0;
            if (hunger > neededHunger) hunger = neededHunger;
            var durationStarving = new TimeSpan(0, 0, (int)((neededHunger - hunger) / foodDepletion));
            lbTimeUntilStarving.Text = (enoughFood ? $"{Loc.S("TimeUntilFeedingAllFood")}: {Utils.Duration(durationStarving)}" : "");
            if ((double)nudTotalFood.Value < neededHunger)
            {
                lbTimeUntilStarving.Text += (lbTimeUntilStarving.Text.Length > 0 ? "\n" : "") + $"{Loc.S("WarningMoreStarvingThanFood")}";
                lbTimeUntilStarving.ForeColor = Color.DarkRed;
            }
            else lbTimeUntilStarving.ForeColor = SystemColors.ControlText;

            starvingTime = DateTime.Now.Add(durationStarving);
        }

        /// <summary>
        /// Displays infos that no taming data is available.
        /// </summary>
        private void NoTamingData()
        {
            // clear text fields
            labelResult.Text = Loc.S("noTamingData");
            lbTimeUntilStarving.Text = Loc.S("noTamingData");

            // disable entire tab
            this.Enabled = false;
        }

        /// <summary>
        /// Only use the passed food for taming, set all other foods to zero.
        /// </summary>
        /// <param name="food"></param>
        private void OnlyOneFood(string food)
        {
            if (selectedSpecies == null)
            {
                return;
            }
            updateCalculation = false;
            foreach (TamingFoodControl tfc in foodControls)
            {
                tfc.amount = tfc.FoodName == food ? tfc.maxFood : 0;
            }
            updateCalculation = true;
            UpdateTamingData();
        }

        private void numericUpDownCurrentTorpor_ValueChanged(object sender, EventArgs e)
        {
            var duration = new TimeSpan(0, 0, Taming.SecondsUntilWakingUp(selectedSpecies, (int)nudLevel.Value, (double)numericUpDownCurrentTorpor.Value));
            lbTimeUntilWakingUp.Text = string.Format(Loc.S("lbTimeUntilWakingUp"), Utils.Duration(duration));
            if (duration.TotalSeconds < 30) lbTimeUntilWakingUp.ForeColor = Color.DarkRed;
            else if (duration.TotalSeconds < 120) lbTimeUntilWakingUp.ForeColor = Color.DarkGoldenrod;
            else lbTimeUntilWakingUp.ForeColor = Color.Black;
            wakeUpTime = DateTime.Now.Add(duration);
        }

        private void nudCurrentFood_ValueChanged(object sender, EventArgs e)
        {
            UpdateTimeToFeedAll();
        }

        private void nudWDm_ValueChanged(object sender, EventArgs e)
        {
            UpdateKOCounting();
        }

        private void chkbDm_CheckedChanged(object sender, EventArgs e)
        {
            UpdateKOCounting();
        }

        private void UpdateKOCounting(double boneDamageAdjuster = 0)
        {
            if (boneDamageAdjuster == 0)
                boneDamageAdjuster = currentBoneDamageAdjuster;
            lbKOInfo.Text = Taming.KnockoutInfo(selectedSpecies, (int)nudLevel.Value,
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

        public double[] WeaponDamages
        {
            get => new[] { (double)nudWDmLongneck.Value, (double)nudWDmCrossbow.Value, (double)nudWDmBow.Value, (double)nudWDmSlingshot.Value, (double)nudWDmClub.Value, (double)nudWDmProd.Value, (double)nudWDmHarpoon.Value };
            set
            {
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

        public int WeaponDamagesEnabled
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
            if (selectedSpecies != null)
                CreateTimer(Loc.S("timerWakeupOf") + " " + selectedSpecies.name, wakeUpTime, null, TimerControl.TimerGroups.Wakeup.ToString());
        }

        private void btnAddStarvingTimer_Click(object sender, EventArgs e)
        {
            if (selectedSpecies != null)
                CreateTimer(Loc.S("timerStarvingOf") + " " + selectedSpecies.name, starvingTime, null, TimerControl.TimerGroups.Starving.ToString());
        }

        public void SetTamingMultipliers(double tamingSpeedMultiplier, double tamingFoodRateMultiplier)
        {
            this.tamingSpeedMultiplier = tamingSpeedMultiplier;
            this.tamingFoodRateMultiplier = tamingFoodRateMultiplier;
            UpdateTamingData();
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
                UpdateKOCounting();
            }
        }

        /// <summary>
        /// Calculate the waiting time after the first feeding, which is different to the other times.
        /// </summary>
        private void UpdateFirstFeedingWaiting()
        {
            int s = Taming.DurationAfterFirstFeeding(selectedSpecies, (int)nudLevel.Value, foodDepletion);
            if (s > 0)
                firstFeedingWaiting = "\n\n" + string.Format(Loc.S("waitingAfterFirstFeeding"), Utils.Duration(s));
            else firstFeedingWaiting = "";
        }

        private void LinkLabelWikiPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string speciesName = linkLabelWikiPage.Tag as string;
            if (!string.IsNullOrEmpty(speciesName))
                System.Diagnostics.Process.Start("https://ark.gamepedia.com/" + speciesName);
        }

        private void checkBoxAugmented_CheckedChanged(object sender, EventArgs e)
        {
            SetSpecies(selectedSpecies, true);
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
