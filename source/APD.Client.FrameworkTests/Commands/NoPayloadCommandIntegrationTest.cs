#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using Microsoft.Practices.Composite.Events;
using Moq;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;


namespace APD.Client.FrameworkTests.Commands.NoPayloadCommandIntegrationTest
{

    public class Shared
    {
        protected Mock<NoPayloadCommand> commandMock;
        protected NoPayloadCommandPublisher publisher;
        protected NoPayloadCommandNotifier notifier;
        protected Mock<IEventAggregator> eventAggregatorMock;
    }

    [TestFixture]
    public class when_publisher_notifies : Shared
    {
        [SetUp]
        public void Setup()
        {
            commandMock = new Mock<NoPayloadCommand>();
            commandMock.Setup(c => c.Publish(It.IsAny<object>())).Callback(new Action(commandPublishCalled));

            eventAggregatorMock = new Mock<IEventAggregator>();
            eventAggregatorMock.Setup(e => e.GetEvent<NoPayloadCommand>()).Returns(commandMock.Object);

            publisher = new NoPayloadCommandPublisher(eventAggregatorMock.Object);
            notifier = new NoPayloadCommandNotifier(eventAggregatorMock.Object);
        }


        [Test]
        public void CommandPublished_event_should_be_fired_on_the_notifier()
        {
            bool wasCalled = false;
            notifier.CommandPublished += (o, e) => wasCalled = true;
            
            publisher.Notify();

            wasCalled.ShouldBeTrue();
        }

        public void commandPublishCalled()
        {
            notifier.TriggerCommandPublished();
        }
    }
}
