#region Usings

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yea.DataTypes.Comparison;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    /// <summary>
    ///     IEnumerable extensions
    /// </summary>
    public static class EnumerableExtensions
    {
        #region Functions

        /// <summary>
        ///     Returns elements starting at the index and ending at the end index
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="list">List to search</param>
        /// <param name="start">Start index (inclusive)</param>
        /// <param name="end">End index (exclusive)</param>
        /// <returns>The items between the start and end index</returns>
        public static IEnumerable<T> ElementsBetween<T>(this IEnumerable<T> list, int start, int end)
        {
            if (list.IsNull())
                return list;
            if (end > list.Count())
                end = list.Count();
            if (start < 0)
                start = 0;
            var returnList = new System.Collections.Generic.List<T>();
            for (int x = start; x < end; ++x)
                returnList.Add(list.ElementAt(x));
            return returnList;
        }

        #region FalseForAll

        /// <summary>
        ///     Determines if the predicates are false for each item in a list
        /// </summary>
        /// <typeparam name="T">The type of the items in the list</typeparam>
        /// <param name="list">IEnumerable to look through</param>
        /// <param name="predicates">Predicates to use to check the IEnumerable</param>
        /// <returns>True if they all fail all of the predicates, false otherwise</returns>
        public static bool FalseForAll<T>(this IEnumerable<T> list, params Predicate<T>[] predicates)
        {
            Guard.NotNull(list, "list");
            Guard.NotNull(predicates, "predicates");
            return predicates.All(predicate => !list.All(x => predicate(x)));
        }

        #endregion

        #region FalseForAny

        /// <summary>
        ///     Determines if the predicates are false for any item in a list
        /// </summary>
        /// <typeparam name="T">The type of the items in the list</typeparam>
        /// <param name="list">IEnumerable to look through</param>
        /// <param name="predicates">Predicates to use to check the IEnumerable</param>
        /// <returns>True if any fail any of the predicates, false otherwise</returns>
        public static bool FalseForAny<T>(this IEnumerable<T> list, params Predicate<T>[] predicates)
        {
            Guard.NotNull(list, "list");
            Guard.NotNull(predicates, "predicates");
            return predicates.Any(predicate => list.Any(x => !predicate(x)));
        }

        #endregion

        #region First

        /// <summary>
        ///     Returns the first X number of items from the list
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="list">IEnumerable to iterate over</param>
        /// <param name="count">Numbers of items to return</param>
        /// <returns>The first X items from the list</returns>
        public static IEnumerable<T> First<T>(this IEnumerable<T> list, int count)
        {
            Guard.NotNull(list, "list");
            return list.ElementsBetween(0, count);
        }

        #endregion

        #region For

        /// <summary>
        ///     Does an action for each item in the IEnumerable between the start and end indexes
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="list">IEnumerable to iterate over</param>
        /// <param name="start">Item to start with</param>
        /// <param name="end">Item to end with</param>
        /// <param name="action">Action to do</param>
        /// <returns>The original list</returns>
        public static IEnumerable<T> For<T>(this IEnumerable<T> list, int start, int end, Action<T> action)
        {
            Guard.NotNull(list, "list");
            Guard.NotNull(action, "action");
            foreach (var item in list.ElementsBetween(start, end + 1))
                action(item);
            return list;
        }

        /// <summary>
        ///     Does a function for each item in the IEnumerable between the start and end indexes and returns an IEnumerable of the results
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <typeparam name="TR">Return type</typeparam>
        /// <param name="list">IEnumerable to iterate over</param>
        /// <param name="start">Item to start with</param>
        /// <param name="end">Item to end with</param>
        /// <param name="function">Function to do</param>
        /// <returns>The resulting list</returns>
        public static IEnumerable<TR> For<T, TR>(this IEnumerable<T> list, int start, int end, Func<T, TR> function)
        {
            Guard.NotNull(list, "list");
            Guard.NotNull(function, "function");
            var returnValues = new List<TR>();
            foreach (var item in list.ElementsBetween(start, end + 1))
                returnValues.Add(function(item));
            return returnValues;
        }

        #endregion

        #region ForEach

        /// <summary>
        ///     Does an action for each item in the IEnumerable
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="list">IEnumerable to iterate over</param>
        /// <param name="action">Action to do</param>
        /// <returns>The original list</returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            Guard.NotNull(list, "list");
            Guard.NotNull(action, "action");
            foreach (var item in list)
                action(item);
            return list;
        }

        /// <summary>
        ///     Does a function for each item in the IEnumerable, returning a list of the results
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <typeparam name="TR">Return type</typeparam>
        /// <param name="list">IEnumerable to iterate over</param>
        /// <param name="function">Function to do</param>
        /// <returns>The resulting list</returns>
        public static IEnumerable<TR> ForEach<T, TR>(this IEnumerable<T> list, Func<T, TR> function)
        {
            Guard.NotNull(list, "list");
            Guard.NotNull(function, "function");
            var returnValues = new List<TR>();
            foreach (var item in list)
                returnValues.Add(function(item));
            return returnValues;
        }

        #endregion

        #region ForParallel

        /// <summary>
        ///     Does an action for each item in the IEnumerable between the start and end indexes in parallel
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="list">IEnumerable to iterate over</param>
        /// <param name="start">Item to start with</param>
        /// <param name="end">Item to end with</param>
        /// <param name="action">Action to do</param>
        /// <returns>The original list</returns>
        public static IEnumerable<T> ForParallel<T>(this IEnumerable<T> list, int start, int end, Action<T> action)
        {
            Guard.NotNull(list, "list");
            Guard.NotNull(action, "action");
            Parallel.For(start, end + 1, x => action(list.ElementAt(x)));
            return list;
        }

        /// <summary>
        ///     Does an action for each item in the IEnumerable between the start and end indexes in parallel
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <typeparam name="TR">Results type</typeparam>
        /// <param name="list">IEnumerable to iterate over</param>
        /// <param name="start">Item to start with</param>
        /// <param name="end">Item to end with</param>
        /// <param name="function">Function to do</param>
        /// <returns>The resulting list</returns>
        public static IEnumerable<TR> ForParallel<T, TR>(this IEnumerable<T> list, int start, int end,
                                                         Func<T, TR> function)
        {
            Guard.NotNull(list, "list");
            Guard.NotNull(function, "function");
            var results = new TR[(end + 1) - start];
            Parallel.For(start, end + 1, x => results[x - start] = function(list.ElementAt(x)));
            return results;
        }

        #endregion

        #region ForEachParallel

        /// <summary>
        ///     Does an action for each item in the IEnumerable in parallel
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="list">IEnumerable to iterate over</param>
        /// <param name="action">Action to do</param>
        /// <returns>The original list</returns>
        public static IEnumerable<T> ForEachParallel<T>(this IEnumerable<T> list, Action<T> action)
        {
            Guard.NotNull(list, "list");
            Guard.NotNull(action, "action");
            Parallel.ForEach(list, action);
            return list;
        }

        /// <summary>
        ///     Does an action for each item in the IEnumerable in parallel
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <typeparam name="TR">Results type</typeparam>
        /// <param name="list">IEnumerable to iterate over</param>
        /// <param name="function">Function to do</param>
        /// <returns>The results in an IEnumerable list</returns>
        public static IEnumerable<TR> ForEachParallel<T, TR>(this IEnumerable<T> list, Func<T, TR> function)
        {
            Guard.NotNull(list, "list");
            Guard.NotNull(function, "function");
            return list.ForParallel(0, list.Count() - 1, function);
        }

        #endregion

        #region Last

        /// <summary>
        ///     Returns the last X number of items from the list
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="list">IEnumerable to iterate over</param>
        /// <param name="count">Numbers of items to return</param>
        /// <returns>The last X items from the list</returns>
        public static IEnumerable<T> Last<T>(this IEnumerable<T> list, int count)
        {
            Guard.NotNull(list, "list");
            return list.ElementsBetween(list.Count() - count, list.Count());
        }

        #endregion

        #region PositionOf

        /// <summary>
        ///     Determines the position of an object if it is present, otherwise it returns -1
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="list">List of objects to search</param>
        /// <param name="Object">Object to find the position of</param>
        /// <param name="equalityComparer">Equality comparer used to determine if the object is present</param>
        /// <returns>The position of the object if it is present, otherwise -1</returns>
        public static int PositionOf<T>(this IEnumerable<T> list, T Object, IEqualityComparer<T> equalityComparer = null)
        {
            Guard.NotNull(list, "list");
            equalityComparer = equalityComparer.NullCheck(() => new GenericEqualityComparer<T>());
            int count = 0;
            foreach (var item in list)
            {
                if (equalityComparer.Equals(Object, item))
                    return count;
                ++count;
            }
            return -1;
        }

        #endregion

        #region RemoveDefaults

        /// <summary>
        ///     Removes default values from a list
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="value">List to cull items from</param>
        /// <param name="equalityComparer">Equality comparer used (defaults to GenericEqualityComparer)</param>
        /// <returns>An IEnumerable with the default values removed</returns>
        public static IEnumerable<T> RemoveDefaults<T>(this IEnumerable<T> value,
                                                       IEqualityComparer<T> equalityComparer = null)
        {
            if (value.IsNull())
                return value;
            equalityComparer = equalityComparer.NullCheck(() => new GenericEqualityComparer<T>());
            return value.Where(x => !x.IsDefault(equalityComparer));
        }

        #endregion

        #region ToArray

        /// <summary>
        ///     Converts a list to an array
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TTarget">Target type</typeparam>
        /// <param name="list">List to convert</param>
        /// <param name="convertingFunction">Function used to convert each item</param>
        /// <returns>The array containing the items from the list</returns>
        public static TTarget[] ToArray<TSource, TTarget>(this IEnumerable<TSource> list,
                                                          Func<TSource, TTarget> convertingFunction)
        {
            Guard.NotNull(list, "list");
            Guard.NotNull(convertingFunction, "convertingFunction");
            return list.ForEach(convertingFunction).ToArray();
        }

        #endregion

        #region ToDataTable

        /// <summary>
        ///     Converts the IEnumerable to a DataTable
        /// </summary>
        /// <typeparam name="T">Type of the objects in the IEnumerable</typeparam>
        /// <param name="list">List to convert</param>
        /// <param name="columns">Column names (if empty, uses property names)</param>
        /// <returns>The list as a DataTable</returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> list, params string[] columns)
        {
            var returnValue = new DataTable {Locale = CultureInfo.CurrentCulture};
            if (list.IsNullOrEmpty())
                return returnValue;
            PropertyInfo[] properties = typeof (T).GetProperties();
            if (columns.Length == 0)
                columns = properties.ToArray(x => x.Name);
            columns.ForEach(x => returnValue.Columns.Add(x, properties.FirstOrDefault(z => z.Name == x).PropertyType));
            var row = new object[columns.Length];
            foreach (var item in list)
            {
                for (int x = 0; x < row.Length; ++x)
                {
                    row[x] = properties.FirstOrDefault(z => z.Name == columns[x]).GetValue(item, new object[] {});
                }
                returnValue.Rows.Add(row);
            }
            return returnValue;
        }

        #endregion

        #region ToString

        /// <summary>
        ///     Converts the list to a string where each item is seperated by the Seperator
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="list">List to convert</param>
        /// <param name="itemOutput">Used to convert the item to a string (defaults to calling ToString)</param>
        /// <param name="seperator">Seperator to use between items (defaults to ,)</param>
        /// <returns>The string version of the list</returns>
        public static string ToString<T>(this IEnumerable<T> list, Func<T, string> itemOutput = null,
                                         string seperator = ",")
        {
            Guard.NotNull(list, "list");
            seperator = seperator.NullCheck("");
            itemOutput = itemOutput.NullCheck(x => x.ToString());
            var builder = new StringBuilder();
            string tempSeperator = "";
            list.ForEach(x =>
                {
                    builder.Append(tempSeperator).Append(itemOutput(x));
                    tempSeperator = seperator;
                });
            return builder.ToString();
        }

        #endregion

        #region TrueForAll

        /// <summary>
        ///     Determines if the predicates are true for each item in a list
        /// </summary>
        /// <typeparam name="T">The type of the items in the list</typeparam>
        /// <param name="list">IEnumerable to look through</param>
        /// <param name="predicates">Predicates to use to check the IEnumerable</param>
        /// <returns>True if they all pass all of the predicates, false otherwise</returns>
        public static bool TrueForAll<T>(this IEnumerable<T> list, params Predicate<T>[] predicates)
        {
            Guard.NotNull(list, "list");
            Guard.NotNull(predicates, "predicates");
            return predicates.All(predicate => !list.Any(x => !predicate(x)));
        }

        #endregion

        #region TrueForAny

        /// <summary>
        ///     Determines if the predicates are true for any item in a list
        /// </summary>
        /// <typeparam name="T">The type of the items in the list</typeparam>
        /// <param name="list">IEnumerable to look through</param>
        /// <param name="predicates">Predicates to use to check the IEnumerable</param>
        /// <returns>True if any pass any of the predicates, false otherwise</returns>
        public static bool TrueForAny<T>(this IEnumerable<T> list, params Predicate<T>[] predicates)
        {
            Guard.NotNull(list, "list");
            Guard.NotNull(predicates, "predicates");
            return predicates.Any(predicate => list.Any(x => predicate(x)));
        }

        #endregion

        #region TryAll

        /// <summary>
        ///     Tries to do the action on each item in the list. If an exception is thrown,
        ///     it does the catch action on the item (if it is not null).
        /// </summary>
        /// <typeparam name="T">The type of the items in the list</typeparam>
        /// <param name="list">IEnumerable to look through</param>
        /// <param name="action">Action to run on each item</param>
        /// <param name="catchAction">Catch action (defaults to null)</param>
        /// <returns>The list after the action is run on everything</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static IEnumerable<T> TryAll<T>(this IEnumerable<T> list, Action<T> action, Action<T> catchAction = null)
        {
            Guard.NotNull(list, "list");
            Guard.NotNull(action, "action");
            foreach (var item in list)
            {
                try
                {
                    action(item);
                }
                catch
                {
                    if (catchAction != null) catchAction(item);
                }
            }
            return list;
        }

        #endregion

        #region TryAllParallel

        /// <summary>
        ///     Tries to do the action on each item in the list. If an exception is thrown,
        ///     it does the catch action on the item (if it is not null). This is done in
        ///     parallel.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list</typeparam>
        /// <param name="list">IEnumerable to look through</param>
        /// <param name="action">Action to run on each item</param>
        /// <param name="catchAction">Catch action (defaults to null)</param>
        /// <returns>The list after the action is run on everything</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static IEnumerable<T> TryAllParallel<T>(this IEnumerable<T> list, Action<T> action,
                                                       Action<T> catchAction = null)
        {
            Guard.NotNull(list, "list");
            Guard.NotNull(action, "action");
            Parallel.ForEach(list, delegate(T item)
                {
                    try
                    {
                        action(item);
                    }
                    catch
                    {
                        if (catchAction != null) catchAction(item);
                    }
                });
            return list;
        }

        #endregion

        /*#region ThrowIfTrueForAll

        /// <summary>
        /// Throws the specified exception if the predicates are true for all items
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <typeparam name="TE">Exception type</typeparam>
        /// <param name="items">The list</param>
        /// <param name="predicates">Predicates to check</param>
        /// <param name="exception">Exception to throw if predicates are true</param>
        /// <returns>the original IEnumerable</returns>
        public static IEnumerable<T> ThrowIfTrueForAll<T, TE>(this IEnumerable<T> items, TE exception, params Predicate<T>[] predicates) where TE : Exception
        {
            Guard.NotNull(predicates, "predicates");
            Guard.NotNull(exception, "exception");
            if (items.TrueForAll(predicates))
                throw exception;
            return items;
        }

        #endregion

        #region ThrowIfFalseForAll

        /// <summary>
        /// Throws the specified exception if the predicates are false for all items
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <typeparam name="E">Exception type</typeparam>
        /// <param name="Items">The list</param>
        /// <param name="Predicates">Predicates to check</param>
        /// <param name="Exception">Exception to throw if predicates are false</param>
        /// <returns>the original list</returns>
        public static IEnumerable<T> ThrowIfFalseForAll<T, E>(this IEnumerable<T> Items, E Exception, params Predicate<T>[] Predicates) where E : Exception
        {
            Predicates.ThrowIfNull("Predicates");
            Exception.ThrowIfNull("Exception");
            if (Items.FalseForAll(Predicates))
                throw Exception;
            return Items;
        }

        #endregion

        #region ThrowIfTrueForAny

        /// <summary>
        /// Throws the specified exception if the predicate is true for any items
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <typeparam name="E">Exception type</typeparam>
        /// <param name="Items">The list</param>
        /// <param name="Predicates">Predicates to check</param>
        /// <param name="Exception">Exception to throw if predicate is true</param>
        /// <returns>the original IEnumerable</returns>
        public static IEnumerable<T> ThrowIfTrueForAny<T, E>(this IEnumerable<T> Items, E Exception, params Predicate<T>[] Predicates) where E : Exception
        {
            Predicates.ThrowIfNull("Predicates");
            Exception.ThrowIfNull("Exception");
            if (Items.TrueForAny(Predicates))
                throw Exception;
            return Items;
        }

        #endregion

        #region ThrowIfFalseForAny

        /// <summary>
        /// Throws the specified exception if the predicates are false for any items
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <typeparam name="E">Exception type</typeparam>
        /// <param name="Items">The list</param>
        /// <param name="Predicates">Predicates to check</param>
        /// <param name="Exception">Exception to throw if predicates are false</param>
        /// <returns>the original list</returns>
        public static IEnumerable<T> ThrowIfFalseForAny<T, E>(this IEnumerable<T> Items, E Exception, params Predicate<T>[] Predicates) where E : Exception
        {
            Predicates.ThrowIfNull("Predicates");
            Exception.ThrowIfNull("Exception");
            if (Items.FalseForAny(Predicates))
                throw Exception;
            return Items;
        }

        #endregion*/

        #endregion
    }
}