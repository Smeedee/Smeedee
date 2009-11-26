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
// <author>Your name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System.Linq;
using APD.Client.Framework.ViewModels;
using APD.Client.Widget.Admin.ViewModels;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.Client.Framework;
using APD.Tests;
using Moq;
using System;
using APD.Client.Framework.Controllers;

namespace APD.Client.Widget.AdminTests.ViewModels.UserdbViewModelSpecs
{
    public class Shared
    {
        protected static UserdbViewModel viewModel;
        protected static Mock<ITriggerCommand> saveUserdbCommandTriggererMock;
        protected static Mock<ITriggerCommand> reloadUserdbCommandTriggererMock;
        protected static Mock<ITriggerEvent<EventArgs>> editCommandTriggerMock;

        protected Context viewModel_is_spawned = () =>
        {
            CreateViewModel();
            
        };

        protected Context viewModel_is_created = () =>
        {
            CreateViewModel();
        };

        protected Context data_is_not_dirty = () =>
        {
            viewModel.DataIsChanged = false;
        };

        protected Context viewModel_contains_data = () =>
        {
            var user = new UserViewModel(new NoUIInvocation());
            user.Username = "goeran";
            user.Email = "mail@goeran.no";
            user.ImageUrl = "http://goeran.no/haldis.jpg";
            user.Firstname = "Gøran";
            user.Surname = "Hansen";

            viewModel.Data.Add(user);
        };

        protected Context data_is_changed = () =>
        {
            viewModel.DataIsChanged = true;
        };

        protected static void CreateViewModel()
        {
            saveUserdbCommandTriggererMock = new Mock<ITriggerCommand>();
            reloadUserdbCommandTriggererMock = new Mock<ITriggerCommand>();
            editCommandTriggerMock = new Mock<ITriggerEvent<EventArgs>>();
            viewModel = new UserdbViewModel(new NoUIInvocation(), saveUserdbCommandTriggererMock.Object, reloadUserdbCommandTriggererMock.Object, editCommandTriggerMock.Object);
        }
    }

    [TestFixture]
    public class When_spawned : Shared
    {
        [SetUp]
        public void Setup()
        {
            CreateViewModel();
        }

        [Test]
        public void Assure_ViewModel_isa_BindableViewModel()
        {
            viewModel.ShouldBeInstanceOfType<BindableViewModel<UserViewModel>>();
        }
 
        [Test]
        public void Assure_DataIsChanged_is_false_by_default()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_is_spawned);
                scenario.When("DataIsChanged property is accessed");
                scenario.Then("assure false is returned by default", () =>
                    viewModel.DataIsChanged.ShouldBeFalse());
            });
        }

        [Test]
        public void Assure_DataIsChanged_is_observable()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_is_spawned);
                scenario.When("DataIsChange property changes", () =>
                    PropertyTester.TestChange<UserdbViewModel>(viewModel, vm => vm.DataIsChanged));
                scenario.Then("assure observers are notified about the change", () =>
                    PropertyTester.WasNotified.ShouldBeTrue());
            });
        }

        [Test]
        public void Assure_Name_is_Empty_string_by_default()
        {
            viewModel.Name.ShouldBe(string.Empty);
        }
    }

    [TestFixture]
    public class When_data_is_changed_by_user : Shared
    {
        [Test]
        public void Assure_DataIsChange_is_set_to_true()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_is_created).
                    And(viewModel_contains_data).
                    And(data_is_not_dirty);
                scenario.When("a row is updated by the user", () =>
                    viewModel.Data.First().Firstname = "Gøran");
                scenario.Then("assure DataIsChange is set to true", () =>
                    viewModel.DataIsChanged.ShouldBeTrue());
            });
        }

        [Test]
        public void Assure_DataIsChange_is_set_to_true_when_first_item_is_added()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_is_created).
                    And(data_is_not_dirty);
                scenario.When("new row is added by the user", () =>
                    viewModel.Data.Add(new UserViewModel(new NoUIInvocation())));
                scenario.Then("assure DataIsChange is set to true", () =>
                    viewModel.DataIsChanged.ShouldBeTrue());
            });
        }
    }

    [TestFixture]
    public class When_save : Shared
    {
        [Test]
        public void Assure_SaveUserdbCommand_is_triggered()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_is_created).
                    And(viewModel_contains_data).
                    And(data_is_changed);
                scenario.When("saving", () =>
                    viewModel.SaveUserdbUICommand.Execute(null));
                scenario.Then("assure SaveUserdbUICommand is triggered", () =>
                    saveUserdbCommandTriggererMock.Verify(c => c.Trigger(), Times.Once()));
            });
        }
    }

    [TestFixture]
    public class When_Reload : Shared
    {
        [Test]
        public void Assure_ReloadUserdbCommand_is_triggered()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_is_created);
                scenario.When("reload", () =>
                    viewModel.ReloadUserdbUICommand.Execute(null));
                scenario.Then("assure ReloadUserdbCommand is triggered", () =>
                    reloadUserdbCommandTriggererMock.Verify(c => c.Trigger(), Times.Once()));
            });
        }
    }

    [TestFixture]
    public class When_edit : Shared
    {
        [Test]
        public void Assure_EditCommand_is_triggered()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_is_created);
                scenario.When("edit", () =>
                    viewModel.EditUICommand.Execute(null));
                scenario.Then("assure EditCommand is triggered", () =>
                    editCommandTriggerMock.Verify(t => t.NewEvent(It.IsAny<EventArgs>()), Times.Once()));
            });
        }
    }
}
