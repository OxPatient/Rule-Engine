namespace Yea.Infrastructure.Rule.Builder
{
    public interface IWithProperty
    {
        IBuild IsEqualTo(object value);
        IBuild IsNotEqualTo(object value);
        IBuild GreaterThan(object value);
        IBuild LessThan(object value);
        IBuild IsNull(object value);
        IBuild StartWith(string value);
        IBuild Contains(string value);
    }
}