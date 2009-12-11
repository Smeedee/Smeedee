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
using APD.DomainModel.ProjectInfo;
using APD.Plugin.ProjectInfo.DomainModel.Repositories;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;
using TinyBDD.Dsl.GivenWhenThen;


namespace APD.DomainModel.IterationSpecs
{
    public class Shared
    {
        protected static ProjectInfoServer server;
        protected static Project project;
        protected static Iteration iteration;
        protected static HolidayProvider holidayProvider;

        protected Context iteration_has_been_created = () =>
        {
            iteration = new Iteration {SystemId = Guid.NewGuid().ToString(), HolidayProvider = new HolidayProvider()};
        };

        public Context an_iteration_has_been_created = () =>
        {
            var startDate = new DateTime(2009, 6, 26);
            var endDate = new DateTime(2009, 7, 10);
            var xmlParser = new XmlCountryHolidaysParser();
            holidayProvider = xmlParser.Parse();
            iteration = new Iteration(startDate, endDate, holidayProvider);
        };


        protected Context project_and_projectInfoServer_has_been_created = () =>
        {
            server = new ProjectInfoServer {Url = "Fake.com"};

            project = new Project("Bad Bush") {SystemId = "1"};
        };

        protected Context project_is_added_to_server = () =>
        {
            server.AddProject(project);
        };

        protected Context iteration_is_added_to_project = ()=>
        {
            project.AddIteration(iteration);
        };

    }

    [TestFixture]
    public class When_spawned : Shared
    {

        [Test]
        public void should_have_no_Name_property()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(iteration_has_been_created);

