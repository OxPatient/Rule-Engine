#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.Encryption
{
    /// <summary>
    ///     Hash based extensions
    /// </summary>
    public static class HashExtensions
    {
        #region Functions

        #region GenerateSalt

        /// <summary>
        ///     Generates salt
        /// </summary>
        /// <param name="random">Randomization object</param>
        /// <param name="size">Size of the salt byte array</param>
        /// <returns>A byte array as salt</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "Random")]
        public static byte[] GenerateSalt(this Random random, int size)
        {
            var salt = new byte[size];
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                cryptoProvider.GetNonZeroBytes(salt);
            }
            return salt;
        }

        #endregion

        #region Hash

        /// <summary>
        ///     Computes the hash of a byte array
        /// </summary>
        /// <param name="data">Byte array to hash</param>
        /// <param name="algorithm">Hash algorithm to use (defaults to SHA1)</param>
        /// <returns>The hash of the byte array</returns>
        public static byte[] Hash(this byte[] data, HashAlgorithm algorithm = null)
        {
            if (data.IsNull())
                return null;
            using (HashAlgorithm hasher = algorithm.NullCheck(() => new SHA1CryptoServiceProvider()))
            {
                byte[] hashedArray = hasher.ComputeHash(data);
                hasher.Clear();
                return hashedArray;
            }
        }

        /// <summary>
        ///     Computes the hash of a string
        /// </summary>
        /// <param name="data">string to hash</param>
        /// <param name="algorithm">Algorithm to use (defaults to SHA1)</param>
        /// <param name="encodingUsing">Encoding used by the string (defaults to UTF8)</param>
        /// <returns>The hash of the string</returns>
        public static string Hash(this string data, HashAlgorithm algorithm = null, Encoding encodingUsing = null)
        {
            if (data.IsNullOrEmpty())
                return "";
            return BitConverter.ToString(data.ToByteArray(encodingUsing).Hash(algorithm))
                               .Replace("-", "")
                               .Encode(null, encodingUsing);
        }

        #endregion

        #endregion
    }
}