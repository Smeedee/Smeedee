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
using System.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Widget.BurndownChart.Controllers;
using Smeedee.Widget.BurndownChart.ViewModel;
using Smeedee.Widget.BurndownChart.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.Client.Widget.BurndownChart.BurndownChartControllerSpecs
{

    [TestFixture]
    public class When_the_controller_is_spawned : Shared
    {
        [Test]
        public void should_notify_user_if_no_projectInfoServer_exists()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_no_available_servers);
                scenario.When(the_controller_is_created);

                scenario.Then(() =>
                {
                    Controller.ViewModel.ErrorMessage.ShouldBe("There are no available servers");
                });
            });
        }

        [Test]
        public void should_notify_user_if_project_does_not_exist_in_repository()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_no_projects_in_the_repository);

                scenario.When(the_controller_is_created);

                scenario.Then("the user is notified that there are no projects in the repository", () =>
                {
                    Controller.CurrentProject.ShouldBeNull();
                    Controller.ViewModel.ErrorMessage.ShouldBe("No projects with the given name exists in the repository");
                });
            });
        }

        [Test]
        public void should_notify_user_if_project_has_no_iterations()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_no_current_iteration_in_the_given_project);

                scenario.When(the_controller_is_created);

                scenario.Then("the user is notified that there are no iterations in the current project", () =>
                {
                    Controller.CurrentProject.ShouldNotBeNull();
                    Controller.ViewModel.ErrorMessage.ShouldBe("There are no iterations in the given project");
                });
            });
        }
        
        [Test]
        public void should_notify_user_if_project_has_iteration_but_no_tasks()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_but_no_tasks);
                
                scenario.When(the_controller_is_created);

                scenario.Then("the user is notified that there is an iteration in the current project, but no tasks", () =>
                {
                    Controller.CurrentProject.ShouldNotBeNull();
                    Controller.ViewModel.ErrorMessage.ShouldBe("There are no tasks for the current iteration for the given project");
                });
            });
        }

        [Test]
        public void should_set_correct_Project_and_Iteration_name_in_burndown_chart_viewmodel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks);

                scenario.When(the_controller_is_created);

                scenario.Then("the correct project and iteratin name should be displayed", () =>
                {
                    Controller.ViewModel.ProjectName.ShouldBe("TestProjectName");
                    Controller.ViewModel.IterationName.ShouldBe("Test Iteration 1");
                });
            });
        }

        [Test]
        public void should_set_correct_ActualRegression_coordinates_in_burndown_chart_viewmodel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks).
                    And(settings_are_set_to_not_include_dates_before_Iteration);

                scenario.When(the_controller_is_created);

                scenario.Then("the correct burndown coordinates are displayed", () =>
                {
                    Controller.ViewModel.ActualBurndown.Count.ShouldBe(9);
                    Controller.ViewModel.ActualBurndown[0].RemainingWorkEffort.ShouldBe(70);
                    Controller.ViewModel.ActualBurndown[1].RemainingWorkEffort.ShouldBe(65);
                    Controller.ViewModel.ActualBurndown[2].RemainingWorkEffort.ShouldBe(53);
                    Controller.ViewModel.ActualBurndown[3].RemainingWorkEffort.ShouldBe(32);
                    Controller.ViewModel.ActualBurndown[4].RemainingWorkEffort.ShouldBe(30);
                    Controller.ViewModel.ActualBurndown[5].RemainingWorkEffort.ShouldBe(15);
                    Controller.ViewModel.ActualBurndown[6].RemainingWorkEffort.ShouldBe(4);
                    Controller.ViewModel.ActualBurndown[7].RemainingWorkEffort.ShouldBe(0);
                    Controller.ViewModel.ActualBurndown[8].RemainingWorkEffort.ShouldBe(0);
                });
            });
        }

        [Test]
        public void should_set_correct_last_burndown_coordinate_with_timestamp_now_in_burndown_chart_viewmodel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks);

                scenario.When(the_controller_is_created);

                scenario.Then("two last coordinates should be different, and the last one should be now", () =>
                {
                    DateTime lastTimeStamp = Controller.ViewModel.ActualBurndown[8].TimeStampForUpdate;
                    TimeSpan timeSpanFromLastTimeStampToNow = DateTime.Now.Subtract(lastTimeStamp);

                    Controller.ViewModel.ActualBurndown[7].TimeStampForUpdate.Date.ShouldNotBe(DateTime.Now.Date);
                    lastTimeStamp.Date.ShouldBe(DateTime.Now.Date);
                    timeSpanFromLastTimeStampToNow.Seconds.ShouldBeLessThan(20);

                });
            });
        }

        [Test]
        public void should_set_correct_LinearRegression_coordinates_in_burndown_chart_viewmodel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks);

                scenario.When(the_controller_is_created);

                scenario.Then("the correct burndown coordinates are displayed", () =>
                {
                    Controller.ViewModel.IdealBurndown.Count.ShouldBe(2);
                    Controller.ViewModel.IdealBurndown[0].RemainingWorkEffort.ShouldBe(70);
                    Controller.ViewModel.IdealBurndown[0].TimeStampForUpdate.Date.ShouldBe(StartDate.Date);
                    Controller.ViewModel.IdealBurndown[1].RemainingWorkEffort.ShouldBe(0);
                    Controller.ViewModel.IdealBurndown[1].TimeStampForUpdate.Date.ShouldBe(StartDate.AddDays(15).Date);
                });
            });
        }

        [Test]
        public void should_set_correct_upper_warning_limit_coordinates_in_burndown_chart_viewmodel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks);

                scenario.When(the_controller_is_created);

                scenario.Then("the correct upper warning limit coordinates are displayed", () =>
                {
                    Controller.ViewModel.UpperWarningLimit.Count.ShouldBe(2);
                    Controller.ViewModel.UpperWarningLimit[0].RemainingWorkEffort.ShouldBe(
                            70 * Controller.ViewModel.BurndownUpperWarningLimitInPercent);
                    Controller.ViewModel.UpperWarningLimit[0].TimeStampForUpdate.Date.ShouldBe(StartDate.Date);
                    Controller.ViewModel.UpperWarningLimit[1].RemainingWorkEffort.ShouldBe(0);
                    Controller.ViewModel.UpperWarningLimit[1].TimeStampForUpdate.Date.ShouldBe(StartDate.AddDays(15).Date);
                });
            });
        }

        [Test]
        public void should_set_correct_lower_warning_limit_corrdinates_in_burndown_chart_viewmodel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks);

                scenario.When(the_controller_is_created);

                scenario.Then("the correct lower warning limit coordinates are displayed", () =>
                {
                    Controller.ViewModel.LowerWarningLimit.Count.ShouldBe(2);
                    Controller.ViewModel.LowerWarningLimit[0].RemainingWorkEffort.ShouldBe(
                            70 * Controller.ViewModel.BurndownLowerWarningLimitInPercent);
                    Controller.ViewModel.LowerWarningLimit[0].TimeStampForUpdate.Date.ShouldBe(StartDate.Date);
                    Controller.ViewModel.LowerWarningLimit[1].RemainingWorkEffort.ShouldBe(0);
                    Controller.ViewModel.LowerWarningLimit[1].TimeStampForUpdate.Date.ShouldBe(StartDate.AddDays(15).Date);
                });
            });
        }

        [Test]
        public void Assure_settings_are_loaded_from_repository()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_3_projects_in_repository).
                    And(settings_are_set_to_include_dates_before_Iteration_and_selected_projectName_is_set_in_repository);

                scenario.When(the_controller_is_created);

                scenario.Then( () => 
                {
//                    ConfigRepositoryMock.Verify(r => r.Get(It.IsAny<Specification<Configuration>>()), Times.AtLeastOnce());
                    Controller.settingsViewModel.IncludeWorkItemsBeforeIterationStartdate.ShouldBe(true);
                    Controller.settingsViewModel.SelectedProjectName.ShouldBe("TestProjectName3");
                });
            });
        }

        [Test]
        public void Assure_default_settings_are_loaded_when_there_is_no_settings_in_repsoitory()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_no_projects_in_the_repository).
                    And(there_is_no_settings_in_repository);

                scenario.When(the_controller_is_created);

                scenario.Then( () => 
                {
                    Controller.settingsViewModel.IncludeWorkItemsBeforeIterationStartdate.ShouldBe(false);
                    //Controller.settingsViewModel.SelectedProjectName.ShouldBe("Default Project");
                    Controller.settingsViewModel.SelectedProjectName.ShouldBeNull();
                });
            });
        }

        [Test]
        public void Assure_the_first_available_project_is_selected_when_no_settings_exists()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks).
                    And(there_is_no_settings_in_repository);

                scenario.When(the_controller_is_created);

                scenario.Then(() => Controller.settingsViewModel.SelectedProjectName.ShouldBe("TestProjectName"));
            });
        }

        [Test]
        public void Assure_repository_settings_are_reloaded_when_settings_are_changed_and_reload_settings_is_clicked()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks).
                    And(settings_are_set_to_include_dates_before_Iteration_and_selected_projectName_is_set_in_repository).
                    And(controller_is_created).
                    And(settings_are_changed_to_not_include_dates_before_Iteration);

                scenario.When("reload settings is clicked",
                              () => Controller.settingsViewModel.ReloadSettings.Execute(null));

                scenario.Then(() => Controller.settingsViewModel.IncludeWorkItemsBeforeIterationStartdate.ShouldBeTrue());
            });
        }

        [Test]
        public void Assure_there_are_3_projects_in_availableProjects_when_3_projects_exists_in_repository()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_3_projects_in_repository);

                scenario.When(the_controller_is_created);

                scenario.Then(() => Controller.settingsViewModel.AvailableProjects.Count.ShouldBe(3));
            });
        }
    }

    [TestFixture]
    public class When_workEffortItems_with_date_before_iterations_startdate_exists_and_should_be_included : Shared
    {
        [Test]
        public void Assure_workEffortItems_with_date_before_Iteration_start_are_included()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks_that_starts_before_Iteration).
                    And(settings_are_set_to_include_dates_before_Iteration_and_selected_projectName_is_set_in_repository).
                    And(controller_is_created);

                scenario.When("data is loaded");

                scenario.Then(() =>
                {
                    var listOfActualDates = Controller.ViewModel.ActualBurndown.
                        Select(burndownChartCoordinate =>
                            burndownChartCoordinate.TimeStampForUpdate).ToList();
                    listOfActualDates.Any(w => w.Date < Controller.CurrentProject.CurrentIteration.StartDate.Date);
                });
            });
        }

        [Test]
        public void Assure_ideals_date_in_first_coordinate_is_equal_to_actuals_date_in_first_coordinate()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks_that_starts_before_Iteration).
                    And(settings_are_set_to_include_dates_before_Iteration_and_selected_projectName_is_not_set_in_repository).
                    And(controller_is_created);

                scenario.When("data is loaded");

                scenario.Then(() => Controller.ViewModel.IdealBurndown[0].TimeStampForUpdate.Date.ShouldBe(                        
                    Controller.ViewModel.ActualBurndown[0].TimeStampForUpdate.Date));
            });
        }
    }
    
    [TestFixture]
    public class When_workEffortItems_with_date_before_iterations_startdate_exists_and_should_not_be_included : Shared
    {
        [Test]
        public void Assure_ideal_and_actual_regressions_first_coordinate_is_equal_to_startdate_in_iteration()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks_that_starts_before_Iteration).
                    And(settings_are_set_to_not_include_dates_before_Iteration).
                    And(controller_is_created);

                scenario.When("data is loaded");

                scenario.Then(() =>
                {
                    Controller.ViewModel.IdealBurndown[0].TimeStampForUpdate.Date.ShouldBe(
                        Controller.CurrentProject.CurrentIteration.StartDate.Date);
                    Controller.ViewModel.ActualBurndown[0].TimeStampForUpdate.Date.ShouldBe(
                        Controller.CurrentProject.CurrentIteration.StartDate.Date);
                });
            });
        }

        [Test]
        public void Assure_workEffortItems_with_date_before_Iteration_start_are_not_included()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks_that_starts_before_Iteration).
                    And(settings_are_set_to_not_include_dates_before_Iteration).
                    And(controller_is_created);

                scenario.When("data is loaded");

                scenario.Then(() => Controller.ViewModel.ActualBurndown.
                    Select(c => c.TimeStampForUpdate).
                    Any(w => w.Date < Controller.CurrentProject.CurrentIteration.StartDate.Date).
                    ShouldBeFalse());
            });
        }

        [Test]
        public void Assure_remaining_workEffort_is_correct_when_dates_before_Iteration_start_are_not_included()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks_that_starts_before_Iteration).
                    And(settings_are_set_to_not_include_dates_before_Iteration).
                    And(controller_is_created);

                scenario.When("data is loaded");

                scenario.Then(() =>
                {
                    Controller.ViewModel.ActualBurndown[0].RemainingWorkEffort.ShouldBe(75);
                    Controller.ViewModel.IdealBurndown[0].RemainingWorkEffort.ShouldBe(75);
                });
            });
        }
    }

    [TestFixture]
    public class When_notified_to_refresh : Shared
    {
        [Test]
        public void should_query_repository_for_iteration_in_given_project()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks).
                        And(settings_are_set_to_not_include_dates_before_Iteration).
                        And(controller_is_created);

                scenario.When("notified to refresh", () =>
                    INotifyWhenToRefreshMock.Raise(n => n.Elapsed += null, new EventArgs()));

                scenario.Then("assure it queried project repository for the given project", () =>
                    RepositoryMock.Verify(
                        r => r.Get(It.IsAny<Specification<ProjectInfoServer>>()), Times.AtLeastOnce()));
            });
        }
    }

    [TestFixture]
    public class When_loading_and_saving_data_and_settings : Shared
    {

        [Test]
        public void Assure_progressbar_is_shown_while_loading()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks).And(controller_is_created);

                scenario.When(loading_data_and_notified_to_refresh);

                scenario.Then("the loading notifier should be shown", () =>
                              LoadingNotifierMock.Verify(l => l.ShowInView(It.IsAny<string>()), Times.AtLeastOnce()));
            });
        }

        [Test]
        public void Assure_progressbar_is_hidden_after_loading()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks).
                    And(controller_is_created);

                scenario.When("data has been loaded");

                scenario.Then(() =>
                {
                    LoadingNotifierMock.Verify(l => l.HideInView(), Times.AtLeastOnce());
                    Controller.ViewModel.IsLoading.ShouldBeFalse();
                });
            });
        }

        [Ignore]
        [Test]
        public void Assure_progressbar_is_shown_while_saving_settings()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(controller_is_created);

                scenario.When(save_command_is_executed);

                scenario.Then(() =>
                {
                    LoadingNotifierMock.Verify(r => r.ShowInSettingsView(It.IsAny<string>()), Times.AtLeastOnce());
                    Controller.ViewModel.IsSaving.ShouldBe(true);
                });
            });
        }

        [Test]
        public void Assure_progressbar_is_hidden_after_saving_settings()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(controller_is_created).
                    And(save_command_has_been_executed);

                scenario.When("allways");

                scenario.Then(() =>
                {
//                    LoadingNotifierMock.Verify(l => l.HideInSettingsView(), Times.AtLeastOnce());
                    Controller.settingsViewModel.IsSaving.ShouldBe(false);
                });
            });
        }

        [Test]
        public void Assure_progressbar_is_shown_while_loading_settings()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(controller_is_created);
                scenario.When(reloadSettings_delegate_is_executed);
                scenario.Then(() =>
                {
                    LoadingNotifierMock.Verify(l => l.ShowInBothViews(It.IsAny<string>()), Times.AtLeastOnce());
                    //Controller.ViewModel.IsLoadingConfig.ShouldBe(true); should be true when async repositorys is implemented
                });
            });
        }

        [Test]
        public void Assure_progressbar_is_hidden_after_loading_settings()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(controller_is_created).
                    And(settings_are_set_to_include_dates_before_Iteration_and_selected_projectName_is_set_in_repository);

                scenario.When(reloadSettings_delegate_is_executed);
                scenario.Then(() =>
                {
                    LoadingNotifierMock.Verify(l => l.HideInBothViews(), Times.AtLeastOnce());
                    Controller.ViewModel.IsLoadingConfig.ShouldBe(false);
                });
            });
        }
    }

    [TestFixture]
    public class When_settings_are_changed : Shared
    {
        [Test]
        public void Assure_settings_are_saved_when_save_is_clicked()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks).
                    And(settings_are_set_to_include_dates_before_Iteration_and_selected_projectName_is_set_in_repository).
                    And(controller_is_created).
                    And(selected_project_name_is_changed_to_newName);

                scenario.When(save_command_is_executed);

