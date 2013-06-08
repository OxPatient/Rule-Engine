#region Usings

using System;
using System.Threading;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    /// <summary>
    ///     Extensions for Func, Action, and EventHandler
    /// </summary>
    public static class DelegateExtensions
    {
        #region Async

        /// <summary>
        ///     Runs an action async
        /// </summary>
        /// <param name="action">Action to run</param>
        public static void Async(this Action action)
        {
            new Thread(action.Invoke).Start();
        }

        #endregion

        #region Raise

        /// <summary>
        ///     Safely calls the specified action
        /// </summary>
        /// <typeparam name="T">The type of the event args</typeparam>
        /// <param name="Delegate">The delegate</param>
        /// <param name="eventArgs">The event args</param>
        public static void Raise<T>(this Action<T> Delegate, T eventArgs)
        {
            if (Delegate != null)
                Delegate(eventArgs);
        }

        /// <summary>
        ///     Safely raises the event
        /// </summary>
        /// <typeparam name="T">The type of the event args</typeparam>
        /// <param name="Delegate">The delegate</param>
        /// <param name="sender">The sender</param>
        /// <param name="eventArg">The event args</param>
        public static void Raise<T>(this EventHandler<T> Delegate, object sender, T eventArg)
            where T : System.EventArgs
        {
            if (Delegate != null)
                Delegate(sender, eventArg);
        }

        /// <summary>
        ///     Safely calls the Func
        /// </summary>
        /// <typeparam name="T1">The event arg type</typeparam>
        /// <typeparam name="T2">The return type</typeparam>
        /// <param name="Delegate">The delegate</param>
        /// <param name="eventArgs">The event args</param>
        /// <returns>The value returned by the function</returns>
        public static T2 Raise<T1, T2>(this Func<T1, T2> Delegate, T1 eventArgs)
        {
            if (Delegate != null)
                return Delegate(eventArgs);
            return default(T2);
        }

        #endregion
    }
}