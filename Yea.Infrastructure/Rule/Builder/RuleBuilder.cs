using System;
using Yea.Infrastructure.Rule.Rules;

namespace Yea.Infrastructure.Rule.Builder
{
    public class RuleBuilder : INew, IFromType, IWithProperty, IBuild
    {
        private string property;
        private AbstractRule rule;
        private Type type;

        #region IBuild Members

        public IRule Build()
        {
            return rule;
        }

        #endregion

        #region IFromType Members

        public IWithProperty WithProperty(string property)
        {
            this.property = property;
            return this;
        }

        #endregion

        #region INew Members

        public IFromType FromType<T>()
        {
            return FromType(typeof (T));
        }

        public IFromType FromType(Type type)
        {
            this.type = type;
            return this;
        }

        #endregion

        #region IWithProperty Members

        public IBuild IsEqualTo(object value)
        {
            rule = new EqualsRule(property, type, value);
            return this;
        }

        public IBuild IsNotEqualTo(object value)
        {
            rule = new NotEqualsRule(property, type, value);
            return this;
        }

        public IBuild GreaterThan(object value)
        {
            throw new NotImplementedException();
        }

        public IBuild LessThan(object value)
        {
            throw new NotImplementedException();
        }

        public IBuild IsNull(object value)
        {
            rule = new IsNullRule(property, type, value);
            return this;
        }

        public IBuild StartWith(string value)
        {
            rule = new StartWithRule(property, type, value);
            return this;
        }

        public IBuild Contains(string value)
        {
            rule = new ContainsRule(property, type, value);
            return this;
        }

        #endregion

        public INew New()
        {
            type = null;
            property = null;
            rule = null;
            return this;
        }
    }
}
