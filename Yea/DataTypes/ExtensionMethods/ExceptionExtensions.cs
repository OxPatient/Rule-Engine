#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    /// <summary>
    ///     Class for housing exception specific extensions
    /// </summary>
    public static class ExceptionExtensions
    {
        #region ToString

        /// <summary>
        ///     Converts the exception to a string and appends the specified prefix/suffix (used for logging)
        /// </summary>
        /// <param name="exception">Exception to convert</param>
        /// <param name="prefix">Prefix</param>
        /// <param name="suffix">Suffix</param>
        /// <returns>The exception as a string</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static string ToString(this Exception exception, string prefix, string suffix = "")
        {
            if (exception == null)
                return "";
            var builder = new StringBuilder();
            builder.AppendLine(prefix);
            try
            {
                builder.AppendLineFormat("Exception: {0}", exception.Message);
                builder.AppendLineFormat("Exception Type: {0}", exception.GetType().FullName);
                foreach (var Object in exception.Data)
                    builder.AppendLineFormat("Data: {0}:{1}", Object, exception.Data[Object]);
                builder.AppendLineFormat("StackTrace: {0}", exception.StackTrace);
                builder.AppendLineFormat("Source: {0}", exception.Source);
                builder.AppendLineFormat("TargetSite: {0}", exception.TargetSite);
                builder.Append(exception.InnerException.ToString(prefix, suffix));
            }
            catch
            {
            }
            builder.AppendLine(suffix);
            return builder.ToString();
        }

        #endregion
    }
}