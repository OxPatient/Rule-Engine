#region Usings

using System;
using System.Web;
using System.Web.Caching;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.Caching
{
    /// <summary>
    ///     Extension methods relating to caching of data
    /// </summary>
    public static class CachingExtensions
    {
        /// <summary>
        ///     Caches an object to the specified cache, using the specified key
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="Object">Object to cache</param>
        /// <param name="type">Caching type</param>
        /// <param name="key">Key to cache the item under</param>
        public static void Cache<T>(this T Object, string key, CacheType type)
        {
            if (HttpContext.Current == null && !type.HasFlag(CacheType.Internal))
                return;

            if (HttpContext.Current != null && type.HasFlag(CacheType.Cache))
            {
                HttpContext.Current.Cache.Add(key, Object, null,
                                              System.Web.Caching.Cache.NoAbsoluteExpiration,
                                              System.Web.Caching.Cache.NoSlidingExpiration,
                                              CacheItemPriority.Normal, null);
            }
            else if (HttpContext.Current != null && type.HasFlag(CacheType.Item))
            {
                HttpContext.Current.Items[key] = Object;
            }
            else if (HttpContext.Current != null && type.HasFlag(CacheType.Session))
            {
                HttpContext.Current.Session[key] = Object;
            }
            else if (HttpContext.Current != null && type.HasFlag(CacheType.Cookie))
            {
                HttpContext.Current.Response.Cookies.Add(new HttpCookie(key, Object.ToString()));
            }
            else if (type.HasFlag(CacheType.Internal))
            {
                new Cache<string>().Add(key, Object);
            }
        }

        /// <summary>
        ///     Gets the specified object from the cache if it exists, otherwise the default value is returned
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="key">Key that the object is under</param>
        /// <param name="type">Cache types to search</param>
        /// <param name="defaultValue">Default value to return</param>
        /// <returns>The specified object if it exists, otherwise the default value</returns>
        public static T GetFromCache<T>(this string key, CacheType type, T defaultValue = default(T))
        {
            if (HttpContext.Current == null && !type.HasFlag(CacheType.Internal))
                return defaultValue;

            if (HttpContext.Current != null && type.HasFlag(CacheType.Cache))
            {
                return HttpContext.Current.Cache.Get(key).TryTo(defaultValue);
            }
            if (HttpContext.Current != null && type.HasFlag(CacheType.Item))
            {
                return HttpContext.Current.Items[key].TryTo(defaultValue);
            }
            if (HttpContext.Current != null && type.HasFlag(CacheType.Session))
            {
                return HttpContext.Current.Session[key].TryTo(defaultValue);
            }
            if (HttpContext.Current != null && type.HasFlag(CacheType.Cookie))
            {
                return HttpContext.Current.Response.Cookies[key].Value.TryTo(defaultValue);
            }
            if (type.HasFlag(CacheType.Internal))
            {
                return new Cache<string>().Get<T>(key);
            }
            return defaultValue;
        }
    }

    /// <summary>
    ///     Determines where an item is cached
    /// </summary>
    [Flags]
    public enum CacheType
    {
        /// <summary>
        ///     Cache (ASP.Net only)
        /// </summary>
        Cache = 1,

        /// <summary>
        ///     Item (ASP.Net only)
        /// </summary>
        Item = 2,

        /// <summary>
        ///     Session (ASP.Net only)
        /// </summary>
        Session = 4,

        /// <summary>
        ///     Cookie (ASP.Net only)
        /// </summary>
        Cookie = 8,

        /// <summary>
        ///     Internal caching
        /// </summary>
        Internal = 16
    }
}