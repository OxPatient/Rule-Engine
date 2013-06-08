#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.Encryption
{
    /// <summary>
    ///     Symmetric key extensions
    /// </summary>
    public static class SymmetricExtensions
    {
        #region Functions

        #region Encrypt

        /// <summary>
        ///     Encrypts a string
        /// </summary>
        /// <param name="data">Text to be encrypted</param>
        /// <param name="key">Password to encrypt with</param>
        /// <param name="algorithmUsing">Algorithm to use for encryption (defaults to AES)</param>
        /// <param name="salt">Salt to encrypt with</param>
        /// <param name="hashAlgorithm">Can be either SHA1 or MD5</param>
        /// <param name="passwordIterations">Number of iterations to do</param>
        /// <param name="initialVector">Needs to be 16 ASCII characters long</param>
        /// <param name="keySize">Can be 64 (DES only), 128 (AES), 192 (AES and Triple DES), or 256 (AES)</param>
        /// <param name="encodingUsing">Encoding that the original string is using (defaults to UTF8)</param>
        /// <returns>An encrypted string (Base 64 string)</returns>
        public static string Encrypt(this string data, string key,
                                     Encoding encodingUsing = null,
                                     SymmetricAlgorithm algorithmUsing = null, string salt = "Kosher",
                                     string hashAlgorithm = "SHA1", int passwordIterations = 2,
                                     string initialVector = "OFRna73m*aze01xY", int keySize = 256)
        {
            if (data.IsNullOrEmpty())
                return "";
            return data.ToByteArray(encodingUsing)
                       .Encrypt(key, algorithmUsing, salt, hashAlgorithm, passwordIterations, initialVector, keySize)
                       .ToBase64String();
        }

        /// <summary>
        ///     Encrypts a byte array
        /// </summary>
        /// <param name="data">Data to encrypt</param>
        /// <param name="key">Key to use to encrypt the data (can use PasswordDeriveBytes, Rfc2898DeriveBytes, etc. Really anything that implements DeriveBytes)</param>
        /// <param name="algorithmUsing">Algorithm to use for encryption (defaults to AES)</param>
        /// <param name="initialVector">Needs to be 16 ASCII characters long</param>
        /// <param name="keySize">Can be 64 (DES only), 128 (AES), 192 (AES and Triple DES), or 256 (AES)</param>
        /// <param name="encodingUsing">Encoding that the original string is using (defaults to UTF8)</param>
        /// <returns>An encrypted byte array</returns>
        public static string Encrypt(this string data,
                                     DeriveBytes key,
                                     Encoding encodingUsing = null,
                                     SymmetricAlgorithm algorithmUsing = null,
                                     string initialVector = "OFRna73m*aze01xY",
                                     int keySize = 256)
        {
            if (data.IsNullOrEmpty())
                return "";
            return data.ToByteArray(encodingUsing)
                       .Encrypt(key, algorithmUsing, initialVector, keySize)
                       .ToBase64String();
        }

        /// <summary>
        ///     Encrypts a byte array
        /// </summary>
        /// <param name="data">Data to be encrypted</param>
        /// <param name="key">Password to encrypt with</param>
        /// <param name="algorithmUsing">Algorithm to use for encryption (defaults to AES)</param>
        /// <param name="salt">Salt to encrypt with</param>
        /// <param name="hashAlgorithm">Can be either SHA1 or MD5</param>
        /// <param name="passwordIterations">Number of iterations to do</param>
        /// <param name="initialVector">Needs to be 16 ASCII characters long</param>
        /// <param name="keySize">Can be 64 (DES only), 128 (AES), 192 (AES and Triple DES), or 256 (AES)</param>
        /// <returns>An encrypted byte array</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static byte[] Encrypt(this byte[] data, string key,
                                     SymmetricAlgorithm algorithmUsing = null, string salt = "Kosher",
                                     string hashAlgorithm = "SHA1", int passwordIterations = 2,
                                     string initialVector = "OFRna73m*aze01xY", int keySize = 256)
        {
            return data.Encrypt(new PasswordDeriveBytes(key, salt.ToByteArray(), hashAlgorithm, passwordIterations),
                                algorithmUsing, initialVector, keySize);
        }

        /// <summary>
        ///     Encrypts a byte array
        /// </summary>
        /// <param name="data">Data to encrypt</param>
        /// <param name="key">Key to use to encrypt the data (can use PasswordDeriveBytes, Rfc2898DeriveBytes, etc. Really anything that implements DeriveBytes)</param>
        /// <param name="algorithmUsing">Algorithm to use for encryption (defaults to AES)</param>
        /// <param name="initialVector">Needs to be 16 ASCII characters long</param>
        /// <param name="keySize">Can be 64 (DES only), 128 (AES), 192 (AES and Triple DES), or 256 (AES)</param>
        /// <returns>An encrypted byte array</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:DoNotDisposeObjectsMultipleTimes")]
        public static byte[] Encrypt(this byte[] data,
                                     DeriveBytes key,
                                     SymmetricAlgorithm algorithmUsing = null,
                                     string initialVector = "OFRna73m*aze01xY",
                                     int keySize = 256)
        {
            if (data.IsNull())
                return null;
            algorithmUsing = algorithmUsing.NullCheck(() => new RijndaelManaged());

            Guard.NotEmpty(initialVector, "initialVector");
            using (DeriveBytes derivedPassword = key)
            {
                using (SymmetricAlgorithm symmetricKey = algorithmUsing)
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    byte[] cipherTextBytes;
                    using (
                        ICryptoTransform encryptor = symmetricKey.CreateEncryptor(derivedPassword.GetBytes(keySize/8),
                                                                                  initialVector.ToByteArray()))
                    {
                        using (var memStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                                cipherTextBytes = memStream.ToArray();
                            }
                        }
                    }
                    symmetricKey.Clear();
                    return cipherTextBytes;
                }
            }
        }

        #endregion

        #region Decrypt

        /// <summary>
        ///     Decrypts a string
        /// </summary>
        /// <param name="data">Text to be decrypted (Base 64 string)</param>
        /// <param name="key">Key to use to encrypt the data (can use PasswordDeriveBytes, Rfc2898DeriveBytes, etc. Really anything that implements DeriveBytes)</param>
        /// <param name="encodingUsing">Encoding that the output string should use (defaults to UTF8)</param>
        /// <param name="algorithmUsing">Algorithm to use for decryption (defaults to AES)</param>
        /// <param name="initialVector">Needs to be 16 ASCII characters long</param>
        /// <param name="keySize">Can be 64 (DES only), 128 (AES), 192 (AES and Triple DES), or 256 (AES)</param>
        /// <returns>A decrypted string</returns>
        public static string Decrypt(this string data,
                                     DeriveBytes key,
                                     Encoding encodingUsing = null,
                                     SymmetricAlgorithm algorithmUsing = null,
                                     string initialVector = "OFRna73m*aze01xY",
                                     int keySize = 256)
        {
            if (data.IsNullOrEmpty())
                return "";
            return Convert.FromBase64String(data)
                          .Decrypt(key, algorithmUsing, initialVector, keySize)
                          .ToEncodedString(encodingUsing);
        }

        /// <summary>
        ///     Decrypts a string
        /// </summary>
        /// <param name="data">Text to be decrypted (Base 64 string)</param>
        /// <param name="key">Password to decrypt with</param>
        /// <param name="encodingUsing">Encoding that the output string should use (defaults to UTF8)</param>
        /// <param name="algorithmUsing">Algorithm to use for decryption (defaults to AES)</param>
        /// <param name="salt">Salt to decrypt with</param>
        /// <param name="hashAlgorithm">Can be either SHA1 or MD5</param>
        /// <param name="passwordIterations">Number of iterations to do</param>
        /// <param name="initialVector">Needs to be 16 ASCII characters long</param>
        /// <param name="keySize">Can be 64 (DES only), 128 (AES), 192 (AES and Triple DES), or 256 (AES)</param>
        /// <returns>A decrypted string</returns>
        public static string Decrypt(this string data, string key,
                                     Encoding encodingUsing = null,
                                     SymmetricAlgorithm algorithmUsing = null, string salt = "Kosher",
                                     string hashAlgorithm = "SHA1", int passwordIterations = 2,
                                     string initialVector = "OFRna73m*aze01xY", int keySize = 256)
        {
            if (data.IsNullOrEmpty())
                return "";
            return Convert.FromBase64String(data)
                          .Decrypt(key, algorithmUsing, salt, hashAlgorithm, passwordIterations, initialVector, keySize)
                          .ToEncodedString(encodingUsing);
        }


        /// <summary>
        ///     Decrypts a byte array
        /// </summary>
        /// <param name="data">Data to encrypt</param>
        /// <param name="key">Key to use to encrypt the data (can use PasswordDeriveBytes, Rfc2898DeriveBytes, etc. Really anything that implements DeriveBytes)</param>
        /// <param name="algorithmUsing">Algorithm to use for encryption (defaults to AES)</param>
        /// <param name="initialVector">Needs to be 16 ASCII characters long</param>
        /// <param name="keySize">Can be 64 (DES only), 128 (AES), 192 (AES and Triple DES), or 256 (AES)</param>
        /// <returns>An encrypted byte array</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:DoNotDisposeObjectsMultipleTimes")]
        public static byte[] Decrypt(this byte[] data,
                                     DeriveBytes key,
                                     SymmetricAlgorithm algorithmUsing = null,
                                     string initialVector = "OFRna73m*aze01xY",
                                     int keySize = 256)
        {
            if (data.IsNull())
                return null;
            algorithmUsing = algorithmUsing.NullCheck(() => new RijndaelManaged());
            Guard.NotEmpty(initialVector, "initialVector");
            using (DeriveBytes derivedPassword = key)
            {
                using (SymmetricAlgorithm symmetricKey = algorithmUsing)
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    byte[] plainTextBytes;
                    using (
                        ICryptoTransform decryptor = symmetricKey.CreateDecryptor(derivedPassword.GetBytes(keySize/8),
                                                                                  initialVector.ToByteArray()))
                    {
                        using (var memStream = new MemoryStream(data))
                        {
                            using (var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                            {
                                plainTextBytes = cryptoStream.ReadAllBinary();
                            }
                        }
                    }
                    symmetricKey.Clear();
                    return plainTextBytes;
                }
            }
        }

        /// <summary>
        ///     Decrypts a byte array
        /// </summary>
        /// <param name="data">Data to be decrypted</param>
        /// <param name="key">Password to decrypt with</param>
        /// <param name="algorithmUsing">Algorithm to use for decryption</param>
        /// <param name="salt">Salt to decrypt with</param>
        /// <param name="hashAlgorithm">Can be either SHA1 or MD5</param>
        /// <param name="passwordIterations">Number of iterations to do</param>
        /// <param name="initialVector">Needs to be 16 ASCII characters long</param>
        /// <param name="keySize">Can be 64 (DES only), 128 (AES), 192 (AES and Triple DES), or 256 (AES)</param>
        /// <returns>A decrypted byte array</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static byte[] Decrypt(this byte[] data, string key,
                                     SymmetricAlgorithm algorithmUsing = null, string salt = "Kosher",
                                     string hashAlgorithm = "SHA1", int passwordIterations = 2,
                                     string initialVector = "OFRna73m*aze01xY", int keySize = 256)
        {
            return data.Decrypt(new PasswordDeriveBytes(key, salt.ToByteArray(), hashAlgorithm, passwordIterations),
                                algorithmUsing, initialVector, keySize);
        }

        #endregion

        #endregion
    }
}