#region Usings

using System.Security.Cryptography;
using System.Text;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.Encryption
{
    /// <summary>
    ///     Utility class for doing RSA Encryption
    /// </summary>
    public static class RSAEncryption
    {
        #region Public Static Functions

        /// <summary>
        ///     Encrypts a string using RSA
        /// </summary>
        /// <param name="input">Input string (should be small as anything over 128 bytes can not be decrypted)</param>
        /// <param name="key">Key to use for encryption</param>
        /// <param name="encodingUsing">Encoding that the input string uses (defaults to UTF8)</param>
        /// <returns>An encrypted string (64bit string)</returns>
        public static string Encrypt(string input, string key, Encoding encodingUsing = null)
        {
            Guard.NotEmpty(input, "input");
            Guard.NotEmpty(key, "key");
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(key);
                byte[] encryptedBytes = rsa.Encrypt(input.ToByteArray(encodingUsing), true);
                rsa.Clear();
                return encryptedBytes.ToBase64String();
            }
        }

        /// <summary>
        ///     Decrypts a string using RSA
        /// </summary>
        /// <param name="input">Input string (should be small as anything over 128 bytes can not be decrypted)</param>
        /// <param name="key">Key to use for decryption</param>
        /// <param name="encodingUsing">Encoding that the result should use (defaults to UTF8)</param>
        /// <returns>A decrypted string</returns>
        public static string Decrypt(string input, string key, Encoding encodingUsing = null)
        {
            Guard.NotEmpty(input, "input");
            Guard.NotEmpty(key, "key");
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(key);
                byte[] encryptedBytes = rsa.Decrypt(input.FromBase64(), true);
                rsa.Clear();
                return encryptedBytes.ToEncodedString(encodingUsing);
            }
        }

        /// <summary>
        ///     Creates a new set of keys
        /// </summary>
        /// <param name="privatePublic">True if private key should be included, false otherwise</param>
        /// <returns>XML representation of the key information</returns>
        public static string CreateKey(bool privatePublic)
        {
            using (var provider = new RSACryptoServiceProvider())
            {
                return provider.ToXmlString(privatePublic);
            }
        }

        /// <summary>
        ///     Takes a string and creates a signed hash of it
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="key">Key to encrypt/sign with</param>
        /// <param name="hash">This will be filled with the unsigned hash</param>
        /// <param name="encodingUsing">Encoding that the input is using (defaults to UTF8)</param>
        /// <returns>A signed hash of the input (64bit string)</returns>
        public static string SignHash(string input, string key, out string hash, Encoding encodingUsing = null)
        {
            Guard.NotEmpty(input, "input");
            Guard.NotEmpty(key, "key");
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(key);
                byte[] hashBytes = input.ToByteArray(encodingUsing).Hash();
                byte[] signedHash = rsa.SignHash(hashBytes, CryptoConfig.MapNameToOID("SHA1"));
                rsa.Clear();
                hash = hashBytes.ToBase64String();
                return signedHash.ToBase64String();
            }
        }

        /// <summary>
        ///     Verifies a signed hash against the unsigned version
        /// </summary>
        /// <param name="hash">The unsigned hash (should be 64bit string)</param>
        /// <param name="signedHash">The signed hash (should be 64bit string)</param>
        /// <param name="key">The key to use in decryption</param>
        /// <returns>True if it is verified, false otherwise</returns>
        public static bool VerifyHash(string hash, string signedHash, string key)
        {
            Guard.NotEmpty(hash, "hash");
            Guard.NotEmpty(signedHash, "signedHash");
            Guard.NotEmpty(key, "key");
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(key);
                byte[] inputArray = signedHash.FromBase64();
                byte[] hashArray = hash.FromBase64();
                bool result = rsa.VerifyHash(hashArray, CryptoConfig.MapNameToOID("SHA1"), inputArray);
                rsa.Clear();
                return result;
            }
        }

        #endregion
    }
}