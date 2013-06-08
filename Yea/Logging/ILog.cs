#region Usings

using System;

#endregion

namespace Yea.Logging
{
    /// <summary>
    ///     Log interface
    /// </summary>
    public interface ILog : IDisposable
    {
        #region Functions

        /// <summary>
        ///     Logs a message
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="type">Message type</param>
        /// <param name="args">Any additional arguments that will be used in formatting the message</param>
        void LogMessage(string message, MessageType type, params object[] args);

        #endregion
    }
}