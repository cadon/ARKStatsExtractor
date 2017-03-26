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
        public bool incubationStarted;
        public TimeSpan incubationDuration;
        public DateTime incubationEnd;
        [XmlIgnore]
        public Creature _mother, _father;
        public Guid motherGuid;
        public Guid fatherGuid;

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
            incubationStarted = true;
            incubationEnd = DateTime.Now.Add(incubationDuration);
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
