using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            public void Assure_all_subscribed_event_handlers_are_called()
            {
                int callCount = 0;
                int numberOfSubscribersToTestWith = 10;
                for (int i = 0; i < numberOfSubscribersToTestWith; i++)
                {
                    ea.Subscribe<TestMessage>(this, m => { callCount++; });
                }
                
                ea.PublishMessage(testMessage);
                callCount.ShouldBe(numberOfSubscribersToTestWith);
            }

            [Test]
            public void Assure_only_correct_subscriptions_are_notified()
            {
                bool firstWasCalled = false, secondWasCalled = false;
                ea.Subscribe<TestMessage>(this, m => { firstWasCalled = true; });
                ea.Subscribe<AnotherTestMessage>(this, m => { secondWasCalled = true; });

                ea.PublishMessage(testMessage);
                firstWasCalled.ShouldBeTrue();
                secondWasCalled.ShouldBeFalse();
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
            public void Assure_dead_subscribers_are_removed()
            {
                DateTime firstStartTime = DateTime.Now;
                DateTime firstRunCompleted;
                DateTime secondStartTime;
                DateTime secondRunCompleted;

                ea.Subscribe<TestMessage>(this, msg => {});
                ea.PublishMessage<TestMessage>(new TestMessage(this));

                firstRunCompleted = DateTime.Now;

                for (int i = 0; i < 100000; i++)
                {
                    SubscribingObject so = new SubscribingObject();
                    ea.Subscribe<TestMessage>(so, so.ProcessMessage);
                    so = null;
                }
                GC.Collect();
                GC.WaitForFullGCComplete();
                // This call is expected to remove dead subscribers
                ea.PublishMessage(testMessage);

                secondStartTime = DateTime.Now;
                ea.PublishMessage(testMessage);
                secondRunCompleted = DateTime.Now;

                var timeDifferenceBetweenFirstAndSecondRun = Math.Abs(
                    ((firstRunCompleted - firstStartTime) - (secondRunCompleted - secondStartTime)).TotalMilliseconds);
                Debug.WriteLine(timeDifferenceBetweenFirstAndSecondRun);
                timeDifferenceBetweenFirstAndSecondRun.ShouldBeLessThan(50);
            }

        }

        [TestFixture]
        public class when_publishing_messages_async : shared
        {
            [Test]
            public void Assure_message_prosessing_is_done_on_background_thread()
            {
                Mock<IInvokeBackgroundWorker> backgroundWorkerMock = new Mock<IInvokeBackgroundWorker>();
                bool mockWasUsed = false;
                backgroundWorkerMock.Setup(m => m.RunAsyncVoid(It.IsAny<Action>())).Callback(() => mockWasUsed = true);

                ea = new EventAggregator(backgroundWorkerMock.Object);

                ea.Subscribe<TestMessage>(this, msg => { });
                ea.PublishMessageAsync(testMessage);

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

    public class AnotherTestMessage : MessageBase
    {
        public Guid Id { get; private set; }

        public AnotherTestMessage(object sender)
            : base(sender)
        {
            Id = Guid.NewGuid();
        }
    }
}
