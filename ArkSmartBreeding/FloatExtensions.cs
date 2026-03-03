using System;

namespace ARKBreedingStats
{
    public static class FloatExtensions
    {
        /// <summary>
        /// Returns the float precision (ULP) of the given value.
        /// </summary>
        public static float FloatPrecision(this float x)
        {
            if (float.IsNaN(x))
            {
                return x;
            }

            float v;
            if (x == 0.0f)
            {
                v = BitConverter.ToSingle(BitConverter.GetBytes((uint)1), 0);
                return (x > 0) ? v : -v;
            }

            uint i = BitConverter.ToUInt32(BitConverter.GetBytes(x), 0) + 1;
            v = BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
            return v - x;
        }
    }
}
