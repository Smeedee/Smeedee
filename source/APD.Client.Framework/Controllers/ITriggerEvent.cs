using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APD.Client.Framework.Controllers
{
    /// <summary>
    /// Used as an abstraction for triggering an event. An implementation of this can:
    /// - Trigger an event on an EventAggregator (mediator/pub-sub pattern)
    /// - Trigger an language feature Event
    ///
    /// </summary>
    /// <typeparam name="TTriggerArgs"></typeparam>
    public interface ITriggerEvent<TTriggerArgs> where TTriggerArgs : EventArgs
    {
        void NewEvent(TTriggerArgs args);
    }
}
