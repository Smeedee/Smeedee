using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Widgets.WebSnapshot.Controllers;
using Smeedee.Widgets.WebSnapshot.ViewModel;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework.Services;

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
            Given("configuration is null", () => config = null);
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
                repositoryMock.Verify(r => r.BeginGet(It.IsAny<Specification<DomainModel.WebSnapshot.WebSnapshot>>()), Times.Exactly(2));
            });
        }

        private Context configPersisterMock_has_saveCompletedEvent =
            () => configPersisterMock.Setup(c => c.Save(It.IsAny<Configuration>())).Raises(r => r.SaveCompleted += null, new SaveCompletedEventArgs());
    }

    [TestFixture]
    public class When_loading_data : Shared
    {
        [Test]
        public void Assure_load_is_called_on_repository()
        {
            Given(there_is_one_snapshot_in_repository);
            When(creating_controller);
            Then("Controller should get data from repository", () =>
                repositoryMock.Verify(r => r.BeginGet(
                    It.IsAny<Specification<DomainModel.WebSnapshot.WebSnapshot>>()), Times.Once()));
        }


    }

    public class Shared : ScenarioClass
    {
        protected static WebSnapshotController controller;
        protected static WebSnapshotViewModel viewModel;
        protected static WebSnapshotSettingsViewModel settingsViewModel;
        protected static Configuration config;
        protected static Mock<ITimer> timerMock;
        protected static Mock<ILog> loggerMock;
        protected static Mock<IUIInvoker> uiInvokerMock;
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
                config,
                timerMock.Object,
                GetLogger(),
                uiInvokerMock.Object,
                progressbarMock.Object,
                configPersisterMock.Object,
                repositoryMock.Object);
        }

        protected static ILog GetLogger()
        {
            return loggerMock != null ? loggerMock.Object : null;
        }

        protected static void SetupWebSnapshotRepositoryMock(List<DomainModel.WebSnapshot.WebSnapshot> listOfSnapshots)
        {
            repositoryMock.Setup(r => r.BeginGet(It.IsAny<AllSpecification<DomainModel.WebSnapshot.WebSnapshot>>()))
                .Raises(t => t.GetCompleted += null,
                        new GetCompletedEventArgs<DomainModel.WebSnapshot.WebSnapshot>(listOfSnapshots, null));
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
            config = WebSnapshotConfig.NewDefaultConfiguration();
            timerMock = new Mock<ITimer>();
            loggerMock = new Mock<ILog>();
            uiInvokerMock = new Mock<IUIInvoker>();
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
