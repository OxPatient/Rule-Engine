﻿#region Usings

using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.Scheduler.Model
{
    public class YearOnDayOfYearUnit
    {
        public YearOnDayOfYearUnit(Schedule schedule, int duration, int dayOfYear)
        {
            Schedule = schedule;
            Duration = duration;
            DayOfYear = dayOfYear;
            Schedule.CalculateNextRun = x =>
                {
                    var nextRun = x.Date.FirstDayOfYear().AddDays(DayOfYear - 1);
                    return (x > nextRun) ? x.Date.FirstDayOfYear().AddYears(Duration).AddDays(DayOfYear - 1) : nextRun;
                };
        }

        internal Schedule Schedule { get; private set; }
        internal int Duration { get; private set; }
        internal int DayOfYear { get; private set; }

        /// <summary>
        ///     Schedules the specified task to run at the hour and minute specified.  If the hour and minute have passed, the task will execute the next scheduled year.
        /// </summary>
        /// <param name="hours">0-23: Represents the hour of the day</param>
        /// <param name="minutes">0-59: Represents the minute of the day</param>
        /// <returns></returns>
        public void At(int hours, int minutes)
        {
            Schedule.CalculateNextRun = x =>
                {
                    var nextRun = x.Date.FirstDayOfYear().AddDays(DayOfYear - 1).AddHours(hours).AddMinutes(minutes);
                    return (x > nextRun)
                               ? x.Date.FirstDayOfYear()
                                  .AddYears(Duration)
                                  .AddDays(DayOfYear - 1)
                                  .AddHours(hours)
                                  .AddMinutes(minutes)
                               : nextRun;
                };
        }
    }
}