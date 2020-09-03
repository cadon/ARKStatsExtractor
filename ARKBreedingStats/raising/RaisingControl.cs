using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ARKBreedingStats.raising
{
    public partial class RaisingControl : UserControl
    {
        public delegate void ExtractBabyEventHandler(Creature mother, Creature father);

        public event ExtractBabyEventHandler extractBaby;
        public event Form1.CollectionChangedEventHandler onChange;
        public event Action<Species> SetGlobalSpecies;
        private Species selectedSpecies;
        public bool updateListView;
        private TimeSpan babyTime, maturationTime;
        private CreatureCollection cc;
        public TimerControl timerControl;
        private IncubationTimerEntry iteEdit;
        private Creature creatureMaturationEdit;

        public delegate void AdjustTimersByOffsetEventHandler(TimeSpan offset);

        public event AdjustTimersByOffsetEventHandler AdjustTimers;

        public RaisingControl()
        {
            InitializeComponent();
            listViewBabies.Groups.Add("incubation", "Incubation/Gestation");
            listViewBabies.Groups.Add("baby", "Babies");
            listViewBabies.Groups.Add("growing", "Juveniles / Adolescent");
            updateListView = false;
            listViewBabies.DoubleBuffered(true); // prevent flickering
            listViewBabies.ListViewItemSorter = new ListViewColumnSorter();
        }

        public void UpdateRaisingData()
        {
            UpdateRaisingData(selectedSpecies, true);
        }

        /// <summary>
        /// Updates the general raising data for the given species
        /// </summary>
        /// <param name="species"></param>
        /// <param name="forceUpdate"></param>
        public void UpdateRaisingData(Species species, bool forceUpdate = false)
        {
            if (forceUpdate || this.selectedSpecies != species)
            {
                selectedSpecies = species;
                if (selectedSpecies?.taming != null && selectedSpecies.breeding != null)
                {
                    this.SuspendLayout();

                    listViewRaisingTimes.Items.Clear();

                    if (Raising.GetRaisingTimes(selectedSpecies, out TimeSpan matingTime, out string incubationMode,
                        out TimeSpan incubationTime, out babyTime, out maturationTime, out TimeSpan nextMatingMin,
                        out TimeSpan nextMatingMax))
                    {
                        if (matingTime != TimeSpan.Zero)
                            listViewRaisingTimes.Items.Add(new ListViewItem(new[]
                                {Loc.S("matingTime"), matingTime.ToString("d':'hh':'mm':'ss")}));

                        TimeSpan totalTime = incubationTime;
                        DateTime until = DateTime.Now.Add(totalTime);
                        string[] times =
                        {
                            incubationMode, incubationTime.ToString("d':'hh':'mm':'ss"),
                            totalTime.ToString("d':'hh':'mm':'ss"), Utils.ShortTimeDate(until)
                        };
                        listViewRaisingTimes.Items.Add(new ListViewItem(times));

                        totalTime += babyTime;
                        until = DateTime.Now.Add(totalTime);
                        times = new[]
                        {
                            Loc.S("Baby"), babyTime.ToString("d':'hh':'mm':'ss"),
                            totalTime.ToString("d':'hh':'mm':'ss"), Utils.ShortTimeDate(until)
                        };
                        listViewRaisingTimes.Items.Add(new ListViewItem(times));

                        totalTime = incubationTime + maturationTime;
                        until = DateTime.Now.Add(totalTime);
                        times = new[]
                        {
                            Loc.S("Maturation"), maturationTime.ToString("d':'hh':'mm':'ss"),
                            totalTime.ToString("d':'hh':'mm':'ss"), Utils.ShortTimeDate(until)
                        };
                        listViewRaisingTimes.Items.Add(new ListViewItem(times));

                        // food amount needed
                        string foodamount = null;
                        if (selectedSpecies.taming.eats != null
                            && selectedSpecies.taming.eats.Any()
                            && uiControls.Trough.foodAmount(selectedSpecies,
                                Values.V.currentServerMultipliers.BabyFoodConsumptionSpeedMultiplier,
                                out double babyfood, out double totalfood))
                        {
                            foodamount = FoodAmountString("Raw Meat");
                            if (string.IsNullOrEmpty(foodamount))
                                foodamount = FoodAmountString("Mejoberries");
                            if (string.IsNullOrEmpty(foodamount))
                                foodamount = FoodAmountString("Raw Prime Meat");
                            if (string.IsNullOrEmpty(foodamount))
                                foodamount = FoodAmountString("Raw Mutton");
                            if (string.IsNullOrEmpty(foodamount))
                                foodamount = FoodAmountString(selectedSpecies.taming.eats[0]);

                            string FoodAmountString(string _foodName)
                            {
                                if (Array.IndexOf(selectedSpecies.taming.eats, _foodName) == -1) return null;
                                double foodValue;
                                if (selectedSpecies.taming.specialFoodValues.TryGetValue(_foodName, out TamingFood tf))
                                    foodValue = tf.foodValue;
                                else if (Values.V.defaultFoodData.TryGetValue(_foodName, out tf))
                                    foodValue = tf.foodValue;
                                else return null;
                                if (foodValue == 0) return null;
                                return $"\n\nFood for Baby-Phase: ~{Math.Ceiling(babyfood / foodValue)} {_foodName}"
                                       + $"\nTotal Food for maturation: ~{Math.Ceiling(totalfood / foodValue)} {_foodName}";
                            }

                            foodamount += "\n - Loss by spoiling is only a rough estimate and may vary.";
                        }


                        var raisingInfo = new StringBuilder();
                        if (nextMatingMin != TimeSpan.Zero)
                            raisingInfo.AppendLine(
                                $"{Loc.S("TimeBetweenMating")}: {nextMatingMin:d':'hh':'mm':'ss} to {nextMatingMax:d':'hh':'mm':'ss}");

                        string eggInfo = Raising.EggTemperature(selectedSpecies);
                        if (!string.IsNullOrEmpty(eggInfo))
                            raisingInfo.AppendLine(eggInfo);
                        if (!string.IsNullOrEmpty(foodamount))
                            raisingInfo.AppendLine(foodamount);

                        labelRaisingInfos.Text = raisingInfo.ToString().Trim();

                        tabPageMaturationProgress.Enabled = true;
                    }
                    else
                    {
                        labelRaisingInfos.Text = "No raising-data available.";
                        tabPageMaturationProgress.Enabled = false;
                    }

                    ResumeLayout();
                }
                else
                {
                    // no taming- or breeding-data available
                    labelRaisingInfos.Text = "No raising-data available.";
                    tabPageMaturationProgress.Enabled = false;
                }
            }
        }

        public CreatureCollection creatureCollection
        {
            set
            {
                if (value != null)
                {
                    cc = value;
                    parentStats1.maxChartLevel = cc.maxChartLevel;
                }
            }
        }

        private void nudMaturationProgress_ValueChanged(object sender, EventArgs e)
        {
            UpdateMaturationProgress();
        }

        private void UpdateMaturationProgress()
        {
            double maturation = (double)nudMaturationProgress.Value / 100;
            double maturationSeconds = maturationTime.TotalSeconds * maturation;
            if (maturationSeconds < babyTime.TotalSeconds)
            {
                labelTimeLeftBaby.Text =
                    Utils.DurationUntil(babyTime.Subtract(new TimeSpan(0, 0, (int)(maturationSeconds))));
                labelTimeLeftBaby.ForeColor = SystemColors.ControlText;
            }
            else
            {
                labelTimeLeftBaby.Text = Loc.S("notABabyAnymore");
                labelTimeLeftBaby.ForeColor = SystemColors.GrayText;
            }

            if (maturation < 1)
            {
                labelTimeLeftGrowing.Text =
                    Utils.DurationUntil(maturationTime.Subtract(new TimeSpan(0, 0, (int)(maturationSeconds))));
                labelTimeLeftGrowing.ForeColor = SystemColors.ControlText;
            }
            else
            {
                labelTimeLeftGrowing.Text = Loc.S("mature");
                labelTimeLeftGrowing.ForeColor = SystemColors.GrayText;
            }

            string foodAmountBabyString = null;
            string foodAmountAdultString = null;
            if (selectedSpecies.taming.eats != null)
            {
                double foodAmount;
                if (Array.IndexOf(selectedSpecies.taming.eats, "Raw Meat") != -1)
                {
                    if (uiControls.Trough.foodAmountFromUntil(selectedSpecies,
                        Values.V.currentServerMultipliers.BabyFoodConsumptionSpeedMultiplier, maturation, 0.1,
                        out foodAmount))
                        foodAmountBabyString = Math.Ceiling(foodAmount / 50) + " Raw Meat";
                    if (uiControls.Trough.foodAmountFromUntil(selectedSpecies,
                        Values.V.currentServerMultipliers.BabyFoodConsumptionSpeedMultiplier, maturation, 1,
                        out foodAmount))
                        foodAmountAdultString = Math.Ceiling(foodAmount / 50) + " Raw Meat";
                }
                else if (Array.IndexOf(selectedSpecies.taming.eats, "Mejoberry") != -1)
                {
                    if (uiControls.Trough.foodAmountFromUntil(selectedSpecies,
                        Values.V.currentServerMultipliers.BabyFoodConsumptionSpeedMultiplier, maturation, 0.1,
                        out foodAmount))
                        foodAmountBabyString = Math.Ceiling(foodAmount / 30) + " Mejoberries";
                    if (uiControls.Trough.foodAmountFromUntil(selectedSpecies,
                        Values.V.currentServerMultipliers.BabyFoodConsumptionSpeedMultiplier, maturation, 1,
                        out foodAmount))
                        foodAmountAdultString = Math.Ceiling(foodAmount / 30) + " Mejoberries";
                }
            }

            labelAmountFoodBaby.Text = foodAmountBabyString;
            labelAmountFoodAdult.Text = foodAmountAdultString;
        }

        public void AddIncubationTimer(Creature mother, Creature father, TimeSpan incubationDuration,
            bool incubationStarted)
        {
            cc.incubationListEntries.Add(
                new IncubationTimerEntry(mother, father, incubationDuration, incubationStarted));
            onChange?.Invoke();
            RecreateList();
        }

        private void RemoveIncubationTimer(IncubationTimerEntry ite)
        {
            cc.incubationListEntries.Remove(ite);
        }

        /// <summary>
        /// Update timer list, checks every creature if it needs to be added
        /// </summary>
        public void RecreateList()
        {
            if (cc == null)
                return;

            updateListView = false;
            listViewBabies.BeginUpdate();
            listViewBabies.Items.Clear();

            // if both parents of an incubation entry were deleted, remove that entry as well.
            cc.incubationListEntries =
                cc.incubationListEntries.Where(t => t.mother != null || t.father != null).ToList();

            ListViewGroup g = listViewBabies.Groups[0];
            // add eggs / pregnancies
            foreach (IncubationTimerEntry t in cc.incubationListEntries)
            {
                Species species = t.mother?.Species ?? t.father?.Species;
                if (species?.breeding != null)
                {
                    t.kind = species.breeding.gestationTimeAdjusted > 0 ? "Gestation" : "Egg";
                    string[] cols =
                    {
                        t.kind,
                        species.name,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty
                    };
                    ListViewItem lvi = new ListViewItem(cols, g);
                    t.expired = (t.incubationEnd.Subtract(DateTime.Now).TotalSeconds < 0);
                    lvi.Tag = t;
                    listViewBabies.Items.Add(lvi);
                }
            }

            // add babies / growing
            DateTime now = DateTime.Now;
            foreach (Creature c in cc.creatures)
            {
                if (c.growingUntil.HasValue
                    && (c.growingUntil > now
                        || (c.growingPaused && c.growingLeft.TotalHours > 0)))
                {
                    Species species = c.Species;
                    if (species?.breeding != null)
                    {
                        DateTime babyUntil = c.growingPaused
                            ? now.Add(c.growingLeft).AddSeconds(-0.9 * species.breeding.maturationTimeAdjusted)
                            : c.growingUntil.Value.AddSeconds(-0.9 * species.breeding.maturationTimeAdjusted);
                        string[] cols;
                        if (babyUntil > now)
                        {
                            g = listViewBabies.Groups[1];
                            cols = new[]
                            {
                                c.name,
                                c.Species.name,
                                "-",
                                string.Empty,
                                string.Empty,
                                string.Empty
                            };
                        }
                        else
                        {
                            g = listViewBabies.Groups[2];
                            cols = new[]
                            {
                                c.name,
                                c.Species.name,
                                "-",
                                "-",
                                string.Empty,
                                string.Empty
                            };
                        }

                        ListViewItem lvi = new ListViewItem(cols, g)
                        {
                            Tag = c
                        };
                        listViewBabies.Items.Add(lvi);
                    }
                }
            }

            listViewBabies.EndUpdate();
            updateListView = true;
        }

        public void Tick()
        {
            DateTime now = DateTime.Now;
            DateTime alertTime = now.AddMinutes(1);
            if (updateListView)
            {
                listViewBabies.BeginUpdate();
                foreach (ListViewItem lvi in listViewBabies.Items)
                {
                    if ((lvi.Tag.GetType() == typeof(IncubationTimerEntry)))
                    {
                        var t = (IncubationTimerEntry)lvi.Tag;

                        Species species = t.mother?.Species ?? t.father?.Species;
                        if (species?.breeding != null)
                        {
                            lvi.SubItems[3].Text = Utils.Duration((int)(species.breeding.maturationTimeAdjusted / 10));
                            lvi.SubItems[4].Text = Utils.Duration((int)species.breeding.maturationTimeAdjusted);
                        }

                        if (t.expired)
                        {
                            lvi.SubItems[2].Text = Utils.TimeLeft(t.incubationEnd);
                            lvi.SubItems[5].Text = Loc.S("expired");
                        }
                        else if (!t.timerIsRunning)
                        {
                            lvi.SubItems[2].Text = Utils.Duration(t.incubationDuration);
                            lvi.SubItems[5].Text = Loc.S("paused");
                        }
                        else
                        {
                            lvi.SubItems[2].Text = Utils.TimeLeft(t.incubationEnd);
                            lvi.SubItems[5].Text = string.Empty;
                            double diff = t.incubationEnd.Subtract(alertTime).TotalSeconds;
                            if (diff >= 0 && diff < 1)
                            {
                                timerControl.PlaySound("Birth", 1);
                            }
                            else if (diff < 0)
                            {
                                t.expired = true;
                            }
                        }
                    }
                    else if ((lvi.Tag.GetType() == typeof(Creature)))
                    {
                        var c = (Creature)lvi.Tag;
                        Species species = c.Species;
                        if (species?.breeding != null)
                        {
                            if (c.growingPaused)
                            {
                                DateTime babyUntil = now.Add(c.growingLeft)
                                    .AddSeconds(-0.9 * species.breeding.maturationTimeAdjusted);
                                lvi.SubItems[3].Text = Utils.TimeLeft(babyUntil);
                                lvi.SubItems[4].Text = Utils.Duration(c.growingLeft);
                                lvi.SubItems[5].Text = Loc.S("paused");
                            }
                            else if (c.growingUntil.HasValue)
                            {
                                DateTime babyUntil =
                                    c.growingUntil.Value.AddSeconds(-0.9 * species.breeding.maturationTimeAdjusted);
                                lvi.SubItems[3].Text = Utils.TimeLeft(babyUntil);
                                lvi.SubItems[4].Text = Utils.TimeLeft(c.growingUntil.Value);
                                lvi.SubItems[5].Text = string.Empty;
                            }
                        }
                    }
                }

                listViewBabies.EndUpdate();
            }
            else
            {
                foreach (ListViewItem lvi in listViewBabies.Items)
                {
                    if ((lvi.Tag.GetType() == typeof(IncubationTimerEntry)))
                    {
                        var t = (IncubationTimerEntry)lvi.Tag;
                        double diff = t.incubationEnd.Subtract(alertTime).TotalSeconds;
                        if (diff >= 0 && diff < 1)
                            timerControl.PlaySound("Birth", 1);
                    }
                }
            }
        }

        private void extractValuesOfHatchedbornBabyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewBabies.SelectedIndices.Count > 0 &&
                listViewBabies.SelectedItems[0].Tag.GetType() == typeof(IncubationTimerEntry))
            {
                var ite = (IncubationTimerEntry)listViewBabies.SelectedItems[0].Tag;
                extractBaby?.Invoke(ite.mother, ite.father);
            }
        }

        private void deleteTimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewBabies.SelectedIndices.Count > 0)
            {
                if (listViewBabies.SelectedItems[0].Tag.GetType() == typeof(IncubationTimerEntry))
                {
                    IncubationTimerEntry ite = (IncubationTimerEntry)listViewBabies.SelectedItems[0].Tag;
                    if (MessageBox.Show("Delete this timer?\n" + (ite.mother?.Species?.name ?? "unknown") +
                                        ", ending in " + Utils.TimeLeft(ite.incubationEnd)
                                        + (listViewBabies.SelectedIndices.Count > 1
                                            ? "\n\nand " + (listViewBabies.SelectedIndices.Count - 1).ToString() +
                                              " more selected timers"
                                            : string.Empty) + "?"
                        , "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        for (int t = listViewBabies.SelectedIndices.Count - 1; t >= 0; t--)
                            RemoveIncubationTimer((IncubationTimerEntry)listViewBabies.SelectedItems[t].Tag);

                        RecreateList();
                        onChange?.Invoke();
                    }
                }
                else
                {
                    // selected entry is not an egg-timer
                    MessageBox.Show(
                        "The selected timer is an entry for a growing creature and cannot be deleted directly.\nTo remove it, delete the according creature or set the creature to mature",
                        "Timer cannot be deleted",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        public void DeleteAllExpiredIncubationTimers()
        {
            if (MessageBox.Show("Delete all expired incubation timers?", "Delete?", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DateTime now = DateTime.Now;
                foreach (ListViewItem lvi in listViewBabies.Items)
                {
                    if ((lvi.Tag.GetType() == typeof(IncubationTimerEntry)))
                    {
                        IncubationTimerEntry ite = (IncubationTimerEntry)lvi.Tag;
                        if (ite.incubationEnd < now)
                            RemoveIncubationTimer(ite);
                    }
                }

                RecreateList();
                onChange?.Invoke();
            }
        }

        private void removeAllExpiredTimersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteAllExpiredIncubationTimers();
        }

        private void listViewBabies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewBabies.SelectedIndices.Count > 0)
            {
                if (listViewBabies.SelectedItems[0].Tag is Creature c)
                {
                    Species species = c.Species;
                    SetGlobalSpecies?.Invoke(species);
                    if (species?.breeding != null && c.growingUntil.HasValue && c.growingUntil.Value > DateTime.Now)
                    {
                        double maturing = 100 * (1 - c.growingUntil.Value.Subtract(DateTime.Now).TotalSeconds /
                            species.breeding.maturationTimeAdjusted);
                        if (maturing > 0 && maturing <= 100)
                        {
                            nudMaturationProgress.Value = (decimal)maturing;
                        }
                    }

                    parentStats1.SetParentValues(c.Mother, c.Father);

                    // edit-box
                    creatureMaturationEdit = c;
                    iteEdit = null;

                    SetEditTimer();
                }
                else if (listViewBabies.SelectedItems[0].Tag is IncubationTimerEntry ite)
                {
                    Species species = ite.mother.Species;
                    SetGlobalSpecies?.Invoke(species);

                    parentStats1.SetParentValues(ite.mother, ite.father);

                    // edit-box
                    creatureMaturationEdit = null;
                    iteEdit = ite;

                    SetEditTimer();
                }
            }
            else
            {
                iteEdit = null;
                creatureMaturationEdit = null;
                SetEditTimer();
            }
        }

        private void SetEditTimer()
        {
            if (iteEdit != null)
            {
                lEditTimerName.Text =
                    $"{Loc.S("incubation")}{(iteEdit.mother != null ? " (" + (iteEdit.mother.Species?.name ?? Loc.S("Unknown")) + ")" : string.Empty)}";
                dateTimePickerEditTimerFinish.Value = iteEdit.incubationEnd;
                TimeSpan ts = iteEdit.incubationEnd.Subtract(DateTime.Now);
                dhmsInputTimerEditTimer.Timespan = (ts.TotalSeconds > 0 ? ts : TimeSpan.Zero);

            }
            else if (creatureMaturationEdit?.growingUntil != null)
            {
                lEditTimerName.Text =
                    $"{creatureMaturationEdit.name} ({(creatureMaturationEdit.Species?.name ?? Loc.S("Unknown"))})";
                TimeSpan ts;
                if (creatureMaturationEdit.growingPaused)
                {
                    dateTimePickerEditTimerFinish.Value = DateTime.Now.Add(creatureMaturationEdit.growingLeft);
                    ts = creatureMaturationEdit.growingLeft;
                }
                else
                {
                    dateTimePickerEditTimerFinish.Value = creatureMaturationEdit.growingUntil.Value;
                    ts = creatureMaturationEdit.growingUntil.Value.Subtract(DateTime.Now);
                }

                dhmsInputTimerEditTimer.Timespan = (ts > TimeSpan.Zero ? ts : TimeSpan.Zero);
            }
            else
            {
                lEditTimerName.Text = Loc.S("noTimerSelected");
                dateTimePickerEditTimerFinish.Value = DateTime.Now;
                dhmsInputTimerEditTimer.Timespan = TimeSpan.Zero;
            }
        }

        private void dhmsInputTimerEditTimer_TextChanged(object sender, EventArgs e)
        {
            if (dhmsInputTimerEditTimer.Focused)
                dateTimePickerEditTimerFinish.Value = DateTime.Now.Add(dhmsInputTimerEditTimer.Timespan);
        }

        private void dateTimePickerEditTimerFinish_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePickerEditTimerFinish.Focused)
                dhmsInputTimerEditTimer.Timespan = dateTimePickerEditTimerFinish.Value.Subtract(DateTime.Now);
        }

        private void dhmsInputTimerEditTimer_ValueChanged(uiControls.dhmsInput sender, TimeSpan timespan)
        {
            dateTimePickerEditTimerFinish.Value = DateTime.Now.Add(timespan);
        }

        private void btStartPauseTimer_Click(object sender, EventArgs e)
        {
            if (listViewBabies.SelectedIndices.Count > 0)
            {
                bool timerRunning = true;
                for (int i = 0; i < listViewBabies.SelectedIndices.Count; i++)
                {
                    if (listViewBabies.SelectedItems[i].Tag.GetType() == typeof(IncubationTimerEntry))
                    {
                        if (i == 0)
                        {
                            timerRunning = ((IncubationTimerEntry)(listViewBabies.SelectedItems[i].Tag))
                                .timerIsRunning;
                        }

                        ((IncubationTimerEntry)listViewBabies.SelectedItems[i].Tag).startStopTimer(!timerRunning);
                    }
                    else if (listViewBabies.SelectedItems[i].Tag.GetType() == typeof(Creature))
                    {
                        if (i == 0)
                        {
                            timerRunning = !((Creature)(listViewBabies.SelectedItems[i].Tag)).growingPaused;
                        }

                        ((Creature)listViewBabies.SelectedItems[i].Tag).StartStopMatureTimer(!timerRunning);
                    }
                }
            }
        }

        private void cbAddOffsetToAllTimers_CheckedChanged(object sender, EventArgs e)
        {
            cbSubtractOffsetToAllTimers.Text = cbSubtractOffsetToAllTimers.Checked ? "-" : "+";
        }

        private void btAdjustAllTimers_Click(object sender, EventArgs e)
        {
            TimeSpan offset = dhmsInputOffsetAllTimers.Timespan;
            if (cbSubtractOffsetToAllTimers.Checked) offset = -offset;

            DateTime now = DateTime.Now;

            foreach (ListViewItem lvi in listViewBabies.Items)
            {
                if (lvi.Tag is IncubationTimerEntry ite
                    && !ite.expired)
                {
                    ite.incubationEnd += offset;
                }
                else if (lvi.Tag is Creature c
                         && c.growingUntil > now)
                {
                    c.growingUntil += offset;
                }
            }

            AdjustTimers?.Invoke(offset);
            dhmsInputOffsetAllTimers.Timespan = default;
            Utils.BlinkAsync(btAdjustAllTimers, Color.LightGreen, 500, false);
        }

        private void listViewBabies_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnSorter.DoSort((ListView)sender, e.Column);
        }

        private void bSaveTimerEdit_Click(object sender, EventArgs e)
        {
            if (iteEdit != null)
            {
                iteEdit.incubationEnd = dateTimePickerEditTimerFinish.Value;
            }
            else if (creatureMaturationEdit != null)
            {
                creatureMaturationEdit.growingUntil = dateTimePickerEditTimerFinish.Value;
            }
            else return;

            RecreateList();
            onChange?.Invoke();
        }

        internal void SetLocalizations()
        {
            parentStats1.SetLocalizations();
        }
    }
}
