using System;

namespace Smeedee.Client.Framework.Services
{
    public interface IEventAggregator
    {
        void Subscribe<TMessage>(object subscriber, Action<TMessage> eventHandler)
            where TMessage: MessageBase;

        void Unsubscribe<TMessage>(object subscriberToUnsusbscribe)
            where TMessage: MessageBase;

        void PublishMessage<TMessage>(TMessage message)
            where TMessage: MessageBase;

        void PublishMessageAsync<TMessage>(TMessage message)
            where TMessage : MessageBase;

    }

    public class MessageBase
    {
        public object Sender { get; protected set; }

        public MessageBase(object sender)
        {
            Sender = sender;
        }
    }
}