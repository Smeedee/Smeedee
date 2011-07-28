using System.Collections.Generic;
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
        protected static Mock<IRepository<Smeedee.DomainModel.WebSnapshot.WebSnapshot>> repository;

        protected Context controller_is_created = () =>
                                                      {
                                                          controller = new WebSnapshotController(viewModel, settingsViewModel, config, backgroundWorker.Object, timer.Object, logger.Object, uiInvoker.Object, progressbar.Object, configPersister.Object, repository.Object);
                                                      };

        protected When OnReset_is_called = () => settingsViewModel.Reset.ExecuteDelegate();
        protected When OnSave_is_called = () => settingsViewModel.Save.ExecuteDelegate();




        [SetUp]
        public void SetUp()
        {
            Scenario("");
             
            viewModel = new WebSnapshotViewModel();
            settingsViewModel = new WebSnapshotSettingsViewModel();
            config = WebSnapshotConfig.NewDefaultConfiguration();
            backgroundWorker = new Mock<IInvokeBackgroundWorker<IEnumerable<DomainModel.WebSnapshot.WebSnapshot>>>();
            timer = new Mock<ITimer>();
            logger = new Mock<ILog>();
            uiInvoker = new Mock<IUIInvoker>();
            progressbar = new Mock<IProgressbar>();
            configPersister = new Mock<IPersistDomainModelsAsync<Configuration>>();
            repository = new Mock<IRepository<DomainModel.WebSnapshot.WebSnapshot>>();

        }
        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }

    }
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

    }

    [TestFixture]
    public class When_saving_configuration : Shared
    {
        private Context there_is_settings_in_viewmodel = () =>
                                                             {

                                                             };
        [Test]
        public void Then_save_should_be_called_on_configPersister()
        {
            Given(controller_is_created);
            When(OnSave_is_called);
            Then("save should be called on the configPersister",
                 () => configPersister.Verify(c => c.Save(It.IsAny<Configuration>()), Times.AtLeastOnce()));
        }

        [Test]
        public void Then_string_settings_from_viewmodel_should_be_copied_into_configuration()
        {
            Given(controller_is_created).And(there_is_settings_in_viewmodel); ;
            When("");
            Then("");
        }
    }
}
