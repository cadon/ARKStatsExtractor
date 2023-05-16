using ARKBreedingStats.Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ARKBreedingStats.utils;

namespace ARKBreedingStats
{
    public partial class TimerControl : UserControl
    {
        private const string DefaultSoundName = "default";

        public delegate void CreateTimerEventHandler(string name, DateTime time, Creature creature, string group);

        public bool updateTimer;
        private List<TimerListEntry> timerListEntries;
        public event Form1.CollectionChangedEventHandler OnTimerChange;
        public event Action TimerAddedRemoved;
        private List<Creature> creatures;
        public SoundPlayer[] sounds;
        /// <summary>
        /// List of seconds when an alarm should be played if a countdown reaches one these values.
        /// </summary>
        private List<int> timerAlerts;
        private bool noOverlayUpdate;

        public TimerControl()
        {
            Load += TimerControl_Load;
            InitializeComponent();
            sounds = new SoundPlayer[4];
            timerAlerts = new List<int>();
            // prevent flickering
            listViewTimer.DoubleBuffered(true);

            // add ButtonAddTimers
            var times = new Dictionary<string, TimeSpan>()
            {
                    { "+1 m", new TimeSpan(0, 1, 0) },
                    { "+5 m", new TimeSpan(0, 5, 0) },
                    { "+20 m", new TimeSpan(0, 20, 0) },
                    { "+1 h", new TimeSpan(1, 0, 0) },
                    { "+5 h", new TimeSpan(5, 0, 0) },
                    { "+1 d", new TimeSpan(24, 0, 0) }
            };

            int i = 0;
            foreach (KeyValuePair<string, TimeSpan> ts in times)
            {
                var bta = new uiControls.ButtonAddTime
                {
                    timeSpan = ts.Value,
                    Text = ts.Key,
                    Size = new Size(54, 23),
                    Location = new Point(6 + i % 3 * 60, 48 + i / 3 * 29)
                };
                bta.addTimer += buttonAddTime_addTimer;
                groupBox1.Controls.Add(bta);
                i++;
            }
        }

        private void TimerControl_Load(object sender, EventArgs e)
        {
            SoundListBox.Items.Clear();
            SoundListBox.Items.Add(DefaultSoundName);
            //Load sounds from filesystem
            var soundPath = FileService.GetPath("sounds");
            if (Directory.Exists(soundPath))
            {
                SoundListBox.Items.AddRange(Directory.EnumerateFiles(soundPath)
                    .Where(p => Path.GetExtension(p) == ".wav")
                    .Select(p => Path.GetFileName(p)).ToArray());
            }
            SoundListBox.SelectedIndex = 0;
        }

        public void AddTimer(string name, DateTime finishTime, Creature creature = null, string group = "Custom", string soundName = null)
        {
            if (soundName == null)
                soundName = SoundListBox.SelectedItem as string == DefaultSoundName
                    ? null
                    : SoundListBox.SelectedItem as string;

            TimerListEntry tle = new TimerListEntry
            {
                name = name,
                group = group,
                time = finishTime,
                creature = creature,
                sound = soundName,
                showInOverlay = Properties.Settings.Default.DisplayTimersInOverlayAutomatically
            };
            tle.lvi = CreateLvi(name, tle);
            int i = 0;
            while (i < listViewTimer.Items.Count && ((TimerListEntry)listViewTimer.Items[i].Tag).time < finishTime)
            {
                i++;
            }
            listViewTimer.Items.Insert(i, tle.lvi);
            timerListEntries.Add(tle);
            OnTimerChange?.Invoke();
            TimerAddedRemoved?.Invoke();
            RefreshOverlayTimers();
        }

        private void RemoveTimer(TimerListEntry timerEntry, bool invokeChange = true)
        {
            timerEntry.lvi.Remove();
            timerListEntries.Remove(timerEntry);
            if (!invokeChange) return;
            OnTimerChange?.Invoke();
            TimerAddedRemoved?.Invoke();
        }

