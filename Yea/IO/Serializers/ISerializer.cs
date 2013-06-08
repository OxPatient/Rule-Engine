#region Usings

using System;

#endregion

namespace Yea.IO.Serializers
{
    /// <summary>
    ///     Serializer interface
    /// </summary>
    /// <typeparam name="T">Type that the object is serialized to/from</typeparam>
    public interface ISerializer<T>
    {
        /// <summary>
        ///     Serializes the object
        /// </summary>
        /// <param name="Object">Object to serialize</param>
        /// <returns>The serialized object</returns>
        T Serialize(object Object);

        /// <summary>
        ///     Deserializes the data
        /// </summary>
        /// <param name="objectType">Object type</param>
        /// <param name="data">Data to deserialize</param>
        /// <returns>The resulting object</returns>
        object Deserialize(T data, Type objectType);
    }
}