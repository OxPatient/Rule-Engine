#region Usings

using System;

#endregion

namespace Yea
{
    [Serializable]
    public struct Time : IComparable, IFormattable, IComparable<Time>, IEquatable<Time>
    {
        #region operators

        public static bool operator >(Time t1, Time t2)
        {
            return t1.CompareTo(t2) > 0;
        }

        public static bool operator <(Time t1, Time t2)
        {
            return t1.CompareTo(t2) < 0;
        }

        public static bool operator >=(Time t1, Time t2)
        {
            return t1.CompareTo(t2) >= 0;
        }

        public static bool operator <=(Time t1, Time t2)
        {
            return t1.CompareTo(t2) <= 0;
        }

        public static bool operator ==(Time t1, Time t2)
        {
            return t1.CompareTo(t2) == 0;
        }

        public static bool operator !=(Time t1, Time t2)
        {
            return t1.CompareTo(t2) != 0;
        }

        public static TimeSpan operator -(Time time1, Time time2)
        {
            return time1.TimeSpan.Subtract(time1.TimeSpan);
        }

        public static DateTime operator +(Time time, Date date)
        {
            return date.DateTime.Add(time.TimeSpan);
        }

        public static explicit operator TimeSpan(Time time)
        {
            return time.TimeSpan;
        }

        #endregion

        #region common

        private TimeSpan _ts;

        public Time(DateTime dt)
            : this(dt.TimeOfDay)
        {
        }

        public Time(TimeSpan span)
        {
            _ts = span;
        }

        public Time(int ticks)
        {
            _ts = TimeSpan.FromTicks(ticks);
        }

        public Time(int hours, int minutes, int seconds)
        {
            _ts = new TimeSpan(hours, minutes, seconds);
        }

        public Time(int hours, int minutes, int seconds, int milliseconds)
        {
            _ts = new TimeSpan(0, hours, minutes, seconds, milliseconds);
        }

        public TimeSpan TimeSpan
        {
            get
            {
                while (_ts.Days != 0)
                {
                    _ts = _ts.Subtract(TimeSpan.FromDays(_ts.Days));
                }
                return _ts;
            }
        }

        public int Hour
        {
            get { return TimeSpan.Hours; }
        }

        public int Minute
        {
            get { return TimeSpan.Minutes; }
        }

        public int Second
        {
            get { return TimeSpan.Seconds; }
        }

        public int Millisecond
        {
            get { return TimeSpan.Milliseconds; }
        }

        public static Time Now
        {
            get { return new Time(DateTime.Now.TimeOfDay); }
        }

        public static Time Zero
        {
            get { return new Time(TimeSpan.Zero); }
        }

        public Time AddHours(double value)
        {
            return new Time(TimeSpan.Add(TimeSpan.FromHours(value)));
        }

        public Time AddMinutes(double value)
        {
            return new Time(TimeSpan.Add(TimeSpan.FromMinutes(value)));
        }

        public Time AddSecond(double value)
        {
            return new Time(TimeSpan.Add(TimeSpan.FromSeconds(value)));
        }

        public Time AddMillisecond(double value)
        {
            return new Time(TimeSpan.Add(TimeSpan.FromMilliseconds(value)));
        }

        public Time Add(TimeSpan value)
        {
            return new Time(TimeSpan.Add(value));
        }

        public Time AddTicks(long value)
        {
            return new Time(TimeSpan.Add(TimeSpan.FromTicks(value)));
        }

        public static bool TryParse(string value, out Time result)
        {
            TimeSpan ts;
            bool rs = TimeSpan.TryParse(value, out ts);
            result = rs ? new Time(ts) : Zero;
            return rs;
        }

        public static Time Parse(string value)
        {
            Time time;
            if (TryParse(value, out time))
            {
                return time;
            }
            throw new FormatException("Only time fields are supported.");
        }

        public string ToString(string format)
        {
            return TimeSpan.ToString(format);
        }

        public override bool Equals(object obj)
        {
            return TimeSpan.Ticks.Equals(((Time) obj).TimeSpan.Ticks);
        }

        public override int GetHashCode()
        {
            return TimeSpan.GetHashCode();
        }

        public override string ToString()
        {
            return TimeSpan.ToString("hh:mm:ss");
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return TimeSpan.CompareTo(((Time) obj).TimeSpan);
        }

        #endregion

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return TimeSpan.ToString(format, formatProvider);
        }

        #endregion

        #region IComparable<Time> Members

        public int CompareTo(Time other)
        {
            return TimeSpan.CompareTo(other.TimeSpan);
        }

        #endregion

        #region IEquatable<Time> Members

        public bool Equals(Time other)
        {
            return TimeSpan.Equals(other.TimeSpan);
        }

        #endregion
    }
}