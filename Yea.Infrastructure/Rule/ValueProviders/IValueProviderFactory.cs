using System;

namespace Yea.Infrastructure.Rule.ValueProviders
{
    public interface IValueProviderFactory
    {
        object GetValue(string value, Type type);
    }
}