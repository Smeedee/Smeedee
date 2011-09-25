using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Widget.SourceControlTests.Controllers.CommitStatisticsControllerSpecs;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.SourceControl;
using Smeedee.Widget.SourceControl.Controllers;
using Smeedee.Widget.SourceControl.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.SourceControl.Tests.Integration
{
    [TestFixture]
    class CommitStatisticsSpecs : ScenarioClass
    {
        private CommitStatisticsController _controller;
        private Mock<IAsyncRepository<Configuration>> configRepository;
        private static CommitStatisticsSettingsViewModel settingsViewModel;
        private static Mock<IPersistDomainModelsAsync<Configuration>> configPersister;
        protected static Mock<IProgressbar> loadingNotifyerMock = new Mock<IProgressbar>();

        [SetUp]
        public void Setup()
        {
            MockRepository();
            configPersister = new Mock<IPersistDomainModelsAsync<Configuration>>();
            settingsViewModel = new CommitStatisticsSettingsViewModel();

            _controller = new CommitStatisticsController(new BindableViewModel<CommitStatisticsForDate>(), 
                                                        settingsViewModel,
                                                        new Mock<IRepository<Changeset>>().Object,
                                                        new NoBackgroundWorkerInvocation<IEnumerable<Changeset>>(),
                                                        new Mock<ITimer>().Object,
                                                        new NoUIInvokation(),
                                                        configRepository.Object,
                                                        configPersister.Object,
                                                        new Logger(new LogEntryMockPersister()),
                                                        loadingNotifyerMock.Object);
        }

        private void MockRepository()
        {
            
            var config = new Configuration("Commit Statistics");
            config.NewSetting("timespan", "4");
            var fakeConfig = new List<Configuration> { config };

            configRepository = new Mock<IAsyncRepository<Configuration>>();
            configRepository.Setup(
                r => r.BeginGet(It.IsAny<Specification<Configuration>>())).Raises(
                t => t.GetCompleted += null, new GetCompletedEventArgs<Configuration>(fakeConfig, null));
        }

        private When save_command_is_called = () =>
        {
            settingsViewModel.SaveSettings.Execute(null);
        };

        private Then configPersister_Save_is_called_exactly_once = () =>
        {
            configPersister.Verify(r => r.Save(It.IsAny<Configuration>()), Times.Exactly(1));
        };

        [Test]
        public void Assure_save_command_calls_save_once()
        {
            Given("view_model_is_instantiated");
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
