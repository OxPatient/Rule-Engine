using System;

namespace Yea.Infrastructure.Rule.Rules
{
    public class ContainsRule : AbstractRule
    {
        public ContainsRule()
        {
        }

        public ContainsRule(string propertyName, Type type, string value)
            : base(propertyName, type, value)
        {
        }

        protected override bool EvaluateInternal(object propValue, object expectedValue)
        {
            return propValue is string && ((string) propValue).Contains((string)expectedValue);
        }

        public override string GetOperation()
        {
            return "Contains";
        }
    }
}