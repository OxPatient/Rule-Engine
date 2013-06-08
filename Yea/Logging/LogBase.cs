#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Yea.Logging
{
    /// <summary>
    ///     Base class for logs
    /// </summary>
    /// <typeparam name="TLogType">Log type</typeparam>
    public class LogBase<TLogType> : ILog where TLogType : LogBase<TLogType>
    {
        #region Constructors

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="start">Action called when the log is created</param>
        public LogBase(Action<TLogType> start)
        {
//// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Start = start;
//// ReSharper restore DoNotCallOverridableMethodsInConstructor
            start((TLogType) this);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Called to log the current message
        /// </summary>
        private readonly Dictionary<MessageType, Action<string>> _log = new Dictionary<MessageType, Action<string>>();

        /// <summary>
        ///     Called when the log is "opened"
        /// </summary>
        protected virtual Action<TLogType> Start { get; set; }

        /// <summary>
        ///     Called when the log is "closed"
        /// </summary>
        protected virtual Action<TLogType> End { get; set; }

        /// <summary>
        ///     Called to log the current message
        /// </summary>
        protected Dictionary<MessageType, Action<string>> Log
        {
            get { return _log; }
        }

        /// <summary>
        ///     Format message function
        /// </summary>
        protected Format FormatMessage { get; set; }

        #endregion

        #region Interface Functions

        /// <summary>
        ///     Disposes the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Logs a message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="type">Type of message</param>
        /// <param name="args">args to format/insert into the message</param>
        public virtual void LogMessage(string message, MessageType type, params object[] args)
        {
            message = FormatMessage(message, type, args);
            if (Log.ContainsKey(type))
                Log[type](message);
        }

        /// <summary>
        ///     Disposes of the objects
        /// </summary>
        /// <param name="disposing">True to dispose of all resources, false only disposes of native resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                End((TLogType) this);
        }

        /// <summary>
        ///     Destructor
        /// </summary>
        ~LogBase()
        {
            Dispose(false);
        }

        #endregion
    }

    /// <summary>
    ///     Delegate used to format the message
    /// </summary>
    /// <param name="message">Message to format</param>
    /// <param name="type">Type of message</param>
    /// <param name="args">Args to insert into the message</param>
    /// <returns>The formatted message</returns>
    public delegate string Format(string message, MessageType type, params object[] args);
}