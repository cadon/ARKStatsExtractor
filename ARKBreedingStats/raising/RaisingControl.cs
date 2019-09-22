using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats.raising
{
    public partial class RaisingControl : UserControl
    {
        public delegate void ExtractBabyEventHandler(Creature mother, Creature father);
        public event ExtractBabyEventHandler extractBaby;
        public event Form1.collectionChangedEventHandler onChange;
        public event Form1.SetSpeciesEventHandler SetGlobalSpecies;
        private Species selectedSpecies;
        public bool updateListView;
        private TimeSpan babyTime, maturationTime;
        private CreatureCollection cc;
        public TimerControl timerControl;
        private IncubationTimerEntry iteEdit;
        private Creature creatureMaturationEdit;

        public RaisingControl()
        {
            InitializeComponent();
            listViewBabies.Groups.Add("incubation", "Incubation/Gestation");
            listViewBabies.Groups.Add("baby", "Babies");
            listViewBabies.Groups.Add("growing", "Juveniles / Adolescent");
            updateListView = false;
            listViewBabies.DoubleBuffered(true); // prevent flickering
        }

        public void UpdateRaisingData()
        {
            UpdateRaisingData(selectedSpecies, true);
        }

        /// <summary>
        /// Updates the general raising data for the given species
        /// </summary>
        /// <param name="speciesIndex"></param>
        /// <param name="forceUpdate"></param>
        public void UpdateRaisingData(Species species, bool forceUpdate = false)
        {
            if (forceUpdate || this.selectedSpecies != species)
            {
                selectedSpecies = species;
                if (selectedSpecies != null && selectedSpecies.taming != null && selectedSpecies.breeding != null)
                {
                    this.SuspendLayout();

                    listViewRaisingTimes.Items.Clear();

                    if (Raising.getRaisingTimes(selectedSpecies, out string incubationMode, out TimeSpan incubationTime, out babyTime, out maturationTime, out TimeSpan nextMatingMin, out TimeSpan nextMatingMax))
                    {
                        string eggInfo = Raising.eggTemperature(selectedSpecies);
                        if (eggInfo.Length > 0)
                            eggInfo = "\n\n" + eggInfo;

                        TimeSpan totalTime = incubationTime;
                        DateTime until = DateTime.Now.Add(totalTime);
                        string[] times = { incubationMode, incubationTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.shortTimeDate(until) };
                        listViewRaisingTimes.Items.Add(new ListViewItem(times));

                        totalTime += babyTime;
                        until = DateTime.Now.Add(totalTime);
                        times = new[] { "Baby", babyTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.shortTimeDate(until) };
                        listViewRaisingTimes.Items.Add(new ListViewItem(times));

                        totalTime = incubationTime + maturationTime;
                        until = DateTime.Now.Add(totalTime);
                        times = new[] { "Maturation", maturationTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.shortTimeDate(until) };
                        listViewRaisingTimes.Items.Add(new ListViewItem(times));

                        // food amount needed
                        string foodamount = "";
                        if (selectedSpecies.taming.eats != null &&
                            uiControls.Trough.foodAmount(selectedSpecies, Values.V.currentServerMultipliers.BabyFoodConsumptionSpeedMultiplier, out double babyfood, out double totalfood))
                        {
                            if (selectedSpecies.taming.eats.IndexOf("Raw Meat") >= 0)
                            {
                                foodamount = "\n\nFood for Baby-Phase: ~" + Math.Ceiling(babyfood / 50) + " Raw Meat"
                                    + "\nTotal Food for maturation: ~" + Math.Ceiling(totalfood / 50) + " Raw Meat";
                            }
                            else if (selectedSpecies.taming.eats.IndexOf("Mejoberry") >= 0)
                            {
                                foodamount = "\n\nFood for Baby-Phase: ~" + Math.Ceiling(babyfood / 30) + " Mejoberries"
                                    + "\nTotal Food for maturation: ~" + Math.Ceiling(totalfood / 30) + " Mejoberries";
                            }
                            foodamount += "\n - Loss by spoiling is only a rough estimate and may vary.";
                        }


                        labelRaisingInfos.Text = "Time between mating: " + nextMatingMin.ToString("d':'hh':'mm':'ss") + " to " + nextMatingMax.ToString("d':'hh':'mm':'ss")
                            + eggInfo
                            + foodamount;

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
            double maturation = Math.Round((double)nudMaturationProgress.Value / 100, 3);
            double maturationSeconds = maturationTime.TotalSeconds * maturation;
            if (maturationSeconds < babyTime.TotalSeconds)
            {
                labelTimeLeftBaby.Text = Utils.durationUntil(babyTime.Subtract(new TimeSpan(0, 0, (int)(maturationSeconds))));
                labelTimeLeftBaby.ForeColor = SystemColors.ControlText;
            }
            else
            {
                labelTimeLeftBaby.Text = "not a baby anymore";
                labelTimeLeftBaby.ForeColor = SystemColors.GrayText;
            }
            if (maturation < 1)
            {
                labelTimeLeftGrowing.Text = Utils.durationUntil(maturationTime.Subtract(new TimeSpan(0, 0, (int)(maturationSeconds))));
                labelTimeLeftGrowing.ForeColor = SystemColors.ControlText;
            }
            else
            {
                labelTimeLeftGrowing.Text = "mature";
                labelTimeLeftGrowing.ForeColor = SystemColors.GrayText;
            }

            string foodAmountBabyString = "", foodAmountAdultString = "";
            if (selectedSpecies.taming.eats != null)
            {
                double foodAmount;
                if (selectedSpecies.taming.eats.IndexOf("Raw Meat") >= 0)
                {
                    if (uiControls.Trough.foodAmountFromUntil(selectedSpecies, Values.V.currentServerMultipliers.BabyFoodConsumptionSpeedMultiplier, maturation, 0.1, out foodAmount))
                        foodAmountBabyString = Math.Ceiling(foodAmount / 50) + " Raw Meat";
                    if (uiControls.Trough.foodAmountFromUntil(selectedSpecies, Values.V.currentServerMultipliers.BabyFoodConsumptionSpeedMultiplier, maturation, 1, out foodAmount))
                        foodAmountAdultString = Math.Ceiling(foodAmount / 50) + " Raw Meat";
                }
                else if (selectedSpecies.taming.eats.IndexOf("Mejoberry") >= 0)
                {
                    if (uiControls.Trough.foodAmountFromUntil(selectedSpecies, Values.V.currentServerMultipliers.BabyFoodConsumptionSpeedMultiplier, maturation, 0.1, out foodAmount))
                        foodAmountBabyString = Math.Ceiling(foodAmount / 30) + " Mejoberries";
                    if (uiControls.Trough.foodAmountFromUntil(selectedSpecies, Values.V.currentServerMultipliers.BabyFoodConsumptionSpeedMultiplier, maturation, 1, out foodAmount))
                        foodAmountAdultString = Math.Ceiling(foodAmount / 30) + " Mejoberries";
                }
            }
            labelAmountFoodBaby.Text = foodAmountBabyString;
            labelAmountFoodAdult.Text = foodAmountAdultString;
        }

        public void AddIncubationTimer(Creature mother, Creature father, TimeSpan incubationDuration, bool incubationStarted)
        {
            cc.incubationListEntries.Add(new IncubationTimerEntry(mother, father, incubationDuration, incubationStarted));
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
            cc.incubationListEntries = cc.incubationListEntries.Where(t => t.mother != null || t.father != null).ToList();

            ListViewGroup g = listViewBabies.Groups[0];
            // add eggs / pregnancies
            foreach (IncubationTimerEntry t in cc.incubationListEntries)
            {
                Species species = t.mother?.Species ?? t.father?.Species;
                if (species?.breeding != null)
                {
                    t.kind = species.breeding.gestationTimeAdjusted > 0 ? "Gestation" : "Egg";
                    string[] cols = { t.kind,
                                species.name,
                                "",
                                "",
                                "",
                                "" };
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
                        DateTime babyUntil = c.growingPaused ? now.Add(c.growingLeft).AddSeconds(-0.9 * species.breeding.maturationTimeAdjusted)
                            : c.growingUntil.Value.AddSeconds(-0.9 * species.breeding.maturationTimeAdjusted);
                        string[] cols;
                        if (babyUntil > now)
                        {
                            g = listViewBabies.Groups[1];
                            cols = new[] { c.name,
                                        c.Species.name,
                                        "-",
                                        "",
                                        "",
                                        "" };
                        }
                        else
                        {
                            g = listViewBabies.Groups[2];
                            cols = new[] { c.name,
                                        c.Species.name,
                                        "-",
                                        "-",
                                        "",
                                        "" };
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

                        Species species = t.mother.Species;
                        if (species?.breeding != null)
                        {
                            lvi.SubItems[3].Text = Utils.duration((int)(species.breeding.maturationTimeAdjusted / 10));
                            lvi.SubItems[4].Text = Utils.duration((int)species.breeding.maturationTimeAdjusted);
                        }

                        if (t.expired)
                        {
                            lvi.SubItems[2].Text = Utils.timeLeft(t.incubationEnd);
                            lvi.SubItems[5].Text = "Expired";
                        }
                        else if (!t.timerIsRunning)
                        {
                            lvi.SubItems[2].Text = Utils.duration(t.incubationDuration);
                            lvi.SubItems[5].Text = "Paused";
                        }
                        else
                        {
                            lvi.SubItems[2].Text = Utils.timeLeft(t.incubationEnd);
                            lvi.SubItems[5].Text = "";
                            double diff = t.incubationEnd.Subtract(alertTime).TotalSeconds;
                            if (diff >= 0 && diff < 1)
                            {
                                timerControl.playSound("Birth", 1);
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
                                DateTime babyUntil = now.Add(c.growingLeft).AddSeconds(-0.9 * species.breeding.maturationTimeAdjusted);
                                lvi.SubItems[3].Text = Utils.timeLeft(babyUntil);
                                lvi.SubItems[4].Text = Utils.duration(c.growingLeft);
                                lvi.SubItems[5].Text = "Paused";
                            }
                            else if (c.growingUntil.HasValue)
                            {
                                DateTime babyUntil = c.growingUntil.Value.AddSeconds(-0.9 * species.breeding.maturationTimeAdjusted);
                                lvi.SubItems[3].Text = Utils.timeLeft(babyUntil);
                                lvi.SubItems[4].Text = Utils.timeLeft(c.growingUntil.Value);
                                lvi.SubItems[5].Text = "";
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
                            timerControl.playSound("Birth", 1);
                    }
                }
            }
        }

        private void extractValuesOfHatchedbornBabyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewBabies.SelectedIndices.Count > 0 && listViewBabies.SelectedItems[0].Tag.GetType() == typeof(IncubationTimerEntry))
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
                    if (MessageBox.Show("Delete this timer?\n" + (ite.mother?.Species?.name ?? "unknown") + ", ending in " + Utils.timeLeft(ite.incubationEnd)
                        + (listViewBabies.SelectedIndices.Count > 1 ? "\n\nand " + (listViewBabies.SelectedIndices.Count - 1).ToString() + " more selected timers" : "") + "?"
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
                    MessageBox.Show("The selected timer is an entry for a growing creature and cannot be deleted directly.\nTo remove it, delete the according creature or set the creature to mature",
                        "Timer cannot be deleted",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        public void DeleteAllExpiredIncubationTimers()
        {
            if (MessageBox.Show("Delete all expired incubation timers?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
                if (listViewBabies.SelectedItems[0].Tag.GetType() == typeof(Creature))
                {
                    Creature c = (Creature)listViewBabies.SelectedItems[0].Tag;
                    Species species = c.Species;
                    SetGlobalSpecies?.Invoke(species);
                    if (species?.breeding != null && c.growingUntil.HasValue && c.growingUntil.Value > DateTime.Now)
                    {
                        double maturing = Math.Round(1 - c.growingUntil.Value.Subtract(DateTime.Now).TotalSeconds / species.breeding.maturationTimeAdjusted, 2);
                        if (maturing > 0 && maturing <= 1)
                        {
                            nudMaturationProgress.Value = (decimal)(100 * maturing);
                        }
                    }
                    parentStats1.setParentValues(c.Mother, c.Father);

                    // edit-box
                    creatureMaturationEdit = c;
                    iteEdit = null;

                    SetEditTimer();
                }
                else if (listViewBabies.SelectedItems[0].Tag.GetType() == typeof(IncubationTimerEntry))
                {
                    IncubationTimerEntry ite = (IncubationTimerEntry)listViewBabies.SelectedItems[0].Tag;
                    Species species = ite.mother.Species;
                    SetGlobalSpecies?.Invoke(species);

                    parentStats1.setParentValues(ite.mother, ite.father);

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
                lEditTimerName.Text = "Incubation" + (iteEdit.mother != null ? " (" + (iteEdit.mother.Species?.name ?? "unknown") + ")" : "");
                dateTimePickerEditTimerFinish.Value = iteEdit.incubationEnd;
                TimeSpan ts = iteEdit.incubationEnd.Subtract(DateTime.Now);
                dhmsInputTimerEditTimer.Timespan = (ts.TotalSeconds > 0 ? ts : TimeSpan.Zero);

            }
            else if (creatureMaturationEdit != null && creatureMaturationEdit.growingUntil.HasValue)
            {
                lEditTimerName.Text = creatureMaturationEdit.name + " (" + (creatureMaturationEdit.Species?.name ?? "unknown") + ")";
                dateTimePickerEditTimerFinish.Value = creatureMaturationEdit.growingUntil.Value;
                TimeSpan ts = creatureMaturationEdit.growingUntil.Value.Subtract(DateTime.Now);
                dhmsInputTimerEditTimer.Timespan = (ts.TotalSeconds > 0 ? ts : TimeSpan.Zero);
            }
            else
            {
                lEditTimerName.Text = "no timer selected";
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
                            timerRunning = ((IncubationTimerEntry)(listViewBabies.SelectedItems[i].Tag)).timerIsRunning;
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
    }
}
