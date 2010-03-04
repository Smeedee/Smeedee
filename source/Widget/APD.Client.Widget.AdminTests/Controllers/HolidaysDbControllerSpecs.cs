using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Framework;
using APD.Client.Framework.Controllers;
using APD.Client.Widget.Admin.Controllers;
using APD.Client.Widget.Admin.ViewModels;
using APD.DomainModel.Framework;
using APD.DomainModel.Holidays;

using Moq;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.Client.Widget.AdminTests.ControllersSpecs
{
    // ReSharper disable InconsistentNaming


    [TestFixture]
    public class Shared : ScenarioClass
    {
        protected static IInvokeUI uiInvoker = new NoUIInvocation();
        protected static ManualNotifyRefresh refreshNotifier  = new ManualNotifyRefresh();
        protected static IInvokeBackgroundWorker<IEnumerable<Holiday>> asyncClient = new NoBackgroundWorkerInvocation<IEnumerable<Holiday>>();

        protected static Mock<IRepository<Holiday>> holidaysRepositoryMock = new Mock<IRepository<Holiday>>();
        protected static Mock<ITriggerCommand> saveHolidaysTrigger = new Mock<ITriggerCommand>();
        protected static Mock<ITriggerCommand> reloadHolidaysTrigger = new Mock<ITriggerCommand>();
        protected static HolidaysDbViewModel viewModel;


        protected static HolidaysDbController controller;

        protected static Context There_are_holidays_in_repository = () =>
        {
            holidaysRepositoryMock = new Mock<IRepository<Holiday>>();
            holidaysRepositoryMock.Setup(hr => hr.Get(It.IsAny<AllSpecification<Holiday>>()))
                .Returns(new List<Holiday>()
                {
                    new Holiday() {Date = new DateTime(10, 10, 10), Description = "The ten day"},
                    new Holiday() {Date = new DateTime(11, 11, 11), Description = "The elleven day"}
                });
        };

        protected static Context The_content_of_the_repository_have_changed = () =>
        {
            holidaysRepositoryMock.Setup(hr => hr.Get(It.IsAny<AllSpecification<Holiday>>()))
                .Returns(new List<Holiday>()
                {
                    new Holiday() {Date = new DateTime(10, 10, 10), Description = "The ten day"}
                });
        };

        protected static When Creating_controller = CreateController;

        protected static Context Controller_is_created = CreateController;

        protected static When Notified_to_refresh = () => refreshNotifier.NotifyRefresh();

        protected static void CreateController()
        {
            viewModel = new HolidaysDbViewModel(uiInvoker, saveHolidaysTrigger.Object, reloadHolidaysTrigger.Object);
            controller = new HolidaysDbController(refreshNotifier, uiInvoker, holidaysRepositoryMock.Object, viewModel, asyncClient);
        }

        [TearDown]
        public void RunScenario()
        {
            this.StartScenario();
        }

    }

    [TestFixture]
    public class when_spawning : Shared
    {
        [Test]
        public void Should_load_holidays()
        {
            Given(There_are_holidays_in_repository);
            When(Creating_controller);
            Then("Controller should get data from repository", () =>
                holidaysRepositoryMock.Verify(hr => hr.Get(It.IsAny<AllSpecification<Holiday>>()), Times.Once()));
        }

        [Test]
        public void Should_load_data_into_viewModel()
        {
            Given(There_are_holidays_in_repository);
            When(Creating_controller);
            Then("Data sould be loaded into the view model", () =>
            {
                controller.ViewModel.Data.Count.ShouldBe(2);
                controller.ViewModel.Data[0].Date.ShouldBe(new DateTime(10, 10, 10));
                controller.ViewModel.Data[0].Description.ShouldBe("The ten day");
                controller.ViewModel.Data[1].Date.ShouldBe(new DateTime(11, 11, 11));
                controller.ViewModel.Data[1].Description.ShouldBe("The elleven day");
            });
        }
    }

    [TestFixture]
    public class when_notified_to_refresh : Shared
    {
        [Test]
        public void should_reload_data_from_repository()
        {
            Given(There_are_holidays_in_repository)
                .And(Controller_is_created);
            When(Notified_to_refresh);
            Then("Controller should get data from repository", () =>
                holidaysRepositoryMock.Verify(hr => hr.Get(It.IsAny<AllSpecification<Holiday>>()), Times.Exactly(2)));
        }

        [Test]
        public void should_update_the_viewModel_data()
        {
            Given(There_are_holidays_in_repository)
                .And(Controller_is_created)
                .And(The_content_of_the_repository_have_changed);
            When(Notified_to_refresh);
            Then("The new data sould be loaded into the view model", () =>
            {
                controller.ViewModel.Data.Count.ShouldBe(1);
                controller.ViewModel.Data[0].Date.ShouldBe(new DateTime(10, 10, 10));
                controller.ViewModel.Data[0].Description.ShouldBe("The ten day");
            });
        }

    }

    // ReSharper enable InconsistentNaming
}
