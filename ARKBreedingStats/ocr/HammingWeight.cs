namespace ARKBreedingStats.ocr
{
    /// <summary>
    /// Lookup table to count set bits of a number.
    /// </summary>
    public static class HammingWeight
    {
        // Hamming-weight lookup-table, https://en.wikipedia.org/wiki/Hamming_weight

        private static byte[] _bitCounts = new byte[byte.MaxValue + 1];

        private static bool _hammingIsInitialized;

        /// <summary>
        /// Returns the number of 1-bits in an uint. E.g. 3 == 0b11 => 2, 4 == 0b100 => 1, 7 == 0b111 => 3.
        /// </summary>
        private static byte BitsSetCountWegner(uint input)
        {
            byte count;
            for (count = 0; input != 0; count++)
            {
                input &= input - 1; // turn off the rightmost 1-bit
            }
            return count;
        }

        /// <summary>
        /// Initialized bit counts if not yet done.
        /// </summary>
        public static void InitializeBitCounts()
        {
            if (_hammingIsInitialized) return;

            //// for ushort
            //_bitCounts = new byte[ushort.MaxValue + 1];
            //_bitCounts[ushort.MaxValue] = 16;

            // for byte
            _bitCounts = new byte[byte.MaxValue + 1];
            _bitCounts[byte.MaxValue] = 8;

            //for (uint i = 0; i < ushort.MaxValue; i++)
            for (uint i = 0; i < byte.MaxValue; i++)
                _bitCounts[i] = BitsSetCountWegner(i);

            _hammingIsInitialized = true;
        }

        /// <summary>
        /// Returns the number of set bits in the passed value.
        /// To compare two integers pass with xor ^
        /// </summary>
        public static int SetBitCount(byte i) => _bitCounts[i];
        //public static int SetBitCount(uint i) => _bitCounts[i & 0xFFFF] + _bitCounts[(i >> 16) & 0xFFFF];
    }
}
