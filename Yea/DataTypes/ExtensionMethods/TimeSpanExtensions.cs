#region Usings

using System;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    /// <summary>
    ///     TimeSpan extension methods
    /// </summary>
    public static class TimeSpanExtensions
    {
        #region Extension Methods

        #region DaysRemainder

        /// <summary>
        ///     Days in the TimeSpan minus the months and years
        /// </summary>
        /// <param name="span">TimeSpan to get the days from</param>
        /// <returns>The number of days minus the months and years that the TimeSpan has</returns>
        public static int DaysRemainder(this TimeSpan span)
        {
            return (DateTime.MinValue + span).Day - 1;
        }

        #endregion

        #region Months

        /// <summary>
        ///     Months in the TimeSpan
        /// </summary>
        /// <param name="span">TimeSpan to get the months from</param>
        /// <returns>The number of months that the TimeSpan has</returns>
        public static int Months(this TimeSpan span)
        {
            return (DateTime.MinValue + span).Month - 1;
        }

        #endregion

        #region Years

        /// <summary>
        ///     Years in the TimeSpan
        /// </summary>
        /// <param name="span">TimeSpan to get the years from</param>
        /// <returns>The number of years that the TimeSpan has</returns>
        public static int Years(this TimeSpan span)
        {
            return (DateTime.MinValue + span).Year - 1;
        }

        #endregion

        #region ToStringFull

        /// <summary>
        ///     Converts the input to a string in this format:
        ///     (Years) years, (Months) months, (DaysRemainder) days, (Hours) hours, (Minutes) minutes, (Seconds) seconds
        /// </summary>
        /// <returns>The TimeSpan as a string</returns>
        public static string ToStringFull()
        {
            return ToStringFull(new TimeSpan());
        }

        /// <summary>
        ///     Converts the input to a string in this format:
        ///     (Years) years, (Months) months, (DaysRemainder) days, (Hours) hours, (Minutes) minutes, (Seconds) seconds
        /// </summary>
        /// <param name="input">Input TimeSpan</param>
        /// <returns>The TimeSpan as a string</returns>
        public static string ToStringFull(this TimeSpan input)
        {
            string result = "";
            string splitter = "";
            if (input.Years() > 0)
            {
                result += input.Years() + " year" + (input.Years() > 1 ? "s" : "");
                splitter = ", ";
            }
            if (input.Months() > 0)
            {
                result += splitter + input.Months() + " month" + (input.Months() > 1 ? "s" : "");
                splitter = ", ";
            }
            if (input.DaysRemainder() > 0)
            {
                result += splitter + input.DaysRemainder() + " day" + (input.DaysRemainder() > 1 ? "s" : "");
                splitter = ", ";
            }
            if (input.Hours > 0)
            {
                result += splitter + input.Hours + " hour" + (input.Hours > 1 ? "s" : "");
                splitter = ", ";
            }
            if (input.Minutes > 0)
            {
                result += splitter + input.Minutes + " minute" + (input.Minutes > 1 ? "s" : "");
                splitter = ", ";
            }
            if (input.Seconds > 0)
            {
                result += splitter + input.Seconds + " second" + (input.Seconds > 1 ? "s" : "");
            }
            return result;
        }

        #endregion

        #endregion
    }
}