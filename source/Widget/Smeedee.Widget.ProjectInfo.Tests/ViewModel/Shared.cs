using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Holidays;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Widget.ProjectInfo;
using Smeedee.Widget.ProjectInfo.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Testing.Services;
using TinyMVVM.IoC;

namespace Smeedee.Widget.ProjectInfoTests.Controllers.WorkingDaysLeftViewModelSpecs
{
    public class Shared : ScenarioClass
    {
        protected const string PROJECT_NAME = "TestProjectName";
        protected const string SERVER_NAME = "Mock Server";
        protected static WorkingDaysLeftViewModel viewModel;
        protected static WorkingDaysLeftSettingsViewModel settingViewModel;
        protected static List<Holiday> NoHolidays = new List<Holiday>();
        protected static ObservableCollection<WorkingDay> NoNonWorkingDays =
            new ObservableCollection<WorkingDay> {
                                      new WorkingDay(DayOfWeek.Monday, false),
                                      new WorkingDay(DayOfWeek.Tuesday, false),
                                      new WorkingDay(DayOfWeek.Wednesday, false),
                                      new WorkingDay(DayOfWeek.Thursday, false),
                                      new WorkingDay(DayOfWeek.Friday, false),
                                      new WorkingDay(DayOfWeek.Saturday, false),
                                      new WorkingDay(DayOfWeek.Sunday, false)
                                  };
        protected static ObservableCollection<WorkingDay> StandardNonWorkingDays =
            new ObservableCollection<WorkingDay> {
                                              new WorkingDay(DayOfWeek.Monday, false),
                                              new WorkingDay(DayOfWeek.Tuesday, false),
                                              new WorkingDay(DayOfWeek.Wednesday, false),
                                              new WorkingDay(DayOfWeek.Thursday, false),
                                              new WorkingDay(DayOfWeek.Friday, false),
                                              new WorkingDay(DayOfWeek.Saturday, true),
                                              new WorkingDay(DayOfWeek.Sunday, true)
                                          };
        
        protected static string DisplayText;
        protected static Mock<ITimer> iTimerMock;
        protected static Iteration IterationFromConfig;

        protected static DateTime NowTime = DateTime.Now.Date;
        protected static Mock<IAsyncRepository<ProjectInfoServer>> projectRepositoryMock;
        protected static Iteration TestingIteration;
        protected static Mock<ILog> loggerMock = new Mock<ILog>();

        protected static Mock<IAsyncRepository<Holiday>> holidayRepositoryMock;
        protected static Mock<IProgressbar> loadingNotifierMock;


        protected Context configuration_says_not_to_use_end_date_from_configuration =
            () => SetupConfig(false, DateTime.Now);

        protected Context configuration_says_to_use_end_date_from_configuration = () =>
        {
            DateTime startDate = NowTime.AddDays(-14);
            DateTime endDate = NowTime.AddDays(3).AddMinutes(30);

            IterationFromConfig = new Iteration(startDate, endDate);

            SetupConfig(true, IterationFromConfig.EndDate);
        };

        protected Context configuration_says_to_use_end_date_from_configuration_and_end_date_is_bad = () =>
        {
            var startDate = NowTime.AddDays(-14);
            var endDate = new DateTime(2010, 12, 31);

            IterationFromConfig = new Iteration(startDate, endDate);

            SetupConfigWithCorruptDateFormat(true);
        };

        protected Context viewModels_exists = () =>
        {
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>());


            settingViewModel = new WorkingDaysLeftSettingsViewModel();
            viewModel = new WorkingDaysLeftViewModel();
        };

        protected Context controller_is_created = () => { CreateController(); };

        protected Context large_text_is_displayed =
            () => { DisplayText = viewModel.DaysRemainingTextLarge; };

        protected Context small_text_is_displayed =
            () => { DisplayText = viewModel.DaysRemainingTextSmall; };

        protected When the_controller_is_created = () => { CreateController(); };

        protected Context configuration_has_invalid_value_for_use_config_repo_key =
            () => SetupCorruptConfig("haldis");

