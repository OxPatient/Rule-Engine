using System;

namespace Yea.Infrastructure.Rule.Rules
{
    public class IsNullRule : AbstractRule
    {
        public IsNullRule()
        {
        }

        public IsNullRule(string propertyName, Type type, object expectedValue)
            : base(propertyName, type, expectedValue)
        {
        }

        protected override bool EvaluateInternal(object propValue, object expectedValue)
        {
            return propValue == null;
        }

        public override string GetOperation()
        {
            return "IsNull";
        }
    }
}