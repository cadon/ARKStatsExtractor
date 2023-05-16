using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.raising
{
    public partial class RaisingControl : UserControl
    {
        public event Action<Creature, Creature> ExtractBaby;
        public event Form1.CollectionChangedEventHandler onChange;
        public event Action<TimeSpan> AdjustTimersByOffset;
        public event Action<Species> SetGlobalSpecies;
        public event Action TimerAddedRemoved;
        private Species _selectedSpecies;
        public bool updateListView;
        private TimeSpan _babyTime;
        private TimeSpan _maturationTime;
        private CreatureCollection _cc;
        public TimerControl timerControl;
        private IncubationTimerEntry _iteEdit;
        private Creature _creatureMaturationEdit;
        private string _lastSelectedFood;
        private bool _ignoreChangedFood;
        private readonly Debouncer _debouncer = new Debouncer();

        public RaisingControl()
        {
            InitializeComponent();
            listViewBabies.Groups.Add("incubation", "Incubation/Gestation");
            listViewBabies.Groups.Add("baby", "Babies");
            listViewBabies.Groups.Add("growing", "Juveniles / Adolescent");
            updateListView = false;
            listViewBabies.DoubleBuffered(true); // prevent flickering
            listViewBabies.ListViewItemSorter = new ListViewColumnSorter();
            _lastSelectedFood = Properties.Settings.Default.RaisingFoodLastSelected;
        }

        public void UpdateRaisingData()
        {
            UpdateRaisingData(_selectedSpecies, true);
        }

        /// <summary>
        /// Updates the general raising data for the given species
        /// </summary>
        public void UpdateRaisingData(Species species, bool forceUpdate = false)
        {
            if (!forceUpdate && _selectedSpecies == species) return;

            _selectedSpecies = species;
            CbGrowingFood.DataSource = null;
            listViewRaisingTimes.Items.Clear();

            if (_selectedSpecies?.taming == null || _selectedSpecies.breeding == null)
            {
                // no taming- or breeding-data available
                labelRaisingInfos.Text = "No raising-data available.";
                tabPageMaturationProgress.Enabled = false;
                return;
            }

            SuspendLayout();

            var eats = new List<string>();
            if (_selectedSpecies.taming.eats != null)
                eats.AddRange(_selectedSpecies.taming.eats);
            if (_selectedSpecies.taming.eatsAlsoPostTame != null)
                eats.AddRange(_selectedSpecies.taming.eatsAlsoPostTame);

            _ignoreChangedFood = true;
            CbGrowingFood.DataSource = eats;
            _ignoreChangedFood = false;
            var selectIndex = string.IsNullOrEmpty(_lastSelectedFood) ? 0 : eats.IndexOf(_lastSelectedFood);
            if (selectIndex == -1) selectIndex = 0;
            if (CbGrowingFood.Items.Count > 0)
                CbGrowingFood.SelectedIndex = selectIndex;

            if (Raising.GetRaisingTimes(_selectedSpecies, out TimeSpan matingTime, out string incubationMode,
                out TimeSpan incubationTime, out _babyTime, out _maturationTime, out TimeSpan nextMatingMin,
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

                totalTime += _babyTime;
                until = DateTime.Now.Add(totalTime);
                times = new[]
                {
                    Loc.S("Baby"), _babyTime.ToString("d':'hh':'mm':'ss"),
                    totalTime.ToString("d':'hh':'mm':'ss"), Utils.ShortTimeDate(until)
                };
                listViewRaisingTimes.Items.Add(new ListViewItem(times));

                totalTime = incubationTime + _maturationTime;
                until = DateTime.Now.Add(totalTime);
                times = new[]
                {
                    Loc.S("Maturation"), _maturationTime.ToString("d':'hh':'mm':'ss"),
                    totalTime.ToString("d':'hh':'mm':'ss"), Utils.ShortTimeDate(until)
                };
                listViewRaisingTimes.Items.Add(new ListViewItem(times));

                var raisingInfo = new StringBuilder();
                if (nextMatingMin != TimeSpan.Zero)
                    raisingInfo.AppendLine(
                        $"{Loc.S("TimeBetweenMating")}: {nextMatingMin:d':'hh':'mm':'ss} to {nextMatingMax:d':'hh':'mm':'ss}");

                string eggInfo = Raising.EggTemperature(_selectedSpecies);
                if (!string.IsNullOrEmpty(eggInfo))
                    raisingInfo.AppendLine(eggInfo);

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

        private void FoodAmountNeeded()
        {
            string foodAmount = null;
            if (_selectedSpecies.taming.eats?.Any() == true
                && uiControls.Trough.FoodAmountFromUntil(_selectedSpecies,
                    Values.V.currentServerMultipliers.BabyFoodConsumptionSpeedMultiplier,
                    Values.V.currentServerMultipliers.DinoCharacterFoodDrainMultiplier,
                    Values.V.currentServerMultipliers.TamedDinoCharacterFoodDrainMultiplier,
                    0, 1, out double totalFood))
            {
                var babyPhaseFoodValid = uiControls.Trough.FoodAmountFromUntil(_selectedSpecies,
                    Values.V.currentServerMultipliers.BabyFoodConsumptionSpeedMultiplier,
                    Values.V.currentServerMultipliers.DinoCharacterFoodDrainMultiplier,
                    Values.V.currentServerMultipliers.TamedDinoCharacterFoodDrainMultiplier,
                    0, .1, out double babyPhaseFood);

                if (!string.IsNullOrEmpty(_lastSelectedFood))
                    foodAmount = FoodAmountString(_lastSelectedFood);
                if (string.IsNullOrEmpty(foodAmount))
                    foodAmount = FoodAmountString("Raw Meat");
                if (string.IsNullOrEmpty(foodAmount))
                    foodAmount = FoodAmountString("Mejoberries");
                if (string.IsNullOrEmpty(foodAmount))
                    foodAmount = FoodAmountString(_selectedSpecies.taming.eats[0]);

                string FoodAmountString(string foodName)
                {
                    if (Array.IndexOf(_selectedSpecies.taming.eats, foodName) == -1
                        && (_selectedSpecies.taming.eatsAlsoPostTame == null
                            || Array.IndexOf(_selectedSpecies.taming.eatsAlsoPostTame, foodName) == -1)) return null;

                    var food = Values.V.GetTamingFood(_selectedSpecies, foodName);

                    if (food == null) return null;

                    var foodValue = food.foodValue;
                    if (foodValue == 0) return null;

                    return (babyPhaseFoodValid ? $"\n\nFood for Baby-Phase: ~{Math.Ceiling(babyPhaseFood / foodValue)} {foodName}" : string.Empty)
                           + $"\nTotal Food for maturation: ~{Math.Ceiling(totalFood / foodValue)} {foodName}";
                }

                foodAmount += "\n- Loss by spoiling is not considered!";
            }

            LbFoodInfoGeneral.Text = foodAmount;
        }

        public CreatureCollection CreatureCollection
        {
            set
            {
                if (value != null)
                {
                    _cc = value;
                    parentStats1.MaxChartLevel = _cc.maxChartLevel;
                }
            }
        }

        private void nudMaturationProgress_ValueChanged(object sender, EventArgs e)
        {
            _debouncer.Debounce(200, UpdateMaturationProgress, Dispatcher.CurrentDispatcher);
        }

        private void UpdateMaturationProgress()
        {
            double maturation = (double)nudMaturationProgress.Value / 100;
            double maturationSeconds = _maturationTime.TotalSeconds * maturation;
            if (maturationSeconds < _babyTime.TotalSeconds)
            {
                labelTimeLeftBaby.Text =
                    Utils.DurationUntil(_babyTime.Subtract(new TimeSpan(0, 0, (int)maturationSeconds)));
                labelTimeLeftBaby.ForeColor = SystemColors.ControlText;
            }
            else
            {
                labelTimeLeftBaby.Text = Loc.S("notABabyAnymore");
                labelTimeLeftBaby.ForeColor = SystemColors.GrayText;
            }

            labelAmountFoodBaby.Text = string.Empty;
            labelAmountFoodAdult.Text = string.Empty;

            if (maturation < 1)
            {
                labelTimeLeftGrowing.Text =
                    Utils.DurationUntil(_maturationTime.Subtract(new TimeSpan(0, 0, (int)(maturationSeconds))));
                labelTimeLeftGrowing.ForeColor = SystemColors.ControlText;
            }
            else
            {
                labelTimeLeftGrowing.Text = Loc.S("mature");
                labelTimeLeftGrowing.ForeColor = SystemColors.GrayText;
                return;
            }

            if (_lastSelectedFood == null) return;


            var food = Values.V.GetTamingFood(_selectedSpecies, _lastSelectedFood);
            if (food == null) return;

            var foodValue = food.foodValue;
            if (foodValue == 0) return;

            if (uiControls.Trough.FoodAmountFromUntil(_selectedSpecies,
                Values.V.currentServerMultipliers.BabyFoodConsumptionSpeedMultiplier,
                Values.V.currentServerMultipliers.DinoCharacterFoodDrainMultiplier,
                Values.V.currentServerMultipliers.TamedDinoCharacterFoodDrainMultiplier,
                maturation, 0.1,
                out var foodAmount))
                labelAmountFoodBaby.Text = $"{Math.Ceiling(foodAmount / foodValue)} {_lastSelectedFood} ({foodAmount:0.#} food units)";

            if (uiControls.Trough.FoodAmountFromUntil(_selectedSpecies,
                Values.V.currentServerMultipliers.BabyFoodConsumptionSpeedMultiplier,
                Values.V.currentServerMultipliers.DinoCharacterFoodDrainMultiplier,
                Values.V.currentServerMultipliers.TamedDinoCharacterFoodDrainMultiplier,
                maturation, 1,
                out foodAmount))
                labelAmountFoodAdult.Text = $"{Math.Ceiling(foodAmount / foodValue)} {_lastSelectedFood} ({foodAmount:0.#} food units)";
        }

        public void AddIncubationTimer(Creature mother, Creature father, TimeSpan incubationDuration,
            bool incubationStarted)
        {
            _cc.incubationListEntries.Add(
                new IncubationTimerEntry(mother, father, incubationDuration, incubationStarted));
            onChange?.Invoke();
            TimerAddedRemoved?.Invoke();
            RecreateList();
        }

        private void RemoveIncubationTimer(IncubationTimerEntry ite)
        {
            _cc.incubationListEntries.Remove(ite);
        }

        /// <summary>
        /// Update timer list, checks every creature if it needs to be added
        /// </summary>
        public void RecreateList()
        {
            if (_cc == null)
                return;

            updateListView = false;
            listViewBabies.BeginUpdate();
            listViewBabies.Items.Clear();
            var items = new List<ListViewItem>();

            // if both parents of an incubation entry were deleted, remove that entry as well.
            _cc.incubationListEntries =
                _cc.incubationListEntries.Where(t => t.Mother != null || t.Father != null).ToList();

            ListViewGroup g = listViewBabies.Groups[0];
            // add eggs / pregnancies
            foreach (IncubationTimerEntry t in _cc.incubationListEntries)
            {
                Species species = t.Mother?.Species ?? t.Father?.Species;
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
                    items.Add(lvi);
                }
            }
            if (items.Any())
                listViewBabies.Items.AddRange(items.ToArray());

            // add babies / growing
            items.Clear();
            DateTime now = DateTime.Now;
            foreach (Creature c in _cc.creatures)
            {
                if (c.growingUntil.HasValue
                    && (c.growingUntil > now
                        || (c.growingPaused && c.growingLeft.TotalHours > 0)))
                {
                    Species species = c.Species;
                    if (species?.breeding == null) continue;

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
                    items.Add(lvi);
                }
            }
            if (items.Any())
                listViewBabies.Items.AddRange(items.ToArray());

            listViewBabies.EndUpdate();
            updateListView = true;
            TimerAddedRemoved?.Invoke();
        }

        public bool TimerIsNeeded => listViewBabies.Items.Count != 0;

        public void Tick()
        {
            DateTime now = DateTime.Now;
            DateTime alertTime = now.AddMinutes(1);
            if (updateListView)
            {
                listViewBabies.BeginUpdate();
                foreach (ListViewItem lvi in listViewBabies.Items)
                {
                    if (lvi.Tag is IncubationTimerEntry ite)
                    {
                        Species species = ite.Mother?.Species ?? ite.Father?.Species;
                        if (species?.breeding != null)
                        {
                            lvi.SubItems[3].Text = Utils.Duration((int)(species.breeding.maturationTimeAdjusted / 10));
                            lvi.SubItems[4].Text = Utils.Duration((int)species.breeding.maturationTimeAdjusted);
                        }

                        if (ite.expired)
                        {
                            lvi.SubItems[2].Text = Utils.TimeLeft(ite.incubationEnd);
                            lvi.SubItems[5].Text = Loc.S("expired");
                        }
                        else if (!ite.timerIsRunning)
                        {
                            lvi.SubItems[2].Text = Utils.Duration(ite.incubationDuration);
                            lvi.SubItems[5].Text = Loc.S("paused");
                        }
                        else
                        {
                            lvi.SubItems[2].Text = Utils.TimeLeft(ite.incubationEnd);
                            lvi.SubItems[5].Text = string.Empty;
                            double diff = ite.incubationEnd.Subtract(alertTime).TotalSeconds;
                            if (diff >= 0 && diff < 1)
                            {
                                timerControl.PlaySound("Birth", 1);
                            }
                            else if (diff < 0)
                            {
                                ite.expired = true;
                            }
                        }
                    }
                    else if (lvi.Tag is Creature c)
                    {
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
                    if (lvi.Tag is IncubationTimerEntry ite)
                    {
                        double diff = ite.incubationEnd.Subtract(alertTime).TotalSeconds;
                        if (diff >= 0 && diff < 1)
                            timerControl.PlaySound("Birth", 1);
                    }
                }
            }
        }

        private void extractValuesOfHatchedbornBabyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewBabies.SelectedIndices.Count > 0 &&
                listViewBabies.SelectedItems[0].Tag is IncubationTimerEntry ite)
            {
                ExtractBaby?.Invoke(ite.Mother, ite.Father);
            }
        }

        private void deleteTimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewBabies.SelectedIndices.Count > 0)
            {
                if (listViewBabies.SelectedItems[0].Tag is IncubationTimerEntry ite)
                {
                    if (MessageBox.Show("Delete this timer?\n" + (ite.Mother?.Species?.name ?? "unknown") +
                                        ", ending in " + Utils.TimeLeft(ite.incubationEnd)
                                        + (listViewBabies.SelectedIndices.Count > 1
                                            ? "\n\nand " + (listViewBabies.SelectedIndices.Count - 1).ToString() +
                                              " more selected timers"
                                            : string.Empty) + "?",
                        "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        for (int t = listViewBabies.SelectedIndices.Count - 1; t >= 0; t--)
                            RemoveIncubationTimer((IncubationTimerEntry)listViewBabies.SelectedItems[t].Tag);

                        RecreateList();
                        onChange?.Invoke();
                        TimerAddedRemoved?.Invoke();
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
                    if (lvi.Tag is IncubationTimerEntry ite && ite.incubationEnd < now)
                    {
                        RemoveIncubationTimer(ite);
                    }
                }

                RecreateList();
                onChange?.Invoke();
                TimerAddedRemoved?.Invoke();
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
                    double maturation = c.Maturation;
                    if (maturation >= 1)
                    {
                        c.growingUntil = null;
                        maturation = 1;
                    }
                    if (maturation > 0)
                    {
                        nudMaturationProgress.Value = (decimal)maturation * 100;
                    }

                    parentStats1.SetParentValues(c.Mother, c.Father);

                    // edit-box
                    _creatureMaturationEdit = c;
                    _iteEdit = null;

                    SetEditTimer();
                }
                else if (listViewBabies.SelectedItems[0].Tag is IncubationTimerEntry ite)
                {
                    var species = ite.Mother?.Species ?? ite.Father?.Species;
                    if (species != null)
                        SetGlobalSpecies?.Invoke(species);

                    parentStats1.SetParentValues(ite.Mother, ite.Father);

                    // edit-box
                    _creatureMaturationEdit = null;
                    _iteEdit = ite;

                    SetEditTimer();
                }
            }
            else
            {
                _iteEdit = null;
                _creatureMaturationEdit = null;
                SetEditTimer();
            }
        }

        private void SetEditTimer()
        {
            if (_iteEdit != null)
            {
                lEditTimerName.Text =
                    $"{Loc.S("incubation")}{(_iteEdit.Mother != null ? " (" + (_iteEdit.Mother.Species?.name ?? Loc.S("Unknown")) + ")" : string.Empty)}";
                dateTimePickerEditTimerFinish.Value = _iteEdit.incubationEnd;
                TimeSpan ts = _iteEdit.incubationEnd.Subtract(DateTime.Now);
                dhmsInputTimerEditTimer.Timespan = (ts.TotalSeconds > 0 ? ts : TimeSpan.Zero);

            }
            else if (_creatureMaturationEdit?.growingUntil != null)
            {
                lEditTimerName.Text =
                    $"{_creatureMaturationEdit.name} ({(_creatureMaturationEdit.Species?.name ?? Loc.S("Unknown"))})";
                TimeSpan ts;
                if (_creatureMaturationEdit.growingPaused)
                {
                    dateTimePickerEditTimerFinish.Value = DateTime.Now.Add(_creatureMaturationEdit.growingLeft);
                    ts = _creatureMaturationEdit.growingLeft;
                }
                else
                {
                    dateTimePickerEditTimerFinish.Value = _creatureMaturationEdit.growingUntil.Value;
                    ts = _creatureMaturationEdit.growingUntil.Value.Subtract(DateTime.Now);
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
            if (listViewBabies.SelectedIndices.Count == 0) return;

            bool startTimer = true;
            for (int i = 0; i < listViewBabies.SelectedIndices.Count; i++)
            {
                if (listViewBabies.SelectedItems[i].Tag is IncubationTimerEntry ite)
                {
                    if (i == 0)
                    {
                        startTimer = !ite.timerIsRunning;
                    }

                    ite.StartStopTimer(startTimer);
                }
                else if (listViewBabies.SelectedItems[i].Tag is Creature c)
                {
                    if (i == 0)
                    {
                        startTimer = c.growingPaused;
                    }

                    c.StartStopMatureTimer(startTimer);
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

            AdjustTimersByOffset?.Invoke(offset);
            dhmsInputOffsetAllTimers.Timespan = default;
            Utils.BlinkAsync(btAdjustAllTimers, Color.LightGreen, 500, false);
        }

        private void listViewBabies_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnSorter.DoSort((ListView)sender, e.Column);
        }

        private void bSaveTimerEdit_Click(object sender, EventArgs e)
        {
            if (_iteEdit != null)
            {
                _iteEdit.incubationEnd = dateTimePickerEditTimerFinish.Value;
            }
            else if (_creatureMaturationEdit != null)
            {
                _creatureMaturationEdit.growingUntil = dateTimePickerEditTimerFinish.Value;
            }
            else return;

            RecreateList();
            onChange?.Invoke();
            TimerAddedRemoved?.Invoke();
        }

        internal void SetLocalizations()
        {
            parentStats1.SetLocalizations();
        }

        private void CbGrowingFood_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ignoreChangedFood) return;
            var foodName = (sender as ComboBox)?.SelectedItem as string;
            if (!string.IsNullOrEmpty(foodName))
                _lastSelectedFood = foodName;

            FoodAmountNeeded();
            UpdateMaturationProgress();
        }

        internal string LastSelectedFood => _lastSelectedFood;

        private void listViewBabies_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var isChecked = e.Item.Checked;
            if (e.Item.Tag is Creature creature)
            {
                if (isChecked) ARKOverlay.AddTimer(creature);
                else ARKOverlay.RemoveTimer(creature);
                return;
            }
            if (e.Item.Tag is IncubationTimerEntry incubationTimerEntry)
            {
                if (isChecked) ARKOverlay.AddTimer(incubationTimerEntry);
                else ARKOverlay.RemoveTimer(incubationTimerEntry);
            }
        }
    }
}
