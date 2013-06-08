namespace Yea.Rule.Engine
{
    public interface ICondition
    {
        bool Evaluate(object context);
    }

    public interface ICondition<T> : ICondition
    {
        bool Evaluate(T context);
    }
}