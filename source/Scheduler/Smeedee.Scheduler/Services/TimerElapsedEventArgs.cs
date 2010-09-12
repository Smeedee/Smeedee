using System;

namespace Smeedee.Scheduler.Services
{
    public class TimerElapsedEventArgs : EventArgs
    {
        public DateTime Time { get; protected set; }

        public TimerElapsedEventArgs(DateTime time)
        {
            Time = time;
        }
    }
}