        protected Context there_are_no_projects_in_the_repository = () =>
        {
            var server = new ProjectInfoServer(SERVER_NAME, "http://wdl.com");

            projectRepositoryMock = new Mock<IAsyncRepository<ProjectInfoServer>>();
            projectRepositoryMock.Setup(r => r.BeginGet(It.IsAny<Specification<ProjectInfoServer>>()))
                .Raises(r => r.GetCompleted += null,new GetCompletedEventArgs<ProjectInfoServer>(new List<ProjectInfoServer> {server}, null));

            SetupConfig(false, DateTime.Now);
        };
        
        protected Context there_are_no_servers = () =>
        {
            projectRepositoryMock = new Mock<IAsyncRepository<ProjectInfoServer>>();
            projectRepositoryMock.Setup(r => r.BeginGet(It.IsAny<Specification<ProjectInfoServer>>()))
                .Raises(r => r.GetCompleted += null,new GetCompletedEventArgs<ProjectInfoServer>(new List<ProjectInfoServer>(), null));
        };

        protected Context there_is_an_iteration_in_the_given_project = () =>
        {
            var server = new ProjectInfoServer(SERVER_NAME, "http://wdl.com");

            DateTime startDate = NowTime.AddDays(-14);
            DateTime endDate = NowTime.AddDays(2).AddHours(2);

            var currentproject = new Project(PROJECT_NAME);
            var iteration = new Iteration(startDate, endDate);
            currentproject.AddIteration(iteration);

            TestingIteration = currentproject.CurrentIteration;

            server.AddProject(currentproject);

            projectRepositoryMock = new Mock<IAsyncRepository<ProjectInfoServer>>();
            projectRepositoryMock.Setup(r => r.BeginGet(It.IsAny<AllSpecification<ProjectInfoServer>>()))
                .Raises(r => r.GetCompleted += null, new GetCompletedEventArgs<ProjectInfoServer>(new List<ProjectInfoServer> {server}, new AllSpecification<ProjectInfoServer>()));

            projectRepositoryMock.Setup(r => r.BeginGet(new ProjectInfoServerByName(SERVER_NAME)))
                .Raises(r => r.GetCompleted += null, new GetCompletedEventArgs<ProjectInfoServer>(new List<ProjectInfoServer> { server }, new ProjectInfoServerByName(SERVER_NAME)));

            SetupConfig(false, DateTime.Now);
        };

        protected Context there_is_a_configured_server_to_use = () =>
        {
            config.NewSetting("project-server", SERVER_NAME);
        };

        protected Context there_is_a_configured_project = () =>
        {
            config.NewSetting("project", PROJECT_NAME);
        };


        protected Context project_is_not_configured_manually = () =>
        {
            config.NewSetting(WorkingDaysLeftController.SETTING_IS_MANUALLY_CONFIGURED, "False");
        };


        protected Context there_is_an_iteration_in_the_given_project_running_over_deadline = () =>
        {
            var server = new ProjectInfoServer(SERVER_NAME, "http://wdl.com");

            DateTime endDate = NowTime.AddDays(-14);

            var currentproject = new Project(PROJECT_NAME);
            var iteration = new Iteration(endDate.AddDays(-14), endDate);
            currentproject.AddIteration(iteration);

            TestingIteration = currentproject.CurrentIteration;

            server.AddProject(currentproject);

            projectRepositoryMock = new Mock<IAsyncRepository<ProjectInfoServer>>();
            projectRepositoryMock.Setup(r => r.BeginGet(It.IsAny<Specification<ProjectInfoServer>>()))
                .Raises(r => r.GetCompleted += null, new GetCompletedEventArgs<ProjectInfoServer>(new List<ProjectInfoServer> {server}, null));

            SetupConfig(false, DateTime.Now);
        };

        protected Context there_are_holidays_in_the_given_time_period = () =>
        {
            holidayRepositoryMock.Setup(m => m.BeginGet(It.IsAny<Specification<Holiday>>()))
                .Raises( r=> r.GetCompleted+=null, new GetCompletedEventArgs<Holiday>(holidays, null));
        };

