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
using System.Net;

using APD.IntegrationTests.CI.TFSBuild.DomainModel.Repositories;

using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

using MSBuildStatus = Microsoft.TeamFoundation.Build.Client.BuildStatus;
using APD.Integration.CI.TFSBuild.DomainModel.Repositories;

namespace APD.IntegrationTests.CI.TFSBuild.DomainModel.Repositories.Integration
{
    public class Shared : ScenarioClass
    {
        protected static String serverAddress = "https://tfs08.codeplex.com";
        protected static String projectName = "smeedee";
        protected static String buildName = "TFS Integration Mock Project";
        protected static NetworkCredential credentials = new NetworkCredential("smeedee_cp", "haldis", "snd");

        protected static BuildFetcher fetcher;

        protected Context build_fetcher_is_instantiated = () => 
            fetcher = new BuildFetcher(serverAddress, projectName, credentials);

        [TearDown]
        public void Teardown()
        {
            StartScenario();
        }
    }

    [TestFixture]
    //[Ignore]
    public class BuildFetcherSpecs : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("BuildFetcher is spanwed");
        }

        [Test]
        public void Assure_connection_can_be_made()
        {
            Given(build_fetcher_is_instantiated);
             
            When("valid credentials are used");
            
            Then("no exceptions should be thrown");
        }


        [Test]
        public void Assure_list_with_build_definitions_in_project_are_returned()
        {
            IEnumerable<IBuildDefinition> buildDefinitions = null;

            Given(build_fetcher_is_instantiated);

            When("a list of the build definitions in the project is requested", () =>
                buildDefinitions = fetcher.GetBuildDefinitions());

            Then("a list of the build definitions in the project should be returned", () =>
                buildDefinitions.ShouldNotBeNull());
        }

        [Test]
        [Ignore("There are no project on Codeplex. Set up local server and update connection info.")]
        public void Assure_list_of_all_build_definitions_in_the_project_has_content()
        {
            IEnumerable<IBuildDefinition> buildDefinitions = null;

            Given(build_fetcher_is_instantiated);

            When("a list of the build definitions in the project is requested", () =>
                buildDefinitions = fetcher.GetBuildDefinitions());

            Then("a list of the build definitions in the project should be returned", () =>
                (buildDefinitions.Count() > 0).ShouldBeTrue());
        }

        [Test]
        [Ignore("There are no project on Codeplex. Set up local server and update connection info.")]
        public void Assure_can_fetch_history_for_a_given_build_definition()
        {
            IEnumerable<IBuildDefinition> buildDefinitions = null;
            IEnumerable<IBuildDetail> buildDetails = null;
            
            Given(build_fetcher_is_instantiated);
        
            When("a list of the build definitions are retrieved, and the history for it's first item i requested", () =>
            {
                buildDefinitions = fetcher.GetBuildDefinitions();
                buildDetails = fetcher.GetBuildHistory(buildDefinitions.First());
            });

            Then("a list of history items for that build definition should be returned", () =>
            {
                buildDetails.ShouldNotBeNull();
                (buildDetails.Count() > 0).ShouldBeTrue();
            });
        }

        [Test]
        [Ignore("There are no project on Codeplex. Set up local server and update connection info.")]
        public void Assure_no_future_builds_will_be_returned()
        {
            var tfsServer = new TeamFoundationServer(serverAddress, credentials);
            tfsServer.Authenticate();

            IEnumerable<IBuildDetail> buildDetails = null;
            var buildServer = (IBuildServer)tfsServer.GetService(typeof(IBuildServer));

            Given(build_fetcher_is_instantiated).
                And("we have a connection to the build server");

            When("The build history for a specification is requested, when there are builds not yet started.", () =>
            {
                buildServer.QueueBuild(fetcher.GetBuildDefinitions().First());
                buildDetails = fetcher.GetBuildHistory(fetcher.GetBuildDefinitions().First());
            });
            
            Then("no queued builds should be returned.", () =>
            {
                buildDetails.ShouldNotBeNull();
                buildDetails.First().Status.ShouldNotBe(MSBuildStatus.NotStarted);
            });
        }
    }
}
