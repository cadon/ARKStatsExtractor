using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.utils;
using ARKBreedingStats.values;

namespace ARKBreedingStats
{
    public partial class TamingControl : UserControl
    {
        private readonly List<TamingFoodControl> _foodControls = new List<TamingFoodControl>();
        private bool _updateCalculation;
        private Species _selectedSpecies;
        public event TimerControl.CreateTimerEventHandler CreateTimer;
        private DateTime _wakeUpTime;
        private DateTime _starvingTime;
        private ServerMultipliers _serverMultipliers =new ServerMultipliers();
        private string _koNumbers;
        private string _boneDamageAdjustersImmobilization;
        public string quickTamingInfos;
        private double _foodDepletion;
        private string _firstFeedingWaiting;
        private string _kibbleRecipe;
        private readonly List<RadioButton> _rbBoneDamageAdjusters;
        private readonly List<double> _rbBoneDamageAdjusterValues;
        private double _currentBoneDamageAdjuster;
        private double _neededHunger;
        private readonly ToolTip _tt = new ToolTip();
        private readonly Debouncer _debouncer = new Debouncer();

        public TamingControl()
        {
            InitializeComponent();
            _updateCalculation = true;
            _wakeUpTime = DateTime.Now;
            _starvingTime = DateTime.Now;
            _rbBoneDamageAdjusters = new List<RadioButton> { rbBoneDamageDefault };
            flcBodyDamageMultipliers.SetFlowBreak(rbBoneDamageDefault, true);
            _rbBoneDamageAdjusterValues = new List<double> { 1 };
        }

        public void SetLevel(int level, bool updateTamingData = true)
        {
            if (nudLevel.Value == level) return;

            bool updateKeeper = _updateCalculation;
            _updateCalculation = updateTamingData;
            nudLevel.ValueSave = level;
            _updateCalculation = updateKeeper;
        }

        public void SetSpecies(Species species, bool forceRefresh = false)
        {
            if (species == null || (_selectedSpecies == species && !forceRefresh))
                return;

            _selectedSpecies = species;

            if (species.taming == null)
            {
                NoTamingData();
                return;
            }

            this.SuspendDrawing();
            SuspendLayout();

            string speciesName = species.name;
            linkLabelWikiPage.Text = "Wiki: " + speciesName;
            linkLabelWikiPage.Tag = speciesName;
            _tt.SetToolTip(linkLabelWikiPage, ArkWiki.WikiUrl(speciesName));

            // bone damage adjusters
            _boneDamageAdjustersImmobilization = Taming.BoneDamageAdjustersImmobilization(_selectedSpecies,
                out Dictionary<string, double> boneDamageAdjusters);

            int ib = 0;
            foreach (KeyValuePair<string, double> bd in boneDamageAdjusters)
            {
                ib++;
                if (ib >= _rbBoneDamageAdjusters.Count)
                {
                    RadioButton rbBD = new RadioButton();
                    flcBodyDamageMultipliers.Controls.Add(rbBD);
                    flcBodyDamageMultipliers.SetFlowBreak(rbBD, true);
                    rbBD.AutoSize = true;

                    _rbBoneDamageAdjusters.Add(rbBD);
                    _rbBoneDamageAdjusterValues.Add(1);
                    rbBD.CheckedChanged += rbBoneDamage_CheckedChanged;
                }
                _rbBoneDamageAdjusterValues[ib] = bd.Value;
                _rbBoneDamageAdjusters[ib].Text = $"{Loc.S(bd.Key)} (× {bd.Value})";
                _rbBoneDamageAdjusters[ib].Visible = true;
            }
            for (int j = ib + 1; j < _rbBoneDamageAdjusters.Count; j++)
                _rbBoneDamageAdjusters[j].Visible = false;
            _rbBoneDamageAdjusters[0].Checked = true;
            // bone damage adjusters adjusted

            _updateCalculation = false;
            TamingData td = species.taming;
            _kibbleRecipe = string.Empty;

            // list all recipes of kibbles that give a reasonable affinity (assuming that is larger or equal than 100)
            foreach (var k in Kibbles.K.kibble)
            {
                var kibbleName = $"{k.Key} Kibble";
                var kibbleFood = Values.V.GetTamingFood(species, kibbleName);

                if (kibbleFood != null
                    && kibbleFood.affinity >= 100)
                {
                    _kibbleRecipe += $"\n\n{k.Key} Kibble:{k.Value.RecipeAsText()}";
                }
            }

            _foodDepletion = td.foodConsumptionBase * td.foodConsumptionMult * _serverMultipliers.DinoCharacterFoodDrainMultiplier;

            SetTamingFoodControls(species);

            _updateCalculation = true;
            UpdateFirstFeedingWaiting();
            UpdateTamingData();
            if (Properties.Settings.Default.TamingFoodOrderByTime)
                SetOrderOfTamingFood(true, true);

            ResumeLayout();
            this.ResumeDrawing();
        }

