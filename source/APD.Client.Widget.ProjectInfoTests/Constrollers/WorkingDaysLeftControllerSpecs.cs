#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;

using APD.Client.Framework;
using APD.Client.Widget.ProjectInfo.Controllers;
using APD.Client.Widget.ProjectInfo.ViewModels;
using APD.DomainModel.Config;
using APD.DomainModel.Framework;
using APD.DomainModel.Framework.Logging;
using APD.DomainModel.Holidays;
using APD.DomainModel.ProjectInfo;
using Moq;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

// ReSharper disable InconsistentNaming
namespace APD.Client.Widget.ProjectInfoTests.Controllers.WorkingDaysLeftControllerSpecs
{
    public class Shared : ScenarioClass
    {
        protected static List<Holiday> NoHolidays = new List<Holiday>();

        protected static AllSpecification<ProjectInfoServer> AllSpec;


        protected static IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>> BackgroundWorkerInvoker =
            new NoBackgroundWorkerInvocation<IEnumerable<ProjectInfoServer>>();

        protected static Mock<IPersistDomainModels<Configuration>> configPersisterRepositoryMock =
            new Mock<IPersistDomainModels<Configuration>>();

        protected static Mock<IRepository<Configuration>> configRepositoryMock =
            new Mock<IRepository<Configuration>>();

        protected static WorkingDaysLeftController controller;
        protected static string DisplayText;
        protected static Mock<INotifyWhenToRefresh> iNotifyWhenToRefreshMock;
        protected static Iteration IterationFromConfig;

        protected static DateTime NowTime = DateTime.Now;
        protected static Mock<IRepository<ProjectInfoServer>> projectRepositoryMock;
        protected static Iteration TestingIteration;
        protected static Mock<ILog> loggerMock = new Mock<ILog>();
        
        protected static Mock<IRepository<Holiday>> holidayRepositoryMock =
            new Mock<IRepository<Holiday>>();


        protected Context configuration_says_not_to_use_end_date_from_configuration =
            () => SetupConfigRepositoryMock(false, DateTime.Now);

        protected Context configuration_says_to_use_end_date_from_configuration = () =>
        {
            DateTime startDate = NowTime.AddDays(-14);
            DateTime endDate = NowTime.AddDays(3).AddMinutes(30);

            IterationFromConfig = new Iteration(startDate, endDate);

            SetupConfigRepositoryMock(true, IterationFromConfig.EndDate);
        };

        protected Context configuration_says_to_use_end_date_from_configuration_and_end_date_is_bad = () =>
        {
            var startDate = NowTime.AddDays(-14);
            var endDate = new DateTime(2010, 12, 31);

            IterationFromConfig = new Iteration(startDate, endDate);

            SetupConfigRepositoryMockWithCorruptDateFormat(true, IterationFromConfig.EndDate);
        };

        protected Context controller_is_created = () => { CreateController(); };

        protected Context large_text_is_displayed =
            () => { DisplayText = controller.ViewModel.DaysRemainingTextLarge; };

        protected Context small_text_is_displayed =
            () => { DisplayText = controller.ViewModel.DaysRemainingTextSmall; };

        protected When the_controller_is_created = () => { CreateController(); };

        protected Context configuration_has_invalid_value_for_use_config_repo_key =
            () => SetupWrongConfigRepositoryMock("haldis");

        protected Context there_are_no_projects_in_the_repository = () =>
        {
            var server = new ProjectInfoServer("Mock Server", "http://wdl.com");

            projectRepositoryMock = new Mock<IRepository<ProjectInfoServer>>();
            projectRepositoryMock.Setup(
                r => r.Get(It.IsAny<AllSpecification<ProjectInfoServer>>()))
                .Returns(new List<ProjectInfoServer> {server});

            SetupConfigRepositoryMock(false, DateTime.Now);
        };

        protected Context there_are_no_servers = () =>
        {
            projectRepositoryMock = new Mock<IRepository<ProjectInfoServer>>();
            projectRepositoryMock.Setup(
                r => r.Get(It.IsAny<AllSpecification<ProjectInfoServer>>()))
                .Returns(new List<ProjectInfoServer>());
        };

        protected Context there_is_an_iteration_in_the_given_project = () =>
        {
            var server = new ProjectInfoServer("Mock Server", "http://wdl.com");

            DateTime startDate = NowTime.AddDays(-14);
            DateTime endDate = NowTime.AddDays(2).AddHours(2);

            var currentproject = new Project("TestProjectName");
            var iteration = new Iteration(startDate, endDate);
            currentproject.AddIteration(iteration);

            TestingIteration = currentproject.CurrentIteration;

            server.AddProject(currentproject);

            projectRepositoryMock = new Mock<IRepository<ProjectInfoServer>>();
            projectRepositoryMock.Setup(
                r => r.Get(It.IsAny<AllSpecification<ProjectInfoServer>>()))
                .Returns(new List<ProjectInfoServer> {server});


            SetupConfigRepositoryMock(false, DateTime.Now);
        };

