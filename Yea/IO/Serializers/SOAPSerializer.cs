#region Usings

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Security;
using System.Text;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.IO.Serializers
{
    /// <summary>
    ///     SOAP serializer
    /// </summary>
    public class SOAPSerializer : ISerializer<string>
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="encodingUsing">Encoding that the serializer should use (defaults to ASCII)</param>
        public SOAPSerializer(Encoding encodingUsing = null)
        {
            EncodingUsing = encodingUsing.NullCheck(new ASCIIEncoding());
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Encoding that the serializer should use
        /// </summary>
        public virtual Encoding EncodingUsing { get; set; }

        #endregion

        #region Functions

        /// <summary>
        ///     Serializes the object
        /// </summary>
        /// <param name="Object">Object to serialize</param>
        /// <returns>The serialized object</returns>
        [SecuritySafeCritical]
        public string Serialize(object Object)
        {
            Guard.NotNull(Object, "Object");
            using (var stream = new MemoryStream())
            {
                var serializer = new SoapFormatter();
                serializer.Serialize(stream, Object);
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
        [SecuritySafeCritical]
        public object Deserialize(string data, Type objectType)
        {
            if (data.IsNullOrEmpty())
                return null;
            using (var stream = new MemoryStream(EncodingUsing.GetBytes(data)))
            {
                var formatter = new SoapFormatter();
                return formatter.Deserialize(stream);
            }
        }

        #endregion
    }
}