        private ListViewItem CreateLvi(string name, TimerListEntry tle)
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
            ListViewItem lvi = new ListViewItem(new[] { name, tle.timerIsRunning ? tle.time.ToString() : Loc.S("paused"), string.Empty }, g)
            {
                Tag = tle,
                Checked = Properties.Settings.Default.DisplayTimersInOverlayAutomatically
            };
            return lvi;
        }

        public bool TimerIsNeeded => timerListEntries?.Any() == true;

        public void Tick()
        {
            if (timerListEntries == null || !timerListEntries.Any()) return;

            listViewTimer.BeginUpdate();
            DateTime now = DateTime.Now;
            foreach (TimerListEntry t in timerListEntries)
            {
                if (t.lvi == null)
                    continue;
                TimeSpan diff = t.timerIsRunning ? t.time.Subtract(now) : t.leftTime;
                int totalSeconds = (int)diff.TotalSeconds;
                if (updateTimer)
                    t.lvi.SubItems[2].Text = totalSeconds > 0 ? diff.ToString("dd':'hh':'mm':'ss") : "Finished";
                if (diff.TotalSeconds < 0)
                    continue;
                if (totalSeconds < 11)
                    t.lvi.BackColor = Color.LightSalmon;
                else if (totalSeconds < 61)
                    t.lvi.BackColor = Color.Gold;

                if (timerAlerts == null || !timerAlerts.Any() || totalSeconds > timerAlerts.First())
                    continue;

                for (int i = 0; i < timerAlerts.Count; i++)
                {
                    if (totalSeconds == timerAlerts[i])
                    {
                        PlaySound(t.group, i, null, t.sound);
                        break;
                    }
                }
            }
            listViewTimer.EndUpdate();
        }

        public void PlaySound(string group, int alert, string speakText = null, string customSoundFile = null)
        {
            if (!string.IsNullOrEmpty(speakText))
            {
                using (SpeechSynthesizer synth = new SpeechSynthesizer())
                {
                    synth.SetOutputToDefaultAudioDevice();
                    synth.Speak(speakText);
                }
            }
            else if (!PlayCustomSound(customSoundFile))
            {
                switch (group)
                {
                    case "Starving":
                        PlaySoundFile(sounds[0]);
                        break;
                    case "Wakeup":
                        PlaySoundFile(sounds[1]);
                        break;
                    case "Birth":
                        PlaySoundFile(sounds[2]);
                        break;
                    case "Custom":
                        PlaySoundFile(sounds[3]);
                        break;
                    default:
                        SystemSounds.Hand.Play();
                        break;
                }
            }
        }

        private void PlaySoundFile(SoundPlayer sound)
        {
            if (sound == null) SystemSounds.Hand.Play();
            else sound.Play();
        }

