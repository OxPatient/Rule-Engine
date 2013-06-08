#region Usings

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.DataTypes
{
    /// <summary>
    ///     Dictionary that matches multiple keys to each value
    /// </summary>
    /// <typeparam name="Key">Key type</typeparam>
    /// <typeparam name="Value">Value type</typeparam>
    public class TagDictionary<Key, Value> : IDictionary<Key, IEnumerable<Value>>
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        public TagDictionary()
        {
            Items = new ConcurrentBag<TaggedItem<Key, Value>>();
            KeyList = new List<Key>();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Items in the dictionary
        /// </summary>
        private ConcurrentBag<TaggedItem<Key, Value>> Items { get; set; }

        /// <summary>
        ///     List of keys that have been entered
        /// </summary>
        private List<Key> KeyList { get; set; }

        /// <summary>
        ///     Gets the values found in the dictionary
        /// </summary>
        public ICollection<IEnumerable<Value>> Values
        {
            get { return new IEnumerable<Value>[] {Items.ToArray(x => x.Value)}; }
        }

        /// <summary>
        ///     Gets the keys found in the dictionary
        /// </summary>
        public ICollection<Key> Keys
        {
            get { return KeyList; }
        }

        /// <summary>
        ///     Gets the values based on a key
        /// </summary>
        /// <param name="key">Key to get the values of</param>
        /// <returns>The values associated with the key</returns>
        public IEnumerable<Value> this[Key key]
        {
            get { return Items.Where(x => x.Keys.Contains(key)).ToArray(x => x.Value); }
            set { Add(key, value); }
        }

        /// <summary>
        ///     Number of items in the dictionary
        /// </summary>
        public int Count
        {
            get { return Items.Count(); }
        }

        /// <summary>
        ///     Always false
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Functions

        /// <summary>
        ///     Adds a list of values to the key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Values to add</param>
        public void Add(Key key, IEnumerable<Value> value)
        {
            value.ToArray(x => new TaggedItem<Key, Value>(key, x)).ForEach(x => Items.Add(x));
            KeyList.AddIfUnique(key);
        }

        /// <summary>
        ///     Determines if a key is in the dictionary
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if it exists, false otherwise</returns>
        public bool ContainsKey(Key key)
        {
            return KeyList.Contains(key);
        }

        /// <summary>
        ///     Removes all items that are associated with a key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Returns true if the key was found, false otherwise</returns>
        public bool Remove(Key key)
        {
            bool ReturnValue = ContainsKey(key);
            Items = new ConcurrentBag<TaggedItem<Key, Value>>(Items.ToArray(x => x).Where(x => !x.Keys.Contains(key)));
            KeyList.Remove(key);
            return ReturnValue;
        }

        /// <summary>
        ///     Attempts to get the values associated with a key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Values associated with a key</param>
        /// <returns>True if something is returned, false otherwise</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public bool TryGetValue(Key key, out IEnumerable<Value> value)
        {
            value = new List<Value>();
            try
            {
                value = this[key];
            }
            catch
            {
            }
            return value.Count() > 0;
        }

        /// <summary>
        ///     Adds an item to the dictionary
        /// </summary>
        /// <param name="item">item to add</param>
        public void Add(KeyValuePair<Key, IEnumerable<Value>> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        ///     Clears the dictionary
        /// </summary>
        public void Clear()
        {
            Items = new ConcurrentBag<TaggedItem<Key, Value>>();
        }

        /// <summary>
        ///     Determines if the dictionary contains the key/value pair
        /// </summary>
        /// <param name="item">item to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public bool Contains(KeyValuePair<Key, IEnumerable<Value>> item)
        {
            return ContainsKey(item.Key);
        }

        /// <summary>
        ///     Copies itself to an array
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="arrayIndex">Array index</param>
        public void CopyTo(KeyValuePair<Key, IEnumerable<Value>>[] array, int arrayIndex)
        {
            for (int x = 0; x < Keys.Count; ++x)
            {
                array[arrayIndex + x] = new KeyValuePair<Key, IEnumerable<Value>>(Keys.ElementAt(x),
                                                                                  this[Keys.ElementAt(x)]);
            }
        }

        /// <summary>
        ///     Removes a specific key/value pair
        /// </summary>
        /// <param name="item">item to remove</param>
        /// <returns>True if it is removed, false otherwise</returns>
        public bool Remove(KeyValuePair<Key, IEnumerable<Value>> item)
        {
            return Remove(item.Key);
        }

        /// <summary>
        ///     Gets the enumerator
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<KeyValuePair<Key, IEnumerable<Value>>> GetEnumerator()
        {
            foreach (var Key in Keys)
            {
                yield return new KeyValuePair<Key, IEnumerable<Value>>(Key, this[Key]);
            }
        }

        /// <summary>
        ///     Gets the enumerator
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var Key in Keys)
            {
                yield return this[Key];
            }
        }

        /// <summary>
        ///     Adds a value to the dicionary
        /// </summary>
        /// <param name="Value">Value to add</param>
        /// <param name="Keys">Keys to associate the value with</param>
        public void Add(Value Value, params Key[] Keys)
        {
            Items.Add(new TaggedItem<Key, Value>(Keys, Value));
            Keys.ForEach(x => KeyList.AddIfUnique(x));
        }

        #endregion

        #region Internal Classes

        /// <summary>
        ///     Holds information about each value
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        private class TaggedItem<TKey, TValue>
        {
            #region Constructor

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="Keys">Keys</param>
            /// <param name="Value">Value</param>
            public TaggedItem(IEnumerable<TKey> Keys, TValue Value)
            {
                this.Keys = new ConcurrentBag<TKey>(Keys);
                this.Value = Value;
            }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="Key">Key</param>
            /// <param name="Value">Value</param>
            public TaggedItem(TKey Key, TValue Value)
            {
                Keys = new ConcurrentBag<TKey>(new[] {Key});
                this.Value = Value;
            }

            #endregion

            #region Properties

            /// <summary>
            ///     The list of keys associated with the value
            /// </summary>
            public ConcurrentBag<TKey> Keys { get; set; }

            /// <summary>
            ///     Value
            /// </summary>
            public TValue Value { get; set; }

            #endregion
        }

        #endregion
    }
}