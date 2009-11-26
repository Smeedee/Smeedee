using System;
using System.Collections.Generic;
using System.Linq;

using APD.DomainModel.CI;
using APD.DomainModel.Framework;
using APD.Integration.CI.TeamCity.DomainModel.Factories;
using APD.Integration.Framework.Atom.DomainModel.Repositories;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;


namespace APD.IntegrationTests.CI.TeamCity.Tests.DomainModel.Repositories.CIProjectRepositorySpecs
{
    public class Shared : ScenarioClass
    {
        protected const string ADDRESS_SINGLE_FAILED_BUILD = @"CI\TeamCity\TestData\SingleFailedBuild.xml";
        protected const string ADDRESS_SINGLE_SUCCESSFUL_BUILD = @"CI\TeamCity\TestData\SingleSuccessfulBuild.xml";

        protected static IEnumerable<Build> allBuilds;
        protected static AtomEntryRepository<Build> repository;

        protected When asking_for_all_builds =
            () => { allBuilds = repository.Get(new AllSpecification<Build>()); };


        protected Context repository_has_been_created = () =>
        {
            repository = new AtomEntryRepository<Build>(null)
            {
                EntryFactory = new TeamCityBuildAtomEntryFactory()
            };
        };

        protected Context there_is_one_failed_build =
            () => { repository.Address = new Uri(ADDRESS_SINGLE_FAILED_BUILD, UriKind.Relative); };

        protected Context there_is_one_successful_build = 
            () => { repository.Address = new Uri(ADDRESS_SINGLE_SUCCESSFUL_BUILD, UriKind.Relative);};

        [TearDown]
        protected void RunTests()
        {
            StartScenario();
        }
    }

    internal static class TestBuilds
    {
        internal static Build Build1 = new Build
        {
            Status = BuildStatus.FinishedWithFailure,
            StartTime = new DateTime(2009, 09, 21, 18, 58, 39, DateTimeKind.Utc).ToLocalTime(),
            FinishedTime = new DateTime(2009, 09, 21, 18, 58, 39, DateTimeKind.Utc).ToLocalTime(),
            Project = new CIProject("Smeedee::Continuous Build"),
            SystemId = new DateTime(2009, 09, 21, 18, 58, 39, DateTimeKind.Utc).ToLocalTime().Ticks.ToString()
        };
        internal static Build Build2 = new Build
        {
            Status = BuildStatus.FinishedSuccefully,
            StartTime = new DateTime(2009, 09, 21, 12, 53, 34, DateTimeKind.Utc).ToLocalTime(),
            FinishedTime = new DateTime(2009, 09, 21, 12, 53, 34, DateTimeKind.Utc).ToLocalTime(),
            Project = new CIProject("Smeedee::Continuous Build"),
            SystemId = new DateTime(2009, 09, 21, 12, 53, 34, DateTimeKind.Utc).ToLocalTime().Ticks.ToString(),
        };

        internal static bool Equals(Build build1, Build build2)
        {
            return build1.FinishedTime == build2.FinishedTime
                   && build1.StartTime == build2.StartTime
                   && build1.Status == build2.Status 
                   && build1.Project.ProjectName == build2.Project.ProjectName;
        }

        internal static string ToString(Build build)
        {
            return string.Format("Build: [{0}, {1}, {2}, {3}]", build.SystemId, build.StartTime.ToLocalTime(),
                                 build.FinishedTime.ToLocalTime(), build.Status);
        }
    }

    [TestFixture]
    public class One_failed_build_exists : Shared
    {
        [Test]
        public void Assert_right_build_is_returned()
        {
            Given(repository_has_been_created).And(there_is_one_failed_build);
            When(asking_for_all_builds);
            Then("assert right build is returned", () =>
            {
                Assert.AreEqual(1, allBuilds.Count());
                Assert.IsTrue(TestBuilds.Equals(TestBuilds.Build1, allBuilds.First()),
                              "Expected <{0}>, found <{1}>", 
                              TestBuilds.ToString(TestBuilds.Build1),
                              TestBuilds.ToString(allBuilds.First()));
            });
        }
    }

    [TestFixture]
    public class One_successful_build_exists : Shared
    {
        [Test]
        public void Assert_right_build_is_returned()
        {
            Given(repository_has_been_created).And(there_is_one_successful_build);
            When(asking_for_all_builds);
            Then("assert right build is returned", () =>
            {
                Assert.AreEqual(1, allBuilds.Count());
                Assert.IsTrue(TestBuilds.Equals(TestBuilds.Build2, allBuilds.First()),
                              "Expected [{0}], found [{1}]",
                              TestBuilds.ToString(TestBuilds.Build1),
                              TestBuilds.ToString(allBuilds.First()));
            });
        }
    }
}