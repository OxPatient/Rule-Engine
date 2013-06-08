#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Yea.DataTypes.Comparison;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    /// <summary>
    ///     IDictionary extensions
    /// </summary>
    public static class DictionaryExtensions
    {
        #region Functions

        #region GetValue

        /// <summary>
        ///     Gets the value from a dictionary or the default value if it isn't found
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary">Dictionary to get the value from</param>
        /// <param name="key">Key to look for</param>
        /// <param name="Default">Default value if the key is not found</param>
        /// <returns>The value associated with the key or the default value if the key is not found</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if the dictionary is null</exception>
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
                                                    TValue Default = default(TValue))
        {
            Guard.NotNull(dictionary, "dictionary");
            TValue returnValue;
            return dictionary.TryGetValue(key, out returnValue) ? returnValue : Default;
        }

        #endregion

        #region SetValue

        /// <summary>
        ///     Sets the value in a dictionary
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary">Dictionary to set the value in</param>
        /// <param name="key">Key to look for</param>
        /// <param name="value">Value to add</param>
        /// <returns>The dictionary</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if the dictionary is null</exception>
        public static IDictionary<TKey, TValue> SetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                                       TKey key, TValue value)
        {
            Guard.NotNull(dictionary, "dictionary");
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
            return dictionary;
        }

        #endregion

        #region Sort

        /// <summary>
        ///     Sorts a dictionary
        /// </summary>
        /// <typeparam name="T1">Key type</typeparam>
        /// <typeparam name="T2">Value type</typeparam>
        /// <param name="dictionary">Dictionary to sort</param>
        /// <param name="comparer">Comparer used to sort (defaults to GenericComparer)</param>
        /// <returns>The sorted dictionary</returns>
        public static IDictionary<T1, T2> Sort<T1, T2>(this IDictionary<T1, T2> dictionary,
                                                       IComparer<T1> comparer = null)
            where T1 : IComparable
        {
            Guard.NotNull(dictionary, "dictionary");
            return dictionary.Sort(x => x.Key, comparer);
        }

        /// <summary>
        ///     Sorts a dictionary
        /// </summary>
        /// <typeparam name="T1">Key type</typeparam>
        /// <typeparam name="T2">Value type</typeparam>
        /// <typeparam name="T3">Order by type</typeparam>
        /// <param name="dictionary">Dictionary to sort</param>
        /// <param name="orderBy">Function used to order the dictionary</param>
        /// <param name="comparer">Comparer used to sort (defaults to GenericComparer)</param>
        /// <returns>The sorted dictionary</returns>
        public static IDictionary<T1, T2> Sort<T1, T2, T3>(this IDictionary<T1, T2> dictionary,
                                                           Func<KeyValuePair<T1, T2>, T3> orderBy,
                                                           IComparer<T3> comparer = null)
            where T3 : IComparable
        {
            Guard.NotNull(dictionary, "dictionary");
            Guard.NotNull(orderBy, "orderBy");
            return
                dictionary.OrderBy(orderBy, comparer.NullCheck(() => new GenericComparer<T3>()))
                          .ToDictionary(x => x.Key, x => x.Value);
        }

        #endregion

        #endregion
    }
}