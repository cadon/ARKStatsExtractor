using System.Runtime.Serialization;

namespace ARKBreedingStats
{
    [DataContract]
    public class CreatureStat
    {
        public StatName Stat;
        public float BaseValue;
        public float IncPerWildLevel;
        public float IncPerTamedLevel;
        public float AddWhenTamed;
        public float MultAffinity; // used with taming effectiveness

        public CreatureStat(StatName s) { Stat = s; }

    }

    public enum StatName
    {
        Health = 0,
        Stamina,
        Oxygen,
        Food,
        Weight,
        Damage,
        Speed,
        Torpor,
    };
}
