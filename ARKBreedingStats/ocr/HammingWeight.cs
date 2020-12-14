namespace ARKBreedingStats.ocr
{
    public static class HammingWeight
    {
        // Hamming-weight lookup-table, https://en.wikipedia.org/wiki/Hamming_weight

        private static readonly byte[] bitCounts = new byte[ushort.MaxValue + 1];

        private static bool HammingIsInitialized; // will be false by default

        /// <summary>
        /// Returns the number of 1-bits in an uint. E.g. 3 == 0b11 => 2, 4 == 0b100 => 1, 7 == 0b111 => 3.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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
            for (uint i = 0; i < ushort.MaxValue; i++)
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
