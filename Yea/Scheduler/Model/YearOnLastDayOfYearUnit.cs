#region Usings

using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.Scheduler.Model
{
    public class YearOnLastDayOfYearUnit
    {
        public YearOnLastDayOfYearUnit(Schedule schedule, int duration)
        {
            Schedule = schedule;
            Duration = duration;
            Schedule.CalculateNextRun = x =>
                {
                    var nextRun = x.Date.FirstDayOfYear().AddMonths(11).LastDayOfMonth();
                    return (x > nextRun) ? x.Date.FirstDayOfYear().AddYears(Duration).AddMonths(11).LastDayOfMonth() : nextRun;
                };
        }

        internal Schedule Schedule { get; private set; }
        internal int Duration { get; private set; }

        /// <summary>
        ///     Schedules the specified task to run at the hour and minute specified on the last day of year.  If the hour and minute have passed, the task will execute the next scheduled year.
        /// </summary>
        /// <param name="hours">0-23: Represents the hour of the day</param>
        /// <param name="minutes">0-59: Represents the minute of the day</param>
        /// <returns></returns>
        public void At(int hours, int minutes)
        {
            Schedule.CalculateNextRun = x =>
                {
                    var nextRun = x.Date.FirstDayOfYear().AddMonths(11).LastDayOfMonth().AddHours(hours).AddMinutes(minutes);
                    return (x > nextRun)
                               ? x.Date.FirstDayOfYear()
                                  .AddYears(Duration)
                                  .AddMonths(11)
                                  .LastDayOfMonth()
                                  .AddHours(hours)
                                  .AddMinutes(minutes)
                               : nextRun;
                };
        }
    }
}