        /// <summary>
        /// Sets the taming food controls.
        /// </summary>
        private void SetTamingFoodControls(Species species)
        {
            var td = species.taming;
            int i = 0;
            if (td.eats != null)
            {
                var length = td.eats.Length;
                for (; i < length; i++)
                {
                    string f = td.eats[i];
                    TamingFoodControl tf;

                    // if Augmented are not wanted, and food control already exist, update it and hide it.
                    if (!checkBoxAugmented.Checked && f.Contains("Augmented"))
                    {
                        if (i < _foodControls.Count)
                        {
                            tf = _foodControls[i];
                            tf.FoodName = f;
                            tf.Hide();
                        }
                        continue;
                    }

                    if (i >= _foodControls.Count)
                    {
                        tf = new TamingFoodControl(f);
                        tf.valueChanged += UpdateTamingData;
                        tf.Clicked += OnlyOneFood;
                        _foodControls.Add(tf);
                        flpTamingFood.Controls.Add(tf);
                    }
                    else
                    {
                        tf = _foodControls[i];
                        tf.FoodName = f;
                        flpTamingFood.Controls.SetChildIndex(tf, i);
                        tf.Show();
                    }

                    // special cases where a creature eats multiple food items of one kind at once
                    var food = Values.V.GetTamingFood(species, f);
                    if (food != null && food.quantity > 1)
                        tf.foodNameDisplay = food.quantity + "× " + tf.foodNameDisplay;
                }
            }

            for (int fci = _foodControls.Count - 1; fci >= i; fci--)
            {
                _foodControls[fci].FoodName = null;
                _foodControls[fci].Hide();
            }

            if (i > 0)
                _foodControls[0].amount = Taming.FoodAmountNeeded(species, (int)nudLevel.Value, _serverMultipliers.TamingSpeedMultiplier, _foodControls[0].FoodName, td.nonViolent, CbSanguineElixir.Checked);
        }

        /// <summary>
        ///  If orderByFoodAmount is false, order by taming time.
        /// </summary>
        private void SetOrderOfTamingFood(bool orderByTamingTime, bool forceDo = false)
        {
            if (Properties.Settings.Default.TamingFoodOrderByTime == orderByTamingTime && !forceDo)
                return;

            Properties.Settings.Default.TamingFoodOrderByTime = orderByTamingTime;

            var order = _foodControls.Where(c => c.FoodName != null)
                .Select(c => (c, orderByTamingTime ? c.TamingSeconds : c.maxFood)).OrderBy(ct => ct.Item2).ToArray();

            this.SuspendDrawing();
            for (int i = 0; i < order.Length; i++)
                flpTamingFood.Controls.SetChildIndex(order[i].c, i);

            SetTamingFoodSortAdorner(orderByTamingTime);
            this.ResumeDrawing();
        }

