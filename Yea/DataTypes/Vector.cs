#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Yea.DataTypes.EventArgs;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.DataTypes
{
    /// <summary>
    ///     Vector class
    /// </summary>
    /// <typeparam name="T">The type of item the vector should hold</typeparam>
    public class Vector<T> : IList<T>
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        public Vector()
        {
            DefaultSize = 2;
            Items = new T[DefaultSize];
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="InitialSize">Initial size of the vector</param>
        public Vector(int InitialSize)
        {
            if (InitialSize < 1) throw new ArgumentOutOfRangeException("InitialSize");
            DefaultSize = InitialSize;
            Items = new T[InitialSize];
        }

        #endregion

        #region IList<T> Members

        /// <summary>
        ///     Determines the index of an item
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <returns>The index that an item is at</returns>
        public virtual int IndexOf(T item)
        {
            return Array.IndexOf(Items, item, 0, NumberItems);
        }

        /// <summary>
        ///     Inserts an item into the vector
        /// </summary>
        /// <param name="index">Index to insert at</param>
        /// <param name="item">Item to insert</param>
        public virtual void Insert(int index, T item)
        {
            if (index > NumberItems || index < 0) throw new ArgumentOutOfRangeException("index");

            if (NumberItems == Items.Length)
                Array.Resize(ref Items, Items.Length*2);
            if (index < NumberItems)
                Array.Copy(Items, index, Items, index + 1, NumberItems - index);
            Items[index] = item;
            ++NumberItems;
            Changed.Raise(this, new ChangedEventArgs());
        }

        /// <summary>
        ///     Removes an item from the vector
        /// </summary>
        /// <param name="index">Index at which the item is removed</param>
        public virtual void RemoveAt(int index)
        {
            if (index > NumberItems || index < 0) throw new ArgumentOutOfRangeException("index");

            if (index < NumberItems)
                Array.Copy(Items, index + 1, Items, index, NumberItems - (index + 1));
            Items[NumberItems - 1] = default(T);
            --NumberItems;
            Changed.Raise(this, new ChangedEventArgs());
        }

        /// <summary>
        ///     Gets an item at the specified index
        /// </summary>
        /// <param name="index">Index to get</param>
        /// <returns>The specified item</returns>
        public virtual T this[int index]
        {
            get
            {
                if (index > NumberItems || index < 0) throw new ArgumentOutOfRangeException("index");
                return Items[index];
            }
            set
            {
                if (index > NumberItems || index < 0) throw new ArgumentOutOfRangeException("index");
                Items[index] = value;
                Changed.Raise(this, new ChangedEventArgs());
            }
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        ///     Adds an item to the vector
        /// </summary>
        /// <param name="item">Item to add</param>
        public virtual void Add(T item)
        {
            Insert(NumberItems, item);
        }

        /// <summary>
        ///     Clears the vector
        /// </summary>
        public virtual void Clear()
        {
            Array.Clear(Items, 0, Items.Length);
            NumberItems = 0;
            Changed.Raise(this, new ChangedEventArgs());
        }

        /// <summary>
        ///     Determines if the vector contains an item
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public virtual bool Contains(T item)
        {
            return (IndexOf(item) >= 0);
        }

        /// <summary>
        ///     Copies the vector to an array
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="arrayIndex">Index to start at</param>
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(Items, 0, array, arrayIndex, NumberItems);
        }

        /// <summary>
        ///     Number of items in the vector
        /// </summary>
        public virtual int Count
        {
            get { return NumberItems; }
        }

        /// <summary>
        ///     Is this read only?
        /// </summary>
        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///     Removes an item from the vector
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>True if it is removed, false otherwise</returns>
        public virtual bool Remove(T item)
        {
            int Index = IndexOf(item);
            if (Index >= 0)
            {
                RemoveAt(Index);
                return true;
            }
            return false;
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        ///     Gets the enumerator
        /// </summary>
        /// <returns>The enumerator</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            for (int x = 0; x < NumberItems; ++x)
                yield return Items[x];
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        ///     Gets the enumerator
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int x = 0; x < NumberItems; ++x)
                yield return Items[x];
        }

        #endregion

        #region Protected Variables/Properties

        /// <summary>
        ///     Internal list of items
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")] protected T[] Items = null;

        /// <summary>
        ///     Default size
        /// </summary>
        protected virtual int DefaultSize { get; set; }

        /// <summary>
        ///     Number of items in the list
        /// </summary>
        protected virtual int NumberItems { get; set; }

        #endregion

        #region Events

        /// <summary>
        ///     Event that is fired when the vector is changed
        /// </summary>
        public virtual EventHandler<ChangedEventArgs> Changed { get; set; }

        #endregion
    }
}