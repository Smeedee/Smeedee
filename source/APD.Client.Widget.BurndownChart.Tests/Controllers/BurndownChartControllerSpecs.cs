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
using APD.Client.Framework;
using APD.Client.Widget.BurndownChart.Controllers;
using APD.DomainModel.Framework;
using APD.DomainModel.ProjectInfo;
using Moq;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.Client.Widget.BurndownChart.BurndownChartControllerSpecs
{
    public class Shared
    {
        protected static BurndownChartController controller;
        protected static Mock<IRepository<ProjectInfoServer>> repositoryMock;
        protected static Mock<INotifyWhenToRefresh> iNotifyWhenToRefreshMock;
        protected static IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>> BackgroundWorkerInvoker =
                            new NoBackgroundWorkerInvocation<IEnumerable<ProjectInfoServer>>();

        protected static DateTime _startDate;

        protected Context there_are_no_projects_in_the_repository = () =>
        {
            var server = new List<ProjectInfoServer>();

            repositoryMock = new Mock<IRepository<ProjectInfoServer>>();
            repositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<ProjectInfoServer>>())).Returns(server);
        };

        protected Context there_is_no_current_iteration_in_the_given_project = () =>
        {
            var server = new ProjectInfoServer("Mock Server", "http://mock.com");

            var project = new Project("TestProjectName");
            project.SystemId = "ProjectNr1";
            server.AddProject(project);

            repositoryMock = new Mock<IRepository<ProjectInfoServer>>();
            repositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<ProjectInfoServer>>()))
                        .Returns(new List<ProjectInfoServer> {server});
        };
        
        protected Context there_is_a_current_iteration_in_the_given_project_but_no_tasks = () =>
        {
            var currentIteration = new Iteration();
            currentIteration.SystemId = "iteration system ID";

            var currentproject = new Project("TestProjectName");
            currentproject.SystemId = "project system ID";
            currentproject.AddIteration(currentIteration);

            var server = new ProjectInfoServer("server name", "url");
            server.AddProject(currentproject);

            repositoryMock = new Mock<IRepository<ProjectInfoServer>>();
            repositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<ProjectInfoServer>>()))
                        .Returns(new List<ProjectInfoServer> { server });
        };

        protected Context there_is_a_current_iteration_in_the_given_project_with_tasks = () =>
        {
            var server = new ProjectInfoServer("server name", "url");

            var currentIteration = new Iteration();
            currentIteration.SystemId = "iteration system ID";

            var tenDays = new TimeSpan(10, 0, 0, 0);
            _startDate = DateTime.Now.Subtract(tenDays);
            currentIteration.StartDate = _startDate;
            currentIteration.EndDate = _startDate.AddDays(14);
            currentIteration.Name = "Test Iteration 1";

            var task1 = new Task();
            var task2 = new Task();
            task1.WorkEffortEstimate = 50;
            task2.WorkEffortEstimate = 20;
            task1.SystemId = "task1 system ID";
            task2.SystemId = "task2 system ID";

            var item11 = new WorkEffortHistoryItem(45, _startDate);
            var item12 = new WorkEffortHistoryItem(35, _startDate.AddDays(1));
            var item13 = new WorkEffortHistoryItem(20, _startDate.AddDays(3));
            var item14 = new WorkEffortHistoryItem(5, _startDate.AddDays(6));
            var item15 = new WorkEffortHistoryItem(0, _startDate.AddDays(7));
            task1.AddWorkEffortHistoryItem(item11);
            task1.AddWorkEffortHistoryItem(item12);
            task1.AddWorkEffortHistoryItem(item13);
            task1.AddWorkEffortHistoryItem(item14);
            task1.AddWorkEffortHistoryItem(item15);

            var item21 = new WorkEffortHistoryItem(18, _startDate.AddDays(1));
            var item22 = new WorkEffortHistoryItem(12, _startDate.AddDays(3));
            var item23 = new WorkEffortHistoryItem(10, _startDate.AddDays(5));
            var item24 = new WorkEffortHistoryItem(4, _startDate.AddDays(7));
            var item25 = new WorkEffortHistoryItem(0, _startDate.AddDays(8));
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

            repositoryMock = new Mock<IRepository<ProjectInfoServer>>();
            repositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<ProjectInfoServer>>()))
                        .Returns(new List<ProjectInfoServer> { server });
        };


        protected Context controller_is_created = () =>
        {
            CreateController();
        };

        protected When the_controller_is_created = () =>
        {
            CreateController();
        };

        protected static void CreateController()
        {
            iNotifyWhenToRefreshMock = new Mock<INotifyWhenToRefresh>();
            controller = new BurndownChartController(repositoryMock.Object, iNotifyWhenToRefreshMock.Object,
                                                     new NoUIInvocation(), BackgroundWorkerInvoker);
        }
    }


    [TestFixture]
    public class When_the_controller_is_spawned : Shared
    {
        [Test]
        public void should_notify_user_if_project_does_not_exist_in_repository()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_no_projects_in_the_repository);

                scenario.When(the_controller_is_created);

                scenario.Then("the user is notified that there are no projects in the repository", () =>
                {
                    controller.CurrentProject.ShouldBeNull();
                    controller.ViewModel.ErrorMessage.ShouldBe("No projects with the given name exists in the repository");
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
                    controller.CurrentProject.ShouldNotBeNull();
                    controller.ViewModel.ErrorMessage.ShouldBe("There are no iterations in the given project");
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
                    controller.CurrentProject.ShouldNotBeNull();
                    controller.ViewModel.ErrorMessage.ShouldBe("There are no tasks for the current iteration for the given project");
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
                    controller.ViewModel.ProjectName.ShouldBe("TestProjectName");
                    controller.ViewModel.IterationName.ShouldBe("Test Iteration 1");
                });
            });
        }

        [Test]
        public void should_set_correct_ActualRegression_coordinates_in_burndown_chart_viewmodel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks);

                scenario.When(the_controller_is_created);

                scenario.Then("the correct burndown coordinates are displayed", () =>
                {
                    controller.ViewModel.ActualBurndown.Count().ShouldBe(9);
                    controller.ViewModel.ActualBurndown[0].RemainingWorkEffort.ShouldBe(70);
                    controller.ViewModel.ActualBurndown[1].RemainingWorkEffort.ShouldBe(65);
                    controller.ViewModel.ActualBurndown[2].RemainingWorkEffort.ShouldBe(53);
                    controller.ViewModel.ActualBurndown[3].RemainingWorkEffort.ShouldBe(32);
                    controller.ViewModel.ActualBurndown[4].RemainingWorkEffort.ShouldBe(30);
                    controller.ViewModel.ActualBurndown[5].RemainingWorkEffort.ShouldBe(15);
                    controller.ViewModel.ActualBurndown[6].RemainingWorkEffort.ShouldBe(4);
                    controller.ViewModel.ActualBurndown[7].RemainingWorkEffort.ShouldBe(0);
                    controller.ViewModel.ActualBurndown[8].RemainingWorkEffort.ShouldBe(0);
                });
            });
        }

        [Test]
        public void should_set_correct_last_borndown_coordinate_with_timestamp_now_in_burndown_chart_viewmodel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_is_a_current_iteration_in_the_given_project_with_tasks);

                scenario.When(the_controller_is_created);

                scenario.Then("two last coordinates should be different, and the last one should be now", () =>
                {
                    DateTime lastTimeStamp = controller.ViewModel.ActualBurndown[8].TimeStampForUpdate;
                    TimeSpan timeSpanFromLastTimeStampToNow = DateTime.Now.Subtract(lastTimeStamp);

                    controller.ViewModel.ActualBurndown[7].TimeStampForUpdate.Date.ShouldNotBe(DateTime.Now.Date);
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
                    controller.ViewModel.IdealBurndown.Count().ShouldBe(2);
                    controller.ViewModel.IdealBurndown[0].RemainingWorkEffort.ShouldBe(70);
                    controller.ViewModel.IdealBurndown[0].TimeStampForUpdate.Date.ShouldBe(_startDate.Date);
                    controller.ViewModel.IdealBurndown[1].RemainingWorkEffort.ShouldBe(0);
                    controller.ViewModel.IdealBurndown[1].TimeStampForUpdate.Date.ShouldBe(_startDate.AddDays(15).Date);
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
                         And(controller_is_created);

                scenario.When("notified to refresh", () =>
                    iNotifyWhenToRefreshMock.Raise(n => n.Refresh += null, new RefreshEventArgs()));

                scenario.Then("assure it queried project repository for the given project", () =>
                    repositoryMock.Verify(
                        r => r.Get(It.IsAny<Specification<ProjectInfoServer>>()), Times.Exactly(2)));
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
}