        private void SetTamingFoodSortAdorner(bool orderByTamingTime)
        {
            Loc.ControlText(lbMax);
            Loc.ControlText(lbTamingTime);
            if (orderByTamingTime)
                lbTamingTime.Text += "▲";
            else
                lbMax.Text += "▲";
        }

        private void nudLevel_ValueChanged(object sender, EventArgs e)
        {
            _debouncer.Debounce(300,
                () =>
                {
                    UpdateFirstFeedingWaiting();
                    UpdateTamingData();
                },
                Dispatcher.CurrentDispatcher);
        }

        private void UpdateTamingData()
        {
            if (!_updateCalculation || _selectedSpecies == null)
            {
                return;
            }
            if (_selectedSpecies.taming == null)
            {
                NoTamingData();
                return;
            }

            Enabled = true;
            UpdateKOCounting();

            TimeSpan duration = new TimeSpan();
            int narcoBerries = 0, ascerbicMushrooms = 0, narcotics = 0, bioToxines = 0, bonusLevel = 0;
            double te = 0;
            _neededHunger = 0;
            bool enoughFood = false;
            var usedFood = new List<string>();
            var foodAmount = new List<int>();
            var foodAmountUsed = new List<int>();
            quickTamingInfos = "n/a";
            int level = (int)nudLevel.Value;
            bool tameable = _selectedSpecies.taming.nonViolent || _selectedSpecies.taming.violent;

            if (tameable && _selectedSpecies.taming.eats != null)
            {
                int foodCounter = _selectedSpecies.taming.eats.Length;
                foreach (TamingFoodControl tfc in _foodControls)
                {
                    if (foodCounter == 0)
                        break;
                    foodCounter--;

                    usedFood.Add(tfc.FoodName);
                    foodAmount.Add(tfc.amount);
                    tfc.maxFood = Taming.FoodAmountNeeded(_selectedSpecies, level, _serverMultipliers.TamingSpeedMultiplier, tfc.FoodName, _selectedSpecies.taming.nonViolent, CbSanguineElixir.Checked);
                    tfc.tamingDuration = Taming.TamingDuration(_selectedSpecies, tfc.maxFood, tfc.FoodName, _serverMultipliers.DinoCharacterFoodDrainMultiplier, _selectedSpecies.taming.nonViolent);
                }
                Taming.TamingTimes(_selectedSpecies, level, _serverMultipliers, usedFood, foodAmount,
                    out foodAmountUsed, out duration, out narcoBerries, out ascerbicMushrooms, out narcotics, out bioToxines, out te, out _neededHunger, out bonusLevel, out enoughFood, CbSanguineElixir.Checked);

                for (int f = 0; f < foodAmountUsed.Count; f++)
                {
                    _foodControls[f].foodUsed = foodAmountUsed[f];
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
                labelResult.Text = $"It takes {Utils.DurationUntil(duration)} to tame the {_selectedSpecies.name}.\n\n" +
                        $"Taming Effectiveness: {Math.Round(100 * te, 1)} %\n" +
                        $"Bonus-Level: +{bonusLevel} (total level after Taming: {(nudLevel.Value + bonusLevel)})\n\n" +
                        $"Food has to drop by {_neededHunger:F1} units.\n\n" +
                        $"{narcoBerries} Narcoberries or\n" +
                        $"{ascerbicMushrooms} Ascerbic Mushrooms or\n" +
                        $"{narcotics} Narcotics or\n" +
                        $"{bioToxines} Bio Toxines are needed{_firstFeedingWaiting}";

                labelResult.Text += _kibbleRecipe;
            }
            else if (foodAmountUsed.Count == 0)
                labelResult.Text = Loc.S("noTamingData");
            else
                labelResult.Text = Loc.S("notEnoughFoodToTame");

            numericUpDownCurrentTorpor.ValueSave = (decimal)(_selectedSpecies.stats[Stats.Torpidity].BaseValue * (1 + _selectedSpecies.stats[Stats.Torpidity].IncPerWildLevel * (level - 1)));

            nudTotalFood.Value = (decimal)(_selectedSpecies.stats[Stats.Food].BaseValue * (1 + _selectedSpecies.stats[Stats.Food].IncPerWildLevel * (level / 7))); // approximating the food level
            nudCurrentFood.Value = nudTotalFood.Value;
            UpdateTimeToFeedAll(enoughFood);

            //// quickTame infos
            if (foodAmountUsed.Any())
            {
                quickTamingInfos = Taming.QuickInfoOneFood(_selectedSpecies, level, _serverMultipliers, _foodControls[0].FoodName, _foodControls[0].maxFood, _foodControls[0].foodNameDisplay);
                // show raw meat or mejoberries as alternative (often used)
                for (int i = 1; i < usedFood.Count; i++)
                {
                    if (usedFood[i] == "Raw Meat" || usedFood[i] == "Mejoberry")
                    {
                        quickTamingInfos += "\n\n" + Taming.QuickInfoOneFood(_selectedSpecies, level, _serverMultipliers, _foodControls[i].FoodName, _foodControls[i].maxFood, _foodControls[i].foodNameDisplay);
                        break;
                    }
                }

                quickTamingInfos += "\n\n" + _koNumbers
                        + "\n\n" + _boneDamageAdjustersImmobilization
                        + _firstFeedingWaiting
                        + _kibbleRecipe;
            }
        }

        /// <summary>
        /// Update the info when all food can be fed at once.
        /// </summary>
        private void UpdateTimeToFeedAll(bool enoughFood = true)
        {
            double hunger = (double)(nudTotalFood.Value - nudCurrentFood.Value);
            if (hunger < 0) hunger = 0;
            if (hunger > _neededHunger) hunger = _neededHunger;
            var durationStarving = new TimeSpan(0, 0, (int)((_neededHunger - hunger) / _foodDepletion));
            lbTimeUntilStarving.Text = (enoughFood ? $"{Loc.S("TimeUntilFeedingAllFood")}: {Utils.Duration(durationStarving)}" : string.Empty);
            if ((double)nudTotalFood.Value < _neededHunger)
            {
                lbTimeUntilStarving.Text += (lbTimeUntilStarving.Text.Length > 0 ? "\n" : string.Empty) + $"{Loc.S("WarningMoreStarvingThanFood")}";
                lbTimeUntilStarving.ForeColor = Color.DarkRed;
            }
            else lbTimeUntilStarving.ForeColor = SystemColors.ControlText;

            _starvingTime = DateTime.Now.Add(durationStarving);
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
        private void OnlyOneFood(string food)
        {
            if (_selectedSpecies == null)
            {
                return;
            }
            _updateCalculation = false;
            foreach (TamingFoodControl tfc in _foodControls)
            {
                tfc.amount = tfc.FoodName == food ? tfc.maxFood : 0;
            }
            _updateCalculation = true;
            UpdateTamingData();
        }

        private void numericUpDownCurrentTorpor_ValueChanged(object sender, EventArgs e)
        {
            var duration = new TimeSpan(0, 0, Taming.SecondsUntilWakingUp(_selectedSpecies, _serverMultipliers, (int)nudLevel.Value, (double)numericUpDownCurrentTorpor.Value));
            lbTimeUntilWakingUp.Text = string.Format(Loc.S("lbTimeUntilWakingUp"), Utils.Duration(duration));
            if (duration.TotalSeconds < 30) lbTimeUntilWakingUp.ForeColor = Color.DarkRed;
            else if (duration.TotalSeconds < 120) lbTimeUntilWakingUp.ForeColor = Color.DarkGoldenrod;
            else lbTimeUntilWakingUp.ForeColor = Color.Black;
            _wakeUpTime = DateTime.Now.Add(duration);
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
                boneDamageAdjuster = _currentBoneDamageAdjuster;
            lbKOInfo.Text = Taming.KnockoutInfo(_selectedSpecies, _serverMultipliers, (int)nudLevel.Value,
                    chkbDmLongneck.Checked ? (double)nudWDmLongneck.Value / 100 : 0,
                    chkbDmCrossbow.Checked ? (double)nudWDmCrossbow.Value / 100 : 0,
                    chkbDmBow.Checked ? (double)nudWDmBow.Value / 100 : 0,
                    chkbDmSlingshot.Checked ? (double)nudWDmSlingshot.Value / 100 : 0,
                    chkbDmClub.Checked ? (double)nudWDmClub.Value / 100 : 0,
                    chkbDmProd.Checked ? (double)nudWDmProd.Value / 100 : 0,
                    chkbDmHarpoon.Checked ? (double)nudWDmHarpoon.Value / 100 : 0,
                    boneDamageAdjuster,
                    out bool knockoutNeeded, out _koNumbers)
                            + (string.IsNullOrEmpty(_boneDamageAdjustersImmobilization) ? string.Empty : "\n\n" + _boneDamageAdjustersImmobilization);
            lbKOInfo.ForeColor = knockoutNeeded ? SystemColors.ControlText : SystemColors.GrayText;
            if (!knockoutNeeded)
                _koNumbers = string.Empty;
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
            if (_selectedSpecies != null)
                CreateTimer(Loc.S("timerWakeupOf") + " " + _selectedSpecies.name, _wakeUpTime, null, TimerControl.TimerGroups.Wakeup.ToString());
        }

        private void btnAddStarvingTimer_Click(object sender, EventArgs e)
        {
            if (_selectedSpecies != null)
                CreateTimer(Loc.S("timerStarvingOf") + " " + _selectedSpecies.name, _starvingTime, null, TimerControl.TimerGroups.Starving.ToString());
        }

        public void SetServerMultipliers(ServerMultipliers serverMultipliers)
        {
            _serverMultipliers = serverMultipliers;
            UpdateTamingData();
        }

        private void rbBoneDamage_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Checked)
            {
                int i = _rbBoneDamageAdjusters.IndexOf(rb);
                if (i >= 0)
                    _currentBoneDamageAdjuster = _rbBoneDamageAdjusterValues[i];
                else
                    _currentBoneDamageAdjuster = 1;
                UpdateKOCounting();
            }
        }

