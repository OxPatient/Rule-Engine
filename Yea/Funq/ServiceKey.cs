#region Usings

using System;

#endregion

namespace Yea.Funq
{
    internal sealed class ServiceKey
    {
        private readonly int hash;

        public Type FactoryType;
        public string Name;

        #region Equality

        public bool Equals(ServiceKey other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return Equals(this, obj as ServiceKey);
        }

        public static bool Equals(ServiceKey obj1, ServiceKey obj2)
        {
            if (Object.Equals(null, obj1) ||
                Object.Equals(null, obj2))
                return false;

            return obj1.FactoryType == obj2.FactoryType &&
                   obj1.Name == obj2.Name;
        }

        public override int GetHashCode()
        {
            return hash;
        }

        #endregion

        public ServiceKey(Type factoryType, string serviceName)
        {
            FactoryType = factoryType;
            Name = serviceName;

            hash = factoryType.GetHashCode();
            if (serviceName != null)
                hash ^= serviceName.GetHashCode();
        }
    }
}