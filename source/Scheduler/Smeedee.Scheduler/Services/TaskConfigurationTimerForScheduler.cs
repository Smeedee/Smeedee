namespace Smeedee.Scheduler.Services
{
    public class TaskConfigurationTimerForScheduler : TimerWithTimestamp
    {
        public TaskConfigurationTimerForScheduler() : base(0, 2000)
        {
        }
    }
}
