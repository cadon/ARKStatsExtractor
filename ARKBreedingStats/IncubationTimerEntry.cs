using System;
using System.Xml.Serialization;

namespace ARKBreedingStats
{
    [Serializable]
    public class IncubationTimerEntry
    {
        public bool timerIsRunning;
        public TimeSpan incubationDuration;
        public DateTime incubationEnd;
        [XmlIgnore]
        public Creature _mother;
        [XmlIgnore]
        public Creature _father;
        public Guid motherGuid;
        public Guid fatherGuid;
        [XmlIgnore]
        public string kind; // contains "Egg" or "Gestation", depending on the species
        [XmlIgnore]
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

        public void startStopTimer(bool timerRunning)
        {
            if (timerRunning)
                startTimer();
            else pauseTimer();
        }

        [XmlIgnore]
        public Creature mother
        {
            get => _mother;
            set {
                motherGuid = value?.guid ?? Guid.Empty;
                _mother = value;
            }
        }

        [XmlIgnore]
        public Creature father
        {
            get => _father;
            set {
                fatherGuid = value?.guid ?? Guid.Empty;
                _father = value;
            }
        }
    }
}
