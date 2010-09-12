using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Holidays;
using Smeedee.Widget.Admin.Holidays.Controllers;
using Smeedee.Widget.Admin.Holidays.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.Admin.Holidays.Tests.Controllers
{
    //ReSharper disable InconsistentNaming

    [TestFixture]
    public class when_spawning : Shared
    {
        [Test]
        public void Should_load_holidays()
        {
            Given(there_are_2_holidays_in_repository);

            When(creating_controller);

            Then("Controller should get data from repository", () =>
                holidaysRepositoryMock.Verify(hr => hr.BeginGet(It.IsAny<AllSpecification<Holiday>>()), Times.Once()));
        }

        [Test]
        public void Should_load_data_into_viewModel()
        {
            Given(there_are_2_holidays_in_repository);

            When(creating_controller);

            Then("Data sould be loaded into the view model", () =>
            {
                controller.ViewModel.Data.Count.ShouldBe(2);
                controller.ViewModel.Data[0].Date.ShouldBe(theTenDay.Date);
                controller.ViewModel.Data[0].Description.ShouldBe(theTenDay.Description);
                controller.ViewModel.Data[1].Date.ShouldBe(theEllevenDay.Date);
                controller.ViewModel.Data[1].Description.ShouldBe(theEllevenDay.Description);
            });
        }
    }

    [TestFixture]
    public class when_notified_to_refresh : Shared
    {
        [Test]
        public void Should_reload_data_from_repository()
        {
            Given(there_are_2_holidays_in_repository)
                .And(controller_is_created);

            When(notified_to_refresh);

            Then("Controller should get data from repository", () =>
                holidaysRepositoryMock.Verify(hr => hr.BeginGet(It.IsAny<AllSpecification<Holiday>>()), Times.Exactly(2)));
        }

        [Test]
        public void Should_update_the_viewModel_data()
        {
            Given(there_are_2_holidays_in_repository)
                .And(controller_is_created)
                .And(the_repository_have_changed_to_contain_theTenDay_only);

            When(notified_to_refresh);

            Then("The new data sould be loaded into the view model", () =>
            {
                controller.ViewModel.Data.Count.ShouldBe(1);
                controller.ViewModel.Data.First().Date.ShouldBe(theTenDay.Date);
                controller.ViewModel.Data.First().Description.ShouldBe(theTenDay.Description);
            });
        }
    }

    [TestFixture]
    public class when_save_command_is_executed : Shared
    {
        [Test]
        public void should_save_holidays_in_viewModel()
        {
            Given(controller_is_created);

            When(save_command_is_executed);

            Then(() => holidaysPersisterMock.Verify(hp => hp.Save(It.IsAny<IEnumerable<Holiday>>()), Times.Exactly(1)));
        }
    }

    [TestFixture]
    public class when_reload_command_is_executed : Shared
    {
        [Test]
        public void should_reload_holidays_from_repository()
        {
            Given(the_repository_is_setup_to_return_GetCompleted).
                And(controller_is_created);

            When(reload_command_is_executed);

            Then(() => holidaysRepositoryMock.Verify(h => h.BeginGet(It.IsAny<AllSpecification<Holiday>>()), Times.Exactly(2)));
        }
    }

    [TestFixture]
    public class when_createNew_command_is_executed : Shared
    {
        [Test]
        public void should_add_holiday_to_viewModel()
        {
            Given(there_are_2_holidays_in_repository).
                And(controller_is_created);

            When(createNew_command_is_executed);

            Then("there should be a new entry last in the list of holidays", () => 
                controller.ViewModel.Data.Last().Date.ShouldBe(theEllevenDay.Date.AddDays(1)));
        }
    }

    [TestFixture]
    public class when_deleteSelectedHoliday_command_is_executed : Shared
    {
        [Test]
        public void should_delete_holiday_at_selected_index()
        {
            Given(there_are_2_holidays_in_repository)
                .And(controller_is_created)
                .And(selected_index_is_0);

            When(deleteSelectedHoliday_command_is_executed);

            Then(() =>
            {
                controller.ViewModel.Data.Count.ShouldBe(1);
                controller.ViewModel.Data.First().Date.ShouldBe(theEllevenDay.Date);
                controller.ViewModel.Data.First().Description.ShouldBe(theEllevenDay.Description);
            });
        }
    }

    [TestFixture]
    public class When_loading_and_saving_data_and_settings : Shared
    {

        [Test]
        public void Assure_progressbar_is_shown_while_loading_data()
        {
            Given("the repository does not return GetCompleted");

            When(creating_controller);

            Then("the loadingNotifyer should be shown during spawn and never removed", () =>
            {
                progressbarMock.Verify(l => l.ShowInView(It.IsAny<string>()), Times.Exactly(1));
                controller.ViewModel.IsLoading.ShouldBe(true);
            });
        }

        [Test]
        public void Assure_progressbar_is_hidden_after_loading_data()
        {
            Given(the_repository_is_setup_to_return_GetCompleted);

            When(creating_controller);

            Then("", () =>
            {
                progressbarMock.Verify(l => l.HideInView(), Times.Once());
                controller.ViewModel.IsLoading.ShouldBe(false);
            });
        }

        [Test]
        public void Assure_progressbar_is_shown_while_saving_data()
        {
            Given(the_repository_is_setup_to_return_GetCompleted).
                And(controller_is_created);

            When(save_command_is_executed);

            Then("during spawn (load) and save", () =>
            {
                progressbarMock.Verify(l => l.ShowInView(It.IsAny<string>()), Times.Exactly(2));
                controller.ViewModel.IsSaving.ShouldBe(true);
            });
        }

        [Test]
        public void Assure_progressbar_is_hidden_after_saving_data()
        {
            Given(the_repository_is_setup_to_return_GetCompleted).
                And(controller_is_created).
                And(holidayPersister_is_setup_to_return_saveCompleted);

            When(save_command_is_executed);

            Then(() =>
            {
                progressbarMock.Verify(l => l.HideInView(), Times.Exactly(2));
                controller.ViewModel.IsSaving.ShouldBe(false);
            });
        }

    }

    public class Shared : ScenarioClass
    {
        protected static IUIInvoker uiInvoker = new NoUIInvokation();
        protected static Mock<ITimer> timerMock = new Mock<ITimer>();
        protected static Mock<IProgressbar> progressbarMock = new Mock<IProgressbar>();
        protected static Mock<IAsyncRepository<Holiday>> holidaysRepositoryMock = new Mock<IAsyncRepository<Holiday>>();
        protected static Mock<IPersistDomainModelsAsync<Holiday>> holidaysPersisterMock = new Mock<IPersistDomainModelsAsync<Holiday>>();

        protected static HolidaysController controller;
        protected static Holiday theTenDay = new Holiday { Date = new DateTime(10, 10, 10), Description = "The ten day" };
        protected static Holiday theEllevenDay = new Holiday { Date = new DateTime(11, 11, 11), Description = "The elleven day" };

        protected static Context controller_is_created = CreateController;

        protected static Context selected_index_is_0 = () => controller.ViewModel.SelectedHolidayIndex = 0;

        protected static Context holidayPersister_is_setup_to_return_saveCompleted = () => 
            holidaysPersisterMock.Setup(r => r.Save(It.IsAny<List<Holiday>>())).
            Raises(t => t.SaveCompleted += null, new SaveCompletedEventArgs());

        protected static Context there_are_2_holidays_in_repository = () =>
        {
            var listOfHolidays = new List<Holiday> { theTenDay, theEllevenDay };

            SetupHolidayRepsitoryMock(listOfHolidays);
        };

        protected static Context the_repository_have_changed_to_contain_theTenDay_only = () =>
            SetupHolidayRepsitoryMock(new List<Holiday> { theTenDay });

        protected static Context the_repository_is_setup_to_return_GetCompleted = () =>
            SetupHolidayRepsitoryMock(new List<Holiday>());

        protected static Context repository_no_longer_returns_GetCompleted = () => 
            holidaysRepositoryMock = new Mock<IAsyncRepository<Holiday>>();

        protected static When creating_controller = CreateController;

        protected static When notified_to_refresh = () => timerMock.Raise(n => n.Elapsed += null, new EventArgs());
        protected When save_command_is_executed = () => controller.ViewModel.SaveSettings.Execute(null);
        protected When reload_command_is_executed = () => controller.ViewModel.ReloadFromRepository.Execute(null);
        protected When createNew_command_is_executed = () => controller.ViewModel.CreateNewHoliday.Execute(null);
        protected When deleteSelectedHoliday_command_is_executed = () => controller.ViewModel.DeleteSelectedHoliday.Execute(null);

        protected static void SetupHolidayRepsitoryMock(List<Holiday> listOfHolidays)
        {
            holidaysRepositoryMock.Setup(hr => hr.BeginGet(It.IsAny<AllSpecification<Holiday>>()))
                .Raises(t => t.GetCompleted += null, new GetCompletedEventArgs<Holiday>(listOfHolidays, null));
        }

        protected static void CreateController()
        {
            controller = new HolidaysController(
                progressbarMock.Object,
                timerMock.Object,
                uiInvoker,
                holidaysRepositoryMock.Object,
                new HolidaysViewModel(),
                holidaysPersisterMock.Object);
        }

        [TearDown]
        public void RunScenario()
        {
            StartScenario();
        }

        [SetUp]
        public void Setup()
        {
            holidaysRepositoryMock = new Mock<IAsyncRepository<Holiday>>();
            holidaysPersisterMock = new Mock<IPersistDomainModelsAsync<Holiday>>();
            progressbarMock = new Mock<IProgressbar>();
        }
    }

    // ReSharper enable InconsistentNaming
}
