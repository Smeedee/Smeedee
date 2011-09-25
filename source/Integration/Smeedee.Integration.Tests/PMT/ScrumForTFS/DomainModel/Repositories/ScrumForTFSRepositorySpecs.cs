using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Integration.PMT.ScrumForTFS.DomainModel.Repositories;
using Smeedee.Integration.Database.DomainModel.Repositories;

using Moq;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.IntegrationTests.PMT.ScrumForTFS.DomainModel.Repositories
{
    [TestFixture][Category("IntegrationTest")]
    public class ScrumForTFSRepositorySpecsShared
    {
        static IFetchWorkItems fetcher;

        protected static ScrumForTFSRepository repository;
        protected static ProjectInfoDatabaseRepository persister;

        protected Context a_ScrumForTFSRepository_is_created = () =>
        {
            repository = new ScrumForTFSRepository(fetcher);
        };

        [SetUp]
        public void setup()
        {
            var mockFetcher = new Mock<IFetchWorkItems>();

            var pTasks = new List<Task>
            {
                new Task
                {
                    SystemId = "Task 1",
                    Name = "Task 1",
                    Status = "Open",
                    WorkEffortEstimate = 20
                },
                new Task
                {
                    SystemId = "Task 2",
                    Name = "Task 2",
                    Status = "Open",
                    WorkEffortEstimate = 10
                },
                new Task
                {
                    SystemId = "Task 3",
                    Name = "Task 3",
                    Status = "Open",
                    WorkEffortEstimate = 15
                }
            };

            pTasks[0].AddWorkEffortHistoryItem(new WorkEffortHistoryItem
            {
                RemainingWorkEffort = 10,
                TimeStampForUpdate = DateTime.Now.AddDays(-4)
            });
            pTasks[0].AddWorkEffortHistoryItem(new WorkEffortHistoryItem
            {
                RemainingWorkEffort = 6,
                TimeStampForUpdate = DateTime.Now.AddDays(-3)
            });
            pTasks[0].AddWorkEffortHistoryItem(new WorkEffortHistoryItem
            {
                RemainingWorkEffort = 2,
                TimeStampForUpdate = DateTime.Now.AddDays(-2)
            });
            pTasks[0].AddWorkEffortHistoryItem(new WorkEffortHistoryItem
            {
                RemainingWorkEffort = 1,
                TimeStampForUpdate = DateTime.Now.AddDays(-1)
            });

            pTasks[1].AddWorkEffortHistoryItem(new WorkEffortHistoryItem
            {
                RemainingWorkEffort = 7,
                TimeStampForUpdate = DateTime.Now.AddDays(-3)
            });
            pTasks[1].AddWorkEffortHistoryItem(new WorkEffortHistoryItem
            {
                RemainingWorkEffort = 5,
                TimeStampForUpdate = DateTime.Now.AddDays(-2)
            });

            pTasks[2].AddWorkEffortHistoryItem(new WorkEffortHistoryItem
            {
                RemainingWorkEffort = 12,
                TimeStampForUpdate = DateTime.Now.AddDays(-7)
            });
            pTasks[2].AddWorkEffortHistoryItem(new WorkEffortHistoryItem
            {
                RemainingWorkEffort = 8,
                TimeStampForUpdate = DateTime.Now.AddDays(-5)
            });
            pTasks[2].AddWorkEffortHistoryItem(new WorkEffortHistoryItem
            {
                RemainingWorkEffort = 4,
                TimeStampForUpdate = DateTime.Now.AddDays(-3)
            });

            mockFetcher.Setup((o) => o.GetAllWorkEffort())
                .Returns(pTasks);

            mockFetcher.Setup((o) => o.GetAllIterations())
                .Returns(new List<String>() {"Iteration 1"});

            mockFetcher.Setup((o) => o.GetAllWorkEffortInSprint("Iteration 1"))
                .Returns(pTasks);

            fetcher = mockFetcher.Object;
        }

        [Test]
        public void assure_will_retrieve_all_tasks()
        {
            IEnumerable<ProjectInfoServer> result = new List<ProjectInfoServer>();
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_ScrumForTFSRepository_is_created);
               
                scenario.When("all workitems are requested", () =>
                    result = repository.Get(new AllSpecification<ProjectInfoServer>()));
                
                scenario.Then("all the tasks are retrieved", () =>
                {
                    var projectInfoServer = result.First();
                    var project = projectInfoServer.Projects.First();
                    var iteration = project.Iterations.First();
                    var tasks = iteration.Tasks;

                    tasks.Count().ShouldBe(3);
                    tasks.ElementAt(0).Name.ShouldBe("Task 1");
                    tasks.ElementAt(1).Name.ShouldBe("Task 2");
                    tasks.ElementAt(2).Name.ShouldBe("Task 3");
                });
            });
        }


        [Test]
        public void assure_all_effort_history_is_contained_in_tasks()
        {
            IEnumerable<ProjectInfoServer> result = new List<ProjectInfoServer>();
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_ScrumForTFSRepository_is_created);

                scenario.When("all workitems are requested", () =>
                    result = repository.Get(new AllSpecification<ProjectInfoServer>()));

                scenario.Then("all the tasks have the correct amount of WorkEffortHistoryItems", () =>
                {
                    var projectInfoServer = result.First();
                    var project = projectInfoServer.Projects.First();
                    var iteration = project.Iterations.First();
                    var tasks = iteration.Tasks;

                    tasks.ElementAt(0).WorkEffortHistory.Count().ShouldBe(4);
                    tasks.ElementAt(1).WorkEffortHistory.Count().ShouldBe(2);
                    tasks.ElementAt(2).WorkEffortHistory.Count().ShouldBe(3);
                });
            });
        }
    }
}
