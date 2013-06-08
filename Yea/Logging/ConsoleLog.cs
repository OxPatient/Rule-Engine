#region Usings

using System;
using System.Globalization;

#endregion

namespace Yea.Logging
{
    /// <summary>
    ///     Outputs messages to the console
    /// </summary>
    public class ConsoleLog : LogBase<ConsoleLog>
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        public ConsoleLog()
            : base(
                x =>
                Console.WriteLine("--------------------------------Logging started--------------------------------"))
        {
            End =
                x =>
                Console.WriteLine("---------------------------------Logging ended---------------------------------");
            Log.Add(MessageType.Debug, Console.WriteLine);
            Log.Add(MessageType.Error, Console.WriteLine);
            Log.Add(MessageType.General, Console.WriteLine);
            Log.Add(MessageType.Info, Console.WriteLine);
            Log.Add(MessageType.Trace, Console.WriteLine);
            Log.Add(MessageType.Warn, Console.WriteLine);
            FormatMessage = (message, type, args) => type.ToString()
                                                     + ": " +
                                                     (args.Length > 0
                                                          ? string.Format(CultureInfo.InvariantCulture, message, args)
                                                          : message);
        }

        #endregion
    }
}