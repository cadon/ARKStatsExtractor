using System;
using ARKBreedingStats.Library;
using Newtonsoft.Json;

namespace ARKBreedingStats.BreedingPlanning
{
    /// <summary>
    /// Represents a pair currently breeding.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class CurrentBreedingPair
    {
        private Creature _mother;
        private Creature _father;
        [JsonProperty] public Guid GuidMother;
        [JsonProperty] public Guid GuidFather;

        public Creature Mother
        {
            get => _mother;
            set
            {
                _mother = value;
                GuidMother = value?.guid ?? Guid.Empty;
            }
        }

        public Creature Father
        {
            get => _father;
            set
            {
                _father = value;
                GuidFather = value?.guid ?? Guid.Empty;
            }
        }

        public DateTime StartedBreedingAt;

        public CurrentBreedingPair(Creature mother, Creature father)
        {
            Mother = mother;
            Father = father;
            StartedBreedingAt = DateTime.UtcNow;
        }

        public override int GetHashCode()
        {
            return GuidMother.GetHashCode() ^ GuidFather.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is CurrentBreedingPair cbp
                   && GuidFather == cbp.GuidFather
                   && GuidMother == cbp.GuidMother;
        }

        public static bool operator ==(CurrentBreedingPair a, CurrentBreedingPair b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return (a.GuidMother == b.GuidMother && a.GuidFather == b.GuidFather)
                || (a.GuidMother == b.GuidFather && a.GuidFather == b.GuidMother);
        }

        public static bool operator !=(CurrentBreedingPair a, CurrentBreedingPair b) => !(a == b);
    }
}
