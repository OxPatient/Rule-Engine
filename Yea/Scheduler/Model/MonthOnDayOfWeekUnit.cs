#region Usings

using System;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.Scheduler.Model
{
    public class MonthOnDayOfWeekUnit
    {
        public MonthOnDayOfWeekUnit(Schedule schedule, int duration, Week week, DayOfWeek dayOfWeek)
        {
            Schedule = schedule;
            Duration = duration;
            Week = week;
            DayOfWeek = dayOfWeek;
            if (Week == Week.Last)
                Schedule.CalculateNextRun = x =>
                    {
                        var nextRun = x.Date.FirstDayOfMonth().LastWeekdayOfMonth(DayOfWeek);
                        return (x > nextRun) ? x.Date.FirstDayOfMonth().AddMonths(Duration).LastWeekdayOfMonth(DayOfWeek) : nextRun;
                    };
            else
                Schedule.CalculateNextRun = x =>
                    {
                        var nextRun = x.Date.FirstDayOfMonth().AddWeeks((int)Week).NextDayOfWeek(DayOfWeek);
                        return (x > nextRun)
                                   ? x.Date.FirstDayOfMonth().AddMonths(Duration).AddWeeks((int)Week).NextDayOfWeek(DayOfWeek)
                                   : nextRun;
                    };
        }

        internal Schedule Schedule { get; private set; }
        internal int Duration { get; private set; }
        internal Week Week { get; private set; }
        internal DayOfWeek DayOfWeek { get; private set; }

        /// <summary>
        ///     Schedules the specified task to run at the hour and minute specified.  If the hour and minute have passed, the task will execute the next scheduled month.
        /// </summary>
        /// <param name="hours">0-23: Represents the hour of the day</param>
        /// <param name="minutes">0-59: Represents the minute of the day</param>
        /// <returns></returns>
        public void At(int hours, int minutes)
        {
            if (Week == Week.Last)
                Schedule.CalculateNextRun = x =>
                    {
                        var nextRun = x.Date.FirstDayOfMonth().LastWeekdayOfMonth(DayOfWeek).AddHours(hours).AddMinutes(minutes);
                        return (x > nextRun)
                                   ? x.Date.FirstDayOfMonth()
                                      .AddMonths(Duration)
                                      .LastWeekdayOfMonth(DayOfWeek)
                                      .AddHours(hours)
                                      .AddMinutes(minutes)
                                   : nextRun;
                    };
            else
                Schedule.CalculateNextRun = x =>
                    {
                        var nextRun =
                            x.Date.FirstDayOfMonth().AddWeeks((int)Week).NextDayOfWeek(DayOfWeek).AddHours(hours).AddMinutes(minutes);
                        return (x > nextRun)
                                   ? x.Date.FirstDayOfMonth()
                                      .AddMonths(Duration)
                                      .AddWeeks((int)Week)
                                      .NextDayOfWeek(DayOfWeek)
                                      .AddHours(hours)
                                      .AddMinutes(minutes)
                                   : nextRun;
                    };
        }
    }
}