                scenario.When("always");
                scenario.Then("the Iteration should have a Name property", ()=>
                {
                    iteration.Name.ShouldBeNull();
                });
            });

        }

        [Test]
        public void should_have_Tasks_property()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(iteration_has_been_created);

                scenario.When("always");

                scenario.Then("the Iteration should have Tasks in a list", ()=>
                {
                    iteration.Tasks.ShouldBeInstanceOfType<List<Task>>();
                });
            });
        }

        [Test]
        public void iteration_should_have_unique_hash_code()
        {
            
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(iteration_has_been_created)
                    .And(project_and_projectInfoServer_has_been_created)
                        .And(project_is_added_to_server)
                            .And(iteration_is_added_to_project);

                scenario.When("the hashcode of a iteration object is requested");

                scenario.Then("then a unique hashcode should be returned", ()=>
                {
                    var otherIteration = new Iteration();
                    project.AddIteration(otherIteration);

                    otherIteration.SystemId = Guid.NewGuid().ToString();
                    
                    iteration.GetHashCode().ShouldNotBe(otherIteration.GetHashCode());
                });
            });
        
        }

        [Test]
        public void should_have_equals_method()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(iteration_has_been_created)
                    .And(project_and_projectInfoServer_has_been_created)
                        .And(project_is_added_to_server)
                            .And(iteration_is_added_to_project);

                scenario.When("the iteration is compared to another");

                scenario.Then("the equals method should return true only if they have the same hashCode", () =>
                {
                    var otherIteration = new Iteration();
                    project.AddIteration(otherIteration);
                    otherIteration.SystemId = iteration.SystemId;

                    iteration.Equals(otherIteration).ShouldBeTrue();
                });
            });
        }

        [Test]
        public void should_get_correct_list_of_actual_working_days()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(an_iteration_has_been_created);

                scenario.When("getting a list of actual working days");

                scenario.Then("the list should only consist of all the actual working days for this iteration", () =>
                {
                    iteration.StartDate = new DateTime(2009, 8, 3);
                    iteration.EndDate = new DateTime(2009, 8, 16);
                    List<DateTime> days = iteration.GetWorkingDays();
                    
                    days.Count.ShouldBe(10);
                });
            });
        }
    }

    [TestFixture]
    public class When_adding_tasks : Shared
    {

        [Test]
        public void should_be_able_to_add_Tasks()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(iteration_has_been_created)
                    .And(project_and_projectInfoServer_has_been_created)
                        .And(project_is_added_to_server)
                            .And(iteration_is_added_to_project);

                scenario.When("a task is added");
                scenario.Then("it should be stored in a list" , () =>
                {
                    var task = new Task();

                    iteration.AddTask(task);

                    var tasks = (List<Task>) iteration.Tasks;

                    tasks[0].ShouldBe(task);
                });
            });
        }

        [Test]
        public void should_set_Iteration_reference_on_Task()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(iteration_has_been_created);

                scenario.When("a Task is added");

                scenario.Then(
                    "the Iteration property on the Task object should point to the current Iteration", () =>
                    {
                        var task = new Task();
                        iteration.AddTask(task);

                        task.Iteration.Name.ShouldBe(iteration.Name);
                    });
            });
        }

        [Test]
        public void should_return_number_Tasks()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(iteration_has_been_created);
                scenario.When("the number of tasks is requested");
                scenario.Then("then the correct number of tasks should be returned", ()=>
                {
                    iteration.AddTask(new Task());
                    iteration.GetTaskCount().ShouldBe(1);
                });
            });
        }
    }

    [TestFixture]
    public class When_calculating_WorkingDays : Shared
    {
        
        [Test]
        public void should_calculate_workingdays_with_startDate_earlier_than_endDate()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(an_iteration_has_been_created);
                scenario.When("the iteration has a startDate earlier then its endDate");
                scenario.Then("workingdays left should be calculated", ()=>
                {
                    iteration.StartDate = new DateTime(2009, 6, 26);
                    iteration.EndDate = new DateTime(2009, 7, 10);
                    iteration.CalculateWorkingdaysLeft(iteration.StartDate).Days.ShouldBe(11);
                });
            });
        }

        [Test]
        public void should_calculate_workingdays_with_startDate_on_the_same_day_as_the_endDate()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(an_iteration_has_been_created);
                scenario.When("the iteration has a startDate on the same day as the endDate");
                scenario.Then("workingdays left should be 1", () =>
                {
                    iteration.StartDate = new DateTime(2009, 6, 26);
                    iteration.EndDate = new DateTime(2009, 6, 26);

                    iteration.CalculateWorkingdaysLeft(iteration.StartDate).Days.ShouldBe(1);
                });
            });
        }

        [Test]
        public void should_calculate_workingdays_with_startDate_later_than_endDate()
        {

            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(an_iteration_has_been_created);
                scenario.When("the iteration has a startDate later then its endDate");
                scenario.Then("workingdays left should be 1", () =>
                {
                    iteration.StartDate = new DateTime(2009, 6, 27);
                    iteration.EndDate = new DateTime(2009, 6, 26);

                    iteration.CalculateWorkingdaysLeft(iteration.StartDate).Days.ShouldBe(1);
                });
            });
        }

        [Test]
        public void should_calculate_workingdays_in_iteration_spanning_across_two_years()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(an_iteration_has_been_created);
                scenario.When("the iteration has a startDate in one year and endDate in the next");
                scenario.Then("workingdays left should caluculated correctly", () =>
                {
                    iteration.StartDate = new DateTime(2009, 12, 23);
                    iteration.EndDate = new DateTime(2010, 1, 3);

                    iteration.CalculateWorkingdaysLeft(iteration.StartDate).Days.ShouldBe(4);
                });
            });
        }

        [Test]
        public void should_calculate_workingdays_when_iteration_is_affected_by_a_leap_year()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(an_iteration_has_been_created);
                scenario.When("the iteration span is affected by leap yaer");
                scenario.Then("workingdays left should caluculated correctly", () =>
                {
                    iteration.StartDate = new DateTime(2009, 2, 22);
                    iteration.EndDate = new DateTime(2009, 3, 3);

                    iteration.CalculateWorkingdaysLeft(iteration.StartDate).Days.ShouldBe(7);
                });
            });
        }

        [Test]
        public void should_calculate_workingdays_when_iteration_is_affected_easter()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(an_iteration_has_been_created);
                scenario.When("the iteration span is affected by easter");
                scenario.Then("workingdays left should caluculated correctly", () =>
                {
                    iteration.StartDate = new DateTime(2009, 4, 1);
                    iteration.EndDate = new DateTime(2009, 4, 22);

                    iteration.CalculateWorkingdaysLeft(iteration.StartDate).Days.ShouldBe(13);
                });
            });
        }
    }

    [TestFixture]
    public class When_changing_status_of_a_day : Shared
    {

        [Test]
        public void should_set_a_non_working_day_as_a_working_day()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(an_iteration_has_been_created);
                scenario.When("the the user wants to add a extra working day");
                scenario.Then("an extra workingday should be added", () =>
                {
                    iteration.StartDate = new DateTime(2009, 6, 15);
                    iteration.EndDate = new DateTime(2009, 6, 22);

                    iteration.CalculateWorkingdaysLeft(iteration.StartDate).Days.ShouldBe(6);

                    var extraWorkingDay = new DateTime(2009, 6, 20);

                    iteration.SetExtraWorkingDay(extraWorkingDay);

                    iteration.CalculateWorkingdaysLeft(iteration.StartDate).Days.ShouldBe(7);
                });
            });
        }

        [Test]
        public void should_set_a_working_day_as_a_non_working_day()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(an_iteration_has_been_created);
                scenario.When("the the user wants add a holiday");
                scenario.Then("an holiday is added", () =>
                {
                    iteration.StartDate = new DateTime(2009, 6, 15);
                    iteration.EndDate = new DateTime(2009, 6, 22);

                    iteration.CalculateWorkingdaysLeft(iteration.StartDate).Days.ShouldBe(6);

                    var nonWorkingDay = new DateTime(2009, 6, 19);
                    iteration.SetExtraHoliday(nonWorkingDay);

                    iteration.CalculateWorkingdaysLeft(iteration.StartDate).Days.ShouldBe(5);
                });
            });
        }

    }

    [TestFixture]
    public class When_getting_WorkEffort_info : Shared
    {
        [Test]
        public void should_be_able_to_get_WorkEffortEstimateForTasks()
        {
            
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(an_iteration_has_been_created).And("it has tasks");
                scenario.When("remaining workeffort for iteration is requested");
                scenario.Then("the sum of every tasks workeffort should be returned", ()=>
                {
                    var task1 = new Task();
                    task1.WorkEffortEstimate = 20;
                    iteration.AddTask(task1);

                    var task2 = new Task();
                    task2.WorkEffortEstimate = 30;
                    iteration.AddTask(task2);

                    iteration.GetWorkEffortEstimateForTasks().ShouldBe(50);

                });
            });
        }
    }
}