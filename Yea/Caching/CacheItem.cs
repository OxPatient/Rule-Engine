namespace Yea.Caching
{
    /// <summary>
    ///     Cache item
    /// </summary>
    /// <typeparam name="TKeyType">Key type</typeparam>
    public class CacheItem<TKeyType> : ICacheItem, ICacheItem<TKeyType>
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public CacheItem(TKeyType key, object value)
        {
            Key = key;
            Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Value stored in the cache item
        /// </summary>
        public virtual object Value { get; set; }

        /// <summary>
        ///     Key associated with the cache item
        /// </summary>
        public virtual TKeyType Key { get; set; }

        #endregion
    }
}