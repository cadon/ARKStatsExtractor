using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ARKBreedingStats
{
    [Serializable()]
    public class IncubationTimerEntry
    {
        public bool timerIsRunning;
        public TimeSpan incubationDuration;
        public DateTime incubationEnd;
        [XmlIgnore]
        public Creature _mother, _father;
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
            this.incubationDuration = new TimeSpan();
            this.incubationEnd = new DateTime();
        }

        public IncubationTimerEntry(Creature mother, Creature father, TimeSpan incubationDuration, bool incubationStarted)
        {
            this.mother = mother;
            this.father = father;
            this.incubationDuration = incubationDuration;
            this.incubationEnd = new DateTime();
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
            set
            {
                if (value != null)
                    motherGuid = value.guid;
                else motherGuid = Guid.Empty;
                _mother = value;
            }
            get { return _mother; }
        }
        [XmlIgnore]
        public Creature father
        {
            set
            {
                if (value != null)
                    fatherGuid = value.guid;
                else fatherGuid = Guid.Empty;
                _father = value;
            }
            get { return _father; }
        }
    }
}
