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
using NUnit.Framework;
using APD.Client.Widget.Admin.Controllers;
using TinyBDD.Specification.NUnit;
using APD.Client.Widget.Admin.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;


namespace APD.Client.Widget.AdminTests.Controllers.CommandNotifierWiringSpecs
{
    public class CommandTrigger : ITriggerCommand
    {
        #region ITriggerCommand Members

        public void Trigger()
        {
            
        }

        #endregion
    }
 

    public class Shared
    {
        protected static CommandNotifierWiring<EventArgs> CommandNotifierWiring;

        protected Context commandWiring_is_created = () =>
        {
            CommandNotifierWiring = new CommandNotifierWiring<EventArgs>();
        };
    }

    [TestFixture]
    public class When_creating : Shared
    {
    }

    [TestFixture]
    public class When_Command_is_triggered : Shared
    {
        [Test]
        public void Assure_NewNotification_is_triggered()
        {
            Scenario.StartNew(this, scenario =>
            {
                bool notificationTriggered = false;

                scenario.Given(commandWiring_is_created).
                    And("test has subscribed to NewNotification event", () =>
                        CommandNotifierWiring.NewNotification += (o, e) => { notificationTriggered = true; });

                scenario.When("Command is triggered", () =>
                    CommandNotifierWiring.Trigger());

                scenario.Then("assure NewNotification is triggered on Notifier", () =>
                    notificationTriggered.ShouldBeTrue());
            });
        }
    }
}
