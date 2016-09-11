using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ARKBreedingStats
{
    [DataContract]
    public class CreatureStat
    {
        public StatName Stat;
        [DataMember]
        public double BaseValue;
        [DataMember]
        public double IncPerWildLevel;
        [DataMember]
        public double IncPerTamedLevel;
        [DataMember]
        public double AddWhenTamed;
        [DataMember]
        public double MultAffinity; // used with taming effectiveness

        public CreatureStat() { }

        public CreatureStat(StatName s) { Stat = s; }

        public CreatureStat(double[] stats)
        {
            setValues(stats);
        }

        public void setValues(double[] stats)
        {
            BaseValue = stats[0];
            IncPerWildLevel = stats[1];
            IncPerTamedLevel = stats[2];
            if (stats.Length > 3)
                AddWhenTamed = stats[3];
            if (stats.Length > 4)
                MultAffinity = stats[4];
        }

        [DataMember]
        private double[] Stats
        {
            get { return new double[] { BaseValue, IncPerWildLevel, IncPerTamedLevel, AddWhenTamed, MultAffinity }; }
            set { setValues(value); }
        }
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
