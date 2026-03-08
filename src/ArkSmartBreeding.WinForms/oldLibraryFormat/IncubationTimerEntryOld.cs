using System;
using System.Xml.Serialization;

namespace ARKBreedingStats
{
    [Serializable]
    [XmlType(TypeName = "IncubationTimerEntry")]
    public class IncubationTimerEntryOld
    {
        public bool timerIsRunning;
        [XmlIgnore]
        public TimeSpan incubationDuration;
        public DateTime incubationEnd;
        public Guid motherGuid;
        public Guid fatherGuid;

        public IncubationTimerEntryOld()
        {
            incubationDuration = new TimeSpan();
            incubationEnd = new DateTime();
        }

        // XmlSerializer does not support TimeSpan, so use this property for serialization instead.
        [System.ComponentModel.Browsable(false)]
        [XmlElement(DataType = "duration", ElementName = "incubationDuration")]
        public string incubationDurationString
        {
            get
            {
                return System.Xml.XmlConvert.ToString(incubationDuration);
            }
            set
            {
                incubationDuration = string.IsNullOrEmpty(value) ?
                    TimeSpan.Zero : System.Xml.XmlConvert.ToTimeSpan(value);
            }
        }
    }
}
