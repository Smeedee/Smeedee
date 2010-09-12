namespace Smeedee.Scheduler.Services
{
    public class DispatchTimerForScheduler : TimerWithTimestamp
    {
        public DispatchTimerForScheduler() : base(0, 1000)
        {
        }
    }
}
