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


namespace APD.Client.FrameworkTests.Commands.PayloadCommandNotifierSpecs
{
    public class Shared
    {
        protected Mock<StringPayloadCommand> commandMock;
        protected Mock<IEventAggregator> eventAggregator;
        protected StringPayloadCommandNotifier notifier;
    }

    [TestFixture]
    public class when_spawned : Shared
    {
        [SetUp]
        public void Setup()
        {
            commandMock = new Mock<StringPayloadCommand>();

            eventAggregator = new Mock<IEventAggregator>(); 
            eventAggregator.Setup(e => e.GetEvent<StringPayloadCommand>()).Returns(commandMock.Object);

            notifier = new StringPayloadCommandNotifier(eventAggregator.Object);
        }

        [Test]
        public void should_have_commandPublished_event()
        {
            notifier.CommandPublished += null;
        }

        [Test]
        public void should_get_command_from_eventAggregator()
        {
            eventAggregator.Verify(e=>e.GetEvent<StringPayloadCommand>(),Times.Once());
        }

        [Test]
        [Ignore("The mock does not verify this call correct.")]
        public void should_subscribe_to_the_command_from_the_eventAggregator()
        {
            
            commandMock.Verify(c => c.Subscribe(It.IsAny<Action<string>>()));
        }
    }
}
