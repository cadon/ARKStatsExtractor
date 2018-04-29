using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace ARKBreedingStats
{
    public partial class TimerControl : UserControl
    {
        public delegate void CreateTimerEventHandler(string name, DateTime time, Creature creature, string group);
        public bool updateTimer;
        private List<TimerListEntry> timerListEntries;
        public event Form1.collectionChangedEventHandler onTimerChange;
        private List<Creature> creatures;
        public SoundPlayer[] sounds;
        private List<int> timerAlerts;


        public TimerControl()
        {
            InitializeComponent();
            sounds = new SoundPlayer[4];
            timerAlerts = new List<int>();
            // prevent flickering
            listViewTimer.DoubleBuffered(true);

            // add ButtonAddTimers
            var times = new Dictionary<string, TimeSpan>() {
                {"+1 m", new TimeSpan(0,1,0)},
                {"+5 m", new TimeSpan(0,5,0)},
                {"+20 m", new TimeSpan(0,20,0)},
                {"+1 h", new TimeSpan(1,0,0)},
                {"+5 h", new TimeSpan(5,0,0)},
                {"+1 d", new TimeSpan(24,0,0)}
            };

            int i = 0;
            foreach (KeyValuePair<string, TimeSpan> ts in times)
            {
                var bta = new uiControls.ButtonAddTime();
                bta.timeSpan = ts.Value;
                bta.Text = "Hi";
                bta.Text = ts.Key;
                bta.addTimer += buttonAddTime_addTimer;
                bta.Size = new Size(54, 23);
                bta.Location = new Point(6 + (i % 3) * 60, 48 + (i / 3) * 29);
                groupBox1.Controls.Add(bta);
                i++;
            }
        }

        public void addTimer(string name, DateTime finishTime, Creature c, string group = "Custom")
        {
            TimerListEntry tle = new TimerListEntry();
            tle.name = name;
            tle.group = group;
            tle.time = finishTime;
            tle.creature = c;
            tle.lvi = createLvi(name, finishTime, tle);
            int i = 0;
            while (i < listViewTimer.Items.Count && ((TimerListEntry)listViewTimer.Items[i].Tag).time < finishTime) { i++; }
            listViewTimer.Items.Insert(i, tle.lvi);
            timerListEntries.Add(tle);
            onTimerChange?.Invoke();
        }

        public void removeTimer(TimerListEntry timerEntry, bool invokeChange = true)
        {
            timerEntry.lvi.Remove();
            timerListEntries.Remove(timerEntry);
            if (invokeChange)
                onTimerChange?.Invoke();
        }

        private ListViewItem createLvi(string name, DateTime finishTime, TimerListEntry tle)
        {
            // check if group of timers exists
            ListViewGroup g = null;
            foreach (ListViewGroup lvg in listViewTimer.Groups)
            {
                if (lvg.Header == tle.group)
                {
                    g = lvg;
                    break;
                }
            }
            if (g == null)
            {
                g = new ListViewGroup(tle.group);
                listViewTimer.Groups.Add(g);
            }
            ListViewItem lvi = new ListViewItem(new string[] { name, finishTime.ToString(), "" }, g);
            lvi.Tag = tle;
            return lvi;
        }

        public void Tick()
        {
            if (timerListEntries != null && timerListEntries.Count > 0)
            {
                listViewTimer.BeginUpdate();
                DateTime now = DateTime.Now;
                TimeSpan diff;
                foreach (TimerListEntry t in timerListEntries)
                {
                    if (t.lvi != null)
                    {
                        diff = t.time.Subtract(now);
                        if (updateTimer)
                            t.lvi.SubItems[2].Text = (diff.TotalSeconds > 0 ? diff.ToString("d':'hh':'mm':'ss") : "Finished");
                        if (diff.TotalSeconds >= 0)
                        {
                            if (diff.TotalSeconds < 60 && diff.TotalSeconds > 10)
                                t.lvi.BackColor = Color.Gold;
                            else if (diff.TotalSeconds < 11)
                                t.lvi.BackColor = Color.LightSalmon;

                            if (diff.TotalSeconds < timerAlerts.First() + 1)
                            {
                                for (int i = 0; i < timerAlerts.Count; i++)
                                {
                                    if (diff.TotalSeconds < timerAlerts[i] + 0.8 && diff.TotalSeconds > timerAlerts[i] - 0.8)
                                    {
                                        playSound(t.group, i);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                listViewTimer.EndUpdate();
            }
        }

        public void playSound(string group, int alert)
        {
            // todo, different sound depending on alert-level? or pre-/suffix?
            switch (group)
            {
                case "Starving": playSoundFile(sounds[0]); break;
                case "Wakeup": playSoundFile(sounds[1]); break;
                case "Birth": playSoundFile(sounds[2]); break;
                case "Custom": playSoundFile(sounds[3]); break;
                default: SystemSounds.Hand.Play(); break;
            }
        }

        private void playSoundFile(SoundPlayer sound)
        {
            if (sound == null) SystemSounds.Hand.Play();
            else sound.Play();
        }

        public List<int> TimerAlerts
        {
            set
            {
                if (value != null)
                {
                    timerAlerts = value;
                    for (int i = 0; i < timerAlerts.Count; i++)
                    {
                        if (timerAlerts[i] < 0)
                            timerAlerts.RemoveAt(i--);
                    }
                    timerAlerts.Sort((t1, t2) => -t1.CompareTo(t2));

                    if (timerAlerts.Count == 0)
                        timerAlerts.Add(0);
                }
            }
        }

        public string TimerAlertsCSV
        {
            set
            {
                if (value.Length > 0)
                {
                    List<int> list = new List<int>();
                    var csv = value.Split(',');
                    for (int i = 0; i < csv.Length; i++)
                    {
                        int o = -1;
                        if (Int32.TryParse(csv[i].Trim(), out o))
                            list.Add(o);
                    }
                    if (list.Count > 0)
                        TimerAlerts = list;
                }
            }
            get
            {
                return string.Join(",", timerAlerts);
            }
        }

        public CreatureCollection CreatureCollection
        {
            set
            {
                timerListEntries = value.timerListEntries;
                creatures = value.creatures;

                listViewTimer.Items.Clear();

                foreach (TimerListEntry tle in timerListEntries)
                {
                    tle.lvi = createLvi(tle.name, tle.time, tle);
                    int i = 0;
                    while (i < listViewTimer.Items.Count && ((TimerListEntry)listViewTimer.Items[i].Tag).time < tle.time) { i++; }
                    listViewTimer.Items.Insert(i, tle.lvi);

                    if (tle.creatureGuid != Guid.Empty)
                    {
                        foreach (Creature p in creatures)
                        {
                            if (tle.creatureGuid == p.guid)
                            {
                                tle.creature = p;
                                break;
                            }
                        }
                    }
                }
                // timer.Enabled = (timerListEntries.Count > 0); invoke event to check if there are any timers and if not disable ticking? todo
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeSelectedEntry();
        }

        private void listViewTimer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                removeSelectedEntry();
        }

        private void removeSelectedEntry()
        {
            if (listViewTimer.SelectedIndices.Count > 0 && MessageBox.Show("Remove the timer \"" + ((TimerListEntry)listViewTimer.SelectedItems[0].Tag).name + "\""
                + (listViewTimer.SelectedIndices.Count > 1 ? " and " + (listViewTimer.SelectedIndices.Count - 1).ToString() + " more timers" : "") + "?"
                , "Remove Timer?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                for (int t = listViewTimer.SelectedIndices.Count - 1; t >= 0; t--)
                    removeTimer((TimerListEntry)listViewTimer.SelectedItems[t].Tag, false);

                onTimerChange?.Invoke();
            }
        }

        private void buttonAddTimer_Click(object sender, EventArgs e)
        {
            addTimer(textBoxTimerName.Text, dateTimePickerTimerFinish.Value, null);
        }

        private void bSetTimerNow_Click(object sender, EventArgs e)
        {
            dateTimePickerTimerFinish.Value = DateTime.Now;
            dhmsInputTimer.Timespan = TimeSpan.Zero;
        }

        private void buttonAddTime_addTimer(TimeSpan timeSpan)
        {
            dhmsInputTimer.Timespan = dhmsInputTimer.Timespan.Add(timeSpan);
            dateTimePickerTimerFinish.Value = DateTime.Now.Add(dhmsInputTimer.Timespan);
        }

        private void dhmsInputTimer_ValueChanged(uiControls.dhmsInput sender, TimeSpan timespan)
        {
            dateTimePickerTimerFinish.Value = DateTime.Now.Add(timespan);
        }

        private void addToOverlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewTimer.SelectedIndices.Count > 0)
            {
                bool show = !((TimerListEntry)listViewTimer.SelectedItems[0].Tag).showInOverlay;
                for (int i = 0; i < listViewTimer.SelectedIndices.Count; i++)
                    ((TimerListEntry)listViewTimer.SelectedItems[i].Tag).showInOverlay = show;
                refreshOverlayTimers();
            }
        }

        private void refreshOverlayTimers()
        {
            if (ARKOverlay.theOverlay != null)
            {
                ARKOverlay.theOverlay.timers.Clear();
                foreach (TimerListEntry tle in timerListEntries)
                {
                    if (tle.showInOverlay == true)
                    {
                        ARKOverlay.theOverlay.timers.Add(tle);
                    }
                }
                ARKOverlay.theOverlay.timers.Sort((t1, t2) => t1.time.CompareTo(t2.time)); // sort timers according to time
            }
            else MessageBox.Show("Overlay is not enabled.", "No Overlay", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public ListViewColumnSorter ColumnSorter { set { listViewTimer.ListViewItemSorter = value; } }

        private void listViewTimer_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnSorter.doSort((ListView)sender, e.Column);
        }

        public enum TimerGroups { Birth, Wakeup, Starving }

        internal void deleteAllExpiredTimers()
        {
            if (MessageBox.Show("Delete all expired timers?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                for (int i = 0; i < timerListEntries.Count; i++)
                {
                    if (timerListEntries[i].time < DateTime.Now)
                        removeTimer(timerListEntries[i--], false);
                }

                onTimerChange?.Invoke();
            }
        }

        private void removeAllExpiredTimersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteAllExpiredTimers();
        }
    }
}
