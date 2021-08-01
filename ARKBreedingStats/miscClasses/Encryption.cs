using System;
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
    }
}