//                scenario.Then(() => ConfigPersisterMock.Verify(r => r.Save(It.IsAny<Configuration>()), Times.AtLeastOnce()));
            });
        }
    }

    [Ignore]
    [TestFixture]
    public class When_the_view_model_is_allready_updated_by_the_controller : Shared
    {
        [Test]
        public void should_not_update_the_view_model_if_there_are_no_changes()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks);

                scenario.When(the_controller_is_created);

                scenario.Then("the view model should not be updated", () =>
                {
                    //NOTE: need to set some methods and variables public in the controller
                    //controller.LoadData();
                });
            });
        }
    }



    public class Shared
    {
        protected static BurndownChartController Controller;
        protected static DateTime StartDate;

        protected static Mock<IRepository<ProjectInfoServer>> RepositoryMock = new Mock<IRepository<ProjectInfoServer>>();
//        protected static Mock<IRepository<Configuration>> ConfigRepositoryMock;
//        protected static Mock<IPersistDomainModelsAsync<Configuration>> ConfigPersisterMock;
        protected static Configuration configuration;
        protected static Mock<ITimer> INotifyWhenToRefreshMock;
        protected static Mock<IProgressbar> LoadingNotifierMock;
        protected static IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>> BackgroundWorkerInvoker =
                            new NoBackgroundWorkerInvocation<IEnumerable<ProjectInfoServer>>();
        
        private const string SETTINGS_ENTRY_NAME = "BurndownChartController";
        private const string INCLUDE_WORK_ITEMS_BEFORE_ITERATION_START_DATE = "IncludeWorkItemsBeforeIterationStartdate";
        private const string SELECTED_PROJECT_NAME = "SelectedProjectName";

        [SetUp]
        public void SetUp()
        {
//            ConfigRepositoryMock = new Mock<IRepository<Configuration>>();
//            ConfigPersisterMock = new Mock<IPersistDomainModelsAsync<Configuration>>();
            configuration = new Configuration();
        }

        protected Context there_are_no_available_servers = () =>
        {
            var server = new List<ProjectInfoServer>();

            RepositoryMock = new Mock<IRepository<ProjectInfoServer>>();
            RepositoryMock.Setup(r => r.Get(It.IsAny<Specification<ProjectInfoServer>>())).Returns(server);
        };

        protected Context there_are_no_projects_in_the_repository = () =>
        {
            var server = new List<ProjectInfoServer> {new ProjectInfoServer()};

            RepositoryMock = new Mock<IRepository<ProjectInfoServer>>();
            RepositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<ProjectInfoServer>>())).Returns(server);
        };

        protected Context there_is_no_current_iteration_in_the_given_project = () =>
        {
            var server = new ProjectInfoServer("Mock Server", "http://mock.com");

            var project = new Project("TestProjectName");
            project.SystemId = "ProjectNr1";
            server.AddProject(project);

            RepositoryMock = new Mock<IRepository<ProjectInfoServer>>();
            RepositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<ProjectInfoServer>>()))
                        .Returns(new List<ProjectInfoServer> { server });
        };

        protected Context there_is_a_current_iteration_in_the_given_project_but_no_tasks = () =>
        {
            var currentIteration = new Iteration {SystemId = "iteration system ID"};

            var currentproject = new Project("TestProjectName") {SystemId = "project system ID"};
            currentproject.AddIteration(currentIteration);

            var server = new ProjectInfoServer("server name", "url");
            server.AddProject(currentproject);

            RepositoryMock = new Mock<IRepository<ProjectInfoServer>>();
            RepositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<ProjectInfoServer>>()))
                        .Returns(new List<ProjectInfoServer> { server });
        };

        protected Context there_is_a_current_iteration_in_the_given_project_with_tasks = () =>
        {
            var server = new ProjectInfoServer("server name", "url");

            var currentIteration = new Iteration();
            currentIteration.SystemId = "iteration system ID";

            var tenDays = new TimeSpan(10, 0, 0, 0);
            StartDate = DateTime.Now.Subtract(tenDays);
            currentIteration.StartDate = StartDate;
            currentIteration.EndDate = StartDate.AddDays(14);
            currentIteration.Name = "Test Iteration 1";

            var task1 = new Task();
            var task2 = new Task();
            task1.WorkEffortEstimate = 50;
            task2.WorkEffortEstimate = 20;
            task1.SystemId = "task1 system ID";
            task2.SystemId = "task2 system ID";

            var item11 = new WorkEffortHistoryItem(45, StartDate);
            var item12 = new WorkEffortHistoryItem(35, StartDate.AddDays(1));
            var item13 = new WorkEffortHistoryItem(20, StartDate.AddDays(3));
            var item14 = new WorkEffortHistoryItem(5, StartDate.AddDays(6));
            var item15 = new WorkEffortHistoryItem(0, StartDate.AddDays(7));
            task1.AddWorkEffortHistoryItem(item11);
            task1.AddWorkEffortHistoryItem(item12);
            task1.AddWorkEffortHistoryItem(item13);
            task1.AddWorkEffortHistoryItem(item14);
            task1.AddWorkEffortHistoryItem(item15);

            var item21 = new WorkEffortHistoryItem(18, StartDate.AddDays(1));
            var item22 = new WorkEffortHistoryItem(12, StartDate.AddDays(3));
            var item23 = new WorkEffortHistoryItem(10, StartDate.AddDays(5));
            var item24 = new WorkEffortHistoryItem(4, StartDate.AddDays(7));
            var item25 = new WorkEffortHistoryItem(0, StartDate.AddDays(8));
            task2.AddWorkEffortHistoryItem(item21);
            task2.AddWorkEffortHistoryItem(item22);
            task2.AddWorkEffortHistoryItem(item23);
            task2.AddWorkEffortHistoryItem(item24);
            task2.AddWorkEffortHistoryItem(item25);

            currentIteration.AddTask(task1);
            currentIteration.AddTask(task2);

            var currentproject = new Project("TestProjectName");
            currentproject.SystemId = "project system ID";
            currentproject.AddIteration(currentIteration);
            server.AddProject(currentproject);

            RepositoryMock = new Mock<IRepository<ProjectInfoServer>>();
            RepositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<ProjectInfoServer>>()))
                        .Returns(new List<ProjectInfoServer> { server });
        };

        protected Context there_is_a_current_iteration_in_the_given_project_with_tasks_that_starts_before_Iteration = () =>
        {
            var server = new ProjectInfoServer("server name", "url");

            var currentIteration = new Iteration();
            currentIteration.SystemId = "iteration system ID";
            
            var tenDays = new TimeSpan(10, 0, 0, 0);
            StartDate = DateTime.Now.Subtract(tenDays);
            currentIteration.StartDate = StartDate;
            currentIteration.EndDate = StartDate.AddDays(14);
            currentIteration.Name = "Test Iteration 1";

            var task1 = new Task();
            var task2 = new Task();
            task1.WorkEffortEstimate = 60;
            task2.WorkEffortEstimate = 20;
            task1.SystemId = "task1 system ID";
            task2.SystemId = "task2 system ID";

            var fourteenDays = new TimeSpan(14, 0, 0, 0);

            var item11 = new WorkEffortHistoryItem(45, StartDate);
            var item12 = new WorkEffortHistoryItem(55, StartDate.Subtract(fourteenDays));
            var item13 = new WorkEffortHistoryItem(20, StartDate.AddDays(3));
            var item14 = new WorkEffortHistoryItem(5, StartDate.AddDays(6));
            var item15 = new WorkEffortHistoryItem(0, StartDate.AddDays(7));
            task1.AddWorkEffortHistoryItem(item11);
            task1.AddWorkEffortHistoryItem(item12);
            task1.AddWorkEffortHistoryItem(item13);
            task1.AddWorkEffortHistoryItem(item14);
            task1.AddWorkEffortHistoryItem(item15);

            var item21 = new WorkEffortHistoryItem(18, StartDate.AddDays(1));
            var item22 = new WorkEffortHistoryItem(12, StartDate.AddDays(3));
            var item23 = new WorkEffortHistoryItem(10, StartDate.AddDays(5));
            var item24 = new WorkEffortHistoryItem(4, StartDate.AddDays(7));
            var item25 = new WorkEffortHistoryItem(0, StartDate.AddDays(8));
            task2.AddWorkEffortHistoryItem(item21);
            task2.AddWorkEffortHistoryItem(item22);
            task2.AddWorkEffortHistoryItem(item23);
            task2.AddWorkEffortHistoryItem(item24);
            task2.AddWorkEffortHistoryItem(item25);

            currentIteration.AddTask(task1);
            currentIteration.AddTask(task2);

            var currentproject = new Project("TestProjectName");
            currentproject.SystemId = "project system ID";
            currentproject.AddIteration(currentIteration);
            server.AddProject(currentproject);

            RepositoryMock = new Mock<IRepository<ProjectInfoServer>>();
            RepositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<ProjectInfoServer>>()))
                        .Returns(new List<ProjectInfoServer> { server });
        };

        protected Context there_are_3_projects_in_repository = () =>
        {
            var server = new ProjectInfoServer("server name", "url");

            var project1 = new Project("TestProjectName1");
            project1.SystemId = "project system ID1";
            project1.AddIteration(new Iteration(DateTime.Today, DateTime.Today.AddDays(2)));
            server.AddProject(project1);
            project1 = new Project("TestProjectName2");
            project1.SystemId = "project system ID2";
            project1.AddIteration(new Iteration(DateTime.Today, DateTime.Today.AddDays(4)));
            server.AddProject(project1);
            project1 = new Project("TestProjectName3");
            project1.SystemId = "project system ID3";
            project1.AddIteration(new Iteration(DateTime.Today, DateTime.Today.AddDays(6)));
            server.AddProject(project1);

            RepositoryMock = new Mock<IRepository<ProjectInfoServer>>();
            RepositoryMock.Setup(r => r.Get(It.IsAny<Specification<ProjectInfoServer>>())).Returns(
                new List<ProjectInfoServer> {server});
        };
        
        protected Context controller_is_created = CreateController;
        
        protected Context settings_are_set_to_include_dates_before_Iteration_and_selected_projectName_is_set_in_repository = () =>
        {
            configuration.IsConfigured = true;
            configuration.NewSetting(INCLUDE_WORK_ITEMS_BEFORE_ITERATION_START_DATE, "true");
            configuration.NewSetting(SELECTED_PROJECT_NAME, "TestProjectName3");

            var configList = new List<Configuration> { configuration };

//            ConfigRepositoryMock.Setup(
//                r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configList);
        };

        protected Context settings_are_set_to_include_dates_before_Iteration_and_selected_projectName_is_not_set_in_repository = () =>
        {
            configuration.NewSetting(INCLUDE_WORK_ITEMS_BEFORE_ITERATION_START_DATE, "true");


//            ConfigRepositoryMock.Setup(
//                r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configList);
        };

        protected Context there_is_no_settings_in_repository = () =>
        {
            var configList = new List<Configuration> ();
//            configuration.Settings = configList;
//            ConfigRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configList);
        };

        protected Context settings_are_set_to_not_include_dates_before_Iteration = () =>
        {
            configuration.NewSetting(INCLUDE_WORK_ITEMS_BEFORE_ITERATION_START_DATE, "false");
            configuration.IsConfigured = true;
            var configList = new List<Configuration> { configuration };

//            ConfigRepositoryMock.Setup(
//                r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configList);
        };

        protected Context settings_are_changed_to_not_include_dates_before_Iteration = () => 
        {
            Controller.settingsViewModel.IncludeWorkItemsBeforeIterationStartdate = false; 
        };

        protected Context selected_project_name_is_changed_to_newName = () =>
        {
            Controller.settingsViewModel.SelectedProjectName = "newName";
        };

