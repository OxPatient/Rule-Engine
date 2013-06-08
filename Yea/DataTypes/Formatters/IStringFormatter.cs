namespace Yea.DataTypes.Formatters
{
    /// <summary>
    ///     String formatter
    /// </summary>
    public interface IStringFormatter
    {
        #region Functions

        /// <summary>
        ///     Formats the string based on the pattern
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="formatPattern">Format pattern</param>
        /// <returns>The formatted string</returns>
        string Format(string input, string formatPattern);

        #endregion
    }
}