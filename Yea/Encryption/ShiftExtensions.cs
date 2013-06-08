#region Usings

using System;
using System.Text;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.Encryption
{
    /// <summary>
    ///     Extensions that deal with bit xoring
    /// </summary>
    public static class ShiftExtensions
    {
        #region Functions

        #region Encrypt

        /// <summary>
        ///     Encrypts the data using a basic xor of the key (not very secure unless doing a one time pad)
        /// </summary>
        /// <param name="data">Data to encrypt</param>
        /// <param name="key">Key to use</param>
        /// <param name="oneTimePad">Is this a one time pad?</param>
        /// <returns>The encrypted data</returns>
        public static byte[] Encrypt(this byte[] data, byte[] key, bool oneTimePad)
        {
            if (data.IsNull())
                return null;
            Guard.NotNull(key, "key");
            if (oneTimePad && key.Length < data.Length)
                throw new ArgumentException("Key is not long enough");
            return Process(data, key);
        }

        /// <summary>
        ///     Encrypts the data using a basic xor of the key (not very secure unless doing a one time pad)
        /// </summary>
        /// <param name="data">Data to encrypt</param>
        /// <param name="key">Key to use</param>
        /// <param name="oneTimePad">Is this a one time pad?</param>
        /// <param name="encodingUsing">Encoding that the Data uses (defaults to UTF8)</param>
        /// <returns>The encrypted data</returns>
        public static string Encrypt(this string data, string key, bool oneTimePad, Encoding encodingUsing = null)
        {
            if (data.IsNull())
                return "";
            Guard.NotNull(key, "key");
            return
                data.ToByteArray(encodingUsing)
                    .Encrypt(key.ToByteArray(encodingUsing), oneTimePad)
                    .ToEncodedString(encodingUsing);
        }

        #endregion

        #region Decrypt

        /// <summary>
        ///     Decrypts the data using a basic xor of the key (not very secure unless doing a one time pad)
        /// </summary>
        /// <param name="data">Data to encrypt</param>
        /// <param name="key">Key to use</param>
        /// <param name="oneTimePad">Is this a one time pad?</param>
        /// <returns>The decrypted data</returns>
        public static byte[] Decrypt(this byte[] data, byte[] key, bool oneTimePad)
        {
            if (data.IsNull())
                return null;
            Guard.NotNull(key, "key");
            if (oneTimePad && key.Length < data.Length)
                throw new ArgumentException("Key is not long enough");
            return Process(data, key);
        }

        /// <summary>
        ///     Decrypts the data using a basic xor of the key (not very secure unless doing a one time pad)
        /// </summary>
        /// <param name="data">Data to decrypt</param>
        /// <param name="key">Key to use</param>
        /// <param name="oneTimePad">Is this a one time pad?</param>
        /// <param name="encodingUsing">Encoding that the Data uses (defaults to UTF8)</param>
        /// <returns>The encrypted data</returns>
        public static string Decrypt(this string data, string key, bool oneTimePad, Encoding encodingUsing = null)
        {
            if (data.IsNull())
                return "";
            Guard.NotNull(key, "key");
            return
                data.ToByteArray(encodingUsing)
                    .Decrypt(key.ToByteArray(encodingUsing), oneTimePad)
                    .ToEncodedString(encodingUsing);
        }

        #endregion

        #region Process

        /// <summary>
        ///     Actually does the encryption/decryption
        /// </summary>
        private static byte[] Process(byte[] input, byte[] key)
        {
            var outputArray = new byte[input.Length];
            int position = 0;
            for (int x = 0; x < input.Length; ++x)
            {
                outputArray[x] = (byte) (input[x] ^ key[position]);
                ++position;
                if (position >= key.Length)
                    position = 0;
            }
            return outputArray;
        }

        #endregion

        #region XOr

        /// <summary>
        ///     XOrs two strings together, returning the result
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="key">Key to use</param>
        /// <param name="encodingUsing">Encoding that the data uses (defaults to UTF8)</param>
        /// <returns>The XOred string</returns>
        public static string XOr(this string input, string key, Encoding encodingUsing = null)
        {
            if (input.IsNullOrEmpty())
                return "";
            if (key.IsNullOrEmpty())
                return input;
            byte[] inputArray = input.ToByteArray(encodingUsing);
            byte[] keyArray = key.ToByteArray(encodingUsing);
            var outputArray = new byte[inputArray.Length];
            for (int x = 0; x < inputArray.Length; ++x)
            {
                byte keyByte = x < keyArray.Length ? keyArray[x] : (byte) 0;
                outputArray[x] = (byte) (inputArray[x] ^ keyByte);
            }
            return outputArray.ToEncodedString(encodingUsing);
        }

        #endregion

        #endregion
    }
}