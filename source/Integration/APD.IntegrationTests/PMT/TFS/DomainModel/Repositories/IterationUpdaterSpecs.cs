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
using System.Linq;
using System.Collections.Generic;
using System.Net;

using APD.DomainModel.ProjectInfo;
using APD.Integration.PMT.TFS.DomainModel.Repositories;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.IntegrationTests.PMT.TFS.DomainModel.Repositories
{
    [TestFixture]
    [Ignore]
    public class When_updating_info_about_given_sprint
    {
        protected static String serverAddress = "https://tfs08.codeplex.com";
        protected static String projectName = "smeedee";
        protected static String sprintName = "Sprint 1";
        protected static NetworkCredential credentials = new NetworkCredential("smeedee_cp", "haldis");

        protected static Iteration iteration;
        protected static int numberOfTasksBeforeUpdate;
        protected static IterationUpdater repository;

        protected Context the_repository_and_iteration_are_created = () =>
        {
            repository = new IterationUpdater(serverAddress, projectName, credentials);

            iteration = new Iteration
                        {
                            SystemId = "SprintX",
                            Name = sprintName,
                            HolidayProvider = new HolidayProvider(),
                            StartDate = new DateTime(2009, 8, 5),
                            EndDate = new DateTime(2009, 8, 19),
                        };
            PopulateWithThreeTasks(iteration);
            numberOfTasksBeforeUpdate = iteration.GetTaskCount();
        };

        protected When info_in_given_sprint_is_updated =
            () => { iteration = repository.UpdateIteration(iteration); };


        protected static void PopulateWithThreeTasks(Iteration iteration)
        {
            var task1 = new Task
                        {
                            SystemId = "1",
                            Status = "In progress",
                            Name = "Set up: Set Permissions",
                            WorkEffortEstimate = 30,
                        };
            task1.AddWorkEffortHistoryItem(GetWorkEffortHistoryItem());
            iteration.AddTask(task1);

            var task2 = new Task
                        {
                            SystemId = "2",
                            Status = "In progress",
                            Name = "Set up: Migration of Source Code",
                            WorkEffortEstimate = 25
                        };
            task2.AddWorkEffortHistoryItem(GetWorkEffortHistoryItem());
            iteration.AddTask(task2);

            var task3 = new Task
                        {
                            SystemId = "3",
                            Status = "In progress",
                            Name = "Set up: Migration of Work Items",
                            WorkEffortEstimate = 6
                        };
            task3.AddWorkEffortHistoryItem(GetWorkEffortHistoryItem());
            iteration.AddTask(task3);
        }

        private static WorkEffortHistoryItem GetWorkEffortHistoryItem()
        {
            var workEffortHistoryItem = new WorkEffortHistoryItem
                                        {
                                            RemainingWorkEffort = 23,
                                            TimeStampForUpdate = new DateTime(2009, 3, 6)
                                        };
            return workEffortHistoryItem;
        }


        [Test]
        public void Should_be_able_to_add_all_new_tasks_in_the_sprint()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_repository_and_iteration_are_created);
                scenario.When(info_in_given_sprint_is_updated);

                scenario.Then("it should add all new tasks in the sprint", () =>
                {
                    List<Task> tasks = new List<Task>();
                    tasks.AddRange(iteration.Tasks);
                    
                    tasks.Count.ShouldNotBe(numberOfTasksBeforeUpdate);
                });
            });
        }

        [Test]
        public void Should_be_able_to_correctly_add_new_changesets_to_all_tasks()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_repository_and_iteration_are_created);
                scenario.When(info_in_given_sprint_is_updated);

                scenario.Then("it should correctly add new changesets to all tasks", () =>
                {
                    var tasks = new List<Task>();
                    tasks.AddRange(iteration.Tasks);
                    
                    var oldTasks = new List<Task>();
                    oldTasks.AddRange(
                        tasks.Where(t => t.WorkEffortHistory.Count() == 2)
                    );

                    oldTasks.Count.ShouldBe(numberOfTasksBeforeUpdate);
                });
            });
        }

        [Test]
        public void Should_be_able_to_correctly_update_iteration_work_history_with_correct_date_when_supplying_custom_date()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_repository_and_iteration_are_created);
                scenario.When("info in given sprint is updated with a supplied DateTime object",
                              () => { iteration = repository.UpdateIterationByDate(iteration, DateTime.Today); });

                scenario.Then("it should correctly add correct date timestamps", () =>
                {
                    var tasks = new List<Task>();
                    tasks.AddRange(iteration.Tasks);

                    foreach (Task task in tasks)
                    {
                        task.Iteration = iteration;

                        var historyItems = new List<WorkEffortHistoryItem>();
                        foreach (WorkEffortHistoryItem item in task.WorkEffortHistory)
                            historyItems.Add(item);

                        historyItems[task.WorkEffortHistory.Count() - 1].TimeStampForUpdate.ShouldBe(DateTime.Today);
                    }
                });
            });
        }

        // TODO: Perhaps a bit to generic.
        [Test]
        public void Should_return_a_proper_project_when_get_is_called_with_correct_arguments()
        {
            Scenario.StartNew(this, scenario =>
            {
                Project project = null;
                scenario.Given(the_repository_and_iteration_are_created);
                scenario.When("get is called with proper arguments", () =>
                                                                     project =
                                                                     repository.Get("Sprint 1",
                                                                                    DateTime.Today.AddDays(-10),
                                                                                    DateTime.Today));

                scenario.Then("A proper project should be returned.", () =>
                {
                    project.Name.ShouldBe(projectName);
                    project.Iterations.ShouldNotBeNull();
                    project.CurrentIteration.ShouldNotBeNull();
                    project.CurrentIteration.StartDate.ShouldNotBeNull();
                    project.CurrentIteration.EndDate.ShouldNotBeNull();
                    project.CurrentIteration.Name.ShouldBe("Sprint 1");
                });
            });
        }

        [Test]
        public void Should_convert_end_date_to_today_when_end_date_in_future()
        {
            Scenario.StartNew(this, scenario =>
            {
                DateTime endDate = DateTime.Today.AddDays(1);
                scenario.Given(the_repository_and_iteration_are_created);
                scenario.When("the end date converter is called with date in the future", () =>
                                                                                          endDate =
                                                                                          repository.
                                                                                              ConvertDateToTodayIfDateInFuture
                                                                                              (endDate));

                scenario.Then("the end date should be converted to today.", () =>
                {
                    endDate.Year.ShouldBe(DateTime.Today.Year);
                    endDate.Month.ShouldBe(DateTime.Today.Month);
                    endDate.Day.ShouldBe(DateTime.Today.Day);
                });
            });
        }

        [Test]
        public void Should_not_convert_end_date_if_end_date_is_less_than_today()
        {
            Scenario.StartNew(this, scenario =>
            {
                DateTime endDate = DateTime.Today.AddDays(-1);
                scenario.Given(the_repository_and_iteration_are_created);
                scenario.When("the end date converter is called with date in the past", () =>
                                                                                        endDate =
                                                                                        repository.
                                                                                            ConvertDateToTodayIfDateInFuture
                                                                                            (endDate));

                scenario.Then("the end date should not be converted to today", () =>
                {
                    endDate.Year.ShouldBe(DateTime.Today.AddDays(-1).Year);
                    endDate.Month.ShouldBe(DateTime.Today.AddDays(-1).Month);
                    endDate.Day.ShouldBe(DateTime.Today.AddDays(-1).Day);
                });
            });
        }
    }
}