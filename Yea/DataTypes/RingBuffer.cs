#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Yea.DataTypes.Comparison;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.DataTypes
{
    /// <summary>
    ///     Implements a ring buffer
    /// </summary>
    /// <typeparam name="T">Type of the data it holds</typeparam>
    public class RingBuffer<T> : ICollection<T>, ICollection
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        public RingBuffer()
            : this(10, false)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="MaxCapacity">Max capacity for the circular buffer</param>
        /// <param name="AllowOverflow">Is overflow allowed (defaults to false)</param>
        public RingBuffer(int MaxCapacity, bool AllowOverflow = false)
        {
            Count = 0;
            IsReadOnly = false;
            this.AllowOverflow = AllowOverflow;
            this.MaxCapacity = MaxCapacity;
            IsSynchronized = false;
            ReadPosition = 0;
            WritePosition = 0;
            Buffer = new T[MaxCapacity];
        }

        #endregion

        #region Properties

        private object Root;

        /// <summary>
        ///     Is overflow allowed?
        /// </summary>
        public bool AllowOverflow { get; protected set; }

        /// <summary>
        ///     Maximum capacity
        /// </summary>
        public int MaxCapacity { get; protected set; }

        /// <summary>
        ///     Buffer that the circular buffer uses
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        protected T[] Buffer { get; set; }

        /// <summary>
        ///     Read position
        /// </summary>
        protected int ReadPosition { get; set; }

        /// <summary>
        ///     Write position
        /// </summary>
        protected int WritePosition { get; set; }

        /// <summary>
        ///     Allows getting an item at a specific position in the buffer
        /// </summary>
        /// <param name="Position">Position to look at</param>
        /// <returns>The specified item</returns>
        public T this[int Position]
        {
            get
            {
                Position %= Count;
                int FinalPosition = (ReadPosition + Position)%MaxCapacity;
                return Buffer[FinalPosition];
            }
            set
            {
                Position %= Count;
                int FinalPosition = (ReadPosition + Position)%MaxCapacity;
                Buffer[FinalPosition] = value;
            }
        }

        /// <summary>
        ///     Is this synchronized?
        /// </summary>
        public bool IsSynchronized { get; protected set; }

        /// <summary>
        ///     Sync root
        /// </summary>
        public object SyncRoot
        {
            get
            {
                if (Root == null)
                    Interlocked.CompareExchange(ref Root, new object(), null);
                return Root;
            }
        }

        /// <summary>
        ///     Item count for the circular buffer
        /// </summary>
        public int Count { get; protected set; }

        /// <summary>
        ///     Is this read only?
        /// </summary>
        public bool IsReadOnly { get; protected set; }

        #endregion

        #region Functions

        #region Add

        /// <summary>
        ///     Adds an item to the buffer
        /// </summary>
        /// <param name="item">Item to add</param>
        public virtual void Add(T item)
        {
            if (Count >= MaxCapacity && !AllowOverflow)
                throw new InvalidOperationException("Unable to add item to circular buffer because the buffer is full");
            Buffer[WritePosition] = item;
            ++Count;
            ++WritePosition;
            if (WritePosition >= MaxCapacity)
                WritePosition = 0;
            if (Count >= MaxCapacity)
                Count = MaxCapacity;
        }

        /// <summary>
        ///     Adds a number of items to the buffer
        /// </summary>
        /// <param name="Items">Items to add</param>
        public virtual void Add(IEnumerable<T> Items)
        {
            Items.ForEach(x => Add(x));
        }

        /// <summary>
        ///     Adds a number of items to the buffer
        /// </summary>
        /// <param name="buffer">Items to add</param>
        /// <param name="count">Number of items to add</param>
        /// <param name="offset">Offset to start at</param>
        public virtual void Add(T[] buffer, int offset, int count)
        {
            if (count > buffer.Length - offset)
                throw new ArgumentOutOfRangeException("buffer");
            for (int x = offset; x < offset + count; ++x)
                Add(buffer[x]);
        }

        #endregion

        #region Clear

        /// <summary>
        ///     Clears the buffer
        /// </summary>
        public virtual void Clear()
        {
            ReadPosition = 0;
            WritePosition = 0;
            Count = 0;
            for (int x = 0; x < MaxCapacity; ++x)
                Buffer[x] = default(T);
        }

        #endregion

        #region Constains

        /// <summary>
        ///     Determines if the buffer contains the item
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <returns>True if the item is present, false otherwise</returns>
        public virtual bool Contains(T item)
        {
            int y = ReadPosition;
            var Comparer = new GenericEqualityComparer<T>();
            for (int x = 0; x < Count; ++x)
            {
                if (Comparer.Equals(Buffer[y], item))
                    return true;
                ++y;
                if (y >= MaxCapacity)
                    y = 0;
            }
            return false;
        }

        #endregion

        #region CopyTo

        /// <summary>
        ///     Copies the buffer to an array
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="index">Array index to start at</param>
        public virtual void CopyTo(Array array, int index)
        {
            int y = ReadPosition;
            int y2 = index;
            int MaxLength = (array.Length - index) < Count ? (array.Length - index) : Count;
            for (int x = 0; x < MaxLength; ++x)
            {
                array.SetValue(Buffer[y], y2);
                ++y2;
                ++y;
                if (y >= MaxCapacity)
                    y = 0;
            }
        }

        /// <summary>
        ///     Copies the buffer to an array
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="arrayIndex">Array index to start at</param>
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            int y = ReadPosition;
            int y2 = arrayIndex;
            int MaxLength = (array.Length - arrayIndex) < Count ? (array.Length - arrayIndex) : Count;
            for (int x = 0; x < MaxLength; ++x)
            {
                array[y2] = Buffer[y];
                ++y2;
                ++y;
                if (y >= MaxCapacity)
                    y = 0;
            }
        }

        #endregion

        #region GetEnumerator

        /// <summary>
        ///     Gets the enumerator for the buffer
        /// </summary>
        /// <returns>The enumerator</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            int y = ReadPosition;
            for (int x = 0; x < Count; ++x)
            {
                yield return Buffer[y];
                ++y;
                if (y >= MaxCapacity)
                    y = 0;
            }
        }

        /// <summary>
        ///     Gets the enumerator for the buffer
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Remove

        /// <summary>
        ///     Removes an item from the buffer
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>True if it is removed, false otherwise</returns>
        public virtual bool Remove(T item)
        {
            int y = ReadPosition;
            var Comparer = new GenericEqualityComparer<T>();
            for (int x = 0; x < Count; ++x)
            {
                if (Comparer.Equals(Buffer[y], item))
                {
                    Buffer[y] = default(T);
                    return true;
                }
                ++y;
                if (y >= MaxCapacity)
                    y = 0;
            }
            return false;
        }

        /// <summary>
        ///     Reads the next item from the buffer
        /// </summary>
        /// <returns>The next item from the buffer</returns>
        public virtual T Remove()
        {
            if (Count == 0)
                return default(T);
            T ReturnValue = Buffer[ReadPosition];
            Buffer[ReadPosition] = default(T);
            ++ReadPosition;
            ReadPosition %= MaxCapacity;
            --Count;
            return ReturnValue;
        }

        /// <summary>
        ///     Reads the next X number of items from the buffer
        /// </summary>
        /// <param name="Amount">Number of items to return</param>
        /// <returns>The next X items from the buffer</returns>
        public virtual IEnumerable<T> Remove(int Amount)
        {
            if (Count == 0)
                return new List<T>();
            var ReturnValue = new List<T>();
            for (int x = 0; x < Amount; ++x)
                ReturnValue.Add(Remove());
            return ReturnValue;
        }

        /// <summary>
        ///     Reads the next X number of items and places them in the array passed in
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="offset">Offset to start at</param>
        /// <param name="count">Number of items to read</param>
        /// <returns>The number of items that were read</returns>
        public virtual int Remove(T[] array, int offset, int count)
        {
            if (Count > array.Length - offset)
                throw new ArgumentOutOfRangeException("array");
            if (Count == 0)
                return 0;
            int y = ReadPosition;
            int y2 = offset;
            int MaxLength = count < Count ? count : Count;
            for (int x = 0; x < MaxLength; ++x)
            {
                array[y2] = Buffer[y];
                ++y2;
                ++y;
                if (y >= MaxCapacity)
                    y = 0;
            }
            Count -= MaxLength;
            return MaxLength;
        }

        #endregion

        #region Skip

        /// <summary>
        ///     Skips ahead in the buffer
        /// </summary>
        /// <param name="Count">Number of items in the buffer to skip</param>
        public virtual void Skip(int Count)
        {
            if (Count > this.Count)
                Count = this.Count;
            ReadPosition += Count;
            this.Count -= Count;
            if (ReadPosition >= MaxCapacity)
                ReadPosition %= MaxCapacity;
        }

        #endregion

        #region ToString

        /// <summary>
        ///     Returns the buffer as a string
        /// </summary>
        /// <returns>The buffer as a string</returns>
        public override string ToString()
        {
            return Buffer.ToString<T>();
        }

        #endregion

        #endregion
    }
}