using System;

namespace Smeedee.Client.Framework.Services
{
    public interface ITimer
    {
        event EventHandler Elapsed;
        void Start(int interval);
        void Stop();
    }
}
