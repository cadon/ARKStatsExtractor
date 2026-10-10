using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace ARKBreedingStats.BreedingPlanning
{
    /// <summary>
    /// Color pattern of a creature.
    /// </summary>
    public class ColorPattern : INotifyPropertyChanged
    {
        public string Name
        {
            get;
            set => SetField(ref field, value);
        }

        /// <summary>
        /// Only the regions whose index is true will be considered.
        /// </summary>
        public bool[] ConsideredRegions;

        /// <summary>
        /// Color id for each region.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public byte[] ColorIds;

        /// <summary>
        /// Color group for each region. If the property or an index is null, the according color id in <see cref="ColorIds"/> is used.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ColorGroup[] ColorGroups;

        public override string ToString() => Name ?? "unknown";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