//        protected Context configPersisterMock_setup_to_return_savecomplete = () => 
//            ConfigPersisterMock.Setup(r => r.Save(It.IsAny<Configuration>())).
//            Raises(t => t.SaveCompleted += null, new SaveCompletedEventArgs());

        protected Context save_command_has_been_executed = () => Controller.settingsViewModel.Save.Execute(null);

        protected When the_controller_is_created = CreateController;

        protected When loading_data_and_notified_to_refresh = () =>
        {
            Controller.ViewModel.IsLoading = true;
            INotifyWhenToRefreshMock.Raise(n => n.Elapsed += null, new EventArgs());
        };

        protected When save_command_is_executed = () => Controller.settingsViewModel.Save.Execute(null);

        protected When reloadSettings_delegate_is_executed =
            () => Controller.settingsViewModel.ReloadSettings.Execute(null);

        protected static void CreateController()
        {
            INotifyWhenToRefreshMock = new Mock<ITimer>();
            LoadingNotifierMock = new Mock<IProgressbar>();
            Controller = new BurndownChartController(
                new BurndownChartViewModel(),
                new BurndownChartSettingsViewModel(),
                RepositoryMock.Object,
                configuration,
                INotifyWhenToRefreshMock.Object,
                new NoUIInvokation(),
                BackgroundWorkerInvoker,
                new Logger(new LogEntryMockPersister()),
                LoadingNotifierMock.Object
                );
        }

        internal class LogEntryMockPersister : IPersistDomainModels<LogEntry>
        {
            public void Save(LogEntry domainModel) { }
            public void Save(IEnumerable<LogEntry> domainModels) { }
        }
    }
}