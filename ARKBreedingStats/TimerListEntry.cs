using ARKBreedingStats.Library;
using Newtonsoft.Json;
using System;

namespace ARKBreedingStats
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TimerListEntry
    {
        [JsonProperty]
        public DateTime time;
        [JsonProperty]
        public TimeSpan leftTime;
        [JsonProperty]
        public bool timerIsRunning;
        [JsonProperty]
        public string name;
        [JsonProperty]
        public string sound;
        [JsonProperty]
        public string group;
        public System.Windows.Forms.ListViewItem lvi;
        public bool showInOverlay;
        [JsonProperty]
        public Guid creatureGuid;
        private Creature _creature;

        public TimerListEntry()
        {
            timerIsRunning = true;
        }

        public Creature creature
        {
            get => _creature;
            set
            {
                _creature = value;
                creatureGuid = value?.guid ?? Guid.Empty;
            }
        }

        private void StartTimer()
        {
            if (!timerIsRunning)
            {
                timerIsRunning = true;
                time = DateTime.Now.Add(leftTime);
                lvi.SubItems[1].Text = time.ToString();
            }
        }

        private void PauseTimer()
        {
            if (timerIsRunning)
            {
                timerIsRunning = false;
                leftTime = time.Subtract(DateTime.Now);
                lvi.SubItems[1].Text = Loc.S("paused");
            }
        }

        public void StartStopTimer(bool start)
        {
            if (start)
                StartTimer();
            else PauseTimer();
        }

        // Serializer does not support TimeSpan directly, so use this property for serialization instead.
        [System.ComponentModel.Browsable(false)]
        [JsonProperty("timerDuration")]
        public string timerDurationString
        {
            get => System.Xml.XmlConvert.ToString(leftTime);
            set => leftTime = string.IsNullOrEmpty(value) ?
                    TimeSpan.Zero : System.Xml.XmlConvert.ToTimeSpan(value);
        }
    }
}
