using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Framework;
using APD.Client.Framework.Controllers;
using APD.Client.Framework.ViewModels;
using APD.Client.Widget.Admin.ViewModels;
using APD.Tests;

using Moq;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.Client.Widget.AdminTests.ViewModels.HolidayDbViewModelSpecs
{
    public class Shared
    {
        protected static HolidaysDbViewModel viewModel;
        protected static Mock<ITriggerCommand> saveHolidaysCommandTriggererMock;
        protected static Mock<ITriggerCommand> reloadHolidaysCommandTriggererMock;
        protected static Mock<ITriggerCommand> newHolidayCommandTrigger = new Mock<ITriggerCommand>();
        protected static Mock<ITriggerCommand> deleteHolidayCommandTrigger = new Mock<ITriggerCommand>();

        protected Context viewModel_is_spawned = CreateViewModel;

        protected Context viewModel_is_created = CreateViewModel;

        protected Context data_is_not_dirty = () =>
        {
            viewModel.DataIsChanged = false;
        };

        protected Context viewModel_contains_data = () =>
        {
            var holiday = new HolidayViewModel(new NoUIInvocation());
            holiday.Date = new DateTime(2010, 11, 11);
            holiday.Description = "Hello!";

            viewModel.Data.Add(holiday);
        };

        protected Context data_is_changed = () =>
        {
            viewModel.DataIsChanged = true;
        };


        protected static void CreateViewModel()
        {
            saveHolidaysCommandTriggererMock = new Mock<ITriggerCommand>();
            reloadHolidaysCommandTriggererMock = new Mock<ITriggerCommand>();
            viewModel = new HolidaysDbViewModel(new NoUIInvocation(), saveHolidaysCommandTriggererMock.Object, reloadHolidaysCommandTriggererMock.Object, newHolidayCommandTrigger.Object, deleteHolidayCommandTrigger.Object);
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
            viewModel.ShouldBeInstanceOfType<BindableViewModel<HolidayViewModel>>();
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
                    PropertyTester.TestChange<HolidaysDbViewModel>(viewModel, vm => vm.DataIsChanged));
                scenario.Then("assure observers are notified about the change", () =>
                    PropertyTester.WasNotified.ShouldBeTrue());
            });
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
                    viewModel.Data.First().Description = "New decription of a holiday");
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
                    viewModel.Data.Add(new HolidayViewModel(new NoUIInvocation())));
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
                    viewModel.SaveHolidaysUICommand.Execute(null));
                scenario.Then("assure SaveHolidaysUICommand is triggered", () =>
                    saveHolidaysCommandTriggererMock.Verify(c => c.Trigger(), Times.Once()));
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
                    viewModel.ReloadHolidaysUICommand.Execute(null));
                scenario.Then("assure ReloadHolidaysUICommand is triggered", () =>
                    reloadHolidaysCommandTriggererMock.Verify(c => c.Trigger(), Times.Once()));
            });
        }
    }

    [TestFixture]
    public class When_adding_new_holiday : Shared
    {
        [Test]
        public void Assure_CreateNewHolidayUICommand_is_triggered()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_is_created);
                scenario.When("adding a new holiday", () =>
                    viewModel.CreateNewHolidayUICommand.Execute(null));
                scenario.Then("assure CreateNewHolidayUICommand is triggered", () =>
                    newHolidayCommandTrigger.Verify(c => c.Trigger(), Times.Once()));
            });
        }
    }

    [TestFixture]
    public class When_deleting_holiday : Shared
    {
        [Test]
        public void Assure_DeleteSelectedHolidayUICommand_is_triggered()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_is_created);
                scenario.When("adding a new holiday", () =>
                    viewModel.DeleteSelectedHolidayUICommand.Execute(null));
                scenario.Then("assure DeleteSelectedHolidayUICommand is triggered", () =>
                    deleteHolidayCommandTrigger.Verify(c => c.Trigger(), Times.Once()));
            });
        }
    }
}
