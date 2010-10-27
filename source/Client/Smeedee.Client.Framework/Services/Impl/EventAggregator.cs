using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using TinyMVVM.Framework.Services.Impl;

namespace Smeedee.Client.Framework.Services.Impl
{
    public class EventAggregator : IEventAggregator
    {
        private IInvokeBackgroundWorker backgroundworker;

        private Dictionary<Type, List<IInvokeEventHandler>> activeSubscriptions = new Dictionary<Type, List<IInvokeEventHandler>>(10);

    	public static IEventAggregator Instance = new EventAggregator(new AsyncVoidClient());

        public EventAggregator(IInvokeBackgroundWorker backgroundworker)
        {
            this.backgroundworker = backgroundworker;
        }

        public void Subscribe<TMessage>(object subscriber, Action<TMessage> eventHandler)
            where TMessage: MessageBase
        {
            var subscription = new Subscription<TMessage>(subscriber, eventHandler);
            var t = typeof(TMessage);

            List<IInvokeEventHandler> subscriptions;
            activeSubscriptions.TryGetValue(t, out subscriptions);
            if (subscriptions == null)
            {
                subscriptions = new List<IInvokeEventHandler>();
                activeSubscriptions.Add(t, subscriptions);
            }

            subscriptions.Add(subscription);
        }


        public void Unsubscribe<TMessage>(object subscriberToUnsusbscribe)
            where TMessage: MessageBase
        {
            var messageType = typeof (TMessage);
            List<IInvokeEventHandler> subscribers;
            activeSubscriptions.TryGetValue(messageType, out subscribers);

        	var subscribersToRemove = new List<IInvokeEventHandler>();
        	foreach (var s in subscribers)
        	{
				if (ReferenceEquals(s.Subscriber.Target, subscriberToUnsusbscribe))
					subscribersToRemove.Add(s);
        	}
			subscribersToRemove.ForEach(s => subscribers.Remove(s));

			//Had to remove this code because it wasn't silverlight complient
            //subscribers.RemoveAll(s => ReferenceEquals(s.Subscriber.Target, subscriberToUnsusbscribe));

        }

        public void PublishMessage<TMessage>(TMessage message)
            where TMessage: MessageBase
        {
            var messageType = typeof (TMessage);
            List<IInvokeEventHandler> subscriptions;
            activeSubscriptions.TryGetValue(messageType, out subscriptions);
            if(subscriptions != null)
            {
                foreach (var subscription in subscriptions)
                {
                    bool targetStillAlive = subscription.Invoke(message, backgroundworker);
                }
            }
        }

        private class Subscription<TMessage> : IInvokeEventHandler
            where TMessage : MessageBase
        {
            public WeakReference Subscriber { get; set; }
            private WeakReference  eventHandlerReference;

            public Subscription(object subscriber, Action<TMessage> eventHandler)
            {
                Subscriber = new WeakReference(subscriber);
                this.eventHandlerReference = new WeakReference(eventHandler);
            }

            public bool Invoke(MessageBase message, IInvokeBackgroundWorker workerToInvokeOn)
            {
                if( eventHandlerReference.IsAlive)
                {
                    var action = eventHandlerReference.Target as Action<TMessage>;
                    workerToInvokeOn.RunAsyncVoid(() => action(message as TMessage));
                }

                return eventHandlerReference.IsAlive;
            }
        }

        private interface IInvokeEventHandler
        {
            bool Invoke(MessageBase message, IInvokeBackgroundWorker workerToInvokeOn);
            WeakReference Subscriber { get; set; }
        }
    }
}
