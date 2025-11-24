using System;

namespace ARKBreedingStats.utils
{
    internal static class Convert32
    {
        /// <summary>
        /// Encodes a byte array to a string using the characters [0-9a-v].
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
        /// Byte is expected to be in the range of [0, 2^5 - 1].
        /// </summary>
        private static char ByteToChar(int b) => OutputChars[b & 0x1f];

        private static readonly char[] OutputChars = "0123456789abcdefghijklmnopqrstuv".ToCharArray();

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
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'a': return 10;
                case 'b': return 11;
                case 'c': return 12;
                case 'd': return 13;
                case 'e': return 14;
                case 'f': return 15;
                case 'g': return 16;
                case 'h': return 17;
                case 'i': return 18;
                case 'j': return 19;
                case 'k': return 20;
                case 'l': return 21;
                case 'm': return 22;
                case 'n': return 23;
                case 'o': return 24;
                case 'p': return 25;
                case 'q': return 26;
                case 'r': return 27;
                case 's': return 28;
                case 't': return 29;
                case 'u': return 30;
                case 'v': return 31;
                default: throw new ArgumentOutOfRangeException(c + " is an invalid character for a base32 encoded string, only [0-9a-v] are expected.");
            }
        }
    }
}
