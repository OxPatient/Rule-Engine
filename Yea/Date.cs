#region Usings

using System;

#endregion

namespace Yea
{
    [Serializable]
    public struct Date : IComparable, IFormattable, IComparable<Date>, IEquatable<Date>
    {
        #region operators

        public static bool operator >(Date d1, Date d2)
        {
            return d1.CompareTo(d2) > 0;
        }

        public static bool operator <(Date d1, Date d2)
        {
            return d1.CompareTo(d2) < 0;
        }

        public static bool operator >=(Date d1, Date d2)
        {
            return d1.CompareTo(d2) >= 0;
        }

        public static bool operator <=(Date d1, Date d2)
        {
            return d1.CompareTo(d2) <= 0;
        }

        public static bool operator ==(Date d1, Date d2)
        {
            return d1.CompareTo(d2) == 0;
        }

        public static bool operator !=(Date d1, Date d2)
        {
            return d1.CompareTo(d2) != 0;
        }

        public static TimeSpan operator -(Date d1, Date d2)
        {
            return d1.DateTime - d2.DateTime;
        }

        public static DateTime operator +(Date date, Time time)
        {
            return date.DateTime.Add(time.TimeSpan);
        }

        public static explicit operator DateTime(Date date)
        {
            return date.DateTime;
        }

        #endregion

        #region common

        private readonly DateTime _dt;

        public Date(int year, int month, int day)
        {
            _dt = new DateTime(year, month, day).Date;
        }

        public Date(DateTime dt)
        {
            _dt = dt.Date;
        }

        public int Year
        {
            get { return DateTime.Year; }
        }

        public int Month
        {
            get { return DateTime.Month; }
        }

        public int Day
        {
            get { return DateTime.Day; }
        }

        public DateTime DateTime
        {
            get { return _dt; }
        }

        public DayOfWeek DayOfWeek
        {
            get { return DateTime.DayOfWeek; }
        }

        public static Date Today
        {
            get { return new Date(DateTime.Today); }
        }

        public static Date MinValue
        {
            get { return new Date(DateTime.MinValue); }
        }

        public static Date MaxValue
        {
            get { return new Date(DateTime.MaxValue); }
        }

        public int DayOfYear
        {
            get { return DateTime.DayOfYear; }
        }

        public Date AddYears(int value)
        {
            return new Date(DateTime.AddYears(value));
        }

        public Date AddMonths(int value)
        {
            return new Date(DateTime.AddMonths(value));
        }

        public Date AddDays(double value)
        {
            return new Date(DateTime.AddDays(value));
        }

        public static bool TryParse(string value, out Date result)
        {
            DateTime dt;
            bool could = DateTime.TryParse(value, out dt);
            result = could ? new Date(dt) : MinValue;
            return could;
        }

        public static Date Parse(string value)
        {
            Date date;
            if (TryParse(value, out date))
            {
                return date;
            }
            throw new FormatException("Only date fields are supported.");
        }

        public string ToString(string format)
        {
            return DateTime.ToString(format);
        }

        public override bool Equals(object obj)
        {
            return DateTime.Equals(((Date) obj).DateTime);
        }

        public override int GetHashCode()
        {
            return DateTime.GetHashCode();
        }

        public override string ToString()
        {
            return DateTime.ToString("yyyy-MM-dd");
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            DateTime tdt = ((Date) obj).DateTime;
            return DateTime.CompareTo(tdt.Date);
        }

        #endregion

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return DateTime.ToString(format, formatProvider);
        }

        #endregion

        #region IComparable<Date> Members

        public int CompareTo(Date other)
        {
            return DateTime.CompareTo(other.DateTime);
        }

        #endregion

        #region IEquatable<Date> Members

        public bool Equals(Date other)
        {
            return DateTime.Equals(other.DateTime);
        }

        #endregion
    }
}