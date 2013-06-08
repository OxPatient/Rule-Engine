#region Usings

using System;
using System.Collections.Generic;
using Yea.DataTypes.EventArgs;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.DataTypes
{
    /// <summary>
    ///     Class designed to replace List. Contains events so that we can tell
    ///     when the list has been changed.
    /// </summary>
    public class List<T> : System.Collections.Generic.List<T>
    {
        #region Events

        /// <summary>
        ///     Event called if the list is changed
        /// </summary>
        public virtual EventHandler<ChangedEventArgs> Changed { get; set; }

        #endregion

        #region Public Functions

        /// <summary>
        ///     Adds a new item to the list
        /// </summary>
        /// <param name="value">Value to add</param>
        public new void Add(T value)
        {
            base.Add(value);
            var TempArgs = new ChangedEventArgs();
            TempArgs.Content = PropertyName;
            Changed.Raise(this, TempArgs);
        }

        /// <summary>
        ///     Adds a range of items to the list
        /// </summary>
        /// <param name="value">Items to add</param>
        public new void AddRange(IEnumerable<T> value)
        {
            base.AddRange(value);
            var TempArgs = new ChangedEventArgs();
            TempArgs.Content = PropertyName;
            Changed.Raise(this, TempArgs);
        }

        /// <summary>
        ///     Removes an item from the list
        /// </summary>
        /// <param name="obj">Object to remove</param>
        /// <returns>True if it is removed, false otherwise</returns>
        public new bool Remove(T obj)
        {
            bool ReturnValue = base.Remove(obj);
            var TempArgs = new ChangedEventArgs();
            TempArgs.Content = PropertyName;
            Changed.Raise(this, TempArgs);
            return ReturnValue;
        }

        /// <summary>
        ///     Removes an item at a specific index
        /// </summary>
        /// <param name="index">Index to remove an item at</param>
        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            var TempArgs = new ChangedEventArgs();
            TempArgs.Content = PropertyName;
            Changed.Raise(this, TempArgs);
        }

        /// <summary>
        ///     Removes all items that match the predicate
        /// </summary>
        /// <param name="match">Predicate to check each item against</param>
        /// <returns>The number of items removed</returns>
        public new int RemoveAll(Predicate<T> match)
        {
            int ReturnValue = base.RemoveAll(match);
            var TempArgs = new ChangedEventArgs();
            TempArgs.Content = PropertyName;
            Changed.Raise(this, TempArgs);
            return ReturnValue;
        }

        /// <summary>
        ///     Removes a range of items
        /// </summary>
        /// <param name="index">Index to start at</param>
        /// <param name="count">Number of items to remove</param>
        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            var TempArgs = new ChangedEventArgs();
            TempArgs.Content = PropertyName;
            Changed.Raise(this, TempArgs);
        }

        /// <summary>
        ///     Inserts an item at a specified index
        /// </summary>
        /// <param name="index">Index to insert at</param>
        /// <param name="value">Value to insert</param>
        public new void Insert(int index, T value)
        {
            base.Insert(index, value);
            var TempArgs = new ChangedEventArgs();
            TempArgs.Content = PropertyName;
            Changed.Raise(this, TempArgs);
        }

        /// <summary>
        ///     Inserts a range of items at a specified index
        /// </summary>
        /// <param name="index">Index to start at</param>
        /// <param name="collection">List of items to insert</param>
        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            var TempArgs = new ChangedEventArgs();
            TempArgs.Content = PropertyName;
            Changed.Raise(this, TempArgs);
        }

        /// <summary>
        ///     Clears the list
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            var TempArgs = new ChangedEventArgs();
            TempArgs.Content = PropertyName;
            Changed.Raise(this, TempArgs);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets an item at a specified index
        /// </summary>
        /// <param name="index">Index of the item to get</param>
        /// <returns>The specified item</returns>
        public new T this[int index]
        {
            get { return base[index]; }
            set
            {
                base[index] = value;
                Changed.Raise(this, new ChangedEventArgs());
            }
        }

        /// <summary>
        ///     If set, it will set the Content property of the event args with this value
        /// </summary>
        public virtual string PropertyName { get; set; }

        #endregion
    }
}