        protected Context some_valid_configuration_exists = () =>
        {
            config.NewSetting(WorkingDaysLeftController.SETTING_IS_MANUALLY_CONFIGURED, "True");
            config.NewSetting(WorkingDaysLeftController.SETTING_END_DATE, DateTime.Now.AddDays(3).Date.ToString(CultureInfo.InvariantCulture));
            config.NewSetting(WorkingDaysLeftController.SETTING_SERVER, "some server");
            config.NewSetting(WorkingDaysLeftController.SETTING_PROJECT, "some-project");
            config.NewSetting(WorkingDaysLeftController.SETTING_NON_WORK_DAYS, "");
        };

        protected Context there_is_no_iteration_in_the_given_project = () =>
        {
            var server = new ProjectInfoServer(SERVER_NAME, "http://wdl.com");
            server.AddProject(new Project(PROJECT_NAME));

            projectRepositoryMock = new Mock<IAsyncRepository<ProjectInfoServer>>();

            projectRepositoryMock.Setup(
                r => r.BeginGet(It.IsAny<AllSpecification<ProjectInfoServer>>()))
                .Raises(r => r.GetCompleted += null, new GetCompletedEventArgs<ProjectInfoServer>(new List<ProjectInfoServer> { server }, null));


            SetupConfig(false, DateTime.Now);
        };

        protected Context WorkingDaysLeft_Configuration_does_not_exist = () =>
        {
            config = null;
        };
        
        protected When notifiedToRefresh = () => iTimerMock.Raise(n => n.Elapsed += null, new EventArgs()); 

        private static List<Holiday> holidays = new List<Holiday>
                                                    {
                                                        new Holiday() { Date = DateTime.Now.Date }
                                                    };

        protected static WorkingDaysLeftController _controller;
        
        private static void SetupConfig(bool useEndDateFromConfig, DateTime endDate)
        {
            var nonWorkingDays = NoNonWorkingDays.Where(d => d.IsNotWorkingDay).Select(d => d.Day.ToString()).ToArray();
            
            config = new Configuration("project-info");
            config.IsConfigured = true;
            config.NewSetting(WorkingDaysLeftController.SETTING_IS_MANUALLY_CONFIGURED, useEndDateFromConfig ? "True" : "False");
            config.NewSetting(WorkingDaysLeftController.SETTING_END_DATE, endDate.ToString(CultureInfo.InvariantCulture));
            config.NewSetting(WorkingDaysLeftController.SETTING_NON_WORK_DAYS, nonWorkingDays);
        }

        private static void SetupCorruptConfig(string returnValue)
        {
            config = new Configuration("project-info");
            config.NewSetting("use-config-repo", returnValue);
            config.NewSetting("end-date", DateTime.Now.AddDays(3).ToString(new CultureInfo("en-US")));
        }

        private static void SetupConfigWithCorruptDateFormat(bool useConfigValues)
        {
            config = new Configuration("project-info");
            config.NewSetting("use-config-repo", useConfigValues ? "true" : "false");
            config.NewSetting("end-date", "epic fail");
        }

        protected static Configuration config;
        protected static void CreateController()
        {
            iTimerMock = new Mock<ITimer>();
            if( holidayRepositoryMock == null )
            {
                holidayRepositoryMock = new Mock<IAsyncRepository<Holiday>>();
                holidayRepositoryMock.Setup(r => r.BeginGet(It.IsAny<Specification<Holiday>>())).Raises(
                    r => r.GetCompleted += null, new GetCompletedEventArgs<Holiday>(new List<Holiday>(), null));
            }

            _controller = new WorkingDaysLeftController(viewModel, settingViewModel, projectRepositoryMock.Object,
                                                        holidayRepositoryMock.Object, iTimerMock.Object,
                                                        loggerMock.Object, loadingNotifierMock.Object,
                                                        config);

            //lightweight recreation of the bindings the widget does
            settingViewModel.Save.BeforeExecute += (o,e) => _controller.BeforeSave();
            

        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
        [SetUp]
        public void SetUp()
        {
            config = new Configuration{ IsConfigured = true };
            loadingNotifierMock = new Mock<IProgressbar>();
            viewModel = new WorkingDaysLeftViewModel();
            settingViewModel = new WorkingDaysLeftSettingsViewModel();
            projectRepositoryMock = new Mock<IAsyncRepository<ProjectInfoServer>>();
        }
    }

}