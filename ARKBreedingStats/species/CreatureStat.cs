using System.Runtime.Serialization;

namespace ARKBreedingStats
{
    [DataContract]
    public class CreatureStat
    {
        public StatName Stat;
        public double BaseValue;
        public double IncPerWildLevel;
        public double IncPerTamedLevel;
        public double AddWhenTamed;
        public double MultAffinity; // used with taming effectiveness

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
