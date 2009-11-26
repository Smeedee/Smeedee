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
using APD.DomainModel.ProjectInfo;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.DomainModel.TaskSpecs
{
    public class Shared
    {
        protected static ProjectInfoServer server;
        protected static Project project;
        protected static Iteration iteration;
        protected static Task task;
        protected static WorkEffortHistoryItem historyItem1;
        protected static WorkEffortHistoryItem historyItem2;
        protected static WorkEffortHistoryItem historyItem3;

        protected Context task_and_parents_have_been_created_with_system_id = () =>
        {
            server = new ProjectInfoServer();
            server.Url = "Fake.com";

            project = new Project();
            project.SystemId = "1";

            iteration = new Iteration();
            iteration.SystemId = "2";

            task = new Task();
            task.SystemId = "3";
        };

        protected Context task_is_added_to_iteration = () =>
        {
            iteration.AddTask(task);
            project.AddIteration(iteration);
            server.AddProject(project);
        };

        protected When a_history_item_is_added_to_the_task = () =>
        {
            WorkEffortHistoryItem historyItem = new WorkEffortHistoryItem(10, DateTime.Now);
            task.AddWorkEffortHistoryItem(historyItem);
        };

        protected When two_history_items_and_two_updated_history_items_are_added_to_the_task = () =>
        {
            var historyItem1 = new WorkEffortHistoryItem(20, DateTime.Now);
            var historyItem2 = new WorkEffortHistoryItem(10, DateTime.Now.AddDays(1));
            var updatedHistoryItem1 = new WorkEffortHistoryItem(18, DateTime.Now.AddHours(2));
            var updatedHistoryItem2 = new WorkEffortHistoryItem(8, DateTime.Now.AddDays(1).AddHours(2));

            task.AddWorkEffortHistoryItem(historyItem1);
            task.AddWorkEffortHistoryItem(historyItem2);
            task.AddWorkEffortHistoryItem(updatedHistoryItem1);
            task.AddWorkEffortHistoryItem(updatedHistoryItem2);
        };

        protected When three_unsorted_history_items_are_added_to_the_task = () =>
        {
            historyItem1 = new WorkEffortHistoryItem(8, new DateTime(2009, 10, 10));
            historyItem2 = new WorkEffortHistoryItem(4, new DateTime(2009, 10, 15));
            historyItem3 = new WorkEffortHistoryItem(2, new DateTime(2009, 10, 19));
            task.AddWorkEffortHistoryItem(historyItem3);
            task.AddWorkEffortHistoryItem(historyItem1);
            task.AddWorkEffortHistoryItem(historyItem2);
        };
    }

    
    [TestFixture]
    public class When_spawned : Shared
    {
        [Test]
        public void should_have_default_constructor()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(task_and_parents_have_been_created_with_system_id).
                         And(task_is_added_to_iteration);
                scenario.When("the Task object is created with default constructor");
                scenario.Then("the Task object should be instanciated correctly", () =>
                {
                    task.ShouldNotBeNull();
                    task.SystemId.ShouldBe("3");
                    task.Iteration.ShouldBeSameAs(iteration);
                    task.Name.ShouldBeNull();
                    task.WorkEffortEstimate.ShouldBe(0);
                    task.Status.ShouldBeNull();
                    task.WorkEffortHistory.ShouldBeInstanceOfType<List<WorkEffortHistoryItem>>();
                    task.WorkEffortHistory.Count().ShouldBe(0);
                    task.Equals(task).ShouldBeTrue();
                });
            });
        }
    }

    
    [TestFixture]
    public class When_adding_WorkEffortHistoryItems : Shared
    {
        [Test]
        public void should_be_able_to_add_WorkEffortHistoryItem()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(task_and_parents_have_been_created_with_system_id).
                    And(task_is_added_to_iteration);
                scenario.When(a_history_item_is_added_to_the_task);
                scenario.Then("task should have a history item", () =>
                {
                    task.WorkEffortHistory.Count().ShouldBe(1);
                });
            });
        }

        [Test]
        public void should_be_able_to_add_more_than_one_history_item_for_a_day()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(task_and_parents_have_been_created_with_system_id).
                    And(task_is_added_to_iteration);
                scenario.When(two_history_items_and_two_updated_history_items_are_added_to_the_task);
                scenario.Then("should get correct remaining work effort for any given day", () =>
                {
                    task.WorkEffortHistory.Count().ShouldBe(4);
                });
            });
        }

        [Test]
        public void should_be_able_to_sort_WorkEffortHistoryItems()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(task_and_parents_have_been_created_with_system_id).
                    And(task_is_added_to_iteration);
                scenario.When(three_unsorted_history_items_are_added_to_the_task);
                scenario.Then("should be able to sort the history items by date", () =>
                {
                    List<WorkEffortHistoryItem> updateSets = new List<WorkEffortHistoryItem>();
                    updateSets.AddRange(task.WorkEffortHistory);

                    historyItem1.ShouldBe(updateSets[1]);
                    historyItem2.ShouldBe(updateSets[2]);
                    historyItem3.ShouldBe(updateSets[0]);

                    updateSets.Sort();

                    historyItem1.ShouldBe(updateSets[0]);
                    historyItem2.ShouldBe(updateSets[1]);
                    historyItem3.ShouldBe(updateSets[2]);
                });
            });
        }
    }
}