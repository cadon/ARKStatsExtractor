using Newtonsoft.Json;
using System;

namespace ARKBreedingStats.Library
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TimerListEntry
    {
        [JsonProperty]
        public DateTime time { get; set; }
        [JsonProperty]
        public TimeSpan leftTime { get; set; }
        [JsonProperty]
        public bool timerIsRunning { get; set; }
        [JsonProperty]
        public string name { get; set; }
        [JsonProperty]
        public string sound { get; set; }
        [JsonProperty]
        public string group { get; set; }
        public bool showInOverlay { get; set; }
        [JsonProperty]
        public Guid creatureGuid { get; set; }
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
            }
        }

        private void PauseTimer()
        {
            if (timerIsRunning)
            {
                timerIsRunning = false;
                leftTime = time.Subtract(DateTime.Now);
            }
        }

        public void StartStopTimer(bool start)
        {
            if (start)
            {
                StartTimer();
            }
            else
            {
                PauseTimer();
            }
        }

        // Serializer does not support TimeSpan, so use this property for serialization instead.
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
