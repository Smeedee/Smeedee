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
using APD.Tests.SpecialMocks;
using NUnit.Framework;
using APD.Client.Widget.General.ViewModels;
using Moq;
using APD.Client.Widget.General.Controllers;
using APD.Client.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.Client.Widget.GeneralTests.Controllers.LoadingAnimationControllerSpecs
{
    public class Shared
    {
        protected static Mock<LoadingAnimationViewModel> viewModelMock;
        protected static IEventAggregatorDependantMock<DisplayLoadingAnimationNotifier> showAnimationNotifierMock;
        protected static IEventAggregatorDependantMock<HideLoadingAnimationNotifier> hideAnimationNotifierMock;
        protected static LoadingAnimationController controller;

        protected Context controller_is_created = () => 
        {
            viewModelMock = new Mock<LoadingAnimationViewModel>(new NoUIInvocation());
            showAnimationNotifierMock = new IEventAggregatorDependantMock<DisplayLoadingAnimationNotifier>();
            hideAnimationNotifierMock = new IEventAggregatorDependantMock<HideLoadingAnimationNotifier>();
            controller = new LoadingAnimationController(viewModelMock.Object, showAnimationNotifierMock.Object, hideAnimationNotifierMock.Object);
        };
    }

    [TestFixture]
    public class When_Controller_is_created : Shared
    {
        [SetUp]
        public void Setup()
        {
            viewModelMock = new Mock<LoadingAnimationViewModel>(new NoUIInvocation());
            showAnimationNotifierMock = new IEventAggregatorDependantMock<DisplayLoadingAnimationNotifier>();
            hideAnimationNotifierMock = new IEventAggregatorDependantMock<HideLoadingAnimationNotifier>();
        }

        [Test]
        public void Assure_ArgumentException_is_thrown_if_NullObject_is_passed_as_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("NullObject is passed as ViewModel argument");

                scenario.When("the controller is created");

                scenario.Then("assure ArgumentException is thrown", () =>
                    this.ShouldThrowException<ArgumentException>(() =>
                        new LoadingAnimationController(null, showAnimationNotifierMock.Object, hideAnimationNotifierMock.Object), exception =>
                            exception.Message.ShouldBe("Value can't be null\r\nParameter name: viewModel")));
            });
        }

        [Test]
        public void Assure_ArgumentException_is_thrown_if_NullObject_is_passed_as_showAnimationNotifier()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("NullObject is passed as show Command Notifier");

                scenario.When("the controller is created");

                scenario.Then("assure ArgumentException is thrown", () =>
                    this.ShouldThrowException<ArgumentException>(() =>
                        new LoadingAnimationController(viewModelMock.Object, null, hideAnimationNotifierMock.Object), exception =>
                            exception.Message.StartsWith("Value can't be null").ShouldBeTrue()));
            });
               
        }
    

        [Test]
        public void Assure_ArgumentException_is_thrown_if_NullObject_is_passed_as_hideAnimationNotifier()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("NullObject is passed as hide Command Notifier");

                scenario.When("the controller is created");

                scenario.Then("assure ArgumentException is thrown", () =>
                    this.ShouldThrowException<ArgumentException>(() =>
                        new LoadingAnimationController(viewModelMock.Object, showAnimationNotifierMock.Object, null), exception =>
                            exception.Message.StartsWith("Value can't be null").ShouldBeTrue()));
            });
               
        }
    }

    [TestFixture]
    public class When_Controller_is_spawned : Shared
    {
        [Test]
        public void Assure_it_has_a_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(controller_is_created);

                scenario.When("Controller is spawned");

                scenario.Then("assure it has a ViewModel", () =>
                    (controller.ViewModel is LoadingAnimationViewModel).ShouldBeTrue());
            });
               
        }
        
    }
        

    [TestFixture]
    public class When_hide_command_is_received : Shared
    {
        [Test]
        public void Assure_Display_property_is_set_to_false_on_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(controller_is_created);

                scenario.When("hide command is received", () =>
                    hideAnimationNotifierMock.Object.TriggerCommandPublished());

                scenario.Then("assure Display property is set to False on ViewModel", () =>
                    viewModelMock.VerifySet(vm => vm.Display = false));
            });
        
        }
    }

    [TestFixture]
    public class When_display_command_is_received : Shared
    {
        [Test]
        public void Assure_Display_property_is_set_to_true_on_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(controller_is_created);
         
                scenario.When("display command is received", () =>
                    showAnimationNotifierMock.Object.TriggerCommandPublished());

                scenario.Then("assure Display property is set to True on ViewModel", () =>
                    viewModelMock.VerifySet(vm => vm.Display = true));
            });
        }
    }
}
