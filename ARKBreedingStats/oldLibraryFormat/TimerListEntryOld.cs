using System;
using System.Xml.Serialization;

namespace ARKBreedingStats
{
    [Serializable]
    [XmlType(TypeName = "TimerListEntry")]
    public class TimerListEntryOld
    {
        public DateTime time;
        public string name;
        public string sound;
        public string group;
        public Guid creatureGuid = Guid.Empty;
    }
}
