using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats.utils
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns the float precision of a given number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float FloatPrecision(this float x)
        {
            // Handle NaNs
            if (float.IsNaN(x))
                return x;

            // Return the correct EPSILON either side of zero
            float v;
            if (x == 0.0f)
            {
                v = BitConverter.ToSingle(BitConverter.GetBytes((UInt32)1), 0);
                return (x > 0) ? v : -v;
            }

            // Move one value to the next larger possible value
            UInt32 i = BitConverter.ToUInt32(BitConverter.GetBytes(x), 0) + 1;
            v = BitConverter.ToSingle(BitConverter.GetBytes(i), 0);

            return v - x;
        }
    }
}
