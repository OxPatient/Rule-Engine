#region Usings

using System.Collections.Generic;

#endregion

namespace Yea.Caching
{
    /// <summary>
    ///     Cache interface
    /// </summary>
    public interface ICache<TKeyType> : IEnumerable<object>
    {
        #region Functions

        /// <summary>
        ///     Clears the cache
        /// </summary>
        void Clear();

        /// <summary>
        ///     Removes an item from the cache
        /// </summary>
        /// <param name="key">Key to remove</param>
        void Remove(TKeyType key);

        /// <summary>
        ///     Checks if a key exists in the cache
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if it exists, false otherwise</returns>
        bool Exists(TKeyType key);

        /// <summary>
        ///     Adds an item to the cache
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        void Add(TKeyType key, object value);

        /// <summary>
        ///     Gets a value
        /// </summary>
        /// <typeparam name="TValueType">Value type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>The value specified by the key</returns>
        TValueType Get<TValueType>(TKeyType key);

        #endregion

        #region Properties

        /// <summary>
        ///     Keys used in the cache
        /// </summary>
        ICollection<TKeyType> Keys { get; }

        /// <summary>
        ///     Number of items in the cache
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Gets a specific item based on the key
        /// </summary>
        /// <param name="key">Key to use</param>
        /// <returns>The value associated with the key</returns>
        object this[TKeyType key] { get; set; }

        #endregion
    }
}