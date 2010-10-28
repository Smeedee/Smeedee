using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Smeedee.Client.Framework.Services.Impl
{
    public class EventAggregator : IEventAggregator
    {
        private IInvokeBackgroundWorker backgroundworker;

        private static EventAggregator instance;
        public static EventAggregator Instance
        {
            get
            {
                if(instance == null)
                    instance = new EventAggregator(new AsyncVoidClient());

                return instance;
            }
        }


        private Dictionary<Type, List<IInvokeEventHandler>> activeSubscriptions = new Dictionary<Type, List<IInvokeEventHandler>>(10);

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

            var subscribersToRemove =
                subscribers.Where(s => ReferenceEquals(s.Subscriber.Target, subscriberToUnsusbscribe)).ToList();
			
			subscribersToRemove.ForEach(s => subscribers.Remove(s));
        }

        public void PublishMessageAsync<TMessage>(TMessage message) 
            where TMessage : MessageBase
        {
            PublishMessageInternal(message, true);
        }

        public void PublishMessage<TMessage>(TMessage message)
            where TMessage: MessageBase
        {
            PublishMessageInternal(message, false);
        }

        private void PublishMessageInternal<TMessage>(TMessage message, bool asyncInvokation)
            where TMessage: MessageBase
        {
            var messageType = typeof(TMessage);
            List<IInvokeEventHandler> subscriptions;
            List<IInvokeEventHandler> deadSubscribers = new List<IInvokeEventHandler>();
            activeSubscriptions.TryGetValue(messageType, out subscriptions);
            if (subscriptions != null)
            {
                foreach (var subscription in subscriptions)
                {
                    if (subscription.Subscriber.IsAlive)
                    {
                        if(asyncInvokation)
                        {
                            subscription.Invoke(message, backgroundworker);
                        }
                        else
                        {
                            subscription.Invoke(message);
                        }
                    }
                    else
                    {
                        deadSubscribers.Add(subscription);
                    }
                }

                deadSubscribers.ForEach(s => subscriptions.Remove(s));
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

            public void Invoke(MessageBase message, IInvokeBackgroundWorker workerToInvokeOn)
            {
                var action = eventHandlerReference.Target as Action<TMessage>;
                workerToInvokeOn.RunAsyncVoid(() => action(message as TMessage));
            }

            public void Invoke(MessageBase message)
            {
                var action = eventHandlerReference.Target as Action<TMessage>;
                if(action != null)
                    action(message as TMessage);
            }
        }

        private interface IInvokeEventHandler
        {
            void Invoke(MessageBase message);
            void Invoke(MessageBase message, IInvokeBackgroundWorker workerToInvokeOn);
            WeakReference Subscriber { get; set; }
        }



    }
}
