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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

using Smeedee.DomainModel.SourceControl;
using Smeedee.Integration.VCS.TFSVC.DomainModel.Repositories;

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

using Changeset=Smeedee.DomainModel.SourceControl.Changeset;
using MSChangeset = Microsoft.TeamFoundation.VersionControl.Client.Changeset;


namespace Smeedee.IntegrationTests.VCS.TFSVC.DomainModel.Repositories
{
    public class Shared
    {
        public static NetworkCredential credentials = new NetworkCredential("smeedee", "dlog4321.");
        public static string repositoryUrl = "http://80.203.160.221:8080/tfs";
        public static string repositoryPath = "$/Smeedee";

        protected IEnumerable<MSChangeset> QueryAllChangesets()
        {
            return QueryAllChangesetsFromRevision(1);
        }

        protected IEnumerable<MSChangeset> QueryAllChangesetsFromRevision(int revisionId)
        {
            var tfsClient = new TeamFoundationServer(repositoryUrl, credentials);
            tfsClient.Authenticate();

            var vcs = (VersionControlServer) tfsClient.GetService(typeof (VersionControlServer));

            var version = VersionSpec.Latest;
            var versionTo = VersionSpec.Latest;
            var versionFrom = new ChangesetVersionSpec(revisionId);

            const int deletionId = 0;
            const string user = null;
            IEnumerable changesets = vcs.QueryHistory(repositoryPath, version, deletionId, RecursionType.Full, user,
                                                      versionFrom, versionTo, int.MaxValue, true, false);

            var retvalue = new List<MSChangeset>();
            foreach (object item in changesets)
                retvalue.Add(item as MSChangeset);

            return retvalue;
        }
    }

    [TestFixture][Category("IntegrationTest")]
    [Ignore("TFS Server is down")]
    public class When_query_all_changesets : Shared
    {
        private static TFSChangesetRepository repository;

        private static IEnumerable<Changeset> resultset;

        private readonly Context there_are_changesets_in_the_repository = () => { };

        private readonly When query_all_changesets =
            () => { resultset = repository.Get(new AllChangesetsSpecification()); };

        private readonly Context repository_is_created =
            () => { repository = new TFSChangesetRepository(repositoryUrl, repositoryPath, credentials); };


        [Test]
        public void Assure_all_changesets_are_recieved()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_the_repository).
                    And(repository_is_created);

                scenario.When(query_all_changesets);
                

                scenario.Then("assure all changesets are recieved", () =>
                {
                    IEnumerable<MSChangeset> tfsResultset = QueryAllChangesets();
                    resultset.Count().ShouldBe(tfsResultset.Count());
                });
            });
        }


        [Test]
        public void Assure_Author_is_loaded_into_the_resultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_the_repository).
                    And(repository_is_created);

                scenario.When(query_all_changesets);

                scenario.Then("assure the Author is loaded into the resultset", () =>
                {
                    foreach (Changeset changeset in resultset)
                        changeset.Author.ShouldNotBeNull();
                });
            });
        }

        [Test]
        public void Assure_LogMessage_is_loaded_into_the_resultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_the_repository).
                    And(repository_is_created);

                scenario.When(query_all_changesets);

                scenario.Then("assure the LogMessage is loaded into the resultset", () =>
                {
                    foreach (Changeset changeset in resultset)
                        changeset.Comment.ShouldNotBeNull();
                });
            });
        }

        [Test]
        public void Assure_Revision_is_loaded_into_the_resultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_the_repository).
                    And(repository_is_created);

                scenario.When(query_all_changesets);

                scenario.Then("assure the Revision is loaded into the resultset", () =>
                {
                    foreach (Changeset changeset in resultset)
                        changeset.Revision.ShouldNotBe(0);
                });
            });
        }

        [Test]
        public void Assure_Time_is_loaded_into_the_resultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_the_repository).
                    And(repository_is_created);

                scenario.When(query_all_changesets);

                scenario.Then("assure the timestamp is loaded into the resultset", () =>
                {
                    foreach (Changeset changeset in resultset)
                        changeset.Time.ShouldNotBe(DateTime.MinValue);
                });
            });
        }
    }
    
    [TestFixture][Category("IntegrationTest")]
    [Ignore("TFS Server is down")]
    public class When_query_all_changesets_after_a_given_revision : Shared
    {
        [Test]
        public void Assert_changesets_are_recieved()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("there are changesets in the repository");

                scenario.When("all changesets after revision 4 are requested");

                scenario.Then("assure all chagesets after revision 4 are recived", () =>
                {
                    IEnumerable<MSChangeset> resultsets = QueryAllChangesetsFromRevision(4);
                    foreach (MSChangeset resultset in resultsets)
                        Assert.Greater(resultset.ChangesetId, 3);
                });
            });
        }
    }

    [TestFixture][Category("IntegrationTest")]
    [Ignore("TFS Server is down")]
    public class When_query_all_changesets_from_user : Shared
    {
        private static string username = "Administrator";

        [Test]
        public void Assert_changesets_can_be_filtered_on_user()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("the user " + username + " has committed changesets to the repository");

                scenario.When("these changesets are retrieved");

                scenario.Then("at least one changeset is returned", () =>
                {
                    var repository = new TFSChangesetRepository(repositoryUrl, repositoryPath, credentials);
                    IEnumerable<Changeset> resultsets =
                        repository.Get(new ChangesetsForUserSpecification(username));
                    resultsets.Count().ShouldNotBe(0);
                });
            });
        }

        [Test]
        public void Assert_no_changesets_from_other_autors_are_returned()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("the user " + username + " has committed changesets to the repository");

                scenario.When("these changesets are retrieved");

                scenario.Then("assure all changesets returned have this user as its author", () =>
                {
                    var repository = new TFSChangesetRepository(repositoryUrl, repositoryPath, credentials);
                    IEnumerable<Changeset> resultsets =
                        repository.Get(new ChangesetsForUserSpecification(username));

                    resultsets.Count().ShouldNotBeNull();

                    foreach (Changeset resultset in resultsets)
                        Assert.AreEqual(resultset.Author.Username, username);
                });
            });
        }
    }
}