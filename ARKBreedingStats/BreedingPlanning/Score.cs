using System;

namespace ARKBreedingStats.BreedingPlanning
{
    /// <summary>
    /// Represents a score with multiple parts.
    /// When comparing, only the highest different parts is relevant (similar to System.Version).
    /// </summary>
    public struct Score : IComparable<Score>
    {
        public double Primary;
        public double Secondary;
        public double Tertiary;

        public Score(double primary)
        {
            Primary = primary;
            Secondary = 0;
            Tertiary = 0;
        }

        public Score(double primary, double secondary) : this(primary)
        {
            Secondary = secondary;
        }

        public Score(double primary, double secondary, double tertiary) : this(primary, secondary)
        {
            Tertiary = tertiary;
        }

        /// <summary>
        /// Score condensed to one double with loss of information.
        /// </summary>
        public double OneNumber => Primary + Secondary * 0.01 + Tertiary * 0.0001;

        #region overrides

        public override string ToString() => $"{Primary}.{Secondary}.{Tertiary}";

        public string ToString(string format)
        {
            if (Secondary == 0 && Tertiary == 0)
                return Primary.ToString(format);
            if (Tertiary == 0) return $"{Primary.ToString(format)}.{Secondary.ToString(format)}";

            return $"{Primary.ToString(format)}.{Secondary.ToString(format)}.{Tertiary.ToString(format)}";
        }

        public override bool Equals(object obj)
            => obj is Score other && Equals(other);

        public bool Equals(Score other) =>
            Primary == other.Primary
            && Secondary == other.Secondary
            && Tertiary == other.Tertiary;

        public static bool operator ==(Score left, Score right) => left.Equals(right);
        public static bool operator !=(Score left, Score right) => !left.Equals(right);

        public static bool operator <(Score left, Score right)
        {
            if (left.Primary < right.Primary) return true;
            if (left.Primary > right.Primary) return false;
            if (left.Secondary < right.Secondary) return true;
            if (left.Secondary > right.Secondary) return false;
            if (left.Tertiary < right.Tertiary) return true;

            return false;
        }

        public static bool operator >(Score left, Score right)
        {
            if (left.Primary > right.Primary) return true;
            if (left.Primary < right.Primary) return false;
            if (left.Secondary > right.Secondary) return true;
            if (left.Secondary < right.Secondary) return false;
            if (left.Tertiary > right.Tertiary) return true;
            return false;
        }

        public static bool operator <=(Score left, Score right) => left.Equals(right) || left < right;
        public static bool operator >=(Score left, Score right) => left.Equals(right) || left > right;

        public override int GetHashCode() => (int)(Primary * 10000 + Secondary * 100 + Tertiary);

        public int CompareTo(Score other) =>
            other == this ? 0
                : this < other ? -1 : 1;

        #endregion
    }
}
