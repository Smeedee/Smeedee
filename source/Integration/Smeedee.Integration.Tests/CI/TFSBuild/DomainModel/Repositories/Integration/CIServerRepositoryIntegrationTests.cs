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

using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using Smeedee.Integration.CI.TFSBuild.DomainModel.Repositories;

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;

using System.Net;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

using MSBuildStatus = Microsoft.TeamFoundation.Build.Client.BuildStatus;
using BuildStatus = Smeedee.DomainModel.CI.BuildStatus;


namespace Smeedee.IntegrationTests.CI.TFSBuild.DomainModel.Repositories.Integration.CIServerRepositoryIntegration
{
    public class Shared : ScenarioClass
    {
        protected static String SERVER_ADDRESS = "http://80.203.160.221:8080/tfs";
        protected static String PROJECT_NAME = "smeedee";
        protected static ICredentials credentials = new NetworkCredential("smeedee", "dlog4321.");

        protected static TFSCIServerRepository repository;

        protected readonly Context repository_is_instantiated =
            () => { repository = new TFSCIServerRepository(SERVER_ADDRESS, PROJECT_NAME, credentials); };

        public void ExpectException<TException> (Action fn) where TException : Exception
        {
            bool hasThrownException = false;
            try
            {
                fn.Invoke();
            }
            catch (TException e)
            {
                hasThrownException = true;
            } finally
            {
                if (!hasThrownException) Assert.Fail("Did not throw expected exception");
            }
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }

    [TestFixture][Category("IntegrationTest")]
    [Ignore("TFS Server is down")]
    public class CIProjectRepositoryIntegrationTests : Shared
    {
        [SetUp]
        public void SetUp()
        {
            Scenario("CIProjectRepository is spawned");
        }

        [Test]
        public void Assure_projects_are_returned()
        {
            IEnumerable<CIProject> projects = null;

            Given(repository_is_instantiated);

            When("all project with history and current builds are requested", () =>
                projects = repository.Get(new AllSpecification<CIServer>()).First().Projects);
            
            Then("a list of all projects should be returned", () =>
            {
                projects.ShouldNotBeNull();
                //(projects.Count() > 0).ShouldBeTrue();
            });
        }

        [Test]
        public void Assure_latest_build_exists()
        {
            IEnumerable<CIProject> projects = null;
            
            Given(repository_is_instantiated);
            
            When("a list of all projects are requested.", () =>
                projects = repository.Get(new AllSpecification<CIServer>()).First().Projects);
            
            Then("all projects should have a latest build defined", () =>
            {
                foreach (var project in projects)
                {
                    project.LatestBuild.ShouldNotBeNull();
                }
            });
        }

        [Test]
        public void Assure_when_finished_build_should_have_valid_duration()
        {
            IEnumerable<CIProject> projects = null;
                
            Given(repository_is_instantiated);
            
            When("a list of all projects are requested", () =>
                    projects = repository.Get(new AllSpecification<CIServer>()).First().Projects);

            Then("all projects that are returned and finished should have a valid duration", () =>
            {
                foreach (var project in projects)
                {
                    if (project.LatestBuild.Status == BuildStatus.FinishedSuccefully ||
                        project.LatestBuild.Status == BuildStatus.FinishedWithFailure)
                    {
                        (project.LatestBuild.Duration > new TimeSpan(0)).ShouldBeTrue();
                    }
                }
            });
        }

        [Test]
        public void Assure_wrong_url_throws_exception()
        {
            TFSCIServerRepository repository;

            Given("a repository is instantiated with a wrong server address");
            When("it is instantiated");
            Then("an exception should be thrown", () =>
            {
                ExpectException<Exception>(() =>
                    repository = new TFSCIServerRepository("idonotexist48123", PROJECT_NAME, credentials)
                );
            });
        }

        [Test]
        public void Assure_wrong_credentials_throws_exception()
        {
            TFSCIServerRepository repository;
            
            Given("a repository is instantiated with wrong credentials");
            
            When("it is instantiated");

            Then("and exception should be thrown", () =>
            {
                ExpectException<TeamFoundationServerUnauthorizedException>(() =>
                    repository = new TFSCIServerRepository(SERVER_ADDRESS, PROJECT_NAME, new NetworkCredential("foo", "bar"))
                );
            });
        }

        [Test]
        [Ignore("This works locally, but not on our specific build server. Should be looked into.")]
        public void Assure_the_ammount_of_returned_projects_corresponds_with_server()
        {
            TeamFoundationServer tfsServer;
            IBuildServer buildServer = null;

            Given(repository_is_instantiated).
                And("we have a connection to the TFS server", () =>
                {
                    tfsServer = new TeamFoundationServer(SERVER_ADDRESS, credentials);
                    tfsServer.Authenticate();

                    buildServer = (IBuildServer)tfsServer.GetService(typeof(IBuildServer));
                });
            
            When("we request a list of all projects");
            
            Then("it should be the same length as the amount of build specifications on the server", () =>
            {
                buildServer.ShouldNotBeNull();
                buildServer.QueryBuildDefinitions(PROJECT_NAME).Count().ShouldBe(
                    repository.Get(new AllSpecification<CIServer>()).First().Projects.Count());
            });
        }

        [Test]
        [Ignore("This works locally, but not on our specific build server. Should be looked into.")]
        public void Assure_the_ammount_of_returned_builds_in_a_project_corresponds_with_server()
        {
            TeamFoundationServer tfsServer;
            IBuildServer buildServer = null;

            Given(repository_is_instantiated).
                And("we have a connection to the TFS server", () =>
                {
                    tfsServer = new TeamFoundationServer(SERVER_ADDRESS, credentials);
                    tfsServer.Authenticate();

                    buildServer = (IBuildServer)tfsServer.GetService(typeof(IBuildServer));
                });
            
            When("we request a list of all projects");
            
            Then("it should be the same length as the amount of build specificaions on the server", () =>
            {
                buildServer.ShouldNotBeNull();
                var buildDefinition = buildServer.QueryBuildDefinitions(PROJECT_NAME).First();
                buildServer.QueryBuilds(buildDefinition).Count().ShouldBe(
                    repository.Get(new AllSpecification<CIServer>()).First().Projects.First().Builds.Count());
            });
        }
    }
}
