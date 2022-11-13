using System;

namespace ARKBreedingStats.utils
{
    internal static class Convert32
    {
        /// <summary>
        /// Encodes a byte array to a string using the characters A-V and 0-9.
        /// This can be used instead of ToBase64String if the resulting string should be different also when ignoring the casing.
        /// </summary>
        public static string ToBase32String(byte[] bytes)
        {
            if (bytes == null) return null;
            var outputLength = (int)Math.Ceiling(bytes.Length * 8d / 5);
            var s = new char[outputLength];

            var bitsRemaining = 0;
            byte lastByteReminder = 0;
            var byteIndex = 0;

            for (int i = 0; i < outputLength; i++)
            {
                char nextChar;
                if (bitsRemaining < 5 && byteIndex < bytes.Length)
                {
                    var nextByte = bytes[byteIndex++];
                    nextChar = ByteToChar(lastByteReminder | (nextByte << bitsRemaining));
                    var bitsOfNextByteTaken = 5 - bitsRemaining;
                    lastByteReminder = (byte)(nextByte >> bitsOfNextByteTaken);
                    bitsRemaining = 8 - bitsOfNextByteTaken;
                }
                else
                {
                    nextChar = ByteToChar(lastByteReminder);
                    bitsRemaining -= 5;
                    lastByteReminder >>= 5;
                }

                s[i] = nextChar;
            }

            return new string(s);
        }

        /// <summary>
        /// Byte is expected to be in the range of 0 - 2^5.
        /// </summary>
        private static char ByteToChar(int b)
        {
            return OutputChars[b & 0x1f];
        }

        private static readonly char[] OutputChars = "ABCDEFGHIJKLMNOPQRSTUV01234567890".ToCharArray();

        /// <summary>
        /// Decodes a string created by ToBase32String.
        /// </summary>
        public static byte[] FromBase32String(string s)
        {
            var bytes = new byte[s.Length * 5 / 8];

            var byteIndex = 0;
            int nextByteBuffer = 0;
            var nextByteHasBits = 0;

            foreach (var c in s)
            {
                var bitValues = CharToByte(c);
                nextByteBuffer |= bitValues << nextByteHasBits;
                nextByteHasBits += 5;
                if (nextByteHasBits > 7)
                {
                    bytes[byteIndex++] = (byte)nextByteBuffer;
                    nextByteHasBits -= 8;
                    nextByteBuffer >>= 8;
                }
            }
            if (byteIndex < bytes.Length)
            {
                bytes[byteIndex] = (byte)nextByteBuffer;
            }

            return bytes;
        }

        private static int CharToByte(char c)
        {
            switch (c)
            {
                case 'A': return 0;
                case 'B': return 1;
                case 'C': return 2;
                case 'D': return 3;
                case 'E': return 4;
                case 'F': return 5;
                case 'G': return 6;
                case 'H': return 7;
                case 'I': return 8;
                case 'J': return 9;
                case 'K': return 10;
                case 'L': return 11;
                case 'M': return 12;
                case 'N': return 13;
                case 'O': return 14;
                case 'P': return 15;
                case 'Q': return 16;
                case 'R': return 17;
                case 'S': return 18;
                case 'T': return 19;
                case 'U': return 20;
                case 'V': return 21;
                case '0': return 22;
                case '1': return 23;
                case '2': return 24;
                case '3': return 25;
                case '4': return 26;
                case '5': return 27;
                case '6': return 28;
                case '7': return 29;
                case '8': return 30;
                case '9': return 31;
                default: throw new ArgumentOutOfRangeException(c + " is an invalid character for base32 encoded string");
            }
        }
    }
}