        protected Context there_is_an_iteration_in_the_given_project_running_over_deadline = () =>
        {
            var server = new ProjectInfoServer("Mock Server", "http://wdl.com");

            DateTime startDate = NowTime.AddDays(-28);
            DateTime endDate = NowTime.AddDays(-14);

            var currentproject = new Project("TestProjectName");
            var iteration = new Iteration(startDate, endDate);
            currentproject.AddIteration(iteration);

            TestingIteration = currentproject.CurrentIteration;

            server.AddProject(currentproject);

            projectRepositoryMock = new Mock<IRepository<ProjectInfoServer>>();
            projectRepositoryMock.Setup(
                r => r.Get(It.IsAny<AllSpecification<ProjectInfoServer>>())).Returns(
                new List<ProjectInfoServer> {server});


            SetupConfigRepositoryMock(false, DateTime.Now);
        };

        protected Context there_are_holidays_in_the_given_time_period = () =>
        {
            holidayRepositoryMock.Setup(m => m.Get(It.IsAny<Specification<Holiday>>()))
                .Returns(holidays);
        };

        protected Context there_is_no_iteration_in_the_given_project = () =>
        {
            var server = new ProjectInfoServer("Mock Server", "http://wdl.com");
            server.AddProject(new Project("TestProjectName"));

            projectRepositoryMock = new Mock<IRepository<ProjectInfoServer>>();
            projectRepositoryMock.Setup(
                r => r.Get(It.IsAny<AllSpecification<ProjectInfoServer>>()))
                .Returns(new List<ProjectInfoServer> {server});


            SetupConfigRepositoryMock(false, DateTime.Now);
        };

        protected Context WorkingDaysLeft_Configuration_does_not_exist = () =>
        {
            configRepositoryMock = new Mock<IRepository<Configuration>>();
            configPersisterRepositoryMock = new Mock<IPersistDomainModels<Configuration>>();

            var configlist = new List<Configuration>();
            configRepositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configlist);
        };

        private static List<Holiday> holidays = new List<Holiday>
        {
            new Holiday() { Date = DateTime.Now.Date }
        };

