using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Holidays;
using Smeedee.Tests;
using Smeedee.Widget.Admin.Holidays.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widget.Admin.Holidays.Tests.ViewModels
{
    public class Shared
    {
        protected static HolidaysViewModel viewModel;

        protected static Mock<IRepository<Holiday>> holidaysRepositoryMock = new Mock<IRepository<Holiday>>();
        protected static Mock<IPersistDomainModels<Holiday>> holidayPersisterMock = new Mock<IPersistDomainModels<Holiday>>();

        protected Context viewModel_is_spawned = CreateViewModel;

        protected Context data_is_not_dirty = () =>
        {
            viewModel.DataIsChanged = false;
        };

        protected Context viewModel_contains_data = () =>
        {
            var holiday = new SingleHolidayViewModel();
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
            viewModel = new HolidaysViewModel();
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
            viewModel.ShouldBeInstanceOfType<BindableViewModel<SingleHolidayViewModel>>();
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
                    PropertyTester.TestChange(viewModel, vm => vm.DataIsChanged));
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
                scenario.Given(viewModel_is_spawned).
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
                scenario.Given(viewModel_is_spawned).
                    And(data_is_not_dirty);
                scenario.When("new row is added by the user", () =>
                    viewModel.Data.Add(new SingleHolidayViewModel()));
                scenario.Then("assure DataIsChange is set to true", () =>
                    viewModel.DataIsChanged.ShouldBeTrue());
            });
        }
    }
}
