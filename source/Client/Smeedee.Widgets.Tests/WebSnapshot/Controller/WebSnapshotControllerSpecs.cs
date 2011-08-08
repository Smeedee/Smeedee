using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.WebSnapshot;
using Smeedee.Widgets.WebSnapshot.Controllers;
using Smeedee.Widgets.WebSnapshot.ViewModel;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework.Services;
using TinyMVVM.IoC;

namespace Smeedee.Widgets.Tests.WebSnapshot.Controller
{
    [TestFixture]
    public class When_controller_is_spawning : Shared
    {
        [Test]
        public void Assure_null_as_viewmodel_throws_exception()
        {
            Given("viewmodel is null", () => viewModel = null);
            When("creating controller");
            Then("exception should be thrown", () => this.ShouldThrowException<ArgumentNullException>(CreateController));
        }

        [Test]
        public void Assure_null_as_settings_viewmodel_throws_exception()
        {
            Given("settings viewmodel is null", () => settingsViewModel = null);
            When("creating controller");
            Then("exception should be thrown", () => this.ShouldThrowException<ArgumentNullException>(CreateController));
        }

        [Test]
        public void Assure_null_as_configuration_throws_exception()
        {
            Given("configuration is null", () => configuration = null);
            When("creating controller");
            Then("exception should be thrown", () => this.ShouldThrowException<ArgumentNullException>(CreateController));
        }

        [Test]
        public void Assure_null_as_logger_throws_exception()
        {
            Given("viewmodel is null", () => loggerMock = null);
            When("creating controller");
            Then("exception should be thrown", () => this.ShouldThrowException<ArgumentNullException>(CreateController));
        }
    }

    [TestFixture]
    public class When_Spawned : Shared
    {
        [Test]
        public void Assure_controller_is_created()
        {
            Given(controller_is_created);
            When("");
            Then("controller is not null", () => controller.ShouldNotBeNull());
        }

        [Test]
        public void Then_assure_timer_is_started()
        {
            Given(controller_is_created);
            When("");
            Then("the timer should have started",
                 () => timerMock.Verify(t => t.Start(It.IsAny<int>()), Times.Once()));
        }
    }

    [TestFixture]
    public class When_saving_configuration : Shared
    {
        [Test]
        public void Then_save_should_be_called_on_configPersister()
        {
            Given(controller_is_created);
            When(save_is_called);
            Then("save should be called on the configPersister", () =>
                 {
                     configPersisterMock.Verify(c => c.Save(It.IsAny<Configuration>()), Times.AtLeastOnce());
                     settingsViewModel.IsTimeToUpdate.ShouldBeFalse();
                 });
        }

        [Test]
        public void Assure_isTimeUpToDate_is_set_and_data_loading_is_started_when_save_completed()
        {
            Given(controller_is_created).
                And(configPersisterMock_has_saveCompletedEvent);
            When(save_is_called);
            Then(() =>
            {
                settingsViewModel.IsTimeToUpdate.ShouldBeTrue();
                repositoryMock.Verify(r => r.BeginGet(It.IsAny<Specification<DomainModel.WebSnapshot.WebSnapshot>>()), Times.Once());
            });
        }

        private Context configPersisterMock_has_saveCompletedEvent =
            () => configPersisterMock.Setup(c => c.Save(It.IsAny<Configuration>())).Raises(r => r.SaveCompleted += null, new SaveCompletedEventArgs());
    }

    [TestFixture]
    public class When_loading_data : Shared
    {
        [Test]
        public void Assure_starting_loading_sets_isLoading()
        {
            Given("");
            When(creating_controller);
            Then(() => controller.ViewModel.IsLoading.ShouldBeTrue());
        }

        [Test]
        public void Assure_load_is_called_on_repository()
        {
            Given(there_is_one_snapshot_in_repository);
            When(creating_controller);
            Then("Controller should get data from repository", () => repositoryMock.Verify(r => r.BeginGet(
                It.IsAny<Specification<DomainModel.WebSnapshot.WebSnapshot>>()), Times.Once()));

        }

        [Test]
        public void Assure_finished_loading_sets_isLoading()
        {
            Given(controller_is_created).
                And("", () => controller.ViewModel.IsLoading = true);
            When(repositoryMock_raises_getCompleted);
            Then(() => controller.ViewModel.IsLoading.ShouldBeFalse());
        }

        [Test]
        public void Assure_error_is_logged_when_load_is_completed_with_error()
        {
            Given(controller_is_created);
            When(repositoryMock_raises_getCompleted_with_error);
            Then("", () => loggerMock.Verify(l => l.WriteEntry(It.IsAny<LogEntry>())));
        }

