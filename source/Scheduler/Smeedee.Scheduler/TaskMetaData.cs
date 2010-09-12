using System;
using Smeedee.Tasks.Framework;

namespace Smeedee.Scheduler
{
    public class TaskMetaData
    {
        public string InstanceName { get; set; }
        public TaskBase Task { get; set; }
        public DateTime LastDispatch { get; set; }
        public bool IsRunning { get; set; }
        public int FailureCounter { get; set; }
        public DateTime CooldownPoint { get; set; }
    }
}