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
using System.Collections.ObjectModel;
using APD.DomainModel.Framework;
using APD.DomainModel.SourceControl;
using APD.Integration.VCS.Git.DomainModel.Repositories;
using APD.IntegrationTests.VCS.Git.Context;
using GitSharp;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using System.Linq;
namespace APD.IntegrationTests.VCS.Git.DomainModel
{
    [TestFixture]
    public class WhenQueryAllChangesets : GitPluginTestContext
    {
        protected const long MAX_NUMBER_OF_REVISIONS_TO_RECEIVE = 50;
        protected static Collection<Commit> changesetsLog;
        protected static GitChangesetRepository changesetRepository;
        protected static IEnumerable<Changeset> resultset;
        protected static Commit head;
        protected When changesetsAreRequested;
        protected static long minChangesetCache;

        #region Setup/Teardown
        [SetUp]
        public void SetupContext()
        {
            SetupSharedContext();
            changesetsAreRequested = GetAllChangesets;
            changesetRepository = new GitChangesetRepository(pathToLocalRepoClone);
        }
        #endregion

        protected void GetAllChangesets()
        {
            resultset = changesetRepository.Get(new ChangesetsAfterRevisionSpecification(MinChangeset()));
        }

        protected Collection<Commit> QueryChangesetsLog()
        {
            head = repo.Get<Commit>("HEAD");
            if (changesetsLog == null)
            {
                var log = new Collection<Commit> {head};

                foreach (Commit c in head.Ancestors)
                {
                    log.Add(c);
                }

                changesetsLog = log;
            }
            return changesetsLog;
        }

        protected long MinChangeset()
        {
            return 0;
        }
   
        [Test]
        [Ignore] // Integration specific
        public void AssureAllChangesetsAreReceived()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("The repository exists and contains changesets", TheRepositoryExistsAndContainsChangesets);
                scenario.When("changesets are requested", GetAllChangesets);
                scenario.Then("assure all changesets are received", () => resultset.Count().ShouldBe(QueryChangesetsLog().Count()));
            });
        }

        [Test]
        [Ignore] // Integration specific
        public void AssureAuthorIsLoadedIntoTheResultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("The repository contains changesets");
                scenario.When(changesetsAreRequested);
                scenario.Then("assure Author is loaded into all rows in the _resultset", () =>
                {
                    IEnumerable<Changeset> q =
                        resultset.Where(c => c.Author != null && c.Author.Username != null);
                    q.Count().ShouldBe(resultset.Count());
                });
            });
        }

        [Test]
        [Ignore] // Integration specific
        public void AssureLogMessageIsLoadedIntoTheResultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("The repository contains changesets");
                scenario.When(changesetsAreRequested);
                scenario.Then("assure Comment is loaded into all rows of the result set", () =>
                {
                    IEnumerable<Changeset> q = resultset.Where(c => c.Comment != null);
                    q.Count().ShouldBe(resultset.Count());
                });
            });
        }

        [Test]
        [Ignore] // Integration specific
        public void AssureTimeIsLoadedIntoTheResultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("The repository contains changesets");
                scenario.When(changesetsAreRequested);
                scenario.Then("assure Time is loaded into all rows in _resultset", () =>
                {
                    foreach (Changeset changeset in resultset)
                        changeset.Time.ShouldNotBe(DateTime.MinValue);
                });
            });
        }
 
        [Test]
        [Ignore] // Integration specific
        public void AssureChangesetsAreReceived()
        {
            var users = "unknown|Martin Altin|Christian Jonassen".Split(new[] { '|' }).ToList();
            long minChangest = MinChangeset();

            users.ForEach(user =>
            {
                Scenario.StartNew(this, scenario =>
                {
                    scenario.Given("The repository contains changesets");
                    scenario.When("all changesets for '" + user + "' are requested", () =>
                                                                                     resultset = changesetRepository.Get(new AndExpressionSpecification<Changeset>(
                                                                                                                    new ChangesetsForUserSpecification(user),
                                                                                                                    new ChangesetsAfterRevisionSpecification(minChangest))));
                    scenario.Then("assure all changesets for '" + user + "' are received", () =>
                    {
                        changesetsLog = QueryChangesetsLog();
                        IEnumerable<Commit> usersChangetsLog =
                            changesetsLog.Where(l => l.Author.Name == user);
                        resultset.Count().ShouldBe(usersChangetsLog.Count());
                    });
                });
            });
        }

    }
}
