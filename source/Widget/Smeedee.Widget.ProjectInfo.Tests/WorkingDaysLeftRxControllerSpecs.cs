using System;
using System.Collections.Generic;
using System.Globalization;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Tests;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Holidays;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Widget.ProjectInfo;
using Smeedee.Widget.ProjectInfo.ViewModels;
using Smeedee.Widget.ProjectInfoTests.Controllers.WorkingDaysLeftViewModelSpecs;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Testing.Services;
using TinyMVVM.IoC;

namespace Smeedee.Client.Widget.ProjectInfoTests
{
    public class shared
    {

        protected const string SERVER_NAME = "Mock Server";
        protected const string PROJECT_NAME = "Mock Project";

        public static List<Configuration> CreateConfiguration(bool isManuallyConfigured, DateTime endDate)
        {
            var _configuration = new Configuration("project-info");
            _configuration.NewSetting(WorkingDaysLeftController.SETTING_IS_MANUALLY_CONFIGURED, isManuallyConfigured ? "true" : "false");
            _configuration.NewSetting(WorkingDaysLeftController.SETTING_END_DATE, endDate.ToString(CultureInfo.InvariantCulture));
            _configuration.NewSetting(WorkingDaysLeftController.SETTING_SERVER, SERVER_NAME);
            _configuration.NewSetting(WorkingDaysLeftController.SETTING_PROJECT, PROJECT_NAME);
            _configuration.NewSetting(WorkingDaysLeftController.SETTING_NON_WORK_DAYS, "Saturday", "Sunday");

            return new List<Configuration> { _configuration };
        }
    }


    [TestFixture]
    public class WorkingDaysLeftRxControllerSpecs : shared
    {
        private static WorkingDaysLeftRxController _controller;
        private static WorkingDaysLeftViewModel _workingDaysLeftViewModel;
        private static WorkingDaysLeftSettingsViewModel _workingDaysLeftSettingsViewModel;
        
        protected static Mock<ITimer> _iTimerMock = new Mock<ITimer>();
        protected static Mock<IPersistDomainModels<Configuration>> configPersisterRepositoryMock = new Mock<IPersistDomainModels<Configuration>>();
        protected static Mock<IAsyncRepository<Configuration>> configRepositoryMock = new Mock<IAsyncRepository<Configuration>>();
        protected static Mock<IAsyncRepository<ProjectInfoServer>> projectRepositoryMock = new Mock<IAsyncRepository<ProjectInfoServer>>();
        protected static Mock<ILog> loggerMock = new Mock<ILog>();
        protected static Mock<IAsyncRepository<Holiday>> holidayRepositoryMock = new Mock<IAsyncRepository<Holiday>>();

        protected static void CreateController()
        {
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>());

            _workingDaysLeftViewModel = new WorkingDaysLeftViewModel();
            _workingDaysLeftSettingsViewModel = new WorkingDaysLeftSettingsViewModel();
            _controller = new WorkingDaysLeftRxController(
                _workingDaysLeftViewModel,
                _workingDaysLeftSettingsViewModel,
                projectRepositoryMock.Object,
                configRepositoryMock.Object,
                holidayRepositoryMock.Object,
                configPersisterRepositoryMock.Object,
                _iTimerMock.Object,
                loggerMock.Object
                );
        }

        [TestFixture]
        public class when_receving_valid_project_configuration
        {
            [SetUp]
            public void Setup()
            {
                CreateController();
            }

            [Test]
            public void Assure_configuration_is_loaded_into_settingsViewModel()
            {

                var configs = CreateConfiguration(false, DateTime.Now.AddDays(10));
                configRepositoryMock.Raise(r => r.GetCompleted += null, new GetCompletedEventArgs<Configuration>(configs, null));

                _workingDaysLeftSettingsViewModel.IsManuallyConfigured.ShouldBe(false);
                _workingDaysLeftSettingsViewModel.SelectedProject.ShouldBe(PROJECT_NAME);
                _workingDaysLeftSettingsViewModel.SelectedServer.ShouldBe(SERVER_NAME);
            }

            [Test]
            public void Assure_projectServerRepository_is_queried()
            {
                var configs = CreateConfiguration(false, DateTime.Now.AddDays(10));
                configRepositoryMock.Raise(r => r.GetCompleted += null, new GetCompletedEventArgs<Configuration>(configs, null));

                projectRepositoryMock.Verify(r => r.BeginGet(new ProjectInfoServerByName(SERVER_NAME)));
            }

        }

        [TestFixture]
        public class when_receiving_valid_manual_configuration
        {
            [SetUp]
            public void Setup()
            {
                CreateController();
            }

            [Test]
            public void Assure_configuration_is_loaded_into_settingsViewModel()
            {
                var endDate = DateTime.Now.Date.AddDays(10);
                var configs = CreateConfiguration(true, endDate);
                configRepositoryMock.Raise(r => r.GetCompleted += null, new GetCompletedEventArgs<Configuration>(configs, null));

                _workingDaysLeftSettingsViewModel.IsManuallyConfigured.ShouldBe(true);
                _workingDaysLeftSettingsViewModel.SelectedEndDate.ShouldBe(endDate);
            }
        }

        [TestFixture]
        public class when_receiving_invalid_configuration
        {
            [SetUp]
            public void Setup()
            {
                CreateController();
            }

            [Test]
            public void Assure_error_is_logged()
            {
                configRepositoryMock.Raise(r => r.GetCompleted += null, new GetCompletedEventArgs<Configuration>(new List<Configuration> {new Configuration("WR0NG!")}, null));
                loggerMock.Verify(l=>l.WriteEntry(It.IsAny<ErrorLogEntry>()));
            }
        }
    }
}
