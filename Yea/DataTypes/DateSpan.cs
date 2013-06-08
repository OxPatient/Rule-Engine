#region Usings

using System;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.DataTypes
{
    /// <summary>
    ///     Represents a date span
    /// </summary>
    public class DateSpan
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="Start">Start of the date span</param>
        /// <param name="End">End of the date span</param>
        public DateSpan(DateTime Start, DateTime End)
        {
            if (Start > End)
                throw new ArgumentException(Start.ToString() + " is after " + End.ToString());
            this.Start = Start;
            this.End = End;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Start date
        /// </summary>
        public virtual DateTime Start { get; protected set; }

        /// <summary>
        ///     End date
        /// </summary>
        public virtual DateTime End { get; protected set; }

        /// <summary>
        ///     Years between the two dates
        /// </summary>
        public virtual int Years
        {
            get { return (End - Start).Years(); }
        }

        /// <summary>
        ///     Months between the two dates
        /// </summary>
        public virtual int Months
        {
            get { return (End - Start).Months(); }
        }

        /// <summary>
        ///     Days between the two dates
        /// </summary>
        public virtual int Days
        {
            get { return (End - Start).DaysRemainder(); }
        }

        /// <summary>
        ///     Hours between the two dates
        /// </summary>
        public virtual int Hours
        {
            get { return (End - Start).Hours; }
        }

        /// <summary>
        ///     Minutes between the two dates
        /// </summary>
        public virtual int Minutes
        {
            get { return (End - Start).Minutes; }
        }

        /// <summary>
        ///     Seconds between the two dates
        /// </summary>
        public virtual int Seconds
        {
            get { return (End - Start).Seconds; }
        }

        /// <summary>
        ///     Milliseconds between the two dates
        /// </summary>
        public virtual int MilliSeconds
        {
            get { return (End - Start).Milliseconds; }
        }

        #endregion

        #region Functions

        /// <summary>
        ///     Returns the intersecting time span between the two values
        /// </summary>
        /// <param name="Span">Span to use</param>
        /// <returns>The intersection of the two time spans</returns>
        public DateSpan Intersection(DateSpan Span)
        {
            if (Span.IsNull())
                return null;
            if (!Overlap(Span))
                return null;
            DateTime Start = Span.Start > this.Start ? Span.Start : this.Start;
            DateTime End = Span.End < this.End ? Span.End : this.End;
            return new DateSpan(Start, End);
        }

        /// <summary>
        ///     Determines if two DateSpans overlap
        /// </summary>
        /// <param name="Span">The span to compare to</param>
        /// <returns>True if they overlap, false otherwise</returns>
        public bool Overlap(DateSpan Span)
        {
            return ((Start >= Span.Start && Start < Span.End) || (End <= Span.End && End > Span.Start) ||
                    (Start <= Span.Start && End >= Span.End));
        }

        #endregion

        #region Operators

        /// <summary>
        ///     Addition operator
        /// </summary>
        /// <param name="Span1">Span 1</param>
        /// <param name="Span2">Span 2</param>
        /// <returns>The combined date span</returns>
        public static DateSpan operator +(DateSpan Span1, DateSpan Span2)
        {
            if (Span1.IsNull() && Span2.IsNull())
                return null;
            if (Span1.IsNull())
                return new DateSpan(Span2.Start, Span2.End);
            if (Span2.IsNull())
                return new DateSpan(Span1.Start, Span1.End);
            DateTime Start = Span1.Start < Span2.Start ? Span1.Start : Span2.Start;
            DateTime End = Span1.End > Span2.End ? Span1.End : Span2.End;
            return new DateSpan(Start, End);
        }

        /// <summary>
        ///     Determines if two DateSpans are equal
        /// </summary>
        /// <param name="Span1">Span 1</param>
        /// <param name="Span2">Span 2</param>
        /// <returns>True if they are, false otherwise</returns>
        public static bool operator ==(DateSpan Span1, DateSpan Span2)
        {
            if ((object) Span1 == null && (object) Span2 == null)
                return true;
            if ((object) Span1 == null || (object) Span2 == null)
                return false;
            return Span1.Start == Span2.Start && Span1.End == Span2.End;
        }

        /// <summary>
        ///     Determines if two DateSpans are not equal
        /// </summary>
        /// <param name="Span1">Span 1</param>
        /// <param name="Span2">Span 2</param>
        /// <returns>True if they are not equal, false otherwise</returns>
        public static bool operator !=(DateSpan Span1, DateSpan Span2)
        {
            return !(Span1 == Span2);
        }

        #endregion

        #region Overridden Functions

        /// <summary>
        ///     Converts the DateSpan to a string
        /// </summary>
        /// <returns>The DateSpan as a string</returns>
        public override string ToString()
        {
            return "Start: " + Start.ToString() + " End: " + End.ToString();
        }

        /// <summary>
        ///     Determines if two objects are equal
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if they are, false otherwise</returns>
        public override bool Equals(object obj)
        {
            var Tempobj = obj as DateSpan;
            return Tempobj != null && Tempobj == this;
        }

        /// <summary>
        ///     Gets the hash code for the date span
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode()
        {
            return End.GetHashCode() & Start.GetHashCode();
        }

        #endregion
    }
}