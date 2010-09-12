using System;
using System.Threading;

namespace Smeedee.Scheduler.Services
{
    public class TimerWithTimestamp : ITimerWithTimestamp
    {
        public event EventHandler<TimerElapsedEventArgs> Elapsed;
        
        private Timer timer;
        private long dueTimeInMilliseconds;
        private long intervallInMilliseconds;

        public TimerWithTimestamp(int dueTimeInMilliseconds, int intervallInMilliseconds)
        {
            this.dueTimeInMilliseconds = dueTimeInMilliseconds;
            this.intervallInMilliseconds = intervallInMilliseconds;
        }

        private void TimePeriodElapsed(object state)
        {
            if (Elapsed != null)
            {
                Elapsed(this, new TimerElapsedEventArgs(DateTime.Now));
            }
        }

        public void Start()
        {
            timer = new Timer(TimePeriodElapsed, null, dueTimeInMilliseconds, intervallInMilliseconds);
        }
    }
}