#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
using Yea.DataTypes.ExtensionMethods;
using Yea.IO.Serializers;

#endregion

namespace Yea.IO
{
    /// <summary>
    ///     Serialization extensions
    /// </summary>
    public static class SerializationExtensions
    {
        #region Extension Methods

        #region Serialize

        /// <summary>
        ///     Serializes the object using the specified serializer
        /// </summary>
        /// <param name="Object">Object to serialize</param>
        /// <param name="serializer">Serializer to use (defaults to JSONSerializer)</param>
        /// <param name="encodingUsing">Encoding to use (defaults to ASCII)</param>
        /// <param name="fileLocation">File location to save to</param>
        /// <returns>The serialized object</returns>
        public static string Serialize(this object Object, ISerializer<string> serializer = null,
                                       Encoding encodingUsing = null, string fileLocation = "")
        {
            Guard.NotNull(Object, "Object");
            string data = serializer.NullCheck(() => new JsonSerializer(encodingUsing)).Serialize(Object);
            if (!fileLocation.IsNullOrEmpty())
                fileLocation.Save(data);
            return data;
        }

        #endregion

        #region SerializeBinary

        /// <summary>
        ///     Serializes the object using the specified serializer
        /// </summary>
        /// <param name="Object">Object to serialize</param>
        /// <param name="serializer">Serializer to use (defaults to BinarySerializer)</param>
        /// <param name="fileLocation">File location to save to</param>
        /// <returns>The serialized object</returns>
        public static byte[] SerializeBinary(this object Object, ISerializer<byte[]> serializer = null,
                                             string fileLocation = "")
        {
            Guard.NotNull(Object, "Object");
            byte[] data = serializer.NullCheck(() => new BinarySerializer()).Serialize(Object);
            if (!fileLocation.IsNullOrEmpty())
                fileLocation.Save(data);
            return data;
        }

        #endregion

        #region Deserialize

        /// <summary>
        ///     Deserializes an object
        /// </summary>
        /// <typeparam name="TR">Object type</typeparam>
        /// <param name="data">Data to deserialize</param>
        /// <param name="serializer">Serializer to use (defaults to JSONSerializer)</param>
        /// <param name="encodingUsing">Encoding to use (defaults to ASCII)></param>
        /// <returns>The deserialized object</returns>
        public static TR Deserialize<TR>(this string data, ISerializer<string> serializer = null,
                                         Encoding encodingUsing = null)
        {
            return (data.IsNullOrEmpty()) ? default(TR) : (TR) data.Deserialize(typeof (TR), serializer, encodingUsing);
        }

