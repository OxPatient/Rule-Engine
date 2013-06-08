#region Usings

using System;
using System.Text;

#endregion

namespace Yea.DataTypes.Formatters
{
    /// <summary>
    ///     Generic string formatter
    /// </summary>
    public class GenericStringFormatter : IFormatProvider, ICustomFormatter, IStringFormatter
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        public GenericStringFormatter()
        {
            DigitChar = '#';
            AlphaChar = '@';
            EscapeChar = '\\';
        }

        #endregion

        #region IFormatProvider Functions

        /// <summary>
        ///     Gets the format associated with the type
        /// </summary>
        /// <param name="formatType">Format type</param>
        /// <returns>The appropriate formatter based on the type</returns>
        public object GetFormat(Type formatType)
        {
            return formatType == typeof (ICustomFormatter) ? this : null;
        }

        #endregion

        #region ICustomFormatter Functions

        /// <summary>
        ///     Formats the string
        /// </summary>
        /// <param name="format">Format to use</param>
        /// <param name="arg">Argument object to use</param>
        /// <param name="formatProvider">Format provider to use</param>
        /// <returns>The formatted string</returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            return Format(arg.ToString(), format);
        }

        #endregion

        #region Functions

        #region Format

        /// <summary>
        ///     Formats the string based on the pattern
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="formatPattern">Format pattern</param>
        /// <returns>The formatted string</returns>
        public string Format(string input, string formatPattern)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (!IsValid(formatPattern))
                throw new ArgumentException("FormatPattern is not valid");

            var returnValue = new StringBuilder();
            for (int x = 0; x < formatPattern.Length; ++x)
            {
                if (formatPattern[x] == EscapeChar)
                {
                    ++x;
                    returnValue.Append(formatPattern[x]);
                }
                else
                {
                    char nextValue;
                    input = GetMatchingInput(input, formatPattern[x], out nextValue);
                    if (nextValue != char.MinValue)
                        returnValue.Append(nextValue);
                }
            }
            return returnValue.ToString();
        }

        #endregion

        #region GetMatchingInput

        /// <summary>
        ///     Gets matching input
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="formatChar">Current format character</param>
        /// <param name="matchChar">The matching character found</param>
        /// <returns>The remainder of the input string left</returns>
        private string GetMatchingInput(string input, char formatChar, out char matchChar)
        {
            bool digit = formatChar == DigitChar;
            bool alpha = formatChar == AlphaChar;
            if (!digit && !alpha)
            {
                matchChar = formatChar;
                return input;
            }
            int index = 0;
            matchChar = char.MinValue;
            for (int x = 0; x < input.Length; ++x)
            {
                if ((digit && char.IsDigit(input[x])) || (alpha && char.IsLetter(input[x])))
                {
                    matchChar = input[x];
                    index = x + 1;
                    break;
                }
            }
            return input.Substring(index);
        }

        #endregion

        #region IsValid

        /// <summary>
        ///     Checks if the format pattern is valid
        /// </summary>
        /// <param name="formatPattern">Format pattern</param>
        /// <returns>Returns true if it's valid, otherwise false</returns>
        private bool IsValid(string formatPattern)
        {
            bool escapeCharFound = false;
            foreach (var t in formatPattern)
            {
                if (escapeCharFound && t != DigitChar
                    && t != AlphaChar
                    && t != EscapeChar)
                    return false;
                if (escapeCharFound)
                    escapeCharFound = false;
                else
                {
                    if (t == EscapeChar)
                        escapeCharFound = true;
                }
            }
            if (escapeCharFound)
                return false;
            return true;
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        ///     Represents digits (defaults to #)
        /// </summary>
        public char DigitChar { get; protected set; }

        /// <summary>
        ///     Represents alpha characters (defaults to @)
        /// </summary>
        public char AlphaChar { get; protected set; }

        /// <summary>
        ///     Represents the escape character (defaults to \)
        /// </summary>
        public char EscapeChar { get; protected set; }

        #endregion
    }
}