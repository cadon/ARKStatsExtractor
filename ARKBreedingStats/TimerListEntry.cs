using System;
using System.Xml.Serialization;

namespace ARKBreedingStats
{
    [Serializable]
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
            get => _creature;
            set {
                _creature = value;
                creatureGuid = value?.guid ?? Guid.Empty;
            }
        }
    }
}