        private static void SetupConfigRepositoryMock(bool returnValue, DateTime endDate)
        {
            configRepositoryMock = new Mock<IRepository<Configuration>>();
            var configuration = new Configuration("project-info");
            configuration.NewSetting("use-config-repo", returnValue ? "true" : "false");
            configuration.NewSetting("end-date", endDate.ToString(new CultureInfo("en-US")));
            var configList = new List<Configuration> {configuration};

            configRepositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configList);
        }

        private static void SetupWrongConfigRepositoryMock(string returnValue)
        {
            configRepositoryMock = new Mock<IRepository<Configuration>>();
            var configuration = new Configuration("project-info");
            configuration.NewSetting("use-config-repo", returnValue);
            configuration.NewSetting("end-date", DateTime.Now.AddDays(3).ToString(new CultureInfo("en-US")));
            var configList = new List<Configuration> { configuration };

            configRepositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configList);
        }

        private static void SetupConfigRepositoryMockWithCorruptDateFormat(bool useConfigValues, DateTime endDate)
        {
            configRepositoryMock = new Mock<IRepository<Configuration>>();
            var configuration = new Configuration("project-info");
            configuration.NewSetting("use-config-repo", useConfigValues ? "true" : "false");
            configuration.NewSetting("end-date", "epic fail");
            var configList = new List<Configuration> { configuration };

            configRepositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configList);
        }


        protected static void CreateController()
        {
            iNotifyWhenToRefreshMock = new Mock<INotifyWhenToRefresh>();
            controller = new WorkingDaysLeftController(
                projectRepositoryMock.Object,
                configRepositoryMock.Object,
                holidayRepositoryMock.Object,
                configPersisterRepositoryMock.Object,
                iNotifyWhenToRefreshMock.Object,
                new NoUIInvocation(), BackgroundWorkerInvoker,
                loggerMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }


    [TestFixture]
    public class When_the_controller_is_spawned : Shared
    {
        [SetUp]
        public void SetUp()
        {
            Scenario("Controller is spawned");
        }

        [Test]
        public void Assure_Configuration_is_created_if_it_does_not_exist()
        {
            Given(there_are_no_projects_in_the_repository).
                And(WorkingDaysLeft_Configuration_does_not_exist);

            When(the_controller_is_created);
            
            Then("assure Configuration is created if it doesn't exist", () => 
                configPersisterRepositoryMock.Verify(r => r.Save(It.IsAny<Configuration>()), Times.Once()));
        }

        [Test]
        public void Configuration_should_be_checked_to_see_if_project_info_or_configuration_settings_should_be_used()
        {
            Given(configuration_says_to_use_end_date_from_configuration)
                .And(there_is_an_iteration_in_the_given_project);

            When(the_controller_is_created);

            Then("the configuration service should be asked for the project-info configuration", () => 
                configRepositoryMock.Verify(r => r.Get(It.IsAny<Specification<Configuration>>()), Times.Once()));
        }

        [Test]
        public void Should_get_end_date_from_config_if_project_info_setting_is_set()
        {
            Given(there_is_an_iteration_in_the_given_project)
                .And(configuration_says_to_use_end_date_from_configuration);

            When(the_controller_is_created);

            Then("the project infor repository should not be queried", () => 
                projectRepositoryMock.Verify(r => r.Get(It.IsAny<Specification<ProjectInfoServer>>()),
                                      Times.Never()));
        }

        [Test]
        public void Should_log_if_the_end_date_parsing_fails()
        {
            Given(there_is_an_iteration_in_the_given_project)
                .And(configuration_says_to_use_end_date_from_configuration_and_end_date_is_bad);

            When(the_controller_is_created);

            Then("the error should be logged", () =>
                loggerMock.Verify(l=>l.WriteEntry(It.IsAny<ErrorLogEntry>())));
        }

        [Test]
        public void should_notify_user_if_project_does_not_exist_in_repository()
        {
            // TODO: Review the scenario. Might not describe what is actually tested.
            Given(there_are_no_projects_in_the_repository).
                And(controller_is_created).
                And(large_text_is_displayed);

            When("requesting days remaining");

            Then("the user is notified that there are no projects in the repository", () =>
            {
                controller.CurrentProject.ShouldBeNull();
                DisplayText.ShouldBe(WorkingDaysLeftViewModel.NO_PROJECTS_STRING_LARGE);
            });
        }


        [Test]
        public void should_notify_user_if_project_has_no_iterations()
        {
            Given(there_is_no_iteration_in_the_given_project)
                .And(controller_is_created)
                .And(large_text_is_displayed);

            When("requesting days remaining");

            Then("the user is notified that there are no iterations in the current project", () =>
            {
                controller.CurrentProject.ShouldNotBeNull();
                DisplayText.ShouldBe(
                WorkingDaysLeftViewModel.NO_ITERATIONS_STRING_LARGE);
            });
        }

        [Test]
        public void should_display_warning_about_config_if_use_config_repo_setting_value_is_wrong()
        {
            Given(there_are_no_servers)
               .And(configuration_has_invalid_value_for_use_config_repo_key)
               .And(controller_is_created)
               .And(large_text_is_displayed);

            When("loading data");

            Then("user is notified there is a problem with the configuration", () =>
                 DisplayText.ShouldBe(WorkingDaysLeftViewModel.ERROR_IN_CONFIG));
        }

        [Test]
        public void should_update_days_on_overtime_in_viewmodel()
        {
            Given(there_is_an_iteration_in_the_given_project_running_over_deadline)
                .And(controller_is_created)
                .And(large_text_is_displayed);

            When("requesting days remaining");

            Then("the number of days on overtime in regards to the current iteration is displayed", () =>
                controller.ViewModel.DaysRemaining.ShouldBe(
                    TestingIteration.CalculateWorkingdaysLeft(NowTime, NoHolidays).Days))

                .And("the text should indicate that the iteration is on overtime", () =>
                    DisplayText.ShouldBe(WorkingDaysLeftViewModel.DAYS_ON_OVERTIME_STRING));
        }

        [Test]
        public void Should_use_end_date_from_config_if_project_info_setting_is_set()
        {
            Given(there_is_an_iteration_in_the_given_project)
                   .And(configuration_says_to_use_end_date_from_configuration);

            When(the_controller_is_created);

            Then("the configuration setting project_end_date should be read and returned", () =>
                controller.ViewModel.DaysRemaining.ShouldBe(
                   IterationFromConfig.CalculateWorkingdaysLeft(DateTime.Today, NoHolidays).Days));
        }

        [Test]
        public void Should_use_end_date_from_project_if_project_info_setting_is_not_set()
        {
            Given(configuration_says_not_to_use_end_date_from_configuration)
                   .And(there_is_an_iteration_in_the_given_project);

            When(the_controller_is_created);

            Then("the project info repository should be queried for the end date", () =>
                
                controller.ViewModel.DaysRemaining.ShouldBe(
                   TestingIteration.CalculateWorkingdaysLeft(DateTime.Today, NoHolidays).Days));
        }
    }

    [TestFixture]
    public class When_notified_to_refresh : Shared
    {
        [Test]
        public void should_query_repository_for_iteration_in_given_project()
        {
            Given(there_is_an_iteration_in_the_given_project).
                And(controller_is_created);

            When("notified to refresh", () =>
                iNotifyWhenToRefreshMock.Raise(n => n.Refresh += null, new RefreshEventArgs()));

            Then("assure it queried project repository for the given project", () =>
            {
                AllSpec = new AllSpecification<ProjectInfoServer>();
                projectRepositoryMock.Verify(
                    r => r.Get(It.IsAny<AllSpecification<ProjectInfoServer>>()),
                    Times.Exactly(2));
            });
        }
    }
}
// ReSharper restore InconsistentNaming