#region Usings

using System;
using System.Text;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    /// <summary>
    ///     Value type extension methods
    /// </summary>
    public static class ValueTypeExtensions
    {
        #region Functions

        #region ToBool

        /// <summary>
        ///     Turns an int into a bool
        /// </summary>
        /// <param name="input">Int value</param>
        /// <returns>bool equivalent</returns>
        public static bool ToBool(this int input)
        {
            return input > 0;
        }

        #endregion

        #region ToInt

        /// <summary>
        ///     Converts the bool to an integer
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>The integer equivalent</returns>
        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }

        #endregion

        #region ToBase64String

        /// <summary>
        ///     Converts a byte array into a base 64 string
        /// </summary>
        /// <param name="input">Input array</param>
        /// <returns>The equivalent byte array in a base 64 string</returns>
        public static string ToBase64String(this byte[] input)
        {
            return input.IsNull() ? "" : Convert.ToBase64String(input);
        }

        #endregion

        #region ToEncodedString

        /// <summary>
        ///     Converts a byte array to a string
        /// </summary>
        /// <param name="input">input array</param>
        /// <param name="encodingUsing">The type of encoding the string is using (defaults to UTF8)</param>
        /// <param name="count">Number of bytes starting at the index to convert (use -1 for the entire array starting at the index)</param>
        /// <param name="index">Index to start at</param>
        /// <returns>string of the byte array</returns>
        public static string ToEncodedString(this byte[] input, Encoding encodingUsing = null, int index = 0,
                                             int count = -1)
        {
            if (input.IsNull())
                return "";
            if (count == -1)
                count = input.Length - index;
            return encodingUsing.NullCheck(new UTF8Encoding()).GetString(input, index, count);
        }

        #endregion

        #region IsControl

        /// <summary>
        ///     Is the character a control character
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsControl(this char value)
        {
            return char.IsControl(value);
        }

        #endregion

        #region IsDigit

        /// <summary>
        ///     Is the character a digit character
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsDigit(this char value)
        {
            return char.IsDigit(value);
        }

        #endregion

        #region IsHighSurrogate

        /// <summary>
        ///     Is the character a high surrogate character
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsHighSurrogate(this char value)
        {
            return char.IsHighSurrogate(value);
        }

        #endregion

        #region IsLetter

        /// <summary>
        ///     Is the character a letter character
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsLetter(this char value)
        {
            return char.IsLetter(value);
        }

        #endregion

        #region IsLetterOrDigit

        /// <summary>
        ///     Is the character a letter or digit character
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsLetterOrDigit(this char value)
        {
            return char.IsLetterOrDigit(value);
        }

        #endregion

        #region IsLower

        /// <summary>
        ///     Is the character a lower case character
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsLower(this char value)
        {
            return char.IsLower(value);
        }

        #endregion

        #region IsLowSurrogate

        /// <summary>
        ///     Is the character a low surrogate character
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsLowSurrogate(this char value)
        {
            return char.IsLowSurrogate(value);
        }

        #endregion

        #region IsNumber

        /// <summary>
        ///     Is the character a number character
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsNumber(this char value)
        {
            return char.IsNumber(value);
        }

        #endregion

        #region IsPunctuation

        /// <summary>
        ///     Is the character a punctuation character
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsPunctuation(this char value)
        {
            return char.IsPunctuation(value);
        }

        #endregion

        #region IsSurrogate

        /// <summary>
        ///     Is the character a surrogate character
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsSurrogate(this char value)
        {
            return char.IsSurrogate(value);
        }

        #endregion

        #region IsSymbol

        /// <summary>
        ///     Is the character a symbol character
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsSymbol(this char value)
        {
            return char.IsSymbol(value);
        }

        #endregion

        #region IsUpper

        /// <summary>
        ///     Is the character an upper case character
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsUpper(this char value)
        {
            return char.IsUpper(value);
        }

        #endregion

        #region IsWhiteSpace

        /// <summary>
        ///     Is the character a whitespace character
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsWhiteSpace(this char value)
        {
            return char.IsWhiteSpace(value);
        }

        #endregion

        #region IsUnicode

        /// <summary>
        ///     Determines if a byte array is unicode
        /// </summary>
        /// <param name="input">Input array</param>
        /// <returns>True if it's unicode, false otherwise</returns>
        public static bool IsUnicode(this byte[] input)
        {
            return input.IsNull() || input.ToEncodedString(new UnicodeEncoding()).IsUnicode();
        }

        #endregion

        #endregion
    }
}