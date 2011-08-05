using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.SourceControl;
using Smeedee.Widgets.SourceControl.Controllers;
using Smeedee.Widgets.SourceControl.ViewModels;
using Smeedee.Widgets.Tests.SourceControl.Controllers;
using TinyBDD.Dsl.GivenWhenThen;

namespace Smeedee.Widgets.Tests.SourceControl.Integration
{
    [TestFixture]
    class CommitStatisticsSpecs : ScenarioClass
    {
        private CommitStatisticsController _controller;
        private static Mock<IWidget> widgetMock;
        private static CommitStatisticsSettingsViewModel settingsViewModel;
        private static Mock<IPersistDomainModelsAsync<Configuration>> configPersister;
        protected static Mock<IProgressbar> loadingNotifyerMock = new Mock<IProgressbar>();

        [SetUp]
        public void Setup()
        {
            widgetMock = new Mock<IWidget>();
            MockRepository();
            configPersister = new Mock<IPersistDomainModelsAsync<Configuration>>();
            settingsViewModel = new CommitStatisticsSettingsViewModel();

            _controller = new CommitStatisticsController(new BindableViewModel<CommitStatisticsForDate>(), 
                                                        settingsViewModel,
                                                        new Mock<IAsyncRepository<Changeset>>().Object,
                                                        new Mock<ITimer>().Object,
                                                        new NoUIInvokation(),
                                                        configPersister.Object,
                                                        new Logger(new LogEntryMockPersister()),
                                                        loadingNotifyerMock.Object,
                                                        widgetMock.Object);
        }

        private void MockRepository()
        {
            
            var config = new Configuration("Commit Statistics");
            config.NewSetting("timespan", "4");
            var fakeConfig = new List<Configuration> { config };

            widgetMock.SetupGet(w => w.Configuration).Returns(config);
        }

        private When save_command_is_called = () =>
        {
            settingsViewModel.SaveSettings.ExecuteDelegate();
        };

        private Then configPersister_Save_is_called_exactly_once = () =>
        {
            configPersister.Verify(r => r.Save(It.IsAny<Configuration>()), Times.Exactly(1));
        };

        [Test]
        public void Assure_save_command_calls_save_once()
        {
            Given("view model is instantiated");
            When(save_command_is_called);
            Then(configPersister_Save_is_called_exactly_once);
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }
}
