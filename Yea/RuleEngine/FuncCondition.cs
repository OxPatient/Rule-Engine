#region Usings

using System;

#endregion

namespace Yea.RuleEngine
{
    public class FuncCondition<T> : ICondition<T>
    {
        private readonly Func<T, bool> _condition;

        public FuncCondition(Func<T, bool> condition)
        {
            _condition = condition;
        }

        #region Implementation of ICondition

        public bool Decide(T obj)
        {
            return _condition.Invoke(obj);
        }

        public bool Decide(object obj)
        {
            return _condition.Invoke((T) obj);
        }

        #endregion
    }
}