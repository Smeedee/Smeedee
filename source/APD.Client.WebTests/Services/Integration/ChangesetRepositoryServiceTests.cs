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
// The project webpage is located at http://agileprojectdashboard.org/
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
using NUnit.Framework;
using APD.Client.WebTests.ChangesetRepositoryService;
using APD.DomainModel.SourceControl;
using TinyBDD.Specification.NUnit;
using TinyBDD.Dsl.GivenWhenThen;

using AllChangesetsSpecification = APD.DomainModel.SourceControl.AllChangesetsSpecification;
using Author = APD.DomainModel.SourceControl.Author;
using Changeset = APD.DomainModel.SourceControl.Changeset;
using ChangesetsAfterRevisionSpecification = APD.DomainModel.SourceControl.ChangesetsAfterRevisionSpecification;
using ChangesetsForUserSpecification = APD.DomainModel.SourceControl.ChangesetsForUserSpecification;


namespace APD.Client.WebTests.Services.Integration.ChangesetRepositoryServiceTests
{
    public class ChangesetRepositoryServiceTestsShared : Shared
    {
        protected static ChangesetRepositoryServiceClient webServiceClient;

        protected Context WebServiceClient_is_created = () =>
        {
            webServiceClient = new ChangesetRepositoryServiceClient();
        };

        protected Context Database_contains_changesets = () =>
        {
            var changeset = new Changeset()
            {
                Author = new Author("goeran"),
                Comment = "Yet another post",
                Revision = 1,
                Time = DateTime.Now
            };

            var changeset2 = new Changeset()
            {
                Author = new Author("jonas"),
                Comment = "Added tests",
                Revision = 2,
                Time = DateTime.Now.AddDays(1)
            };

            databaseSession.Save(changeset);
            databaseSession.Save(changeset2);
            databaseSession.Flush();
        };
    }

    [TestFixture]
    public class When_get_changesets_data_via_WebService : ChangesetRepositoryServiceTestsShared
    {
        [Test]
        public void Assure_all_data_is_successfully_serialized()
        {
            Scenario.StartNew(this, scenario =>
            {
                IEnumerable<APD.DomainModel.SourceControl.Changeset> changesetsFromWS = null;

                scenario.Given(Database_is_created).
                    And(Database_contains_changesets).
                    And(WebServiceClient_is_created);

                scenario.When("get all", () =>
                    changesetsFromWS = webServiceClient.Get(new AllChangesetsSpecification()));

                scenario.Then("assure all data is successfully serialized", () =>
                {
                    var changesetsFromDB = databaseSession.CreateCriteria(typeof (Changeset)).
                        List<Changeset>();

                    AssertResultsets(changesetsFromWS, changesetsFromDB);
                });
            });
        }

        private void AssertResultsets(IEnumerable<APD.DomainModel.SourceControl.Changeset> changesetsFromWS, IList<APD.DomainModel.SourceControl.Changeset> changesetsFromDB) 
        {
            changesetsFromWS.ShouldNotBeNull();

            changesetsFromWS.Count().ShouldBe(changesetsFromDB.Count);

            foreach (var changesetDB in changesetsFromDB)
            {
                var q = changesetsFromWS.Where(c => c.Revision == changesetDB.Revision);
                q.Count().ShouldNotBe(0);
                var changesetWS = q.Single();
                changesetWS.Comment.ShouldBe(changesetDB.Comment);
                changesetWS.Time.ToString(DATE_FORMAT).
                    ShouldBe(changesetDB.Time.ToString(DATE_FORMAT));
                changesetWS.Author.ShouldNotBeNull();
                changesetWS.Author.Username.ShouldBe(changesetDB.Author.Username);
            }
        }

        [Test]
        public void Assure_its_possible_to_get_changesets_using_specifications()
        {
            Scenario.StartNew(this, scenario =>
            {
                IEnumerable<APD.DomainModel.SourceControl.Changeset> changesetsFromWS = null;
                IEnumerable<APD.DomainModel.SourceControl.Changeset> changesetsFromDB = null;

                scenario.Given(Database_is_created).
                    And(Database_contains_changesets).
                    And(WebServiceClient_is_created).
                    And("changesets is fetched from database", () =>
                        changesetsFromDB = databaseSession.CreateCriteria(typeof(APD.DomainModel.SourceControl.Changeset)).List<APD.DomainModel.SourceControl.Changeset>());

                scenario.When("get all changesets for a given user", () =>
                    changesetsFromWS = webServiceClient.Get(new ChangesetsForUserSpecification("goeran")));
                scenario.Then("assure only changesets for the specified user is received", () =>
                    AssertResultsets(changesetsFromWS, 
                        changesetsFromDB.Where(c => c.Author.Username == "goeran").ToList()));

                scenario.When("get all changesets after a given revision", () =>
                    changesetsFromWS = webServiceClient.Get(new ChangesetsAfterRevisionSpecification(1)));
                scenario.Then("assure only changesets after the specified revision is received", () =>
                    AssertResultsets(changesetsFromWS,
                        changesetsFromDB.Where(c => c.Revision > 1).ToList()));
            });
        }
    }
}
