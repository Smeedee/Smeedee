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
using System.Net;

using APD.Integration.PMT.TFS.DomainModel.Repositories;

using Microsoft.TeamFoundation.WorkItemTracking.Client;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


// TODO Tests are integration specific and will only work in trondheim for now.
// Because of this they are now set to  and must be run manually.

namespace APD.IntegrationTests.PMT.TFS.DomainModel.Repositories
{
    public class Shared
    {
        public static String serverAddress = "https://tfs08.codeplex.com";
        public static String projectName = "smeedee";
        public static String sprintName = "Sprint 1";
        public static NetworkCredential credentials = new NetworkCredential("smeedee_cp", "haldis");

        public static WorkItemFetcher fetcher;

        protected Context work_item_fetcher_is_instantiated =
            () => { fetcher = new WorkItemFetcher(serverAddress, projectName, credentials); };
    }

    [TestFixture]
    [Ignore]
    public class When_spawned : Shared
    {
        [Test]
        public void Assure_correct_server_data_is_set()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(work_item_fetcher_is_instantiated);
                scenario.When("Nothing is done to it");
                scenario.Then("correct project name should be set",
                              () => fetcher.ProjectName.ShouldBe(projectName));
            });
        }
    }

    [TestFixture]
    [Ignore]
    public class Integration_tests : Shared
    {
        private static WorkItemCollection result;

        [Test]
        public void Assure_WorkItems_can_be_fetched()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(work_item_fetcher_is_instantiated);
                scenario.When("we ask for work items", () => { result = fetcher.GetAllWorkItems(); });
                scenario.Then("At least some work items are returned", () =>
                {
                    result = fetcher.GetAllWorkItems();
                    result.ShouldHaveMoreThan(1);
                });
            });
        }

        [Test]
        public void Assert_all_retrieved_work_items_are_from_the_correct_sprint()
        {
            string iteration = "Sprint 1";
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(work_item_fetcher_is_instantiated);
                scenario.When("we ask for all work items in Sprint 1",
                              () => { result = fetcher.GetWorkItemsByIteration(iteration); });
                scenario.Then("no retrieved work items should be from any other iteration", () =>
                {
                    foreach (var item in result)
                        ( (WorkItem) item ).IterationPath.EndsWith(iteration).ShouldBeTrue();
                });
            });
        }

        [Test]
        public void Assert_querying_by_date_does_not_give_newer_work_items()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(work_item_fetcher_is_instantiated);
                scenario.When("we ask for all work items older than 2009/07/09",
                              () =>
                              {
                                  result =
                                      fetcher.GetAllWorkItemsAsOfDate(new DateTime(2009, 07, 09, 23, 59, 59));
                              });
                scenario.Then("no work items created on 2009/07/10 or later should be returned.", () =>
                {
                    foreach (var item in result)
                        Assert.IsTrue(( (WorkItem) item ).ChangedDate < new DateTime(2009, 07, 10));
                });
            });
        }

        [Test]
        public void Assert_querying_by_date_and_sprint_does_not_give_newer_work_items()
        {
            string iteration = "Sprint 1";
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(work_item_fetcher_is_instantiated);
                scenario.When("we ask for all work items older than 2009/07/09 from Sprint 1",
                              () =>
                              {
                                  result = fetcher.GetWorkItemsByIterationAsOfDate(iteration,
                                                                                   new DateTime(2009, 07, 09, 23, 59, 59));
                              });
                scenario.Then("no work items created on 2009/07/10 or later should be returned.", () =>
                {
                    foreach (var item in result)
                        Assert.IsTrue(( (WorkItem) item ).ChangedDate < new DateTime(2009, 07, 10));
                });
            });
        }

        [Test]
        public void Assert_querying_by_date_and_sprint_does_not_give_items_from_other_sprints()
        {
            string iteration = "Sprint 1";
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(work_item_fetcher_is_instantiated);
                scenario.When("we ask for all work items older than 2009/07/09 from Sprint 1",
                              () =>
                              {
                                  result = fetcher.GetWorkItemsByIterationAsOfDate(iteration,
                                                                                   new DateTime(2009, 07, 09, 23, 59, 59));
                              });
                scenario.Then("no work items created on 2009/07/10 or later should be returned.", () =>
                {
                    foreach (var item in result)
                        ( (WorkItem) item ).IterationPath.EndsWith(iteration).ShouldBeTrue();
                });
            });
        }
    }
}