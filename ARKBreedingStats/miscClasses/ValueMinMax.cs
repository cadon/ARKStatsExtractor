using System;

namespace ARKBreedingStats.miscClasses
{
    public struct MinMaxDouble
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

        public double Mean => (Min + Max) / 2;

        public double MinMax
        {
            set
            {
                Min = value;
                Max = value;
            }
        }

        public bool Includes(MinMaxDouble range)
        {
            return Max >= range.Max && Min <= range.Min;
        }

        public bool Overlaps(MinMaxDouble range)
        {
            return Max >= range.Min && Min <= range.Max;
        }

        public bool SetToIntersectionWith(MinMaxDouble range)
        {
            if (Overlaps(range))
            {
                Min = Math.Max(Min, range.Min);
                Max = Math.Min(Max, range.Max);
                return true;
            }
            return false;
        }

        public bool SetToIntersectionWith(double min, double max)
        {
            return SetToIntersectionWith(new MinMaxDouble(min, max));
        }

        public bool Includes(double value)
        {
            return Max >= value && Min <= value;
        }

        public MinMaxDouble Clone()
        {
            return new MinMaxDouble(Min, Max);
        }

        public static bool Overlaps(MinMaxDouble range1, MinMaxDouble range2)
        {
            return range1.Overlaps(range2);
        }

        public override string ToString() => $"{Min}, {Mean}, {Max}";
    }

    public struct MinMaxInt
    {
        public int Min, Max;

        public MinMaxInt(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int MinMax
        {
            set
            {
                Min = value;
                Max = value;
            }
        }

        public bool Includes(int value)
        {
            return Max >= value && Min <= value;
        }

        public override string ToString() => $"{Min}, {Max}";
    }
}