        private List<int> TimerAlerts
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
                }
            }
        }

        public string TimerAlertsCSV
        {
            get => string.Join(",", timerAlerts);
            set
            {
                if (value.Length > 0)
                {
                    List<int> list = new List<int>();
                    var csv = value.Split(',');
                    foreach (string c in csv)
                    {
                        if (int.TryParse(c.Trim(), out int o))
                            list.Add(o);
                    }
                    if (list.Any())
                        TimerAlerts = list;
                }
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
                    tle.lvi = CreateLvi(tle.name, tle);
                    int i = 0;
                    while (i < listViewTimer.Items.Count && ((TimerListEntry)listViewTimer.Items[i].Tag).time < tle.time)
                    {
                        i++;
                    }
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
                // timer.Enabled = (timerListEntries.Any()); invoke event to check if there are any timers and if not disable ticking? todo
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedEntry();
        }

        private void listViewTimer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                RemoveSelectedEntry();
        }

        private void RemoveSelectedEntry()
        {
            if (listViewTimer.SelectedIndices.Count > 0 && MessageBox.Show("Remove the timer \"" + ((TimerListEntry)listViewTimer.SelectedItems[0].Tag).name + "\""
                    + (listViewTimer.SelectedIndices.Count > 1 ? " and " + (listViewTimer.SelectedIndices.Count - 1) + " more timers" : "") + "?"
                    , "Remove Timer?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                for (int t = listViewTimer.SelectedIndices.Count - 1; t >= 0; t--)
                    RemoveTimer((TimerListEntry)listViewTimer.SelectedItems[t].Tag, false);

                RefreshOverlayTimers();
                OnTimerChange?.Invoke();
                TimerAddedRemoved?.Invoke();
            }
        }

        private void buttonAddTimer_Click(object sender, EventArgs e)
        {
            AddTimer(textBoxTimerName.Text, dateTimePickerTimerFinish.Value);
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
                noOverlayUpdate = true;
                bool show = !listViewTimer.SelectedItems[0].Checked;
                for (int i = 0; i < listViewTimer.SelectedIndices.Count; i++)
                    listViewTimer.SelectedItems[i].Checked = show;
                noOverlayUpdate = false;
                RefreshOverlayTimers();
            }
        }

        private void addAllTimersToOverlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AllTimersToOverlay(true);
        }

        private void hideAllTimersFromOverlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AllTimersToOverlay(false);
        }

        /// <summary>
        /// Displays or hides all timers in the overlay.
        /// </summary>
        /// <param name="show"></param>
        private void AllTimersToOverlay(bool show)
        {
            noOverlayUpdate = true;
            for (int i = 0; i < listViewTimer.Items.Count; i++)
                listViewTimer.Items[i].Checked = show;
            noOverlayUpdate = false;
            RefreshOverlayTimers();
        }

        private void RefreshOverlayTimers()
        {
            if (noOverlayUpdate || ARKOverlay.theOverlay == null)
                return;

            ARKOverlay.theOverlay.timers = timerListEntries.Where(t => t.showInOverlay).OrderBy(t => t.time).ToArray();
        }

        public ListViewColumnSorter ColumnSorter
        {
            set => listViewTimer.ListViewItemSorter = value;
        }

        private void listViewTimer_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnSorter.DoSort((ListView)sender, e.Column);
        }

        public enum TimerGroups
        {
            Birth,
            Wakeup,
            Starving
        }

        /// <summary>
        /// Removes all timers that are expired.
        /// </summary>
        /// <param name="confirm">If true, the user is asked for confirmation.</param>
        internal void DeleteAllExpiredTimers(bool confirm = true, bool triggerLibraryChange = true)
        {
            if (!confirm || MessageBox.Show("Delete all expired timers?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                bool timerRemoved = false;
                for (int i = 0; i < timerListEntries.Count; i++)
                {
                    if (timerListEntries[i].time < DateTime.Now)
                    {
                        RemoveTimer(timerListEntries[i--], false);
                        timerRemoved = true;
                    }
                }
                RefreshOverlayTimers();

                if (triggerLibraryChange && timerRemoved)
                {
                    OnTimerChange?.Invoke();
                    TimerAddedRemoved?.Invoke();
                }
            }
        }

        private void removeAllExpiredTimersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteAllExpiredTimers();
        }

        private void btOpenSoundFolder_Click(object sender, EventArgs e)
        {
            var soundPath = FileService.GetPath("sounds");
            try
            {
                Directory.CreateDirectory(soundPath);
            }
            catch (Exception ex)
            {
                MessageBoxes.ExceptionMessageBox(ex, "Error while trying to create the custom sound folder for custom timer-sounds");
                return;
            }
            if (Directory.Exists(soundPath))
                System.Diagnostics.Process.Start(soundPath);
        }

        private void btPlaySelectedSound_Click(object sender, EventArgs e)
        {
            string customSoundFile = SoundListBox.SelectedItem?.ToString();
            if (customSoundFile == DefaultSoundName)
            {
                SystemSounds.Hand.Play();
                return;
            }

            PlayCustomSound(customSoundFile);
        }

        /// <summary>
        /// Plays a custom sound file at a specific folder. Returns false if the file wasn't found.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool PlayCustomSound(string fileName)
        {
            string soundPath = null;
            if (!string.IsNullOrEmpty(fileName))
            {
                soundPath = Path.Combine(FileService.GetPath("sounds"), fileName);
                if (!File.Exists(soundPath))
                    soundPath = null;
            }
            if (!string.IsNullOrEmpty(soundPath))
            {
                using (var sp = new SoundPlayer(soundPath))
                {
                    PlaySoundFile(sp);
                    return true;
                }
            }
            return false;
        }

        public void AdjustAllTimersByOffset(TimeSpan offset)
        {
            foreach (var t in timerListEntries) t.time += offset;
        }

        private void listViewTimer_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ((TimerListEntry)e.Item.Tag).showInOverlay = e.Item.Checked;
            RefreshOverlayTimers();
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Win32API.IsMouseOnListViewHeader(listViewTimer.Handle, MousePosition.Y))
            {
                e.Cancel = true;
                contextMenuStripTimerHeader.Show(Control.MousePosition);
            }
        }

        private void toolStripMenuItemResetLibraryColumnWidths_Click(object sender, EventArgs e)
        {
            for (int ci = 0; ci < listViewTimer.Columns.Count; ci++)
                listViewTimer.Columns[ci].Width = 100;
        }

        private void BtStartPauseTimers_Click(object sender, EventArgs e)
        {
            if (listViewTimer.SelectedIndices.Count == 0) return;

            bool startTimer = true;
            for (int i = 0; i < listViewTimer.SelectedIndices.Count; i++)
            {
                if (listViewTimer.SelectedItems[i].Tag is TimerListEntry tle)
                {
                    if (i == 0)
                    {
                        startTimer = !tle.timerIsRunning;
                    }

                    tle.StartStopTimer(startTimer);
                }
            }
        }

        public ListView ListViewTimers => listViewTimer;

        private void LbTimerPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtRemovePreset.Enabled = LbTimerPresets.SelectedIndex != -1;
        }

        private void LbTimerPresets_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!(LbTimerPresets.SelectedItem is string preset)) return;

            var r = new Regex(@"\A(\d+):(\d+):(\d+):(\d+) - (.*?)(?: - (.*))?\z");
            var m = r.Match(preset);
            if (!m.Success) return;

            var timer = new TimeSpan(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value));
            var soundName = m.Groups[6].Value;
            if (string.IsNullOrWhiteSpace(soundName)) soundName = null;

            AddTimer(m.Groups[5].Value, DateTime.Now.Add(timer), soundName: soundName);
        }

        internal void SetTimerPresets(string[] presets)
        {
            if (presets != null)
                LbTimerPresets.Items.AddRange(presets);
        }

        internal string[] GetTimerPresets()
        {
            return LbTimerPresets.Items.Cast<string>().ToArray();
        }

        private void BtAddPreset_Click(object sender, EventArgs e)
        {
            var soundName = SoundListBox.SelectedItem as string;
            if (soundName == DefaultSoundName) soundName = null;
            if (soundName != null) soundName = " - " + soundName;
            LbTimerPresets.Items.Add($"{dhmsInputTimer.Timespan:dd\\:hh\\:mm\\:ss} - {textBoxTimerName.Text}{soundName}");
        }

        private void BtRemovePreset_Click(object sender, EventArgs e)
        {
            int i = LbTimerPresets.SelectedIndex;
            if (i == -1) return;
            LbTimerPresets.Items.RemoveAt(i);
            if (LbTimerPresets.Items.Count == i) i--;
            if (i != -1)
                LbTimerPresets.SelectedIndex = i;
        }
    }
}