        /// <summary>
        ///     Deserializes an object
        /// </summary>
        /// <typeparam name="TR">Object type</typeparam>
        /// <param name="data">Data to deserialize</param>
        /// <param name="serializer">Serializer to use (defaults to XMLSerializer)</param>
        /// <param name="encodingUsing">Encoding to use (defaults to ASCII)></param>
        /// <returns>The deserialized object</returns>
        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes",
            MessageId = "System.Xml.XmlNode")]
        public static TR Deserialize<TR>(this XmlDocument data, ISerializer<string> serializer = null,
                                         Encoding encodingUsing = null)
        {
            return (data == null)
                       ? default(TR)
                       : (TR)
                         data.InnerXml.Deserialize(typeof (TR),
                                                   serializer.NullCheck(() => new XMLSerializer(encodingUsing)),
                                                   encodingUsing);
        }

        /// <summary>
        ///     Deserializes an object
        /// </summary>
        /// <typeparam name="TR">Object type</typeparam>
        /// <param name="data">Data to deserialize</param>
        /// <param name="serializer">Serializer to use (defaults to BinarySerializer)</param>
        /// <returns>The deserialized object</returns>
        public static TR Deserialize<TR>(this byte[] data, ISerializer<byte[]> serializer = null)
        {
            return (data.IsNull()) ? default(TR) : (TR) data.Deserialize(typeof (TR), serializer);
        }

        /// <summary>
        ///     Deserializes an object
        /// </summary>
        /// <typeparam name="TR">Object type</typeparam>
        /// <param name="data">Data to deserialize</param>
        /// <param name="serializer">Serializer to use (defaults to JSONSerializer)</param>
        /// <param name="encodingUsing">Encoding to use (defaults to ASCII)></param>
        /// <returns>The deserialized object</returns>
        public static TR Deserialize<TR>(this FileInfo data, ISerializer<string> serializer = null,
                                         Encoding encodingUsing = null)
        {
            return (data.IsNull() || !data.Exists)
                       ? default(TR)
                       : (TR) data.Read().Deserialize(typeof (TR), serializer, encodingUsing);
        }

        /// <summary>
        ///     Deserializes an object
        /// </summary>
        /// <param name="data">Data to deserialize</param>
        /// <param name="serializer">Serializer to use (defaults to JSONSerializer)</param>
        /// <param name="encodingUsing">Encoding to use (defaults to ASCII)></param>
        /// <param name="objectType">Object type</param>
        /// <returns>The deserialized object</returns>
        public static object Deserialize(this FileInfo data, Type objectType, ISerializer<string> serializer = null,
                                         Encoding encodingUsing = null)
        {
            if (objectType == null) throw new ArgumentNullException("objectType");
            return (data.IsNull() || !data.Exists)
                       ? null
                       : data.Read().Deserialize(objectType, serializer, encodingUsing);
        }

        /// <summary>
        ///     Deserializes an object
        /// </summary>
        /// <param name="data">Data to deserialize</param>
        /// <param name="objectType">Object type</param>
        /// <param name="serializer">Serializer to use (defaults to JSONSerializer)</param>
        /// <param name="encodingUsing">Encoding to use (defaults to ASCII)></param>
        /// <returns>The deserialized object</returns>
        public static object Deserialize(this string data, Type objectType, ISerializer<string> serializer = null,
                                         Encoding encodingUsing = null)
        {
            return serializer.NullCheck(() => new JsonSerializer(encodingUsing)).Deserialize(data, objectType);
        }

        /// <summary>
        ///     Deserializes an object
        /// </summary>
        /// <param name="data">Data to deserialize</param>
        /// <param name="objectType">Object type</param>
        /// <param name="serializer">Serializer to use (defaults to BinarySerializer)</param>
        /// <returns>The deserialized object</returns>
        public static object Deserialize(this byte[] data, Type objectType, ISerializer<byte[]> serializer = null)
        {
            return serializer.NullCheck(() => new BinarySerializer()).Deserialize(data, objectType);
        }

        /// <summary>
        ///     Deserializes an object
        /// </summary>
        /// <param name="data">Data to deserialize</param>
        /// <param name="objectType">Object type</param>
        /// <param name="serializer">Serializer to use (defaults to XMLSerializer)</param>
        /// <param name="encodingUsing">Encoding to use (defaults to ASCII)></param>
        /// <returns>The deserialized object</returns>
        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes",
            MessageId = "System.Xml.XmlNode")]
        public static object Deserialize(this XmlDocument data, Type objectType, ISerializer<string> serializer = null,
                                         Encoding encodingUsing = null)
        {
            return (data == null)
                       ? null
                       : data.InnerXml.Deserialize(objectType,
                                                   serializer.NullCheck(() => new XMLSerializer(encodingUsing)),
                                                   encodingUsing);
        }

        #endregion

        #region DeserializeBinary

        /// <summary>
        ///     Deserializes an object
        /// </summary>
        /// <param name="data">Data to deserialize</param>
        /// <param name="objectType">Object type</param>
        /// <param name="serializer">Serializer to use (defaults to BinarySerializer)</param>
        /// <returns>The deserialized object</returns>
        public static object DeserializeBinary(this FileInfo data, Type objectType,
                                               ISerializer<byte[]> serializer = null)
        {
            return (data.IsNull() || !data.Exists) ? null : data.ReadBinary().Deserialize(objectType, serializer);
        }

        /// <summary>
        ///     Deserializes an object
        /// </summary>
        /// <typeparam name="TR">Object type</typeparam>
        /// <param name="data">Data to deserialize</param>
        /// <param name="serializer">Serializer to use (defaults to BinarySerializer)</param>
        /// <returns>The deserialized object</returns>
        public static TR DeserializeBinary<TR>(this FileInfo data, ISerializer<byte[]> serializer = null)
        {
            return (data.IsNull() || !data.Exists)
                       ? default(TR)
                       : (TR) data.ReadBinary().Deserialize(typeof (TR), serializer);
        }

        #endregion

        #endregion
    }
}