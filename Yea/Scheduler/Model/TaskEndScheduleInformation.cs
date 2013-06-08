#region Usings

using System;

#endregion

namespace Yea.Scheduler.Model
{
    public class TaskEndScheduleInformation : TaskStartScheduleInformation
    {
        public TimeSpan Duration { get; set; }
        public DateTime? NextRunTime { get; set; }
    }
}