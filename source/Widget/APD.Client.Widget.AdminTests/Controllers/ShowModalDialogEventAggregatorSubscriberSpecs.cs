using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Framework.Commands;

using Microsoft.Practices.Composite.Events;

using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using APD.Client.Widget.Admin.Controllers;
using Moq;

using TinyBDD.Specification.NUnit;
using Microsoft.Practices.Composite.Presentation.Events;


namespace APD.Client.Widget.AdminTests.Controllers.ShowModalDialogEventNotifierSpecs
{
    public class Shared 
    {
        protected static IEventAggregator eventAggregator;
        protected static ShowModalDialogEventNotifier subscriber;
        protected static bool notification_triggered;

        protected Context EventAggregator_is_created = () =>
        {
            eventAggregator = new EventAggregator();
        };

        protected Context Subscriber_is_created = () =>
        {
            notification_triggered = false;
            subscriber = new ShowModalDialogEventNotifier(eventAggregator);
            subscriber.NewNotification += new EventHandler<ShowModalDialogEventArgs>(subscriber_NewNotification);
        };

        static void subscriber_NewNotification(object sender, ShowModalDialogEventArgs e)
        {
            notification_triggered = true;
        }

        protected When new_ShowModalDialog_event = () =>
        {
            eventAggregator.GetEvent<CompositePresentationEvent<ShowModalDialogEventArgs>>().
                Publish(new ShowModalDialogEventArgs());
        };
    }

    [TestFixture]
    public class When_new_ShowModalDialog_event : Shared
    {
        [Test]
        public void Assure_subscribers_are_notified()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(EventAggregator_is_created).
                    And(Subscriber_is_created);

                scenario.When(new_ShowModalDialog_event);
                scenario.Then("assure subscribers are notified", () =>
                    notification_triggered.ShouldBeTrue());
            });   
        }
    }
}
