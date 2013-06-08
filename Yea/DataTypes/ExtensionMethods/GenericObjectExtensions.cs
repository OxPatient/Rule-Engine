#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Yea.DataTypes.Comparison;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    /// <summary>
    ///     Generic extensions dealing with objects
    /// </summary>
    public static class GenericObjectExtensions
    {
        #region IsNotDefault

        /// <summary>
        ///     Determines if the object is not null
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="Object">The object to check</param>
        /// <param name="equalityComparer">Equality comparer used to determine if the object is equal to default</param>
        /// <returns>False if it is null, true otherwise</returns>
        public static bool IsNotDefault<T>(this T Object, IEqualityComparer<T> equalityComparer = null)
        {
            return !Object.IsDefault(equalityComparer);
        }

        #endregion

        #region IsDefault

        /// <summary>
        ///     Determines if the object is null
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="Object">The object to check</param>
        /// <param name="equalityComparer">Equality comparer used to determine if the object is equal to default</param>
        /// <returns>True if it is null, false otherwise</returns>
        public static bool IsDefault<T>(this T Object, IEqualityComparer<T> equalityComparer = null)
        {
            return equalityComparer.NullCheck(() => new GenericEqualityComparer<T>()).Equals(Object, default(T));
        }

        #endregion

        #region IsNotNull

        /// <summary>
        ///     Determines if the object is not null
        /// </summary>
        /// <param name="Object">The object to check</param>
        /// <returns>False if it is null, true otherwise</returns>
        public static bool IsNotNull(this object Object)
        {
            return !Object.IsNull();
        }

        #endregion

        #region IsNull

        /// <summary>
        ///     Determines if the object is null
        /// </summary>
        /// <param name="Object">The object to check</param>
        /// <returns>True if it is null, false otherwise</returns>
        public static bool IsNull(this object Object)
        {
            return Object == null || Convert.IsDBNull(Object);
        }

        #endregion

        #region IsNotNullOrEmpty

        /// <summary>
        ///     Determines if a list is not null or empty
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">List to check</param>
        /// <returns>True if it is not null or empty, false otherwise</returns>
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> value)
        {
            return !value.IsNullOrEmpty();
        }

        #endregion

        #region IsNullOrEmpty

        /// <summary>
        ///     Determines if a list is null or empty
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">List to check</param>
        /// <returns>True if it is null or empty, false otherwise</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
        {
            return value.IsNull() || !value.Any();
        }

        #endregion

        #region If

        /// <summary>
        ///     Determines if the object fullfills the predicate and if it does, returns itself. Otherwise the default value.
        ///     If the predicate is null, it returns the default value.
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="Object">Object to check</param>
        /// <param name="predicate">Predicate to run on the object</param>
        /// <param name="defaultValue">Default value to return if it does not succeed the predicate test</param>
        /// <returns>The original value if predicate is true, the default value otherwise</returns>
        public static T If<T>(this T Object, Predicate<T> predicate, T defaultValue = default(T))
        {
            if (predicate.IsNull())
                return defaultValue;
            return predicate(Object) ? Object : defaultValue;
        }

        #endregion

        #region NotIf

        /// <summary>
        ///     Determines if the object fails the predicate and if it does, returns itself. Otherwise the default value.
        ///     If the predicate is null, it returns the default value.
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="Object">Object to check</param>
        /// <param name="predicate">Predicate to run on the object</param>
        /// <param name="defaultValue">Default value to return if it succeeds the predicate test</param>
        /// <returns>The original value if predicate is false, the default value otherwise</returns>
        public static T NotIf<T>(this T Object, Predicate<T> predicate, T defaultValue = default(T))
        {
            if (predicate.IsNull())
                return defaultValue;
            return predicate(Object) ? defaultValue : Object;
        }

        #endregion

        #region NullCheck

        /// <summary>
        ///     Does a null check and either returns the default value (if it is null) or the object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="Object">Object to check</param>
        /// <param name="defaultValue">The default value in case it is null</param>
        /// <returns>The default value if it is null, the object otherwise</returns>
        public static T NullCheck<T>(this T Object, T defaultValue = default(T))
        {
            return Object.IsNull() ? defaultValue : Object;
        }

        /// <summary>
        ///     Does a null check and either returns the default value (if it is null) or the object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="Object">Object to check</param>
        /// <param name="defaultValue">Function that returns the default value in case it is null</param>
        /// <returns>The default value if it is null, the object otherwise</returns>
        public static T NullCheck<T>(this T Object, Func<T> defaultValue)
        {
            return Object.IsNull() ? defaultValue() : Object;
        }

        #endregion

        #region Times

        /// <summary>
        ///     Runs a function based on the number of times specified and returns the results
        /// </summary>
        /// <typeparam name="T">Type that gets returned</typeparam>
        /// <param name="count">Number of times the function should run</param>
        /// <param name="function">The function that should run</param>
        /// <returns>The results from the function</returns>
        public static IEnumerable<T> Times<T>(this int count, Func<int, T> function)
        {
            var returnValue = new System.Collections.Generic.List<T>();
            for (int x = 0; x < count; ++x)
                returnValue.Add(function(x));
            return returnValue;
        }

        /// <summary>
        ///     Runs an action based on the number of times specified
        /// </summary>
        /// <param name="count">Number of times to run the action</param>
        /// <param name="action">Action to run</param>
        public static void Times(this int count, Action<int> action)
        {
            for (int x = 0; x < count; ++x)
                action(x);
        }

        #endregion

        /// <summary>
        ///     判断对象是否与指定集合中的任一对象相等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool EqualsAny<T>(this T obj, params T[] values)
        {
            return (Array.IndexOf(values, obj) != -1);
        }

        /// <summary>
        ///     判断对象是否与指定集合中的所有对象都不相等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool EqualsNone<T>(this T obj, params T[] values)
        {
            return (obj.EqualsAny(values) == false);
        }
    }
}