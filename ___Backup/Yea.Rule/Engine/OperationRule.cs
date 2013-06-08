using System;

namespace Yea.Rule.Engine
{
    internal sealed class OperationRule : RuleBase
    {
        private readonly Func<bool, bool, bool> _operation;
        private readonly RuleBase _rule1;
        private readonly RuleBase _rule2;

        public OperationRule(RuleBase rule1, RuleBase rule2, Func<bool, bool, bool> operation)
        {
            _operation = operation;
            _rule1 = rule1;
            _rule2 = rule2;
        }

        public override bool Evaluate<T>(T context)
        {
            return _operation(_rule1.Evaluate(context), _rule2.Evaluate(context));
        }
    }
}