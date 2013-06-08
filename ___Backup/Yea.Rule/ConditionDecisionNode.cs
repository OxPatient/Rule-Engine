using Yea.Rule.Engine;

namespace Yea.Rule
{
    public class ConditionDecisionNode<T> : DecisionNode<T>
    {
        private readonly ICondition _condition;

        public ConditionDecisionNode(ICondition condition, FlowElement<T> master)
            : base(master)
        {
            _condition = condition;
        }

        protected override bool Condition(T instance)
        {
            return _condition.Evaluate(instance);
        }
    }
}