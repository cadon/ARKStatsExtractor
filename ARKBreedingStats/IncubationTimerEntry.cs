using ARKBreedingStats.Library;
using System;
using System.Runtime.Serialization;

namespace ARKBreedingStats
{
    [DataContract]
    public class IncubationTimerEntry
    {
        [DataMember]
        public bool timerIsRunning;
        [IgnoreDataMember]
        public TimeSpan incubationDuration;
        [DataMember]
        public DateTime incubationEnd;
        [IgnoreDataMember]
        public Creature _mother;
        [IgnoreDataMember]
        public Creature _father;
        [DataMember]
        public Guid motherGuid;
        [DataMember]
        public Guid fatherGuid;
        [IgnoreDataMember]
        public string kind; // contains "Egg" or "Gestation", depending on the species
        [IgnoreDataMember]
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

        [IgnoreDataMember]
        public Creature mother
        {
            get => _mother;
            set
            {
                motherGuid = value?.guid ?? Guid.Empty;
                _mother = value;
            }
        }

        [IgnoreDataMember]
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
        [DataMember(Name = "incubationDuration")]
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
