namespace Yea.Infrastructure.Rule.ValueProviders
{
    public interface IValueProvider
    {
        string Key { get; }
        object GetValue();
    }
}