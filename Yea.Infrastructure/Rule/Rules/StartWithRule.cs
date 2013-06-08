using System;

namespace Yea.Infrastructure.Rule.Rules
{
    public class StartWithRule : AbstractRule
    {
        public StartWithRule()
        {
        }

        public StartWithRule(string propertyName, Type type, string value)
            : base(propertyName, type, value)
        {
        }

        protected override bool EvaluateInternal(object propValue, object expectedValue)
        {
            return propValue is string && ((string) propValue).StartsWith((string)expectedValue);
        }

        public override string GetOperation()
        {
            return "StartWith";
        }
    }
}