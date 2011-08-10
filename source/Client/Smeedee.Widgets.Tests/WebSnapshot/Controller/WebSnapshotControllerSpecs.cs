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

        [Test]
        public void Assure_notified_to_refresh()
        {
            Given(controller_is_created);
            When("timer has elapsed", () => timerMock.Raise(n => n.Elapsed += null, new EventArgs()));
            Then("onNotifiedToRefresh and loaddata is called", 
                () => repositoryMock.Verify(r => r.BeginGet(It.IsAny<Specification<DomainModel.WebSnapshot.WebSnapshot>>())));
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
            When(repositoryMock_raises_getCompleted_with_null_result);
            Then("", () => loggerMock.Verify(l => l.WriteEntry(It.IsAny<LogEntry>())));
        }

        [Test]
        public void Assure_returns_and_clears_when_load_has_empty_snapshotlist()
        {
            Given(controller_is_created).
                And(there_is_one_snapshot_in_repository);
            When(repositoryMock_raises_getCompleted);
            Then("return when gets to updateviewmodel", () =>
            {
                settingsViewModel.IsTimeToUpdate.ShouldBeFalse();
                settingsViewModel.AvailableImages.Count.ShouldBe(0);
                settingsViewModel.AvailableImagesUri.Count.ShouldBe(0);
            });
        }

        [Test]
        public void Assure_updates_availableImages_when_load_has_populated_snapshotlist()
        {
            Given(controller_is_created).
                And(there_is_one_snapshot_in_repository);
            When(repositoryMock_raises_getCompleted_with_populated_snapshotlist);
            Then("available images should be updated", () => settingsViewModel.AvailableImages.Count.ShouldBe(2));
        }

        [Test]
        public void Assure_returns_when_snapshot_element_is_same_as_selected_image_and_firstrun_is_true()
        {
            Given(controller_is_created).
                And("an image is selected", () => settingsViewModel.SelectedImage = "WebSnapshotTask2").
                And("settingsviewmodel has an image", () => settingsViewModel.Image = new object());
            When(repositoryMock_raises_getCompleted_with_populated_snapshotlist);
            Then(() => settingsViewModel.IsTimeToUpdate.ShouldBeFalse());
        }

        [Test]
        public void Assure_is_time_to_update_is_true_when_snapshot_element_is_same_as_selected_image()
        {
            Given(controller_is_created).
                And("an image is selected", () => settingsViewModel.SelectedImage = "WebSnapshotTask2").
                And("settingsviewmodel has an image", () => settingsViewModel.Image = new object()).
                And(repositoryMock_has_raised_getCompleted_with_populated_snapshotlist);
            When(repositoryMock_raises_getCompleted_with_populated_snapshotlist);
            Then(() => settingsViewModel.IsTimeToUpdate.ShouldBeTrue());
        }

        [Test]
        public void Assure_is_time_to_update_is_false_when_snapshot_element_is_same_as_selected_image_and_timestamp_is_the_same()
        {
            Given(controller_is_created).
                And("an image is selected", () => settingsViewModel.SelectedImage = "WebSnapshotTask2").
                And("settingsviewmodel has an image", () => settingsViewModel.Image = new object()).
                And(repositoryMock_has_raised_getCompleted_with_populated_snapshotlist).
                And(repositoryMock_has_raised_getCompleted_with_populated_snapshotlist);
            When(repositoryMock_raises_getCompleted_with_populated_snapshotlist);
            Then(() => settingsViewModel.IsTimeToUpdate.ShouldBeFalse());
        }

        [Test]
        public void Assure_is_time_to_update_is_true_when_settingsviewmodel_image_is_null()
        {
            Given(controller_is_created);
            When(repositoryMock_raises_getCompleted_with_populated_snapshotlist);
            Then(() => settingsViewModel.IsTimeToUpdate.ShouldBeTrue());
        }

        [Test]
        public void Assure_populates_available_images_when_snapshot_list_is_populated()
        {
            Given(controller_is_created);
            When(repositoryMock_raises_getCompleted_with_populated_snapshotlist);
            Then(() =>
            {
                settingsViewModel.AvailableImages.Count.ShouldBe(2);
                settingsViewModel.AvailableImagesUri.Count.ShouldBe(2);
            });
        }

        [Test]
        public void Assure_selected_image_is_set_when_populate_available_images()
        {
            Given(controller_is_created).
                And("settingsviewmodel has an image", () => settingsViewModel.SelectedImage = "MySelectedImage");
            When(repositoryMock_raises_getCompleted_with_populated_snapshotlist);
            Then(() =>
            {
                settingsViewModel.SelectedImage.ShouldBe("MySelectedImage");
            });
        }



        private static IEnumerable<DomainModel.WebSnapshot.WebSnapshot> result = new List<DomainModel.WebSnapshot.WebSnapshot>();
        private static Specification<DomainModel.WebSnapshot.WebSnapshot> specification = new WebSnapshotSpecification();

        private Context repository_has_getCompleted = () => repositoryMock.Setup(r => r.BeginGet(It.IsAny<AllSpecification<DomainModel.WebSnapshot.WebSnapshot>>())).
            Raises(t => t.GetCompleted += null, new GetCompletedEventArgs<DomainModel.WebSnapshot.WebSnapshot>(result, specification));


        private Context repositoryMock_has_raised_getCompleted_with_populated_snapshotlist = () => repositoryMock.Raise(r => r.GetCompleted += null,
                                 new GetCompletedEventArgs<DomainModel.WebSnapshot.WebSnapshot>(populatedResult, specification));


        private When repositoryMock_raises_getCompleted =
            () =>
            repositoryMock.Raise(r => r.GetCompleted += null,
                                 new GetCompletedEventArgs<DomainModel.WebSnapshot.WebSnapshot>(result, specification));

        private When repositoryMock_raises_getCompleted_with_error =
            () =>
            repositoryMock.Raise(r => r.GetCompleted += null,
                                 new GetCompletedEventArgs<DomainModel.WebSnapshot.WebSnapshot>(specification, new Exception("Error!")));

        private When repositoryMock_raises_getCompleted_with_null_result =
            () =>
            repositoryMock.Raise(r => r.GetCompleted += null,
                         new GetCompletedEventArgs<DomainModel.WebSnapshot.WebSnapshot>(null, specification));

        private static IEnumerable<DomainModel.WebSnapshot.WebSnapshot> populatedResult = new List<DomainModel.WebSnapshot.WebSnapshot> { snapshot1, snapshot2 };
        private When repositoryMock_raises_getCompleted_with_populated_snapshotlist = () => repositoryMock.Raise(r => r.GetCompleted += null,
                                 new GetCompletedEventArgs<DomainModel.WebSnapshot.WebSnapshot>(populatedResult, specification));


    }

    [TestFixture]
    public class When_reloading_settings : Shared
    {
        [Test]
        public void Assure_settings_are_copied_to_settingsviewmodel()
        {
            Given(controller_is_created);
            When(reloadsettings_is_called);
            Then("", () =>
            {
                settingsViewModel.SelectedImage.ShouldBe(webSnapshotConfig.TaskName);
                settingsViewModel.CropCoordinateX.ShouldBe(webSnapshotConfig.CoordinateX);
                settingsViewModel.CropCoordinateY.ShouldBe(webSnapshotConfig.CoordinateY);
                settingsViewModel.CropRectangleHeight.ShouldBe(webSnapshotConfig.RectangleHeight);
                settingsViewModel.CropRectangleWidth.ShouldBe(webSnapshotConfig.RectangleWidth);
            });
        }

        [Test]
        public void Assure_load_data_is_called_when_reload_settings_is_finished()
        {
            Given(controller_is_created);
            When(reloadsettings_is_called);
            Then("load data should be called", () => repositoryMock.Verify(r => r.BeginGet(It.IsAny<Specification<DomainModel.WebSnapshot.WebSnapshot>>())));
        }

        private When reloadsettings_is_called = () => settingsViewModel.ReloadSettings.ExecuteDelegate();

    }

    [TestFixture]
    public class When_configuration_is_updated : Shared
    {
        [Test]
        public void Assure_configuration_is_correctly_copied_to_settingsviewmodel()
        {
            Given(controller_is_created);
            When(configuration_is_changed);
            Then(() =>
            {
                settingsViewModel.SelectedImage.ShouldBe("myTaskName");
                settingsViewModel.CropCoordinateX.ShouldBe("myXCoord");
                settingsViewModel.CropCoordinateY.ShouldBe("myYCoord");
                settingsViewModel.CropRectangleHeight.ShouldBe("myRecHeight");
                settingsViewModel.CropRectangleWidth.ShouldBe("myRecWidth");
            });
        }

        [Test]
        public void Assure_loading_data_starts()
        {
            Given(controller_is_created);
            When(configuration_is_changed);
            Then(
                () =>
                repositoryMock.Verify(l => l.BeginGet(It.IsAny<Specification<DomainModel.WebSnapshot.WebSnapshot>>())));
        }

        private static Configuration aConfiguration = new Configuration("myConfig");

        private static void populateConfiguration()
        {
            aConfiguration.NewSetting(WebSnapshotConfig.taskname, "myTaskName");
            aConfiguration.NewSetting(WebSnapshotConfig.coordinateX, "myXCoord");
            aConfiguration.NewSetting(WebSnapshotConfig.coordinateY, "myYCoord");
            aConfiguration.NewSetting(WebSnapshotConfig.rectangleHeight, "myRecHeight");
            aConfiguration.NewSetting(WebSnapshotConfig.rectangleWidth, "myRecWidth");
            aConfiguration.NewSetting(WebSnapshotConfig.timestamp, "myTime");
            aConfiguration.IsConfigured = false;
        }

        private When configuration_is_changed = () => 
        {
            populateConfiguration();
            controller.UpdateConfiguration(aConfiguration); 
        };
    }

    [TestFixture]
    public class When_show_image_in_settingsview : Shared
    {
        [Test]
        public void Assure_settingsviewmodel_image_is_set()
        {
            Given(controller_is_created).
            And("loaded image", () => settingsViewModel.LoadedImage = "a loaded image");
            When("we want to show image in settingsview", () => controller.ShowImageInSettingsView());
            Then(() => settingsViewModel.Image.ShouldBe("a loaded image"));
        }
    }

    


    public class Shared : ScenarioClass
    {
        protected static WebSnapshotController controller;
        protected static WebSnapshotViewModel viewModel;
        protected static WebSnapshotSettingsViewModel settingsViewModel;
        protected static WebSnapshotConfig webSnapshotConfig;
        protected static Configuration configuration;
        protected static Mock<ITimer> timerMock;
        protected static Mock<ILog> loggerMock;
        protected static IUIInvoker uiInvoker;
        protected static Mock<IProgressbar> progressbarMock;
        protected static Mock<IPersistDomainModelsAsync<Configuration>> configPersisterMock;
        protected static Mock<IAsyncRepository<DomainModel.WebSnapshot.WebSnapshot>> repositoryMock;

        protected static DomainModel.WebSnapshot.WebSnapshot snapshot1 = new DomainModel.WebSnapshot.WebSnapshot { Name = "WebSnapshotTask1", PictureFilePath = @"C:\path\to\picture1.png", PictureHeight = 500, PictureWidth = 600, Timestamp = "201108011141209678" };
        protected static DomainModel.WebSnapshot.WebSnapshot snapshot2 = new DomainModel.WebSnapshot.WebSnapshot { Name = "WebSnapshotTask2", PictureFilePath = @"C:\path\to\picture2.png", PictureHeight = 500, PictureWidth = 600, Timestamp = "201108011141209678" };

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

        

        protected static void SetupWebSnapshotRepositoryMock(List<DomainModel.WebSnapshot.WebSnapshot> listOfSnapshots)
        {
            repositoryMock.Setup(r => r.BeginGet(It.IsAny<AllSpecification<DomainModel.WebSnapshot.WebSnapshot>>()))
                .Raises(t => t.GetCompleted += null,
                        new GetCompletedEventArgs<DomainModel.WebSnapshot.WebSnapshot>(listOfSnapshots, new AllSpecification<DomainModel.WebSnapshot.WebSnapshot>()));
        }

        protected When save_is_called = () => settingsViewModel.Save.ExecuteDelegate();

        protected Context there_is_one_snapshot_in_repository =
            () => SetupWebSnapshotRepositoryMock(new List<DomainModel.WebSnapshot.WebSnapshot> { snapshot1 });

        [SetUp]
        public void SetUp()
        {
            Scenario("");

            viewModel = new WebSnapshotViewModel();
            settingsViewModel = new WebSnapshotSettingsViewModel();
            configuration = WebSnapshotConfig.NewDefaultConfiguration();
            webSnapshotConfig = new WebSnapshotConfig(configuration);
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
