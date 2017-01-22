using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ARKBreedingStats
{
    public partial class TimerList : UserControl
    {
        private bool updateTimer;
        private List<TimerListEntry> timerListEntries;
        private Timer timer = new Timer();
        public delegate void timerChanged(bool changed = true, string species = "0"); // it's a change, and the species doesn't matter (no updates required)
        public event timerChanged onTimerChange;

        public TimerList()
        {
            InitializeComponent();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(TimerEventProcessor);
            timer.Enabled = true;
        }

        public void addTimer(string name, DateTime finishTime)
        {
            TimerListEntry tle = new TimerListEntry();
            tle.name = name;
            tle.time = finishTime;
            tle.lvi = createLvi(name, finishTime, tle);
            int i = 0;
            while (i < listViewTimer.Items.Count && ((TimerListEntry)listViewTimer.Items[i].Tag).time < finishTime) { i++; }
            listViewTimer.Items.Insert(i, tle.lvi);
            timerListEntries.Add(tle);
            timer.Enabled = true;
            onTimerChange?.Invoke();
        }

        public void removeTimer(TimerListEntry timerEntry)
        {
            timerEntry.lvi.Remove();
            timerListEntries.Remove(timerEntry);
            timer.Enabled = (timerListEntries.Count > 0);
            onTimerChange?.Invoke();
        }

        private ListViewItem createLvi(string name, DateTime finishTime, TimerListEntry tle)
        {
            ListViewItem lvi = new ListViewItem(new string[] { name, finishTime.ToString(), "" });
            lvi.Tag = tle;
            return lvi;
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
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
                            if (diff.TotalSeconds < 11)
                                t.lvi.BackColor = Color.LightSalmon;
                            if (diff.TotalSeconds < 60.8 && diff.TotalSeconds > 59.2)
                            {
                                System.Media.SystemSounds.Hand.Play();
                            }
                            if (diff.TotalSeconds < 20.8 && diff.TotalSeconds > 19.2)
                            {
                                System.Media.SystemSounds.Beep.Play();
                            }
                        }
                    }
                }
                listViewTimer.EndUpdate();
            }
        }

        public bool UpdateTimes
        {
            set
            {
                updateTimer = value;
                if (value)
                    TimerEventProcessor(null, null);
            }
            get { return updateTimer; }
        }

        public List<TimerListEntry> TimerListEntries
        {
            set
            {
                timerListEntries = value;
                listViewTimer.Items.Clear();

                foreach (TimerListEntry tle in timerListEntries)
                {
                    tle.lvi = createLvi(tle.name, tle.time, tle);
                    int i = 0;
                    while (i < listViewTimer.Items.Count && ((TimerListEntry)listViewTimer.Items[i].Tag).time < tle.time) { i++; }
                    listViewTimer.Items.Insert(i, tle.lvi);
                }
                timer.Enabled = (timerListEntries.Count > 0);
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
            addTimer(textBoxTimerName.Text, dateTimePickerTimerFinish.Value);
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

        private void buttonSet_Click(object sender, EventArgs e)
        {
            dateTimePickerTimerFinish.Value = DateTime.Now.AddHours(dTPickerCustom.Value.Hour).AddMinutes(dTPickerCustom.Value.Minute).AddSeconds(dTPickerCustom.Value.Second);
        }
    }
}
