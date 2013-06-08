#region Usings

using System.IO;
using System.Linq;

#endregion

namespace Yea.IO
{
    /// <summary>
    ///     Extension methods for strings
    /// </summary>
    public static class StringExtensions
    {
        #region Functions

        #region RemoveIllegalDirectoryNameCharacters

        /// <summary>
        ///     Removes illegal characters from a directory
        /// </summary>
        /// <param name="directoryName">Directory name</param>
        /// <param name="replacementChar">Replacement character</param>
        /// <returns>DirectoryName with all illegal characters replaced with ReplacementChar</returns>
        public static string RemoveIllegalDirectoryNameCharacters(this string directoryName, char replacementChar = '_')
        {
            if (string.IsNullOrEmpty(directoryName))
                return directoryName;
            return Path.GetInvalidPathChars()
                       .Aggregate(directoryName, (current, Char) => current.Replace(Char, replacementChar));
        }

        #endregion

        #region RemoveIllegalFileNameCharacters

        /// <summary>
        ///     Removes illegal characters from a file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="replacementChar">Replacement character</param>
        /// <returns>FileName with all illegal characters replaced with ReplacementChar</returns>
        public static string RemoveIllegalFileNameCharacters(this string fileName, char replacementChar = '_')
        {
            if (string.IsNullOrEmpty(fileName))
                return fileName;
            return Path.GetInvalidFileNameChars()
                       .Aggregate(fileName, (current, Char) => current.Replace(Char, replacementChar));
        }

        #endregion

        #endregion
    }
}