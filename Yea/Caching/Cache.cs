#region Usings

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.Caching
{
    /// <summary>
    ///     Acts as a cache
    /// </summary>
    public class Cache<TKeyType> : ICache<TKeyType>
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        public Cache()
        {
            if (InternalCache == null)
                InternalCache = new ConcurrentDictionary<TKeyType, object>();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Internal cache
        /// </summary>
        protected static ConcurrentDictionary<TKeyType, object> InternalCache { get; private set; }

        /// <summary>
        ///     Collection of keys
        /// </summary>
        public virtual ICollection<TKeyType> Keys
        {
            get { return InternalCache.Keys; }
        }

        /// <summary>
        ///     The number of items in the cache
        /// </summary>
        public virtual int Count
        {
            get { return InternalCache.Count; }
        }

        /// <summary>
        ///     Gets the item associated with the key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>The item associated with the key</returns>
        public virtual object this[TKeyType key]
        {
            get { return Get<object>(key); }
            set { Add(key, value); }
        }

        #endregion

        #region Functions

        /// <summary>
        ///     Gets the enumerator
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<object> GetEnumerator()
        {
            return InternalCache.Keys.Select(key => InternalCache[key]).GetEnumerator();
        }

        /// <summary>
        ///     Gets the enumerator
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalCache.Keys.Select(key => InternalCache[key]).GetEnumerator();
        }

        /// <summary>
        ///     Clears the cache
        /// </summary>
        public virtual void Clear()
        {
            InternalCache.Clear();
        }

        /// <summary>
        ///     Removes an item from the cache
        /// </summary>
        /// <param name="key">Key associated with the item to remove</param>
        public virtual void Remove(TKeyType key)
        {
            if (Exists(key))
            {
                object tempItem;
                InternalCache.TryRemove(key, out tempItem);
            }
        }

        /// <summary>
        ///     Determines if the item exists
        /// </summary>
        /// <param name="key">The key associated with the item</param>
        /// <returns>True if it does, false otherwise</returns>
        public virtual bool Exists(TKeyType key)
        {
            return InternalCache.ContainsKey(key);
        }

        /// <summary>
        ///     Adds an item to the cache
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public virtual void Add(TKeyType key, object value)
        {
            InternalCache.AddOrUpdate(key, value, (x, y) => value);
        }

        /// <summary>
        ///     Gets an item from the cache
        /// </summary>
        /// <typeparam name="TValueType">Item type</typeparam>
        /// <param name="key">Key to search for</param>
        /// <returns>The item associated with the key</returns>
        public virtual TValueType Get<TValueType>(TKeyType key)
        {
            object tempItem;
            return InternalCache.TryGetValue(key, out tempItem)
                       ? tempItem.TryTo(default(TValueType))
                       : default(TValueType);
        }

        #endregion
    }
}