using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Repositories;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
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
    public class When_Spawned : Shared
    {
        [Test]
        public void assure_controller_is_created()
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
                 () => timer.Verify(t => t.Start(It.IsAny<int>()), Times.Once()));
        }

        [Test]
        public void assure_we_get_data_from_repo()
        {
            Given(there_is_one_snapshot_in_repository);
            When(creating_controller);
            Then("Controller should get data from repository", () =>
                repository.Verify(r => r.BeginGet(
                    It.IsAny<Specification<DomainModel.WebSnapshot.WebSnapshot>>()),
                    Times.Once()));
        }
    }

    [TestFixture]
    public class When_saving_configuration : Shared
    {
        [Test]
        public void Then_save_should_be_called_on_configPersister()
        {
            Given(controller_is_created);
            When(OnSave_is_called);
            Then("save should be called on the configPersister",
                 () => configPersister.Verify(c => c.Save(It.IsAny<Configuration>()), Times.AtLeastOnce()));
        }
    }


    public class Shared : ScenarioClass
    {
        protected static WebSnapshotController controller;
        protected static WebSnapshotViewModel viewModel;
        protected static WebSnapshotSettingsViewModel settingsViewModel;
        protected static Configuration config;
        protected static Mock<ITimer> timer;
        protected static Mock<ILog> logger;
        protected static Mock<IUIInvoker> uiInvoker;
        protected static Mock<IProgressbar> progressbar;
        protected static Mock<IPersistDomainModelsAsync<Configuration>> configPersister;
        protected static Mock<IAsyncRepository<Smeedee.DomainModel.WebSnapshot.WebSnapshot>> repository;
        protected static DomainModel.WebSnapshot.WebSnapshot snapshot = new DomainModel.WebSnapshot.WebSnapshot { Name = "New WebSnapshot Task", PictureFilePath = @"C:\path\to\picture.png", PictureHeight = 500, PictureWidth = 600, Timestamp = "201108011141209678" };

        protected Context controller_is_created = CreateController;
        protected When creating_controller = CreateController;

        protected static void CreateController()
        {
            controller = new WebSnapshotController(
                viewModel,
                settingsViewModel,
                config,
                timer.Object,
                logger.Object,
                uiInvoker.Object,
                progressbar.Object,
                configPersister.Object,
                repository.Object);
        }

        protected static void SetupWebSnapshotRepositoryMock(List<DomainModel.WebSnapshot.WebSnapshot> listOfSnapshots)
        {
            repository.Setup(r => r.BeginGet(It.IsAny<AllSpecification<DomainModel.WebSnapshot.WebSnapshot>>()))
                .Raises(t => t.GetCompleted += null,
                        new GetCompletedEventArgs<DomainModel.WebSnapshot.WebSnapshot>(listOfSnapshots, null));
        }

        protected When OnSave_is_called = () => settingsViewModel.Save.ExecuteDelegate();

        protected Context there_is_one_snapshot_in_repository =
            () => SetupWebSnapshotRepositoryMock(new List<DomainModel.WebSnapshot.WebSnapshot> { snapshot });


        [SetUp]
        public void SetUp()
        {
            Scenario("");

            viewModel = new WebSnapshotViewModel();
            settingsViewModel = new WebSnapshotSettingsViewModel();
            config = WebSnapshotConfig.NewDefaultConfiguration();
            timer = new Mock<ITimer>();
            logger = new Mock<ILog>();
            uiInvoker = new Mock<IUIInvoker>();
            progressbar = new Mock<IProgressbar>();
            configPersister = new Mock<IPersistDomainModelsAsync<Configuration>>();
            repository = new Mock<IAsyncRepository<DomainModel.WebSnapshot.WebSnapshot>>();

        }
        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }

    }
}
