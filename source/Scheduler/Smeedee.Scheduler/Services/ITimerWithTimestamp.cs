using System;

namespace Smeedee.Scheduler.Services
{
    public interface ITimerWithTimestamp
    {
        event EventHandler<TimerElapsedEventArgs> Elapsed;

        void Start();
    }
}