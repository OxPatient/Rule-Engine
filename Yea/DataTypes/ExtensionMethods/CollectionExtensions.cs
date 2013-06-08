#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    /// <summary>
    ///     ICollection extensions
    /// </summary>
    public static class CollectionExtensions
    {
        #region Functions

        #region Add

        /// <summary>
        ///     Adds a list of items to the collection
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection</typeparam>
        /// <param name="collection">Collection</param>
        /// <param name="items">Items to add</param>
        /// <returns>The collection with the added items</returns>
        public static ICollection<T> Add<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            Guard.NotNull(collection, "collection");
            if (items.IsNull())
                return collection;
            items.ForEach(collection.Add);
            return collection;
        }

        /// <summary>
        ///     Adds a list of items to the collection
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection</typeparam>
        /// <param name="collection">Collection</param>
        /// <param name="items">Items to add</param>
        /// <returns>The collection with the added items</returns>
        public static ICollection<T> Add<T>(this ICollection<T> collection, params T[] items)
        {
            Guard.NotNull(collection, "collection");

            if (items.IsNull())
                return collection;
            items.ForEach(collection.Add);
            return collection;
        }

        #endregion

        #region AddAndReturn

        /// <summary>
        ///     Adds an item to a list and returns the item
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="collection">Collection to add to</param>
        /// <param name="item">Item to add to the collection</param>
        /// <returns>The original item</returns>
        public static T AddAndReturn<T>(this ICollection<T> collection, T item)
        {
            Guard.NotNull(collection, "collection");
            Guard.NotNull(item, "item");
            collection.Add(item);
            return item;
        }

        #endregion

        #region AddIf

        /// <summary>
        ///     Adds items to the collection if it passes the predicate test
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">Collection to add to</param>
        /// <param name="items">Items to add to the collection</param>
        /// <param name="predicate">Predicate that an item needs to satisfy in order to be added</param>
        /// <returns>True if any are added, false otherwise</returns>
        public static bool AddIf<T>(this ICollection<T> collection, Predicate<T> predicate, params T[] items)
        {
            Guard.NotNull(collection, "collection");
            Guard.NotNull(predicate, "predicate");
            bool returnValue = false;
            foreach (var item in items)
            {
                if (predicate(item))
                {
                    collection.Add(item);
                    returnValue = true;
                }
            }
            return returnValue;
        }

        /// <summary>
        ///     Adds an item to the collection if it isn't already in the collection
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">Collection to add to</param>
        /// <param name="items">Items to add to the collection</param>
        /// <param name="predicate">Predicate that an item needs to satisfy in order to be added</param>
        /// <returns>True if it is added, false otherwise</returns>
        public static bool AddIf<T>(this ICollection<T> collection, Predicate<T> predicate, IEnumerable<T> items)
        {
            Guard.NotNull(collection, "collection");
            Guard.NotNull(predicate, "predicate");
            return collection.AddIf(predicate, items.ToArray());
        }

        #endregion

        #region AddIfUnique

        /// <summary>
        ///     Adds an item to the collection if it isn't already in the collection
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">Collection to add to</param>
        /// <param name="items">Items to add to the collection</param>
        /// <returns>True if it is added, false otherwise</returns>
        public static bool AddIfUnique<T>(this ICollection<T> collection, params T[] items)
        {
            Guard.NotNull(collection, "collection");
            return collection.AddIf(x => !collection.Contains(x), items);
        }

        /// <summary>
        ///     Adds an item to the collection if it isn't already in the collection
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">Collection to add to</param>
        /// <param name="items">Items to add to the collection</param>
        /// <returns>True if it is added, false otherwise</returns>
        public static bool AddIfUnique<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            Guard.NotNull(collection, "collection");
            return collection.AddIf(x => !collection.Contains(x), items);
        }

        #endregion

        #region Remove

        /// <summary>
        ///     Removes all items that fit the predicate passed in
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection</typeparam>
        /// <param name="collection">Collection to remove items from</param>
        /// <param name="predicate">Predicate used to determine what items to remove</param>
        public static ICollection<T> Remove<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            Guard.NotNull(collection, "collection");
            return collection.Where(x => !predicate(x)).ToList();
        }

        /// <summary>
        ///     Removes all items in the list from the collection
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection</typeparam>
        /// <param name="collection">Collection</param>
        /// <param name="items">Items to remove</param>
        /// <returns>The collection with the items removed</returns>
        public static ICollection<T> Remove<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            Guard.NotNull(collection, "collection");
            if (items.IsNull())
                return collection;
            return collection.Where(x => !items.Contains(x)).ToList();
        }

        #endregion

        #endregion
    }
}