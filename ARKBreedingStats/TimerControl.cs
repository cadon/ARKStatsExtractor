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


        public TimerControl()
        {
            InitializeComponent();
            sounds = new SoundPlayer[3];
            // prevent flickering
            ControlExtensions.DoubleBuffered(listViewTimer, true);
        }

        public void addTimer(string name, DateTime finishTime, Creature c, string group = "Manual Timers")
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

        public void removeTimer(TimerListEntry timerEntry)
        {
            timerEntry.lvi.Remove();
            timerListEntries.Remove(timerEntry);
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

                            if (diff.TotalSeconds < 60.8 && diff.TotalSeconds > 59.2)
                            {
                                playSound(t.group, 3);
                            }
                            else if (diff.TotalSeconds < 20.8 && diff.TotalSeconds > 19.2)
                            {
                                playSound(t.group, 2);
                            }
                            else if (diff.TotalSeconds < 1.2)
                            {
                                playSound(t.group, 1);
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
                default: SystemSounds.Hand.Play(); break;
            }
        }

        private void playSoundFile(SoundPlayer sound)
        {
            if (sound == null) SystemSounds.Hand.Play();
            else sound.Play();
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
            if (listViewTimer.SelectedIndices.Count > 0 && MessageBox.Show("Remove the timer \"" + ((TimerListEntry)listViewTimer.SelectedItems[0].Tag).name + "\"?", "Remove Timer?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                removeTimer((TimerListEntry)listViewTimer.SelectedItems[0].Tag);
            }
        }

        private void buttonAddTimer_Click(object sender, EventArgs e)
        {
            addTimer(textBoxTimerName.Text, dateTimePickerTimerFinish.Value, null);
        }

        private void button10m_Click(object sender, EventArgs e)
        {
            dateTimePickerTimerFinish.Value = DateTime.Now.AddMinutes(10);
        }

        private void button1h_Click(object sender, EventArgs e)
        {
            dateTimePickerTimerFinish.Value = DateTime.Now.AddHours(1);
        }

        private void button5h_Click(object sender, EventArgs e)
        {
            dateTimePickerTimerFinish.Value = DateTime.Now.AddHours(5);
        }

        private void button10h_Click(object sender, EventArgs e)
        {
            dateTimePickerTimerFinish.Value = DateTime.Now.AddHours(10);
        }

        private void buttonSet_Click(object sender, EventArgs e)
        {
            dateTimePickerTimerFinish.Value = DateTime.Now.Add(dhmInputTimer.Timespan);
        }

        private void addToOverlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewTimer.SelectedIndices.Count > 0)
            {
                ((TimerListEntry)listViewTimer.SelectedItems[0].Tag).showInOverlay = !((TimerListEntry)listViewTimer.SelectedItems[0].Tag).showInOverlay;
            }
        }

        private void refreshOverlayTimers()
        {
            ARKOverlay.theOverlay.timers.Clear();
            foreach (TimerListEntry tle in timerListEntries)
            {
                if (tle.showInOverlay == true)
                {
                    ARKOverlay.theOverlay.timers.Add(tle);
                }
            }
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
                        removeTimer(timerListEntries[i--]);
                }
            }
        }
    }
}
