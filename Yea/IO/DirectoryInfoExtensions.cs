#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.IO
{
    /// <summary>
    ///     Extension methods for <see cref="System.IO.DirectoryInfo" />
    /// </summary>
    public static class DirectoryInfoExtensions
    {
        #region Extension Methods

        #region CopyTo

        /// <summary>
        ///     Copies a directory to another location
        /// </summary>
        /// <param name="source">Source directory</param>
        /// <param name="destination">Destination directory</param>
        /// <param name="recursive">Should the copy be recursive</param>
        /// <param name="options">Options used in copying</param>
        /// <returns>The DirectoryInfo for the destination info</returns>
        public static DirectoryInfo CopyTo(this DirectoryInfo source, string destination, bool recursive = true,
                                           CopyOptions options = CopyOptions.CopyAlways)
        {
            Guard.NotNull(source, "source");
            Guard.Assert(source.Exists,
                         new DirectoryNotFoundException("Source directory " + source.FullName + " not found."));
            Guard.NotEmpty(destination, "destination");
            var destinationInfo = new DirectoryInfo(destination);
            destinationInfo.Create();
            foreach (var tempFile in source.EnumerateFiles())
            {
                if (options == CopyOptions.CopyAlways)
                {
                    tempFile.CopyTo(Path.Combine(destinationInfo.FullName, tempFile.Name), true);
                }
                else if (options == CopyOptions.CopyIfNewer)
                {
                    if (File.Exists(Path.Combine(destinationInfo.FullName, tempFile.Name)))
                    {
                        var fileInfo = new FileInfo(Path.Combine(destinationInfo.FullName, tempFile.Name));
                        if (fileInfo.LastWriteTime.CompareTo(tempFile.LastWriteTime) < 0)
                            tempFile.CopyTo(Path.Combine(destinationInfo.FullName, tempFile.Name), true);
                    }
                    else
                    {
                        tempFile.CopyTo(Path.Combine(destinationInfo.FullName, tempFile.Name), true);
                    }
                }
                else if (options == CopyOptions.DoNotOverwrite)
                {
                    tempFile.CopyTo(Path.Combine(destinationInfo.FullName, tempFile.Name), false);
                }
            }
            if (recursive)
            {
                foreach (var subDirectory in source.EnumerateDirectories())
                    subDirectory.CopyTo(Path.Combine(destinationInfo.FullName, subDirectory.Name), true, options);
            }
            return new DirectoryInfo(destination);
        }

        #endregion

        #region DeleteAll

        /// <summary>
        ///     Deletes directory and all content found within it
        /// </summary>
        /// <param name="info">Directory info object</param>
        public static void DeleteAll(this DirectoryInfo info)
        {
            Guard.NotNull(info, "info");
            if (!info.Exists)
                return;
            info.DeleteFiles();
            info.EnumerateDirectories().ForEach(x => x.DeleteAll());
            info.Delete(true);
        }

        #endregion

        #region DeleteDirectoriesNewerThan

        /// <summary>
        ///     Deletes directories newer than the specified date
        /// </summary>
        /// <param name="directory">Directory to look within</param>
        /// <param name="compareDate">The date to compare to</param>
        /// <param name="recursive">Is this a recursive call</param>
        /// <returns>Returns the directory object</returns>
        public static DirectoryInfo DeleteDirectoriesNewerThan(this DirectoryInfo directory, DateTime compareDate,
                                                               bool recursive = false)
        {
            Guard.NotNull(directory, "directory");
            Guard.Assert(directory.Exists, new DirectoryNotFoundException("directory"));
            directory.EnumerateDirectories("*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                     .Where(x => x.LastWriteTime > compareDate)
                     .ForEach(x => x.DeleteAll());
            return directory;
        }

        #endregion

        #region DeleteDirectoriesOlderThan

        /// <summary>
        ///     Deletes directories newer than the specified date
        /// </summary>
        /// <param name="directory">Directory to look within</param>
        /// <param name="compareDate">The date to compare to</param>
        /// <param name="recursive">Is this a recursive call</param>
        /// <returns>Returns the directory object</returns>
        public static DirectoryInfo DeleteDirectoriesOlderThan(this DirectoryInfo directory, DateTime compareDate,
                                                               bool recursive = false)
        {
            Guard.NotNull(directory, "directory");
            Guard.Assert(directory.Exists, new DirectoryNotFoundException("directory"));
            directory.EnumerateDirectories("*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                     .Where(x => x.LastWriteTime < compareDate)
                     .ForEach(x => x.DeleteAll());
            return directory;
        }

        #endregion

        #region DeleteFiles

        /// <summary>
        ///     Deletes files from a directory
        /// </summary>
        /// <param name="directory">Directory to delete the files from</param>
        /// <param name="recursive">Should this be recursive?</param>
        /// <returns>The directory that is sent in</returns>
        public static DirectoryInfo DeleteFiles(this DirectoryInfo directory, bool recursive = false)
        {
            Guard.NotNull(directory, "directory");
            Guard.Assert(directory.Exists, new DirectoryNotFoundException("directory"));
            directory.EnumerateFiles("*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                     .ForEach(x => x.Delete());
            return directory;
        }

        #endregion

        #region DeleteFilesNewerThan

        /// <summary>
        ///     Deletes files newer than the specified date
        /// </summary>
        /// <param name="directory">Directory to look within</param>
        /// <param name="compareDate">The date to compare to</param>
        /// <param name="recursive">Is this a recursive call</param>
        /// <returns>Returns the directory object</returns>
        public static DirectoryInfo DeleteFilesNewerThan(this DirectoryInfo directory, DateTime compareDate,
                                                         bool recursive = false)
        {
            Guard.NotNull(directory, "directory");
            Guard.Assert(directory.Exists, new DirectoryNotFoundException("directory"));
            directory.EnumerateFiles("*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                     .Where(x => x.LastWriteTime > compareDate)
                     .ForEach(x => x.Delete());
            return directory;
        }

        #endregion

        #region DeleteFilesOlderThan

        /// <summary>
        ///     Deletes files older than the specified date
        /// </summary>
        /// <param name="directory">Directory to look within</param>
        /// <param name="compareDate">The date to compare to</param>
        /// <param name="recursive">Is this a recursive call</param>
        /// <returns>Returns the directory object</returns>
        public static DirectoryInfo DeleteFilesOlderThan(this DirectoryInfo directory, DateTime compareDate,
                                                         bool recursive = false)
        {
            Guard.NotNull(directory, "directory");
            Guard.Assert(directory.Exists, new DirectoryNotFoundException("directory"));
            directory.EnumerateFiles("*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                     .Where(x => x.LastWriteTime < compareDate)
                     .ForEach(x => x.Delete());
            return directory;
        }

        #endregion

        #region DriveInfo

        /// <summary>
        ///     Gets the drive information for a directory
        /// </summary>
        /// <param name="directory">The directory to get the drive info of</param>
        /// <returns>The drive info connected to the directory</returns>
        public static DriveInfo DriveInfo(this DirectoryInfo directory)
        {
            Guard.NotNull(directory, "directory");
            return new DriveInfo(directory.Root.FullName);
        }

        #endregion

        #region EnumerateFiles

        /// <summary>
        ///     Enumerates the files within a directory
        /// </summary>
        /// <param name="directory">Directory to search in</param>
        /// <param name="searchPatterns">Patterns to search for</param>
        /// <param name="options">Search options</param>
        /// <returns>The enumerated files from the directory</returns>
        public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo directory,
                                                           IEnumerable<string> searchPatterns, SearchOption options)
        {
            var files = new List<FileInfo>();
            foreach (var searchPattern in searchPatterns)
                files.Add(directory.EnumerateFiles(searchPattern, options));
            return files;
        }

        #endregion

        #region Size

        /// <summary>
        ///     Gets the size of all files within a directory
        /// </summary>
        /// <param name="directory">Directory</param>
        /// <param name="searchPattern">Search pattern used to tell what files to include (defaults to all)</param>
        /// <param name="recursive">determines if this is a recursive call or not</param>
        /// <returns>The directory size</returns>
        public static long Size(this DirectoryInfo directory, string searchPattern = "*", bool recursive = false)
        {
            Guard.NotNull(directory, "directory");
            return directory.EnumerateFiles(searchPattern,
                                            recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                            .Sum(x => x.Length);
        }

        #endregion

        #region SetAttributes

        /// <summary>
        ///     Sets a directory's attributes
        /// </summary>
        /// <param name="directory">Directory</param>
        /// <param name="attributes">Attributes to set</param>
        /// <param name="recursive">Determines if this is a recursive call</param>
        /// <returns>The directory object</returns>
        public static DirectoryInfo SetAttributes(this DirectoryInfo directory, FileAttributes attributes,
                                                  bool recursive = false)
        {
            Guard.NotNull(directory, "directory");
            directory.EnumerateFiles()
                     .ForEach(x => x.SetAttributes(attributes));
            if (recursive)
                directory.EnumerateDirectories().ForEach(x => x.SetAttributes(attributes, true));
            return directory;
        }

        #endregion

        #endregion
    }
}