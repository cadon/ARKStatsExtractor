using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats.valueClasses
{
    public class MinMaxDouble
    {
        public double Min, Max;
        public MinMaxDouble(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public MinMaxDouble(double minMax)
        {
            Min = minMax;
            Max = minMax;
        }

        public MinMaxDouble(MinMaxDouble source)
        {
            Min = source.Min;
            Max = source.Max;
        }

        public double Mean { get { return (Min + Max) / 2; } }

        public double MinMax { set { Min = value; Max = value; } }

        public bool Includes(MinMaxDouble range)
        {
            return Max >= range.Max && Min <= range.Min;
        }
        public bool Overlaps(MinMaxDouble range)
        {
            return Max >= range.Min && Min <= range.Max;
        }
        public bool SetToInsersectionWith(MinMaxDouble range)
        {
            if (Overlaps(range))
            {
                Min = Math.Max(Min, range.Min);
                Max = Math.Min(Max, range.Max);
                return true;
            }
            return false;
        }
        public bool Includes(double value)
        {
            return Max >= value && Min <= value;
        }

        public MinMaxDouble Clone()
        {
            return new MinMaxDouble(Min, Max);
        }

        static public bool Overlaps(MinMaxDouble range1, MinMaxDouble range2) { return range1.Overlaps(range2); }
    }

    public class MinMaxInt
    {
        public int Min, Max;
        public MinMaxInt(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int MinMax { set { Min = value; Max = value; } }

        public bool Includes(int value)
        {
            return Max >= value && Min <= value;
        }
    }
}
