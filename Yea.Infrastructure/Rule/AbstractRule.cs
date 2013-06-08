using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yea.Infrastructure.Rule
{
    public abstract class AbstractRule : IRule
    {
        public virtual string PropertyName { get; protected set; }
        public virtual string ExpectedValue { get; protected set; }
        public virtual Type Type { get; protected set; }

        private Delegate _delegate;
        protected Delegate DelegateRule;
        private Type _finalType;

        private Delegate VisitProperties(Type modelType, string propertyName)
        {
            string[] partes = propertyName.Split('.');
            Type tempType = modelType;
            Expression columnExpr = null;

            var entityParam = Expression.Parameter(modelType, "e");

            int i = 0;
            foreach (string parte in partes)
            {
                PropertyInfo propertyInfo = tempType.GetProperty(parte);
                tempType = propertyInfo.PropertyType;
                columnExpr = Expression.Property(i == 0 ? entityParam : columnExpr, propertyInfo);
                i++;
            }

            _finalType = tempType;
            var lambda = Expression.Lambda(columnExpr, entityParam);
            return lambda.Compile();
        }

        protected AbstractRule()
        {
        }

        protected AbstractRule(string propertyName, Type type, object expectedValue)
        {
            Guard.NotNull(propertyName, "propertyName");
            Guard.NotNull(type, "type");

            PropertyName = propertyName;
            Type = type;

            _delegate = VisitProperties(type, propertyName);
            
            if (expectedValue != null)
                ExpectedValue = expectedValue.ToString();

            Environment.CurrentValueProviderFactory.GetValue(ExpectedValue, _finalType);
        }

        public virtual bool Evaluate(object instance)
        {
            Guard.NotNull(instance, "instance");

            if (_delegate == null)
            {
                _delegate = VisitProperties(Type, PropertyName);
            }
            
            object instanceTemp = _delegate.DynamicInvoke(instance);
            
            return EvaluateInternal(instanceTemp, Environment.CurrentValueProviderFactory.GetValue(ExpectedValue, _finalType));
        }

        protected abstract bool EvaluateInternal(object propValue, object expectedValue);

        public abstract string GetOperation();

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", PropertyName, GetOperation(), ExpectedValue);
        }
    }
}
