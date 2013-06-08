namespace Yea.Caching
{
    /// <summary>
    ///     Cache item interface
    /// </summary>
    public interface ICacheItem<TKeyType>
    {
        #region Properties

        /// <summary>
        ///     Key of the cache item
        /// </summary>
        TKeyType Key { get; set; }

        #endregion
    }

    /// <summary>
    ///     Cache item interface
    /// </summary>
    public interface ICacheItem
    {
        #region Properties

        /// <summary>
        ///     Value (not strongly typed)
        /// </summary>
        object Value { get; set; }

        #endregion
    }
}