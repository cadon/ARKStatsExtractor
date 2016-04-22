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
        private TimerListCollection timerListCollection;
        private Timer timer = new Timer();

        public TimerList()
        {
            InitializeComponent();
            loadTimerList();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(TimerEventProcessor);
            timer.Enabled = true;

            //addTimer("Test", DateTime.Now.AddSeconds(65)); // test-timer
        }

        public void addTimer(string name, DateTime finishTime)
        {
            TimerListEntry tle = new TimerListEntry();
            tle.name = name;
            tle.time = finishTime;
            ListViewItem lvi = new ListViewItem(new string[] { tle.name, tle.time.ToString(), "" });
            tle.lvi = lvi;
            lvi.Tag = tle;
            listViewTimer.Items.Add(lvi);
            timerListCollection.timerListEntries.Add(tle);
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            listViewTimer.BeginUpdate();
            DateTime now = DateTime.Now;
            TimeSpan diff;
            foreach (TimerListEntry t in timerListCollection.timerListEntries)
            {
                if (t.lvi != null)
                {
                    diff = t.time.Subtract(now);
                    if (updateTimer)
                        t.lvi.SubItems[2].Text = diff.ToString("d':'hh':'mm':'ss");
                    if (diff.TotalSeconds < 60.8 && diff.TotalSeconds > 59.2)
                    {
                        t.lvi.BackColor = Color.LightSalmon;
                        System.Media.SystemSounds.Hand.Play();
                    }
                }
            }
            listViewTimer.EndUpdate();
        }

        private void loadTimerList(string fileName = "timerList.xml")
        {
            timerListCollection = new TimerListCollection();
            XmlSerializer reader = new XmlSerializer(typeof(CreatureCollection));

            if (!System.IO.File.Exists(fileName))
            {
                //MessageBox.Show("File with name \"" + fileName + "\" does not exist!", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            System.IO.FileStream file = System.IO.File.OpenRead(fileName);

            try
            {
                timerListCollection = (TimerListCollection)reader.Deserialize(file);
            }
            catch (Exception e)
            {
                MessageBox.Show("TimerList-File couldn't be opened, we thought you should know.\nErrormessage:\n\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                file.Close();
                return;
            }
            file.Close();
        }

        public void saveTimerList(string fileName = "timerList.xml")
        {
            XmlSerializer writer = new XmlSerializer(typeof(TimerListCollection));
            try
            {
                System.IO.FileStream file = System.IO.File.Create(fileName);
                writer.Serialize(file, timerListCollection);
                file.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error during serialization.\nErrormessage:\n\n" + e.Message, "Serialization-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
