#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.DataTypes
{
    /// <summary>
    ///     Used to count the number of times something is added to the list
    /// </summary>
    /// <typeparam name="T">Type of data within the bag</typeparam>
    public class Bag<T> : ICollection<T>
    {
        #region Constructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public Bag()
        {
            Items = new Dictionary<T, int>();
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        ///     Adds an item to the bag
        /// </summary>
        /// <param name="item">Item to add</param>
        public virtual void Add(T item)
        {
            if (Items.ContainsKey(item))
                ++Items[item];
            else
                Items.Add(item, 1);
        }

        /// <summary>
        ///     Clears the bag
        /// </summary>
        public virtual void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        ///     Determines if the bag contains an item
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <returns>True if it does, false otherwise</returns>
        public virtual bool Contains(T item)
        {
            return Items.ContainsKey(item);
        }

        /// <summary>
        ///     Copies the bag to an array
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="arrayIndex">Index to start at</param>
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(Items.ToList().ToArray(x => x.Key), 0, array, arrayIndex, Count);
        }

        /// <summary>
        ///     Number of items in the bag
        /// </summary>
        public virtual int Count
        {
            get { return Items.Count; }
        }

        /// <summary>
        ///     Is this read only?
        /// </summary>
        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///     Removes an item from the bag
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>True if it is removed, false otherwise</returns>
        public virtual bool Remove(T item)
        {
            return Items.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        ///     Gets the enumerator
        /// </summary>
        /// <returns>The enumerator</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) Items.Keys).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        ///     Gets the enumerator
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.Keys.GetEnumerator();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a specified item
        /// </summary>
        /// <param name="index">Item to get</param>
        /// <returns>The number of this item in the bag</returns>
        public virtual int this[T index]
        {
            get { return Items[index]; }
            set { Items[index] = value; }
        }

        /// <summary>
        ///     Actual internal container
        /// </summary>
        protected Dictionary<T, int> Items { get; private set; }

        #endregion
    }
}