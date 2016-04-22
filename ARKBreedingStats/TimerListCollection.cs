using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ARKBreedingStats
{
    [Serializable()]
    class TimerListCollection
    {
        [XmlArray]
        public List<TimerListEntry> timerListEntries = new List<TimerListEntry>();
    }
}
