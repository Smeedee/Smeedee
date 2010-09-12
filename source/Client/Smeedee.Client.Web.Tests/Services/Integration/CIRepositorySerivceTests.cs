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
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Smeedee.Client.Web.Tests.CIRepositoryService;
using NUnit.Framework;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using TinyBDD.Specification.NUnit;
using TinyBDD.Dsl.GivenWhenThen;
using System.Runtime.Serialization;

namespace Smeedee.Client.WebTests.Services.Integration.CIRepositoryServiceTests
{
    public class CIRepositoryServiceTestsShared : Shared
    {
        protected static CIRepositoryServiceClient webServiceClient;

        protected Context WebServiceClient_is_created = () =>
        {
            webServiceClient = new CIRepositoryServiceClient();
        };

        protected Context Database_contains_CIData = () =>
        {
            var ciServer = new CIServer("CC.NET", "http://agileprojectdashboard.org");
            var project = new CIProject("Smeedee - trunk");
            var startTime = DateTime.Now.Date.AddHours(12);
            project.SystemId = "Smeedee - trunk";
            project.AddBuild(new Build()
            {
                SystemId = "1",
                StartTime = startTime,
                FinishedTime = startTime.AddMinutes(5),
                Status = BuildStatus.FinishedSuccefully,
                Trigger = new UnknownTrigger()
            });
            ciServer.AddProject(project);

            databaseSession.Save(ciServer);
            databaseSession.Flush();
        };
    }

    [TestFixture][Category("IntegrationTest")][Category("IntegrationTest")]
    public class When_get_Continuous_Integration_data_via_WebService : CIRepositoryServiceTestsShared
    {
        [Test]
        public void Assure_all_data_is_successfully_serialized()
        {
            Scenario.StartNew(this, scenario =>
            {
                IEnumerable<CIServer> resultsetWS = null;
                IEnumerable<CIServer> resultsetDB = null;

                scenario.Given(Database_is_created).
                    And(Database_contains_CIData).
                    And(WebServiceClient_is_created).
                    And("CI data is fetched from database", () =>
                        resultsetDB = databaseSession.CreateCriteria(typeof(CIServer)).List<CIServer>());

                scenario.When("get all", () =>
                    resultsetWS = webServiceClient.Get(new AllSpecification<CIServer>()));

                scenario.Then("assure data is successfully serialized", () =>
                {
                    AssertCIServers(resultsetWS, resultsetDB);

                    foreach (var ciServerDB in resultsetDB)
                    {
                        var ciServerWS = resultsetWS.Where(r => r.Name == ciServerDB.Name).SingleOrDefault();

                        AssertCIServer(ciServerDB, ciServerWS);

                        foreach (var ciProjectDB in ciServerDB.Projects)
                        {
                            var ciProjectWS =
                                ciServerWS.Projects.Where(p => p.SystemId == ciProjectDB.SystemId).
                                    SingleOrDefault();

                            AssertCIProject(ciProjectDB, ciProjectWS);

                            foreach (var ciBuildDB in ciProjectDB.Builds)
                            {
                                var ciBuildWS =
                                    ciProjectWS.Builds.Where(p => p.SystemId == ciBuildDB.SystemId).
                                        SingleOrDefault();

                                AssertCIBuild(ciBuildDB, ciBuildWS);
                            }
                        }
                    }
                });
            });
        }

        private void AssertCIServers(IEnumerable<CIServer> actual, IEnumerable<CIServer> expected)
        {
            actual.ShouldNotBeNull();
            expected.ShouldNotBeNull();

            actual.Count().ShouldBe(expected.Count());
        }

        private void AssertCIBuild(Build expected, Build actual)
        {
            actual.ShouldNotBeNull();
            actual.Project.ShouldNotBeNull();
            actual.Project.ShouldBe(expected.Project);
            actual.Status.ShouldBe(expected.Status);
            actual.StartTime.ToString(DATE_FORMAT).
                ShouldBe(expected.StartTime.ToString(DATE_FORMAT));
            actual.FinishedTime.ToString(DATE_FORMAT).
                ShouldBe(expected.FinishedTime.ToString(DATE_FORMAT));
            actual.Trigger.ShouldNotBeNull();
            actual.Trigger.ShouldBe(expected.Trigger);
            actual.Duration.ShouldBe(expected.Duration);
        }

        private void AssertCIProject(CIProject expected, CIProject actual)
        {
            actual.ShouldNotBeNull();
            actual.ProjectName.ShouldBe(expected.ProjectName);
            actual.Server.ShouldNotBeNull();
            actual.Server.Name.ShouldBe(expected.Server.Name);
            actual.Builds.ShouldNotBeNull();
            actual.Builds.Count().ShouldBe(expected.Builds.Count());
            actual.LatestBuild.ShouldNotBeNull();
            actual.LatestBuild.SystemId.ShouldBe(expected.LatestBuild.SystemId);
        }

        private static void AssertCIServer(CIServer expected, CIServer actual)
        {
            actual.ShouldNotBeNull();
            actual.Url.ShouldBe(expected.Url);
            actual.Projects.ShouldNotBeNull();
            actual.Projects.Count().ShouldBe(expected.Projects.Count());
        }
    }
}
