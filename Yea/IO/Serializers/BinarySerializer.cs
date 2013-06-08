#region Usings

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#endregion

namespace Yea.IO.Serializers
{
    /// <summary>
    ///     Binary serializer
    /// </summary>
    public class BinarySerializer : ISerializer<byte[]>
    {
        /// <summary>
        ///     Serializes the object
        /// </summary>
        /// <param name="Object">Object to serialize</param>
        /// <returns>The serialized object</returns>
        public byte[] Serialize(object Object)
        {
            Guard.NotNull(Object, "Object");
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, Object);
                return stream.ToArray();
            }
        }

        /// <summary>
        ///     Deserializes the data
        /// </summary>
        /// <param name="objectType">Object type</param>
        /// <param name="data">Data to deserialize</param>
        /// <returns>The resulting object</returns>
        public object Deserialize(byte[] data, Type objectType)
        {
            if (data == null)
                return null;
            using (var stream = new MemoryStream(data))
            {
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(stream);
            }
        }
    }
}