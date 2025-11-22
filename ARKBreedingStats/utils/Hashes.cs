using System.Collections.Generic;

namespace ARKBreedingStats.utils
{
    internal static class Hashes
    {
        /// <summary>
        /// Combines a sequence of hash codes into a single hash code, preserving the order of the input hashes.
        /// </summary>
        public static int CombineOrderedHashes(IEnumerable<int> hashes)
        {
            if (hashes == null) return 0;
            unchecked
            {
                var combinedHash = 17;
                foreach (var hash in hashes)
                    combinedHash = combinedHash * 31 + hash;

                return combinedHash;
            }
        }
    }
}
