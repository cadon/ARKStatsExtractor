using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class RaisingControl : UserControl
    {
        public delegate void ExtractBabyEventHandler(Creature mother, Creature father);
        public event ExtractBabyEventHandler extractBaby;
        public event Form1.collectionChangedEventHandler onChange;
        public event Form1.setSpeciesIndexEventHandler setSpeciesIndex;
        private int speciesIndex;
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

        public void updateRaisingData()
        {
            updateRaisingData(speciesIndex, true);
        }

        public void updateRaisingData(int speciesIndex, bool forceUpdate = false)
        {
            if (forceUpdate || this.speciesIndex != speciesIndex)
            {
                this.speciesIndex = speciesIndex;
                if (speciesIndex >= 0 && Values.V.species[speciesIndex].taming != null && Values.V.species[speciesIndex].breeding != null)
                {
                    this.SuspendLayout();
                    BreedingData bd = Values.V.species[speciesIndex].breeding;

                    listViewRaisingTimes.Items.Clear();

                    if (Raising.getRaisingTimes(speciesIndex, out string incubationMode, out TimeSpan incubationTime, out babyTime, out maturationTime, out TimeSpan nextMatingMin, out TimeSpan nextMatingMax))
                    {
                        string eggInfo = Raising.eggTemperature(speciesIndex);
                        if (eggInfo.Length > 0)
                            eggInfo = "\n\n" + eggInfo;

                        TimeSpan totalTime = incubationTime;
                        DateTime until = DateTime.Now.Add(totalTime);
                        string[] times = new string[] { incubationMode, incubationTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.shortTimeDate(until) };
                        listViewRaisingTimes.Items.Add(new ListViewItem(times));

                        totalTime += babyTime;
                        until = DateTime.Now.Add(totalTime);
                        times = new string[] { "Baby", babyTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.shortTimeDate(until) };
                        listViewRaisingTimes.Items.Add(new ListViewItem(times));

                        totalTime = incubationTime + maturationTime;
                        until = DateTime.Now.Add(totalTime);
                        times = new string[] { "Maturation", maturationTime.ToString("d':'hh':'mm':'ss"), totalTime.ToString("d':'hh':'mm':'ss"), Utils.shortTimeDate(until) };
                        listViewRaisingTimes.Items.Add(new ListViewItem(times));

                        // food amount needed
                        string foodamount = "";
                        if (Values.V.species[speciesIndex].taming.eats != null &&
                            uiControls.Trough.foodAmount(speciesIndex, Values.V.babyFoodConsumptionSpeedMultiplier, out double babyfood, out double totalfood))
                        {
                            if (Values.V.species[speciesIndex].taming.eats.IndexOf("Raw Meat") >= 0)
                            {
                                foodamount = "\n\nFood for Baby-Phase: ~" + Math.Ceiling(babyfood / 50) + " Raw Meat"
                                    + "\nTotal Food for maturation: ~" + Math.Ceiling(totalfood / 50) + " Raw Meat";
                            }
                            else if (Values.V.species[speciesIndex].taming.eats.IndexOf("Mejoberry") >= 0)
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
            updateMaturationProgress();
        }

        private void updateMaturationProgress()
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

            double foodAmount = 0;
            string foodAmountBabyString = "", foodAmountAdultString = "";
            if (Values.V.species[speciesIndex].taming.eats != null)
            {
                if (Values.V.species[speciesIndex].taming.eats.IndexOf("Raw Meat") >= 0)
                {
                    if (uiControls.Trough.foodAmountFromUntil(speciesIndex, Values.V.babyFoodConsumptionSpeedMultiplier, maturation, 0.1, out foodAmount))
                        foodAmountBabyString = Math.Ceiling(foodAmount / 50) + " Raw Meat";
                    if (uiControls.Trough.foodAmountFromUntil(speciesIndex, Values.V.babyFoodConsumptionSpeedMultiplier, maturation, 1, out foodAmount))
                        foodAmountAdultString = Math.Ceiling(foodAmount / 50) + " Raw Meat";
                }
                else if (Values.V.species[speciesIndex].taming.eats.IndexOf("Mejoberry") >= 0)
                {
                    if (uiControls.Trough.foodAmountFromUntil(speciesIndex, Values.V.babyFoodConsumptionSpeedMultiplier, maturation, 0.1, out foodAmount))
                        foodAmountBabyString = Math.Ceiling(foodAmount / 30) + " Mejoberries";
                    if (uiControls.Trough.foodAmountFromUntil(speciesIndex, Values.V.babyFoodConsumptionSpeedMultiplier, maturation, 1, out foodAmount))
                        foodAmountAdultString = Math.Ceiling(foodAmount / 30) + " Mejoberries";
                }
            }
            labelAmountFoodBaby.Text = foodAmountBabyString;
            labelAmountFoodAdult.Text = foodAmountAdultString;
        }

        public void addIncubationTimer(Creature mother, Creature father, TimeSpan incubationDuration, bool incubationStarted)
        {
            cc.incubationListEntries.Add(new IncubationTimerEntry(mother, father, incubationDuration, incubationStarted));
            onChange?.Invoke();
            recreateList();
        }

        private void removeIncubationTimer(IncubationTimerEntry ite)
        {
            cc.incubationListEntries.Remove(ite);
        }

        public void recreateList()
        {
            if (cc != null)
            {
                updateListView = false;
                listViewBabies.BeginUpdate();
                listViewBabies.Items.Clear();

                ListViewGroup g = listViewBabies.Groups[0];
                // add eggs / pregnancies
                foreach (IncubationTimerEntry t in cc.incubationListEntries)
                {
                    int i = Values.V.speciesNames.IndexOf(t.mother.species);
                    if (i >= 0 && Values.V.species[i].breeding != null)
                    {
                        if (Values.V.species[i].breeding.gestationTimeAdjusted > 0)
                            t.kind = "Gestation";
                        else t.kind = "Egg";
                        string[] cols = new string[] { t.kind,
                                t.mother.species,
                                Utils.timeLeft(t.incubationEnd),
                                Utils.duration((int)(Values.V.species[i].breeding.maturationTimeAdjusted / 10)),
                                Utils.duration((int)Values.V.species[i].breeding.maturationTimeAdjusted),
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
                    if (c.growingUntil > now)
                    {
                        int i = Values.V.speciesNames.IndexOf(c.species);
                        if (i >= 0 && Values.V.species[i].breeding != null)
                        {
                            DateTime babyUntil = c.growingUntil.AddSeconds(-0.9 * Values.V.species[i].breeding.maturationTimeAdjusted);
                            string[] cols;
                            if (babyUntil > now)
                            {
                                g = listViewBabies.Groups[1];
                                cols = new string[] { c.name,
                                        c.species,
                                        "-",
                                        Utils.timeLeft(babyUntil),
                                        Utils.timeLeft(c.growingUntil) };
                            }
                            else
                            {
                                g = listViewBabies.Groups[2];
                                cols = new string[] { c.name,
                                        c.species,
                                        "-",
                                        "-",
                                        Utils.timeLeft(c.growingUntil) };
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
                        if (!t.expired && t.timerIsRunning)
                        {
                            int i = Values.V.speciesNames.IndexOf(t.mother.species);
                            if (i >= 0)
                            {
                                lvi.SubItems[2].Text = Utils.timeLeft(t.incubationEnd);
                                lvi.SubItems[3].Text = Utils.duration((int)(Values.V.species[i].breeding.maturationTimeAdjusted / 10));
                                lvi.SubItems[4].Text = Utils.duration((int)Values.V.species[i].breeding.maturationTimeAdjusted);
                                lvi.SubItems[5].Text = "";
                                double diff = t.incubationEnd.Subtract(alertTime).TotalSeconds;
                                if (diff >= 0 && diff < 1)
                                {
                                    timerControl.playSound("Birth", 1);
                                    t.expired = true;
                                }
                            }
                        }
                        else if (t.expired)
                            lvi.SubItems[5].Text = "Expired";
                        else
                            lvi.SubItems[5].Text = "Paused";
                    }
                    else if ((lvi.Tag.GetType() == typeof(Creature)))
                    {
                        var c = (Creature)lvi.Tag;
                        int i = Values.V.speciesNames.IndexOf(c.species);
                        if (i >= 0 && Values.V.species[i].breeding != null)
                        {
                            DateTime babyUntil = c.growingUntil.AddSeconds(-0.9 * Values.V.species[i].breeding.maturationTimeAdjusted);
                            lvi.SubItems[3].Text = Utils.timeLeft(babyUntil);
                            lvi.SubItems[4].Text = Utils.timeLeft(c.growingUntil);
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
                        int i = Values.V.speciesNames.IndexOf(t.mother.species);
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
            if (listViewBabies.SelectedIndices.Count > 0
                && listViewBabies.SelectedItems[0].Tag.GetType() == typeof(IncubationTimerEntry))
            {
                IncubationTimerEntry ite = (IncubationTimerEntry)listViewBabies.SelectedItems[0].Tag;
                if (MessageBox.Show("Delete this timer?\n" + ite.mother.species + ", ending in " + Utils.timeLeft(ite.incubationEnd)
                    + (listViewBabies.SelectedIndices.Count > 1 ? "\n\nand " + (listViewBabies.SelectedIndices.Count - 1).ToString() + " more selected timers" : "") + "?"
                    , "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    for (int t = listViewBabies.SelectedIndices.Count - 1; t >= 0; t--)
                        removeIncubationTimer((IncubationTimerEntry)listViewBabies.SelectedItems[t].Tag);

                    recreateList();
                    onChange?.Invoke();
                }
            }
        }

        public void deleteAllExpiredIncubationTimers()
        {
            if (MessageBox.Show("Delete all expired incubation timers?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (ListViewItem lvi in listViewBabies.Items)
                {
                    if ((lvi.Tag.GetType() == typeof(IncubationTimerEntry)))
                    {
                        IncubationTimerEntry ite = (IncubationTimerEntry)lvi.Tag;
                        if (ite.timerIsRunning && ite.incubationEnd < DateTime.Now)
                            removeIncubationTimer(ite);
                    }
                }
                recreateList();
                onChange?.Invoke();
            }
        }

        private void removeAllExpiredTimersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteAllExpiredIncubationTimers();
        }

        private void listViewBabies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewBabies.SelectedIndices.Count > 0)
            {
                if (listViewBabies.SelectedItems[0].Tag.GetType() == typeof(Creature))
                {
                    Creature c = (Creature)listViewBabies.SelectedItems[0].Tag;
                    int sI = Values.V.speciesNames.IndexOf(c.species);
                    setSpeciesIndex?.Invoke(sI);
                    if (sI >= 0 && Values.V.species[sI].breeding != null && c.growingUntil > DateTime.Now)
                    {
                        double maturing = Math.Round(1 - c.growingUntil.Subtract(DateTime.Now).TotalSeconds / Values.V.species[sI].breeding.maturationTimeAdjusted, 2);
                        if (maturing > 0 && maturing <= 1)
                        {
                            nudMaturationProgress.Value = (decimal)(100 * maturing);
                        }
                    }
                    parentStats1.setParentValues(c.Mother, c.Father);

                    // edit-box
                    creatureMaturationEdit = c;
                    iteEdit = null;

                    setEditTimer();
                }
                else if (listViewBabies.SelectedItems[0].Tag.GetType() == typeof(IncubationTimerEntry))
                {
                    IncubationTimerEntry ite = (IncubationTimerEntry)listViewBabies.SelectedItems[0].Tag;
                    int sI = Values.V.speciesNames.IndexOf(ite.mother.species);
                    setSpeciesIndex?.Invoke(sI);

                    parentStats1.setParentValues(ite.mother, ite.father);

                    // edit-box
                    creatureMaturationEdit = null;
                    iteEdit = ite;

                    setEditTimer();
                }
            }
            else
            {
                iteEdit = null;
                creatureMaturationEdit = null;
                setEditTimer();
            }
        }

        private void setEditTimer()
        {
            if (iteEdit != null)
            {
                lEditTimerName.Text = "Incubation" + (iteEdit.mother != null ? " (" + iteEdit.mother.species + ")" : "");
                dateTimePickerEditTimerFinish.Value = iteEdit.incubationEnd;
                TimeSpan ts = iteEdit.incubationEnd.Subtract(DateTime.Now);
                dhmsInputTimerEditTimer.Timespan = (ts.TotalSeconds > 0 ? ts : TimeSpan.Zero);

            }
            else if (creatureMaturationEdit != null)
            {
                lEditTimerName.Text = creatureMaturationEdit.name + " (" + creatureMaturationEdit.species + ")";
                dateTimePickerEditTimerFinish.Value = creatureMaturationEdit.growingUntil;
                TimeSpan ts = creatureMaturationEdit.growingUntil.Subtract(DateTime.Now);
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
                List<int> incTimerIndices = new List<int>();
                for (int i = 0; i < listViewBabies.SelectedIndices.Count; i++)
                {
                    if (listViewBabies.SelectedItems[i].Tag.GetType() == typeof(IncubationTimerEntry))
                    {
                        incTimerIndices.Add(i);
                    }
                }
                if (incTimerIndices.Count > 0)
                {
                    bool timerRunning = ((IncubationTimerEntry)(listViewBabies.SelectedItems[incTimerIndices[0]].Tag)).timerIsRunning;
                    for (int i = 0; i < incTimerIndices.Count; i++)
                    {
                        ((IncubationTimerEntry)(listViewBabies.SelectedItems[incTimerIndices[i]].Tag)).startStopTimer(!timerRunning);
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

            recreateList();
            onChange?.Invoke();
        }
    }
}
