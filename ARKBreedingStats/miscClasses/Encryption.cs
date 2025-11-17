using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ARKBreedingStats.miscClasses
{
    static class Encryption
    {
        private static readonly byte[] AdditionalEntropy = Encoding.UTF8.GetBytes("Additional Entropy");

        /// <summary>
        /// Converts a string into a base64 encoded ProdectedData encrypted byte array
        /// </summary>
        /// <param name="value">The string to encrypt</param>
        public static string Protect(string value)
        {
            try
            {
                var decryptedBytes = Encoding.UTF8.GetBytes(value);

                // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
                //  only by the same current user.
                var encryptedBytes = ProtectedData.Protect(decryptedBytes, AdditionalEntropy, DataProtectionScope.CurrentUser);

                return Convert.ToBase64String(encryptedBytes);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("Data was not encrypted. An error occurred.");
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Converts base64 encoded ProtectedData encrypted byte array into a string
        /// </summary>
        /// <param name="value">The string to decrypt</param>
        public static string Unprotect(string value)
        {
            if (value == null)
            {
                return null;
            }

            try
            {
                var encryptedBytes = Convert.FromBase64String(value);

                //Decrypt the data using DataProtectionScope.CurrentUser.
                var decryptedBytes = ProtectedData.Unprotect(encryptedBytes, AdditionalEntropy, DataProtectionScope.CurrentUser);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("Data was not decrypted. An error occurred.");
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Checks if file matches the given hash.
        /// The hash is the md5 hash of a file and the length separated by a colon and prefixed by "md5:".
        /// Returns true if file is available and the hash matches.
        /// Returns false if file is not available or hash does not match.
        /// Returns null on error.
        /// </summary>
        public static bool? FileEqualByHash(string filePath, string remoteFileHash)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.WriteLine("File path cannot be null or empty");
                return null;
            }
            if (string.IsNullOrEmpty(remoteFileHash))
            {
                Debug.WriteLine("hash cannot be null or empty");
                return null;
            }
            if (!remoteFileHash.StartsWith("md5:"))
            {
                Debug.WriteLine("Invalid hash, should start with md5:");
                return null;
            }
            if (!File.Exists(filePath))
            {
                Debug.WriteLine("The specified file does not exist: " + filePath);
                return false;
            }
            var parts = remoteFileHash.Split(':');
            if (parts.Length != 3 || !long.TryParse(parts[2], out var hashFileLength))
            {
                Debug.WriteLine("Invalid hash, should contain file length in index 2");
                return null;
            }

            var fileLength = new FileInfo(filePath).Length;
            if (fileLength != hashFileLength) return false;

            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = md5.ComputeHash(stream);
                var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                return remoteFileHash == $"md5:{hashString}:{fileLength}";
            }
        }

        public static string Md5(string text)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(text));
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
        }
    }
}
