using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.Client.Framework.Services
{
    public interface ITimer
    {
        event EventHandler Elapsed;
        void Start(int interval);
        void Stop();
    }
}
