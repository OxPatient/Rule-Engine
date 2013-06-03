﻿#region Usings

using System;

#endregion

namespace Yea.RuleEngine
{
    public class ActionHandler<T> : IHandler<T>
    {
        private readonly Action<T> _action;

        public ActionHandler(Action<T> action)
        {
            _action = action;
        }

        #region Implementation of IHandler

        public void Handle(T obj)
        {
            _action.Invoke(obj);
        }

        public void Handle(object obj)
        {
            _action.Invoke((T) obj);
        }

        #endregion
    }
}