        /// <summary>
        /// Calculate the waiting time after the first feeding, which is different to the other times.
        /// </summary>
        private void UpdateFirstFeedingWaiting()
        {
            int s = Taming.DurationAfterFirstFeeding(_selectedSpecies, (int)nudLevel.Value, _foodDepletion);
            if (s > 0)
                _firstFeedingWaiting = "\n\n" + string.Format(Loc.S("waitingAfterFirstFeeding"), Utils.Duration(s));
            else _firstFeedingWaiting = string.Empty;
        }

        private void LinkLabelWikiPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ArkWiki.OpenPage(linkLabelWikiPage.Tag as string);
        }

        private void checkBoxAugmented_CheckedChanged(object sender, EventArgs e)
        {
            SetSpecies(_selectedSpecies, true);
        }

        private void CbSanguineElixir_CheckedChanged(object sender, EventArgs e)
        {
            SetSpecies(_selectedSpecies, true);
        }

        public void SetLocalizations()
        {
            SetTamingFoodSortAdorner(Properties.Settings.Default.TamingFoodOrderByTime);
            Loc.ControlText(lbUsed);
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

        private void lbTamingTime_Click(object sender, EventArgs e)
        {
            // order by time
            SetOrderOfTamingFood(true);
        }

        private void lbMax_Click(object sender, EventArgs e)
        {
            // order by food amount
            SetOrderOfTamingFood(false);
        }
    }
}
