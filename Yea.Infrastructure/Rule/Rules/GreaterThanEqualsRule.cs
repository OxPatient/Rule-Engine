using System;
using System.Linq.Expressions;

namespace Yea.Infrastructure.Rule.Rules
{
    public class GreaterThanEqualsRule : AbstractRule
    {
        public GreaterThanEqualsRule()
        {
        }

        public GreaterThanEqualsRule(string propertyName, Type type, object value)
            : base(propertyName, type, value)
        {
        }

        protected override bool EvaluateInternal(object propValue, object expectedValue)
        {
            if (DelegateRule == null)
            {
                Type internalType = propValue.GetType();
                
                var x = Expression.Parameter(internalType, "x");
                var y = Expression.Parameter(internalType, "y");

                var lambda = Expression.Lambda(Expression.GreaterThanOrEqual(x, y), x, y);
                DelegateRule = lambda.Compile();
            }

            return (bool)DelegateRule.DynamicInvoke(propValue, expectedValue);
        }

        public override string GetOperation()
        {
            return ">=";
        }
    }
}