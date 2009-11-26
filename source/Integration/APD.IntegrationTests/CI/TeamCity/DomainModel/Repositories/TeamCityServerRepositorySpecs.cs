using System;
using System.Collections.Generic;
using System.Linq;

using APD.DomainModel.CI;
using APD.DomainModel.Framework;
using APD.Integration.CI.TeamCity.DomainModel.Repositories;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;


namespace APD.IntegrationTests.CI.TeamCity.Tests.DomainModel.Repositories.TeamCityServerRepositorySpecs
{
    public class Shared : ScenarioClass
    {
        private const string ADDRESS_NO_PROJECTS = @"CI\TeamCity\TestData\NoProjects.xml";

        private const string ADDRESS_TWO_PROJECTS_MANY_BUILDS =
            @"CI\TeamCity\TestData\TwoProjectsManyBuilds.xml";

        private const string ADDRESS_TWO_PROJECTS_TWO_BUILDS =
            @"CI\TeamCity\TestData\TwoProjectsTwoBuilds.xml";

        protected static TeamCityServerRepository repository;
        protected static IEnumerable<CIServer> servers;

        #region Givens

        protected readonly Context repository_has_been_created =
            () => repository = new TeamCityServerRepository(null);

        protected readonly Context there_are_multiple_projects_with_many_builds =
            () => repository.Address = new Uri(ADDRESS_TWO_PROJECTS_MANY_BUILDS, UriKind.Relative);

        protected readonly Context there_are_multiple_projects_with_single_builds =
            () => repository.Address = new Uri(ADDRESS_TWO_PROJECTS_TWO_BUILDS, UriKind.Relative);

        protected readonly Context there_are_no_projects =
            () => repository.Address = new Uri(ADDRESS_NO_PROJECTS, UriKind.Relative);

        #endregion

        #region Whens

        protected readonly When asking_for_all_builds =
            () => servers = repository.Get(new AllSpecification<CIServer>());

        #endregion

        #region Thens

        #endregion

        [TearDown]
        public void RunTests()
        {
            StartScenario();
        }

        #region Nested type: TestObjects

        protected static class TestObjects
        {
            public static CIProject Project1_DP = new CIProject
            {
                ProjectName = "Smeedee::DummyProject",
                Builds = new List<Build>
                {
                    new Build {StartTime = new DateTime(2009, 09, 29, 13, 36, 21)}
                },
                SystemId = "Smeedee::DummyProject"
            };

            public static CIProject Project2_CB = new CIProject
            {
                ProjectName = "Smeedee::Continuous Build",
                Builds = new List<Build>
                {
                    new Build {StartTime = new DateTime(2009, 09, 29, 13, 26, 00).ToLocalTime()}
                },
                SystemId = "Smeedee::Continuous Build"
            };

            public static Build LatestBuild_CB = new Build
            {
                Project = Project2_CB,
                StartTime = new DateTime(2009, 9, 30, 8, 57, 20).ToLocalTime(),
                FinishedTime = new DateTime(2009, 9, 30, 8, 57, 20).ToLocalTime(),
                SystemId = new DateTime(2009, 09, 29, 13, 26, 00).ToLocalTime().Ticks.ToString()
            };

            public static Build LatestBuild_DP = new Build
            {
                Project = Project1_DP,
                StartTime = new DateTime(2009, 9, 29, 13, 36, 21).ToLocalTime(),
                FinishedTime = new DateTime(2009, 9, 29, 13, 36, 21).ToLocalTime(),
                SystemId = new DateTime(2009, 09, 29, 13, 36, 21).ToLocalTime().Ticks.ToString()
            };
        }

        #endregion

        protected static string GetAssertionFailMessage(Build expectedDp, Build actualDp)
        {
            return
                string.Format(
                    "Expected: <Project: {0}, StartTime: {1}, SystemId: {2}>, but was: <Project: {3}, StartTime: {4}, SystemId: {5}>",
                    expectedDp.Project.ProjectName, expectedDp.StartTime, expectedDp.SystemId, 
                    actualDp.Project.ProjectName, actualDp.StartTime, actualDp.SystemId);
        }
    }

    [TestFixture]
    public class No_projects_exist : Shared
    {
        [Test]
        public void Assert_get_all_returns_empty_server()
        {
            Given(repository_has_been_created).And(there_are_no_projects);
            When(asking_for_all_builds);
            Then("assert an empty CI server is returned", () =>
            {
                Assert.AreEqual(1, servers.Count());
                Assert.AreEqual(0, servers.First().Projects.Count());
            });
        }
    }

    [TestFixture]
    public class Multiple_projects_with_single_builds_exist : Shared
    {
        [Test]
        public void Assert_get_all_returns_all_projects()
        {
            Given(repository_has_been_created).And(there_are_multiple_projects_with_single_builds);
            When(asking_for_all_builds);
            Then("assert all projects are returned",
                 () => { Assert.AreEqual(2, servers.First().Projects.Count()); });
        }

        [Test]
        public void Assert_get_all_returns_newest_builds()
        {
            Given(repository_has_been_created).And(there_are_multiple_projects_with_single_builds);
            When(asking_for_all_builds);
            Then("assert the newest builds for each project are returned", () =>
            {
                IEnumerable<CIProject> projects = servers.First().Projects;

                Build dpExpectedBuild = TestObjects.LatestBuild_DP;
                Build dpActualBuild = projects
                    .Single(project => project.ProjectName == TestObjects.Project1_DP.ProjectName).LatestBuild;
                
                Assert.AreEqual(dpExpectedBuild, dpActualBuild,
                                GetAssertionFailMessage(dpExpectedBuild, dpActualBuild));

                Build cbExpectedBuild = TestObjects.LatestBuild_CB;
                Build cbActualBuild = projects
                    .Single(project => project.ProjectName == TestObjects.Project2_CB.ProjectName).LatestBuild;

                Assert.AreEqual(cbExpectedBuild, cbActualBuild, GetAssertionFailMessage(cbExpectedBuild, cbActualBuild));
            });
        }

        [Test]
        public void Assert_get_all_returns_one_server()
        {
            Given(repository_has_been_created).And(there_are_multiple_projects_with_single_builds);
            When(asking_for_all_builds);
            Then("assert all projects are returned", () =>
            {
                Assert.AreEqual(1, servers.Count());
            });
        }
    }
}