using Yea.Rule.Engine;

namespace Yea.Rule.RubyEvaluator
{
    public class RubyEvaluator : IDslConditionEvaluator
    {
        public bool Evaluate<T>(string condition, T context)
        {
            var ruleEngine = new RubyEngine(context.GetType(), condition);
            var rule = ruleEngine.Create();
            return rule.Evaluate(context);
        }
    }
}