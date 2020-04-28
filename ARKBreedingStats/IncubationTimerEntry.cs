using ARKBreedingStats.Library;
using Newtonsoft.Json;
using System;

namespace ARKBreedingStats
{
    [JsonObject(MemberSerialization.OptIn)]
    public class IncubationTimerEntry
    {
        [JsonProperty]
        public bool timerIsRunning;
        public TimeSpan incubationDuration;
        [JsonProperty]
        public DateTime incubationEnd;
        public Creature _mother;
        public Creature _father;
        [JsonProperty]
        public Guid motherGuid;
        [JsonProperty]
        public Guid fatherGuid;
        public string kind; // contains "Egg" or "Gestation", depending on the species
        public bool expired;

        public IncubationTimerEntry()
        {
            mother = new Creature();
            father = new Creature();
            incubationDuration = new TimeSpan();
            incubationEnd = new DateTime();
        }

        public IncubationTimerEntry(Creature mother, Creature father, TimeSpan incubationDuration, bool incubationStarted)
        {
            this.mother = mother;
            this.father = father;
            this.incubationDuration = incubationDuration;
            incubationEnd = new DateTime();
            if (incubationStarted)
                startTimer();
        }

        private void startTimer()
        {
            if (!timerIsRunning)
            {
                timerIsRunning = true;
                incubationEnd = DateTime.Now.Add(incubationDuration);
            }
        }

        private void pauseTimer()
        {
            if (timerIsRunning)
            {
                timerIsRunning = false;
                incubationDuration = incubationEnd.Subtract(DateTime.Now);
            }
        }

        public void startStopTimer(bool start)
        {
            if (start)
                startTimer();
            else pauseTimer();
        }

        public Creature mother
        {
            get => _mother;
            set
            {
                motherGuid = value?.guid ?? Guid.Empty;
                _mother = value;
            }
        }

        public Creature father
        {
            get => _father;
            set
            {
                fatherGuid = value?.guid ?? Guid.Empty;
                _father = value;
            }
        }

        // XmlSerializer does not support TimeSpan, so use this property for serialization instead.
        [System.ComponentModel.Browsable(false)]
        [JsonProperty("incubationDuration")]
        public string incubationDurationString
        {
            get => System.Xml.XmlConvert.ToString(incubationDuration);
            set
            {
                incubationDuration = string.IsNullOrEmpty(value) ?
                    TimeSpan.Zero : System.Xml.XmlConvert.ToTimeSpan(value);
            }
        }
    }
}
