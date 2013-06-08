#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Yea.Logging
{
    /// <summary>
    ///     Logging manager
    /// </summary>
    public static class LoggingManager
    {
        #region Fields

        /// <summary>
        ///     Logs
        /// </summary>
        private static readonly Dictionary<string, ILog> Logs = new Dictionary<string, ILog>();

        #endregion

        #region Functions

        /// <summary>
        ///     Gets a specified log
        /// </summary>
        /// <param name="name">The name of the log file</param>
        /// <typeparam name="TLogType">Log type that the log object should be</typeparam>
        /// <returns>The log file specified</returns>
        public static TLogType GetLog<TLogType>(string name = "Default") where TLogType : ILog
        {
            if (!Logs.ContainsKey(name))
                throw new ArgumentException(name + " was not found");
            if (!(Logs[name] is TLogType))
                throw new ArgumentException(name + " is not the type specified");
            return (TLogType) Logs[name];
        }

        /// <summary>
        ///     Gets a specified log
        /// </summary>
        /// <param name="name">The name of the log file</param>
        /// <returns>The log file specified</returns>
        public static ILog GetLog(string name = "Default")
        {
            if (!Logs.ContainsKey(name))
                throw new ArgumentException(name + " was not found");
            return Logs[name];
        }

        /// <summary>
        ///     Adds a log object or replaces one already in use
        /// </summary>
        /// <param name="name">The name of the log file</param>
        /// <typeparam name="TLogType">Log type to add</typeparam>
        public static void AddLog<TLogType>(string name = "Default") where TLogType : LogBase<TLogType>, new()
        {
            AddLog(new TLogType(), name);
        }

        /// <summary>
        ///     Adds a log object or replaces one already in use
        /// </summary>
        /// <param name="log">The log object to add</param>
        /// <param name="name">The name of the log file</param>
        public static void AddLog(ILog log, string name = "Default")
        {
            if (log == null)
                throw new ArgumentNullException("log");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (Logs.ContainsKey(name))
                Logs[name] = log;
            else
                Logs.Add(name, log);
        }

        /// <summary>
        ///     Destroys the logging manager
        /// </summary>
        public static void Destroy()
        {
            foreach (var key in Logs.Keys)
            {
                Logs[key].Dispose();
            }
            Logs.Clear();
        }

        #endregion
    }
}