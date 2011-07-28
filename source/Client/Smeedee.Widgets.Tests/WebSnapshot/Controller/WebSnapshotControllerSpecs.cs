using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
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
        public void assure_OnReset_resets_image()
        {
            Given(controller_is_created);
            When(OnReset_is_called);
            Then("image is reset", () => settingsViewModel.Image.ShouldBeNull());
        }

        [Test]
        public void assure_we_get_data_from_repo()
        {
            Given(there_is_one_snapshot_in_repository);
            When(creating_controller);
            Then("Controller should get data from repository", () =>
                repository.Verify(r => r.BeginGet(
                    It.IsAny<AllSpecification<DomainModel.WebSnapshot.WebSnapshot>>()),
                    Times.Once()));

        }
    }


    public class Shared : ScenarioClass
    {
        protected static WebSnapshotController controller;
        protected static WebSnapshotViewModel viewModel;
        protected static WebSnapshotSettingsViewModel settingsViewModel;
        protected static Configuration config;
        protected static Mock<IInvokeBackgroundWorker<IEnumerable<DomainModel.WebSnapshot.WebSnapshot>>> backgroundWorker;
        protected static Mock<ITimer> timer;
        protected static Mock<ILog> logger;
        protected static Mock<IUIInvoker> uiInvoker;
        protected static Mock<IProgressbar> progressbar;
        protected static Mock<IPersistDomainModelsAsync<Configuration>> configPersister;
        protected static Mock<IAsyncRepository<Smeedee.DomainModel.WebSnapshot.WebSnapshot>> repository;
        protected static DomainModel.WebSnapshot.WebSnapshot snapshot = new DomainModel.WebSnapshot.WebSnapshot { Name = "New WebSnapshot Task", PictureFilePath = @"C:\path\to\picture.png", PictureHeight = 500, PictureWidth = 600};
        
        protected Context controller_is_created = CreateController;
        protected When creating_controller = CreateController;

        protected static void CreateController()
        {
            controller = new WebSnapshotController(
                viewModel, 
                settingsViewModel, 
                config, 
                backgroundWorker.Object, 
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

        protected When OnReset_is_called = () => settingsViewModel.Reset.ExecuteDelegate();

        protected Context there_is_one_snapshot_in_repository =
            () => SetupWebSnapshotRepositoryMock(new List<DomainModel.WebSnapshot.WebSnapshot> {snapshot});


        [SetUp]
        public void SetUp()
        {
            Scenario("");
             
            viewModel = new WebSnapshotViewModel();
            settingsViewModel = new WebSnapshotSettingsViewModel();
            config = new Configuration();
            backgroundWorker = new Mock<IInvokeBackgroundWorker<IEnumerable<DomainModel.WebSnapshot.WebSnapshot>>>();
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
