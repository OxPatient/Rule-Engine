using System;

namespace Yea.Rule.Engine
{
    public class FuncCondition<T> : ICondition<T>
    {
        private readonly Func<T, bool> _condition;

        public FuncCondition(Func<T, bool> condition)
        {
            _condition = condition;
        }

        public virtual bool Evaluate(object context)
        {
            return _condition((T) context);
        }

        public virtual bool Evaluate(T context)
        {
            return _condition(context);
        }
    }
}