using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ARKBreedingStats
{
    [Serializable()]
    public class TimerListEntry
    {
        public DateTime time;
        public string name;
        public string group;
        [XmlIgnore]
        public System.Windows.Forms.ListViewItem lvi;
        [XmlIgnore]
        public bool showInOverlay;
        public Guid creatureGuid = Guid.Empty;
        [XmlIgnore]
        private Creature _creature;
        [XmlIgnore]
        public Creature creature
        {
            set
            {
                _creature = value;
                creatureGuid = value == null ? Guid.Empty : value.guid;
            }
            get { return _creature; }
        }
    }
}