        [Test]
        public void Assure_error_is_logged_when_load_is_completed_with_result_null()
        {
            Given(controller_is_created);
            When(repositoryMock_raises_getCompleted_with_error);
            Then("", () => loggerMock.Verify(l => l.WriteEntry(It.IsAny<LogEntry>())));
        }
    }

    public class Shared : ScenarioClass
    {
        protected static WebSnapshotController controller;
        protected static WebSnapshotViewModel viewModel;
        protected static WebSnapshotSettingsViewModel settingsViewModel;
        protected static Configuration configuration;
        protected static Mock<ITimer> timerMock;
        protected static Mock<ILog> loggerMock;
        protected static IUIInvoker uiInvoker;
        protected static Mock<IProgressbar> progressbarMock;
        protected static Mock<IPersistDomainModelsAsync<Configuration>> configPersisterMock;
        protected static Mock<IAsyncRepository<DomainModel.WebSnapshot.WebSnapshot>> repositoryMock;
        protected static DomainModel.WebSnapshot.WebSnapshot snapshot = new DomainModel.WebSnapshot.WebSnapshot { Name = "New WebSnapshot Task", PictureFilePath = @"C:\path\to\picture.png", PictureHeight = 500, PictureWidth = 600, Timestamp = "201108011141209678" };

        protected Context controller_is_created = CreateController;
        protected When creating_controller = CreateController;

        protected static void CreateController()
        {
            controller = new WebSnapshotController(
                viewModel,
                settingsViewModel,
                configuration,
                timerMock.Object,
                GetLogger(),
                uiInvoker,
                progressbarMock.Object,
                configPersisterMock.Object,
                repositoryMock.Object);
        }

        protected static ILog GetLogger()
        {
            return loggerMock != null ? loggerMock.Object : null;
        }

        protected static IEnumerable<DomainModel.WebSnapshot.WebSnapshot> result = new List<DomainModel.WebSnapshot.WebSnapshot>{snapshot};
        protected static Specification<DomainModel.WebSnapshot.WebSnapshot> specification = new WebSnapshotSpecification(); 

        protected Context repository_has_getCompleted = () => repositoryMock.Setup(r => r.BeginGet(It.IsAny<AllSpecification<DomainModel.WebSnapshot.WebSnapshot>>())).
            Raises(t => t.GetCompleted += null, new GetCompletedEventArgs<DomainModel.WebSnapshot.WebSnapshot>(result, specification));

        protected When repositoryMock_raises_getCompleted =
            () =>
            repositoryMock.Raise(r => r.GetCompleted += null,
                                 new GetCompletedEventArgs<DomainModel.WebSnapshot.WebSnapshot>(result, specification));

        protected When repositoryMock_raises_getCompleted_with_error =
            () =>
            repositoryMock.Raise(r => r.GetCompleted += null,
                                 new GetCompletedEventArgs<DomainModel.WebSnapshot.WebSnapshot>(specification, new Exception("Error!")));

        protected When repositoryMock_raises_getCompleted_with_null_result =
            () =>
            repositoryMock.Raise(r => r.GetCompleted += null,
                         new GetCompletedEventArgs<DomainModel.WebSnapshot.WebSnapshot>(null, specification));


        protected static void SetupWebSnapshotRepositoryMock(List<DomainModel.WebSnapshot.WebSnapshot> listOfSnapshots)
        {
            repositoryMock.Setup(r => r.BeginGet(It.IsAny<AllSpecification<DomainModel.WebSnapshot.WebSnapshot>>()))
                .Raises(t => t.GetCompleted += null,
                        new GetCompletedEventArgs<DomainModel.WebSnapshot.WebSnapshot>(listOfSnapshots, new AllSpecification<DomainModel.WebSnapshot.WebSnapshot>()));
        }

        protected When save_is_called = () => settingsViewModel.Save.ExecuteDelegate();

        protected Context there_is_one_snapshot_in_repository =
            () => SetupWebSnapshotRepositoryMock(new List<DomainModel.WebSnapshot.WebSnapshot> { snapshot });

        [SetUp]
        public void SetUp()
        {
            Scenario("");

            viewModel = new WebSnapshotViewModel();
            settingsViewModel = new WebSnapshotSettingsViewModel();
            configuration = WebSnapshotConfig.NewDefaultConfiguration();
            timerMock = new Mock<ITimer>();
            loggerMock = new Mock<ILog>();
  
            uiInvoker = new NoUIInvokation();
            progressbarMock = new Mock<IProgressbar>();
            configPersisterMock = new Mock<IPersistDomainModelsAsync<Configuration>>();
            repositoryMock = new Mock<IAsyncRepository<DomainModel.WebSnapshot.WebSnapshot>>();

        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }
}
