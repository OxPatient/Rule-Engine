#region Usings

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    /// <summary>
    ///     DateTime extension methods
    /// </summary>
    public static class DateTimeExtensions
    {
        #region ConvertToTimeZone

        /// <summary>
        ///     Converts a DateTime to a specific time zone
        /// </summary>
        /// <param name="date">DateTime to convert</param>
        /// <param name="timeZone">Time zone to convert to</param>
        /// <returns>The converted DateTime</returns>
        public static DateTime ConvertToTimeZone(this DateTime date, TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTime(date, timeZone);
        }

        #endregion

        #region DaysInMonth

        /// <summary>
        ///     Returns the number of days in the month
        /// </summary>
        /// <param name="date">Date to get the month from</param>
        /// <returns>The number of days in the month</returns>
        public static int DaysInMonth(this DateTime date)
        {
            Guard.NotNull(date, "date");
            return date.LastDayOfMonth().Day;
        }

        #endregion

        #region DaysLeftInMonth

        /// <summary>
        ///     Gets the number of days left in the month based on the date passed in
        /// </summary>
        /// <param name="date">The date to check against</param>
        /// <returns>The number of days left in a month</returns>
        public static int DaysLeftInMonth(this DateTime date)
        {
            Guard.NotNull(date, "date");
            return Thread.CurrentThread.CurrentCulture.Calendar.GetDaysInMonth(date.Year, date.Month) - date.Day;
        }

        #endregion

        #region DaysLeftInYear

        /// <summary>
        ///     Gets the number of days left in a year based on the date passed in
        /// </summary>
        /// <param name="date">The date to check against</param>
        /// <returns>The number of days left in a year</returns>
        public static int DaysLeftInYear(this DateTime date)
        {
            Guard.NotNull(date, "date");
            return Thread.CurrentThread.CurrentCulture.Calendar.GetDaysInYear(date.Year) - date.DayOfYear;
        }

        #endregion

        #region DaysLeftInWeek

        /// <summary>
        ///     Gets the number of days left in a week
        /// </summary>
        /// <param name="date">The date to check against</param>
        /// <returns>The number of days left in a week</returns>
        public static int DaysLeftInWeek(this DateTime date)
        {
            Guard.NotNull(date, "date");
            return 7 - ((int)date.DayOfWeek + 1);
        }

        #endregion

        #region EndOfDay

        /// <summary>
        ///     Returns the end of the day
        /// </summary>
        /// <param name="input">Input date</param>
        /// <returns>The end of the day</returns>
        public static DateTime EndOfDay(this DateTime input)
        {
            return new DateTime(input.Year, input.Month, input.Day, 23, 59, 59);
        }

        #endregion

        #region FirstDayOfMonth

        /// <summary>
        ///     Returns the first day of a month based on the date sent in
        /// </summary>
        /// <param name="date">Date to get the first day of the month from</param>
        /// <returns>The first day of the month</returns>
        public static DateTime FirstDayOfMonth(this DateTime date)
        {
            Guard.NotNull(date, "date");
            return new DateTime(date.Year, date.Month, 1);
        }

        #endregion

        #region FirstDayOfQuarter

        /// <summary>
        ///     Returns the first day of a quarter based on the date sent in
        /// </summary>
        /// <param name="date">Date to get the first day of the quarter from</param>
        /// <param name="quarter1Start">Beginning of the first quarter (defaults to the beginning of the year)</param>
        /// <returns>The first day of the quarter</returns>
        public static DateTime FirstDayOfQuarter(this DateTime date, DateTime quarter1Start = default(DateTime))
        {
            Guard.NotNull(date, "date");
            if (quarter1Start.IsDefault())
                quarter1Start = date.FirstDayOfYear();
            if (date.Between(quarter1Start, quarter1Start.AddMonths(3).AddDays(-1).EndOfDay()))
                return quarter1Start.Date;
            if (date.Between(quarter1Start.AddMonths(3), quarter1Start.AddMonths(6).AddDays(-1).EndOfDay()))
                return quarter1Start.AddMonths(3).Date;
            if (date.Between(quarter1Start.AddMonths(6), quarter1Start.AddMonths(9).AddDays(-1).EndOfDay()))
                return quarter1Start.AddMonths(6).Date;
            return quarter1Start.AddMonths(9).Date;
        }

        #endregion

        #region FirstDayOfYear

        /// <summary>
        ///     Returns the first day of a year based on the date sent in
        /// </summary>
        /// <param name="date">Date to get the first day of the year from</param>
        /// <returns>The first day of the year</returns>
        public static DateTime FirstDayOfYear(this DateTime date)
        {
            Guard.NotNull(date, "date");
            return new DateTime(date.Year, 1, 1);
        }

        #endregion

        #region FromUnixTime

        /// <summary>
        ///     Returns the Unix based date as a DateTime object
        /// </summary>
        /// <param name="date">Unix date to convert</param>
        /// <returns>The Unix Date in DateTime format</returns>
        public static DateTime FromUnixTime(this int date)
        {
            return
                new DateTime(
                    (date * TimeSpan.TicksPerSecond) + new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks,
                    DateTimeKind.Utc);
        }

        /// <summary>
        ///     Returns the Unix based date as a DateTime object
        /// </summary>
        /// <param name="date">Unix date to convert</param>
        /// <returns>The Unix Date in DateTime format</returns>
        public static DateTime FromUnixTime(this long date)
        {
            return
                new DateTime(
                    (date * TimeSpan.TicksPerSecond) + new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks,
                    DateTimeKind.Utc);
        }

        #endregion

        #region IsInFuture

        /// <summary>
        ///     Determines if the date is some time in the future
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsInFuture(this DateTime date)
        {
            Guard.NotNull(date, "date");
            return DateTime.Now < date;
        }

        #endregion

        #region IsInPast

        /// <summary>
        ///     Determines if the date is some time in the past
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsInPast(this DateTime date)
        {
            Guard.NotNull(date, "date");
            return DateTime.Now > date;
        }

        #endregion

        #region IsToday

        /// <summary>
        ///     Is this today?
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsToday(this DateTime date)
        {
            return (date.Date == DateTime.Today);
        }

        #endregion

        #region IsWeekDay

        /// <summary>
        ///     Determines if this is a week day
        /// </summary>
        /// <param name="date">Date to check against</param>
        /// <returns>Whether this is a week day or not</returns>
        public static bool IsWeekDay(this DateTime date)
        {
            Guard.NotNull(date, "date");
            return (int)date.DayOfWeek < 6 && (int)date.DayOfWeek > 0;
        }

        #endregion

        #region IsWeekEnd

        /// <summary>
        ///     Determines if this is a week end
        /// </summary>
        /// <param name="date">Date to check against</param>
        /// <returns>Whether this is a week end or not</returns>
        public static bool IsWeekEnd(this DateTime date)
        {
            Guard.NotNull(date, "date");
            return !IsWeekDay(date);
        }

        #endregion

        #region LastDayOfMonth

        /// <summary>
        ///     Returns the last day of the month based on the date sent in
        /// </summary>
        /// <param name="date">Date to get the last day from</param>
        /// <returns>The last day of the month</returns>
        public static DateTime LastDayOfMonth(this DateTime date)
        {
            Guard.NotNull(date, "date");
            return date.AddMonths(1).FirstDayOfMonth().AddDays(-1).Date;
        }

        #endregion

        #region LastDayOfQuarter

        /// <summary>
        ///     Returns the last day of a quarter based on the date sent in
        /// </summary>
        /// <param name="date">Date to get the last day of the quarter from</param>
        /// <param name="quarter1Start">Beginning of the first quarter (defaults to the beginning of the year)</param>
        /// <returns>The last day of the quarter</returns>
        public static DateTime LastDayOfQuarter(this DateTime date, DateTime quarter1Start = default(DateTime))
        {
            Guard.NotNull(date, "date");
            if (quarter1Start.IsDefault())
                quarter1Start = date.FirstDayOfYear();
            if (date.Between(quarter1Start, quarter1Start.AddMonths(3).AddDays(-1).EndOfDay()))
                return quarter1Start.AddMonths(3).AddDays(-1).Date;
            if (date.Between(quarter1Start.AddMonths(3), quarter1Start.AddMonths(6).AddDays(-1).EndOfDay()))
                return quarter1Start.AddMonths(6).AddDays(-1).Date;
            if (date.Between(quarter1Start.AddMonths(6), quarter1Start.AddMonths(9).AddDays(-1).EndOfDay()))
                return quarter1Start.AddMonths(9).AddDays(-1).Date;
            return quarter1Start.AddYears(1).AddDays(-1).Date;
        }

        #endregion

        #region LastDayOfYear

        /// <summary>
        ///     Returns the last day of the year based on the date sent in
        /// </summary>
        /// <param name="date">Date to get the last day from</param>
        /// <returns>The last day of the year</returns>
        public static DateTime LastDayOfYear(this DateTime date)
        {
            Guard.NotNull(date, "date");
            return new DateTime(date.Year, 12, 31);
        }

        #endregion

        #region LocalTimeZone

        /// <summary>
        ///     Gets the local time zone
        /// </summary>
        /// <param name="date">Date object</param>
        /// <returns>The local time zone</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "Date")]
        public static TimeZoneInfo LocalTimeZone(this DateTime date)
        {
            return TimeZoneInfo.Local;
        }

        #endregion

        #region RelativeTime

        /// <summary>
        ///     Converts the DateTime object to string describing, relatively how long ago or how far in the future
        ///     the input is based off of another DateTime object specified.
        ///     ex:
        ///     Input=March 21, 2013
        ///     Epoch=March 22, 2013
        ///     returns "1 day ago"
        ///     Input=March 22, 2013
        ///     Epoch=March 21, 2013
        ///     returns "1 day from now"
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="epoch">DateTime object that the input is comparred to</param>
        /// <returns>The difference between the input and epoch expressed as a string</returns>
        public static string RelativeTime(this DateTime input, DateTime epoch)
        {
            if (epoch == input)
                return "now";
            return epoch > input
                       ? (epoch - input).ToStringFull() + " ago"
                       : (input - epoch).ToStringFull() + " from now";
        }

        #endregion

        #region SetTime

        /// <summary>
        ///     Sets the time portion of a specific date
        /// </summary>
        /// <param name="date">Date input</param>
        /// <param name="hour">Hour to set</param>
        /// <param name="minutes">Minutes to set</param>
        /// <param name="seconds">Seconds to set</param>
        /// <returns>Sets the time portion of the specified date</returns>
        public static DateTime SetTime(this DateTime date, int hour, int minutes, int seconds)
        {
            return date.SetTime(new TimeSpan(hour, minutes, seconds));
        }

        /// <summary>
        ///     Sets the time portion of a specific date
        /// </summary>
        /// <param name="date">Date input</param>
        /// <param name="time">Time to set</param>
        /// <returns>Sets the time portion of the specified date</returns>
        public static DateTime SetTime(this DateTime date, TimeSpan time)
        {
            return date.Date.Add(time);
        }

        #endregion

        #region ToUnix

        /// <summary>
        ///     Returns the date in Unix format
        /// </summary>
        /// <param name="date">Date to convert</param>
        /// <returns>The date in Unix format</returns>
        public static int ToUnix(this DateTime date)
        {
            Guard.NotNull(date, "date");
            return
                (int)
                ((date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks /
                 TimeSpan.TicksPerSecond);
        }

        #endregion

        #region UTCOffset

        /// <summary>
        ///     Gets the UTC offset
        /// </summary>
        /// <param name="date">Date to get the offset of</param>
        /// <returns>UTC offset</returns>
        public static double UTCOffset(this DateTime date)
        {
            return (date - date.ToUniversalTime()).TotalHours;
        }

        #endregion

        /// <summary>
        ///     取得系统UTC Offset
        /// </summary>
        public static double UtcOffset
        {
            get { return DateTime.Now.Subtract(DateTime.UtcNow).TotalHours; }
        }

        /// <summary>
        ///     取得本周的第一天
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="cultureInfo">文化</param>
        /// <returns></returns>
        public static DateTime FirstDayOfWeek(this DateTime date, CultureInfo cultureInfo = null)
        {
            DayOfWeek firstWeekday = (cultureInfo ?? CultureInfo.CurrentCulture).DateTimeFormat.FirstDayOfWeek;
            while (date.DayOfWeek != firstWeekday)
                date = date.AddDays(-1);

            return date;
        }

        /// <summary>
        ///     取得本周的最后一天
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="cultureInfo">文化</param>
        /// <returns></returns>
        public static DateTime LastDayOfWeek(this DateTime date, CultureInfo cultureInfo = null)
        {
            return date.FirstDayOfWeek(cultureInfo).AddDays(6);
        }

        /// <summary>
        ///     取得本周的星期几
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="weekday">星期几</param>
        /// <param name="cultureInfo">文化</param>
        /// <returns></returns>
        public static DateTime DayOfWeek(this DateTime date, DayOfWeek weekday, CultureInfo cultureInfo = null)
        {
            DateTime firstDayOfWeek = date.FirstDayOfWeek(cultureInfo);
            while (firstDayOfWeek.DayOfWeek != weekday)
                firstDayOfWeek = firstDayOfWeek.AddDays(1);
            return firstDayOfWeek;
        }

        /// <summary>
        ///     取得下一个星期几
        /// </summary>
        /// <param name="currentDate">日期</param>
        /// <param name="weekday">星期几</param>
        /// <param name="includeCurrentDate">指定是否包含当前指定日期</param>
        /// <returns></returns>
        public static DateTime NextDayOfWeek(this DateTime currentDate, DayOfWeek weekday, bool includeCurrentDate = true)
        {
            var offsetDays = weekday - currentDate.DayOfWeek;

            if (offsetDays < 0 || (offsetDays == 0 && includeCurrentDate))
            {
                offsetDays += 7;
            }

            return currentDate.AddDays(offsetDays);
        }

        public static Date GetDate(this DateTime dateTime)
        {
            return new Date(dateTime);
        }

        public static Time GetTime(this DateTime dateTime)
        {
            return new Time(dateTime.TimeOfDay);
        }


        /// <summary>
        ///     增加若干星期
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="value">增加的星期数</param>
        /// <returns></returns>
        public static DateTime AddWeeks(this DateTime date, int value)
        {
            return date.AddDays(value * 7);
        }

        /// <summary>
        ///     返回当前日期所属年份的总天数
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public static int DaysOfYear(this DateTime date)
        {
            var first = new DateTime(date.Year, 1, 1);
            var last = new DateTime(date.Year + 1, 1, 1);
            return DaysBetween(first, last);
        }

        /// <summary>
        ///     返回两个日期之间相差的总天数
        /// </summary>
        /// <param name="fromDate">日期</param>
        /// <param name="toDate">日期</param>
        /// <returns></returns>
        public static int DaysBetween(this DateTime fromDate, DateTime toDate)
        {
            return Convert.ToInt32(toDate.Subtract(fromDate).TotalDays);
        }

        /// <summary>
        ///     返回当前年份的星期总数
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static int WeeksOfYear(this DateTime dateTime, CultureInfo cultureInfo = null)
        {
            CultureInfo culture = cultureInfo ?? CultureInfo.CurrentUICulture;
            Calendar calendar = culture.Calendar;
            DateTimeFormatInfo dateTimeFormat = culture.DateTimeFormat;

            return calendar.GetWeekOfYear(dateTime, dateTimeFormat.CalendarWeekRule, dateTimeFormat.FirstDayOfWeek);
        }


        /// <summary>
        ///     取得上一个星期几
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="weekday">星期几</param>
        /// <returns></returns>
        public static DateTime PreviousWeekday(this DateTime date, DayOfWeek weekday)
        {
            while (date.DayOfWeek != weekday)
                date = date.AddDays(-1);
            return date;
        }

        /// <summary>
        ///     判断日期是否相同，忽略时间部分
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="dateToCompare">日期</param>
        public static bool IsDateEqual(this DateTime date, DateTime dateToCompare)
        {
            return (date.Date == dateToCompare.Date);
        }

        /// <summary>
        ///     判断时间是否相同，忽略日期部分
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="timeToCompare">时间</param>
        /// <returns></returns>
        public static bool IsTimeEqual(this DateTime time, DateTime timeToCompare)
        {
            return (time.TimeOfDay == timeToCompare.TimeOfDay);
        }

        /// <summary>
        ///     取得与1970-01-01相差的时间间隔
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static TimeSpan TimeSpanSince1970(this DateTime datetime)
        {
            var date1970 = new DateTime(1970, 1, 1);
            return datetime.Subtract(date1970);
        }

        /// <summary>
        ///     判断是否为周末(周六，周日)
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public static bool IsWeekend(this DateTime date)
        {
            return date.DayOfWeek.EqualsAny(System.DayOfWeek.Saturday, System.DayOfWeek.Sunday);
        }


        public static bool IsLeapYear(this DateTime date)
        {
            return date.Year % 4 == 0 && (date.Year % 100 != 0 || date.Year % 400 == 0);
        }

        /// <summary>
        ///     取得当月天数
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int DaysCountOfMonth(this DateTime date)
        {
            if (IsLeapYear(date) && date.Month == 2)
                return 29;
            if (date.Month == 2)
                return 28;
            if (date.Month == 4 || date.Month == 6 || date.Month == 9 || date.Month == 11)
                return 30;
            return 31;
        }

        /// <summary>
        ///     取得当月的最后一个星期几
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="dayOfWeek">星期几</param>
        /// <returns></returns>
        public static DateTime LastWeekdayOfMonth(this DateTime date, DayOfWeek dayOfWeek)
        {
            DateTime dt = date.LastDayOfMonth();
            while (dt.DayOfWeek != dayOfWeek)
                dt = dt.AddDays(-1);
            return dt;
        }


        /// <summary>
        ///     取得当月的第一个星期几
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="dayOfWeek">星期几</param>
        /// <returns>The date time</returns>
        public static DateTime FirstWeekdayOfMonth(this DateTime date, DayOfWeek dayOfWeek)
        {
            DateTime dt = date.FirstDayOfMonth();
            while (dt.DayOfWeek != dayOfWeek)
                dt = dt.AddDays(1);
            return dt;
        }

        /// <summary>
        ///     判断是否为当月第一天
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsFirstDayOfMonth(this DateTime date)
        {
            DateTime dt = FirstDayOfMonth(date);
            return dt.Day == date.Day;
        }

        /// <summary>
        ///     判断是否为当月最后一天
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsLastDayOfMonth(this DateTime date)
        {
            int lastDayValueOfMonth = DaysCountOfMonth(date);
            return lastDayValueOfMonth == date.Day;
        }


        /// <summary>
        ///     转DateTimeOffset
        /// </summary>
        /// <param name="localDateTime">日期</param>
        /// <param name="localTimeZone">时区</param>
        /// <returns></returns>
        public static DateTimeOffset ToDateTimeOffset(this DateTime localDateTime, TimeZoneInfo localTimeZone = null)
        {
            if (localDateTime.Kind != DateTimeKind.Unspecified)
                localDateTime = new DateTime(localDateTime.Ticks, DateTimeKind.Unspecified);

            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, localTimeZone ?? TimeZoneInfo.Local);
        }

        /// <summary>
        ///     计算年龄
        /// </summary>
        /// <param name="dateOfBirth">生日</param>
        /// <returns>年龄</returns>
        public static int CalculateAge(this DateTime dateOfBirth)
        {
            return CalculateAge(dateOfBirth, DateTime.Today);
        }

        /// <summary>
        ///     计算年龄
        /// </summary>
        /// <param name="dateOfBirth">生日</param>
        /// <param name="referenceDate">参考日期</param>
        /// <returns>年龄</returns>
        public static int CalculateAge(this DateTime dateOfBirth, DateTime referenceDate)
        {
            int years = referenceDate.Year - dateOfBirth.Year;
            if (referenceDate.Month < dateOfBirth.Month ||
                (referenceDate.Month == dateOfBirth.Month && referenceDate.Day < dateOfBirth.Day))
                --years;
            return years;
        }

        #region DateDiff

        /// <summary>
        ///     时间间隔比较枚举
        /// </summary>
        public enum DateInterval
        {
            /// <summary>
            ///     天
            /// </summary>
            Day,

            /// <summary>
            ///     小时
            /// </summary>
            Hour,

            /// <summary>
            ///     分钟
            /// </summary>
            Minute,

            /// <summary>
            ///     月
            /// </summary>
            Month,

            /// <summary>
            ///     季度
            /// </summary>
            Quarter,

            /// <summary>
            ///     秒
            /// </summary>
            Second,

            /// <summary>
            ///     星期(=天/7)
            /// </summary>
            Weekday,

            /// <summary>
            ///     当年的第几星期
            /// </summary>
            WeekOfYear,

            /// <summary>
            ///     年
            /// </summary>
            Year
        }

        /// <summary>
        ///     时间间隔差
        /// </summary>
        /// <param name="interval">时间间隔比较枚举</param>
        /// <param name="sourceDate"></param>
        /// <param name="compareDate"></param>
        /// <returns></returns>
        public static long DateDiff(this DateTime sourceDate,
                                    DateTime compareDate,
                                    DateInterval interval)
        {
            if (interval == DateInterval.Year)
                return compareDate.Year - sourceDate.Year;

            if (interval == DateInterval.Month)
                return (compareDate.Month - sourceDate.Month) + (12 * (compareDate.Year - sourceDate.Year));

            TimeSpan ts = compareDate - sourceDate;

            if (interval == DateInterval.Day)
                return Round(ts.TotalDays);

            if (interval == DateInterval.Hour)
                return Round(ts.TotalHours);

            if (interval == DateInterval.Minute)
                return Round(ts.TotalMinutes);

            if (interval == DateInterval.Second)
                return Round(ts.TotalSeconds);

            if (interval == DateInterval.Weekday)
                return Round(ts.TotalDays / 7.0);

            if (interval == DateInterval.WeekOfYear)
            {
                while (compareDate.DayOfWeek != System.DayOfWeek.Monday)
                {
                    compareDate = compareDate.AddDays(-1);
                }
                while (sourceDate.DayOfWeek != System.DayOfWeek.Monday)
                {
                    sourceDate = sourceDate.AddDays(-1);
                }
                ts = compareDate - sourceDate;
                return Round(ts.TotalDays / 7.0);
            }

            if (interval == DateInterval.Quarter)
            {
                double d1Quarter = GetQuarter(sourceDate);
                double d2Quarter = GetQuarter(compareDate);
                double d1 = d2Quarter - d1Quarter;
                double d2 = (4 * (compareDate.Year - sourceDate.Year));
                return Round(d1 + d2);
            }

            return 0;
        }

        private static long Round(double dVal)
        {
            if (dVal >= 0)
                return (long)Math.Floor(dVal);
            return (long)Math.Ceiling(dVal);
        }

        /// <summary>
        ///     取得当前时间是在第？季度
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetQuarter(this DateTime date)
        {
            return (date.Month + 2) / 3;
        }

        #endregion
    }
}