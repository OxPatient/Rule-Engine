#region Usings

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.Compression
{
    /// <summary>
    ///     Extension methods dealing with compression
    /// </summary>
    public static class CompressionExtensions
    {
        #region Compress

        /// <summary>
        ///     Compresses the data using the specified compression type
        /// </summary>
        /// <param name="data">Data to compress</param>
        /// <param name="compressionType">Compression type</param>
        /// <returns>The compressed data</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:DoNotDisposeObjectsMultipleTimes")]
        public static byte[] Compress(this byte[] data, CompressionType compressionType = CompressionType.Default)
        {
            Guard.NotNull(data, "data");
            using (var stream = new MemoryStream())
            {
                using (Stream zipStream = GetStream(stream, CompressionMode.Compress, compressionType))
                {
                    zipStream.Write(data, 0, data.Length);
                    zipStream.Close();
                    return stream.ToArray();
                }
            }
        }

        /// <summary>
        ///     Compresses a string of data
        /// </summary>
        /// <param name="data">Data to Compress</param>
        /// <param name="encodingUsing">Encoding that the data uses (defaults to UTF8)</param>
        /// <param name="compressionType">The compression type used</param>
        /// <returns>The data Compressed</returns>
        public static string Compress(this string data, Encoding encodingUsing = null,
                                      CompressionType compressionType = CompressionType.Default)
        {
            Guard.NotEmpty(data, "data");
            return data.ToByteArray(encodingUsing).Compress(compressionType).ToBase64String();
        }

        #endregion

        #region Decompress

        /// <summary>
        ///     Decompresses the byte array that is sent in
        /// </summary>
        /// <param name="data">Data to decompress</param>
        /// <param name="compressionType">The compression type used</param>
        /// <returns>The data decompressed</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:DoNotDisposeObjectsMultipleTimes")]
        public static byte[] Decompress(this byte[] data, CompressionType compressionType = CompressionType.Default)
        {
            Guard.NotNull(data, "data");
            using (var stream = new MemoryStream())
            {
                using (var dataStream = new MemoryStream(data))
                {
                    using (Stream zipStream = GetStream(dataStream, CompressionMode.Decompress, compressionType))
                    {
                        var buffer = new byte[4096];
                        while (true)
                        {
                            int size = zipStream.Read(buffer, 0, buffer.Length);
                            if (size > 0) stream.Write(buffer, 0, size);
                            else break;
                        }
                        zipStream.Close();
                        return stream.ToArray();
                    }
                }
            }
        }

        /// <summary>
        ///     Decompresses a string of data
        /// </summary>
        /// <param name="data">Data to decompress</param>
        /// <param name="encodingUsing">Encoding that the result should use (defaults to UTF8)</param>
        /// <param name="compressionType">The compression type used</param>
        /// <returns>The data decompressed</returns>
        public static string Decompress(this string data, Encoding encodingUsing = null,
                                        CompressionType compressionType = CompressionType.Default)
        {
            Guard.NotEmpty(data, "data");
            return data.FromBase64().Decompress(compressionType).ToEncodedString(encodingUsing);
        }

        #endregion

        #region GetStream

        private static Stream GetStream(MemoryStream stream, CompressionMode mode, CompressionType compressionType)
        {
            switch (compressionType)
            {
                case CompressionType.Deflate:
                    return new DeflateStream(stream, mode, true);
                case CompressionType.GZip:
                    return new GZipStream(stream, mode, true);
                //case CompressionType.BitArray:
                default:
                    //todo: bit array conpress adapter
                    throw new NotImplementedException();
            }

        }

        private static uint[] Transfer(MemoryStream stream)
        {
            var array = new List<UInt32>();
            var reader = new BinaryReader(stream);
            while (stream.Position < stream.Length)
                array.Add(reader.ReadUInt32());

            return array.ToArray();
        }

        private static byte[] Transfer(uint[] uints)
        {
            return uints.SelectMany(BitConverter.GetBytes).ToArray();
        }
/*
        private static uint[] Transfer(byte[] bytes)
        {
            
        }*/
        #endregion
    }
}