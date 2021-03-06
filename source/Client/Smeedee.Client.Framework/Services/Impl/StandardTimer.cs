﻿using System;
using System.Windows.Threading;

namespace Smeedee.Client.Framework.Services.Impl
{
    public class StandardTimer : ITimer
    {
        private DispatcherTimer timer;

        public StandardTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (Elapsed != null) Elapsed(this, EventArgs.Empty);
        }

        public event EventHandler Elapsed;
        public void Start(int interval)
        {
			timer.Interval = new TimeSpan(0, 0, 0, 0, interval);
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }
}
