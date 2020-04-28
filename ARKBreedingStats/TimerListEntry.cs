using ARKBreedingStats.Library;
using Newtonsoft.Json;
using System;

namespace ARKBreedingStats
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TimerListEntry
    {
        [JsonProperty]
        public DateTime time;
        [JsonProperty]
        public string name;
        [JsonProperty]
        public string sound;
        [JsonProperty]
        public string group;
        public System.Windows.Forms.ListViewItem lvi;
        public bool showInOverlay;
        [JsonProperty]
        public Guid creatureGuid;
        private Creature _creature;

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
