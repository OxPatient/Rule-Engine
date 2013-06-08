#region Usings

using System;
using System.Threading;

#endregion

namespace Yea.Funq
{
    internal sealed class ServiceEntry<TService, TFunc> : ServiceEntry, IRegistration<TService>
    {
        /// <summary>
        ///     The Func delegate that creates instances of the service.
        /// </summary>
        public TFunc Factory;

        /// <summary>
        ///     The Func delegate that initializes the object after creation.
        /// </summary>
        internal Action<Container, TService> Initializer;

        public ServiceEntry(TFunc factory)
        {
            Factory = factory;
        }

        internal TService Instance { get; set; }


        public IReusedOwned InitializedBy(Action<Container, TService> initializer)
        {
            Initializer = initializer;
            return this;
        }

        internal void InitializeInstance(TService instance)
        {
            // Save instance if Hierarchy or Container Reuse 
            if (Reuse != ReuseScope.None)
            {
                Instance = instance;
            }

            // Track for disposal if necessary
            if (Owner == Owner.Container && instance is IDisposable)
                Container.TrackDisposable(instance);

            // Call initializer if necessary
            if (Initializer != null)
                Initializer(Container, instance);
        }

        /// <summary>
        ///     Clones the service entry assigning the <see cref="Container" /> to the
        ///     <paramref name="newContainer" />. Does not copy the <see cref="Instance" />.
        /// </summary>
        public ServiceEntry<TService, TFunc> CloneFor(Container newContainer)
        {
            return new ServiceEntry<TService, TFunc>(Factory)
                {
                    Owner = Owner,
                    Reuse = Reuse,
                    Container = newContainer,
                    Initializer = Initializer,
                };
        }

        public IDisposable AquireLockIfNeeded()
        {
            if (Reuse == ReuseScope.None || Instance != null)
                return null;

            return new AquiredLock(this);
        }

        internal class AquiredLock : IDisposable
        {
            private readonly object _syncRoot;

            public AquiredLock(object syncRoot)
            {
                Monitor.Enter(_syncRoot = syncRoot);
            }

            public void Dispose()
            {
                Monitor.Exit(_syncRoot);
            }
        }
    }
}