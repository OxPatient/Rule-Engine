#region Usings

using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.IO.Serializers
{
    /// <summary>
    ///     JSON serializer
    /// </summary>
    public sealed class JsonSerializer : ISerializer<string>
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="encodingUsing">Encoding that the serializer should use (defaults to ASCII)</param>
        public JsonSerializer(Encoding encodingUsing = null)
        {
            EncodingUsing = encodingUsing.NullCheck(new ASCIIEncoding());
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Encoding that the serializer should use
        /// </summary>
        public Encoding EncodingUsing { get; set; }

        #endregion

        #region Functions

        /// <summary>
        ///     Serializes the object
        /// </summary>
        /// <param name="Object">Object to serialize</param>
        /// <returns>The serialized object</returns>
        public string Serialize(object Object)
        {
            Guard.NotNull(Object, "Object");
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(Object.GetType());
                serializer.WriteObject(stream, Object);
                stream.Flush();
                return EncodingUsing.GetString(stream.GetBuffer(), 0, (int) stream.Position);
            }
        }

        /// <summary>
        ///     Deserializes the data
        /// </summary>
        /// <param name="objectType">Object type</param>
        /// <param name="data">Data to deserialize</param>
        /// <returns>The resulting object</returns>
        public object Deserialize(string data, Type objectType)
        {
            if (data.IsNullOrEmpty())
                return null;
            using (var stream = new MemoryStream(EncodingUsing.GetBytes(data)))
            {
                var serializer = new DataContractJsonSerializer(objectType);
                return serializer.ReadObject(stream);
            }
        }

        #endregion
    }
}