using System;

namespace Yea.Rule.Engine
{
    public class ActionActivity<T> : IActivity<T>
    {
        private readonly Action<T> _action;

        public ActionActivity(Action<T> action)
        {
            _action = action;
        }

        public virtual void Execute(object context)
        {
            _action((T) context);
        }

        public virtual void Execute(T context)
        {
            _action(context);
        }
    }
}