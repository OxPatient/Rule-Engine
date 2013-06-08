#region Usings

using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Yea.DataTypes.Formatters;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    /// <summary>
    ///     String and StringBuilder extensions
    /// </summary>
    public static class StringExtensions
    {
        #region Functions

        #region AlphaCharactersOnly

        /// <summary>
        ///     Keeps only alpha characters
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>the string only containing alpha characters</returns>
        public static string AlphaCharactersOnly(this string input)
        {
            return input.KeepFilterText("[a-zA-Z]");
        }

        #endregion

        #region AlphaNumericOnly

        /// <summary>
        ///     Keeps only alphanumeric characters
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>the string only containing alphanumeric characters</returns>
        public static string AlphaNumericOnly(this string input)
        {
            return input.KeepFilterText("[a-zA-Z0-9]");
        }

        #endregion

        #region AppendLineFormat

        /// <summary>
        ///     Does an AppendFormat and then an AppendLine on the StringBuilder
        /// </summary>
        /// <param name="builder">Builder object</param>
        /// <param name="format">Format string</param>
        /// <param name="objects">Objects to format</param>
        /// <returns>The StringBuilder passed in</returns>
        public static StringBuilder AppendLineFormat(this StringBuilder builder, string format, params object[] objects)
        {
            return builder.AppendFormat(CultureInfo.InvariantCulture, format, objects).AppendLine();
        }

        #endregion

        #region Center

        /// <summary>
        ///     Centers the input string (if it's longer than the length) and pads it using the padding string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <param name="padding"></param>
        /// <returns>The centered string</returns>
        public static string Center(this string input, int length, string padding = " ")
        {
            if (input.IsNullOrEmpty())
                input = "";
            string output = "";
            for (int x = 0; x < (length - input.Length)/2; ++x)
            {
                output += padding[x%padding.Length];
            }
            output += input;
            for (int x = 0; x < (length - input.Length)/2; ++x)
            {
                output += padding[x%padding.Length];
            }
            return output;
        }

        #endregion

        #region Encode

        /// <summary>
        ///     Converts a string to a string of another encoding
        /// </summary>
        /// <param name="input">input string</param>
        /// <param name="originalEncodingUsing">The type of encoding the string is currently using (defaults to ASCII)</param>
        /// <param name="encodingUsing">The type of encoding the string is converted into (defaults to UTF8)</param>
        /// <returns>string of the byte array</returns>
        public static string Encode(this string input, Encoding originalEncodingUsing = null,
                                    Encoding encodingUsing = null)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            originalEncodingUsing = originalEncodingUsing.NullCheck(new ASCIIEncoding());
            encodingUsing = encodingUsing.NullCheck(new UTF8Encoding());
            return Encoding.Convert(originalEncodingUsing, encodingUsing, input.ToByteArray(originalEncodingUsing))
                           .ToEncodedString(encodingUsing);
        }

        #endregion

        #region ExpandTabs

        /// <summary>
        ///     Expands tabs and replaces them with spaces
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="tabSize">Number of spaces</param>
        /// <returns>The input string, with the tabs replaced with spaces</returns>
        public static string ExpandTabs(this string input, int tabSize = 4)
        {
            if (input.IsNullOrEmpty())
                return input;
            string spaces = "";
            for (int x = 0; x < tabSize; ++x)
                spaces += " ";
            return input.Replace("\t", spaces);
        }

        #endregion

        #region FilterOutText

        /// <summary>
        ///     Removes the filter text from the input.
        /// </summary>
        /// <param name="input">Input text</param>
        /// <param name="filter">Regex expression of text to filter out</param>
        /// <returns>The input text minus the filter text.</returns>
        public static string FilterOutText(this string input, string filter)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return string.IsNullOrEmpty(filter) ? input : new Regex(filter).Replace(input, "");
        }

        #endregion

        #region FormatString

        /// <summary>
        ///     Formats a string based on a format string passed in.
        ///     The default formatter uses the following format:
        ///     # = digits
        ///     @ = alpha characters
        ///     \ = escape char
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="format">Format of the output string</param>
        /// <param name="provider">String formatter provider (defaults to GenericStringFormatter)</param>
        /// <returns>The formatted string</returns>
        public static string FormatString(this string input, string format, IStringFormatter provider = null)
        {
            return provider.NullCheck(new GenericStringFormatter()).Format(input, format);
        }

        /// <summary>
        ///     Formats a string based on the object's properties
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="Object">Object to use to format the string</param>
        /// <param name="startSeperator">Seperator character/string to use to describe the start of the property name</param>
        /// <param name="endSeperator">Seperator character/string to use to describe the end of the property name</param>
        /// <returns>The formatted string</returns>
        public static string FormatString(this string input, object Object, string startSeperator = "{",
                                          string endSeperator = "}")
        {
            if (Object.IsNull())
                return input;
            Object.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                  .Where(x => x.CanRead)
                  .ForEach(x =>
                      {
                          var value = x.GetValue(Object, null);
                          input = input.Replace(startSeperator + x.Name + endSeperator,
                                                value.IsNull() ? "" : value.ToString());
                      });
            return input;
        }

        /// <summary>
        ///     Formats a string based on the key/value pairs that are sent in
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="pairs">Key/value pairs. Replaces the key with the corresponding value.</param>
        /// <returns>The string after the changes have been made</returns>
        public static string FormatString(this string input, params KeyValuePair<string, string>[] pairs)
        {
            if (input.IsNullOrEmpty())
                return input;
            return pairs.Aggregate(input, (current, pair) => current.Replace(pair.Key, pair.Value));
        }

        #endregion

        #region FromBase64

        /// <summary>
        ///     Converts base 64 string based on the encoding passed in
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="encodingUsing">The type of encoding the string is using (defaults to UTF8)</param>
        /// <returns>string in the encoding format</returns>
        public static string FromBase64(this string input, Encoding encodingUsing)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            byte[] tempArray = Convert.FromBase64String(input);
            return encodingUsing.NullCheck(() => new UTF8Encoding()).GetString(tempArray);
        }

        /// <summary>
        ///     Converts base 64 string to a byte array
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>A byte array equivalent of the base 64 string</returns>
        public static byte[] FromBase64(this string input)
        {
            return string.IsNullOrEmpty(input) ? new byte[0] : Convert.FromBase64String(input);
        }

        #endregion

        #region IsCreditCard

        /// <summary>
        ///     Checks if a credit card number is valid
        /// </summary>
        /// <param name="creditCardNumber">Number to check</param>
        /// <returns>True if it is valid, false otherwise</returns>
        public static bool IsCreditCard(this string creditCardNumber)
        {
            long checkSum = 0;
            creditCardNumber = creditCardNumber.Replace("-", "").Reverse();
            for (int x = 0; x < creditCardNumber.Length; ++x)
            {
                if (!creditCardNumber[x].IsDigit())
                    return false;
                int value = (creditCardNumber[x] - '0')*(x%2 == 1 ? 2 : 1);
                while (value > 0)
                {
                    checkSum += value%10;
                    value /= 10;
                }
            }
            return (checkSum%10) == 0;
        }

        #endregion

        #region IsAnagram

        /// <summary>
        ///     Determines if the two strings are anagrams or not
        /// </summary>
        /// <param name="input1">Input 1</param>
        /// <param name="input2">Input 2</param>
        /// <returns>True if they are anagrams, false otherwise</returns>
        public static bool IsAnagram(this string input1, string input2)
        {
            return String.Equals(new string(input1.OrderBy(x => x).ToArray()),
                                 new string(input2.OrderBy(x => x).ToArray()), StringComparison.CurrentCulture);
        }

        #endregion

        #region IsUnicode

        /// <summary>
        ///     Determines if a string is unicode
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>True if it's unicode, false otherwise</returns>
        public static bool IsUnicode(this string input)
        {
            return string.IsNullOrEmpty(input) ||
                   !String.Equals(Regex.Replace(input, @"[^\u0000-\u007F]", ""), input, StringComparison.CurrentCulture);
        }

        #endregion

        #region KeepFilterText

        /// <summary>
        ///     Removes everything that is not in the filter text from the input.
        /// </summary>
        /// <param name="input">Input text</param>
        /// <param name="filter">Regex expression of text to keep</param>
        /// <returns>The input text minus everything not in the filter text.</returns>
        public static string KeepFilterText(this string input, string filter)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(filter))
                return "";
            var tempRegex = new Regex(filter);
            MatchCollection collection = tempRegex.Matches(input);
            var builder = new StringBuilder();
            foreach (Match match in collection)
                builder.Append(match.Value);
            return builder.ToString();
        }

        #endregion

        #region Left

        /// <summary>
        ///     Gets the first x number of characters from the left hand side
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="length">x number of characters to return</param>
        /// <returns>The resulting string</returns>
        public static string Left(this string input, int length)
        {
            return string.IsNullOrEmpty(input) ? "" : input.Substring(0, input.Length > length ? length : input.Length);
        }

        #endregion

        #region LevenshteinDistance

        /// <summary>
        ///     Calculates the Levenshtein distance
        /// </summary>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <returns>The Levenshtein distance</returns>
        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body")]
        public static int LevenshteinDistance(this string value1, string value2)
        {
            var matrix = new int[value1.Length + 1,value2.Length + 1];
            for (int x = 0; x <= value1.Length; ++x)
                matrix[x, 0] = x;
            for (int x = 0; x <= value2.Length; ++x)
                matrix[0, x] = x;

            for (int x = 1; x <= value1.Length; ++x)
            {
                for (int y = 1; y <= value2.Length; ++y)
                {
                    int cost = value1[x - 1] == value2[y - 1] ? 0 : 1;
                    matrix[x, y] = new[] {matrix[x - 1, y] + 1, matrix[x, y - 1] + 1, matrix[x - 1, y - 1] + cost}.Min();
                    if (x > 1 && y > 1 && value1[x - 1] == value2[y - 2] && value1[x - 2] == value2[y - 1])
                        matrix[x, y] = new[] {matrix[x, y], matrix[x - 2, y - 2] + cost}.Min();
                }
            }

            return matrix[value1.Length, value2.Length];
        }

        #endregion

        #region MaskLeft

        /// <summary>
        ///     Masks characters to the left ending at a specific character
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="endPosition">End position (counting from the left)</param>
        /// <param name="mask">Mask character to use</param>
        /// <returns>The masked string</returns>
        public static string MaskLeft(this string input, int endPosition = 4, char mask = '#')
        {
            string appending = "";
            for (int x = 0; x < endPosition; ++x)
                appending += mask;
            return appending + input.Remove(0, endPosition);
        }

        #endregion

        #region MaskRight

        /// <summary>
        ///     Masks characters to the right starting at a specific character
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="startPosition">Start position (counting from the left)</param>
        /// <param name="mask">Mask character to use</param>
        /// <returns>The masked string</returns>
        public static string MaskRight(this string input, int startPosition = 4, char mask = '#')
        {
            if (startPosition > input.Length)
                return input;
            string appending = "";
            for (int x = 0; x < input.Length - startPosition; ++x)
                appending += mask;
            return input.Remove(startPosition) + appending;
        }

        #endregion

        #region NextSequence

        /// <summary>
        ///     Function that is useful for generating a string in a series. so a becomes b, b becomes c, etc.
        ///     and after hitting the max character, it goes to two characters (so ~ becomes aa, then ab, ac, etc).
        /// </summary>
        /// <param name="sequence">Current sequence</param>
        /// <param name="min">Min character</param>
        /// <param name="max">Max character</param>
        /// <returns>The next item in the sequence</returns>
        public static string NextSequence(this string sequence, char min = ' ', char max = '~')
        {
            byte[] values = sequence.ToByteArray();
            var maxValue = (byte) max;
            byte remainder = 1;
            for (int x = sequence.Length - 1; x >= 0; --x)
            {
                values[x] += remainder;
                remainder = 0;
                if (values[x] > maxValue)
                {
                    remainder = 1;
                    values[x] = (byte) min;
                }
                else
                    break;
            }
            if (remainder == 1)
                return min + values.ToEncodedString();
            return values.ToEncodedString();
        }

        #endregion

        #region NumericOnly

        /// <summary>
        ///     Keeps only numeric characters
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="keepNumericPunctuation">Determines if decimal places should be kept</param>
        /// <returns>the string only containing numeric characters</returns>
        public static string NumericOnly(this string input, bool keepNumericPunctuation = true)
        {
            return keepNumericPunctuation ? input.KeepFilterText(@"[0-9\.]") : input.KeepFilterText("[0-9]");
        }

        #endregion

        #region NumberTimesOccurs

        /// <summary>
        ///     returns the number of times a string occurs within the text
        /// </summary>
        /// <param name="input">input text</param>
        /// <param name="match">The string to match (can be regex)</param>
        /// <returns>The number of times the string occurs</returns>
        public static int NumberTimesOccurs(this string input, string match)
        {
            return string.IsNullOrEmpty(input) ? 0 : new Regex(match).Matches(input).Count;
        }

        #endregion

        #region Pluralize

        /// <summary>
        ///     Pluralizes a word
        /// </summary>
        /// <param name="word">Word to pluralize</param>
        /// <param name="culture">Culture info used to pluralize the word (defaults to current culture)</param>
        /// <returns>The word pluralized</returns>
        public static string Pluralize(this string word, CultureInfo culture = null)
        {
            if (word.IsNullOrEmpty())
                return "";
            culture = culture.NullCheck(CultureInfo.CurrentCulture);
            return PluralizationService.CreateService(culture).Pluralize(word);
        }

        #endregion

        #region RegexFormat

        /// <summary>
        ///     Uses a regex to format the input string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="format">Regex string used to</param>
        /// <param name="outputFormat">Output format</param>
        /// <param name="options">Regex options</param>
        /// <returns>The input string formatted by using the regex string</returns>
        public static string RegexFormat(this string input, string format, string outputFormat,
                                         RegexOptions options = RegexOptions.None)
        {
            Guard.NotEmpty(input, "input");
            return Regex.Replace(input, format, outputFormat, options);
        }

        #endregion

        #region RemoveExtraSpaces

        /// <summary>
        ///     Removes multiple spaces from a string and replaces it with a single space
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>The input string with multiple spaces replaced with a single space</returns>
        public static string RemoveExtraSpaces(this string input)
        {
            return new Regex(@"[ ]{2,}", RegexOptions.None).Replace(input, " ");
        }

        #endregion

        #region Reverse

        /// <summary>
        ///     Reverses a string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>The reverse of the input string</returns>
        public static string Reverse(this string input)
        {
            return new string(input.Reverse<char>().ToArray());
        }

        #endregion

        #region Right

        /// <summary>
        ///     Gets the last x number of characters from the right hand side
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="length">x number of characters to return</param>
        /// <returns>The resulting string</returns>
        public static string Right(this string input, int length)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            length = input.Length > length ? length : input.Length;
            return input.Substring(input.Length - length, length);
        }

        #endregion

        #region Singularize

        /// <summary>
        ///     Singularizes a word
        /// </summary>
        /// <param name="word">Word to singularize</param>
        /// <param name="culture">Culture info used to singularize the word (defaults to current culture)</param>
        /// <returns>The word singularized</returns>
        public static string Singularize(this string word, CultureInfo culture = null)
        {
            if (word.IsNullOrEmpty())
                return "";
            culture = culture.NullCheck(CultureInfo.CurrentCulture);
            return PluralizationService.CreateService(culture).Singularize(word);
        }

        #endregion

        #region StripLeft

        /// <summary>
        ///     Strips out any of the characters specified starting on the left side of the input string (stops when a character not in the list is found)
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="characters">Characters to string (defaults to a space)</param>
        /// <returns>The Input string with specified characters stripped out</returns>
        public static string StripLeft(this string input, string characters = " ")
        {
            if (input.IsNullOrEmpty())
                return input;
            if (characters.IsNullOrEmpty())
                return input;
            if (characters != null)
                return input.SkipWhile(characters.Contains).ToString(x => x.ToString(CultureInfo.InvariantCulture), "");
            return null;
        }

        #endregion

        #region StripRight

        /// <summary>
        ///     Strips out any of the characters specified starting on the right side of the input string (stops when a character not in the list is found)
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="characters">Characters to string (defaults to a space)</param>
        /// <returns>The Input string with specified characters stripped out</returns>
        public static string StripRight(this string input, string characters = " ")
        {
            if (input.IsNullOrEmpty())
                return input;
            if (characters.IsNullOrEmpty())
                return input;
            int position = input.Length - 1;
            for (int x = input.Length - 1; x >= 0; --x)
            {
                if (!characters.Contains(input[x]))
                {
                    position = x + 1;
                    break;
                }
            }
            return input.Left(position);
        }

        #endregion

        #region StripIllegalXML

        /// <summary>
        ///     Strips illegal characters for XML content
        /// </summary>
        /// <param name="content">Content</param>
        /// <returns>The stripped string</returns>
        public static string StripIllegalXml(this string content)
        {
            if (content.IsNullOrEmpty())
                return "";
            var builder = new StringBuilder();
            foreach (var Char in content)
            {
                if (Char == 0x9
                    || Char == 0xA
                    || Char == 0xD
                    || (Char >= 0x20 && Char <= 0xD7FF)
                    || (Char >= 0xE000 && Char <= 0xFFFD))
                    builder.Append(Char);
            }
            return builder.ToString().Replace('\u2013', '-').Replace('\u2014', '-')
                          .Replace('\u2015', '-').Replace('\u2017', '_').Replace('\u2018', '\'')
                          .Replace('\u2019', '\'').Replace('\u201a', ',').Replace('\u201b', '\'')
                          .Replace('\u201c', '\"').Replace('\u201d', '\"').Replace('\u201e', '\"')
                          .Replace("\u2026", "...").Replace('\u2032', '\'').Replace('\u2033', '\"')
                          .Replace("`", "\'")
                          .Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
                          .Replace("\"", "&quot;").Replace("\'", "&apos;");
        }

        #endregion

        #region ToBase64

        /// <summary>
        ///     Converts from the specified encoding to a base 64 string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="originalEncodingUsing">The type of encoding the string is using (defaults to UTF8)</param>
        /// <returns>Bas64 string</returns>
        public static string ToBase64(this string input, Encoding originalEncodingUsing = null)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            byte[] tempArray = originalEncodingUsing.NullCheck(new UTF8Encoding()).GetBytes(input);
            return Convert.ToBase64String(tempArray);
        }

        #endregion

        #region ToByteArray

        /// <summary>
        ///     Converts a string to a byte array
        /// </summary>
        /// <param name="input">input string</param>
        /// <param name="encodingUsing">The type of encoding the string is using (defaults to UTF8)</param>
        /// <returns>the byte array representing the string</returns>
        public static byte[] ToByteArray(this string input, Encoding encodingUsing = null)
        {
            return string.IsNullOrEmpty(input) ? null : encodingUsing.NullCheck(new UTF8Encoding()).GetBytes(input);
        }

        #endregion

        #region ToFirstCharacterUpperCase

        /// <summary>
        ///     Takes the first character of an input string and makes it uppercase
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>String with the first character capitalized</returns>
        public static string ToFirstCharacterUpperCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            char[] inputChars = input.ToCharArray();
            for (int x = 0; x < inputChars.Length; ++x)
            {
                if (inputChars[x] != ' ' && inputChars[x] != '\t')
                {
                    inputChars[x] = char.ToUpper(inputChars[x], CultureInfo.InvariantCulture);
                    break;
                }
            }
            return new string(inputChars);
        }

        #endregion

        #region ToSentenceCapitalize

        /// <summary>
        ///     Capitalizes each sentence within the string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>String with each sentence capitalized</returns>
        public static string ToSentenceCapitalize(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            string[] seperator = {".", "?", "!"};
            string[] inputStrings = input.Split(seperator, StringSplitOptions.None);
            for (int x = 0; x < inputStrings.Length; ++x)
            {
                if (!string.IsNullOrEmpty(inputStrings[x]))
                {
                    var tempRegex = new Regex(inputStrings[x]);
                    inputStrings[x] = inputStrings[x].ToFirstCharacterUpperCase();
                    input = tempRegex.Replace(input, inputStrings[x]);
                }
            }
            return input;
        }

        #endregion

        #region ToTitleCase

        /// <summary>
        ///     Capitalizes the first character of each word
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>String with each word capitalized</returns>
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            string[] seperator = {" ", ".", "\t", Environment.NewLine, "!", "?"};
            string[] inputStrings = input.Split(seperator, StringSplitOptions.None);
            for (int x = 0; x < inputStrings.Length; ++x)
            {
                if (!string.IsNullOrEmpty(inputStrings[x])
                    && inputStrings[x].Length > 3)
                {
                    var tempRegex =
                        new Regex(inputStrings[x].Replace(")", @"\)").Replace("(", @"\(").Replace("*", @"\*"));
                    inputStrings[x] = inputStrings[x].ToFirstCharacterUpperCase();
                    input = tempRegex.Replace(input, inputStrings[x]);
                }
            }
            return input;
        }

        #endregion

        #endregion
    }
}