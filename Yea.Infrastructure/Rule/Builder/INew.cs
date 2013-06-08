using System;

namespace Yea.Infrastructure.Rule.Builder
{
    public interface INew
    {
        IFromType FromType<T>();
        IFromType FromType(Type type);
    }
}