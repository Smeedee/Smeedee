using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Framework.Tests.Services.Impl
{
    public class EventAggregatorSpecs
    {
        [TestFixture]
        public class shared
        {
            protected EventAggregator ea;
            protected Guid originalGuid;
            protected TestMessage testMessage;

            [SetUp]
            public void Setup()
            {
                ea = new EventAggregator(new NoBackgroundWorkerInvocation());
                testMessage = new TestMessage(this);
                originalGuid = new Guid();
            }
            
        }

        [TestFixture]
        public class when_publishing_messages : shared
        {
            [Test]
            public void Assure_subscribed_event_handlers_are_called()
            {
                ea.Subscribe<TestMessage>(this, msg => originalGuid = msg.Id);
                ea.PublishMessage(testMessage);
                originalGuid.ShouldBe(testMessage.Id);
            }

            [Test]
            public void Assure_unsubscribed_handlers_does_not_get_called()
            {
                Guid guidSetInEventhandler = originalGuid;
                ea.Subscribe<TestMessage>(this, msg => guidSetInEventhandler = msg.Id);
                ea.Unsubscribe<TestMessage>(this);
                ea.PublishMessage(testMessage);
                
                guidSetInEventhandler.ShouldBe(originalGuid);
            }

            [Test]
            public void Assure_message_prosessing_is_done_on_background_thread()
            {
                Mock<IInvokeBackgroundWorker> backgroundWorkerMock = new Mock<IInvokeBackgroundWorker>();
                bool mockWasUsed = false;
                backgroundWorkerMock.Setup(m => m.RunAsyncVoid(It.IsAny<Action>())).Callback(() => mockWasUsed = true);
                
                ea = new EventAggregator(backgroundWorkerMock.Object);

                ea.Subscribe<TestMessage>(this, msg => {} );
                ea.PublishMessage(testMessage);

                mockWasUsed.ShouldBeTrue();
            }
        }

        [TestFixture]
        public class when_subscriber_has_been_disposed : shared
        {
            [Test]
            public void Assure_subscription_does_not_keep_it_alive()
            {
                var subscribingObject = new SubscribingObject();
                WeakReference weakReference = new WeakReference(subscribingObject);
                ea.Subscribe<TestMessage>(this, subscribingObject.ProcessMessage);
                subscribingObject = null;
                GC.Collect();

                bool hasBeenDisposed = !weakReference.IsAlive;
                hasBeenDisposed.ShouldBeTrue();
            }

            [Test]
            public void Assure_subscriber_is_ignored_on_consequent_publishings()
            {
                var subscribingObject = new SubscribingObject();
                ea.Subscribe<TestMessage>(this, subscribingObject.ProcessMessage);
                subscribingObject = null;
                GC.Collect();

                ea.PublishMessage(testMessage);
            }
        }

        private class SubscribingObject
        { public void ProcessMessage(TestMessage message) {}}
    }

    public class TestMessage : MessageBase
    {
        public Guid Id { get; private set; }

        public TestMessage(object sender) : base(sender)
        {
            Id = Guid.NewGuid();
        }
    }
}
