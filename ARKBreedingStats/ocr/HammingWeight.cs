using System;

namespace ARKBreedingStats.ocr
{
    public static class HammingWeight
    {
        // Hamming-weight lookup-table, taken from http://www.necessaryandsufficient.net/2009/04/optimising-bit-counting-using-iterative-data-driven-development/

        private static readonly byte[] bitCounts = new byte[ushort.MaxValue + 1];

        private static bool HammingIsInitialized;  // will be false by default

        private static uint BitsSetCountWegner(uint input)
        {
            uint count;
            for (count = 0; input != 0; count++)
            {
                input &= input - 1; // turn off the rightmost 1-bit
            }
            return count;
        }

        private static void InitializeBitcounts()
        {
            for (uint i = 0; i < UInt16.MaxValue; i++)
            {
                bitCounts[i] = (byte)BitsSetCountWegner(i);
            }
            bitCounts[ushort.MaxValue] = 16;
            HammingIsInitialized = true;
        }

        public static uint HWeight(uint i)
        {
            if (!HammingIsInitialized)
                InitializeBitcounts();

            return (uint)(bitCounts[i & 0xFFFF] + bitCounts[(i >> 16) & 0xFFFF]);
        }
    }
}
