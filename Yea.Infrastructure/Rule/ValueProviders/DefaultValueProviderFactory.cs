using System;
using System.Collections.Generic;
using System.Globalization;

namespace Yea.Infrastructure.Rule.ValueProviders
{
    public class DefaultValueProviderFactory : IValueProviderFactory
    {
        private readonly Dictionary<string, IValueProvider> providers = new Dictionary<string, IValueProvider>();

        #region IValueProviderFactory Members

        public object GetValue(string value, Type type)
        {
            if (value == null)
                return null;

            if (typeof(string) == type)
            {

            }

            if (value.StartsWith("\"") && value.EndsWith("\""))
                value = value.Substring(1, value.Length - 2);

            if (typeof(DateTime) == type)
            {
                return DateTime.ParseExact(value, "yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
            }

            if (type.IsValueType)
            {
                return Convert.ChangeType(value, type);
            }


            return providers.ContainsKey(value) ? providers[value].GetValue() : value;
        }

        #endregion

        public void AddValueProvider(string key, IValueProvider valueProvider)
        {
            Guard.NotNull(key, "key");
            Guard.NotNull(valueProvider, "valueProvider");
            providers.Add(key, valueProvider);
        }
    }
}