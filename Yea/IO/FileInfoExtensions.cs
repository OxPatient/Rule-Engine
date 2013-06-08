#region Usings

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security;
using System.Text;
using System.Web;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.IO
{
    /// <summary>
    ///     Extension methods for <see cref="System.IO.FileInfo" />
    /// </summary>
    public static class FileInfoExtensions
    {
        #region Extension Methods

        #region CompareTo

        /// <summary>
        ///     Compares two files against one another
        /// </summary>
        /// <param name="file1">First file</param>
        /// <param name="file2">Second file</param>
        /// <returns>True if the content is the same, false otherwise</returns>
        public static bool CompareTo(this FileInfo file1, FileInfo file2)
        {
            if (file1 == null || !file1.Exists)
                throw new ArgumentNullException("file1");
            if (file2 == null || !file2.Exists)
                throw new ArgumentNullException("file2");
            if (file1.Length != file2.Length)
                return false;
            return file1.Read().Equals(file2.Read());
        }

        #endregion

        #region DriveInfo

        /// <summary>
        ///     Gets the drive information for a file
        /// </summary>
        /// <param name="file">The file to get the drive info of</param>
        /// <returns>The drive info connected to the file</returns>
        public static DriveInfo DriveInfo(this FileInfo file)
        {
            Guard.NotNull(file, "file");
            return file.Directory.DriveInfo();
        }

        #endregion

        #region Read

        /// <summary>
        ///     Reads a file to the end as a string
        /// </summary>
        /// <param name="file">File to read</param>
        /// <returns>A string containing the contents of the file</returns>
        public static string Read(this FileInfo file)
        {
            Guard.NotNull(file, "file");
            if (!file.Exists)
                return "";
            using (StreamReader reader = file.OpenText())
            {
                string contents = reader.ReadToEnd();
                return contents;
            }
        }

        /// <summary>
        ///     Reads a file to the end as a string
        /// </summary>
        /// <param name="location">File to read</param>
        /// <returns>A string containing the contents of the file</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison",
            MessageId = "System.String.StartsWith(System.String,System.StringComparison)")]
        public static string Read(this string location)
        {
            if (location.StartsWith("~", StringComparison.InvariantCulture))
            {
                location = HttpContext.Current == null
                               ? location.Replace("~", AppDomain.CurrentDomain.BaseDirectory)
                               : HttpContext.Current.Server.MapPath(location);
            }
            return new FileInfo(location).Read();
        }

        #endregion

        #region ReadBinary

        /// <summary>
        ///     Reads a file to the end and returns a binary array
        /// </summary>
        /// <param name="file">File to open</param>
        /// <returns>A binary array containing the contents of the file</returns>
        public static byte[] ReadBinary(this FileInfo file)
        {
            Guard.NotNull(file, "file");
            if (!file.Exists)
                return new byte[0];
            using (FileStream reader = file.OpenRead())
            {
                byte[] output = reader.ReadAllBinary();
                return output;
            }
        }

        /// <summary>
        ///     Reads a file to the end and returns a binary array
        /// </summary>
        /// <param name="location">File to open</param>
        /// <returns>A binary array containing the contents of the file</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison",
            MessageId = "System.String.StartsWith(System.String,System.StringComparison)")]
        public static byte[] ReadBinary(this string location)
        {
            if (location.StartsWith("~", StringComparison.InvariantCulture))
            {
                location = HttpContext.Current == null
                               ? location.Replace("~", AppDomain.CurrentDomain.BaseDirectory)
                               : HttpContext.Current.Server.MapPath(location);
            }
            return new FileInfo(location).ReadBinary();
        }

        #endregion

        #region Execute

        /// <summary>
        ///     Executes the file
        /// </summary>
        /// <param name="file">File to execute</param>
        /// <param name="arguments">Arguments sent to the executable</param>
        /// <param name="domain">Domain of the user</param>
        /// <param name="password">Password of the user</param>
        /// <param name="user">User to run the file as</param>
        /// <param name="windowStyle">Window style</param>
        /// <param name="workingDirectory">Working directory</param>
        /// <returns>The process object created when the executable is started</returns>
        public static Process Execute(this FileInfo file, string arguments = "",
                                      string domain = "", string user = "", string password = "",
                                      ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal,
                                      string workingDirectory = "")
        {
            Guard.NotNull(file, "file");
            if (!file.Exists)
                throw new FileNotFoundException("File note found", file.FullName);
            var info = new ProcessStartInfo {Arguments = arguments, Domain = domain, Password = new SecureString()};
            foreach (var Char in password)
                info.Password.AppendChar(Char);
            info.UserName = user;
            info.WindowStyle = windowStyle;
            info.UseShellExecute = false;
            info.WorkingDirectory = string.IsNullOrEmpty(workingDirectory) ? file.DirectoryName : workingDirectory;
            return file.Execute(info);
        }

        /// <summary>
        ///     Executes the file
        /// </summary>
        /// <param name="file">File to execute</param>
        /// <param name="arguments">Arguments sent to the executable</param>
        /// <param name="domain">Domain of the user</param>
        /// <param name="password">Password of the user</param>
        /// <param name="user">User to run the file as</param>
        /// <param name="windowStyle">Window style</param>
        /// <param name="workingDirectory">Working directory</param>
        /// <returns>The process object created when the executable is started</returns>
        public static Process Execute(this string file, string arguments = "",
                                      string domain = "", string user = "", string password = "",
                                      ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal,
                                      string workingDirectory = "")
        {
            Guard.NotEmpty(file, "file");
            var info = new ProcessStartInfo {Arguments = arguments, Domain = domain, Password = new SecureString()};
            foreach (var Char in password)
                info.Password.AppendChar(Char);
            info.UserName = user;
            info.WindowStyle = windowStyle;
            info.UseShellExecute = false;
            info.WorkingDirectory = workingDirectory;
            return file.Execute(info);
        }

        /// <summary>
        ///     Executes the file
        /// </summary>
        /// <param name="file">File to execute</param>
        /// <param name="info">Info used to execute the file</param>
        /// <returns>The process object created when the executable is started</returns>
        public static Process Execute(this FileInfo file, ProcessStartInfo info)
        {
            Guard.NotNull(file, "file");
            if (!file.Exists)
                throw new FileNotFoundException("File note found", file.FullName);
            Guard.NotNull(info, "info");
            info.FileName = file.FullName;
            return Process.Start(info);
        }

        /// <summary>
        ///     Executes the file
        /// </summary>
        /// <param name="file">File to execute</param>
        /// <param name="info">Info used to execute the file</param>
        /// <returns>The process object created when the executable is started</returns>
        public static Process Execute(this string file, ProcessStartInfo info)
        {
            Guard.NotEmpty(file, "file");
            Guard.NotNull(info, "info");
            info.FileName = file;
            return Process.Start(info);
        }

        #endregion

        #region Save

        /// <summary>
        ///     Saves a string to a file
        /// </summary>
        /// <param name="file">File to save to</param>
        /// <param name="content">Content to save to the file</param>
        /// <param name="encodingUsing">Encoding that the content is using (defaults to ASCII)</param>
        /// <param name="mode">Mode for saving the file (defaults to Create)</param>
        /// <returns>The FileInfo object</returns>
        public static FileInfo Save(this FileInfo file, string content, FileMode mode = FileMode.Create,
                                    Encoding encodingUsing = null)
        {
            Guard.NotNull(file, "file");
            return file.Save(encodingUsing.NullCheck(new ASCIIEncoding()).GetBytes(content), mode);
        }

        /// <summary>
        ///     Saves a byte array to a file
        /// </summary>
        /// <param name="file">File to save to</param>
        /// <param name="content">Content to save to the file</param>
        /// <param name="mode">Mode for saving the file (defaults to Create)</param>
        /// <returns>The FileInfo object</returns>
        public static FileInfo Save(this FileInfo file, byte[] content, FileMode mode = FileMode.Create)
        {
            Guard.NotNull(file, "file");
            new DirectoryInfo(file.DirectoryName).Create();
            using (FileStream writer = file.Open(mode, FileAccess.Write))
            {
                writer.Write(content, 0, content.Length);
            }
            return file;
        }

        /// <summary>
        ///     Saves the string to the specified file
        /// </summary>
        /// <param name="location">Location to save the content to</param>
        /// <param name="content">Content to save the the file</param>
        /// <param name="encodingUsing">Encoding that the content is using (defaults to ASCII)</param>
        /// <param name="mode">Mode for saving the file (defaults to Create)</param>
        /// <returns>The FileInfo object associated with the location</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison",
            MessageId = "System.String.StartsWith(System.String,System.StringComparison)")]
        public static FileInfo Save(this string location, string content, FileMode mode = FileMode.Create,
                                    Encoding encodingUsing = null)
        {
            if (location.StartsWith("~", StringComparison.InvariantCulture))
            {
                if (HttpContext.Current == null)
                    location = location.Replace("~", AppDomain.CurrentDomain.BaseDirectory);
                else
                    location = HttpContext.Current.Server.MapPath(location);
            }
            return new FileInfo(location).Save(content, mode, encodingUsing);
        }

        /// <summary>
        ///     Saves a byte array to a file
        /// </summary>
        /// <param name="location">File to save to</param>
        /// <param name="content">Content to save to the file</param>
        /// <param name="mode">Mode for saving the file (defaults to Create)</param>
        /// <returns>The FileInfo object associated with the location</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison",
            MessageId = "System.String.StartsWith(System.String,System.StringComparison)")]
        public static FileInfo Save(this string location, byte[] content, FileMode mode = FileMode.Create)
        {
            if (location.StartsWith("~", StringComparison.InvariantCulture))
            {
                if (HttpContext.Current == null)
                    location = location.Replace("~", AppDomain.CurrentDomain.BaseDirectory);
                else
                    location = HttpContext.Current.Server.MapPath(location);
            }
            return new FileInfo(location).Save(content, mode);
        }

        #endregion

        #region SaveAsync

        /// <summary>
        ///     Saves a string to a file (asynchronously)
        /// </summary>
        /// <param name="file">File to save to</param>
        /// <param name="content">Content to save to the file</param>
        /// <param name="callBack">Call back function</param>
        /// <param name="stateObject">State object</param>
        /// <param name="encodingUsing">Encoding that the content is using (defaults to ASCII)</param>
        /// <param name="mode">Mode for saving the file (defaults to Create)</param>
        /// <returns>The FileInfo object</returns>
        public static FileInfo SaveAsync(this FileInfo file, string content, AsyncCallback callBack,
                                         object stateObject, FileMode mode = FileMode.Create,
                                         Encoding encodingUsing = null)
        {
            Guard.NotNull(file, "file");
            return file.SaveAsync(encodingUsing.NullCheck(new ASCIIEncoding()).GetBytes(content), callBack, stateObject,
                                  mode);
        }

        /// <summary>
        ///     Saves a byte array to a file (asynchronously)
        /// </summary>
        /// <param name="file">File to save to</param>
        /// <param name="content">Content to save to the file</param>
        /// <param name="callBack">Call back function</param>
        /// <param name="stateObject">State object</param>
        /// <param name="mode">Mode for saving the file (defaults to Create)</param>
        /// <returns>The FileInfo object</returns>
        public static FileInfo SaveAsync(this FileInfo file, byte[] content, AsyncCallback callBack,
                                         object stateObject, FileMode mode = FileMode.Create)
        {
            Guard.NotNull(file, "file");
            new DirectoryInfo(file.DirectoryName).Create();
            using (FileStream writer = file.Open(mode, FileAccess.Write))
            {
                writer.BeginWrite(content, 0, content.Length, callBack, stateObject);
            }
            return file;
        }

        #endregion

        #region SetAttributes

        /// <summary>
        ///     Sets the attributes of a file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="attributes">Attributes to set</param>
        /// <returns>The file info</returns>
        public static FileInfo SetAttributes(this FileInfo file, FileAttributes attributes)
        {
            Guard.Assert(file != null && file.Exists, new ArgumentNullException("file"));
            File.SetAttributes(file.FullName, attributes);
            return file;
        }

        #endregion

        #endregion
    }
}