#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Yea.DataTypes.Patterns
{
    /*/// <summary>
    ///     Base class used for singletons
    /// </summary>
    /// <typeparam name="T">The class type</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors")]
    public class Singleton<T> where T : class
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        protected Singleton()
        {
        }

        #endregion

        #region Private Variables

        private static T _Instance;
        private static object Temp = 1;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the instance of the singleton
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (Temp)
                    {
                        if (_Instance == null)
                        {
                            ConstructorInfo Constructor =
                                typeof (T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                                                          null, new Type[0], null);
                            if (Constructor == null || Constructor.IsAssembly)
                                throw new InvalidOperationException(
                                    "Constructor is not private or protected for type " + typeof (T).Name);
                            _Instance = (T) Constructor.Invoke(null);
                        }
                    }
                }
                return _Instance;
            }
        }

        #endregion
    }*/


    /// <summary>
    ///     A statically compiled "singleton" used to store objects throughout the
    ///     lifetime of the app domain. Not so much singleton in the pattern's
    ///     sense of the word as a standardized way to store single instances.
    /// </summary>
    /// <typeparam name="T">The type of object to store.</typeparam>
    /// <remarks>Access to the instance is not synchrnoized.</remarks>
    public class Singleton<T> : Singleton
    {
        private static T _instance;

        /// <summary>The singleton instance for the specified type T. Only one instance (at the time) of this object for each type of T.</summary>
        public static T Instance
        {
            get { return _instance; }
            set
            {
                _instance = value;
                AllSingletons[typeof (T)] = value;
            }
        }
    }

    /// <summary>
    ///     Provides a singleton list for a certain type.
    /// </summary>
    /// <typeparam name="T">The type of list to store.</typeparam>
    public class SingletonList<T> : Singleton<IList<T>>
    {
        static SingletonList()
        {
            Singleton<IList<T>>.Instance = new System.Collections.Generic.List<T>();
        }

        /// <summary>The singleton instance for the specified type T. Only one instance (at the time) of this list for each type of T.</summary>
        public new static IList<T> Instance
        {
            get { return Singleton<IList<T>>.Instance; }
        }
    }

    /// <summary>
    ///     Provides a singleton dictionary for a certain key and vlaue type.
    /// </summary>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <typeparam name="TValue">The type of value.</typeparam>
    public class SingletonDictionary<TKey, TValue> : Singleton<IDictionary<TKey, TValue>>
    {
        static SingletonDictionary()
        {
            Singleton<Dictionary<TKey, TValue>>.Instance = new Dictionary<TKey, TValue>();
        }

        /// <summary>The singleton instance for the specified type T. Only one instance (at the time) of this dictionary for each type of T.</summary>
        public new static IDictionary<TKey, TValue> Instance
        {
            get { return Singleton<Dictionary<TKey, TValue>>.Instance; }
        }
    }

    /// <summary>
    ///     Provides access to all "singletons" stored by <see cref="Singleton{T}" />.
    /// </summary>
    public class Singleton
    {
        private static readonly IDictionary<Type, object> _allSingletons;

        static Singleton()
        {
            _allSingletons = new Dictionary<Type, object>();
        }

        /// <summary>Dictionary of type to singleton instances.</summary>
        public static IDictionary<Type, object> AllSingletons
        {
            get { return _allSingletons; }
        }
    }
}