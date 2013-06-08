#region Usings

using System;
using System.Globalization;
using System.IO;
using Yea.IO;

#endregion

namespace Yea.Logging
{
    /// <summary>
    ///     Outputs messages to a file
    /// </summary>
    public class FileLog : LogBase<FileLog>
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        public FileLog(string fileName)
            : base(x => new FileInfo(fileName).Save("Logging started at " + DateTime.Now + Environment.NewLine))
        {
            End =
                x =>
                new FileInfo(fileName).Save("Logging ended at " + DateTime.Now + Environment.NewLine, FileMode.Append);
            Log.Add(MessageType.Debug, x => new FileInfo(fileName).Save(x, FileMode.Append));
            Log.Add(MessageType.Error, x => new FileInfo(fileName).Save(x, FileMode.Append));
            Log.Add(MessageType.General, x => new FileInfo(fileName).Save(x, FileMode.Append));
            Log.Add(MessageType.Info, x => new FileInfo(fileName).Save(x, FileMode.Append));
            Log.Add(MessageType.Trace, x => new FileInfo(fileName).Save(x, FileMode.Append));
            Log.Add(MessageType.Warn, x => new FileInfo(fileName).Save(x, FileMode.Append));
            FormatMessage = (message, type, args) => type.ToString()
                                                     + ": " +
                                                     (args.Length > 0
                                                          ? string.Format(CultureInfo.InvariantCulture, message, args)
                                                          : message)
                                                     + Environment.NewLine;
        }

        #endregion
    }
}