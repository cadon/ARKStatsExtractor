using System.Collections;
using System.Collections.Generic;

namespace ARKBreedingStats.utils
{
    class NaturalStringComparer : IComparer<string>
    {
        public bool SkipSpaces { get; set; } = false;

        int IComparer<string>.Compare(string aStr, string bStr)
        {
            if (aStr is null && bStr is null) return 0;
            if (aStr is null) return -1;
            if (bStr is null) return 1;

            // State vars
            int aI = 0;
            int bI = 0;
            int aLen = aStr.Length;
            int bLen = bStr.Length;

            while (true)
            {
                // Handle when we hit the end of either string
                if (aI >= aLen && bI >= bLen) return 0;
                if (aI >= aLen) return -1; // The shorter string sorts first
                if (bI >= bLen) return 1;

                // Skip spaces on both sides, if requested
                if (SkipSpaces)
                {
                    aI += SkipWhiteSpace(aStr, aI);
                    bI += SkipWhiteSpace(bStr, bI);
                }

                // Pick up the next character from each string
                char a = aStr[aI];
                char b = bStr[bI];

                // Easy case - one or both sides are not digits
                if (!IsDigit(a) || !IsDigit(b))
                {
                    // Use simple ASCII indexes to compare characters
                    // (may want to upgrade to Unicode comparisons)
                    if (a > b) return 1;
                    if (b > a) return -1;

                    // Both match, so move on
                    aI += 1;
                    bI += 1;
                    continue;
                }

                // Both sides are numbers so compare runs of digits as numbers
                var (aNum, aSpanLen) = ParseNumber(aStr, aI);
                var (bNum, bSpanLen) = ParseNumber(bStr, bI);
                if (aNum > bNum) return 1;
                if (bNum > aNum) return -1;
                aI += aSpanLen;
                bI += bSpanLen;
            }
        }

        bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        /// <summary>
        /// Take characters from a string at the given start position, parsing them into an integer.
        /// Stops when any non-digit is found.
        /// </summary>
        /// <returns>The value of the integer and the number of characters consumed.</returns>
        (int value, int len) ParseNumber(string str, int start)
        {
            int acc = 0;
            int len = 0;

            for (int i = start; i < str.Length; i++)
            {
                char c = str[i];
                if (!IsDigit(c)) break;

                acc = acc * 10 + (c - '0');
                len += 1;
            }

            return (acc, len);
        }

        /// <summary>
        /// Consume whitespace from the string.
        /// </summary>
        /// <returns>The number of characters consumed.</returns>
        int SkipWhiteSpace(string str, int start)
        {
            int len = 0;
            for (int i = start; i < str.Length; i++)
            {
                char c = str[i];
                if (!char.IsWhiteSpace(c)) break;
                len += 1;
            }

            return len;
        }
    }


    /// <summary>
    /// A comparer that uses natural sorting for strings and delegates to the
    /// system comparer for any other types.
    /// </summary>
    class NaturalComparer : IComparer, IComparer<object>
    {
        readonly IComparer<string> _stringComparer;

        int IComparer<object>.Compare(object a, object b) => InternalCompare(a, b);
        int IComparer.Compare(object a, object b) => InternalCompare(a, b);

        /// <summary>
        /// A natural sort comparer.
        /// </summary>
        /// <param name="skipSpaces">If true, white space between words is ignored.</param>
        public NaturalComparer(bool skipSpaces = false)
        {
            _stringComparer = new NaturalStringComparer() { SkipSpaces = skipSpaces };
        }

        int InternalCompare(object a, object b)
        {
            // Handle cases with nulls and where both references point to the same object
            if (ReferenceEquals(a, b)) return 0; // includes if both are null
            if (a is null) return -1;
            if (b is null) return 1;

            // Use our natural comparer if both are strings
            if (a is string aStr && b is string bStr) return _stringComparer.Compare(aStr, bStr);

            // Fall back to the correct comparer for this type
            return Comparer.Default.Compare(a, b);
        }
    }
}
