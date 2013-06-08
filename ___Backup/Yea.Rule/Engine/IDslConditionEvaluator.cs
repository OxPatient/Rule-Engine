namespace Yea.Rule.Engine
{
    public interface IDslConditionEvaluator
    {
        bool Evaluate<T>(string condition, T context);
    }
}