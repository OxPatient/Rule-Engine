#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Yea.DataTypes.Patterns
{
    /// <summary>
    ///     Factory class
    /// </summary>
    /// <typeparam name="TKey">The "message" type</typeparam>
    /// <typeparam name="T">The class type that you want created</typeparam>
    public class Factory<TKey, T>
    {
        #region Constructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public Factory()
        {
            Constructors = new Dictionary<TKey, Func<T>>();
        }

        #endregion

        #region Protected Variables

        /// <summary>
        ///     List of constructors/initializers
        /// </summary>
        protected Dictionary<TKey, Func<T>> Constructors { get; private set; }

        #endregion

        #region Public Functions

        /// <summary>
        ///     Registers an item
        /// </summary>
        public virtual void Register()
        {
            Register(default(T));
        }

        /// <summary>
        ///     Registers an item
        /// </summary>
        /// <param name="result">The object to be returned</param>
        public virtual void Register(T result)
        {
            Register(default(TKey), result);
        }

        /// <summary>
        ///     Registers an item
        /// </summary>
        /// <param name="key">Item to register</param>
        /// <param name="result">The object to be returned</param>
        public virtual void Register(TKey key, T result)
        {
            if (Exists(key))
                Constructors[key] = () => result;
            else
                Constructors.Add(key, () => result);
        }

        /// <summary>
        ///     Registers an item
        /// </summary>
        /// <param name="key">Item to register</param>
        /// <param name="constructor">The function to call when creating the item</param>
        public virtual void Register(TKey key, Func<T> constructor)
        {
            if (Exists(key))
                Constructors[key] = constructor;
            else
                Constructors.Add(key, constructor);
        }

        /// <summary>
        ///     Creates an instance associated with the key
        /// </summary>
        /// <param name="key">Registered item</param>
        /// <returns>The type returned by the initializer</returns>
        public virtual T Create(TKey key)
        {
            if (Exists(key))
                return Constructors[key]();
            return default(T);
        }

        /// <summary>
        ///     Determines if a key has been registered
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if it exists, false otherwise</returns>
        public virtual bool Exists(TKey key)
        {
            return Constructors.ContainsKey(key);
        }

        #endregion
    }
}