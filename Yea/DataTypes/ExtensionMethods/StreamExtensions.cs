#region Usings

using System.IO;
using System.Text;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    /// <summary>
    ///     Extension methods for Streams
    /// </summary>
    public static class StreamExtensions
    {
        #region Functions

        #region ReadAllBinary

        /// <summary>
        ///     Takes all of the data in the stream and returns it as an array of bytes
        /// </summary>
        /// <param name="input">Input stream</param>
        /// <returns>A byte array</returns>
        public static byte[] ReadAllBinary(this Stream input)
        {
            Guard.NotNull(input, "input");
            var tempInput = input as MemoryStream;
            if (tempInput != null)
                return tempInput.ToArray();
            var buffer = new byte[1024];
            byte[] returnValue;
            using (var temp = new MemoryStream())
            {
                while (true)
                {
                    int count = input.Read(buffer, 0, buffer.Length);
                    if (count <= 0)
                    {
                        returnValue = temp.ToArray();
                        break;
                    }
                    temp.Write(buffer, 0, count);
                }
            }
            return returnValue;
        }

        #endregion

        #region ReadAll

        /// <summary>
        ///     Takes all of the data in the stream and returns it as a string
        /// </summary>
        /// <param name="input">Input stream</param>
        /// <param name="encodingUsing">Encoding that the string should be in (defaults to UTF8)</param>
        /// <returns>A string containing the content of the stream</returns>
        public static string ReadAll(this Stream input, Encoding encodingUsing = null)
        {
            return input.ReadAllBinary().ToEncodedString(encodingUsing);
        }

        #endregion

        #endregion
    }
}