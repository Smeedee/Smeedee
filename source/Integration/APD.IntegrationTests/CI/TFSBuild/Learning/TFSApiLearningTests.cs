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
using System.Threading;

using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

using NUnit.Framework;

using TinyBDD.Specification.NUnit;


namespace APD.IntegrationTests.CI.TFSBuild.Learning
{
    [TestFixture]
    [Ignore("Un-ignore after getting a local TFS Server with license")]
    public class TFSApiLearningTests
    {
        private TeamFoundationServer tfsServer;
        private String projectName = "ADPMockProject";
        private String buildName = "TFS Integration Mock Project";
        private NetworkCredential credentials = new NetworkCredential("dagolap", "gold1234.");
        private IBuildServer buildServer;


        [SetUp]
        public void SetUp()
        {
            tfsServer = new TeamFoundationServer("http://80.203.160.221:8080", credentials);
            tfsServer.Authenticate();
            buildServer = (IBuildServer) tfsServer.GetService(typeof (IBuildServer));
        }

        private IBuildDefinition GetBuildDefinition()
        {
            var buildDefinitions = buildServer.QueryBuildDefinitions(projectName);
            return buildDefinitions.First();
        }

        [Test]
        public void How_to_get_a_list_of_build_definitions_from_a_tfs_project()
        {
            IBuildDefinition[] builds = buildServer.QueryBuildDefinitions(projectName);
            ( builds.Count() > 0 ).ShouldBeTrue();
        }

        [Test]
        public void How_to_get_history_for_a_build_definition()
        {
            var buildDefinition = GetBuildDefinition();

            var buildTime = DateTime.Now;
            buildServer.QueueBuild(buildDefinition);
            buildServer.QueueBuild(buildDefinition);

            while (
                buildServer.QueryBuilds(buildDefinition).Where(
                    b => b.Status == BuildStatus.InProgress || b.Status == BuildStatus.NotStarted).Count() > 0)
                Thread.Sleep(50);

            var buildHistory = buildServer.QueryBuilds(buildDefinition).Reverse();

            ( buildHistory.Count() >= 2 ).ShouldBeTrue();
            buildHistory.First().RequestedBy.ShouldBe("TFSX86\\dagolap");
            buildHistory.First().StartTime.Hour.ShouldBe(buildTime.Hour);
        }

        [Test]
        public void How_to_get_all_projects()
        {
            VersionControlServer versionControlServer = tfsServer.GetService(typeof(VersionControlServer)) as VersionControlServer;
            versionControlServer.GetAllTeamProjects(false);
        }

        [Test]
        [Ignore]
        public void Can_we_get_information_about_ongoing_builds()
        {
            var buildDefinition = GetBuildDefinition();

            buildServer.QueueBuild(buildDefinition);

            var buildDetailsSampling = new List<IBuildDetail>();
            IBuildDetail latestBuild;
            while (true)
            {
                latestBuild = buildServer.QueryBuilds(buildDefinition).Reverse().First();
                buildDetailsSampling.Add(latestBuild);
                if (latestBuild.Status != BuildStatus.NotStarted &&
                    latestBuild.Status != BuildStatus.InProgress)
                {
                    break;
                }
            }

            buildDetailsSampling.Count.ShouldNotBe(0);
            Console.WriteLine(buildDetailsSampling.Count);
            buildDetailsSampling.Where(s => s.Status == BuildStatus.InProgress).Count().ShouldNotBe(0);
        }
    }
}