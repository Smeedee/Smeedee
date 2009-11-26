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

using Microsoft.Practices.Composite.Events;
using Moq;
using NUnit.Framework;


namespace APD.Client.FrameworkTests.Commands.NoPayloadCommandPublisherSpecs
{
    public class Shared
    {
        protected Mock<IEventAggregator> eventAggregatorMock;
        protected NoPayloadCommandPublisher noPayloadCommandPublisher;
    }

    [TestFixture]
    public class when_notified : Shared
    {
        protected Mock<NoPayloadCommand> command;

        [SetUp]
        public void Setup()
        {
            command = new Mock<NoPayloadCommand>();
            eventAggregatorMock = new Mock<IEventAggregator>();

            eventAggregatorMock.Setup(e => e.GetEvent<NoPayloadCommand>()).Returns(command.Object);

            noPayloadCommandPublisher = new NoPayloadCommandPublisher(eventAggregatorMock.Object);
        }

        [Test]
        public void should_retrive_command_from_eventAggregator()
        {
            noPayloadCommandPublisher.Notify();
            eventAggregatorMock.Verify(e => e.GetEvent<NoPayloadCommand>(), Times.Once());
        }

        [Test]
        public void should_publish_the_command_from_the_eventAggregator()
        {
            noPayloadCommandPublisher.Notify();
            command.Verify(c => c.Publish(It.IsAny<object>()), Times.Once());
        }
    }
}
