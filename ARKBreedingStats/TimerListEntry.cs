using ARKBreedingStats.Library;
using System;
using System.Runtime.Serialization;

namespace ARKBreedingStats
{
    [DataContract]
    public class TimerListEntry
    {
        [DataMember]
        public DateTime time;
        [DataMember]
        public string name;
        [DataMember]
        public string sound;
        [DataMember]
        public string group;
        [IgnoreDataMember]
        public System.Windows.Forms.ListViewItem lvi;
        [IgnoreDataMember]
        public bool showInOverlay;
        [DataMember]
        public Guid creatureGuid = Guid.Empty;
        [IgnoreDataMember]
        private Creature _creature;

        [IgnoreDataMember]
        public Creature creature
        {
            get => _creature;
            set
            {
                _creature = value;
                creatureGuid = value?.guid ?? Guid.Empty;
            }
        }
    }
}
