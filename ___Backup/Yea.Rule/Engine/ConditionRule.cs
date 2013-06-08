namespace Yea.Rule.Engine
{
    public class ConditionalRule : RuleBase
    {
        private readonly ICondition _condition;

        public ConditionalRule(ICondition condition)
        {
            _condition = condition;
        }

        public override bool Evaluate<T>(T context)
        {
            return _condition.Evaluate(context);
        }
    }
}